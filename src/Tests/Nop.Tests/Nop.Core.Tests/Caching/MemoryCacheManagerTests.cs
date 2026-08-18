using FluentAssertions;
using Nop.Core.Caching;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Core.Tests.Caching;

[TestFixture]
public class MemoryCacheManagerTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private MemoryCacheManager _staticCacheManager;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _staticCacheManager = GetService<IStaticCacheManager>() as MemoryCacheManager;
		_logger.LogDebug("Setup(): _staticCacheManager → {_staticCacheManager}", _staticCacheManager);
		_logger.LogDebug("<<< Exiting Setup()");
    }

    [TearDown]
    public async Task TaskTearDown()
    {
		_logger.LogDebug(">>> Entering TaskTearDown()");
        await _staticCacheManager.ClearAsync();
		_logger.LogDebug("<<< Exiting TaskTearDown()");
    }

    [Test]
    public async Task CanSetAndGetObjectFromCache()
    {
		_logger.LogDebug(">>> Entering CanSetAndGetObjectFromCache()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 3);
        var rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), () => 0);
		_logger.LogDebug("CanSetAndGetObjectFromCache(): rez → {Rez}", rez);
		_logger.LogInformation("CanSetAndGetObjectFromCache(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
        rez.Should().Be(3);
		_logger.LogDebug("<<< Exiting CanSetAndGetObjectFromCache()");
    }

    [Test]
    public async Task DoesNotIgnoreKeyCase()
    {
		_logger.LogDebug(">>> Entering DoesNotIgnoreKeyCase()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _staticCacheManager.SetAsync(new CacheKey("Some_Key_1"), 3);
        var rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), () => 0);
		_logger.LogDebug("DoesNotIgnoreKeyCase(): rez → {Rez}", rez);
		_logger.LogInformation("DoesNotIgnoreKeyCase(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
        rez.Should().Be(0);
		_logger.LogDebug("<<< Exiting DoesNotIgnoreKeyCase()");
    }

    [Test]
    public async Task CanValidateWhetherObjectIsCached()
    {
		_logger.LogDebug(">>> Entering CanValidateWhetherObjectIsCached()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 3);
        await _staticCacheManager.SetAsync(new CacheKey("some_key_2"), 4);

        var rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), () => 2);
		_logger.LogDebug("CanValidateWhetherObjectIsCached(): rez → {Rez}", rez);
        rez.Should().Be(3);
        rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_2"), () => 2);
        rez.Should().Be(4);
		_logger.LogInformation("CanValidateWhetherObjectIsCached(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting CanValidateWhetherObjectIsCached()");
    }

    [Test]
    public async Task CanClearCache()
    {
		_logger.LogDebug(">>> Entering CanClearCache()");
        await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 3);

        await _staticCacheManager.ClearAsync();

        var rez = await _staticCacheManager.GetAsync<object>(new CacheKey("some_key_1"));
		_logger.LogDebug("CanClearCache(): rez → {Rez}", rez);
        rez.Should().BeNull();
		_logger.LogDebug("<<< Exiting CanClearCache()");
    }

    [Test]
    public async Task GetReturnsValueIfSet()
    {
		_logger.LogDebug(">>> Entering GetReturnsValueIfSet()");
        var key = new CacheKey("some_key_1");
        await _staticCacheManager.SetAsync(key, 3);
		_logger.LogWarning("GetReturnsValueIfSet(): key → {Key}", key);
        var res = await _staticCacheManager.GetAsync<int>(key);
		_logger.LogDebug("GetReturnsValueIfSet(): res → {Res}", res);
        res.Should().Be(3);
		_logger.LogDebug("<<< Exiting GetReturnsValueIfSet()");
    }

    [Test]
    public async Task GetReturnsDefaultIfNotSet()
    {
		_logger.LogDebug(">>> Entering GetReturnsDefaultIfNotSet()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = new CacheKey("some_key_1");
        var res = await _staticCacheManager.GetAsync(key, 1);
		_logger.LogWarning("GetReturnsDefaultIfNotSet(): key → {Key}", key);
		_logger.LogDebug("GetReturnsDefaultIfNotSet(): res → {Res}", res);
        res.Should().Be(1);
        res = await _staticCacheManager.GetAsync<int>(key);
        res.Should().Be(0);
		_logger.LogInformation("GetReturnsDefaultIfNotSet(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetReturnsDefaultIfNotSet()");
    }

    [Test]
    public async Task CanRemoveByPrefix()
    {
		_logger.LogDebug(">>> Entering CanRemoveByPrefix()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 1);
        await _staticCacheManager.SetAsync(new CacheKey("some_key_2"), 2);
        await _staticCacheManager.SetAsync(new CacheKey("some_other_key"), 3);

        await _staticCacheManager.RemoveByPrefixAsync("some_key");

        var result = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), 0);
		_logger.LogDebug("CanRemoveByPrefix(): result → {Result}", result);
        result.Should().Be(0);
        result = await _staticCacheManager.GetAsync(new CacheKey("some_key_2"), 0);
        result.Should().Be(0);
        result = await _staticCacheManager.GetAsync(new CacheKey("some_other_key"), 0);
        result.Should().Be(3);
		_logger.LogInformation("CanRemoveByPrefix(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting CanRemoveByPrefix()");
    }

    [Test]
    public async Task ExecutesSetInOrder()
    {
		_logger.LogDebug(">>> Entering ExecutesSetInOrder()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await Task.WhenAll(Enumerable.Range(1, 5).Select(i => _staticCacheManager.SetAsync(new CacheKey("some_key_1"), i)));
        var value = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), 0);
		_logger.LogDebug("ExecutesSetInOrder(): value → {Value}", value);
		_logger.LogInformation("ExecutesSetInOrder(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
        value.Should().Be(5);
		_logger.LogDebug("<<< Exiting ExecutesSetInOrder()");
    }

    [Test]
    public async Task GetsLazily()
    {
		_logger.LogDebug(">>> Entering GetsLazily()");
        var xs = new int[5];
        await Task.WhenAll(xs.Select((_, i) => _staticCacheManager.GetAsync(
            new CacheKey("some_key_1"),
			_logger.LogDebug(">>> Entering async()");
			_logger.LogDebug("<<< Exiting async()");
            async () =>
            {
                xs[i] = 1;
                await Task.Delay(10);
                return i;
            })));
        var value = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), () => Task.FromResult(-1));
        value.Should().Be(0);
        xs.Sum().Should().Be(1);
    }

    [Test]
    public async Task SholThrowsExceptionButNotCacheIt()
    {
		_logger.LogDebug(">>> Entering SholThrowsExceptionButNotCacheIt()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var cacheKey = new CacheKey("some_key_1");

		_logger.LogDebug("SholThrowsExceptionButNotCacheIt(): cacheKey → {CacheKey}", cacheKey);
        Assert.ThrowsAsync<ApplicationException>(() => _staticCacheManager.GetAsync(
            cacheKey,
            Task<object> () => throw new ApplicationException()));

        //should not cache exception
        var rez = await _staticCacheManager.GetAsync(cacheKey, Task<object> () => Task.FromResult((object)1));
		_logger.LogDebug("SholThrowsExceptionButNotCacheIt(): rez → {Rez}", rez);
        rez.Should().Be(1);

        await _staticCacheManager.RemoveAsync(cacheKey);

        Assert.ThrowsAsync<ApplicationException>(() => _staticCacheManager.GetAsync(
            cacheKey,
            Task<object> () => throw new ApplicationException()));

        //should not cache exception
        rez = await _staticCacheManager.GetAsync(cacheKey, (object)1);
        rez.Should().Be(1);

        await _staticCacheManager.RemoveAsync(cacheKey);

        Assert.ThrowsAsync<ApplicationException>(() => _staticCacheManager.GetAsync<object>(
            cacheKey,
            () => throw new ApplicationException()));

        //should not cache exception
        rez = await _staticCacheManager.GetAsync(cacheKey, () => (object)1);
        rez.Should().Be(1);

        await _staticCacheManager.RemoveAsync(cacheKey);

        Assert.ThrowsAsync<ApplicationException>(() => _staticCacheManager.GetAsync<object>(
            cacheKey,
            () => throw new ApplicationException()));

        //should not cache exception
        rez = await _staticCacheManager.GetAsync(cacheKey);
        rez.Should().BeNull();
		_logger.LogInformation("SholThrowsExceptionButNotCacheIt(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting SholThrowsExceptionButNotCacheIt()");
    }

    [Test]
    public async Task CanGetAsObject()
    {
		_logger.LogDebug(">>> Entering CanGetAsObject()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = new CacheKey("some_key_1");
        await _staticCacheManager.SetAsync(key, 1);
		_logger.LogWarning("CanGetAsObject(): key → {Key}", key);
        var obj = await _staticCacheManager.GetAsync(key);
		_logger.LogDebug("CanGetAsObject(): obj → {Obj}", obj);
        obj.Should().Be(1);
        obj = await _staticCacheManager.GetAsync(new CacheKey("some_key_2"));
        obj.Should().BeNull();
		_logger.LogInformation("CanGetAsObject(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting CanGetAsObject()");
    }

    [Test]
    public async Task CanGetNullButNotCacheIt()
    {
		_logger.LogDebug(">>> Entering CanGetNullButNotCacheIt()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = new CacheKey("some_key_1");

		_logger.LogWarning("CanGetNullButNotCacheIt(): key → {Key}", key);
        var obj = await _staticCacheManager.GetAsync<object>(
            key, () => null);
			_logger.LogDebug("CanGetNullButNotCacheIt(): obj → {Obj}", obj);

        obj.Should().BeNull();

        obj = await _staticCacheManager.GetAsync(
            key, Task<object> () => Task.FromResult((object)null));

        obj.Should().BeNull();

        obj = await _staticCacheManager.GetAsync<object>(key, () => 1);
        obj.Should().Be(1);

        obj = await _staticCacheManager.GetAsync<object>(key, () => null);
        obj.Should().Be(1);

        obj = await _staticCacheManager.GetAsync(
            key, Task<object> () => Task.FromResult((object)null));

        obj.Should().Be(1);

        await _staticCacheManager.RemoveAsync(key);

        await _staticCacheManager.SetAsync<object>(key, null);

        obj = await _staticCacheManager.GetAsync<int>(key);
        obj.Should().Be(0);
		_logger.LogInformation("CanGetNullButNotCacheIt(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting CanGetNullButNotCacheIt()");
    }
}