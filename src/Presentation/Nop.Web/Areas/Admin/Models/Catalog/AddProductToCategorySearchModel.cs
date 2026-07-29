using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product search model to add to the category
/// </summary>
public partial record AddProductToCategorySearchModel : BaseSearchModel
{
    #region Ctor

    public AddProductToCategorySearchModel()
    {
		logger.LogDebug(">>> Entering AddProductToCategorySearchModel()");
        AvailableCategories = new List<SelectListItem>();
        AvailableManufacturers = new List<SelectListItem>();
		logger.LogDebug("AddProductToCategorySearchModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
		logger.LogDebug("AddProductToCategorySearchModel(): AvailableManufacturers → {AvailableManufacturers}", AvailableManufacturers);
        AvailableStores = new List<SelectListItem>();
        AvailableVendors = new List<SelectListItem>();
		logger.LogDebug("AddProductToCategorySearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
		logger.LogDebug("AddProductToCategorySearchModel(): AvailableVendors → {AvailableVendors}", AvailableVendors);
        AvailableProductTypes = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting AddProductToCategorySearchModel()");
		logger.LogDebug("AddProductToCategorySearchModel(): AvailableProductTypes → {AvailableProductTypes}", AvailableProductTypes);
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

    public IList<SelectListItem> AvailableCategories { get; set; }

    public IList<SelectListItem> AvailableManufacturers { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    public IList<SelectListItem> AvailableVendors { get; set; }

    public IList<SelectListItem> AvailableProductTypes { get; set; }

    #endregion
}