using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class CrossSellProductsViewComponent : NopViewComponent
{
    protected readonly IAclService _aclService;
    protected readonly IProductModelFactory _productModelFactory;
    protected readonly IProductService _productService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IWorkContext _workContext;
    protected readonly ShoppingCartSettings _shoppingCartSettings;

    public CrossSellProductsViewComponent(IAclService aclService,
        IProductModelFactory productModelFactory,
        IProductService productService,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IWorkContext workContext,
        ShoppingCartSettings shoppingCartSettings)
    {
        _aclService = aclService;
        _productModelFactory = productModelFactory;
        _productService = productService;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _workContext = workContext;
        _shoppingCartSettings = shoppingCartSettings;
    }

    public async Task<IViewComponentResult> InvokeAsync(int? productThumbPictureSize)
    {
		logger.LogDebug(">>> Entering InvokeAsync(productThumbPictureSize={ProductThumbPictureSize})", productThumbPictureSize);
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, store.Id);
		logger.LogDebug("InvokeAsync(productThumbPictureSize={ProductThumbPictureSize}): store → {Store}", productThumbPictureSize, store);
		logger.LogDebug("InvokeAsync(productThumbPictureSize={ProductThumbPictureSize}): cart → {Cart}", productThumbPictureSize, cart);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var products = await (await _productService.GetCrossSellProductsByShoppingCartAsync(cart, _shoppingCartSettings.CrossSellsNumber))
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _productService.ProductIsAvailable(p))
            //visible individually
            .Where(p => p.VisibleIndividually).ToListAsync();
			logger.LogInformation("InvokeAsync(productThumbPictureSize)={ProductThumbPictureSize}: database query executed in {Arg1} ms", productThumbPictureSize, __logStart.ElapsedMilliseconds);

        if (!products.Any())
            return Content("");

        //Cross-sell products are displayed on the shopping cart page.
        //We know that the entire shopping cart page is not refresh
        //even if "ShoppingCartSettings.DisplayCartAfterAddingProduct" setting  is enabled.
        //That's why we force page refresh (redirect) in this case
        var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(products,
                productThumbPictureSize: productThumbPictureSize, forceRedirectionAfterAddingToCart: true))
            .ToList();

		logger.LogDebug("<<< Exiting InvokeAsync(productThumbPictureSize={ProductThumbPictureSize})", productThumbPictureSize);
        return View(model);
    }
}