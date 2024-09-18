using Blogplace.Web.Auth;
using Blogplace.Web.Commons.Consts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Blogplace.Web.Controllers.V1;

public sealed class AuthController(IOptions<CookieOptions> cookieOptions, IAuthManager authManager) : V1ControllerBase
{
    private readonly IAuthManager authManager = authManager;
    private readonly CookieOptions cookieOptions = cookieOptions.Value;

    [HttpPost]
    [AllowAnonymous]
    public Task Signin()
    {
        var token = authManager.CreateToken(Guid.NewGuid());
        this.AddCookie(AuthConsts.ACCESS_TOKEN_COOKIE, token.AccessToken);
        return Task.CompletedTask;
    }

    [HttpPost]
    [AllowAnonymous]
    public Task Signout()
    {
        this.DeleteCookie(AuthConsts.ACCESS_TOKEN_COOKIE);
        return Task.CompletedTask;
    }

    private void AddCookie(string key, string value) => this.Response.Cookies.Append(key, value, this.cookieOptions);
    private void DeleteCookie(string key) => this.Response.Cookies.Delete(key, this.cookieOptions);
    //private string GetCookieValue(string key) => this.httpContextAccessor.HttpContext!.Request.Cookies[key]!;
}
