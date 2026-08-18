using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Widgets.FacebookPixel.Models;

/// <summary>
/// Represents a custom event search model
/// </summary>
public record CustomEventSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CustomEventSearchModel()
    {
		_logger.LogDebug(">>> Entering CustomEventSearchModel()");
        AddCustomEvent = new CustomEventModel();
		_logger.LogDebug("CustomEventSearchModel(): AddCustomEvent → {AddCustomEvent}", AddCustomEvent);
		_logger.LogDebug("<<< Exiting CustomEventSearchModel()");
    }

    #endregion

    #region Properties

    public int ConfigurationId { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.CustomEvents.Search.WidgetZone")]
    public string WidgetZone { get; set; }

    public CustomEventModel AddCustomEvent { get; set; }

    #endregion
}