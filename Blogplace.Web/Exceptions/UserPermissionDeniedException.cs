using System.Net;

namespace Blogplace.Web.Exceptions;

public class UserPermissionDeniedException(string? message) : CustomException(message)
{
    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Forbidden;
}