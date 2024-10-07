using Blogplace.Web.Background;
using Blogplace.Web.Infrastructure.Database;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Blogplace.Tests.Unit.Tests;
[TestFixture]
public class TagsCleaningServiceTests
{
    private const string TAGS_TO_CHECK = "TagsToCheck";
    private const string LAST_TAGS_CLEANING = "LastTagsCleaning";
    private const int SECONDS_BETWEEN_CLEANING = 15;

    private Mock<IArticlesRepository> _articlesRepositoryMock;
    private Mock<ITagsRepository> _tagsRepositoryMock;
    private IMemoryCache _cache;

    private readonly TagsCleaningChannel _channel = new();
    private TagsCleaningService? _service;

    private CancellationTokenSource _cts;
    private CancellationToken _ct;

    [SetUp]
    public void SetUp()
    {
        this._articlesRepositoryMock = new Mock<IArticlesRepository>();
        this._tagsRepositoryMock = new Mock<ITagsRepository>();

        this._cache = new ServiceCollection()
            .AddMemoryCache()
            .BuildServiceProvider()
            .GetService<IMemoryCache>()!;

        this._cts = new CancellationTokenSource();
        this._ct = this._cts.Token;
    }

    [TearDown]
    public void TearDown()
    {
        this._service!.StopAsync(this._ct).Wait();
        this._service!.Dispose();
        this._cts.Dispose();
    }

    [Test]
    [CancelAfter(3_000)]
    public async Task ShouldAddTagToWhitelist_AndNotDelete_IfTimeIsShort()
    {
        await this.SetupService();
        this.SetLastCleaning(DateTime.UtcNow);

        var id = Guid.NewGuid();
        await this._channel.Publish(id);
        await this.WaitForService();
        var tagsToCheck = this.GetTagsToCheck();
        tagsToCheck.Should().Contain(id).And.ContainSingle();

        this._articlesRepositoryMock.Verify(x => x.CountArticlesThatContainsTag(id), Times.Never);
        this._tagsRepositoryMock.Verify(x => x.Delete(id), Times.Never);

        this.GetLastCleaning().Should().BeBefore(DateTime.UtcNow.AddSeconds(-3));
    }

    [Test]
    [CancelAfter(3_000)]
    public async Task ShouldCleanIfCleaningTimeExceded()
    {
        await this.SetupService();

        this.SetLastCleaning(DateTime.UtcNow.AddSeconds(-SECONDS_BETWEEN_CLEANING - 1));
        var id = Guid.NewGuid();
        await this._channel.Publish(id);
        await this.WaitForService();

        var tagsToCheck = this.GetTagsToCheck();
        tagsToCheck.Should().BeEmpty();

        this.GetLastCleaning().Should().BeAfter(DateTime.UtcNow.AddSeconds(-3));
    }

    [Test]
    [CancelAfter(3_000)]
    public async Task ShouldDelete_WhenArticlesNotContainsTag()
    {
        this._articlesRepositoryMock.Setup(x => x.CountArticlesThatContainsTag(It.IsAny<Guid>())).Returns(Task.FromResult(0));

        await this.SetupService();

        this.SetLastCleaning(DateTime.UtcNow.AddSeconds(-SECONDS_BETWEEN_CLEANING - 1));
        var id = Guid.NewGuid();
        await this._channel.Publish(id);
        await this.WaitForService();

        this._articlesRepositoryMock.Verify(x => x.CountArticlesThatContainsTag(id), Times.Once());
        this._tagsRepositoryMock.Verify(x => x.Delete(id), Times.Once());

        this.GetLastCleaning().Should().BeAfter(DateTime.UtcNow.AddSeconds(-3));
    }

    [Test]
    [CancelAfter(3_000)]
    public async Task ShouldNotDelete_WhenArticlesContainsTag()
    {
        this._articlesRepositoryMock.Setup(x => x.CountArticlesThatContainsTag(It.IsAny<Guid>())).Returns(Task.FromResult(1));

        await this.SetupService();

        this.SetLastCleaning(DateTime.UtcNow.AddSeconds(-SECONDS_BETWEEN_CLEANING - 1));
        var id = Guid.NewGuid();
        await this._channel.Publish(id);
        await this.WaitForService();

        this._articlesRepositoryMock.Verify(x => x.CountArticlesThatContainsTag(id), Times.Once());
        this._tagsRepositoryMock.Verify(x => x.Delete(id), Times.Never);

        this.GetLastCleaning().Should().BeAfter(DateTime.UtcNow.AddSeconds(-3));
    }

    [TestCase(true, false)]
    [TestCase(false, true)]
    [CancelAfter(3_000)]
    public async Task ShouldStopOnCancellation(bool cancel, bool lastExecutionChanged)
    {
        await this.SetupService();
        if (cancel)
        {
            this._cts.Cancel();
        }

        var id = Guid.NewGuid();
        await this._channel.Publish(id);
        Thread.Sleep(500);

        if (lastExecutionChanged)
        {
            this._service!.LastExecution.Should().NotBe(default);
        }
        else
        {
            this._service!.LastExecution.Should().Be(default);
        }
    }

    private Task SetupService()
    {
        this._service = new TagsCleaningService(this._channel, this._articlesRepositoryMock.Object, this._tagsRepositoryMock.Object, this._cache);
        return this._service.StartAsync(this._ct);
    }

    private HashSet<Guid> GetTagsToCheck() => this._cache.Get<HashSet<Guid>>(TAGS_TO_CHECK)!;
    private DateTime GetLastCleaning() => this._cache.Get<DateTime>(LAST_TAGS_CLEANING)!;
    private void SetLastCleaning(DateTime time) => this._cache.Set(LAST_TAGS_CLEANING, time);

    private async Task WaitForService()
    {
        while (this._service!.LastExecution == default)
        {
            await Task.Delay(100);
        }
    }
}
