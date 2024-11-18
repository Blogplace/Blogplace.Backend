using Blogplace.Web.Infrastructure.Database;
using Blogplace.Web.Domain.Articles;
using Blogplace.Web.Commons;

namespace Blogplace.Web.Background.Jobs;

public class ImportBlogArticlesJob(IArticlesRepository articlesRepository, IRssDownloader rssDownloader) : IJob
{
    private readonly Uri[] feeds = 
    [
        //todo database list
        new Uri("https://techcrunch.com/feed/"),
        new Uri("https://lifehacker.com/feed/rss"),
        new Uri("http://feeds.hbr.org/harvardbusiness"),
        new Uri("https://www.entrepreneur.com/latest.rss"),
        new Uri("https://copyblogger.com/feed/"),
    ];

    public async Task Run()
    {
        foreach (var feed in this.feeds)
        {
            try
            {
                var lastUpdate = await articlesRepository.GetLastSourceUpdate(feed);
                await this.Import(feed, lastUpdate);
            }
            catch //todo
            {
            }
        }
    }

    private async Task Import(Uri source, DateTime lastUpdateUtc)
    {
        var feed = await rssDownloader.Download(source);

        if (lastUpdateUtc > feed.LastUpdatedTime.UtcDateTime)
        {
            return;
        }

        foreach (var item in feed.Items.Where(x => x.LastUpdatedTime.UtcDateTime > lastUpdateUtc))
        {
            var id = item.Id;
            var title = item.Title?.Text ?? string.Empty;
            var content = item.Summary?.Text ?? string.Empty;
            var url = item.Links.First().Uri;
            var article = await articlesRepository.Get(id);
            if (article != null) 
            {
                article.Title = title;
                article.Content = content;
                await articlesRepository.Update(article);
            }
            else
            {
                article = new Article(id, source, url, title, content);
                await articlesRepository.Add(article);
            }
        }
    }
}
