using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Distributed;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Caching;

/// <summary>
/// Represents a MsSql server distributed cache 
/// </summary>
public partial class MsSqlServerCacheManager : DistributedCacheManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly DistributedCacheConfig _distributedCacheConfig;

    #endregion

    #region Ctor

    public MsSqlServerCacheManager(AppSettings appSettings,
        IDistributedCache distributedCache,
        ICacheKeyManager cacheKeyManager,
        IConcurrentCollection<object> concurrentCollection)
        : base(appSettings, distributedCache, cacheKeyManager, concurrentCollection)
    {
		_logger.LogError(">>> Entering MsSqlServerCacheManager(appSettings={AppSettings},distributedCache={DistributedCache},cacheKeyManager={CacheKeyManager},concurrentCollection={ConcurrentCollection})", appSettings, distributedCache, cacheKeyManager, concurrentCollection);
        _distributedCacheConfig = appSettings.Get<DistributedCacheConfig>();
		_logger.LogDebug("MsSqlServerCacheManager(appSettings={AppSettings},distributedCache={DistributedCache},cacheKeyManager={CacheKeyManager},concurrentCollection={ConcurrentCollection}): _distributedCacheConfig → {_distributedCacheConfig}", appSettings, distributedCache, cacheKeyManager, concurrentCollection, _distributedCacheConfig);
    _logger.LogError("MsSqlServerCacheManager(appSettings={AppSettings},distributedCache={DistributedCache},cacheKeyManager={CacheKeyManager},concurrentCollection={ConcurrentCollection}): _distributedCacheConfig → {_distributedCacheConfig}", appSettings, distributedCache, cacheKeyManager, concurrentCollection, _distributedCacheConfig);
    _logger.LogWarning("<<< Exiting MsSqlServerCacheManager(appSettings={AppSettings},distributedCache={DistributedCache},cacheKeyManager={CacheKeyManager},concurrentCollection={ConcurrentCollection})", appSettings, distributedCache, cacheKeyManager, concurrentCollection);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Performs SQL command
    /// </summary>
    /// <param name="command">SQL command</param>
    /// <param name="parameters">Parameters for SQL command</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PerformActionAsync(SqlCommand command, params SqlParameter[] parameters)
    {
		_logger.LogError(">>> Entering PerformActionAsync(command={Command},parameters={Parameters})", command, parameters);
        var conn = new SqlConnection(_distributedCacheConfig.ConnectionString);

		_logger.LogError("PerformActionAsync(command={Command},parameters={Parameters}): conn → {Conn}", command, parameters, conn);
        try
        {
            await conn.OpenAsync();
            command.Connection = conn;
			_logger.LogDebug("PerformActionAsync(command={Command},parameters={Parameters}): command.Connection → {Arg2}", command, parameters, command.Connection);
            if (parameters.Any())
                command.Parameters.AddRange(parameters);

			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogError("PerformActionAsync(command,parameters)={Command},{Parameters}: database query executed in {Arg2} ms", command, parameters, __logStart.ElapsedMilliseconds);
            await command.ExecuteNonQueryAsync();
        }
        finally
        {
            await conn.CloseAsync();
        }
		_logger.LogWarning("<<< Exiting PerformActionAsync(command={Command},parameters={Parameters})", command, parameters);
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
		_logger.LogError(">>> Entering RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
        prefix = PrepareKeyPrefix(prefix, prefixParameters);
		_logger.LogWarning("<<< Exiting RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
		_logger.LogError("RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters}): prefix → {Prefix1}", prefix, prefixParameters, prefix);

        var command =
            new SqlCommand(
                $"DELETE FROM {_distributedCacheConfig.SchemaName}.{_distributedCacheConfig.TableName} WHERE Id LIKE @Prefix + '%'");

        await PerformActionAsync(command, new SqlParameter("Prefix", SqlDbType.NVarChar) { Value = prefix });

        RemoveByPrefixInstanceData(prefix);
    }

    /// <summary>
    /// Clear all cache data
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task ClearAsync()
    {
		_logger.LogError(">>> Entering ClearAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var command =
            new SqlCommand(
                $"TRUNCATE TABLE {_distributedCacheConfig.SchemaName}.{_distributedCacheConfig.TableName}");
				_logger.LogError("ClearAsync(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

        await PerformActionAsync(command);

        ClearInstanceData();
		_logger.LogWarning("<<< Exiting ClearAsync()");
    }

    #endregion
}