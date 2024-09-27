using Blogplace.Web.Commons;

namespace Blogplace.Web.Auth;

public interface IPermissionsChecker
{
    bool CanCreateArticle(CommonPermissionsEnum permissions);
    bool CanReadArticle(CommonPermissionsEnum permissions);
    bool CanUpdateArticle(CommonPermissionsEnum permissions);
    bool CanDeleteArticle(CommonPermissionsEnum permissions);
}

public class PermissionsChecker : IPermissionsChecker
{
    public bool CanCreateArticle(CommonPermissionsEnum permissions) =>
        permissions.HasFlag(CommonPermissionsEnum.ArticleCreate);

    public bool CanReadArticle(CommonPermissionsEnum permissions) =>
        permissions.HasFlag(CommonPermissionsEnum.ArticleRead);

    public bool CanUpdateArticle(CommonPermissionsEnum permissions) =>
        permissions.HasFlag(CommonPermissionsEnum.ArticleUpdate);

    public bool CanDeleteArticle(CommonPermissionsEnum permissions) =>
        permissions.HasFlag(CommonPermissionsEnum.ArticleDelete);
}
