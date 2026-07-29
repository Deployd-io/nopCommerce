using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product search model to associate to the product attribute value
/// </summary>
public partial record AssociateProductToAttributeValueSearchModel : BaseSearchModel
{
    #region Ctor

    public AssociateProductToAttributeValueSearchModel()
    {
		logger.LogDebug(">>> Entering AssociateProductToAttributeValueSearchModel()");
        AvailableCategories = new List<SelectListItem>();
        AvailableManufacturers = new List<SelectListItem>();
		logger.LogDebug("AssociateProductToAttributeValueSearchModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
		logger.LogDebug("AssociateProductToAttributeValueSearchModel(): AvailableManufacturers → {AvailableManufacturers}", AvailableManufacturers);
        AvailableStores = new List<SelectListItem>();
        AvailableVendors = new List<SelectListItem>();
		logger.LogDebug("AssociateProductToAttributeValueSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
		logger.LogDebug("AssociateProductToAttributeValueSearchModel(): AvailableVendors → {AvailableVendors}", AvailableVendors);
        AvailableProductTypes = new List<SelectListItem>();
        AssociateProductToAttributeValueModel = new AssociateProductToAttributeValueModel();
		logger.LogDebug("<<< Exiting AssociateProductToAttributeValueSearchModel()");
		logger.LogDebug("AssociateProductToAttributeValueSearchModel(): AvailableProductTypes → {AvailableProductTypes}", AvailableProductTypes);
		logger.LogDebug("AssociateProductToAttributeValueSearchModel(): AssociateProductToAttributeValueModel → {AssociateProductToAttributeValueModel}", AssociateProductToAttributeValueModel);
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

    public bool IsLoggedInAsVendor { get; set; }

    public AssociateProductToAttributeValueModel AssociateProductToAttributeValueModel { get; set; }

    #endregion
}