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
        _localizationService = localizationService;
        _notificationService = notificationService;
        _settingService = settingService;
        _omnisendService = omnisendService;
        _omnisendSettings = omnisendSettings;
    }

    #endregion

    #region Utilities

    private async Task FillBatchesAsync(ConfigurationModel model)
    {
        if (!_omnisendSettings.BatchesIds.Any())
            return;

        bool needBlock(BatchResponse response, string endpoint)
        {
            return response.Endpoint.Equals(endpoint, StringComparison.InvariantCultureIgnoreCase) &&
                !response.Status.Equals(OmnisendDefaults.BatchFinishedStatus,
                    StringComparison.InvariantCultureIgnoreCase);
        }

        var batches = await _omnisendService.GetStoredBatchesAsync();
        model.Batches = batches;

        model.BlockSyncContacts = model.Batches.Any(p => needBlock(p, OmnisendDefaults.ContactsEndpoint));
        model.BlockSyncOrders = model.Batches.Any(p => needBlock(p, OmnisendDefaults.OrdersEndpoint));
        model.BlockSyncProducts = model.Batches.Any(p => needBlock(p, OmnisendDefaults.ProductsEndpoint)) ||
            batches.Any(p => needBlock(p, OmnisendDefaults.CategoriesEndpoint));
			_logger.LogDebug("<<< Exiting FillBatchesAsync(model={Model})", model);
    }

    #endregion

    #region Methods

    public async Task<IActionResult> Configure()
    {
		_logger.LogDebug(">>> Entering Configure()");
        var model = new ConfigurationModel
        {
            ApiKey = _omnisendSettings.ApiKey,
            UseTracking = _omnisendSettings.UseTracking
        };

        await FillBatchesAsync(model);

		_logger.LogDebug("<<< Exiting Configure()");
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
		_logger.LogDebug(">>> Entering SyncContacts()");
        if (!ModelState.IsValid || string.IsNullOrEmpty(_omnisendSettings.BrandId))
            return await Configure();

        await _omnisendService.SyncContactsAsync();

		_logger.LogDebug("<<< Exiting SyncContacts()");
        return await Configure();
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("sync-products")]
    public async Task<IActionResult> SyncProducts()
    {
		_logger.LogDebug(">>> Entering SyncProducts()");
        if (!ModelState.IsValid || string.IsNullOrEmpty(_omnisendSettings.BrandId))
            return await Configure();

        await _omnisendService.SyncCategoriesAsync();
        await _omnisendService.SyncProductsAsync();

		_logger.LogDebug("<<< Exiting SyncProducts()");
        return await Configure();
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("sync-orders")]
    public async Task<IActionResult> SyncOrders()
    {
		_logger.LogDebug(">>> Entering SyncOrders()");
        if (!ModelState.IsValid || string.IsNullOrEmpty(_omnisendSettings.BrandId))
            return await Configure();

        await _omnisendService.SyncOrdersAsync();
        await _omnisendService.SyncCartsAsync();

		_logger.LogDebug("<<< Exiting SyncOrders()");
        return await Configure();
    }

    #endregion
}