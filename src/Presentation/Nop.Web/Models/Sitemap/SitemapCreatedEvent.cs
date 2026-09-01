using Microsoft.Extensions.Logging;
namespace Nop.Web.Models.Sitemap;

/// <summary>
/// Represents an event that occurs when the sitemap is created
/// </summary>
public partial class SitemapCreatedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public SitemapCreatedEvent(IList<SitemapUrlModel> sitemapUrls)
    {
		_logger.LogDebug(">>> Entering SitemapCreatedEvent(sitemapUrls={SitemapUrls})", sitemapUrls);
        SitemapUrls = sitemapUrls ?? new List<SitemapUrlModel>();
		_logger.LogDebug("SitemapCreatedEvent(sitemapUrls={SitemapUrls}): SitemapUrls → {SitemapUrls1}", sitemapUrls, SitemapUrls);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a list of sitemap URLs
    /// </summary>
    public IList<SitemapUrlModel> SitemapUrls { get; }

    #endregion
}