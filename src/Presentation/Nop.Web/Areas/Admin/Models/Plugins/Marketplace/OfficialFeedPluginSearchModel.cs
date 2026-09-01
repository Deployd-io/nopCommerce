using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Plugins.Marketplace;

/// <summary>
/// Represents a search model of plugins of the official feed
/// </summary>
public partial record OfficialFeedPluginSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public OfficialFeedPluginSearchModel()
    {
		_logger.LogDebug(">>> Entering OfficialFeedPluginSearchModel()");
        AvailableVersions = new List<SelectListItem>();
		_logger.LogDebug("OfficialFeedPluginSearchModel(): AvailableVersions → {AvailableVersions}", AvailableVersions);
        AvailableCategories = new List<SelectListItem>();
		_logger.LogDebug("OfficialFeedPluginSearchModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
        AvailablePrices = new List<SelectListItem>();
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