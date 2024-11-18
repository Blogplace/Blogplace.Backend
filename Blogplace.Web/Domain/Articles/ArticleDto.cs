namespace Blogplace.Web.Domain.Articles;

public record ArticleDto(string Id, Uri Url, string Title, string Content, long Views, IEnumerable<Guid> Tags, DateTime CreatedAt, DateTime UpdatedAt);
