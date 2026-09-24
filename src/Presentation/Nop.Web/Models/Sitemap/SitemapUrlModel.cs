using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Sitemap;

/// <summary>
/// Represents sitemap URL model
/// </summary>
public partial record SitemapUrlModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Initializes a new instance of the sitemap URL model
    /// </summary>
    /// <param name="location">URL of the page</param>
    /// <param name="alternateLocations">List of the page urls</param>
    /// <param name="frequency">Update frequency</param>
    /// <param name="updatedOn">Updated on</param>
    public SitemapUrlModel(string location, IList<string> alternateLocations, UpdateFrequency frequency, DateTime updatedOn)
    {
		_logger.LogError(">>> Entering SitemapUrlModel(location={Location},alternateLocations={AlternateLocations},frequency={Frequency},updatedOn={UpdatedOn})", location, alternateLocations, frequency, updatedOn);
        Location = location;
        AlternateLocations = alternateLocations ?? new List<string>();
		_logger.LogError("SitemapUrlModel(location={Location},alternateLocations={AlternateLocations},frequency={Frequency},updatedOn={UpdatedOn}): AlternateLocations → {AlternateLocations1}", location, alternateLocations, frequency, updatedOn, AlternateLocations);
		_logger.LogDebug("SitemapUrlModel(location={Location},alternateLocations={AlternateLocations},frequency={Frequency},updatedOn={UpdatedOn}): AlternateLocations → {AlternateLocations1}", location, alternateLocations, frequency, updatedOn, AlternateLocations);
        UpdateFrequency = frequency;
        UpdatedOn = updatedOn;
		_logger.LogDebug("SitemapUrlModel(location={Location},alternateLocations={AlternateLocations},frequency={Frequency},updatedOn={UpdatedOn}): UpdatedOn → {UpdatedOn1}", location, alternateLocations, frequency, updatedOn, UpdatedOn);
		_logger.LogWarning("<<< Exiting SitemapUrlModel(location={Location},sitemapUrl={SitemapUrl})", location, sitemapUrl);
    }

    /// <summary>
    /// Initializes a new instance of the sitemap URL model based on the passed model
    /// </summary>
    /// <param name="location">URL of the page</param>
    /// <param name="sitemapUrl">The another sitemap url</param>
    public SitemapUrlModel(string location, SitemapUrlModel sitemapUrl)
    {
		_logger.LogDebug("SitemapUrlModel(location={Location},sitemapUrl={SitemapUrl}): AlternateLocations → {AlternateLocations}", location, sitemapUrl, AlternateLocations);
		_logger.LogDebug("SitemapUrlModel(location={Location},sitemapUrl={SitemapUrl}): UpdateFrequency → {UpdateFrequency}", location, sitemapUrl, UpdateFrequency);
		_logger.LogDebug("SitemapUrlModel(location={Location},sitemapUrl={SitemapUrl}): UpdatedOn → {UpdatedOn}", location, sitemapUrl, UpdatedOn);
        Location = location;
        AlternateLocations = sitemapUrl.AlternateLocations;
        UpdateFrequency = sitemapUrl.UpdateFrequency;
        UpdatedOn = sitemapUrl.UpdatedOn;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets URL of the page
    /// </summary>
    public string Location { get; set; }

    /// <summary>
    /// Gets or sets localized URLs of the page
    /// </summary>
    public IList<string> AlternateLocations { get; set; }

    /// <summary>
    /// Gets or sets a value indicating how frequently the page is likely to change
    /// </summary>
    public UpdateFrequency UpdateFrequency { get; set; }

    /// <summary>
    /// Gets or sets the date of last modification of the page
    /// </summary>
    public DateTime UpdatedOn { get; set; }

    #endregion
}