using Blogplace.Web.Auth;
using Blogplace.Web.Commons.Logging;
using Blogplace.Web.Exceptions;
using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Articles.Requests;

public record CreateArticleResponse(Guid Id);
public record CreateArticleRequest(string Title, string Content, string[] Tags) : IRequest<CreateArticleResponse>;
public class CreateArticleRequestHandler(
    ISessionStorage sessionStorage,
    IArticlesRepository articlesRepository,
    ITagsRepository tagsRepository,
    IUsersRepository usersRepository,
    IPermissionsChecker permissionsChecker,
    IEventLogger logger
) : IRequestHandler<CreateArticleRequest, CreateArticleResponse>
{
    public async Task<CreateArticleResponse> Handle(CreateArticleRequest request, CancellationToken cancellationToken)
    {
        var userId = sessionStorage.UserId;
        var user = await usersRepository.Get(userId);

        if (!permissionsChecker.CanCreateArticle(user.Permissions))
        {
            throw new UserPermissionDeniedException("No permission to create the article");
        }

        await tagsRepository.AddIfNotExists(request.Tags);
        var tagsIds = await tagsRepository.Get(request.Tags);

        var article = new Article(request.Title, request.Content, userId, tagsIds);
        await articlesRepository.Add(article);

        logger.UserCreatedArticle(userId, article.Id);
        return new CreateArticleResponse(article.Id);
    }
}