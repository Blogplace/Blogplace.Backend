using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Blogplace.Tests.Integration;
public class AuthTests : TestBase
{
    [Test]
    public async Task Signin_ShouldAddCookie()
    {
        //Arrange
        var client = this.CreateClient(withSession: false);

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Auth/Signin");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cookieHeader = response.Headers.GetValues("Set-Cookie").Single();
        cookieHeader.Should()
            .NotStartWith("__access-token=; expires=Thu, 01 Jan 1970 00:00:00 GMT;")
            .And.MatchRegex("^__access-token=([\\w-]*\\.[\\w-]*\\.[\\w-]*)") //
            .And.EndWith("; domain=localhost; path=/; secure; samesite=none; httponly");
    }

    [Test]
    public async Task Signout_ShouldRemoveCookie()
    {
        //Arrange
        var client = this.CreateClient(withSession: true); //signed in

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Auth/Signout");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cookieHeader = response.Headers.GetValues("Set-Cookie").Single();
        cookieHeader.Should().StartWith("__access-token=; expires=Thu, 01 Jan 1970 00:00:00 GMT;");
    }

    [TestCase(true, HttpStatusCode.OK)]
    [TestCase(false, HttpStatusCode.Unauthorized)]
    public async Task Protected_OnlySignedInIsAllowed(bool signedIn, HttpStatusCode statusCodeFromProtected)
    {
        var client = this.CreateClient(withSession: signedIn);
        (await client.PostAsync($"{this.urlBaseV1}/Example/Protected")).StatusCode.Should().Be(statusCodeFromProtected);
        (await client.PostAsync($"{this.urlBaseV1}/Example/AllowAnonymous")).StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
