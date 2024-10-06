using Blogplace.Web.Domain.Articles;
using Blogplace.Web.Infrastructure.Database;

namespace Blogplace.Tests.Integration.Data;

public class TagsRepositoryFake : ITagsRepository
{
    public static Tag? DefaultTag { get; set; }

    public List<Tag> Tags { get; }

    private static readonly object obj = new();

    public TagsRepositoryFake()
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
            .Where(x => this.Tags.Any(t => t.Name == x))
            .Select(x => new Tag(x));
        this.Tags.AddRange(toAdd);
        return Task.CompletedTask;
    }
}
