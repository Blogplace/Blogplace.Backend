using Blogplace.Web.Configuration;
using Blogplace.Web.Email;
using Blogplace.Web.Services;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Blogplace.Tests.Integration.Tests;
public class EmailTests : TestBase
{
    private IEmailSender emailService;
    private readonly IContainer mailpitContainer = new ContainerBuilder()
        .WithImage("axllent/mailpit")
        .WithPortBinding(1025, true)
        .WithPortBinding(8025, true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(8025)))
        .Build();

    private string mailpitApiHost;
    private int mailpitApiPort;
    private HttpClient mailpitApiClient;

    [OneTimeSetUp]
    public void SetUp()
    {
        this.mailpitContainer.StartAsync().Wait();

        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Configure<EmailOptions>(o =>
                {
                    o.Host = "localhost";
                    o.Port = this.mailpitContainer.GetMappedPublicPort(1025);
                    o.User = "test@blogplace";
                    o.Password = "";
                    o.SenderEmail = "test@blogplace";
                    o.EnableSsl = false;
                });
            });
        });
        this.emailService = factory.Services.GetService<IEmailSender>()!;

        this.mailpitApiHost = this.mailpitContainer.Hostname;
        this.mailpitApiPort = this.mailpitContainer.GetMappedPublicPort(8025);

        this.mailpitApiClient = new HttpClient
        {
            BaseAddress = new UriBuilder(Uri.UriSchemeHttp, this.mailpitApiHost, this.mailpitApiPort, "/api/v1/").Uri
        };
        this.mailpitApiClient.DefaultRequestHeaders.Accept.Clear();
        this.mailpitApiClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        this.mailpitContainer.StopAsync();
    }

    [Test]
    public async Task Send_SuccessfullySent()
    {
        var subject = "subject_" + Guid.NewGuid();
        var content = "content_" + Guid.NewGuid();
        await this.emailService.SendEmailAsync("test-receiver@host", subject, content);

        var response = await this.mailpitApiClient.GetFromJsonAsync<MessageSearchResponse>($"search?query=subject:\"{subject}\"");
        response.Should().NotBeNull();

        var messages = response?.Messages;
        messages.Should().ContainSingle().Which.Subject.Should().Be(subject);
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