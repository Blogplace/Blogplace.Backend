using Blogplace.Tests.Integration.Data;
using Blogplace.Web.Domain.Comments;
using Blogplace.Web.Domain.Comments.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

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
        var createRequest = new CreateCommentRequest(
            ArticlesRepositoryFake.StandardUserArticle!.Id,
            "TEST_COMMENT_CONTENT"
        );
        var updateRequest = new UpdateCommentRequest(
            CommentsRepositoryFake.StdUserCommentOnStdUserArticle!.Id,
            "NEW_CONTENT"
        );
        var deleteRequest =
            new DeleteCommentRequest(CommentsRepositoryFake.StdUserCommentOnStdUserArticle!.Id);

        requests["/Comments/Create"] = createRequest;
        requests["/Comments/Update"] = updateRequest;
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
        var articleId = ArticlesRepositoryFake.StandardUserArticle!.Id;
        var commentContent = "TEST_CONTENT_" + Guid.NewGuid();
        var request = new CreateCommentRequest(
            articleId,
            commentContent
        );

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Comments/Create", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var commentId = (await response.Content.ReadFromJsonAsync<CreateCommentResponse>())!.Id;
        var comments = await this.GetArticleComments(client, articleId);
        comments.Should().NotBeEmpty();
        comments.Should().ContainSingle(x =>
            x.Id == commentId &&
            x.ArticleId == articleId &&
            x.Content == commentContent);
    }
    
    [Test]
    public async Task SearchByArticle_CommentsShouldBeReturned()
    {
        //Arrange
        var client = this._factory.CreateClient_Standard();
        var request = new SearchCommentsByArticleRequest(ArticlesRepositoryFake.StandardUserArticle!.Id);

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Comments/SearchByArticle", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var comments = (await response.Content.ReadFromJsonAsync<SearchCommentsResponse>())!.Comments.ToArray();

        comments.Should().NotBeEmpty();
        comments.Should().ContainSingle(x =>
            x.ArticleId == ArticlesRepositoryFake.StandardUserArticle!.Id &&
            x.Content == CommentsRepositoryFake.StdUserCommentOnStdUserArticle!.Content
        );
    }

    [Test]
    public async Task SearchByParent_CommentsShouldBeReturned()
    {
        //Arrange
        var client = this._factory.CreateClient_Standard();
        var articleId = ArticlesRepositoryFake.StandardUserArticle!.Id;
        var parentCommentId = CommentsRepositoryFake.StdUserCommentOnStdUserArticle!.Id;

        var commentContent = "CHILD_COMMENT_CONTENT";
        var createChildRequest = new CreateCommentRequest(articleId, commentContent, parentCommentId);
        var createChildResponse = await client.PostAsync($"{this.urlBaseV1}/Comments/Create", createChildRequest);
        createChildResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var request =
            new SearchCommentsByParentRequest(CommentsRepositoryFake.StdUserCommentOnStdUserArticle!.Id);

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Comments/SearchByParent", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var comments = (await response.Content.ReadFromJsonAsync<SearchCommentsResponse>())!.Comments.ToArray();

        comments.Should().NotBeEmpty();
        comments.Should().ContainSingle(x => x.ArticleId == articleId && x.Content == commentContent);
    }

    [Test]
    public async Task Update_CommentShouldBeUpdated()
    {
        //Arrange
        var client = this._factory.CreateClient_Standard();
        var articleId = ArticlesRepositoryFake.StandardUserArticle!.Id;
        var commentId = await this.CreateComment(
            client,
            articleId,
            "TEST_CONTENT"
        );

        var newContent = "UPDATED_CONTENT_" + Guid.NewGuid();
        var request = new UpdateCommentRequest(
            commentId,
            newContent
        );

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Comments/Update", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var comments = await this.GetArticleComments(client, articleId);
        comments.Should().NotBeEmpty();
        comments.Should().ContainSingle(x =>
            x.Id == commentId &&
            x.ArticleId == articleId &&
            x.Content == newContent);
    }

    [Test]
    public async Task Delete_CommentShouldBeDeleted()
    {
        //Arrange
        var client = this._factory.CreateClient_Standard();
        var articleId = ArticlesRepositoryFake.StandardUserArticle!.Id;
        var commentId = await this.CreateComment(
            client,
            articleId,
            "TEST_COMMENT_CONTENT"
        );
        var request = new DeleteCommentRequest(commentId);

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Comments/Delete", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var comments = await this.GetArticleComments(client, articleId);
        comments.Should().NotContain(x => x.Id == commentId);
    }

    private async Task<Guid> CreateComment(ApiClient client, Guid articleId, string content, Guid? parentId = null)
    {
        var request = new CreateCommentRequest(articleId, content);
        var response = await client.PostAsync($"{this.urlBaseV1}/Comments/Create", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var commentId = (await response.Content.ReadFromJsonAsync<CreateCommentResponse>())!.Id;
        return commentId;
    }

    private async Task<CommentDto[]> GetArticleComments(ApiClient client, Guid articleId)
    {
        var request = new SearchCommentsByArticleRequest(articleId);
        var response = await client.PostAsync($"{this.urlBaseV1}/Comments/SearchByArticle", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var comments = (await response.Content.ReadFromJsonAsync<SearchCommentsResponse>())!.Comments.ToArray();
        return comments.ToArray();
    }
}