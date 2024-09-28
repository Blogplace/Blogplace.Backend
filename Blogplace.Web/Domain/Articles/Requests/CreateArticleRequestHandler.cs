﻿using Blogplace.Web.Auth;
using Blogplace.Web.Commons.Logging;
using Blogplace.Web.Exceptions;
using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Articles.Requests;

public record CreateArticleResponse(Guid Id);
public record CreateArticleRequest(string Title, string Content) : IRequest<CreateArticleResponse>;
public class CreateArticleRequestHandler(
    ISessionStorage sessionStorage,
    IArticlesRepository repository,
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

        var article = new Article(request.Title, request.Content, userId);
        await repository.Add(article);

        logger.UserCreatedArticle(userId, article.Id);
        return new CreateArticleResponse(article.Id);
    }
}