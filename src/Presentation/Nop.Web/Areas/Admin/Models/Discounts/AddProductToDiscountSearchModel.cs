using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Discounts;

/// <summary>
/// Represents a product search model to add to the discount
/// </summary>
public partial record AddProductToDiscountSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AddProductToDiscountSearchModel()
    {
		_logger.LogDebug(">>> Entering AddProductToDiscountSearchModel()");
        AvailableCategories = new List<SelectListItem>();
        AvailableManufacturers = new List<SelectListItem>();
		_logger.LogDebug("AddProductToDiscountSearchModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
		_logger.LogDebug("AddProductToDiscountSearchModel(): AvailableManufacturers → {AvailableManufacturers}", AvailableManufacturers);
        AvailableStores = new List<SelectListItem>();
        AvailableVendors = new List<SelectListItem>();
		_logger.LogDebug("AddProductToDiscountSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
		_logger.LogDebug("AddProductToDiscountSearchModel(): AvailableVendors → {AvailableVendors}", AvailableVendors);
        AvailableProductTypes = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting AddProductToDiscountSearchModel()");
		_logger.LogDebug("AddProductToDiscountSearchModel(): AvailableProductTypes → {AvailableProductTypes}", AvailableProductTypes);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
    public string SearchProductName { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
    public int SearchCategoryId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
    public int SearchManufacturerId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchStore")]
    public int SearchStoreId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchVendor")]
    public int SearchVendorId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
    public int SearchProductTypeId { get; set; }

    public bool IsLoggedInAsVendor { get; set; }

    public IList<SelectListItem> AvailableCategories { get; set; }

    public IList<SelectListItem> AvailableManufacturers { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    public IList<SelectListItem> AvailableVendors { get; set; }

    public IList<SelectListItem> AvailableProductTypes { get; set; }

    #endregion
}