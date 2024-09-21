using Blogplace.Web.Domain;

namespace Blogplace.Web.Infrastructure.Database;

public class ArticlesRepository
{
    private readonly List<Article>? _articles;

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

    public void Update(Guid id, )
    {
        var item = this._articles.Single(x => x.Id == id);
        item.Title = "a";
    }

    public void Delete(Guid id)
    {

    }
}
