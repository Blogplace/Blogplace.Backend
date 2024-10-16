using Blogplace.Web.Auth;
using Blogplace.Web.Commons.Logging;
using Blogplace.Web.Domain.Articles.Comments.Requests;
using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Comments.Requests;

public record CreateCommentResponse(Guid Id);

public record CreateCommentRequest(Guid ArticleId, string Content, Guid? ParentId = null)
    : IRequest<CreateCommentResponse>;

public class CreateCommentRequestHandler(
    ISessionStorage sessionStorage,
    ICommentsRepository repository,
    IEventLogger logger
) : IRequestHandler<CreateCommentRequest, CreateCommentResponse>
{
    public async Task<CreateCommentResponse> Handle(
        CreateCommentRequest request,
        CancellationToken cancellationToken)
    {
        var userId = sessionStorage.UserId;
        // TODO: Check user permissions

        if (request.ParentId.HasValue)
        {
            ChildCountSharedLock.Mutex.WaitOne();
            try
            {
                var parentComment = await repository.Get(request.ParentId.Value);
                parentComment.ChildCount++;
                await repository.Update(parentComment);
            }
            finally
            {
                ChildCountSharedLock.Mutex.ReleaseMutex();
            }
        }
        
        var comment = new Comment(request.ArticleId, userId, request.Content, request.ParentId);
        await repository.Add(comment);

        logger.UserCreatedComment(userId, comment.Id);

        return new CreateCommentResponse(comment.Id);
    }
}