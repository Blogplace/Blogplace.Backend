using Blogplace.Web.Domain.Articles;

namespace Blogplace.Web.Infrastructure.Database;

public interface IArticlesRepository
{
    Task Add(Article article);
    Task Delete(Guid id);
    Task<Article> Get(Guid id);
    Task<IEnumerable<Article>> Search();
    Task Update(Article article);
}

public class ArticlesRepository : IArticlesRepository
{
    private readonly List<Article> _articles = [];

    public Task Add(Article article)
    {
        this._articles.Add(article);
        return Task.CompletedTask;
    }

    public Task<Article> Get(Guid id)
    {
        var result = this._articles.Single(x => x.Id == id);
        return Task.FromResult(result!);
    }

    public Task<IEnumerable<Article>> Search()
    {
        var results = this._articles.AsEnumerable();
        return Task.FromResult(results!);
    }

    public Task Update(Article article)
    {
        var item = this._articles.Single(x => x.Id == article.Id);
        item.Title = article.Title;
        item.Content = article.Content;
        item.UpdatedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task Delete(Guid id)
    {
        var item = this._articles.Single(x => x.Id == id);
        this._articles.Remove(item);
        return Task.CompletedTask;
    }
}
