using Blogplace.Web.Auth;
using Blogplace.Web.Commons.Consts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Blogplace.Tests.Integration;

[TestFixture]
public abstract class TestBase
{
    protected readonly WebApplicationFactory<Program> _factory = new();
    protected Guid currentUserId = Guid.NewGuid();
    protected string urlBaseV1 = "/public/api/v1.0";

    protected ApiClient CreateClient(Action<IServiceCollection>? registerServices = null, bool withSession = true, Guid? customUserId = null/*, bool mobileSession = false, bool withSession = true, string? customToken = null*/)
    {
        var client = this._factory
            .WithWebHostBuilder(builder => builder.ConfigureServices(x => registerServices?.Invoke(x)))
            .CreateDefaultClient();

        //if (customToken != null)
        //{
        //    return new(client, customToken);
        //}

        var authManager = this._factory.Services.GetService<IAuthManager>()!;
        if (withSession)
        {
            var userId = customUserId ?? this.currentUserId;
            var token = authManager.CreateToken(userId, AuthConsts.ROLE_WEB).AccessToken;
            return new(client, authManager, token);
        }

        return new(client, authManager);
    }
}

public class ApiClient(HttpClient client, IAuthManager authManager, string? token = null)
{
    public async Task<HttpResponseMessage> PostAsync([StringSyntax(StringSyntaxAttribute.Uri)] string url, object? value = null)
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

        var result = await client.SendAsync(message);
        return result;
    }

    public ApiClient WithDifferentToken(Guid? userId = null, string? newToken = null)
    {
        if (newToken == null)
        {
            var newUserId = userId ?? Guid.NewGuid();
            newToken = authManager.CreateToken(newUserId, AuthConsts.ROLE_WEB).AccessToken;
        }

        return new(client, authManager, newToken);
    }

    public ApiClient WithoutToken() => new(client, authManager);

    //todo better GET or use only POST
    public async Task<HttpResponseMessage> GetAsync([StringSyntax(StringSyntaxAttribute.Uri)] string url)
    {
        var message = new HttpRequestMessage(HttpMethod.Get, url);
        var result = await client.SendAsync(message);
        return result;
    }
}