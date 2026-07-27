using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Customer;

[TestFixture]
public class RegisterValidatorTests : BaseNopTest
{
    private RegisterValidator _validator;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _validator = new RegisterValidator(GetService<ILocalizationService>(), GetService<IStateProvinceService>(), GetService<CustomerSettings>(), GetService<TaxSettings>());
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenEmailIsNullOrEmpty()");
        var model = new RegisterModel
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
        var model = new RegisterModel
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
        var model = new RegisterModel
        {
            Email = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenEmailIsCorrectFormat()");
    }

    [Test]
    public void ShouldHaveErrorWhenFirstnameIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenFirstnameIsNullOrEmpty()");
        var customerSettings = new CustomerSettings
        {
            FirstNameEnabled = true,
            FirstNameRequired = true
        };

        var validator = new RegisterValidator(GetService<ILocalizationService>(), GetService<IStateProvinceService>(), customerSettings, GetService<TaxSettings>());
        var model = new RegisterModel
		logger.LogDebug("ShouldHaveErrorWhenFirstnameIsNullOrEmpty(): validator → {Validator}", validator);
        {
            FirstName = null
        };
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);
        model.FirstName = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenFirstnameIsNullOrEmpty(): model.FirstName → {Arg0}", model.FirstName);
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenFirstnameIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenFirstnameIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenFirstnameIsSpecified()");
        var customerSettings = new CustomerSettings
        {
            FirstNameEnabled = true
        };

        var validator = new RegisterValidator(GetService<ILocalizationService>(), GetService<IStateProvinceService>(), customerSettings, GetService<TaxSettings>());

		logger.LogDebug("ShouldNotHaveErrorWhenFirstnameIsSpecified(): validator → {Validator}", validator);
        var model = new RegisterModel
        {
            FirstName = "John"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FirstName);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenFirstnameIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenLastNameIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenLastNameIsNullOrEmpty()");
        var customerSettings = new CustomerSettings
        {
            LastNameEnabled = true,
            LastNameRequired = true
        };

        var validator = new RegisterValidator(GetService<ILocalizationService>(), GetService<IStateProvinceService>(), customerSettings, GetService<TaxSettings>());

		logger.LogDebug("ShouldHaveErrorWhenLastNameIsNullOrEmpty(): validator → {Validator}", validator);
        var model = new RegisterModel
        {
            LastName = null
        };

        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);
        model.LastName = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenLastNameIsNullOrEmpty(): model.LastName → {Arg0}", model.LastName);
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenLastNameIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenLastNameIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenLastNameIsSpecified()");
        var customerSettings = new CustomerSettings
        {
            LastNameEnabled = true
        };

        var validator = new RegisterValidator(GetService<ILocalizationService>(), GetService<IStateProvinceService>(), customerSettings, GetService<TaxSettings>());

		logger.LogDebug("ShouldNotHaveErrorWhenLastNameIsSpecified(): validator → {Validator}", validator);
        var model = new RegisterModel
        {
            LastName = "Smith"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.LastName);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenLastNameIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenPasswordIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenPasswordIsNullOrEmpty()");
        var model = new RegisterModel
        {
            Password = null
        };
        //we know that password should equal confirmation password
        model.ConfirmPassword = model.Password;
		logger.LogDebug("ShouldHaveErrorWhenPasswordIsNullOrEmpty(): model.ConfirmPassword → {Arg0}", model.ConfirmPassword);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Password);
        model.Password = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenPasswordIsNullOrEmpty(): model.Password → {Arg0}", model.Password);
        //we know that password should equal confirmation password
        model.ConfirmPassword = model.Password;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Password);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenPasswordIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenPasswordIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenPasswordIsSpecified()");
        var model = new RegisterModel
        {
            Password = "password"
        };
        //we know that password should equal confirmation password
        model.ConfirmPassword = model.Password;
		logger.LogDebug("ShouldNotHaveErrorWhenPasswordIsSpecified(): model.ConfirmPassword → {Arg0}", model.ConfirmPassword);
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Password);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenPasswordIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenConfirmPasswordIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenConfirmPasswordIsNullOrEmpty()");
        var model = new RegisterModel
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
        var model = new RegisterModel
        {
            ConfirmPassword = "some password"
        };
        //we know that new password should equal confirmation password
        model.Password = model.ConfirmPassword;
		logger.LogDebug("ShouldNotHaveErrorWhenConfirmPasswordIsSpecified(): model.Password → {Arg0}", model.Password);
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ConfirmPassword);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenConfirmPasswordIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenPasswordDoesNotEqualConfirmationPassword()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenPasswordDoesNotEqualConfirmationPassword()");
        var model = new RegisterModel
        {
            Password = "some password",
            ConfirmPassword = "another password"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenPasswordDoesNotEqualConfirmationPassword()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenPasswordEqualsConfirmationPassword()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenPasswordEqualsConfirmationPassword()");
        var model = new RegisterModel
        {
            Password = "some password",
            ConfirmPassword = "some password"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Password);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenPasswordEqualsConfirmationPassword()");
    }
}