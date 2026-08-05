using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class PopularProductTagsViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly ICatalogModelFactory _catalogModelFactory;

    public PopularProductTagsViewComponent(CatalogSettings catalogSettings, ICatalogModelFactory catalogModelFactory)
    {
		_logger.LogDebug(">>> Entering PopularProductTagsViewComponent(catalogSettings={CatalogSettings},catalogModelFactory={CatalogModelFactory})", catalogSettings, catalogModelFactory);
        _catalogSettings = catalogSettings;
        _catalogModelFactory = catalogModelFactory;
		_logger.LogDebug("<<< Exiting PopularProductTagsViewComponent(catalogSettings={CatalogSettings},catalogModelFactory={CatalogModelFactory})", catalogSettings, catalogModelFactory);
		_logger.LogDebug("PopularProductTagsViewComponent(catalogSettings={CatalogSettings},catalogModelFactory={CatalogModelFactory}): _catalogModelFactory → {_catalogModelFactory}", catalogSettings, catalogModelFactory, _catalogModelFactory);
		_logger.LogDebug("PopularProductTagsViewComponent(catalogSettings={CatalogSettings},catalogModelFactory={CatalogModelFactory}): _catalogSettings → {_catalogSettings}", catalogSettings, catalogModelFactory, _catalogSettings);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogDebug(">>> Entering InvokeAsync()");
        if (_catalogSettings.NumberOfProductTags == 0)
            return Content("");

        var model = await _catalogModelFactory.PreparePopularProductTagsModelAsync(_catalogSettings.NumberOfProductTags);
        if (!model.Tags.Any())
			_logger.LogDebug("InvokeAsync(): model → {Model}", model);
            return Content("");

        return View(model);
    }
}