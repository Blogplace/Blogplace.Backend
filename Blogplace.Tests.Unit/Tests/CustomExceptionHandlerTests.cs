using Blogplace.Web.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Blogplace.Tests.Unit.Tests;
internal class CustomExceptionHandlerTests
{
    [Test]
    public async Task CustomExceptionHandler_TryHandleAsync_ReturnsTrue()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var customException = new TestCustomException();
        var handler = new CustomExceptionHandler();

        // Act
        var result = await handler.TryHandleAsync(context, customException, CancellationToken.None);

        // Assert
        _ = result.Should().BeTrue();
        _ = context.Response.StatusCode.Should().Be((int)customException.GetStatusCode());
    }

    [Test]
    public async Task CustomExceptionHandler_TryHandleAsync_ReturnsFalse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var randomException = new Exception();
        var handler = new CustomExceptionHandler();

        // Act
        var result = await handler.TryHandleAsync(context, randomException, CancellationToken.None);

        // Assert
        _ = result.Should().BeFalse();
    }

    private class TestCustomException : CustomException
    {
        public TestCustomException()
        {
        }

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
    }
}
