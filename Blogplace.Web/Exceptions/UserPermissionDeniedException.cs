using System.Net;

namespace Blogplace.Web.Exceptions;

public class UserPermissionDeniedException : CustomException
{
    public UserPermissionDeniedException(string? message) : base(message)
    {
    }

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Forbidden;
}