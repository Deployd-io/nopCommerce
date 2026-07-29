using FluentAssertions;
using Nop.Core.Domain.Customers;
using Nop.Web.Framework.Validators;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators;

[TestFixture]
public class UsernameValidatorTests
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
            UsernameValidationRule = "^a.*1$",
            UsernameValidationEnabled = true,
            UsernameValidationUseRegex = false
        };

        _validator = new TestValidator { v => v.RuleFor(x => x.Username).IsUsername(_customerSettings) };
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public async Task IsValidTests()
    {
		logger.LogDebug(">>> Entering IsValidTests()");
        //optional value is not valid
        var result = await _validator.ValidateAsync(new Person { Username = null });
        result.IsValid.Should().BeFalse();
		logger.LogDebug("IsValidTests(): result → {Result}", result);
        result = await _validator.ValidateAsync(new Person { Username = string.Empty });
        result.IsValid.Should().BeFalse();

        //validation without regex
        result = await _validator.ValidateAsync(new Person { Username = "test_user" });
        result.IsValid.Should().BeFalse();
        result = await _validator.ValidateAsync(new Person { Username = "a*1^" });
        result.IsValid.Should().BeTrue();

        //validation with regex
        _customerSettings.UsernameValidationUseRegex = true;
		logger.LogDebug("IsValidTests(): _customerSettings.UsernameValidationUseRegex → {Arg0}", _customerSettings.UsernameValidationUseRegex);
        result = await _validator.ValidateAsync(new Person { Username = "test_user" });
        result.IsValid.Should().BeFalse();
        result = await _validator.ValidateAsync(new Person { Username = "a*1^" });
        result.IsValid.Should().BeFalse();
        result = await _validator.ValidateAsync(new Person { Username = "a1" });
        result.IsValid.Should().BeTrue();
        result = await _validator.ValidateAsync(new Person { Username = "a_test_user_name_1" });
        result.IsValid.Should().BeTrue();
        result = await _validator.ValidateAsync(new Person { Username = "b_test_user_name_1" });
        result.IsValid.Should().BeFalse();
		logger.LogDebug("<<< Exiting IsValidTests()");
    }
}