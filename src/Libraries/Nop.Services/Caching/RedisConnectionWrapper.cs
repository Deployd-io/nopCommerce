using System.Net;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Nop.Services.Caching;

/// <summary>
/// Redis connection wrapper
/// </summary>
/// <remarks>
/// This class should be registered on IoC as singleton instance
/// </remarks>
public partial class RedisConnectionWrapper : IRedisConnectionWrapper
{
    #region Fields

    protected readonly SemaphoreSlim _connectionLock = new(1, 1);
    protected volatile IConnectionMultiplexer _connection;
    protected readonly RedisCacheOptions _options;

    #endregion

    #region Ctor

    public RedisConnectionWrapper(IOptions<RedisCacheOptions> optionsAccessor)
    {
		logger.LogDebug(">>> Entering RedisConnectionWrapper(optionsAccessor={OptionsAccessor})", optionsAccessor);
        _options = optionsAccessor.Value;
		logger.LogDebug("<<< Exiting RedisConnectionWrapper(optionsAccessor={OptionsAccessor})", optionsAccessor);
		logger.LogDebug("RedisConnectionWrapper(optionsAccessor={OptionsAccessor}): _options → {_options}", optionsAccessor, _options);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Create a new ConnectionMultiplexer instance
    /// </summary>
    /// <returns></returns>
    protected virtual async Task<IConnectionMultiplexer> ConnectAsync()
    {
		logger.LogDebug(">>> Entering ConnectAsync()");
        IConnectionMultiplexer connection;

        if (_options.ConnectionMultiplexerFactory is null)
        {
            if (_options.ConfigurationOptions is not null)
                connection = await ConnectionMultiplexer.ConnectAsync(_options.ConfigurationOptions);
				logger.LogDebug("ConnectAsync(): connection → {Connection}", connection);
            else
                connection = await ConnectionMultiplexer.ConnectAsync(_options.Configuration);
        }
        else
        {
            connection = await _options.ConnectionMultiplexerFactory();
        }

        if (_options.ProfilingSession != null)
            connection.RegisterProfiler(_options.ProfilingSession);

		logger.LogDebug("<<< Exiting ConnectAsync()");
        return connection;
    }

    /// <summary>
    /// Create a new ConnectionMultiplexer instance
    /// </summary>
    /// <returns></returns>
    protected virtual IConnectionMultiplexer Connect()
    {
		logger.LogDebug(">>> Entering Connect()");
        IConnectionMultiplexer connection;

        if (_options.ConnectionMultiplexerFactory is null)
            connection = _options.ConfigurationOptions is not null ? ConnectionMultiplexer.Connect(_options.ConfigurationOptions) : ConnectionMultiplexer.Connect(_options.Configuration);
			logger.LogDebug("Connect(): connection → {Connection}", connection);
        else
            connection = _options.ConnectionMultiplexerFactory().GetAwaiter().GetResult();

        if (_options.ProfilingSession != null)
            connection.RegisterProfiler(_options.ProfilingSession);

		logger.LogDebug("<<< Exiting Connect()");
        return connection;
    }

    /// <summary>
    /// Get connection to Redis servers, and reconnects if necessary
    /// </summary>
    /// <returns></returns>
    protected virtual async Task<IConnectionMultiplexer> GetConnectionAsync()
    {
		logger.LogDebug(">>> Entering GetConnectionAsync()");
        if (_connection?.IsConnected == true)
            return _connection;

        await _connectionLock.WaitAsync();
        try
        {
            if (_connection?.IsConnected == true)
                return _connection;

            //Connection disconnected. Disposing connection...
            _connection?.Dispose();

            //Creating new instance of Redis Connection
            _connection = await ConnectAsync();
			logger.LogDebug("GetConnectionAsync(): _connection → {_connection}", _connection);
        }
        finally
        {
            _connectionLock.Release();
        }

		logger.LogDebug("<<< Exiting GetConnectionAsync()");
        return _connection;
    }

    /// <summary>
    /// Get connection to Redis servers, and reconnects if necessary
    /// </summary>
    /// <returns></returns>
    protected virtual IConnectionMultiplexer GetConnection()
    {
		logger.LogDebug(">>> Entering GetConnection()");
        if (_connection?.IsConnected == true)
            return _connection;

        _connectionLock.Wait();
        try
        {
            if (_connection?.IsConnected == true)
                return _connection;

            //Connection disconnected. Disposing connection...
            _connection?.Dispose();

            //Creating new instance of Redis Connection
            _connection = Connect();
			logger.LogDebug("GetConnection(): _connection → {_connection}", _connection);
        }
        finally
        {
            _connectionLock.Release();
        }

		logger.LogDebug("<<< Exiting GetConnection()");
        return _connection;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Obtain an interactive connection to a database inside Redis
    /// </summary>
    /// <returns>Redis cache database</returns>
    public virtual async Task<IDatabase> GetDatabaseAsync()
    {
		logger.LogDebug(">>> Entering GetDatabaseAsync()");
		logger.LogDebug("<<< Exiting GetDatabaseAsync()");
        return (await GetConnectionAsync()).GetDatabase();
    }

    /// <summary>
    /// Obtain an interactive connection to a database inside Redis
    /// </summary>
    /// <returns>Redis cache database</returns>
    public virtual IDatabase GetDatabase()
    {
		logger.LogDebug(">>> Entering GetDatabase()");
		logger.LogDebug("<<< Exiting GetDatabase()");
        return GetConnection().GetDatabase();
    }

    /// <summary>
    /// Obtain a configuration API for an individual server
    /// </summary>
    /// <param name="endPoint">The network endpoint</param>
    /// <returns>Redis server</returns>
    public virtual async Task<IServer> GetServerAsync(EndPoint endPoint)
    {
		logger.LogDebug(">>> Entering GetServerAsync(endPoint={EndPoint})", endPoint);
		logger.LogDebug("<<< Exiting GetServerAsync(endPoint={EndPoint})", endPoint);
        return (await GetConnectionAsync()).GetServer(endPoint);
    }

    /// <summary>
    /// Gets all endpoints defined on the server
    /// </summary>
    /// <returns>Array of endpoints</returns>
    public virtual async Task<EndPoint[]> GetEndPointsAsync()
    {
		logger.LogDebug(">>> Entering GetEndPointsAsync()");
		logger.LogDebug("<<< Exiting GetEndPointsAsync()");
        return (await GetConnectionAsync()).GetEndPoints();
    }

    /// <summary>
    /// Gets a subscriber for the server
    /// </summary>
    /// <returns>Array of endpoints</returns>
    public virtual async Task<ISubscriber> GetSubscriberAsync()
    {
		logger.LogDebug(">>> Entering GetSubscriberAsync()");
		logger.LogDebug("<<< Exiting GetSubscriberAsync()");
        return (await GetConnectionAsync()).GetSubscriber();
    }

    /// <summary>
    /// Gets a subscriber for the server
    /// </summary>
    /// <returns>Array of endpoints</returns>
    public virtual ISubscriber GetSubscriber()
    {
		logger.LogDebug(">>> Entering GetSubscriber()");
		logger.LogDebug("<<< Exiting GetSubscriber()");
        return GetConnection().GetSubscriber();
    }

    /// <summary>
    /// Delete all the keys of the database
    /// </summary>
    public virtual async Task FlushDatabaseAsync()
    {
		logger.LogDebug(">>> Entering FlushDatabaseAsync()");
        var endPoints = await GetEndPointsAsync();
        await Task.WhenAll(endPoints.Select(async endPoint =>
        {
		logger.LogDebug("FlushDatabaseAsync(): endPoints → {EndPoints}", endPoints);
            var server = await GetServerAsync(endPoint);
            if (!server.IsReplica)
			logger.LogDebug("FlushDatabaseAsync(): server → {Server}", server);
            {
                await server.FlushDatabaseAsync();
            }
        }));
		logger.LogDebug("<<< Exiting FlushDatabaseAsync()");
    }

    /// <summary>
    /// Release all resources associated with this object
    /// </summary>
    public virtual void Dispose()
    {
		logger.LogDebug(">>> Entering Dispose()");
        //dispose ConnectionMultiplexer
        _connection?.Dispose();
		logger.LogDebug("<<< Exiting Dispose()");
    }

    #endregion

    #region Properties

    /// <summary>
    /// The Redis instance name
    /// </summary>
    public string Instance => _options.InstanceName ?? string.Empty;

    #endregion
}
