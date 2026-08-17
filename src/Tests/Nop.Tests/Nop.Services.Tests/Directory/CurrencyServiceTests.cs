using FluentAssertions;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Directory;

[TestFixture]
public class CurrencyServiceTests : ServiceTest<Currency>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ICurrencyService _currencyService;
    private IExchangeRatePluginManager _exchangeRatePluginManager;

    private Currency _currencyUsd, _currencyRur, _currencyEur;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _currencyService = GetService<ICurrencyService>();
        _exchangeRatePluginManager = GetService<IExchangeRatePluginManager>();
		_logger.LogDebug("SetUp(): _exchangeRatePluginManager → {_exchangeRatePluginManager}", _exchangeRatePluginManager);
		_logger.LogDebug("SetUp(): _currencyService → {_currencyService}", _currencyService);

        _currencyEur = await _currencyService.GetCurrencyByCodeAsync("EUR");
		_logger.LogDebug("SetUp(): _currencyEur → {_currencyEur}", _currencyEur);
        _currencyUsd = await _currencyService.GetCurrencyByCodeAsync("USD");
		_logger.LogDebug("SetUp(): _currencyUsd → {_currencyUsd}", _currencyUsd);
        _currencyRur = await _currencyService.GetCurrencyByCodeAsync("RUB");
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _currencyRur → {_currencyRur}", _currencyRur);
    }

    [Test]
    public async Task CanLoadExchangeRateProviders()
    {
		_logger.LogDebug(">>> Entering CanLoadExchangeRateProviders()");
        var providers = await _exchangeRatePluginManager.LoadAllPluginsAsync();
        providers.Should().NotBeNull();
		_logger.LogDebug("CanLoadExchangeRateProviders(): providers → {Providers}", providers);
        providers.Any().Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanLoadExchangeRateProviders()");
    }

    [Test]
    public async Task CanLoadExchangeRateProviderBySystemKeyword()
    {
		_logger.LogDebug(">>> Entering CanLoadExchangeRateProviderBySystemKeyword()");
        var provider = await _exchangeRatePluginManager.LoadPluginBySystemNameAsync("CurrencyExchange.TestProvider");
        provider.Should().NotBeNull();
		_logger.LogDebug("<<< Exiting CanLoadExchangeRateProviderBySystemKeyword()");
		_logger.LogDebug("CanLoadExchangeRateProviderBySystemKeyword(): provider → {Provider}", provider);
    }

    [Test]
    public async Task CanLoadActiveExchangeRateProvider()
    {
		_logger.LogDebug(">>> Entering CanLoadActiveExchangeRateProvider()");
        var provider = await _exchangeRatePluginManager.LoadPrimaryPluginAsync();
        provider.Should().NotBeNull();
		_logger.LogDebug("<<< Exiting CanLoadActiveExchangeRateProvider()");
		_logger.LogDebug("CanLoadActiveExchangeRateProvider(): provider → {Provider}", provider);
    }

    [Test]
    public void CanConvertCurrency()
    {
		_logger.LogDebug(">>> Entering CanConvertCurrency()");
        _currencyService.ConvertCurrency(10.1M, 1.5M).Should().Be(15.15M);
        _currencyService.ConvertCurrency(10.1M, 1).Should().Be(10.1M);
        _currencyService.ConvertCurrency(10.1M, 0).Should().Be(0);
        _currencyService.ConvertCurrency(0, 5).Should().Be(0);
		_logger.LogDebug("<<< Exiting CanConvertCurrency()");
    }

    [Test]
    public async Task CanConvertToPrimaryStoreCurrency()
    {
		_logger.LogDebug(">>> Entering CanConvertToPrimaryStoreCurrency()");
        var newCurrency = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(8.686M, _currencyEur);
        newCurrency.Should().Be(10.1M);
		_logger.LogDebug("CanConvertToPrimaryStoreCurrency(): newCurrency → {NewCurrency}", newCurrency);
        newCurrency = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(638.825M, _currencyRur);
        newCurrency.Should().Be(10.1M);
        newCurrency = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(759M, _currencyUsd);
        newCurrency.Should().Be(759M);
		_logger.LogDebug("<<< Exiting CanConvertToPrimaryStoreCurrency()");
    }

    [Test]
    public async Task CanConvertFromPrimaryStoreCurrency()
    {
		_logger.LogDebug(">>> Entering CanConvertFromPrimaryStoreCurrency()");
        var newCurrency = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(10.1M, _currencyEur);
        newCurrency.Should().Be(8.686M);
		_logger.LogDebug("CanConvertFromPrimaryStoreCurrency(): newCurrency → {NewCurrency}", newCurrency);
        newCurrency = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(10.1M, _currencyRur);
        newCurrency.Should().Be(638.825M);
        newCurrency = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(759M, _currencyUsd);
        newCurrency.Should().Be(759M);
		_logger.LogDebug("<<< Exiting CanConvertFromPrimaryStoreCurrency()");
    }

    protected override CrudData<Currency> CrudData
    {
        get
        {
            var insertItem = new Currency
            {
                Name = "Test name",
                CurrencyCode = "tn"
            };

            var updateItem = new Currency
            {
                Name = "Test name 1",
                CurrencyCode = "tn"
            };

            return new CrudData<Currency>
            {
                BaseEntity = insertItem,
                UpdatedEntity = updateItem,
                Insert = _currencyService.InsertCurrencyAsync,
                Update = _currencyService.UpdateCurrencyAsync,
                GetById = _currencyService.GetCurrencyByIdAsync,
                IsEqual = (item, other) => item.Name.Equals(other.Name),
                Delete = _currencyService.DeleteCurrencyAsync
            };
        }
    }
}