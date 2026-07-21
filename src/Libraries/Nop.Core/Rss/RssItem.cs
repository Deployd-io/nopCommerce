using System.Xml.Linq;

namespace Nop.Core.Rss;

/// <summary>
/// Represents the item of RSS feed
/// </summary>
public partial class RssItem
{
    /// <summary>
    /// Initialize new instance of RSS feed item
    /// </summary>
    /// <param name="title">Title</param>
    /// <param name="content">Content</param>
    /// <param name="link">Link</param>
    /// <param name="id">Unique identifier</param>
    /// <param name="pubDate">Last build date</param>
    public RssItem(string title, string content, Uri link, string id, DateTimeOffset pubDate)
    {
        Title = new XElement(NopRssDefaults.Title, title);
        Content = new XElement(NopRssDefaults.Description, content);
        Link = new XElement(NopRssDefaults.Link, link);
        Id = new XElement(NopRssDefaults.Guid, new XAttribute("isPermaLink", false), id);
        PubDate = new XElement(NopRssDefaults.PubDate, pubDate.ToString("r"));
    }

    /// <summary>
    /// Initialize new instance of RSS feed item
    /// </summary>
    /// <param name="item">XML view of rss item</param>
    public RssItem(XContainer item)
    {
		logger.LogDebug(">>> Entering RssItem(item={Item})", item);
		logger.LogDebug("RssItem(item={Item}): Title → {Title}", item, Title);
		logger.LogDebug("RssItem(item={Item}): Content → {Content}", item, Content);
		logger.LogDebug("RssItem(item={Item}): Link → {Link}", item, Link);
        var title = item.Element(NopRssDefaults.Title)?.Value ?? string.Empty;
		logger.LogDebug("RssItem(item={Item}): title → {Title}", item, title);
        var content = item.Element(NopRssDefaults.Content)?.Value ?? string.Empty;
        if (string.IsNullOrEmpty(content))
            content = item.Element(NopRssDefaults.Description)?.Value ?? string.Empty;
			logger.LogDebug("RssItem(item={Item}): content → {Content}", item, content);
        var link = new Uri(item.Element(NopRssDefaults.Link)?.Value ?? string.Empty);
        var pubDateValue = item.Element(NopRssDefaults.PubDate)?.Value;
        var pubDate = pubDateValue == null ? DateTimeOffset.Now : DateTimeOffset.ParseExact(pubDateValue, "r", null);
		logger.LogDebug("RssItem(item={Item}): link → {Link}", item, link);
		logger.LogDebug("RssItem(item={Item}): pubDateValue → {PubDateValue}", item, pubDateValue);
		logger.LogDebug("RssItem(item={Item}): pubDate → {PubDate}", item, pubDate);
		logger.LogDebug("RssItem(item={Item}): PubDate → {PubDate}", item, PubDate);
        var id = item.Element(NopRssDefaults.Guid)?.Value ?? string.Empty;

        Title = new XElement(NopRssDefaults.Title, title);
        Content = new XElement(NopRssDefaults.Description, content);
        Link = new XElement(NopRssDefaults.Link, link);
        Id = new XElement(NopRssDefaults.Guid, new XAttribute("isPermaLink", false), id);
		logger.LogDebug("RssItem(item={Item}): id → {Id}", item, id);
		logger.LogDebug("RssItem(item={Item}): Id → {Id}", item, Id);
        PubDate = new XElement(NopRssDefaults.PubDate, pubDate.ToString("r"));
		logger.LogDebug("<<< Exiting RssItem(item={Item})", item);
    }

    #region Methods

    /// <summary>
    /// Get representation item of RSS feed as XElement object
    /// </summary>
    /// <returns></returns>
    public XElement ToXElement()
    {
		logger.LogDebug(">>> Entering ToXElement()");
        var element = new XElement(NopRssDefaults.Item, Id, Link, Title, Content);

		logger.LogDebug("ToXElement(): element → {Element}", element);
        foreach (var elementExtensions in ElementExtensions)
        {
            element.Add(elementExtensions);
        }

		logger.LogDebug("<<< Exiting ToXElement()");
        return element;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Title
    /// </summary>
    public XElement Title { get; }

    /// <summary>
    /// Get title text
    /// </summary>
    public string TitleText => Title?.Value ?? string.Empty;

    /// <summary>
    /// Content
    /// </summary>
    public XElement Content { get; }

    /// <summary>
    /// Link
    /// </summary>
    public XElement Link { get; }

    /// <summary>
    /// Get URL
    /// </summary>
    public Uri Url => new(Link.Value);

    /// <summary>
    /// Unique identifier
    /// </summary>
    public XElement Id { get; }

    /// <summary>
    /// Last build date
    /// </summary>
    public XElement PubDate { get; }

    /// <summary>
    /// Publish date
    /// </summary>
    public DateTimeOffset PublishDate => PubDate?.Value == null ? DateTimeOffset.Now : DateTimeOffset.ParseExact(PubDate.Value, "r", null);

    /// <summary>
    /// Element extensions
    /// </summary>
    public List<XElement> ElementExtensions { get; } = new List<XElement>();

    #endregion
}
