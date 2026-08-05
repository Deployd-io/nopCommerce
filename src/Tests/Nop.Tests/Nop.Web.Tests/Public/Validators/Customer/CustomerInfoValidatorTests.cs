using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Customer;

[TestFixture]
public class CustomerInfoValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ILocalizationService _localizationService;
    private IStateProvinceService _stateProvinceService;
    private TaxSettings _taxSettings;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _localizationService = GetService<ILocalizationService>();
        _stateProvinceService = GetService<IStateProvinceService>();
		_logger.LogDebug("SetUp(): _localizationService → {_localizationService}", _localizationService);
		_logger.LogDebug("SetUp(): _stateProvinceService → {_stateProvinceService}", _stateProvinceService);
        _taxSettings = GetService<TaxSettings>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _taxSettings → {_taxSettings}", _taxSettings);
    }

    private CustomerInfoValidator GetDefaultValidator()
    {
		_logger.LogDebug("<<< Exiting GetDefaultValidator()");
        return new CustomerInfoValidator(_localizationService, _stateProvinceService, GetService<CustomerSettings>(), _taxSettings);
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenEmailIsNullOrEmpty()");
        var validator = GetDefaultValidator();

		_logger.LogDebug("ShouldHaveErrorWhenEmailIsNullOrEmpty(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            Email = null
        };
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        model.Email = string.Empty;
		_logger.LogDebug("ShouldHaveErrorWhenEmailIsNullOrEmpty(): model.Email → {Arg0}", model.Email);
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenEmailIsNullOrEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsWrongFormat()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenEmailIsWrongFormat()");
        var validator = GetDefaultValidator();

		_logger.LogDebug("ShouldHaveErrorWhenEmailIsWrongFormat(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            Email = "adminexample.com"
        };
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenEmailIsWrongFormat()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenEmailIsCorrectFormat()");
        var validator = GetDefaultValidator();

		_logger.LogDebug("ShouldNotHaveErrorWhenEmailIsCorrectFormat(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            Email = "admin@example.com"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenEmailIsCorrectFormat()");
    }

    [Test]
    public void ShouldHaveErrorWhenFirstNameIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenFirstNameIsNullOrEmpty()");
        var customerSettings = new CustomerSettings
        {
            FirstNameEnabled = true,
            FirstNameRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("ShouldHaveErrorWhenFirstNameIsNullOrEmpty(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            FirstName = null
        };
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);
        model.FirstName = string.Empty;
		_logger.LogDebug("ShouldHaveErrorWhenFirstNameIsNullOrEmpty(): model.FirstName → {Arg0}", model.FirstName);
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenFirstNameIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenFirstNameIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenFirstNameIsSpecified()");
        var customerSettings = new CustomerSettings
        {
            FirstNameEnabled = true,
            FirstNameRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("ShouldNotHaveErrorWhenFirstNameIsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            FirstName = "John"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FirstName);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenFirstNameIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenLastNameIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenLastNameIsNullOrEmpty()");
        var model = new CustomerInfoModel();
        //required
		_logger.LogDebug("ShouldHaveErrorWhenLastNameIsNullOrEmpty(): model → {Model}", model);
        var customerSettings = new CustomerSettings
        {
            LastNameEnabled = true,
            LastNameRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("ShouldHaveErrorWhenLastNameIsNullOrEmpty(): validator → {Validator}", validator);
		_logger.LogDebug("ShouldHaveErrorWhenLastNameIsNullOrEmpty(): model.LastName → {Arg0}", model.LastName);
        model.LastName = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);
        model.LastName = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);

        //not required
        customerSettings.LastNameRequired = false;
		_logger.LogDebug("ShouldHaveErrorWhenLastNameIsNullOrEmpty(): customerSettings.LastNameRequired → {Arg0}", customerSettings.LastNameRequired);
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.LastName = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.LastName);
        model.LastName = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.LastName);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenLastNameIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenLastNameIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenLastNameIsSpecified()");
        var customerSettings = new CustomerSettings
        {
            LastNameEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("ShouldNotHaveErrorWhenLastNameIsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            LastName = "Smith"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.LastName);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenLastNameIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting()");
        var model = new CustomerInfoModel();

		_logger.LogDebug("ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var customerSettings = new CustomerSettings
        {
            CompanyEnabled = true,
            CompanyRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting(): validator → {Validator}", validator);
		_logger.LogDebug("ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting(): model.Company → {Arg0}", model.Company);
        model.Company = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Company);
        model.Company = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Company);

        //not required
        customerSettings.CompanyRequired = false;
		_logger.LogDebug("ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting(): customerSettings.CompanyRequired → {Arg0}", customerSettings.CompanyRequired);
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.Company = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Company);
        model.Company = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Company);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenCompanyIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenCompanyIsSpecified()");
        var customerSettings = new CustomerSettings
        {
            CompanyEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("ShouldNotHaveErrorWhenCompanyIsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            Company = "Company"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Company);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenCompanyIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting()");
        var model = new CustomerInfoModel();

		_logger.LogDebug("ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var customerSettings = new CustomerSettings
        {
            StreetAddressEnabled = true,
            StreetAddressRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting(): validator → {Validator}", validator);
		_logger.LogDebug("ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting(): model.StreetAddress → {Arg0}", model.StreetAddress);
        model.StreetAddress = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StreetAddress);
        model.StreetAddress = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StreetAddress);

        //not required
        customerSettings.StreetAddressRequired = false;
		_logger.LogDebug("ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting(): customerSettings.StreetAddressRequired → {Arg0}", customerSettings.StreetAddressRequired);
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.StreetAddress = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress);
        model.StreetAddress = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenStreetAddressIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenStreetAddressIsSpecified()");
        var customerSettings = new CustomerSettings
        {
            StreetAddressEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("ShouldNotHaveErrorWhenStreetAddressIsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            StreetAddress = "Street address"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenStreetAddressIsSpecified()");
    }

    [Test]
    public void PublicVoidShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting()
    {
		_logger.LogDebug(">>> Entering PublicVoidShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting()");
        var model = new CustomerInfoModel();

		_logger.LogDebug("PublicVoidShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var customerSettings = new CustomerSettings
        {
            StreetAddress2Enabled = true,
            StreetAddress2Required = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("PublicVoidShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting(): validator → {Validator}", validator);
		_logger.LogDebug("PublicVoidShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting(): model.StreetAddress2 → {Arg0}", model.StreetAddress2);
        model.StreetAddress2 = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StreetAddress2);
        model.StreetAddress2 = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StreetAddress2);

        //not required
        customerSettings.StreetAddress2Required = false;
		_logger.LogDebug("PublicVoidShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting(): customerSettings.StreetAddress2Required → {Arg0}", customerSettings.StreetAddress2Required);
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.StreetAddress2 = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress2);
        model.StreetAddress2 = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress2);
		_logger.LogDebug("<<< Exiting PublicVoidShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenStreetAddress2IsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenStreetAddress2IsSpecified()");
        var customerSettings = new CustomerSettings
        {
            StreetAddress2Enabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("ShouldNotHaveErrorWhenStreetAddress2IsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            StreetAddress2 = "Street address 2"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress2);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenStreetAddress2IsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting()");
        var model = new CustomerInfoModel();

		_logger.LogDebug("ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var customerSettings = new CustomerSettings
        {
            ZipPostalCodeEnabled = true,
            ZipPostalCodeRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting(): validator → {Validator}", validator);
		_logger.LogDebug("ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting(): model.ZipPostalCode → {Arg0}", model.ZipPostalCode);
        model.ZipPostalCode = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ZipPostalCode);
        model.ZipPostalCode = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ZipPostalCode);

        //not required
        customerSettings.ZipPostalCodeRequired = false;
		_logger.LogDebug("ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting(): customerSettings.ZipPostalCodeRequired → {Arg0}", customerSettings.ZipPostalCodeRequired);
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.ZipPostalCode = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode);
        model.ZipPostalCode = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenZipPostalCodeIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenZipPostalCodeIsSpecified()");
        var customerSettings = new CustomerSettings
        {
            ZipPostalCodeEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("ShouldNotHaveErrorWhenZipPostalCodeIsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            ZipPostalCode = "zip"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenZipPostalCodeIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting()");
        var model = new CustomerInfoModel();

		_logger.LogDebug("ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var customerSettings = new CustomerSettings
        {
            CityEnabled = true,
            CityRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting(): validator → {Validator}", validator);
		_logger.LogDebug("ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting(): model.City → {Arg0}", model.City);
        model.City = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.City);
        model.City = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.City);

        //not required
        customerSettings.CityRequired = false;
		_logger.LogDebug("ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting(): customerSettings.CityRequired → {Arg0}", customerSettings.CityRequired);
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.City = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.City);
        model.City = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.City);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenCityIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenCityIsSpecified()");
        var customerSettings = new CustomerSettings
        {
            CityEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("ShouldNotHaveErrorWhenCityIsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            City = "City"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.City);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenCityIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting()");
        var model = new CustomerInfoModel();

		_logger.LogDebug("ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var customerSettings = new CustomerSettings
        {
            PhoneEnabled = true,
            PhoneRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting(): validator → {Validator}", validator);
		_logger.LogDebug("ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting(): model.Phone → {Arg0}", model.Phone);
        model.Phone = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Phone);
        model.Phone = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Phone);

        //not required
        customerSettings.PhoneRequired = false;
		_logger.LogDebug("ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting(): customerSettings.PhoneRequired → {Arg0}", customerSettings.PhoneRequired);
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.Phone = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Phone);
        model.Phone = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Phone);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenPhoneIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenPhoneIsSpecified()");
        var customerSettings = new CustomerSettings
        {
            PhoneEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("ShouldNotHaveErrorWhenPhoneIsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            Phone = "Phone"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Phone);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenPhoneIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting()");
        var model = new CustomerInfoModel();

		_logger.LogDebug("ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var customerSettings = new CustomerSettings
        {
            FaxEnabled = true,
            FaxRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting(): validator → {Validator}", validator);
		_logger.LogDebug("ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting(): model.Fax → {Arg0}", model.Fax);
        model.Fax = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Fax);
        model.Fax = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Fax);

        //not required
        customerSettings.FaxRequired = false;
		_logger.LogDebug("ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting(): customerSettings.FaxRequired → {Arg0}", customerSettings.FaxRequired);
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.Fax = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Fax);
        model.Fax = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Fax);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenFaxIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenFaxIsSpecified()");
        var customerSettings = new CustomerSettings
        {
            FaxEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		_logger.LogDebug("ShouldNotHaveErrorWhenFaxIsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            Fax = "Fax"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Fax);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenFaxIsSpecified()");
    }
}