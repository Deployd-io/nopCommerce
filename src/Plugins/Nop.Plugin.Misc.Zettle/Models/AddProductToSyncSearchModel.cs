using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Zettle.Models;

/// <summary>
/// Represents a product search model to add for synchronization
/// </summary>
public record AddProductToSyncSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AddProductToSyncSearchModel()
    {
		_logger.LogDebug(">>> Entering AddProductToSyncSearchModel()");
        AvailableCategories = new List<SelectListItem>();
        AvailableManufacturers = new List<SelectListItem>();
		_logger.LogDebug("AddProductToSyncSearchModel(): AvailableManufacturers → {AvailableManufacturers}", AvailableManufacturers);
		_logger.LogDebug("AddProductToSyncSearchModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("AddProductToSyncSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
        AvailableVendors = new List<SelectListItem>();
		_logger.LogDebug("AddProductToSyncSearchModel(): AvailableVendors → {AvailableVendors}", AvailableVendors);
        AvailableProductTypes = new List<SelectListItem>();
		_logger.LogDebug("AddProductToSyncSearchModel(): AvailableProductTypes → {AvailableProductTypes}", AvailableProductTypes);
		_logger.LogDebug("<<< Exiting AddProductToSyncSearchModel()");
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