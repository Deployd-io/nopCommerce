using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Omnisend.DTO;
using Nop.Plugin.Misc.Omnisend.Models;
using Nop.Plugin.Misc.Omnisend.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Omnisend.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class OmnisendAdminController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly ISettingService _settingService;
    private readonly OmnisendService _omnisendService;
    private readonly OmnisendSettings _omnisendSettings;

    #endregion

    #region Ctor

    public OmnisendAdminController(ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService,
        OmnisendService omnisendService,
        OmnisendSettings omnisendSettings)
    {
		_logger.LogError(">>> Entering OmnisendAdminController(localizationService={LocalizationService},notificationService={NotificationService},settingService={SettingService},omnisendService={OmnisendService},omnisendSettings={OmnisendSettings})", localizationService, notificationService, settingService, omnisendService, omnisendSettings);
        _localizationService = localizationService;
        _notificationService = notificationService;
		_logger.LogDebug("OmnisendAdminController(localizationService={LocalizationService},notificationService={NotificationService},settingService={SettingService},omnisendService={OmnisendService},omnisendSettings={OmnisendSettings}): _notificationService → {_notificationService}", localizationService, notificationService, settingService, omnisendService, omnisendSettings, _notificationService);
        _settingService = settingService;
		_logger.LogDebug("OmnisendAdminController(localizationService={LocalizationService},notificationService={NotificationService},settingService={SettingService},omnisendService={OmnisendService},omnisendSettings={OmnisendSettings}): _settingService → {_settingService}", localizationService, notificationService, settingService, omnisendService, omnisendSettings, _settingService);
        _omnisendService = omnisendService;
		_logger.LogDebug("OmnisendAdminController(localizationService={LocalizationService},notificationService={NotificationService},settingService={SettingService},omnisendService={OmnisendService},omnisendSettings={OmnisendSettings}): _omnisendService → {_omnisendService}", localizationService, notificationService, settingService, omnisendService, omnisendSettings, _omnisendService);
        _omnisendSettings = omnisendSettings;
		_logger.LogDebug("OmnisendAdminController(localizationService={LocalizationService},notificationService={NotificationService},settingService={SettingService},omnisendService={OmnisendService},omnisendSettings={OmnisendSettings}): _omnisendSettings → {_omnisendSettings}", localizationService, notificationService, settingService, omnisendService, omnisendSettings, _omnisendSettings);
    }

    #endregion

    #region Utilities

    private async Task FillBatchesAsync(ConfigurationModel model)
    {
		_logger.LogError(">>> Entering FillBatchesAsync(model={Model})", model);
        if (!_omnisendSettings.BatchesIds.Any())
            return;

        bool needBlock(BatchResponse response, string endpoint)
        {
            return response.Endpoint.Equals(endpoint, StringComparison.InvariantCultureIgnoreCase) &&
                !response.Status.Equals(OmnisendDefaults.BatchFinishedStatus,
                    StringComparison.InvariantCultureIgnoreCase);
        }

        var batches = await _omnisendService.GetStoredBatchesAsync();
		_logger.LogError("FillBatchesAsync(model={Model}): batches → {Batches}", model, batches);
        model.Batches = batches;
		_logger.LogDebug("FillBatchesAsync(model={Model}): model.Batches → {Arg1}", model, model.Batches);

        model.BlockSyncContacts = model.Batches.Any(p => needBlock(p, OmnisendDefaults.ContactsEndpoint));
        model.BlockSyncOrders = model.Batches.Any(p => needBlock(p, OmnisendDefaults.OrdersEndpoint));
        model.BlockSyncProducts = model.Batches.Any(p => needBlock(p, OmnisendDefaults.ProductsEndpoint)) ||
            batches.Any(p => needBlock(p, OmnisendDefaults.CategoriesEndpoint));
			_logger.LogWarning("<<< Exiting FillBatchesAsync(model={Model})", model);
    }

    #endregion

    #region Methods

    public async Task<IActionResult> Configure()
    {
		_logger.LogError(">>> Entering Configure()");
        var model = new ConfigurationModel
        {
            ApiKey = _omnisendSettings.ApiKey,
            UseTracking = _omnisendSettings.UseTracking
        };

        await FillBatchesAsync(model);

		_logger.LogWarning("<<< Exiting Configure()");
		_logger.LogWarning("<<< Exiting Configure(model={Model})", model);
        return View("~/Plugins/Misc.Omnisend/Views/Configure.cshtml", model);
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("save")]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        if (_omnisendSettings.ApiKey != model.ApiKey || string.IsNullOrEmpty(_omnisendSettings.BrandId))
        {
            var brandId = await _omnisendService.GetBrandIdAsync(model.ApiKey);

            if (brandId != null)
            {
                _omnisendSettings.ApiKey = model.ApiKey;
                _omnisendSettings.BrandId = brandId;

                await _omnisendService.SendCustomerDataAsync();
            }
        }

        //_omnisendSettings.UseTracking = model.UseTracking;
        //recommended not to change this setting
        _omnisendSettings.UseTracking = true;

        await _settingService.SaveSettingAsync(_omnisendSettings);

        if (string.IsNullOrEmpty(_omnisendSettings.BrandId))
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Omnisend.CantGetBrandId"));
        else
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("sync-contacts")]
    public async Task<IActionResult> SyncContacts()
    {
		_logger.LogError(">>> Entering SyncContacts()");
        if (!ModelState.IsValid || string.IsNullOrEmpty(_omnisendSettings.BrandId))
            return await Configure();

        await _omnisendService.SyncContactsAsync();

		_logger.LogWarning("<<< Exiting SyncContacts()");
        return await Configure();
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("sync-products")]
    public async Task<IActionResult> SyncProducts()
    {
		_logger.LogError(">>> Entering SyncProducts()");
        if (!ModelState.IsValid || string.IsNullOrEmpty(_omnisendSettings.BrandId))
            return await Configure();

        await _omnisendService.SyncCategoriesAsync();
        await _omnisendService.SyncProductsAsync();

		_logger.LogWarning("<<< Exiting SyncProducts()");
        return await Configure();
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("sync-orders")]
    public async Task<IActionResult> SyncOrders()
    {
		_logger.LogError(">>> Entering SyncOrders()");
        if (!ModelState.IsValid || string.IsNullOrEmpty(_omnisendSettings.BrandId))
            return await Configure();

        await _omnisendService.SyncOrdersAsync();
        await _omnisendService.SyncCartsAsync();

		_logger.LogWarning("<<< Exiting SyncOrders()");
        return await Configure();
    }

    #endregion
}