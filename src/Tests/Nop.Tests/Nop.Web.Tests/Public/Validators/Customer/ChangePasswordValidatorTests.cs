using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Customer;

[TestFixture]
public class ChangePasswordValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ChangePasswordValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new ChangePasswordValidator(GetService<ILocalizationService>(), GetService<CustomerSettings>());
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
		_logger.LogDebug("<<< Exiting Setup()");
    }

    [Test]
    public void ShouldHaveErrorWhenOldPasswordIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenOldPasswordIsNullOrEmpty()");
        var model = new ChangePasswordModel
        {
            OldPassword = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.OldPassword);
        model.OldPassword = string.Empty;
		_logger.LogDebug("ShouldHaveErrorWhenOldPasswordIsNullOrEmpty(): model.OldPassword → {Arg0}", model.OldPassword);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.OldPassword);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenOldPasswordIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenOldPasswordIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenOldPasswordIsSpecified()");
        var model = new ChangePasswordModel
        {
            OldPassword = "old password"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.OldPassword);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenOldPasswordIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenNewPasswordIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenNewPasswordIsNullOrEmpty()");
        var model = new ChangePasswordModel
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
        var model = new ChangePasswordModel
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
        var model = new ChangePasswordModel
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
        var model = new ChangePasswordModel
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
        var model = new ChangePasswordModel
        {
            NewPassword = "some password",
            ConfirmNewPassword = "another password"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNewPasswordDoesNotEqualConfirmationPassword()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNewPasswordEqualsConfirmationPassword()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNewPasswordEqualsConfirmationPassword()");
        var model = new ChangePasswordModel
        {
            NewPassword = "some password",
            ConfirmNewPassword = "some password"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.NewPassword);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNewPasswordEqualsConfirmationPassword()");
    }
}