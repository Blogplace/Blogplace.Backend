using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Articles.Requests;

public record SearchArticlesResponse(IEnumerable<ArticleSmallDto> Articles); //todo light dtos
public record SearchArticlesRequest : IRequest<SearchArticlesResponse>; //todo filters
public class SearchArticlesRequestHandler(IArticlesRepository repository) : IRequestHandler<SearchArticlesRequest, SearchArticlesResponse>
{
    public async Task<SearchArticlesResponse> Handle(SearchArticlesRequest request, CancellationToken cancellationToken)
    {
        var results = await repository.Search();
        var dtos = results.Select(x => new ArticleSmallDto(x.Id, x.Title, x.Views, x.TagIds, x.CreatedAt, x.UpdatedAt, x.AuthorId));
        return new SearchArticlesResponse(dtos);
    }
}
