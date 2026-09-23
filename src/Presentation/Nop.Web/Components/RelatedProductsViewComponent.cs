using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class RelatedProductsViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IAclService _aclService;
    protected readonly IProductModelFactory _productModelFactory;
    protected readonly IProductService _productService;
    protected readonly IStoreMappingService _storeMappingService;

    public RelatedProductsViewComponent(IAclService aclService,
        IProductModelFactory productModelFactory,
        IProductService productService,
        IStoreMappingService storeMappingService)
    {
		_logger.LogError(">>> Entering RelatedProductsViewComponent(aclService={AclService},productModelFactory={ProductModelFactory},productService={ProductService},storeMappingService=<redacted>)", aclService, productModelFactory, productService);
        _aclService = aclService;
        _productModelFactory = productModelFactory;
		_logger.LogDebug("RelatedProductsViewComponent(aclService={AclService},productModelFactory={ProductModelFactory},productService={ProductService},storeMappingService=<redacted>): _productModelFactory → {_productModelFactory}", aclService, productModelFactory, productService, _productModelFactory);
		_logger.LogDebug("RelatedProductsViewComponent(aclService={AclService},productModelFactory={ProductModelFactory},productService={ProductService},storeMappingService=<redacted>): _aclService → {_aclService}", aclService, productModelFactory, productService, _aclService);
    _logger.LogWarning("<<< Exiting RelatedProductsViewComponent(aclService={AclService},productModelFactory={ProductModelFactory},productService={ProductService},storeMappingService=<redacted>)", aclService, productModelFactory, productService);
        _productService = productService;
		_logger.LogDebug("RelatedProductsViewComponent(aclService={AclService},productModelFactory={ProductModelFactory},productService={ProductService},storeMappingService=<redacted>): _productService → {_productService}", aclService, productModelFactory, productService, _productService);
        _storeMappingService = storeMappingService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int productId, int? productThumbPictureSize)
    {
		_logger.LogError(">>> Entering InvokeAsync(productId={ProductId},productThumbPictureSize={ProductThumbPictureSize})", productId, productThumbPictureSize);
        //load and cache report
        var productIds = (await _productService.GetRelatedProductsByProductId1Async(productId)).Select(x => x.ProductId2).ToArray();
		_logger.LogError("InvokeAsync(productId={ProductId},productThumbPictureSize={ProductThumbPictureSize}): productIds → {ProductIds}", productId, productThumbPictureSize, productIds);

        //load products
        var products = await (await _productService.GetProductsByIdsAsync(productIds))
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _productService.ProductIsAvailable(p))
            //visible individually
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            .Where(p => p.VisibleIndividually).ToListAsync();
			_logger.LogError("InvokeAsync(productId,productThumbPictureSize)={ProductId},{ProductThumbPictureSize}: database query executed in {Arg2} ms", productId, productThumbPictureSize, __logStart.ElapsedMilliseconds);

        if (!products.Any())
            return Content(string.Empty);

        var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, true, true, productThumbPictureSize)).ToList();
		_logger.LogError("InvokeAsync(productId={ProductId},productThumbPictureSize={ProductThumbPictureSize}): model → {Model}", productId, productThumbPictureSize, model);
		_logger.LogWarning("<<< Exiting InvokeAsync(productId={ProductId},productThumbPictureSize={ProductThumbPictureSize})", productId, productThumbPictureSize);
        return View(model);
    }
}