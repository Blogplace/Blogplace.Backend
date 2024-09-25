using Blogplace.Web.Auth;
using Blogplace.Web.Commons;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blogplace.Tests.Unit.Tests;

internal class PermissionsCheckerTests
{
    [Test]
    [TestCase(CommonPermissionsEnum.ArticleCreate, true)]
    [TestCase(CommonPermissionsEnum.All & ~CommonPermissionsEnum.ArticleCreate, false)]
    public Task PermissionsChecker_CanCreateArticle(CommonPermissionsEnum permissions, bool expected)
    {
        var permissionsChecker = new PermissionsChecker();

        permissionsChecker
            .CanCreateArticle(permissions)
            .Should().Be(expected);
                    
        return Task.CompletedTask;
    }
}

