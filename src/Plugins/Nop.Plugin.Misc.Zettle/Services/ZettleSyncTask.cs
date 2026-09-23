using Microsoft.Extensions.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Misc.Zettle.Services;

/// <summary>
/// Represents a schedule task to synchronize products
/// </summary>
public class ZettleSyncTask : IScheduleTask
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ZettleService _zettleService;
    protected readonly ZettleSettings _zettleSettings;

    #endregion

    #region Ctor

    public ZettleSyncTask(ZettleService zettleService,
        ZettleSettings zettleSettings)
    {
		_logger.LogError(">>> Entering ZettleSyncTask(zettleService={ZettleService},zettleSettings={ZettleSettings})", zettleService, zettleSettings);
        _zettleService = zettleService;
        _zettleSettings = zettleSettings;
		_logger.LogDebug("ZettleSyncTask(zettleService={ZettleService},zettleSettings={ZettleSettings}): _zettleSettings → {_zettleSettings}", zettleService, zettleSettings, _zettleSettings);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Execute task
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ExecuteAsync()
    {
		_logger.LogError(">>> Entering ExecuteAsync()");
        if (ZettleService.IsConfigured(_zettleSettings))
            await _zettleService.ImportAsync();
			_logger.LogWarning("<<< Exiting ExecuteAsync()");
    }

    #endregion
}