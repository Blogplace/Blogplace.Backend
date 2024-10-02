using Blogplace.Web.Domain.Comments;

namespace Blogplace.Web.Infrastructure.Database;

public interface ICommentsRepository
{
    Task Add(Comment comment);
    Task<Comment> Get(Guid id);
    Task<IEnumerable<Comment>> GetByArticle(Guid articleId);
    Task<IEnumerable<Comment>> GetByParent(Guid parentId);
    Task Delete(Guid id);
}

public class CommentsRepository : ICommentsRepository
{
    private readonly List<Comment> _comments = [];

    public Task Add(Comment comment)
    {
        this._comments.Add(comment);
        return Task.CompletedTask;
    }

    public Task<Comment> Get(Guid id)
    {
        var result = this._comments.Single(x => x.Id == id);
        return Task.FromResult(result!);
    }

    public Task<IEnumerable<Comment>> GetByArticle(Guid articleId)
    {
        var result = this._comments.Where(x => x.ArticleId == articleId && !x.ParentId.HasValue);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<Comment>> GetByParent(Guid parentId)
    {
        var result = this._comments.Where(x => x.ParentId == parentId);
        return Task.FromResult(result);
    }

    public Task Delete(Guid id)
    {
        var item = this._comments.Single(x => x.Id == id);
        this._comments.Remove(item);
        return Task.CompletedTask;
    }
}