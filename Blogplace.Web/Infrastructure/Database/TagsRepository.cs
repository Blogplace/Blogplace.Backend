//using Blogplace.Web.Domain.Articles;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq;

//namespace Blogplace.Web.Infrastructure.Database;

//public interface ITagsRepository
//{
//    Task Add(Tag tag);
//    Task Delete(Guid id);
//    Task<Tag> Get(Guid id);
//    Task<IEnumerable<Tag>> Get(IEnumerable<Guid> ids);
//    Task<IEnumerable<Tag>> Get(IEnumerable<string> names);
//    public Task AddIfNotExists(IEnumerable<string> names);
//    Task<IEnumerable<KeyValuePair<Tag, int>>> SearchTopTags(int limit, string? containsName);
//}

//[ExcludeFromCodeCoverage]
//public class TagsRepository(IArticlesRepository articlesRepository) : ITagsRepository
//{
//    private readonly List<Tag> _tags = [];
//    private static readonly object obj = new();

//    public Task Add(Tag tag)
//    {
//        this._tags.Add(tag);
//        return Task.CompletedTask;
//    }

//    public Task<Tag> Get(Guid id)
//    {
//        var result = this._tags.Single(x => x.Id == id);
//        return Task.FromResult(result!);
//    }

//    public Task<IEnumerable<Tag>> Get(IEnumerable<Guid> ids)
//    {
//        var result = ids.Select(x => this._tags.First(t => t.Id == x));
//        return Task.FromResult(result!);
//    }

//    public Task<IEnumerable<Tag>> Get(IEnumerable<string> names)
//    {
//        var result = names.Select(x => this._tags.First(t => t.Name == x));
//        return Task.FromResult(result!);
//    }

//    public Task Delete(Guid id)
//    {
//        var item = this._tags.Single(x => x.Id == id);
//        this._tags.Remove(item);
//        return Task.CompletedTask;
//    }

//    public Task AddIfNotExists(IEnumerable<string> names) 
//    {
//        lock (obj)
//        {
//            var toAdd = names
//                .Where(x => !this._tags.Any(t => t.Name == x))
//                .Select(x => new Tag(x));
//            this._tags.AddRange(toAdd);
//        }
//        return Task.CompletedTask;
//    }

//    public async Task<IEnumerable<KeyValuePair<Tag, int>>> SearchTopTags(int limit, string? containsName)
//    {
//        var matchedTags = containsName == null ? this._tags : this._tags.Where(x => x.Name.Contains(containsName));
//        var pairs = new List<KeyValuePair<Tag, int>>();
//        foreach (var tag in matchedTags)
//        {
//            var count = await articlesRepository.CountArticlesWithTag(tag.Id);
//            pairs.Add(new KeyValuePair<Tag, int>(tag, count));
//        }

//        return pairs.OrderByDescending(x => x.Value).Take(limit);
//    }
//}
