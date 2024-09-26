using Blogplace.Web.Domain.Users;
using Blogplace.Web.Domain.Users.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace Blogplace.Tests.Integration.Tests;
public class UsersTests : TestBase
{
    private WebApplicationFactory<Program> _factory;

    [SetUp]
    public void SetUp() => this._factory = StartServer();

    [TearDown]
    public void TearDown() => this._factory?.Dispose();

    [Test]
    public async Task GetById_ShouldBePublic()
    {
        //Arrange
        var userClient = this._factory.CreateClient_Standard();
        var user = await this.GetMe(userClient);
        var anonymousClient = this._factory.CreateClient_Anonymous();
        var request = new GetUserByIdRequest(user.Id);

        //Act
        var response = await anonymousClient.PostAsync($"{this.urlBaseV1}/Users/GetById", request);

        //Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetUserByIdResponse>();
        result!.User.Should().BeEquivalentTo(user);
    }

    [Test]
    public async Task Update_ShouldUpdateOwnUsername()
    {
        //Arrange
        var newUsername = Guid.NewGuid().ToString();
        var userClient = this._factory.CreateClient_Standard();
        var oldUser = await this.GetMe(userClient);

        var request = new UpdateUserRequest(newUsername);

        //Act
        await userClient.PostAsync($"{this.urlBaseV1}/Users/Update", request);
        var updatedUser = await this.GetMe(userClient);

        //Assert
        oldUser.Username.Should().NotBe(newUsername);
        updatedUser.Username.Should().Be(newUsername);
    }

    private async Task<UserDto> GetMe(ApiClient client)
    {
        var response = await client.PostAsync($"{this.urlBaseV1}/Users/GetMe");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetUserMeResponse>();
        return result!.User;
    }
}
