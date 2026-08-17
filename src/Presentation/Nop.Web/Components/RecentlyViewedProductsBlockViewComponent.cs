using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class RecentlyViewedProductsBlockViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly IAclService _aclService;
    protected readonly IProductModelFactory _productModelFactory;
    protected readonly IProductService _productService;
    protected readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
    protected readonly IStoreMappingService _storeMappingService;

    public RecentlyViewedProductsBlockViewComponent(CatalogSettings catalogSettings,
        IAclService aclService,
        IProductModelFactory productModelFactory,
        IProductService productService,
        IRecentlyViewedProductsService recentlyViewedProductsService,
        IStoreMappingService storeMappingService)
    {
        _catalogSettings = catalogSettings;
        _aclService = aclService;
        _productModelFactory = productModelFactory;
        _productService = productService;
        _recentlyViewedProductsService = recentlyViewedProductsService;
        _storeMappingService = storeMappingService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int? productThumbPictureSize, bool? preparePriceModel)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(productThumbPictureSize={ProductThumbPictureSize},preparePriceModel={PreparePriceModel})", productThumbPictureSize, preparePriceModel);
        if (!_catalogSettings.RecentlyViewedProductsEnabled)
            return Content("");

        var preparePictureModel = productThumbPictureSize.HasValue;
        var products = await (await _recentlyViewedProductsService.GetRecentlyViewedProductsAsync(_catalogSettings.RecentlyViewedProductsNumber))
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogInformation("InvokeAsync(productThumbPictureSize,preparePriceModel)={ProductThumbPictureSize},{PreparePriceModel}: database query executed in {Arg2} ms", productThumbPictureSize, preparePriceModel, __logStart.ElapsedMilliseconds);
            .Where(p => _productService.ProductIsAvailable(p)).ToListAsync();

        if (!products.Any())
            return Content("");

        //prepare model
        var model = new List<ProductOverviewModel>();
		_logger.LogDebug("InvokeAsync(productThumbPictureSize={ProductThumbPictureSize},preparePriceModel={PreparePriceModel}): model → {Model}", productThumbPictureSize, preparePriceModel, model);
        model.AddRange(await _productModelFactory.PrepareProductOverviewModelsAsync(products,
            preparePriceModel.GetValueOrDefault(),
            preparePictureModel,
            productThumbPictureSize));

		_logger.LogDebug("<<< Exiting InvokeAsync(productThumbPictureSize={ProductThumbPictureSize},preparePriceModel={PreparePriceModel})", productThumbPictureSize, preparePriceModel);
        return View(model);
    }
}