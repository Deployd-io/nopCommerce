using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Stores;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components;

public partial class AclDisabledWarningViewComponent : NopViewComponent
{
    protected readonly CatalogSettings _catalogSettings;
    protected readonly ISettingService _settingService;
    protected readonly IStoreService _storeService;

    public AclDisabledWarningViewComponent(CatalogSettings catalogSettings,
        ISettingService settingService,
        IStoreService storeService)
    {
        _catalogSettings = catalogSettings;
        _settingService = settingService;
        _storeService = storeService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		logger.LogDebug(">>> Entering InvokeAsync()");
        //action displaying notification (warning) to a store owner that "ACL rules" feature is ignored

        //default setting
        var enabled = _catalogSettings.IgnoreAcl;
        if (!enabled)
		logger.LogDebug("InvokeAsync(): enabled → {Enabled}", enabled);
        {
            //overridden settings
            var stores = await _storeService.GetAllStoresAsync();
            foreach (var store in stores)
            {
			logger.LogDebug("InvokeAsync(): stores → {Stores}", stores);
                var catalogSettings = await _settingService.LoadSettingAsync<CatalogSettings>(store.Id);
                enabled = catalogSettings.IgnoreAcl;
				logger.LogDebug("InvokeAsync(): catalogSettings → {CatalogSettings}", catalogSettings);

                if (enabled)
                    break;
            }
        }

        //This setting is disabled. No warnings.
        if (!enabled)
		logger.LogDebug("<<< Exiting InvokeAsync()");
            return Content(string.Empty);

        return View();
    }
}
