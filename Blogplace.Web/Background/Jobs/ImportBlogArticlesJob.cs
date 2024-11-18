using Blogplace.Web.Infrastructure.Database;
using System.Xml;
using System;
using System.ServiceModel.Syndication;

namespace Blogplace.Web.Background.Jobs;

public class ImportBlogArticlesJob(IArticlesRepository articlesRepository) : IJob
{
    private readonly HttpClient client = new();

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
                await this.Import(feed, DateTime.UtcNow.Date);
            }
            catch //todo
            {
            }
        }
    }

    private async Task Import(Uri source, DateTime lastUpdateUtc)
    {
        var rssStream = await (await this.client.GetAsync(source)).Content.ReadAsStreamAsync();
        using var reader = XmlReader.Create(rssStream);
        var feed = SyndicationFeed.Load(reader);

        if (lastUpdateUtc > feed.LastUpdatedTime.UtcDateTime)
        {
            return;
        }

        foreach (var article in feed.Items.Where(x => lastUpdateUtc > x.LastUpdatedTime.UtcDateTime))
        {
            var id = article.Id;
            //todo change article model
        }
    }
}
