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
    public async Task Comments_AnonymousShouldReturnUnauthorized()
    {
        //Arrange
        var client = this._factory.CreateClient_Anonymous();
        var requests = new Dictionary<string, object>();
        var createRequest = new CreateCommentRequest(ArticlesRepositoryFake.StandardUserArticle!.Id,
            "TEST_COMMENT_CONTENT");
        var deleteRequest =
            new DeleteCommentRequest(CommentsRepositoryFake.StandardUserCommentOnStandardUserArticle!.Id);

        requests["/Comments/Create"] = createRequest;
        requests["/Comments/Delete"] = deleteRequest;

        foreach (var request in requests)
        {
            //Act
            var response = await client.PostAsync($"{this.urlBaseV1}{request.Key}", request.Value);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
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
        // TODO: check if the comment actually exist in the repository
    }

    [Test]
    public async Task Delete_CommentShouldBeDeleted()
    {
        //Arrange
        var client = this._factory.CreateClient_Standard();
        var request = new DeleteCommentRequest(CommentsRepositoryFake.StandardUserCommentOnStandardUserArticle!.Id);

        // Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Comments/Delete", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}