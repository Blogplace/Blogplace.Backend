using MediatR;

namespace Blogplace.Web.Domain;

public class Article(Guid id, string title, string content)
{
    public Guid Id { get; } = id;
    public string Title { get; } = title;
    public string Content { get; } = content;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}

public record CreateArticleResponse;
public record CreateArticleRequest : IRequest<CreateArticleResponse>;
public class CreateArticleRequestHandler : IRequestHandler<CreateArticleRequest, CreateArticleResponse>
{
    public Task<CreateArticleResponse> Handle(CreateArticleRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
}

public record GetArticleResponse;
public record GetArticleRequest : IRequest<GetArticleResponse>;
public class GetArticleRequestHandler : IRequestHandler<GetArticleRequest, GetArticleResponse>
{
    public Task<GetArticleResponse> Handle(GetArticleRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
}

public record SearchArticlesResponse;
public record SearchArticlesRequest : IRequest<SearchArticlesResponse>;
public class SearchArticlesRequestHandler : IRequestHandler<SearchArticlesRequest, SearchArticlesResponse>
{
    public Task<SearchArticlesResponse> Handle(SearchArticlesRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
}

public record UpdateArticleResponse;
public record UpdateArticleRequest : IRequest<UpdateArticleResponse>;
public class UpdateArticleRequestHandler : IRequestHandler<UpdateArticleRequest, UpdateArticleResponse>
{
    public Task<UpdateArticleResponse> Handle(UpdateArticleRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
}

public record DeleteArticleResponse;
public record DeleteArticleRequest : IRequest<DeleteArticleResponse>;
public class DeleteArticleRequestHandler : IRequestHandler<DeleteArticleRequest, DeleteArticleResponse>
{
    public Task<DeleteArticleResponse> Handle(DeleteArticleRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
}