namespace Blogplace.Web.Domain.Articles;

public class Article(string externalId, string title, string content, IEnumerable<Tag> tags)
{
    public string Id { get; } = externalId;
    public Uri Url { get; }
    public string Title { get; set; } = title;
    public string Content { get; set; } = content;
    public long Views { get; set; }
    public List<Guid> TagIds { get; set; } = [.. tags.Select(x => x.Id)];

    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
