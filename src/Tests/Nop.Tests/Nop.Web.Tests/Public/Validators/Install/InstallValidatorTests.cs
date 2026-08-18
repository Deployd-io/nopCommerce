using FluentValidation.TestHelper;
using Nop.Web.Infrastructure.Installation;
using Nop.Web.Models.Install;
using Nop.Web.Validators.Install;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Install;

[TestFixture]
public class InstallValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private InstallValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new InstallValidator(GetService<IInstallationLocalizationService>());
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
		_logger.LogDebug("<<< Exiting Setup()");
    }

    [Test]
    public void ShouldHaveErrorWhenAdminEmailIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenAdminEmailIsNullOrEmpty()");
        var model = new InstallModel
        {
            AdminEmail = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminEmail);
        model.AdminEmail = string.Empty;
		_logger.LogDebug("ShouldHaveErrorWhenAdminEmailIsNullOrEmpty(): model.AdminEmail → {Arg0}", model.AdminEmail);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminEmail);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenAdminEmailIsNullOrEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenAdminEmailIsWrongFormat()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenAdminEmailIsWrongFormat()");
        var model = new InstallModel
        {
            AdminEmail = "adminexample.com"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminEmail);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenAdminEmailIsWrongFormat()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenAdminEmailIsCorrectFormat()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenAdminEmailIsCorrectFormat()");
        var model = new InstallModel
        {
            AdminEmail = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AdminEmail);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenAdminEmailIsCorrectFormat()");
    }

    [Test]
    public void ShouldHaveErrorWhenPasswordIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenPasswordIsNullOrEmpty()");
        var model = new InstallModel
        {
            AdminPassword = null
        };
        //we know that password should equal confirmation password
        model.ConfirmPassword = model.AdminPassword;
		_logger.LogDebug("ShouldHaveErrorWhenPasswordIsNullOrEmpty(): model.ConfirmPassword → {Arg0}", model.ConfirmPassword);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminPassword);
        model.AdminPassword = string.Empty;
		_logger.LogDebug("ShouldHaveErrorWhenPasswordIsNullOrEmpty(): model.AdminPassword → {Arg0}", model.AdminPassword);
        //we know that password should equal confirmation password
        model.ConfirmPassword = model.AdminPassword;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminPassword);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenPasswordIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenPasswordIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenPasswordIsSpecified()");
        var model = new InstallModel
        {
            AdminPassword = "password"
        };
        //we know that password should equal confirmation password
        model.ConfirmPassword = model.AdminPassword;
		_logger.LogDebug("ShouldNotHaveErrorWhenPasswordIsSpecified(): model.ConfirmPassword → {Arg0}", model.ConfirmPassword);
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AdminPassword);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenPasswordIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenConfirmPasswordIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenConfirmPasswordIsNullOrEmpty()");
        var model = new InstallModel
        {
            ConfirmPassword = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
        model.ConfirmPassword = string.Empty;
		_logger.LogDebug("ShouldHaveErrorWhenConfirmPasswordIsNullOrEmpty(): model.ConfirmPassword → {Arg0}", model.ConfirmPassword);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenConfirmPasswordIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenConfirmPasswordIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenConfirmPasswordIsSpecified()");
        var model = new InstallModel
        {
            ConfirmPassword = "some password"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ConfirmPassword);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenConfirmPasswordIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenPasswordDoesNotEqualConfirmationPassword()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenPasswordDoesNotEqualConfirmationPassword()");
        var model = new InstallModel
        {
            AdminPassword = "some password",
            ConfirmPassword = "another password"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminPassword);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenPasswordDoesNotEqualConfirmationPassword()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenPasswordEqualsConfirmationPassword()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenPasswordEqualsConfirmationPassword()");
        var model = new InstallModel
        {
            AdminPassword = "some password",
            ConfirmPassword = "some password"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AdminPassword);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenPasswordEqualsConfirmationPassword()");
    }
}