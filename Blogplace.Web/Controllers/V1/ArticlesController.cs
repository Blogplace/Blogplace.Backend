using Blogplace.Web.Domain.Articles.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blogplace.Web.Controllers.V1;

public sealed class ArticlesController(IMediator mediator) : V1ControllerBase
{
    //[HttpPost]
    //public Task<CreateArticleResponse> Create(CreateArticleRequest request) => mediator.Send(request);

    //[AllowAnonymous]
    //[HttpPost]
    //public Task<GetArticleResponse> Get(GetArticleRequest request) => mediator.Send(request);

    [AllowAnonymous]
    [HttpPost]
    public Task<SearchArticlesResponse> Search(SearchArticlesRequest request) => mediator.Send(request);

    //[HttpPost]
    //public Task Update(UpdateArticleRequest request) => mediator.Send(request);

    //[HttpPost]
    //public Task Delete(DeleteArticleRequest request) => mediator.Send(request);

    //[AllowAnonymous]
    //[HttpPost]
    //public Task View(ViewArticleRequest request) => mediator.Send(request);

    //[AllowAnonymous]
    //[HttpPost]
    //public Task<SearchTagsResponse> SearchTags(SearchTagsRequest request) => mediator.Send(request);

    //[AllowAnonymous]
    //[HttpPost]
    //public Task<GetTagsByIdsResponse> GetTagsByIds(GetTagsByIdsRequest request) => mediator.Send(request);
}
