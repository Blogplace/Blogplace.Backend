using Blogplace.Web.Domain.Articles;
using Blogplace.Web.Infrastructure.Database;

namespace Blogplace.Tests.Integration.Data;

public class TagsRepositoryFake : ITagsRepository
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

    public Task<IEnumerable<Tag>> Get(IEnumerable<Guid> ids)
    {
        var result = this._tags.Where(x => ids.Contains(x.Id));
        return Task.FromResult(result!);
    }

    public Task Delete(Guid id)
    {
        var item = this._tags.Single(x => x.Id == id);
        this._tags.Remove(item);
        return Task.CompletedTask;
    }
}
