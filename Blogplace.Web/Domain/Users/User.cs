using System.Text.RegularExpressions;

namespace Blogplace.Web.Domain;

public class User(string email)
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Email { get; } = email;
    //Default username = part of email before last @ sign
    //testuser@example.com => testuser
    public string Username { get; set; } = Regex.Match(email, @"^(?<Name>.*)@[^@]+$").Groups["Name"].Value;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
