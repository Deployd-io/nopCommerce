using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class HomepageCategoriesViewComponent : NopViewComponent
{
    protected readonly ICatalogModelFactory _catalogModelFactory;

    public HomepageCategoriesViewComponent(ICatalogModelFactory catalogModelFactory)
    {
		logger.LogDebug(">>> Entering HomepageCategoriesViewComponent(catalogModelFactory={CatalogModelFactory})", catalogModelFactory);
        _catalogModelFactory = catalogModelFactory;
		logger.LogDebug("<<< Exiting HomepageCategoriesViewComponent(catalogModelFactory={CatalogModelFactory})", catalogModelFactory);
		logger.LogDebug("HomepageCategoriesViewComponent(catalogModelFactory={CatalogModelFactory}): _catalogModelFactory → {_catalogModelFactory}", catalogModelFactory, _catalogModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		logger.LogDebug(">>> Entering InvokeAsync()");
        var model = await _catalogModelFactory.PrepareHomepageCategoryModelsAsync();
        if (!model.Any())
			logger.LogDebug("InvokeAsync(): model → {Model}", model);
            return Content("");

        return View(model);
    }
}