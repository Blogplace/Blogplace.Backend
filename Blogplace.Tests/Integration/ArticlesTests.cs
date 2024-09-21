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
    public async Task Create_ArticleShouldBeCreated()
    {
        //Arrange
        var authorId = Guid.NewGuid();

        var client = this.CreateClient(withSession: true, customUserId: authorId);
        var createRequest = new CreateArticleRequest("TEST_TITLE", "TEST_CONTENT");

        //Act
        var createResponse = await client.PostAsync($"{this.urlBaseV1}/Articles/Create", createRequest);

        //Assert
        createResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var articleId = (await createResponse.Content.ReadFromJsonAsync<CreateArticleResponse>())!.Id;
        articleId.Should().NotBeEmpty();
    }
}
