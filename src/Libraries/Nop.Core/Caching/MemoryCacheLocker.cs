using Microsoft.Extensions.Logging;
﻿using Microsoft.Extensions.Caching.Memory;

namespace Nop.Core.Caching;

/// <summary>
/// A distributed cache manager that locks the acquisition task
/// </summary>
public partial class MemoryCacheLocker : ILocker
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IMemoryCache _memoryCache;

    #endregion

    #region Ctor

    public MemoryCacheLocker(IMemoryCache memoryCache)
    {
		_logger.LogDebug(">>> Entering MemoryCacheLocker(memoryCache={MemoryCache})", memoryCache);
        _memoryCache = memoryCache;
		_logger.LogDebug("MemoryCacheLocker(memoryCache={MemoryCache}): _memoryCache → {_memoryCache}", memoryCache, _memoryCache);
		_logger.LogDebug("<<< Exiting MemoryCacheLocker(memoryCache={MemoryCache})", memoryCache);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Run action
    /// </summary>
    /// <param name="key">The key of the background task</param>
    /// <param name="expirationTime">The time after which the lock will automatically be expired</param>
    /// <param name="action">The action to perform</param>
    /// <param name="cancellationTokenSource">A CancellationTokenSource for manually canceling the task</param>
    /// <returns></returns>
    protected virtual async Task<bool> RunAsync(string key, TimeSpan? expirationTime, Func<CancellationToken, Task> action, CancellationTokenSource cancellationTokenSource = default)
    {
		_logger.LogDebug(">>> Entering RunAsync(key={Key},expirationTime={ExpirationTime},action={Action},cancellationTokenSource=<redacted>)", key, expirationTime, action);
        var started = false;

		_logger.LogDebug("RunAsync(key={Key},expirationTime={ExpirationTime},action={Action},cancellationTokenSource=<redacted>): started → {Started}", key, expirationTime, action, started);
        try
        {
            var tokenSource = _memoryCache.GetOrCreate(key, entry => new Lazy<CancellationTokenSource>(() =>
            {
                entry.AbsoluteExpirationRelativeToNow = expirationTime;
				_logger.LogDebug("RunAsync(key={Key},expirationTime={ExpirationTime},action={Action},cancellationTokenSource=<redacted>): entry.AbsoluteExpirationRelativeToNow → {Arg4}", key, expirationTime, action, entry.AbsoluteExpirationRelativeToNow);
                entry.SetPriority(CacheItemPriority.NeverRemove);
                started = true;
                return cancellationTokenSource ?? new CancellationTokenSource();
            }, true))?.Value;

            if (tokenSource != null && started)
                await action(tokenSource.Token);
        }
        catch (OperationCanceledException) { }
		_logger.LogError("Exception in RunAsync(key={Key},expirationTime={ExpirationTime},action={Action},cancellationTokenSource=<redacted>): {Arg4}", key, expirationTime, action, OperationCanceledException.getMessage());
        finally
        {
            if (started)
                _memoryCache.Remove(key);
        }

		_logger.LogDebug("<<< Exiting RunAsync(key={Key},expirationTime={ExpirationTime},action={Action},cancellationTokenSource=<redacted>)", key, expirationTime, action);
        return started;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Performs some asynchronous task with exclusive lock
    /// </summary>
    /// <param name="resource">The key we are locking on</param>
    /// <param name="expirationTime">The time after which the lock will automatically be expired</param>
    /// <param name="action">Asynchronous task to be performed with locking</param>
    /// <returns>A task that resolves true if lock was acquired and action was performed; otherwise false</returns>
    public virtual async Task<bool> PerformActionWithLockAsync(string resource, TimeSpan expirationTime, Func<Task> action)
    {
		_logger.LogDebug(">>> Entering PerformActionWithLockAsync(resource={Resource},expirationTime={ExpirationTime},action={Action})", resource, expirationTime, action);
		_logger.LogDebug("<<< Exiting PerformActionWithLockAsync(resource={Resource},expirationTime={ExpirationTime},action={Action})", resource, expirationTime, action);
        return await RunAsync(resource, expirationTime, _ => action());
    }

    /// <summary>
    /// Starts a background task with "heartbeat": a status flag that will be periodically updated to signal to
    /// others that the task is running and stop them from starting the same task.
    /// </summary>
    /// <param name="key">The key of the background task</param>
    /// <param name="expirationTime">The time after which the heartbeat key will automatically be expired. Should be longer than <paramref name="heartbeatInterval"/></param>
    /// <param name="heartbeatInterval">The interval at which to update the heartbeat, if required by the implementation</param>
    /// <param name="action">Asynchronous background task to be performed</param>
    /// <param name="cancellationTokenSource">A CancellationTokenSource for manually canceling the task</param>
    /// <returns>A task that resolves true if lock was acquired and action was performed; otherwise false</returns>
    public virtual async Task RunWithHeartbeatAsync(string key, TimeSpan expirationTime, TimeSpan heartbeatInterval, Func<CancellationToken, Task> action, CancellationTokenSource cancellationTokenSource = default)
    {
		_logger.LogDebug(">>> Entering RunWithHeartbeatAsync(key={Key},expirationTime={ExpirationTime},heartbeatInterval={HeartbeatInterval},action={Action},cancellationTokenSource=<redacted>)", key, expirationTime, heartbeatInterval, action);
        // We ignore expirationTime and heartbeatInterval here, as the cache is not shared with other instances,
        // and will be cleared on system failure anyway. The task is guaranteed to still be running as long as it is in the cache.
        await RunAsync(key, null, action, cancellationTokenSource);
		_logger.LogDebug("<<< Exiting RunWithHeartbeatAsync(key={Key},expirationTime={ExpirationTime},heartbeatInterval={HeartbeatInterval},action={Action},cancellationTokenSource=<redacted>)", key, expirationTime, heartbeatInterval, action);
    }

    /// <summary>
    /// Tries to cancel a background task by flagging it for cancellation on the next heartbeat.
    /// </summary>
    /// <param name="key">The task's key</param>
    /// <param name="expirationTime">The time after which the task will be considered stopped due to system shutdown or other causes,
    /// even if not explicitly canceled.</param>
    /// <returns>A task that represents requesting cancellation of the task. Note that the completion of this task does not
    /// necessarily imply that the task has been canceled, only that cancellation has been requested.</returns>
    public virtual Task CancelTaskAsync(string key, TimeSpan expirationTime)
    {
		_logger.LogDebug(">>> Entering CancelTaskAsync(key={Key},expirationTime={ExpirationTime})", key, expirationTime);
        if (_memoryCache.TryGetValue(key, out Lazy<CancellationTokenSource> tokenSource))
            tokenSource.Value.Cancel();

		_logger.LogDebug("<<< Exiting CancelTaskAsync(key={Key},expirationTime={ExpirationTime})", key, expirationTime);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Check if a background task is running.
    /// </summary>
    /// <param name="key">The task's key</param>
    /// <returns>A task that resolves to true if the background task is running; otherwise false</returns>
    public virtual Task<bool> IsTaskRunningAsync(string key)
    {
		_logger.LogDebug(">>> Entering IsTaskRunningAsync(key={Key})", key);
		_logger.LogDebug("<<< Exiting IsTaskRunningAsync(key={Key})", key);
        return Task.FromResult(_memoryCache.TryGetValue(key, out _));
    }

    #endregion
}