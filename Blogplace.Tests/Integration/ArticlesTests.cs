using Blogplace.Web.Domain;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Blogplace.Tests.Integration;
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
    public async Task CreateArticleAndGetAsAnonymousUser()
    {
        var authorId = Guid.NewGuid();

        var client = this.CreateClient(withSession: true, customUserId: authorId);
        var createRequest = new CreateArticleRequest("TEST_TITLE", "TEST_CONTENT");
        var createResponse = await client.PostAsync($"{this.urlBaseV1}/Articles/Create", createRequest);

        createResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var articleId = (await createResponse.Content.ReadFromJsonAsync<CreateArticleResponse>())!.Id;
        articleId.Should().NotBeEmpty();

        var unauthorizedClient = this.CreateClient(withSession: false);
        var getRequest = new GetArticleRequest(articleId);
        var getResponse = await unauthorizedClient.PostAsync($"{this.urlBaseV1}/Articles/Get", getRequest);

        var result = (await getResponse.Content.ReadFromJsonAsync<GetArticleResponse>())!.Article;

        result.Title.Should().Be(createRequest.Title);
        result.Content.Should().Be(createRequest.Content);
        result.AuthorId.Should().Be(authorId);
        result.CreatedAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1)).And.BeBefore(DateTime.UtcNow);
        result.UpdatedAt.Should().Be(result.CreatedAt);
    }
}
