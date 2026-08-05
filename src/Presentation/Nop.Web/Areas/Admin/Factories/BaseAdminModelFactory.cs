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
        _categoryService = categoryService;
        _categoryTemplateService = categoryTemplateService;
        _countryService = countryService;
        _currencyService = currencyService;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _dateRangeService = dateRangeService;
        _dateTimeHelper = dateTimeHelper;
        _emailAccountService = emailAccountService;
        _languageService = languageService;
        _localizationService = localizationService;
        _manufacturerService = manufacturerService;
        _manufacturerTemplateService = manufacturerTemplateService;
        _newsLetterSubscriptionTypeService = newsLetterSubscriptionTypeService;
        _pluginService = pluginService;
        _productTemplateService = productTemplateService;
        _specificationAttributeService = specificationAttributeService;
        _stateProvinceService = stateProvinceService;
        _staticCacheManager = staticCacheManager;
        _storeService = storeService;
        _taxCategoryService = taxCategoryService;
        _topicTemplateService = topicTemplateService;
        _vendorService = vendorService;
        _warehouseService = warehouseService;
        _translationSettings = translationSettings;
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
		_logger.LogDebug(">>> Entering PrepareDefaultItemAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText},defaultItemValue={DefaultItemValue})", items, withSpecialDefaultItem, defaultItemText, defaultItemValue);
        ArgumentNullException.ThrowIfNull(items);

        //whether to insert the first special item for the default value
        if (!withSpecialDefaultItem)
            return;

        //prepare item text
        defaultItemText ??= await _localizationService.GetResourceAsync("Admin.Common.All");

        //insert this default item at first
        items.Insert(0, new SelectListItem { Text = defaultItemText, Value = defaultItemValue });
		_logger.LogDebug("<<< Exiting PrepareDefaultItemAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText},defaultItemValue={DefaultItemValue})", items, withSpecialDefaultItem, defaultItemText, defaultItemValue);
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
		_logger.LogDebug(">>> Entering GetCategoryListAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var categories = await _staticCacheManager.GetAsync(NopModelCacheDefaults.CategoriesListKey, async () => await _categoryService.GetAllCategoriesAsync(showHidden: true));

		_logger.LogDebug("GetCategoryListAsync(): categories → {Categories}", categories);
        var result = new List<SelectListItem>();
        foreach (var category in categories)
        {
		_logger.LogDebug("GetCategoryListAsync(): result → {Result}", result);
            if (!await _categoryService.CanVendorAddProductsAsync(category, categories))
                continue;

            result.Add(new SelectListItem
            {
                Text = await _categoryService.GetFormattedBreadCrumbAsync(category, categories),
                Value = category.Id.ToString()
            });
        }

		_logger.LogInformation("GetCategoryListAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetCategoryListAsync()");
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
		_logger.LogDebug(">>> Entering GetManufacturerListAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var listItems = await _staticCacheManager.GetAsync(NopModelCacheDefaults.ManufacturersListKey, async () =>
        {
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true);
            return manufacturers.Select(m => new SelectListItem
            {
			_logger.LogDebug("GetManufacturerListAsync(): manufacturers → {Manufacturers}", manufacturers);
                Text = m.Name,
                Value = m.Id.ToString()
            });
        });

        var result = new List<SelectListItem>();
        //clone the list to ensure that "selected" property is not set
		_logger.LogDebug("GetManufacturerListAsync(): result → {Result}", result);
        foreach (var item in listItems)
        {
            result.Add(new SelectListItem
            {
                Text = item.Text,
                Value = item.Value
            });
        }

		_logger.LogInformation("GetManufacturerListAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetManufacturerListAsync()");
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
		_logger.LogDebug(">>> Entering GetVendorListAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var listItems = await _staticCacheManager.GetAsync(NopModelCacheDefaults.VendorsListKey, async () =>
        {
            var vendors = await _vendorService.GetAllVendorsAsync(showHidden: true);
            return vendors.Select(v => new SelectListItem
            {
			_logger.LogDebug("GetVendorListAsync(): vendors → {Vendors}", vendors);
                Text = v.Name,
                Value = v.Id.ToString()
            });
        });

        var result = new List<SelectListItem>();
        //clone the list to ensure that "selected" property is not set
		_logger.LogDebug("GetVendorListAsync(): result → {Result}", result);
        foreach (var item in listItems)
        {
            result.Add(new SelectListItem
            {
                Text = item.Text,
                Value = item.Value
            });
        }

		_logger.LogInformation("GetVendorListAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetVendorListAsync()");
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
		_logger.LogDebug(">>> Entering PrepareActivityLogTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available activity log types
        var availableActivityTypes = await _customerActivityService.GetAllActivityTypesAsync();
        foreach (var activityType in availableActivityTypes)
        {
		_logger.LogDebug("PrepareActivityLogTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableActivityTypes → {AvailableActivityTypes}", items, withSpecialDefaultItem, defaultItemText, availableActivityTypes);
            items.Add(new SelectListItem { Value = activityType.Id.ToString(), Text = activityType.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareActivityLogTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareOrderStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available order statuses
        var availableStatusItems = await OrderStatus.Pending.ToSelectListAsync(false);
        foreach (var statusItem in availableStatusItems)
        {
		_logger.LogDebug("PrepareOrderStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableStatusItems → {AvailableStatusItems}", items, withSpecialDefaultItem, defaultItemText, availableStatusItems);
            items.Add(statusItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareOrderStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PreparePaymentStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available payment statuses
        var availableStatusItems = await PaymentStatus.Pending.ToSelectListAsync(false);
        foreach (var statusItem in availableStatusItems)
        {
		_logger.LogDebug("PreparePaymentStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableStatusItems → {AvailableStatusItems}", items, withSpecialDefaultItem, defaultItemText, availableStatusItems);
            items.Add(statusItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PreparePaymentStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareShippingStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available shipping statuses
        var availableStatusItems = await ShippingStatus.NotYetShipped.ToSelectListAsync(false);
        foreach (var statusItem in availableStatusItems)
        {
		_logger.LogDebug("PrepareShippingStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableStatusItems → {AvailableStatusItems}", items, withSpecialDefaultItem, defaultItemText, availableStatusItems);
            items.Add(statusItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareShippingStatusesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareCountriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available countries
        var availableCountries = await _countryService.GetAllCountriesAsync(showHidden: true);
        foreach (var country in availableCountries)
        {
		_logger.LogDebug("PrepareCountriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableCountries → {AvailableCountries}", items, withSpecialDefaultItem, defaultItemText, availableCountries);
            items.Add(new SelectListItem { Value = country.Id.ToString(), Text = country.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText ?? await _localizationService.GetResourceAsync("Admin.Address.SelectCountry"));
		_logger.LogDebug("<<< Exiting PrepareCountriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
        ArgumentNullException.ThrowIfNull(items);

        if (countryId.HasValue)
        {
            //prepare available states and provinces of the country
            var availableStates = await _stateProvinceService.GetStateProvincesByCountryIdAsync(countryId.Value, showHidden: true);
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
		_logger.LogDebug(">>> Entering PrepareLanguagesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available languages
        var availableLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
        foreach (var language in availableLanguages)
        {
		_logger.LogDebug("PrepareLanguagesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableLanguages → {AvailableLanguages}", items, withSpecialDefaultItem, defaultItemText, availableLanguages);
            items.Add(new SelectListItem { Value = language.Id.ToString(), Text = language.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareLanguagesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareStoresAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available stores
        var availableStores = await _storeService.GetAllStoresAsync();
        foreach (var store in availableStores)
        {
		_logger.LogDebug("PrepareStoresAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableStores → {AvailableStores}", items, withSpecialDefaultItem, defaultItemText, availableStores);
            items.Add(new SelectListItem { Value = store.Id.ToString(), Text = store.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareStoresAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareCustomerRolesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available customer roles
        var availableCustomerRoles = await _customerService.GetAllCustomerRolesAsync();
        foreach (var customerRole in availableCustomerRoles)
        {
		_logger.LogDebug("PrepareCustomerRolesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableCustomerRoles → {AvailableCustomerRoles}", items, withSpecialDefaultItem, defaultItemText, availableCustomerRoles);
            items.Add(new SelectListItem { Value = customerRole.Id.ToString(), Text = customerRole.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareCustomerRolesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareSubscriptionTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available newsletter subscription types
        var availableSubscriptionTypes = await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync();
        foreach (var subscriptionType in availableSubscriptionTypes)
        {
		_logger.LogDebug("PrepareSubscriptionTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableSubscriptionTypes → {AvailableSubscriptionTypes}", items, withSpecialDefaultItem, defaultItemText, availableSubscriptionTypes);
            items.Add(new SelectListItem { Value = subscriptionType.Id.ToString(), Text = subscriptionType.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareSubscriptionTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareEmailAccountsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available email accounts
        var availableEmailAccounts = await _emailAccountService.GetAllEmailAccountsAsync();
        foreach (var emailAccount in availableEmailAccounts)
        {
		_logger.LogDebug("PrepareEmailAccountsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableEmailAccounts → {AvailableEmailAccounts}", items, withSpecialDefaultItem, defaultItemText, availableEmailAccounts);
            items.Add(new SelectListItem { Value = emailAccount.Id.ToString(), Text = $"{emailAccount.DisplayName} ({emailAccount.Email})" });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareEmailAccountsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareTaxCategoriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available tax categories
        var availableTaxCategories = await _taxCategoryService.GetAllTaxCategoriesAsync();
        foreach (var taxCategory in availableTaxCategories)
        {
		_logger.LogDebug("PrepareTaxCategoriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableTaxCategories → {AvailableTaxCategories}", items, withSpecialDefaultItem, defaultItemText, availableTaxCategories);
            items.Add(new SelectListItem { Value = taxCategory.Id.ToString(), Text = taxCategory.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem,
            defaultItemText ?? await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Tax.TaxCategories.None"));
			_logger.LogDebug("<<< Exiting PrepareTaxCategoriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareCategoriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available categories
        var availableCategoryItems = await GetCategoryListAsync();
        foreach (var categoryItem in availableCategoryItems)
        {
		_logger.LogDebug("PrepareCategoriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableCategoryItems → {AvailableCategoryItems}", items, withSpecialDefaultItem, defaultItemText, availableCategoryItems);
            items.Add(categoryItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareCategoriesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareManufacturersAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available manufacturers
        var availableManufacturerItems = await GetManufacturerListAsync();
        foreach (var manufacturerItem in availableManufacturerItems)
        {
		_logger.LogDebug("PrepareManufacturersAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableManufacturerItems → {AvailableManufacturerItems}", items, withSpecialDefaultItem, defaultItemText, availableManufacturerItems);
            items.Add(manufacturerItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareManufacturersAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareVendorsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available vendors
        var availableVendorItems = await GetVendorListAsync();
        foreach (var vendorItem in availableVendorItems)
        {
		_logger.LogDebug("PrepareVendorsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableVendorItems → {AvailableVendorItems}", items, withSpecialDefaultItem, defaultItemText, availableVendorItems);
            items.Add(vendorItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareVendorsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareProductTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available product types
        var availableProductTypeItems = await ProductType.SimpleProduct.ToSelectListAsync(false);
        foreach (var productTypeItem in availableProductTypeItems)
        {
		_logger.LogDebug("PrepareProductTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableProductTypeItems → {AvailableProductTypeItems}", items, withSpecialDefaultItem, defaultItemText, availableProductTypeItems);
            items.Add(productTypeItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareProductTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareCategoryTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available category templates
        var availableTemplates = await _categoryTemplateService.GetAllCategoryTemplatesAsync();
        foreach (var template in availableTemplates)
        {
		_logger.LogDebug("PrepareCategoryTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableTemplates → {AvailableTemplates}", items, withSpecialDefaultItem, defaultItemText, availableTemplates);
            items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareCategoryTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareTimeZonesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available time zones
        var availableTimeZones = _dateTimeHelper.GetSystemTimeZones();
        foreach (var timeZone in availableTimeZones)
        {
		_logger.LogDebug("PrepareTimeZonesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableTimeZones → {AvailableTimeZones}", items, withSpecialDefaultItem, defaultItemText, availableTimeZones);
            items.Add(new SelectListItem { Value = timeZone.Id, Text = timeZone.DisplayName });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareTimeZonesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareShoppingCartTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available shopping cart types
        var availableShoppingCartTypeItems = await ShoppingCartType.ShoppingCart.ToSelectListAsync(false);
        foreach (var shoppingCartTypeItem in availableShoppingCartTypeItems)
        {
		_logger.LogDebug("PrepareShoppingCartTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableShoppingCartTypeItems → {AvailableShoppingCartTypeItems}", items, withSpecialDefaultItem, defaultItemText, availableShoppingCartTypeItems);
            items.Add(shoppingCartTypeItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareShoppingCartTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareTaxDisplayTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available tax display types
        var availableTaxDisplayTypeItems = await TaxDisplayType.ExcludingTax.ToSelectListAsync(false);
        foreach (var taxDisplayTypeItem in availableTaxDisplayTypeItems)
        {
		_logger.LogDebug("PrepareTaxDisplayTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableTaxDisplayTypeItems → {AvailableTaxDisplayTypeItems}", items, withSpecialDefaultItem, defaultItemText, availableTaxDisplayTypeItems);
            items.Add(taxDisplayTypeItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareTaxDisplayTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareCurrenciesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available currencies
        var availableCurrencies = await _currencyService.GetAllCurrenciesAsync(true);
        foreach (var currency in availableCurrencies)
        {
		_logger.LogDebug("PrepareCurrenciesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableCurrencies → {AvailableCurrencies}", items, withSpecialDefaultItem, defaultItemText, availableCurrencies);
            items.Add(new SelectListItem { Value = currency.Id.ToString(), Text = currency.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareCurrenciesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareDiscountTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available discount types
        var availableDiscountTypeItems = await DiscountType.AssignedToOrderTotal.ToSelectListAsync(false);
        foreach (var discountTypeItem in availableDiscountTypeItems)
        {
		_logger.LogDebug("PrepareDiscountTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableDiscountTypeItems → {AvailableDiscountTypeItems}", items, withSpecialDefaultItem, defaultItemText, availableDiscountTypeItems);
            items.Add(discountTypeItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareDiscountTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareLogLevelsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available log levels
        var availableLogLevelItems = await LogLevel.Debug.ToSelectListAsync(false);
        foreach (var logLevelItem in availableLogLevelItems)
        {
		_logger.LogDebug("PrepareLogLevelsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableLogLevelItems → {AvailableLogLevelItems}", items, withSpecialDefaultItem, defaultItemText, availableLogLevelItems);
            items.Add(logLevelItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareLogLevelsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
        ArgumentNullException.ThrowIfNull(items);

        //prepare available manufacturer templates
        var availableTemplates = await _manufacturerTemplateService.GetAllManufacturerTemplatesAsync();
        foreach (var template in availableTemplates)
        {
            items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareLoadPluginModesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available load plugin modes
        var availableLoadPluginModeItems = await LoadPluginsMode.All.ToSelectListAsync(false);
        foreach (var loadPluginModeItem in availableLoadPluginModeItems)
        {
		_logger.LogDebug("PrepareLoadPluginModesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableLoadPluginModeItems → {AvailableLoadPluginModeItems}", items, withSpecialDefaultItem, defaultItemText, availableLoadPluginModeItems);
            items.Add(loadPluginModeItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareLoadPluginModesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PreparePluginGroupsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available plugin groups
        var availablePluginGroups = (await _pluginService.GetPluginDescriptorsAsync<IPlugin>(LoadPluginsMode.All))
            .Select(plugin => plugin.Group).Distinct().OrderBy(groupName => groupName).ToList();
        foreach (var group in availablePluginGroups)
            items.Add(new SelectListItem { Value = @group, Text = @group });

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PreparePluginGroupsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
        ArgumentNullException.ThrowIfNull(items);

        //prepare available return request statuses
        var availableStatusItems = await ReturnRequestStatus.Pending.ToSelectListAsync(false);
        foreach (var statusItem in availableStatusItems)
        {
            items.Add(statusItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareProductTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available product templates
        var availableTemplates = await _productTemplateService.GetAllProductTemplatesAsync();
        foreach (var template in availableTemplates)
        {
		_logger.LogDebug("PrepareProductTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableTemplates → {AvailableTemplates}", items, withSpecialDefaultItem, defaultItemText, availableTemplates);
            items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareProductTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareTopicTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available topic templates
        var availableTemplates = await _topicTemplateService.GetAllTopicTemplatesAsync();
        foreach (var template in availableTemplates)
        {
		_logger.LogDebug("PrepareTopicTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableTemplates → {AvailableTemplates}", items, withSpecialDefaultItem, defaultItemText, availableTemplates);
            items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareTopicTemplatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareWarehousesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available warehouses
        var availableWarehouses = await _warehouseService.GetAllWarehousesAsync();
        foreach (var warehouse in availableWarehouses)
        {
		_logger.LogDebug("PrepareWarehousesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableWarehouses → {AvailableWarehouses}", items, withSpecialDefaultItem, defaultItemText, availableWarehouses);
            items.Add(new SelectListItem { Value = warehouse.Id.ToString(), Text = warehouse.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareWarehousesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareDeliveryDatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available delivery dates
        var availableDeliveryDates = await _dateRangeService.GetAllDeliveryDatesAsync();
        foreach (var date in availableDeliveryDates)
        {
		_logger.LogDebug("PrepareDeliveryDatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableDeliveryDates → {AvailableDeliveryDates}", items, withSpecialDefaultItem, defaultItemText, availableDeliveryDates);
            items.Add(new SelectListItem { Value = date.Id.ToString(), Text = date.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareDeliveryDatesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
        ArgumentNullException.ThrowIfNull(items);

        //prepare available product availability ranges
        var availableProductAvailabilityRanges = await _dateRangeService.GetAllProductAvailabilityRangesAsync();
        foreach (var range in availableProductAvailabilityRanges)
        {
            items.Add(new SelectListItem { Value = range.Id.ToString(), Text = range.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareGdprRequestTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available request types
        var gdprRequestTypeItems = await GdprRequestType.ConsentAgree.ToSelectListAsync(false);
        foreach (var gdprRequestTypeItem in gdprRequestTypeItems)
        {
		_logger.LogDebug("PrepareGdprRequestTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): gdprRequestTypeItems → {GdprRequestTypeItems}", items, withSpecialDefaultItem, defaultItemText, gdprRequestTypeItems);
            items.Add(gdprRequestTypeItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
		_logger.LogDebug("<<< Exiting PrepareGdprRequestTypesAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
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
		_logger.LogDebug(">>> Entering PrepareSpecificationAttributeGroupsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
        ArgumentNullException.ThrowIfNull(items);

        //prepare available specification attribute groups
        var availableSpecificationAttributeGroups = await _specificationAttributeService.GetSpecificationAttributeGroupsAsync();
        foreach (var group in availableSpecificationAttributeGroups)
        {
		_logger.LogDebug("PrepareSpecificationAttributeGroupsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText}): availableSpecificationAttributeGroups → {AvailableSpecificationAttributeGroups}", items, withSpecialDefaultItem, defaultItemText, availableSpecificationAttributeGroups);
            items.Add(new SelectListItem { Value = group.Id.ToString(), Text = group.Name });
        }

        // use empty string for nullable field
        var defaultItemValue = string.Empty;

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText, defaultItemValue);
		_logger.LogDebug("<<< Exiting PrepareSpecificationAttributeGroupsAsync(items={Items},withSpecialDefaultItem={WithSpecialDefaultItem},defaultItemText={DefaultItemText})", items, withSpecialDefaultItem, defaultItemText);
    }    

    /// <summary>
    /// Prepare translation supported model
    /// </summary>
    /// <param name="model">Translation supported model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PreparePreTranslationSupportModelAsync(ITranslationSupportedModel model)
    {
		_logger.LogDebug(">>> Entering PreparePreTranslationSupportModelAsync(model={Model})", model);
        if (!_translationSettings.AllowPreTranslate)
            return;

        var allLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
        model.PreTranslationAvailable = allLanguages.Any(l =>
            !_translationSettings.NotTranslateLanguages.Contains(l.Id) &&
            l.Id != _translationSettings.TranslateFromLanguageId);
			_logger.LogDebug("PreparePreTranslationSupportModelAsync(model={Model}): allLanguages → {AllLanguages}", model, allLanguages);
			_logger.LogDebug("<<< Exiting PreparePreTranslationSupportModelAsync(model={Model})", model);
    }

    #endregion
}