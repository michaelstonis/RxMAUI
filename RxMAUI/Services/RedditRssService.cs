using System.Xml.Linq;

namespace RxMAUI;

public class RedditRssService
{
    private readonly HttpClient _client = new HttpClient();

    public async Task<IReadOnlyList<RssEntry>> DownloadMultipleRssAsync()
    {
        var askReddit = DownloadRssAsync("https://www.reddit.com/r/AskReddit/new/.rss");
        var todayILearned = DownloadRssAsync("https://www.reddit.com/r/todayilearned/new/.rss");
        var news = DownloadRssAsync("https://www.reddit.com/r/news/new/.rss");
        var worldNews = DownloadRssAsync("https://www.reddit.com/r/worldnews/new/.rss");

        await Task.WhenAll(askReddit, todayILearned, news, worldNews).ConfigureAwait(false);

        var masterList = new List<RssEntry>();
        masterList.AddRange(await askReddit);
        masterList.AddRange(await todayILearned);
        masterList.AddRange(await news);
        masterList.AddRange(await worldNews);

        return
            masterList
                .GroupBy(x => x.Id)
                .Select(x => x.First())
                .ToArray();
    }

    async Task<List<RssEntry>> DownloadRssAsync(string url)
    {
        var rssStream = await _client.GetStringAsync(url).ConfigureAwait(false);

        XNamespace ns = "http://www.w3.org/2005/Atom";

        var entries =
            XDocument
                .Parse(rssStream)
                .Root
                .Descendants(ns + "entry");

        var rssEntries =
            entries
                .Select(entry =>
                    new RssEntry
                    {
                        Id = entry?.Element(ns + "id")?.Value ?? string.Empty,
                        Author = entry?.Element(ns + "author")?.Element(ns + "name")?.Value ?? string.Empty,
                        Category = entry?.Element(ns + "category")?.Attribute("label")?.Value ?? string.Empty,
                        Content = entry?.Element(ns + "content")?.Value ?? string.Empty,
                        Updated = DateTimeOffset.Parse(entry?.Element(ns + "updated")?.Value).ToLocalTime().ToString("dd MMM yyyy hh:mm tt"),
                        Title = entry?.Element(ns + "title")?.Value ?? string.Empty
                    }
                )
                .OrderByDescending(rssEntry => rssEntry.Updated)
                .ToList();

        return rssEntries;
    }
}

public record RssEntry
{

    public string Id
    {
        get;
        set;
    }

    public string Author
    {
        get;
        set;
    }

    public string Category
    {
        get;
        set;
    }

    public string Content
    {
        get;
        set;
    }

    public string Updated
    {
        get;
        set;
    }

    public string Title
    {
        get;
        set;
    }

    public bool New
    {
        get;
        set;
    }
}
