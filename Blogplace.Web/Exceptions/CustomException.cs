using System.Net;

namespace Blogplace.Web.Exceptions;

public abstract class CustomException : Exception
{
    protected CustomException()
    {
    }

    protected CustomException(string? message) : base(message)
    {
    }

    public abstract HttpStatusCode GetStatusCode();
}
