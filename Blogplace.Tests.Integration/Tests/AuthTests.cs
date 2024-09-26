using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Blogplace.Tests.Integration.Tests;
public class AuthTests : TestBase
{
    private WebApplicationFactory<Program> _factory;

    [OneTimeSetUp]
    public void OneTimeSetUp() => this._factory = StartServer();

    [OneTimeTearDown]
    public void OneTimeTearDown() => this._factory?.Dispose();

    [Test]
    public async Task Signin_ShouldAddCookie()
    {
        //Arrange
        var client = this._factory.CreateClient_Anonymous();

        //Act
        var response = await client.GetAsync($"{this.urlBaseV1}/Auth/Signin?email=test@example.com");

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
        var client = this._factory.CreateClient_Standard(); //signed in

        //Act
        var response = await client.PostAsync($"{this.urlBaseV1}/Auth/Signout");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cookieHeader = response.Headers.GetValues("Set-Cookie").Single();
        cookieHeader.Should().StartWith("__access-token=; expires=Thu, 01 Jan 1970 00:00:00 GMT;");
    }
}
