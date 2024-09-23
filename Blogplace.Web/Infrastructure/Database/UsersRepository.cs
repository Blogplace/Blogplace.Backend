using Blogplace.Web.Domain;

namespace Blogplace.Web.Infrastructure.Database;

public interface IUsersRepository
{
    Task Add(User user);
    Task<User> Get(Guid id);
    Task<User?> GetByEmail(string email);
    Task Update(User user);
}

public class UsersRepository : IUsersRepository
{
    private readonly List<User> _users = [];

    public Task Add(User user)
    {
        this._users.Add(user);
        return Task.CompletedTask;
    }

    public Task<User> Get(Guid id)
    {
        var result = this._users.Single(x => x.Id == id);
        return Task.FromResult(result!);
    }

    public Task<User?> GetByEmail(string email)
    {
        var result = this._users.SingleOrDefault(x => x.Email == email);
        return Task.FromResult(result);
    }

    public Task Update(User user)
    {
        var item = this._users.Single(x => x.Id == user.Id);
        item.Username = user.Username;
        item.UpdatedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }
}
