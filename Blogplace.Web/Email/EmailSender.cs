using Blogplace.Web.Configuration;
using Blogplace.Web.Email;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Blogplace.Web.Services;

public class EmailSender(IOptions<EmailOptions> options) : IEmailSender
{
    private readonly EmailOptions options = options.Value;

    public Task SendEmailAsync(string email, string subject, string message)
    {
        var client = new SmtpClient()
        {
            Host = this.options.Host,
            Port = this.options.Port,
            EnableSsl = this.options.EnableSsl,
            Credentials = new NetworkCredential(this.options.User, this.options.Password)
        };

        return client.SendMailAsync(
            new MailMessage(
               from: this.options.SenderEmail,
               to: email,
               subject,
               message
               ));
    }
}
