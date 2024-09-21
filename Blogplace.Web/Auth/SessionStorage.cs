namespace Blogplace.Web.Auth;

public interface ISessionStorage
{
    Guid UserId { get; }

    void SetUserId(Guid userId);
}

public class SessionStorage : ISessionStorage
{
    public Guid UserId { get; private set; }

    public void SetUserId(Guid userId)
    {
        if (this.UserId != default)
        {
            throw new InvalidOperationException("Session user can be set only once");
        }

        this.UserId = userId;
    }
}
