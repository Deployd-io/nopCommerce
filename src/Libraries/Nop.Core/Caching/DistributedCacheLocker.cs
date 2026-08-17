using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Nop.Core.Caching;

public partial class DistributedCacheLocker : ILocker
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected static readonly string _running = JsonConvert.SerializeObject(TaskStatus.Running);
    protected readonly IDistributedCache _distributedCache;

    #endregion

    #region Ctor

    public DistributedCacheLocker(IDistributedCache distributedCache)
    {
		_logger.LogDebug(">>> Entering DistributedCacheLocker(distributedCache={DistributedCache})", distributedCache);
        _distributedCache = distributedCache;
		_logger.LogDebug("<<< Exiting DistributedCacheLocker(distributedCache={DistributedCache})", distributedCache);
		_logger.LogDebug("DistributedCacheLocker(distributedCache={DistributedCache}): _distributedCache → {_distributedCache}", distributedCache, _distributedCache);
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
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //ensure that lock is acquired
        if (!string.IsNullOrEmpty(await _distributedCache.GetStringAsync(resource)))
            return false;

        try
        {
            await _distributedCache.SetStringAsync(resource, resource, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime
            });

            await action();

			_logger.LogInformation("PerformActionWithLockAsync(resource,expirationTime,action)={Resource},{ExpirationTime},{Action}: external service call took {Arg3} ms", resource, expirationTime, action, __logStart.ElapsedMilliseconds);
			_logger.LogDebug("<<< Exiting PerformActionWithLockAsync(resource={Resource},expirationTime={ExpirationTime},action={Action})", resource, expirationTime, action);
            return true;
        }
        finally
        {
            //release lock even if action fails
            await _distributedCache.RemoveAsync(resource);
        }
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
        if (!string.IsNullOrEmpty(await _distributedCache.GetStringAsync(key)))
            return;

        var tokenSource = cancellationTokenSource ?? new CancellationTokenSource();

        try
        {
            // run heartbeat early to minimize risk of multiple execution
            await _distributedCache.SetStringAsync(
                key,
                _running,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expirationTime },
                token: tokenSource.Token);

            await using var timer = new Timer(
                callback: _ =>
                {
                    try
                    {
                        tokenSource.Token.ThrowIfCancellationRequested();
                        var status = _distributedCache.GetString(key);
                        if (!string.IsNullOrEmpty(status) && JsonConvert.DeserializeObject<TaskStatus>(status) ==
                            TaskStatus.Canceled)
                        {
                            tokenSource.Cancel();
                            return;
                        }

                        _distributedCache.SetString(
                            key,
                            _running,
                            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expirationTime });
                    }
                    catch (OperationCanceledException) { }
                },
                state: null,
                dueTime: 0,
                period: (int)heartbeatInterval.TotalMilliseconds);

            await action(tokenSource.Token);
        }
        catch (OperationCanceledException) { }
        finally
        {
            await _distributedCache.RemoveAsync(key);
        }
    }

    /// <summary>
    /// Tries to cancel a background task by flagging it for cancellation on the next heartbeat.
    /// </summary>
    /// <param name="key">The task's key</param>
    /// <param name="expirationTime">The time after which the task will be considered stopped due to system shutdown or other causes,
    /// even if not explicitly canceled.</param>
    /// <returns>A task that represents requesting cancellation of the task. Note that the completion of this task does not
    /// necessarily imply that the task has been canceled, only that cancellation has been requested.</returns>
    public virtual async Task CancelTaskAsync(string key, TimeSpan expirationTime)
    {
		_logger.LogDebug(">>> Entering CancelTaskAsync(key={Key},expirationTime={ExpirationTime})", key, expirationTime);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var status = await _distributedCache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(status) &&
			_logger.LogDebug("CancelTaskAsync(key={Key},expirationTime={ExpirationTime}): status → {Status}", key, expirationTime, status);
            JsonConvert.DeserializeObject<TaskStatus>(status) != TaskStatus.Canceled)
            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(TaskStatus.Canceled),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expirationTime });
				_logger.LogDebug("<<< Exiting CancelTaskAsync(key={Key},expirationTime={ExpirationTime})", key, expirationTime);
				_logger.LogInformation("CancelTaskAsync(key,expirationTime)={Key},{ExpirationTime}: external service call took {Arg2} ms", key, expirationTime, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Check if a background task is running.
    /// </summary>
    /// <param name="key">The task's key</param>
    /// <returns>A task that resolves to true if the background task is running; otherwise false</returns>
    public virtual async Task<bool> IsTaskRunningAsync(string key)
    {
		_logger.LogDebug(">>> Entering IsTaskRunningAsync(key={Key})", key);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("IsTaskRunningAsync(key)={Key}: external service call took {Arg1} ms", key, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting IsTaskRunningAsync(key={Key})", key);
        return !string.IsNullOrEmpty(await _distributedCache.GetStringAsync(key));
    }

    #endregion
}