using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.GoogleAnalytics.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class WidgetsGoogleAnalyticsController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public WidgetsGoogleAnalyticsController(
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        IStoreContext storeContext)
    {
		_logger.LogError(">>> Entering WidgetsGoogleAnalyticsController(localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext})", localizationService, notificationService, permissionService, settingService, storeContext);
        _localizationService = localizationService;
        _notificationService = notificationService;
		_logger.LogDebug("WidgetsGoogleAnalyticsController(localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext}): _notificationService → {_notificationService}", localizationService, notificationService, permissionService, settingService, storeContext, _notificationService);
		_logger.LogDebug("WidgetsGoogleAnalyticsController(localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext}): _localizationService → {_localizationService}", localizationService, notificationService, permissionService, settingService, storeContext, _localizationService);
    _logger.LogWarning("<<< Exiting WidgetsGoogleAnalyticsController(localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext})", localizationService, notificationService, permissionService, settingService, storeContext);
        _permissionService = permissionService;
		_logger.LogDebug("WidgetsGoogleAnalyticsController(localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext}): _permissionService → {_permissionService}", localizationService, notificationService, permissionService, settingService, storeContext, _permissionService);
        _settingService = settingService;
		_logger.LogDebug("WidgetsGoogleAnalyticsController(localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext}): _settingService → {_settingService}", localizationService, notificationService, permissionService, settingService, storeContext, _settingService);
        _storeContext = storeContext;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public async Task<IActionResult> Configure()
    {
		_logger.LogError(">>> Entering Configure()");
        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogError("Configure(): storeScope → {StoreScope}", storeScope);
        var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(storeScope);
		_logger.LogError("Configure(): googleAnalyticsSettings → {GoogleAnalyticsSettings}", googleAnalyticsSettings);

        var model = new ConfigurationModel
        {
            GoogleId = googleAnalyticsSettings.GoogleId,
            ApiSecret = googleAnalyticsSettings.ApiSecret,
            TrackingScript = googleAnalyticsSettings.TrackingScript,
            EnableEcommerce = googleAnalyticsSettings.EnableEcommerce,
            UseSandbox = googleAnalyticsSettings.UseSandbox,
            IncludingTax = googleAnalyticsSettings.IncludingTax,
            IncludeCustomerId = googleAnalyticsSettings.IncludeCustomerId,
            ActiveStoreScopeConfiguration = storeScope
        };

        if (storeScope > 0)
        {
            model.GoogleId_OverrideForStore = await _settingService.SettingExistsAsync(googleAnalyticsSettings, x => x.GoogleId, storeScope);
            model.ApiSecret_OverrideForStore = await _settingService.SettingExistsAsync(googleAnalyticsSettings, x => x.ApiSecret, storeScope);
            model.TrackingScript_OverrideForStore = await _settingService.SettingExistsAsync(googleAnalyticsSettings, x => x.TrackingScript, storeScope);
            model.EnableEcommerce_OverrideForStore = await _settingService.SettingExistsAsync(googleAnalyticsSettings, x => x.EnableEcommerce, storeScope);
            model.UseSandbox_OverrideForStore = await _settingService.SettingExistsAsync(googleAnalyticsSettings, x => x.UseSandbox, storeScope);
            model.IncludingTax_OverrideForStore = await _settingService.SettingExistsAsync(googleAnalyticsSettings, x => x.IncludingTax, storeScope);
            model.IncludeCustomerId_OverrideForStore = await _settingService.SettingExistsAsync(googleAnalyticsSettings, x => x.IncludeCustomerId, storeScope);
        }

		_logger.LogWarning("<<< Exiting Configure()");
		_logger.LogWarning("<<< Exiting Configure(model={Model})", model);
        return View("~/Plugins/Widgets.GoogleAnalytics/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(storeScope);

        googleAnalyticsSettings.GoogleId = model.GoogleId;
        googleAnalyticsSettings.ApiSecret = model.ApiSecret;
        googleAnalyticsSettings.TrackingScript = model.TrackingScript;
        googleAnalyticsSettings.EnableEcommerce = model.EnableEcommerce;
        googleAnalyticsSettings.UseSandbox = model.UseSandbox;
        googleAnalyticsSettings.IncludingTax = model.IncludingTax;
        googleAnalyticsSettings.IncludeCustomerId = model.IncludeCustomerId;

        /* We do not clear cache after each setting update.
         * This behavior can increase performance because cached settings will not be cleared 
         * and loaded from database after each update */
        await _settingService.SaveSettingOverridablePerStoreAsync(googleAnalyticsSettings, x => x.GoogleId, model.GoogleId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(googleAnalyticsSettings, x => x.ApiSecret, model.ApiSecret_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(googleAnalyticsSettings, x => x.TrackingScript, model.TrackingScript_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(googleAnalyticsSettings, x => x.EnableEcommerce, model.EnableEcommerce_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(googleAnalyticsSettings, x => x.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(googleAnalyticsSettings, x => x.IncludingTax, model.IncludingTax_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(googleAnalyticsSettings, x => x.IncludeCustomerId, model.IncludeCustomerId_OverrideForStore, storeScope, false);

        //now clear settings cache
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    #endregion
}