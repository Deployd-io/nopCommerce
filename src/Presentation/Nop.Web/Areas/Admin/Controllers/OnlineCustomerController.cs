using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class OnlineCustomerController : BaseAdminController
{
    #region Fields

    protected readonly ICustomerModelFactory _customerModelFactory;
    protected readonly IPermissionService _permissionService;

    #endregion

    #region Ctor

    public OnlineCustomerController(ICustomerModelFactory customerModelFactory,
        IPermissionService permissionService)
    {
        _customerModelFactory = customerModelFactory;
        _permissionService = permissionService;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _customerModelFactory.PrepareOnlineCustomerSearchModelAsync(new OnlineCustomerSearchModel());

		logger.LogDebug("List(): model → {Model}", model);
		logger.LogDebug("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    public virtual async Task<IActionResult> List(OnlineCustomerSearchModel searchModel)
    {
        //prepare model
        var model = await _customerModelFactory.PrepareOnlineCustomerListModelAsync(searchModel);

		logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion
}
