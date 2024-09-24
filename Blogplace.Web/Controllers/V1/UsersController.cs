using Blogplace.Web.Domain.Users.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blogplace.Web.Controllers.V1;

public sealed class UsersController(IMediator mediator) : V1ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public Task<GetUserByIdResponse> GetById(GetUserByIdRequest request) => mediator.Send(request);

    [HttpPost]
    public Task<GetUserMeResponse> GetMe() => mediator.Send(new GetUserMeRequest());

    [HttpPost]
    public Task Update(UpdateUserRequest request) => mediator.Send(request);
}
