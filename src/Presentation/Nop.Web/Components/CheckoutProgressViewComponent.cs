using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Checkout;

namespace Nop.Web.Components;

public partial class CheckoutProgressViewComponent : NopViewComponent
{
    protected readonly ICheckoutModelFactory _checkoutModelFactory;

    public CheckoutProgressViewComponent(ICheckoutModelFactory checkoutModelFactory)
    {
		logger.LogDebug(">>> Entering CheckoutProgressViewComponent(checkoutModelFactory={CheckoutModelFactory})", checkoutModelFactory);
        _checkoutModelFactory = checkoutModelFactory;
		logger.LogDebug("<<< Exiting CheckoutProgressViewComponent(checkoutModelFactory={CheckoutModelFactory})", checkoutModelFactory);
		logger.LogDebug("CheckoutProgressViewComponent(checkoutModelFactory={CheckoutModelFactory}): _checkoutModelFactory → {_checkoutModelFactory}", checkoutModelFactory, _checkoutModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(CheckoutProgressStep step)
    {
		logger.LogDebug(">>> Entering InvokeAsync(step={Step})", step);
        var model = await _checkoutModelFactory.PrepareCheckoutProgressModelAsync(step);
		logger.LogDebug("InvokeAsync(step={Step}): model → {Model}", step, model);
		logger.LogDebug("<<< Exiting InvokeAsync(step={Step})", step);
        return View(model);
    }
}