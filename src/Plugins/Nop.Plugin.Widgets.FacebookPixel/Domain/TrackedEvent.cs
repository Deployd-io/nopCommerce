using Microsoft.Extensions.Logging;
namespace Nop.Plugin.Widgets.FacebookPixel.Domain;

/// <summary>
/// Represents tracked event details
/// </summary>
public class TrackedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public TrackedEvent()
    {
		_logger.LogError(">>> Entering TrackedEvent()");
        EventObjects = new List<string>();
		_logger.LogDebug("TrackedEvent(): EventObjects → {EventObjects}", EventObjects);
    _logger.LogError("TrackedEvent(): EventObjects → {EventObjects}", EventObjects);
    _logger.LogWarning("<<< Exiting TrackedEvent()");
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the event name
    /// </summary>
    public string EventName { get; set; }

    /// <summary>
    /// Gets or sets the store identifier
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this event is a custom event
    /// </summary>
    public bool IsCustomEvent { get; set; }

    /// <summary>
    /// Gets or sets the formatted objects for this event
    /// </summary>
    public IList<string> EventObjects { get; set; }

    #endregion
}