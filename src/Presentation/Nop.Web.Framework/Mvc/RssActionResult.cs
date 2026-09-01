using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Rss;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc;

/// <summary>
/// RSS action result
/// </summary>
public partial class RssActionResult : ContentResult
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="feed">Syndication feed</param>
    /// <param name="feedPageUrl">Feed page url for atom self link</param>
    public RssActionResult(RssFeed feed, string feedPageUrl)
    {
		_logger.LogDebug(">>> Entering RssActionResult(feed={Feed},feedPageUrl={FeedPageUrl})", feed, feedPageUrl);
        ContentType = "application/atom+xml";
        Feed = feed;
		_logger.LogDebug("RssActionResult(feed={Feed},feedPageUrl={FeedPageUrl}): ContentType → {ContentType}", feed, feedPageUrl, ContentType);
		_logger.LogDebug("RssActionResult(feed={Feed},feedPageUrl={FeedPageUrl}): Feed → {Feed1}", feed, feedPageUrl, Feed);

        //add atom namespace
        XNamespace atom = "http://www.w3.org/2005/Atom";
		_logger.LogDebug("RssActionResult(feed={Feed},feedPageUrl={FeedPageUrl}): atom → {Atom}", feed, feedPageUrl, atom);
        Feed.AttributeExtension = new KeyValuePair<XmlQualifiedName, string>(new XmlQualifiedName("atom", XNamespace.Xmlns.NamespaceName), atom.NamespaceName);
        //add atom:link with rel='self' 
        Feed.ElementExtensions.Add(new XElement(atom + "link", new XAttribute("href", new Uri(feedPageUrl)), new XAttribute("rel", "self"), new XAttribute("type", "application/rss+xml")));
		_logger.LogDebug("<<< Exiting RssActionResult(feed={Feed},feedPageUrl={FeedPageUrl})", feed, feedPageUrl);
    }

    /// <summary>
    /// Feed
    /// </summary>
    public RssFeed Feed { get; set; }

    /// <summary>
    /// Execute result async
    /// </summary>
    /// <param name="context">ActionContext</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override Task ExecuteResultAsync(ActionContext context)
    {
		_logger.LogDebug(">>> Entering ExecuteResultAsync(context={Context})", context);
        Content = Feed.GetContent();
		_logger.LogDebug("ExecuteResultAsync(context={Context}): Content → {Content}", context, Content);
        return base.ExecuteResultAsync(context);
    }

    /// <summary>
    /// Execute result
    /// </summary>
    /// <param name="context">ActionContext</param>
    public override void ExecuteResult(ActionContext context)
    {
		_logger.LogDebug(">>> Entering ExecuteResult(context={Context})", context);
        Content = Feed.GetContent();
		_logger.LogDebug("<<< Exiting ExecuteResult(context={Context})", context);
		_logger.LogDebug("ExecuteResult(context={Context}): Content → {Content}", context, Content);
        base.ExecuteResult(context);
    }
}