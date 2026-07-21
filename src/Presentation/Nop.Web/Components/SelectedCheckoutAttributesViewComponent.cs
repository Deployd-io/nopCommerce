using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class SelectedCheckoutAttributesViewComponent : NopViewComponent
{
    protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;

    public SelectedCheckoutAttributesViewComponent(IShoppingCartModelFactory shoppingCartModelFactory)
    {
		logger.LogDebug(">>> Entering SelectedCheckoutAttributesViewComponent(shoppingCartModelFactory={ShoppingCartModelFactory})", shoppingCartModelFactory);
        _shoppingCartModelFactory = shoppingCartModelFactory;
		logger.LogDebug("<<< Exiting SelectedCheckoutAttributesViewComponent(shoppingCartModelFactory={ShoppingCartModelFactory})", shoppingCartModelFactory);
		logger.LogDebug("SelectedCheckoutAttributesViewComponent(shoppingCartModelFactory={ShoppingCartModelFactory}): _shoppingCartModelFactory → {_shoppingCartModelFactory}", shoppingCartModelFactory, _shoppingCartModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		logger.LogDebug(">>> Entering InvokeAsync()");
        var attributes = await _shoppingCartModelFactory.FormatSelectedCheckoutAttributesAsync();
		logger.LogDebug("InvokeAsync(): attributes → {Attributes}", attributes);
		logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(null, attributes);
    }
}
