//using Blogplace.Web.Auth;
//using Blogplace.Web.Commons.Logging;
//using Blogplace.Web.Domain.Articles.Comments.Requests;
//using Blogplace.Web.Exceptions;
//using Blogplace.Web.Infrastructure.Database;
//using MediatR;

//namespace Blogplace.Web.Domain.Comments.Requests;

//public record DeleteCommentRequest(Guid CommentId) : IRequest;

//public class DeleteCommentRequestHandler(
//    ICommentsRepository repository,
//    ISessionStorage sessionStorage,
//    IEventLogger eventLogger
//) : IRequestHandler<DeleteCommentRequest>
//{
//    public async Task Handle(DeleteCommentRequest request, CancellationToken cancellationToken)
//    {
//        // TODO: Check user permissions

//        var commentId = request.CommentId;
//        // TODO: Request only author property of specified article
//        var comment = await repository.Get(commentId);
//        // TODO: What if specified comment doesn't exist in the repository? Currently, repository.Get throws an exception

//        var userId = sessionStorage.UserId;
//        if (comment.AuthorId != userId)
//        {
//            throw new UserPermissionDeniedException("Requester is not author of article");
//        }

//        if (comment.ParentId.HasValue)
//        {
//            ChildCountSharedLock.Mutex.WaitOne();
//            try
//            {
//                var parentComment = await repository.Get(comment.ParentId.Value);
//                parentComment.ChildCount--;
//                await repository.Update(parentComment);
//            }
//            finally
//            {
//                ChildCountSharedLock.Mutex.ReleaseMutex();
//            }
//        }
        
//        await repository.Delete(commentId);
//        eventLogger.UserDeletedComment(userId, commentId);
//    }
//}