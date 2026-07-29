namespace Nop.Web.Models.Sitemap;

/// <summary>
/// Represents an event that occurs when the sitemap is created
/// </summary>
public partial class SitemapCreatedEvent
{
    #region Ctor

    public SitemapCreatedEvent(IList<SitemapUrlModel> sitemapUrls)
    {
		logger.LogDebug(">>> Entering SitemapCreatedEvent(sitemapUrls={SitemapUrls})", sitemapUrls);
        SitemapUrls = sitemapUrls ?? new List<SitemapUrlModel>();
		logger.LogDebug("<<< Exiting SitemapCreatedEvent(sitemapUrls={SitemapUrls})", sitemapUrls);
		logger.LogDebug("SitemapCreatedEvent(sitemapUrls={SitemapUrls}): SitemapUrls → {SitemapUrls1}", sitemapUrls, SitemapUrls);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a list of sitemap URLs
    /// </summary>
    public IList<SitemapUrlModel> SitemapUrls { get; }

    #endregion
}