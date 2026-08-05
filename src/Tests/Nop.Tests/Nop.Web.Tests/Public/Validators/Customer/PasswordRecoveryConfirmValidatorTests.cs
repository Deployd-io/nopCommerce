using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Customer;

[TestFixture]
public class PasswordRecoveryConfirmValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private PasswordRecoveryConfirmValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new PasswordRecoveryConfirmValidator(GetService<ILocalizationService>(), GetService<CustomerSettings>());
		_logger.LogDebug("<<< Exiting Setup()");
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenNewPasswordIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenNewPasswordIsNullOrEmpty()");
        var model = new PasswordRecoveryConfirmModel
        {
            NewPassword = null
        };
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
		_logger.LogDebug("ShouldHaveErrorWhenNewPasswordIsNullOrEmpty(): model.ConfirmNewPassword → {Arg0}", model.ConfirmNewPassword);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.NewPassword);
        model.NewPassword = string.Empty;
		_logger.LogDebug("ShouldHaveErrorWhenNewPasswordIsNullOrEmpty(): model.NewPassword → {Arg0}", model.NewPassword);
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.NewPassword);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNewPasswordIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNewPasswordIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNewPasswordIsSpecified()");
        var model = new PasswordRecoveryConfirmModel
        {
            NewPassword = "new password"
        };
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
		_logger.LogDebug("ShouldNotHaveErrorWhenNewPasswordIsSpecified(): model.ConfirmNewPassword → {Arg0}", model.ConfirmNewPassword);
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.NewPassword);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNewPasswordIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenConfirmNewPasswordIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenConfirmNewPasswordIsNullOrEmpty()");
        var model = new PasswordRecoveryConfirmModel
        {
            ConfirmNewPassword = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword);
        model.ConfirmNewPassword = string.Empty;
		_logger.LogDebug("ShouldHaveErrorWhenConfirmNewPasswordIsNullOrEmpty(): model.ConfirmNewPassword → {Arg0}", model.ConfirmNewPassword);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenConfirmNewPasswordIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenConfirmNewPasswordIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenConfirmNewPasswordIsSpecified()");
        var model = new PasswordRecoveryConfirmModel
        {
            ConfirmNewPassword = "some password"
        };
        //we know that new password should equal confirmation password
        model.NewPassword = model.ConfirmNewPassword;
		_logger.LogDebug("ShouldNotHaveErrorWhenConfirmNewPasswordIsSpecified(): model.NewPassword → {Arg0}", model.NewPassword);
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ConfirmNewPassword);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenConfirmNewPasswordIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenNewPasswordDoesNotEqualConfirmationPassword()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenNewPasswordDoesNotEqualConfirmationPassword()");
        var model = new PasswordRecoveryConfirmModel
        {
            NewPassword = "some password",
            ConfirmNewPassword = "another password"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNewPasswordDoesNotEqualConfirmationPassword()");
    }

    [Test]
    public void Should_not_have_error_when_newPassword_equals_confirmationPassword()
    {
		_logger.LogDebug(">>> Entering Should_not_have_error_when_newPassword_equals_confirmationPassword()");
        var model = new PasswordRecoveryConfirmModel
        {
            NewPassword = "some password",
            ConfirmNewPassword = "some password"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.NewPassword);
		_logger.LogDebug("<<< Exiting Should_not_have_error_when_newPassword_equals_confirmationPassword()");
    }
}