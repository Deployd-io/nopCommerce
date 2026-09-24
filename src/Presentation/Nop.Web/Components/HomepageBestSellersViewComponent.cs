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

public partial class HomepageBestSellersViewComponent : NopViewComponent
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

    public HomepageBestSellersViewComponent(CatalogSettings catalogSettings,
        IAclService aclService,
        IOrderReportService orderReportService,
        IProductModelFactory productModelFactory,
        IProductService productService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService)
    {
		_logger.LogError(">>> Entering HomepageBestSellersViewComponent(catalogSettings={CatalogSettings},aclService={AclService},orderReportService={OrderReportService},productModelFactory={ProductModelFactory},productService={ProductService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>)", catalogSettings, aclService, orderReportService, productModelFactory, productService, staticCacheManager, storeContext);
        _catalogSettings = catalogSettings;
        _aclService = aclService;
		_logger.LogDebug("HomepageBestSellersViewComponent(catalogSettings={CatalogSettings},aclService={AclService},orderReportService={OrderReportService},productModelFactory={ProductModelFactory},productService={ProductService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>): _aclService → {_aclService}", catalogSettings, aclService, orderReportService, productModelFactory, productService, staticCacheManager, storeContext, _aclService);
		_logger.LogDebug("HomepageBestSellersViewComponent(catalogSettings={CatalogSettings},aclService={AclService},orderReportService={OrderReportService},productModelFactory={ProductModelFactory},productService={ProductService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>): _catalogSettings → {_catalogSettings}", catalogSettings, aclService, orderReportService, productModelFactory, productService, staticCacheManager, storeContext, _catalogSettings);
    _logger.LogWarning("<<< Exiting HomepageBestSellersViewComponent(catalogSettings={CatalogSettings},aclService={AclService},orderReportService={OrderReportService},productModelFactory={ProductModelFactory},productService={ProductService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>)", catalogSettings, aclService, orderReportService, productModelFactory, productService, staticCacheManager, storeContext);
        _orderReportService = orderReportService;
		_logger.LogDebug("HomepageBestSellersViewComponent(catalogSettings={CatalogSettings},aclService={AclService},orderReportService={OrderReportService},productModelFactory={ProductModelFactory},productService={ProductService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>): _orderReportService → {_orderReportService}", catalogSettings, aclService, orderReportService, productModelFactory, productService, staticCacheManager, storeContext, _orderReportService);
        _productModelFactory = productModelFactory;
		_logger.LogDebug("HomepageBestSellersViewComponent(catalogSettings={CatalogSettings},aclService={AclService},orderReportService={OrderReportService},productModelFactory={ProductModelFactory},productService={ProductService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>): _productModelFactory → {_productModelFactory}", catalogSettings, aclService, orderReportService, productModelFactory, productService, staticCacheManager, storeContext, _productModelFactory);
        _productService = productService;
		_logger.LogDebug("HomepageBestSellersViewComponent(catalogSettings={CatalogSettings},aclService={AclService},orderReportService={OrderReportService},productModelFactory={ProductModelFactory},productService={ProductService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>): _productService → {_productService}", catalogSettings, aclService, orderReportService, productModelFactory, productService, staticCacheManager, storeContext, _productService);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("HomepageBestSellersViewComponent(catalogSettings={CatalogSettings},aclService={AclService},orderReportService={OrderReportService},productModelFactory={ProductModelFactory},productService={ProductService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>): _staticCacheManager → {_staticCacheManager}", catalogSettings, aclService, orderReportService, productModelFactory, productService, staticCacheManager, storeContext, _staticCacheManager);
        _storeContext = storeContext;
		_logger.LogDebug("HomepageBestSellersViewComponent(catalogSettings={CatalogSettings},aclService={AclService},orderReportService={OrderReportService},productModelFactory={ProductModelFactory},productService={ProductService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeMappingService=<redacted>): _storeContext → {_storeContext}", catalogSettings, aclService, orderReportService, productModelFactory, productService, staticCacheManager, storeContext, _storeContext);
        _storeMappingService = storeMappingService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int? productThumbPictureSize)
    {
		_logger.LogError(">>> Entering InvokeAsync(productThumbPictureSize={ProductThumbPictureSize})", productThumbPictureSize);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (!_catalogSettings.ShowBestsellersOnHomepage || _catalogSettings.NumberOfBestsellersOnHomepage == 0)
            return Content("");

        //load and cache report
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("InvokeAsync(productThumbPictureSize={ProductThumbPictureSize}): store → {Store}", productThumbPictureSize, store);
        var report = await _staticCacheManager.GetAsync(
            _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.HomepageBestsellersIdsKey,
                store),
            async () => await (await _orderReportService.BestSellersReportAsync(
                storeId: store.Id,
				_logger.LogError(">>> Entering async()");
                pageSize: _catalogSettings.NumberOfBestsellersOnHomepage)).ToListAsync());

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("InvokeAsync(productThumbPictureSize)={ProductThumbPictureSize}: database query executed in {Arg1} ms", productThumbPictureSize, __logStartDb.ElapsedMilliseconds);
        //load products
        var products = await (await _productService.GetProductsByIdsAsync(report.Select(x => x.ProductId).ToArray()))
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _productService.ProductIsAvailable(p)).ToListAsync();

        if (!products.Any())
            return Content("");

        //prepare model
        var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, true, true, productThumbPictureSize)).ToList();
		_logger.LogError("InvokeAsync(productThumbPictureSize={ProductThumbPictureSize}): model → {Model}", productThumbPictureSize, model);
		_logger.LogError("InvokeAsync(productThumbPictureSize)={ProductThumbPictureSize}: external service call took {Arg1} ms", productThumbPictureSize, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting InvokeAsync(productThumbPictureSize={ProductThumbPictureSize})", productThumbPictureSize);
        return View(model);
    }
}