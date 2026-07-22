using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Core.Security;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.ScheduleTasks;
using Nop.Services.Vendors;
using Nop.Web.Framework.Globalization;

namespace Nop.Web.Framework;

/// <summary>
/// Represents work context for web application
/// </summary>
public partial class WebWorkContext : IWorkContext
{
    #region Fields

    protected readonly CookieSettings _cookieSettings;
    protected readonly CurrencySettings _currencySettings;
    protected readonly IAuthenticationService _authenticationService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILanguageService _languageService;
    protected readonly IStoreContext _storeContext;
    protected readonly IUserAgentHelper _userAgentHelper;
    protected readonly IVendorService _vendorService;
    protected readonly IWebHelper _webHelper;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly TaxSettings _taxSettings;

    protected Customer _cachedCustomer;
    protected Customer _originalCustomerIfImpersonated;
    protected Vendor _cachedVendor;
    protected Language _cachedLanguage;
    protected Currency _cachedCurrency;
    protected TaxDisplayType? _cachedTaxDisplayType;

    #endregion

    #region Ctor

    public WebWorkContext(CookieSettings cookieSettings,
        CurrencySettings currencySettings,
        IAuthenticationService authenticationService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IEventPublisher eventPublisher,
        IGenericAttributeService genericAttributeService,
        IHttpContextAccessor httpContextAccessor,
        ILanguageService languageService,
        IStoreContext storeContext,
        IUserAgentHelper userAgentHelper,
        IVendorService vendorService,
        IWebHelper webHelper,
        LocalizationSettings localizationSettings,
        TaxSettings taxSettings)
    {
        _cookieSettings = cookieSettings;
        _currencySettings = currencySettings;
        _authenticationService = authenticationService;
        _currencyService = currencyService;
        _customerService = customerService;
        _eventPublisher = eventPublisher;
        _genericAttributeService = genericAttributeService;
        _httpContextAccessor = httpContextAccessor;
        _languageService = languageService;
        _storeContext = storeContext;
        _userAgentHelper = userAgentHelper;
        _vendorService = vendorService;
        _webHelper = webHelper;
        _localizationSettings = localizationSettings;
        _taxSettings = taxSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get nop customer cookie
    /// </summary>
    /// <returns>String value of cookie</returns>
    protected virtual string GetCustomerCookie()
    {
		logger.LogDebug(">>> Entering GetCustomerCookie()");
        var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CustomerCookie}";
		logger.LogDebug("GetCustomerCookie(): cookieName → {CookieName}", cookieName);
		logger.LogDebug("<<< Exiting GetCustomerCookie()");
        return _httpContextAccessor.HttpContext?.Request?.Cookies[cookieName];
    }

    /// <summary>
    /// Set nop customer cookie
    /// </summary>
    /// <param name="customerGuid">Guid of the customer</param>
    protected virtual void SetCustomerCookie(Guid customerGuid)
    {
		logger.LogDebug(">>> Entering SetCustomerCookie(customerGuid={CustomerGuid})", customerGuid);
        if (_httpContextAccessor.HttpContext?.Response.HasStarted ?? true)
            return;

        //delete current cookie value
        var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CustomerCookie}";
        _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);
		logger.LogDebug("SetCustomerCookie(customerGuid={CustomerGuid}): cookieName → {CookieName}", customerGuid, cookieName);

        //get date of cookie expiration
        var cookieExpires = _cookieSettings.CustomerCookieExpires;
        var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

		logger.LogDebug("SetCustomerCookie(customerGuid={CustomerGuid}): cookieExpiresDate → {CookieExpiresDate}", customerGuid, cookieExpiresDate);
        //if passed guid is empty set cookie as expired
        if (customerGuid == Guid.Empty)
            cookieExpiresDate = DateTime.Now.AddMonths(-1);

        //set new cookie value
        var options = new CookieOptions
        {
            HttpOnly = true,
            Expires = cookieExpiresDate,
            Secure = _webHelper.IsCurrentConnectionSecured()
        };
        _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, customerGuid.ToString(), options);
		logger.LogDebug("<<< Exiting SetCustomerCookie(customerGuid={CustomerGuid})", customerGuid);
    }

    /// <summary>
    /// Set language culture cookie
    /// </summary>
    /// <param name="language">Language</param>
    protected virtual void SetLanguageCookie(Language language)
    {
        if (_httpContextAccessor.HttpContext?.Response.HasStarted ?? true)
            return;

        //delete current cookie value
        var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CultureCookie}";
        _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

        if (string.IsNullOrEmpty(language?.LanguageCulture))
            return;

        //set new cookie value
        var value = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language.LanguageCulture));
        var options = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) };
        _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, value, options);
		logger.LogDebug("<<< Exiting SetLanguageCookie(language={Language})", language);
    }

    /// <summary>
    /// Get language from the request
    /// </summary>
    /// <param name="storeId">Get a language that is only allowed for the specified store; pass 0 to ignore limitations</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the found language
    /// </returns>
    protected virtual async Task<Language> GetLanguageFromRequestAsync(int storeId = 0)
    {
		logger.LogDebug(">>> Entering GetLanguageFromRequestAsync(storeId={StoreId})", storeId);
        var requestCultureFeature = _httpContextAccessor.HttpContext?.Features.Get<IRequestCultureFeature>();
        if (requestCultureFeature is null)
			logger.LogDebug("GetLanguageFromRequestAsync(storeId={StoreId}): requestCultureFeature → {RequestCultureFeature}", storeId, requestCultureFeature);
            return null;

        //whether we should detect the current language by customer settings
        if (requestCultureFeature.Provider is not NopSeoUrlCultureProvider && !_localizationSettings.AutomaticallyDetectLanguage)
            return null;

        //get request culture
        if (requestCultureFeature.RequestCulture is null)
            return null;

        //try to get language by culture name
        var store = await _storeContext.GetCurrentStoreAsync();
        var requestLanguage = (await _languageService.GetAllLanguagesAsync(storeId: storeId)).FirstOrDefault(language =>
            language.LanguageCulture.Equals(requestCultureFeature.RequestCulture.Culture.Name, StringComparison.InvariantCultureIgnoreCase));
			logger.LogDebug("GetLanguageFromRequestAsync(storeId={StoreId}): store → {Store}", storeId, store);

		logger.LogDebug("<<< Exiting GetLanguageFromRequestAsync(storeId={StoreId})", storeId);
        return requestLanguage;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current customer
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<Customer> GetCurrentCustomerAsync()
    {
		logger.LogDebug(">>> Entering GetCurrentCustomerAsync()");
        //whether there is a cached value
        if (_cachedCustomer != null)
            return _cachedCustomer;

        await SetCurrentCustomerAsync();

		logger.LogDebug("<<< Exiting GetCurrentCustomerAsync()");
        return _cachedCustomer;
    }

    /// <summary>
    /// Sets the current customer
    /// </summary>
    /// <param name="customer">Current customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetCurrentCustomerAsync(Customer customer = null)
    {
		logger.LogDebug(">>> Entering SetCurrentCustomerAsync(customer={Customer})", customer);
        if (customer == null)
        {
			logger.LogDebug("SetCurrentCustomerAsync(customer={Customer}): customer → {Customer1}", customer, customer);
            //check whether request is made by a background (schedule) task
            if (_httpContextAccessor.HttpContext?.Request
                    ?.Path.Equals(new PathString($"/{NopTaskDefaults.ScheduleTaskPath}"), StringComparison.InvariantCultureIgnoreCase)
                ?? true)
            {
                //in this case return built-in customer record for background task
                customer = await _customerService.GetOrCreateBackgroundTaskUserAsync();
            }

            if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
            {
                //check whether request is made by a search engine, in this case return built-in customer record for search engines
                if (_userAgentHelper.IsSearchEngine())
                    customer = await _customerService.GetOrCreateSearchEngineUserAsync();
            }

            if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
            {
                //try to get registered user
                customer = await _authenticationService.GetAuthenticatedCustomerAsync();
            }

            if (customer != null && !customer.Deleted && customer.Active && !customer.RequireReLogin)
            {
                //get impersonate user if required
                var impersonatedCustomerId = await _genericAttributeService
                    .GetAttributeAsync<int?>(customer, NopCustomerDefaults.ImpersonatedCustomerIdAttribute);
                if (impersonatedCustomerId.HasValue && impersonatedCustomerId.Value > 0)
                {
                    var impersonatedCustomer = await _customerService.GetCustomerByIdAsync(impersonatedCustomerId.Value);
                    if (impersonatedCustomer != null && !impersonatedCustomer.Deleted &&
						logger.LogDebug("SetCurrentCustomerAsync(customer={Customer}): impersonatedCustomer → {ImpersonatedCustomer}", customer, impersonatedCustomer);
                        impersonatedCustomer.Active &&
                        !impersonatedCustomer.RequireReLogin)
                    {
                        //set impersonated customer
                        _originalCustomerIfImpersonated = customer;
						logger.LogDebug("SetCurrentCustomerAsync(customer={Customer}): _originalCustomerIfImpersonated → {_originalCustomerIfImpersonated}", customer, _originalCustomerIfImpersonated);
                        customer = impersonatedCustomer;
                    }
                }
            }

            if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
            {
                //get guest customer
                var customerCookie = GetCustomerCookie();
                if (Guid.TryParse(customerCookie, out var customerGuid))
				logger.LogDebug("SetCurrentCustomerAsync(customer={Customer}): customerCookie → {CustomerCookie}", customer, customerCookie);
                {
                    //get customer from cookie (should not be registered)
                    var customerByCookie = await _customerService.GetCustomerByGuidAsync(customerGuid);
                    if (customerByCookie != null && !await _customerService.IsRegisteredAsync(customerByCookie))
						logger.LogDebug("SetCurrentCustomerAsync(customer={Customer}): customerByCookie → {CustomerByCookie}", customer, customerByCookie);
                        customer = customerByCookie;
                }
            }

            if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
            {
                //create guest if not exists
                customer = await _customerService.InsertGuestCustomerAsync();
            }
        }

        if (!customer.Deleted && customer.Active && !customer.RequireReLogin)
        {
            //set customer cookie
            SetCustomerCookie(customer.CustomerGuid);

            //cache the found customer
            _cachedCustomer = customer;
			logger.LogDebug("SetCurrentCustomerAsync(customer={Customer}): _cachedCustomer → {_cachedCustomer}", customer, _cachedCustomer);
        }
		logger.LogDebug("<<< Exiting SetCurrentCustomerAsync(customer={Customer})", customer);
    }

    /// <summary>
    /// Gets the original customer (in case the current one is impersonated)
    /// </summary>
    public virtual Customer OriginalCustomerIfImpersonated => _originalCustomerIfImpersonated;

    /// <summary>
    /// Gets the current vendor (logged-in manager)
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<Vendor> GetCurrentVendorAsync()
    {
		logger.LogDebug(">>> Entering GetCurrentVendorAsync()");
        //whether there is a cached value
        if (_cachedVendor != null)
            return _cachedVendor;

        var customer = await GetCurrentCustomerAsync();
        if (customer == null)
			logger.LogDebug("GetCurrentVendorAsync(): customer → {Customer}", customer);
            return null;

        //check vendor availability
        var vendor = await _vendorService.GetVendorByIdAsync(customer.VendorId);
        if (vendor == null || vendor.Deleted || !vendor.Active)
			logger.LogDebug("GetCurrentVendorAsync(): vendor → {Vendor}", vendor);
            return null;

        //cache the found vendor
        _cachedVendor = vendor;
		logger.LogDebug("GetCurrentVendorAsync(): _cachedVendor → {_cachedVendor}", _cachedVendor);

		logger.LogDebug("<<< Exiting GetCurrentVendorAsync()");
        return _cachedVendor;
    }

    /// <summary>
    /// Sets current user working language
    /// </summary>
    /// <param name="language">Language</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetWorkingLanguageAsync(Language language)
    {
        //save passed language identifier
        var customer = await GetCurrentCustomerAsync();

        if (!customer.IsSystemAccount)
        {
            customer.LanguageId = language?.Id;
            await _customerService.UpdateCustomerAsync(customer);

            //raise event
            await _eventPublisher.PublishAsync(new CustomerChangeWorkingLanguageEvent(customer));
        }

        //set cookie
        SetLanguageCookie(language);

        //then reset the cached value
        _cachedLanguage = null;
    }

    /// <summary>
    /// Gets current user working language
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<Language> GetWorkingLanguageAsync()
    {
		logger.LogDebug(">>> Entering GetWorkingLanguageAsync()");
        //whether there is a cached value
        if (_cachedLanguage != null)
            return _cachedLanguage;

        var customer = await GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
		logger.LogDebug("GetWorkingLanguageAsync(): customer → {Customer}", customer);
		logger.LogDebug("GetWorkingLanguageAsync(): store → {Store}", store);

        //whether we should detect the language from the request
        var detectedLanguage = await GetLanguageFromRequestAsync(store.Id);

		logger.LogDebug("GetWorkingLanguageAsync(): detectedLanguage → {DetectedLanguage}", detectedLanguage);
        //get current saved language identifier
        var currentLanguageId = customer.LanguageId;

        //if the language is detected we need to save it
        if (detectedLanguage != null)
        {
            //save the detected language identifier if it differs from the current one
            if (detectedLanguage.Id != currentLanguageId)
                await SetWorkingLanguageAsync(detectedLanguage);
        }
        else
        {
            var allStoreLanguages = await _languageService.GetAllLanguagesAsync(storeId: store.Id);

			logger.LogDebug("GetWorkingLanguageAsync(): allStoreLanguages → {AllStoreLanguages}", allStoreLanguages);
            //check customer language availability
            detectedLanguage = allStoreLanguages.FirstOrDefault(language => language.Id == currentLanguageId);

            //it not found, then try to get the default language for the current store (if specified)
            detectedLanguage ??= allStoreLanguages.FirstOrDefault(language => language.Id == store.DefaultLanguageId);

            //if the default language for the current store not found, then try to get the first one
            detectedLanguage ??= allStoreLanguages.FirstOrDefault();

            //if there are no languages for the current store try to get the first one regardless of the store
            detectedLanguage ??= (await _languageService.GetAllLanguagesAsync()).FirstOrDefault();

            SetLanguageCookie(detectedLanguage);
        }

        //cache the found language
        _cachedLanguage = detectedLanguage;
		logger.LogDebug("GetWorkingLanguageAsync(): _cachedLanguage → {_cachedLanguage}", _cachedLanguage);

		logger.LogDebug("<<< Exiting GetWorkingLanguageAsync()");
        return _cachedLanguage;
    }

    /// <summary>
    /// Gets current user working currency
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<Currency> GetWorkingCurrencyAsync()
    {
		logger.LogDebug(">>> Entering GetWorkingCurrencyAsync()");
        //whether there is a cached value
        if (_cachedCurrency != null)
            return _cachedCurrency;

        var adminAreaUrl = $"{_webHelper.GetStoreLocation()}admin";

		logger.LogDebug("GetWorkingCurrencyAsync(): adminAreaUrl → {AdminAreaUrl}", adminAreaUrl);
        //return primary store currency when we're in admin area/mode
        if (_webHelper.GetThisPageUrl(false).StartsWith(adminAreaUrl, StringComparison.InvariantCultureIgnoreCase))
        {
            var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
            if (primaryStoreCurrency != null)
			logger.LogDebug("GetWorkingCurrencyAsync(): primaryStoreCurrency → {PrimaryStoreCurrency}", primaryStoreCurrency);
            {
                _cachedCurrency = primaryStoreCurrency;
				logger.LogDebug("GetWorkingCurrencyAsync(): _cachedCurrency → {_cachedCurrency}", _cachedCurrency);
                return primaryStoreCurrency;
            }
        }

        var customer = await GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
		logger.LogDebug("GetWorkingCurrencyAsync(): customer → {Customer}", customer);
		logger.LogDebug("GetWorkingCurrencyAsync(): store → {Store}", store);

        if (customer.IsSearchEngineAccount())
        {
            _cachedCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)
                              ?? (await _currencyService.GetAllCurrenciesAsync(storeId: store.Id)).FirstOrDefault();

            return _cachedCurrency;
        }

        var allStoreCurrencies = await _currencyService.GetAllCurrenciesAsync(storeId: store.Id);

		logger.LogDebug("GetWorkingCurrencyAsync(): allStoreCurrencies → {AllStoreCurrencies}", allStoreCurrencies);
        //check customer currency availability
        var customerCurrency = allStoreCurrencies.FirstOrDefault(currency => currency.Id == customer.CurrencyId);
        if (customerCurrency == null)
		logger.LogDebug("GetWorkingCurrencyAsync(): customerCurrency → {CustomerCurrency}", customerCurrency);
        {
            //it not found, then try to get the default currency for the current language (if specified)
            var language = await GetWorkingLanguageAsync();
            customerCurrency = allStoreCurrencies
				logger.LogDebug("GetWorkingCurrencyAsync(): language → {Language}", language);
                .FirstOrDefault(currency => currency.Id == language.DefaultCurrencyId);
        }

        //if the default currency for the current store not found, then try to get the first one
        customerCurrency ??= allStoreCurrencies.FirstOrDefault();

        //if there are no currencies for the current store try to get the first one regardless of the store
        customerCurrency ??= (await _currencyService.GetAllCurrenciesAsync()).FirstOrDefault();

        //cache the found currency
        _cachedCurrency = customerCurrency;

		logger.LogDebug("<<< Exiting GetWorkingCurrencyAsync()");
        return _cachedCurrency;
    }

    /// <summary>
    /// Sets current user working currency
    /// </summary>
    /// <param name="currency">Currency</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetWorkingCurrencyAsync(Currency currency)
    {
		logger.LogDebug(">>> Entering SetWorkingCurrencyAsync(currency={Currency})", currency);
        //save passed currency identifier
        var customer = await GetCurrentCustomerAsync();
        if (customer.IsSearchEngineAccount())
			logger.LogDebug("SetWorkingCurrencyAsync(currency={Currency}): customer → {Customer}", currency, customer);
            return;

        customer.CurrencyId = currency?.Id;
		logger.LogDebug("SetWorkingCurrencyAsync(currency={Currency}): customer.CurrencyId → {Arg1}", currency, customer.CurrencyId);
        await _customerService.UpdateCustomerAsync(customer);

        //then reset the cached value
        _cachedCurrency = null;
		logger.LogDebug("SetWorkingCurrencyAsync(currency={Currency}): _cachedCurrency → {_cachedCurrency}", currency, _cachedCurrency);
		logger.LogDebug("<<< Exiting SetWorkingCurrencyAsync(currency={Currency})", currency);
    }

    /// <summary>
    /// Gets or sets current tax display type
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<TaxDisplayType> GetTaxDisplayTypeAsync()
    {
		logger.LogDebug(">>> Entering GetTaxDisplayTypeAsync()");
        //whether there is a cached value
        if (_cachedTaxDisplayType.HasValue)
            return _cachedTaxDisplayType.Value;

        var customer = await GetCurrentCustomerAsync();
        var taxDisplayType = await _customerService.GetCustomerTaxDisplayTypeAsync(customer);
		logger.LogDebug("GetTaxDisplayTypeAsync(): customer → {Customer}", customer);
		logger.LogDebug("GetTaxDisplayTypeAsync(): taxDisplayType → {TaxDisplayType}", taxDisplayType);

        //cache the value
        _cachedTaxDisplayType = taxDisplayType;
		logger.LogDebug("GetTaxDisplayTypeAsync(): _cachedTaxDisplayType → {_cachedTaxDisplayType}", _cachedTaxDisplayType);

		logger.LogDebug("<<< Exiting GetTaxDisplayTypeAsync()");
        return _cachedTaxDisplayType.Value;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetTaxDisplayTypeAsync(TaxDisplayType taxDisplayType)
    {
        //whether customers are allowed to select tax display type
        if (!_taxSettings.AllowCustomersToSelectTaxDisplayType)
            return;

        //save passed value
        var customer = await GetCurrentCustomerAsync();
        customer.TaxDisplayType = taxDisplayType;
        await _customerService.UpdateCustomerAsync(customer);

        //then reset the cached value
        _cachedTaxDisplayType = null;
		logger.LogDebug("<<< Exiting SetTaxDisplayTypeAsync(taxDisplayType={TaxDisplayType})", taxDisplayType);
    }

    /// <summary>
    /// Gets or sets value indicating whether we're in admin area
    /// </summary>
    public virtual bool IsAdmin { get; set; }

    #endregion
}
