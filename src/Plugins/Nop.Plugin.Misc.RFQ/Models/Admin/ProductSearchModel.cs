using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.RFQ.Models.Admin;

/// <summary>
/// Represents a product search model to add to the quote
/// </summary>
public record ProductSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ProductSearchModel()
    {
		_logger.LogDebug(">>> Entering ProductSearchModel()");
        AvailableCategories = new List<SelectListItem>();
        AvailableManufacturers = new List<SelectListItem>();
		_logger.LogDebug("ProductSearchModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
		_logger.LogDebug("ProductSearchModel(): AvailableManufacturers → {AvailableManufacturers}", AvailableManufacturers);
        AvailableProductTypes = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting ProductSearchModel()");
		_logger.LogDebug("ProductSearchModel(): AvailableProductTypes → {AvailableProductTypes}", AvailableProductTypes);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
    public string SearchProductName { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
    public int SearchCategoryId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
    public int SearchManufacturerId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
    public int SearchProductTypeId { get; set; }

    public IList<SelectListItem> AvailableCategories { get; set; }

    public IList<SelectListItem> AvailableManufacturers { get; set; }

    public IList<SelectListItem> AvailableProductTypes { get; set; }

    public int EntityId { get; set; }

    #endregion
}