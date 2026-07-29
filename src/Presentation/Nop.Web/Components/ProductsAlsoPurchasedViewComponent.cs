using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Infrastructure.Cache;

namespace Nop.Web.Components;

public partial class ProductsAlsoPurchasedViewComponent : NopViewComponent
{
    protected readonly CatalogSettings _catalogSettings;
    protected readonly IAclService _aclService;
    protected readonly IOrderReportService _orderReportService;
    protected readonly IProductModelFactory _productModelFactory;
    protected readonly IProductService _productService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;

    public ProductsAlsoPurchasedViewComponent(CatalogSettings catalogSettings,
        IAclService aclService,
        IOrderReportService orderReportService,
        IProductModelFactory productModelFactory,
        IProductService productService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService)
    {
        _catalogSettings = catalogSettings;
        _aclService = aclService;
        _orderReportService = orderReportService;
        _productModelFactory = productModelFactory;
        _productService = productService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int productId, int? productThumbPictureSize)
    {
		logger.LogDebug(">>> Entering InvokeAsync(productId={ProductId},productThumbPictureSize={ProductThumbPictureSize})", productId, productThumbPictureSize);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (!_catalogSettings.ProductsAlsoPurchasedEnabled)
            return Content("");

        //load and cache report
        var store = await _storeContext.GetCurrentStoreAsync();
		logger.LogInformation("InvokeAsync(productId,productThumbPictureSize)={ProductId},{ProductThumbPictureSize}: external service call took {Arg2} ms", productId, productThumbPictureSize, __logStart.ElapsedMilliseconds);
		logger.LogDebug("InvokeAsync(productId={ProductId},productThumbPictureSize={ProductThumbPictureSize}): store → {Store}", productId, productThumbPictureSize, store);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var productIds = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductsAlsoPurchasedIdsKey, productId, store),
            async () => await _orderReportService.GetAlsoPurchasedProductsIdsAsync(store.Id, productId, _catalogSettings.ProductsAlsoPurchasedNumber)
        );
		logger.LogDebug(">>> Entering async()");

        //load products
        var products = await (await _productService.GetProductsByIdsAsync(productIds))
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _productService.ProductIsAvailable(p)).ToListAsync();
			logger.LogInformation("InvokeAsync(productId,productThumbPictureSize)={ProductId},{ProductThumbPictureSize}: database query executed in {Arg2} ms", productId, productThumbPictureSize, __logStart.ElapsedMilliseconds);

        if (!products.Any())
            return Content("");

        var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, true, true, productThumbPictureSize)).ToList();
		logger.LogDebug("<<< Exiting InvokeAsync(productId={ProductId},productThumbPictureSize={ProductThumbPictureSize})", productId, productThumbPictureSize);
        return View(model);
    }
}