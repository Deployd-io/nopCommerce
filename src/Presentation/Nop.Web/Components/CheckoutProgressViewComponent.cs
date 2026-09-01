using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Checkout;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class CheckoutProgressViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly ICheckoutModelFactory _checkoutModelFactory;

    public CheckoutProgressViewComponent(ICheckoutModelFactory checkoutModelFactory)
    {
		_logger.LogDebug(">>> Entering CheckoutProgressViewComponent(checkoutModelFactory={CheckoutModelFactory})", checkoutModelFactory);
        _checkoutModelFactory = checkoutModelFactory;
		_logger.LogDebug("CheckoutProgressViewComponent(checkoutModelFactory={CheckoutModelFactory}): _checkoutModelFactory → {_checkoutModelFactory}", checkoutModelFactory, _checkoutModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(CheckoutProgressStep step)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(step={Step})", step);
        var model = await _checkoutModelFactory.PrepareCheckoutProgressModelAsync(step);
		_logger.LogDebug("InvokeAsync(step={Step}): model → {Model}", step, model);
		_logger.LogDebug("<<< Exiting InvokeAsync(step={Step})", step);
        return View(model);
    }
}