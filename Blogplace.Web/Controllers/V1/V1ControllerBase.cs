using Blogplace.Web.Commons.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blogplace.Web.Controllers.V1;

[ApiController]
[Authorize(Policy = AuthConsts.WEB_POLICY)]
[Route("/public/api/v1.0/[controller]/[action]")]
public class V1ControllerBase : ControllerBase
{
}
