using Blogplace.Web.Domain.Articles;
using Blogplace.Web.Infrastructure.Database;

namespace Blogplace.Tests.Integration.Data;

public class ArticlesRepositoryFake : IArticlesRepository
{
    public static Article? StandardUserArticle { get; set; }
    public static Article? NonePermissionsUserArticle { get; set; }

    public List<Article> Articles { get; private set; } = [];

    private static readonly object obj = new();

    public void Init()
    {
        lock (obj)
        {
            StandardUserArticle ??=
                new Article(Guid.NewGuid().ToString(), "TEST_TITLE", "TEST_CONTENT",
                    [TagsRepositoryFake.DefaultTag!]);
            NonePermissionsUserArticle ??=
                new Article(Guid.NewGuid().ToString(), "TEST_TITLE", "TEST_CONTENT",
                    [TagsRepositoryFake.DefaultTag!]);
        }

        this.Articles =
        [
            StandardUserArticle,
            NonePermissionsUserArticle
        ];
    }

    public Task Add(Article article)
    {
        this.Articles.Add(article);
        return Task.CompletedTask;
    }

    public Task<Article> Get(string id)
    {
        var result = this.Articles.Single(x => x.Id == id);
        return Task.FromResult(result!);
    }

    public Task<IEnumerable<Article>> Search(int limit, Guid? tagId = null)
    {
        var results = this.Articles.Where(x => 
        {
            if (tagId.HasValue && !x.TagIds.Contains(tagId.Value))
            {
                return false;
            }

            return true;
        }).Take(limit);
        return Task.FromResult(results!);
    }

    public Task Update(Article article)
    {
        var item = this.Articles.Single(x => x.Id == article.Id);
        item.Title = article.Title;
        item.Content = article.Content;
        item.UpdatedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task Delete(string id)
    {
        var item = this.Articles.Single(x => x.Id == id);
        this.Articles.Remove(item);
        return Task.CompletedTask;
    }

    public Task<int> CountArticlesWithTag(Guid tag)
    {
        var results = this.Articles.Count(x => x.TagIds.Contains(tag));
        return Task.FromResult(results!);
    }
}
