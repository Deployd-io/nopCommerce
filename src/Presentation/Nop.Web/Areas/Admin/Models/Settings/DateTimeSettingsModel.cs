using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a date time settings model
/// </summary>
public partial record DateTimeSettingsModel : BaseNopModel, ISettingsModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public DateTimeSettingsModel()
    {
		_logger.LogDebug(">>> Entering DateTimeSettingsModel()");
        AvailableTimeZones = new List<SelectListItem>();
		_logger.LogDebug("DateTimeSettingsModel(): AvailableTimeZones → {AvailableTimeZones}", AvailableTimeZones);
    }

    #endregion

    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AllowCustomersToSetTimeZone")]
    public bool AllowCustomersToSetTimeZone { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.DefaultStoreTimeZone")]
    public string DefaultStoreTimeZoneId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.DefaultStoreTimeZone")]
    public IList<SelectListItem> AvailableTimeZones { get; set; }

    #endregion
}