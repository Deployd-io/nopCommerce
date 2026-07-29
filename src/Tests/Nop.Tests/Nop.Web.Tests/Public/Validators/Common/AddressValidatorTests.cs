using FluentValidation.TestHelper;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Common;
using Nop.Web.Validators.Common;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Common;

[TestFixture]
public class AddressValidatorTests : BaseNopTest
{
    private ILocalizationService _localizationService;
    private IStateProvinceService _stateProvinceService;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _localizationService = GetService<ILocalizationService>();
        _stateProvinceService = GetService<IStateProvinceService>();
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _localizationService → {_localizationService}", _localizationService);
		logger.LogDebug("Setup(): _stateProvinceService → {_stateProvinceService}", _stateProvinceService);
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenEmailIsNullOrEmpty()");
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings(), new CustomerSettings());

        var model = new AddressModel
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
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings(), new CustomerSettings());

        var model = new AddressModel
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
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings(), new CustomerSettings());

        var model = new AddressModel
        {
            Email = "admin@example.com"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenEmailIsCorrectFormat()");
    }

    [Test]
    public void ShouldHaveErrorWhenFirstnameIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenFirstnameIsNullOrEmpty()");
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings(), new CustomerSettings());

        var model = new AddressModel
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
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings(), new CustomerSettings());

        var model = new AddressModel
        {
            FirstName = "John"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FirstName);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenFirstnameIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenLastnameIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenLastnameIsNullOrEmpty()");
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings(), new CustomerSettings());

        var model = new AddressModel
        {
            LastName = null
        };
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);
        model.LastName = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenLastnameIsNullOrEmpty(): model.LastName → {Arg0}", model.LastName);
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenLastnameIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenLastnameIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenLastnameIsSpecified()");
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings(), new CustomerSettings());

        var model = new AddressModel
        {
            LastName = "Smith"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.LastName);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenLastnameIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting()");
        var model = new AddressModel();

		logger.LogDebug("ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                CompanyEnabled = true,
                CompanyRequired = true
            }, new CustomerSettings());
        model.Company = null;
		logger.LogDebug("ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting(): model.Company → {Arg0}", model.Company);
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Company);
        model.Company = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Company);

        //not required
        validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                CompanyEnabled = true,
                CompanyRequired = false
            }, new CustomerSettings());
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
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                CompanyEnabled = true
            }, new CustomerSettings());

        var model = new AddressModel
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
        var model = new AddressModel();

		logger.LogDebug("ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                StreetAddressEnabled = true,
                StreetAddressRequired = true
            }, new CustomerSettings());
        model.Address1 = null;
		logger.LogDebug("ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting(): model.Address1 → {Arg0}", model.Address1);
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Address1);
        model.Address1 = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Address1);

        //not required
        validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                StreetAddressEnabled = true,
                StreetAddressRequired = false
            }, new CustomerSettings());
        model.Address1 = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Address1);
        model.Address1 = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Address1);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenStreetAddressIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenStreetAddressIsSpecified()");
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                StreetAddressEnabled = true
            }, new CustomerSettings());

        var model = new AddressModel
        {
            Address1 = "Street address"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Address1);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenStreetAddressIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting()");
        var model = new AddressModel();

		logger.LogDebug("ShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                StreetAddress2Enabled = true,
                StreetAddress2Required = true
            }, new CustomerSettings());
        model.Address2 = null;
		logger.LogDebug("ShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting(): model.Address2 → {Arg0}", model.Address2);
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Address2);
        model.Address2 = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Address2);

        //not required
        validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                StreetAddress2Enabled = true,
                StreetAddress2Required = false
            }, new CustomerSettings());
        model.Address2 = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Address2);
        model.Address2 = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Address2);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenStreetAddress2IsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenStreetAddress2IsSpecified()");
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                StreetAddress2Enabled = true
            }, new CustomerSettings());

        var model = new AddressModel
        {
            Address2 = "Street address 2"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Address2);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenStreetAddress2IsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting()");
        var model = new AddressModel();

		logger.LogDebug("ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                ZipPostalCodeEnabled = true,
                ZipPostalCodeRequired = true
            }, new CustomerSettings());
        model.ZipPostalCode = null;
		logger.LogDebug("ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting(): model.ZipPostalCode → {Arg0}", model.ZipPostalCode);
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ZipPostalCode);
        model.ZipPostalCode = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ZipPostalCode);

        //not required
        validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                ZipPostalCodeEnabled = true,
                ZipPostalCodeRequired = false
            }, new CustomerSettings());
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
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                StreetAddress2Enabled = true
            }, new CustomerSettings());

        var model = new AddressModel
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
        var model = new AddressModel();

		logger.LogDebug("ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                CityEnabled = true,
                CityRequired = true
            }, new CustomerSettings());
        model.City = null;
		logger.LogDebug("ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting(): model.City → {Arg0}", model.City);
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.City);
        model.City = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.City);

        //not required
        validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                CityEnabled = true,
                CityRequired = false
            }, new CustomerSettings());
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
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                CityEnabled = true
            }, new CustomerSettings());

        var model = new AddressModel
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
        var model = new AddressModel();

		logger.LogDebug("ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                PhoneEnabled = true,
                PhoneRequired = true
            }, new CustomerSettings());
        model.PhoneNumber = null;
		logger.LogDebug("ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting(): model.PhoneNumber → {Arg0}", model.PhoneNumber);
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        model.PhoneNumber = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.PhoneNumber);

        //not required
        validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                PhoneEnabled = true,
                PhoneRequired = false
            }, new CustomerSettings());
        model.PhoneNumber = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
        model.PhoneNumber = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenPhoneIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenPhoneIsSpecified()");
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                PhoneEnabled = true
            }, new CustomerSettings());

        var model = new AddressModel
        {
            PhoneNumber = "Phone"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenPhoneIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting()");
        var model = new AddressModel();

		logger.LogDebug("ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting(): model → {Model}", model);
        //required
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                FaxEnabled = true,
                FaxRequired = true
            }, new CustomerSettings());
        model.FaxNumber = null;
		logger.LogDebug("ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting(): model.FaxNumber → {Arg0}", model.FaxNumber);
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FaxNumber);
        model.FaxNumber = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FaxNumber);

        //not required
        validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                FaxEnabled = true,
                FaxRequired = false
            }, new CustomerSettings());
        model.FaxNumber = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FaxNumber);
        model.FaxNumber = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FaxNumber);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenFaxIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenFaxIsSpecified()");
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                FaxEnabled = true
            }, new CustomerSettings());

        var model = new AddressModel
        {
            FaxNumber = "Fax"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FaxNumber);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenFaxIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenCountryIdIsNullOrZeroAnd()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenCountryIdIsNullOrZeroAnd()");
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                CountryEnabled = true
            }, new CustomerSettings());

        var model = new AddressModel
        {
            CountryId = null
        };
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.CountryId);
        model.CountryId = 0;
		logger.LogDebug("ShouldHaveErrorWhenCountryIdIsNullOrZeroAnd(): model.CountryId → {Arg0}", model.CountryId);
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.CountryId);
        model.CountryId = 1;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.CountryId);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenCountryIdIsNullOrZeroAnd()");
    }

    [Test]
    public void ShouldHaveErrorWhenStateIdIsNullOrZeroAnd()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenStateIdIsNullOrZeroAnd()");
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                CountryEnabled = true,
                StateProvinceEnabled = true
            }, new CustomerSettings());

        var model = new AddressModel
        {
            CountryId = 237, //USA
            StateProvinceId = null
        };
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StateProvinceId);
        model.StateProvinceId = 0;
		logger.LogDebug("ShouldHaveErrorWhenStateIdIsNullOrZeroAnd(): model.StateProvinceId → {Arg0}", model.StateProvinceId);
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StateProvinceId);
        model.StateProvinceId = 1828; //New York
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StateProvinceId);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenStateIdIsNullOrZeroAnd()");
    }

    [Test]
    public void ShouldHaveErrorWhenCountyIsNullOrEmptyAnd()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenCountyIsNullOrEmptyAnd()");
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                CountyEnabled = true,
                CountyRequired = true
            }, new CustomerSettings());

        var model = new AddressModel
        {
            County = string.Empty
        };
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.County);
        model.County = null;
		logger.LogDebug("ShouldHaveErrorWhenCountyIsNullOrEmptyAnd(): model.County → {Arg0}", model.County);
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.County);
        model.County = "Test county";
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.County);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenCountyIsNullOrEmptyAnd()");
    }

    
}