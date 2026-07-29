using System.Globalization;
using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Catalog;

[TestFixture]
public class PriceFormatterTests : ServiceTest
{
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
		logger.LogDebug(">>> Entering SetUp()");
        _currencySettings = GetService<CurrencySettings>();
        _priceFormatter = GetService<IPriceFormatter>();
		logger.LogDebug("SetUp(): _currencySettings → {_currencySettings}", _currencySettings);
		logger.LogDebug("SetUp(): _priceFormatter → {_priceFormatter}", _priceFormatter);
        _settingService = GetService<ISettingService>();

		logger.LogDebug("SetUp(): _settingService → {_settingService}", _settingService);
        var languageService = GetService<ILanguageService>();
        var currencyService = GetService<ICurrencyService>();
		logger.LogDebug("SetUp(): languageService → {LanguageService}", languageService);
		logger.LogDebug("SetUp(): currencyService → {CurrencyService}", currencyService);

        var languages = await languageService.GetAllLanguagesAsync();

		logger.LogDebug("SetUp(): languages → {Languages}", languages);
        _enLangId = languages.FirstOrDefault(p => p.LanguageCulture == "en-US")?.Id ??
                   languages.FirstOrDefault()?.Id ?? 1;

        _euro = await currencyService.GetCurrencyByCodeAsync("EUR");
		logger.LogDebug("SetUp(): _euro → {_euro}", _euro);
        _dollar = await currencyService.GetCurrencyByCodeAsync("USD");
        _pound = await currencyService.GetCurrencyByCodeAsync("GBP");
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _dollar → {_dollar}", _dollar);
		logger.LogDebug("SetUp(): _pound → {_pound}", _pound);
    }

    [Test]
    public async Task CanFormatPriceWithCustomCurrencyFormatting()
    {
		logger.LogDebug(">>> Entering CanFormatPriceWithCustomCurrencyFormatting()");
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        var formatPrice = await _priceFormatter.FormatPriceAsync(1234.5M, false, _euro, _enLangId, false, false);
        formatPrice.Should().Be("€1234.50");
		logger.LogDebug("<<< Exiting CanFormatPriceWithCustomCurrencyFormatting()");
		logger.LogDebug("CanFormatPriceWithCustomCurrencyFormatting(): formatPrice → {FormatPrice}", formatPrice);
    }

    [Test]
    public async Task CanFormatPriceWithDistinctCurrencyDisplayLocale()
    {
		logger.LogDebug(">>> Entering CanFormatPriceWithDistinctCurrencyDisplayLocale()");
        var formatPrice = await _priceFormatter.FormatPriceAsync(1234.5M, false, _dollar, _enLangId, false, false);
        formatPrice.Should().Be("$1,234.50");
		logger.LogDebug("CanFormatPriceWithDistinctCurrencyDisplayLocale(): formatPrice → {FormatPrice}", formatPrice);
        formatPrice = await _priceFormatter.FormatPriceAsync(1234.5M, false, _pound, _enLangId, false, false);
        formatPrice.Should().Be("£1,234.50");
		logger.LogDebug("<<< Exiting CanFormatPriceWithDistinctCurrencyDisplayLocale()");
    }

    [Test]
    public async Task CanFormatPriceWithShowTax()
    {
		logger.LogDebug(">>> Entering CanFormatPriceWithShowTax()");
        var formatPrice = await _priceFormatter.FormatPriceAsync(1234.5M, false, _dollar, _enLangId, true, true);
        formatPrice.Should().Be("$1,234.50 incl tax");
		logger.LogDebug("CanFormatPriceWithShowTax(): formatPrice → {FormatPrice}", formatPrice);
        formatPrice = await _priceFormatter.FormatPriceAsync(1234.5M, false, _dollar, _enLangId, false, true);
        formatPrice.Should().Be("$1,234.50 excl tax");
		logger.LogDebug("<<< Exiting CanFormatPriceWithShowTax()");
    }

    [Test]
    public async Task CanFormatPriceWithShowCurrencyCode()
    {
		logger.LogDebug(">>> Entering CanFormatPriceWithShowCurrencyCode()");
        _currencySettings.DisplayCurrencyLabel = true;
        await _settingService.SaveSettingAsync(_currencySettings);
		logger.LogDebug("CanFormatPriceWithShowCurrencyCode(): _currencySettings.DisplayCurrencyLabel → {Arg0}", _currencySettings.DisplayCurrencyLabel);
        //recreate IPriceFormatter to read new currency settings
        var formatPrice = await GetService<IPriceFormatter>().FormatPriceAsync(1234.5M, true, _dollar, _enLangId, false, false);
        formatPrice.Should().Be("$1,234.50 (USD)");
		logger.LogDebug("CanFormatPriceWithShowCurrencyCode(): formatPrice → {FormatPrice}", formatPrice);

        _currencySettings.DisplayCurrencyLabel = false;
        await _settingService.SaveSettingAsync(_currencySettings);
        //recreate IPriceFormatter to read new currency settings
        formatPrice = await GetService<IPriceFormatter>().FormatPriceAsync(1234.5M, true, _dollar, _enLangId, false, false);
        formatPrice.Should().Be("$1,234.50");
		logger.LogDebug("<<< Exiting CanFormatPriceWithShowCurrencyCode()");
    }

    [Test]
    public async Task CanFormatWithMinimumAndMaximumDecimalValues()
    {
		logger.LogDebug(">>> Entering CanFormatWithMinimumAndMaximumDecimalValues()");
        //test with minimum decimal value
        var minDecimalValue = decimal.MinValue;
        var formatMinDecimal = await _priceFormatter.FormatPriceAsync(minDecimalValue);
        formatMinDecimal.Should().NotBeNull();
		logger.LogDebug("CanFormatWithMinimumAndMaximumDecimalValues(): formatMinDecimal → {FormatMinDecimal}", formatMinDecimal);

        //test with maximum decimal value
        var maxDecimalValue = decimal.MaxValue;
        var formatMaxDecimal = await _priceFormatter.FormatPriceAsync(maxDecimalValue);
        formatMaxDecimal.Should().NotBeNull();
		logger.LogDebug("<<< Exiting CanFormatWithMinimumAndMaximumDecimalValues()");
		logger.LogDebug("CanFormatWithMinimumAndMaximumDecimalValues(): formatMaxDecimal → {FormatMaxDecimal}", formatMaxDecimal);
    }

    [Test]
    public async Task CanFormatWithZeroValuesForPrices()
    {
		logger.LogDebug(">>> Entering CanFormatWithZeroValuesForPrices()");
        //test with zero price
        var zeroPrice = 0M;
        var formatZeroPrice = await _priceFormatter.FormatPriceAsync(zeroPrice);
        formatZeroPrice.Should().NotBeNull();
		logger.LogDebug("<<< Exiting CanFormatWithZeroValuesForPrices()");
		logger.LogDebug("CanFormatWithZeroValuesForPrices(): formatZeroPrice → {FormatZeroPrice}", formatZeroPrice);
    }

    [Test]
    public async Task CanHandleNullOrEmptyCurrencyCodes()
    {
		logger.LogDebug(">>> Entering CanHandleNullOrEmptyCurrencyCodes()");
        //test with null currency code
        string nullCurrencyCode = null;
        var formatNullCurrency = await _priceFormatter.FormatPriceAsync(100M, true, nullCurrencyCode, _enLangId, false);
        formatNullCurrency.Should().NotBeNull();
		logger.LogDebug("CanHandleNullOrEmptyCurrencyCodes(): formatNullCurrency → {FormatNullCurrency}", formatNullCurrency);

        //test with empty currency code
        var emptyCurrencyCode = string.Empty;
        var formatEmptyCurrency = await _priceFormatter.FormatPriceAsync(100M, true, emptyCurrencyCode, _enLangId, false);
        formatEmptyCurrency.Should().NotBeNull();
		logger.LogDebug("<<< Exiting CanHandleNullOrEmptyCurrencyCodes()");
		logger.LogDebug("CanHandleNullOrEmptyCurrencyCodes(): formatEmptyCurrency → {FormatEmptyCurrency}", formatEmptyCurrency);
    }
    [Test]
    public async Task CanFormatRentalProductPriceForDays()
    {
		logger.LogDebug(">>> Entering CanFormatRentalProductPriceForDays()");
        //create or fetch a rental product with a known price
        var rentalProduct = new Product
        {
            IsRental = true,
            RentalPricePeriod = RentalPricePeriod.Days,
            RentalPriceLength = 5 
        };

        var productPrice = 50M; 
        var formattedRentalPrice = await _priceFormatter.FormatRentalProductPeriodAsync(rentalProduct, productPrice.ToString());

		logger.LogDebug("CanFormatRentalProductPriceForDays(): formattedRentalPrice → {FormattedRentalPrice}", formattedRentalPrice);
        //assert that the formatted rental price for days is not null or empty
        formattedRentalPrice.Should().NotBeNullOrEmpty();

        //assert that the formatted rental price contains the expected details
        formattedRentalPrice.Should().Contain("50"); 
        formattedRentalPrice.Should().Contain("5"); 
        formattedRentalPrice.Should().Contain("day");
		logger.LogDebug("<<< Exiting CanFormatRentalProductPriceForDays()");
    }

    [Test]
    public void FormatBasePrice_WhenProductIsNull_ReturnsNull()
    {
		logger.LogDebug(">>> Entering FormatBasePrice_WhenProductIsNull_ReturnsNull()");
        //arrange
        decimal? productPrice = 10.0M;

        //act and Assert
		logger.LogDebug("<<< Exiting FormatBasePrice_WhenProductIsNull_ReturnsNull()");
        Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _priceFormatter.FormatBasePriceAsync(null, productPrice)
        );
			logger.LogDebug(">>> Entering async()");
    }
    [Test]
    public async Task FormatBasePrice_WhenBasePriceNotEnabled_ReturnsNull()
    {
		logger.LogDebug(">>> Entering FormatBasePrice_WhenBasePriceNotEnabled_ReturnsNull()");
        //arrange
        var product = new Product { BasepriceEnabled = false };
        decimal? productPrice = 10.0M;

        //act
        var result = await _priceFormatter.FormatBasePriceAsync(product, productPrice);

		logger.LogDebug("FormatBasePrice_WhenBasePriceNotEnabled_ReturnsNull(): result → {Result}", result);
        //assert
        Assert.That(result, Is.Null);
		logger.LogDebug("<<< Exiting FormatBasePrice_WhenBasePriceNotEnabled_ReturnsNull()");
    }
    [Test]
    public void FormatTaxRate_ReturnsCorrectFormat()
    {
		logger.LogDebug(">>> Entering FormatTaxRate_ReturnsCorrectFormat()");
        //arrange
        var taxRate = 0.123456m;
        var expectedFormat = "0.123456";

		logger.LogDebug("FormatTaxRate_ReturnsCorrectFormat(): expectedFormat → {ExpectedFormat}", expectedFormat);
        //act
        var result = _priceFormatter.FormatTaxRate(taxRate);

		logger.LogDebug("FormatTaxRate_ReturnsCorrectFormat(): result → {Result}", result);
        //assert
        Assert.That(expectedFormat, Is.EqualTo(result));
		logger.LogDebug("<<< Exiting FormatTaxRate_ReturnsCorrectFormat()");
    }
}