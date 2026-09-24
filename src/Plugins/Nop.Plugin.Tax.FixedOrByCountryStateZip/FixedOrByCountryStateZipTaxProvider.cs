using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Domain;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Infrastructure.Cache;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Tax;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip;

/// <summary>
/// Fixed or by country & state & zip rate tax provider
/// </summary>
public class FixedOrByCountryStateZipTaxProvider : BasePlugin, ITaxProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly FixedOrByCountryStateZipTaxSettings _countryStateZipSettings;
    protected readonly ICountryStateZipService _taxRateService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
    protected readonly IPaymentService _paymentService;
    protected readonly ISettingService _settingService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly ITaxCategoryService _taxCategoryService;
    protected readonly ITaxService _taxService;
    protected readonly IWebHelper _webHelper;
    protected readonly TaxSettings _taxSettings;

    #endregion

    #region Ctor

    public FixedOrByCountryStateZipTaxProvider(FixedOrByCountryStateZipTaxSettings countryStateZipSettings,
        ICountryStateZipService taxRateService,
        IGenericAttributeService genericAttributeService,
        IHttpContextAccessor httpContextAccessor,
        ILocalizationService localizationService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IPaymentService paymentService,
        ISettingService settingService,
        IStaticCacheManager staticCacheManager,
        ITaxCategoryService taxCategoryService,
        ITaxService taxService,
        IWebHelper webHelper,
        TaxSettings taxSettings)
    {
		_logger.LogError(">>> Entering FixedOrByCountryStateZipTaxProvider(countryStateZipSettings={CountryStateZipSettings},taxRateService={TaxRateService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},paymentService={PaymentService},settingService={SettingService},staticCacheManager={StaticCacheManager},taxCategoryService={TaxCategoryService},taxService={TaxService},webHelper={WebHelper},taxSettings={TaxSettings})", countryStateZipSettings, taxRateService, genericAttributeService, httpContextAccessor, localizationService, orderTotalCalculationService, paymentService, settingService, staticCacheManager, taxCategoryService, taxService, webHelper, taxSettings);
        _countryStateZipSettings = countryStateZipSettings;
        _taxRateService = taxRateService;
		_logger.LogDebug("FixedOrByCountryStateZipTaxProvider(countryStateZipSettings={CountryStateZipSettings},taxRateService={TaxRateService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},paymentService={PaymentService},settingService={SettingService},staticCacheManager={StaticCacheManager},taxCategoryService={TaxCategoryService},taxService={TaxService},webHelper={WebHelper},taxSettings={TaxSettings}): _taxRateService → {_taxRateService}", countryStateZipSettings, taxRateService, genericAttributeService, httpContextAccessor, localizationService, orderTotalCalculationService, paymentService, settingService, staticCacheManager, taxCategoryService, taxService, webHelper, taxSettings, _taxRateService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("FixedOrByCountryStateZipTaxProvider(countryStateZipSettings={CountryStateZipSettings},taxRateService={TaxRateService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},paymentService={PaymentService},settingService={SettingService},staticCacheManager={StaticCacheManager},taxCategoryService={TaxCategoryService},taxService={TaxService},webHelper={WebHelper},taxSettings={TaxSettings}): _genericAttributeService → {_genericAttributeService}", countryStateZipSettings, taxRateService, genericAttributeService, httpContextAccessor, localizationService, orderTotalCalculationService, paymentService, settingService, staticCacheManager, taxCategoryService, taxService, webHelper, taxSettings, _genericAttributeService);
        _httpContextAccessor = httpContextAccessor;
		_logger.LogDebug("FixedOrByCountryStateZipTaxProvider(countryStateZipSettings={CountryStateZipSettings},taxRateService={TaxRateService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},paymentService={PaymentService},settingService={SettingService},staticCacheManager={StaticCacheManager},taxCategoryService={TaxCategoryService},taxService={TaxService},webHelper={WebHelper},taxSettings={TaxSettings}): _httpContextAccessor → {_httpContextAccessor}", countryStateZipSettings, taxRateService, genericAttributeService, httpContextAccessor, localizationService, orderTotalCalculationService, paymentService, settingService, staticCacheManager, taxCategoryService, taxService, webHelper, taxSettings, _httpContextAccessor);
        _localizationService = localizationService;
		_logger.LogDebug("FixedOrByCountryStateZipTaxProvider(countryStateZipSettings={CountryStateZipSettings},taxRateService={TaxRateService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},paymentService={PaymentService},settingService={SettingService},staticCacheManager={StaticCacheManager},taxCategoryService={TaxCategoryService},taxService={TaxService},webHelper={WebHelper},taxSettings={TaxSettings}): _localizationService → {_localizationService}", countryStateZipSettings, taxRateService, genericAttributeService, httpContextAccessor, localizationService, orderTotalCalculationService, paymentService, settingService, staticCacheManager, taxCategoryService, taxService, webHelper, taxSettings, _localizationService);
        _orderTotalCalculationService = orderTotalCalculationService;
		_logger.LogDebug("FixedOrByCountryStateZipTaxProvider(countryStateZipSettings={CountryStateZipSettings},taxRateService={TaxRateService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},paymentService={PaymentService},settingService={SettingService},staticCacheManager={StaticCacheManager},taxCategoryService={TaxCategoryService},taxService={TaxService},webHelper={WebHelper},taxSettings={TaxSettings}): _orderTotalCalculationService → {_orderTotalCalculationService}", countryStateZipSettings, taxRateService, genericAttributeService, httpContextAccessor, localizationService, orderTotalCalculationService, paymentService, settingService, staticCacheManager, taxCategoryService, taxService, webHelper, taxSettings, _orderTotalCalculationService);
        _paymentService = paymentService;
		_logger.LogDebug("FixedOrByCountryStateZipTaxProvider(countryStateZipSettings={CountryStateZipSettings},taxRateService={TaxRateService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},paymentService={PaymentService},settingService={SettingService},staticCacheManager={StaticCacheManager},taxCategoryService={TaxCategoryService},taxService={TaxService},webHelper={WebHelper},taxSettings={TaxSettings}): _paymentService → {_paymentService}", countryStateZipSettings, taxRateService, genericAttributeService, httpContextAccessor, localizationService, orderTotalCalculationService, paymentService, settingService, staticCacheManager, taxCategoryService, taxService, webHelper, taxSettings, _paymentService);
        _settingService = settingService;
		_logger.LogDebug("FixedOrByCountryStateZipTaxProvider(countryStateZipSettings={CountryStateZipSettings},taxRateService={TaxRateService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},paymentService={PaymentService},settingService={SettingService},staticCacheManager={StaticCacheManager},taxCategoryService={TaxCategoryService},taxService={TaxService},webHelper={WebHelper},taxSettings={TaxSettings}): _settingService → {_settingService}", countryStateZipSettings, taxRateService, genericAttributeService, httpContextAccessor, localizationService, orderTotalCalculationService, paymentService, settingService, staticCacheManager, taxCategoryService, taxService, webHelper, taxSettings, _settingService);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("FixedOrByCountryStateZipTaxProvider(countryStateZipSettings={CountryStateZipSettings},taxRateService={TaxRateService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},paymentService={PaymentService},settingService={SettingService},staticCacheManager={StaticCacheManager},taxCategoryService={TaxCategoryService},taxService={TaxService},webHelper={WebHelper},taxSettings={TaxSettings}): _staticCacheManager → {_staticCacheManager}", countryStateZipSettings, taxRateService, genericAttributeService, httpContextAccessor, localizationService, orderTotalCalculationService, paymentService, settingService, staticCacheManager, taxCategoryService, taxService, webHelper, taxSettings, _staticCacheManager);
        _taxCategoryService = taxCategoryService;
		_logger.LogDebug("FixedOrByCountryStateZipTaxProvider(countryStateZipSettings={CountryStateZipSettings},taxRateService={TaxRateService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},paymentService={PaymentService},settingService={SettingService},staticCacheManager={StaticCacheManager},taxCategoryService={TaxCategoryService},taxService={TaxService},webHelper={WebHelper},taxSettings={TaxSettings}): _taxCategoryService → {_taxCategoryService}", countryStateZipSettings, taxRateService, genericAttributeService, httpContextAccessor, localizationService, orderTotalCalculationService, paymentService, settingService, staticCacheManager, taxCategoryService, taxService, webHelper, taxSettings, _taxCategoryService);
        _taxService = taxService;
		_logger.LogDebug("FixedOrByCountryStateZipTaxProvider(countryStateZipSettings={CountryStateZipSettings},taxRateService={TaxRateService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},paymentService={PaymentService},settingService={SettingService},staticCacheManager={StaticCacheManager},taxCategoryService={TaxCategoryService},taxService={TaxService},webHelper={WebHelper},taxSettings={TaxSettings}): _taxService → {_taxService}", countryStateZipSettings, taxRateService, genericAttributeService, httpContextAccessor, localizationService, orderTotalCalculationService, paymentService, settingService, staticCacheManager, taxCategoryService, taxService, webHelper, taxSettings, _taxService);
        _webHelper = webHelper;
		_logger.LogDebug("FixedOrByCountryStateZipTaxProvider(countryStateZipSettings={CountryStateZipSettings},taxRateService={TaxRateService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},paymentService={PaymentService},settingService={SettingService},staticCacheManager={StaticCacheManager},taxCategoryService={TaxCategoryService},taxService={TaxService},webHelper={WebHelper},taxSettings={TaxSettings}): _webHelper → {_webHelper}", countryStateZipSettings, taxRateService, genericAttributeService, httpContextAccessor, localizationService, orderTotalCalculationService, paymentService, settingService, staticCacheManager, taxCategoryService, taxService, webHelper, taxSettings, _webHelper);
        _taxSettings = taxSettings;
		_logger.LogDebug("FixedOrByCountryStateZipTaxProvider(countryStateZipSettings={CountryStateZipSettings},taxRateService={TaxRateService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},paymentService={PaymentService},settingService={SettingService},staticCacheManager={StaticCacheManager},taxCategoryService={TaxCategoryService},taxService={TaxService},webHelper={WebHelper},taxSettings={TaxSettings}): _taxSettings → {_taxSettings}", countryStateZipSettings, taxRateService, genericAttributeService, httpContextAccessor, localizationService, orderTotalCalculationService, paymentService, settingService, staticCacheManager, taxCategoryService, taxService, webHelper, taxSettings, _taxSettings);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets tax rate
    /// </summary>
    /// <param name="taxRateRequest">Tax rate request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ax
    /// </returns>
    public async Task<TaxRateResult> GetTaxRateAsync(TaxRateRequest taxRateRequest)
    {
		_logger.LogError(">>> Entering GetTaxRateAsync(taxRateRequest={TaxRateRequest})", taxRateRequest);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var result = new TaxRateResult();

		_logger.LogError("GetTaxRateAsync(taxRateRequest={TaxRateRequest}): result → {Result}", taxRateRequest, result);
        //the tax rate calculation by fixed rate
        if (!_countryStateZipSettings.CountryStateZipEnabled)
        {
            result.TaxRate = await _settingService.GetSettingByKeyAsync<decimal>(string.Format(FixedOrByCountryStateZipDefaults.FIXED_RATE_SETTINGS_KEY, taxRateRequest.TaxCategoryId));
            return result;
        }

        //the tax rate calculation by country & state & zip 
        if (taxRateRequest.Address == null)
        {
            result.Errors.Add("Address is not set");
            return result;
        }

        //first, load all tax rate records (cached) - loaded only once
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(ModelCacheEventConsumer.ALL_TAX_RATES_MODEL_KEY);
		_logger.LogError("GetTaxRateAsync(taxRateRequest={TaxRateRequest}): cacheKey → {CacheKey}", taxRateRequest, cacheKey);
        var allTaxRates = await _staticCacheManager.GetAsync(cacheKey, async () => (await _taxRateService.GetAllTaxRatesAsync()).Select(taxRate => new TaxRate
        {
            Id = taxRate.Id,
            StoreId = taxRate.StoreId,
            TaxCategoryId = taxRate.TaxCategoryId,
            CountryId = taxRate.CountryId,
            StateProvinceId = taxRate.StateProvinceId,
            Zip = taxRate.Zip,
            Percentage = taxRate.Percentage
        }).ToList());
		_logger.LogDebug("GetTaxRateAsync(taxRateRequest={TaxRateRequest}): Percentage → {Percentage}", taxRateRequest, Percentage);

        var storeId = taxRateRequest.CurrentStoreId;
        var taxCategoryId = taxRateRequest.TaxCategoryId;
        var countryId = taxRateRequest.Address.CountryId;
        var stateProvinceId = taxRateRequest.Address.StateProvinceId;
        var zip = taxRateRequest.Address.ZipPostalCode?.Trim() ?? string.Empty;
		_logger.LogError("GetTaxRateAsync(taxRateRequest={TaxRateRequest}): zip → {Zip}", taxRateRequest, zip);

        var existingRates = allTaxRates.Where(taxRate => taxRate.CountryId == countryId && taxRate.TaxCategoryId == taxCategoryId);
		_logger.LogError("GetTaxRateAsync(taxRateRequest={TaxRateRequest}): existingRates → {ExistingRates}", taxRateRequest, existingRates);

        //filter by store
        var matchedByStore = existingRates.Where(taxRate => storeId == taxRate.StoreId || taxRate.StoreId == 0);
		_logger.LogError("GetTaxRateAsync(taxRateRequest={TaxRateRequest}): matchedByStore → {MatchedByStore}", taxRateRequest, matchedByStore);

        //filter by state/province
        var matchedByStateProvince = matchedByStore.Where(taxRate => stateProvinceId == taxRate.StateProvinceId || taxRate.StateProvinceId == 0);
		_logger.LogError("GetTaxRateAsync(taxRateRequest={TaxRateRequest}): matchedByStateProvince → {MatchedByStateProvince}", taxRateRequest, matchedByStateProvince);

        //filter by zip
        var matchedByZip = matchedByStateProvince.Where(taxRate => string.IsNullOrWhiteSpace(taxRate.Zip) || taxRate.Zip.Equals(zip, StringComparison.InvariantCultureIgnoreCase));
		_logger.LogError("GetTaxRateAsync(taxRateRequest={TaxRateRequest}): matchedByZip → {MatchedByZip}", taxRateRequest, matchedByZip);

        //sort from particular to general, more particular cases will be the first
        var foundRecords = matchedByZip.OrderBy(r => r.StoreId == 0).ThenBy(r => r.StateProvinceId == 0).ThenBy(r => string.IsNullOrEmpty(r.Zip));
		_logger.LogError("GetTaxRateAsync(taxRateRequest={TaxRateRequest}): foundRecords → {FoundRecords}", taxRateRequest, foundRecords);

        var foundRecord = foundRecords.FirstOrDefault();
		_logger.LogError("GetTaxRateAsync(taxRateRequest={TaxRateRequest}): foundRecord → {FoundRecord}", taxRateRequest, foundRecord);

        if (foundRecord != null)
            result.TaxRate = foundRecord.Percentage;
			_logger.LogDebug("GetTaxRateAsync(taxRateRequest={TaxRateRequest}): result.TaxRate → {Arg1}", taxRateRequest, result.TaxRate);

		_logger.LogError("GetTaxRateAsync(taxRateRequest)={TaxRateRequest}: external service call took {Arg1} ms", taxRateRequest, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetTaxRateAsync(taxRateRequest={TaxRateRequest})", taxRateRequest);
        return result;
    }

    /// <summary>
    /// Gets tax total
    /// </summary>
    /// <param name="taxTotalRequest">Tax total request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ax total
    /// </returns>
    public async Task<TaxTotalResult> GetTaxTotalAsync(TaxTotalRequest taxTotalRequest)
    {
		_logger.LogError(">>> Entering GetTaxTotalAsync(taxTotalRequest={TaxTotalRequest})", taxTotalRequest);
        if (_httpContextAccessor.HttpContext.Items.TryGetValue("nop.TaxTotal", out var result)
            && result is (TaxTotalResult taxTotalResult, decimal paymentTax))
        {
            //short-circuit to avoid circular reference when calculating payment method additional fee during the checkout process
            if (!taxTotalRequest.UsePaymentMethodAdditionalFee)
                return new TaxTotalResult { TaxTotal = taxTotalResult.TaxTotal - paymentTax };

            return taxTotalResult;
        }

        var taxRates = new SortedDictionary<decimal, decimal>();
		_logger.LogError("GetTaxTotalAsync(taxTotalRequest={TaxTotalRequest}): taxRates → {TaxRates}", taxTotalRequest, taxRates);
        var taxTotal = decimal.Zero;
		_logger.LogError("GetTaxTotalAsync(taxTotalRequest={TaxTotalRequest}): taxTotal → {TaxTotal}", taxTotalRequest, taxTotal);

        //order sub total (items + checkout attributes)
        var (_, _, _, _, orderSubTotalTaxRates) = await _orderTotalCalculationService
            .GetShoppingCartSubTotalAsync(taxTotalRequest.ShoppingCart, false);
        var subTotalTaxTotal = decimal.Zero;
        foreach (var kvp in orderSubTotalTaxRates)
        {
            var taxRate = kvp.Key;
            var taxValue = kvp.Value;
            subTotalTaxTotal += taxValue;

            if (taxRate > decimal.Zero && taxValue > decimal.Zero)
            {
                if (!taxRates.TryGetValue(taxRate, out var value))
                    taxRates.Add(taxRate, taxValue);
                else
                    taxRates[taxRate] = value + taxValue;
            }
        }
        taxTotal += subTotalTaxTotal;

        //shipping
        var shippingTax = decimal.Zero;
		_logger.LogError("GetTaxTotalAsync(taxTotalRequest={TaxTotalRequest}): shippingTax → {ShippingTax}", taxTotalRequest, shippingTax);
        if (_taxSettings.ShippingIsTaxable)
        {
            var (shippingExclTax, _, _) = await _orderTotalCalculationService
                .GetShoppingCartShippingTotalAsync(taxTotalRequest.ShoppingCart, false);
            var (shippingInclTax, taxRate, _) = await _orderTotalCalculationService
                .GetShoppingCartShippingTotalAsync(taxTotalRequest.ShoppingCart, true);
            if (shippingExclTax.HasValue && shippingInclTax.HasValue)
            {
                shippingTax = shippingInclTax.Value - shippingExclTax.Value;
                if (shippingTax < decimal.Zero)
                    shippingTax = decimal.Zero;

                if (taxRate > decimal.Zero && shippingTax > decimal.Zero)
                {
                    if (!taxRates.TryGetValue(taxRate, out var value))
                        taxRates.Add(taxRate, shippingTax);
                    else
                        taxRates[taxRate] = value + shippingTax;
                }
            }
        }
        taxTotal += shippingTax;

        //short-circuit to avoid circular reference when calculating payment method additional fee during the checkout process
        if (!taxTotalRequest.UsePaymentMethodAdditionalFee)
            return new TaxTotalResult { TaxTotal = taxTotal };

        //payment method additional fee
        var paymentMethodAdditionalFeeTax = decimal.Zero;
		_logger.LogError("GetTaxTotalAsync(taxTotalRequest={TaxTotalRequest}): paymentMethodAdditionalFeeTax → {PaymentMethodAdditionalFeeTax}", taxTotalRequest, paymentMethodAdditionalFeeTax);
        if (_taxSettings.PaymentMethodAdditionalFeeIsTaxable)
        {
            var paymentMethodSystemName = taxTotalRequest.Customer != null
                ? await _genericAttributeService
                    .GetAttributeAsync<string>(taxTotalRequest.Customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, taxTotalRequest.StoreId)
                : string.Empty;

            var paymentMethodAdditionalFee = await _paymentService
                .GetAdditionalHandlingFeeAsync(taxTotalRequest.ShoppingCart, paymentMethodSystemName);
            var (paymentMethodAdditionalFeeExclTax, _) = await _taxService
                .GetPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFee, false, taxTotalRequest.Customer);
            var (paymentMethodAdditionalFeeInclTax, taxRate) = await _taxService
                .GetPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFee, true, taxTotalRequest.Customer);

            paymentMethodAdditionalFeeTax = paymentMethodAdditionalFeeInclTax - paymentMethodAdditionalFeeExclTax;
            if (paymentMethodAdditionalFeeTax < decimal.Zero)
                paymentMethodAdditionalFeeTax = decimal.Zero;

            if (taxRate > decimal.Zero && paymentMethodAdditionalFeeTax > decimal.Zero)
            {
                if (!taxRates.TryGetValue(taxRate, out var value))
                    taxRates.Add(taxRate, paymentMethodAdditionalFeeTax);
                else
                    taxRates[taxRate] = value + paymentMethodAdditionalFeeTax;
            }
        }
        taxTotal += paymentMethodAdditionalFeeTax;

        //add at least one tax rate (0%)
        if (!taxRates.Any())
            taxRates.Add(decimal.Zero, decimal.Zero);

        if (taxTotal < decimal.Zero)
            taxTotal = decimal.Zero;

        taxTotalResult = new TaxTotalResult { TaxTotal = taxTotal, TaxRates = taxRates, };

        //store values within the scope of the request to avoid duplicate calculations
        _httpContextAccessor.HttpContext.Items.TryAdd("nop.TaxTotal", (taxTotalResult, paymentMethodAdditionalFeeTax));

		_logger.LogWarning("<<< Exiting GetTaxTotalAsync(taxTotalRequest={TaxTotalRequest})", taxTotalRequest);
        return taxTotalResult;
    }

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
		_logger.LogError(">>> Entering GetConfigurationPageUrl()");
        _logger.LogWarning("<<< Exiting GetConfigurationPageUrl()");
        return $"{_webHelper.GetStoreLocation()}Admin/FixedOrByCountryStateZip/Configure";
    }

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
		_logger.LogError(">>> Entering InstallAsync()");
        //settings
        await _settingService.SaveSettingAsync(new FixedOrByCountryStateZipTaxSettings());

        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Tax.FixedOrByCountryStateZip.Fixed"] = "Fixed rate",
            ["Plugins.Tax.FixedOrByCountryStateZip.Tax.Categories.Manage"] = "Manage tax categories",
            ["Plugins.Tax.FixedOrByCountryStateZip.TaxCategoriesCanNotLoaded"] = "No tax categories can be loaded. You may manage tax categories by <a href='{0}'>this link</a>",
            ["Plugins.Tax.FixedOrByCountryStateZip.TaxByCountryStateZip"] = "By Country",
            ["Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategoryName"] = "Tax category",
            ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Rate"] = "Rate",
            ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Store"] = "Store",
            ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Store.Hint"] = "If an asterisk is selected, then this shipping rate will apply to all stores.",
            ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Country"] = "Country",
            ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Country.Hint"] = "The country.",
            ["Plugins.Tax.FixedOrByCountryStateZip.Fields.StateProvince"] = "State / province",
            ["Plugins.Tax.FixedOrByCountryStateZip.Fields.StateProvince.Hint"] = "If an asterisk is selected, then this tax rate will apply to all customers from the given country, regardless of the state.",
            ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Zip"] = "Zip",
            ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Zip.Hint"] = "Zip / postal code. If zip is empty, then this tax rate will apply to all customers from the given country or state, regardless of the zip code.",
            ["Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategory"] = "Tax category",
            ["Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategory.Hint"] = "The tax category.",
            ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Percentage"] = "Percentage",
            ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Percentage.Hint"] = "The tax rate.",
            ["Plugins.Tax.FixedOrByCountryStateZip.AddRecord"] = "Add tax rate",
            ["Plugins.Tax.FixedOrByCountryStateZip.AddRecordTitle"] = "New tax rate",
            ["Plugins.Tax.FixedOrByCountryStateZip.SwitchRate"] = @"
                    <p>
                        You are going to change the way the tax rate is calculated. This will cause the tax rate to be calculated based on the settings specified on the configuration page.
                    </p>
                    <p>
                        Any current tax rate settings will be saved, but will not be active until you return to this tax calculation method.
                    </p>",
        });

        await base.InstallAsync();
		_logger.LogWarning("<<< Exiting InstallAsync()");
    }

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
		_logger.LogError(">>> Entering UninstallAsync()");
        //settings
        await _settingService.DeleteSettingAsync<FixedOrByCountryStateZipTaxSettings>();

        //fixed rates
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var fixedRates = await (await _taxCategoryService.GetAllTaxCategoriesAsync())
            .SelectAwait(async taxCategory => await _settingService.GetSettingAsync(string.Format(FixedOrByCountryStateZipDefaults.FIXED_RATE_SETTINGS_KEY, taxCategory.Id)))
            .Where(setting => setting != null).ToListAsync();
			_logger.LogError("UninstallAsync(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        await _settingService.DeleteSettingsAsync(fixedRates);

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Tax.FixedOrByCountryStateZip");

        await base.UninstallAsync();
		_logger.LogWarning("<<< Exiting UninstallAsync()");
    }

    #endregion
}