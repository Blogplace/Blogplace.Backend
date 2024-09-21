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

public record CreateArticleResponse(Guid Id);
public record CreateArticleRequest(string Title, string Content) : IRequest<CreateArticleResponse>;
public class CreateArticleRequestHandler(IArticlesRepository repository) : IRequestHandler<CreateArticleRequest, CreateArticleResponse>
{
    public async Task<CreateArticleResponse> Handle(CreateArticleRequest request, CancellationToken cancellationToken)
    {
        var userId = Guid.NewGuid(); //todo
        var article = new Article(Guid.NewGuid(), request.Title, request.Content, userId);
        await repository.Add(article);
        return new CreateArticleResponse(article.Id);
    }
}

public record GetArticleResponse(Article article); //todo dto
public record GetArticleRequest(Guid Id) : IRequest<GetArticleResponse>;
public class GetArticleRequestHandler(IArticlesRepository repository) : IRequestHandler<GetArticleRequest, GetArticleResponse>
{
    public Task<GetArticleResponse> Handle(GetArticleRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
}

public record SearchArticlesResponse(IEnumerable<Article> articles); //todo light dtos
public record SearchArticlesRequest : IRequest<SearchArticlesResponse>; //todo filters
public class SearchArticlesRequestHandler(IArticlesRepository repository) : IRequestHandler<SearchArticlesRequest, SearchArticlesResponse>
{
    public Task<SearchArticlesResponse> Handle(SearchArticlesRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
}

public record UpdateArticleResponse;
public record UpdateArticleRequest : IRequest<UpdateArticleResponse>;
public class UpdateArticleRequestHandler(IArticlesRepository repository) : IRequestHandler<UpdateArticleRequest, UpdateArticleResponse>
{
    public Task<UpdateArticleResponse> Handle(UpdateArticleRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
}

public record DeleteArticleResponse;
public record DeleteArticleRequest : IRequest<DeleteArticleResponse>;
public class DeleteArticleRequestHandler(IArticlesRepository repository) : IRequestHandler<DeleteArticleRequest, DeleteArticleResponse>
{
    public Task<DeleteArticleResponse> Handle(DeleteArticleRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
}