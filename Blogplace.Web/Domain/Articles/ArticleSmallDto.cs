namespace Blogplace.Web.Domain.Articles;

public record ArticleSmallDto(Guid Id, string Title, long Views, DateTime CreatedAt, DateTime UpdatedAt, Guid AuthorId);
