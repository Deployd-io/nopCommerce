using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators;

[TestFixture]
public class PasswordValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogDebug(">>> Entering Setup()");
        _customerSettings = new CustomerSettings
        {
			_logger.LogDebug("Setup(): _customerSettings → {_customerSettings}", _customerSettings);
            PasswordMinLength = 8,
            PasswordMaxLength = 20,
            PasswordRequireUppercase = true,
            PasswordRequireLowercase = true,
            PasswordRequireDigit = true,
            PasswordRequireNonAlphanumeric = true
        };

        _localizationService = GetService<ILocalizationService>();
		_logger.LogDebug("Setup(): _localizationService → {_localizationService}", _localizationService);
        _stateProvinceService = GetService<IStateProvinceService>();
		_logger.LogDebug("Setup(): _stateProvinceService → {_stateProvinceService}", _stateProvinceService);
        _taxSettings = GetService<TaxSettings>();
		_logger.LogDebug("Setup(): _taxSettings → {_taxSettings}", _taxSettings);
        _changePasswordValidator = new ChangePasswordValidator(_localizationService, _customerSettings);
		_logger.LogDebug("Setup(): _changePasswordValidator → {_changePasswordValidator}", _changePasswordValidator);
        _registerValidator = new RegisterValidator(_localizationService, _stateProvinceService, _customerSettings, _taxSettings);
		_logger.LogDebug("Setup(): _registerValidator → {_registerValidator}", _registerValidator);
        _passwordRecoveryConfirmValidator = new PasswordRecoveryConfirmValidator(_localizationService, _customerSettings);
		_logger.LogDebug("Setup(): _passwordRecoveryConfirmValidator → {_passwordRecoveryConfirmValidator}", _passwordRecoveryConfirmValidator);

        _person = new Person();
		_logger.LogDebug("<<< Exiting Setup()");
		_logger.LogDebug("Setup(): _person → {_person}", _person);
    }

    [Test]
    public void IsValidTestsLowercase()
    {
		_logger.LogDebug(">>> Entering IsValidTestsLowercase()");
        var validator = new TestValidator();

		_logger.LogDebug("IsValidTestsLowercase(): validator → {Validator}", validator);
        var cs = new CustomerSettings
        {
            PasswordMinLength = 3,
            PasswordMaxLength = 32,
            PasswordRequireLowercase = true
        };

        validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

        //ShouldHaveValidationError
        _person.Password = "NOP123";
		_logger.LogDebug("IsValidTestsLowercase(): _person.Password → {Arg0}", _person.Password);
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

        //ShouldNotHaveValidationError
        _person.Password = "nop123";
        validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
		_logger.LogDebug("<<< Exiting IsValidTestsLowercase()");
    }

    [Test]
    public void IsValidTestsUppercase()
    {
		_logger.LogDebug(">>> Entering IsValidTestsUppercase()");
        var validator = new TestValidator();

		_logger.LogDebug("IsValidTestsUppercase(): validator → {Validator}", validator);
        var cs = new CustomerSettings
        {
            PasswordMinLength = 3,
            PasswordMaxLength = 32,
            PasswordRequireUppercase = true
        };

        validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

        //ShouldHaveValidationError
        _person.Password = "nop";
		_logger.LogDebug("IsValidTestsUppercase(): _person.Password → {Arg0}", _person.Password);
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

        //ShouldNotHaveValidationError
        _person.Password = "Nop";
        validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
		_logger.LogDebug("<<< Exiting IsValidTestsUppercase()");
    }

    [Test]
    public void IsValidTestsDigit()
    {
		_logger.LogDebug(">>> Entering IsValidTestsDigit()");
        var validator = new TestValidator();

		_logger.LogDebug("IsValidTestsDigit(): validator → {Validator}", validator);
        var cs = new CustomerSettings
        {
            PasswordMinLength = 3,
            PasswordMaxLength = 32,
            PasswordRequireDigit = true
        };

        validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

        //ShouldHaveValidationError
        _person.Password = "nop";
		_logger.LogDebug("IsValidTestsDigit(): _person.Password → {Arg0}", _person.Password);
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

        //ShouldNotHaveValidationError
        _person.Password = "Nop1";
        validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
		_logger.LogDebug("<<< Exiting IsValidTestsDigit()");
    }

    [Test]
    public void IsValidTestsNonAlphanumeric()
    {
		_logger.LogDebug(">>> Entering IsValidTestsNonAlphanumeric()");
        var validator = new TestValidator();

		_logger.LogDebug("IsValidTestsNonAlphanumeric(): validator → {Validator}", validator);
        var cs = new CustomerSettings
        {
            PasswordMinLength = 3,
            PasswordMaxLength = 32,
            PasswordRequireNonAlphanumeric = true
        };

        validator.RuleFor(x => x.Password).IsPassword(_localizationService, cs);

        //ShouldHaveValidationError
        _person.Password = "nop";
		_logger.LogDebug("IsValidTestsNonAlphanumeric(): _person.Password → {Arg0}", _person.Password);
        validator.TestValidate(_person).ShouldHaveValidationErrorFor(x => x.Password);

        //ShouldNotHaveValidationError
        _person.Password = "Nop#";
        validator.TestValidate(_person).ShouldNotHaveValidationErrorFor(x => x.Password);
		_logger.LogDebug("<<< Exiting IsValidTestsNonAlphanumeric()");
    }

    [Test]
    public void IsValidTestsAllRules()
    {
		_logger.LogDebug(">>> Entering IsValidTestsAllRules()");
        var validator = new TestValidator();

		_logger.LogDebug("IsValidTestsAllRules(): validator → {Validator}", validator);
        //Example:  (?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$
        validator.RuleFor(x => x.Password).IsPassword(_localizationService, _customerSettings);

        //ShouldHaveValidationError
        _person.Password = string.Empty;
		_logger.LogDebug("IsValidTestsAllRules(): _person.Password → {Arg0}", _person.Password);
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
		_logger.LogDebug("<<< Exiting IsValidTestsAllRules()");
    }

    [Test]
    public void ShouldValidateOnChangePasswordModelIsAllRule()
    {
		_logger.LogDebug(">>> Entering ShouldValidateOnChangePasswordModelIsAllRule()");
        var model = new ChangePasswordModel
        {
            NewPassword = "1234"
        };
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
		_logger.LogDebug("ShouldValidateOnChangePasswordModelIsAllRule(): model.ConfirmNewPassword → {Arg0}", model.ConfirmNewPassword);
        _changePasswordValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.NewPassword);
        model.NewPassword = "nopCommerce123$";
		_logger.LogDebug("ShouldValidateOnChangePasswordModelIsAllRule(): model.NewPassword → {Arg0}", model.NewPassword);
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
        _changePasswordValidator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.NewPassword);
		_logger.LogDebug("<<< Exiting ShouldValidateOnChangePasswordModelIsAllRule()");
    }

    [Test]
    public void ShouldValidateOnPasswordRecoveryConfirmModelIsAllRule()
    {
		_logger.LogDebug(">>> Entering ShouldValidateOnPasswordRecoveryConfirmModelIsAllRule()");
        var model = new PasswordRecoveryConfirmModel
        {
            NewPassword = "1234"
        };
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
		_logger.LogDebug("ShouldValidateOnPasswordRecoveryConfirmModelIsAllRule(): model.ConfirmNewPassword → {Arg0}", model.ConfirmNewPassword);
        _passwordRecoveryConfirmValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.NewPassword);
        model.NewPassword = "nopCommerce123$";
		_logger.LogDebug("ShouldValidateOnPasswordRecoveryConfirmModelIsAllRule(): model.NewPassword → {Arg0}", model.NewPassword);
        //we know that new password should equal confirmation password
        model.ConfirmNewPassword = model.NewPassword;
        _passwordRecoveryConfirmValidator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.NewPassword);
		_logger.LogDebug("<<< Exiting ShouldValidateOnPasswordRecoveryConfirmModelIsAllRule()");
    }

    [Test]
    public void ShouldValidateOnRegisterModelIsAllRule()
    {
		_logger.LogDebug(">>> Entering ShouldValidateOnRegisterModelIsAllRule()");
        var model = new RegisterModel
        {
            Password = "1234"
        };
        //we know that password should equal confirmation password
        model.ConfirmPassword = model.Password;
		_logger.LogDebug("ShouldValidateOnRegisterModelIsAllRule(): model.ConfirmPassword → {Arg0}", model.ConfirmPassword);
        _registerValidator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Password);
        model.Password = "nopCommerce123$";
		_logger.LogDebug("ShouldValidateOnRegisterModelIsAllRule(): model.Password → {Arg0}", model.Password);
        //we know that password should equal confirmation password
        model.ConfirmPassword = model.Password;
        _registerValidator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Password);
		_logger.LogDebug("<<< Exiting ShouldValidateOnRegisterModelIsAllRule()");
    }
}