using Blogplace.Web.Auth;

namespace Blogplace.Tests.Unit.Tests;

[TestFixture]
public class SessionStorageTests
{
    [Test]
    public void SetUserId_CanBeUsedOnlyOnce()
    {
        var storage = new SessionStorage();
        storage.SetUserId(Guid.NewGuid());
        Assert.Throws<InvalidOperationException>(() => storage.SetUserId(Guid.NewGuid()));
    }
}
