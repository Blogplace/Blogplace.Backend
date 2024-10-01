namespace Blogplace.Web.Domain.Comments;

public class Comment(Guid articleId, Guid authorId, string content, long? parentCommentId)
{
    public Guid Id { get; } = new Guid();
    public Guid ArticleId { get; } = articleId;
    public long? ParentId { get; } = parentCommentId;
    public Guid AuthorId { get; } = authorId;
    public string Content { get; } = content;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}