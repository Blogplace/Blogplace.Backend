using Blogplace.Web.Domain.Articles;
using Blogplace.Web.Domain.Users;
using Blogplace.Web.Infrastructure.Database;

namespace Blogplace.Tests.Integration.Data;

public class ArticlesRepositoryFake : IArticlesRepository
{
    public List<Article> Articles { get; set; } = [];

    public Task Add(Article article)
    {
        this.Articles.Add(article);
        return Task.CompletedTask;
    }

    public Task<Article> Get(Guid id)
    {
        var result = this.Articles.Single(x => x.Id == id);
        return Task.FromResult(result!);
    }

    public Task<IEnumerable<Article>> Search()
    {
        var results = this.Articles.AsEnumerable();
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

    public Task Delete(Guid id)
    {
        var item = this.Articles.Single(x => x.Id == id);
        this.Articles.Remove(item);
        return Task.CompletedTask;
    }
}

public class UsersRepositoryFake : IUsersRepository
{
    //there could be more predefined "actors", like moderator, banned etc
    public static User Standard { get; } = new User("standard@blogplace.com", Web.Commons.CommonPermissionsEnum.All);

    public List<User> Users { get; set; } = 
    [
        Standard
    ];

    public Task Add(User user)
    {
        this.Users.Add(user);
        return Task.CompletedTask;
    }

    public Task<User> Get(Guid id)
    {
        var result = this.Users.Single(x => x.Id == id);
        return Task.FromResult(result!);
    }

    public Task<User?> GetByEmail(string email)
    {
        var result = this.Users.SingleOrDefault(x => x.Email == email);
        return Task.FromResult(result);
    }

    public Task Update(User user)
    {
        var item = this.Users.Single(x => x.Id == user.Id);
        item.Username = user.Username;
        item.UpdatedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }
}