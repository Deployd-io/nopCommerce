using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product search model
/// </summary>
public partial record ProductSearchModel : BaseSearchModel
{
    #region Ctor

    public ProductSearchModel()
    {
		logger.LogDebug(">>> Entering ProductSearchModel()");
        AvailableCategories = new List<SelectListItem>();
        AvailableManufacturers = new List<SelectListItem>();
		logger.LogDebug("ProductSearchModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
		logger.LogDebug("ProductSearchModel(): AvailableManufacturers → {AvailableManufacturers}", AvailableManufacturers);
        AvailableStores = new List<SelectListItem>();
        AvailableWarehouses = new List<SelectListItem>();
		logger.LogDebug("ProductSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
		logger.LogDebug("ProductSearchModel(): AvailableWarehouses → {AvailableWarehouses}", AvailableWarehouses);
        AvailableVendors = new List<SelectListItem>();
        AvailableProductTypes = new List<SelectListItem>();
		logger.LogDebug("ProductSearchModel(): AvailableVendors → {AvailableVendors}", AvailableVendors);
		logger.LogDebug("ProductSearchModel(): AvailableProductTypes → {AvailableProductTypes}", AvailableProductTypes);
        AvailablePublishedOptions = new List<SelectListItem>();
        LicenseCheckModel = new();
		logger.LogDebug("<<< Exiting ProductSearchModel()");
		logger.LogDebug("ProductSearchModel(): AvailablePublishedOptions → {AvailablePublishedOptions}", AvailablePublishedOptions);
		logger.LogDebug("ProductSearchModel(): LicenseCheckModel → {LicenseCheckModel}", LicenseCheckModel);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
    public string SearchProductName { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
    public int SearchCategoryId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchIncludeSubCategories")]
    public bool SearchIncludeSubCategories { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
    public int SearchManufacturerId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchStore")]
    public int SearchStoreId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchVendor")]
    public int SearchVendorId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchWarehouse")]
    public int SearchWarehouseId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
    public int SearchProductTypeId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchPublished")]
    public int SearchPublishedId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.GoDirectlyToSku")]
    public string GoDirectlyToSku { get; set; }

    public bool IsLoggedInAsVendor { get; set; }

    public bool AllowVendorsToImportProducts { get; set; }

    public LicenseCheckModel LicenseCheckModel { get; set; }

    public bool HideStoresList { get; set; }

    public IList<SelectListItem> AvailableCategories { get; set; }

    public IList<SelectListItem> AvailableManufacturers { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    public IList<SelectListItem> AvailableWarehouses { get; set; }

    public IList<SelectListItem> AvailableVendors { get; set; }

    public IList<SelectListItem> AvailableProductTypes { get; set; }

    public IList<SelectListItem> AvailablePublishedOptions { get; set; }

    #endregion
}