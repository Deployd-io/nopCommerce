using FluentAssertions;
using Nop.Core.Infrastructure;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Infrastructure;

[TestFixture]
public class TypeFinderTests : BaseNopTest
{
    [Test]
    public void TypeFinderBenchmarkFindings()
    {
		logger.LogDebug(">>> Entering TypeFinderBenchmarkFindings()");
        var finder = GetService<ITypeFinder>();
        var type = finder.FindClassesOfType<ISomeInterface>().ToList();
		logger.LogDebug("TypeFinderBenchmarkFindings(): finder → {Finder}", finder);
		logger.LogDebug("TypeFinderBenchmarkFindings(): type → {Type}", type);
        type.Count.Should().Be(1);
        typeof(ISomeInterface).IsAssignableFrom(type.FirstOrDefault()).Should().BeTrue();
		logger.LogDebug("<<< Exiting TypeFinderBenchmarkFindings()");
    }

    public interface ISomeInterface;

    public class SomeClass : ISomeInterface;
}