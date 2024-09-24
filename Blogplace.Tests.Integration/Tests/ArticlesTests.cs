using Blogplace.Web.Domain.Articles;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Blogplace.Tests.Integration.Tests;
public class ArticlesTests : TestBase
{
    [Test]
    public async Task Create_UnauthorizedReturnsUnauthorized()
    {
        //Arrange
        var client = this.CreateClient(withSession: false);
        var request = new CreateArticleRequest("TEST_TITLE", "TEST_CONTENT");

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Articles/Create", request);

        //Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Create_ArticleShouldBeCreated()
    {
        //Arrange
        var client = this.CreateClient(withSession: true);
        var createRequest = new CreateArticleRequest("TEST_TITLE", "TEST_CONTENT");

        //Act
        var createResponse = await client.PostAsync($"{this.urlBaseV1}/Articles/Create", createRequest);

        //Assert
        createResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var articleId = (await createResponse.Content.ReadFromJsonAsync<CreateArticleResponse>())!.Id;

        var result = await this.GetArticleById(client, articleId, anonymous: true);

        result.Id.Should().Be(articleId);
        result.Title.Should().Be(createRequest.Title);
        result.Content.Should().Be(createRequest.Content);
        result.AuthorId.Should().Be(this.currentUserId);
        result.CreatedAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1)).And.BeBefore(DateTime.UtcNow);
        result.UpdatedAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1)).And.BeBefore(DateTime.UtcNow);
    }

    [Test]
    public async Task Search_ShouldReturnListOfArticles()
    {
        //Arrange
        var client = this.CreateClient(withSession: true);
        var article1 = await this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT");
        var article2 = await this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT");
        var article3 = await this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT");
        var request = new SearchArticlesRequest();

        var anonymousClient = client.WithoutToken();

        //Act
        var response = await anonymousClient.PostAsync($"{this.urlBaseV1}/Articles/Search", request);

        //Assert
        var result = (await response.Content.ReadFromJsonAsync<SearchArticlesResponse>())!.Articles;
        result.Should().NotBeEmpty();
        result.Should().HaveCount(3);
        result.Select(x => x.Id).ToArray().Should().BeEquivalentTo([article1, article2, article3]);
    }

    [Test]
    public async Task Update_UnauthorizedReturnsUnauthorized()
    {
        //Arrange
        var client = this.CreateClient(withSession: false);
        var request = new UpdateArticleRequest(Guid.NewGuid());

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Articles/Update", request);

        //Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Update_ArticleShouldBeUpdated()
    {
        //Arrange
        var client = this.CreateClient(withSession: true);
        var articleId = await this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT");

        var updateRequest = new UpdateArticleRequest(articleId, "NEW_TITLE", "NEW_CONTENT");

        //Act
        var updateResponse = await client.PostAsync($"{this.urlBaseV1}/Articles/Update", updateRequest);

        //Assert
        updateResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var result = await this.GetArticleById(client, articleId, anonymous: true);

        result.Id.Should().Be(articleId);
        result.Title.Should().Be(updateRequest.NewTitle);
        result.Content.Should().Be(updateRequest.NewContent);
        result.AuthorId.Should().Be(this.currentUserId);
        result.CreatedAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1)).And.BeBefore(DateTime.UtcNow);
        result.UpdatedAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1)).And.BeBefore(DateTime.UtcNow);
        result.UpdatedAt.Should().NotBe(result.CreatedAt);
    }

    [Test]
    public async Task Update_ArticleShouldNotBeUpdatedByOtherUser()
    {
        //Arrange
        var client = this.CreateClient(withSession: true);
        var articleId = await this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT");

        var otherClient = client.WithDifferentToken();

        var updateRequest = new UpdateArticleRequest(articleId, "NEW_TITLE", "NEW_CONTENT");

        //Act
        var updateResponse = await otherClient.PostAsync($"{this.urlBaseV1}/Articles/Update", updateRequest);

        //Assert
        updateResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        var article = await this.GetArticleById(client, articleId, anonymous: true);

        article.Title.Should().NotBe("NEW_TITLE");
        article.Content.Should().NotBe("NEW_CONTENT");
    }

    [Test]
    public async Task Delete_UnauthorizedReturnsUnauthorized()
    {
        //Arrange
        var client = this.CreateClient(withSession: false);
        var request = new DeleteArticleRequest(Guid.NewGuid());

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Articles/Delete", request);

        //Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Delete_ShouldDelete()
    {
        //Arrange
        var client = this.CreateClient(withSession: true);
        var articleId = await this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT");
        var request = new DeleteArticleRequest(articleId);

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Articles/Delete", request);

        //Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        //todo get should return 404 Not Found
    }

    [Test]
    public async Task Delete_ShouldNotBeDeletedByOtherUser()
    {
        //Arrange
        var client = this.CreateClient(withSession: true);
        var articleId = await this.CreateArticle(client, "TEST_TITLE", "TEST_CONTENT");

        var otherClient = client.WithDifferentToken();

        var request = new DeleteArticleRequest(articleId);

        //Act
        var response = await otherClient.PostAsync($"{this.urlBaseV1}/Articles/Delete", request);

        //Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        var article = await this.GetArticleById(client, articleId, anonymous: true);
        article.Should().NotBeNull();
    }

    [TestCase(true, true, 3000, true)]
    [TestCase(true, true, 500, false)]
    [TestCase(true, false, 3000, false)]
    [TestCase(false, true, 3000, false)]
    public async Task View_ShouldIncreaseCounter_IfValid(bool sameUserAgent, bool sameArticleId, int delay, bool shouldIncrease)
    {
        //Arrange
        var client = this.CreateClient(withSession: true);
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
        var anonymousClient = client.WithoutToken();
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

    private async Task<Guid> CreateArticle(ApiClient client, string title, string content)
    {
        var request = new CreateArticleRequest(title, content);
        var response = await client.PostAsync($"{this.urlBaseV1}/Articles/Create", request);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var articleId = (await response.Content.ReadFromJsonAsync<CreateArticleResponse>())!.Id;
        return articleId;
    }

    private async Task<ArticleDto> GetArticleById(ApiClient client, Guid articleId, bool anonymous = false)
    {
        var currentClient = anonymous ? client.WithoutToken() : client;
        var getRequest = new GetArticleRequest(articleId);
        var getResponse = await currentClient.PostAsync($"{this.urlBaseV1}/Articles/Get", getRequest);
        getResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var result = (await getResponse.Content.ReadFromJsonAsync<GetArticleResponse>())!.Article;
        return result;
    }
}
