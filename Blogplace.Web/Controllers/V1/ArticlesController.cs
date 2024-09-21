using Blogplace.Web.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Blogplace.Web.Controllers.V1;

public sealed class ArticlesController(IMediator mediator) : V1ControllerBase
{
    public Task<CreateArticleResponse> Create(CreateArticleRequest request) => mediator.Send(request);

    [AllowAnonymous]
    public Task<GetArticleResponse> Get(GetArticleRequest request) => mediator.Send(request);

    [AllowAnonymous]
    public Task<SearchArticlesResponse> Search(SearchArticlesRequest request) => mediator.Send(request);

    public Task Update(UpdateArticleRequest request) => mediator.Send(request);

    public Task Delete(DeleteArticleRequest request) => mediator.Send(request);
}
