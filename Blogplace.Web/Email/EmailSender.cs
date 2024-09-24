using Blogplace.Web.Commons.Logging;
using Blogplace.Web.Configuration;
using Blogplace.Web.Email;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Blogplace.Web.Services;

public class EmailSender(IOptions<EmailOptions> options, IEventLogger logger) : IEmailSender
{
    private readonly EmailOptions options = options.Value;

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var client = new SmtpClient()
        {
            Host = this.options.Host,
            Port = this.options.Port,
            EnableSsl = this.options.EnableSsl,
            Credentials = new NetworkCredential(this.options.User, this.options.Password)
        };

        await client.SendMailAsync(
            new MailMessage(
               from: this.options.SenderEmail,
               to: email,
               subject,
               message
               ));

        logger.EmailSent(email, subject);
    }
}
