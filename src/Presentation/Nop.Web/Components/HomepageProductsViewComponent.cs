using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class HomepageProductsViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IAclService _aclService;
    protected readonly IProductModelFactory _productModelFactory;
    protected readonly IProductService _productService;
    protected readonly IStoreMappingService _storeMappingService;

    public HomepageProductsViewComponent(IAclService aclService,
        IProductModelFactory productModelFactory,
        IProductService productService,
        IStoreMappingService storeMappingService)
    {
        _aclService = aclService;
        _productModelFactory = productModelFactory;
        _productService = productService;
        _storeMappingService = storeMappingService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int? productThumbPictureSize)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(productThumbPictureSize={ProductThumbPictureSize})", productThumbPictureSize);
        var products = await (await _productService.GetAllProductsDisplayedOnHomepageAsync())
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _productService.ProductIsAvailable(p))
            //visible individually
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogInformation("InvokeAsync(productThumbPictureSize)={ProductThumbPictureSize}: database query executed in {Arg1} ms", productThumbPictureSize, __logStart.ElapsedMilliseconds);
            .Where(p => p.VisibleIndividually).ToListAsync();

        if (!products.Any())
            return Content("");

        var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, true, true, productThumbPictureSize)).ToList();
		_logger.LogDebug("InvokeAsync(productThumbPictureSize={ProductThumbPictureSize}): model → {Model}", productThumbPictureSize, model);

		_logger.LogDebug("<<< Exiting InvokeAsync(productThumbPictureSize={ProductThumbPictureSize})", productThumbPictureSize);
        return View(model);
    }
}