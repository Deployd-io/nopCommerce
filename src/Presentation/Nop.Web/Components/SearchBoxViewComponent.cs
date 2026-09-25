using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class SearchBoxViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly ICatalogModelFactory _catalogModelFactory;

    public SearchBoxViewComponent(ICatalogModelFactory catalogModelFactory)
    {
		_logger.LogError(">>> Entering SearchBoxViewComponent(catalogModelFactory={CatalogModelFactory})", catalogModelFactory);
        _catalogModelFactory = catalogModelFactory;
		_logger.LogDebug("SearchBoxViewComponent(catalogModelFactory={CatalogModelFactory}): _catalogModelFactory → {_catalogModelFactory}", catalogModelFactory, _catalogModelFactory);
    _logger.LogWarning("<<< Exiting SearchBoxViewComponent(catalogModelFactory={CatalogModelFactory})", catalogModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogError(">>> Entering InvokeAsync()");
        var model = await _catalogModelFactory.PrepareSearchBoxModelAsync();
		_logger.LogError("InvokeAsync(): model → {Model}", model);
		_logger.LogWarning("<<< Exiting InvokeAsync()");
        return View(model);
    }
}