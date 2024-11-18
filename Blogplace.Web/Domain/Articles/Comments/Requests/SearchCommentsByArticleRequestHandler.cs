//using Blogplace.Web.Infrastructure.Database;
//using MediatR;

//namespace Blogplace.Web.Domain.Comments.Requests;

//public record SearchCommentsByArticleRequest(Guid ArticleId) : IRequest<SearchCommentsResponse>;

//// TODO: Send response with a list of comments with author name and author icon url additional properties
//public class SearchCommentsByArticleRequestHandler(
//    ICommentsRepository repository
//) : IRequestHandler<SearchCommentsByArticleRequest, SearchCommentsResponse>
//{
//    public async Task<SearchCommentsResponse> Handle(SearchCommentsByArticleRequest request,
//        CancellationToken cancellationToken)
//    {
//        var comments = await repository.GetByArticle(request.ArticleId);
//        var commentsDtos =
//            comments.Select(x =>
//                new CommentDto(x.Id, x.ArticleId, x.ParentId, x.AuthorId, x.Content, x.CreatedAt, x.UpdatedAt));
//        return new SearchCommentsResponse(commentsDtos);
//    }
//}