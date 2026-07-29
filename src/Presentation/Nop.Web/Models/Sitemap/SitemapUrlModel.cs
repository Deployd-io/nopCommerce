using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Sitemap;

/// <summary>
/// Represents sitemap URL model
/// </summary>
public partial record SitemapUrlModel : BaseNopModel
{
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
		logger.LogDebug(">>> Entering SitemapUrlModel(location={Location},alternateLocations={AlternateLocations},frequency={Frequency},updatedOn={UpdatedOn})", location, alternateLocations, frequency, updatedOn);
        Location = location;
        AlternateLocations = alternateLocations ?? new List<string>();
		logger.LogDebug("SitemapUrlModel(location={Location},alternateLocations={AlternateLocations},frequency={Frequency},updatedOn={UpdatedOn}): AlternateLocations → {AlternateLocations1}", location, alternateLocations, frequency, updatedOn, AlternateLocations);
        UpdateFrequency = frequency;
        UpdatedOn = updatedOn;
		logger.LogDebug("SitemapUrlModel(location={Location},alternateLocations={AlternateLocations},frequency={Frequency},updatedOn={UpdatedOn}): UpdateFrequency → {UpdateFrequency}", location, alternateLocations, frequency, updatedOn, UpdateFrequency);
		logger.LogDebug("SitemapUrlModel(location={Location},alternateLocations={AlternateLocations},frequency={Frequency},updatedOn={UpdatedOn}): UpdatedOn → {UpdatedOn1}", location, alternateLocations, frequency, updatedOn, UpdatedOn);
		logger.LogDebug("SitemapUrlModel(location={Location},alternateLocations={AlternateLocations},frequency={Frequency},updatedOn={UpdatedOn}): Location → {Location1}", location, alternateLocations, frequency, updatedOn, Location);
		logger.LogDebug("SitemapUrlModel(location={Location},sitemapUrl={SitemapUrl}): AlternateLocations → {AlternateLocations}", location, sitemapUrl, AlternateLocations);
		logger.LogDebug("SitemapUrlModel(location={Location},sitemapUrl={SitemapUrl}): UpdateFrequency → {UpdateFrequency}", location, sitemapUrl, UpdateFrequency);
		logger.LogDebug("<<< Exiting SitemapUrlModel(location={Location},sitemapUrl={SitemapUrl})", location, sitemapUrl);
		logger.LogDebug("SitemapUrlModel(location={Location},sitemapUrl={SitemapUrl}): UpdatedOn → {UpdatedOn}", location, sitemapUrl, UpdatedOn);
		logger.LogDebug("SitemapUrlModel(location={Location},sitemapUrl={SitemapUrl}): Location → {Location1}", location, sitemapUrl, Location);
    }

    /// <summary>
    /// Initializes a new instance of the sitemap URL model based on the passed model
    /// </summary>
    /// <param name="location">URL of the page</param>
    /// <param name="sitemapUrl">The another sitemap url</param>
    public SitemapUrlModel(string location, SitemapUrlModel sitemapUrl)
    {
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