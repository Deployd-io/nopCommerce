using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class ManufacturerNavigationViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly ICatalogModelFactory _catalogModelFactory;

    public ManufacturerNavigationViewComponent(CatalogSettings catalogSettings, ICatalogModelFactory catalogModelFactory)
    {
		_logger.LogDebug(">>> Entering ManufacturerNavigationViewComponent(catalogSettings={CatalogSettings},catalogModelFactory={CatalogModelFactory})", catalogSettings, catalogModelFactory);
        _catalogSettings = catalogSettings;
		_logger.LogDebug("ManufacturerNavigationViewComponent(catalogSettings={CatalogSettings},catalogModelFactory={CatalogModelFactory}): _catalogSettings → {_catalogSettings}", catalogSettings, catalogModelFactory, _catalogSettings);
        _catalogModelFactory = catalogModelFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync(int currentManufacturerId)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(currentManufacturerId={CurrentManufacturerId})", currentManufacturerId);
        if (_catalogSettings.ManufacturersBlockItemsToDisplay == 0)
            return Content("");

        var model = await _catalogModelFactory.PrepareManufacturerNavigationModelAsync(currentManufacturerId);
		_logger.LogDebug("InvokeAsync(currentManufacturerId={CurrentManufacturerId}): model → {Model}", currentManufacturerId, model);
        if (!model.Manufacturers.Any())
            return Content("");

        return View(model);
    }
}