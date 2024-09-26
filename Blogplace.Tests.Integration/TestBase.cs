using Blogplace.Tests.Integration.Data;
using Blogplace.Web.Auth;
using Blogplace.Web.Commons.Consts;
using Blogplace.Web.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Blogplace.Tests.Integration;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public abstract class TestBase
{
    protected Guid StandardUserId { get; } = UsersRepositoryFake.Standard.Id;
    protected string urlBaseV1 = "/public/api/v1.0";
    
    protected WebApplicationFactory<Program> StartServer(Action<IServiceCollection>? registerServices = null)
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureServices(x => 
            {
                x.AddSingleton<IUsersRepository>(new UsersRepositoryFake());
                x.AddSingleton<IArticlesRepository>(new ArticlesRepositoryFake());
                registerServices?.Invoke(x); 
            }));

        return factory;
    }
}

public class ApiClient(HttpClient client, string? token = null)
{
    public async Task<HttpResponseMessage> PostAsync([StringSyntax(StringSyntaxAttribute.Uri)] string url, object? value = null, Dictionary<string, string>? customHeaders = null)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, url);
        if (value != null)
        {
            message.Content = new StringContent(
                JsonConvert.SerializeObject(value),
                Encoding.UTF8,
                "application/json");
        }

        if (token != null)
        {
            message.Headers.Add("Cookie", $"{AuthConsts.ACCESS_TOKEN_COOKIE}={token};");
        }

        if (customHeaders != null)
        {
            foreach (var header in customHeaders)
            {
                message.Headers.Add(header.Key, header.Value);
            }
        }

        var result = await client.SendAsync(message);
        return result;
    }

    //todo better GET or use only POST
    public async Task<HttpResponseMessage> GetAsync([StringSyntax(StringSyntaxAttribute.Uri)] string url)
    {
        var message = new HttpRequestMessage(HttpMethod.Get, url);
        var result = await client.SendAsync(message);
        return result;
    }
}

public static class TestApiClientExtensions
{
    public static ApiClient CreateClient_Standard(this WebApplicationFactory<Program> factory)
        => factory.CreateClient_CustomUserId(UsersRepositoryFake.Standard.Id);

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