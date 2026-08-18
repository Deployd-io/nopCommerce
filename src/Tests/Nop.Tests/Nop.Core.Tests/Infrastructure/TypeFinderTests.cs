using FluentAssertions;
using Nop.Core.Infrastructure;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Core.Tests.Infrastructure;

[TestFixture]
public class TypeFinderTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    [Test]
    public void TypeFinderBenchmarkFindings()
    {
		_logger.LogDebug(">>> Entering TypeFinderBenchmarkFindings()");
        var finder = GetService<ITypeFinder>();
        var type = finder.FindClassesOfType<ISomeInterface>().ToList();
		_logger.LogDebug("TypeFinderBenchmarkFindings(): type → {Type}", type);
		_logger.LogDebug("TypeFinderBenchmarkFindings(): finder → {Finder}", finder);
        type.Count.Should().Be(1);
        typeof(ISomeInterface).IsAssignableFrom(type.FirstOrDefault()).Should().BeTrue();
		_logger.LogDebug("<<< Exiting TypeFinderBenchmarkFindings()");
    }

    public interface ISomeInterface;

    public class SomeClass : ISomeInterface;
}