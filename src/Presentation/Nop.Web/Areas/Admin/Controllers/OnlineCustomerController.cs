using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class OnlineCustomerController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerModelFactory _customerModelFactory;
    protected readonly IPermissionService _permissionService;

    #endregion

    #region Ctor

    public OnlineCustomerController(ICustomerModelFactory customerModelFactory,
        IPermissionService permissionService)
    {
		_logger.LogError(">>> Entering OnlineCustomerController(customerModelFactory={CustomerModelFactory},permissionService={PermissionService})", customerModelFactory, permissionService);
        _customerModelFactory = customerModelFactory;
        _permissionService = permissionService;
		_logger.LogDebug("OnlineCustomerController(customerModelFactory={CustomerModelFactory},permissionService={PermissionService}): _customerModelFactory → {_customerModelFactory}", customerModelFactory, permissionService, _customerModelFactory);
    _logger.LogWarning("<<< Exiting OnlineCustomerController(customerModelFactory={CustomerModelFactory},permissionService={PermissionService})", customerModelFactory, permissionService);
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogError(">>> Entering List()");
        //prepare model
        var model = await _customerModelFactory.PrepareOnlineCustomerSearchModelAsync(new OnlineCustomerSearchModel());
		_logger.LogError("List(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting List()");
		_logger.LogWarning("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    public virtual async Task<IActionResult> List(OnlineCustomerSearchModel searchModel)
    {
        //prepare model
        var model = await _customerModelFactory.PrepareOnlineCustomerListModelAsync(searchModel);

        return Json(model);
    }

    #endregion
}