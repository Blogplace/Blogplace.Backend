using Blogplace.Web.Auth;
using Blogplace.Web.Exceptions;
using Blogplace.Web.Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Blogplace.Web.Domain;

public class Article(string title, string content, Guid authorId)
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Title { get; set; } = title;
    public string Content { get; set; } = content;
    public long Views { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid AuthorId { get; } = authorId;
}

public record ArticleDto(Guid Id, string Title, string Content, long Views, DateTime CreatedAt, DateTime UpdatedAt, Guid AuthorId);
public record ArticleSmallDto(Guid Id, string Title, long Views, DateTime CreatedAt, DateTime UpdatedAt, Guid AuthorId);

public record CreateArticleResponse(Guid Id);
public record CreateArticleRequest(string Title, string Content) : IRequest<CreateArticleResponse>;
public class CreateArticleRequestHandler(ISessionStorage sessionStorage, IArticlesRepository repository) : IRequestHandler<CreateArticleRequest, CreateArticleResponse>
{
    public async Task<CreateArticleResponse> Handle(CreateArticleRequest request, CancellationToken cancellationToken)
    {
        var userId = sessionStorage.UserId;
        var article = new Article(request.Title, request.Content, userId);
        await repository.Add(article);
        return new CreateArticleResponse(article.Id);
    }
}

public record GetArticleResponse(ArticleDto Article, Guid ViewId);
public record GetArticleRequest(Guid Id) : IRequest<GetArticleResponse>;
public class GetArticleRequestHandler(IArticlesRepository repository, IMemoryCache cache, ISessionStorage sessionStorage) : IRequestHandler<GetArticleRequest, GetArticleResponse>
{
    public async Task<GetArticleResponse> Handle(GetArticleRequest request, CancellationToken cancellationToken)
    {
        var result = await repository.Get(request.Id);
        var dto = new ArticleDto(result.Id, result.Title, result.Content, result.Views, result.CreatedAt, result.UpdatedAt, result.AuthorId);

        var viewId = Guid.NewGuid();
        var viewData = new ArticleViewData(sessionStorage.Ip!, DateTime.UtcNow, result.Id, sessionStorage.UserAgent!);
        cache.Set($"ArticleView_{viewId}", viewData);

        return new GetArticleResponse(dto, viewId);
    }
}

public record SearchArticlesResponse(IEnumerable<ArticleSmallDto> Articles); //todo light dtos
public record SearchArticlesRequest : IRequest<SearchArticlesResponse>; //todo filters
public class SearchArticlesRequestHandler(IArticlesRepository repository) : IRequestHandler<SearchArticlesRequest, SearchArticlesResponse>
{
    public async Task<SearchArticlesResponse> Handle(SearchArticlesRequest request, CancellationToken cancellationToken)
    {
        var results = await repository.Search();
        var dtos = results.Select(x => new ArticleSmallDto(x.Id, x.Title, x.Views, x.CreatedAt, x.UpdatedAt, x.AuthorId));
        return new SearchArticlesResponse(dtos);
    }
}

public record UpdateArticleRequest(Guid Id, string? NewTitle = null, string? NewContent = null) : IRequest;
public class UpdateArticleRequestHandler(ISessionStorage sessionStorage, IArticlesRepository repository) : IRequestHandler<UpdateArticleRequest>
{
    public async Task Handle(UpdateArticleRequest request, CancellationToken cancellationToken)
    {
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
        }
    }
}

public record DeleteArticleRequest(Guid Id) : IRequest;
public class DeleteArticleRequestHandler(ISessionStorage sessionStorage, IArticlesRepository repository) : IRequestHandler<DeleteArticleRequest>
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
    }
}

public record ArticleViewData(string Ip, DateTime DateTime, Guid PostId, string UserAgent);
public record ViewArticleRequest(Guid ViewId) : IRequest;
public class ViewArticleRequestHandler(IArticlesRepository repository, ISessionStorage sessionStorage, IMemoryCache cache) : IRequestHandler<ViewArticleRequest>
{
    public async Task Handle(ViewArticleRequest request, CancellationToken cancellationToken)
    {
        var referer = sessionStorage.Referer!.Trim(" /".ToCharArray());
        var uri = new Uri(referer);
        var postIdText = Regex.Match(uri.AbsolutePath, @"/(?<PostId>[^/]*)$").Groups["PostId"].Value;
        var postId = Guid.Parse(postIdText);

        var key = $"ArticleView_{request.ViewId}";
        var cacheValue = cache.Get<ArticleViewData>($"ArticleView_{request.ViewId}")!;

        if (!CompareHttpContext(cacheValue, sessionStorage.Ip!, sessionStorage.UserAgent!)
            || !CompareDateTime(cacheValue)
            || !ComparePostId(cacheValue, postId))
        {
            cache.Remove(key);
            throw new UserNotAuthorizedException(string.Empty);
        }

        var post = await repository.Get(postId);
        post.Views++;
        await repository.Update(post);
        cache.Remove(key);
    }

    private static bool CompareHttpContext(ArticleViewData data, string ip, string userAgent) 
        => data.Ip == ip && data.UserAgent == userAgent;

    private static bool CompareDateTime(ArticleViewData data)
        => data.DateTime <= DateTime.UtcNow.AddSeconds(-3);

    private static bool ComparePostId(ArticleViewData data, Guid postId)
        => data.PostId == postId;
}