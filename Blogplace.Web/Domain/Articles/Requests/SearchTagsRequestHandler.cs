using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Articles.Requests;

public record TagCount(Guid Id, string Name, int Count);
public record SearchTagsResponse(IEnumerable<TagCount> TagCounts);
public record SearchTagsRequest(string? ContainsName) : IRequest<SearchTagsResponse>;
public class SearchTagsRequestHandler(
    ITagsRepository tagsRepository
) : IRequestHandler<SearchTagsRequest, SearchTagsResponse>
{
    //todo config
    private const int SEARCH_LIMIT = 5;

    public async Task<SearchTagsResponse> Handle(SearchTagsRequest request, CancellationToken cancellationToken)
    {
        var data = await tagsRepository.SearchTopTags(SEARCH_LIMIT, request.ContainsName);
        var result = data.Select(x => new TagCount(x.Key.Id, x.Key.Name, x.Value));
        return new SearchTagsResponse(result);
    }
}