using Microsoft.Extensions.Caching.Distributed;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Caching;

public partial class MemoryDistributedCacheManager : DistributedCacheManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public MemoryDistributedCacheManager(AppSettings appSettings,
        IDistributedCache distributedCache,
        ICacheKeyManager cacheKeyManager,
        IConcurrentCollection<object> concurrentCollection)
        : base(appSettings, distributedCache, cacheKeyManager, concurrentCollection)
    {
	_logger.LogDebug(">>> Entering MemoryDistributedCacheManager(appSettings={AppSettings},distributedCache={DistributedCache},cacheKeyManager={CacheKeyManager},concurrentCollection={ConcurrentCollection})", appSettings, distributedCache, cacheKeyManager, concurrentCollection);
	_logger.LogDebug("<<< Exiting MemoryDistributedCacheManager(appSettings={AppSettings},distributedCache={DistributedCache},cacheKeyManager={CacheKeyManager},concurrentCollection={ConcurrentCollection})", appSettings, distributedCache, cacheKeyManager, concurrentCollection);
    }

    #endregion

    /// <summary>
    /// Clear all cache data
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task ClearAsync()
    {
		_logger.LogDebug(">>> Entering ClearAsync()");
        foreach (var key in _localKeyManager.Keys)
            await RemoveAsync(key, false);

        ClearInstanceData();
		_logger.LogDebug("<<< Exiting ClearAsync()");
    }

    /// <summary>
    /// Remove items by cache key prefix
    /// </summary>
    /// <param name="prefix">Cache key prefix</param>
    /// <param name="prefixParameters">Parameters to create cache key prefix</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
    {
		_logger.LogDebug(">>> Entering RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
        var keyPrefix = PrepareKeyPrefix(prefix, prefixParameters);

		_logger.LogDebug("RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters}): keyPrefix → {KeyPrefix}", prefix, prefixParameters, keyPrefix);
        foreach (var key in RemoveByPrefixInstanceData(keyPrefix))
            await RemoveAsync(key, false);
			_logger.LogDebug("<<< Exiting RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
    }
}