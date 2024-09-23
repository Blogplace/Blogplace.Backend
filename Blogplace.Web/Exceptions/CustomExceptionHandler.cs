using Microsoft.AspNetCore.Diagnostics;

namespace Blogplace.Web.Exceptions;

public class CustomExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> 
        TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellation)
    {
        if (exception is CustomException customException)
        {
            context.Response.StatusCode = (int)customException.GetStatusCode();
            return true;
        }

        return false;
    }
}
