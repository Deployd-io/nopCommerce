using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Translation;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Framework.Models.Translation;
using LogLevel = Nop.Core.Domain.Logging.LogLevel;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the implementation of the base model factory that implements a most common admin model factories methods
/// </summary>
public partial class BaseAdminModelFactory : IBaseAdminModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICategoryService _categoryService;
    protected readonly ICategoryTemplateService _categoryTemplateService;
    protected readonly ICountryService _countryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateRangeService _dateRangeService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IEmailAccountService _emailAccountService;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly IManufacturerTemplateService _manufacturerTemplateService;
    protected readonly INewsLetterSubscriptionTypeService _newsLetterSubscriptionTypeService;
    protected readonly IPluginService _pluginService;
    protected readonly IProductTemplateService _productTemplateService;
    protected readonly ISpecificationAttributeService _specificationAttributeService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreService _storeService;
    protected readonly ITaxCategoryService _taxCategoryService;
    protected readonly ITopicTemplateService _topicTemplateService;
    protected readonly IVendorService _vendorService;
    protected readonly IWarehouseService _warehouseService;
    protected readonly TranslationSettings _translationSettings;

    #endregion

    #region Ctor

    public BaseAdminModelFactory(ICategoryService categoryService,
        ICategoryTemplateService categoryTemplateService,
        ICountryService countryService,
        ICurrencyService currencyService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IDateRangeService dateRangeService,
        IDateTimeHelper dateTimeHelper,
        IEmailAccountService emailAccountService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IManufacturerService manufacturerService,
        IManufacturerTemplateService manufacturerTemplateService,
        INewsLetterSubscriptionTypeService newsLetterSubscriptionTypeService,
        IPluginService pluginService,
        IProductTemplateService productTemplateService,
        ISpecificationAttributeService specificationAttributeService,
        IStateProvinceService stateProvinceService,
        IStaticCacheManager staticCacheManager,
        IStoreService storeService,
        ITaxCategoryService taxCategoryService,
        ITopicTemplateService topicTemplateService,
        IVendorService vendorService,
        IWarehouseService warehouseService,
        TranslationSettings translationSettings)
    {
		_logger.LogError(">>> Entering BaseAdminModelFactory()");
        _categoryService = categoryService;
        _categoryTemplateService = categoryTemplateService;
		_logger.LogDebug("BaseAdminModelFactory(): _categoryTemplateService → {_categoryTemplateService}", _categoryTemplateService);
        _countryService = countryService;
		_logger.LogDebug("BaseAdminModelFactory(): _countryService → {_countryService}", _countryService);
        _currencyService = currencyService;
		_logger.LogDebug("BaseAdminModelFactory(): _currencyService → {_currencyService}", _currencyService);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("BaseAdminModelFactory(): _customerActivityService → {_customerActivityService}", _customerActivityService);
        _customerService = customerService;
		_logger.LogDebug("BaseAdminModelFactory(): _customerService → {_customerService}", _customerService);
        _dateRangeService = dateRangeService;
		_logger.LogDebug("BaseAdminModelFactory(): _dateRangeService → {_dateRangeService}", _dateRangeService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("BaseAdminModelFactory(): _dateTimeHelper → {_dateTimeHelper}", _dateTimeHelper);
        _emailAccountService = emailAccountService;
		_logger.LogDebug("BaseAdminModelFactory(): _emailAccountService → {_emailAccountService}", _emailAccountService);
        _languageService = languageService;
		_logger.LogDebug("BaseAdminModelFactory(): _languageService → {_languageService}", _languageService);
        _localizationService = localizationService;
		_logger.LogDebug("BaseAdminModelFactory(): _localizationService → {_localizationService}", _localizationService);
        _manufacturerService = manufacturerService;
		_logger.LogDebug("BaseAdminModelFactory(): _manufacturerService → {_manufacturerService}", _manufacturerService);
        _manufacturerTemplateService = manufacturerTemplateService;
		_logger.LogDebug("BaseAdminModelFactory(): _manufacturerTemplateService → {_manufacturerTemplateService}", _manufacturerTemplateService);
        _newsLetterSubscriptionTypeService = newsLetterSubscriptionTypeService;
		_logger.LogDebug("BaseAdminModelFactory(): _newsLetterSubscriptionTypeService → {_newsLetterSubscriptionTypeService}", _newsLetterSubscriptionTypeService);
        _pluginService = pluginService;
		_logger.LogDebug("BaseAdminModelFactory(): _pluginService → {_pluginService}", _pluginService);
        _productTemplateService = productTemplateService;
		_logger.LogDebug("BaseAdminModelFactory(): _productTemplateService → {_productTemplateService}", _productTemplateService);
        _specificationAttributeService = specificationAttributeService;
		_logger.LogDebug("BaseAdminModelFactory(): _specificationAttributeService → {_specificationAttributeService}", _specificationAttributeService);
        _stateProvinceService = stateProvinceService;
		_logger.LogDebug("BaseAdminModelFactory(): _stateProvinceService → {_stateProvinceService}", _stateProvinceService);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("BaseAdminModelFactory(): _staticCacheManager → {_staticCacheManager}", _staticCacheManager);
        _storeService = storeService;
		_logger.LogDebug("BaseAdminModelFactory(): _storeService → {_storeService}", _storeService);
        _taxCategoryService = taxCategoryService;
		_logger.LogDebug("BaseAdminModelFactory(): _taxCategoryService → {_taxCategoryService}", _taxCategoryService);
        _topicTemplateService = topicTemplateService;
		_logger.LogDebug("BaseAdminModelFactory(): _topicTemplateService → {_topicTemplateService}", _topicTemplateService);
        _vendorService = vendorService;
		_logger.LogDebug("BaseAdminModelFactory(): _vendorService → {_vendorService}", _vendorService);
        _warehouseService = warehouseService;
		_logger.LogDebug("BaseAdminModelFactory(): _warehouseService → {_warehouseService}", _warehouseService);
        _translationSettings = translationSettings;
		_logger.LogDebug("BaseAdminModelFactory(): _translationSettings → {_translationSettings}", _translationSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare default item
    /// </summary>
    /// <param name="items">Available items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use "All" text</param>
    /// <param name="defaultItemValue">Default item value; defaults 0</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareDefaultItemAsync(IList<SelectListItem> items, bool withSpecialDefaultItem, string defaultItemText = null, string defaultItemValue = "0")
    {
		_logger.LogError(">>> Entering PrepareDefaultItemAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText},defaultItemValue={DefaultItemValue})", items, withSpecialDefaultItem, defaultItemText, defaultItemValue);
        ArgumentNullException.ThrowIfNull(items);

        //whether to insert the first special item for the default value
        if (!withSpecialDefaultItem)
            return;

        //prepare item text
        defaultItemText ??= await _localizationService.GetResourceAsync("Admin.Common.All");

        //insert this default item at first
        items.Insert(0, new SelectListItem { Text = defaultItemText, Value = defaultItemValue });
		_logger.LogWarning("<<< Exiting PrepareDefaultItemAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText},defaultItemValue={DefaultItemValue})", items, withSpecialDefaultItem, defaultItemText, defaultItemValue);
    }

    /// <summary>
    /// Get category list
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category list
    /// </returns>
    protected virtual async Task<List<SelectListItem>> GetCategoryListAsync()
    {
		_logger.LogError(">>> Entering GetCategoryListAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var categories = await _staticCacheManager.GetAsync(NopModelCacheDefaults.CategoriesListKey, async () => await _categoryService.GetAllCategoriesAsync(showHidden: true));

		_logger.LogError("GetCategoryListAsync(): categories → {Categories}", categories);
        var result = new List<SelectListItem>();
		_logger.LogError("GetCategoryListAsync(): result → {Result}", result);
        foreach (var category in categories)
        {
            if (!await _categoryService.CanVendorAddProductsAsync(category, categories))
                continue;

            result.Add(new SelectListItem
            {
                Text = await _categoryService.GetFormattedBreadCrumbAsync(category, categories),
                Value = category.Id.ToString()
            });
				_logger.LogDebug("GetCategoryListAsync(): Value → {Value}", Value);
        }

		_logger.LogError("GetCategoryListAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetCategoryListAsync()");
        return result;
    }

    /// <summary>
    /// Get manufacturer list
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer list
    /// </returns>
    protected virtual async Task<List<SelectListItem>> GetManufacturerListAsync()
    {
		_logger.LogError(">>> Entering GetManufacturerListAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var listItems = await _staticCacheManager.GetAsync(NopModelCacheDefaults.ManufacturersListKey, async () =>
        {
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true);
			_logger.LogError("GetManufacturerListAsync(): manufacturers → {Manufacturers}", manufacturers);
            return manufacturers.Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString()
            });
        });

        var result = new List<SelectListItem>();
		_logger.LogError("GetManufacturerListAsync(): result → {Result}", result);
        //clone the list to ensure that "selected" property is not set
        foreach (var item in listItems)
        {
            result.Add(new SelectListItem
            {
                Text = item.Text,
                Value = item.Value
            });
			_logger.LogDebug("GetManufacturerListAsync(): Value → {Value}", Value);
        }

		_logger.LogError("GetManufacturerListAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetManufacturerListAsync()");
        return result;
    }

    /// <summary>
    /// Get vendor list
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor list
    /// </returns>
    protected virtual async Task<List<SelectListItem>> GetVendorListAsync()
    {
		_logger.LogError(">>> Entering GetVendorListAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var listItems = await _staticCacheManager.GetAsync(NopModelCacheDefaults.VendorsListKey, async () =>
        {
            var vendors = await _vendorService.GetAllVendorsAsync(showHidden: true);
			_logger.LogError("GetVendorListAsync(): vendors → {Vendors}", vendors);
            return vendors.Select(v => new SelectListItem
            {
                Text = v.Name,
                Value = v.Id.ToString()
            });
        });

        var result = new List<SelectListItem>();
		_logger.LogError("GetVendorListAsync(): result → {Result}", result);
        //clone the list to ensure that "selected" property is not set
        foreach (var item in listItems)
        {
            result.Add(new SelectListItem
            {
                Text = item.Text,
                Value = item.Value
            });
			_logger.LogDebug("GetVendorListAsync(): Value → {Value}", Value);
        }

		_logger.LogError("GetVendorListAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetVendorListAsync()");
        return result;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare available activity log types
    /// </summary>
    /// <param name="items">Activity log type items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareActivityLogTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareActivityLogTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available activity log types
        var availableActivityTypes = await _customerActivityService.GetAllActivityTypesAsync();
		_logger.LogError("PrepareActivityLogTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableActivityTypes → {AvailableActivityTypes}", items, withSpecialDefaultItem, defaultItemText, availableActivityTypes);
        foreach (var activityType in availableActivityTypes)
        {
            items.Add(new SelectListItem { Value = activityType.Id.ToString(), Text = activityType.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareActivityLogTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available order statuses
    /// </summary>
    /// <param name="items">Order status items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareOrderStatusesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareOrderStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available order statuses
        var availableStatusItems = await OrderStatus.Pending.ToSelectListAsync(false);
		_logger.LogError("PrepareOrderStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableStatusItems → {AvailableStatusItems}", items, withSpecialDefaultItem, defaultItemText, availableStatusItems);
        foreach (var statusItem in availableStatusItems)
        {
            items.Add(statusItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareOrderStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available payment statuses
    /// </summary>
    /// <param name="items">Payment status items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PreparePaymentStatusesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PreparePaymentStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available payment statuses
        var availableStatusItems = await PaymentStatus.Pending.ToSelectListAsync(false);
		_logger.LogError("PreparePaymentStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableStatusItems → {AvailableStatusItems}", items, withSpecialDefaultItem, defaultItemText, availableStatusItems);
        foreach (var statusItem in availableStatusItems)
        {
            items.Add(statusItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PreparePaymentStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available shipping statuses
    /// </summary>
    /// <param name="items">Shipping status items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareShippingStatusesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareShippingStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available shipping statuses
        var availableStatusItems = await ShippingStatus.NotYetShipped.ToSelectListAsync(false);
		_logger.LogError("PrepareShippingStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableStatusItems → {AvailableStatusItems}", items, withSpecialDefaultItem, defaultItemText, availableStatusItems);
        foreach (var statusItem in availableStatusItems)
        {
            items.Add(statusItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareShippingStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available countries
    /// </summary>
    /// <param name="items">Country items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareCountriesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareCountriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available countries
        var availableCountries = await _countryService.GetAllCountriesAsync(showHidden: true);
		_logger.LogError("PrepareCountriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableCountries → {AvailableCountries}", items, withSpecialDefaultItem, defaultItemText, availableCountries);
        foreach (var country in availableCountries)
        {
            items.Add(new SelectListItem { Value = country.Id.ToString(), Text = country.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText ?? await _localizationService.GetResourceAsync("Admin.Address.SelectCountry"));
		_logger.LogWarning("<<< Exiting PrepareCountriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available states and provinces
    /// </summary>
    /// <param name="items">State and province items</param>
    /// <param name="countryId">Country identifier; pass null to don't load states and provinces</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareStatesAndProvincesAsync(IList<SelectListItem> items, int? countryId,
        bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareStatesAndProvincesAsync(items={Items},countryId={CountryId},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, countryId, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        if (countryId.HasValue)
        {
            //prepare available states and provinces of the country
            var availableStates = await _stateProvinceService.GetStateProvincesByCountryIdAsync(countryId.Value, showHidden: true);
			_logger.LogError("PrepareStatesAndProvincesAsync(items={Items},countryId={CountryId},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableStates → {AvailableStates}", items, countryId, withSpecialDefaultItem, defaultItemText, availableStates);
            foreach (var state in availableStates)
            {
                items.Add(new SelectListItem { Value = state.Id.ToString(), Text = state.Name });
            }

            //insert special item for the default value
            if (items.Count > 1)
                await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText ?? await _localizationService.GetResourceAsync("Admin.Address.SelectState"));
        }

        //insert special item for the default value
        if (!items.Any())
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText ?? await _localizationService.GetResourceAsync("Admin.Address.Other"));
			_logger.LogWarning("<<< Exiting PrepareStatesAndProvincesAsync(items={Items},countryId={CountryId},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, countryId, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available languages
    /// </summary>
    /// <param name="items">Language items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareLanguagesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareLanguagesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available languages
        var availableLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
		_logger.LogError("PrepareLanguagesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableLanguages → {AvailableLanguages}", items, withSpecialDefaultItem, defaultItemText, availableLanguages);
        foreach (var language in availableLanguages)
        {
            items.Add(new SelectListItem { Value = language.Id.ToString(), Text = language.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareLanguagesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available stores
    /// </summary>
    /// <param name="items">Store items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareStoresAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareStoresAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available stores
        var availableStores = await _storeService.GetAllStoresAsync();
		_logger.LogError("PrepareStoresAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableStores → {AvailableStores}", items, withSpecialDefaultItem, defaultItemText, availableStores);
        foreach (var store in availableStores)
        {
            items.Add(new SelectListItem { Value = store.Id.ToString(), Text = store.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareStoresAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available customer roles
    /// </summary>
    /// <param name="items">Customer role items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareCustomerRolesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareCustomerRolesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available customer roles
        var availableCustomerRoles = await _customerService.GetAllCustomerRolesAsync();
		_logger.LogError("PrepareCustomerRolesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableCustomerRoles → {AvailableCustomerRoles}", items, withSpecialDefaultItem, defaultItemText, availableCustomerRoles);
        foreach (var customerRole in availableCustomerRoles)
        {
            items.Add(new SelectListItem { Value = customerRole.Id.ToString(), Text = customerRole.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareCustomerRolesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available newsletter subscription types
    /// </summary>
    /// <param name="items">Newsletter subscription type items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareSubscriptionTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareSubscriptionTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available newsletter subscription types
        var availableSubscriptionTypes = await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync();
		_logger.LogError("PrepareSubscriptionTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableSubscriptionTypes → {AvailableSubscriptionTypes}", items, withSpecialDefaultItem, defaultItemText, availableSubscriptionTypes);
        foreach (var subscriptionType in availableSubscriptionTypes)
        {
            items.Add(new SelectListItem { Value = subscriptionType.Id.ToString(), Text = subscriptionType.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareSubscriptionTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available email accounts
    /// </summary>
    /// <param name="items">Email account items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareEmailAccountsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareEmailAccountsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available email accounts
        var availableEmailAccounts = await _emailAccountService.GetAllEmailAccountsAsync();
		_logger.LogError("PrepareEmailAccountsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableEmailAccounts → {AvailableEmailAccounts}", items, withSpecialDefaultItem, defaultItemText, availableEmailAccounts);
        foreach (var emailAccount in availableEmailAccounts)
        {
            items.Add(new SelectListItem { Value = emailAccount.Id.ToString(), Text = $"{emailAccount.DisplayName} ({emailAccount.Email})" });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareEmailAccountsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available tax categories
    /// </summary>
    /// <param name="items">Tax category items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareTaxCategoriesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareTaxCategoriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available tax categories
        var availableTaxCategories = await _taxCategoryService.GetAllTaxCategoriesAsync();
		_logger.LogError("PrepareTaxCategoriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableTaxCategories → {AvailableTaxCategories}", items, withSpecialDefaultItem, defaultItemText, availableTaxCategories);
        foreach (var taxCategory in availableTaxCategories)
        {
            items.Add(new SelectListItem { Value = taxCategory.Id.ToString(), Text = taxCategory.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem,
            defaultItemText ?? await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Tax.TaxCategories.None"));
			_logger.LogWarning("<<< Exiting PrepareTaxCategoriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available categories
    /// </summary>
    /// <param name="items">Category items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareCategoriesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareCategoriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available categories
        var availableCategoryItems = await GetCategoryListAsync();
		_logger.LogError("PrepareCategoriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableCategoryItems → {AvailableCategoryItems}", items, withSpecialDefaultItem, defaultItemText, availableCategoryItems);
        foreach (var categoryItem in availableCategoryItems)
        {
            items.Add(categoryItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareCategoriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available manufacturers
    /// </summary>
    /// <param name="items">Manufacturer items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareManufacturersAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareManufacturersAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available manufacturers
        var availableManufacturerItems = await GetManufacturerListAsync();
		_logger.LogError("PrepareManufacturersAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableManufacturerItems → {AvailableManufacturerItems}", items, withSpecialDefaultItem, defaultItemText, availableManufacturerItems);
        foreach (var manufacturerItem in availableManufacturerItems)
        {
            items.Add(manufacturerItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareManufacturersAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available vendors
    /// </summary>
    /// <param name="items">Vendor items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareVendorsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareVendorsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available vendors
        var availableVendorItems = await GetVendorListAsync();
		_logger.LogError("PrepareVendorsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableVendorItems → {AvailableVendorItems}", items, withSpecialDefaultItem, defaultItemText, availableVendorItems);
        foreach (var vendorItem in availableVendorItems)
        {
            items.Add(vendorItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareVendorsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available product types
    /// </summary>
    /// <param name="items">Product type items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareProductTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareProductTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available product types
        var availableProductTypeItems = await ProductType.SimpleProduct.ToSelectListAsync(false);
		_logger.LogError("PrepareProductTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableProductTypeItems → {AvailableProductTypeItems}", items, withSpecialDefaultItem, defaultItemText, availableProductTypeItems);
        foreach (var productTypeItem in availableProductTypeItems)
        {
            items.Add(productTypeItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareProductTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available category templates
    /// </summary>
    /// <param name="items">Category template items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareCategoryTemplatesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareCategoryTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available category templates
        var availableTemplates = await _categoryTemplateService.GetAllCategoryTemplatesAsync();
		_logger.LogError("PrepareCategoryTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableTemplates → {AvailableTemplates}", items, withSpecialDefaultItem, defaultItemText, availableTemplates);
        foreach (var template in availableTemplates)
        {
            items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareCategoryTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available time zones
    /// </summary>
    /// <param name="items">Time zone items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareTimeZonesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareTimeZonesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available time zones
        var availableTimeZones = _dateTimeHelper.GetSystemTimeZones();
		_logger.LogError("PrepareTimeZonesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableTimeZones → {AvailableTimeZones}", items, withSpecialDefaultItem, defaultItemText, availableTimeZones);
        foreach (var timeZone in availableTimeZones)
        {
            items.Add(new SelectListItem { Value = timeZone.Id, Text = timeZone.DisplayName });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareTimeZonesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available shopping cart types
    /// </summary>
    /// <param name="items">Shopping cart type items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareShoppingCartTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareShoppingCartTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available shopping cart types
        var availableShoppingCartTypeItems = await ShoppingCartType.ShoppingCart.ToSelectListAsync(false);
		_logger.LogError("PrepareShoppingCartTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableShoppingCartTypeItems → {AvailableShoppingCartTypeItems}", items, withSpecialDefaultItem, defaultItemText, availableShoppingCartTypeItems);
        foreach (var shoppingCartTypeItem in availableShoppingCartTypeItems)
        {
            items.Add(shoppingCartTypeItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareShoppingCartTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available tax display types
    /// </summary>
    /// <param name="items">Tax display type items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareTaxDisplayTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareTaxDisplayTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available tax display types
        var availableTaxDisplayTypeItems = await TaxDisplayType.ExcludingTax.ToSelectListAsync(false);
		_logger.LogError("PrepareTaxDisplayTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableTaxDisplayTypeItems → {AvailableTaxDisplayTypeItems}", items, withSpecialDefaultItem, defaultItemText, availableTaxDisplayTypeItems);
        foreach (var taxDisplayTypeItem in availableTaxDisplayTypeItems)
        {
            items.Add(taxDisplayTypeItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareTaxDisplayTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available currencies
    /// </summary>
    /// <param name="items">Currency items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareCurrenciesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareCurrenciesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available currencies
        var availableCurrencies = await _currencyService.GetAllCurrenciesAsync(true);
		_logger.LogError("PrepareCurrenciesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableCurrencies → {AvailableCurrencies}", items, withSpecialDefaultItem, defaultItemText, availableCurrencies);
        foreach (var currency in availableCurrencies)
        {
            items.Add(new SelectListItem { Value = currency.Id.ToString(), Text = currency.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareCurrenciesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available discount types
    /// </summary>
    /// <param name="items">Discount type items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareDiscountTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareDiscountTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available discount types
        var availableDiscountTypeItems = await DiscountType.AssignedToOrderTotal.ToSelectListAsync(false);
		_logger.LogError("PrepareDiscountTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableDiscountTypeItems → {AvailableDiscountTypeItems}", items, withSpecialDefaultItem, defaultItemText, availableDiscountTypeItems);
        foreach (var discountTypeItem in availableDiscountTypeItems)
        {
            items.Add(discountTypeItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareDiscountTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available log levels
    /// </summary>
    /// <param name="items">Log level items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareLogLevelsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareLogLevelsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available log levels
        var availableLogLevelItems = await LogLevel.Debug.ToSelectListAsync(false);
		_logger.LogError("PrepareLogLevelsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableLogLevelItems → {AvailableLogLevelItems}", items, withSpecialDefaultItem, defaultItemText, availableLogLevelItems);
        foreach (var logLevelItem in availableLogLevelItems)
        {
            items.Add(logLevelItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareLogLevelsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available manufacturer templates
    /// </summary>
    /// <param name="items">Manufacturer template items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareManufacturerTemplatesAsync(IList<SelectListItem> items,
        bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareManufacturerTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available manufacturer templates
        var availableTemplates = await _manufacturerTemplateService.GetAllManufacturerTemplatesAsync();
		_logger.LogError("PrepareManufacturerTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableTemplates → {AvailableTemplates}", items, withSpecialDefaultItem, defaultItemText, availableTemplates);
        foreach (var template in availableTemplates)
        {
            items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareManufacturerTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available load plugin modes
    /// </summary>
    /// <param name="items">Load plugin mode items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareLoadPluginModesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareLoadPluginModesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available load plugin modes
        var availableLoadPluginModeItems = await LoadPluginsMode.All.ToSelectListAsync(false);
		_logger.LogError("PrepareLoadPluginModesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableLoadPluginModeItems → {AvailableLoadPluginModeItems}", items, withSpecialDefaultItem, defaultItemText, availableLoadPluginModeItems);
        foreach (var loadPluginModeItem in availableLoadPluginModeItems)
        {
            items.Add(loadPluginModeItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareLoadPluginModesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available plugin groups
    /// </summary>
    /// <param name="items">Plugin group items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PreparePluginGroupsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PreparePluginGroupsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available plugin groups
        var availablePluginGroups = (await _pluginService.GetPluginDescriptorsAsync<IPlugin>(LoadPluginsMode.All))
            .Select(plugin => plugin.Group).Distinct().OrderBy(groupName => groupName).ToList();
        foreach (var group in availablePluginGroups)
            items.Add(new SelectListItem { Value = @group, Text = @group });

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PreparePluginGroupsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available return request statuses
    /// </summary>
    /// <param name="items">Return request status items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareReturnRequestStatusesAsync(IList<SelectListItem> items,
        bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareReturnRequestStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available return request statuses
        var availableStatusItems = await ReturnRequestStatus.Pending.ToSelectListAsync(false);
		_logger.LogError("PrepareReturnRequestStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableStatusItems → {AvailableStatusItems}", items, withSpecialDefaultItem, defaultItemText, availableStatusItems);
        foreach (var statusItem in availableStatusItems)
        {
            items.Add(statusItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareReturnRequestStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available product templates
    /// </summary>
    /// <param name="items">Product template items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareProductTemplatesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareProductTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available product templates
        var availableTemplates = await _productTemplateService.GetAllProductTemplatesAsync();
		_logger.LogError("PrepareProductTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableTemplates → {AvailableTemplates}", items, withSpecialDefaultItem, defaultItemText, availableTemplates);
        foreach (var template in availableTemplates)
        {
            items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareProductTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available topic templates
    /// </summary>
    /// <param name="items">Topic template items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareTopicTemplatesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareTopicTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available topic templates
        var availableTemplates = await _topicTemplateService.GetAllTopicTemplatesAsync();
		_logger.LogError("PrepareTopicTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableTemplates → {AvailableTemplates}", items, withSpecialDefaultItem, defaultItemText, availableTemplates);
        foreach (var template in availableTemplates)
        {
            items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareTopicTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available warehouses
    /// </summary>
    /// <param name="items">Warehouse items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareWarehousesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareWarehousesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available warehouses
        var availableWarehouses = await _warehouseService.GetAllWarehousesAsync();
		_logger.LogError("PrepareWarehousesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableWarehouses → {AvailableWarehouses}", items, withSpecialDefaultItem, defaultItemText, availableWarehouses);
        foreach (var warehouse in availableWarehouses)
        {
            items.Add(new SelectListItem { Value = warehouse.Id.ToString(), Text = warehouse.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareWarehousesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available delivery dates
    /// </summary>
    /// <param name="items">Delivery date items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareDeliveryDatesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareDeliveryDatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available delivery dates
        var availableDeliveryDates = await _dateRangeService.GetAllDeliveryDatesAsync();
		_logger.LogError("PrepareDeliveryDatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableDeliveryDates → {AvailableDeliveryDates}", items, withSpecialDefaultItem, defaultItemText, availableDeliveryDates);
        foreach (var date in availableDeliveryDates)
        {
            items.Add(new SelectListItem { Value = date.Id.ToString(), Text = date.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareDeliveryDatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available product availability ranges
    /// </summary>
    /// <param name="items">Product availability range items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareProductAvailabilityRangesAsync(IList<SelectListItem> items,
        bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareProductAvailabilityRangesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available product availability ranges
        var availableProductAvailabilityRanges = await _dateRangeService.GetAllProductAvailabilityRangesAsync();
		_logger.LogError("PrepareProductAvailabilityRangesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableProductAvailabilityRanges → {AvailableProductAvailabilityRanges}", items, withSpecialDefaultItem, defaultItemText, availableProductAvailabilityRanges);
        foreach (var range in availableProductAvailabilityRanges)
        {
            items.Add(new SelectListItem { Value = range.Id.ToString(), Text = range.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareProductAvailabilityRangesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available GDPR request types
    /// </summary>
    /// <param name="items">Request type items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareGdprRequestTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareGdprRequestTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available request types
        var gdprRequestTypeItems = await GdprRequestType.ConsentAgree.ToSelectListAsync(false);
		_logger.LogError("PrepareGdprRequestTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): gdprRequestTypeItems → {GdprRequestTypeItems}", items, withSpecialDefaultItem, defaultItemText, gdprRequestTypeItems);
        foreach (var gdprRequestTypeItem in gdprRequestTypeItems)
        {
            items.Add(gdprRequestTypeItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogWarning("<<< Exiting PrepareGdprRequestTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available specification attribute groups
    /// </summary>
    /// <param name="items">Specification attributes</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareSpecificationAttributeGroupsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
		_logger.LogError(">>> Entering PrepareSpecificationAttributeGroupsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available specification attribute groups
        var availableSpecificationAttributeGroups = await _specificationAttributeService.GetSpecificationAttributeGroupsAsync();
		_logger.LogError("PrepareSpecificationAttributeGroupsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableSpecificationAttributeGroups → {AvailableSpecificationAttributeGroups}", items, withSpecialDefaultItem, defaultItemText, availableSpecificationAttributeGroups);
        foreach (var group in availableSpecificationAttributeGroups)
        {
            items.Add(new SelectListItem { Value = group.Id.ToString(), Text = group.Name });
        }

        // use empty string for nullable field
        var defaultItemValue = string.Empty;

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText, defaultItemValue);
		_logger.LogWarning("<<< Exiting PrepareSpecificationAttributeGroupsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }    

    /// <summary>
    /// Prepare translation supported model
    /// </summary>
    /// <param name="model">Translation supported model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PreparePreTranslationSupportModelAsync(ITranslationSupportedModel model)
    {
		_logger.LogError(">>> Entering PreparePreTranslationSupportModelAsync(model={Model})", model);
        if (!_translationSettings.AllowPreTranslate)
            return;

        var allLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
		_logger.LogError("PreparePreTranslationSupportModelAsync(model={Model}): allLanguages → {AllLanguages}", model, allLanguages);
        model.PreTranslationAvailable = allLanguages.Any(l =>
            !_translationSettings.NotTranslateLanguages.Contains(l.Id) &&
            l.Id != _translationSettings.TranslateFromLanguageId);
			_logger.LogWarning("<<< Exiting PreparePreTranslationSupportModelAsync(model={Model})", model);
    }

    #endregion
}