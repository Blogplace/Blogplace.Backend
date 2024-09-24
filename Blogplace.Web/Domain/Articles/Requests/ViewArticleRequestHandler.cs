using Blogplace.Web.Auth;
using Blogplace.Web.Commons.Logging;
using Blogplace.Web.Exceptions;
using Blogplace.Web.Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;

namespace Blogplace.Web.Domain.Articles.Requests;

public record ArticleViewData(string Ip, DateTime DateTime, Guid PostId, string UserAgent);
public record ViewArticleRequest(Guid ViewId) : IRequest;
public class ViewArticleRequestHandler(IArticlesRepository repository, ISessionStorage sessionStorage, IMemoryCache cache, IEventLogger logger) : IRequestHandler<ViewArticleRequest>
{
    public async Task Handle(ViewArticleRequest request, CancellationToken cancellationToken)
    {
        var referer = sessionStorage.Referer!.Trim(" /".ToCharArray());
        var uri = new Uri(referer);
        var articleIdText = Regex.Match(uri.AbsolutePath, @"/(?<ArticleId>[^/]*)$").Groups["ArticleId"].Value;
        var articleId = Guid.Parse(articleIdText);

        var key = $"ArticleView_{request.ViewId}";
        var cacheValue = cache.Get<ArticleViewData>($"ArticleView_{request.ViewId}")!;

        if (!CompareHttpContext(cacheValue, sessionStorage.Ip!, sessionStorage.UserAgent!)
            || !CompareDateTime(cacheValue)
            || !ComparePostId(cacheValue, articleId))
        {
            cache.Remove(key);
            throw new UserNotAuthorizedException(string.Empty);
        }

        var article = await repository.Get(articleId);
        article.Views++;
        await repository.Update(article);
        cache.Remove(key);

        logger.UserViewedArticle(sessionStorage.UserId, articleId);
    }

    private static bool CompareHttpContext(ArticleViewData data, string ip, string userAgent)
        => data.Ip == ip && data.UserAgent == userAgent;

    private static bool CompareDateTime(ArticleViewData data)
        => data.DateTime <= DateTime.UtcNow.AddSeconds(-3);

    private static bool ComparePostId(ArticleViewData data, Guid articleId)
        => data.PostId == articleId;
}