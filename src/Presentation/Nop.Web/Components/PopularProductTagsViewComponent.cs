using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class PopularProductTagsViewComponent : NopViewComponent
{
    protected readonly CatalogSettings _catalogSettings;
    protected readonly ICatalogModelFactory _catalogModelFactory;

    public PopularProductTagsViewComponent(CatalogSettings catalogSettings, ICatalogModelFactory catalogModelFactory)
    {
		logger.LogDebug(">>> Entering PopularProductTagsViewComponent(catalogSettings={CatalogSettings},catalogModelFactory={CatalogModelFactory})", catalogSettings, catalogModelFactory);
        _catalogSettings = catalogSettings;
        _catalogModelFactory = catalogModelFactory;
		logger.LogDebug("<<< Exiting PopularProductTagsViewComponent(catalogSettings={CatalogSettings},catalogModelFactory={CatalogModelFactory})", catalogSettings, catalogModelFactory);
		logger.LogDebug("PopularProductTagsViewComponent(catalogSettings={CatalogSettings},catalogModelFactory={CatalogModelFactory}): _catalogModelFactory → {_catalogModelFactory}", catalogSettings, catalogModelFactory, _catalogModelFactory);
		logger.LogDebug("PopularProductTagsViewComponent(catalogSettings={CatalogSettings},catalogModelFactory={CatalogModelFactory}): _catalogSettings → {_catalogSettings}", catalogSettings, catalogModelFactory, _catalogSettings);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		logger.LogDebug(">>> Entering InvokeAsync()");
        if (_catalogSettings.NumberOfProductTags == 0)
            return Content("");

        var model = await _catalogModelFactory.PreparePopularProductTagsModelAsync(_catalogSettings.NumberOfProductTags);
        if (!model.Tags.Any())
			logger.LogDebug("InvokeAsync(): model → {Model}", model);
            return Content("");

        return View(model);
    }
}
