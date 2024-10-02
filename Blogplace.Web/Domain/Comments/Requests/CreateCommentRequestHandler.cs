using Blogplace.Web.Auth;
using Blogplace.Web.Commons.Logging;
using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Comments.Requests;

public record CreateCommentResponse(Guid Id);

public record CreateCommentRequest(Guid ArticleId, string Content, Guid? ParentId) : IRequest<CreateCommentResponse>;

public class CreateCommentRequestHandler(
    ISessionStorage sessionStorage,
    ICommentsRepository commentsRepository,
    IEventLogger logger
) : IRequestHandler<CreateCommentRequest, CreateCommentResponse>
{
    public async Task<CreateCommentResponse> Handle(
        CreateCommentRequest request,
        CancellationToken cancellationToken)
    {
        // TODO: Check user permissions

        var userId = sessionStorage.UserId;

        var comment = new Comment(request.ArticleId, userId, request.Content, request.ParentId);
        await commentsRepository.Add(comment);

        logger.UserCreatedComment(userId, comment.Id);

        return new CreateCommentResponse(comment.Id);
    }
}