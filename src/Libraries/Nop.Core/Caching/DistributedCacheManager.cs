using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Core.Caching;

/// <summary>
/// Represents a base distributed cache 
/// </summary>
public abstract class DistributedCacheManager : CacheKeyService, IStaticCacheManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    /// <summary>
    /// Holds the keys known by this nopCommerce instance
    /// </summary>
    protected readonly ICacheKeyManager _localKeyManager;
    protected readonly IDistributedCache _distributedCache;
    protected readonly IConcurrentCollection<object> _concurrentCollection;

    /// <summary>
    /// Holds ongoing acquisition tasks, used to avoid duplicating work
    /// </summary>
    protected readonly ConcurrentDictionary<string, Lazy<Task<object>>> _ongoing = new();

    #endregion

    #region Ctor

    protected DistributedCacheManager(AppSettings appSettings,
        IDistributedCache distributedCache,
        ICacheKeyManager cacheKeyManager,
        IConcurrentCollection<object> concurrentCollection)
        : base(appSettings)
    {
        _distributedCache = distributedCache;
        _localKeyManager = cacheKeyManager;
        _concurrentCollection = concurrentCollection;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Clear all data on this instance
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual void ClearInstanceData()
    {
		_logger.LogDebug(">>> Entering ClearInstanceData()");
        _concurrentCollection.Clear();
        _localKeyManager.Clear();
		_logger.LogDebug("<<< Exiting ClearInstanceData()");
    }

    /// <summary>
    /// Remove items by cache key prefix
    /// </summary>
    /// <param name="prefix">Cache key prefix</param>
    /// <param name="prefixParameters">Parameters to create cache key prefix</param>
    /// <returns>The removed keys</returns>
    protected virtual IEnumerable<string> RemoveByPrefixInstanceData(string prefix, params object[] prefixParameters)
    {
		_logger.LogDebug(">>> Entering RemoveByPrefixInstanceData(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
        var keyPrefix = PrepareKeyPrefix(prefix, prefixParameters);
        _concurrentCollection.Prune(keyPrefix, out _);
		_logger.LogDebug("RemoveByPrefixInstanceData(prefix={Prefix},prefixParameters={PrefixParameters}): keyPrefix → {KeyPrefix}", prefix, prefixParameters, keyPrefix);

		_logger.LogDebug("<<< Exiting RemoveByPrefixInstanceData(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
        return _localKeyManager.RemoveByPrefix(keyPrefix);
    }

    /// <summary>
    /// Prepare cache entry options for the passed key
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <returns>Cache entry options</returns>
    protected virtual DistributedCacheEntryOptions PrepareEntryOptions(CacheKey key)
    {
		_logger.LogDebug(">>> Entering PrepareEntryOptions(key={Key})", key);
        //set expiration time for the passed cache key
		_logger.LogDebug("<<< Exiting PrepareEntryOptions(key={Key})", key);
        return new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(key.CacheTime)
        };
    }

    /// <summary>
    /// Add the specified key and object to the local cache
    /// </summary>
    /// <param name="key">Key of cached item</param>
    /// <param name="value">Value for caching</param>
    protected virtual void SetLocal(string key, object value)
    {
        _concurrentCollection.Add(key, value);
        _localKeyManager.AddKey(key);
    }

    /// <summary>
    /// Remove the value with the specified key from the cache
    /// </summary>
    /// <param name="key">Cache key</param>
    protected virtual void RemoveLocal(string key)
    {
		_logger.LogDebug(">>> Entering RemoveLocal(key={Key})", key);
        _concurrentCollection.Remove(key);
        _localKeyManager.RemoveKey(key);
		_logger.LogDebug("<<< Exiting RemoveLocal(key={Key})", key);
    }

    /// <summary>
    /// Try get a cached item. If it's not in the cache yet, then return default object
    /// </summary>
    /// <typeparam name="T">Type of cached item</typeparam>
    /// <param name="key">Cache key</param>
    protected virtual async Task<(bool isSet, T item)> TryGetItemAsync<T>(string key)
    {
        var json = await _distributedCache.GetStringAsync(key);

        return string.IsNullOrEmpty(json)
            ? (false, default)
            : (true, item: JsonConvert.DeserializeObject<T>(json));
    }

    /// <summary>
    /// Remove the value with the specified key from the cache
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="removeFromInstance">Remove from instance</param>
    protected virtual async Task RemoveAsync(string key, bool removeFromInstance = true)
    {
		_logger.LogDebug(">>> Entering RemoveAsync(key={Key},removeFromInstance={RemoveFromInstance})", key, removeFromInstance);
        _ongoing.TryRemove(key, out _);
        await _distributedCache.RemoveAsync(key);

        if (!removeFromInstance)
            return;

        RemoveLocal(key);
		_logger.LogDebug("<<< Exiting RemoveAsync(key={Key},removeFromInstance={RemoveFromInstance})", key, removeFromInstance);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Remove the value with the specified key from the cache
    /// </summary>
    /// <param name="cacheKey">Cache key</param>
    /// <param name="cacheKeyParameters">Parameters to create cache key</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task RemoveAsync(CacheKey cacheKey, params object[] cacheKeyParameters)
    {
		_logger.LogDebug(">>> Entering RemoveAsync(cacheKey={CacheKey},cacheKeyParameters={CacheKeyParameters})", cacheKey, cacheKeyParameters);
        await RemoveAsync(PrepareKey(cacheKey, cacheKeyParameters).Key);
    }

    /// <summary>
    /// Get a cached item. If it's not in the cache yet, then load and cache it
    /// </summary>
    /// <typeparam name="T">Type of cached item</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="acquire">Function to load item if it's not in the cache yet</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cached value associated with the specified key
    /// </returns>
    public virtual async Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire)
    {
		_logger.LogDebug(">>> Entering GetAsync(key={Key},acquire={Acquire})", key, acquire);
        if (_concurrentCollection.TryGetValue(key.Key, out var data))
            return (T)data;

        var lazy = _ongoing.GetOrAdd(key.Key, _ => new(async () => await acquire(), true));
        var setTask = Task.CompletedTask;
		_logger.LogDebug("GetAsync(key={Key},acquire={Acquire}): lazy → {Lazy}", key, acquire, lazy);

        try
        {
            if (lazy.IsValueCreated)
                return (T)await lazy.Value;

            var (isSet, item) = await TryGetItemAsync<T>(key.Key);
            if (!isSet)
            {
                item = (T)await lazy.Value;
				_logger.LogDebug("GetAsync(key={Key},acquire={Acquire}): item → {Item}", key, acquire, item);

                if (key.CacheTime == 0 || item == null)
                    return item;

                setTask = _distributedCache.SetStringAsync(
                    key.Key,
                    JsonConvert.SerializeObject(item),
                    PrepareEntryOptions(key));
            }

            SetLocal(key.Key, item);

            return item;
        }
        finally
        {
            _ = setTask.ContinueWith(_ => _ongoing.TryRemove(new KeyValuePair<string, Lazy<Task<object>>>(key.Key, lazy)));
        }
    }

    /// <summary>
    /// Get a cached item. If it's not in the cache yet, then load and cache it
    /// </summary>
    /// <typeparam name="T">Type of cached item</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="acquire">Function to load item if it's not in the cache yet</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cached value associated with the specified key
    /// </returns>
    public virtual async Task<T> GetAsync<T>(CacheKey key, Func<T> acquire)
    {
        return await GetAsync(key, () => Task.FromResult(acquire()));
    }

    public virtual async Task<T> GetAsync<T>(CacheKey key, T defaultValue = default)
    {
		_logger.LogDebug(">>> Entering GetAsync(key={Key},defaultValue={DefaultValue})", key, defaultValue);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var value = await _distributedCache.GetStringAsync(key.Key);

		_logger.LogDebug("GetAsync(key={Key},defaultValue={DefaultValue}): value → {Value}", key, defaultValue, value);
        return value != null
            ? JsonConvert.DeserializeObject<T>(value)
            : defaultValue;
			_logger.LogInformation("GetAsync(key,defaultValue)={Key},{DefaultValue}: external service call took {Arg2} ms", key, defaultValue, __logStart.ElapsedMilliseconds);
			_logger.LogDebug("<<< Exiting GetAsync(key={Key})", key);
    }

    /// <summary>
    /// Get a cached item as an <see cref="object"/> instance, or null on a cache miss.
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cached value associated with the specified key, or null if none was found
    /// </returns>
    public virtual async Task<object> GetAsync(CacheKey key)
    {
		_logger.LogDebug("<<< Exiting GetAsync(key={Key})", key);
        return await GetAsync<object>(key);
    }

    /// <summary>
    /// Add the specified key and object to the cache
    /// </summary>
    /// <param name="key">Key of cached item</param>
    /// <param name="data">Value for caching</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetAsync<T>(CacheKey key, T data)
    {
		_logger.LogDebug(">>> Entering SetAsync(key={Key},data={Data})", key, data);
        if (data == null || (key?.CacheTime ?? 0) <= 0)
			_logger.LogDebug("<<< Exiting SetAsync(key={Key},data={Data})", key, data);
            return;

        var lazy = new Lazy<Task<object>>(() => Task.FromResult(data as object), true);

		_logger.LogDebug("SetAsync(key={Key},data={Data}): lazy → {Lazy}", key, data, lazy);
        try
        {
            _ongoing.TryAdd(key.Key, lazy);
            // await the lazy task in order to force value creation instead of directly setting data
            // this way, other cache manager instances can access it while it is being set
            SetLocal(key.Key, await lazy.Value);
            await _distributedCache.SetStringAsync(key.Key, JsonConvert.SerializeObject(data), PrepareEntryOptions(key));
        }
        finally
        {
            _ongoing.TryRemove(new KeyValuePair<string, Lazy<Task<object>>>(key.Key, lazy));
        }
    }

    /// <summary>
    /// Remove items by cache key prefix
    /// </summary>
    /// <param name="prefix">Cache key prefix</param>
    /// <param name="prefixParameters">Parameters to create cache key prefix</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public abstract Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters);

    /// <summary>
    /// Clear all cache data
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public abstract Task ClearAsync();

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public virtual void Dispose()
    {
		_logger.LogDebug(">>> Entering Dispose()");
        GC.SuppressFinalize(this);
		_logger.LogDebug("<<< Exiting Dispose()");
    }

    #endregion
}