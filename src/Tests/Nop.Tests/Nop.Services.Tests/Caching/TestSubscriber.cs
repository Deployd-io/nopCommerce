using System.Net;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Caching;

public class TestSubscriber : ISubscriber
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected static IList<Action<RedisChannel, RedisValue>> _handlers = new List<Action<RedisChannel, RedisValue>>();
    
    public Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
    {
		_logger.LogDebug(">>> Entering PingAsync(flags={Flags})", flags);
        throw new NotImplementedException();
    }

    public bool TryWait(Task task)
    {
		_logger.LogDebug(">>> Entering TryWait(task={Task})", task);
        throw new NotImplementedException();
    }

    public void Wait(Task task)
    {
        throw new NotImplementedException();
    }

    public T Wait<T>(Task<T> task)
    {
		_logger.LogDebug(">>> Entering Wait(task={Task})", task);
        throw new NotImplementedException();
    }

    public void WaitAll(params Task[] tasks)
    {
		_logger.LogDebug(">>> Entering WaitAll(tasks={Tasks})", tasks);
        throw new NotImplementedException();
    }

    public IConnectionMultiplexer Multiplexer => null;

    public TimeSpan Ping(CommandFlags flags = CommandFlags.None)
    {
		_logger.LogDebug(">>> Entering Ping(flags={Flags})", flags);
        throw new NotImplementedException();
    }

    public EndPoint IdentifyEndpoint(RedisChannel channel, CommandFlags flags = CommandFlags.None)
    {
		_logger.LogDebug(">>> Entering IdentifyEndpoint(channel={Channel},flags={Flags})", channel, flags);
        throw new NotImplementedException();
    }

    public Task<EndPoint> IdentifyEndpointAsync(RedisChannel channel, CommandFlags flags = CommandFlags.None)
    {
		_logger.LogDebug(">>> Entering IdentifyEndpointAsync(channel={Channel},flags={Flags})", channel, flags);
        throw new NotImplementedException();
    }

    public bool IsConnected(RedisChannel channel = new RedisChannel())
    {
		_logger.LogDebug(">>> Entering IsConnected(channel={Channel})", channel);
        throw new NotImplementedException();
    }

    /// <summary>Posts a message to the given channel.</summary>
    /// <param name="channel">The channel to publish to.</param>
    /// <param name="message">The message to publish.</param>
    /// <param name="flags">The command flags to use.</param>
    /// <returns>
    /// The number of clients that received the message *on the destination server*,
    /// note that this doesn't mean much in a cluster as clients can get the message through other nodes.
    /// </returns>
    /// <remarks><seealso href="https://redis.io/commands/publish" /></remarks>
    public long Publish(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
    {
		_logger.LogDebug(">>> Entering Publish(channel={Channel},message={Message},flags={Flags})", channel, message, flags);
        foreach (var handler in _handlers) 
            handler(channel, message);

		_logger.LogDebug("<<< Exiting Publish(channel={Channel},message={Message},flags={Flags})", channel, message, flags);
        return _handlers.Count;
    }

    /// <inheritdoc cref="M:StackExchange.Redis.ISubscriber.Publish(StackExchange.Redis.RedisChannel,StackExchange.Redis.RedisValue,StackExchange.Redis.CommandFlags)" />
    public Task<long> PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
    {
		_logger.LogDebug(">>> Entering PublishAsync(channel={Channel},message={Message},flags={Flags})", channel, message, flags);
		_logger.LogDebug("<<< Exiting PublishAsync(channel={Channel},message={Message},flags={Flags})", channel, message, flags);
        return Task.FromResult(Publish(channel, message, flags));
    }

    /// <summary>
    /// Subscribe to perform some operation when a message to the preferred/active node is broadcast, without any guarantee of ordered handling.
    /// </summary>
    /// <param name="channel">The channel to subscribe to.</param>
    /// <param name="handler">The handler to invoke when a message is received on <paramref name="channel" />.</param>
    /// <param name="flags">The command flags to use.</param>
    /// <remarks>
    /// <seealso href="https://redis.io/commands/subscribe" />,
    /// <seealso href="https://redis.io/commands/psubscribe" />
    /// </remarks>
    public void Subscribe(RedisChannel channel, Action<RedisChannel, RedisValue> handler, CommandFlags flags = CommandFlags.None)
    {
        _handlers.Add(handler);
    }

    /// <inheritdoc cref="M:StackExchange.Redis.ISubscriber.Subscribe(StackExchange.Redis.RedisChannel,System.Action{StackExchange.Redis.RedisChannel,StackExchange.Redis.RedisValue},StackExchange.Redis.CommandFlags)" />
    public Task SubscribeAsync(RedisChannel channel, Action<RedisChannel, RedisValue> handler, CommandFlags flags = CommandFlags.None)
    {
        _handlers.Add(handler);

        return Task.CompletedTask;
    }

    public ChannelMessageQueue Subscribe(RedisChannel channel, CommandFlags flags = CommandFlags.None)
    {
		_logger.LogDebug(">>> Entering Subscribe(channel={Channel},flags={Flags})", channel, flags);
        throw new NotImplementedException();
    }

    public Task<ChannelMessageQueue> SubscribeAsync(RedisChannel channel, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public EndPoint SubscribedEndpoint(RedisChannel channel)
    {
		_logger.LogDebug(">>> Entering SubscribedEndpoint(channel={Channel})", channel);
        throw new NotImplementedException();
    }

    public void Unsubscribe(RedisChannel channel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public Task UnsubscribeAsync(RedisChannel channel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None)
    {
        throw new NotImplementedException();
    }

    public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
    {
		_logger.LogDebug(">>> Entering UnsubscribeAll(flags={Flags})", flags);
        throw new NotImplementedException();
    }

    public Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
    {
		_logger.LogDebug(">>> Entering UnsubscribeAllAsync(flags={Flags})", flags);
        throw new NotImplementedException();
    }
}