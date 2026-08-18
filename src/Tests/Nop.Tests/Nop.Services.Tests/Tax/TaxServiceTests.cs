using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Tax;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Tax;

[TestFixture]
public class TaxServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private TaxSettings _taxSettings;
    private bool _defaultEuVatAssumeValid;
    private ITaxPluginManager _taxPluginManager;
    private ISettingService _settingService;
    private ITaxService _taxService;
    private ICustomerService _customerService;
    private bool _defaultAdminRoleTaxExempt;
    private bool _defaultAdminTaxExempt;
    private bool _defaultEuVatUseWebService;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _settingService = GetService<ISettingService>();
        _taxSettings = GetService<TaxSettings>();
        _defaultEuVatUseWebService = _taxSettings.EuVatUseWebService;
        _defaultEuVatAssumeValid = _taxSettings.EuVatAssumeValid;
		_logger.LogDebug("SetUp(): _settingService → {_settingService}", _settingService);
		_logger.LogDebug("SetUp(): _taxSettings → {_taxSettings}", _taxSettings);
		_logger.LogDebug("SetUp(): _defaultEuVatUseWebService → {_defaultEuVatUseWebService}", _defaultEuVatUseWebService);
		_logger.LogDebug("SetUp(): _defaultEuVatAssumeValid → {_defaultEuVatAssumeValid}", _defaultEuVatAssumeValid);
		_logger.LogDebug("SetUp(): _taxSettings.EuVatAssumeValid → {Arg0}", _taxSettings.EuVatAssumeValid);
		_logger.LogDebug("SetUp(): _taxSettings.EuVatUseWebService → {Arg0}", _taxSettings.EuVatUseWebService);
		_logger.LogDebug("SetUp(): _taxSettings.HmrcApiUrl → {Arg0}", _taxSettings.HmrcApiUrl);
		_logger.LogDebug("SetUp(): _taxSettings.HmrcClientId → {Arg0}", _taxSettings.HmrcClientId);
		_logger.LogDebug("SetUp(): _taxSettings.HmrcClientSecret → {Arg0}", _taxSettings.HmrcClientSecret);
        _taxSettings.EuVatAssumeValid = false;
        _taxSettings.EuVatUseWebService = true;
        _taxSettings.HmrcApiUrl = "https://test-api.service.hmrc.gov.uk";
        _taxSettings.HmrcClientId = "TTLMDds3fJQhQjJOO5JHo1R6nxxT";
        _taxSettings.HmrcClientSecret = "cb9da68a-4184-4e8a-a071-11bfcb654ae8";

        await _settingService.SaveSettingAsync(_taxSettings);

        _taxService = GetService<ITaxService>();
        _taxPluginManager = GetService<ITaxPluginManager>();
            
			_logger.LogDebug("SetUp(): _taxService → {_taxService}", _taxService);
        _customerService = GetService<ICustomerService>();

		_logger.LogDebug("SetUp(): _taxPluginManager → {_taxPluginManager}", _taxPluginManager);
		_logger.LogDebug("SetUp(): _customerService → {_customerService}", _customerService);
        var adminRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.AdministratorsRoleName);
		_logger.LogDebug("SetUp(): adminRole → {AdminRole}", adminRole);
        _defaultAdminRoleTaxExempt = adminRole.TaxExempt;
        var admin = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
		_logger.LogWarning("SetUp(): admin → {Admin}", admin);
		_logger.LogDebug("SetUp(): _defaultAdminRoleTaxExempt → {_defaultAdminRoleTaxExempt}", _defaultAdminRoleTaxExempt);
        _defaultAdminTaxExempt = admin.IsTaxExempt;
		_logger.LogDebug("SetUp(): _defaultAdminTaxExempt → {_defaultAdminTaxExempt}", _defaultAdminTaxExempt);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        _taxSettings.EuVatAssumeValid = _defaultEuVatAssumeValid;
        _taxSettings.EuVatUseWebService = _defaultEuVatUseWebService;
		_logger.LogDebug("TearDown(): _taxSettings.EuVatUseWebService → {Arg0}", _taxSettings.EuVatUseWebService);
		_logger.LogDebug("TearDown(): _taxSettings.EuVatAssumeValid → {Arg0}", _taxSettings.EuVatAssumeValid);

        await _settingService.SaveSettingAsync(_taxSettings);

        var adminRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.AdministratorsRoleName);
		_logger.LogDebug("TearDown(): adminRole → {AdminRole}", adminRole);
		_logger.LogDebug("TearDown(): adminRole.TaxExempt → {Arg0}", adminRole.TaxExempt);
		_logger.LogDebug("TearDown(): adminRole.Active → {Arg0}", adminRole.Active);
        adminRole.TaxExempt = _defaultAdminRoleTaxExempt;
        adminRole.Active = true;
        await _customerService.UpdateCustomerRoleAsync(adminRole);

        var admin = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
		_logger.LogWarning("TearDown(): admin → {Admin}", admin);
		_logger.LogDebug("TearDown(): admin.IsTaxExempt → {Arg0}", admin.IsTaxExempt);
        admin.IsTaxExempt = _defaultAdminTaxExempt;
        await _customerService.UpdateCustomerAsync(admin);
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanLoadTaxProviders()
    {
		_logger.LogDebug(">>> Entering CanLoadTaxProviders()");
        var providers = await _taxPluginManager.LoadAllPluginsAsync();
        providers.Should().NotBeNull();
		_logger.LogDebug("CanLoadTaxProviders(): providers → {Providers}", providers);
        providers.Any().Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanLoadTaxProviders()");
    }

    [Test]
    public async Task CanLoadTaxProviderBySystemKeyword()
    {
		_logger.LogDebug(">>> Entering CanLoadTaxProviderBySystemKeyword()");
        var provider = await _taxPluginManager.LoadPluginBySystemNameAsync("FixedTaxRateTest");
        provider.Should().NotBeNull();
		_logger.LogDebug("CanLoadTaxProviderBySystemKeyword(): provider → {Provider}", provider);
		_logger.LogDebug("<<< Exiting CanLoadTaxProviderBySystemKeyword()");
    }

    [Test]
    public async Task CanLoadActiveTaxProvider()
    {
		_logger.LogDebug(">>> Entering CanLoadActiveTaxProvider()");
        var provider = await _taxPluginManager.LoadPrimaryPluginAsync();
        provider.Should().NotBeNull();
		_logger.LogDebug("CanLoadActiveTaxProvider(): provider → {Provider}", provider);
		_logger.LogDebug("<<< Exiting CanLoadActiveTaxProvider()");
    }

    [Test]
    public async Task CanCheckIsPluginActive()
    {
		_logger.LogDebug(">>> Entering CanCheckIsPluginActive()");
        var provider = await _taxPluginManager.LoadPrimaryPluginAsync();
        provider.Should().NotBeNull();
		_logger.LogDebug("CanCheckIsPluginActive(): provider → {Provider}", provider);
        _taxPluginManager.IsPluginActive(provider).Should().BeTrue();
        var isActive = await _taxPluginManager.IsPluginActiveAsync(provider.PluginDescriptor.SystemName);
		_logger.LogDebug("CanCheckIsPluginActive(): isActive → {IsActive}", isActive);
        isActive.Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanCheckIsPluginActive()");
    }

    [Test]
    public async Task CanGetProductPricePriceIncludesTaxIncludingTaxTaxable()
    {
		_logger.LogDebug(">>> Entering CanGetProductPricePriceIncludesTaxIncludingTaxTaxable()");
        var customer = new Customer();
        var product = new Product();
		_logger.LogDebug("CanGetProductPricePriceIncludesTaxIncludingTaxTaxable(): product → {Product}", product);
		_logger.LogDebug("CanGetProductPricePriceIncludesTaxIncludingTaxTaxable(): customer → {Customer}", customer);

        var (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, true, customer, true);
        price.Should().Be(1000);
        (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, true, customer, false);
        price.Should().Be(1100);
        (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, false, customer, true);
        price.Should().Be(909.0909090909090909090909091M);
        (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, false, customer, false);
        price.Should().Be(1000);
		_logger.LogDebug("<<< Exiting CanGetProductPricePriceIncludesTaxIncludingTaxTaxable()");
    }

    [Test]
    public async Task CanGetProductPrice()
    {
		_logger.LogDebug(">>> Entering CanGetProductPrice()");
        var product = new Product();
        var customer = new Customer();
		_logger.LogDebug("CanGetProductPrice(): customer → {Customer}", customer);
		_logger.LogDebug("CanGetProductPrice(): product → {Product}", product);

        var (price, _) = await _taxService.GetProductPriceAsync(product, 1000M);
        price.Should().Be(1000);
        (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, true, customer, true);
        price.Should().Be(1000);
		_logger.LogDebug("<<< Exiting CanGetProductPrice()");
    }

    [Test]
    public async Task CanGetProductPricePriceIncludesTaxIncludingTaxNonTaxable()
    {
		_logger.LogDebug(">>> Entering CanGetProductPricePriceIncludesTaxIncludingTaxNonTaxable()");
        var customer = new Customer();
        var product = new Product();
		_logger.LogDebug("CanGetProductPricePriceIncludesTaxIncludingTaxNonTaxable(): product → {Product}", product);
		_logger.LogDebug("CanGetProductPricePriceIncludesTaxIncludingTaxNonTaxable(): customer → {Customer}", customer);

        //not taxable
        customer.IsTaxExempt = true;
		_logger.LogDebug("CanGetProductPricePriceIncludesTaxIncludingTaxNonTaxable(): customer.IsTaxExempt → {Arg0}", customer.IsTaxExempt);

        var (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, true, customer, true);
        price.Should().Be(909.0909090909090909090909091M);
        (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, true, customer, false);
        price.Should().Be(1000);
        (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, false, customer, true);
        price.Should().Be(909.0909090909090909090909091M);
        (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, false, customer, false);
        price.Should().Be(1000);
		_logger.LogDebug("<<< Exiting CanGetProductPricePriceIncludesTaxIncludingTaxNonTaxable()");
    }

    [Test]
    [TestCase("GB553557881", VatNumberStatus.Valid)]
    [TestCase("NO974761076", VatNumberStatus.Unknown)]
    [TestCase("GB430479893", VatNumberStatus.Invalid)]
    [TestCase("IT00478390347", VatNumberStatus.Valid)]
    public async Task CanCheckVatNumber(string vatNumber, VatNumberStatus canBeStatus)
    {
		_logger.LogDebug(">>> Entering CanCheckVatNumber(vatNumber={VatNumber},canBeStatus={CanBeStatus})", vatNumber, canBeStatus);
        var result = await _taxService.GetVatNumberStatusAsync(vatNumber);

		_logger.LogDebug("CanCheckVatNumber(vatNumber={VatNumber},canBeStatus={CanBeStatus}): result → {Result}", vatNumber, canBeStatus, result);
        result.vatNumberStatus.Should().Be(canBeStatus);
		_logger.LogDebug("<<< Exiting CanCheckVatNumber(vatNumber={VatNumber},canBeStatus={CanBeStatus})", vatNumber, canBeStatus);
    }
}