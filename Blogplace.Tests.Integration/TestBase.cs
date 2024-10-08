using Blogplace.Tests.Integration.Data;
using Blogplace.Web.Commons.Consts;
using Blogplace.Web.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Blogplace.Tests.Integration;

[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
public abstract class TestBase
{
    protected static Guid StandardUserId => UsersRepositoryFake.Standard?.Id ?? Guid.Empty;
    protected string urlBaseV1 = "/public/api/v1.0";
    
    protected static WebApplicationFactory<Program> StartServer(Action<IServiceCollection>? registerServices = null)
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureServices(x => 
            {
                x.AddSingleton<IUsersRepository, UsersRepositoryFake>();
                x.AddSingleton<IArticlesRepository, ArticlesRepositoryFake>();
                x.AddSingleton<ITagsRepository, TagsRepositoryFake>();
                x.AddSingleton<ICommentsRepository, CommentsRepositoryFake>();
                registerServices?.Invoke(x); 
            }));

        InitializeRepositories(factory.Services);

        return factory;
    }

    private static void InitializeRepositories(IServiceProvider services)
    {
        ((UsersRepositoryFake)services.GetService<IUsersRepository>()!).Init();
        ((TagsRepositoryFake)services.GetService<ITagsRepository>()!).Init();
        ((ArticlesRepositoryFake)services.GetService<IArticlesRepository>()!).Init();
        ((CommentsRepositoryFake)services.GetService<ICommentsRepository>()!).Init();
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
