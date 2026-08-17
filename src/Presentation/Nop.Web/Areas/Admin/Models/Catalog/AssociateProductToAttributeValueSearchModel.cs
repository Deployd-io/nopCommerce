using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product search model to associate to the product attribute value
/// </summary>
public partial record AssociateProductToAttributeValueSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AssociateProductToAttributeValueSearchModel()
    {
		_logger.LogDebug(">>> Entering AssociateProductToAttributeValueSearchModel()");
        AvailableCategories = new List<SelectListItem>();
        AvailableManufacturers = new List<SelectListItem>();
		_logger.LogDebug("AssociateProductToAttributeValueSearchModel(): AvailableManufacturers → {AvailableManufacturers}", AvailableManufacturers);
		_logger.LogDebug("AssociateProductToAttributeValueSearchModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("AssociateProductToAttributeValueSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
        AvailableVendors = new List<SelectListItem>();
		_logger.LogDebug("AssociateProductToAttributeValueSearchModel(): AvailableVendors → {AvailableVendors}", AvailableVendors);
        AvailableProductTypes = new List<SelectListItem>();
		_logger.LogDebug("AssociateProductToAttributeValueSearchModel(): AvailableProductTypes → {AvailableProductTypes}", AvailableProductTypes);
        AssociateProductToAttributeValueModel = new AssociateProductToAttributeValueModel();
		_logger.LogDebug("<<< Exiting AssociateProductToAttributeValueSearchModel()");
		_logger.LogDebug("AssociateProductToAttributeValueSearchModel(): AssociateProductToAttributeValueModel → {AssociateProductToAttributeValueModel}", AssociateProductToAttributeValueModel);
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