using System.Net;

namespace Blogplace.Web.Exceptions;

public class UserNotAuthorizedException : CustomException
{
    public UserNotAuthorizedException(string? message) : base(message)
    {
    }

    public UserNotAuthorizedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
}
