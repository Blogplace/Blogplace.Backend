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

    protected CustomException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public abstract HttpStatusCode GetStatusCode();
}
