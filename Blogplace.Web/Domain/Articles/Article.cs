namespace Blogplace.Web.Domain.Articles;

public class Article(string title, string content, Guid authorId)
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Title { get; set; } = title;
    public string Content { get; set; } = content;
    public long Views { get; set; }
    public List<Guid> TagIds { get; set; }

    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid AuthorId { get; } = authorId;
}
