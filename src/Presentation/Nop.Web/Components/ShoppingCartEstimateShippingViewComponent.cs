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
        _shoppingCartModelFactory = shoppingCartModelFactory;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _workContext = workContext;
        _shippingSettings = shippingSettings;
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