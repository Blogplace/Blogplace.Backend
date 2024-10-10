using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Comments.Requests;

public record SearchCommentsByParentRequest(Guid ParentId) : IRequest<SearchCommentsResponse>;

public class SearchCommentsByParentRequestHandler(
    ICommentsRepository repository
) : IRequestHandler<SearchCommentsByParentRequest, SearchCommentsResponse>
{
    public async Task<SearchCommentsResponse> Handle(SearchCommentsByParentRequest request,
        CancellationToken cancellationToken)
    {
        var comments = await repository.GetByParent(request.ParentId);
        var commentsDtos =
            comments.Select(x =>
                new CommentDto(x.Id, x.ArticleId, x.ParentId, x.AuthorId, x.Content, x.CreatedAt, x.UpdatedAt));
        return new SearchCommentsResponse(commentsDtos);
    }
}