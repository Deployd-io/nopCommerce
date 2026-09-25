using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Stores;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Components;

public partial class AclDisabledWarningViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly ISettingService _settingService;
    protected readonly IStoreService _storeService;

    public AclDisabledWarningViewComponent(CatalogSettings catalogSettings,
        ISettingService settingService,
        IStoreService storeService)
    {
		_logger.LogError(">>> Entering AclDisabledWarningViewComponent(catalogSettings={CatalogSettings},settingService={SettingService},storeService={StoreService})", catalogSettings, settingService, storeService);
        _catalogSettings = catalogSettings;
        _settingService = settingService;
		_logger.LogDebug("AclDisabledWarningViewComponent(catalogSettings={CatalogSettings},settingService={SettingService},storeService={StoreService}): _settingService → {_settingService}", catalogSettings, settingService, storeService, _settingService);
        _storeService = storeService;
		_logger.LogDebug("AclDisabledWarningViewComponent(catalogSettings={CatalogSettings},settingService={SettingService},storeService={StoreService}): _storeService → {_storeService}", catalogSettings, settingService, storeService, _storeService);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogError(">>> Entering InvokeAsync()");
        //action displaying notification (warning) to a store owner that "ACL rules" feature is ignored

        //default setting
        var enabled = _catalogSettings.IgnoreAcl;
		_logger.LogError("InvokeAsync(): enabled → {Enabled}", enabled);
        if (!enabled)
        {
            //overridden settings
            var stores = await _storeService.GetAllStoresAsync();
			_logger.LogError("InvokeAsync(): stores → {Stores}", stores);
            foreach (var store in stores)
            {
                var catalogSettings = await _settingService.LoadSettingAsync<CatalogSettings>(store.Id);
				_logger.LogError("InvokeAsync(): catalogSettings → {CatalogSettings}", catalogSettings);
                enabled = catalogSettings.IgnoreAcl;

                if (enabled)
                    break;
            }
        }

        //This setting is disabled. No warnings.
		_logger.LogWarning("<<< Exiting InvokeAsync()");
        if (!enabled) {
            return Content(string.Empty);
        }

        return View();
    }
}