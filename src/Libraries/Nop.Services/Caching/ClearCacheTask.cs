using Nop.Core.Caching;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Caching;

/// <summary>
/// Clear cache scheduled task implementation
/// </summary>
public partial class ClearCacheTask : IScheduleTask
{
    #region Fields

    protected readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public ClearCacheTask(IStaticCacheManager staticCacheManager)
    {
		logger.LogDebug(">>> Entering ClearCacheTask(staticCacheManager={StaticCacheManager})", staticCacheManager);
        _staticCacheManager = staticCacheManager;
		logger.LogDebug("<<< Exiting ClearCacheTask(staticCacheManager={StaticCacheManager})", staticCacheManager);
		logger.LogDebug("ClearCacheTask(staticCacheManager={StaticCacheManager}): _staticCacheManager → {_staticCacheManager}", staticCacheManager, _staticCacheManager);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
		logger.LogDebug(">>> Entering ExecuteAsync()");
        await _staticCacheManager.ClearAsync();
		logger.LogDebug("<<< Exiting ExecuteAsync()");
    }

    #endregion
}