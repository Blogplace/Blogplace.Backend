using Blogplace.Web.Domain.Articles;
using Blogplace.Web.Infrastructure.Database;

namespace Blogplace.Tests.Integration.Data;

public class TagsRepositoryFake(IArticlesRepository articlesRepository) : ITagsRepository
{
    public static Tag? DefaultTag { get; set; }

    public List<Tag> Tags { get; private set; } = [];

    private static readonly object obj = new();

    public void Init()
    {
        lock (obj)
        {
            DefaultTag ??= new Tag("default");
        }

        this.Tags =
        [
            DefaultTag
        ];
    }

    public Task Add(Tag tag)
    {
        this.Tags.Add(tag);
        return Task.CompletedTask;
    }

    public Task<Tag> Get(Guid id)
    {
        var result = this.Tags.Single(x => x.Id == id);
        return Task.FromResult(result!);
    }

    public Task<IEnumerable<Tag>> Get(IEnumerable<string> names)
    {
        var result = names.Select(x => this.Tags.First(t => t.Name == x));
        return Task.FromResult(result!);
    }

    public Task Delete(Guid id)
    {
        var item = this.Tags.Single(x => x.Id == id);
        this.Tags.Remove(item);
        return Task.CompletedTask;
    }

    public Task AddIfNotExists(IEnumerable<string> names)
    {
        var toAdd = names
            .Where(x => !this.Tags.Any(t => t.Name == x))
            .Select(x => new Tag(x));
        this.Tags.AddRange(toAdd);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<KeyValuePair<Tag, int>>> SearchTopTags(int limit, string? containsName)
    {
        var matchedTags = containsName == null ? this.Tags : this.Tags.Where(x => x.Name.Contains(containsName));
        var pairs = new List<KeyValuePair<Tag, int>>();
        foreach (var tag in matchedTags)
        {
            var count = await articlesRepository.CountArticlesWithTag(tag.Id);
            pairs.Add(new KeyValuePair<Tag, int>(tag, count));
        }

        return pairs.OrderByDescending(x => x.Value).Take(limit);
    }
}
