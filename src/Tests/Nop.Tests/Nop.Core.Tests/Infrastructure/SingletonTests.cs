using FluentAssertions;
using Nop.Core.Infrastructure;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Core.Tests.Infrastructure;

[TestFixture]
public class SingletonTests
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    [Test]
    public void SingletonIsNullByDefault()
    {
		_logger.LogDebug(">>> Entering SingletonIsNullByDefault()");
        var instance = Singleton<SingletonTests>.Instance;
        instance.Should().BeNull();
		_logger.LogDebug("<<< Exiting SingletonIsNullByDefault()");
    }

    [Test]
    public void SingletonsShareSameSingletonsDictionary()
    {
		_logger.LogDebug(">>> Entering SingletonsShareSameSingletonsDictionary()");
        Singleton<int>.Instance = 1;
        Singleton<double>.Instance = 2.0;

        Singleton<int>.AllSingletons.Should().BeSameAs(Singleton<double>.AllSingletons);
        BaseSingleton.AllSingletons[typeof(int)].Should().Be(1);
        BaseSingleton.AllSingletons[typeof(double)].Should().Be(2.0M);
		_logger.LogDebug("<<< Exiting SingletonsShareSameSingletonsDictionary()");
    }

    [Test]
    public void SingletonDictionaryIsCreatedByDefault()
    {
		_logger.LogDebug(">>> Entering SingletonDictionaryIsCreatedByDefault()");
        var instance = SingletonDictionary<SingletonTests, object>.Instance;
        instance.Should().NotBeNull();
		_logger.LogDebug("<<< Exiting SingletonDictionaryIsCreatedByDefault()");
    }

    [Test]
    public void SingletonDictionaryCanStoreStuff()
    {
		_logger.LogDebug(">>> Entering SingletonDictionaryCanStoreStuff()");
        var instance = SingletonDictionary<Type, SingletonTests>.Instance;
        instance[typeof(SingletonTests)] = this;
        instance[typeof(SingletonTests)].Should().BeSameAs(this);
		_logger.LogDebug("<<< Exiting SingletonDictionaryCanStoreStuff()");
    }

    [Test]
    public void SingletonListIsCreatedByDefault()
    {
		_logger.LogDebug(">>> Entering SingletonListIsCreatedByDefault()");
        var instance = SingletonList<SingletonTests>.Instance;
        instance.Should().NotBeNull();
		_logger.LogDebug("<<< Exiting SingletonListIsCreatedByDefault()");
    }

    [Test]
    public void SingletonListCanStoreItems()
    {
		_logger.LogDebug(">>> Entering SingletonListCanStoreItems()");
        var instance = SingletonList<SingletonTests>.Instance;
        instance.Insert(0, this);
        instance[0].Should().BeSameAs(this);
		_logger.LogDebug("<<< Exiting SingletonListCanStoreItems()");
    }
}