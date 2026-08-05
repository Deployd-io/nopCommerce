using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents a customer role product search model
/// </summary>
public partial record CustomerRoleProductSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CustomerRoleProductSearchModel()
    {
		_logger.LogDebug(">>> Entering CustomerRoleProductSearchModel()");
        AvailableCategories = new List<SelectListItem>();
        AvailableManufacturers = new List<SelectListItem>();
		_logger.LogDebug("CustomerRoleProductSearchModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
		_logger.LogDebug("CustomerRoleProductSearchModel(): AvailableManufacturers → {AvailableManufacturers}", AvailableManufacturers);
        AvailableStores = new List<SelectListItem>();
        AvailableVendors = new List<SelectListItem>();
		_logger.LogDebug("CustomerRoleProductSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
		_logger.LogDebug("CustomerRoleProductSearchModel(): AvailableVendors → {AvailableVendors}", AvailableVendors);
        AvailableProductTypes = new List<SelectListItem>();
        AddProductToCustomerRoleModel = new AddProductToCustomerRoleModel();
		_logger.LogDebug("<<< Exiting CustomerRoleProductSearchModel()");
		_logger.LogDebug("CustomerRoleProductSearchModel(): AvailableProductTypes → {AvailableProductTypes}", AvailableProductTypes);
		_logger.LogDebug("CustomerRoleProductSearchModel(): AddProductToCustomerRoleModel → {AddProductToCustomerRoleModel}", AddProductToCustomerRoleModel);
    }

    #endregion

    #region Properties

    public AddProductToCustomerRoleModel AddProductToCustomerRoleModel { get; set; }

    public bool IsLoggedInAsVendor { get; set; }

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