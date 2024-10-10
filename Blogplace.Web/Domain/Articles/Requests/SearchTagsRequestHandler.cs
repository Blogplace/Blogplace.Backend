using Blogplace.Web.Auth;
using Blogplace.Web.Commons.Logging;
using Blogplace.Web.Exceptions;
using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Articles.Requests;

public record TagCount(Guid Id, string Name, int Count);
public record SearchTagsResponse(IEnumerable<TagCount> TagCounts);
public record SearchTagsRequest(string Title, string Content, string[] Tags) : IRequest<SearchTagsResponse>;
public class SearchTagsRequestHandler(
    ITagsRepository tagsRepository
) : IRequestHandler<SearchTagsRequest, SearchTagsResponse>
{
    public async Task<SearchTagsResponse> Handle(SearchTagsRequest request, CancellationToken cancellationToken)
    {
        var result = tagsRepository.
        return new SearchTagsResponse();
    }
}