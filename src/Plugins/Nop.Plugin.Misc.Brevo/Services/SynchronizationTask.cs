using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Misc.Brevo.Services;

/// <summary>
/// Represents a schedule task to synchronize contacts
/// </summary>
public class SynchronizationTask : IScheduleTask
{
    #region Fields

    protected readonly BrevoManager _brevoEmailManager;

    #endregion

    #region Ctor

    public SynchronizationTask(BrevoManager brevoEmailManager)
    {
		logger.LogDebug(">>> Entering SynchronizationTask(brevoEmailManager={BrevoEmailManager})", brevoEmailManager);
        _brevoEmailManager = brevoEmailManager;
		logger.LogDebug("<<< Exiting SynchronizationTask(brevoEmailManager={BrevoEmailManager})", brevoEmailManager);
		logger.LogDebug("SynchronizationTask(brevoEmailManager={BrevoEmailManager}): _brevoEmailManager → {_brevoEmailManager}", brevoEmailManager, _brevoEmailManager);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Execute task
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ExecuteAsync()
    {
		logger.LogDebug(">>> Entering ExecuteAsync()");
        await _brevoEmailManager.SynchronizeAsync();
		logger.LogDebug("<<< Exiting ExecuteAsync()");
    }

    #endregion
}
