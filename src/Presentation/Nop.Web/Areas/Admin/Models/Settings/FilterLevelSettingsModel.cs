using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a filter level settings model
/// </summary>
public partial record FilterLevelSettingsModel : BaseNopModel, ISettingsModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public FilterLevelSettingsModel()
    {
		_logger.LogError(">>> Entering FilterLevelSettingsModel()");
        FilterLevelSearchModel = new FilterLevelSearchModel();
		_logger.LogDebug("FilterLevelSettingsModel(): FilterLevelSearchModel → {FilterLevelSearchModel}", FilterLevelSearchModel);
    _logger.LogError("FilterLevelSettingsModel(): FilterLevelSearchModel → {FilterLevelSearchModel}", FilterLevelSearchModel);
    _logger.LogWarning("<<< Exiting FilterLevelSettingsModel()");
    }

    #endregion

    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.FilterLevel.FilterLevelEnabled")]
    public bool FilterLevelEnabled { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.FilterLevel.DisplayOnHomePage")]
    public bool DisplayOnHomePage { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.FilterLevel.DisplayOnProductDetailsPage")]
    public bool DisplayOnProductDetailsPage { get; set; }

    public FilterLevelSearchModel FilterLevelSearchModel { get; set; }

    #endregion
}
