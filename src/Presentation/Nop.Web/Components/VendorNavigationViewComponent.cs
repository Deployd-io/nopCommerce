using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class VendorNavigationViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly ICatalogModelFactory _catalogModelFactory;
    protected readonly VendorSettings _vendorSettings;

    public VendorNavigationViewComponent(ICatalogModelFactory catalogModelFactory,
        VendorSettings vendorSettings)
    {
		_logger.LogDebug(">>> Entering VendorNavigationViewComponent(catalogModelFactory={CatalogModelFactory},vendorSettings={VendorSettings})", catalogModelFactory, vendorSettings);
        _catalogModelFactory = catalogModelFactory;
        _vendorSettings = vendorSettings;
		_logger.LogDebug("VendorNavigationViewComponent(catalogModelFactory={CatalogModelFactory},vendorSettings={VendorSettings}): _vendorSettings → {_vendorSettings}", catalogModelFactory, vendorSettings, _vendorSettings);
		_logger.LogDebug("VendorNavigationViewComponent(catalogModelFactory={CatalogModelFactory},vendorSettings={VendorSettings}): _catalogModelFactory → {_catalogModelFactory}", catalogModelFactory, vendorSettings, _catalogModelFactory);
		_logger.LogDebug("<<< Exiting VendorNavigationViewComponent(catalogModelFactory={CatalogModelFactory},vendorSettings={VendorSettings})", catalogModelFactory, vendorSettings);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogDebug(">>> Entering InvokeAsync()");
        if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
            return Content("");

        var model = await _catalogModelFactory.PrepareVendorNavigationModelAsync();
		_logger.LogDebug("InvokeAsync(): model → {Model}", model);
        if (!model.Vendors.Any())
            return Content("");

        return View(model);
    }
}