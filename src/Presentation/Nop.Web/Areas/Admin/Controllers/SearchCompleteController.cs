using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class SearchCompleteController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IPermissionService _permissionService;
    protected readonly IProductService _productService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public SearchCompleteController(
        IPermissionService permissionService,
        IProductService productService,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering SearchCompleteController(permissionService={PermissionService},productService={ProductService},workContext={WorkContext})", permissionService, productService, workContext);
        _permissionService = permissionService;
        _productService = productService;
		_logger.LogDebug("SearchCompleteController(permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _productService → {_productService}", permissionService, productService, workContext, _productService);
		_logger.LogDebug("SearchCompleteController(permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _permissionService → {_permissionService}", permissionService, productService, workContext, _permissionService);
    _logger.LogWarning("<<< Exiting SearchCompleteController(permissionService={PermissionService},productService={ProductService},workContext={WorkContext})", permissionService, productService, workContext);
        _workContext = workContext;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> SearchAutoComplete(string term)
    {
		_logger.LogError(">>> Entering SearchAutoComplete(term={Term})", term);
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL))
            return Content(string.Empty);

        const int searchTermMinimumLength = 3;
        if (string.IsNullOrWhiteSpace(term) || term.Length < searchTermMinimumLength)
            return Content(string.Empty);

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogError("SearchAutoComplete(term={Term}): currentVendor → {CurrentVendor}", term, currentVendor);
        var vendorId = 0;
		_logger.LogError("SearchAutoComplete(term={Term}): vendorId → {VendorId}", term, vendorId);
        if (currentVendor != null)
        {
            vendorId = currentVendor.Id;
        }

        //products
        const int productNumber = 15;
        var products = await _productService.SearchProductsAsync(0,
            vendorId: vendorId,
            keywords: term,
            pageSize: productNumber,
            showHidden: true);

        var result = (from p in products
            select new
            {
                label = p.Name,
                productid = p.Id
            }).ToList();
			_logger.LogDebug("SearchAutoComplete(term={Term}): productid → {Productid}", term, productid);

		_logger.LogWarning("<<< Exiting SearchAutoComplete(term={Term})", term);
        return Json(result);
    }

    #endregion
}