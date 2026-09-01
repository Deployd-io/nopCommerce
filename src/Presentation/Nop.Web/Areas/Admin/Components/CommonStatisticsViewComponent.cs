using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents a view component that displays common statistics
/// </summary>
public partial class CommonStatisticsViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICommonModelFactory _commonModelFactory;
    protected readonly IPermissionService _permissionService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public CommonStatisticsViewComponent(ICommonModelFactory commonModelFactory,
        IPermissionService permissionService,
        IWorkContext workContext)
    {
		_logger.LogDebug(">>> Entering CommonStatisticsViewComponent(commonModelFactory={CommonModelFactory},permissionService={PermissionService},workContext={WorkContext})", commonModelFactory, permissionService, workContext);
        _commonModelFactory = commonModelFactory;
		_logger.LogDebug("CommonStatisticsViewComponent(commonModelFactory={CommonModelFactory},permissionService={PermissionService},workContext={WorkContext}): _commonModelFactory → {_commonModelFactory}", commonModelFactory, permissionService, workContext, _commonModelFactory);
        _permissionService = permissionService;
		_logger.LogDebug("CommonStatisticsViewComponent(commonModelFactory={CommonModelFactory},permissionService={PermissionService},workContext={WorkContext}): _permissionService → {_permissionService}", commonModelFactory, permissionService, workContext, _permissionService);
        _workContext = workContext;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogDebug(">>> Entering InvokeAsync()");
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Customers.CUSTOMERS_VIEW) ||
            !await _permissionService.AuthorizeAsync(StandardPermission.Orders.ORDERS_VIEW) ||
            !await _permissionService.AuthorizeAsync(StandardPermission.Orders.RETURN_REQUESTS_VIEW) ||
            !await _permissionService.AuthorizeAsync(StandardPermission.Catalog.PRODUCTS_VIEW))
        {
            return Content(string.Empty);
        }

        //a vendor doesn't have access to this report
        if (await _workContext.GetCurrentVendorAsync() != null)
            return Content(string.Empty);

        //prepare model
        var model = await _commonModelFactory.PrepareCommonStatisticsModelAsync();
		_logger.LogDebug("InvokeAsync(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(model);
    }

    #endregion
}