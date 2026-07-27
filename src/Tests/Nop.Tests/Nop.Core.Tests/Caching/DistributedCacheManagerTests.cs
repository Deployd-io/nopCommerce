using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Caching;
using Nop.Services.Caching;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Caching;

[TestFixture]
public class DistributedCacheManagerTests : BaseNopTest
{
    private MemoryDistributedCacheManager _staticCacheManager;
    private IDistributedCache _distributedCache;
    private IServiceScopeFactory _serviceScopeFactory;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _staticCacheManager = GetService<MemoryDistributedCacheManager>();
        _distributedCache = GetService<IDistributedCache>();
		logger.LogDebug("Setup(): _staticCacheManager → {_staticCacheManager}", _staticCacheManager);
		logger.LogDebug("Setup(): _distributedCache → {_distributedCache}", _distributedCache);
        _serviceScopeFactory = GetService<IServiceScopeFactory>();
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _serviceScopeFactory → {_serviceScopeFactory}", _serviceScopeFactory);
    }

    [TearDown]
    public async Task TaskTearDown()
    {
        await _staticCacheManager.ClearAsync();
		logger.LogDebug("<<< Exiting TaskTearDown()");
    }

    [Test]
    public async Task CanSetObjectInCacheAndWillTrackIfRemoved()
    {
		logger.LogDebug(">>> Entering CanSetObjectInCacheAndWillTrackIfRemoved()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 1);
        (await _distributedCache.GetAsync("some_key_1")).Should().NotBeNullOrEmpty();
        await _staticCacheManager.RemoveByPrefixAsync("some_key_1");
        (await _distributedCache.GetAsync("some_key_1")).Should().BeNullOrEmpty();
		logger.LogDebug("<<< Exiting CanSetObjectInCacheAndWillTrackIfRemoved()");
		logger.LogInformation("CanSetObjectInCacheAndWillTrackIfRemoved(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
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
    public async Task CanGetAsyncFromCacheAndWillTrackIfRemoved()
    {
		logger.LogDebug(">>> Entering CanGetAsyncFromCacheAndWillTrackIfRemoved()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _distributedCache.SetAsync("some_key_2", Encoding.UTF8.GetBytes("2"));
        await _staticCacheManager.GetAsync(new CacheKey("some_key_2"), () => 2);
        await _staticCacheManager.RemoveByPrefixAsync("some_key_2");
        (await _distributedCache.GetAsync("some_key_2")).Should().BeNullOrEmpty();
		logger.LogDebug("<<< Exiting CanGetAsyncFromCacheAndWillTrackIfRemoved()");
		logger.LogInformation("CanGetAsyncFromCacheAndWillTrackIfRemoved(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

    [Test]
    public async Task CanGetFromCacheAndWillTrackRemoved()
    {
		logger.LogDebug(">>> Entering CanGetFromCacheAndWillTrackRemoved()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _distributedCache.SetAsync("some_key_3", Encoding.UTF8.GetBytes("3"));
        _staticCacheManager.Get(new CacheKey("some_key_3"), () => 3);
        await _staticCacheManager.RemoveByPrefixAsync("some_key_3");
        (await _distributedCache.GetAsync("some_key_3")).Should().BeNullOrEmpty();
		logger.LogDebug("<<< Exiting CanGetFromCacheAndWillTrackRemoved()");
		logger.LogInformation("CanGetFromCacheAndWillTrackRemoved(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

    [Test]
    public async Task CanClearCache()
    {
		logger.LogDebug(">>> Entering CanClearCache()");
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var manager = GetService<MemoryDistributedCacheManager>(scope);
            manager.Equals(_staticCacheManager).Should().BeFalse();
			logger.LogDebug("CanClearCache(): manager → {Manager}", manager);
            await manager.SetAsync(new CacheKey("some_key_1"), 1);
            await manager.SetAsync(new CacheKey("some_key_2"), 2);
            await manager.SetAsync(new CacheKey("some_key_3"), 3);
        }

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var manager = GetService<MemoryDistributedCacheManager>(scope);
            manager.Equals(_staticCacheManager).Should().BeFalse();
            manager.Get<int?>(new CacheKey("some_key_1"), null).Should().Be(1);
            manager.Get<int?>(new CacheKey("some_key_2"), null).Should().Be(2);
            manager.Get<int?>(new CacheKey("some_key_3"), null).Should().Be(3);
            await manager.ClearAsync();
        }

        _staticCacheManager.Get<int?>(new CacheKey("some_key_1"), () => null).Should().BeNull();
        _staticCacheManager.Get<int?>(new CacheKey("some_key_2"), () => null).Should().BeNull();
        _staticCacheManager.Get<int?>(new CacheKey("some_key_3"), () => null).Should().BeNull();
		logger.LogDebug("<<< Exiting CanClearCache()");
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

        var result = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), () => 0);
        result.Should().Be(0);
		logger.LogDebug("CanRemoveByPrefix(): result → {Result}", result);
        result = await _staticCacheManager.GetAsync(new CacheKey("some_key_2"), () => 0);
        result.Should().Be(0);
        result = await _staticCacheManager.GetAsync(new CacheKey("some_other_key"), () => 0);
        result.Should().Be(3);
		logger.LogDebug("<<< Exiting CanRemoveByPrefix()");
		logger.LogInformation("CanRemoveByPrefix(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

    [Test]
    public async Task CanGet()
    {
		logger.LogDebug(">>> Entering CanGet()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _distributedCache.SetAsync("some_key_1", Encoding.UTF8.GetBytes("1"));
        await _distributedCache.SetAsync("some_key_2", Encoding.UTF8.GetBytes("2"));
        await _distributedCache.SetAsync("some_key_3", Encoding.UTF8.GetBytes("3"));

        _staticCacheManager.Get<int?>(new CacheKey("some_key_1"), null).Should().Be(1);
        _staticCacheManager.Get<int?>(new CacheKey("some_key_2"), null).Should().Be(2);
        _staticCacheManager.Get<int?>(new CacheKey("some_key_3"), null).Should().Be(3);

        _staticCacheManager.Get<int?>(new CacheKey("some_key_4"), () => 4).Should().Be(4);

        var rez = await _staticCacheManager.GetAsync<int?>(new CacheKey("some_key_1"), () => 3);
        rez.Should().Be(1);
		logger.LogDebug("<<< Exiting CanGet()");
		logger.LogInformation("CanGet(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
        rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"),
            async () => int.Parse(Encoding.UTF8.GetString(await _distributedCache.GetAsync("some_key_3"))));
			logger.LogDebug("CanGet(): rez → {Rez}", rez);
        rez.Should().Be(1);
    }

    [Test]
    public async Task CanRemove()
    {
		logger.LogDebug(">>> Entering CanRemove()");
        await _distributedCache.SetAsync("some_key_1", Encoding.UTF8.GetBytes("1"));
        await _distributedCache.SetAsync("some_key_2", Encoding.UTF8.GetBytes("2"));
        await _staticCacheManager.SetAsync(new CacheKey("some_key_3"), "3");

        _staticCacheManager.Get<string>(new CacheKey("some_key_1"), null).Should().Be("1");
        _staticCacheManager.Get<string>(new CacheKey("some_key_2"), null).Should().Be("2");
        _staticCacheManager.Get<string>(new CacheKey("some_key_3"), null).Should().Be("3");

        await _staticCacheManager.RemoveAsync(new CacheKey("some_key_1"));
        await _staticCacheManager.RemoveAsync(new CacheKey("some_key_2"));
        await _staticCacheManager.RemoveAsync(new CacheKey("some_key_3"));

        _distributedCache.Get("some_key_1").Should().BeNullOrEmpty();
        _distributedCache.Get("some_key_2").Should().BeNullOrEmpty();
        _distributedCache.Get("some_key_3").Should().BeNullOrEmpty();
		logger.LogDebug("<<< Exiting CanRemove()");
    }

    [Test]
    public async Task SholdThrowsExceptionButNotCacheIt()
    {
		logger.LogDebug(">>> Entering SholdThrowsExceptionButNotCacheIt()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var cacheKey = new CacheKey("some_key_1");

		logger.LogDebug("SholdThrowsExceptionButNotCacheIt(): cacheKey → {CacheKey}", cacheKey);
        Assert.ThrowsAsync<ApplicationException>(() => _staticCacheManager.GetAsync(
            cacheKey,
            Task<object> () => throw new ApplicationException()));

        //should not cache exception
        var rez = await _staticCacheManager.GetAsync(cacheKey, Task<object> () => Task.FromResult((object)1));
        rez.Should().Be(1);
		logger.LogDebug("<<< Exiting SholdThrowsExceptionButNotCacheIt()");
		logger.LogInformation("SholdThrowsExceptionButNotCacheIt(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		logger.LogDebug("SholdThrowsExceptionButNotCacheIt(): rez → {Rez}", rez);
    }

    [Test]
    public async Task ExecutesSetInOrder()
    {
		logger.LogDebug(">>> Entering ExecutesSetInOrder()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await Task.WhenAll(Enumerable.Range(1, 5).Select(i => _staticCacheManager.SetAsync(new CacheKey("some_key_1"), i)));
        var value = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), () => Task.FromResult(0));
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
}