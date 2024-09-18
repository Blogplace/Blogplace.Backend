namespace Blogplace.Web.Auth;

public class JsonWebToken
{
    public required string AccessToken { get; set; }
    public long Expiry { get; set; }
    public Guid UserId { get; set; }
    public required string Role { get; set; }
    //public required string Email { get; set; }
    public required IDictionary<string, IEnumerable<string>> Claims { get; set; }
}