using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Services.Security;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class FlyoutShoppingCartViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IPermissionService _permissionService;
    protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;
    protected readonly ShoppingCartSettings _shoppingCartSettings;

    public FlyoutShoppingCartViewComponent(IPermissionService permissionService,
        IShoppingCartModelFactory shoppingCartModelFactory,
        ShoppingCartSettings shoppingCartSettings)
    {
		_logger.LogError(">>> Entering FlyoutShoppingCartViewComponent(permissionService={PermissionService},shoppingCartModelFactory=<redacted>,shoppingCartSettings=<redacted>)", permissionService);
        _permissionService = permissionService;
        _shoppingCartModelFactory = shoppingCartModelFactory;
		_logger.LogDebug("FlyoutShoppingCartViewComponent(permissionService={PermissionService},shoppingCartModelFactory=<redacted>,shoppingCartSettings=<redacted>): _shoppingCartModelFactory → {_shoppingCartModelFactory}", permissionService, _shoppingCartModelFactory);
		_logger.LogDebug("FlyoutShoppingCartViewComponent(permissionService={PermissionService},shoppingCartModelFactory=<redacted>,shoppingCartSettings=<redacted>): _permissionService → {_permissionService}", permissionService, _permissionService);
    _logger.LogWarning("<<< Exiting FlyoutShoppingCartViewComponent(permissionService={PermissionService},shoppingCartModelFactory=<redacted>,shoppingCartSettings=<redacted>)", permissionService);
        _shoppingCartSettings = shoppingCartSettings;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogError(">>> Entering InvokeAsync()");
        if (!_shoppingCartSettings.MiniShoppingCartEnabled)
            return Content("");

        if (!await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART))
            return Content("");

        var model = await _shoppingCartModelFactory.PrepareMiniShoppingCartModelAsync();
		_logger.LogError("InvokeAsync(): model → {Model}", model);
		_logger.LogWarning("<<< Exiting InvokeAsync()");
        return View(model);
    }
}