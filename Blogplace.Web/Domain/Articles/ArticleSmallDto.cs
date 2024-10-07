namespace Blogplace.Web.Domain.Articles;

public record ArticleSmallDto(Guid Id, string Title, long Views, IEnumerable<Guid> Tags, DateTime CreatedAt, DateTime UpdatedAt, Guid AuthorId);
