using Blogplace.Web.Domain.Articles;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Blogplace.Web.Infrastructure.Database;

public interface ITagsRepository
{
    Task Add(Tag tag);
    Task Delete(Guid id);
    Task<Tag> Get(Guid id);
    Task<IEnumerable<Tag>> Get(IEnumerable<string> names);
    public Task AddIfNotExists(IEnumerable<string> names);
}

[ExcludeFromCodeCoverage]
public class TagsRepository : ITagsRepository
{
    private readonly List<Tag> _tags = [];

    public Task Add(Tag tag)
    {
        this._tags.Add(tag);
        return Task.CompletedTask;
    }

    public Task<Tag> Get(Guid id)
    {
        var result = this._tags.Single(x => x.Id == id);
        return Task.FromResult(result!);
    }

    public Task<IEnumerable<Tag>> Get(IEnumerable<string> names)
    {
        var result = names.Select(x => this._tags.First(t => t.Name == x));
        return Task.FromResult(result!);
    }

    public Task Delete(Guid id)
    {
        var item = this._tags.Single(x => x.Id == id);
        this._tags.Remove(item);
        return Task.CompletedTask;
    }

    public Task AddIfNotExists(IEnumerable<string> names) 
    {
        var toAdd = names
            .Where(x => this._tags.Any(t => t.Name == x))
            .Select(x => new Tag(x));
        this._tags.AddRange(toAdd);
        return Task.CompletedTask;
    }
}
