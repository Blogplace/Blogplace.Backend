using Blogplace.Web.Domain.Comments;
using Blogplace.Web.Infrastructure.Database;

namespace Blogplace.Tests.Integration.Data;

public class CommentsRepositoryFake : ICommentsRepository
{
    public static Comment? StdUserCommentOnStdUserArticle { get; set; }

    public List<Comment> Comments { get; private set; } = [];

    private static readonly object obj = new();

    public void Init()
    {
        lock (obj)
        {
            StdUserCommentOnStdUserArticle ??=
                new Comment(ArticlesRepositoryFake.StandardUserArticle!.Id, UsersRepositoryFake.Standard!.Id,
                    "TEST_COMMENT_CONTENT", null);
        }

        this.Comments =
        [
            StdUserCommentOnStdUserArticle
        ];
    }

    public Task Add(Comment comment)
    {
        lock (obj)
        {
            this.Comments.Add(comment);
            return Task.CompletedTask;
        }
    }

    public Task<Comment> Get(Guid id)
    {
        lock (obj)
        {
            var result = this.Comments.Single(x => x.Id == id);
            return Task.FromResult(result!);
        }
        
    }

    public Task<IEnumerable<Comment>> GetByArticle(Guid articleId)
    {
        lock (obj)
        {
            var result = this.Comments.Where(x => x.ArticleId == articleId && !x.ParentId.HasValue);
            return Task.FromResult(result);
        }
    }

    public Task<IEnumerable<Comment>> GetByParent(Guid parentId)
    {
        lock (obj)
        {
            var result = this.Comments.Where(x => x.ParentId == parentId);
            return Task.FromResult(result);
        }
    }

    public Task Update(Comment comment)
    {
        lock (obj)
        {
            var item = this.Comments.Single(x => x.Id == comment.Id);
            item.Content = comment.Content;
            item.ChildCount = comment.ChildCount;
            item.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }
    }

    public Task Delete(Guid id)
    {
        lock (obj)
        {
            var item = this.Comments.Single(x => x.Id == id);
            this.Comments.Remove(item);
            return Task.CompletedTask;
        }
    }
}