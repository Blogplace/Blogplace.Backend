using Blogplace.Web.Background;
using Blogplace.Web.Infrastructure.Database;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Blogplace.Tests.Unit.Tests;
public class TagsCleaningServiceTests
{
    private Mock<IArticlesRepository> _articlesRepositoryMock;
    private Mock<ITagsRepository> _tagsRepositoryMock;
    private IMemoryCache _memoryCache;

    private readonly TagsCleaningChannel _channel = new();
    private TagsCleaningService _service;

    [SetUp]
    public void SetUp()
    {
        this._articlesRepositoryMock = new Mock<IArticlesRepository>();
        this._tagsRepositoryMock = new Mock<ITagsRepository>();

        this._memoryCache = new ServiceCollection()
            .AddMemoryCache()
            .BuildServiceProvider()
            .GetService<IMemoryCache>()!;

        this._service = new TagsCleaningService(this._channel, this._articlesRepositoryMock.Object, this._tagsRepositoryMock.Object, this._memoryCache)
    }

    [TearDown]
    public void TearDown()
    {
        this._service.Dispose();
    }
}
