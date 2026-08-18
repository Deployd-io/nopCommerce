using System.Globalization;
using FluentAssertions;
using Nop.Web.Framework.Validators;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators;

[TestFixture]
public class CreditCardValidatorTests
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private TestValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

        _validator = new TestValidator { v => v.RuleFor(x => x.CreditCard).IsCreditCard() };
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
		_logger.LogDebug("<<< Exiting Setup()");
    }

    [Test]
    public async Task IsValidTests()
    {
		_logger.LogDebug(">>> Entering IsValidTests()");
        // Optional value is not valid
        var result = await _validator.ValidateAsync(new Person { CreditCard = null });
		_logger.LogDebug("IsValidTests(): result → {Result}", result);
        result.IsValid.Should().BeFalse();

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
		_logger.LogDebug("<<< Exiting IsValidTests()");
    }
}