using Blogplace.Web.Auth;
using Blogplace.Web.Commons.Logging;
using Blogplace.Web.Exceptions;
using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Comments.Requests;

public record UpdateCommentRequest(Guid Id, string NewContent) : IRequest;

public class UpdateCommentRequestHandler(
    ISessionStorage sessionStorage,
    ICommentsRepository repository,
    IEventLogger logger
) : IRequestHandler<UpdateCommentRequest>
{
    public async Task Handle(UpdateCommentRequest request, CancellationToken cancellationToken)
    {
        // TODO: Check user permissions
        var userId = sessionStorage.UserId;

        // TODO: Request only author property of specified article
        var comment = await repository.Get(request.Id);
        if (comment.AuthorId != userId)
        {
            throw new UserPermissionDeniedException("Requester is not author of comment");
        }

        comment.Content = request.NewContent;

        await repository.Update(comment);
        logger.UserUpdatedComment(userId, comment.Id);
    }
}