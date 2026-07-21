using FluentAssertions;
using Nop.Core.Domain.Customers;
using Nop.Web.Framework.Validators;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators;

[TestFixture]
public class PhoneNumberValidatorTests
{
    private TestValidator _validator;
    private CustomerSettings _customerSettings;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _customerSettings = new CustomerSettings
        {
			logger.LogDebug("Setup(): _customerSettings → {_customerSettings}", _customerSettings);
            PhoneNumberValidationRule = "^[0-9]{1,14}?$",
            PhoneNumberValidationEnabled = true,
            PhoneNumberValidationUseRegex = false
        };

        _validator = new TestValidator { v => v.RuleFor(x => x.PhoneNumber).IsPhoneNumber(_customerSettings) };
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public async Task IsValidTests()
    {
		logger.LogDebug(">>> Entering IsValidTests()");
        //optional value is not valid
        _customerSettings.PhoneRequired = true;
		logger.LogDebug("IsValidTests(): _customerSettings.PhoneRequired → {Arg0}", _customerSettings.PhoneRequired);
        var result = await _validator.ValidateAsync(new Person { PhoneNumber = null });
        result.IsValid.Should().BeFalse();
		logger.LogDebug("IsValidTests(): result → {Result}", result);
        result = await _validator.ValidateAsync(new Person { PhoneNumber = string.Empty });
        result.IsValid.Should().BeFalse();

        //validation without regex
        result = await _validator.ValidateAsync(new Person { PhoneNumber = "test_phone_number" });
        result.IsValid.Should().BeFalse();
        result = await _validator.ValidateAsync(new Person { PhoneNumber = string.Empty });
        result.IsValid.Should().BeFalse();
        result = await _validator.ValidateAsync(new Person { PhoneNumber = "123" });
        result.IsValid.Should().BeFalse();
        result = await _validator.ValidateAsync(new Person { PhoneNumber = "[0-9]{1,14}^" });
        result.IsValid.Should().BeTrue();

        //validation with regex
        _customerSettings.PhoneNumberValidationUseRegex = true;
		logger.LogDebug("IsValidTests(): _customerSettings.PhoneNumberValidationUseRegex → {Arg0}", _customerSettings.PhoneNumberValidationUseRegex);
        result = await _validator.ValidateAsync(new Person { PhoneNumber = "test_phone_number" });
        result.IsValid.Should().BeFalse();
        result = await _validator.ValidateAsync(new Person { PhoneNumber = "123456789" });
        result.IsValid.Should().BeTrue();
        _customerSettings.PhoneRequired = false;
        result = await _validator.ValidateAsync(new Person { PhoneNumber = string.Empty });
        result.IsValid.Should().BeTrue();
        result = await _validator.ValidateAsync(new Person { PhoneNumber = "+123456789" });
        result.IsValid.Should().BeFalse();
		logger.LogDebug("<<< Exiting IsValidTests()");
    }
}
