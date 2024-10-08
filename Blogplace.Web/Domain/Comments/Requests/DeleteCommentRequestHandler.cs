using Blogplace.Web.Auth;
using Blogplace.Web.Commons.Logging;
using Blogplace.Web.Exceptions;
using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Comments.Requests;

public record DeleteCommentRequest(Guid CommentId) : IRequest;

public class DeleteCommentRequestHandler(
    ICommentsRepository repository,
    ISessionStorage sessionStorage,
    IEventLogger eventLogger
) : IRequestHandler<DeleteCommentRequest>
{
    public async Task Handle(DeleteCommentRequest request, CancellationToken cancellationToken)
    {
        // TODO: Check user permissions

        var commentId = request.CommentId;
        // TODO: Request only author property of specified article
        var comment = await repository.Get(commentId);
        // TODO: What if specified comment doesn't exist in the repository? Currently repository.Get throws an exception

        var userId = sessionStorage.UserId;
        if (comment.AuthorId != userId)
        {
            throw new UserPermissionDeniedException("Requester is not author of article");
        }

        await repository.Delete(commentId);
        eventLogger.UserDeletedComment(userId, commentId);
    }
}