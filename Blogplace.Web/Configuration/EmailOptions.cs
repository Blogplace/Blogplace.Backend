namespace Blogplace.Web.Configuration;

public class EmailOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 0;
    public required string User { get; set; }
    public string Password { get; set; } = "";
    public required string SenderEmail { get; set; }
    public bool EnableSsl { get; set; } = true;
}
