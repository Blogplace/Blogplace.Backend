using Blogplace.Web.Auth;
using Blogplace.Web.Commons;
using FluentAssertions;

namespace Blogplace.Tests.Unit.Tests;

internal class PermissionsCheckerTests
{
    [Test]
    [TestCase(CommonPermissionsEnum.ArticleCreate, true)]
    [TestCase(CommonPermissionsEnum.All & ~CommonPermissionsEnum.ArticleCreate, false)]
    public void PermissionsChecker_CanCreateArticle(CommonPermissionsEnum permissions, bool expected)
    {
        var permissionsChecker = new PermissionsChecker();

        permissionsChecker
            .CanCreateArticle(permissions)
            .Should().Be(expected);
    }

    [Test]
    [TestCase(CommonPermissionsEnum.ArticleUpdate, true)]
    [TestCase(CommonPermissionsEnum.All & ~CommonPermissionsEnum.ArticleUpdate, false)]
    public void PermissionsChecker_CanUpdateArticle(CommonPermissionsEnum permissions, bool expected)
    {
        var permissionsChecker = new PermissionsChecker();

        permissionsChecker
            .CanUpdateArticle(permissions)
            .Should().Be(expected);
    }

    [Test]
    [TestCase(CommonPermissionsEnum.ArticleDelete, true)]
    [TestCase(CommonPermissionsEnum.All & ~CommonPermissionsEnum.ArticleDelete, false)]
    public void PermissionsChecker_CanDeleteArticle(CommonPermissionsEnum permissions, bool expected)
    {
        var permissionsChecker = new PermissionsChecker();

        permissionsChecker
            .CanDeleteArticle(permissions)
            .Should().Be(expected);
    }
}

