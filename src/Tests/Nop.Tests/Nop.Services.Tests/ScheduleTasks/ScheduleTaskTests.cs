using FluentAssertions;
using Nop.Core.Caching;
using Nop.Services.Caching;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.ScheduleTasks;

[TestFixture]
public class ScheduleTaskTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IStaticCacheManager _staticCacheManager;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _staticCacheManager = GetService<IStaticCacheManager>();
		_logger.LogDebug("SetUp(): _staticCacheManager → {_staticCacheManager}", _staticCacheManager);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [Test]
    public async Task TestClearCacheTask()
    {
		_logger.LogDebug(">>> Entering TestClearCacheTask()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var test = new ClearCacheTask(_staticCacheManager);

		_logger.LogDebug("TestClearCacheTask(): test → {Test}", test);
        var key = new CacheKey("test_key_1") { CacheTime = 30 };
		_logger.LogWarning("TestClearCacheTask(): key → {Key}", key);

        await _staticCacheManager.SetAsync(key, "test data");
        var data = await _staticCacheManager.GetAsync(key, () => string.Empty);
		_logger.LogDebug("TestClearCacheTask(): data → {Data}", data);
        data.Should().NotBeEmpty();
        await test.ExecuteAsync();
        data = await _staticCacheManager.GetAsync(key, () => string.Empty);
        data.Should().BeEmpty();
		_logger.LogInformation("TestClearCacheTask(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting TestClearCacheTask()");
    }
}