using Blogplace.Web.Domain;
using FluentAssertions;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Blogplace.Tests.Integration.Tests;
public class UsersTests : TestBase
{
    [Test]
    public async Task GetById_ShouldBePublic()
    {
        //Arrange
        var client = this.CreateClient(withSession: false);
        var userClient = await this.RegisterNewUser(client);
        var user = await this.GetMe(userClient);
        var anonymousClient = client.WithoutToken();
        var request = new GetUserByIdRequest(user.Id);

        //Act
        var response = await anonymousClient.PostAsync($"{this.urlBaseV1}/Users/GetById", request);

        //Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetUserByIdResponse>();
        result!.User.Should().BeEquivalentTo(user);
    }

    private async Task<ApiClient> RegisterNewUser(ApiClient baseClient)
    {
        var clearClient = baseClient.WithoutToken();
        var response = await clearClient.GetAsync($"{this.urlBaseV1}/Auth/Signin?email=test@example.com");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var cookieHeader = response.Headers.GetValues("Set-Cookie").Single();
        var token = Regex.Match(cookieHeader, "^__access-token=(?<Token>[\\w-]*\\.[\\w-]*\\.[\\w-]*);").Groups["Token"].Value;

        var userClient = clearClient.WithDifferentToken(newToken: token);
        return userClient;
    }

    private async Task<UserDto> GetMe(ApiClient client)
    {
        var response = await client.PostAsync($"{this.urlBaseV1}/Users/GetMe");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetUserMeResponse>();
        return result!.User;
    }
}
