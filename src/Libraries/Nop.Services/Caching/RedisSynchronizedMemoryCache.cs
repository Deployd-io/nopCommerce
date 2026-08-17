using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Caching;

/// <summary>
/// Represents a local in-memory cache with distributed synchronization by Redis
/// </summary>
/// <remarks>
/// This class should be registered on IoC as singleton instance
/// </remarks>
public partial class RedisSynchronizedMemoryCache : ISynchronizedMemoryCache
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected const string CLEAR_CACHE_EVENT_KEY = "__NOP_CLEAR_CACHE__";
    protected const string REMOVE_BY_PREFIX_EVENT_KEY = "__NOP_REMOVE_BY_PREFIX_";

    protected readonly string _processId;
    protected bool _disposed;

    /// <summary>
    /// Holds the keys known by this nopCommerce instance
    /// </summary>
    protected readonly ICacheKeyManager _keyManager;
    protected readonly IMemoryCache _memoryCache;
    protected readonly IRedisConnectionWrapper _connection;
    protected readonly ConcurrentQueue<string> _messageQueue = new();
    protected readonly Timer _timer;

    #endregion

    #region Ctor

    public RedisSynchronizedMemoryCache(IMemoryCache memoryCache,
        IRedisConnectionWrapper connectionWrapper,
        ICacheKeyManager cacheKeyManager,
        AppSettings appSettings)
    {
		_logger.LogDebug(">>> Entering RedisSynchronizedMemoryCache(memoryCache={MemoryCache},connectionWrapper={ConnectionWrapper},cacheKeyManager={CacheKeyManager},appSettings={AppSettings})", memoryCache, connectionWrapper, cacheKeyManager, appSettings);
        _processId = $"{Guid.NewGuid()}:{Environment.ProcessId}";

		_logger.LogDebug("RedisSynchronizedMemoryCache(memoryCache={MemoryCache},connectionWrapper={ConnectionWrapper},cacheKeyManager={CacheKeyManager},appSettings={AppSettings}): _processId → {_processId}", memoryCache, connectionWrapper, cacheKeyManager, appSettings, _processId);
        _memoryCache = memoryCache;
		_logger.LogDebug("RedisSynchronizedMemoryCache(memoryCache={MemoryCache},connectionWrapper={ConnectionWrapper},cacheKeyManager={CacheKeyManager},appSettings={AppSettings}): _memoryCache → {_memoryCache}", memoryCache, connectionWrapper, cacheKeyManager, appSettings, _memoryCache);
        _keyManager = cacheKeyManager;
		_logger.LogDebug("RedisSynchronizedMemoryCache(memoryCache={MemoryCache},connectionWrapper={ConnectionWrapper},cacheKeyManager={CacheKeyManager},appSettings={AppSettings}): _keyManager → {_keyManager}", memoryCache, connectionWrapper, cacheKeyManager, appSettings, _keyManager);
        _connection = connectionWrapper;
		_logger.LogDebug("RedisSynchronizedMemoryCache(memoryCache={MemoryCache},connectionWrapper={ConnectionWrapper},cacheKeyManager={CacheKeyManager},appSettings={AppSettings}): _connection → {_connection}", memoryCache, connectionWrapper, cacheKeyManager, appSettings, _connection);

        var publishIntervalMs = appSettings.Get<DistributedCacheConfig>().PublishIntervalMs;
		_logger.LogDebug("RedisSynchronizedMemoryCache(memoryCache={MemoryCache},connectionWrapper={ConnectionWrapper},cacheKeyManager={CacheKeyManager},appSettings={AppSettings}): publishIntervalMs → {PublishIntervalMs}", memoryCache, connectionWrapper, cacheKeyManager, appSettings, publishIntervalMs);
        if (publishIntervalMs > 0)
        {
            var timeSpan = TimeSpan.FromMilliseconds(publishIntervalMs);
			_logger.LogDebug("RedisSynchronizedMemoryCache(memoryCache={MemoryCache},connectionWrapper={ConnectionWrapper},cacheKeyManager={CacheKeyManager},appSettings={AppSettings}): timeSpan → {TimeSpan}", memoryCache, connectionWrapper, cacheKeyManager, appSettings, timeSpan);
            _timer = new(_ => PublishQueuedChangeEvents(), null, timeSpan, timeSpan);
			_logger.LogDebug("RedisSynchronizedMemoryCache(memoryCache={MemoryCache},connectionWrapper={ConnectionWrapper},cacheKeyManager={CacheKeyManager},appSettings={AppSettings}): _timer → {_timer}", memoryCache, connectionWrapper, cacheKeyManager, appSettings, _timer);
        }

        Subscribe();
		_logger.LogDebug("<<< Exiting RedisSynchronizedMemoryCache(memoryCache={MemoryCache},connectionWrapper={ConnectionWrapper},cacheKeyManager={CacheKeyManager},appSettings={AppSettings})", memoryCache, connectionWrapper, cacheKeyManager, appSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Subscribe to perform some operation when a message to the preferred/active node is broadcast
    /// </summary>
    protected virtual void Subscribe()
    {
		_logger.LogDebug(">>> Entering Subscribe()");
        var channel = ChannelPrefix;
        var subscriber = _connection.GetSubscriber();
		_logger.LogDebug("Subscribe(): subscriber → {Subscriber}", subscriber);
        subscriber.Subscribe(RedisChannel.Pattern(channel + "*"), (redisChannel, value) =>
        {
            var publisher = redisChannel.ToString().Replace(channel, "");
			_logger.LogDebug("Subscribe(): publisher → {Publisher}", publisher);

            if (publisher == _processId)
                return;

            var keys = JsonConvert.DeserializeObject<string[]>(value);
			_logger.LogDebug("Subscribe(): keys → {Keys}", keys);

            if (keys == null)
                return;

            if (keys.Any(key => key.Equals(CLEAR_CACHE_EVENT_KEY)))
                foreach (var key in _keyManager.Keys.ToList())
                {
                    _memoryCache.Remove(key);
                    _keyManager.RemoveKey(key);
                }
            else
                foreach (var key in keys)
                {
                    if (key.StartsWith(REMOVE_BY_PREFIX_EVENT_KEY))
                    {
                        var prefix = key[..^2].Replace(REMOVE_BY_PREFIX_EVENT_KEY, string.Empty);
						_logger.LogDebug("Subscribe(): prefix → {Prefix}", prefix);

                        foreach (var keyToDelete in _keyManager.RemoveByPrefix(prefix))
                            _memoryCache.Remove(keyToDelete);
                    }
                    else
                    {
                        _memoryCache.Remove(key);
                        _keyManager.RemoveKey(key);
                    }
                }
        });
		_logger.LogDebug(">>> Entering PublishQueuedChangeEvents()");
		_logger.LogDebug("<<< Exiting PublishQueuedChangeEvents()");
    }

    /// <summary>
    /// Unique channel prefix
    /// </summary>
    protected string ChannelPrefix => $"__change@{_connection.GetDatabase().Database}__{_connection.Instance}__:";

    /// <summary>
    /// Enqueue or publish change event
    /// </summary>
    /// <param name="key">The evicted cache key to be published</param>
    protected virtual void PublishChangeEvent(object key)
    {
		_logger.LogDebug(">>> Entering PublishChangeEvent(key={Key})", key);
        var stringKey = key.ToString();

		_logger.LogDebug("PublishChangeEvent(key={Key}): stringKey → {StringKey}", key, stringKey);
        if (_timer == null)
            BatchPublishChangeEvents(stringKey);
        else
            _messageQueue.Enqueue(stringKey);
			_logger.LogDebug("<<< Exiting PublishChangeEvent(key={Key})", key);
    }

    /// <summary>
    /// Publish accumulated change events on key channel
    /// </summary>
    protected virtual void PublishQueuedChangeEvents()
    {
        IEnumerable<string> getKeys()
        {
            while (_messageQueue.TryDequeue(out var key))
                yield return key;
        }

        BatchPublishChangeEvents(getKeys().Distinct().ToArray());
    }

    /// <summary>
    /// Publish change events on key channel
    /// <param name="keys">The evicted entries to publish on the key channel.</param>
    /// </summary>
    protected virtual void BatchPublishChangeEvents(params string[] keys)
    {
        if (keys.Length == 0)
            return;

        var subscriber = _connection.GetSubscriber();
        subscriber.Publish(RedisChannel.Pattern( 
            $"{ChannelPrefix}{_processId}"),
            JsonConvert.SerializeObject(keys),
            CommandFlags.FireAndForget);
			_logger.LogDebug("<<< Exiting BatchPublishChangeEvents(keys={Keys})", keys);
    }

    /// <summary>
    /// The callback method to run after the entry is evicted
    /// </summary>
    /// <param name="key">The key of the entry being evicted.</param>
    /// <param name="value">The value of the entry being evicted.</param>
    /// <param name="reason">The <see cref="EvictionReason"/>.</param>
    /// <param name="state">The information that was passed when registering the callback.</param>
    protected virtual void OnEviction(object key, object value, EvictionReason reason, object state)
    {
		_logger.LogDebug(">>> Entering OnEviction(key={Key},value={Value},reason={Reason},state={State})", key, value, reason, state);
        switch (reason)
        {
            case EvictionReason.Replaced:
            case EvictionReason.TokenExpired: // e.g. clear cache event
                PublishChangeEvent(key);
                break;
            // don't publish here on removed, as it could be triggered by a redis event itself
            default:
                break;
        }
		_logger.LogDebug("<<< Exiting OnEviction(key={Key},value={Value},reason={Reason},state={State})", key, value, reason, state);
    }

    #endregion

    /// <summary>
    /// Create or overwrite an entry in the cache.
    /// </summary>
    /// <param name="key">An object identifying the entry.</param>
    /// <returns>The newly created <see cref="T:Microsoft.Extensions.Caching.Memory.ICacheEntry" /> instance.</returns>
    public virtual ICacheEntry CreateEntry(object key)
    {
		_logger.LogDebug(">>> Entering CreateEntry(key={Key})", key);
		_logger.LogDebug("<<< Exiting CreateEntry(key={Key})", key);
        return _memoryCache.CreateEntry(key).RegisterPostEvictionCallback(OnEviction);
    }

    /// <summary>
    /// Removes the object associated with the given key.
    /// </summary>
    /// <param name="key">An object identifying the entry.</param>
    public virtual void Remove(object key)
    {
		_logger.LogDebug(">>> Entering Remove(key={Key})", key);
        _memoryCache.Remove(key);

        //publish event manually instead of through eviction callback to avoid feedback loops
        PublishChangeEvent(key);
		_logger.LogDebug("<<< Exiting Remove(key={Key})", key);
    }

    /// <summary>
    /// Gets the item associated with this key if present.
    /// </summary>
    /// <param name="key">An object identifying the requested entry.</param>
    /// <param name="value">The located value or null.</param>
    /// <returns>True if the key was found.</returns>
    public virtual bool TryGetValue(object key, out object value)
    {
		_logger.LogDebug(">>> Entering TryGetValue(key={Key})", key);
		_logger.LogDebug("<<< Exiting TryGetValue(key={Key})", key);
        return _memoryCache.TryGetValue(key, out value);
    }

    /// <summary>
    /// Clear all cache data
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task ClearCacheAsync()
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync()");
        BatchPublishChangeEvents(CLEAR_CACHE_EVENT_KEY);

		_logger.LogDebug("<<< Exiting ClearCacheAsync()");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Remove items by prefix
    /// </summary>
    /// <param name="prefix">Prefix to remove cache items</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task RemoveByPrefixAsync(string prefix)
    {
        BatchPublishChangeEvents($"{REMOVE_BY_PREFIX_EVENT_KEY}{prefix}__");

        return Task.CompletedTask;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public virtual void Dispose()
    {
		_logger.LogDebug(">>> Entering Dispose()");
        if (!_disposed)
        {
            _timer?.Dispose();
            var subscriber = _connection.GetSubscriber();
			_logger.LogDebug("Dispose(): subscriber → {Subscriber}", subscriber);
            subscriber.Unsubscribe(RedisChannel.Pattern(ChannelPrefix + "*"));

            _disposed = true;
			_logger.LogDebug("Dispose(): _disposed → {_disposed}", _disposed);
        }

        GC.SuppressFinalize(this);
		_logger.LogDebug("<<< Exiting Dispose()");
    }
}