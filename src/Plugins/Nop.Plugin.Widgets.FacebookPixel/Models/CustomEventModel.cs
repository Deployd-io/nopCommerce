using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Widgets.FacebookPixel.Models;

/// <summary>
/// Represents a custom event model
/// </summary>
public record CustomEventModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CustomEventModel()
    {
		_logger.LogDebug(">>> Entering CustomEventModel()");
        WidgetZonesIds = new List<int>();
        WidgetZones = new List<string>();
		_logger.LogDebug("CustomEventModel(): WidgetZones → {WidgetZones}", WidgetZones);
		_logger.LogDebug("CustomEventModel(): WidgetZonesIds → {WidgetZonesIds}", WidgetZonesIds);
        AvailableWidgetZones = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting CustomEventModel()");
		_logger.LogDebug("CustomEventModel(): AvailableWidgetZones → {AvailableWidgetZones}", AvailableWidgetZones);
    }

    #endregion

    #region Properties

    public int ConfigurationId { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.CustomEvents.Fields.EventName")]
    public string EventName { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.CustomEvents.Fields.WidgetZones")]
    public IList<int> WidgetZonesIds { get; set; }
    public IList<string> WidgetZones { get; set; }
    public IList<SelectListItem> AvailableWidgetZones { get; set; }
    public string WidgetZonesName { get; set; }

    #endregion
}