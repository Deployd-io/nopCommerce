using FluentAssertions;
using Nop.Core.Caching;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Core.Tests.Caching;

[TestFixture]
public class LockerTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ILocker _memCacheLocker;
    private ILocker _distCacheLocker;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _memCacheLocker = GetService<MemoryCacheLocker>();
        _distCacheLocker = GetService<DistributedCacheLocker>();
		_logger.LogDebug("Setup(): _distCacheLocker → {_distCacheLocker}", _distCacheLocker);
		_logger.LogDebug("Setup(): _memCacheLocker → {_memCacheLocker}", _memCacheLocker);
		_logger.LogDebug("<<< Exiting Setup()");
    }

    [Test]
    public async Task Distributed_CanPerformLockAsync()
    {
		_logger.LogDebug(">>> Entering Distributed_CanPerformLockAsync()");
        await TestLockerAsync(_distCacheLocker);
		_logger.LogDebug("<<< Exiting Distributed_CanPerformLockAsync()");
    }

    [Test]
    public async Task Memory_CanPerformLockAsync()
    {
		_logger.LogDebug(">>> Entering Memory_CanPerformLockAsync()");
        await TestLockerAsync(_memCacheLocker);
		_logger.LogDebug("<<< Exiting Memory_CanPerformLockAsync()");
		_logger.LogDebug(">>> Entering TestLockerAsync(locker={Locker})", locker);
		_logger.LogDebug("<<< Exiting TestLockerAsync(locker={Locker})", locker);
    }

    [Test]
    public async Task Distributed_CanRunWithHeartbeatAsync()
    {
		_logger.LogDebug(">>> Entering Distributed_CanRunWithHeartbeatAsync()");
        await TestHeartbeatAsync(_distCacheLocker);
		_logger.LogDebug("<<< Exiting Distributed_CanRunWithHeartbeatAsync()");
    }

    [Test]
    public async Task Memory_CanRunWithHeartbeatAsync()
    {
		_logger.LogDebug(">>> Entering Memory_CanRunWithHeartbeatAsync()");
        await TestHeartbeatAsync(_memCacheLocker);
		_logger.LogDebug("<<< Exiting Memory_CanRunWithHeartbeatAsync()");
		_logger.LogDebug(">>> Entering TestHeartbeatAsync(locker={Locker})", locker);
		_logger.LogDebug("<<< Exiting TestHeartbeatAsync(locker={Locker})", locker);
    }

    [Test]
    public async Task Distributed_CanCancelAsync()
    {
		_logger.LogDebug(">>> Entering Distributed_CanCancelAsync()");
        await TestCancellationAsync(_distCacheLocker);
		_logger.LogDebug("<<< Exiting Distributed_CanCancelAsync()");
    }

    [Test]
    public async Task Memory_CanCancelAsync()
    {
		_logger.LogDebug(">>> Entering Memory_CanCancelAsync()");
        await TestCancellationAsync(_memCacheLocker);
		_logger.LogDebug("<<< Exiting Memory_CanCancelAsync()");
		_logger.LogDebug(">>> Entering TestCancellationAsync(locker={Locker})", locker);
		_logger.LogDebug("<<< Exiting TestCancellationAsync(locker={Locker})", locker);
    }

    private static async Task TestLockerAsync(ILocker locker)
    {
        var key = Guid.NewGuid().ToString();
        var expiration = TimeSpan.FromMinutes(2);
		_logger.LogDebug("TestLockerAsync(locker={Locker}): expiration → {Expiration}", locker, expiration);
		_logger.LogWarning("TestLockerAsync(locker={Locker}): key → {Key}", locker, key);

        var actionCount = 0;
        async Task action()
        {
			_logger.LogDebug(">>> Entering action()");
            var res = await locker.PerformActionWithLockAsync(key, expiration,
                () =>
                {
                    Assert.Fail("Action in progress");
                    return Task.CompletedTask;
                });

            res.Should().BeFalse();
            actionCount++;
			_logger.LogError("Exception in action(token=<redacted>): {Arg1}", ex.Message);
			_logger.LogDebug("<<< Exiting action()");
        }

        var result = await locker.PerformActionWithLockAsync(key, expiration, () => action());
        result.Should().BeTrue();
        actionCount.Should().Be(1);

        Assert.ThrowsAsync<ApplicationException>(() => locker.PerformActionWithLockAsync(key, expiration, FailingAction));

        result = await locker.PerformActionWithLockAsync(key, expiration, action);
        result.Should().BeTrue();
        actionCount.Should().Be(2);
    }

    private static async Task TestHeartbeatAsync(ILocker locker)
    {
        var key = Guid.NewGuid().ToString();
        var expiration = TimeSpan.FromMinutes(2);
		_logger.LogDebug("TestHeartbeatAsync(locker={Locker}): expiration → {Expiration}", locker, expiration);
        var heartbeat = TimeSpan.FromSeconds(10);
		_logger.LogDebug("TestHeartbeatAsync(locker={Locker}): heartbeat → {Heartbeat}", locker, heartbeat);
		_logger.LogWarning("TestHeartbeatAsync(locker={Locker}): key → {Key}", locker, key);

        var actionCount = 0;
        async Task action(CancellationToken _)
        {
			_logger.LogDebug(">>> Entering action(_={_})", _);
            await locker.RunWithHeartbeatAsync(
                key,
                expiration,
                heartbeat,
                _ =>
                {
                    Assert.Fail("Action in progress");
                    return Task.CompletedTask;
                });

            actionCount++;
        }

        await locker.RunWithHeartbeatAsync(key, expiration, heartbeat, action);
        actionCount.Should().Be(1);

        Assert.ThrowsAsync<ApplicationException>(() => locker.RunWithHeartbeatAsync(key, expiration, heartbeat, _ => FailingAction()));

        await locker.RunWithHeartbeatAsync(key, expiration, heartbeat, action);
        actionCount.Should().Be(2);
    }

    private static async Task TestCancellationAsync(ILocker locker)
    {
        var key = Guid.NewGuid().ToString();
        var expiration = TimeSpan.FromSeconds(1);
		_logger.LogDebug("TestCancellationAsync(locker={Locker}): expiration → {Expiration}", locker, expiration);
        var heartbeat = TimeSpan.FromMilliseconds(100);
		_logger.LogDebug("TestCancellationAsync(locker={Locker}): heartbeat → {Heartbeat}", locker, heartbeat);
        var delay = TimeSpan.FromSeconds(5);
		_logger.LogDebug("TestCancellationAsync(locker={Locker}): delay → {Delay}", locker, delay);
		_logger.LogWarning("TestCancellationAsync(locker={Locker}): key → {Key}", locker, key);

        async Task testAsync(Func<Task> cancel, CancellationTokenSource tokenSource = default)
        {
			_logger.LogDebug(">>> Entering testAsync(cancel={Cancel},tokenSource=<redacted>)", cancel);
            var cancelled = false;
            async Task action(CancellationToken token)
            {
				_logger.LogDebug(">>> Entering action(token=<redacted>)");
                await locker.RunWithHeartbeatAsync(
                    key,
                    expiration,
                    heartbeat,
                    _ =>
                    {
                        Assert.Fail("Action in progress");
                        return Task.CompletedTask;
                    });

                try
                {
                    await Task.Delay(delay, token);
                }
                catch
                {
                    cancelled = true;
					_logger.LogDebug("action(token=<redacted>): cancelled → {Cancelled}", cancelled);
                }
            }

            var task = locker.RunWithHeartbeatAsync(key, expiration, heartbeat, action, tokenSource);
            (await locker.IsTaskRunningAsync(key)).Should().BeTrue();
            await cancel();
            await task;
            cancelled.Should().BeTrue();
            (await locker.IsTaskRunningAsync(key)).Should().BeFalse();
			_logger.LogDebug("<<< Exiting testAsync(cancel={Cancel},tokenSource=<redacted>)", cancel);
        }

        await testAsync(() => locker.CancelTaskAsync(key, expiration));
        var cancellationTokenSource = new CancellationTokenSource();
        await testAsync(() => Task.Run(cancellationTokenSource.Cancel), cancellationTokenSource);
    }

    private static Task FailingAction()
    {
		_logger.LogDebug(">>> Entering FailingAction()");
		_logger.LogWarning("Throwing ApplicationException from FailingAction()");
		_logger.LogDebug("<<< Exiting FailingAction()");
        throw new ApplicationException("This action is supposed to fail");
    }
}