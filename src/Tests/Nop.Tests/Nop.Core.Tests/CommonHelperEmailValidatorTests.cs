using FluentAssertions;
using Nop.Core;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Core.Tests;

[TestFixture]
public class CommonHelperEmailValidatorTests
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    [Test]
    public void WhenTheTextIsAValidEmailAddressThenTheValidatorShouldPass()
    {
		_logger.LogDebug(">>> Entering WhenTheTextIsAValidEmailAddressThenTheValidatorShouldPass()");
        var email = "testperson@gmail.com";
        var result = CommonHelper.IsValidEmail(email);
		_logger.LogDebug("WhenTheTextIsAValidEmailAddressThenTheValidatorShouldPass(): result → {Result}", result);
		_logger.LogDebug("WhenTheTextIsAValidEmailAddressThenTheValidatorShouldPass(): email → {Email}", email);
        result.Should().BeTrue();
		_logger.LogDebug("<<< Exiting WhenTheTextIsAValidEmailAddressThenTheValidatorShouldPass()");
    }

    [Test]
    public void WhenTheTextIsAValidEmailAddressIncludingPlusValidatorShouldPass()
    {
		_logger.LogDebug(">>> Entering WhenTheTextIsAValidEmailAddressIncludingPlusValidatorShouldPass()");
        var email = "testperson+label@gmail.com";
        var result = CommonHelper.IsValidEmail(email);
		_logger.LogDebug("WhenTheTextIsAValidEmailAddressIncludingPlusValidatorShouldPass(): result → {Result}", result);
		_logger.LogDebug("WhenTheTextIsAValidEmailAddressIncludingPlusValidatorShouldPass(): email → {Email}", email);
        result.Should().BeTrue();
		_logger.LogDebug("<<< Exiting WhenTheTextIsAValidEmailAddressIncludingPlusValidatorShouldPass()");
    }

    [Test]
    public void WhenTheTextIsNullThenTheValidatorShouldFail()
    {
		_logger.LogDebug(">>> Entering WhenTheTextIsNullThenTheValidatorShouldFail()");
        var result = CommonHelper.IsValidEmail(null);
        result.Should().BeFalse();
		_logger.LogDebug("<<< Exiting WhenTheTextIsNullThenTheValidatorShouldFail()");
		_logger.LogDebug("WhenTheTextIsNullThenTheValidatorShouldFail(): result → {Result}", result);
    }

    [Test]
    public void WhenTheTextIsEmptyThenTheValidatorShouldFail()
    {
		_logger.LogDebug(">>> Entering WhenTheTextIsEmptyThenTheValidatorShouldFail()");
        var email = string.Empty;
        var result = CommonHelper.IsValidEmail(email);
		_logger.LogDebug("WhenTheTextIsEmptyThenTheValidatorShouldFail(): result → {Result}", result);
		_logger.LogDebug("WhenTheTextIsEmptyThenTheValidatorShouldFail(): email → {Email}", email);
        result.Should().BeFalse();
		_logger.LogDebug("<<< Exiting WhenTheTextIsEmptyThenTheValidatorShouldFail()");
    }

    [Test]
    public void WhenTheTextIsNotAValidEmailAddressThenTheValidatorShouldFail()
    {
		_logger.LogDebug(">>> Entering WhenTheTextIsNotAValidEmailAddressThenTheValidatorShouldFail()");
        const string email = "testperso";
        var result = CommonHelper.IsValidEmail(email);
		_logger.LogDebug("WhenTheTextIsNotAValidEmailAddressThenTheValidatorShouldFail(): result → {Result}", result);
		_logger.LogDebug("WhenTheTextIsNotAValidEmailAddressThenTheValidatorShouldFail(): email → {Email}", email);
        result.Should().BeFalse();
		_logger.LogDebug("<<< Exiting WhenTheTextIsNotAValidEmailAddressThenTheValidatorShouldFail()");
    }

    [Test]
    public void ThisShouldNotHang()
    {
		_logger.LogDebug(">>> Entering ThisShouldNotHang()");
        const string email = "thisisaverylongstringcodeplex.com";
        var result = CommonHelper.IsValidEmail(email);
		_logger.LogDebug("ThisShouldNotHang(): result → {Result}", result);
		_logger.LogDebug("ThisShouldNotHang(): email → {Email}", email);
        result.Should().BeFalse();
		_logger.LogDebug("<<< Exiting ThisShouldNotHang()");
    }

    [Test]
    public void WhenEmailAddressContainsUpperCasesThenTheValidatorShouldPass()
    {
		_logger.LogDebug(">>> Entering WhenEmailAddressContainsUpperCasesThenTheValidatorShouldPass()");
        var email = "testperson@gmail.com";
        var result = CommonHelper.IsValidEmail(email);
		_logger.LogDebug("WhenEmailAddressContainsUpperCasesThenTheValidatorShouldPass(): result → {Result}", result);
		_logger.LogDebug("WhenEmailAddressContainsUpperCasesThenTheValidatorShouldPass(): email → {Email}", email);
        result.Should().BeTrue();

        email = "TestPerson@gmail.com";
        result = CommonHelper.IsValidEmail(email);
        result.Should().BeTrue();
		_logger.LogDebug("<<< Exiting WhenEmailAddressContainsUpperCasesThenTheValidatorShouldPass()");
    }

    [Test]
    public void WhenEmailAddressContainsHyphenInDomainThenTheValidatorShouldPass()
    {
		_logger.LogDebug(">>> Entering WhenEmailAddressContainsHyphenInDomainThenTheValidatorShouldPass()");
        //underscore is not allowed in domain part 
        var email = "testperson@domain_with-hyphen.com";
		_logger.LogDebug("WhenEmailAddressContainsHyphenInDomainThenTheValidatorShouldPass(): email → {Email}", email);
        var result = CommonHelper.IsValidEmail(email);
		_logger.LogDebug("WhenEmailAddressContainsHyphenInDomainThenTheValidatorShouldPass(): result → {Result}", result);
        result.Should().BeFalse();

        email = "testperson@example-domain.net";
        result = CommonHelper.IsValidEmail(email);
        result.Should().BeTrue();
		_logger.LogDebug("<<< Exiting WhenEmailAddressContainsHyphenInDomainThenTheValidatorShouldPass()");
    }

    [Test]
    public void WhenEmailAddressContainsDifferentTLDsThenTheValidatorShouldPass()
    {
		_logger.LogDebug(">>> Entering WhenEmailAddressContainsDifferentTLDsThenTheValidatorShouldPass()");
        var email = "testperson@example.co.uk";
        var result = CommonHelper.IsValidEmail(email);
		_logger.LogDebug("WhenEmailAddressContainsDifferentTLDsThenTheValidatorShouldPass(): result → {Result}", result);
		_logger.LogDebug("WhenEmailAddressContainsDifferentTLDsThenTheValidatorShouldPass(): email → {Email}", email);
        result.Should().BeTrue();

        email = "testperson@example.museum";
        result = CommonHelper.IsValidEmail(email);
        result.Should().BeTrue();
		_logger.LogDebug("<<< Exiting WhenEmailAddressContainsDifferentTLDsThenTheValidatorShouldPass()");
    }
}