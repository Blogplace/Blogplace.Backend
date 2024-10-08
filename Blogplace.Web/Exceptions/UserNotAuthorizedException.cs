using System.Net;

namespace Blogplace.Web.Exceptions;

public class UserNotAuthorizedException(string? message) : CustomException(message)
{
    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
}