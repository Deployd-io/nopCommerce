using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a setting model
/// </summary>
public partial record SettingModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public SettingModel()
    {
		_logger.LogDebug(">>> Entering SettingModel()");
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("SettingModel(): AvailableStores → {AvailableStores}", AvailableStores);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.Value")]
    public string Value { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.StoreName")]
    public string Store { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.Store")]
    public int StoreId { get; set; }
    public IList<SelectListItem> AvailableStores { get; set; }

    #endregion
}