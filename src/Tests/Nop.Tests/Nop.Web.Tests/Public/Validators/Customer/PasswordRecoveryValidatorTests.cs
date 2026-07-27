using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Customer;

[TestFixture]
public class PasswordRecoveryValidatorTests : BaseNopTest
{
    private PasswordRecoveryValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new PasswordRecoveryValidator(GetService<ILocalizationService>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenEmailIsNullOrEmpty()");
        var model = new PasswordRecoveryModel
        {
            Email = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        model.Email = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenEmailIsNullOrEmpty(): model.Email → {Arg0}", model.Email);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenEmailIsNullOrEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsWrongFormat()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenEmailIsWrongFormat()");
        var model = new PasswordRecoveryModel
        {
            Email = "adminexample.com"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenEmailIsWrongFormat()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenEmailIsCorrectFormat()");
        var model = new PasswordRecoveryModel
        {
            Email = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenEmailIsCorrectFormat()");
    }
}