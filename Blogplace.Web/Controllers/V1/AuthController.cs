using Blogplace.Web.Auth;
using Blogplace.Web.Commons.Consts;
using Blogplace.Web.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Blogplace.Web.Controllers.V1;

public sealed class AuthController(IOptions<CookieOptions> cookieOptions, IAuthManager authManager, IMediator mediator) : V1ControllerBase
{
    private readonly IAuthManager authManager = authManager;
    private readonly CookieOptions cookieOptions = cookieOptions.Value;

    [AllowAnonymous]
    [HttpGet]
    public async Task Signin(string email)//todo token
    {
        var userId = (await mediator.Send(new GetUserByEmailRequest(email))).User?.Id 
            ?? (await mediator.Send(new CreateUserRequest(email))).Id;

        var token = this.authManager.CreateToken(userId, AuthConsts.ROLE_WEB);
        this.AddCookie(AuthConsts.ACCESS_TOKEN_COOKIE, token.AccessToken);
    }

    [AllowAnonymous]
    [HttpPost]
    public Task Signout()
    {
        this.DeleteCookie(AuthConsts.ACCESS_TOKEN_COOKIE);
        return Task.CompletedTask;
    }

    private void AddCookie(string key, string value) => this.Response.Cookies.Append(key, value, this.cookieOptions);
    private void DeleteCookie(string key) => this.Response.Cookies.Delete(key, this.cookieOptions);
    //private string GetCookieValue(string key) => this.httpContextAccessor.HttpContext!.Request.Cookies[key]!;
}
