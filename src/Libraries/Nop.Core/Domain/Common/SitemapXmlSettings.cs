using Microsoft.Extensions.Logging;
﻿using Nop.Core.Configuration;

namespace Nop.Core.Domain.Common;

/// <summary>
/// Represent sitemap.xml settings
/// </summary>
public partial class SitemapXmlSettings : ISettings
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public SitemapXmlSettings()
    {
		_logger.LogDebug(">>> Entering SitemapXmlSettings()");
        SitemapCustomUrls = new List<string>();
		_logger.LogDebug("<<< Exiting SitemapXmlSettings()");
		_logger.LogDebug("SitemapXmlSettings(): SitemapCustomUrls → {SitemapCustomUrls}", SitemapCustomUrls);
    }

    /// <summary>
    /// Gets or sets a value indicating whether sitemap.xml is enabled
    /// </summary>
    public bool SitemapXmlEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include blog posts to sitemap.xml
    /// </summary>
    public bool SitemapXmlIncludeBlogPosts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include categories to sitemap.xml
    /// </summary>
    public bool SitemapXmlIncludeCategories { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include custom urls to sitemap.xml
    /// </summary>
    public bool SitemapXmlIncludeCustomUrls { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include manufacturers to sitemap.xml
    /// </summary>
    public bool SitemapXmlIncludeManufacturers { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include news to sitemap.xml
    /// </summary>
    public bool SitemapXmlIncludeNews { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include products to sitemap.xml
    /// </summary>
    public bool SitemapXmlIncludeProducts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include product tags to sitemap.xml
    /// </summary>
    public bool SitemapXmlIncludeProductTags { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include topics to sitemap.xml
    /// </summary>
    public bool SitemapXmlIncludeTopics { get; set; }

    /// <summary>
    /// A list of custom URLs to be added to sitemap.xml (include page names only)
    /// </summary>
    public List<string> SitemapCustomUrls { get; set; }

    /// <summary>
    /// Gets or sets a value indicating after which period of time the sitemap files will be rebuilt (in hours)
    /// </summary>
    public int RebuildSitemapXmlAfterHours { get; set; }

    /// <summary>
    /// Gets or sets the wait time (in seconds) before the operation can be started again
    /// </summary>
    public int SitemapBuildOperationDelay { get; set; }
}