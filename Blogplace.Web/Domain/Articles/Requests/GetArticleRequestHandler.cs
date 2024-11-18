using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Articles.Requests;

public record GetArticleResponse(ArticleDto Article, Guid ViewId);
public record GetArticleRequest(string Id) : IRequest<GetArticleResponse>;

public class GetArticleRequestHandler(
    IArticlesRepository repository
    //IMemoryCache cache,
    //ISessionStorage sessionStorage
) : IRequestHandler<GetArticleRequest, GetArticleResponse>
{
    public async Task<GetArticleResponse> Handle(GetArticleRequest request, CancellationToken cancellationToken)
    {
        var result = await repository.Get(request.Id);
        var dto = new ArticleDto(result.Id, result.Url, result.Title, result.Content, result.Views, result.TagIds, result.CreatedAt, result.UpdatedAt);

        //var viewId = Guid.NewGuid();
        //var viewData = new ArticleViewData(sessionStorage.Ip!, DateTime.UtcNow, result.Id, sessionStorage.UserAgent!);
        //cache.Set($"ArticleView_{viewId}", viewData);

        return new GetArticleResponse(dto, Guid.NewGuid()); //new guid is temp
    }
}
