using Blogplace.Web.Auth;
using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain;

public class Article(Guid id, string title, string content, Guid authorId)
{
    public Guid Id { get; } = id;
    public string Title { get; set; } = title;
    public string Content { get; set; } = content;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid AuthorId { get; } = authorId;
}

public record ArticleDto(Guid Id, string Title, string Content, DateTime CreatedAt, DateTime UpdatedAt, Guid AuthorId);
public record ArticleSmallDto(Guid Id, string Title, DateTime CreatedAt, DateTime UpdatedAt, Guid AuthorId);

public record CreateArticleResponse(Guid Id);
public record CreateArticleRequest(string Title, string Content) : IRequest<CreateArticleResponse>;
public class CreateArticleRequestHandler(ISessionStorage sessionStorage, IArticlesRepository repository) : IRequestHandler<CreateArticleRequest, CreateArticleResponse>
{
    public async Task<CreateArticleResponse> Handle(CreateArticleRequest request, CancellationToken cancellationToken)
    {
        var userId = sessionStorage.UserId;
        var article = new Article(Guid.NewGuid(), request.Title, request.Content, userId);
        await repository.Add(article);
        return new CreateArticleResponse(article.Id);
    }
}

public record GetArticleResponse(ArticleDto Article);
public record GetArticleRequest(Guid Id) : IRequest<GetArticleResponse>;
public class GetArticleRequestHandler(IArticlesRepository repository) : IRequestHandler<GetArticleRequest, GetArticleResponse>
{
    public async Task<GetArticleResponse> Handle(GetArticleRequest request, CancellationToken cancellationToken)
    {
        var result = await repository.Get(request.Id);
        var dto = new ArticleDto(result.Id, result.Title, result.Content, result.CreatedAt, result.UpdatedAt, result.AuthorId);
        return new GetArticleResponse(dto);
    }
}

public record SearchArticlesResponse(IEnumerable<ArticleSmallDto> Articles); //todo light dtos
public record SearchArticlesRequest : IRequest<SearchArticlesResponse>; //todo filters
public class SearchArticlesRequestHandler(IArticlesRepository repository) : IRequestHandler<SearchArticlesRequest, SearchArticlesResponse>
{
    public async Task<SearchArticlesResponse> Handle(SearchArticlesRequest request, CancellationToken cancellationToken)
    {
        var results = await repository.Search();
        var dtos = results.Select(x => new ArticleSmallDto(x.Id, x.Title, x.CreatedAt, x.UpdatedAt, x.AuthorId));
        return new SearchArticlesResponse(dtos);
    }
}

public record UpdateArticleRequest(Guid Id, string? NewTitle, string? NewContent) : IRequest;
public class UpdateArticleRequestHandler(ISessionStorage sessionStorage, IArticlesRepository repository) : IRequestHandler<UpdateArticleRequest>
{
    public async Task Handle(UpdateArticleRequest request, CancellationToken cancellationToken)
    {
        var isChanged = false;
        //todo get only author of article
        var article = await repository.Get(request.Id);

        if (article.AuthorId != sessionStorage.UserId)
        {
            throw new ArgumentException("Requester is not author of article");
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
            throw new ArgumentException("Requester is not author of article");
        }

        await repository.Delete(request.Id);
    }
}