using Blogplace.Web.Background.Jobs;
using Blogplace.Web.Infrastructure.Database;
using Moq;

namespace Blogplace.Tests.Unit.Tests;

[TestFixture]
public  class ImportBlogArticlesJobTests
{
    public async Task ShouldIgnoreOldArticles()
    {
        //Arrange
        var articlesRepositoryMock = new Mock<IArticlesRepository>();
        var job = new ImportBlogArticlesJob(articlesRepositoryMock.Object);

        //Act


        //Assert
    }
}
