using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Plugins.Marketplace;

/// <summary>
/// Represents a search model of plugins of the official feed
/// </summary>
public partial record OfficialFeedPluginSearchModel : BaseSearchModel
{
    #region Ctor

    public OfficialFeedPluginSearchModel()
    {
		logger.LogDebug(">>> Entering OfficialFeedPluginSearchModel()");
        AvailableVersions = new List<SelectListItem>();
        AvailableCategories = new List<SelectListItem>();
		logger.LogDebug("OfficialFeedPluginSearchModel(): AvailableVersions → {AvailableVersions}", AvailableVersions);
		logger.LogDebug("OfficialFeedPluginSearchModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
        AvailablePrices = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting OfficialFeedPluginSearchModel()");
		logger.LogDebug("OfficialFeedPluginSearchModel(): AvailablePrices → {AvailablePrices}", AvailablePrices);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Plugins.OfficialFeed.Name")]
    public string SearchName { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.OfficialFeed.Version")]
    public int SearchVersionId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.OfficialFeed.Category")]
    public int SearchCategoryId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.OfficialFeed.Price")]
    public int SearchPriceId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.OfficialFeed.Version")]
    public IList<SelectListItem> AvailableVersions { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.OfficialFeed.Category")]
    public IList<SelectListItem> AvailableCategories { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.OfficialFeed.Price")]
    public IList<SelectListItem> AvailablePrices { get; set; }

    #endregion
}