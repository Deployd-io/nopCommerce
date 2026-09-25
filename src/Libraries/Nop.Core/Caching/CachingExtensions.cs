using Microsoft.Extensions.Logging;
namespace Nop.Core.Caching;

public static class CachingExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogError(">>> Entering Get(cacheManager={CacheManager},key={Key},acquire={Acquire})", cacheManager, key, acquire);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("Get(cacheManager,key,acquire)={CacheManager},{Key},{Acquire}: external service call took {Arg3} ms", cacheManager, key, acquire, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting Get(cacheManager={CacheManager},key={Key},acquire={Acquire})", cacheManager, key, acquire);
        return cacheManager.GetAsync(key, acquire).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Remove items by cache key prefix
    /// </summary>
    /// <param name="cacheManager">Cache manager</param>
    /// <param name="prefix">Cache key prefix</param>
    /// <param name="prefixParameters">Parameters to create cache key prefix</param>
    public static void RemoveByPrefix(this IStaticCacheManager cacheManager, string prefix, params object[] prefixParameters)
    {
		_logger.LogError(">>> Entering RemoveByPrefix(cacheManager={CacheManager},prefix={Prefix},prefixParameters={PrefixParameters})", cacheManager, prefix, prefixParameters);
        cacheManager.RemoveByPrefixAsync(prefix, prefixParameters).Wait();
		_logger.LogWarning("<<< Exiting RemoveByPrefix(cacheManager={CacheManager},prefix={Prefix},prefixParameters={PrefixParameters})", cacheManager, prefix, prefixParameters);
    }
}