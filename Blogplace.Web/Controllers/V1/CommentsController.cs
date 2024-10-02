using Blogplace.Web.Domain.Comments.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Blogplace.Web.Controllers.V1;

public sealed class CommentsController(IMediator mediator) : V1ControllerBase
{
    [HttpPost]
    public Task<CreateCommentResponse> Create(CreateCommentRequest request) => mediator.Send(request);
}