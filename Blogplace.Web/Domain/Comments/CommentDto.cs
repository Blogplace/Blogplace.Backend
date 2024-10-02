namespace Blogplace.Web.Domain.Comments;

// TODO: Replace GUID (where possible) with numerical ID for indexing optimization.
// TODO: Add Upvotes system
public record CommentDto(
    Guid Id,
    Guid ArticleId,
    Guid? ParentId,
    Guid AuthorId,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt
    );