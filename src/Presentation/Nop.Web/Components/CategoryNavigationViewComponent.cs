using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class CategoryNavigationViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly ICatalogModelFactory _catalogModelFactory;

    public CategoryNavigationViewComponent(ICatalogModelFactory catalogModelFactory)
    {
		_logger.LogDebug(">>> Entering CategoryNavigationViewComponent(catalogModelFactory={CatalogModelFactory})", catalogModelFactory);
        _catalogModelFactory = catalogModelFactory;
		_logger.LogDebug("CategoryNavigationViewComponent(catalogModelFactory={CatalogModelFactory}): _catalogModelFactory → {_catalogModelFactory}", catalogModelFactory, _catalogModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(int currentCategoryId, int currentProductId)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId})", currentCategoryId, currentProductId);
        var model = await _catalogModelFactory.PrepareCategoryNavigationModelAsync(currentCategoryId, currentProductId);
		_logger.LogDebug("InvokeAsync(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId}): model → {Model}", currentCategoryId, currentProductId, model);
		_logger.LogDebug("<<< Exiting InvokeAsync(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId})", currentCategoryId, currentProductId);
        return View(model);
    }
}