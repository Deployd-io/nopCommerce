using FluentAssertions;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Attributes;
using Nop.Services.Configuration;
using Nop.Web.Factories;
using Nop.Web.Models.Customer;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class CustomerModelFactoryTests : WebTest
{
    private IAttributeService<CustomerAttribute, CustomerAttributeValue> _customerAttributeService;
    private ICustomerModelFactory _customerModelFactory;
    private Customer _customer;
    private CustomerAttribute[] _customerAttributes;
    private CustomerSettings _customerSettings;
    private ISettingService _settingService;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _customerSettings = GetService<CustomerSettings>();
        _settingService = GetService<ISettingService>();
		logger.LogDebug("SetUp(): _customerSettings → {_customerSettings}", _customerSettings);
		logger.LogDebug("SetUp(): _settingService → {_settingService}", _settingService);
		logger.LogDebug("SetUp(): _customerSettings.CountryEnabled → {Arg0}", _customerSettings.CountryEnabled);
		logger.LogDebug("SetUp(): _customerSettings.StateProvinceEnabled → {Arg0}", _customerSettings.StateProvinceEnabled);

        _customerSettings.CountryEnabled = true;
        _customerSettings.StateProvinceEnabled = true;
        await _settingService.SaveSettingAsync(_customerSettings);

        _customerAttributeService = GetService<IAttributeService<CustomerAttribute, CustomerAttributeValue>>();
		logger.LogDebug("SetUp(): _customerAttributeService → {_customerAttributeService}", _customerAttributeService);
        _customerModelFactory = GetService<ICustomerModelFactory>();
        _customer = await GetService<IWorkContext>().GetCurrentCustomerAsync();
		logger.LogDebug("SetUp(): _customerModelFactory → {_customerModelFactory}", _customerModelFactory);
		logger.LogDebug("SetUp(): _customer → {_customer}", _customer);
        
        _customerAttributes =
        [
            new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.Checkboxes, Name = "Test customer attribute 1"
        },
        new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.ColorSquares, Name = "Test customer attribute 2"
        },
        new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.Datepicker, Name = "Test customer attribute 3"
        },
        new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.DropdownList, Name = "Test customer attribute 4"
        },
        new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.FileUpload, Name = "Test customer attribute 5"
        },
        new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.ImageSquares, Name = "Test customer attribute 6"
        },
        new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.MultilineTextbox, Name = "Test customer attribute 7"
        },
        new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.RadioList, Name = "Test customer attribute 8"
        },
        new CustomerAttribute
        {
            AttributeControlType = AttributeControlType.ReadonlyCheckboxes, Name = "Test customer attribute 9"
        },
        new CustomerAttribute
            {
                AttributeControlType = AttributeControlType.TextBox, Name = "Test customer attribute 10"
            }
            ];

        foreach (var customerAttribute in _customerAttributes)
            await _customerAttributeService.InsertAttributeAsync(customerAttribute);
			logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
        _customerSettings.CountryEnabled = false;
        _customerSettings.StateProvinceEnabled = false;
		logger.LogDebug("TearDown(): _customerSettings.StateProvinceEnabled → {Arg0}", _customerSettings.StateProvinceEnabled);
		logger.LogDebug("TearDown(): _customerSettings.CountryEnabled → {Arg0}", _customerSettings.CountryEnabled);
        await _settingService.SaveSettingAsync(_customerSettings);

        foreach (var customerAttribute in _customerAttributes)
            await _customerAttributeService.DeleteAttributeAsync(customerAttribute);
			logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanPrepareCustomerInfoModel()
    {
		logger.LogDebug(">>> Entering CanPrepareCustomerInfoModel()");
        var model = await _customerModelFactory.PrepareCustomerInfoModelAsync(new CustomerInfoModel(), _customer, false);
        model.AvailableTimeZones.Any().Should().BeTrue();
		logger.LogDebug("CanPrepareCustomerInfoModel(): model → {Model}", model);

        model.Email.Should().Be(NopTestsDefaults.AdminEmail);
        model.Username.Should().Be(NopTestsDefaults.AdminEmail);
        model.FirstName.Should().Be("John");
        model.LastName.Should().Be("Smith");

        model = await _customerModelFactory.PrepareCustomerInfoModelAsync(new CustomerInfoModel(), _customer, true);

        model.Email.Should().BeNullOrEmpty();
        model.Username.Should().BeNullOrEmpty();
        model.FirstName.Should().BeNullOrEmpty();
        model.LastName.Should().BeNullOrEmpty();
		logger.LogDebug("<<< Exiting CanPrepareCustomerInfoModel()");
    }

    [Test]
    public async Task CanPrepareRegisterModel()
    {
		logger.LogDebug(">>> Entering CanPrepareRegisterModel()");
        var model = await _customerModelFactory.PrepareRegisterModelAsync(new RegisterModel(), false);
        model.AvailableTimeZones.Any().Should().BeTrue();
		logger.LogDebug("CanPrepareRegisterModel(): model → {Model}", model);
        model.CustomerAttributes.Any().Should().BeTrue();
		logger.LogDebug("<<< Exiting CanPrepareRegisterModel()");
    }

    [Test]
    public async Task CanPrepareLoginModel()
    {
		logger.LogDebug(">>> Entering CanPrepareLoginModel()");
        var model = await _customerModelFactory.PrepareLoginModelAsync(null);
        model.CheckoutAsGuest.Should().Be(default);
		logger.LogDebug("CanPrepareLoginModel(): model → {Model}", model);
        model = await _customerModelFactory.PrepareLoginModelAsync(true);
        model.CheckoutAsGuest.Should().BeTrue();
        model = await _customerModelFactory.PrepareLoginModelAsync(false);
        model.CheckoutAsGuest.Should().BeFalse();
		logger.LogDebug("<<< Exiting CanPrepareLoginModel()");
    }

    [Test]
    public async Task CanPreparePasswordRecoveryModel()
    {
		logger.LogDebug(">>> Entering CanPreparePasswordRecoveryModel()");
        var model = await _customerModelFactory.PreparePasswordRecoveryModelAsync(new PasswordRecoveryModel { Email = "test@email.com" });
        model.DisplayCaptcha.Should().BeFalse();
		logger.LogDebug("CanPreparePasswordRecoveryModel(): model → {Model}", model);
        model.Email.Should().Be("test@email.com");
		logger.LogDebug("<<< Exiting CanPreparePasswordRecoveryModel()");
    }

    [Test]
    public async Task CanPrepareRegisterResultModel()
    {
		logger.LogDebug(">>> Entering CanPrepareRegisterResultModel()");
        var model = await _customerModelFactory.PrepareRegisterResultModelAsync((int)UserRegistrationType.AdminApproval, string.Empty);
        model.Result.Should().Be("Your account will be activated after approving by administrator.");
		logger.LogDebug("CanPrepareRegisterResultModel(): model → {Model}", model);
        model = await _customerModelFactory.PrepareRegisterResultModelAsync((int)UserRegistrationType.Disabled, string.Empty);
        model.Result.Should().Be("Registration not allowed. You can edit this in the admin area.");
        model = await _customerModelFactory.PrepareRegisterResultModelAsync((int)UserRegistrationType.EmailValidation, string.Empty);
        model.Result.Should().Be("Your registration has been successfully completed. You have just been sent an email containing activation instructions.");
        model = await _customerModelFactory.PrepareRegisterResultModelAsync((int)UserRegistrationType.Standard, string.Empty);
        model.Result.Should().Be("Your registration completed");
        model = await _customerModelFactory.PrepareRegisterResultModelAsync(400, string.Empty);
        model.Result.Should().BeNullOrEmpty();
		logger.LogDebug("<<< Exiting CanPrepareRegisterResultModel()");
    }

    [Test]
    public async Task CanPrepareCustomCustomerAttributes()
    {
		logger.LogDebug(">>> Entering CanPrepareCustomCustomerAttributes()");
        var model = await _customerModelFactory.PrepareCustomCustomerAttributesAsync(_customer);
        model.Any().Should().BeTrue();
		logger.LogDebug("CanPrepareCustomCustomerAttributes(): model → {Model}", model);
        model.Count.Should().Be(10);
		logger.LogDebug("<<< Exiting CanPrepareCustomCustomerAttributes()");
    }
}
