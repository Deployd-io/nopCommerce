using System.Net;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Caching;

/// <summary>
/// Redis connection wrapper
/// </summary>
/// <remarks>
/// This class should be registered on IoC as singleton instance
/// </remarks>
public partial class RedisConnectionWrapper : IRedisConnectionWrapper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly SemaphoreSlim _connectionLock = new(1, 1);
    protected volatile IConnectionMultiplexer _connection;
    protected readonly RedisCacheOptions _options;

    #endregion

    #region Ctor

    public RedisConnectionWrapper(IOptions<RedisCacheOptions> optionsAccessor)
    {
		_logger.LogDebug(">>> Entering RedisConnectionWrapper(optionsAccessor={OptionsAccessor})", optionsAccessor);
        _options = optionsAccessor.Value;
		_logger.LogDebug("RedisConnectionWrapper(optionsAccessor={OptionsAccessor}): _options → {_options}", optionsAccessor, _options);
		_logger.LogDebug("<<< Exiting RedisConnectionWrapper(optionsAccessor={OptionsAccessor})", optionsAccessor);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Create a new ConnectionMultiplexer instance
    /// </summary>
    /// <returns></returns>
    protected virtual async Task<IConnectionMultiplexer> ConnectAsync()
    {
		_logger.LogDebug(">>> Entering ConnectAsync()");
        IConnectionMultiplexer connection;

        if (_options.ConnectionMultiplexerFactory is null)
        {
            if (_options.ConfigurationOptions is not null)
                connection = await ConnectionMultiplexer.ConnectAsync(_options.ConfigurationOptions);
            else
                connection = await ConnectionMultiplexer.ConnectAsync(_options.Configuration);
        }
        else
        {
            connection = await _options.ConnectionMultiplexerFactory();
        }

        if (_options.ProfilingSession != null)
            connection.RegisterProfiler(_options.ProfilingSession);

		_logger.LogDebug("<<< Exiting ConnectAsync()");
        return connection;
    }

    /// <summary>
    /// Create a new ConnectionMultiplexer instance
    /// </summary>
    /// <returns></returns>
    protected virtual IConnectionMultiplexer Connect()
    {
		_logger.LogDebug(">>> Entering Connect()");
        IConnectionMultiplexer connection;

        if (_options.ConnectionMultiplexerFactory is null)
            connection = _options.ConfigurationOptions is not null ? ConnectionMultiplexer.Connect(_options.ConfigurationOptions) : ConnectionMultiplexer.Connect(_options.Configuration);
        else
            connection = _options.ConnectionMultiplexerFactory().GetAwaiter().GetResult();

        if (_options.ProfilingSession != null)
            connection.RegisterProfiler(_options.ProfilingSession);

		_logger.LogDebug("<<< Exiting Connect()");
        return connection;
    }

    /// <summary>
    /// Get connection to Redis servers, and reconnects if necessary
    /// </summary>
    /// <returns></returns>
    protected virtual async Task<IConnectionMultiplexer> GetConnectionAsync()
    {
		_logger.LogDebug(">>> Entering GetConnectionAsync()");
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
        }
        finally
        {
            _connectionLock.Release();
        }

		_logger.LogDebug("<<< Exiting GetConnectionAsync()");
        return _connection;
    }

    /// <summary>
    /// Get connection to Redis servers, and reconnects if necessary
    /// </summary>
    /// <returns></returns>
    protected virtual IConnectionMultiplexer GetConnection()
    {
		_logger.LogDebug(">>> Entering GetConnection()");
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
        }
        finally
        {
            _connectionLock.Release();
        }

		_logger.LogDebug("<<< Exiting GetConnection()");
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
		_logger.LogDebug(">>> Entering GetDatabaseAsync()");
		_logger.LogDebug("<<< Exiting GetDatabaseAsync()");
        return (await GetConnectionAsync()).GetDatabase();
    }

    /// <summary>
    /// Obtain an interactive connection to a database inside Redis
    /// </summary>
    /// <returns>Redis cache database</returns>
    public virtual IDatabase GetDatabase()
    {
		_logger.LogDebug(">>> Entering GetDatabase()");
		_logger.LogDebug("<<< Exiting GetDatabase()");
        return GetConnection().GetDatabase();
    }

    /// <summary>
    /// Obtain a configuration API for an individual server
    /// </summary>
    /// <param name="endPoint">The network endpoint</param>
    /// <returns>Redis server</returns>
    public virtual async Task<IServer> GetServerAsync(EndPoint endPoint)
    {
		_logger.LogDebug(">>> Entering GetServerAsync(endPoint={EndPoint})", endPoint);
		_logger.LogDebug("<<< Exiting GetServerAsync(endPoint={EndPoint})", endPoint);
        return (await GetConnectionAsync()).GetServer(endPoint);
    }

    /// <summary>
    /// Gets all endpoints defined on the server
    /// </summary>
    /// <returns>Array of endpoints</returns>
    public virtual async Task<EndPoint[]> GetEndPointsAsync()
    {
		_logger.LogDebug(">>> Entering GetEndPointsAsync()");
		_logger.LogDebug("<<< Exiting GetEndPointsAsync()");
        return (await GetConnectionAsync()).GetEndPoints();
    }

    /// <summary>
    /// Gets a subscriber for the server
    /// </summary>
    /// <returns>Array of endpoints</returns>
    public virtual async Task<ISubscriber> GetSubscriberAsync()
    {
		_logger.LogDebug(">>> Entering GetSubscriberAsync()");
		_logger.LogDebug("<<< Exiting GetSubscriberAsync()");
        return (await GetConnectionAsync()).GetSubscriber();
    }

    /// <summary>
    /// Gets a subscriber for the server
    /// </summary>
    /// <returns>Array of endpoints</returns>
    public virtual ISubscriber GetSubscriber()
    {
		_logger.LogDebug(">>> Entering GetSubscriber()");
		_logger.LogDebug("<<< Exiting GetSubscriber()");
        return GetConnection().GetSubscriber();
    }

    /// <summary>
    /// Delete all the keys of the database
    /// </summary>
    public virtual async Task FlushDatabaseAsync()
    {
		_logger.LogDebug(">>> Entering FlushDatabaseAsync()");
        var endPoints = await GetEndPointsAsync();
        await Task.WhenAll(endPoints.Select(async endPoint =>
        {
			_logger.LogDebug("FlushDatabaseAsync(): endPoints → {EndPoints}", endPoints);
            var server = await GetServerAsync(endPoint);
			_logger.LogDebug("FlushDatabaseAsync(): server → {Server}", server);
            if (!server.IsReplica)
            {
                await server.FlushDatabaseAsync();
            }
        }));
		_logger.LogDebug("<<< Exiting FlushDatabaseAsync()");
    }

    /// <summary>
    /// Release all resources associated with this object
    /// </summary>
    public virtual void Dispose()
    {
		_logger.LogDebug(">>> Entering Dispose()");
        //dispose ConnectionMultiplexer
        _connection?.Dispose();
		_logger.LogDebug("<<< Exiting Dispose()");
    }

    #endregion

    #region Properties

    /// <summary>
    /// The Redis instance name
    /// </summary>
    public string Instance => _options.InstanceName ?? string.Empty;

    #endregion
}