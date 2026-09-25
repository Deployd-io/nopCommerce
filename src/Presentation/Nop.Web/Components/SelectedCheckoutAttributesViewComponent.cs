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
		_logger.LogError(">>> Entering SelectedCheckoutAttributesViewComponent(shoppingCartModelFactory=<redacted>)");
        _shoppingCartModelFactory = shoppingCartModelFactory;
		_logger.LogDebug("SelectedCheckoutAttributesViewComponent(shoppingCartModelFactory=<redacted>): _shoppingCartModelFactory → {_shoppingCartModelFactory}", _shoppingCartModelFactory);
    _logger.LogWarning("<<< Exiting SelectedCheckoutAttributesViewComponent(shoppingCartModelFactory=<redacted>)");
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogError(">>> Entering InvokeAsync()");
        var attributes = await _shoppingCartModelFactory.FormatSelectedCheckoutAttributesAsync();
		_logger.LogError("InvokeAsync(): attributes → {Attributes}", attributes);
		_logger.LogWarning("<<< Exiting InvokeAsync()");
        return View(null, attributes);
    }
}