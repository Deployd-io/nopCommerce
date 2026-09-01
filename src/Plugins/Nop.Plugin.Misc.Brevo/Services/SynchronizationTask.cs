using Microsoft.Extensions.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Misc.Brevo.Services;

/// <summary>
/// Represents a schedule task to synchronize contacts
/// </summary>
public class SynchronizationTask : IScheduleTask
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly BrevoManager _brevoEmailManager;

    #endregion

    #region Ctor

    public SynchronizationTask(BrevoManager brevoEmailManager)
    {
		_logger.LogDebug(">>> Entering SynchronizationTask(brevoEmailManager={BrevoEmailManager})", brevoEmailManager);
        _brevoEmailManager = brevoEmailManager;
		_logger.LogDebug("SynchronizationTask(brevoEmailManager={BrevoEmailManager}): _brevoEmailManager → {_brevoEmailManager}", brevoEmailManager, _brevoEmailManager);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Execute task
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ExecuteAsync()
    {
		_logger.LogDebug(">>> Entering ExecuteAsync()");
        await _brevoEmailManager.SynchronizeAsync();
		_logger.LogDebug("<<< Exiting ExecuteAsync()");
    }

    #endregion
}