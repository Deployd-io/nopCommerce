using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Customer;

[TestFixture]
public class PasswordRecoveryConfirmValidatorTests : BaseNopTest
{
    private PasswordRecoveryConfirmValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new PasswordRecoveryConfirmValidator(GetService<ILocalizationService>(), GetService<CustomerSettings>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenNewPasswordIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenNewPasswordIsNullOrEmpty()");
        var model = new PasswordRecoveryConfirmModel
        {
            NewPassword = null
        };
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
		logger.LogDebug("ShouldHaveErrorWhenNewPasswordIsNullOrEmpty(): model.ConfirmNewPassword → {Arg0}", model.ConfirmNewPassword);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.NewPassword);
        model.NewPassword = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenNewPasswordIsNullOrEmpty(): model.NewPassword → {Arg0}", model.NewPassword);
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.NewPassword);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNewPasswordIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNewPasswordIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNewPasswordIsSpecified()");
        var model = new PasswordRecoveryConfirmModel
        {
            NewPassword = "new password"
        };
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
		logger.LogDebug("ShouldNotHaveErrorWhenNewPasswordIsSpecified(): model.ConfirmNewPassword → {Arg0}", model.ConfirmNewPassword);
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.NewPassword);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNewPasswordIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenConfirmNewPasswordIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenConfirmNewPasswordIsNullOrEmpty()");
        var model = new PasswordRecoveryConfirmModel
        {
            ConfirmNewPassword = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword);
        model.ConfirmNewPassword = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenConfirmNewPasswordIsNullOrEmpty(): model.ConfirmNewPassword → {Arg0}", model.ConfirmNewPassword);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenConfirmNewPasswordIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenConfirmNewPasswordIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenConfirmNewPasswordIsSpecified()");
        var model = new PasswordRecoveryConfirmModel
        {
            ConfirmNewPassword = "some password"
        };
        //we know that new password should equal confirmation password
        model.NewPassword = model.ConfirmNewPassword;
		logger.LogDebug("ShouldNotHaveErrorWhenConfirmNewPasswordIsSpecified(): model.NewPassword → {Arg0}", model.NewPassword);
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ConfirmNewPassword);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenConfirmNewPasswordIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenNewPasswordDoesNotEqualConfirmationPassword()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenNewPasswordDoesNotEqualConfirmationPassword()");
        var model = new PasswordRecoveryConfirmModel
        {
            NewPassword = "some password",
            ConfirmNewPassword = "another password"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNewPasswordDoesNotEqualConfirmationPassword()");
    }

    [Test]
    public void Should_not_have_error_when_newPassword_equals_confirmationPassword()
    {
		logger.LogDebug(">>> Entering Should_not_have_error_when_newPassword_equals_confirmationPassword()");
        var model = new PasswordRecoveryConfirmModel
        {
            NewPassword = "some password",
            ConfirmNewPassword = "some password"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.NewPassword);
		logger.LogDebug("<<< Exiting Should_not_have_error_when_newPassword_equals_confirmationPassword()");
    }
}
