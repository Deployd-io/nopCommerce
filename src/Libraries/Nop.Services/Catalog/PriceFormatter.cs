using System.Globalization;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Tax;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog;

/// <summary>
/// Price formatter
/// </summary>
public partial class PriceFormatter : IPriceFormatter
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly ICurrencyService _currencyService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IMeasureService _measureService;
    protected readonly IPriceCalculationService _priceCalculationService;
    protected readonly IWorkContext _workContext;
    protected readonly TaxSettings _taxSettings;

    #endregion

    #region Ctor

    public PriceFormatter(CurrencySettings currencySettings,
        ICurrencyService currencyService,
        ILocalizationService localizationService,
        IMeasureService measureService,
        IPriceCalculationService priceCalculationService,
        IWorkContext workContext,
        TaxSettings taxSettings)
    {
		_logger.LogDebug(">>> Entering PriceFormatter(currencySettings={CurrencySettings},currencyService={CurrencyService},localizationService={LocalizationService},measureService={MeasureService},priceCalculationService={PriceCalculationService},workContext={WorkContext},taxSettings={TaxSettings})", currencySettings, currencyService, localizationService, measureService, priceCalculationService, workContext, taxSettings);
        _currencySettings = currencySettings;
		_logger.LogDebug("PriceFormatter(currencySettings={CurrencySettings},currencyService={CurrencyService},localizationService={LocalizationService},measureService={MeasureService},priceCalculationService={PriceCalculationService},workContext={WorkContext},taxSettings={TaxSettings}): _currencySettings → {_currencySettings}", currencySettings, currencyService, localizationService, measureService, priceCalculationService, workContext, taxSettings, _currencySettings);
        _currencyService = currencyService;
		_logger.LogDebug("PriceFormatter(currencySettings={CurrencySettings},currencyService={CurrencyService},localizationService={LocalizationService},measureService={MeasureService},priceCalculationService={PriceCalculationService},workContext={WorkContext},taxSettings={TaxSettings}): _currencyService → {_currencyService}", currencySettings, currencyService, localizationService, measureService, priceCalculationService, workContext, taxSettings, _currencyService);
        _localizationService = localizationService;
		_logger.LogDebug("PriceFormatter(currencySettings={CurrencySettings},currencyService={CurrencyService},localizationService={LocalizationService},measureService={MeasureService},priceCalculationService={PriceCalculationService},workContext={WorkContext},taxSettings={TaxSettings}): _localizationService → {_localizationService}", currencySettings, currencyService, localizationService, measureService, priceCalculationService, workContext, taxSettings, _localizationService);
        _measureService = measureService;
		_logger.LogDebug("PriceFormatter(currencySettings={CurrencySettings},currencyService={CurrencyService},localizationService={LocalizationService},measureService={MeasureService},priceCalculationService={PriceCalculationService},workContext={WorkContext},taxSettings={TaxSettings}): _measureService → {_measureService}", currencySettings, currencyService, localizationService, measureService, priceCalculationService, workContext, taxSettings, _measureService);
        _priceCalculationService = priceCalculationService;
		_logger.LogDebug("PriceFormatter(currencySettings={CurrencySettings},currencyService={CurrencyService},localizationService={LocalizationService},measureService={MeasureService},priceCalculationService={PriceCalculationService},workContext={WorkContext},taxSettings={TaxSettings}): _priceCalculationService → {_priceCalculationService}", currencySettings, currencyService, localizationService, measureService, priceCalculationService, workContext, taxSettings, _priceCalculationService);
        _workContext = workContext;
		_logger.LogDebug("PriceFormatter(currencySettings={CurrencySettings},currencyService={CurrencyService},localizationService={LocalizationService},measureService={MeasureService},priceCalculationService={PriceCalculationService},workContext={WorkContext},taxSettings={TaxSettings}): _workContext → {_workContext}", currencySettings, currencyService, localizationService, measureService, priceCalculationService, workContext, taxSettings, _workContext);
        _taxSettings = taxSettings;
		_logger.LogDebug("PriceFormatter(currencySettings={CurrencySettings},currencyService={CurrencyService},localizationService={LocalizationService},measureService={MeasureService},priceCalculationService={PriceCalculationService},workContext={WorkContext},taxSettings={TaxSettings}): _taxSettings → {_taxSettings}", currencySettings, currencyService, localizationService, measureService, priceCalculationService, workContext, taxSettings, _taxSettings);
		_logger.LogDebug("<<< Exiting PriceFormatter(currencySettings={CurrencySettings},currencyService={CurrencyService},localizationService={LocalizationService},measureService={MeasureService},priceCalculationService={PriceCalculationService},workContext={WorkContext},taxSettings={TaxSettings})", currencySettings, currencyService, localizationService, measureService, priceCalculationService, workContext, taxSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets currency string
    /// </summary>
    /// <param name="amount">Amount</param>
    /// <param name="showCurrency">A value indicating whether to show a currency</param>
    /// <param name="targetCurrency">Target currency</param>
    /// <returns>Currency string without exchange rate</returns>
    protected virtual string GetCurrencyString(decimal amount,
        bool showCurrency, Currency targetCurrency)
    {
		_logger.LogDebug(">>> Entering GetCurrencyString(amount={Amount},showCurrency={ShowCurrency},targetCurrency={TargetCurrency})", amount, showCurrency, targetCurrency);
        ArgumentNullException.ThrowIfNull(targetCurrency);

        string result;
        if (!string.IsNullOrEmpty(targetCurrency.CustomFormatting))
            //custom formatting specified by a store owner
            result = amount.ToString(targetCurrency.CustomFormatting);
			_logger.LogDebug("GetCurrencyString(amount={Amount},showCurrency={ShowCurrency},targetCurrency={TargetCurrency}): result → {Result}", amount, showCurrency, targetCurrency, result);
        else
        {
            if (!string.IsNullOrEmpty(targetCurrency.DisplayLocale))
                //default behavior
                result = amount.ToString("C", new CultureInfo(targetCurrency.DisplayLocale));
            else
            {
                //not possible because "DisplayLocale" should be always specified
                //but anyway let's just handle this behavior
                result = $"{amount:N} ({targetCurrency.CurrencyCode})";
                return result;
            }
        }

        //display currency code?
        if (showCurrency && _currencySettings.DisplayCurrencyLabel)
            result = $"{result} ({targetCurrency.CurrencyCode})";
		_logger.LogDebug("<<< Exiting GetCurrencyString(amount={Amount},showCurrency={ShowCurrency},targetCurrency={TargetCurrency})", amount, showCurrency, targetCurrency);
        return result;
    }

    /// <summary>
    /// Formats the shipping price
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="showCurrency">A value indicating whether to show a currency</param>
    /// <param name="targetCurrency">Target currency</param>
    /// <param name="languageId">Language</param>
    /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
    /// <param name="showTax">A value indicating whether to show tax suffix</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price
    /// </returns>
    protected virtual async Task<string> FormatShippingPriceAsync(decimal price, bool showCurrency,
        Currency targetCurrency, int languageId, bool priceIncludesTax, bool showTax)
    {
		_logger.LogDebug("<<< Exiting FormatShippingPriceAsync(price={Price},showCurrency={ShowCurrency})", price, showCurrency);
        return await FormatPriceAsync(price, showCurrency, targetCurrency, languageId, priceIncludesTax, showTax);
    }

    /// <summary>
    /// Formats the payment method additional fee
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="showCurrency">A value indicating whether to show a currency</param>
    /// <param name="targetCurrency">Target currency</param>
    /// <param name="languageId">Language</param>
    /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
    /// <param name="showTax">A value indicating whether to show tax suffix</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price
    /// </returns>
    protected virtual async Task<string> FormatPaymentMethodAdditionalFeeAsync(decimal price, bool showCurrency,
        Currency targetCurrency, int languageId, bool priceIncludesTax, bool showTax)
    {
		_logger.LogDebug(">>> Entering FormatPaymentMethodAdditionalFeeAsync(price={Price},showCurrency={ShowCurrency})", price, showCurrency);
		_logger.LogDebug("<<< Exiting FormatPaymentMethodAdditionalFeeAsync(price={Price},showCurrency={ShowCurrency})", price, showCurrency);
        return await FormatPriceAsync(price, showCurrency, targetCurrency, languageId,
            priceIncludesTax, showTax);
			_logger.LogDebug(">>> Entering FormatPriceAsync(price={Price})", price);
			_logger.LogDebug("<<< Exiting FormatPriceAsync(price={Price})", price);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Formats the price
    /// </summary>
    /// <param name="price">Price</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price
    /// </returns>
    public virtual async Task<string> FormatPriceAsync(decimal price)
    {
		_logger.LogDebug(">>> Entering FormatPriceAsync(price={Price})", price);
		_logger.LogDebug("<<< Exiting FormatPriceAsync(price={Price})", price);
        return await FormatPriceAsync(price, true, await _workContext.GetWorkingCurrencyAsync());
    }

    /// <summary>
    /// Formats the price
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="showCurrency">A value indicating whether to show a currency</param>
    /// <param name="targetCurrency">Target currency</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price
    /// </returns>
    public virtual async Task<string> FormatPriceAsync(decimal price, bool showCurrency, Currency targetCurrency)
    {
        var priceIncludesTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;
		_logger.LogDebug("FormatPriceAsync(price={Price},showCurrency={ShowCurrency},targetCurrency={TargetCurrency}): priceIncludesTax → {PriceIncludesTax}", price, showCurrency, targetCurrency, priceIncludesTax);
		_logger.LogDebug("<<< Exiting FormatPriceAsync(price={Price},showCurrency={ShowCurrency},targetCurrency={TargetCurrency})", price, showCurrency, targetCurrency);
        return await FormatPriceAsync(price, showCurrency, targetCurrency, (await _workContext.GetWorkingLanguageAsync()).Id, priceIncludesTax);
    }

    /// <summary>
    /// Formats the price
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="showCurrency">A value indicating whether to show a currency</param>
    /// <param name="showTax">A value indicating whether to show tax suffix</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price
    /// </returns>
    public virtual async Task<string> FormatPriceAsync(decimal price, bool showCurrency, bool showTax)
    {
        var priceIncludesTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;
		_logger.LogDebug("FormatPriceAsync(price={Price},showCurrency={ShowCurrency},showTax={ShowTax}): priceIncludesTax → {PriceIncludesTax}", price, showCurrency, showTax, priceIncludesTax);
		_logger.LogDebug("<<< Exiting FormatPriceAsync(price={Price},showCurrency={ShowCurrency},showTax={ShowTax})", price, showCurrency, showTax);
        return await FormatPriceAsync(price, showCurrency, await _workContext.GetWorkingCurrencyAsync(), (await _workContext.GetWorkingLanguageAsync()).Id, priceIncludesTax, showTax);
    }

    /// <summary>
    /// Formats the price
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="showCurrency">A value indicating whether to show a currency</param>
    /// <param name="currencyCode">Currency code</param>
    /// <param name="showTax">A value indicating whether to show tax suffix</param>
    /// <param name="languageId">Language</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price
    /// </returns>
    public virtual async Task<string> FormatPriceAsync(decimal price, bool showCurrency,
        string currencyCode, bool showTax, int languageId)
    {
        var currency = await _currencyService.GetCurrencyByCodeAsync(currencyCode) ?? new Currency
        {
            CurrencyCode = currencyCode
        };

        var priceIncludesTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;
		_logger.LogDebug("<<< Exiting FormatPriceAsync(price={Price},showCurrency={ShowCurrency},currencyCode={CurrencyCode},showTax={ShowTax},languageId={LanguageId})", price, showCurrency, currencyCode, showTax, languageId);
        return await FormatPriceAsync(price, showCurrency, currency, languageId, priceIncludesTax, showTax);
    }

    /// <summary>
    /// Formats the order price
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="currencyRate">Currency rate</param>
    /// <param name="customerCurrencyCode">Customer currency code</param>
    /// <param name="displayCustomerCurrency">A value indicating whether to display price on customer currency</param>
    /// <param name="primaryStoreCurrency">Primary store currency</param>
    /// <param name="languageId">Language</param>
    /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
    /// <param name="showTax">A value indicating whether to show tax suffix</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price
    /// </returns>
    public virtual async Task<string> FormatOrderPriceAsync(decimal price,
        decimal currencyRate, string customerCurrencyCode, bool displayCustomerCurrency,
        Currency primaryStoreCurrency, int languageId, bool? priceIncludesTax = null, bool? showTax = null)
    {
		_logger.LogDebug(">>> Entering FormatOrderPriceAsync(price={Price},currencyRate={CurrencyRate},customerCurrencyCode={CustomerCurrencyCode},displayCustomerCurrency={DisplayCustomerCurrency},primaryStoreCurrency={PrimaryStoreCurrency},languageId={LanguageId},priceIncludesTax={PriceIncludesTax},showTax={ShowTax})", price, currencyRate, customerCurrencyCode, displayCustomerCurrency, primaryStoreCurrency, languageId, priceIncludesTax, showTax);
        var needAddPriceOnCustomerCurrency = primaryStoreCurrency.CurrencyCode != customerCurrencyCode && displayCustomerCurrency;
        var includesTax = priceIncludesTax ?? await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;
		_logger.LogDebug("FormatOrderPriceAsync(price={Price},currencyRate={CurrencyRate},customerCurrencyCode={CustomerCurrencyCode},displayCustomerCurrency={DisplayCustomerCurrency},primaryStoreCurrency={PrimaryStoreCurrency},languageId={LanguageId},priceIncludesTax={PriceIncludesTax},showTax={ShowTax}): includesTax → {IncludesTax}", price, currencyRate, customerCurrencyCode, displayCustomerCurrency, primaryStoreCurrency, languageId, priceIncludesTax, showTax, includesTax);
        var priceText = await FormatPriceAsync(price, true, primaryStoreCurrency,
            languageId, includesTax, showTax ?? _taxSettings.DisplayTaxSuffix);

        if (!needAddPriceOnCustomerCurrency || await _currencyService.GetCurrencyByCodeAsync(customerCurrencyCode) is not Currency currency)
            return priceText;

        var customerPrice = _currencyService.ConvertCurrency(price, currencyRate);
		_logger.LogDebug("FormatOrderPriceAsync(price={Price},currencyRate={CurrencyRate},customerCurrencyCode={CustomerCurrencyCode},displayCustomerCurrency={DisplayCustomerCurrency},primaryStoreCurrency={PrimaryStoreCurrency},languageId={LanguageId},priceIncludesTax={PriceIncludesTax},showTax={ShowTax}): customerPrice → {CustomerPrice}", price, currencyRate, customerCurrencyCode, displayCustomerCurrency, primaryStoreCurrency, languageId, priceIncludesTax, showTax, customerPrice);
        var customerPriceText = await FormatPriceAsync(customerPrice, true, currency,
            languageId, includesTax, showTax ?? _taxSettings.DisplayTaxSuffix);
        priceText += $"<br />[{customerPriceText}]";

		_logger.LogDebug("<<< Exiting FormatOrderPriceAsync(price={Price},currencyRate={CurrencyRate},customerCurrencyCode={CustomerCurrencyCode},displayCustomerCurrency={DisplayCustomerCurrency},primaryStoreCurrency={PrimaryStoreCurrency},languageId={LanguageId},priceIncludesTax={PriceIncludesTax},showTax={ShowTax})", price, currencyRate, customerCurrencyCode, displayCustomerCurrency, primaryStoreCurrency, languageId, priceIncludesTax, showTax);
        return priceText;
    }

    /// <summary>
    /// Formats the price
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="showCurrency">A value indicating whether to show a currency</param>
    /// <param name="currencyCode">Currency code</param>
    /// <param name="languageId">Language</param>
    /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price
    /// </returns>
    public virtual async Task<string> FormatPriceAsync(decimal price, bool showCurrency,
        string currencyCode, int languageId, bool priceIncludesTax)
    {
        var currency = await _currencyService.GetCurrencyByCodeAsync(currencyCode)
                       ?? new Currency
                       {
                           CurrencyCode = currencyCode
                       };
        return await FormatPriceAsync(price, showCurrency, currency, languageId, priceIncludesTax);
    }

    /// <summary>
    /// Formats the price
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="showCurrency">A value indicating whether to show a currency</param>
    /// <param name="targetCurrency">Target currency</param>
    /// <param name="languageId">Language</param>
    /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price
    /// </returns>
    public virtual async Task<string> FormatPriceAsync(decimal price, bool showCurrency,
        Currency targetCurrency, int languageId, bool priceIncludesTax)
    {
        return await FormatPriceAsync(price, showCurrency, targetCurrency, languageId,
            priceIncludesTax, _taxSettings.DisplayTaxSuffix);
    }

    /// <summary>
    /// Formats the price
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="showCurrency">A value indicating whether to show a currency</param>
    /// <param name="targetCurrency">Target currency</param>
    /// <param name="languageId">Language</param>
    /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
    /// <param name="showTax">A value indicating whether to show tax suffix</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price
    /// </returns>
    public virtual async Task<string> FormatPriceAsync(decimal price, bool showCurrency,
        Currency targetCurrency, int languageId, bool priceIncludesTax, bool showTax)
    {
        //we should round it no matter of "ShoppingCartSettings.RoundPricesDuringCalculation" setting
        price = await _priceCalculationService.RoundPriceAsync(price, targetCurrency);

        var currencyString = GetCurrencyString(price, showCurrency, targetCurrency);
        if (!showTax)
            return currencyString;

        //show tax suffix
        string formatStr;
        if (priceIncludesTax)
        {
            formatStr = await _localizationService.GetResourceAsync("Products.InclTaxSuffix", languageId, false);
            if (string.IsNullOrEmpty(formatStr))
                formatStr = "{0} incl tax";
        }
        else
        {
            formatStr = await _localizationService.GetResourceAsync("Products.ExclTaxSuffix", languageId, false);
            if (string.IsNullOrEmpty(formatStr))
                formatStr = "{0} excl tax";
        }

        return string.Format(formatStr, currencyString);
    }

    /// <summary>
    /// Formats the price of rental product (with rental period)
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="price">Price</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the rental product price with period
    /// </returns>
    public virtual async Task<string> FormatRentalProductPeriodAsync(Product product, string price)
    {
		_logger.LogDebug(">>> Entering FormatRentalProductPeriodAsync(product={Product},price={Price})", product, price);
        ArgumentNullException.ThrowIfNull(product);

        if (!product.IsRental)
            return price;

        if (string.IsNullOrWhiteSpace(price))
            return price;
        var result = product.RentalPricePeriod switch
        {
            RentalPricePeriod.Days => string.Format(await _localizationService.GetResourceAsync("Products.Price.Rental.Days"), price, product.RentalPriceLength),
            RentalPricePeriod.Weeks => string.Format(await _localizationService.GetResourceAsync("Products.Price.Rental.Weeks"), price, product.RentalPriceLength),
            RentalPricePeriod.Months => string.Format(await _localizationService.GetResourceAsync("Products.Price.Rental.Months"), price, product.RentalPriceLength),
            RentalPricePeriod.Years => string.Format(await _localizationService.GetResourceAsync("Products.Price.Rental.Years"), price, product.RentalPriceLength),
            _ => throw new NopException("Not supported rental period"),
        };
		_logger.LogDebug("<<< Exiting FormatRentalProductPeriodAsync(product={Product},price={Price})", product, price);
        return result;
    }

    /// <summary>
    /// Formats the shipping price
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="showCurrency">A value indicating whether to show a currency</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price
    /// </returns>
    public virtual async Task<string> FormatShippingPriceAsync(decimal price, bool showCurrency)
    {
        var priceIncludesTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;
		_logger.LogDebug("FormatShippingPriceAsync(price={Price},showCurrency={ShowCurrency}): priceIncludesTax → {PriceIncludesTax}", price, showCurrency, priceIncludesTax);
        return await FormatShippingPriceAsync(price, showCurrency, await _workContext.GetWorkingCurrencyAsync(), (await _workContext.GetWorkingLanguageAsync()).Id, priceIncludesTax);
    }

    /// <summary>
    /// Formats the shipping price
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="showCurrency">A value indicating whether to show a currency</param>
    /// <param name="targetCurrency">Target currency</param>
    /// <param name="languageId">Language</param>
    /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price
    /// </returns>
    public virtual async Task<string> FormatShippingPriceAsync(decimal price, bool showCurrency,
        Currency targetCurrency, int languageId, bool priceIncludesTax)
    {
        var showTax = _taxSettings.ShippingIsTaxable && _taxSettings.DisplayTaxSuffix;
        return await FormatShippingPriceAsync(price, showCurrency, targetCurrency, languageId, priceIncludesTax, showTax);
    }

    /// <summary>
    /// Formats the shipping price
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="showCurrency">A value indicating whether to show a currency</param>
    /// <param name="currencyCode">Currency code</param>
    /// <param name="languageId">Language</param>
    /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price
    /// </returns>
    public virtual async Task<string> FormatShippingPriceAsync(decimal price, bool showCurrency,
        string currencyCode, int languageId, bool priceIncludesTax)
    {
        var currency = await _currencyService.GetCurrencyByCodeAsync(currencyCode)
                       ?? new Currency
                       {
                           CurrencyCode = currencyCode
                       };
        return await FormatShippingPriceAsync(price, showCurrency, currency, languageId, priceIncludesTax);
    }

    /// <summary>
    /// Formats the payment method additional fee
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="showCurrency">A value indicating whether to show a currency</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price
    /// </returns>
    public virtual async Task<string> FormatPaymentMethodAdditionalFeeAsync(decimal price, bool showCurrency)
    {
        var priceIncludesTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;

        return await FormatPaymentMethodAdditionalFeeAsync(price, showCurrency, await _workContext.GetWorkingCurrencyAsync(),
            (await _workContext.GetWorkingLanguageAsync()).Id, priceIncludesTax);
    }

    /// <summary>
    /// Formats the payment method additional fee
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="showCurrency">A value indicating whether to show a currency</param>
    /// <param name="targetCurrency">Target currency</param>
    /// <param name="languageId">Language</param>
    /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price
    /// </returns>
    public virtual async Task<string> FormatPaymentMethodAdditionalFeeAsync(decimal price, bool showCurrency,
        Currency targetCurrency, int languageId, bool priceIncludesTax)
    {
        var showTax = _taxSettings.PaymentMethodAdditionalFeeIsTaxable && _taxSettings.DisplayTaxSuffix;
        return await FormatPaymentMethodAdditionalFeeAsync(price, showCurrency, targetCurrency, languageId, priceIncludesTax, showTax);
    }

    /// <summary>
    /// Formats the payment method additional fee
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="showCurrency">A value indicating whether to show a currency</param>
    /// <param name="currencyCode">Currency code</param>
    /// <param name="languageId">Language</param>
    /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price
    /// </returns>
    public virtual async Task<string> FormatPaymentMethodAdditionalFeeAsync(decimal price, bool showCurrency,
        string currencyCode, int languageId, bool priceIncludesTax)
    {
        var currency = await _currencyService.GetCurrencyByCodeAsync(currencyCode)
                       ?? new Currency
                       {
                           CurrencyCode = currencyCode
                       };
        return await FormatPaymentMethodAdditionalFeeAsync(price, showCurrency, currency,
            languageId, priceIncludesTax);
    }

    /// <summary>
    /// Formats a tax rate
    /// </summary>
    /// <param name="taxRate">Tax rate</param>
    /// <returns>Formatted tax rate</returns>
    public virtual string FormatTaxRate(decimal taxRate)
    {
		_logger.LogDebug(">>> Entering FormatTaxRate(taxRate={TaxRate})", taxRate);
		_logger.LogDebug("<<< Exiting FormatTaxRate(taxRate={TaxRate})", taxRate);
        return taxRate.ToString("G29", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Format base price (PAngV)
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="productPrice">Product price (in primary currency). Pass null if you want to use a default produce price</param>
    /// <param name="totalWeight">Total weight of product (with attribute weight adjustment). Pass null if you want to use a default produce weight</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the base price
    /// </returns>
    public virtual async Task<string> FormatBasePriceAsync(Product product, decimal? productPrice, decimal? totalWeight = null)
    {
		_logger.LogDebug(">>> Entering FormatBasePriceAsync(product={Product},productPrice={ProductPrice},totalWeight={TotalWeight})", product, productPrice, totalWeight);
        ArgumentNullException.ThrowIfNull(product);

        if (!product.BasepriceEnabled)
            return null;

        var productAmount = totalWeight.HasValue && totalWeight.Value > decimal.Zero ? totalWeight.Value : product.BasepriceAmount;
        //Amount in product cannot be 0
        if (productAmount == 0)
            return null;
        var referenceAmount = product.BasepriceBaseAmount;
        var productUnit = await _measureService.GetMeasureWeightByIdAsync(product.BasepriceUnitId);
		_logger.LogDebug("FormatBasePriceAsync(product={Product},productPrice={ProductPrice},totalWeight={TotalWeight}): productUnit → {ProductUnit}", product, productPrice, totalWeight, productUnit);
        //measure weight cannot be loaded
        if (productUnit == null)
            return null;
        var referenceUnit = await _measureService.GetMeasureWeightByIdAsync(product.BasepriceBaseUnitId);
		_logger.LogDebug("FormatBasePriceAsync(product={Product},productPrice={ProductPrice},totalWeight={TotalWeight}): referenceUnit → {ReferenceUnit}", product, productPrice, totalWeight, referenceUnit);
        //measure weight cannot be loaded
        if (referenceUnit == null)
            return null;

        productPrice ??= product.Price;

        var basePrice = productPrice.Value /
                        //do not round. otherwise, it can cause issues
                        await _measureService.ConvertWeightAsync(productAmount, productUnit, referenceUnit, false) *
                        referenceAmount;
        var basePriceInCurrentCurrency = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(basePrice, await _workContext.GetWorkingCurrencyAsync());
		_logger.LogDebug("FormatBasePriceAsync(product={Product},productPrice={ProductPrice},totalWeight={TotalWeight}): basePriceInCurrentCurrency → {BasePriceInCurrentCurrency}", product, productPrice, totalWeight, basePriceInCurrentCurrency);
        var basePriceStr = await FormatPriceAsync(basePriceInCurrentCurrency, true, false);
		_logger.LogDebug("FormatBasePriceAsync(product={Product},productPrice={ProductPrice},totalWeight={TotalWeight}): basePriceStr → {BasePriceStr}", product, productPrice, totalWeight, basePriceStr);

        var result = string.Format(await _localizationService.GetResourceAsync("Products.BasePrice"),
            basePriceStr, referenceAmount.ToString("G29"), referenceUnit.Name);
		_logger.LogDebug("<<< Exiting FormatBasePriceAsync(product={Product},productPrice={ProductPrice},totalWeight={TotalWeight})", product, productPrice, totalWeight);
        return result;
    }

    #endregion
}