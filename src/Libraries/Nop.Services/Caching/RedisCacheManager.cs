using System.Net;
using Microsoft.Extensions.Caching.Distributed;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Caching;

/// <summary>
/// Represents a redis distributed cache 
/// </summary>
public partial class RedisCacheManager : DistributedCacheManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRedisConnectionWrapper _connectionWrapper;

    #endregion

    #region Ctor

    public RedisCacheManager(AppSettings appSettings,
        IDistributedCache distributedCache,
        IRedisConnectionWrapper connectionWrapper,
        ICacheKeyManager cacheKeyManager,
        IConcurrentCollection<object> concurrentCollection)
        : base(appSettings, distributedCache, cacheKeyManager, concurrentCollection)
    {
		_logger.LogDebug(">>> Entering RedisCacheManager(appSettings={AppSettings},distributedCache={DistributedCache},connectionWrapper={ConnectionWrapper},cacheKeyManager={CacheKeyManager},concurrentCollection={ConcurrentCollection})", appSettings, distributedCache, connectionWrapper, cacheKeyManager, concurrentCollection);
        _connectionWrapper = connectionWrapper;
		_logger.LogDebug("<<< Exiting RedisCacheManager(appSettings={AppSettings},distributedCache={DistributedCache},connectionWrapper={ConnectionWrapper},cacheKeyManager={CacheKeyManager},concurrentCollection={ConcurrentCollection})", appSettings, distributedCache, connectionWrapper, cacheKeyManager, concurrentCollection);
		_logger.LogDebug("RedisCacheManager(appSettings={AppSettings},distributedCache={DistributedCache},connectionWrapper={ConnectionWrapper},cacheKeyManager={CacheKeyManager},concurrentCollection={ConcurrentCollection}): _connectionWrapper → {_connectionWrapper}", appSettings, distributedCache, connectionWrapper, cacheKeyManager, concurrentCollection, _connectionWrapper);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets the list of cache keys prefix
    /// </summary>
    /// <param name="endPoint">Network address</param>
    /// <param name="prefix">String key pattern</param>
    /// <returns>List of cache keys</returns>
    protected virtual async Task<IEnumerable<RedisKey>> GetKeysAsync(EndPoint endPoint, string prefix = null)
    {
		_logger.LogDebug(">>> Entering GetKeysAsync(endPoint={EndPoint},prefix={Prefix})", endPoint, prefix);
		_logger.LogDebug("<<< Exiting GetKeysAsync(endPoint={EndPoint},prefix={Prefix})", endPoint, prefix);
        return await (await _connectionWrapper.GetServerAsync(endPoint))
            .KeysAsync((await _connectionWrapper.GetDatabaseAsync()).Database, string.IsNullOrEmpty(prefix) ? null : $"{prefix}*")
            .ToListAsync();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Remove items by cache key prefix
    /// </summary>
    /// <param name="prefix">Cache key prefix</param>
    /// <param name="prefixParameters">Parameters to create cache key prefix</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
    {
		_logger.LogDebug(">>> Entering RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
        prefix = PrepareKeyPrefix(prefix, prefixParameters);
        var db = await _connectionWrapper.GetDatabaseAsync();
		_logger.LogDebug("RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters}): db → {Db}", prefix, prefixParameters, db);
		_logger.LogDebug("RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters}): prefix → {Prefix1}", prefix, prefixParameters, prefix);

        var instanceName = _appSettings.Get<DistributedCacheConfig>().InstanceName ?? string.Empty;
		_logger.LogDebug("RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters}): instanceName → {InstanceName}", prefix, prefixParameters, instanceName);

        foreach (var endPoint in await _connectionWrapper.GetEndPointsAsync())
        {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
            var keys = await GetKeysAsync(endPoint, instanceName + prefix);
            await db.KeyDeleteAsync(keys.ToArray());
        }
		_logger.LogDebug("RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters}): keys → {Keys}", prefix, prefixParameters, keys);
		_logger.LogInformation("RemoveByPrefixAsync(prefix,prefixParameters)={Prefix},{PrefixParameters}: database query executed in {Arg2} ms", prefix, prefixParameters, __logStart.ElapsedMilliseconds);

        RemoveByPrefixInstanceData(prefix);
		_logger.LogDebug("<<< Exiting RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
    }

    /// <summary>
    /// Clear all cache data
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task ClearAsync()
    {
		_logger.LogDebug(">>> Entering ClearAsync()");
        await _connectionWrapper.FlushDatabaseAsync();

        ClearInstanceData();
		_logger.LogDebug("<<< Exiting ClearAsync()");
    }

    #endregion
}