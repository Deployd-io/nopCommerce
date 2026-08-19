using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Core.Caching;

/// <summary>
/// Represents a per request cache manager
/// </summary>
public partial class PerRequestCacheManager : CacheKeyService, IShortTermCacheManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ConcurrentTrie<object> _concurrentCollection;

    #endregion

    #region Ctor

    public PerRequestCacheManager(AppSettings appSettings) : base(appSettings)
    {
		_logger.LogDebug(">>> Entering PerRequestCacheManager(appSettings={AppSettings})", appSettings);
        _concurrentCollection = new ConcurrentTrie<object>();
		_logger.LogDebug("PerRequestCacheManager(appSettings={AppSettings}): _concurrentCollection → {_concurrentCollection}", appSettings, _concurrentCollection);
		_logger.LogDebug("<<< Exiting PerRequestCacheManager(appSettings={AppSettings})", appSettings);
    }

    #endregion

    #region Methods
        
    /// <summary>
    /// Get a cached item. If it's not in the cache yet, then load and cache it
    /// </summary>
    /// <typeparam name="T">Type of cached item</typeparam>
    /// /// <param name="acquire">Function to load item if it's not in the cache yet</param>
    /// <param name="cacheKey">Initial cache key</param>
    /// <param name="cacheKeyParameters">Parameters to create cache key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cached value associated with the specified key
    /// </returns>
    public virtual async Task<T> GetAsync<T>(Func<Task<T>> acquire, CacheKey cacheKey, params object[] cacheKeyParameters)
    {
		_logger.LogDebug(">>> Entering GetAsync(acquire={Acquire},cacheKey={CacheKey},cacheKeyParameters={CacheKeyParameters})", acquire, cacheKey, cacheKeyParameters);
        var key = cacheKey.Create(CreateCacheKeyParameters, cacheKeyParameters).Key;

		_logger.LogWarning("GetAsync(acquire={Acquire},cacheKey={CacheKey},cacheKeyParameters={CacheKeyParameters}): key → {Key}", acquire, cacheKey, cacheKeyParameters, key);
        if (_concurrentCollection.TryGetValue(key, out var data))
            return (T)data;

        var result = await acquire();
		_logger.LogDebug("GetAsync(acquire={Acquire},cacheKey={CacheKey},cacheKeyParameters={CacheKeyParameters}): result → {Result}", acquire, cacheKey, cacheKeyParameters, result);

        if (result != null)
            _concurrentCollection.Add(key, result);

		_logger.LogDebug("<<< Exiting GetAsync(acquire={Acquire},cacheKey={CacheKey},cacheKeyParameters={CacheKeyParameters})", acquire, cacheKey, cacheKeyParameters);
        return result;
    }
        
    /// <summary>
    /// Remove items by cache key prefix
    /// </summary>
    /// <param name="prefix">Cache key prefix</param>
    /// <param name="prefixParameters">Parameters to create cache key prefix</param>
    public virtual void RemoveByPrefix(string prefix, params object[] prefixParameters)
    {
		_logger.LogDebug(">>> Entering RemoveByPrefix(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
        var keyPrefix = PrepareKeyPrefix(prefix, prefixParameters);
        _concurrentCollection.Prune(keyPrefix, out _);
		_logger.LogDebug("RemoveByPrefix(prefix={Prefix},prefixParameters={PrefixParameters}): keyPrefix → {KeyPrefix}", prefix, prefixParameters, keyPrefix);
		_logger.LogDebug("<<< Exiting RemoveByPrefix(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
    }
        
    /// <summary>
    /// Remove the value with the specified key from the cache
    /// </summary>
    /// <param name="cacheKey">Cache key</param>
    /// <param name="cacheKeyParameters">Parameters to create cache key</param>
    public virtual void Remove(string cacheKey, params object[] cacheKeyParameters)
    {
		_logger.LogDebug(">>> Entering Remove(cacheKey={CacheKey},cacheKeyParameters={CacheKeyParameters})", cacheKey, cacheKeyParameters);
        _concurrentCollection.Remove(PrepareKey(new CacheKey(cacheKey), cacheKeyParameters).Key);
		_logger.LogDebug("<<< Exiting Remove(cacheKey={CacheKey},cacheKeyParameters={CacheKeyParameters})", cacheKey, cacheKeyParameters);
    }

    #endregion
}