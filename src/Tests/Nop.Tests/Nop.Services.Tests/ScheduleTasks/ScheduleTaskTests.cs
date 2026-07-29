using FluentAssertions;
using Nop.Core.Caching;
using Nop.Services.Caching;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.ScheduleTasks;

[TestFixture]
public class ScheduleTaskTests : BaseNopTest
{
    private IStaticCacheManager _staticCacheManager;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _staticCacheManager = GetService<IStaticCacheManager>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _staticCacheManager → {_staticCacheManager}", _staticCacheManager);
    }

    [Test]
    public async Task TestClearCacheTask()
    {
		logger.LogDebug(">>> Entering TestClearCacheTask()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var test = new ClearCacheTask(_staticCacheManager);

		logger.LogDebug("TestClearCacheTask(): test → {Test}", test);
        var key = new CacheKey("test_key_1") { CacheTime = 30 };

		logger.LogWarning("TestClearCacheTask(): key → {Key}", key);
        await _staticCacheManager.SetAsync(key, "test data");
        var data = await _staticCacheManager.GetAsync(key, () => string.Empty);
        data.Should().NotBeEmpty();
		logger.LogDebug("TestClearCacheTask(): data → {Data}", data);
        await test.ExecuteAsync();
        data = await _staticCacheManager.GetAsync(key, () => string.Empty);
        data.Should().BeEmpty();
		logger.LogDebug("<<< Exiting TestClearCacheTask()");
		logger.LogInformation("TestClearCacheTask(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }
}