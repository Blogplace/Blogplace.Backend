using Blogplace.Web.Infrastructure.Database;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Channels;

namespace Blogplace.Web.Background;

public interface ITagsCleaningChannel
{
    ValueTask Publish(Guid tagDeletedFromArticle);
    IAsyncEnumerable<Guid> Subscribe(CancellationToken ct);
}

public class TagsCleaningChannel : ITagsCleaningChannel
{
    private readonly Channel<Guid> _tagsToCheck = Channel.CreateUnbounded<Guid>();

    public ValueTask Publish(Guid tagDeletedFromArticle) => this._tagsToCheck.Writer.WriteAsync(tagDeletedFromArticle);

    public IAsyncEnumerable<Guid> Subscribe(CancellationToken ct) => this._tagsToCheck.Reader.ReadAllAsync(ct);
}

public class TagsCleaningService(
    ITagsCleaningChannel channel, 
    IArticlesRepository articlesRepository, 
    ITagsRepository tagsRepository,
    IMemoryCache cache) : BackgroundService
{
    private const string TAGS_TO_CHECK = "TagsToCheck";
    private const string LAST_TAGS_CLEANING = "LastTagsCleaning";

    //todo options
    private const int SECONDS_BETWEEN_CLEANING = 15;

    public DateTime LastExecution { get; private set; }

    //todo improve data safety
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var tagToCheck in channel.Subscribe(stoppingToken)) 
        {
            var tagsToCheck = await cache.GetOrCreateAsync(TAGS_TO_CHECK, x => Task.FromResult(new HashSet<Guid>() { tagToCheck }));
            tagsToCheck!.Add(tagToCheck);

            if (await this.TryCleaning(tagsToCheck))
            {
                tagsToCheck.Clear();
            }

            cache.Set(TAGS_TO_CHECK, tagsToCheck);
            this.LastExecution = DateTime.UtcNow;
        }
    }

    private async Task<bool> TryCleaning(HashSet<Guid> tagsToCheck)
    {
        var now = DateTime.UtcNow;
        var lastTime = await cache.GetOrCreateAsync(LAST_TAGS_CLEANING, x => Task.FromResult(default(DateTime)));
        var secondsBetween = (now - lastTime).TotalSeconds;
        if (secondsBetween < SECONDS_BETWEEN_CLEANING)
        {
            return false;
        }

        foreach (var tagToCheck in tagsToCheck)
        {
            var count = await articlesRepository.CountArticlesWithTag(tagToCheck);
            if (count == 0) 
            {
                await tagsRepository.Delete(tagToCheck);
            }
        }

        cache.Set(LAST_TAGS_CLEANING, DateTime.UtcNow);
        return true;
    }
}
