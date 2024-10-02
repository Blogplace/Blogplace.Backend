using Blogplace.Tests.Integration.Data;
using Blogplace.Web.Domain.Comments.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Blogplace.Tests.Integration.Tests;

public class CommentsTests : TestBase
{
    private WebApplicationFactory<Program> _factory;

    [SetUp]
    public void SetUp() => this._factory = StartServer();

    [TearDown]
    public void TearDown() => this._factory?.Dispose();

    [Test]
    [TestCase("/Comments/Create")]
    public async Task Comments_AnonymousShouldReturnUnauthorized(string url)
    {
        //Arrange
        var client = this._factory.CreateClient_Anonymous();
        var request = new CreateCommentRequest(ArticlesRepositoryFake.NonePermissionsUserArticle!.Id,
            "TEST_COMMENT_CONTENT");

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}{url}", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        // TODO: check if the comment actually exist in the repository
    }

    [Test]
    public async Task Create_CommentShouldBeCreated()
    {
        //Arrange
        var client = this._factory.CreateClient_Standard();
        var request = new CreateCommentRequest(ArticlesRepositoryFake.NonePermissionsUserArticle!.Id,
            "TEST_COMMENT_CONTENT");

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Comments/Create", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}