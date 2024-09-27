using Blogplace.Web.Auth;
using Blogplace.Web.Commons.Logging;
using Blogplace.Web.Exceptions;
using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Articles.Requests;

public record UpdateArticleRequest(Guid Id, string? NewTitle = null, string? NewContent = null) : IRequest;

public class UpdateArticleRequestHandler(
    ISessionStorage sessionStorage,
    IArticlesRepository repository,
    IUsersRepository usersRepository,
    IPermissionsChecker permissionsChecker,
    IEventLogger logger
) : IRequestHandler<UpdateArticleRequest>
{
    public async Task Handle(UpdateArticleRequest request, CancellationToken cancellationToken)
    {
        var user = await usersRepository.Get(sessionStorage.UserId);
        if (!permissionsChecker.CanUpdateArticle(user.Permissions))
        {
            throw new UserNotAuthorizedException("No permission to update the article");
        }
        
        var isChanged = false;
        //todo get only author of article
        var article = await repository.Get(request.Id);

        if (article.AuthorId != sessionStorage.UserId)
        {
            throw new UserNotAuthorizedException("Requester is not author of article");
        }

        if (request.NewTitle != null)
        {
            article.Title = request.NewTitle;
            isChanged = true;
        }

        if (request.NewContent != null)
        {
            article.Content = request.NewContent;
            isChanged = true;
        }

        if (isChanged)
        {
            await repository.Update(article);
            logger.UserUpdatedArticle(sessionStorage.UserId, article.Id);
        }
    }
}
