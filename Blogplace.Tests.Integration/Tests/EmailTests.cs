using Blogplace.Web.Configuration;
using Blogplace.Web.Email;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Blogplace.Tests.Integration.Tests;
public class EmailTests : TestBase
{
    private IContainer mailpitContainer;
    private WebApplicationFactory<Program> _factory;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        this.mailpitContainer = new ContainerBuilder()
            .WithImage("axllent/mailpit")
            .WithPortBinding(1025, true)
            .WithPortBinding(8025, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(8025)))
            .Build();
        await this.mailpitContainer.StartAsync();

        this._factory = StartServer(x => x.Configure<EmailOptions>(o =>
            {
                o.Host = "localhost";
                o.Port = this.mailpitContainer.GetMappedPublicPort(1025);
                o.User = "test@blogplace";
                o.Password = "";
                o.SenderEmail = "test@blogplace";
                o.EnableSsl = false;
            }));
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        this._factory?.Dispose();
        if (this.mailpitContainer != null)
        {
            await this.mailpitContainer.StopAsync();
        }
    }

    [Test]
    public async Task Send_SuccessfullySent()
    {
        //Arrange
        var emailService = this._factory.Services.GetService<IEmailSender>()!;
        var client = this.CreateMailpitClient();
        var subject = "subject_" + Guid.NewGuid();
        var content = "content_" + Guid.NewGuid();

        //Act
        await emailService.SendEmailAsync("test-receiver@host", subject, content);
        var response = await client.GetFromJsonAsync<MessageSearchResponse>($"search?query=subject:\"{subject}\"");

        //Assert
        response.Should().NotBeNull();
        response?.Messages.Should().ContainSingle().Which.Subject.Should().Be(subject);
    }

    private HttpClient CreateMailpitClient()
    {
        var mailpitApiHost = this.mailpitContainer.Hostname;
        var mailpitApiPort = this.mailpitContainer.GetMappedPublicPort(8025);
        var mailpitApiClient = new HttpClient
        {
            BaseAddress = new UriBuilder(Uri.UriSchemeHttp, mailpitApiHost, mailpitApiPort, "/api/v1/").Uri
        };
        mailpitApiClient.DefaultRequestHeaders.Accept.Clear();
        mailpitApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        return mailpitApiClient;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public class MessageSearchResponse
    {
        public int MessagesCount { get; set; }
        public int Start { get; set; }
        public List<string> Tags { get; set; }
        public int Total { get; set; }
        public int Unread { get; set; }
        public List<EmailMessage> Messages { get; set; }
    }

    public class EmailMessage
    {
        public int Attachments { get; set; }
        public List<Recipient> Bcc { get; set; }
        public List<Recipient> Cc { get; set; }
        public DateTime Created { get; set; }
        public Sender From { get; set; }
        public string ID { get; set; }
        public string MessageID { get; set; }
        public bool Read { get; set; }
        public List<Recipient> ReplyTo { get; set; }
        public int Size { get; set; }
        public string Snippet { get; set; }
        public string Subject { get; set; }
        public List<string> Tags { get; set; }
        public List<Recipient> To { get; set; }
    }

    public class Recipient
    {
        public string Address { get; set; }
        public string Name { get; set; }
    }

    public class Sender
    {
        public string Address { get; set; }
        public string Name { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}