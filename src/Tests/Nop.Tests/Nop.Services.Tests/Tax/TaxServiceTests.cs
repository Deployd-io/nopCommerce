using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Tax;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Tax;

[TestFixture]
public class TaxServiceTests : ServiceTest
{
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
		logger.LogDebug(">>> Entering SetUp()");
        _settingService = GetService<ISettingService>();
        _taxSettings = GetService<TaxSettings>();
		logger.LogDebug("SetUp(): _settingService → {_settingService}", _settingService);
		logger.LogDebug("SetUp(): _taxSettings → {_taxSettings}", _taxSettings);
		logger.LogDebug("SetUp(): _taxSettings.EuVatAssumeValid → {Arg0}", _taxSettings.EuVatAssumeValid);
		logger.LogDebug("SetUp(): _taxSettings.EuVatUseWebService → {Arg0}", _taxSettings.EuVatUseWebService);
		logger.LogDebug("SetUp(): _taxSettings.HmrcApiUrl → {Arg0}", _taxSettings.HmrcApiUrl);
		logger.LogDebug("SetUp(): _taxSettings.HmrcClientId → {Arg0}", _taxSettings.HmrcClientId);
		logger.LogDebug("SetUp(): _taxSettings.HmrcClientSecret → {Arg0}", _taxSettings.HmrcClientSecret);
        _defaultEuVatUseWebService = _taxSettings.EuVatUseWebService;
        _defaultEuVatAssumeValid = _taxSettings.EuVatAssumeValid;
		logger.LogDebug("SetUp(): _defaultEuVatUseWebService → {_defaultEuVatUseWebService}", _defaultEuVatUseWebService);
        _taxSettings.EuVatAssumeValid = false;
        _taxSettings.EuVatUseWebService = true;
		logger.LogDebug("SetUp(): _defaultEuVatAssumeValid → {_defaultEuVatAssumeValid}", _defaultEuVatAssumeValid);
        _taxSettings.HmrcApiUrl = "https://test-api.service.hmrc.gov.uk";
        _taxSettings.HmrcClientId = "TTLMDds3fJQhQjJOO5JHo1R6nxxT";
        _taxSettings.HmrcClientSecret = "cb9da68a-4184-4e8a-a071-11bfcb654ae8";

        await _settingService.SaveSettingAsync(_taxSettings);

        _taxService = GetService<ITaxService>();
		logger.LogDebug("SetUp(): _taxService → {_taxService}", _taxService);
        _taxPluginManager = GetService<ITaxPluginManager>();
            
			logger.LogDebug("SetUp(): _taxPluginManager → {_taxPluginManager}", _taxPluginManager);
        _customerService = GetService<ICustomerService>();

		logger.LogDebug("SetUp(): _customerService → {_customerService}", _customerService);
        var adminRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.AdministratorsRoleName);
        _defaultAdminRoleTaxExempt = adminRole.TaxExempt;
		logger.LogDebug("SetUp(): adminRole → {AdminRole}", adminRole);
		logger.LogDebug("SetUp(): _defaultAdminRoleTaxExempt → {_defaultAdminRoleTaxExempt}", _defaultAdminRoleTaxExempt);
        var admin = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
        _defaultAdminTaxExempt = admin.IsTaxExempt;
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogWarning("SetUp(): admin → {Admin}", admin);
		logger.LogDebug("SetUp(): _defaultAdminTaxExempt → {_defaultAdminTaxExempt}", _defaultAdminTaxExempt);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
        _taxSettings.EuVatAssumeValid = _defaultEuVatAssumeValid;
        _taxSettings.EuVatUseWebService = _defaultEuVatUseWebService;
		logger.LogDebug("TearDown(): _taxSettings.EuVatUseWebService → {Arg0}", _taxSettings.EuVatUseWebService);
		logger.LogDebug("TearDown(): _taxSettings.EuVatAssumeValid → {Arg0}", _taxSettings.EuVatAssumeValid);

        await _settingService.SaveSettingAsync(_taxSettings);

        var adminRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.AdministratorsRoleName);
        adminRole.TaxExempt = _defaultAdminRoleTaxExempt;
		logger.LogDebug("TearDown(): adminRole → {AdminRole}", adminRole);
		logger.LogDebug("TearDown(): adminRole.TaxExempt → {Arg0}", adminRole.TaxExempt);
		logger.LogDebug("TearDown(): adminRole.Active → {Arg0}", adminRole.Active);
        adminRole.Active = true;
        await _customerService.UpdateCustomerRoleAsync(adminRole);

        var admin = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
        admin.IsTaxExempt = _defaultAdminTaxExempt;
		logger.LogWarning("TearDown(): admin → {Admin}", admin);
		logger.LogDebug("TearDown(): admin.IsTaxExempt → {Arg0}", admin.IsTaxExempt);
        await _customerService.UpdateCustomerAsync(admin);
		logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanLoadTaxProviders()
    {
		logger.LogDebug(">>> Entering CanLoadTaxProviders()");
        var providers = await _taxPluginManager.LoadAllPluginsAsync();
        providers.Should().NotBeNull();
		logger.LogDebug("CanLoadTaxProviders(): providers → {Providers}", providers);
        providers.Any().Should().BeTrue();
		logger.LogDebug("<<< Exiting CanLoadTaxProviders()");
    }

    [Test]
    public async Task CanLoadTaxProviderBySystemKeyword()
    {
		logger.LogDebug(">>> Entering CanLoadTaxProviderBySystemKeyword()");
        var provider = await _taxPluginManager.LoadPluginBySystemNameAsync("FixedTaxRateTest");
        provider.Should().NotBeNull();
		logger.LogDebug("<<< Exiting CanLoadTaxProviderBySystemKeyword()");
		logger.LogDebug("CanLoadTaxProviderBySystemKeyword(): provider → {Provider}", provider);
    }

    [Test]
    public async Task CanLoadActiveTaxProvider()
    {
		logger.LogDebug(">>> Entering CanLoadActiveTaxProvider()");
        var provider = await _taxPluginManager.LoadPrimaryPluginAsync();
        provider.Should().NotBeNull();
		logger.LogDebug("<<< Exiting CanLoadActiveTaxProvider()");
		logger.LogDebug("CanLoadActiveTaxProvider(): provider → {Provider}", provider);
    }

    [Test]
    public async Task CanCheckIsPluginActive()
    {
		logger.LogDebug(">>> Entering CanCheckIsPluginActive()");
        var provider = await _taxPluginManager.LoadPrimaryPluginAsync();
        provider.Should().NotBeNull();
		logger.LogDebug("CanCheckIsPluginActive(): provider → {Provider}", provider);
        _taxPluginManager.IsPluginActive(provider).Should().BeTrue();
        var isActive = await _taxPluginManager.IsPluginActiveAsync(provider.PluginDescriptor.SystemName);
        isActive.Should().BeTrue();
		logger.LogDebug("<<< Exiting CanCheckIsPluginActive()");
		logger.LogDebug("CanCheckIsPluginActive(): isActive → {IsActive}", isActive);
    }

    [Test]
    public async Task CanGetProductPricePriceIncludesTaxIncludingTaxTaxable()
    {
		logger.LogDebug(">>> Entering CanGetProductPricePriceIncludesTaxIncludingTaxTaxable()");
        var customer = new Customer();
        var product = new Product();
		logger.LogDebug("CanGetProductPricePriceIncludesTaxIncludingTaxTaxable(): customer → {Customer}", customer);
		logger.LogDebug("CanGetProductPricePriceIncludesTaxIncludingTaxTaxable(): product → {Product}", product);

        var (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, true, customer, true);
        price.Should().Be(1000);
        (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, true, customer, false);
        price.Should().Be(1100);
        (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, false, customer, true);
        price.Should().Be(909.0909090909090909090909091M);
        (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, false, customer, false);
        price.Should().Be(1000);
		logger.LogDebug("<<< Exiting CanGetProductPricePriceIncludesTaxIncludingTaxTaxable()");
    }

    [Test]
    public async Task CanGetProductPrice()
    {
		logger.LogDebug(">>> Entering CanGetProductPrice()");
        var product = new Product();
        var customer = new Customer();
		logger.LogDebug("CanGetProductPrice(): product → {Product}", product);
		logger.LogDebug("CanGetProductPrice(): customer → {Customer}", customer);

        var (price, _) = await _taxService.GetProductPriceAsync(product, 1000M);
        price.Should().Be(1000);
        (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, true, customer, true);
        price.Should().Be(1000);
		logger.LogDebug("<<< Exiting CanGetProductPrice()");
    }

    [Test]
    public async Task CanGetProductPricePriceIncludesTaxIncludingTaxNonTaxable()
    {
		logger.LogDebug(">>> Entering CanGetProductPricePriceIncludesTaxIncludingTaxNonTaxable()");
        var customer = new Customer();
        var product = new Product();
		logger.LogDebug("CanGetProductPricePriceIncludesTaxIncludingTaxNonTaxable(): customer → {Customer}", customer);
		logger.LogDebug("CanGetProductPricePriceIncludesTaxIncludingTaxNonTaxable(): product → {Product}", product);

        //not taxable
        customer.IsTaxExempt = true;
		logger.LogDebug("CanGetProductPricePriceIncludesTaxIncludingTaxNonTaxable(): customer.IsTaxExempt → {Arg0}", customer.IsTaxExempt);

        var (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, true, customer, true);
        price.Should().Be(909.0909090909090909090909091M);
        (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, true, customer, false);
        price.Should().Be(1000);
        (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, false, customer, true);
        price.Should().Be(909.0909090909090909090909091M);
        (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, false, customer, false);
        price.Should().Be(1000);
		logger.LogDebug("<<< Exiting CanGetProductPricePriceIncludesTaxIncludingTaxNonTaxable()");
    }

    [Test]
    [TestCase("GB553557881", VatNumberStatus.Valid)]
    [TestCase("NO974761076", VatNumberStatus.Unknown)]
    [TestCase("GB430479893", VatNumberStatus.Invalid)]
    [TestCase("IT00478390347", VatNumberStatus.Valid)]
    public async Task CanCheckVatNumber(string vatNumber, VatNumberStatus canBeStatus)
    {
		logger.LogDebug(">>> Entering CanCheckVatNumber(vatNumber={VatNumber},canBeStatus={CanBeStatus})", vatNumber, canBeStatus);
        var result = await _taxService.GetVatNumberStatusAsync(vatNumber);

		logger.LogDebug("CanCheckVatNumber(vatNumber={VatNumber},canBeStatus={CanBeStatus}): result → {Result}", vatNumber, canBeStatus, result);
        result.vatNumberStatus.Should().Be(canBeStatus);
		logger.LogDebug("<<< Exiting CanCheckVatNumber(vatNumber={VatNumber},canBeStatus={CanBeStatus})", vatNumber, canBeStatus);
    }
}
