using FluentAssertions;
using Nop.Core;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests;

[TestFixture]
public class CommonHelperIpAddressValidatorTests
{
    [Test]
    public void WhenTheTextIsAValidIpv4AddressThenTheValidatorShouldPass()
    {
		logger.LogDebug(">>> Entering WhenTheTextIsAValidIpv4AddressThenTheValidatorShouldPass()");
        const string ip = "123.123.123.123";
        var result = CommonHelper.IsValidIpAddress(ip);
		logger.LogDebug("WhenTheTextIsAValidIpv4AddressThenTheValidatorShouldPass(): result → {Result}", result);
        result.Should().BeTrue();
		logger.LogDebug("<<< Exiting WhenTheTextIsAValidIpv4AddressThenTheValidatorShouldPass()");
    }

    [Test]
    public void WhenTheTextIsAValidIpv6AddressThenTheValidatorShouldPass()
    {
		logger.LogDebug(">>> Entering WhenTheTextIsAValidIpv6AddressThenTheValidatorShouldPass()");
        const string ip = "FE80:0000:0000:0000:0202:B3FF:FE1E:8329";
        var result = CommonHelper.IsValidIpAddress(ip);
		logger.LogDebug("WhenTheTextIsAValidIpv6AddressThenTheValidatorShouldPass(): result → {Result}", result);
        result.Should().BeTrue();
		logger.LogDebug("<<< Exiting WhenTheTextIsAValidIpv6AddressThenTheValidatorShouldPass()");
    }

    [Test]
    public void WhenTheTextIsNotAValidIpAddressThenTheValidatorShouldFail()
    {
		logger.LogDebug(">>> Entering WhenTheTextIsNotAValidIpAddressThenTheValidatorShouldFail()");
        const string ip = "abc";
        var result = CommonHelper.IsValidIpAddress(ip);
		logger.LogDebug("WhenTheTextIsNotAValidIpAddressThenTheValidatorShouldFail(): result → {Result}", result);
        result.Should().BeFalse();
		logger.LogDebug("<<< Exiting WhenTheTextIsNotAValidIpAddressThenTheValidatorShouldFail()");
    }

    [Test]
    public void WhenTheTextIsAnIpAddressButWithWrongRangeThenTheValidatorShouldFail()
    {
		logger.LogDebug(">>> Entering WhenTheTextIsAnIpAddressButWithWrongRangeThenTheValidatorShouldFail()");
        const string ip = "999.999.999.999";
        var result = CommonHelper.IsValidIpAddress(ip);
		logger.LogDebug("WhenTheTextIsAnIpAddressButWithWrongRangeThenTheValidatorShouldFail(): result → {Result}", result);
        result.Should().BeFalse();
		logger.LogDebug("<<< Exiting WhenTheTextIsAnIpAddressButWithWrongRangeThenTheValidatorShouldFail()");
    }
}
