using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blogplace.Web.Controllers.V1;

//to delete in future
public class ExampleController : V1ControllerBase
{
    [HttpPost]
    public Task Protected() => Task.CompletedTask;

    [AllowAnonymous]
    [HttpPost]
    public Task AllowAnonymous() => Task.CompletedTask;
}
