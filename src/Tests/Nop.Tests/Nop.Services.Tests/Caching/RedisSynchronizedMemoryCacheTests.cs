using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Services.Caching;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Caching;

[TestFixture]
internal class RedisSynchronizedMemoryCacheTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected IStaticCacheManager CreateCacheInstance()
    {
		_logger.LogDebug(">>> Entering CreateCacheInstance()");
        var appSettings = Singleton<AppSettings>.Instance;
        var cacheKeyManager = new CacheKeyManager(new ConcurrentTrie<byte>());
        var redisSynchronizedMemoryCache = new RedisSynchronizedMemoryCache(new MemoryCache(new MemoryCacheOptions()),
            new TestRedisConnectionWrapper(), cacheKeyManager, appSettings);
			_logger.LogDebug("CreateCacheInstance(): cacheKeyManager → {CacheKeyManager}", cacheKeyManager);

		_logger.LogDebug("<<< Exiting CreateCacheInstance()");
        return new MemoryCacheManager(appSettings, redisSynchronizedMemoryCache, cacheKeyManager);
    }

    protected IList<KeyValuePair<CacheKey, string>> _values;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _values = new List<KeyValuePair<CacheKey, string>>();

		_logger.LogDebug("SetUp(): _values → {_values}", _values);
        for (var i = 1; i <= 5; i++)
            _values.Add(new(new CacheKey($"key_{i}"), i.ToString()));
			_logger.LogDebug("<<< Exiting SetUp()");
    }

    [Test]
    public async Task CanClearCacheAsync()
    {
		_logger.LogDebug(">>> Entering CanClearCacheAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var c1 = CreateCacheInstance();
        await c1.SetAsync(_values[0].Key, _values[0].Value);
		_logger.LogDebug("CanClearCacheAsync(): c1 → {C1}", c1);
        await c1.SetAsync(_values[2].Key, _values[2].Value);
        await c1.SetAsync(_values[4].Key, _values[4].Value);

        var c2 = CreateCacheInstance();
        await c2.SetAsync(_values[0].Key, _values[0].Value);
		_logger.LogDebug("CanClearCacheAsync(): c2 → {C2}", c2);
        await c2.SetAsync(_values[1].Key, _values[1].Value);
        await c2.SetAsync(_values[2].Key, _values[2].Value);

        var c3 = CreateCacheInstance();
        await c3.SetAsync(_values[4].Key, _values[0].Value);
		_logger.LogDebug("CanClearCacheAsync(): c3 → {C3}", c3);
        await c3.SetAsync(_values[3].Key, _values[1].Value);

        await c2.ClearAsync();

        (await c1.GetAsync(_values[4].Key)).Should().BeNull();
        (await c3.GetAsync(_values[4].Key)).Should().BeNull();
        (await c1.GetAsync(_values[3].Key)).Should().BeNull();
		_logger.LogDebug("<<< Exiting CanClearCacheAsync()");
		_logger.LogInformation("CanClearCacheAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

    [Test]
    public async Task CanRemoveByPrefixAsync()
    {
		_logger.LogDebug(">>> Entering CanRemoveByPrefixAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var c1 = CreateCacheInstance();
        await c1.SetAsync(new("key_1"), "1");
		_logger.LogDebug("CanRemoveByPrefixAsync(): c1 → {C1}", c1);
        await c1.SetAsync(new("key_12"), "12");
        await c1.SetAsync(new("key_11"), "11");
        await c1.SetAsync(new("key_111"), "111");
        await c1.SetAsync(new("key_2"), "2");
        await c1.SetAsync(new("key_22"), "22");

        var c2 = CreateCacheInstance();
        await c2.SetAsync(new("key_12"), "12");
		_logger.LogDebug("CanRemoveByPrefixAsync(): c2 → {C2}", c2);
        await c2.SetAsync(new("key_112"), "112");
        await c2.SetAsync(new("key_113"), "113");

        await c2.RemoveByPrefixAsync("key_11");

        (await c1.GetAsync(new("key_1"))).Should().NotBeNull();
        (await c1.GetAsync(new("key_11"))).Should().BeNull();
        (await c1.GetAsync(new("key_111"))).Should().BeNull();
        (await c1.GetAsync(new("key_2"))).Should().NotBeNull();
        (await c1.GetAsync(new("key_22"))).Should().NotBeNull();

        (await c2.GetAsync(new("key_12"))).Should().NotBeNull();

        await c1.RemoveByPrefixAsync("key_1");

        (await c2.GetAsync(new("key_12"))).Should().BeNull();
        (await c1.GetAsync(new("key_12"))).Should().BeNull();
        (await c1.GetAsync(new("key_1"))).Should().BeNull();
		_logger.LogDebug("<<< Exiting CanRemoveByPrefixAsync()");
		_logger.LogInformation("CanRemoveByPrefixAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }
}
