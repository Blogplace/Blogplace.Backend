using Blogplace.Web.Commons.Consts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Blogplace.Web.Auth;

public static class JwtApiExtensions
{
    public static WebApplication RegisterJwt(this WebApplication app)
    {
        app.Use(async (ctx, next) =>
        {
            //if (ctx.Request.Headers.ContainsKey(AuthConsts.AUTHORIZATION_HEADER))
            //{
            //    var authenticateResult = await ctx.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
            //    if (authenticateResult is { Succeeded: true, Principal: { } principal, })
            //    {
            //        ctx.User = principal;
            //    }
            //}
            if (ctx.Request.Cookies.ContainsKey(AuthConsts.ACCESS_TOKEN_COOKIE))
            {
                var authenticateResult = await ctx.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                if (authenticateResult is { Succeeded: true, Principal: { } principal, })
                {
                    ctx.User = principal;
                }
            }
            //if (ctx.Request.Query.ContainsKey(AuthConsts.AUTHORIZATION_QUERY))
            //{
            //    var authenticateResult = await ctx.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
            //    if (authenticateResult is { Succeeded: true, Principal: { } principal, })
            //    {
            //        ctx.User = principal;
            //    }
            //}

            await next();
        });
        return app;
    }
}
