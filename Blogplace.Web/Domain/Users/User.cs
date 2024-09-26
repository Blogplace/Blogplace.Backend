using Blogplace.Web.Commons;
using System.Text.RegularExpressions;

namespace Blogplace.Web.Domain.Users;

public class User(string email, CommonPermissionsEnum permissions, Guid? userId = null)
{
    public Guid Id { get; } = userId ?? Guid.NewGuid();
    public string Email { get; } = email;
    //Default username = part of email before last @ sign
    //testuser@example.com => testuser
    public string Username { get; set; } = Regex.Match(email, @"^(?<Name>.*)@[^@]+$").Groups["Name"].Value;
    public CommonPermissionsEnum Permissions { get; set; } = permissions;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
