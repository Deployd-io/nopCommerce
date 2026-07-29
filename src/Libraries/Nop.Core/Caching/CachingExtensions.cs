namespace Nop.Core.Caching;

public static class CachingExtensions
{
    /// <summary>
    /// Get a cached item. If it's not in the cache yet, then load and cache it.
    /// NOTE: this method is only kept for backwards compatibility: the async overload is preferred!
    /// </summary>
    /// <typeparam name="T">Type of cached item</typeparam>
    /// <param name="cacheManager">Cache manager</param>
    /// <param name="key">Cache key</param>
    /// <param name="acquire">Function to load item if it's not in the cache yet</param>
    /// <returns>The cached value associated with the specified key</returns>
    public static T Get<T>(this IStaticCacheManager cacheManager, CacheKey key, Func<T> acquire)
    {
		logger.LogDebug(">>> Entering Get(cacheManager={CacheManager},key={Key},acquire={Acquire})", cacheManager, key, acquire);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return cacheManager.GetAsync(key, acquire).GetAwaiter().GetResult();
    }

	logger.LogInformation("Get(cacheManager,key,acquire)={CacheManager},{Key},{Acquire}: external service call took {Arg3} ms", cacheManager, key, acquire, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Remove items by cache key prefix
    /// </summary>
    /// <param name="cacheManager">Cache manager</param>
    /// <param name="prefix">Cache key prefix</param>
    /// <param name="prefixParameters">Parameters to create cache key prefix</param>
    public static void RemoveByPrefix(this IStaticCacheManager cacheManager, string prefix, params object[] prefixParameters)
    {
		logger.LogDebug(">>> Entering RemoveByPrefix(cacheManager={CacheManager},prefix={Prefix},prefixParameters={PrefixParameters})", cacheManager, prefix, prefixParameters);
        cacheManager.RemoveByPrefixAsync(prefix, prefixParameters).Wait();
		logger.LogDebug("<<< Exiting RemoveByPrefix(cacheManager={CacheManager},prefix={Prefix},prefixParameters={PrefixParameters})", cacheManager, prefix, prefixParameters);
    }
}