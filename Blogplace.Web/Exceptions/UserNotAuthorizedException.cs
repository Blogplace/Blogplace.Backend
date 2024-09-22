using System.Net;

namespace Blogplace.Web.Exceptions;

public class UserNotAuthorizedException : CustomException
{
    public UserNotAuthorizedException()
    {
    }

    public UserNotAuthorizedException(string? message) : base(message)
    {
    }

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
}
