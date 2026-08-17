using Nop.Core.Caching;
using Nop.Services.ScheduleTasks;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Caching;

/// <summary>
/// Clear cache scheduled task implementation
/// </summary>
public partial class ClearCacheTask : IScheduleTask
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public ClearCacheTask(IStaticCacheManager staticCacheManager)
    {
		_logger.LogDebug(">>> Entering ClearCacheTask(staticCacheManager={StaticCacheManager})", staticCacheManager);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("<<< Exiting ClearCacheTask(staticCacheManager={StaticCacheManager})", staticCacheManager);
		_logger.LogDebug("ClearCacheTask(staticCacheManager={StaticCacheManager}): _staticCacheManager → {_staticCacheManager}", staticCacheManager, _staticCacheManager);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
		_logger.LogDebug(">>> Entering ExecuteAsync()");
        await _staticCacheManager.ClearAsync();
		_logger.LogDebug("<<< Exiting ExecuteAsync()");
    }

    #endregion
}