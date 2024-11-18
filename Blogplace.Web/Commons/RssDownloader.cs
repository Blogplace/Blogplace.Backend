using System.Diagnostics.CodeAnalysis;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Blogplace.Web.Commons;

public interface IRssDownloader
{
    Task<SyndicationFeed> Download(Uri source);
}

[ExcludeFromCodeCoverage]
public class RssDownloader : IRssDownloader
{
    //todo httpClient pool
    private readonly HttpClient client = new();

    public async Task<SyndicationFeed> Download(Uri source)
    {
        //todo error handling
        var rssStream = await (await this.client.GetAsync(source)).Content.ReadAsStreamAsync();
        using var reader = XmlReader.Create(rssStream);
        var feed = SyndicationFeed.Load(reader);
        return feed;
    }
}
