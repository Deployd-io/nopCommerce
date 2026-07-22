using FluentAssertions;
using Nop.Core;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests;

[TestFixture]
public class CommonHelperEmailValidatorTests
{
    [Test]
    public void WhenTheTextIsAValidEmailAddressThenTheValidatorShouldPass()
    {
		logger.LogDebug(">>> Entering WhenTheTextIsAValidEmailAddressThenTheValidatorShouldPass()");
        var email = "testperson@gmail.com";
        var result = CommonHelper.IsValidEmail(email);
		logger.LogDebug("WhenTheTextIsAValidEmailAddressThenTheValidatorShouldPass(): email → {Email}", email);
		logger.LogDebug("WhenTheTextIsAValidEmailAddressThenTheValidatorShouldPass(): result → {Result}", result);
        result.Should().BeTrue();
		logger.LogDebug("<<< Exiting WhenTheTextIsAValidEmailAddressThenTheValidatorShouldPass()");
    }

    [Test]
    public void WhenTheTextIsAValidEmailAddressIncludingPlusValidatorShouldPass()
    {
		logger.LogDebug(">>> Entering WhenTheTextIsAValidEmailAddressIncludingPlusValidatorShouldPass()");
        var email = "testperson+label@gmail.com";
        var result = CommonHelper.IsValidEmail(email);
		logger.LogDebug("WhenTheTextIsAValidEmailAddressIncludingPlusValidatorShouldPass(): email → {Email}", email);
		logger.LogDebug("WhenTheTextIsAValidEmailAddressIncludingPlusValidatorShouldPass(): result → {Result}", result);
        result.Should().BeTrue();
		logger.LogDebug("<<< Exiting WhenTheTextIsAValidEmailAddressIncludingPlusValidatorShouldPass()");
    }

    [Test]
    public void WhenTheTextIsNullThenTheValidatorShouldFail()
    {
		logger.LogDebug(">>> Entering WhenTheTextIsNullThenTheValidatorShouldFail()");
        var result = CommonHelper.IsValidEmail(null);
        result.Should().BeFalse();
		logger.LogDebug("<<< Exiting WhenTheTextIsNullThenTheValidatorShouldFail()");
		logger.LogDebug("WhenTheTextIsNullThenTheValidatorShouldFail(): result → {Result}", result);
    }

    [Test]
    public void WhenTheTextIsEmptyThenTheValidatorShouldFail()
    {
		logger.LogDebug(">>> Entering WhenTheTextIsEmptyThenTheValidatorShouldFail()");
        var email = string.Empty;
        var result = CommonHelper.IsValidEmail(email);
		logger.LogDebug("WhenTheTextIsEmptyThenTheValidatorShouldFail(): email → {Email}", email);
		logger.LogDebug("WhenTheTextIsEmptyThenTheValidatorShouldFail(): result → {Result}", result);
        result.Should().BeFalse();
		logger.LogDebug("<<< Exiting WhenTheTextIsEmptyThenTheValidatorShouldFail()");
    }

    [Test]
    public void WhenTheTextIsNotAValidEmailAddressThenTheValidatorShouldFail()
    {
		logger.LogDebug(">>> Entering WhenTheTextIsNotAValidEmailAddressThenTheValidatorShouldFail()");
        const string email = "testperso";
        var result = CommonHelper.IsValidEmail(email);
		logger.LogDebug("WhenTheTextIsNotAValidEmailAddressThenTheValidatorShouldFail(): email → {Email}", email);
		logger.LogDebug("WhenTheTextIsNotAValidEmailAddressThenTheValidatorShouldFail(): result → {Result}", result);
        result.Should().BeFalse();
		logger.LogDebug("<<< Exiting WhenTheTextIsNotAValidEmailAddressThenTheValidatorShouldFail()");
    }

    [Test]
    public void ThisShouldNotHang()
    {
		logger.LogDebug(">>> Entering ThisShouldNotHang()");
        const string email = "thisisaverylongstringcodeplex.com";
        var result = CommonHelper.IsValidEmail(email);
		logger.LogDebug("ThisShouldNotHang(): email → {Email}", email);
		logger.LogDebug("ThisShouldNotHang(): result → {Result}", result);
        result.Should().BeFalse();
		logger.LogDebug("<<< Exiting ThisShouldNotHang()");
    }

    [Test]
    public void WhenEmailAddressContainsUpperCasesThenTheValidatorShouldPass()
    {
		logger.LogDebug(">>> Entering WhenEmailAddressContainsUpperCasesThenTheValidatorShouldPass()");
        var email = "testperson@gmail.com";
        var result = CommonHelper.IsValidEmail(email);
		logger.LogDebug("WhenEmailAddressContainsUpperCasesThenTheValidatorShouldPass(): email → {Email}", email);
		logger.LogDebug("WhenEmailAddressContainsUpperCasesThenTheValidatorShouldPass(): result → {Result}", result);
        result.Should().BeTrue();

        email = "TestPerson@gmail.com";
        result = CommonHelper.IsValidEmail(email);
        result.Should().BeTrue();
		logger.LogDebug("<<< Exiting WhenEmailAddressContainsUpperCasesThenTheValidatorShouldPass()");
    }

    [Test]
    public void WhenEmailAddressContainsHyphenInDomainThenTheValidatorShouldPass()
    {
		logger.LogDebug(">>> Entering WhenEmailAddressContainsHyphenInDomainThenTheValidatorShouldPass()");
        //underscore is not allowed in domain part 
        var email = "testperson@domain_with-hyphen.com";
        var result = CommonHelper.IsValidEmail(email);
		logger.LogDebug("WhenEmailAddressContainsHyphenInDomainThenTheValidatorShouldPass(): email → {Email}", email);
		logger.LogDebug("WhenEmailAddressContainsHyphenInDomainThenTheValidatorShouldPass(): result → {Result}", result);
        result.Should().BeFalse();

        email = "testperson@example-domain.net";
        result = CommonHelper.IsValidEmail(email);
        result.Should().BeTrue();
		logger.LogDebug("<<< Exiting WhenEmailAddressContainsHyphenInDomainThenTheValidatorShouldPass()");
    }

    [Test]
    public void WhenEmailAddressContainsDifferentTLDsThenTheValidatorShouldPass()
    {
		logger.LogDebug(">>> Entering WhenEmailAddressContainsDifferentTLDsThenTheValidatorShouldPass()");
        var email = "testperson@example.co.uk";
        var result = CommonHelper.IsValidEmail(email);
		logger.LogDebug("WhenEmailAddressContainsDifferentTLDsThenTheValidatorShouldPass(): email → {Email}", email);
		logger.LogDebug("WhenEmailAddressContainsDifferentTLDsThenTheValidatorShouldPass(): result → {Result}", result);
        result.Should().BeTrue();

        email = "testperson@example.museum";
        result = CommonHelper.IsValidEmail(email);
        result.Should().BeTrue();
		logger.LogDebug("<<< Exiting WhenEmailAddressContainsDifferentTLDsThenTheValidatorShouldPass()");
    }
}
