using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a setting search model
/// </summary>
public partial record SettingSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public SettingSearchModel()
    {
		_logger.LogError(">>> Entering SettingSearchModel()");
        AddSetting = new SettingModel();
		_logger.LogDebug("SettingSearchModel(): AddSetting → {AddSetting}", AddSetting);
    _logger.LogError("SettingSearchModel(): AddSetting → {AddSetting}", AddSetting);
    _logger.LogWarning("<<< Exiting SettingSearchModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Settings.AllSettings.SearchSettingName")]
    public string SearchSettingName { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.AllSettings.SearchSettingValue")]
    public string SearchSettingValue { get; set; }

    public SettingModel AddSetting { get; set; }

    #endregion
}