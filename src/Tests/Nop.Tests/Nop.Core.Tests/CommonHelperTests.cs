using FluentAssertions;
using Nop.Core;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Core.Tests;

[TestFixture]
public class CommonHelperTests
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    [Test]
    public void CanGetTypedValue()
    {
		_logger.LogDebug(">>> Entering CanGetTypedValue()");
        CommonHelper.To<int>("1000").Should().BeOfType(typeof(int));
        CommonHelper.To<int>("1000").Should().Be(1000);
		_logger.LogDebug("<<< Exiting CanGetTypedValue()");
    }
}