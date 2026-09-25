using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.ShoppingCart;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ShoppingCartController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IPermissionService _permissionService;
    protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;
    protected readonly IShoppingCartService _shoppingCartService;
    #endregion

    #region Ctor

    public ShoppingCartController(ICustomerService customerService,
        IPermissionService permissionService,
        IShoppingCartService shoppingCartService,
        IShoppingCartModelFactory shoppingCartModelFactory)
    {
		_logger.LogError(">>> Entering ShoppingCartController(customerService={CustomerService},permissionService={PermissionService},shoppingCartService=<redacted>,shoppingCartModelFactory=<redacted>)", customerService, permissionService);
        _customerService = customerService;
        _permissionService = permissionService;
		_logger.LogDebug("ShoppingCartController(customerService={CustomerService},permissionService={PermissionService},shoppingCartService=<redacted>,shoppingCartModelFactory=<redacted>): _permissionService → {_permissionService}", customerService, permissionService, _permissionService);
        _shoppingCartModelFactory = shoppingCartModelFactory;
		_logger.LogDebug("ShoppingCartController(customerService={CustomerService},permissionService={PermissionService},shoppingCartService=<redacted>,shoppingCartModelFactory=<redacted>): _shoppingCartModelFactory → {_shoppingCartModelFactory}", customerService, permissionService, _shoppingCartModelFactory);
        _shoppingCartService = shoppingCartService;
		_logger.LogDebug("ShoppingCartController(customerService={CustomerService},permissionService={PermissionService},shoppingCartService=<redacted>,shoppingCartModelFactory=<redacted>): _shoppingCartService → {_shoppingCartService}", customerService, permissionService, _shoppingCartService);
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Orders.CURRENT_CARTS_MANAGE)]
    public virtual async Task<IActionResult> CurrentCarts()
    {
		_logger.LogError(">>> Entering CurrentCarts()");
        //prepare model
        var model = await _shoppingCartModelFactory.PrepareShoppingCartSearchModelAsync(new ShoppingCartSearchModel());
		_logger.LogError("CurrentCarts(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting CurrentCarts()");
		_logger.LogWarning("<<< Exiting CurrentCarts(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.CURRENT_CARTS_MANAGE)]
    public virtual async Task<IActionResult> CurrentCarts(ShoppingCartSearchModel searchModel)
    {
        //prepare model
        var model = await _shoppingCartModelFactory.PrepareShoppingCartListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.CURRENT_CARTS_MANAGE)]
    public virtual async Task<IActionResult> GetCartDetails(ShoppingCartItemSearchModel searchModel)
    {
		_logger.LogError(">>> Entering GetCartDetails(searchModel={SearchModel})", searchModel);
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(searchModel.CustomerId)
                       ?? throw new ArgumentException("No customer found with the specified id");

        //prepare model
        var model = await _shoppingCartModelFactory.PrepareShoppingCartItemListModelAsync(searchModel, customer);
		_logger.LogError("GetCartDetails(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting GetCartDetails(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.CURRENT_CARTS_MANAGE)]
    public virtual async Task<IActionResult> DeleteItem(int id)
    {
		_logger.LogError(">>> Entering DeleteItem(id={Id})", id);
        await _shoppingCartService.DeleteShoppingCartItemAsync(id);

		_logger.LogWarning("<<< Exiting DeleteItem(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion
}