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
public class CustomerInfoValidatorTests : BaseNopTest
{
    private ILocalizationService _localizationService;
    private IStateProvinceService _stateProvinceService;
    private TaxSettings _taxSettings;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _localizationService = GetService<ILocalizationService>();
        _stateProvinceService = GetService<IStateProvinceService>();
		logger.LogDebug("SetUp(): _localizationService → {_localizationService}", _localizationService);
		logger.LogDebug("SetUp(): _stateProvinceService → {_stateProvinceService}", _stateProvinceService);
        _taxSettings = GetService<TaxSettings>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _taxSettings → {_taxSettings}", _taxSettings);
    }

    private CustomerInfoValidator GetDefaultValidator()
    {
		logger.LogDebug(">>> Entering GetDefaultValidator()");
		logger.LogDebug("<<< Exiting GetDefaultValidator()");
        return new CustomerInfoValidator(_localizationService, _stateProvinceService, GetService<CustomerSettings>(), _taxSettings);
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenEmailIsNullOrEmpty()");
        var validator = GetDefaultValidator();

		logger.LogDebug("ShouldHaveErrorWhenEmailIsNullOrEmpty(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            Email = null
        };
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        model.Email = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenEmailIsNullOrEmpty(): model.Email → {Arg0}", model.Email);
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenEmailIsNullOrEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsWrongFormat()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenEmailIsWrongFormat()");
        var validator = GetDefaultValidator();

		logger.LogDebug("ShouldHaveErrorWhenEmailIsWrongFormat(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            Email = "adminexample.com"
        };
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenEmailIsWrongFormat()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenEmailIsCorrectFormat()");
        var validator = GetDefaultValidator();

		logger.LogDebug("ShouldNotHaveErrorWhenEmailIsCorrectFormat(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            Email = "admin@example.com"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenEmailIsCorrectFormat()");
    }

    [Test]
    public void ShouldHaveErrorWhenFirstNameIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenFirstNameIsNullOrEmpty()");
        var customerSettings = new CustomerSettings
        {
            FirstNameEnabled = true,
            FirstNameRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("ShouldHaveErrorWhenFirstNameIsNullOrEmpty(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            FirstName = null
        };
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);
        model.FirstName = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenFirstNameIsNullOrEmpty(): model.FirstName → {Arg0}", model.FirstName);
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenFirstNameIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenFirstNameIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenFirstNameIsSpecified()");
        var customerSettings = new CustomerSettings
        {
            FirstNameEnabled = true,
            FirstNameRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("ShouldNotHaveErrorWhenFirstNameIsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            FirstName = "John"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FirstName);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenFirstNameIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenLastNameIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenLastNameIsNullOrEmpty()");
        var model = new CustomerInfoModel();
        //required
		logger.LogDebug("ShouldHaveErrorWhenLastNameIsNullOrEmpty(): model → {Model}", model);
        var customerSettings = new CustomerSettings
        {
            LastNameEnabled = true,
            LastNameRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("ShouldHaveErrorWhenLastNameIsNullOrEmpty(): validator → {Validator}", validator);
		logger.LogDebug("ShouldHaveErrorWhenLastNameIsNullOrEmpty(): model.LastName → {Arg0}", model.LastName);
        model.LastName = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);
        model.LastName = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);

        //not required
        customerSettings.LastNameRequired = false;
		logger.LogDebug("ShouldHaveErrorWhenLastNameIsNullOrEmpty(): customerSettings.LastNameRequired → {Arg0}", customerSettings.LastNameRequired);
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.LastName = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.LastName);
        model.LastName = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.LastName);
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

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("ShouldNotHaveErrorWhenLastNameIsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            LastName = "Smith"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.LastName);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenLastNameIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting()");
        var model = new CustomerInfoModel();

		logger.LogDebug("ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var customerSettings = new CustomerSettings
        {
            CompanyEnabled = true,
            CompanyRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting(): validator → {Validator}", validator);
		logger.LogDebug("ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting(): model.Company → {Arg0}", model.Company);
        model.Company = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Company);
        model.Company = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Company);

        //not required
        customerSettings.CompanyRequired = false;
		logger.LogDebug("ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting(): customerSettings.CompanyRequired → {Arg0}", customerSettings.CompanyRequired);
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.Company = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Company);
        model.Company = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Company);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenCompanyIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenCompanyIsSpecified()");
        var customerSettings = new CustomerSettings
        {
            CompanyEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("ShouldNotHaveErrorWhenCompanyIsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            Company = "Company"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Company);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenCompanyIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting()");
        var model = new CustomerInfoModel();

		logger.LogDebug("ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var customerSettings = new CustomerSettings
        {
            StreetAddressEnabled = true,
            StreetAddressRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting(): validator → {Validator}", validator);
		logger.LogDebug("ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting(): model.StreetAddress → {Arg0}", model.StreetAddress);
        model.StreetAddress = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StreetAddress);
        model.StreetAddress = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StreetAddress);

        //not required
        customerSettings.StreetAddressRequired = false;
		logger.LogDebug("ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting(): customerSettings.StreetAddressRequired → {Arg0}", customerSettings.StreetAddressRequired);
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.StreetAddress = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress);
        model.StreetAddress = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenStreetAddressIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenStreetAddressIsSpecified()");
        var customerSettings = new CustomerSettings
        {
            StreetAddressEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("ShouldNotHaveErrorWhenStreetAddressIsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            StreetAddress = "Street address"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenStreetAddressIsSpecified()");
    }

    [Test]
    public void PublicVoidShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting()
    {
		logger.LogDebug(">>> Entering PublicVoidShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting()");
        var model = new CustomerInfoModel();

		logger.LogDebug("PublicVoidShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var customerSettings = new CustomerSettings
        {
            StreetAddress2Enabled = true,
            StreetAddress2Required = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("PublicVoidShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting(): validator → {Validator}", validator);
		logger.LogDebug("PublicVoidShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting(): model.StreetAddress2 → {Arg0}", model.StreetAddress2);
        model.StreetAddress2 = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StreetAddress2);
        model.StreetAddress2 = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StreetAddress2);

        //not required
        customerSettings.StreetAddress2Required = false;
		logger.LogDebug("PublicVoidShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting(): customerSettings.StreetAddress2Required → {Arg0}", customerSettings.StreetAddress2Required);
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.StreetAddress2 = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress2);
        model.StreetAddress2 = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress2);
		logger.LogDebug("<<< Exiting PublicVoidShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenStreetAddress2IsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenStreetAddress2IsSpecified()");
        var customerSettings = new CustomerSettings
        {
            StreetAddress2Enabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("ShouldNotHaveErrorWhenStreetAddress2IsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            StreetAddress2 = "Street address 2"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress2);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenStreetAddress2IsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting()");
        var model = new CustomerInfoModel();

		logger.LogDebug("ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var customerSettings = new CustomerSettings
        {
            ZipPostalCodeEnabled = true,
            ZipPostalCodeRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting(): validator → {Validator}", validator);
		logger.LogDebug("ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting(): model.ZipPostalCode → {Arg0}", model.ZipPostalCode);
        model.ZipPostalCode = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ZipPostalCode);
        model.ZipPostalCode = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ZipPostalCode);

        //not required
        customerSettings.ZipPostalCodeRequired = false;
		logger.LogDebug("ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting(): customerSettings.ZipPostalCodeRequired → {Arg0}", customerSettings.ZipPostalCodeRequired);
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.ZipPostalCode = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode);
        model.ZipPostalCode = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenZipPostalCodeIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenZipPostalCodeIsSpecified()");
        var customerSettings = new CustomerSettings
        {
            ZipPostalCodeEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("ShouldNotHaveErrorWhenZipPostalCodeIsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            ZipPostalCode = "zip"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenZipPostalCodeIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting()");
        var model = new CustomerInfoModel();

		logger.LogDebug("ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var customerSettings = new CustomerSettings
        {
            CityEnabled = true,
            CityRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting(): validator → {Validator}", validator);
		logger.LogDebug("ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting(): model.City → {Arg0}", model.City);
        model.City = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.City);
        model.City = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.City);

        //not required
        customerSettings.CityRequired = false;
		logger.LogDebug("ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting(): customerSettings.CityRequired → {Arg0}", customerSettings.CityRequired);
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.City = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.City);
        model.City = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.City);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenCityIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenCityIsSpecified()");
        var customerSettings = new CustomerSettings
        {
            CityEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("ShouldNotHaveErrorWhenCityIsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            City = "City"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.City);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenCityIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting()");
        var model = new CustomerInfoModel();

		logger.LogDebug("ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var customerSettings = new CustomerSettings
        {
            PhoneEnabled = true,
            PhoneRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting(): validator → {Validator}", validator);
		logger.LogDebug("ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting(): model.Phone → {Arg0}", model.Phone);
        model.Phone = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Phone);
        model.Phone = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Phone);

        //not required
        customerSettings.PhoneRequired = false;
		logger.LogDebug("ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting(): customerSettings.PhoneRequired → {Arg0}", customerSettings.PhoneRequired);
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.Phone = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Phone);
        model.Phone = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Phone);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenPhoneIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenPhoneIsSpecified()");
        var customerSettings = new CustomerSettings
        {
            PhoneEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("ShouldNotHaveErrorWhenPhoneIsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            Phone = "Phone"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Phone);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenPhoneIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting()");
        var model = new CustomerInfoModel();

		logger.LogDebug("ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var customerSettings = new CustomerSettings
        {
            FaxEnabled = true,
            FaxRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting(): validator → {Validator}", validator);
		logger.LogDebug("ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting(): model.Fax → {Arg0}", model.Fax);
        model.Fax = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Fax);
        model.Fax = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Fax);

        //not required
        customerSettings.FaxRequired = false;
		logger.LogDebug("ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting(): customerSettings.FaxRequired → {Arg0}", customerSettings.FaxRequired);
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.Fax = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Fax);
        model.Fax = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Fax);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenFaxIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenFaxIsSpecified()");
        var customerSettings = new CustomerSettings
        {
            FaxEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

		logger.LogDebug("ShouldNotHaveErrorWhenFaxIsSpecified(): validator → {Validator}", validator);
        var model = new CustomerInfoModel
        {
            Fax = "Fax"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Fax);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenFaxIsSpecified()");
    }
}