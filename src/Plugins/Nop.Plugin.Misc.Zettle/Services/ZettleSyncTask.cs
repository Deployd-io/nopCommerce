using Microsoft.Extensions.Logging;
﻿using Nop.Services.ScheduleTasks;

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
		_logger.LogDebug(">>> Entering ZettleSyncTask(zettleService={ZettleService},zettleSettings={ZettleSettings})", zettleService, zettleSettings);
        _zettleService = zettleService;
        _zettleSettings = zettleSettings;
		_logger.LogDebug("<<< Exiting ZettleSyncTask(zettleService={ZettleService},zettleSettings={ZettleSettings})", zettleService, zettleSettings);
		_logger.LogDebug("ZettleSyncTask(zettleService={ZettleService},zettleSettings={ZettleSettings}): _zettleSettings → {_zettleSettings}", zettleService, zettleSettings, _zettleSettings);
		_logger.LogDebug("ZettleSyncTask(zettleService={ZettleService},zettleSettings={ZettleSettings}): _zettleService → {_zettleService}", zettleService, zettleSettings, _zettleService);
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
        if (ZettleService.IsConfigured(_zettleSettings))
            await _zettleService.ImportAsync();
			_logger.LogDebug("<<< Exiting ExecuteAsync()");
    }

    #endregion
}