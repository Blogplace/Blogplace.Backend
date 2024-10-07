using Blogplace.Web.Background;
using Blogplace.Web.Infrastructure.Database;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Blogplace.Tests.Unit.Tests;
public class TagsCleaningServiceTests
{
    private const string TAGS_TO_CHECK = "TagsToCheck";
    private const string LAST_TAGS_CLEANING = "LastTagsCleaning";
    private const int SECONDS_BETWEEN_CLEANING = 15;

    private Mock<IArticlesRepository> _articlesRepositoryMock;
    private Mock<ITagsRepository> _tagsRepositoryMock;
    private IMemoryCache _cache;

    private readonly TagsCleaningChannel _channel = new();
    private TagsCleaningService _service;

    private readonly CancellationToken _ct = new ();

    [SetUp]
    public void SetUp()
    {
        this._articlesRepositoryMock = new Mock<IArticlesRepository>();
        this._tagsRepositoryMock = new Mock<ITagsRepository>();

        this._cache = new ServiceCollection()
            .AddMemoryCache()
            .BuildServiceProvider()
            .GetService<IMemoryCache>()!;

        this._service = new TagsCleaningService(this._channel, this._articlesRepositoryMock.Object, this._tagsRepositoryMock.Object, this._cache);
        this._service.StartAsync(this._ct);
    }

    [TearDown]
    public void TearDown()
    {
        this._service.StopAsync(this._ct).Wait();
        this._service.Dispose();
    }

    [Test]
    public async Task ShouldAddTagToWhitelist_IfTimeIsShort()
    {
        var id = Guid.NewGuid();
        await this._channel.Publish(id);
        await this.WaitForService();
        var tagsToCheck = this.GetTagsToCheck();
        tagsToCheck.Should().Contain(id).And.ContainSingle();
    }

    private HashSet<Guid> GetTagsToCheck() => this._cache.Get<HashSet<Guid>>(TAGS_TO_CHECK)!;
    private DateTime GetLastCleaning() => this._cache.Get<DateTime>(LAST_TAGS_CLEANING)!;
    private async Task WaitForService()
    {
        while (this._service.LastExecution == default)
        {
            await Task.Delay(100);
        }
    }
}
