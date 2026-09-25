using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace Nop.Core.Rss;

/// <summary>
/// Represents the item of RSS feed
/// </summary>
public partial class RssItem
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogError(">>> Entering RssItem(title={Title},content={Content},link={Link},id={Id},pubDate={PubDate})", title, content, link, id, pubDate);
        Title = new XElement(NopRssDefaults.Title, title);
        Content = new XElement(NopRssDefaults.Description, content);
		_logger.LogError("RssItem(title={Title},content={Content},link={Link},id={Id},pubDate={PubDate}): Content → {Content1}", title, content, link, id, pubDate, Content);
        Link = new XElement(NopRssDefaults.Link, link);
		_logger.LogError("RssItem(title={Title},content={Content},link={Link},id={Id},pubDate={PubDate}): Link → {Link1}", title, content, link, id, pubDate, Link);
        Id = new XElement(NopRssDefaults.Guid, new XAttribute("isPermaLink", false), id);
		_logger.LogError("RssItem(title={Title},content={Content},link={Link},id={Id},pubDate={PubDate}): Id → {Id1}", title, content, link, id, pubDate, Id);
        PubDate = new XElement(NopRssDefaults.PubDate, pubDate.ToString("r"));
		_logger.LogError("RssItem(title={Title},content={Content},link={Link},id={Id},pubDate={PubDate}): PubDate → {PubDate1}", title, content, link, id, pubDate, PubDate);
		_logger.LogWarning("<<< Exiting RssItem(item={Item})", item);
    }

    /// <summary>
    /// Initialize new instance of RSS feed item
    /// </summary>
    /// <param name="item">XML view of rss item</param>
    public RssItem(XContainer item)
    {
		_logger.LogError(">>> Entering RssItem(item={Item})", item);
        var title = item.Element(NopRssDefaults.Title)?.Value ?? string.Empty;
        var content = item.Element(NopRssDefaults.Content)?.Value ?? string.Empty;
			_logger.LogError("RssItem(item={Item}): content → {Content}", item, content);
        if (string.IsNullOrEmpty(content)) {
            content = item.Element(NopRssDefaults.Description)?.Value ?? string.Empty;
        }
        var link = new Uri(item.Element(NopRssDefaults.Link)?.Value ?? string.Empty);
        var pubDateValue = item.Element(NopRssDefaults.PubDate)?.Value;
		_logger.LogError("RssItem(item={Item}): link → {Link}", item, link);
        var pubDate = pubDateValue == null ? DateTimeOffset.Now : DateTimeOffset.ParseExact(pubDateValue, "r", null);
        var id = item.Element(NopRssDefaults.Guid)?.Value ?? string.Empty;
		_logger.LogError("RssItem(item={Item}): pubDateValue → {PubDateValue}", item, pubDateValue);
		_logger.LogError("RssItem(item={Item}): pubDate → {PubDate}", item, pubDate);
		_logger.LogError("RssItem(item={Item}): id → {Id}", item, id);
		_logger.LogError("RssItem(item={Item}): Content → {Content}", item, Content);
		_logger.LogError("RssItem(item={Item}): Link → {Link}", item, Link);
		_logger.LogError("RssItem(item={Item}): Id → {Id}", item, Id);

        Title = new XElement(NopRssDefaults.Title, title);
		_logger.LogError("RssItem(item={Item}): Title → {Title}", item, Title);
        Content = new XElement(NopRssDefaults.Description, content);
        Link = new XElement(NopRssDefaults.Link, link);
        Id = new XElement(NopRssDefaults.Guid, new XAttribute("isPermaLink", false), id);
        PubDate = new XElement(NopRssDefaults.PubDate, pubDate.ToString("r"));
    }

    #region Methods

    /// <summary>
    /// Get representation item of RSS feed as XElement object
    /// </summary>
    /// <returns></returns>
    public XElement ToXElement()
    {
		_logger.LogError(">>> Entering ToXElement()");
        var element = new XElement(NopRssDefaults.Item, Id, Link, Title, Content);

		_logger.LogError("ToXElement(): element → {Element}", element);
        foreach (var elementExtensions in ElementExtensions)
        {
            element.Add(elementExtensions);
        }

		_logger.LogWarning("<<< Exiting ToXElement()");
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