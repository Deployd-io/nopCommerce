using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Tax.Avalara.Models.ItemClassification;

/// <summary>
/// Represents a product search model to add for classification
/// </summary>
public record AddProductToClassificationSearchModel : BaseSearchModel
{
    #region Ctor

    public AddProductToClassificationSearchModel()
    {
		logger.LogDebug(">>> Entering AddProductToClassificationSearchModel()");
        AvailableCategories = new List<SelectListItem>();
        AvailableManufacturers = new List<SelectListItem>();
		logger.LogDebug("AddProductToClassificationSearchModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
		logger.LogDebug("AddProductToClassificationSearchModel(): AvailableManufacturers → {AvailableManufacturers}", AvailableManufacturers);
        AvailableStores = new List<SelectListItem>();
        AvailableVendors = new List<SelectListItem>();
		logger.LogDebug("AddProductToClassificationSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
		logger.LogDebug("AddProductToClassificationSearchModel(): AvailableVendors → {AvailableVendors}", AvailableVendors);
        AvailableProductTypes = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting AddProductToClassificationSearchModel()");
		logger.LogDebug("AddProductToClassificationSearchModel(): AvailableProductTypes → {AvailableProductTypes}", AvailableProductTypes);
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