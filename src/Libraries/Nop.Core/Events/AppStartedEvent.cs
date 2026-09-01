using Microsoft.Extensions.Logging;
namespace Nop.Core.Events;

/// <summary>
/// Event that is published when the application has started.
/// This event can be consumed by plugins or services that need to perform actions
/// once the application is fully initialized.
/// </summary>
public partial class AppStartedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Initializes a new instance of the <see cref="AppStartedEvent"/> class.
    /// Sets the timestamp of when the application was started in UTC.
    /// </summary>
    public AppStartedEvent()
    {
		_logger.LogDebug(">>> Entering AppStartedEvent()");
        StartedAtUtc = DateTime.UtcNow;
		_logger.LogDebug("AppStartedEvent(): StartedAtUtc → {StartedAtUtc}", StartedAtUtc);
    }

    /// <summary>
    /// Gets the UTC date and time when the application started.
    /// </summary>
    public DateTime StartedAtUtc { get; }
}