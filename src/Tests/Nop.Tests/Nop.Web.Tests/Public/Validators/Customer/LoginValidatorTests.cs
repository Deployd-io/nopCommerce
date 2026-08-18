using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Customer;

[TestFixture]
public class LoginValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private LoginValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new LoginValidator(GetService<ILocalizationService>(), GetService<CustomerSettings>());
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
		_logger.LogDebug("<<< Exiting Setup()");
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenEmailIsNullOrEmpty()");
        var model = new LoginModel
        {
            Email = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        model.Email = string.Empty;
		_logger.LogDebug("ShouldHaveErrorWhenEmailIsNullOrEmpty(): model.Email → {Arg0}", model.Email);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenEmailIsNullOrEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsWrongFormat()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenEmailIsWrongFormat()");
        var model = new LoginModel
        {
            Email = "adminexample.com"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenEmailIsWrongFormat()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenEmailIsCorrectFormat()");
        var model = new LoginModel
        {
            Email = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenEmailIsCorrectFormat()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenEmailIsNullButUsernamesAreEnabled()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenEmailIsNullButUsernamesAreEnabled()");
        var customerSettings = new CustomerSettings
        {
            UsernamesEnabled = true
        };
        _validator = new LoginValidator(GetService<ILocalizationService>(), customerSettings);

		_logger.LogDebug("ShouldNotHaveErrorWhenEmailIsNullButUsernamesAreEnabled(): _validator → {_validator}", _validator);
        var model = new LoginModel
        {
            Email = null
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenEmailIsNullButUsernamesAreEnabled()");
    }
}