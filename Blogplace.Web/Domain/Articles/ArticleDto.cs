namespace Blogplace.Web.Domain.Articles;

public record ArticleDto(Guid Id, string Title, string Content, long Views, IEnumerable<Guid> Tags, DateTime CreatedAt, DateTime UpdatedAt, Guid AuthorId);
