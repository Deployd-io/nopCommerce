using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Orders;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.PayPalCommerce.Domain;
using Nop.Plugin.Payments.PayPalCommerce.Factories;
using Nop.Plugin.Payments.PayPalCommerce.Models.Admin;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.PayPalCommerce.Controllers;

[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
[ValidateIpAddress]
[AuthorizeAdmin]
public class PayPalCommerceController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly AppSettings _appSettings;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;
    private readonly PayPalCommerceModelFactory _modelFactory;
    private readonly PayPalCommerceServiceManager _serviceManager;
    private readonly ShoppingCartSettings _shoppingCartSettings;

    #endregion

    #region Ctor

    public PayPalCommerceController(AppSettings appSettings,
        ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService,
        IStoreContext storeContext,
        IWorkContext workContext,
        PayPalCommerceModelFactory modelFactory,
        PayPalCommerceServiceManager serviceManager,
        ShoppingCartSettings shoppingCartSettings)
    {
		_logger.LogError(">>> Entering PayPalCommerceController(appSettings={AppSettings},localizationService={LocalizationService},notificationService={NotificationService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext},modelFactory={ModelFactory},serviceManager={ServiceManager},shoppingCartSettings=<redacted>)", appSettings, localizationService, notificationService, settingService, storeContext, workContext, modelFactory, serviceManager);
        _appSettings = appSettings;
        _localizationService = localizationService;
		_logger.LogDebug("PayPalCommerceController(appSettings={AppSettings},localizationService={LocalizationService},notificationService={NotificationService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext},modelFactory={ModelFactory},serviceManager={ServiceManager},shoppingCartSettings=<redacted>): _localizationService → {_localizationService}", appSettings, localizationService, notificationService, settingService, storeContext, workContext, modelFactory, serviceManager, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("PayPalCommerceController(appSettings={AppSettings},localizationService={LocalizationService},notificationService={NotificationService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext},modelFactory={ModelFactory},serviceManager={ServiceManager},shoppingCartSettings=<redacted>): _notificationService → {_notificationService}", appSettings, localizationService, notificationService, settingService, storeContext, workContext, modelFactory, serviceManager, _notificationService);
        _settingService = settingService;
		_logger.LogDebug("PayPalCommerceController(appSettings={AppSettings},localizationService={LocalizationService},notificationService={NotificationService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext},modelFactory={ModelFactory},serviceManager={ServiceManager},shoppingCartSettings=<redacted>): _settingService → {_settingService}", appSettings, localizationService, notificationService, settingService, storeContext, workContext, modelFactory, serviceManager, _settingService);
        _storeContext = storeContext;
		_logger.LogDebug("PayPalCommerceController(appSettings={AppSettings},localizationService={LocalizationService},notificationService={NotificationService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext},modelFactory={ModelFactory},serviceManager={ServiceManager},shoppingCartSettings=<redacted>): _storeContext → {_storeContext}", appSettings, localizationService, notificationService, settingService, storeContext, workContext, modelFactory, serviceManager, _storeContext);
        _workContext = workContext;
		_logger.LogDebug("PayPalCommerceController(appSettings={AppSettings},localizationService={LocalizationService},notificationService={NotificationService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext},modelFactory={ModelFactory},serviceManager={ServiceManager},shoppingCartSettings=<redacted>): _workContext → {_workContext}", appSettings, localizationService, notificationService, settingService, storeContext, workContext, modelFactory, serviceManager, _workContext);
        _modelFactory = modelFactory;
		_logger.LogDebug("PayPalCommerceController(appSettings={AppSettings},localizationService={LocalizationService},notificationService={NotificationService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext},modelFactory={ModelFactory},serviceManager={ServiceManager},shoppingCartSettings=<redacted>): _modelFactory → {_modelFactory}", appSettings, localizationService, notificationService, settingService, storeContext, workContext, modelFactory, serviceManager, _modelFactory);
        _serviceManager = serviceManager;
		_logger.LogDebug("PayPalCommerceController(appSettings={AppSettings},localizationService={LocalizationService},notificationService={NotificationService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext},modelFactory={ModelFactory},serviceManager={ServiceManager},shoppingCartSettings=<redacted>): _serviceManager → {_serviceManager}", appSettings, localizationService, notificationService, settingService, storeContext, workContext, modelFactory, serviceManager, _serviceManager);
        _shoppingCartSettings = shoppingCartSettings;
		_logger.LogDebug("PayPalCommerceController(appSettings={AppSettings},localizationService={LocalizationService},notificationService={NotificationService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext},modelFactory={ModelFactory},serviceManager={ServiceManager},shoppingCartSettings=<redacted>): _shoppingCartSettings → {_shoppingCartSettings}", appSettings, localizationService, notificationService, settingService, storeContext, workContext, modelFactory, serviceManager, _shoppingCartSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Load plugin settings
    /// </summary>
    /// <param name="storeId">Store id; pass null to use active store scope configuration</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the plugin settings; store id
    /// </returns>
    private async Task<(PayPalCommerceSettings Settings, int StoreId)> LoadSettingsAsync(int? storeId = null)
    {
        storeId ??= await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<PayPalCommerceSettings>(storeId ?? 0);

        //we don't need some of the shared settings that loaded above, so load them separately for the chosen store
        if (storeId > 0)
        {
            async Task<TPropType> getSettingAsync<TPropType>(Expression<Func<PayPalCommerceSettings, TPropType>> keySelector) =>
                await _settingService.GetSettingByKeyAsync<TPropType>(_settingService.GetSettingKey(settings, keySelector), storeId: storeId ?? 0);
				_logger.LogError(">>> Entering getSettingAsync(keySelector={KeySelector})", keySelector);

            settings.MerchantGuid = await getSettingAsync(setting => setting.MerchantGuid);
            settings.MerchantId = await getSettingAsync(setting => setting.MerchantId);
            settings.WebhookUrl = await getSettingAsync(setting => setting.WebhookUrl);
            settings.SetCredentialsManually = await getSettingAsync(setting => setting.SetCredentialsManually);
            settings.UseSandbox = await getSettingAsync(setting => setting.UseSandbox);
            settings.ClientId = await getSettingAsync(setting => setting.ClientId);
            settings.SecretKey = await getSettingAsync(setting => setting.SecretKey);
            settings.ConfiguratorSupported = await getSettingAsync(setting => setting.ConfiguratorSupported);
        }

        return (settings, storeId ?? 0);
    }

    /// <summary>
    /// Save settings for the passed store
    /// </summary>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="settings">Plugin settings</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="storeId">Store id</param>
    /// <param name="overrideForStore">Whether to overridde this setting for the passed store</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task SaveSettingAsync<TPropType>(PayPalCommerceSettings settings,
        Expression<Func<PayPalCommerceSettings, TPropType>> keySelector, int storeId, bool? overrideForStore = null)
    {
        //save overridden settings
		_logger.LogError(">>> Entering SaveSettingAsync(settings={Settings},keySelector={KeySelector},storeId={StoreId},overrideForStore={OverrideForStore})", settings, keySelector, storeId, overrideForStore);
        await _settingService.SaveSettingOverridablePerStoreAsync(settings, keySelector, overrideForStore ?? true, storeId, false);

        //save shared settings
        if (storeId > 0 && overrideForStore is null)
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, keySelector, true, 0, false);
			_logger.LogWarning("<<< Exiting SaveSettingAsync(settings={Settings},keySelector={KeySelector},storeId={StoreId},overrideForStore={OverrideForStore})", settings, keySelector, storeId, overrideForStore);
    }

    /// <summary>
    /// Get details to sign up a merchant
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the URL to sign up; merchant internal id
    /// </returns>
    private async Task<(string SandboxUrl, string LiveUrl, string MerchantGuid)> GetSignUpDetailsAsync(PayPalCommerceSettings settings)
    {
        if (!string.IsNullOrEmpty(settings.MerchantGuid))
            return (null, null, settings.MerchantGuid);

        //set new GUID
        var merchantGuid = Guid.NewGuid().ToString();

        //prepare URL to sign up
        var ((sandboxUrl, liveUrl), error) = await _serviceManager.PrepareSignUpUrlAsync(merchantGuid);
        if (!string.IsNullOrEmpty(error))
        {
            var locale = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Configuration.Error");
            var errorMessage = string.Format(locale, error, Url.Action("List", "Log"));
            _notificationService.ErrorNotification(errorMessage, false);
        }

        return (sandboxUrl, liveUrl, merchantGuid);
    }

    /// <summary>
    /// Check the merchant status
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="storeId">Store id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the merchant model
    /// </returns>
    private async Task<MerchantModel> CheckMerchantStatusAsync(PayPalCommerceSettings settings, int storeId)
    {
		_logger.LogError(">>> Entering CheckMerchantStatusAsync(settings={Settings},storeId={StoreId})", settings, storeId);
        //no need to check the status when credentials were manually set
        if (string.IsNullOrEmpty(settings.MerchantGuid) || settings.SetCredentialsManually)
            return new();

        var model = await _modelFactory.PrepareMerchantModelAsync(settings);
		_logger.LogError("CheckMerchantStatusAsync(settings={Settings},storeId={StoreId}): model → {Model}", settings, storeId, model);

        //disable appropriate settings when unavailable
        if (settings.ConfiguratorSupported != model.ConfiguratorSupported)
        {
            settings.ConfiguratorSupported = model.ConfiguratorSupported;
			_logger.LogDebug("CheckMerchantStatusAsync(settings={Settings},storeId={StoreId}): settings.ConfiguratorSupported → {Arg2}", settings, storeId, settings.ConfiguratorSupported);
            await SaveSettingAsync(settings, setting => setting.ConfiguratorSupported, storeId);
        }
        if (!model.AdvancedCardsEnabled && settings.UseCardFields)
        {
            settings.UseCardFields = false;
			_logger.LogDebug("CheckMerchantStatusAsync(settings={Settings},storeId={StoreId}): settings.UseCardFields → {Arg2}", settings, storeId, settings.UseCardFields);
            await SaveSettingAsync(settings, setting => setting.UseCardFields, storeId);
        }
        if (!model.ApplePayEnabled && settings.UseApplePay)
        {
            settings.UseApplePay = false;
			_logger.LogDebug("CheckMerchantStatusAsync(settings={Settings},storeId={StoreId}): settings.UseApplePay → {Arg2}", settings, storeId, settings.UseApplePay);
            await SaveSettingAsync(settings, setting => setting.UseApplePay, storeId);
        }
        if (!model.GooglePayEnabled && settings.UseGooglePay)
        {
            settings.UseGooglePay = false;
			_logger.LogDebug("CheckMerchantStatusAsync(settings={Settings},storeId={StoreId}): settings.UseGooglePay → {Arg2}", settings, storeId, settings.UseGooglePay);
            await SaveSettingAsync(settings, setting => setting.UseGooglePay, storeId);
        }
        if (!model.VaultingEnabled && settings.UseVault)
        {
            settings.UseVault = false;
			_logger.LogDebug("CheckMerchantStatusAsync(settings={Settings},storeId={StoreId}): settings.UseVault → {Arg2}", settings, storeId, settings.UseVault);
            await SaveSettingAsync(settings, setting => setting.UseVault, storeId);
        }
        await _settingService.ClearCacheAsync();

        //display notifications
        foreach (var warning in model.Messages.Warning)
        {
            _notificationService.WarningNotification(warning, false);
        }

        foreach (var error in model.Messages.Error)
        {
            _notificationService.ErrorNotification(error, false);
        }

        foreach (var message in model.Messages.Success)
        {
            _notificationService.SuccessNotification(message);
        }

		_logger.LogWarning("<<< Exiting CheckMerchantStatusAsync(settings={Settings},storeId={StoreId})", settings, storeId);
        return model;
    }

    /// <summary>
    /// Set credentials manually
    /// </summary>
    /// <param name="model">Configuration model</param>
    /// <param name="settings">Plugin settings</param>
    /// <param name="storeId">Store id</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task SetCredentialsManuallyAsync(ConfigurationModel model, PayPalCommerceSettings settings, int storeId)
    {
		_logger.LogError(">>> Entering SetCredentialsManuallyAsync(model={Model},settings={Settings},storeId={StoreId})", model, settings, storeId);
        if (!model.SetCredentialsManually)
            return;

        //first delete the unused webhook on a previous client, if changed
        if (PayPalCommerceServiceManager.IsConnected(settings) && !string.Equals(model.ClientId, settings.ClientId))
        {
            await _serviceManager.DeleteWebhookAsync(settings);
            settings.WebhookUrl = string.Empty;
			_logger.LogDebug("SetCredentialsManuallyAsync(model={Model},settings={Settings},storeId={StoreId}): settings.WebhookUrl → {Arg3}", model, settings, storeId, settings.WebhookUrl);
        }

        settings.ClientId = model.ClientId;
		_logger.LogDebug("SetCredentialsManuallyAsync(model={Model},settings={Settings},storeId={StoreId}): settings.ClientId → {Arg3}", model, settings, storeId, settings.ClientId);
		_logger.LogDebug("SetCredentialsManuallyAsync(model={Model},settings={Settings},storeId={StoreId}): settings.MerchantId → {Arg3}", model, settings, storeId, settings.MerchantId);
        settings.SecretKey = model.SecretKey;
		_logger.LogDebug("SetCredentialsManuallyAsync(model={Model},settings={Settings},storeId={StoreId}): settings.SecretKey → {Arg3}", model, settings, storeId, settings.SecretKey);
        settings.MerchantId = model.MerchantId;
        await SaveSettingAsync(settings, setting => setting.ClientId, storeId);
        await SaveSettingAsync(settings, setting => setting.SecretKey, storeId);
        await SaveSettingAsync(settings, setting => setting.MerchantId, storeId);
		_logger.LogWarning("<<< Exiting SetCredentialsManuallyAsync(model={Model},settings={Settings},storeId={StoreId})", model, settings, storeId);
    }

    /// <summary>
    /// Ensure that webhook created, display warning in case of fail
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="storeId">Store id</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task EnsureWebhookCreatedAsync(PayPalCommerceSettings settings, int storeId)
    {
		_logger.LogError(">>> Entering EnsureWebhookCreatedAsync(settings={Settings},storeId={StoreId})", settings, storeId);
        if (!PayPalCommerceServiceManager.IsConfigured(settings))
            return;

        var (webhook, _) = await _serviceManager.CreateWebhookAsync(settings, storeId);
        if (string.IsNullOrEmpty(webhook?.Url))
        {
            var url = Url.Action("List", "Log");
			_logger.LogError("EnsureWebhookCreatedAsync(settings={Settings},storeId={StoreId}): url → {Url}", settings, storeId, url);
            var warningMessage = string.Format(await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.WebhookWarning"), url);
			_logger.LogError("EnsureWebhookCreatedAsync(settings={Settings},storeId={StoreId}): warningMessage → {WarningMessage}", settings, storeId, warningMessage);
            _notificationService.WarningNotification(warningMessage, false);

            return;
        }

        if (string.Equals(settings.WebhookUrl, webhook.Url, StringComparison.InvariantCultureIgnoreCase))
            return;

        settings.WebhookUrl = webhook.Url;
		_logger.LogDebug("EnsureWebhookCreatedAsync(settings={Settings},storeId={StoreId}): settings.WebhookUrl → {Arg2}", settings, storeId, settings.WebhookUrl);
        await SaveSettingAsync(settings, setting => setting.WebhookUrl, storeId);
        await _settingService.ClearCacheAsync();
		_logger.LogWarning("<<< Exiting EnsureWebhookCreatedAsync(settings={Settings},storeId={StoreId})", settings, storeId);
    }

    #endregion

    #region Methods

    #region Configuration

    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Configure()
    {
		_logger.LogError(">>> Entering Configure()");
        var (settings, storeId) = await LoadSettingsAsync();

        var model = new ConfigurationModel
        {
            SetCredentialsManually = settings.SetCredentialsManually,
            UseSandbox = settings.UseSandbox,
            ClientId = settings.SetCredentialsManually ? settings.ClientId : string.Empty,
            SecretKey = settings.SetCredentialsManually ? settings.SecretKey : string.Empty,
            MerchantId = settings.SetCredentialsManually ? settings.MerchantId : string.Empty,
            PaymentTypeId = (int)settings.PaymentType,
            UseCardFields = settings.UseCardFields,
            CustomerAuthenticationRequired = settings.CustomerAuthenticationRequired,
            UseApplePay = settings.UseApplePay,
            UseGooglePay = settings.UseGooglePay,
            UseAlternativePayments = settings.UseAlternativePayments,
            UseVault = settings.UseVault,
            SkipOrderConfirmPage = settings.SkipOrderConfirmPage,
            UseShipmentTracking = settings.UseShipmentTracking,
            DisplayButtonsOnShoppingCart = settings.DisplayButtonsOnShoppingCart,
            DisplayButtonsOnProductDetails = settings.DisplayButtonsOnProductDetails,
            DisplayLogoInHeaderLinks = settings.DisplayLogoInHeaderLinks,
            LogoInHeaderLinks = settings.LogoInHeaderLinks,
            DisplayLogoInFooter = settings.DisplayLogoInFooter,
            LogoInFooter = settings.LogoInFooter,
            ActiveStoreScopeConfiguration = storeId,
            IsConfigured = PayPalCommerceServiceManager.IsConfigured(settings)
        };

        if (storeId > 0)
        {
            model.SetCredentialsManually_OverrideForStore = true;
			_logger.LogDebug("Configure(): model.SetCredentialsManually_OverrideForStore → {Arg0}", model.SetCredentialsManually_OverrideForStore);
			_logger.LogDebug("Configure(): model.ClientId_OverrideForStore → {Arg0}", model.ClientId_OverrideForStore);
			_logger.LogDebug("Configure(): model.SecretKey_OverrideForStore → {Arg0}", model.SecretKey_OverrideForStore);
			_logger.LogDebug("Configure(): model.MerchantId_OverrideForStore → {Arg0}", model.MerchantId_OverrideForStore);
            model.UseSandbox_OverrideForStore = true;
			_logger.LogDebug("Configure(): model.UseSandbox_OverrideForStore → {Arg0}", model.UseSandbox_OverrideForStore);
            model.ClientId_OverrideForStore = true;
            model.SecretKey_OverrideForStore = true;
            model.MerchantId_OverrideForStore = true;

            model.PaymentTypeId_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.PaymentType, storeId);
            model.UseCardFields_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.UseCardFields, storeId);
            model.CustomerAuthenticationRequired_OverrideForStore = await _settingService
                .SettingExistsAsync(settings, setting => setting.CustomerAuthenticationRequired, storeId);
				_logger.LogDebug("Configure(): model.CustomerAuthenticationRequired_OverrideForStore → {Arg0}", model.CustomerAuthenticationRequired_OverrideForStore);
            model.UseApplePay_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.UseApplePay, storeId);
            model.UseGooglePay_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.UseGooglePay, storeId);
            model.UseAlternativePayments_OverrideForStore = await _settingService
                .SettingExistsAsync(settings, setting => setting.UseAlternativePayments, storeId);
				_logger.LogDebug("Configure(): model.UseAlternativePayments_OverrideForStore → {Arg0}", model.UseAlternativePayments_OverrideForStore);
            model.UseVault_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.UseVault, storeId);
            model.SkipOrderConfirmPage_OverrideForStore = await _settingService
                .SettingExistsAsync(settings, setting => setting.SkipOrderConfirmPage, storeId);
				_logger.LogDebug("Configure(): model.SkipOrderConfirmPage_OverrideForStore → {Arg0}", model.SkipOrderConfirmPage_OverrideForStore);
            model.DisplayButtonsOnShoppingCart_OverrideForStore = await _settingService
                .SettingExistsAsync(settings, setting => setting.DisplayButtonsOnShoppingCart, storeId);
				_logger.LogDebug("Configure(): model.DisplayButtonsOnShoppingCart_OverrideForStore → {Arg0}", model.DisplayButtonsOnShoppingCart_OverrideForStore);
            model.DisplayButtonsOnProductDetails_OverrideForStore = await _settingService
                .SettingExistsAsync(settings, setting => setting.DisplayButtonsOnProductDetails, storeId);
				_logger.LogDebug("Configure(): model.DisplayButtonsOnProductDetails_OverrideForStore → {Arg0}", model.DisplayButtonsOnProductDetails_OverrideForStore);
            model.DisplayLogoInHeaderLinks_OverrideForStore = await _settingService
                .SettingExistsAsync(settings, setting => setting.DisplayLogoInHeaderLinks, storeId);
				_logger.LogDebug("Configure(): model.DisplayLogoInHeaderLinks_OverrideForStore → {Arg0}", model.DisplayLogoInHeaderLinks_OverrideForStore);
            model.LogoInHeaderLinks_OverrideForStore = await _settingService
                .SettingExistsAsync(settings, setting => setting.LogoInHeaderLinks, storeId);
				_logger.LogDebug("Configure(): model.LogoInHeaderLinks_OverrideForStore → {Arg0}", model.LogoInHeaderLinks_OverrideForStore);
            model.DisplayLogoInFooter_OverrideForStore = await _settingService
                .SettingExistsAsync(settings, setting => setting.DisplayLogoInFooter, storeId);
				_logger.LogDebug("Configure(): model.DisplayLogoInFooter_OverrideForStore → {Arg0}", model.DisplayLogoInFooter_OverrideForStore);
            model.LogoInFooter_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.LogoInFooter, storeId);
        }

        model.PaymentTypes = (await PaymentType.Capture.ToSelectListAsync(false, [(int)PaymentType.Subscription, (int)PaymentType.Tokenize]))
            .Select(item => new SelectListItem(item.Text, item.Value))
            .ToList();

        //whether the plugin was updated, but no merchant ID was specified
        if (settings.MerchantIdRequired)
        {
            if (settings.SetCredentialsManually)
                _notificationService.WarningNotification("Merchant ID is required for payments, please specify it below");
            else
            {
                var url = Url.Action("AllSettings", "Setting", new { settingName = nameof(PayPalCommerceSettings.MerchantId) });
				_logger.LogError("Configure(): url → {Url}", url);
                _notificationService.WarningNotification($"PayPal account ID of the merchant was not set correctly when updating the plugin. " +
                    $"You should either complete onboarding process again on this page or set the ID yourself on the " +
                    $"<a href=\"{url}\" target=\"_blank\">All Settings page</a> (you can find this ID in your PayPal account)", false);
            }
        }

        //merchant and onboarding details
        (model.SandboxSignUpUrl, model.LiveSignUpUrl, model.MerchantGuid) = await GetSignUpDetailsAsync(settings);
        model.MerchantModel = await CheckMerchantStatusAsync(settings, storeId);
        if (!settings.SetCredentialsManually)
            model.MerchantId = model.MerchantModel.MerchantId;
			_logger.LogDebug("Configure(): model.MerchantId → {Arg0}", model.MerchantId);

        await EnsureWebhookCreatedAsync(settings, storeId);

        if (PayPalCommerceServiceManager.IsConnected(settings) && !_shoppingCartSettings.RoundPricesDuringCalculation)
        {
            //prices and total aren't rounded, so display warning
            var url = Url.Action("AllSettings", "Setting", new { settingName = nameof(ShoppingCartSettings.RoundPricesDuringCalculation) });
            var warningMessage = string.Format(await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.RoundingWarning"), url);
			_logger.LogError("Configure(): warningMessage → {WarningMessage}", warningMessage);
            _notificationService.WarningNotification(warningMessage, false);
        }

        //ensure credentials are valid
        if (PayPalCommerceServiceManager.IsConnected(settings))
        {
            var (_, credentialsError) = await _serviceManager.GetAccessTokenAsync(settings);
            if (!string.IsNullOrEmpty(credentialsError))
            {
                _notificationService.ErrorNotification(await _localizationService
                    .GetResourceAsync("Plugins.Payments.PayPalCommerce.Credentials.Invalid"));
            }
            else
            {
                _notificationService.SuccessNotification(await _localizationService
                    .GetResourceAsync("Plugins.Payments.PayPalCommerce.Credentials.Valid"));
            }
        }

		_logger.LogWarning("<<< Exiting Configure()");
		_logger.LogWarning("<<< Exiting Configure(model={Model})", model);
        return View("~/Plugins/Payments.PayPalCommerce/Views/Admin/Configure.cshtml", model);
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        var (settings, storeId) = await LoadSettingsAsync();

        //set new settings values
        var (configureCredentials, _) = await Request.TryGetFormValueAsync(nameof(model.SetCredentialsManually));
        if (configureCredentials)
        {
            settings.SetCredentialsManually = model.SetCredentialsManually;
            settings.UseSandbox = model.UseSandbox;
        }
        settings.PaymentType = (PaymentType)model.PaymentTypeId;
        settings.UseCardFields = model.UseCardFields;
        settings.CustomerAuthenticationRequired = model.CustomerAuthenticationRequired;
        settings.UseApplePay = model.UseApplePay;
        settings.UseGooglePay = model.UseGooglePay;
        settings.UseAlternativePayments = model.UseAlternativePayments;
        settings.UseVault = model.UseVault;
        settings.SkipOrderConfirmPage = model.SkipOrderConfirmPage;
        settings.UseShipmentTracking = model.UseShipmentTracking;
        settings.DisplayButtonsOnShoppingCart = model.DisplayButtonsOnShoppingCart;
        settings.DisplayButtonsOnProductDetails = model.DisplayButtonsOnProductDetails;
        settings.DisplayLogoInHeaderLinks = model.DisplayLogoInHeaderLinks;
        settings.LogoInHeaderLinks = model.LogoInHeaderLinks;
        settings.DisplayLogoInFooter = model.DisplayLogoInFooter;
        settings.LogoInFooter = model.LogoInFooter;

        await SetCredentialsManuallyAsync(model, settings, storeId);

        settings.MerchantIdRequired = settings.MerchantIdRequired && string.IsNullOrEmpty(settings.MerchantId);

        await SaveSettingAsync(settings, setting => setting.SetCredentialsManually, storeId);
        await SaveSettingAsync(settings, setting => setting.UseSandbox, storeId);
        await SaveSettingAsync(settings, setting => setting.PaymentType, storeId, model.PaymentTypeId_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.UseCardFields, storeId, model.UseCardFields_OverrideForStore);
        await SaveSettingAsync(settings, setting
            => setting.CustomerAuthenticationRequired, storeId, model.CustomerAuthenticationRequired_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.UseApplePay, storeId, model.UseApplePay_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.UseGooglePay, storeId, model.UseGooglePay_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.UseAlternativePayments, storeId, model.UseAlternativePayments_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.UseVault, storeId, model.UseVault_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.SkipOrderConfirmPage, storeId, model.SkipOrderConfirmPage_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.UseShipmentTracking, storeId);
        await SaveSettingAsync(settings, setting
            => setting.DisplayButtonsOnShoppingCart, storeId, model.DisplayButtonsOnShoppingCart_OverrideForStore);
        await SaveSettingAsync(settings, setting
            => setting.DisplayButtonsOnProductDetails, storeId, model.DisplayButtonsOnProductDetails_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.DisplayLogoInHeaderLinks, storeId, model.DisplayLogoInHeaderLinks_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.LogoInHeaderLinks, storeId, model.LogoInHeaderLinks_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.DisplayLogoInFooter, storeId, model.DisplayLogoInFooter_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.LogoInFooter, storeId, model.LogoInFooter_OverrideForStore);
        await SaveSettingAsync(settings, setting => setting.MerchantIdRequired, storeId);
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return RedirectToAction("Configure");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Onboarding(ConfigurationModel model)
    {
		_logger.LogError(">>> Entering Onboarding(model={Model})", model);
        var (settings, storeId) = await LoadSettingsAsync();

        //set onboarding values
        settings.MerchantGuid = model.MerchantGuid;
        settings.MerchantId = string.Empty;
        settings.WebhookUrl = string.Empty;

        //set credentials values
        settings.SetCredentialsManually = false;
        settings.UseSandbox = model.UseSandbox;
        settings.ClientId = string.Empty;
        settings.SecretKey = string.Empty;

        settings.ConfiguratorSupported = false;

        await SaveSettingAsync(settings, setting => setting.MerchantGuid, storeId);
        await SaveSettingAsync(settings, setting => setting.MerchantId, storeId);
        await SaveSettingAsync(settings, setting => setting.WebhookUrl, storeId);
        await SaveSettingAsync(settings, setting => setting.SetCredentialsManually, storeId);
        await SaveSettingAsync(settings, setting => setting.UseSandbox, storeId);
        await SaveSettingAsync(settings, setting => setting.ClientId, storeId);
        await SaveSettingAsync(settings, setting => setting.SecretKey, storeId);
        await SaveSettingAsync(settings, setting => setting.ConfiguratorSupported, storeId);
        await _settingService.ClearCacheAsync();

		_logger.LogWarning("<<< Exiting Onboarding(model={Model})", model);
        return Json(new { success = true });
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Onboarding(OnboardingCallbackModel model)
    {
        var storeId = model.StoreId;
        (var settings, storeId) = await LoadSettingsAsync(storeId);

        if (!string.IsNullOrEmpty(settings.MerchantGuid))
        {
            //we need some time to complete the create credentials request before redirecting the merchant
            await Task.Delay(TimeSpan.FromSeconds(5));

            if (string.IsNullOrEmpty(settings.MerchantId))
            {
                settings.MerchantId = model.MerchantIdInPayPal;
                await SaveSettingAsync(settings, setting => setting.MerchantId, storeId);
                await _settingService.ClearCacheAsync();
            }
        }
        else
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Onboarding.Error"));

        return RedirectToAction("Configure");
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(AuthenticationModel model)
    {
		_logger.LogError(">>> Entering SignUp(model={Model})", model);
        var storeId = model.StoreId;
        (var settings, storeId) = await LoadSettingsAsync(storeId);

        //try to get credentials by authentication parameters
        var (credentials, _) = await _serviceManager.SignUpAsync(settings, model.AuthCode, model.SharedId);
        if (credentials is null)
            return ErrorJson(await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Onboarding.Error"));

        //first delete the unused webhook on a previous client, if changed
        if (PayPalCommerceServiceManager.IsConnected(settings) && !string.Equals(credentials.ClientId, settings.ClientId))
        {
            await _serviceManager.DeleteWebhookAsync(settings);
            settings.WebhookUrl = string.Empty;
        }

        //set onboarding values
        settings.MerchantId = credentials.PayerId;

        //set credentials values
        settings.ClientId = credentials.ClientId;
        settings.SecretKey = credentials.ClientSecret;

        await SaveSettingAsync(settings, setting => setting.MerchantId, storeId);
        await SaveSettingAsync(settings, setting => setting.WebhookUrl, storeId);
        await SaveSettingAsync(settings, setting => setting.ClientId, storeId);
        await SaveSettingAsync(settings, setting => setting.SecretKey, storeId);
        await _settingService.ClearCacheAsync();

		_logger.LogWarning("<<< Exiting SignUp(model={Model})", model);
        return Json(new { success = true });
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("revoke")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> RevokeAccess()
    {
		_logger.LogError(">>> Entering RevokeAccess()");
        var (settings, storeId) = await LoadSettingsAsync();

        //delete webhook
        if (PayPalCommerceServiceManager.IsConnected(settings))
        {
            await _serviceManager.DeleteWebhookAsync(settings);
            settings.WebhookUrl = string.Empty;
        }

        //clear onboarding values
        settings.MerchantGuid = string.Empty;
        settings.MerchantId = string.Empty;

        //clear credentials values
        settings.ClientId = string.Empty;
        settings.SecretKey = string.Empty;

        settings.ConfiguratorSupported = false;

        await SaveSettingAsync(settings, setting => setting.MerchantGuid, storeId);
        await SaveSettingAsync(settings, setting => setting.MerchantId, storeId);
        await SaveSettingAsync(settings, setting => setting.WebhookUrl, storeId);
        await SaveSettingAsync(settings, setting => setting.ClientId, storeId);
        await SaveSettingAsync(settings, setting => setting.SecretKey, storeId);
        await SaveSettingAsync(settings, setting => setting.ConfiguratorSupported, storeId);
        await _settingService.ClearCacheAsync();

        var accessRevokedMessage = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Onboarding.AccessRevoked");
		_logger.LogError("RevokeAccess(): accessRevokedMessage → {AccessRevokedMessage}", accessRevokedMessage);
        _notificationService.SuccessNotification(accessRevokedMessage);

		_logger.LogWarning("<<< Exiting RevokeAccess()");
        return RedirectToAction("Configure");
    }

    public async Task<IActionResult> ChangeUseApplePay(bool enabled)
    {
		_logger.LogError(">>> Entering ChangeUseApplePay(enabled={Enabled})", enabled);
        var message = string.Empty;
		_logger.LogError("ChangeUseApplePay(enabled={Enabled}): message → {Message}", enabled, message);
        if (enabled)
        {
            if (!_appSettings.Get<CommonConfig>().ServeUnknownFileTypes)
            {
                //this setting should be enabled for domain verification
                var locale = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Fields.UseApplePay.Warning");
				_logger.LogError("ChangeUseApplePay(enabled={Enabled}): locale → {Locale}", enabled, locale);
                message = string.Format(locale, Url.Action("AppSettings", "Setting"));
            }
        }

		_logger.LogWarning("<<< Exiting ChangeUseApplePay(enabled={Enabled})", enabled);
        return Json(new { Result = message });
    }

    #endregion

    #region Pay Later

    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> PayLater()
    {
		_logger.LogError(">>> Entering PayLater()");
        var (settings, _) = await LoadSettingsAsync();
        if (!settings.UseSandbox && !settings.ConfiguratorSupported)
            return RedirectToAction("Configure");

        var language = await _workContext.GetWorkingLanguageAsync();
		_logger.LogError("PayLater(): language → {Language}", language);
        var model = new PayLaterConfigurationModel
        {
            ClientId = settings.ClientId,
            UseSandbox = settings.UseSandbox,
            Config = !string.IsNullOrEmpty(settings.PayLaterConfig) ? settings.PayLaterConfig : "{}",
            Locale = language.LanguageCulture?.Replace('-', '_') ?? "en_US"
        };

		_logger.LogWarning("<<< Exiting PayLater()");
        return View("~/Plugins/Payments.PayPalCommerce/Views/Admin/PayLater.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> PayLaterConfig(string config)
    {
		_logger.LogError(">>> Entering PayLaterConfig(config={Config})", config);
        var (settings, storeId) = await LoadSettingsAsync();
        if (!settings.UseSandbox && !settings.ConfiguratorSupported)
            return ErrorJson("Merchant messaging configurator is not available");

        settings.PayLaterConfig = config;
        await SaveSettingAsync(settings, setting => setting.PayLaterConfig, storeId);
        await _settingService.ClearCacheAsync();

		_logger.LogWarning("<<< Exiting PayLaterConfig(config={Config})", config);
        return Json(new { message = await _localizationService.GetResourceAsync("Admin.Plugins.Saved") });

    }

    #endregion

    #endregion
}