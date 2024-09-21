using Blogplace.Web.Exceptions;

namespace Blogplace.Web;

public static class ApiExtensions
{
    public static WebApplication MapCustomException(this WebApplication app)
    {
        app.Use(async (ctx, next) =>
        {
            try
            {
                await next();
            }
            catch (CustomException ex)
            {
                ctx.Response.StatusCode = (int)ex.GetStatusCode();
            }
        });
        return app;
    }
}
