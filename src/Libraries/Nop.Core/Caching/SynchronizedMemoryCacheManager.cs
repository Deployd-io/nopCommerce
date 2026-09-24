using Microsoft.Extensions.Logging;
using Nop.Core.Configuration;

namespace Nop.Core.Caching;

/// <summary>
/// Represents a memory cache manager with distributed synchronization
/// </summary>
/// <remarks>
/// This class should be registered on IoC as singleton instance
/// </remarks>
public partial class SynchronizedMemoryCacheManager : MemoryCacheManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public SynchronizedMemoryCacheManager(AppSettings appSettings,
        ISynchronizedMemoryCache memoryCache,
        ICacheKeyManager cacheKeyManager) : base(appSettings, memoryCache, cacheKeyManager)
    {
	_logger.LogError(">>> Entering SynchronizedMemoryCacheManager(appSettings={AppSettings},memoryCache={MemoryCache},cacheKeyManager={CacheKeyManager})", appSettings, memoryCache, cacheKeyManager);
    _logger.LogWarning("<<< Exiting SynchronizedMemoryCacheManager(appSettings={AppSettings},memoryCache={MemoryCache},cacheKeyManager={CacheKeyManager})", appSettings, memoryCache, cacheKeyManager);
    }
}