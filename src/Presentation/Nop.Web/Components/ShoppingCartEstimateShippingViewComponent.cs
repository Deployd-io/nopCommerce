using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

/// <summary>
/// Represents a estimate shipping view component on shopping cart page
/// </summary>
public partial class ShoppingCartEstimateShippingViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;
    protected readonly ShippingSettings _shippingSettings;

    public ShoppingCartEstimateShippingViewComponent(IShoppingCartModelFactory shoppingCartModelFactory,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IWorkContext workContext,
        ShippingSettings shippingSettings)
    {
		_logger.LogDebug(">>> Entering ShoppingCartEstimateShippingViewComponent(shoppingCartModelFactory=<redacted>,shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},shippingSettings=<redacted>)", storeContext, workContext);
        _shoppingCartModelFactory = shoppingCartModelFactory;
		_logger.LogDebug("ShoppingCartEstimateShippingViewComponent(shoppingCartModelFactory=<redacted>,shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},shippingSettings=<redacted>): _shoppingCartModelFactory → {_shoppingCartModelFactory}", storeContext, workContext, _shoppingCartModelFactory);
        _shoppingCartService = shoppingCartService;
		_logger.LogDebug("ShoppingCartEstimateShippingViewComponent(shoppingCartModelFactory=<redacted>,shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},shippingSettings=<redacted>): _shoppingCartService → {_shoppingCartService}", storeContext, workContext, _shoppingCartService);
        _storeContext = storeContext;
		_logger.LogDebug("ShoppingCartEstimateShippingViewComponent(shoppingCartModelFactory=<redacted>,shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},shippingSettings=<redacted>): _storeContext → {_storeContext}", storeContext, workContext, _storeContext);
        _workContext = workContext;
		_logger.LogDebug("ShoppingCartEstimateShippingViewComponent(shoppingCartModelFactory=<redacted>,shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},shippingSettings=<redacted>): _workContext → {_workContext}", storeContext, workContext, _workContext);
        _shippingSettings = shippingSettings;
		_logger.LogDebug("ShoppingCartEstimateShippingViewComponent(shoppingCartModelFactory=<redacted>,shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},shippingSettings=<redacted>): _shippingSettings → {_shippingSettings}", storeContext, workContext, _shippingSettings);
		_logger.LogDebug("<<< Exiting ShoppingCartEstimateShippingViewComponent(shoppingCartModelFactory=<redacted>,shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},shippingSettings=<redacted>)", storeContext, workContext);
    }

    public async Task<IViewComponentResult> InvokeAsync(bool? prepareAndDisplayOrderReviewData)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(prepareAndDisplayOrderReviewData={PrepareAndDisplayOrderReviewData})", prepareAndDisplayOrderReviewData);
        if (!_shippingSettings.EstimateShippingCartPageEnabled)
            return Content(string.Empty);

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("InvokeAsync(prepareAndDisplayOrderReviewData={PrepareAndDisplayOrderReviewData}): store → {Store}", prepareAndDisplayOrderReviewData, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogDebug("InvokeAsync(prepareAndDisplayOrderReviewData={PrepareAndDisplayOrderReviewData}): cart → {Cart}", prepareAndDisplayOrderReviewData, cart);

        var model = await _shoppingCartModelFactory.PrepareEstimateShippingModelAsync(cart);
		_logger.LogDebug("InvokeAsync(prepareAndDisplayOrderReviewData={PrepareAndDisplayOrderReviewData}): model → {Model}", prepareAndDisplayOrderReviewData, model);
        if (!model.Enabled)
		_logger.LogDebug("<<< Exiting InvokeAsync(prepareAndDisplayOrderReviewData={PrepareAndDisplayOrderReviewData})", prepareAndDisplayOrderReviewData);
            return Content(string.Empty);

        return View(model);
    }
}