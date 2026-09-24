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
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class ProductsAlsoPurchasedViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogError(">>> Entering ProductsAlsoPurchasedViewComponent(catalogSettings={CatalogSettings},aclService={AclService},orderReportService={OrderReportService},productModelFactory={ProductModelFactory},productService={ProductService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>)", catalogSettings, aclService, orderReportService, productModelFactory, productService, staticCacheManager, storeContext);
        _catalogSettings = catalogSettings;
        _aclService = aclService;
		_logger.LogDebug("ProductsAlsoPurchasedViewComponent(catalogSettings={CatalogSettings},aclService={AclService},orderReportService={OrderReportService},productModelFactory={ProductModelFactory},productService={ProductService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>): _aclService → {_aclService}", catalogSettings, aclService, orderReportService, productModelFactory, productService, staticCacheManager, storeContext, _aclService);
        _orderReportService = orderReportService;
		_logger.LogDebug("ProductsAlsoPurchasedViewComponent(catalogSettings={CatalogSettings},aclService={AclService},orderReportService={OrderReportService},productModelFactory={ProductModelFactory},productService={ProductService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>): _orderReportService → {_orderReportService}", catalogSettings, aclService, orderReportService, productModelFactory, productService, staticCacheManager, storeContext, _orderReportService);
        _productModelFactory = productModelFactory;
		_logger.LogDebug("ProductsAlsoPurchasedViewComponent(catalogSettings={CatalogSettings},aclService={AclService},orderReportService={OrderReportService},productModelFactory={ProductModelFactory},productService={ProductService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>): _productModelFactory → {_productModelFactory}", catalogSettings, aclService, orderReportService, productModelFactory, productService, staticCacheManager, storeContext, _productModelFactory);
        _productService = productService;
		_logger.LogDebug("ProductsAlsoPurchasedViewComponent(catalogSettings={CatalogSettings},aclService={AclService},orderReportService={OrderReportService},productModelFactory={ProductModelFactory},productService={ProductService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>): _productService → {_productService}", catalogSettings, aclService, orderReportService, productModelFactory, productService, staticCacheManager, storeContext, _productService);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("ProductsAlsoPurchasedViewComponent(catalogSettings={CatalogSettings},aclService={AclService},orderReportService={OrderReportService},productModelFactory={ProductModelFactory},productService={ProductService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>): _staticCacheManager → {_staticCacheManager}", catalogSettings, aclService, orderReportService, productModelFactory, productService, staticCacheManager, storeContext, _staticCacheManager);
        _storeContext = storeContext;
		_logger.LogDebug("ProductsAlsoPurchasedViewComponent(catalogSettings={CatalogSettings},aclService={AclService},orderReportService={OrderReportService},productModelFactory={ProductModelFactory},productService={ProductService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>): _storeContext → {_storeContext}", catalogSettings, aclService, orderReportService, productModelFactory, productService, staticCacheManager, storeContext, _storeContext);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("ProductsAlsoPurchasedViewComponent(catalogSettings={CatalogSettings},aclService={AclService},orderReportService={OrderReportService},productModelFactory={ProductModelFactory},productService={ProductService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>): _storeMappingService → {_storeMappingService}", catalogSettings, aclService, orderReportService, productModelFactory, productService, staticCacheManager, storeContext, _storeMappingService);
    }

    public async Task<IViewComponentResult> InvokeAsync(int productId, int? productThumbPictureSize)
    {
		_logger.LogError(">>> Entering InvokeAsync(productId={ProductId},productThumbPictureSize={ProductThumbPictureSize})", productId, productThumbPictureSize);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (!_catalogSettings.ProductsAlsoPurchasedEnabled)
            return Content("");

        //load and cache report
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("InvokeAsync(productId={ProductId},productThumbPictureSize={ProductThumbPictureSize}): store → {Store}", productId, productThumbPictureSize, store);
        var productIds = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductsAlsoPurchasedIdsKey, productId, store),
            async () => await _orderReportService.GetAlsoPurchasedProductsIdsAsync(store.Id, productId, _catalogSettings.ProductsAlsoPurchasedNumber)
        );

        //load products
        var products = await (await _productService.GetProductsByIdsAsync(productIds))
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _productService.ProductIsAvailable(p)).ToListAsync();
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogError("InvokeAsync(productId,productThumbPictureSize)={ProductId},{ProductThumbPictureSize}: database query executed in {Arg2} ms", productId, productThumbPictureSize, __logStartDb.ElapsedMilliseconds);

        if (!products.Any())
            return Content("");

        var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, true, true, productThumbPictureSize)).ToList();
		_logger.LogError("InvokeAsync(productId={ProductId},productThumbPictureSize={ProductThumbPictureSize}): model → {Model}", productId, productThumbPictureSize, model);
		_logger.LogError("InvokeAsync(productId,productThumbPictureSize)={ProductId},{ProductThumbPictureSize}: external service call took {Arg2} ms", productId, productThumbPictureSize, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting InvokeAsync(productId={ProductId},productThumbPictureSize={ProductThumbPictureSize})", productId, productThumbPictureSize);
        return View(model);
    }
}