using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using brevo_csharp.Api;
using brevo_csharp.Client;
using brevo_csharp.Model;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Plugin.Misc.Brevo.Domain;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Installation;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Framework.Mvc.Routing;
using static brevo_csharp.Model.GetAttributesAttributes;

namespace Nop.Plugin.Misc.Brevo.Services;

/// <summary>
/// Represents Brevo manager
/// </summary>
public partial class BrevoManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICountryService _countryService;
    protected readonly ICustomerService _customerService;
    protected readonly IEmailAccountService _emailAccountService;
    protected readonly ILanguageService _languageService;
    protected readonly ILogger _logger;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly ISettingService _settingService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreService _storeService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public BrevoManager(ICountryService countryService,
        ICustomerService customerService,
        IEmailAccountService emailAccountService,
        ILanguageService languageService,
        ILogger logger,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        INopUrlHelper nopUrlHelper,
        ISettingService settingService,
        IStateProvinceService stateProvinceService,
        IStoreService storeService,
        IWebHelper webHelper,
        IWorkContext workContext)
    {
        _countryService = countryService;
		_logger.LogDebug("BrevoManager(countryService={CountryService},customerService={CustomerService},emailAccountService={EmailAccountService},languageService={LanguageService},logger={Logger},newsLetterSubscriptionService={NewsLetterSubscriptionService},nopUrlHelper={NopUrlHelper},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},webHelper={WebHelper},workContext={WorkContext}): _countryService → {_countryService}", countryService, customerService, emailAccountService, languageService, logger, newsLetterSubscriptionService, nopUrlHelper, settingService, stateProvinceService, storeService, webHelper, workContext, _countryService);
        _customerService = customerService;
		_logger.LogDebug("BrevoManager(countryService={CountryService},customerService={CustomerService},emailAccountService={EmailAccountService},languageService={LanguageService},logger={Logger},newsLetterSubscriptionService={NewsLetterSubscriptionService},nopUrlHelper={NopUrlHelper},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},webHelper={WebHelper},workContext={WorkContext}): _customerService → {_customerService}", countryService, customerService, emailAccountService, languageService, logger, newsLetterSubscriptionService, nopUrlHelper, settingService, stateProvinceService, storeService, webHelper, workContext, _customerService);
        _emailAccountService = emailAccountService;
		_logger.LogDebug("BrevoManager(countryService={CountryService},customerService={CustomerService},emailAccountService={EmailAccountService},languageService={LanguageService},logger={Logger},newsLetterSubscriptionService={NewsLetterSubscriptionService},nopUrlHelper={NopUrlHelper},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},webHelper={WebHelper},workContext={WorkContext}): _emailAccountService → {_emailAccountService}", countryService, customerService, emailAccountService, languageService, logger, newsLetterSubscriptionService, nopUrlHelper, settingService, stateProvinceService, storeService, webHelper, workContext, _emailAccountService);
        _languageService = languageService;
		_logger.LogDebug("BrevoManager(countryService={CountryService},customerService={CustomerService},emailAccountService={EmailAccountService},languageService={LanguageService},logger={Logger},newsLetterSubscriptionService={NewsLetterSubscriptionService},nopUrlHelper={NopUrlHelper},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},webHelper={WebHelper},workContext={WorkContext}): _languageService → {_languageService}", countryService, customerService, emailAccountService, languageService, logger, newsLetterSubscriptionService, nopUrlHelper, settingService, stateProvinceService, storeService, webHelper, workContext, _languageService);
        _logger = logger;
		_logger.LogError(">>> Entering BrevoManager(countryService={CountryService},customerService={CustomerService},emailAccountService={EmailAccountService},languageService={LanguageService},logger={Logger},newsLetterSubscriptionService={NewsLetterSubscriptionService},nopUrlHelper={NopUrlHelper},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},webHelper={WebHelper},workContext={WorkContext})", countryService, customerService, emailAccountService, languageService, logger, newsLetterSubscriptionService, nopUrlHelper, settingService, stateProvinceService, storeService, webHelper, workContext);
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _nopUrlHelper = nopUrlHelper;
		_logger.LogDebug("BrevoManager(countryService={CountryService},customerService={CustomerService},emailAccountService={EmailAccountService},languageService={LanguageService},logger={Logger},newsLetterSubscriptionService={NewsLetterSubscriptionService},nopUrlHelper={NopUrlHelper},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},webHelper={WebHelper},workContext={WorkContext}): _newsLetterSubscriptionService → {_newsLetterSubscriptionService}", countryService, customerService, emailAccountService, languageService, logger, newsLetterSubscriptionService, nopUrlHelper, settingService, stateProvinceService, storeService, webHelper, workContext, _newsLetterSubscriptionService);
		_logger.LogDebug("BrevoManager(countryService={CountryService},customerService={CustomerService},emailAccountService={EmailAccountService},languageService={LanguageService},logger={Logger},newsLetterSubscriptionService={NewsLetterSubscriptionService},nopUrlHelper={NopUrlHelper},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},webHelper={WebHelper},workContext={WorkContext}): _nopUrlHelper → {_nopUrlHelper}", countryService, customerService, emailAccountService, languageService, logger, newsLetterSubscriptionService, nopUrlHelper, settingService, stateProvinceService, storeService, webHelper, workContext, _nopUrlHelper);
        _settingService = settingService;
        _stateProvinceService = stateProvinceService;
		_logger.LogDebug("BrevoManager(countryService={CountryService},customerService={CustomerService},emailAccountService={EmailAccountService},languageService={LanguageService},logger={Logger},newsLetterSubscriptionService={NewsLetterSubscriptionService},nopUrlHelper={NopUrlHelper},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},webHelper={WebHelper},workContext={WorkContext}): _settingService → {_settingService}", countryService, customerService, emailAccountService, languageService, logger, newsLetterSubscriptionService, nopUrlHelper, settingService, stateProvinceService, storeService, webHelper, workContext, _settingService);
		_logger.LogDebug("BrevoManager(countryService={CountryService},customerService={CustomerService},emailAccountService={EmailAccountService},languageService={LanguageService},logger={Logger},newsLetterSubscriptionService={NewsLetterSubscriptionService},nopUrlHelper={NopUrlHelper},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},webHelper={WebHelper},workContext={WorkContext}): _stateProvinceService → {_stateProvinceService}", countryService, customerService, emailAccountService, languageService, logger, newsLetterSubscriptionService, nopUrlHelper, settingService, stateProvinceService, storeService, webHelper, workContext, _stateProvinceService);
        _storeService = storeService;
        _webHelper = webHelper;
		_logger.LogDebug("BrevoManager(countryService={CountryService},customerService={CustomerService},emailAccountService={EmailAccountService},languageService={LanguageService},logger={Logger},newsLetterSubscriptionService={NewsLetterSubscriptionService},nopUrlHelper={NopUrlHelper},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},webHelper={WebHelper},workContext={WorkContext}): _storeService → {_storeService}", countryService, customerService, emailAccountService, languageService, logger, newsLetterSubscriptionService, nopUrlHelper, settingService, stateProvinceService, storeService, webHelper, workContext, _storeService);
		_logger.LogDebug("BrevoManager(countryService={CountryService},customerService={CustomerService},emailAccountService={EmailAccountService},languageService={LanguageService},logger={Logger},newsLetterSubscriptionService={NewsLetterSubscriptionService},nopUrlHelper={NopUrlHelper},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},webHelper={WebHelper},workContext={WorkContext}): _webHelper → {_webHelper}", countryService, customerService, emailAccountService, languageService, logger, newsLetterSubscriptionService, nopUrlHelper, settingService, stateProvinceService, storeService, webHelper, workContext, _webHelper);
		_logger.LogWarning("<<< Exiting BrevoManager(countryService={CountryService},customerService={CustomerService},emailAccountService={EmailAccountService},languageService={LanguageService},logger={Logger},newsLetterSubscriptionService={NewsLetterSubscriptionService},nopUrlHelper={NopUrlHelper},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},webHelper={WebHelper},workContext={WorkContext})", countryService, customerService, emailAccountService, languageService, logger, newsLetterSubscriptionService, nopUrlHelper, settingService, stateProvinceService, storeService, webHelper, workContext);
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Handle function and get result
    /// </summary>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <param name="function">Function</param>
    /// <param name="logErrors">Whether to log errors</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result; error if exists
    /// </returns>
    private async Task<(TResult Result, string Error)> HandleFunctionAsync<TResult>(Func<Task<TResult>> function, bool logErrors = true)
    {
        try
        {
            //whether plugin is configured
            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>();
            if (!IsConfigured(brevoSettings))
                throw new NopException("Plugin not configured");

            return (await function(), default);
        }
        catch (Exception exception)
        {
            var errorMessage = exception.Message;
            if (logErrors)
            {
                var logMessage = $"{BrevoDefaults.SystemName} error: {Environment.NewLine}{errorMessage}";
                await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());
            }

            return (default, errorMessage);
        }
    }

    /// <summary>
    /// Prepare API client
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the aPI client
    /// </returns>
    protected async Task<TClient> CreateApiClientAsync<TClient>(Func<Configuration, TClient> clientCtor) where TClient : IApiAccessor
    {
		_logger.LogError(">>> Entering CreateApiClientAsync(clientCtor={ClientCtor})", clientCtor);
        //check whether plugin is configured to request services (validate API key)
        var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>();
		_logger.LogError("CreateApiClientAsync(clientCtor={ClientCtor}): brevoSettings → {BrevoSettings}", clientCtor, brevoSettings);
        if (!IsConfigured(brevoSettings))
			_logger.LogError("Throwing NopException from CreateApiClientAsync(clientCtor={ClientCtor})", clientCtor);
            throw new NopException("Plugin not configured");

        var apiConfiguration = new Configuration()
        {
            ApiKey = new Dictionary<string, string>
            {
                [BrevoDefaults.ApiKeyHeader] = brevoSettings.ApiKey,
                [BrevoDefaults.PartnerKeyHeader] = brevoSettings.ApiKey
            },
            ApiKeyPrefix = new Dictionary<string, string> { [BrevoDefaults.PartnerKeyHeader] = BrevoDefaults.PartnerName },
            UserAgent = BrevoDefaults.UserAgent
        };

		_logger.LogWarning("<<< Exiting CreateApiClientAsync(clientCtor={ClientCtor})", clientCtor);
        return clientCtor(apiConfiguration);
    }

    /// <summary>
    /// Import contacts from the store to account
    /// </summary>
    /// <param name="brevoSettings">Plugin settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of messages
    /// </returns>
    protected async Task<IList<(NotifyType Type, string Message)>> ImportContactsAsync(BrevoSettings brevoSettings)
    {
        var messages = new List<(NotifyType, string)>();

        //import contacts to account
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));

            var languages = await _languageService.GetAllLanguagesAsync();

            List<Customer> customers = null;
            async Task<List<Customer>> getCustomersAsync()
            {
				_logger.LogError(">>> Entering getCustomersAsync()");
                customers ??= (await _customerService.GetAllCustomersAsync()).ToList();
				_logger.LogWarning("<<< Exiting getCustomersAsync()");
                return customers;
            }

            async Task<string> getNamesAsync()
            {
				_logger.LogError(">>> Entering getNamesAsync()");
                switch (await GetAccountLanguageAsync())
                {
                    case BrevoAccountLanguage.French:
                        return
                            $"{BrevoDefaults.FirstNameFrenchServiceAttribute};" +
                            $"{BrevoDefaults.LastNameFrenchServiceAttribute};";

                    case BrevoAccountLanguage.German:
                        return
                            $"{BrevoDefaults.FirstNameGermanServiceAttribute};" +
                            $"{BrevoDefaults.LastNameGermanServiceAttribute};";

                    case BrevoAccountLanguage.Italian:
                        return
                            $"{BrevoDefaults.FirstNameItalianServiceAttribute};" +
                            $"{BrevoDefaults.LastNameItalianServiceAttribute};";

                    case BrevoAccountLanguage.Portuguese:
                        return
                            $"{BrevoDefaults.FirstNamePortugueseServiceAttribute};" +
                            $"{BrevoDefaults.LastNamePortugueseServiceAttribute};";

                    case BrevoAccountLanguage.Spanish:
                        return
                            $"{BrevoDefaults.FirstNameSpanishServiceAttribute};" +
                            $"{BrevoDefaults.LastNameSpanishServiceAttribute};";

                    default:
							_logger.LogWarning("<<< Exiting getNamesAsync()");
                        return
                            $"{BrevoDefaults.FirstNameServiceAttribute};" +
                            $"{BrevoDefaults.LastNameServiceAttribute};";
							_logger.LogWarning("<<< Exiting GetAccountLanguageAsync()");
                }
            }

            //get mappings from the settings
            foreach (var mapping in brevoSettings.SubscriptionTypeMappings)
            {
                var typeId = mapping.Key;
                var listId = mapping.Value;
                if (typeId == 0 || listId == 0)
                    continue;

                //try to get subscriptions by the type
                var subscriptions = (await _newsLetterSubscriptionService
                    .GetAllNewsLetterSubscriptionsAsync(subscriptionTypeId: typeId, isActive: true))
                    .ToList();
                if (!subscriptions.Any())
                {
                    await _logger.WarningAsync($"Brevo synchronization warning: There are no subscriptions of type #{typeId}");
                    messages.Add((NotifyType.Warning, $"There are no subscriptions of type #{typeId}"));
                    continue;
                }

                //get notification URL
                var notificationUrl = _nopUrlHelper.RouteUrl(BrevoDefaults.ImportContactsRoute, null, _webHelper.GetCurrentRequestProtocol());

                //prepare CSV 
                var title =
                    $"{BrevoDefaults.EmailServiceAttribute};" +
                    await getNamesAsync() +
                    $"{BrevoDefaults.UsernameServiceAttribute};" +
                    $"{BrevoDefaults.SMSServiceAttribute};" +
                    $"{BrevoDefaults.PhoneServiceAttribute};" +
                    $"{BrevoDefaults.CountryServiceAttribute};" +
                    $"{BrevoDefaults.StoreIdServiceAttribute};" +
                    $"{BrevoDefaults.GenderServiceAttribute};" +
                    $"{BrevoDefaults.DateOfBirthServiceAttribute};" +
                    $"{BrevoDefaults.CompanyServiceAttribute};" +
                    $"{BrevoDefaults.Address1ServiceAttribute};" +
                    $"{BrevoDefaults.Address2ServiceAttribute};" +
                    $"{BrevoDefaults.ZipCodeServiceAttribute};" +
                    $"{BrevoDefaults.CityServiceAttribute};" +
                    $"{BrevoDefaults.CountyServiceAttribute};" +
                    $"{BrevoDefaults.StateServiceAttribute};" +
                    $"{BrevoDefaults.FaxServiceAttribute};" +
                    $"{BrevoDefaults.LanguageAttribute};";
                var csv = await subscriptions.AggregateAwaitAsync(title, async (all, subscription) =>
                {
                    var firstName = string.Empty;
                    var lastName = string.Empty;
                    var phone = string.Empty;
                    var countryName = string.Empty;
                    var sms = string.Empty;
                    var gender = string.Empty;
                    var dateOfBirth = string.Empty;
                    var company = string.Empty;
                    var address1 = string.Empty;
                    var address2 = string.Empty;
                    var zipCode = string.Empty;
                    var city = string.Empty;
                    var county = string.Empty;
                    var state = string.Empty;
                    var fax = string.Empty;
                    Language language = null;

                    var customer = (await getCustomersAsync()).FirstOrDefault(customer => customer.Email == subscription.Email);
                    if (customer != null)
                    {
                        firstName = customer.FirstName;
                        lastName = customer.LastName;
                        phone = customer.Phone;
                        var countryId = customer.CountryId;
                        var country = await _countryService.GetCountryByIdAsync(countryId);
                        countryName = country?.Name;
                        var countryIsoCode = country?.NumericIsoCode ?? 0;
                        if (countryIsoCode > 0 && !string.IsNullOrEmpty(phone))
                        {
                            //use the first phone code only
                            var phoneCode = ISO3166.FromISOCode(countryIsoCode)
                                ?.DialCodes?.FirstOrDefault()?.Replace(" ", string.Empty) ?? string.Empty;
                            sms = phone.Replace($"+{phoneCode}", string.Empty);
                        }
                        gender = customer.Gender;
                        dateOfBirth = customer.DateOfBirth?.ToString("yyyy-MM-dd");
                        company = customer.Company;
                        address1 = customer.StreetAddress;
                        address2 = customer.StreetAddress2;
                        zipCode = customer.ZipPostalCode;
                        city = customer.City;
                        county = customer.County;
                        state = (await _stateProvinceService.GetStateProvinceByIdAsync(customer.StateProvinceId))?.Name;
                        fax = customer.Fax;
                    }

                    language = languages
                        .FirstOrDefault(lang => lang.Id == (customer?.LanguageId ?? subscription.LanguageId))
                        ?? languages.FirstOrDefault();

                    return
                        $"{all}\n" +
                        $"{subscription.Email};" +
                        $"{firstName};" +
                        $"{lastName};" +
                        $"{customer?.Username};" +
                        $"{sms};" +
                        $"{phone};" +
                        $"{countryName};" +
                        $"{subscription.StoreId};" +
                        $"{gender};" +
                        $"{dateOfBirth};" +
                        $"{company};" +
                        $"{address1};" +
                        $"{address2};" +
                        $"{zipCode};" +
                        $"{city};" +
                        $"{county};" +
                        $"{state};" +
                        $"{fax};" +
                        $"{language?.LanguageCulture};";
                });

                //prepare data to import
                var requestContactImport = new RequestContactImport
                {
                    NotifyUrl = notificationUrl,
                    FileBody = csv,
                    ListIds = [listId]
                };

                //start import
                await client.ImportContactsAsync(requestContactImport);
            }
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo synchronization error: {exception.Message}", exception, await _workContext.GetCurrentCustomerAsync());
            messages.Add((NotifyType.Error, $"Brevo synchronization error: {exception.Message}"));
        }

        return messages;
    }

    /// <summary>
    /// Export contacts from account to the store
    /// </summary>
    /// <param name="brevoSettings">Plugin settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of messages
    /// </returns>
    protected async Task<IList<(NotifyType Type, string Message)>> ExportContactsAsync(BrevoSettings brevoSettings)
    {
        var messages = new List<(NotifyType, string)>();

        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));

            //get mappings from the settings
            foreach (var mapping in brevoSettings.SubscriptionTypeMappings)
            {
                var typeId = mapping.Key;
                var listId = mapping.Value;
                if (typeId == 0 || listId == 0)
                    continue;

                //check whether there are contacts in the list
                var contacts = await client.GetContactsFromListAsync(listId);
                var contactObjects = JsonConvert
                    .DeserializeAnonymousType(contacts.ToJson(), new { contacts = new[] { new { email = string.Empty, emailBlacklisted = false } } });
                var blackListedEmails = contactObjects?.contacts
                    ?.Where(contact => contact.emailBlacklisted && !string.IsNullOrEmpty(contact.email))
                    .Select(contact => contact.email)
                    .Distinct()
                    .ToList()
                    ?? new List<string>();

                foreach (var email in blackListedEmails)
                {
                    //email in black list, so unsubscribe contact
                    var subscriptions = await _newsLetterSubscriptionService
                        .GetNewsLetterSubscriptionsByEmailAsync(email, subscriptionTypeId: typeId);
                    foreach (var subscription in subscriptions)
                    {
                        subscription.Active = false;
                        await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription, false);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo synchronization error: {exception.Message}", exception, await _workContext.GetCurrentCustomerAsync());
            messages.Add((NotifyType.Error, $"Brevo synchronization error: {exception.Message}"));
        }

        return messages;
    }

    /// <summary>
    /// Add new service attribute in account
    /// </summary>
    /// <param name="attributes">Collection of attributes</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the errors if exist
    /// </returns>
    protected async Task<string> CreateAttributesAsync(IList<(CategoryEnum Category, string Name, string Value, CreateAttribute.TypeEnum? Type)> attributes)
    {
        if (!attributes.Any())
            return string.Empty;

        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));

            foreach (var attribute in attributes)
            {
                //prepare data
                var createAttribute = new CreateAttribute(attribute.Value, type: attribute.Type);

                //create attribute
                await client.CreateAttributeAsync(attribute.Category.ToString().ToLowerInvariant(), attribute.Name, createAttribute);
            }
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return exception.Message;
        }

        return string.Empty;
    }

    [GeneratedRegex("(%[^\\%]*.%)")]
    private static partial Regex SpecialCharRegex();

    [GeneratedRegex("({{\\s*params\\..*?\\s*}})")]
    private static partial Regex ParamsRegex();

    #endregion

    #region Methods

    #region Synchronization

    /// <summary>
    /// Synchronize contacts 
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of messages
    /// </returns>
    public async Task<IList<(NotifyType Type, string Message)>> SynchronizeAsync()
    {
        var messages = new List<(NotifyType, string)>();
        try
        {
            //whether plugin is configured
            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>();
            if (!string.IsNullOrEmpty(brevoSettings.ApiKey))
            {
                var importMessages = await ImportContactsAsync(brevoSettings);
                messages.AddRange(importMessages);

                var exportMessages = await ExportContactsAsync(brevoSettings);
                messages.AddRange(exportMessages);
            }

        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo synchronization error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            messages.Add((NotifyType.Error, $"Brevo synchronization error: {exception.Message}"));
        }

        return messages;
    }

    /// <summary>
    /// Subscribe new contact
    /// </summary>
    /// <param name="subscription">Subscription</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async System.Threading.Tasks.Task SubscribeAsync(NewsLetterSubscription subscription)
    {
		_logger.LogError(">>> Entering SubscribeAsync(subscription={Subscription})", subscription);
        try
        {
            //try to get list id
            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>();
			_logger.LogError("SubscribeAsync(subscription={Subscription}): brevoSettings → {BrevoSettings}", subscription, brevoSettings);
            if (!brevoSettings.SubscriptionTypeMappings.TryGetValue(subscription.TypeId, out var listId) || listId == 0)
                return;

            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));
			_logger.LogError("SubscribeAsync(subscription={Subscription}): client → {Client}", subscription, client);

            GetExtendedContactDetails contactObject = null;
			_logger.LogError("SubscribeAsync(subscription={Subscription}): contactObject → {ContactObject}", subscription, contactObject);
            try
            {
                contactObject = await client.GetContactInfoAsync(subscription.Email);
            }
            catch (ApiException apiException)
            {
                if (apiException.ErrorCode != 404)
                {
                    await _logger.ErrorAsync($"Brevo error: {apiException.Message}.", apiException, await _workContext.GetCurrentCustomerAsync());
                    return;
                }
            }

            //prepare attributes
            var firstName = string.Empty;
			_logger.LogError("SubscribeAsync(subscription={Subscription}): firstName → {FirstName}", subscription, firstName);
            var lastName = string.Empty;
			_logger.LogError("SubscribeAsync(subscription={Subscription}): lastName → {LastName}", subscription, lastName);
            var phone = string.Empty;
			_logger.LogError("SubscribeAsync(subscription={Subscription}): phone → {Phone}", subscription, phone);
            var sms = string.Empty;
			_logger.LogError("SubscribeAsync(subscription={Subscription}): sms → {Sms}", subscription, sms);
            var countryName = string.Empty;
			_logger.LogError("SubscribeAsync(subscription={Subscription}): countryName → {CountryName}", subscription, countryName);
            var gender = string.Empty;
			_logger.LogError("SubscribeAsync(subscription={Subscription}): gender → {Gender}", subscription, gender);
            var dateOfBirth = string.Empty;
			_logger.LogError("SubscribeAsync(subscription={Subscription}): dateOfBirth → {DateOfBirth}", subscription, dateOfBirth);
            var company = string.Empty;
			_logger.LogError("SubscribeAsync(subscription={Subscription}): company → {Company}", subscription, company);
            var address1 = string.Empty;
			_logger.LogError("SubscribeAsync(subscription={Subscription}): address1 → {Address1}", subscription, address1);
            var address2 = string.Empty;
			_logger.LogError("SubscribeAsync(subscription={Subscription}): address2 → {Address2}", subscription, address2);
            var zipCode = string.Empty;
			_logger.LogError("SubscribeAsync(subscription={Subscription}): zipCode → {ZipCode}", subscription, zipCode);
            var city = string.Empty;
			_logger.LogError("SubscribeAsync(subscription={Subscription}): city → {City}", subscription, city);
            var county = string.Empty;
			_logger.LogError("SubscribeAsync(subscription={Subscription}): county → {County}", subscription, county);
            var state = string.Empty;
			_logger.LogError("SubscribeAsync(subscription={Subscription}): state → {State}", subscription, state);
            var fax = string.Empty;
			_logger.LogError("SubscribeAsync(subscription={Subscription}): fax → {Fax}", subscription, fax);
            Language language = null;

            var customer = await _customerService.GetCustomerByEmailAsync(subscription.Email);
			_logger.LogError("SubscribeAsync(subscription={Subscription}): customer → {Customer}", subscription, customer);
            if (customer != null)
            {
                firstName = customer.FirstName;
                lastName = customer.LastName;
                phone = customer.Phone;
                var countryId = customer.CountryId;
                var country = await _countryService.GetCountryByIdAsync(countryId);
				_logger.LogError("SubscribeAsync(subscription={Subscription}): country → {Country}", subscription, country);
                countryName = country?.Name;
                var countryIsoCode = country?.NumericIsoCode ?? 0;
                if (countryIsoCode > 0 && !string.IsNullOrEmpty(phone))
                {
                    //use the first phone code only
                    var phoneCode = ISO3166.FromISOCode(countryIsoCode)
                        ?.DialCodes?.FirstOrDefault()?.Replace(" ", string.Empty) ?? string.Empty;
                    sms = phone.Replace($"+{phoneCode}", string.Empty);
                }
                gender = customer.Gender;
                dateOfBirth = customer.DateOfBirth?.ToString("yyyy-MM-dd");
                company = customer.Company;
                address1 = customer.StreetAddress;
                address2 = customer.StreetAddress2;
                zipCode = customer.ZipPostalCode;
                city = customer.City;
                county = customer.County;
                state = (await _stateProvinceService.GetStateProvinceByIdAsync(customer.StateProvinceId))?.Name;
                fax = customer.Fax;
            }

            language = await _languageService.GetLanguageByIdAsync(customer?.LanguageId ?? subscription.LanguageId)
                ?? (await _languageService.GetAllLanguagesAsync(storeId: subscription.StoreId)).FirstOrDefault();

            var attributes = new Dictionary<string, string>
            {
                [BrevoDefaults.UsernameServiceAttribute] = customer?.Username,
                [BrevoDefaults.SMSServiceAttribute] = sms,
                [BrevoDefaults.PhoneServiceAttribute] = phone,
                [BrevoDefaults.CountryServiceAttribute] = countryName,
                [BrevoDefaults.StoreIdServiceAttribute] = subscription.StoreId.ToString(),
                [BrevoDefaults.GenderServiceAttribute] = gender,
                [BrevoDefaults.DateOfBirthServiceAttribute] = dateOfBirth,
                [BrevoDefaults.CompanyServiceAttribute] = company,
                [BrevoDefaults.Address1ServiceAttribute] = address1,
                [BrevoDefaults.Address2ServiceAttribute] = address2,
                [BrevoDefaults.ZipCodeServiceAttribute] = zipCode,
                [BrevoDefaults.CityServiceAttribute] = city,
                [BrevoDefaults.CountyServiceAttribute] = county,
                [BrevoDefaults.StateServiceAttribute] = state,
                [BrevoDefaults.FaxServiceAttribute] = fax,
                [BrevoDefaults.LanguageAttribute] = language?.LanguageCulture
            };

            switch (await GetAccountLanguageAsync())
            {
                case BrevoAccountLanguage.French:
                    attributes.Add(BrevoDefaults.FirstNameFrenchServiceAttribute, firstName);
                    attributes.Add(BrevoDefaults.LastNameFrenchServiceAttribute, lastName);
                    break;
                case BrevoAccountLanguage.German:
                    attributes.Add(BrevoDefaults.FirstNameGermanServiceAttribute, firstName);
                    attributes.Add(BrevoDefaults.LastNameGermanServiceAttribute, lastName);
                    break;
                case BrevoAccountLanguage.Italian:
                    attributes.Add(BrevoDefaults.FirstNameItalianServiceAttribute, firstName);
                    attributes.Add(BrevoDefaults.LastNameItalianServiceAttribute, lastName);
                    break;
                case BrevoAccountLanguage.Portuguese:
                    attributes.Add(BrevoDefaults.FirstNamePortugueseServiceAttribute, firstName);
                    attributes.Add(BrevoDefaults.LastNamePortugueseServiceAttribute, lastName);
                    break;
                case BrevoAccountLanguage.Spanish:
                    attributes.Add(BrevoDefaults.FirstNameSpanishServiceAttribute, firstName);
                    attributes.Add(BrevoDefaults.LastNameSpanishServiceAttribute, lastName);
                    break;
                case BrevoAccountLanguage.English:
                    attributes.Add(BrevoDefaults.FirstNameServiceAttribute, firstName);
                    attributes.Add(BrevoDefaults.LastNameServiceAttribute, lastName);
					_logger.LogError(">>> Entering GetAccountLanguageAsync()");
                    break;
            }

            //Add new contact
            if (contactObject == null)
            {
                var createContact = new CreateContact
                {
                    Email = subscription.Email,
                    Attributes = attributes,
                    ListIds = [listId],
                    UpdateEnabled = true
                };
                await client.CreateContactAsync(createContact);
            }
            else
            {
                //update contact
                var updateContact = new UpdateContact
                {
                    Attributes = attributes,
                    ListIds = [listId],
                    EmailBlacklisted = false
                };
                await client.UpdateContactAsync(subscription.Email, updateContact);
            }
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
        }
		_logger.LogWarning("<<< Exiting SubscribeAsync(subscription={Subscription})", subscription);
    }

    /// <summary>
    /// Unsubscribe contact
    /// </summary>
    /// <param name="subscription">Subscription</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async System.Threading.Tasks.Task UnsubscribeAsync(NewsLetterSubscription subscription)
    {
		_logger.LogError(">>> Entering UnsubscribeAsync(subscription={Subscription})", subscription);
        try
        {
            //try to get list id
            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>();
			_logger.LogError("UnsubscribeAsync(subscription={Subscription}): brevoSettings → {BrevoSettings}", subscription, brevoSettings);
            if (!brevoSettings.SubscriptionTypeMappings.TryGetValue(subscription.TypeId, out var listId) || listId == 0)
                return;

            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));
			_logger.LogError("UnsubscribeAsync(subscription={Subscription}): client → {Client}", subscription, client);

            //update contact
            var updateContact = new UpdateContact
            {
                UnlinkListIds = [listId]
            };
            await client.UpdateContactAsync(subscription.Email, updateContact);
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
        }
		_logger.LogWarning("<<< Exiting UnsubscribeAsync(subscription={Subscription})", subscription);
    }

    /// <summary>
    /// Unsubscribe contact
    /// </summary>
    /// <param name="request">HTTP request</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async System.Threading.Tasks.Task HandleWebhookAsync(Microsoft.AspNetCore.Http.HttpRequest request)
    {
		_logger.LogError(">>> Entering HandleWebhookAsync(request={Request})", request);
        await HandleFunctionAsync(async () =>
        {
            using var streamReader = new StreamReader(request.Body);
			_logger.LogError("HandleWebhookAsync(request={Request}): streamReader → {StreamReader}", request, streamReader);
            var requestContent = await streamReader.ReadToEndAsync();
			_logger.LogError("HandleWebhookAsync(request={Request}): requestContent → {RequestContent}", request, requestContent);

            //parse string to JSON object
            var unsubscriber = JsonConvert
                .DeserializeAnonymousType(requestContent, new { list_id = new List<int>(), email = string.Empty });

            if (unsubscriber.list_id?.Any() != true || string.IsNullOrEmpty(unsubscriber.email))
                return true;

            //get subscriptions by email
            var subscriptions = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionsByEmailAsync(unsubscriber.email, isActive: true);
			_logger.LogError("HandleWebhookAsync(request={Request}): subscriptions → {Subscriptions}", request, subscriptions);
            if (!subscriptions.Any())
                return true;

            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>();
			_logger.LogError("HandleWebhookAsync(request={Request}): brevoSettings → {BrevoSettings}", request, brevoSettings);
            foreach (var mapping in brevoSettings.SubscriptionTypeMappings)
            {
                var typeId = mapping.Key;
                var listId = mapping.Value;
                if (typeId == 0 || listId == 0 || !unsubscriber.list_id.Contains(listId))
                    continue;

                if (subscriptions.FirstOrDefault(subscription => subscription.TypeId == typeId) is not NewsLetterSubscription subscription)
                    continue;

                //update subscription
                subscription.Active = false;
				_logger.LogDebug("HandleWebhookAsync(request={Request}): subscription.Active → {Arg1}", request, subscription.Active);
                await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
                await _logger.InformationAsync($"{BrevoDefaults.SystemName} unsubscription: email '{subscription.Email}', subscription type #{subscription.TypeId}");
            }

		_logger.LogWarning("<<< Exiting HandleWebhookAsync(request={Request})", request);
            return true;
        });
    }

    /// <summary>
    /// Create webhook to get notification about unsubscribed contacts
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the webhook id
    /// </returns>
    public async Task<int> GetUnsubscribeWebHookIdAsync()
    {
		_logger.LogError(">>> Entering GetUnsubscribeWebHookIdAsync()");
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new WebhooksApi(config));
			_logger.LogError("GetUnsubscribeWebHookIdAsync(): client → {Client}", client);

            //check whether webhook already exist
            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>();
			_logger.LogError("GetUnsubscribeWebHookIdAsync(): brevoSettings → {BrevoSettings}", brevoSettings);
            if (brevoSettings.UnsubscribeWebhookId != 0)
            {
                await client.GetWebhookAsync(brevoSettings.UnsubscribeWebhookId);
                return brevoSettings.UnsubscribeWebhookId;
            }

            //or create new one
            var notificationUrl = _nopUrlHelper.RouteUrl(BrevoDefaults.UnsubscribeContactRoute, null, _webHelper.GetCurrentRequestProtocol());
			_logger.LogError("GetUnsubscribeWebHookIdAsync(): notificationUrl → {NotificationUrl}", notificationUrl);
            var webhook = new CreateWebhook(notificationUrl, "Unsubscribe event webhook",
                [CreateWebhook.EventsEnum.Unsubscribed], CreateWebhook.TypeEnum.Transactional);
            var result = await client.CreateWebhookAsync(webhook);
			_logger.LogError("GetUnsubscribeWebHookIdAsync(): result → {Result}", result);

            return (int)result.Id;
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
			_logger.LogWarning("<<< Exiting GetUnsubscribeWebHookIdAsync()");
            return 0;
        }
    }

    /// <summary>
    /// Update contact after completing order
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async System.Threading.Tasks.Task UpdateContactAfterCompletingOrderAsync(Core.Domain.Orders.Order order)
    {
		_logger.LogError(">>> Entering UpdateContactAfterCompletingOrderAsync(order={Order})", order);
        try
        {
            ArgumentNullException.ThrowIfNull(order);

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
			_logger.LogError("UpdateContactAfterCompletingOrderAsync(order={Order}): customer → {Customer}", order, customer);
            if (customer.Email is null)
                return;

            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));
			_logger.LogError("UpdateContactAfterCompletingOrderAsync(order={Order}): client → {Client}", order, client);

            try
            {
                var contactInfo = await client.GetContactInfoAsync(customer.Email);
				_logger.LogError("UpdateContactAfterCompletingOrderAsync(order={Order}): contactInfo → {ContactInfo}", order, contactInfo);
            }
            catch (ApiException apiException)
            {
                if (apiException.ErrorCode == 404)
                {
                    return;
                }
                else
                {
                    await _logger.ErrorAsync($"Brevo error: {apiException.Message}.", apiException, await _workContext.GetCurrentCustomerAsync());
                    return;
                }
            }

            //update contact
            var attributes = new Dictionary<string, string>
            {
                [BrevoDefaults.IdServiceAttribute] = order.Id.ToString(),
                [BrevoDefaults.OrderIdServiceAttribute] = order.Id.ToString(),
                [BrevoDefaults.OrderDateServiceAttribute] = order.PaidDateUtc.ToString(),
                [BrevoDefaults.OrderTotalServiceAttribute] = order.OrderTotal.ToString()
            };
            var updateContact = new UpdateContact { Attributes = attributes };
            await client.UpdateContactAsync(customer.Email, updateContact);

        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
        }
		_logger.LogWarning("<<< Exiting UpdateContactAfterCompletingOrderAsync(order={Order})", order);
    }

    #endregion

    #region Common

    /// <summary>
    /// Check whether the plugin is configured
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>Result</returns>
    public static bool IsConfigured(BrevoSettings settings)
    {
		_logger.LogError(">>> Entering IsConfigured(settings={Settings})", settings);
        //API key is required to request remote services
		_logger.LogWarning("<<< Exiting IsConfigured(settings={Settings})", settings);
        return !string.IsNullOrEmpty(settings?.ApiKey);
    }

    /// <summary>
    /// Get account information
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the account info; whether marketing automation is enabled, errors if exist
    /// </returns>
    public async Task<(string Info, bool MarketingAutomationEnabled, string MAkey, string Errors)> GetAccountInfoAsync()
    {
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new AccountApi(config));

            //get account
            var account = await client.GetAccountAsync();

            //prepare info
            var info = string.Format("First name: {1}{0}Last name: {2}{0}Email: {3}{0}Email credits: {4}{0}SMS credits: {5}{0}",
                Environment.NewLine,
                WebUtility.HtmlEncode(account.FirstName),
                WebUtility.HtmlEncode(account.LastName),
                WebUtility.HtmlEncode(account.Email),
                account.Plan.Where(plan => plan.Type != GetAccountPlan.TypeEnum.Sms).Sum(plan => plan.Credits),
                account.Plan.Where(plan => plan.Type == GetAccountPlan.TypeEnum.Sms).Sum(plan => plan.Credits));

            //get marketing automation tacker ID
            var key = account.MarketingAutomation?.Key ?? string.Empty;

            return (info, account.MarketingAutomation?.Enabled ?? false, key, null);
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return (null, false, null, exception.Message);
        }
    }

    /// <summary>
    /// Set partner value
    /// </summary>
    /// <returns>True if partner successfully set; otherwise false</returns>
    public async Task<bool> SetPartnerAsync()
    {
		_logger.LogError(">>> Entering SetPartnerAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var stores = (await _storeService.GetAllStoresAsync()).ToList();
			_logger.LogError("SetPartnerAsync(): stores → {Stores}", stores);
            var storeCredentials = new Dictionary<string, string>();
			_logger.LogError("SetPartnerAsync(): storeCredentials → {StoreCredentials}", storeCredentials);
            foreach (var store in stores)
            {
                var bSettings = await _settingService.LoadSettingAsync<BrevoSettings>(store.Id);
				_logger.LogError("SetPartnerAsync(): bSettings → {BSettings}", bSettings);
                var apiKey = bSettings.ApiKey;
                if (!string.IsNullOrEmpty(apiKey) && !storeCredentials.Where(s => s.Value == apiKey).Any())
                    storeCredentials.Add(store.Url, apiKey);
            }

            //whether plugin is configured
            if (!storeCredentials.Any())
                return false;

            foreach (var storeCredential in storeCredentials)
            {
                await HttpBrevoClientAsync(storeCredential.Key, storeCredential.Value);
            }
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
			_logger.LogError(">>> Entering HttpBrevoClientAsync(storeUrl={StoreUrl},apiKey=<redacted>)", storeUrl);
            var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogError("HttpBrevoClientAsync(storeUrl,apiKey)={StoreUrl},<redacted>: external service call {StoreUrl1} took {Arg3} ms", storeUrl, storeUrl, __logStart.ElapsedMilliseconds);
			_logger.LogWarning("<<< Exiting HttpBrevoClientAsync(storeUrl={StoreUrl},apiKey=<redacted>)", storeUrl);
            return false;
        }

		_logger.LogWarning("<<< Exiting SetPartnerAsync()");
        return true;

        async System.Threading.Tasks.Task HttpBrevoClientAsync(string storeUrl, string apiKey)
        {
			_logger.LogError(">>> Entering HttpBrevoClientAsync(storeUrl={StoreUrl},apiKey=<redacted>)", storeUrl);
            var __logStart = System.Diagnostics.Stopwatch.StartNew();
            //create API client
            var httpClient = new HttpClient
            {
                //configure client
                BaseAddress = new Uri(BrevoDefaults.AccountApiUrl),
                Timeout = TimeSpan.FromSeconds(10),
            };

            //Default Request Headers needed to be added in the HttpClient Object
            httpClient.DefaultRequestHeaders.Add(BrevoDefaults.ApiKeyHeader, apiKey);
            httpClient.DefaultRequestHeaders.Add(BrevoDefaults.SibPluginHeader, BrevoDefaults.PluginVersion);
			_logger.LogError("SetPartnerAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, BrevoDefaults.UserAgentAccountAPI);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

            var requestObject = new JObject
            {
                { "partnerName", BrevoDefaults.PartnerName },
                { "active", true },
                { "plugin_version", "1.0.0" },
                { "shop_version", NopVersion.FULL_VERSION },
                { "shop_url", storeUrl },
                { "created_at", DateTime.UtcNow },
                { "activated_at", DateTime.UtcNow },
                { "type", "sib" }
            };

            var requestString = JsonConvert.SerializeObject(requestObject);
			_logger.LogError("HttpBrevoClientAsync(storeUrl={StoreUrl},apiKey=<redacted>): requestString → {RequestString}", storeUrl, requestString);
            var requestContent = new StringContent(requestString, Encoding.Default, MimeTypes.ApplicationJson);
			_logger.LogError("HttpBrevoClientAsync(storeUrl={StoreUrl},apiKey=<redacted>): requestContent → {RequestContent}", storeUrl, requestContent);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "partner/information") { Content = requestContent };
			_logger.LogError("HttpBrevoClientAsync(storeUrl={StoreUrl},apiKey=<redacted>): requestMessage → {RequestMessage}", storeUrl, requestMessage);

            var httpResponse = await httpClient.SendAsync(requestMessage);
			_logger.LogError("HttpBrevoClientAsync(storeUrl={StoreUrl},apiKey=<redacted>): httpResponse → {HttpResponse}", storeUrl, httpResponse);
            httpResponse.EnsureSuccessStatusCode();
        }
    }

    /// <summary>
    /// Get available lists to synchronize contacts
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of id-name pairs of lists; errors if exist
    /// </returns>
    public async Task<(IList<(string Id, string Name)> Lists, string Errors)> GetListsAsync()
    {
        var availableLists = new List<(string Id, string Name)>();

        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));

            //get available lists
            var lists = await client.GetListsAsync(BrevoDefaults.DefaultSynchronizationListsLimit);

            //prepare id-name pairs
            var template = new { lists = new[] { new { id = string.Empty, name = string.Empty } } };
            var listObjects = JsonConvert.DeserializeAnonymousType(lists.ToJson(), template);
            if (listObjects?.lists != null)
            {
                foreach (var list in listObjects.lists)
                {
                    if (list != null)
                        availableLists.Add((list.id, list.name));
                }
            }
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return (availableLists, exception.Message);
        }

        return (availableLists, null);
    }

    /// <summary>
    /// Get available senders of transactional emails
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of id-name pairs of senders; errors if exist
    /// </returns>
    public async Task<(IList<(string Id, string Name)> Lists, string Errors)> GetSendersAsync()
    {
        var availableSenders = new List<(string Id, string Name)>();

        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new SendersApi(config));

            //get available senderes
            var senders = await client.GetSendersAsync();

            //prepare id-name pairs
            foreach (var sender in senders.Senders)
            {
                availableSenders.Add((sender.Id.ToString(), $"{sender.Name} ({sender.Email})"));
            }
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return (availableSenders, exception.Message);
        }

        return (availableSenders, null);
    }

    /// <summary>
    /// Get account language
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the BrevoAccountLanguage
    /// </returns>
    public async Task<BrevoAccountLanguage> GetAccountLanguageAsync()
    {
		_logger.LogError(">>> Entering GetAccountLanguageAsync()");
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));
			_logger.LogError("GetAccountLanguageAsync(): client → {Client}", client);

            var attributes = await client.GetAttributesAsync();
			_logger.LogError("GetAccountLanguageAsync(): attributes → {Attributes}", attributes);
            var allAttribytes = attributes.Attributes.Select(s => s.Name).ToList();
			_logger.LogError("GetAccountLanguageAsync(): allAttribytes → {AllAttribytes}", allAttribytes);

            var defaultNameAttributes = new List<string>
            {
                BrevoDefaults.FirstNameServiceAttribute,
                BrevoDefaults.LastNameServiceAttribute
            };
            if (defaultNameAttributes.All(attr => allAttribytes.Contains(attr)))
                return BrevoAccountLanguage.English;

            var frenchNameAttributes = new List<string>
            {
                BrevoDefaults.FirstNameFrenchServiceAttribute,
                BrevoDefaults.LastNameFrenchServiceAttribute
            };
            if (frenchNameAttributes.All(attr => allAttribytes.Contains(attr)))
                return BrevoAccountLanguage.French;

            var italianNameAttributes = new List<string>
            {
                BrevoDefaults.FirstNameItalianServiceAttribute,
                BrevoDefaults.LastNameItalianServiceAttribute
            };
            if (italianNameAttributes.All(attr => allAttribytes.Contains(attr)))
                return BrevoAccountLanguage.Italian;

            var spanishNameAttributes = new List<string>
            {
                BrevoDefaults.FirstNameSpanishServiceAttribute,
                BrevoDefaults.LastNameSpanishServiceAttribute
            };
            if (spanishNameAttributes.All(attr => allAttribytes.Contains(attr)))
                return BrevoAccountLanguage.Spanish;

            var germanNameAttributes = new List<string>
            {
                BrevoDefaults.FirstNameGermanServiceAttribute,
                BrevoDefaults.LastNameGermanServiceAttribute
            };
            if (germanNameAttributes.All(attr => allAttribytes.Contains(attr)))
                return BrevoAccountLanguage.German;

            var portugueseNameAttributes = new List<string>
            {
                BrevoDefaults.FirstNamePortugueseServiceAttribute,
                BrevoDefaults.LastNamePortugueseServiceAttribute
            };
            if (portugueseNameAttributes.All(attr => allAttribytes.Contains(attr)))
                return BrevoAccountLanguage.Portuguese;

            //Create default customer names attribytes
            var initialAttributes = new List<(CategoryEnum category, string Name, string Value, CreateAttribute.TypeEnum? Type)>
            {
                (CategoryEnum.Normal, BrevoDefaults.FirstNameServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.LastNameServiceAttribute, null, CreateAttribute.TypeEnum.Text)
            };
            //create attributes that are not already on account
            var newAttributes = new List<(CategoryEnum category, string Name, string Value, CreateAttribute.TypeEnum? Type)>();
			_logger.LogError("GetAccountLanguageAsync(): newAttributes → {NewAttributes}", newAttributes);
            foreach (var attribute in initialAttributes)
            {
                if (!allAttribytes.Contains(attribute.Name))
                    newAttributes.Add(attribute);
            }

            await CreateAttributesAsync(newAttributes);

            return BrevoAccountLanguage.English;
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
			_logger.LogWarning("<<< Exiting GetAccountLanguageAsync()");
			_logger.LogWarning("<<< Exiting CreateAttributesAsync(attributes={Attributes})", attributes);
            return BrevoAccountLanguage.English;
        }
    }

    /// <summary>
    /// Check and create missing attributes in account
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the errors if exist
    /// </returns>
    public async Task<string> PrepareAttributesAsync()
    {
		_logger.LogError(">>> Entering PrepareAttributesAsync()");
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));
			_logger.LogError("PrepareAttributesAsync(): client → {Client}", client);

            var attributes = await client.GetAttributesAsync();
			_logger.LogError("PrepareAttributesAsync(): attributes → {Attributes}", attributes);
            var attributeNames = attributes.Attributes.Select(s => s.Name).ToList();
			_logger.LogError("PrepareAttributesAsync(): attributeNames → {AttributeNames}", attributeNames);

            //prepare attributes to create
            var initialAttributes = new List<(CategoryEnum category, string Name, string Value, CreateAttribute.TypeEnum? Type)>
            {
                (CategoryEnum.Normal, BrevoDefaults.UsernameServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.PhoneServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.CountryServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.StoreIdServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.GenderServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.DateOfBirthServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.CompanyServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.Address1ServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.Address2ServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.ZipCodeServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.CityServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.CountyServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.StateServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.FaxServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Normal, BrevoDefaults.LanguageAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Transactional, BrevoDefaults.OrderIdServiceAttribute, null, CreateAttribute.TypeEnum.Id),
                (CategoryEnum.Transactional, BrevoDefaults.OrderDateServiceAttribute, null, CreateAttribute.TypeEnum.Text),
                (CategoryEnum.Transactional, BrevoDefaults.OrderTotalServiceAttribute, null, CreateAttribute.TypeEnum.Float),
                (CategoryEnum.Calculated, BrevoDefaults.OrderTotalSumServiceAttribute, $"SUM[{BrevoDefaults.OrderTotalServiceAttribute}]", null),
                (CategoryEnum.Calculated, BrevoDefaults.OrderTotalMonthSumServiceAttribute, $"SUM[{BrevoDefaults.OrderTotalServiceAttribute},{BrevoDefaults.OrderDateServiceAttribute},>,NOW(-30)]", null),
                (CategoryEnum.Calculated, BrevoDefaults.OrderCountServiceAttribute, $"COUNT[{BrevoDefaults.OrderIdServiceAttribute}]", null),
                (CategoryEnum.Global, BrevoDefaults.AllOrderTotalSumServiceAttribute, $"SUM[{BrevoDefaults.OrderTotalSumServiceAttribute}]", null),
                (CategoryEnum.Global, BrevoDefaults.AllOrderTotalMonthSumServiceAttribute, $"SUM[{BrevoDefaults.OrderTotalMonthSumServiceAttribute}]", null),
                (CategoryEnum.Global, BrevoDefaults.AllOrderCountServiceAttribute, $"SUM[{BrevoDefaults.OrderCountServiceAttribute}]", null)
            };

            //create attributes that are not already on account
            var newAttributes = new List<(CategoryEnum category, string Name, string Value, CreateAttribute.TypeEnum? Type)>();
			_logger.LogError("PrepareAttributesAsync(): newAttributes → {NewAttributes}", newAttributes);
            foreach (var attribute in initialAttributes)
            {
                if (!attributeNames.Contains(attribute.Name))
                    newAttributes.Add(attribute);
            }

            return await CreateAttributesAsync(newAttributes);
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
			_logger.LogWarning("<<< Exiting PrepareAttributesAsync()");
            return exception.Message;
        }
    }

    /// <summary>
    /// Move message template tokens to transactional attributes
    /// </summary>
    /// <param name="tokens">List of available message templates tokens</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the errors if exist
    /// </returns>
    public async Task<string> PrepareTransactionalAttributesAsync(IList<string> tokens)
    {
		_logger.LogError(">>> Entering PrepareTransactionalAttributesAsync(tokens=<redacted>)");
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new ContactsApi(config));
			_logger.LogError("PrepareTransactionalAttributesAsync(tokens=<redacted>): client → {Client}", client);

            //get already existing transactional attributes
            var attributes = await client.GetAttributesAsync();
			_logger.LogError("PrepareTransactionalAttributesAsync(tokens=<redacted>): attributes → {Attributes}", attributes);
            var transactionalAttributes = attributes.Attributes
                .Where(attribute => attribute.Category == CategoryEnum.Transactional).ToList();

            //bring tokens to attributes format
            tokens = tokens.Select(token => token.Replace("%", "").Replace(".", "_").Replace("(s)", "-s-").ToUpperInvariant()).ToList();
			_logger.LogError("PrepareTransactionalAttributesAsync(tokens=<redacted>): tokens → {Tokens1}", tokens);

            //get attributes that are not already on account
            tokens = tokens.Except(transactionalAttributes.Select(attribute => attribute.Name)).ToList();
            if (!tokens.Any())
                return string.Empty;

            //prepare attributes to create
            var newAttributes = new List<(CategoryEnum category, string Name, string Value, CreateAttribute.TypeEnum? Type)>();
			_logger.LogError("PrepareTransactionalAttributesAsync(tokens=<redacted>): newAttributes → {NewAttributes}", newAttributes);
            foreach (var token in tokens)
            {
                newAttributes.Add((CategoryEnum.Transactional, token, null, CreateAttribute.TypeEnum.Text));
            }

            return await CreateAttributesAsync(newAttributes);
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
			_logger.LogWarning("<<< Exiting PrepareTransactionalAttributesAsync(tokens=<redacted>)");
            return exception.Message;
        }
    }

    #endregion

    #region SMTP

    /// <summary>
    /// Check whether SMTP is enabled on account
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result of check; errors if exist
    /// </returns>
    public async Task<(bool Enabled, string Errors)> SmtpIsEnabledAsync()
    {
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new AccountApi(config));

            //get account
            var account = await client.GetAccountAsync();
            return (account.Relay?.Enabled ?? false, null);
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return (false, exception.Message);
        }
    }

    /// <summary>
    /// Get email account identifier
    /// </summary>
    /// <param name="senderId">Sender identifier</param>
    /// <param name="smtpKey">SMTP key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the email account identifier; errors if exist
    /// </returns>
    public async Task<(int Id, string Errors)> GetEmailAccountIdAsync(string senderId, string smtpKey)
    {
        try
        {
            //create API clients
            var sendersClient = await CreateApiClientAsync(config => new SendersApi(config));
            var accountClient = await CreateApiClientAsync(config => new AccountApi(config));

            //get all available senders
            var senders = await sendersClient.GetSendersAsync();
            if (!senders.Senders.Any())
                return (0, "There are no senders");

            var currentSender = senders.Senders.FirstOrDefault(sender => sender.Id.ToString() == senderId);
            if (currentSender != null)
            {
                //try to find existing email account by name and email
                var emailAccount = (await _emailAccountService.GetAllEmailAccountsAsync())
                    .FirstOrDefault(account => account.DisplayName == currentSender.Name && account.Email == currentSender.Email);
                if (emailAccount != null)
                    return (emailAccount.Id, null);
            }

            //or create new one
            currentSender ??= senders.Senders.FirstOrDefault();
            var relay = (await accountClient.GetAccountAsync()).Relay;
            var newEmailAccount = new EmailAccount
            {
                Host = relay?.Data?.Relay,
                Port = relay?.Data?.Port ?? 0,
                Username = relay?.Data?.UserName,
                Password = smtpKey,
                EnableSsl = true,
                Email = currentSender.Email,
                DisplayName = currentSender.Name
            };
            await _emailAccountService.InsertEmailAccountAsync(newEmailAccount);

            return (newEmailAccount.Id, null);
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
            return (0, exception.Message);
        }
    }

    /// <summary>
    /// Get email template identifier
    /// </summary>
    /// <param name="templateId">Current email template id</param>
    /// <param name="message">Message template</param>
    /// <param name="emailAccount">Email account</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the email template identifier
    /// </returns>
    public async Task<int?> GetTemplateIdAsync(int? templateId, MessageTemplate message, EmailAccount emailAccount)
    {
		_logger.LogError(">>> Entering GetTemplateIdAsync(templateId={TemplateId},message={Message},emailAccount={EmailAccount})", templateId, message, emailAccount);
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new TransactionalEmailsApi(config));
			_logger.LogError("GetTemplateIdAsync(templateId={TemplateId},message={Message},emailAccount={EmailAccount}): client → {Client}", templateId, message, emailAccount, client);

            //check whether email template already exists
            if (templateId > 0)
            {
                await client.GetSmtpTemplateAsync(templateId);
                return templateId;
            }

            //or create new one
            if (emailAccount == null)
				_logger.LogError("Throwing NopException from GetTemplateIdAsync(templateId={TemplateId},message={Message},emailAccount={EmailAccount})", templateId, message, emailAccount);
                throw new NopException("Email account not configured");

            //the original body and subject of the email template are the same as that of the message template
            var body = message.Body.Replace("%if", "\"if\"").Replace("endif%", "\"endif\"");
			_logger.LogError("GetTemplateIdAsync(templateId={TemplateId},message={Message},emailAccount={EmailAccount}): body → {Body}", templateId, message, emailAccount, body);
            body = SpecialCharRegex().Replace(body, x => $"{{{{ params.{x.ToString().Replace("%", "").Replace(".", "_").ToUpperInvariant()} }}}}");
            var subject = message.Subject.Replace("%if", "\"if\"").Replace("endif%", "\"endif\"");
			_logger.LogError("GetTemplateIdAsync(templateId={TemplateId},message={Message},emailAccount={EmailAccount}): subject → {Subject}", templateId, message, emailAccount, subject);
            subject = SpecialCharRegex().Replace(subject, x => $"{{{{ params.{x.ToString().Replace("%", "").Replace(".", "_").ToUpperInvariant()} }}}}");

            //create email template
            var createSmtpTemplate = new CreateSmtpTemplate(sender: new CreateSmtpTemplateSender(emailAccount.DisplayName, emailAccount.Email),
                templateName: message.Name, htmlContent: body, subject: subject, isActive: true);
            var emailTemplate = await client.CreateSmtpTemplateAsync(createSmtpTemplate);
			_logger.LogError("GetTemplateIdAsync(templateId={TemplateId},message={Message},emailAccount={EmailAccount}): emailTemplate → {EmailTemplate}", templateId, message, emailAccount, emailTemplate);

            return (int?)emailTemplate.Id;
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
			_logger.LogWarning("<<< Exiting GetTemplateIdAsync(templateId={TemplateId},message={Message},emailAccount={EmailAccount})", templateId, message, emailAccount);
            return null;
        }
    }

    /// <summary>
    /// Convert Brevo email template to queued email
    /// </summary>
    /// <param name="templateId">Email template identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email
    /// </returns>
    public async Task<QueuedEmail> GetQueuedEmailFromTemplateAsync(int templateId)
    {
		_logger.LogError(">>> Entering GetQueuedEmailFromTemplateAsync(templateId={TemplateId})", templateId);
        try
        {
            //create API client
            var client = await CreateApiClientAsync(config => new TransactionalEmailsApi(config));
			_logger.LogError("GetQueuedEmailFromTemplateAsync(templateId={TemplateId}): client → {Client}", templateId, client);

            if (templateId == 0)
				_logger.LogError("Throwing NopException from GetQueuedEmailFromTemplateAsync(templateId={TemplateId})", templateId);
                throw new NopException("Message template is empty");

            //get template
            var template = await client.GetSmtpTemplateAsync(templateId);
			_logger.LogError("GetQueuedEmailFromTemplateAsync(templateId={TemplateId}): template → {Template}", templateId, template);

            //bring attributes to tokens format
            var subject = ParamsRegex().Replace(template.Subject, x => $"%{x.ToString().Replace("{", "").Replace("}", "").Replace("params.", "").Replace("_", ".").Trim()}%");
			_logger.LogError("GetQueuedEmailFromTemplateAsync(templateId={TemplateId}): subject → {Subject}", templateId, subject);
            subject = subject.Replace("\"if\"", "%if").Replace("\"endif\"", "endif%");
            var body = ParamsRegex().Replace(template.HtmlContent, x => $"%{x.ToString().Replace("{", "").Replace("}", "").Replace("params.", "").Replace("_", ".").Trim()}%");
			_logger.LogError("GetQueuedEmailFromTemplateAsync(templateId={TemplateId}): body → {Body}", templateId, body);
            body = body.Replace("\"if\"", "%if").Replace("\"endif\"", "endif%");

            //map template to queued email
            return new QueuedEmail
            {
                Subject = subject,
                Body = body,
                FromName = template.Sender?.Name,
                From = template.Sender?.Email
            };
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo email sending error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
			_logger.LogWarning("<<< Exiting GetQueuedEmailFromTemplateAsync(templateId={TemplateId})", templateId);
            return null;
        }
    }

    #endregion

    #region SMS

    /// <summary>
    /// Send SMS 
    /// </summary>
    /// <param name="to">Phone number of the receiver</param>
    /// <param name="from">Name of sender</param>
    /// <param name="text">Text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async System.Threading.Tasks.Task SendSMSAsync(string to, string from, string text)
    {
		_logger.LogError(">>> Entering SendSMSAsync(to={To},from={From},text={Text})", to, from, text);
        //whether SMS notifications enabled
        var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>();
		_logger.LogError("SendSMSAsync(to={To},from={From},text={Text}): brevoSettings → {BrevoSettings}", to, from, text, brevoSettings);
        if (!brevoSettings.UseSmsNotifications)
            return;

        try
        {
            //check number and text
            if (string.IsNullOrEmpty(to) || string.IsNullOrEmpty(text))
				_logger.LogError("Throwing NopException from SendSMSAsync(to={To},from={From},text={Text})", to, from, text);
                throw new NopException("Phone number or SMS text is empty");

            //create API client
            var client = await CreateApiClientAsync(config => new TransactionalSMSApi(config));
			_logger.LogError("SendSMSAsync(to={To},from={From},text={Text}): client → {Client}", to, from, text, client);

            //create SMS data
            var transactionalSms = new SendTransacSms(sender: from, recipient: to, content: text, type: SendTransacSms.TypeEnum.Transactional);
			_logger.LogError("SendSMSAsync(to={To},from={From},text={Text}): transactionalSms → {TransactionalSms}", to, from, text, transactionalSms);

            //send SMS
            var sms = await client.SendTransacSmsAsync(transactionalSms);
			_logger.LogError("SendSMSAsync(to={To},from={From},text={Text}): sms → {Sms}", to, from, text, sms);
            await _logger.InformationAsync($"Brevo SMS sent: {sms?.Reference ?? $"credits remaining {sms?.RemainingCredits?.ToString()}"}");
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo SMS sending error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
        }
		_logger.LogWarning("<<< Exiting SendSMSAsync(to={To},from={From},text={Text})", to, from, text);
    }

    /// <summary>
    /// Send SMS campaign
    /// </summary>
    /// <param name="listId">Contact list identifier</param>
    /// <param name="from">Name of sender</param>
    /// <param name="text">Text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<string> SendSMSCampaignAsync(int listId, string from, string text)
    {
		_logger.LogError(">>> Entering SendSMSCampaignAsync(listId={ListId},from={From},text={Text})", listId, from, text);
        try
        {
            //check list and text
            if (listId == 0 || string.IsNullOrEmpty(text) || string.IsNullOrEmpty(from))
				_logger.LogError("Throwing NopException from SendSMSCampaignAsync(listId={ListId},from={From},text={Text})", listId, from, text);
                throw new NopException("List or SMS text or sender name is empty");

            //create API client
            var client = await CreateApiClientAsync(config => new SMSCampaignsApi(config));
			_logger.LogError("SendSMSCampaignAsync(listId={ListId},from={From},text={Text}): client → {Client}", listId, from, text, client);

            //create SMS campaign
            var campaign = await client.CreateSmsCampaignAsync(new CreateSmsCampaign(name: CommonHelper.EnsureMaximumLength(text, 20),
                sender: from, content: text, recipients: new CreateSmsCampaignRecipients([listId])));

            //send campaign
            await client.SendSmsCampaignNowAsync(campaign.Id);
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo SMS sending error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
		_logger.LogWarning("<<< Exiting SendSMSCampaignAsync(listId={ListId},from={From},text={Text})", listId, from, text);
            return exception.Message;
        }

        return string.Empty;
    }

    #endregion

    #endregion
}