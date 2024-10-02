namespace Blogplace.Web.Domain.Comments;

public class Comment(Guid articleId, Guid authorId, string content, Guid? parentCommentId)
{
    public Guid Id { get; } = new Guid();
    public Guid ArticleId { get; } = articleId;
    public Guid? ParentId { get; } = parentCommentId;
    public Guid AuthorId { get; } = authorId;
    public string Content { get; } = content;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}