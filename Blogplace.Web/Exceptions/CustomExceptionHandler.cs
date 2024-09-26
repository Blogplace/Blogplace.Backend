using Microsoft.AspNetCore.Diagnostics;

namespace Blogplace.Web.Exceptions;

public class CustomExceptionHandler : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellation)
    {
        if (exception is CustomException customException)
        {
            context.Response.StatusCode = (int)customException.GetStatusCode();
            return new ValueTask<bool>(true);
        }

        return new ValueTask<bool>(false);
    }
}
