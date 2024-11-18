using Blogplace.Web.Domain.Articles;
using System.Diagnostics.CodeAnalysis;

namespace Blogplace.Web.Infrastructure.Database;

public interface IArticlesRepository
{
    Task Add(Article article);
    Task Delete(string id);
    Task<Article?> Get(string id);
    Task<IEnumerable<Article>> Search(int limit/*, Guid? tagId = null*/);
    Task Update(Article article);
    //Task<int> CountArticlesWithTag(Guid tag);
    Task<DateTime> GetLastSourceUpdate(Uri source);
}

[ExcludeFromCodeCoverage]
public class ArticlesRepository : IArticlesRepository
{
    private readonly List<Article> _articles = [];

    public Task Add(Article article)
    {
        this._articles.Add(article);
        return Task.CompletedTask;
    }

    public Task<Article?> Get(string id)
    {
        var result = this._articles.SingleOrDefault(x => x.Id == id);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<Article>> Search(int limit/*, Guid? tagId = null*/)
    {
        var results = this._articles/*.Where(x =>
        {
            //if (tagId.HasValue && !x.TagIds.Contains(tagId.Value))
            //{
            //    return false;
            //}

            return true;
        })*/.Take(limit);
        return Task.FromResult(results!);
    }

    //todo table to it
    public Task<DateTime> GetLastSourceUpdate(Uri source)
    {
        var result = this._articles
            .Where(x => x.Source == source)
            .OrderByDescending(x => x.UpdatedAt)
            .FirstOrDefault()?.UpdatedAt ?? DateTime.MinValue;
        return Task.FromResult(result);
    }

    public Task Update(Article article)
    {
        var item = this._articles.Single(x => x.Id == article.Id);
        item.Title = article.Title;
        item.Content = article.Content;
        item.UpdatedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task Delete(string id)
    {
        var item = this._articles.Single(x => x.Id == id);
        this._articles.Remove(item);
        return Task.CompletedTask;
    }

    //public Task<int> CountArticlesWithTag(Guid tag)
    //{
    //    var results = this._articles.Count(x => x.TagIds.Contains(tag));
    //    return Task.FromResult(results!);
    //}
}
