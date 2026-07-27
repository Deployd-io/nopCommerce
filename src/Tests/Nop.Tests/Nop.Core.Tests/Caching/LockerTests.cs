using FluentAssertions;
using Nop.Core.Caching;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Caching;

[TestFixture]
public class LockerTests : BaseNopTest
{
    private ILocker _memCacheLocker;
    private ILocker _distCacheLocker;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _memCacheLocker = GetService<MemoryCacheLocker>();
        _distCacheLocker = GetService<DistributedCacheLocker>();
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _memCacheLocker → {_memCacheLocker}", _memCacheLocker);
		logger.LogDebug("Setup(): _distCacheLocker → {_distCacheLocker}", _distCacheLocker);
    }

    [Test]
    public async Task Distributed_CanPerformLockAsync()
    {
		logger.LogDebug(">>> Entering Distributed_CanPerformLockAsync()");
        await TestLockerAsync(_distCacheLocker);
		logger.LogDebug("<<< Exiting Distributed_CanPerformLockAsync()");
    }

    [Test]
    public async Task Memory_CanPerformLockAsync()
    {
		logger.LogDebug(">>> Entering Memory_CanPerformLockAsync()");
        await TestLockerAsync(_memCacheLocker);
		logger.LogDebug("<<< Exiting Memory_CanPerformLockAsync()");
		logger.LogDebug(">>> Entering TestLockerAsync(locker={Locker})", locker);
		logger.LogDebug("<<< Exiting TestLockerAsync(locker={Locker})", locker);
    }

    [Test]
    public async Task Distributed_CanRunWithHeartbeatAsync()
    {
		logger.LogDebug(">>> Entering Distributed_CanRunWithHeartbeatAsync()");
        await TestHeartbeatAsync(_distCacheLocker);
		logger.LogDebug("<<< Exiting Distributed_CanRunWithHeartbeatAsync()");
    }

    [Test]
    public async Task Memory_CanRunWithHeartbeatAsync()
    {
		logger.LogDebug(">>> Entering Memory_CanRunWithHeartbeatAsync()");
        await TestHeartbeatAsync(_memCacheLocker);
		logger.LogDebug("<<< Exiting Memory_CanRunWithHeartbeatAsync()");
		logger.LogDebug(">>> Entering TestHeartbeatAsync(locker={Locker})", locker);
		logger.LogDebug("<<< Exiting TestHeartbeatAsync(locker={Locker})", locker);
    }

    [Test]
    public async Task Distributed_CanCancelAsync()
    {
		logger.LogDebug(">>> Entering Distributed_CanCancelAsync()");
        await TestCancellationAsync(_distCacheLocker);
		logger.LogDebug("<<< Exiting Distributed_CanCancelAsync()");
    }

    [Test]
    public async Task Memory_CanCancelAsync()
    {
		logger.LogDebug(">>> Entering Memory_CanCancelAsync()");
        await TestCancellationAsync(_memCacheLocker);
		logger.LogDebug("<<< Exiting Memory_CanCancelAsync()");
		logger.LogDebug(">>> Entering TestCancellationAsync(locker={Locker})", locker);
		logger.LogError("Exception in TestCancellationAsync(locker={Locker}): {Arg1}", locker, ex.Message);
		logger.LogDebug("<<< Exiting TestCancellationAsync(locker={Locker})", locker);
    }

    private static async Task TestLockerAsync(ILocker locker)
    {
        var key = Guid.NewGuid().ToString();
        var expiration = TimeSpan.FromMinutes(2);
		logger.LogWarning("TestLockerAsync(locker={Locker}): key → {Key}", locker, key);
		logger.LogDebug("TestLockerAsync(locker={Locker}): expiration → {Expiration}", locker, expiration);

        var actionCount = 0;
        async Task action()
        {
			logger.LogDebug(">>> Entering action()");
            var res = await locker.PerformActionWithLockAsync(key, expiration,
                () =>
                {
                    Assert.Fail("Action in progress");
                    return Task.CompletedTask;
                });

            res.Should().BeFalse();
            actionCount++;
			logger.LogDebug("<<< Exiting action()");
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
		logger.LogWarning("TestHeartbeatAsync(locker={Locker}): key → {Key}", locker, key);
		logger.LogDebug("TestHeartbeatAsync(locker={Locker}): expiration → {Expiration}", locker, expiration);
        var heartbeat = TimeSpan.FromSeconds(10);

		logger.LogDebug("TestHeartbeatAsync(locker={Locker}): heartbeat → {Heartbeat}", locker, heartbeat);
        var actionCount = 0;
        async Task action(CancellationToken _)
        {
			logger.LogDebug(">>> Entering action(_={_})", _);
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
		logger.LogWarning("TestCancellationAsync(locker={Locker}): key → {Key}", locker, key);
		logger.LogDebug("TestCancellationAsync(locker={Locker}): expiration → {Expiration}", locker, expiration);
        var heartbeat = TimeSpan.FromMilliseconds(100);
        var delay = TimeSpan.FromSeconds(5);
		logger.LogDebug("TestCancellationAsync(locker={Locker}): heartbeat → {Heartbeat}", locker, heartbeat);
		logger.LogDebug("TestCancellationAsync(locker={Locker}): delay → {Delay}", locker, delay);

        async Task testAsync(Func<Task> cancel, CancellationTokenSource tokenSource = default)
        {
			logger.LogDebug(">>> Entering testAsync(cancel={Cancel},tokenSource={TokenSource})", cancel, tokenSource);
            var cancelled = false;
            async Task action(CancellationToken token)
            {
				logger.LogDebug(">>> Entering action(token={Token})", token);
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
					logger.LogError("Exception in TestCancellationAsync(locker={Locker}): {Arg1}", locker, ex.Message);
                    cancelled = true;
					logger.LogDebug("action(token={Token}): cancelled → {Cancelled}", token, cancelled);
                }
				logger.LogError("Exception in action(token={Token}): {Arg1}", token, ex.Message);
            }

			logger.LogError("Exception in action(token={Token}): {Arg1}", token, ex.Message);
            var task = locker.RunWithHeartbeatAsync(key, expiration, heartbeat, action, tokenSource);
            (await locker.IsTaskRunningAsync(key)).Should().BeTrue();
            await cancel();
            await task;
            cancelled.Should().BeTrue();
            (await locker.IsTaskRunningAsync(key)).Should().BeFalse();
			logger.LogDebug("<<< Exiting testAsync(cancel={Cancel},tokenSource={TokenSource})", cancel, tokenSource);
        }

        await testAsync(() => locker.CancelTaskAsync(key, expiration));
        var cancellationTokenSource = new CancellationTokenSource();
        await testAsync(() => Task.Run(cancellationTokenSource.Cancel), cancellationTokenSource);
    }

    private static Task FailingAction()
    {
		logger.LogDebug(">>> Entering FailingAction()");
        throw new ApplicationException("This action is supposed to fail");
    }
}