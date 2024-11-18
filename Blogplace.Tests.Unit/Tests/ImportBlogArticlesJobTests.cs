using Blogplace.Web.Background.Jobs;
using Blogplace.Web.Commons;
using Blogplace.Web.Domain.Articles;
using Blogplace.Web.Infrastructure.Database;
using Moq;
using System.ServiceModel.Syndication;

namespace Blogplace.Tests.Unit.Tests;

[TestFixture]
public  class ImportBlogArticlesJobTests
{
    [Test] //todo rewrite
    public async Task ShouldIgnoreOldArticles()
    {
        //Arrange
        var title = "title";
        var content = "content";
        var link = new Uri("https://example.com");
        var id = Guid.NewGuid().ToString();

        var articlesRepositoryMock = new Mock<IArticlesRepository>();
        var rssDownloaderMock = new Mock<IRssDownloader>();
        rssDownloaderMock.Setup(x => x.Download(It.IsAny<Uri>()))
            .ReturnsAsync(new SyndicationFeed(
                [
                    new SyndicationItem(title, content, link, id, DateTime.UtcNow)
                ]));

        var job = new ImportBlogArticlesJob(articlesRepositoryMock.Object, rssDownloaderMock.Object);

        //Act
        await job.Run();

        //Assert
        rssDownloaderMock.Verify(x => x.Download(It.IsAny<Uri>()), Times.Exactly(5)); //5 is temp
        articlesRepositoryMock.Verify(x => x.Add(It.IsAny<Article>()), Times.Exactly(5));
    }
}
