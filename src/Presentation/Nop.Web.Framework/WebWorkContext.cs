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
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework;

/// <summary>
/// Represents work context for web application
/// </summary>
public partial class WebWorkContext : IWorkContext
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogError(">>> Entering WebWorkContext(cookieSettings=<redacted>,currencySettings={CurrencySettings},authenticationService={AuthenticationService},currencyService={CurrencyService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},storeContext={StoreContext},userAgentHelper={UserAgentHelper},vendorService={VendorService},webHelper={WebHelper},localizationSettings={LocalizationSettings},taxSettings={TaxSettings})", currencySettings, authenticationService, currencyService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, languageService, storeContext, userAgentHelper, vendorService, webHelper, localizationSettings, taxSettings);
        _cookieSettings = cookieSettings;
        _currencySettings = currencySettings;
		_logger.LogDebug("WebWorkContext(cookieSettings=<redacted>,currencySettings={CurrencySettings},authenticationService={AuthenticationService},currencyService={CurrencyService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},storeContext={StoreContext},userAgentHelper={UserAgentHelper},vendorService={VendorService},webHelper={WebHelper},localizationSettings={LocalizationSettings},taxSettings={TaxSettings}): _currencySettings → {_currencySettings}", currencySettings, authenticationService, currencyService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, languageService, storeContext, userAgentHelper, vendorService, webHelper, localizationSettings, taxSettings, _currencySettings);
		_logger.LogDebug("WebWorkContext(cookieSettings=<redacted>,currencySettings={CurrencySettings},authenticationService={AuthenticationService},currencyService={CurrencyService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},storeContext={StoreContext},userAgentHelper={UserAgentHelper},vendorService={VendorService},webHelper={WebHelper},localizationSettings={LocalizationSettings},taxSettings={TaxSettings}): _cookieSettings → {_cookieSettings}", currencySettings, authenticationService, currencyService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, languageService, storeContext, userAgentHelper, vendorService, webHelper, localizationSettings, taxSettings, _cookieSettings);
    _logger.LogWarning("<<< Exiting WebWorkContext(cookieSettings=<redacted>,currencySettings={CurrencySettings},authenticationService={AuthenticationService},currencyService={CurrencyService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},storeContext={StoreContext},userAgentHelper={UserAgentHelper},vendorService={VendorService},webHelper={WebHelper},localizationSettings={LocalizationSettings},taxSettings={TaxSettings})", currencySettings, authenticationService, currencyService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, languageService, storeContext, userAgentHelper, vendorService, webHelper, localizationSettings, taxSettings);
        _authenticationService = authenticationService;
		_logger.LogDebug("WebWorkContext(cookieSettings=<redacted>,currencySettings={CurrencySettings},authenticationService={AuthenticationService},currencyService={CurrencyService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},storeContext={StoreContext},userAgentHelper={UserAgentHelper},vendorService={VendorService},webHelper={WebHelper},localizationSettings={LocalizationSettings},taxSettings={TaxSettings}): _authenticationService → {_authenticationService}", currencySettings, authenticationService, currencyService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, languageService, storeContext, userAgentHelper, vendorService, webHelper, localizationSettings, taxSettings, _authenticationService);
        _currencyService = currencyService;
		_logger.LogDebug("WebWorkContext(cookieSettings=<redacted>,currencySettings={CurrencySettings},authenticationService={AuthenticationService},currencyService={CurrencyService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},storeContext={StoreContext},userAgentHelper={UserAgentHelper},vendorService={VendorService},webHelper={WebHelper},localizationSettings={LocalizationSettings},taxSettings={TaxSettings}): _currencyService → {_currencyService}", currencySettings, authenticationService, currencyService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, languageService, storeContext, userAgentHelper, vendorService, webHelper, localizationSettings, taxSettings, _currencyService);
        _customerService = customerService;
		_logger.LogDebug("WebWorkContext(cookieSettings=<redacted>,currencySettings={CurrencySettings},authenticationService={AuthenticationService},currencyService={CurrencyService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},storeContext={StoreContext},userAgentHelper={UserAgentHelper},vendorService={VendorService},webHelper={WebHelper},localizationSettings={LocalizationSettings},taxSettings={TaxSettings}): _customerService → {_customerService}", currencySettings, authenticationService, currencyService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, languageService, storeContext, userAgentHelper, vendorService, webHelper, localizationSettings, taxSettings, _customerService);
        _eventPublisher = eventPublisher;
		_logger.LogDebug("WebWorkContext(cookieSettings=<redacted>,currencySettings={CurrencySettings},authenticationService={AuthenticationService},currencyService={CurrencyService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},storeContext={StoreContext},userAgentHelper={UserAgentHelper},vendorService={VendorService},webHelper={WebHelper},localizationSettings={LocalizationSettings},taxSettings={TaxSettings}): _eventPublisher → {_eventPublisher}", currencySettings, authenticationService, currencyService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, languageService, storeContext, userAgentHelper, vendorService, webHelper, localizationSettings, taxSettings, _eventPublisher);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("WebWorkContext(cookieSettings=<redacted>,currencySettings={CurrencySettings},authenticationService={AuthenticationService},currencyService={CurrencyService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},storeContext={StoreContext},userAgentHelper={UserAgentHelper},vendorService={VendorService},webHelper={WebHelper},localizationSettings={LocalizationSettings},taxSettings={TaxSettings}): _genericAttributeService → {_genericAttributeService}", currencySettings, authenticationService, currencyService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, languageService, storeContext, userAgentHelper, vendorService, webHelper, localizationSettings, taxSettings, _genericAttributeService);
        _httpContextAccessor = httpContextAccessor;
		_logger.LogDebug("WebWorkContext(cookieSettings=<redacted>,currencySettings={CurrencySettings},authenticationService={AuthenticationService},currencyService={CurrencyService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},storeContext={StoreContext},userAgentHelper={UserAgentHelper},vendorService={VendorService},webHelper={WebHelper},localizationSettings={LocalizationSettings},taxSettings={TaxSettings}): _httpContextAccessor → {_httpContextAccessor}", currencySettings, authenticationService, currencyService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, languageService, storeContext, userAgentHelper, vendorService, webHelper, localizationSettings, taxSettings, _httpContextAccessor);
        _languageService = languageService;
		_logger.LogDebug("WebWorkContext(cookieSettings=<redacted>,currencySettings={CurrencySettings},authenticationService={AuthenticationService},currencyService={CurrencyService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},storeContext={StoreContext},userAgentHelper={UserAgentHelper},vendorService={VendorService},webHelper={WebHelper},localizationSettings={LocalizationSettings},taxSettings={TaxSettings}): _languageService → {_languageService}", currencySettings, authenticationService, currencyService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, languageService, storeContext, userAgentHelper, vendorService, webHelper, localizationSettings, taxSettings, _languageService);
        _storeContext = storeContext;
		_logger.LogDebug("WebWorkContext(cookieSettings=<redacted>,currencySettings={CurrencySettings},authenticationService={AuthenticationService},currencyService={CurrencyService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},storeContext={StoreContext},userAgentHelper={UserAgentHelper},vendorService={VendorService},webHelper={WebHelper},localizationSettings={LocalizationSettings},taxSettings={TaxSettings}): _storeContext → {_storeContext}", currencySettings, authenticationService, currencyService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, languageService, storeContext, userAgentHelper, vendorService, webHelper, localizationSettings, taxSettings, _storeContext);
        _userAgentHelper = userAgentHelper;
		_logger.LogDebug("WebWorkContext(cookieSettings=<redacted>,currencySettings={CurrencySettings},authenticationService={AuthenticationService},currencyService={CurrencyService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},storeContext={StoreContext},userAgentHelper={UserAgentHelper},vendorService={VendorService},webHelper={WebHelper},localizationSettings={LocalizationSettings},taxSettings={TaxSettings}): _userAgentHelper → {_userAgentHelper}", currencySettings, authenticationService, currencyService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, languageService, storeContext, userAgentHelper, vendorService, webHelper, localizationSettings, taxSettings, _userAgentHelper);
        _vendorService = vendorService;
		_logger.LogDebug("WebWorkContext(cookieSettings=<redacted>,currencySettings={CurrencySettings},authenticationService={AuthenticationService},currencyService={CurrencyService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},storeContext={StoreContext},userAgentHelper={UserAgentHelper},vendorService={VendorService},webHelper={WebHelper},localizationSettings={LocalizationSettings},taxSettings={TaxSettings}): _vendorService → {_vendorService}", currencySettings, authenticationService, currencyService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, languageService, storeContext, userAgentHelper, vendorService, webHelper, localizationSettings, taxSettings, _vendorService);
        _webHelper = webHelper;
		_logger.LogDebug("WebWorkContext(cookieSettings=<redacted>,currencySettings={CurrencySettings},authenticationService={AuthenticationService},currencyService={CurrencyService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},storeContext={StoreContext},userAgentHelper={UserAgentHelper},vendorService={VendorService},webHelper={WebHelper},localizationSettings={LocalizationSettings},taxSettings={TaxSettings}): _webHelper → {_webHelper}", currencySettings, authenticationService, currencyService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, languageService, storeContext, userAgentHelper, vendorService, webHelper, localizationSettings, taxSettings, _webHelper);
        _localizationSettings = localizationSettings;
		_logger.LogDebug("WebWorkContext(cookieSettings=<redacted>,currencySettings={CurrencySettings},authenticationService={AuthenticationService},currencyService={CurrencyService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},languageService={LanguageService},storeContext={StoreContext},userAgentHelper={UserAgentHelper},vendorService={VendorService},webHelper={WebHelper},localizationSettings={LocalizationSettings},taxSettings={TaxSettings}): _localizationSettings → {_localizationSettings}", currencySettings, authenticationService, currencyService, customerService, eventPublisher, genericAttributeService, httpContextAccessor, languageService, storeContext, userAgentHelper, vendorService, webHelper, localizationSettings, taxSettings, _localizationSettings);
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
		_logger.LogError(">>> Entering GetCustomerCookie()");
        var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CustomerCookie}";
		_logger.LogError("GetCustomerCookie(): cookieName → {CookieName}", cookieName);
		_logger.LogWarning("<<< Exiting GetCustomerCookie()");
        return _httpContextAccessor.HttpContext?.Request?.Cookies[cookieName];
    }

    /// <summary>
    /// Set nop customer cookie
    /// </summary>
    /// <param name="customerGuid">Guid of the customer</param>
    protected virtual void SetCustomerCookie(Guid customerGuid)
    {
		_logger.LogError(">>> Entering SetCustomerCookie(customerGuid={CustomerGuid})", customerGuid);
        if (_httpContextAccessor.HttpContext?.Response.HasStarted ?? true)
            return;

        //delete current cookie value
        var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CustomerCookie}";
		_logger.LogError("SetCustomerCookie(customerGuid={CustomerGuid}): cookieName → {CookieName}", customerGuid, cookieName);
        _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

        //get date of cookie expiration
        var cookieExpires = _cookieSettings.CustomerCookieExpires;
        var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);
		_logger.LogError("SetCustomerCookie(customerGuid={CustomerGuid}): cookieExpiresDate → {CookieExpiresDate}", customerGuid, cookieExpiresDate);

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
		_logger.LogWarning("<<< Exiting SetCustomerCookie(customerGuid={CustomerGuid})", customerGuid);
    }

    /// <summary>
    /// Set language culture cookie
    /// </summary>
    /// <param name="language">Language</param>
    protected virtual void SetLanguageCookie(Language language)
    {
		_logger.LogError(">>> Entering SetLanguageCookie(language={Language})", language);
        if (_httpContextAccessor.HttpContext?.Response.HasStarted ?? true)
            return;

        //delete current cookie value
        var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CultureCookie}";
		_logger.LogError("SetLanguageCookie(language={Language}): cookieName → {CookieName}", language, cookieName);
        _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

        if (string.IsNullOrEmpty(language?.LanguageCulture))
            return;

        //set new cookie value
        var value = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language.LanguageCulture));
		_logger.LogError("SetLanguageCookie(language={Language}): value → {Value}", language, value);
        var options = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) };
		_logger.LogError("SetLanguageCookie(language={Language}): options → {Options}", language, options);
        _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, value, options);
		_logger.LogWarning("<<< Exiting SetLanguageCookie(language={Language})", language);
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
		_logger.LogError(">>> Entering GetLanguageFromRequestAsync(storeId={StoreId})", storeId);
        var requestCultureFeature = _httpContextAccessor.HttpContext?.Features.Get<IRequestCultureFeature>();
        if (requestCultureFeature is null)
            return null;

        //whether we should detect the current language by customer settings
        if (requestCultureFeature.Provider is not NopSeoUrlCultureProvider && !_localizationSettings.AutomaticallyDetectLanguage)
            return null;

        //get request culture
        if (requestCultureFeature.RequestCulture is null)
            return null;

        //try to get language by culture name
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("GetLanguageFromRequestAsync(storeId={StoreId}): store → {Store}", storeId, store);
        var requestLanguage = (await _languageService.GetAllLanguagesAsync(storeId: storeId)).FirstOrDefault(language =>
            language.LanguageCulture.Equals(requestCultureFeature.RequestCulture.Culture.Name, StringComparison.InvariantCultureIgnoreCase));

		_logger.LogWarning("<<< Exiting GetLanguageFromRequestAsync(storeId={StoreId})", storeId);
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
		_logger.LogError(">>> Entering GetCurrentCustomerAsync()");
        //whether there is a cached value
        if (_cachedCustomer != null)
            return _cachedCustomer;

        await SetCurrentCustomerAsync();

		_logger.LogWarning("<<< Exiting GetCurrentCustomerAsync()");
        return _cachedCustomer;
    }

    /// <summary>
    /// Sets the current customer
    /// </summary>
    /// <param name="customer">Current customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetCurrentCustomerAsync(Customer customer = null)
    {
		_logger.LogError(">>> Entering SetCurrentCustomerAsync(customer={Customer})", customer);
        if (customer == null)
        {
            //check whether request is made by a background (schedule) task
            if (_httpContextAccessor.HttpContext?.Request
                    ?.Path.Equals(new PathString($"/{NopTaskDefaults.ScheduleTaskPath}"), StringComparison.InvariantCultureIgnoreCase)
                ?? true)
            {
                //in this case return built-in customer record for background task
                customer = await _customerService.GetOrCreateBackgroundTaskUserAsync();
				_logger.LogError("SetCurrentCustomerAsync(customer={Customer}): customer → {Customer1}", customer, customer);
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
					_logger.LogError("SetCurrentCustomerAsync(customer={Customer}): impersonatedCustomer → {ImpersonatedCustomer}", customer, impersonatedCustomer);
                    if (impersonatedCustomer != null && !impersonatedCustomer.Deleted &&
                        impersonatedCustomer.Active &&
                        !impersonatedCustomer.RequireReLogin)
                    {
                        //set impersonated customer
                        _originalCustomerIfImpersonated = customer;
						_logger.LogDebug("SetCurrentCustomerAsync(customer={Customer}): _originalCustomerIfImpersonated → {_originalCustomerIfImpersonated}", customer, _originalCustomerIfImpersonated);
                        customer = impersonatedCustomer;
                    }
                }
            }

            if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
            {
                //get guest customer
                var customerCookie = GetCustomerCookie();
				_logger.LogError("SetCurrentCustomerAsync(customer={Customer}): customerCookie → {CustomerCookie}", customer, customerCookie);
                if (Guid.TryParse(customerCookie, out var customerGuid))
                {
                    //get customer from cookie (should not be registered)
                    var customerByCookie = await _customerService.GetCustomerByGuidAsync(customerGuid);
					_logger.LogError("SetCurrentCustomerAsync(customer={Customer}): customerByCookie → {CustomerByCookie}", customer, customerByCookie);
                    if (customerByCookie != null && !await _customerService.IsRegisteredAsync(customerByCookie))
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
			_logger.LogDebug("SetCurrentCustomerAsync(customer={Customer}): _cachedCustomer → {_cachedCustomer}", customer, _cachedCustomer);
        }
		_logger.LogWarning("<<< Exiting SetCurrentCustomerAsync(customer={Customer})", customer);
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
		_logger.LogError(">>> Entering GetCurrentVendorAsync()");
        //whether there is a cached value
        if (_cachedVendor != null)
            return _cachedVendor;

        var customer = await GetCurrentCustomerAsync();
		_logger.LogError("GetCurrentVendorAsync(): customer → {Customer}", customer);
        if (customer == null)
            return null;

        //check vendor availability
        var vendor = await _vendorService.GetVendorByIdAsync(customer.VendorId);
		_logger.LogError("GetCurrentVendorAsync(): vendor → {Vendor}", vendor);
        if (vendor == null || vendor.Deleted || !vendor.Active)
            return null;

        //cache the found vendor
        _cachedVendor = vendor;
		_logger.LogDebug("GetCurrentVendorAsync(): _cachedVendor → {_cachedVendor}", _cachedVendor);

		_logger.LogWarning("<<< Exiting GetCurrentVendorAsync()");
        return _cachedVendor;
    }

    /// <summary>
    /// Sets current user working language
    /// </summary>
    /// <param name="language">Language</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetWorkingLanguageAsync(Language language)
    {
		_logger.LogError(">>> Entering SetWorkingLanguageAsync(language={Language})", language);
        //save passed language identifier
        var customer = await GetCurrentCustomerAsync();
		_logger.LogError("SetWorkingLanguageAsync(language={Language}): customer → {Customer}", language, customer);

        if (!customer.IsSystemAccount)
        {
            customer.LanguageId = language?.Id;
			_logger.LogDebug("SetWorkingLanguageAsync(language={Language}): customer.LanguageId → {Arg1}", language, customer.LanguageId);
            await _customerService.UpdateCustomerAsync(customer);

            //raise event
            await _eventPublisher.PublishAsync(new CustomerChangeWorkingLanguageEvent(customer));
        }

        //set cookie
        SetLanguageCookie(language);

        //then reset the cached value
		_logger.LogWarning("<<< Exiting SetWorkingLanguageAsync(language={Language})", language);
        _cachedLanguage = null;
		_logger.LogDebug("SetWorkingLanguageAsync(language={Language}): _cachedLanguage → {_cachedLanguage}", language, _cachedLanguage);
    }

    /// <summary>
    /// Gets current user working language
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<Language> GetWorkingLanguageAsync()
    {
		_logger.LogError(">>> Entering GetWorkingLanguageAsync()");
        //whether there is a cached value
        if (_cachedLanguage != null)
            return _cachedLanguage;

        var customer = await GetCurrentCustomerAsync();
		_logger.LogError("GetWorkingLanguageAsync(): customer → {Customer}", customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("GetWorkingLanguageAsync(): store → {Store}", store);

        //whether we should detect the language from the request
        var detectedLanguage = await GetLanguageFromRequestAsync(store.Id);
		_logger.LogError("GetWorkingLanguageAsync(): detectedLanguage → {DetectedLanguage}", detectedLanguage);

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
			_logger.LogError("GetWorkingLanguageAsync(): allStoreLanguages → {AllStoreLanguages}", allStoreLanguages);

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
		_logger.LogDebug("GetWorkingLanguageAsync(): _cachedLanguage → {_cachedLanguage}", _cachedLanguage);

		_logger.LogWarning("<<< Exiting GetWorkingLanguageAsync()");
        return _cachedLanguage;
    }

    /// <summary>
    /// Gets current user working currency
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<Currency> GetWorkingCurrencyAsync()
    {
		_logger.LogError(">>> Entering GetWorkingCurrencyAsync()");
        //whether there is a cached value
        if (_cachedCurrency != null)
            return _cachedCurrency;

        var adminAreaUrl = $"{_webHelper.GetStoreLocation()}admin";
		_logger.LogError("GetWorkingCurrencyAsync(): adminAreaUrl → {AdminAreaUrl}", adminAreaUrl);

        //return primary store currency when we're in admin area/mode
        if (_webHelper.GetThisPageUrl(false).StartsWith(adminAreaUrl, StringComparison.InvariantCultureIgnoreCase))
        {
            var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
			_logger.LogError("GetWorkingCurrencyAsync(): primaryStoreCurrency → {PrimaryStoreCurrency}", primaryStoreCurrency);
            if (primaryStoreCurrency != null)
            {
                _cachedCurrency = primaryStoreCurrency;
				_logger.LogError("GetWorkingCurrencyAsync(): _cachedCurrency → {_cachedCurrency}", _cachedCurrency);
				_logger.LogDebug("GetWorkingCurrencyAsync(): _cachedCurrency → {_cachedCurrency}", _cachedCurrency);
                return primaryStoreCurrency;
            }
        }

        var customer = await GetCurrentCustomerAsync();
		_logger.LogError("GetWorkingCurrencyAsync(): customer → {Customer}", customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("GetWorkingCurrencyAsync(): store → {Store}", store);

        if (customer.IsSearchEngineAccount())
        {
            _cachedCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)
                              ?? (await _currencyService.GetAllCurrenciesAsync(storeId: store.Id)).FirstOrDefault();

            return _cachedCurrency;
        }

        var allStoreCurrencies = await _currencyService.GetAllCurrenciesAsync(storeId: store.Id);
		_logger.LogError("GetWorkingCurrencyAsync(): allStoreCurrencies → {AllStoreCurrencies}", allStoreCurrencies);

        //check customer currency availability
        var customerCurrency = allStoreCurrencies.FirstOrDefault(currency => currency.Id == customer.CurrencyId);
		_logger.LogError("GetWorkingCurrencyAsync(): customerCurrency → {CustomerCurrency}", customerCurrency);
        if (customerCurrency == null)
        {
            //it not found, then try to get the default currency for the current language (if specified)
            var language = await GetWorkingLanguageAsync();
			_logger.LogError("GetWorkingCurrencyAsync(): language → {Language}", language);
            customerCurrency = allStoreCurrencies
                .FirstOrDefault(currency => currency.Id == language.DefaultCurrencyId);
        }

        //if the default currency for the current store not found, then try to get the first one
        customerCurrency ??= allStoreCurrencies.FirstOrDefault();

        //if there are no currencies for the current store try to get the first one regardless of the store
        customerCurrency ??= (await _currencyService.GetAllCurrenciesAsync()).FirstOrDefault();

        //cache the found currency
        _cachedCurrency = customerCurrency;

		_logger.LogWarning("<<< Exiting GetWorkingCurrencyAsync()");
        return _cachedCurrency;
    }

    /// <summary>
    /// Sets current user working currency
    /// </summary>
    /// <param name="currency">Currency</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetWorkingCurrencyAsync(Currency currency)
    {
		_logger.LogError(">>> Entering SetWorkingCurrencyAsync(currency={Currency})", currency);
        //save passed currency identifier
        var customer = await GetCurrentCustomerAsync();
		_logger.LogError("SetWorkingCurrencyAsync(currency={Currency}): customer → {Customer}", currency, customer);
        if (customer.IsSearchEngineAccount())
            return;

        customer.CurrencyId = currency?.Id;
		_logger.LogDebug("SetWorkingCurrencyAsync(currency={Currency}): customer.CurrencyId → {Arg1}", currency, customer.CurrencyId);
        await _customerService.UpdateCustomerAsync(customer);

        //then reset the cached value
        _cachedCurrency = null;
		_logger.LogDebug("SetWorkingCurrencyAsync(currency={Currency}): _cachedCurrency → {_cachedCurrency}", currency, _cachedCurrency);
		_logger.LogWarning("<<< Exiting SetWorkingCurrencyAsync(currency={Currency})", currency);
    }

    /// <summary>
    /// Gets or sets current tax display type
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<TaxDisplayType> GetTaxDisplayTypeAsync()
    {
		_logger.LogError(">>> Entering GetTaxDisplayTypeAsync()");
        //whether there is a cached value
        if (_cachedTaxDisplayType.HasValue)
            return _cachedTaxDisplayType.Value;

        var customer = await GetCurrentCustomerAsync();
		_logger.LogError("GetTaxDisplayTypeAsync(): customer → {Customer}", customer);
        var taxDisplayType = await _customerService.GetCustomerTaxDisplayTypeAsync(customer);
		_logger.LogError("GetTaxDisplayTypeAsync(): taxDisplayType → {TaxDisplayType}", taxDisplayType);

        //cache the value
        _cachedTaxDisplayType = taxDisplayType;
		_logger.LogDebug("GetTaxDisplayTypeAsync(): _cachedTaxDisplayType → {_cachedTaxDisplayType}", _cachedTaxDisplayType);

		_logger.LogWarning("<<< Exiting GetTaxDisplayTypeAsync()");
        return _cachedTaxDisplayType.Value;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetTaxDisplayTypeAsync(TaxDisplayType taxDisplayType)
    {
		_logger.LogError(">>> Entering SetTaxDisplayTypeAsync(taxDisplayType={TaxDisplayType})", taxDisplayType);
        //whether customers are allowed to select tax display type
        if (!_taxSettings.AllowCustomersToSelectTaxDisplayType)
            return;

        //save passed value
        var customer = await GetCurrentCustomerAsync();
		_logger.LogError("SetTaxDisplayTypeAsync(taxDisplayType={TaxDisplayType}): customer → {Customer}", taxDisplayType, customer);
		_logger.LogDebug("SetTaxDisplayTypeAsync(taxDisplayType={TaxDisplayType}): customer.TaxDisplayType → {Arg1}", taxDisplayType, customer.TaxDisplayType);
        customer.TaxDisplayType = taxDisplayType;
        await _customerService.UpdateCustomerAsync(customer);

        //then reset the cached value
        _cachedTaxDisplayType = null;
		_logger.LogDebug("SetTaxDisplayTypeAsync(taxDisplayType={TaxDisplayType}): _cachedTaxDisplayType → {_cachedTaxDisplayType}", taxDisplayType, _cachedTaxDisplayType);
		_logger.LogWarning("<<< Exiting SetTaxDisplayTypeAsync(taxDisplayType={TaxDisplayType})", taxDisplayType);
    }

    /// <summary>
    /// Gets or sets value indicating whether we're in admin area
    /// </summary>
    public virtual bool IsAdmin { get; set; }

    #endregion
}