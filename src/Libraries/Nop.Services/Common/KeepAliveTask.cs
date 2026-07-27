using Nop.Services.ScheduleTasks;

namespace Nop.Services.Common;

/// <summary>
/// Represents a task for keeping the site alive
/// </summary>
public partial class KeepAliveTask : IScheduleTask
{
    #region Fields

    protected readonly StoreHttpClient _storeHttpClient;

    #endregion

    #region Ctor

    public KeepAliveTask(StoreHttpClient storeHttpClient)
    {
		logger.LogDebug(">>> Entering KeepAliveTask(storeHttpClient={StoreHttpClient})", storeHttpClient);
        _storeHttpClient = storeHttpClient;
		logger.LogDebug("<<< Exiting KeepAliveTask(storeHttpClient={StoreHttpClient})", storeHttpClient);
		logger.LogDebug("KeepAliveTask(storeHttpClient={StoreHttpClient}): _storeHttpClient → {_storeHttpClient}", storeHttpClient, _storeHttpClient);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
		logger.LogDebug(">>> Entering ExecuteAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _storeHttpClient.KeepAliveAsync();
		logger.LogDebug("<<< Exiting ExecuteAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("ExecuteAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("ExecuteAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    #endregion
}