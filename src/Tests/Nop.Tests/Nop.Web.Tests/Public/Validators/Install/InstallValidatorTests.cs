using FluentValidation.TestHelper;
using Nop.Web.Infrastructure.Installation;
using Nop.Web.Models.Install;
using Nop.Web.Validators.Install;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Install;

[TestFixture]
public class InstallValidatorTests : BaseNopTest
{
    private InstallValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new InstallValidator(GetService<IInstallationLocalizationService>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenAdminEmailIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenAdminEmailIsNullOrEmpty()");
        var model = new InstallModel
        {
            AdminEmail = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminEmail);
        model.AdminEmail = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenAdminEmailIsNullOrEmpty(): model.AdminEmail → {Arg0}", model.AdminEmail);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminEmail);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenAdminEmailIsNullOrEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenAdminEmailIsWrongFormat()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenAdminEmailIsWrongFormat()");
        var model = new InstallModel
        {
            AdminEmail = "adminexample.com"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminEmail);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenAdminEmailIsWrongFormat()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenAdminEmailIsCorrectFormat()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenAdminEmailIsCorrectFormat()");
        var model = new InstallModel
        {
            AdminEmail = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AdminEmail);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenAdminEmailIsCorrectFormat()");
    }

    [Test]
    public void ShouldHaveErrorWhenPasswordIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenPasswordIsNullOrEmpty()");
        var model = new InstallModel
        {
            AdminPassword = null
        };
        //we know that password should equal confirmation password
        model.ConfirmPassword = model.AdminPassword;
		logger.LogDebug("ShouldHaveErrorWhenPasswordIsNullOrEmpty(): model.ConfirmPassword → {Arg0}", model.ConfirmPassword);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminPassword);
        model.AdminPassword = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenPasswordIsNullOrEmpty(): model.AdminPassword → {Arg0}", model.AdminPassword);
        //we know that password should equal confirmation password
        model.ConfirmPassword = model.AdminPassword;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminPassword);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenPasswordIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenPasswordIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenPasswordIsSpecified()");
        var model = new InstallModel
        {
            AdminPassword = "password"
        };
        //we know that password should equal confirmation password
        model.ConfirmPassword = model.AdminPassword;
		logger.LogDebug("ShouldNotHaveErrorWhenPasswordIsSpecified(): model.ConfirmPassword → {Arg0}", model.ConfirmPassword);
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AdminPassword);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenPasswordIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenConfirmPasswordIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenConfirmPasswordIsNullOrEmpty()");
        var model = new InstallModel
        {
            ConfirmPassword = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
        model.ConfirmPassword = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenConfirmPasswordIsNullOrEmpty(): model.ConfirmPassword → {Arg0}", model.ConfirmPassword);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenConfirmPasswordIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenConfirmPasswordIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenConfirmPasswordIsSpecified()");
        var model = new InstallModel
        {
            ConfirmPassword = "some password"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ConfirmPassword);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenConfirmPasswordIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenPasswordDoesNotEqualConfirmationPassword()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenPasswordDoesNotEqualConfirmationPassword()");
        var model = new InstallModel
        {
            AdminPassword = "some password",
            ConfirmPassword = "another password"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AdminPassword);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenPasswordDoesNotEqualConfirmationPassword()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenPasswordEqualsConfirmationPassword()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenPasswordEqualsConfirmationPassword()");
        var model = new InstallModel
        {
            AdminPassword = "some password",
            ConfirmPassword = "some password"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AdminPassword);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenPasswordEqualsConfirmationPassword()");
    }
}
