//using Blogplace.Web.Domain.Comments;
//using Blogplace.Web.Domain.Comments.Requests;
//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace Blogplace.Web.Controllers.V1;

//public sealed class CommentsController(IMediator mediator) : V1ControllerBase
//{
//    [HttpPost]
//    public Task<CreateCommentResponse> Create(CreateCommentRequest request) => mediator.Send(request);

//    [AllowAnonymous]
//    [HttpPost]
//    public Task<SearchCommentsResponse> SearchByArticle(SearchCommentsByArticleRequest request) =>
//        mediator.Send(request);

//    [AllowAnonymous]
//    [HttpPost]
//    public Task<SearchCommentsResponse> SearchByParent(SearchCommentsByParentRequest request) =>
//        mediator.Send(request);

//    [HttpPost]
//    public Task Update(UpdateCommentRequest request) =>
//        mediator.Send(request);
    
//    [HttpPost]
//    public Task Delete(DeleteCommentRequest request) => mediator.Send(request);
//}