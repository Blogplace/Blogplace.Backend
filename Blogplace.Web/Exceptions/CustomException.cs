using System.Net;

namespace Blogplace.Web.Exceptions;

public abstract class CustomException : Exception
{
    public CustomException(string? message) : base(message)
    {
    }

    public CustomException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public abstract HttpStatusCode GetStatusCode();
}
