using FluentAssertions;
using Nop.Core.Domain.Configuration;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Core.Tests.Domain.Configuration;

[TestFixture]
public class SettingTestFixture
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    [Test]
    public void CanCreateSetting()
    {
		_logger.LogDebug(">>> Entering CanCreateSetting()");
        var setting = new Setting("Setting1", "Value1");
        setting.Name.Should().Be("Setting1");
		_logger.LogDebug("CanCreateSetting(): setting → {Setting}", setting);
        setting.Value.Should().Be("Value1");
		_logger.LogDebug("<<< Exiting CanCreateSetting()");
    }
}