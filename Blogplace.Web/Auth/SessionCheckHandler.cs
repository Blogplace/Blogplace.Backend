using Blogplace.Web.Commons.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Blogplace.Web.Auth;

public class SessionCheckHandler(IHttpContextAccessor contextAccessor)
    : AuthorizationHandler<SessionCheckRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SessionCheckRequirement requirement)
    {
        var request = contextAccessor.HttpContext?.Request;
        if (request == null)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var accessToken = request!.Cookies[AuthConsts.ACCESS_TOKEN_COOKIE];
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            context.Fail();
            return Task.CompletedTask;
        }
        //var session = this.usersSessionsStorage.Get(accessToken!);
        //if (session == null)
        //{
        //    context.Fail();
        //    return Task.CompletedTask;
        //}
        //if (session.UserAgent != request.Headers[AuthConsts.USER_AGENT_HEADER])
        //{
        //    context.Fail();
        //    return Task.CompletedTask;
        //}
        //if (session.IpAddress != request.HttpContext.Connection.RemoteIpAddress!.ToString())
        //{
        //    context.Fail();
        //    return Task.CompletedTask;
        //}
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}