//using Blogplace.Web.Domain.Comments;
//using System.Diagnostics.CodeAnalysis;

//namespace Blogplace.Web.Infrastructure.Database;

//public interface ICommentsRepository
//{
//    Task Add(Comment comment);
//    Task<Comment> Get(Guid id);
//    Task<IEnumerable<Comment>> GetByArticle(Guid articleId);
//    Task<IEnumerable<Comment>> GetByParent(Guid parentId);
//    Task Update(Comment comment);
//    Task Delete(Guid id);
//}

//[ExcludeFromCodeCoverage]
//public class CommentsRepository : ICommentsRepository
//{
//    private readonly List<Comment> _comments = [];
//    private static readonly object obj = new(); 

//    public Task Add(Comment comment)
//    {
//        lock (obj)
//        {
//            this._comments.Add(comment);
//            return Task.CompletedTask;
//        }
//    }

//    public Task<Comment> Get(Guid id)
//    {
//        lock (obj)
//        {
//            var result = this._comments.Single(x => x.Id == id);
//            return Task.FromResult(result!);
//        }
//    }

//    public Task<IEnumerable<Comment>> GetByArticle(Guid articleId)
//    {
//        lock (obj)
//        {
//            var result = this._comments.Where(x => x.ArticleId == articleId && !x.ParentId.HasValue);
//            return Task.FromResult(result);
//        }
//    }

//    public Task<IEnumerable<Comment>> GetByParent(Guid parentId)
//    {
//        lock (obj)
//        {
//            var result = this._comments.Where(x => x.ParentId == parentId);
//            return Task.FromResult(result);
//        }
//    }

//    public Task Update(Comment comment)
//    {
//        lock (obj)
//        {
//            var item = this._comments.Single(x => x.Id == comment.Id);
//            item.Content = comment.Content;
//            item.ChildCount = comment.ChildCount;
//            item.UpdatedAt = DateTime.UtcNow;
//            return Task.CompletedTask;
//        }
//    }

//    public Task Delete(Guid id)
//    {
//        lock (obj)
//        {
//            var item = this._comments.Single(x => x.Id == id);
//            this._comments.Remove(item);
//            return Task.CompletedTask;
//        }
//    }
//}