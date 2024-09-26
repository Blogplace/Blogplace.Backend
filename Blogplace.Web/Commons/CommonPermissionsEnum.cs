namespace Blogplace.Web.Commons;

[Flags]
public enum CommonPermissionsEnum : int
{
    None = 0,
    
    ArticleCreate = 1 << 0,
    ArticleRead = 1 << 1,
    ArticleUpdate = 1 << 2,
    ArticleDelete = 1 << 3,

    CommentCreate = 1 << 4,
    CommentRead = 1 << 5,
    CommentUpdate = 1 << 6,
    CommentDelete = 1 << 7,

    ProfileCreate = 1 << 8,
    ProfileRead = 1 << 9,
    ProfileUpdate = 1 << 10,
    ProfileDelete = 1 << 11,
    
    All = int.MaxValue
}