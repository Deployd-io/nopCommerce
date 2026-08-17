using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Stores;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Components;

public partial class MultistoreDisabledWarningViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly ISettingService _settingService;
    protected readonly IStoreService _storeService;

    public MultistoreDisabledWarningViewComponent(CatalogSettings catalogSettings,
        ISettingService settingService,
        IStoreService storeService)
    {
        _catalogSettings = catalogSettings;
        _settingService = settingService;
		_logger.LogDebug("MultistoreDisabledWarningViewComponent(catalogSettings={CatalogSettings},settingService={SettingService},storeService={StoreService}): _settingService → {_settingService}", catalogSettings, settingService, storeService, _settingService);
		_logger.LogDebug("MultistoreDisabledWarningViewComponent(catalogSettings={CatalogSettings},settingService={SettingService},storeService={StoreService}): _catalogSettings → {_catalogSettings}", catalogSettings, settingService, storeService, _catalogSettings);
        _storeService = storeService;
		_logger.LogDebug("<<< Exiting MultistoreDisabledWarningViewComponent(catalogSettings={CatalogSettings},settingService={SettingService},storeService={StoreService})", catalogSettings, settingService, storeService);
		_logger.LogDebug("MultistoreDisabledWarningViewComponent(catalogSettings={CatalogSettings},settingService={SettingService},storeService={StoreService}): _storeService → {_storeService}", catalogSettings, settingService, storeService, _storeService);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
	_logger.LogDebug(">>> Entering InvokeAsync()");

        //action displaying notification (warning) to a store owner that "limit per store" feature is ignored

        //default setting
        var enabled = _catalogSettings.IgnoreStoreLimitations;
		_logger.LogDebug("InvokeAsync(): enabled → {Enabled}", enabled);
        if (!enabled)
        {
            //overridden settings
            var stores = await _storeService.GetAllStoresAsync();
			_logger.LogDebug("InvokeAsync(): stores → {Stores}", stores);
            foreach (var store in stores)
            {
                var catalogSettings = await _settingService.LoadSettingAsync<CatalogSettings>(store.Id);
				_logger.LogDebug("InvokeAsync(): catalogSettings → {CatalogSettings}", catalogSettings);
                enabled = catalogSettings.IgnoreStoreLimitations;

                if (enabled)
                    break;
            }
        }

        //This setting is disabled. No warnings.
        if (!enabled)
		_logger.LogDebug("<<< Exiting InvokeAsync()");
            return Content("");

        return View();
    }
}