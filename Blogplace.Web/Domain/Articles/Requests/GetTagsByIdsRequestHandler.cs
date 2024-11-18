//using Blogplace.Web.Infrastructure.Database;
//using MediatR;

//namespace Blogplace.Web.Domain.Articles.Requests;

//public record TagDto(Guid Id, string Name);
//public record GetTagsByIdsResponse(IEnumerable<TagDto> Tags);
//public record GetTagsByIdsRequest(IEnumerable<Guid> Ids) : IRequest<GetTagsByIdsResponse>;

//public class GetTagsByIdsRequestHandler(
//    ITagsRepository tagsRepository
//) : IRequestHandler<GetTagsByIdsRequest, GetTagsByIdsResponse>
//{
//    public async Task<GetTagsByIdsResponse> Handle(GetTagsByIdsRequest request, CancellationToken cancellationToken)
//    {
//        var tags = await tagsRepository.Get(request.Ids);
//        var result = tags.Select(x => new TagDto(x.Id, x.Name));
//        return new GetTagsByIdsResponse(result);
//    }
//}
