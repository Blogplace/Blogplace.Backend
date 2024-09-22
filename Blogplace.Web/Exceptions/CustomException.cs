using System.Net;

namespace Blogplace.Web.Exceptions;

public abstract class CustomException : Exception
{
    public CustomException()
    {
    }

    public CustomException(string? message) : base(message)
    {
    }

    public abstract HttpStatusCode GetStatusCode();
}
