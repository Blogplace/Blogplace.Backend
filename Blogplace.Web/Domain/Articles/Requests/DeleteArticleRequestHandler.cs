using Blogplace.Web.Auth;
using Blogplace.Web.Commons.Logging;
using Blogplace.Web.Exceptions;
using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Articles.Requests;

public record DeleteArticleRequest(Guid Id) : IRequest;
public class DeleteArticleRequestHandler(ISessionStorage sessionStorage, IArticlesRepository repository, IEventLogger logger) : IRequestHandler<DeleteArticleRequest>
{
    public async Task Handle(DeleteArticleRequest request, CancellationToken cancellationToken)
    {
        //todo get only author of article
        var article = await repository.Get(request.Id);
        if (article.AuthorId != sessionStorage.UserId)
        {
            throw new UserNotAuthorizedException("Requester is not author of article");
        }

        await repository.Delete(request.Id);
        logger.UserDeletedArticle(sessionStorage.UserId, article.Id);
    }
}
