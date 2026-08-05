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
        _storeService = storeService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
	_logger.LogDebug(">>> Entering InvokeAsync()");

        //action displaying notification (warning) to a store owner that "limit per store" feature is ignored

        //default setting
        var enabled = _catalogSettings.IgnoreStoreLimitations;
        if (!enabled)
		_logger.LogDebug("InvokeAsync(): enabled → {Enabled}", enabled);
        {
            //overridden settings
            var stores = await _storeService.GetAllStoresAsync();
            foreach (var store in stores)
            {
			_logger.LogDebug("InvokeAsync(): stores → {Stores}", stores);
                var catalogSettings = await _settingService.LoadSettingAsync<CatalogSettings>(store.Id);
                enabled = catalogSettings.IgnoreStoreLimitations;
				_logger.LogDebug("InvokeAsync(): catalogSettings → {CatalogSettings}", catalogSettings);

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