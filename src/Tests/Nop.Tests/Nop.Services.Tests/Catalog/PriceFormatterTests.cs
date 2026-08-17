using System.Globalization;
using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Catalog;

[TestFixture]
public class PriceFormatterTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private CurrencySettings _currencySettings;
    private IPriceFormatter _priceFormatter;
    private ISettingService _settingService;

    private int _enLangId;
    private Currency _euro;
    private Currency _dollar;
    private Currency _pound;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _currencySettings = GetService<CurrencySettings>();
        _priceFormatter = GetService<IPriceFormatter>();
		_logger.LogDebug("SetUp(): _priceFormatter → {_priceFormatter}", _priceFormatter);
		_logger.LogDebug("SetUp(): _currencySettings → {_currencySettings}", _currencySettings);
        _settingService = GetService<ISettingService>();
		_logger.LogDebug("SetUp(): _settingService → {_settingService}", _settingService);

        var languageService = GetService<ILanguageService>();
		_logger.LogDebug("SetUp(): languageService → {LanguageService}", languageService);
        var currencyService = GetService<ICurrencyService>();
		_logger.LogDebug("SetUp(): currencyService → {CurrencyService}", currencyService);

        var languages = await languageService.GetAllLanguagesAsync();
		_logger.LogDebug("SetUp(): languages → {Languages}", languages);

        _enLangId = languages.FirstOrDefault(p => p.LanguageCulture == "en-US")?.Id ??
                   languages.FirstOrDefault()?.Id ?? 1;

        _euro = await currencyService.GetCurrencyByCodeAsync("EUR");
		_logger.LogDebug("SetUp(): _euro → {_euro}", _euro);
        _dollar = await currencyService.GetCurrencyByCodeAsync("USD");
		_logger.LogDebug("SetUp(): _dollar → {_dollar}", _dollar);
        _pound = await currencyService.GetCurrencyByCodeAsync("GBP");
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _pound → {_pound}", _pound);
    }

    [Test]
    public async Task CanFormatPriceWithCustomCurrencyFormatting()
    {
		_logger.LogDebug(">>> Entering CanFormatPriceWithCustomCurrencyFormatting()");
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        var formatPrice = await _priceFormatter.FormatPriceAsync(1234.5M, false, _euro, _enLangId, false, false);
		_logger.LogDebug("CanFormatPriceWithCustomCurrencyFormatting(): formatPrice → {FormatPrice}", formatPrice);
        formatPrice.Should().Be("€1234.50");
		_logger.LogDebug("<<< Exiting CanFormatPriceWithCustomCurrencyFormatting()");
    }

    [Test]
    public async Task CanFormatPriceWithDistinctCurrencyDisplayLocale()
    {
		_logger.LogDebug(">>> Entering CanFormatPriceWithDistinctCurrencyDisplayLocale()");
        var formatPrice = await _priceFormatter.FormatPriceAsync(1234.5M, false, _dollar, _enLangId, false, false);
        formatPrice.Should().Be("$1,234.50");
		_logger.LogDebug("CanFormatPriceWithDistinctCurrencyDisplayLocale(): formatPrice → {FormatPrice}", formatPrice);
        formatPrice = await _priceFormatter.FormatPriceAsync(1234.5M, false, _pound, _enLangId, false, false);
        formatPrice.Should().Be("£1,234.50");
		_logger.LogDebug("<<< Exiting CanFormatPriceWithDistinctCurrencyDisplayLocale()");
    }

    [Test]
    public async Task CanFormatPriceWithShowTax()
    {
		_logger.LogDebug(">>> Entering CanFormatPriceWithShowTax()");
        var formatPrice = await _priceFormatter.FormatPriceAsync(1234.5M, false, _dollar, _enLangId, true, true);
        formatPrice.Should().Be("$1,234.50 incl tax");
		_logger.LogDebug("CanFormatPriceWithShowTax(): formatPrice → {FormatPrice}", formatPrice);
        formatPrice = await _priceFormatter.FormatPriceAsync(1234.5M, false, _dollar, _enLangId, false, true);
        formatPrice.Should().Be("$1,234.50 excl tax");
		_logger.LogDebug("<<< Exiting CanFormatPriceWithShowTax()");
    }

    [Test]
    public async Task CanFormatPriceWithShowCurrencyCode()
    {
		_logger.LogDebug(">>> Entering CanFormatPriceWithShowCurrencyCode()");
        _currencySettings.DisplayCurrencyLabel = true;
        await _settingService.SaveSettingAsync(_currencySettings);
		_logger.LogDebug("CanFormatPriceWithShowCurrencyCode(): _currencySettings.DisplayCurrencyLabel → {Arg0}", _currencySettings.DisplayCurrencyLabel);
        //recreate IPriceFormatter to read new currency settings
        var formatPrice = await GetService<IPriceFormatter>().FormatPriceAsync(1234.5M, true, _dollar, _enLangId, false, false);
		_logger.LogDebug("CanFormatPriceWithShowCurrencyCode(): formatPrice → {FormatPrice}", formatPrice);
        formatPrice.Should().Be("$1,234.50 (USD)");

        _currencySettings.DisplayCurrencyLabel = false;
        await _settingService.SaveSettingAsync(_currencySettings);
        //recreate IPriceFormatter to read new currency settings
        formatPrice = await GetService<IPriceFormatter>().FormatPriceAsync(1234.5M, true, _dollar, _enLangId, false, false);
        formatPrice.Should().Be("$1,234.50");
		_logger.LogDebug("<<< Exiting CanFormatPriceWithShowCurrencyCode()");
    }

    [Test]
    public async Task CanFormatWithMinimumAndMaximumDecimalValues()
    {
		_logger.LogDebug(">>> Entering CanFormatWithMinimumAndMaximumDecimalValues()");
        //test with minimum decimal value
        var minDecimalValue = decimal.MinValue;
        var formatMinDecimal = await _priceFormatter.FormatPriceAsync(minDecimalValue);
		_logger.LogDebug("CanFormatWithMinimumAndMaximumDecimalValues(): formatMinDecimal → {FormatMinDecimal}", formatMinDecimal);
        formatMinDecimal.Should().NotBeNull();

        //test with maximum decimal value
        var maxDecimalValue = decimal.MaxValue;
        var formatMaxDecimal = await _priceFormatter.FormatPriceAsync(maxDecimalValue);
		_logger.LogDebug("CanFormatWithMinimumAndMaximumDecimalValues(): formatMaxDecimal → {FormatMaxDecimal}", formatMaxDecimal);
        formatMaxDecimal.Should().NotBeNull();
		_logger.LogDebug("<<< Exiting CanFormatWithMinimumAndMaximumDecimalValues()");
    }

    [Test]
    public async Task CanFormatWithZeroValuesForPrices()
    {
		_logger.LogDebug(">>> Entering CanFormatWithZeroValuesForPrices()");
        //test with zero price
        var zeroPrice = 0M;
        var formatZeroPrice = await _priceFormatter.FormatPriceAsync(zeroPrice);
		_logger.LogDebug("CanFormatWithZeroValuesForPrices(): formatZeroPrice → {FormatZeroPrice}", formatZeroPrice);
        formatZeroPrice.Should().NotBeNull();
		_logger.LogDebug("<<< Exiting CanFormatWithZeroValuesForPrices()");
    }

    [Test]
    public async Task CanHandleNullOrEmptyCurrencyCodes()
    {
		_logger.LogDebug(">>> Entering CanHandleNullOrEmptyCurrencyCodes()");
        //test with null currency code
        string nullCurrencyCode = null;
        var formatNullCurrency = await _priceFormatter.FormatPriceAsync(100M, true, nullCurrencyCode, _enLangId, false);
		_logger.LogDebug("CanHandleNullOrEmptyCurrencyCodes(): formatNullCurrency → {FormatNullCurrency}", formatNullCurrency);
        formatNullCurrency.Should().NotBeNull();

        //test with empty currency code
        var emptyCurrencyCode = string.Empty;
        var formatEmptyCurrency = await _priceFormatter.FormatPriceAsync(100M, true, emptyCurrencyCode, _enLangId, false);
		_logger.LogDebug("CanHandleNullOrEmptyCurrencyCodes(): formatEmptyCurrency → {FormatEmptyCurrency}", formatEmptyCurrency);
        formatEmptyCurrency.Should().NotBeNull();
		_logger.LogDebug("<<< Exiting CanHandleNullOrEmptyCurrencyCodes()");
    }
    [Test]
    public async Task CanFormatRentalProductPriceForDays()
    {
		_logger.LogDebug(">>> Entering CanFormatRentalProductPriceForDays()");
        //create or fetch a rental product with a known price
        var rentalProduct = new Product
        {
            IsRental = true,
            RentalPricePeriod = RentalPricePeriod.Days,
            RentalPriceLength = 5 
        };

        var productPrice = 50M; 
        var formattedRentalPrice = await _priceFormatter.FormatRentalProductPeriodAsync(rentalProduct, productPrice.ToString());
		_logger.LogDebug("CanFormatRentalProductPriceForDays(): formattedRentalPrice → {FormattedRentalPrice}", formattedRentalPrice);

        //assert that the formatted rental price for days is not null or empty
        formattedRentalPrice.Should().NotBeNullOrEmpty();

        //assert that the formatted rental price contains the expected details
        formattedRentalPrice.Should().Contain("50"); 
        formattedRentalPrice.Should().Contain("5"); 
        formattedRentalPrice.Should().Contain("day");
		_logger.LogDebug("<<< Exiting CanFormatRentalProductPriceForDays()");
    }

    [Test]
    public void FormatBasePrice_WhenProductIsNull_ReturnsNull()
    {
		_logger.LogDebug(">>> Entering FormatBasePrice_WhenProductIsNull_ReturnsNull()");
        //arrange
        decimal? productPrice = 10.0M;

        //act and Assert
		_logger.LogDebug("<<< Exiting FormatBasePrice_WhenProductIsNull_ReturnsNull()");
        Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _priceFormatter.FormatBasePriceAsync(null, productPrice)
        );
			_logger.LogDebug(">>> Entering async()");
    }
    [Test]
    public async Task FormatBasePrice_WhenBasePriceNotEnabled_ReturnsNull()
    {
		_logger.LogDebug(">>> Entering FormatBasePrice_WhenBasePriceNotEnabled_ReturnsNull()");
        //arrange
        var product = new Product { BasepriceEnabled = false };
        decimal? productPrice = 10.0M;

        //act
        var result = await _priceFormatter.FormatBasePriceAsync(product, productPrice);
		_logger.LogDebug("FormatBasePrice_WhenBasePriceNotEnabled_ReturnsNull(): result → {Result}", result);

        //assert
        Assert.That(result, Is.Null);
		_logger.LogDebug("<<< Exiting FormatBasePrice_WhenBasePriceNotEnabled_ReturnsNull()");
    }
    [Test]
    public void FormatTaxRate_ReturnsCorrectFormat()
    {
		_logger.LogDebug(">>> Entering FormatTaxRate_ReturnsCorrectFormat()");
        //arrange
        var taxRate = 0.123456m;
        var expectedFormat = "0.123456";
		_logger.LogDebug("FormatTaxRate_ReturnsCorrectFormat(): expectedFormat → {ExpectedFormat}", expectedFormat);

        //act
        var result = _priceFormatter.FormatTaxRate(taxRate);
		_logger.LogDebug("FormatTaxRate_ReturnsCorrectFormat(): result → {Result}", result);

        //assert
        Assert.That(expectedFormat, Is.EqualTo(result));
		_logger.LogDebug("<<< Exiting FormatTaxRate_ReturnsCorrectFormat()");
    }
}