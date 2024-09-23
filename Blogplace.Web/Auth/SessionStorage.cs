
namespace Blogplace.Web.Auth;

public interface ISessionStorage
{
    Guid UserId { get; }
    string? Ip { get; }
    string? Referer { get; }
    string? UserAgent { get; }
    void SetupHttpContext(HttpContext context);
    void SetUserId(Guid userId);
}

public class SessionStorage : ISessionStorage
{
    public Guid UserId { get; private set; }
    public string? Ip { get; private set; }
    public string? Referer { get; private set; }
    public string? UserAgent { get; private set; }

    public void SetupHttpContext(HttpContext context)
    {
        this.Ip = context.Connection.RemoteIpAddress?.ToString();
        this.Referer = context.Request.Headers.Referer.ToString();
        this.UserAgent = context.Request.Headers.UserAgent.ToString();
    }

    public void SetUserId(Guid userId)
    {
        if (this.UserId != default)
        {
            throw new InvalidOperationException("Session user can be set only once");
        }

        this.UserId = userId;
    }
}
