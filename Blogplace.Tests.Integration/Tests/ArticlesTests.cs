using Blogplace.Tests.Integration.Data;
using Blogplace.Web.Domain.Articles;
using Blogplace.Web.Domain.Articles.Requests;
using Blogplace.Web.Infrastructure.Database;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Blogplace.Tests.Integration.Tests;
public class ArticlesTests : TestBase
{
    private WebApplicationFactory<Program> _factory;

    [SetUp]
    public void SetUp() => this._factory = StartServer();

    [TearDown]
    public void TearDown() => this._factory?.Dispose();

    [Test]
    public async Task Create_AnonymousReturnsUnauthorized()
    {
        //Arrange
        var client = this._factory.CreateClient_Anonymous();
        var request = new CreateArticleRequest("TEST_TITLE", "TEST_CONTENT", ["default"]);

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Articles/Create", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Create_ArticleShouldBeCreated()
    {
        //Arrange
        var client = this._factory.CreateClient_Standard();
        var createRequest = new CreateArticleRequest("TEST_TITLE", "TEST_CONTENT", ["default"]);

        //Act
        var createResponse = await client.PostAsync($"{this.urlBaseV1}/Articles/Create", createRequest);

        //Assert
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var articleId = (await createResponse.Content.ReadFromJsonAsync<CreateArticleResponse>())!.Id;

        var result = await this.GetArticleById(client, articleId, anonymous: true);

        result.Id.Should().Be(articleId);
        result.Title.Should().Be(createRequest.Title);
        result.Content.Should().Be(createRequest.Content);
        result.AuthorId.Should().Be(StandardUserId);
        result.CreatedAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1)).And.BeBefore(DateTime.UtcNow);
        result.UpdatedAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1)).And.BeBefore(DateTime.UtcNow);
        result.Tags.Single().Should().Be(TagsRepositoryFake.DefaultTag!.Id);
    }
    
    [Test]
    public async Task Create_ArticleShouldNotBeCreatedWithoutPermission()
    {
        //Arrange
        var client = this._factory.CreateClient_NonePermissions();
        var createRequest = new CreateArticleRequest("TEST_TITLE", "TEST_CONTENT", ["default"]);

        //Act
        var createResponse = await client.PostAsync($"{this.urlBaseV1}/Articles/Create", createRequest);

        //Assert
        createResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task Search_ShouldReturnListOfArticles()
    {
        //Arrange
        var client = this._factory.CreateClient_Standard();
        var article1 = await this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT");
        var article2 = await this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT");
        var article3 = await this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT");
        var request = new SearchArticlesRequest();

        var anonymousClient = this._factory.CreateClient_Anonymous();

        //Act
        var response = await anonymousClient.PostAsync($"{this.urlBaseV1}/Articles/Search", request);

        //Assert
        var result = (await response.Content.ReadFromJsonAsync<SearchArticlesResponse>())!.Articles.ToArray();
        result.Should().NotBeEmpty();
        result.Should().HaveCountGreaterThan(3);
        result.Select(x => x.Id).ToArray().Should().Contain([article1, article2, article3]);
    }

    [Test]
    public async Task Search_ShouldReturnArticleWithTag()
    {
        //Arrange
        var tagToSearch = "search";
        var titleToSearch = Guid.NewGuid().ToString();
        var client = this._factory.CreateClient_Standard();

        Task.WaitAll(
        [
            this.CreateArticle(client, titleToSearch, "TEST_CONTENT", [tagToSearch, "a", "b", "c"]),
            this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT", ["a", "b", "c"]),
        ]);

        var tag = (await this.SearchTags(client, tagToSearch)).Single();
        tag.Count.Should().Be(1);

        var request = new SearchArticlesRequest(tag.Id);
        var anonymousClient = this._factory.CreateClient_Anonymous();

        //Act
        var response = await anonymousClient.PostAsync($"{this.urlBaseV1}/Articles/Search", request);

        //Assert
        var result = (await response.Content.ReadFromJsonAsync<SearchArticlesResponse>())!.Articles.ToArray();
        result.Should().HaveCount(1);
        result.Should().ContainSingle(x => x.Title.Equals(titleToSearch));
    }

    [Test]
    public async Task Update_AnonymousReturnsUnauthorized()
    {
        //Arrange
        var client = this._factory.CreateClient_Anonymous();
        var request = new UpdateArticleRequest(Guid.NewGuid());

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Articles/Update", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Update_ArticleShouldBeUpdated()
    {
        //Arrange
        var tagsRepository = (TagsRepositoryFake)this._factory.Services.GetRequiredService<ITagsRepository>();
        var client = this._factory.CreateClient_Standard();
        var articleId = await this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT");

        var updateRequest = new UpdateArticleRequest(articleId, "NEW_TITLE", "NEW_CONTENT", ["updated_tag"]);

        //Act
        var updateResponse = await client.PostAsync($"{this.urlBaseV1}/Articles/Update", updateRequest);

        //Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await this.GetArticleById(client, articleId, anonymous: true);

        result.Id.Should().Be(articleId);
        result.Title.Should().Be(updateRequest.NewTitle);
        result.Content.Should().Be(updateRequest.NewContent);
        result.AuthorId.Should().Be(StandardUserId);
        result.CreatedAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1)).And.BeBefore(DateTime.UtcNow);
        result.UpdatedAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1)).And.BeBefore(DateTime.UtcNow);
        result.UpdatedAt.Should().NotBe(result.CreatedAt);

        var newTag = tagsRepository.Tags.Single(x => x.Name == "updated_tag");
        result.Tags.Single().Should().Be(newTag.Id);
    }

    [Test]
    public async Task Update_ArticleShouldNotBeUpdatedByOtherUser()
    {
        //Arrange
        var client = this._factory.CreateClient_Standard();
        var articleId = await this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT");

        var otherClient = this._factory.CreateClient_AnotherStandard();

        var updateRequest = new UpdateArticleRequest(articleId, "NEW_TITLE", "NEW_CONTENT");

        //Act
        var updateResponse = await otherClient.PostAsync($"{this.urlBaseV1}/Articles/Update", updateRequest);

        //Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        var article = await this.GetArticleById(client, articleId, anonymous: true);

        article.Title.Should().NotBe("NEW_TITLE");
        article.Content.Should().NotBe("NEW_CONTENT");
    }

    [Test]
    public async Task Update_ArticleShouldNotBeUpdatedWithoutPermission()
    {
        //Arrange
        var client = this._factory.CreateClient_NonePermissions();
        var articleId = ArticlesRepositoryFake.NonePermissionsUserArticle!.Id;
        var updateRequest = new UpdateArticleRequest(articleId, "NEW_TITLE", "NEW_CONTENT");

        //Act
        var updateResponse = await client.PostAsync($"{this.urlBaseV1}/Articles/Update", updateRequest);

        //Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var article = await this.GetArticleById(client, articleId, true);
        article.Title.Should().NotBe("NEW_TITLE");
        article.Content.Should().NotBe("NEW_CONTENT");
    }

    [Test]
    public async Task Delete_AnonymousReturnsUnauthorized()
    {
        //Arrange
        var client = this._factory.CreateClient_Anonymous();
        var request = new DeleteArticleRequest(Guid.NewGuid());

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Articles/Delete", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Delete_ShouldDelete()
    {
        //Arrange
        var client = this._factory.CreateClient_Standard();
        var articleId = await this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT");
        var request = new DeleteArticleRequest(articleId);

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Articles/Delete", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        //todo get should return 404 Not Found
    }

    [Test]
    public async Task Delete_ShouldNotBeDeletedByOtherUser()
    {
        //Arrange
        var client = this._factory.CreateClient_Standard();
        var articleId = await this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT");

        var otherClient = this._factory.CreateClient_AnotherStandard();

        var request = new DeleteArticleRequest(articleId);

        //Act
        var response = await otherClient.PostAsync($"{this.urlBaseV1}/Articles/Delete", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        var article = await this.GetArticleById(client, articleId, anonymous: true);
        article.Should().NotBeNull();
    }

    [Test]
    public async Task Delete_ShouldNotBeDeletedWithoutPermission()
    {
        //Arrange
        var client = this._factory.CreateClient_NonePermissions();
        var articleId = ArticlesRepositoryFake.NonePermissionsUserArticle!.Id;
        var request = new DeleteArticleRequest(articleId);

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Articles/Delete", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        var article = await this.GetArticleById(client, articleId, true);
        article.Should().NotBeNull();
    }

    [TestCase(true, true, 3000, true)]
    [TestCase(true, true, 500, false)]
    [TestCase(true, false, 3000, false)]
    [TestCase(false, true, 3000, false)]
    public async Task View_ShouldIncreaseCounter_IfValid(bool sameUserAgent, bool sameArticleId, int delay, bool shouldIncrease)
    {
        //Arrange
        var client = this._factory.CreateClient_Standard();
        var articleId = await this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT");

        var headersGetPost = new Dictionary<string, string>()
        {
            { "User-Agent", Guid.NewGuid().ToString() },
            { "Referer", $"https://localhost:3000/posts/{articleId}?custom=test" },
        };

        var viewArticleId = sameArticleId ? articleId : Guid.NewGuid();
        var headersViewPost = new Dictionary<string, string>()
        {
            { "User-Agent", sameUserAgent ? headersGetPost["User-Agent"] : Guid.NewGuid().ToString() },
            { "Referer", $"https://localhost:3000/posts/{viewArticleId}?custom=test" },
        };

        //Act
        var anonymousClient = this._factory.CreateClient_Anonymous();
        var getRequest = new GetArticleRequest(articleId);
        var getResponse = await anonymousClient.PostAsync($"{this.urlBaseV1}/Articles/Get", getRequest, customHeaders: headersGetPost);
        (await getResponse.Content.ReadFromJsonAsync<GetArticleResponse>())!
            .Deconstruct(out var oldArticle, out var viewId);

        await Task.Delay(delay);
        var viewRequest = new ViewArticleRequest(viewId);
        var viewResponse = await anonymousClient.PostAsync($"{this.urlBaseV1}/Articles/View", viewRequest, customHeaders: headersViewPost);
        viewResponse.StatusCode.Should().Be(shouldIncrease ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);

        var newGetResponse = await anonymousClient.PostAsync($"{this.urlBaseV1}/Articles/Get", getRequest);
        (await newGetResponse.Content.ReadFromJsonAsync<GetArticleResponse>())!
            .Deconstruct(out var newArticle, out var _);

        //Assert
        oldArticle.Views.Should().Be(0);
        newArticle.Views.Should().Be(shouldIncrease ? 1 : 0);
    }

    [Test]
    public async Task SearchTags_ShouldReturnTagsQuantity()
    {
        //Arrange
        var exceptedTagsCount = new Dictionary<string, int>()
        {
            { "a", 2 },
            { "b", 4 },
            { "c", 5 },
        };

        var client = this._factory.CreateClient_Standard();
        Task.WaitAll(
        [
            this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT", ["a", "b", "c"]),
            this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT", ["a", "b", "c"]),
            this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT", ["b", "c"]),
            this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT", ["b", "c"]),
            this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT", ["c"])
        ]);
        var request = new SearchTagsRequest(null);
        var anonymousClient = this._factory.CreateClient_Anonymous();

        //Act
        var response = await anonymousClient.PostAsync($"{this.urlBaseV1}/Articles/SearchTags", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        //Assert
        (await response.Content.ReadFromJsonAsync<SearchTagsResponse>())!
            .TagCounts
            .Where(x => exceptedTagsCount.ContainsKey(x.Name))
            .ToDictionary(x => x.Name, x => x.Count)!
            .Should().BeEquivalentTo(exceptedTagsCount);
    }

    [Test]
    public async Task GetTagsByIds_ShouldReturnTags()
    {
        //Arrange
        var client = this._factory.CreateClient_Standard();
        Task.WaitAll(
        [
            this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT", ["a", "b", "c"]),
            this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT", ["a", "b", "c"]),
            this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT", ["b", "c"]),
            this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT", ["b", "c"]),
            this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT", ["c"])
        ]);
        var anonymous = this._factory.CreateClient_Anonymous();
        var tags = (await this.SearchTags(anonymous, null))
            .Select(x => new TagDto(x.Id, x.Name))
            .ToArray();
        var request = new GetTagsByIdsRequest(tags.Select(x => x.Id));

        //Act
        var response = await anonymous.PostAsync($"{this.urlBaseV1}/Articles/GetTagsByIds", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        //Assert
        var result = (await response.Content.ReadFromJsonAsync<GetTagsByIdsResponse>())!.Tags.ToArray();
        result.Should().BeEquivalentTo(tags);
    }

    private async Task<Guid> CreateArticle(ApiClient client, string title, string content, string[]? tags = null)
    {
        var request = new CreateArticleRequest(title, content, tags ?? ["default"]);
        var response = await client.PostAsync($"{this.urlBaseV1}/Articles/Create", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var articleId = (await response.Content.ReadFromJsonAsync<CreateArticleResponse>())!.Id;
        return articleId;
    }

    private async Task<ArticleDto> GetArticleById(ApiClient client, Guid articleId, bool anonymous = false)
    {
        var currentClient = anonymous ? this._factory.CreateClient_Anonymous() : client;
        var getRequest = new GetArticleRequest(articleId);
        var getResponse = await currentClient.PostAsync($"{this.urlBaseV1}/Articles/Get", getRequest);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = (await getResponse.Content.ReadFromJsonAsync<GetArticleResponse>())!.Article;
        return result;
    }

    private async Task<IEnumerable<TagCount>> SearchTags(ApiClient client, string? containsName = null)
    {
        var request = new SearchTagsRequest(containsName);
        var response = await client.PostAsync($"{this.urlBaseV1}/Articles/SearchTags", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        return (await response.Content.ReadFromJsonAsync<SearchTagsResponse>())!.TagCounts;
    }
}
