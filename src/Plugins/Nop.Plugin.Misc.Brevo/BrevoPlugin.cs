using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Plugin.Misc.Brevo.Components;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Stores;
using Nop.Web.Framework.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Brevo;

/// <summary>
/// Represents the Brevo plugin
/// </summary>
public class BrevoPlugin : BasePlugin, IMiscPlugin, IWidgetPlugin
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IEmailAccountService _emailAccountService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IMessageTemplateService _messageTemplateService;
    protected readonly IScheduleTaskService _scheduleTaskService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreService _storeService;
    protected readonly IWebHelper _webHelper;
    protected readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public BrevoPlugin(IEmailAccountService emailAccountService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        IMessageTemplateService messageTemplateService,
        IScheduleTaskService scheduleTaskService,
        ISettingService settingService,
        IStoreService storeService,
        IWebHelper webHelper,
        WidgetSettings widgetSettings)
    {
		_logger.LogDebug(">>> Entering BrevoPlugin(emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},scheduleTaskService={ScheduleTaskService},settingService={SettingService},storeService={StoreService},webHelper={WebHelper},widgetSettings={WidgetSettings})", emailAccountService, genericAttributeService, localizationService, messageTemplateService, scheduleTaskService, settingService, storeService, webHelper, widgetSettings);
        _emailAccountService = emailAccountService;
		_logger.LogDebug("BrevoPlugin(emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},scheduleTaskService={ScheduleTaskService},settingService={SettingService},storeService={StoreService},webHelper={WebHelper},widgetSettings={WidgetSettings}): _emailAccountService → {_emailAccountService}", emailAccountService, genericAttributeService, localizationService, messageTemplateService, scheduleTaskService, settingService, storeService, webHelper, widgetSettings, _emailAccountService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("BrevoPlugin(emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},scheduleTaskService={ScheduleTaskService},settingService={SettingService},storeService={StoreService},webHelper={WebHelper},widgetSettings={WidgetSettings}): _genericAttributeService → {_genericAttributeService}", emailAccountService, genericAttributeService, localizationService, messageTemplateService, scheduleTaskService, settingService, storeService, webHelper, widgetSettings, _genericAttributeService);
        _localizationService = localizationService;
		_logger.LogDebug("BrevoPlugin(emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},scheduleTaskService={ScheduleTaskService},settingService={SettingService},storeService={StoreService},webHelper={WebHelper},widgetSettings={WidgetSettings}): _localizationService → {_localizationService}", emailAccountService, genericAttributeService, localizationService, messageTemplateService, scheduleTaskService, settingService, storeService, webHelper, widgetSettings, _localizationService);
        _messageTemplateService = messageTemplateService;
		_logger.LogDebug("BrevoPlugin(emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},scheduleTaskService={ScheduleTaskService},settingService={SettingService},storeService={StoreService},webHelper={WebHelper},widgetSettings={WidgetSettings}): _messageTemplateService → {_messageTemplateService}", emailAccountService, genericAttributeService, localizationService, messageTemplateService, scheduleTaskService, settingService, storeService, webHelper, widgetSettings, _messageTemplateService);
        _scheduleTaskService = scheduleTaskService;
		_logger.LogDebug("BrevoPlugin(emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},scheduleTaskService={ScheduleTaskService},settingService={SettingService},storeService={StoreService},webHelper={WebHelper},widgetSettings={WidgetSettings}): _scheduleTaskService → {_scheduleTaskService}", emailAccountService, genericAttributeService, localizationService, messageTemplateService, scheduleTaskService, settingService, storeService, webHelper, widgetSettings, _scheduleTaskService);
        _settingService = settingService;
		_logger.LogDebug("BrevoPlugin(emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},scheduleTaskService={ScheduleTaskService},settingService={SettingService},storeService={StoreService},webHelper={WebHelper},widgetSettings={WidgetSettings}): _settingService → {_settingService}", emailAccountService, genericAttributeService, localizationService, messageTemplateService, scheduleTaskService, settingService, storeService, webHelper, widgetSettings, _settingService);
        _storeService = storeService;
		_logger.LogDebug("BrevoPlugin(emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},scheduleTaskService={ScheduleTaskService},settingService={SettingService},storeService={StoreService},webHelper={WebHelper},widgetSettings={WidgetSettings}): _storeService → {_storeService}", emailAccountService, genericAttributeService, localizationService, messageTemplateService, scheduleTaskService, settingService, storeService, webHelper, widgetSettings, _storeService);
        _webHelper = webHelper;
		_logger.LogDebug("BrevoPlugin(emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},scheduleTaskService={ScheduleTaskService},settingService={SettingService},storeService={StoreService},webHelper={WebHelper},widgetSettings={WidgetSettings}): _webHelper → {_webHelper}", emailAccountService, genericAttributeService, localizationService, messageTemplateService, scheduleTaskService, settingService, storeService, webHelper, widgetSettings, _webHelper);
        _widgetSettings = widgetSettings;
		_logger.LogDebug("<<< Exiting BrevoPlugin(emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},scheduleTaskService={ScheduleTaskService},settingService={SettingService},storeService={StoreService},webHelper={WebHelper},widgetSettings={WidgetSettings})", emailAccountService, genericAttributeService, localizationService, messageTemplateService, scheduleTaskService, settingService, storeService, webHelper, widgetSettings);
		_logger.LogDebug("BrevoPlugin(emailAccountService={EmailAccountService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},messageTemplateService={MessageTemplateService},scheduleTaskService={ScheduleTaskService},settingService={SettingService},storeService={StoreService},webHelper={WebHelper},widgetSettings={WidgetSettings}): _widgetSettings → {_widgetSettings}", emailAccountService, genericAttributeService, localizationService, messageTemplateService, scheduleTaskService, settingService, storeService, webHelper, widgetSettings, _widgetSettings);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets widget zones where this widget should be rendered
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the widget zones
    /// </returns>
    public Task<IList<string>> GetWidgetZonesAsync()
    {
		_logger.LogDebug(">>> Entering GetWidgetZonesAsync()");
		_logger.LogDebug("<<< Exiting GetWidgetZonesAsync()");
        return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.HeadHtmlTag });
    }

    /// <summary>
    /// Gets a type of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component type</returns>
    public Type GetWidgetViewComponent(string widgetZone)
    {
		_logger.LogDebug(">>> Entering GetWidgetViewComponent(widgetZone={WidgetZone})", widgetZone);
		_logger.LogDebug("<<< Exiting GetWidgetViewComponent(widgetZone={WidgetZone})", widgetZone);
        return typeof(WidgetsBrevoViewComponent);
    }

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
		_logger.LogDebug(">>> Entering GetConfigurationPageUrl()");
		_logger.LogDebug("<<< Exiting GetConfigurationPageUrl()");
        return $"{_webHelper.GetStoreLocation()}Admin/Brevo/Configure";
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
		_logger.LogDebug(">>> Entering InstallAsync()");
        //settings
        await _settingService.SaveSettingAsync(new BrevoSettings
        {
            //prepopulate a tracking script
            TrackingScript = $@"<!-- Brevo tracking code -->
                <script>
                    (function() {{
                        window.sib = {{ equeue: [], client_key: '{BrevoDefaults.TrackingScriptId}' }};
                        window.sib.email_id = '{BrevoDefaults.TrackingScriptCustomerEmail}';
                        window.sendinblue = {{}}; for (var j = ['track', 'identify', 'trackLink', 'page'], i = 0; i < j.length; i++) {{ (function(k) {{ window.sendinblue[k] = function() {{ var arg = Array.prototype.slice.call(arguments); (window.sib[k] || function() {{ var t = {{}}; t[k] = arg; window.sib.equeue.push(t);}})(arg[0], arg[1], arg[2]);}};}})(j[i]);}}var n = document.createElement('script'),i = document.getElementsByTagName('script')[0]; n.type = 'text/javascript', n.id = 'sendinblue-js', n.async = !0, n.src = 'https://sibautomation.com/sa.js?key=' + window.sib.client_key, i.parentNode.insertBefore(n, i), window.sendinblue.page();
                    }})();
                </script>"
        });

        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(BrevoDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(BrevoDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        //install synchronization task
        if (await _scheduleTaskService.GetTaskByTypeAsync(BrevoDefaults.SynchronizationTask) == null)
        {
            await _scheduleTaskService.InsertTaskAsync(new ScheduleTask
            {
                Enabled = true,
                LastEnabledUtc = DateTime.UtcNow,
                Seconds = BrevoDefaults.DefaultSynchronizationPeriod * 60 * 60,
                Name = BrevoDefaults.SynchronizationTaskName,
                Type = BrevoDefaults.SynchronizationTask,
            });
        }

        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Misc.Brevo.AccountInfo"] = "Account info",
            ["Plugins.Misc.Brevo.AccountInfo.Hint"] = "Display account information.",
            ["Plugins.Misc.Brevo.ActivateSMTP"] = "On your Brevo account, the SMTP has not been enabled yet. To request its activation, simply send an email to our support team at contact@brevo.com and mention that you will be using the SMTP with the nopCommerce plugin.",
            ["Plugins.Misc.Brevo.AddNewSMSNotification"] = "Add new SMS notification",
            ["Plugins.Misc.Brevo.BillingAddressPhone"] = "Billing address phone number",
            ["Plugins.Misc.Brevo.CustomerPhone"] = "Customer phone number",
            ["Plugins.Misc.Brevo.EditTemplate"] = "Edit template",
            ["Plugins.Misc.Brevo.Fields.AllowedTokens"] = "Allowed message variables",
            ["Plugins.Misc.Brevo.Fields.AllowedTokens.Hint"] = "This is a list of the message variables you can use in your SMS.",
            ["Plugins.Misc.Brevo.Fields.ApiKey"] = "API v3 key",
            ["Plugins.Misc.Brevo.Fields.ApiKey.Hint"] = "Paste your Brevo account API v3 key.",
            ["Plugins.Misc.Brevo.Fields.CampaignList"] = "List",
            ["Plugins.Misc.Brevo.Fields.CampaignList.Hint"] = "Choose list of contacts to send SMS campaign.",
            ["Plugins.Misc.Brevo.Fields.CampaignSenderName"] = "Send SMS campaign from",
            ["Plugins.Misc.Brevo.Fields.CampaignSenderName.Hint"] = "Input the name of the sender. The number of characters is limited to 11 (alphanumeric format).",
            ["Plugins.Misc.Brevo.Fields.CampaignText"] = "Text",
            ["Plugins.Misc.Brevo.Fields.CampaignText.Hint"] = "Specify SMS campaign content. The number of characters is limited to 160 for one message.",
            ["Plugins.Misc.Brevo.Fields.List"] = "List",
            ["Plugins.Misc.Brevo.Fields.List.Hint"] = "Select the Brevo list where your nopCommerce newsletter subscribers will be added.",
            ["Plugins.Misc.Brevo.Fields.MaKey"] = "Tracker ID",
            ["Plugins.Misc.Brevo.Fields.MaKey.Hint"] = "Input your Tracker ID.",
            ["Plugins.Misc.Brevo.Fields.Sender"] = "Send emails from",
            ["Plugins.Misc.Brevo.Fields.Sender.Hint"] = "Choose sender of your transactional emails.",
            ["Plugins.Misc.Brevo.Fields.SmsSenderName"] = "Send SMS from",
            ["Plugins.Misc.Brevo.Fields.SmsSenderName.Hint"] = "Input the name of the sender. The number of characters is limited to 11 (alphanumeric format).",
            ["Plugins.Misc.Brevo.Fields.SmtpKey"] = "SMTP key",
            ["Plugins.Misc.Brevo.Fields.SmtpKey.Hint"] = "Specify SMTP key (password).",
            ["Plugins.Misc.Brevo.Fields.StoreOwnerPhoneNumber"] = "Store owner phone number",
            ["Plugins.Misc.Brevo.Fields.StoreOwnerPhoneNumber.Hint"] = "Input store owner phone number for SMS notifications.",
            ["Plugins.Misc.Brevo.Fields.TrackingScript"] = "Tracking script",
            ["Plugins.Misc.Brevo.Fields.TrackingScript.Hint"] = $"Paste the tracking script generated by Brevo here. {BrevoDefaults.TrackingScriptId} and {BrevoDefaults.TrackingScriptCustomerEmail} will be dynamically replaced.",
            ["Plugins.Misc.Brevo.Fields.UseMarketingAutomation"] = "Use Marketing Automation",
            ["Plugins.Misc.Brevo.Fields.UseMarketingAutomation.Hint"] = "Check for enable Brevo Automation.",
            ["Plugins.Misc.Brevo.Fields.UseSmsNotifications"] = "Use SMS notifications",
            ["Plugins.Misc.Brevo.Fields.UseSmsNotifications.Hint"] = "Check for sending transactional SMS.",
            ["Plugins.Misc.Brevo.Fields.UseSmtp"] = "Use Brevo SMTP",
            ["Plugins.Misc.Brevo.Fields.UseSmtp.Hint"] = "Check for using Brevo SMTP for sending transactional emails.",
            ["Plugins.Misc.Brevo.General"] = "General",
            ["Plugins.Misc.Brevo.ImportProcess"] = "Your import is in process",
            ["Plugins.Misc.Brevo.ManualSync"] = "Manual synchronization",
            ["Plugins.Misc.Brevo.SyncNow"] = "Sync now",
            ["Plugins.Misc.Brevo.MarketingAutomation"] = "Marketing Automation",
            ["Plugins.Misc.Brevo.MyPhone"] = "Store owner phone number",
            ["Plugins.Misc.Brevo.PhoneType"] = "Type of phone number",
            ["Plugins.Misc.Brevo.PhoneType.Hint"] = "Specify the type of phone number to send SMS.",
            ["Plugins.Misc.Brevo.SMS"] = "SMS",
            ["Plugins.Misc.Brevo.SMS.Campaigns"] = "SMS campaigns",
            ["Plugins.Misc.Brevo.SMS.Campaigns.Sent"] = "Campaign successfully sent",
            ["Plugins.Misc.Brevo.SMS.Campaigns.Submit"] = "Send campaign",
            ["Plugins.Misc.Brevo.SMSText"] = "Text",
            ["Plugins.Misc.Brevo.SMSText.Hint"] = "Enter SMS text to send.",
            ["Plugins.Misc.Brevo.Synchronization"] = "Contacts",
            ["Plugins.Misc.Brevo.Transactional"] = "Transactional emails",
            ["Plugins.Misc.Brevo.UseBrevoTemplate"] = "Brevo template"
        });

        await base.InstallAsync();
		_logger.LogDebug("<<< Exiting InstallAsync()");
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
		_logger.LogDebug(">>> Entering UninstallAsync()");
        //smtp accounts
        foreach (var store in await _storeService.GetAllStoresAsync())
        {
            var key = $"{nameof(BrevoSettings)}.{nameof(BrevoSettings.EmailAccountId)}";
			_logger.LogWarning("UninstallAsync(): key → {Key}", key);
            var emailAccountId = await _settingService.GetSettingByKeyAsync<int>(key, storeId: store.Id, loadSharedValueIfNotFound: true);
			_logger.LogDebug("UninstallAsync(): emailAccountId → {EmailAccountId}", emailAccountId);
            var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(emailAccountId);
			_logger.LogDebug("UninstallAsync(): emailAccount → {EmailAccount}", emailAccount);
            if (emailAccount != null)
                await _emailAccountService.DeleteEmailAccountAsync(emailAccount);
        }

        //settings
        if (_widgetSettings.ActiveWidgetSystemNames.Contains(BrevoDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(BrevoDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }
        await _settingService.DeleteSettingAsync<BrevoSettings>();

        //generic attributes
        foreach (var store in await _storeService.GetAllStoresAsync())
        {
            var messageTemplates = await _messageTemplateService.GetAllMessageTemplatesAsync(store.Id);
			_logger.LogDebug("UninstallAsync(): messageTemplates → {MessageTemplates}", messageTemplates);
            foreach (var messageTemplate in messageTemplates)
            {
                await _genericAttributeService.SaveAttributeAsync<int?>(messageTemplate, BrevoDefaults.TemplateIdAttribute, null);
            }
        }

        //schedule task
        var task = await _scheduleTaskService.GetTaskByTypeAsync(BrevoDefaults.SynchronizationTask);
		_logger.LogDebug("UninstallAsync(): task → {Task}", task);
        if (task != null)
            await _scheduleTaskService.DeleteTaskAsync(task);

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.Brevo");

        await base.UninstallAsync();
		_logger.LogDebug("<<< Exiting UninstallAsync()");
    }

    #endregion

    /// <summary>
    /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
    /// </summary>
    public bool HideInWidgetList => true;
}