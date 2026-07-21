using FluentAssertions;
using Nop.Core.Caching;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Caching;

[TestFixture]
public class MemoryCacheManagerTests : BaseNopTest
{
    private MemoryCacheManager _staticCacheManager;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _staticCacheManager = GetService<IStaticCacheManager>() as MemoryCacheManager;
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _staticCacheManager → {_staticCacheManager}", _staticCacheManager);
    }

    [TearDown]
    public async Task TaskTearDown()
    {
		logger.LogDebug(">>> Entering TaskTearDown()");
        await _staticCacheManager.ClearAsync();
		logger.LogDebug("<<< Exiting TaskTearDown()");
    }

    [Test]
    public async Task CanSetAndGetObjectFromCache()
    {
		logger.LogDebug(">>> Entering CanSetAndGetObjectFromCache()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 3);
        var rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), () => 0);
        rez.Should().Be(3);
		logger.LogDebug("<<< Exiting CanSetAndGetObjectFromCache()");
		logger.LogInformation("CanSetAndGetObjectFromCache(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		logger.LogDebug("CanSetAndGetObjectFromCache(): rez → {Rez}", rez);
    }

    [Test]
    public async Task DoesNotIgnoreKeyCase()
    {
		logger.LogDebug(">>> Entering DoesNotIgnoreKeyCase()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _staticCacheManager.SetAsync(new CacheKey("Some_Key_1"), 3);
        var rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), () => 0);
        rez.Should().Be(0);
		logger.LogDebug("<<< Exiting DoesNotIgnoreKeyCase()");
		logger.LogInformation("DoesNotIgnoreKeyCase(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		logger.LogDebug("DoesNotIgnoreKeyCase(): rez → {Rez}", rez);
    }

    [Test]
    public async Task CanValidateWhetherObjectIsCached()
    {
		logger.LogDebug(">>> Entering CanValidateWhetherObjectIsCached()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 3);
        await _staticCacheManager.SetAsync(new CacheKey("some_key_2"), 4);

        var rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), () => 2);
        rez.Should().Be(3);
		logger.LogDebug("CanValidateWhetherObjectIsCached(): rez → {Rez}", rez);
        rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_2"), () => 2);
        rez.Should().Be(4);
		logger.LogDebug("<<< Exiting CanValidateWhetherObjectIsCached()");
		logger.LogInformation("CanValidateWhetherObjectIsCached(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

    [Test]
    public async Task CanClearCache()
    {
		logger.LogDebug(">>> Entering CanClearCache()");
        await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 3);

        await _staticCacheManager.ClearAsync();

        var rez = await _staticCacheManager.GetAsync<object>(new CacheKey("some_key_1"));
        rez.Should().BeNull();
		logger.LogDebug("<<< Exiting CanClearCache()");
		logger.LogDebug("CanClearCache(): rez → {Rez}", rez);
    }

    [Test]
    public async Task GetReturnsValueIfSet()
    {
		logger.LogDebug(">>> Entering GetReturnsValueIfSet()");
        var key = new CacheKey("some_key_1");
        await _staticCacheManager.SetAsync(key, 3);
		logger.LogWarning("GetReturnsValueIfSet(): key → {Key}", key);
        var res = await _staticCacheManager.GetAsync<int>(key);
        res.Should().Be(3);
		logger.LogDebug("<<< Exiting GetReturnsValueIfSet()");
		logger.LogDebug("GetReturnsValueIfSet(): res → {Res}", res);
    }

    [Test]
    public async Task GetReturnsDefaultIfNotSet()
    {
		logger.LogDebug(">>> Entering GetReturnsDefaultIfNotSet()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = new CacheKey("some_key_1");
        var res = await _staticCacheManager.GetAsync(key, 1);
		logger.LogWarning("GetReturnsDefaultIfNotSet(): key → {Key}", key);
		logger.LogDebug("GetReturnsDefaultIfNotSet(): res → {Res}", res);
        res.Should().Be(1);
        res = await _staticCacheManager.GetAsync<int>(key);
        res.Should().Be(0);
		logger.LogDebug("<<< Exiting GetReturnsDefaultIfNotSet()");
		logger.LogInformation("GetReturnsDefaultIfNotSet(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

    [Test]
    public async Task CanRemoveByPrefix()
    {
		logger.LogDebug(">>> Entering CanRemoveByPrefix()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 1);
        await _staticCacheManager.SetAsync(new CacheKey("some_key_2"), 2);
        await _staticCacheManager.SetAsync(new CacheKey("some_other_key"), 3);

        await _staticCacheManager.RemoveByPrefixAsync("some_key");

        var result = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), 0);
        result.Should().Be(0);
		logger.LogDebug("CanRemoveByPrefix(): result → {Result}", result);
        result = await _staticCacheManager.GetAsync(new CacheKey("some_key_2"), 0);
        result.Should().Be(0);
        result = await _staticCacheManager.GetAsync(new CacheKey("some_other_key"), 0);
        result.Should().Be(3);
		logger.LogDebug("<<< Exiting CanRemoveByPrefix()");
		logger.LogInformation("CanRemoveByPrefix(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

    [Test]
    public async Task ExecutesSetInOrder()
    {
		logger.LogDebug(">>> Entering ExecutesSetInOrder()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await Task.WhenAll(Enumerable.Range(1, 5).Select(i => _staticCacheManager.SetAsync(new CacheKey("some_key_1"), i)));
        var value = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), 0);
        value.Should().Be(5);
		logger.LogDebug("<<< Exiting ExecutesSetInOrder()");
		logger.LogInformation("ExecutesSetInOrder(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		logger.LogDebug("ExecutesSetInOrder(): value → {Value}", value);
    }

    [Test]
    public async Task GetsLazily()
    {
		logger.LogDebug(">>> Entering GetsLazily()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var xs = new int[5];
        await Task.WhenAll(xs.Select((_, i) => _staticCacheManager.GetAsync(
            new CacheKey("some_key_1"),
            async () =>
            {
			logger.LogInformation("GetsLazily(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
                xs[i] = 1;
                await Task.Delay(10);
                return i;
            })));
        var value = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), () => Task.FromResult(-1));
        value.Should().Be(0);
        xs.Sum().Should().Be(1);
		logger.LogDebug("<<< Exiting GetsLazily()");
    }

    [Test]
    public async Task SholThrowsExceptionButNotCacheIt()
    {
		logger.LogDebug(">>> Entering SholThrowsExceptionButNotCacheIt()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var cacheKey = new CacheKey("some_key_1");

		logger.LogDebug("SholThrowsExceptionButNotCacheIt(): cacheKey → {CacheKey}", cacheKey);
        Assert.ThrowsAsync<ApplicationException>(() => _staticCacheManager.GetAsync(
            cacheKey,
            Task<object> () => throw new ApplicationException()));

        //should not cache exception
        var rez = await _staticCacheManager.GetAsync(cacheKey, Task<object> () => Task.FromResult((object)1));
        rez.Should().Be(1);
		logger.LogDebug("SholThrowsExceptionButNotCacheIt(): rez → {Rez}", rez);

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
		logger.LogDebug("<<< Exiting SholThrowsExceptionButNotCacheIt()");
		logger.LogInformation("SholThrowsExceptionButNotCacheIt(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

    [Test]
    public async Task CanGetAsObject()
    {
		logger.LogDebug(">>> Entering CanGetAsObject()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = new CacheKey("some_key_1");
        await _staticCacheManager.SetAsync(key, 1);
		logger.LogWarning("CanGetAsObject(): key → {Key}", key);
        var obj = await _staticCacheManager.GetAsync(key);
        obj.Should().Be(1);
		logger.LogDebug("CanGetAsObject(): obj → {Obj}", obj);
        obj = await _staticCacheManager.GetAsync(new CacheKey("some_key_2"));
        obj.Should().BeNull();
		logger.LogDebug("<<< Exiting CanGetAsObject()");
		logger.LogInformation("CanGetAsObject(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

    [Test]
    public async Task CanGetNullButNotCacheIt()
    {
		logger.LogDebug(">>> Entering CanGetNullButNotCacheIt()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = new CacheKey("some_key_1");

		logger.LogWarning("CanGetNullButNotCacheIt(): key → {Key}", key);
        var obj = await _staticCacheManager.GetAsync<object>(
            key, () => null);
			logger.LogDebug("CanGetNullButNotCacheIt(): obj → {Obj}", obj);

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
		logger.LogDebug("<<< Exiting CanGetNullButNotCacheIt()");
		logger.LogInformation("CanGetNullButNotCacheIt(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }
}
