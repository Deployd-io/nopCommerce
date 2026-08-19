using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class HomepageCategoriesViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly ICatalogModelFactory _catalogModelFactory;

    public HomepageCategoriesViewComponent(ICatalogModelFactory catalogModelFactory)
    {
		_logger.LogDebug(">>> Entering HomepageCategoriesViewComponent(catalogModelFactory={CatalogModelFactory})", catalogModelFactory);
        _catalogModelFactory = catalogModelFactory;
		_logger.LogDebug("HomepageCategoriesViewComponent(catalogModelFactory={CatalogModelFactory}): _catalogModelFactory → {_catalogModelFactory}", catalogModelFactory, _catalogModelFactory);
		_logger.LogDebug("<<< Exiting HomepageCategoriesViewComponent(catalogModelFactory={CatalogModelFactory})", catalogModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogDebug(">>> Entering InvokeAsync()");
        var model = await _catalogModelFactory.PrepareHomepageCategoryModelsAsync();
        if (!model.Any())
            return Content("");

        return View(model);
    }
}