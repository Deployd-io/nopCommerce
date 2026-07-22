using System.Net;
using Microsoft.Extensions.Caching.Distributed;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using StackExchange.Redis;

namespace Nop.Services.Caching;

/// <summary>
/// Represents a redis distributed cache 
/// </summary>
public partial class RedisCacheManager : DistributedCacheManager
{
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
        _connectionWrapper = connectionWrapper;
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
		logger.LogDebug(">>> Entering GetKeysAsync(endPoint={EndPoint},prefix={Prefix})", endPoint, prefix);
		logger.LogDebug("<<< Exiting GetKeysAsync(endPoint={EndPoint},prefix={Prefix})", endPoint, prefix);
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
		logger.LogDebug(">>> Entering RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
        prefix = PrepareKeyPrefix(prefix, prefixParameters);
        var db = await _connectionWrapper.GetDatabaseAsync();
		logger.LogDebug("RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters}): prefix → {Prefix1}", prefix, prefixParameters, prefix);
		logger.LogDebug("RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters}): db → {Db}", prefix, prefixParameters, db);

        var instanceName = _appSettings.Get<DistributedCacheConfig>().InstanceName ?? string.Empty;

		logger.LogDebug("RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters}): instanceName → {InstanceName}", prefix, prefixParameters, instanceName);
        foreach (var endPoint in await _connectionWrapper.GetEndPointsAsync())
        {
            var keys = await GetKeysAsync(endPoint, instanceName + prefix);
            await db.KeyDeleteAsync(keys.ToArray());
			logger.LogDebug("RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters}): keys → {Keys}", prefix, prefixParameters, keys);
        }

        RemoveByPrefixInstanceData(prefix);
		logger.LogDebug("<<< Exiting RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
    }

    /// <summary>
    /// Clear all cache data
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task ClearAsync()
    {
		logger.LogDebug(">>> Entering ClearAsync()");
        await _connectionWrapper.FlushDatabaseAsync();

        ClearInstanceData();
		logger.LogDebug("<<< Exiting ClearAsync()");
    }

    #endregion
}
