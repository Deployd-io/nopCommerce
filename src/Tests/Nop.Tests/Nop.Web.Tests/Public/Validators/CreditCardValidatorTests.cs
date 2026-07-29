using System.Globalization;
using FluentAssertions;
using Nop.Web.Framework.Validators;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators;

[TestFixture]
public class CreditCardValidatorTests
{
    private TestValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

        _validator = new TestValidator { v => v.RuleFor(x => x.CreditCard).IsCreditCard() };
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public async Task IsValidTests()
    {
		logger.LogDebug(">>> Entering IsValidTests()");
        // Optional value is not valid
        var result = await _validator.ValidateAsync(new Person { CreditCard = null });
        result.IsValid.Should().BeFalse();
		logger.LogDebug("IsValidTests(): result → {Result}", result);

        // Simplest valid value
        result = await _validator.ValidateAsync(new Person { CreditCard = "0000000000000000" });
        result.IsValid.Should().BeTrue();

        // Good checksum
        result = await _validator.ValidateAsync(new Person { CreditCard = "1234567890123452" });
        result.IsValid.Should().BeTrue();

        // Good checksum, with dashes
        result = await _validator.ValidateAsync(new Person { CreditCard = "1234-5678-9012-3452" });
        result.IsValid.Should().BeTrue();

        // Good checksum, with spaces
        result = await _validator.ValidateAsync(new Person { CreditCard = "1234 5678 9012 3452" });
        result.IsValid.Should().BeTrue();

        // Bad checksum
        result = await _validator.ValidateAsync(new Person { CreditCard = "0000000000000001" });
        result.IsValid.Should().BeFalse();

        result = await _validator.ValidateAsync(new Person { CreditCard = "0000000000000000" });
        result.IsValid.Should().BeTrue();
		logger.LogDebug("<<< Exiting IsValidTests()");
    }
}