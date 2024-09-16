using Blogplace.Web;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Blogplace.Tests.Integration;
public class ExampleTests : TestBase
{
    [Test]
    public async Task WeatherForecast_ShouldReturnWeathers()
    {
        //Arrange
        var client = this.CreateClient();

        //Act
        var response = await client.GetAsync("/WeatherForecast");

        //Assert
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>();
        result!.Should().NotBeNullOrEmpty();
    }
}
