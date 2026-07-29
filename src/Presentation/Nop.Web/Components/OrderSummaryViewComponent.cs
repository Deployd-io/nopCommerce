using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Components;

public partial class OrderSummaryViewComponent : NopViewComponent
{
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
		logger.LogDebug(">>> Entering InvokeAsync(prepareAndDisplayOrderReviewData={PrepareAndDisplayOrderReviewData},overriddenModel={OverriddenModel})", prepareAndDisplayOrderReviewData, overriddenModel);
        //use already prepared (shared) model
        if (overriddenModel != null)
            return View(overriddenModel);

        //if not passed, then create a new model
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, store.Id);
		logger.LogDebug("InvokeAsync(prepareAndDisplayOrderReviewData={PrepareAndDisplayOrderReviewData},overriddenModel={OverriddenModel}): store → {Store}", prepareAndDisplayOrderReviewData, overriddenModel, store);
		logger.LogDebug("InvokeAsync(prepareAndDisplayOrderReviewData={PrepareAndDisplayOrderReviewData},overriddenModel={OverriddenModel}): cart → {Cart}", prepareAndDisplayOrderReviewData, overriddenModel, cart);

        var model = new ShoppingCartModel();
        model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart,
            isEditable: false,
            prepareAndDisplayOrderReviewData: prepareAndDisplayOrderReviewData.GetValueOrDefault());
			logger.LogDebug("InvokeAsync(prepareAndDisplayOrderReviewData={PrepareAndDisplayOrderReviewData},overriddenModel={OverriddenModel}): model → {Model}", prepareAndDisplayOrderReviewData, overriddenModel, model);
		logger.LogDebug("<<< Exiting InvokeAsync(prepareAndDisplayOrderReviewData={PrepareAndDisplayOrderReviewData},overriddenModel={OverriddenModel})", prepareAndDisplayOrderReviewData, overriddenModel);
        return View(model);
    }
}