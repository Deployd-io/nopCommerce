using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.ShoppingCart;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class OrderSummaryViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;

    public OrderSummaryViewComponent(IShoppingCartModelFactory shoppingCartModelFactory,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IWorkContext workContext)
    {
        _shoppingCartModelFactory = shoppingCartModelFactory;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _workContext = workContext;
    }

    public async Task<IViewComponentResult> InvokeAsync(bool? prepareAndDisplayOrderReviewData, ShoppingCartModel overriddenModel)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(prepareAndDisplayOrderReviewData={PrepareAndDisplayOrderReviewData},overriddenModel={OverriddenModel})", prepareAndDisplayOrderReviewData, overriddenModel);
        //use already prepared (shared) model
        if (overriddenModel != null)
            return View(overriddenModel);

        //if not passed, then create a new model
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("InvokeAsync(prepareAndDisplayOrderReviewData={PrepareAndDisplayOrderReviewData},overriddenModel={OverriddenModel}): store → {Store}", prepareAndDisplayOrderReviewData, overriddenModel, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogDebug("InvokeAsync(prepareAndDisplayOrderReviewData={PrepareAndDisplayOrderReviewData},overriddenModel={OverriddenModel}): cart → {Cart}", prepareAndDisplayOrderReviewData, overriddenModel, cart);

        var model = new ShoppingCartModel();
		_logger.LogDebug("InvokeAsync(prepareAndDisplayOrderReviewData={PrepareAndDisplayOrderReviewData},overriddenModel={OverriddenModel}): model → {Model}", prepareAndDisplayOrderReviewData, overriddenModel, model);
        model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart,
            isEditable: false,
            prepareAndDisplayOrderReviewData: prepareAndDisplayOrderReviewData.GetValueOrDefault());
		_logger.LogDebug("<<< Exiting InvokeAsync(prepareAndDisplayOrderReviewData={PrepareAndDisplayOrderReviewData},overriddenModel={OverriddenModel})", prepareAndDisplayOrderReviewData, overriddenModel);
        return View(model);
    }
}