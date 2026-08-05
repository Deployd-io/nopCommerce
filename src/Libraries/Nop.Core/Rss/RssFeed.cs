using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace Nop.Core.Rss;

/// <summary>
/// Represents the RSS feed
/// </summary>
public partial class RssFeed
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Initialize new instance of RSS feed
    /// </summary>
    /// <param name="title">Title</param>
    /// <param name="description">Description</param>
    /// <param name="link">Link</param>
    /// <param name="lastBuildDate">Last build date</param>
    public RssFeed(string title, string description, Uri link, DateTimeOffset lastBuildDate)
    {
        Title = new XElement(NopRssDefaults.Title, title);
        Description = new XElement(NopRssDefaults.Description, description);
        Link = new XElement(NopRssDefaults.Link, link);
        LastBuildDate = new XElement(NopRssDefaults.LastBuildDate, lastBuildDate.ToString("r"));
    }

    /// <summary>
    /// Initialize new instance of RSS feed
    /// </summary>
    /// <param name="link">URL</param>
    public RssFeed(Uri link) : this(string.Empty, string.Empty, link, DateTimeOffset.Now)
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Attribute extension
    /// </summary>
    public KeyValuePair<XmlQualifiedName, string> AttributeExtension { get; set; }

    /// <summary>
    /// Element extensions
    /// </summary>
    public List<XElement> ElementExtensions { get; } = new List<XElement>();

    /// <summary>
    /// List of rss items
    /// </summary>
    public List<RssItem> Items { get; set; } = new List<RssItem>();

    /// <summary>
    /// Title
    /// </summary>
    public XElement Title { get; protected set; }

    /// <summary>
    /// Description
    /// </summary>
    public XElement Description { get; protected set; }

    /// <summary>
    /// Link
    /// </summary>
    public XElement Link { get; protected set; }

    /// <summary>
    /// Last build date
    /// </summary>
    public XElement LastBuildDate { get; protected set; }

    #endregion

    #region Methods

    /// <summary>
    /// Load RSS feed from the passed stream
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the RSS feed
    /// </returns>
    public static async Task<RssFeed> LoadAsync(Stream stream)
    {
		_logger.LogDebug(">>> Entering LoadAsync(stream={Stream})", stream);
        try
        {
            var document = await XDocument.LoadAsync(stream, LoadOptions.None, default);

			_logger.LogDebug("LoadAsync(stream={Stream}): document → {Document}", stream, document);
            var channel = document.Root?.Element(NopRssDefaults.Channel);

			_logger.LogDebug("LoadAsync(stream={Stream}): channel → {Channel}", stream, channel);
            if (channel == null)
                return null;

            var title = channel.Element(NopRssDefaults.Title)?.Value ?? string.Empty;
            var description = channel.Element(NopRssDefaults.Description)?.Value ?? string.Empty;
			_logger.LogDebug("LoadAsync(stream={Stream}): title → {Title}", stream, title);
			_logger.LogDebug("LoadAsync(stream={Stream}): description → {Description}", stream, description);
            var link = new Uri(channel.Element(NopRssDefaults.Link)?.Value ?? string.Empty);
            var lastBuildDateValue = channel.Element(NopRssDefaults.LastBuildDate)?.Value;
			_logger.LogDebug("LoadAsync(stream={Stream}): link → {Link}", stream, link);
            var lastBuildDate = lastBuildDateValue == null ? DateTimeOffset.Now : DateTimeOffset.ParseExact(lastBuildDateValue, "r", null);
			_logger.LogDebug("LoadAsync(stream={Stream}): lastBuildDateValue → {LastBuildDateValue}", stream, lastBuildDateValue);
			_logger.LogDebug("LoadAsync(stream={Stream}): lastBuildDate → {LastBuildDate}", stream, lastBuildDate);

            var feed = new RssFeed(title, description, link, lastBuildDate);

			_logger.LogDebug("LoadAsync(stream={Stream}): feed → {Feed}", stream, feed);
            foreach (var item in channel.Elements(NopRssDefaults.Item))
            {
                feed.Items.Add(new RssItem(item));
            }

            return feed;
        }
        catch
        {
			_logger.LogError("Exception in LoadAsync(stream={Stream}): {Arg1}", stream, ex.Message);
			_logger.LogDebug("<<< Exiting LoadAsync(stream={Stream})", stream);
            return null;
        }
    }

    /// <summary>
    /// Get content of this RSS feed
    /// </summary>
    /// <returns>Content of RSS feed</returns>
    public string GetContent()
    {
		_logger.LogDebug(">>> Entering GetContent()");
        var document = new XDocument();
        var root = new XElement(NopRssDefaults.RSS, new XAttribute("version", "2.0"));
		_logger.LogDebug("GetContent(): document → {Document}", document);
		_logger.LogWarning("GetContent(): root → {Root}", root);
        var channel = new XElement(NopRssDefaults.Channel,
            new XAttribute(XName.Get(AttributeExtension.Key.Name, AttributeExtension.Key.Namespace), AttributeExtension.Value));

        channel.Add(Title, Description, Link, LastBuildDate);

        foreach (var element in ElementExtensions)
        {
            channel.Add(element);
        }

        foreach (var item in Items)
        {
            channel.Add(item.ToXElement());
        }

        root.Add(channel);
        document.Add(root);

		_logger.LogDebug("<<< Exiting GetContent()");
        return document.ToString();
    }

    #endregion
}