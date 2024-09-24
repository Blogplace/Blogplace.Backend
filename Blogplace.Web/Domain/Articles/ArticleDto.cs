namespace Blogplace.Web.Domain.Articles;

public record ArticleDto(Guid Id, string Title, string Content, long Views, DateTime CreatedAt, DateTime UpdatedAt, Guid AuthorId);
