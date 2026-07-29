using FluentAssertions;
using Nop.Core.Infrastructure;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Infrastructure;

[TestFixture]
public class SingletonTests
{
    [Test]
    public void SingletonIsNullByDefault()
    {
		logger.LogDebug(">>> Entering SingletonIsNullByDefault()");
        var instance = Singleton<SingletonTests>.Instance;
        instance.Should().BeNull();
		logger.LogDebug("<<< Exiting SingletonIsNullByDefault()");
    }

    [Test]
    public void SingletonsShareSameSingletonsDictionary()
    {
		logger.LogDebug(">>> Entering SingletonsShareSameSingletonsDictionary()");
        Singleton<int>.Instance = 1;
        Singleton<double>.Instance = 2.0;

        Singleton<int>.AllSingletons.Should().BeSameAs(Singleton<double>.AllSingletons);
        BaseSingleton.AllSingletons[typeof(int)].Should().Be(1);
        BaseSingleton.AllSingletons[typeof(double)].Should().Be(2.0M);
		logger.LogDebug("<<< Exiting SingletonsShareSameSingletonsDictionary()");
    }

    [Test]
    public void SingletonDictionaryIsCreatedByDefault()
    {
		logger.LogDebug(">>> Entering SingletonDictionaryIsCreatedByDefault()");
        var instance = SingletonDictionary<SingletonTests, object>.Instance;
        instance.Should().NotBeNull();
		logger.LogDebug("<<< Exiting SingletonDictionaryIsCreatedByDefault()");
    }

    [Test]
    public void SingletonDictionaryCanStoreStuff()
    {
		logger.LogDebug(">>> Entering SingletonDictionaryCanStoreStuff()");
        var instance = SingletonDictionary<Type, SingletonTests>.Instance;
        instance[typeof(SingletonTests)] = this;
        instance[typeof(SingletonTests)].Should().BeSameAs(this);
		logger.LogDebug("<<< Exiting SingletonDictionaryCanStoreStuff()");
    }

    [Test]
    public void SingletonListIsCreatedByDefault()
    {
		logger.LogDebug(">>> Entering SingletonListIsCreatedByDefault()");
        var instance = SingletonList<SingletonTests>.Instance;
        instance.Should().NotBeNull();
		logger.LogDebug("<<< Exiting SingletonListIsCreatedByDefault()");
    }

    [Test]
    public void SingletonListCanStoreItems()
    {
		logger.LogDebug(">>> Entering SingletonListCanStoreItems()");
        var instance = SingletonList<SingletonTests>.Instance;
        instance.Insert(0, this);
        instance[0].Should().BeSameAs(this);
		logger.LogDebug("<<< Exiting SingletonListCanStoreItems()");
    }
}