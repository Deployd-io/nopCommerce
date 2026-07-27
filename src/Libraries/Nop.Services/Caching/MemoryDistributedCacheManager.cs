using Microsoft.Extensions.Caching.Distributed;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace Nop.Services.Caching;

public partial class MemoryDistributedCacheManager : DistributedCacheManager
{
    #region Ctor

    public MemoryDistributedCacheManager(AppSettings appSettings,
        IDistributedCache distributedCache,
        ICacheKeyManager cacheKeyManager,
        IConcurrentCollection<object> concurrentCollection)
        : base(appSettings, distributedCache, cacheKeyManager, concurrentCollection)
    {
    }

    #endregion

    /// <summary>
    /// Clear all cache data
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task ClearAsync()
    {
		logger.LogDebug(">>> Entering ClearAsync()");
        foreach (var key in _localKeyManager.Keys)
            await RemoveAsync(key, false);

        ClearInstanceData();
		logger.LogDebug("<<< Exiting ClearAsync()");
    }

    /// <summary>
    /// Remove items by cache key prefix
    /// </summary>
    /// <param name="prefix">Cache key prefix</param>
    /// <param name="prefixParameters">Parameters to create cache key prefix</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
    {
		logger.LogDebug(">>> Entering RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
        var keyPrefix = PrepareKeyPrefix(prefix, prefixParameters);

		logger.LogDebug("RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters}): keyPrefix → {KeyPrefix}", prefix, prefixParameters, keyPrefix);
        foreach (var key in RemoveByPrefixInstanceData(keyPrefix))
            await RemoveAsync(key, false);
			logger.LogDebug("<<< Exiting RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
    }
}