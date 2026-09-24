using Microsoft.Extensions.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Common;

/// <summary>
/// Represents a task for keeping the site alive
/// </summary>
public partial class KeepAliveTask : IScheduleTask
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly StoreHttpClient _storeHttpClient;

    #endregion

    #region Ctor

    public KeepAliveTask(StoreHttpClient storeHttpClient)
    {
		_logger.LogError(">>> Entering KeepAliveTask(storeHttpClient={StoreHttpClient})", storeHttpClient);
        _storeHttpClient = storeHttpClient;
		_logger.LogDebug("KeepAliveTask(storeHttpClient={StoreHttpClient}): _storeHttpClient → {_storeHttpClient}", storeHttpClient, _storeHttpClient);
    _logger.LogWarning("<<< Exiting KeepAliveTask(storeHttpClient={StoreHttpClient})", storeHttpClient);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
		_logger.LogError(">>> Entering ExecuteAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _storeHttpClient.KeepAliveAsync();
		_logger.LogError("ExecuteAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting ExecuteAsync()");
    }

    #endregion
}