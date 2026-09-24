using Microsoft.Extensions.Logging;
namespace Nop.Plugin.Widgets.FacebookPixel.Domain;

/// <summary>
/// Represents custom event configuration
/// </summary>
public class CustomEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CustomEvent()
    {
		_logger.LogError(">>> Entering CustomEvent()");
        WidgetZones = new List<string>();
		_logger.LogDebug("CustomEvent(): WidgetZones → {WidgetZones}", WidgetZones);
    _logger.LogError("CustomEvent(): WidgetZones → {WidgetZones}", WidgetZones);
    _logger.LogWarning("<<< Exiting CustomEvent()");
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the custom event name
    /// </summary>
    public string EventName { get; set; }

    /// <summary>
    /// Gets or sets the list of widget zones in which this event is tracked
    /// </summary>
    public IList<string> WidgetZones { get; set; }

    #endregion
}