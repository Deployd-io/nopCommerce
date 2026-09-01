using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Messages;
using Nop.Plugin.Misc.Brevo.Models;
using Nop.Plugin.Misc.Brevo.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Brevo.Controllers;

[AutoValidateAntiforgeryToken]
public class BrevoController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly BrevoManager _brevoEmailManager;
    protected readonly IEmailAccountService _emailAccountService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly IMessageTemplateService _messageTemplateService;
    protected readonly IMessageTokenProvider _messageTokenProvider;
    protected readonly INewsLetterSubscriptionTypeService _newsLetterSubscriptionTypeService;
    protected readonly INotificationService _notificationService;
    protected readonly ISettingService _settingService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IStoreService _storeService;
    protected readonly IWorkContext _workContext;
    protected readonly MessageTemplatesSettings _messageTemplatesSettings;

    #endregion

    #region Ctor

    public BrevoController(BrevoManager brevoEmailManager,
        IEmailAccountService emailAccountService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        ILogger logger,
        IMessageTemplateService messageTemplateService,
        IMessageTokenProvider messageTokenProvider,
        INewsLetterSubscriptionTypeService newsLetterSubscriptionTypeService,
        INotificationService notificationService,
        ISettingService settingService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IStoreService storeService,
        IWorkContext workContext,
        MessageTemplatesSettings messageTemplatesSettings)
    {
        _brevoEmailManager = brevoEmailManager;
		_logger.LogDebug("BrevoController(brevoEmailManager={BrevoEmailManager},emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},logger={Logger},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},settingService={SettingService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>,storeService={StoreService},workContext={WorkContext},messageTemplatesSettings={MessageTemplatesSettings}): _brevoEmailManager → {_brevoEmailManager}", brevoEmailManager, emailAccountService, genericAttributeService, localizationService, logger, messageTemplateService, newsLetterSubscriptionTypeService, notificationService, settingService, staticCacheManager, storeContext, storeService, workContext, messageTemplatesSettings, _brevoEmailManager);
        _emailAccountService = emailAccountService;
		_logger.LogDebug("BrevoController(brevoEmailManager={BrevoEmailManager},emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},logger={Logger},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},settingService={SettingService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>,storeService={StoreService},workContext={WorkContext},messageTemplatesSettings={MessageTemplatesSettings}): _emailAccountService → {_emailAccountService}", brevoEmailManager, emailAccountService, genericAttributeService, localizationService, logger, messageTemplateService, newsLetterSubscriptionTypeService, notificationService, settingService, staticCacheManager, storeContext, storeService, workContext, messageTemplatesSettings, _emailAccountService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("BrevoController(brevoEmailManager={BrevoEmailManager},emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},logger={Logger},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},settingService={SettingService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>,storeService={StoreService},workContext={WorkContext},messageTemplatesSettings={MessageTemplatesSettings}): _genericAttributeService → {_genericAttributeService}", brevoEmailManager, emailAccountService, genericAttributeService, localizationService, logger, messageTemplateService, newsLetterSubscriptionTypeService, notificationService, settingService, staticCacheManager, storeContext, storeService, workContext, messageTemplatesSettings, _genericAttributeService);
        _localizationService = localizationService;
		_logger.LogDebug("BrevoController(brevoEmailManager={BrevoEmailManager},emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},logger={Logger},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},settingService={SettingService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>,storeService={StoreService},workContext={WorkContext},messageTemplatesSettings={MessageTemplatesSettings}): _localizationService → {_localizationService}", brevoEmailManager, emailAccountService, genericAttributeService, localizationService, logger, messageTemplateService, newsLetterSubscriptionTypeService, notificationService, settingService, staticCacheManager, storeContext, storeService, workContext, messageTemplatesSettings, _localizationService);
        _logger = logger;
		_logger.LogDebug(">>> Entering BrevoController(brevoEmailManager={BrevoEmailManager},emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},logger={Logger},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},settingService={SettingService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>,storeService={StoreService},workContext={WorkContext},messageTemplatesSettings={MessageTemplatesSettings})", brevoEmailManager, emailAccountService, genericAttributeService, localizationService, logger, messageTemplateService, newsLetterSubscriptionTypeService, notificationService, settingService, staticCacheManager, storeContext, storeService, workContext, messageTemplatesSettings);
        _messageTemplateService = messageTemplateService;
		_logger.LogDebug("BrevoController(brevoEmailManager={BrevoEmailManager},emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},logger={Logger},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},settingService={SettingService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>,storeService={StoreService},workContext={WorkContext},messageTemplatesSettings={MessageTemplatesSettings}): _messageTemplateService → {_messageTemplateService}", brevoEmailManager, emailAccountService, genericAttributeService, localizationService, logger, messageTemplateService, newsLetterSubscriptionTypeService, notificationService, settingService, staticCacheManager, storeContext, storeService, workContext, messageTemplatesSettings, _messageTemplateService);
        _messageTokenProvider = messageTokenProvider;
        _newsLetterSubscriptionTypeService = newsLetterSubscriptionTypeService;
		_logger.LogDebug("BrevoController(brevoEmailManager={BrevoEmailManager},emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},logger={Logger},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},settingService={SettingService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>,storeService={StoreService},workContext={WorkContext},messageTemplatesSettings={MessageTemplatesSettings}): _messageTokenProvider → {_messageTokenProvider}", brevoEmailManager, emailAccountService, genericAttributeService, localizationService, logger, messageTemplateService, newsLetterSubscriptionTypeService, notificationService, settingService, staticCacheManager, storeContext, storeService, workContext, messageTemplatesSettings, _messageTokenProvider);
		_logger.LogDebug("BrevoController(brevoEmailManager={BrevoEmailManager},emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},logger={Logger},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},settingService={SettingService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>,storeService={StoreService},workContext={WorkContext},messageTemplatesSettings={MessageTemplatesSettings}): _newsLetterSubscriptionTypeService → {_newsLetterSubscriptionTypeService}", brevoEmailManager, emailAccountService, genericAttributeService, localizationService, logger, messageTemplateService, newsLetterSubscriptionTypeService, notificationService, settingService, staticCacheManager, storeContext, storeService, workContext, messageTemplatesSettings, _newsLetterSubscriptionTypeService);
        _notificationService = notificationService;
        _settingService = settingService;
		_logger.LogDebug("BrevoController(brevoEmailManager={BrevoEmailManager},emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},logger={Logger},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},settingService={SettingService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>,storeService={StoreService},workContext={WorkContext},messageTemplatesSettings={MessageTemplatesSettings}): _notificationService → {_notificationService}", brevoEmailManager, emailAccountService, genericAttributeService, localizationService, logger, messageTemplateService, newsLetterSubscriptionTypeService, notificationService, settingService, staticCacheManager, storeContext, storeService, workContext, messageTemplatesSettings, _notificationService);
		_logger.LogDebug("BrevoController(brevoEmailManager={BrevoEmailManager},emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},logger={Logger},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},settingService={SettingService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>,storeService={StoreService},workContext={WorkContext},messageTemplatesSettings={MessageTemplatesSettings}): _settingService → {_settingService}", brevoEmailManager, emailAccountService, genericAttributeService, localizationService, logger, messageTemplateService, newsLetterSubscriptionTypeService, notificationService, settingService, staticCacheManager, storeContext, storeService, workContext, messageTemplatesSettings, _settingService);
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
		_logger.LogDebug("BrevoController(brevoEmailManager={BrevoEmailManager},emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},logger={Logger},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},settingService={SettingService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>,storeService={StoreService},workContext={WorkContext},messageTemplatesSettings={MessageTemplatesSettings}): _staticCacheManager → {_staticCacheManager}", brevoEmailManager, emailAccountService, genericAttributeService, localizationService, logger, messageTemplateService, newsLetterSubscriptionTypeService, notificationService, settingService, staticCacheManager, storeContext, storeService, workContext, messageTemplatesSettings, _staticCacheManager);
		_logger.LogDebug("BrevoController(brevoEmailManager={BrevoEmailManager},emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},logger={Logger},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},settingService={SettingService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>,storeService={StoreService},workContext={WorkContext},messageTemplatesSettings={MessageTemplatesSettings}): _storeContext → {_storeContext}", brevoEmailManager, emailAccountService, genericAttributeService, localizationService, logger, messageTemplateService, newsLetterSubscriptionTypeService, notificationService, settingService, staticCacheManager, storeContext, storeService, workContext, messageTemplatesSettings, _storeContext);
        _storeMappingService = storeMappingService;
        _storeService = storeService;
		_logger.LogDebug("BrevoController(brevoEmailManager={BrevoEmailManager},emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},logger={Logger},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},settingService={SettingService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>,storeService={StoreService},workContext={WorkContext},messageTemplatesSettings={MessageTemplatesSettings}): _storeMappingService → {_storeMappingService}", brevoEmailManager, emailAccountService, genericAttributeService, localizationService, logger, messageTemplateService, newsLetterSubscriptionTypeService, notificationService, settingService, staticCacheManager, storeContext, storeService, workContext, messageTemplatesSettings, _storeMappingService);
		_logger.LogDebug("BrevoController(brevoEmailManager={BrevoEmailManager},emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},logger={Logger},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},settingService={SettingService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>,storeService={StoreService},workContext={WorkContext},messageTemplatesSettings={MessageTemplatesSettings}): _storeService → {_storeService}", brevoEmailManager, emailAccountService, genericAttributeService, localizationService, logger, messageTemplateService, newsLetterSubscriptionTypeService, notificationService, settingService, staticCacheManager, storeContext, storeService, workContext, messageTemplatesSettings, _storeService);
        _workContext = workContext;
        _messageTemplatesSettings = messageTemplatesSettings;
		_logger.LogDebug("<<< Exiting BrevoController(brevoEmailManager={BrevoEmailManager},emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},logger={Logger},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},settingService={SettingService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>,storeService={StoreService},workContext={WorkContext},messageTemplatesSettings={MessageTemplatesSettings})", brevoEmailManager, emailAccountService, genericAttributeService, localizationService, logger, messageTemplateService, newsLetterSubscriptionTypeService, notificationService, settingService, staticCacheManager, storeContext, storeService, workContext, messageTemplatesSettings);
		_logger.LogDebug("BrevoController(brevoEmailManager={BrevoEmailManager},emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},logger={Logger},messageTemplateService={MessageTemplateService},messageTokenProvider=<redacted>,newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},settingService={SettingService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>,storeService={StoreService},workContext={WorkContext},messageTemplatesSettings={MessageTemplatesSettings}): _workContext → {_workContext}", brevoEmailManager, emailAccountService, genericAttributeService, localizationService, logger, messageTemplateService, newsLetterSubscriptionTypeService, notificationService, settingService, staticCacheManager, storeContext, storeService, workContext, messageTemplatesSettings, _workContext);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare ConfigurationModel
    /// </summary>
    /// <param name="model">Model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task PrepareModelAsync(ConfigurationModel model)
    {
		_logger.LogDebug(">>> Entering PrepareModelAsync(model={Model})", model);
        //load settings for active store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareModelAsync(model={Model}): storeId → {StoreId}", model, storeId);
        var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId);
		_logger.LogDebug("PrepareModelAsync(model={Model}): brevoSettings → {BrevoSettings}", model, brevoSettings);

        //whether plugin is configured
        if (string.IsNullOrEmpty(brevoSettings.ApiKey))
            return;

        //prepare common properties
        model.ActiveStoreScopeConfiguration = storeId;
		_logger.LogDebug("PrepareModelAsync(model={Model}): model.ActiveStoreScopeConfiguration → {Arg1}", model, model.ActiveStoreScopeConfiguration);
		_logger.LogDebug("PrepareModelAsync(model={Model}): model.ApiKey → {Arg1}", model, model.ApiKey);
		_logger.LogDebug("PrepareModelAsync(model={Model}): model.SmtpKey → {Arg1}", model, model.SmtpKey);
		_logger.LogDebug("PrepareModelAsync(model={Model}): model.SenderId → {Arg1}", model, model.SenderId);
		_logger.LogDebug("PrepareModelAsync(model={Model}): model.UseSmsNotifications → {Arg1}", model, model.UseSmsNotifications);
		_logger.LogDebug("PrepareModelAsync(model={Model}): model.SmsSenderName → {Arg1}", model, model.SmsSenderName);
		_logger.LogDebug("PrepareModelAsync(model={Model}): model.StoreOwnerPhoneNumber → {Arg1}", model, model.StoreOwnerPhoneNumber);
		_logger.LogDebug("PrepareModelAsync(model={Model}): model.UseMarketingAutomation → {Arg1}", model, model.UseMarketingAutomation);
		_logger.LogDebug("PrepareModelAsync(model={Model}): model.TrackingScript → {Arg1}", model, model.TrackingScript);
        model.ApiKey = brevoSettings.ApiKey;
        model.SmtpKey = brevoSettings.SmtpKey;
        model.SenderId = brevoSettings.SenderId;
        model.UseSmsNotifications = brevoSettings.UseSmsNotifications;
        model.SmsSenderName = brevoSettings.SmsSenderName;
        model.StoreOwnerPhoneNumber = brevoSettings.StoreOwnerPhoneNumber;
        model.UseMarketingAutomation = brevoSettings.UseMarketingAutomation;
        model.TrackingScript = brevoSettings.TrackingScript;

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("PrepareModelAsync(model={Model}): customer → {Customer}", model, customer);
        model.HideGeneralBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, BrevoDefaults.HideGeneralBlock);
        model.HideSynchronizationBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, BrevoDefaults.HideSynchronizationBlock);
        model.HideTransactionalBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, BrevoDefaults.HideTransactionalBlock);
        model.HideSmsBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, BrevoDefaults.HideSmsBlock);
        model.HideMarketingAutomationBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, BrevoDefaults.HideMarketingAutomationBlock);

        //prepare nested search models
        model.MessageTemplateSearchModel.SetGridPageSize();
        model.SmsSearchModel.SetGridPageSize();

        //prepare add SMS model
        model.AddSms.AvailablePhoneTypes.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.MyPhone"), "0"));
        model.AddSms.AvailablePhoneTypes.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.CustomerPhone"), "1"));
        model.AddSms.AvailablePhoneTypes.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.BillingAddressPhone"), "2"));
        model.AddSms.DefaultSelectedPhoneTypeId = model.AddSms.AvailablePhoneTypes.First().Value;

        var stores = await _storeService.GetAllStoresAsync();
		_logger.LogDebug("PrepareModelAsync(model={Model}): stores → {Stores}", model, stores);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var messageTemplates = await _messageTemplateService.GetAllMessageTemplatesAsync(storeId);
        model.AddSms.AvailableMessages = await messageTemplates.SelectAwait(async messageTemplate =>
        {
			_logger.LogDebug("PrepareModelAsync(model={Model}): messageTemplates → {MessageTemplates}", model, messageTemplates);
            var name = messageTemplate.Name;
			_logger.LogDebug("PrepareModelAsync(model={Model}): name → {Name}", model, name);
            if (storeId == 0 && messageTemplate.LimitedToStores)
            {
                var storeIds = await _storeMappingService.GetStoresIdsWithAccessAsync(messageTemplate);
				_logger.LogDebug("PrepareModelAsync(model={Model}): storeIds → {StoreIds}", model, storeIds);
                var storeNames = stores.Where(store => storeIds.Contains(store.Id)).Select(store => store.Name);
				_logger.LogDebug("PrepareModelAsync(model={Model}): storeNames → {StoreNames}", model, storeNames);
                name = $"{name} ({string.Join(',', storeNames)})";
            }

            return new SelectListItem(name, messageTemplate.Id.ToString());
        }).ToListAsync();
        var defaultSelectedMessage = model.AddSms.AvailableMessages.FirstOrDefault();
		_logger.LogInformation("PrepareModelAsync(model)={Model}: database query executed in {Arg1} ms", model, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("PrepareModelAsync(model={Model}): defaultSelectedMessage → {DefaultSelectedMessage}", model, defaultSelectedMessage);
        model.AddSms.DefaultSelectedMessageId = defaultSelectedMessage?.Value ?? "0";

        //check whether email account exists
        if (brevoSettings.UseSmtp && await _emailAccountService.GetEmailAccountByIdAsync(brevoSettings.EmailAccountId) != null)
            model.UseSmtp = brevoSettings.UseSmtp;
			_logger.LogDebug("PrepareModelAsync(model={Model}): model.UseSmtp → {Arg1}", model, model.UseSmtp);

        //get account info
        var (accountInfo, marketingAutomationEnabled, maKey, accountErrors) = await _brevoEmailManager.GetAccountInfoAsync();
        model.AccountInfo = accountInfo;
		_logger.LogDebug("PrepareModelAsync(model={Model}): model.AccountInfo → {Arg1}", model, model.AccountInfo);
		_logger.LogDebug("PrepareModelAsync(model={Model}): model.MarketingAutomationKey → {Arg1}", model, model.MarketingAutomationKey);
		_logger.LogDebug("PrepareModelAsync(model={Model}): model.MarketingAutomationDisabled → {Arg1}", model, model.MarketingAutomationDisabled);
        model.MarketingAutomationKey = maKey;
        model.MarketingAutomationDisabled = !marketingAutomationEnabled;
        if (!string.IsNullOrEmpty(accountErrors))
            _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {accountErrors}");

        //prepare overridable settings
        if (storeId > 0)
        {
            model.UseSmtp_OverrideForStore = await _settingService.SettingExistsAsync(brevoSettings, settings => settings.UseSmtp, storeId);
            model.SenderId_OverrideForStore = await _settingService.SettingExistsAsync(brevoSettings, settings => settings.SenderId, storeId);
            model.UseSmsNotifications_OverrideForStore = await _settingService.SettingExistsAsync(brevoSettings, settings => settings.UseSmsNotifications, storeId);
            model.SmsSenderName_OverrideForStore = await _settingService.SettingExistsAsync(brevoSettings, settings => settings.SmsSenderName, storeId);
            model.UseMarketingAutomation_OverrideForStore = await _settingService.SettingExistsAsync(brevoSettings, settings => settings.UseMarketingAutomation, storeId);
        }

        //check SMTP status
        var (smtpEnabled, smtpErrors) = await _brevoEmailManager.SmtpIsEnabledAsync();
        if (!string.IsNullOrEmpty(smtpErrors))
            _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {smtpErrors}");

        var newsLetterSubscriptionTypes = await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync(storeId);
		_logger.LogDebug("PrepareModelAsync(model={Model}): newsLetterSubscriptionTypes → {NewsLetterSubscriptionTypes}", model, newsLetterSubscriptionTypes);
        model.NewsLetterSubscriptionTypes = await newsLetterSubscriptionTypes.Select(subscriptionType => new NewsLetterSubscriptionMapModel
        {
            TypeId = subscriptionType.Id,
            Name = subscriptionType.Name,
            ListId = brevoSettings.SubscriptionTypeMappings.TryGetValue(subscriptionType.Id, out var value) ? value : 0,
        }).ToListAsync();

        //get available contact lists to synchronize
        var (lists, listsErrors) = await _brevoEmailManager.GetListsAsync();
        model.AvailableLists = lists.Select(list => new SelectListItem(list.Name, list.Id)).ToList();
        model.AvailableLists.Insert(0, new SelectListItem("Select list", "0"));
        if (!string.IsNullOrEmpty(listsErrors))
            _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {listsErrors}");

        //get available senders of emails from account
        var (senders, sendersErrors) = await _brevoEmailManager.GetSendersAsync();
        model.AvailableSenders = senders.Select(list => new SelectListItem(list.Name, list.Id)).ToList();
        model.AvailableSenders.Insert(0, new SelectListItem("Select sender", "0"));
        if (!string.IsNullOrEmpty(sendersErrors))
            _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {sendersErrors}");

        //get allowed tokens
        model.AllowedTokens = string.Join(", ", await _messageTokenProvider.GetListOfAllowedTokensAsync());

        //create attributes in account
        var attributesErrors = await _brevoEmailManager.PrepareAttributesAsync();
		_logger.LogDebug("PrepareModelAsync(model={Model}): attributesErrors → {AttributesErrors}", model, attributesErrors);
        if (!string.IsNullOrEmpty(attributesErrors))
            _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {attributesErrors}");

        //try to set account partner
        if (!brevoSettings.PartnerValueSet)
        {
            var partnerSet = await _brevoEmailManager.SetPartnerAsync();
			_logger.LogDebug("PrepareModelAsync(model={Model}): partnerSet → {PartnerSet}", model, partnerSet);
            if (partnerSet)
            {
                brevoSettings.PartnerValueSet = true;
				_logger.LogDebug("PrepareModelAsync(model={Model}): brevoSettings.PartnerValueSet → {Arg1}", model, brevoSettings.PartnerValueSet);
                _settingService.SaveSetting(brevoSettings, settings => settings.PartnerValueSet, clearCache: false);
                _settingService.ClearCache();
            }
        }
		_logger.LogDebug("<<< Exiting PrepareModelAsync(model={Model})", model);
    }

    #endregion

    #region Methods

    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    public async Task<IActionResult> Configure()
    {
		_logger.LogDebug(">>> Entering Configure()");
        var model = new ConfigurationModel();
        await PrepareModelAsync(model);
		_logger.LogDebug("Configure(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting Configure()");
		_logger.LogDebug("<<< Exiting Configure(model={Model})", model);
        return View("~/Plugins/Misc.Brevo/Views/Configure.cshtml", model);
    }

    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    [HttpPost, ActionName("Configure")]
    [FormValueRequired("save")]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId);

        //set API key
        brevoSettings.ApiKey = model.ApiKey;
        await _settingService.SaveSettingAsync(brevoSettings, settings => settings.ApiKey, clearCache: false);
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    [HttpPost, ActionName("Configure")]
    [FormValueRequired("saveSync")]
    public async Task<IActionResult> SaveSynchronization(ConfigurationModel model)
    {
		_logger.LogDebug(">>> Entering SaveSynchronization(model={Model})", model);
        if (!ModelState.IsValid)
            return await Configure();

        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("SaveSynchronization(model={Model}): storeId → {StoreId}", model, storeId);
        var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId);
		_logger.LogDebug("SaveSynchronization(model={Model}): brevoSettings → {BrevoSettings}", model, brevoSettings);

        //create webhook for the unsubscribe event
        brevoSettings.UnsubscribeWebhookId = await _brevoEmailManager.GetUnsubscribeWebHookIdAsync();
        await _settingService.SaveSettingAsync(brevoSettings, settings => settings.UnsubscribeWebhookId, clearCache: false);

        //set list of contacts to synchronize
        brevoSettings.SubscriptionTypeMappings = model.NewsLetterSubscriptionTypes
            .ToDictionary(subscriptionType => subscriptionType.TypeId, subscriptionType => subscriptionType.ListId);
			_logger.LogDebug("SaveSynchronization(model={Model}): brevoSettings.SubscriptionTypeMappings → {Arg1}", model, brevoSettings.SubscriptionTypeMappings);
        await _settingService.SaveSettingAsync(brevoSettings, settings => settings.SubscriptionTypeMappings, clearCache: false);

        //now clear settings cache
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

		_logger.LogDebug(">>> Entering Synchronization(model={Model})", model);
		_logger.LogDebug("<<< Exiting Synchronization(model={Model})", model);
		_logger.LogDebug("<<< Exiting SaveSynchronization(model={Model})", model);
        return await Configure();
    }

    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    [HttpPost, ActionName("Configure")]
    [FormValueRequired("sync")]
    public async Task<IActionResult> Synchronization(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        //synchronize contacts of selected store
        var messages = await _brevoEmailManager.SynchronizeAsync();
        foreach (var message in messages)
        {
            _notificationService.Notification(message.Type, message.Message, false);
        }
        if (!messages.Any(message => message.Type == NotifyType.Error))
        {
            ViewData["synchronizationStart"] = true;
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.ImportProcess"));
        }

        return await Configure();
    }

    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<string> GetSynchronizationInfo()
    {
		_logger.LogDebug(">>> Entering GetSynchronizationInfo()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var res = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(BrevoDefaults.SyncKeyCache), () => string.Empty);
        await _staticCacheManager.RemoveAsync(BrevoDefaults.SyncKeyCache);
		_logger.LogDebug("GetSynchronizationInfo(): res → {Res}", res);

		_logger.LogInformation("GetSynchronizationInfo(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetSynchronizationInfo()");
        return res;
    }

    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    [HttpPost, ActionName("Configure")]
    [FormValueRequired("saveSMTP")]
    public async Task<IActionResult> ConfigureSMTP(ConfigurationModel model)
    {
		_logger.LogDebug(">>> Entering ConfigureSMTP(model={Model})", model);
        if (!ModelState.IsValid)
            return await Configure();

        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("ConfigureSMTP(model={Model}): storeId → {StoreId}", model, storeId);
        var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId);
		_logger.LogDebug("ConfigureSMTP(model={Model}): brevoSettings → {BrevoSettings}", model, brevoSettings);

        if (model.UseSmtp)
        {
            //set case invariant for true because tokens are used in uppercase format in Brevo's transactional emails
            _messageTemplatesSettings.CaseInvariantReplacement = true;
			_logger.LogDebug("ConfigureSMTP(model={Model}): _messageTemplatesSettings.CaseInvariantReplacement → {Arg1}", model, _messageTemplatesSettings.CaseInvariantReplacement);
            await _settingService.SaveSettingAsync(_messageTemplatesSettings, settings => settings.CaseInvariantReplacement, clearCache: false);

            //check whether SMTP enabled on account
            var (smtpIsEnabled, smtpErrors) = await _brevoEmailManager.SmtpIsEnabledAsync();
            if (smtpIsEnabled)
            {
                //get email account or create new one
                var (emailAccountId, emailAccountErrors) = await _brevoEmailManager.GetEmailAccountIdAsync(model.SenderId, model.SmtpKey);
                brevoSettings.EmailAccountId = emailAccountId;
				_logger.LogDebug("ConfigureSMTP(model={Model}): brevoSettings.EmailAccountId → {Arg1}", model, brevoSettings.EmailAccountId);
                await _settingService.SaveSettingAsync(brevoSettings, settings => settings.EmailAccountId, storeId, false);
                if (!string.IsNullOrEmpty(emailAccountErrors))
                    _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {emailAccountErrors}");
            }
            else
            {
                //need to activate SMTP account
                _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.ActivateSMTP"));
                model.UseSmtp = false;
				_logger.LogDebug("ConfigureSMTP(model={Model}): model.UseSmtp → {Arg1}", model, model.UseSmtp);
            }
            if (!string.IsNullOrEmpty(smtpErrors))
                _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {smtpErrors}");
        }

        //set whether to use SMTP 
        brevoSettings.UseSmtp = model.UseSmtp;
		_logger.LogDebug("ConfigureSMTP(model={Model}): brevoSettings.UseSmtp → {Arg1}", model, brevoSettings.UseSmtp);
        await _settingService.SaveSettingOverridablePerStoreAsync(brevoSettings, settings => settings.UseSmtp, model.UseSmtp_OverrideForStore, storeId, false);

        //set sender of transactional emails
        brevoSettings.SenderId = model.SenderId;
		_logger.LogDebug("ConfigureSMTP(model={Model}): brevoSettings.SenderId → {Arg1}", model, brevoSettings.SenderId);
        await _settingService.SaveSettingOverridablePerStoreAsync(brevoSettings, settings => settings.SenderId, model.SenderId_OverrideForStore, storeId, false);

        //set SMTP key
        brevoSettings.SmtpKey = model.SmtpKey;
		_logger.LogDebug("ConfigureSMTP(model={Model}): brevoSettings.SmtpKey → {Arg1}", model, brevoSettings.SmtpKey);
        await _settingService.SaveSettingAsync(brevoSettings, settings => settings.SmtpKey, clearCache: false);

        //now clear settings cache
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

		_logger.LogDebug("<<< Exiting ConfigureSMTP(model={Model})", model);
        return await Configure();
    }

    [HttpPost]
    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    public async Task<IActionResult> MessageList(BrevoMessageTemplateSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering MessageList(searchModel={SearchModel})", searchModel);
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var messageTemplates = (await _messageTemplateService.GetAllMessageTemplatesAsync(storeId)).ToPagedList(searchModel);
		_logger.LogDebug("MessageList(searchModel={SearchModel}): messageTemplates → {MessageTemplates}", searchModel, messageTemplates);
		_logger.LogDebug("MessageList(searchModel={SearchModel}): storeId → {StoreId}", searchModel, storeId);

        //prepare list model
        var model = await new BrevoMessageTemplateListModel().PrepareToGridAsync(searchModel, messageTemplates, () =>
        {
            return messageTemplates.SelectAwait(async messageTemplate =>
            {
                //standard template of message is edited in the admin area, Brevo template is edited in the Brevo account
                var templateId = await _genericAttributeService.GetAttributeAsync<int?>(messageTemplate, BrevoDefaults.TemplateIdAttribute);
				_logger.LogDebug("MessageList(searchModel={SearchModel}): templateId → {TemplateId}", searchModel, templateId);
                var stores = (await (await _storeService.GetAllStoresAsync())
                        .WhereAwait(async store => !messageTemplate.LimitedToStores
                                                   || (await _storeMappingService.GetStoresIdsWithAccessAsync(messageTemplate)).Contains(store.Id))
                        .AggregateAsync(string.Empty, (current, next) => $"{current}, {next.Name}"))
                    .Trim(',');

                return new BrevoMessageTemplateModel
                {
                    Id = messageTemplate.Id,
                    Name = messageTemplate.Name,
                    IsActive = messageTemplate.IsActive,
                    ListOfStores = stores,
                    UseBrevoTemplate = templateId.HasValue,
                    EditLink = templateId.HasValue
                        ? $"{string.Format(BrevoDefaults.EditMessageTemplateUrl, templateId.Value)}"
                        : Url.Action("Edit", "MessageTemplate", new { id = messageTemplate.Id, area = AreaNames.ADMIN })
                };
            });
        });

		_logger.LogDebug("<<< Exiting MessageList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    public async Task<IActionResult> MessageUpdate(BrevoMessageTemplateModel model)
    {
		_logger.LogDebug(">>> Entering MessageUpdate(model={Model})", model);
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors().ToString());

        var message = await _messageTemplateService.GetMessageTemplateByIdAsync(model.Id);
		_logger.LogDebug("MessageUpdate(model={Model}): message → {Message}", model, message);

        //Brevo message template
        if (model.UseBrevoTemplate)
        {
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
			_logger.LogDebug("MessageUpdate(model={Model}): storeId → {StoreId}", model, storeId);
            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId);
			_logger.LogDebug("MessageUpdate(model={Model}): brevoSettings → {BrevoSettings}", model, brevoSettings);

            //get template or create new one
            var currentTemplateId = await _genericAttributeService.GetAttributeAsync<int?>(message, BrevoDefaults.TemplateIdAttribute);
			_logger.LogDebug("MessageUpdate(model={Model}): currentTemplateId → {CurrentTemplateId}", model, currentTemplateId);
            var templateId = await _brevoEmailManager.GetTemplateIdAsync(currentTemplateId, message,
                await _emailAccountService.GetEmailAccountByIdAsync(brevoSettings.EmailAccountId));
            await _genericAttributeService.SaveAttributeAsync(message, BrevoDefaults.TemplateIdAttribute, templateId);
            model.EditLink = $"{string.Format(BrevoDefaults.EditMessageTemplateUrl, templateId)}";
        }
        else
        {
            //standard message template
            await _genericAttributeService.SaveAttributeAsync<int?>(message, BrevoDefaults.TemplateIdAttribute, null);
            model.EditLink = Url.Action("Edit", "MessageTemplate", new { id = model.Id, area = AreaNames.ADMIN });
        }

        //update message template
        if (model.IsActive == message.IsActive)
            return new NullJsonResult();

        message.IsActive = model.IsActive;
		_logger.LogDebug("MessageUpdate(model={Model}): message.IsActive → {Arg1}", model, message.IsActive);
        await _messageTemplateService.UpdateMessageTemplateAsync(message);

		_logger.LogDebug("<<< Exiting MessageUpdate(model={Model})", model);
        return new NullJsonResult();
    }

    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    [HttpPost, ActionName("Configure")]
    [FormValueRequired("saveSMS")]
    public async Task<IActionResult> ConfigureSMS(ConfigurationModel model)
    {
		_logger.LogDebug(">>> Entering ConfigureSMS(model={Model})", model);
        if (!ModelState.IsValid)
            return await Configure();

        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("ConfigureSMS(model={Model}): storeId → {StoreId}", model, storeId);
        var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId);
		_logger.LogDebug("ConfigureSMS(model={Model}): brevoSettings → {BrevoSettings}", model, brevoSettings);

        brevoSettings.UseSmsNotifications = model.UseSmsNotifications;
		_logger.LogDebug("ConfigureSMS(model={Model}): brevoSettings.UseSmsNotifications → {Arg1}", model, brevoSettings.UseSmsNotifications);
        await _settingService.SaveSettingOverridablePerStoreAsync(brevoSettings, settings => settings.UseSmsNotifications, model.UseSmsNotifications_OverrideForStore, storeId, false);
        brevoSettings.SmsSenderName = model.SmsSenderName;
		_logger.LogDebug("ConfigureSMS(model={Model}): brevoSettings.SmsSenderName → {Arg1}", model, brevoSettings.SmsSenderName);
        await _settingService.SaveSettingOverridablePerStoreAsync(brevoSettings, settings => settings.SmsSenderName, model.SmsSenderName_OverrideForStore, storeId, false);
        brevoSettings.StoreOwnerPhoneNumber = model.StoreOwnerPhoneNumber;
		_logger.LogDebug("ConfigureSMS(model={Model}): brevoSettings.StoreOwnerPhoneNumber → {Arg1}", model, brevoSettings.StoreOwnerPhoneNumber);
        await _settingService.SaveSettingAsync(brevoSettings, settings => settings.StoreOwnerPhoneNumber, clearCache: false);

        //now clear settings cache
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

		_logger.LogDebug("<<< Exiting ConfigureSMS(model={Model})", model);
        return await Configure();
    }

    [HttpPost]
    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    public async Task<IActionResult> SMSList(SmsSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering SMSList(searchModel={SearchModel})", searchModel);
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();

		_logger.LogDebug("SMSList(searchModel={SearchModel}): storeId → {StoreId}", searchModel, storeId);
        //get message templates which are sending in SMS
        var allMessageTemplates = await _messageTemplateService.GetAllMessageTemplatesAsync(storeId);
		_logger.LogDebug("SMSList(searchModel={SearchModel}): allMessageTemplates → {AllMessageTemplates}", searchModel, allMessageTemplates);
        var messageTemplates = await allMessageTemplates

            .WhereAwait(async messageTemplate => await _genericAttributeService.GetAttributeAsync<bool>(messageTemplate, BrevoDefaults.UseSmsAttribute))
            .ToPagedListAsync(searchModel);

        //prepare list model
        var model = await new SmsListModel().PrepareToGridAsync(searchModel, messageTemplates, () =>
        {
            return messageTemplates.SelectAwait(async messageTemplate =>
            {
                var phoneTypeID = await _genericAttributeService.GetAttributeAsync<int>(messageTemplate, BrevoDefaults.PhoneTypeAttribute);
				_logger.LogDebug("SMSList(searchModel={SearchModel}): phoneTypeID → {PhoneTypeID}", searchModel, phoneTypeID);

                var smsModel = new SmsModel
                {
                    Id = messageTemplate.Id,
                    MessageId = messageTemplate.Id,
                    Name = messageTemplate.Name,
                    PhoneTypeId = phoneTypeID,

                    Text = await _genericAttributeService.GetAttributeAsync<string>(messageTemplate, BrevoDefaults.SmsTextAttribute)
                };

                if (storeId == 0)
                {
                    if (storeId == 0 && messageTemplate.LimitedToStores)
                    {
                        var storeIds = await _storeMappingService.GetStoresIdsWithAccessAsync(messageTemplate);
						_logger.LogDebug("SMSList(searchModel={SearchModel}): storeIds → {StoreIds}", searchModel, storeIds);
                        var storeNames = (await _storeService.GetAllStoresAsync()).Where(store => storeIds.Contains(store.Id)).Select(store => store.Name);
						_logger.LogDebug("SMSList(searchModel={SearchModel}): storeNames → {StoreNames}", searchModel, storeNames);

                        smsModel.Name = $"{smsModel.Name} ({string.Join(',', storeNames)})";
                    }
                }

                //choose phone number to send SMS
                //currently supported: "my phone" (filled on the configuration page), customer phone, phone of the billing address
                switch (phoneTypeID)
                {
                    case 0:
                        smsModel.PhoneType = await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.MyPhone");
                        break;
                    case 1:
                        smsModel.PhoneType = await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.CustomerPhone");
                        break;
                    case 2:
                        smsModel.PhoneType = await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.BillingAddressPhone");
                        break;
                    default:
                        break;
                }

                return smsModel;
            });
        });

		_logger.LogDebug("<<< Exiting SMSList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    public async Task<IActionResult> SMSAdd(SmsModel model)
    {
		_logger.LogDebug(">>> Entering SMSAdd(model={Model})", model);
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var message = await _messageTemplateService.GetMessageTemplateByIdAsync(model.MessageId);
		_logger.LogDebug("SMSAdd(model={Model}): message → {Message}", model, message);
        if (message != null)
        {
            await _genericAttributeService.SaveAttributeAsync(message, BrevoDefaults.UseSmsAttribute, true);
            await _genericAttributeService.SaveAttributeAsync(message, BrevoDefaults.SmsTextAttribute, model.Text);
            await _genericAttributeService.SaveAttributeAsync(message, BrevoDefaults.PhoneTypeAttribute, model.PhoneTypeId);
        }

		_logger.LogDebug("<<< Exiting SMSAdd(model={Model})", model);
        return Json(new { Result = true });
    }

    [HttpPost]
    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    public async Task<IActionResult> SMSDelete(SmsModel model)
    {
		_logger.LogDebug(">>> Entering SMSDelete(model={Model})", model);
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //delete generic attributes
        var message = await _messageTemplateService.GetMessageTemplateByIdAsync(model.Id);
		_logger.LogDebug("SMSDelete(model={Model}): message → {Message}", model, message);
        if (message != null)
        {
            await _genericAttributeService.SaveAttributeAsync<bool?>(message, BrevoDefaults.UseSmsAttribute, null);
            await _genericAttributeService.SaveAttributeAsync<string>(message, BrevoDefaults.SmsTextAttribute, null);
            await _genericAttributeService.SaveAttributeAsync<int?>(message, BrevoDefaults.PhoneTypeAttribute, null);
        }

		_logger.LogDebug("<<< Exiting SMSDelete(model={Model})", model);
        return new NullJsonResult();
    }

    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    [HttpPost, ActionName("Configure")]
    [FormValueRequired("submitCampaign")]
    public async Task<IActionResult> SubmitCampaign(ConfigurationModel model)
    {
		_logger.LogDebug(">>> Entering SubmitCampaign(model={Model})", model);
        if (!ModelState.IsValid)
            return await Configure();

        var campaignErrors = await _brevoEmailManager.SendSMSCampaignAsync(model.CampaignListId, model.CampaignSenderName, model.CampaignText);
		_logger.LogDebug("SubmitCampaign(model={Model}): campaignErrors → {CampaignErrors}", model, campaignErrors);
        if (!string.IsNullOrEmpty(campaignErrors))
            _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {campaignErrors}");
        else
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Brevo.SMS.Campaigns.Sent"));

		_logger.LogDebug("<<< Exiting SubmitCampaign(model={Model})", model);
        return await Configure();
    }

    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    [HttpPost, ActionName("Configure")]
    [FormValueRequired("saveMA")]
    public async Task<IActionResult> ConfigureMA(ConfigurationModel model)
    {
		_logger.LogDebug(">>> Entering ConfigureMA(model={Model})", model);
        if (!ModelState.IsValid)
            return await Configure();

        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("ConfigureMA(model={Model}): storeId → {StoreId}", model, storeId);
        var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId);
		_logger.LogDebug("ConfigureMA(model={Model}): brevoSettings → {BrevoSettings}", model, brevoSettings);

        brevoSettings.UseMarketingAutomation = model.UseMarketingAutomation;
		_logger.LogDebug("ConfigureMA(model={Model}): brevoSettings.UseMarketingAutomation → {Arg1}", model, brevoSettings.UseMarketingAutomation);
        await _settingService.SaveSettingOverridablePerStoreAsync(brevoSettings, settings => settings.UseMarketingAutomation, model.UseMarketingAutomation_OverrideForStore, storeId, false);

        var (accountInfo, marketingAutomationEnabled, maKey, accountErrors) = await _brevoEmailManager.GetAccountInfoAsync();
        brevoSettings.MarketingAutomationKey = maKey;
		_logger.LogDebug("ConfigureMA(model={Model}): brevoSettings.MarketingAutomationKey → {Arg1}", model, brevoSettings.MarketingAutomationKey);
        if (!string.IsNullOrEmpty(accountErrors))
            _notificationService.ErrorNotification($"{BrevoDefaults.NotificationMessage} {accountErrors}");

        await _settingService.SaveSettingAsync(brevoSettings, settings => settings.MarketingAutomationKey, clearCache: false);
        brevoSettings.TrackingScript = model.TrackingScript;
		_logger.LogDebug("ConfigureMA(model={Model}): brevoSettings.TrackingScript → {Arg1}", model, brevoSettings.TrackingScript);
        await _settingService.SaveSettingAsync(brevoSettings, settings => settings.TrackingScript, clearCache: false);

        //now clear settings cache
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

		_logger.LogDebug("<<< Exiting ConfigureMA(model={Model})", model);
        return await Configure();
    }

    public async Task<IActionResult> ImportContacts(BaseNopModel model, IFormCollection form)
    {
		_logger.LogDebug(">>> Entering ImportContacts(model={Model},form={Form})", model, form);
        try
        {
            //logging info
            var logInfo = string.Format("Brevo synchronization: New emails {1},{0} Existing emails {2},{0} Invalid emails {3},{0} Duplicates emails {4}{0}",
                Environment.NewLine, form["new_emails"], form["emails_exists"], form["invalid_email"], form["duplicates_email"]);
            await _logger.InformationAsync(logInfo);

            //display info on configuration page in case of the manually synchronization
            await _staticCacheManager.SetAsync(_staticCacheManager.PrepareKeyForDefaultCache(BrevoDefaults.SyncKeyCache), logInfo);
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync(ex.Message, ex);
            await _staticCacheManager.SetAsync(_staticCacheManager.PrepareKeyForDefaultCache(BrevoDefaults.SyncKeyCache), ex.Message);
        }

		_logger.LogDebug("<<< Exiting ImportContacts(model={Model},form={Form})", model, form);
        return Ok();
    }

    #endregion
}