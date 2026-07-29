using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class SearchBoxViewComponent : NopViewComponent
{
    protected readonly ICatalogModelFactory _catalogModelFactory;

    public SearchBoxViewComponent(ICatalogModelFactory catalogModelFactory)
    {
		logger.LogDebug(">>> Entering SearchBoxViewComponent(catalogModelFactory={CatalogModelFactory})", catalogModelFactory);
        _catalogModelFactory = catalogModelFactory;
		logger.LogDebug("<<< Exiting SearchBoxViewComponent(catalogModelFactory={CatalogModelFactory})", catalogModelFactory);
		logger.LogDebug("SearchBoxViewComponent(catalogModelFactory={CatalogModelFactory}): _catalogModelFactory → {_catalogModelFactory}", catalogModelFactory, _catalogModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		logger.LogDebug(">>> Entering InvokeAsync()");
        var model = await _catalogModelFactory.PrepareSearchBoxModelAsync();
		logger.LogDebug("InvokeAsync(): model → {Model}", model);
		logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(model);
    }
}