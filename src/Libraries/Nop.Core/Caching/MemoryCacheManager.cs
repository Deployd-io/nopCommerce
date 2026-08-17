using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Nop.Core.Configuration;
using Microsoft.Extensions.Logging;

namespace Nop.Core.Caching;

/// <summary>
/// Represents a memory cache manager 
/// </summary>
/// <remarks>
/// This class should be registered on IoC as singleton instance
/// </remarks>
public partial class MemoryCacheManager : CacheKeyService, IStaticCacheManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    // Flag: Has Dispose already been called?
    protected bool _disposed;

    protected readonly IMemoryCache _memoryCache;

    /// <summary>
    /// Holds the keys known by this nopCommerce instance
    /// </summary>
    protected readonly ICacheKeyManager _keyManager;

    protected CancellationTokenSource _clearToken = new();

    #endregion

    #region Ctor

    public MemoryCacheManager(AppSettings appSettings, IMemoryCache memoryCache, ICacheKeyManager cacheKeyManager)
        : base(appSettings)
    {
		_logger.LogDebug(">>> Entering MemoryCacheManager(appSettings={AppSettings},memoryCache={MemoryCache},cacheKeyManager={CacheKeyManager})", appSettings, memoryCache, cacheKeyManager);
        _memoryCache = memoryCache;
        _keyManager = cacheKeyManager;
		_logger.LogDebug("<<< Exiting MemoryCacheManager(appSettings={AppSettings},memoryCache={MemoryCache},cacheKeyManager={CacheKeyManager})", appSettings, memoryCache, cacheKeyManager);
		_logger.LogDebug("MemoryCacheManager(appSettings={AppSettings},memoryCache={MemoryCache},cacheKeyManager={CacheKeyManager}): _keyManager → {_keyManager}", appSettings, memoryCache, cacheKeyManager, _keyManager);
		_logger.LogDebug("MemoryCacheManager(appSettings={AppSettings},memoryCache={MemoryCache},cacheKeyManager={CacheKeyManager}): _memoryCache → {_memoryCache}", appSettings, memoryCache, cacheKeyManager, _memoryCache);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare cache entry options for the passed key
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <returns>Cache entry options</returns>
    protected virtual MemoryCacheEntryOptions PrepareEntryOptions(CacheKey key)
    {
        //set expiration time for the passed cache key
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(key.CacheTime)
        };

        //add token to clear cache entries
        options.AddExpirationToken(new CancellationChangeToken(_clearToken.Token));
        options.RegisterPostEvictionCallback(OnEviction);
        _keyManager.AddKey(key.Key);

        return options;
    }

    /// <summary>
    /// The callback method which gets called when a cache entry expires.
    /// </summary>
    /// <param name="key">The key of the entry being evicted.</param>
    /// <param name="value">The value of the entry being evicted.</param>
    /// <param name="reason">The <see cref="EvictionReason"/>.</param>
    /// <param name="state">The information that was passed when registering the callback.</param>
    protected virtual void OnEviction(object key, object value, EvictionReason reason, object state)
    {
        switch (reason)
        {
            // we clean up after ourselves elsewhere
            case EvictionReason.Removed:
            case EvictionReason.Replaced:
            case EvictionReason.TokenExpired:
                break;
            //if the entry was evicted by the cache itself, we remove the key
            default:
                //checks if the eviction callback happens after the item is re-added to the cache to prevent the erroneously removing entry from the key manager
                if (!_memoryCache.TryGetValue(key, out _))
                    _keyManager.RemoveKey(key as string);
                break;
        }
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
		_logger.LogDebug(">>> Entering Dispose(disposing={Disposing})", disposing);
        if (_disposed)
            return;

        if (disposing)
            // don't dispose of the MemoryCache, as it is injected
            _clearToken.Dispose();

        _disposed = true;
		_logger.LogDebug("Dispose(disposing={Disposing}): _disposed → {_disposed}", disposing, _disposed);
		_logger.LogDebug(">>> Entering Dispose()");
		_logger.LogDebug("<<< Exiting Dispose(disposing={Disposing})", disposing);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Remove the value with the specified key from the cache
    /// </summary>
    /// <param name="cacheKey">Cache key</param>
    /// <param name="cacheKeyParameters">Parameters to create cache key</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task RemoveAsync(CacheKey cacheKey, params object[] cacheKeyParameters)
    {
		_logger.LogDebug(">>> Entering RemoveAsync(cacheKey={CacheKey},cacheKeyParameters={CacheKeyParameters})", cacheKey, cacheKeyParameters);
        var key = PrepareKey(cacheKey, cacheKeyParameters).Key;
        _memoryCache.Remove(key);
		_logger.LogWarning("RemoveAsync(cacheKey={CacheKey},cacheKeyParameters={CacheKeyParameters}): key → {Key}", cacheKey, cacheKeyParameters, key);
        _keyManager.RemoveKey(key);

		_logger.LogDebug("<<< Exiting RemoveAsync(cacheKey={CacheKey},cacheKeyParameters={CacheKeyParameters})", cacheKey, cacheKeyParameters);
        return Task.CompletedTask;
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
        if ((key?.CacheTime ?? 0) <= 0)
            return await acquire();

        var task = _memoryCache.GetOrCreate(
            key.Key,
            entry =>
            {
			_logger.LogDebug("GetAsync(key={Key},acquire={Acquire}): entry → {Entry}", key, acquire, entry);
                entry.SetOptions(PrepareEntryOptions(key));
                return new Lazy<Task<T>>(acquire, true);
            });

        try
        {
            var data = await task!.Value;
			_logger.LogDebug("GetAsync(key={Key},acquire={Acquire}): data → {Data}", key, acquire, data);

            //if a cached function return null, remove it from the cache
            if (data == null)
                await RemoveAsync(key);

            return data;
        }
        catch (Exception ex)
        {
			_logger.LogError("Exception in GetAsync(key={Key},acquire={Acquire}): {Arg2}", key, acquire, ex.Message);
            //if a cached function throws an exception, remove it from the cache
            await RemoveAsync(key);

            if (ex is NullReferenceException)
                return default;

            throw;
        }
    }

    /// <summary>
    /// Get a cached item. If it's not in the cache yet, return a default value
    /// </summary>
    /// <typeparam name="T">Type of cached item</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="defaultValue">A default value to return if the key is not present in the cache</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cached value associated with the specified key, or the default value if none was found
    /// </returns>
    public virtual async Task<T> GetAsync<T>(CacheKey key, T defaultValue = default)
    {
        var value = _memoryCache.Get<Lazy<Task<T>>>(key.Key)?.Value;

        try
        {
            return value != null ? await value : defaultValue;
        }
        catch
        {
            //if a cached function throws an exception, remove it from the cache
            await RemoveAsync(key);

            throw;
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
        var entry = _memoryCache.Get(key.Key);
        if (entry == null)
            return null;
        try
        {
            if (entry.GetType().GetProperty("Value")?.GetValue(entry) is not Task task)
                return null;

            await task;

            return task.GetType().GetProperty("Result")!.GetValue(task);
        }
        catch
        {
            //if a cached function throws an exception, remove it from the cache
            await RemoveAsync(key);

            throw;
        }
    }

    /// <summary>
    /// Add the specified key and object to the cache
    /// </summary>
    /// <param name="key">Key of cached item</param>
    /// <param name="data">Value for caching</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task SetAsync<T>(CacheKey key, T data)
    {
		_logger.LogDebug(">>> Entering SetAsync(key={Key},data={Data})", key, data);
        if (data != null && (key?.CacheTime ?? 0) > 0)
            _memoryCache.Set(
                key.Key,
                new Lazy<Task<T>>(() => Task.FromResult(data), true),
                PrepareEntryOptions(key));

		_logger.LogDebug("<<< Exiting SetAsync(key={Key},data={Data})", key, data);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Remove items by cache key prefix
    /// </summary>
    /// <param name="prefix">Cache key prefix</param>
    /// <param name="prefixParameters">Parameters to create cache key prefix</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
    {
		_logger.LogDebug(">>> Entering RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
        var deletePrefix = PrepareKeyPrefix(prefix, prefixParameters);
        foreach (var key in _keyManager.RemoveByPrefix(deletePrefix))
            _memoryCache.Remove(key);
			_logger.LogDebug("RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters}): deletePrefix → {DeletePrefix}", prefix, prefixParameters, deletePrefix);

        if (_memoryCache is ISynchronizedMemoryCache cache)
            await cache.RemoveByPrefixAsync(deletePrefix);
			_logger.LogDebug("<<< Exiting RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
    }

    /// <summary>
    /// Clear all cache data
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ClearAsync()
    {
		_logger.LogDebug(">>> Entering ClearAsync()");
        await _clearToken.CancelAsync();
        _clearToken.Dispose();
        _clearToken = new CancellationTokenSource();
		_logger.LogDebug("ClearAsync(): _clearToken → {_clearToken}", _clearToken);
        _keyManager.Clear();

        if (_memoryCache is ISynchronizedMemoryCache cache)
            await cache.ClearCacheAsync();
			_logger.LogDebug("<<< Exiting ClearAsync()");
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}