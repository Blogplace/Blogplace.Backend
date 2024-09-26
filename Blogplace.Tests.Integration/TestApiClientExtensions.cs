using Blogplace.Tests.Integration.Data;
using Blogplace.Web.Auth;
using Blogplace.Web.Commons.Consts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Blogplace.Tests.Integration;

public static class TestApiClientExtensions
{
    public static ApiClient CreateClient_Standard(this WebApplicationFactory<Program> factory) 
        => factory.CreateClient_CustomUserId(UsersRepositoryFake.Standard!.Id);

    public static ApiClient CreateClient_AnotherStandard(this WebApplicationFactory<Program> factory)
        => factory.CreateClient_CustomUserId(UsersRepositoryFake.AnotherStandard!.Id);

    public static ApiClient CreateClient_NonePermissions(this WebApplicationFactory<Program> factory)
        => factory.CreateClient_CustomUserId(UsersRepositoryFake.NonePermissions!.Id);

    public static ApiClient CreateClient_CustomUserId(this WebApplicationFactory<Program> factory, Guid userId)
    {
        var client = factory.CreateDefaultClient();
        var authManager = factory.Services.GetService<IAuthManager>()!;
        var token = authManager.CreateToken(userId, AuthConsts.ROLE_WEB).AccessToken;
        return new(client, token);
    }

    public static ApiClient CreateClient_CustomToken(this WebApplicationFactory<Program> factory, string token)
    {
        var client = factory.CreateDefaultClient();
        return new(client, token);
    }

    public static ApiClient CreateClient_Anonymous(this WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateDefaultClient();
        return new(client);
    }
}