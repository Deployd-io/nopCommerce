using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class SelectedCheckoutAttributesViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;

    public SelectedCheckoutAttributesViewComponent(IShoppingCartModelFactory shoppingCartModelFactory)
    {
		_logger.LogDebug(">>> Entering SelectedCheckoutAttributesViewComponent(shoppingCartModelFactory={ShoppingCartModelFactory})", shoppingCartModelFactory);
        _shoppingCartModelFactory = shoppingCartModelFactory;
		_logger.LogDebug("<<< Exiting SelectedCheckoutAttributesViewComponent(shoppingCartModelFactory={ShoppingCartModelFactory})", shoppingCartModelFactory);
		_logger.LogDebug("SelectedCheckoutAttributesViewComponent(shoppingCartModelFactory={ShoppingCartModelFactory}): _shoppingCartModelFactory → {_shoppingCartModelFactory}", shoppingCartModelFactory, _shoppingCartModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogDebug(">>> Entering InvokeAsync()");
        var attributes = await _shoppingCartModelFactory.FormatSelectedCheckoutAttributesAsync();
		_logger.LogDebug("InvokeAsync(): attributes → {Attributes}", attributes);
		_logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(null, attributes);
    }
}