using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators;

[TestFixture]
public class PasswordValidatorTests : BaseNopTest
{
    private Person _person;
    private ChangePasswordValidator _changePasswordValidator;
    private PasswordRecoveryConfirmValidator _passwordRecoveryConfirmValidator;
    private RegisterValidator _registerValidator;
    private ILocalizationService _localizationService;
    private IStateProvinceService _stateProvinceService;
    private CustomerSettings _customerSettings;
    private TaxSettings _taxSettings;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _customerSettings = new CustomerSettings
        {
			logger.LogDebug("Setup(): _customerSettings → {_customerSettings}", _customerSettings);
            PasswordMinLength = 8,
            PasswordMaxLength = 20,
            PasswordRequireUppercase = true,
            PasswordRequireLowercase = true,
            PasswordRequireDigit = true,
            PasswordRequireNonAlphanumeric = true
        };

        _localizationService = GetService<ILocalizationService>();
		logger.LogDebug("Setup(): _localizationService → {_localizationService}", _localizationService);
        _stateProvinceService = GetService<IStateProvinceService>();
        _taxSettings = GetService<TaxSettings>();
		logger.LogDebug("Setup(): _stateProvinceService → {_stateProvinceService}", _stateProvinceService);
		logger.LogDebug("Setup(): _taxSettings → {_taxSettings}", _taxSettings);
        _changePasswordValidator = new ChangePasswordValidator(_localizationService, _customerSettings);
        _registerValidator = new RegisterValidator(_localizationService, _stateProvinceService, _customerSettings, _taxSettings);
		logger.LogDebug("Setup(): _changePasswordValidator → {_changePasswordValidator}", _changePasswordValidator);
		logger.LogDebug("Setup(): _registerValidator → {_registerValidator}", _registerValidator);
        _passwordRecoveryConfirmValidator = new PasswordRecoveryConfirmValidator(_localizationService, _customerSettings);

		logger.LogDebug("Setup(): _passwordRecoveryConfirmValidator → {_passwordRecoveryConfirmValidator}", _passwordRecoveryConfirmValidator);
        _person = new Person();
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _person → {_person}", _person);
    }

    [Test]
    public void IsValidTestsLowercase()
    {
        var validator = new TestValidator();

		logger.LogDebug("IsValidTestsLowercase(): validator → {Validator}", validator);
        var cs = new CustomerSettings
        {
            PasswordMinLength = 3,
            PasswordMaxLength = 32,
            PasswordRequireLowercase = true
        };

        validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

        //ShouldHaveValidationError
        _person.Password = "NOP123";
		logger.LogDebug("IsValidTestsLowercase(): _person.Password → {Arg0}", _person.Password);
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

        //ShouldNotHaveValidationError
        _person.Password = "nop123";
        validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
		logger.LogDebug("<<< Exiting IsValidTestsLowercase()");
    }

    [Test]
    public void IsValidTestsUppercase()
    {
		logger.LogDebug(">>> Entering IsValidTestsUppercase()");
        var validator = new TestValidator();

		logger.LogDebug("IsValidTestsUppercase(): validator → {Validator}", validator);
        var cs = new CustomerSettings
        {
            PasswordMinLength = 3,
            PasswordMaxLength = 32,
            PasswordRequireUppercase = true
        };

        validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

        //ShouldHaveValidationError
        _person.Password = "nop";
		logger.LogDebug("IsValidTestsUppercase(): _person.Password → {Arg0}", _person.Password);
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

        //ShouldNotHaveValidationError
        _person.Password = "Nop";
        validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
		logger.LogDebug("<<< Exiting IsValidTestsUppercase()");
    }

    [Test]
    public void IsValidTestsDigit()
    {
		logger.LogDebug(">>> Entering IsValidTestsDigit()");
        var validator = new TestValidator();

		logger.LogDebug("IsValidTestsDigit(): validator → {Validator}", validator);
        var cs = new CustomerSettings
        {
            PasswordMinLength = 3,
            PasswordMaxLength = 32,
            PasswordRequireDigit = true
        };

        validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

        //ShouldHaveValidationError
        _person.Password = "nop";
		logger.LogDebug("IsValidTestsDigit(): _person.Password → {Arg0}", _person.Password);
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

        //ShouldNotHaveValidationError
        _person.Password = "Nop1";
        validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
		logger.LogDebug("<<< Exiting IsValidTestsDigit()");
    }

    [Test]
    public void IsValidTestsNonAlphanumeric()
    {
		logger.LogDebug(">>> Entering IsValidTestsNonAlphanumeric()");
        var validator = new TestValidator();

		logger.LogDebug("IsValidTestsNonAlphanumeric(): validator → {Validator}", validator);
        var cs = new CustomerSettings
        {
            PasswordMinLength = 3,
            PasswordMaxLength = 32,
            PasswordRequireNonAlphanumeric = true
        };

        validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

        //ShouldHaveValidationError
        _person.Password = "nop";
		logger.LogDebug("IsValidTestsNonAlphanumeric(): _person.Password → {Arg0}", _person.Password);
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

        //ShouldNotHaveValidationError
        _person.Password = "Nop#";
        validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
		logger.LogDebug("<<< Exiting IsValidTestsNonAlphanumeric()");
    }

    [Test]
    public void IsValidTestsAllRules()
    {
		logger.LogDebug(">>> Entering IsValidTestsAllRules()");
        var validator = new TestValidator();

		logger.LogDebug("IsValidTestsAllRules(): validator → {Validator}", validator);
        //Example:  (?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$
        validator.RuleFor(x => x.Password).IsPassword(_localizationService, _customerSettings);

        //ShouldHaveValidationError
        _person.Password = string.Empty;
		logger.LogDebug("IsValidTestsAllRules(): _person.Password → {Arg0}", _person.Password);
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "123";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "12345678";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "nopcommerce";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "nopCommerce";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "nopcommerce123";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "nopCommerce123";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "nopcommerce123$";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "NOPCOMMERCE123$";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "nopCommerce123~";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);
        _person.Password = "nopCommerce123$nopCommerce123$";
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

        //ShouldNotHaveValidationError
        _person.Password = "nopCommerce123$";
        validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
		logger.LogDebug("<<< Exiting IsValidTestsAllRules()");
    }

    [Test]
    public void ShouldValidateOnChangePasswordModelIsAllRule()
    {
		logger.LogDebug(">>> Entering ShouldValidateOnChangePasswordModelIsAllRule()");
        var model = new ChangePasswordModel
        {
            NewPassword = "1234"
        };
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
		logger.LogDebug("ShouldValidateOnChangePasswordModelIsAllRule(): model.ConfirmNewPassword → {Arg0}", model.ConfirmNewPassword);
        _changePasswordValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.NewPassword);
        model.NewPassword = "nopCommerce123$";
		logger.LogDebug("ShouldValidateOnChangePasswordModelIsAllRule(): model.NewPassword → {Arg0}", model.NewPassword);
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
        _changePasswordValidator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.NewPassword);
		logger.LogDebug("<<< Exiting ShouldValidateOnChangePasswordModelIsAllRule()");
    }

    [Test]
    public void ShouldValidateOnPasswordRecoveryConfirmModelIsAllRule()
    {
		logger.LogDebug(">>> Entering ShouldValidateOnPasswordRecoveryConfirmModelIsAllRule()");
        var model = new PasswordRecoveryConfirmModel
        {
            NewPassword = "1234"
        };
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
		logger.LogDebug("ShouldValidateOnPasswordRecoveryConfirmModelIsAllRule(): model.ConfirmNewPassword → {Arg0}", model.ConfirmNewPassword);
        _passwordRecoveryConfirmValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.NewPassword);
        model.NewPassword = "nopCommerce123$";
		logger.LogDebug("ShouldValidateOnPasswordRecoveryConfirmModelIsAllRule(): model.NewPassword → {Arg0}", model.NewPassword);
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
        _passwordRecoveryConfirmValidator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.NewPassword);
		logger.LogDebug("<<< Exiting ShouldValidateOnPasswordRecoveryConfirmModelIsAllRule()");
    }

    [Test]
    public void ShouldValidateOnRegisterModelIsAllRule()
    {
		logger.LogDebug(">>> Entering ShouldValidateOnRegisterModelIsAllRule()");
        var model = new RegisterModel
        {
            Password = "1234"
        };
        //we know that password should equal confirmation password
        model.ConfirmPassword = model.Password;
		logger.LogDebug("ShouldValidateOnRegisterModelIsAllRule(): model.ConfirmPassword → {Arg0}", model.ConfirmPassword);
        _registerValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Password);
        model.Password = "nopCommerce123$";
		logger.LogDebug("ShouldValidateOnRegisterModelIsAllRule(): model.Password → {Arg0}", model.Password);
        //we know that password should equal confirmation password
        model.ConfirmPassword = model.Password;
        _registerValidator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Password);
		logger.LogDebug("<<< Exiting ShouldValidateOnRegisterModelIsAllRule()");
    }
}
