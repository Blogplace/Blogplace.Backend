using Blogplace.Web.Configuration;
using Blogplace.Web.Domain.Users;
using Blogplace.Web.Infrastructure.Database;
using Microsoft.Extensions.Options;

namespace Blogplace.Tests.Integration.Data;

public class UsersRepositoryFake : IUsersRepository
{
    //there could be more predefined "actors", like moderator, banned etc
    public static User? Standard { get; private set; }
    public static User? AnotherStandard { get; private set; }
    public static User? NonePermissions { get; private set; }

    public List<User> Users { get; private set; }

    private static readonly object obj = new ();

    public UsersRepositoryFake(IOptions<PermissionsOptions> permissionOptions)
    {
        var defaultPermissions = permissionOptions.Value.GetDefault();
        lock(obj) 
        {
            Standard ??= new User($"{nameof(Standard).ToLower()}@blogplace.com", defaultPermissions);
            AnotherStandard ??= new User($"{nameof(AnotherStandard).ToLower()}@blogplace.com", defaultPermissions);
            NonePermissions ??= new User($"{nameof(NonePermissions).ToLower()}@blogplace.com", Web.Commons.CommonPermissionsEnum.None);
        }

        this.Users =
        [
            Standard,
            AnotherStandard,
            NonePermissions
        ];
    }

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