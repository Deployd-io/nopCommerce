using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.CheckMoneyOrder.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.CheckMoneyOrder.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class PaymentCheckMoneyOrderController : BasePaymentController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public PaymentCheckMoneyOrderController(ILanguageService languageService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        IStoreContext storeContext)
    {
		_logger.LogDebug(">>> Entering PaymentCheckMoneyOrderController(languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext})", languageService, localizationService, notificationService, permissionService, settingService, storeContext);
        _languageService = languageService;
		_logger.LogDebug("PaymentCheckMoneyOrderController(languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext}): _languageService → {_languageService}", languageService, localizationService, notificationService, permissionService, settingService, storeContext, _languageService);
        _localizationService = localizationService;
		_logger.LogDebug("PaymentCheckMoneyOrderController(languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext}): _localizationService → {_localizationService}", languageService, localizationService, notificationService, permissionService, settingService, storeContext, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("PaymentCheckMoneyOrderController(languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext}): _notificationService → {_notificationService}", languageService, localizationService, notificationService, permissionService, settingService, storeContext, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("PaymentCheckMoneyOrderController(languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext}): _permissionService → {_permissionService}", languageService, localizationService, notificationService, permissionService, settingService, storeContext, _permissionService);
        _settingService = settingService;
		_logger.LogDebug("PaymentCheckMoneyOrderController(languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext}): _settingService → {_settingService}", languageService, localizationService, notificationService, permissionService, settingService, storeContext, _settingService);
        _storeContext = storeContext;
		_logger.LogDebug("<<< Exiting PaymentCheckMoneyOrderController(languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext})", languageService, localizationService, notificationService, permissionService, settingService, storeContext);
		_logger.LogDebug("PaymentCheckMoneyOrderController(languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext}): _storeContext → {_storeContext}", languageService, localizationService, notificationService, permissionService, settingService, storeContext, _storeContext);
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Configure()
    {
		_logger.LogDebug(">>> Entering Configure()");
        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("Configure(): storeScope → {StoreScope}", storeScope);
        var checkMoneyOrderPaymentSettings = await _settingService.LoadSettingAsync<CheckMoneyOrderPaymentSettings>(storeScope);
		_logger.LogDebug("Configure(): checkMoneyOrderPaymentSettings → {CheckMoneyOrderPaymentSettings}", checkMoneyOrderPaymentSettings);

        var model = new ConfigurationModel
        {
            DescriptionText = checkMoneyOrderPaymentSettings.DescriptionText
        };

        //locales
        await AddLocalesAsync(_languageService, model.Locales, async (locale, languageId) =>
        {
            locale.DescriptionText = await _localizationService
				_logger.LogDebug("Configure(): locale.DescriptionText → {Arg0}", locale.DescriptionText);
                .GetLocalizedSettingAsync(checkMoneyOrderPaymentSettings, x => x.DescriptionText, languageId, 0, false, false);
        });
        model.AdditionalFee = checkMoneyOrderPaymentSettings.AdditionalFee;
		_logger.LogDebug("Configure(): model.AdditionalFee → {Arg0}", model.AdditionalFee);
		_logger.LogDebug("Configure(): model.AdditionalFeePercentage → {Arg0}", model.AdditionalFeePercentage);
		_logger.LogDebug("Configure(): model.ShippableProductRequired → {Arg0}", model.ShippableProductRequired);
        model.AdditionalFeePercentage = checkMoneyOrderPaymentSettings.AdditionalFeePercentage;
        model.ShippableProductRequired = checkMoneyOrderPaymentSettings.ShippableProductRequired;

        model.ActiveStoreScopeConfiguration = storeScope;
		_logger.LogDebug("Configure(): model.ActiveStoreScopeConfiguration → {Arg0}", model.ActiveStoreScopeConfiguration);
        if (storeScope > 0)
        {
            model.DescriptionText_OverrideForStore = await _settingService.SettingExistsAsync(checkMoneyOrderPaymentSettings, x => x.DescriptionText, storeScope);
            model.AdditionalFee_OverrideForStore = await _settingService.SettingExistsAsync(checkMoneyOrderPaymentSettings, x => x.AdditionalFee, storeScope);
            model.AdditionalFeePercentage_OverrideForStore = await _settingService.SettingExistsAsync(checkMoneyOrderPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
            model.ShippableProductRequired_OverrideForStore = await _settingService.SettingExistsAsync(checkMoneyOrderPaymentSettings, x => x.ShippableProductRequired, storeScope);
        }

		_logger.LogDebug("<<< Exiting Configure()");
        return View("~/Plugins/Payments.CheckMoneyOrder/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var checkMoneyOrderPaymentSettings = await _settingService.LoadSettingAsync<CheckMoneyOrderPaymentSettings>(storeScope);

        //save settings
        checkMoneyOrderPaymentSettings.DescriptionText = model.DescriptionText;
        checkMoneyOrderPaymentSettings.AdditionalFee = model.AdditionalFee;
        checkMoneyOrderPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
        checkMoneyOrderPaymentSettings.ShippableProductRequired = model.ShippableProductRequired;

        /* We do not clear cache after each setting update.
         * This behavior can increase performance because cached settings will not be cleared 
         * and loaded from database after each update */
        await _settingService.SaveSettingOverridablePerStoreAsync(checkMoneyOrderPaymentSettings, x => x.DescriptionText, model.DescriptionText_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(checkMoneyOrderPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(checkMoneyOrderPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(checkMoneyOrderPaymentSettings, x => x.ShippableProductRequired, model.ShippableProductRequired_OverrideForStore, storeScope, false);

        //now clear settings cache
        await _settingService.ClearCacheAsync();

        //localization. no multi-store support for localization yet.
        foreach (var localized in model.Locales)
        {
            await _localizationService.SaveLocalizedSettingAsync(checkMoneyOrderPaymentSettings,
                x => x.DescriptionText, localized.LanguageId, localized.DescriptionText);
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    #endregion
}