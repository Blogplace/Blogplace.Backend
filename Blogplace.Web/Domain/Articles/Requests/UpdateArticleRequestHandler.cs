﻿//using Blogplace.Web.Auth;
//using Blogplace.Web.Background;
//using Blogplace.Web.Commons.Logging;
//using Blogplace.Web.Exceptions;
//using Blogplace.Web.Infrastructure.Database;
//using MediatR;

//namespace Blogplace.Web.Domain.Articles.Requests;

//public record UpdateArticleRequest(string Id, string? NewTitle = null, string? NewContent = null, string[]? Tags = null) : IRequest;

//public class UpdateArticleRequestHandler(
//    ISessionStorage sessionStorage,
//    IArticlesRepository repository,
//    IUsersRepository usersRepository,
//    ITagsRepository tagsRepository,
//    IPermissionsChecker permissionsChecker,
//    IEventLogger logger,
//    ITagsCleaningChannel tagsCleaningChannel
//) : IRequestHandler<UpdateArticleRequest>
//{
//    public async Task Handle(UpdateArticleRequest request, CancellationToken cancellationToken)
//    {
//        var user = await usersRepository.Get(sessionStorage.UserId);
//        if (!permissionsChecker.CanUpdateArticle(user.Permissions))
//        {
//            throw new UserPermissionDeniedException("No permission to update the article");
//        }
        
//        var isChanged = false;
//        //todo get only author of article
//        var article = await repository.Get(request.Id);

//        //if (article.AuthorId != sessionStorage.UserId)
//        //{
//        //    throw new UserPermissionDeniedException("Requester is not author of article");
//        //}

//        if (request.NewTitle != null)
//        {
//            article.Title = request.NewTitle;
//            isChanged = true;
//        }

//        if (request.NewContent != null)
//        {
//            article.Content = request.NewContent;
//            isChanged = true;
//        }

//        if(request.Tags != null)
//        {
//            await tagsRepository.AddIfNotExists(request.Tags);
//            var tagsIds = (await tagsRepository.Get(request.Tags)).Select(x => x.Id).ToList();

//            var tagsDeletedFromArticle = article.TagIds.Where(x => !tagsIds.Contains(x)).ToArray();
//            article.TagIds = tagsIds;

//            foreach (var tagToCheck in tagsDeletedFromArticle)
//            {
//                //todo do only if repository.Update() succeed
//                await tagsCleaningChannel.Publish(tagToCheck);
//            }

//            isChanged = true;
//        }

//        if (isChanged)
//        {
//            await repository.Update(article);
//            logger.UserUpdatedArticle(sessionStorage.UserId, article.Id);
//        }
//    }
//}
