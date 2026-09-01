using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class OrderTotalsViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;

    public OrderTotalsViewComponent(IShoppingCartModelFactory shoppingCartModelFactory,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IWorkContext workContext)
    {
		_logger.LogDebug(">>> Entering OrderTotalsViewComponent(shoppingCartModelFactory=<redacted>,shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext})", storeContext, workContext);
        _shoppingCartModelFactory = shoppingCartModelFactory;
		_logger.LogDebug("OrderTotalsViewComponent(shoppingCartModelFactory=<redacted>,shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext}): _shoppingCartModelFactory → {_shoppingCartModelFactory}", storeContext, workContext, _shoppingCartModelFactory);
        _shoppingCartService = shoppingCartService;
		_logger.LogDebug("OrderTotalsViewComponent(shoppingCartModelFactory=<redacted>,shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext}): _shoppingCartService → {_shoppingCartService}", storeContext, workContext, _shoppingCartService);
        _storeContext = storeContext;
		_logger.LogDebug("OrderTotalsViewComponent(shoppingCartModelFactory=<redacted>,shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext}): _storeContext → {_storeContext}", storeContext, workContext, _storeContext);
        _workContext = workContext;
    }

    public async Task<IViewComponentResult> InvokeAsync(bool isEditable)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(isEditable={IsEditable})", isEditable);
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogDebug("InvokeAsync(isEditable={IsEditable}): cart → {Cart}", isEditable, cart);
		_logger.LogDebug("InvokeAsync(isEditable={IsEditable}): store → {Store}", isEditable, store);

        var model = await _shoppingCartModelFactory.PrepareOrderTotalsModelAsync(cart, isEditable);
		_logger.LogDebug("InvokeAsync(isEditable={IsEditable}): model → {Model}", isEditable, model);
		_logger.LogDebug("<<< Exiting InvokeAsync(isEditable={IsEditable})", isEditable);
        return View(model);
    }
}