using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.FacebookPixel.Models;

/// <summary>
/// Represents a custom event model
/// </summary>
public record CustomEventModel : BaseNopModel
{
    #region Ctor

    public CustomEventModel()
    {
		logger.LogDebug(">>> Entering CustomEventModel()");
        WidgetZonesIds = new List<int>();
        WidgetZones = new List<string>();
		logger.LogDebug("CustomEventModel(): WidgetZonesIds → {WidgetZonesIds}", WidgetZonesIds);
        AvailableWidgetZones = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting CustomEventModel()");
		logger.LogDebug("CustomEventModel(): WidgetZones → {WidgetZones}", WidgetZones);
		logger.LogDebug("CustomEventModel(): AvailableWidgetZones → {AvailableWidgetZones}", AvailableWidgetZones);
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