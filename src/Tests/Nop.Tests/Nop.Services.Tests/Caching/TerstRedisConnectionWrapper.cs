using System.Net;
using Moq;
using Nop.Services.Caching;
using StackExchange.Redis;

namespace Nop.Tests.Nop.Services.Tests.Caching;

internal class TestRedisConnectionWrapper : IRedisConnectionWrapper
{
    protected static int _npp = 1;

    protected readonly ISubscriber _subscriber = new TestSubscriber();
    protected readonly IDatabase _database;
    protected readonly IServer _server;

    protected string _instance;

    public TestRedisConnectionWrapper()
    {
		logger.LogDebug(">>> Entering TestRedisConnectionWrapper()");
        _instance = $"test_redis_connection_wrapper_{_npp}";
        var database = new Mock<IDatabase>();
		logger.LogDebug("TestRedisConnectionWrapper(): _instance → {_instance}", _instance);
		logger.LogDebug("TestRedisConnectionWrapper(): database → {Database}", database);
        database.Setup(d => d.Database).Returns(_npp);
        _database = database.Object;
		logger.LogDebug("TestRedisConnectionWrapper(): _database → {_database}", _database);
        var server = new Mock<IServer>();
        _server = server.Object;
		logger.LogDebug("TestRedisConnectionWrapper(): server → {Server}", server);
		logger.LogDebug("TestRedisConnectionWrapper(): _server → {_server}", _server);

        _npp += 1;
		logger.LogDebug("<<< Exiting TestRedisConnectionWrapper()");
    }

    /// <summary>
    /// Obtain an interactive connection to a database inside Redis
    /// </summary>
    /// <returns>Redis cache database</returns>
    public Task<IDatabase> GetDatabaseAsync()
    {
		logger.LogDebug(">>> Entering GetDatabaseAsync()");
		logger.LogDebug("<<< Exiting GetDatabaseAsync()");
        return Task.FromResult(GetDatabase());
    }

    /// <summary>
    /// Obtain an interactive connection to a database inside Redis
    /// </summary>
    /// <returns>Redis cache database</returns>
    public IDatabase GetDatabase()
    {
		logger.LogDebug(">>> Entering GetDatabase()");
		logger.LogDebug("<<< Exiting GetDatabase()");
        return _database;
    }

    /// <summary>
    /// Obtain a configuration API for an individual server
    /// </summary>
    /// <param name="endPoint">The network endpoint</param>
    /// <returns>Redis server</returns>
    public Task<IServer> GetServerAsync(EndPoint endPoint)
    {
		logger.LogDebug(">>> Entering GetServerAsync(endPoint={EndPoint})", endPoint);
		logger.LogDebug("<<< Exiting GetServerAsync(endPoint={EndPoint})", endPoint);
        return Task.FromResult(_server);
    }

    /// <summary>
    /// Gets all endpoints defined on the server
    /// </summary>
    /// <returns>Array of endpoints</returns>
    public Task<EndPoint[]> GetEndPointsAsync()
    {
		logger.LogDebug(">>> Entering GetEndPointsAsync()");
		logger.LogDebug("<<< Exiting GetEndPointsAsync()");
        return Task.FromResult(new[] { _server.EndPoint });
    }

    /// <summary>
    /// Gets a subscriber for the server
    /// </summary>
    /// <returns>Array of endpoints</returns>
    public Task<ISubscriber> GetSubscriberAsync()
    {
		logger.LogDebug(">>> Entering GetSubscriberAsync()");
		logger.LogDebug("<<< Exiting GetSubscriberAsync()");
        return Task.FromResult(GetSubscriber());
    }

    /// <summary>
    /// Gets a subscriber for the server
    /// </summary>
    /// <returns>Array of endpoints</returns>
    public ISubscriber GetSubscriber()
    {
		logger.LogDebug(">>> Entering GetSubscriber()");
		logger.LogDebug("<<< Exiting GetSubscriber()");
        return _subscriber;
    }

    /// <summary>
    /// Delete all the keys of the database
    /// </summary>
    public async Task FlushDatabaseAsync()
    {
		logger.LogDebug(">>> Entering FlushDatabaseAsync()");
        await _server.FlushDatabaseAsync(_database.Database);
		logger.LogDebug("<<< Exiting FlushDatabaseAsync()");
    }

    /// <summary>
    /// The Redis instance name
    /// </summary>
    public string Instance => _instance;
}
