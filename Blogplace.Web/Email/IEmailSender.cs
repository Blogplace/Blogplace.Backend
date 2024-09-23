namespace Blogplace.Web.Email;

public interface IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message);
}
