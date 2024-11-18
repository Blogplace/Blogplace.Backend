namespace Blogplace.Web.Domain.Articles;

public record ArticleSmallDto(string Id, Uri Url, string Title, long Views, IEnumerable<Guid> Tags, DateTime CreatedAt, DateTime UpdatedAt);
