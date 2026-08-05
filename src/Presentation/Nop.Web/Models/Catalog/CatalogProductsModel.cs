using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.UI.Paging;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represents a catalog products model
/// </summary>
public partial record CatalogProductsModel : BasePageableModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Properties

    /// <summary>
    /// Get or set a value indicating whether to use standard or AJAX products loading (applicable to 'paging', 'filtering', 'view modes') in catalog
    /// </summary>
    public bool UseAjaxLoading { get; set; }

    /// <summary>
    /// Gets or sets the warning message
    /// </summary>
    public string WarningMessage { get; set; }

    /// <summary>
    /// Gets or sets the message if there are no products to return
    /// </summary>
    public string NoResultMessage { get; set; }

    /// <summary>
    /// Gets or sets the price range filter model
    /// </summary>
    public PriceRangeFilterModel PriceRangeFilter { get; set; }

    /// <summary>
    /// Gets or sets the specification filter model
    /// </summary>
    public SpecificationFilterModel SpecificationFilter { get; set; }

    /// <summary>
    /// Gets or sets the manufacturer filter model
    /// </summary>
    public ManufacturerFilterModel ManufacturerFilter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether product sorting is allowed
    /// </summary>
    public bool AllowProductSorting { get; set; }

    /// <summary>
    /// Gets or sets available sort options
    /// </summary>
    public IList<SelectListItem> AvailableSortOptions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether customers are allowed to change view mode
    /// </summary>
    public bool AllowProductViewModeChanging { get; set; }

    /// <summary>
    /// Gets or sets available view mode options
    /// </summary>
    public IList<SelectListItem> AvailableViewModes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether customers are allowed to select page size
    /// </summary>
    public bool AllowCustomersToSelectPageSize { get; set; }

    /// <summary>
    /// Gets or sets available page size options
    /// </summary>
    public IList<SelectListItem> PageSizeOptions { get; set; }

    /// <summary>
    /// Gets or sets a order by
    /// </summary>
    public int? OrderBy { get; set; }

    /// <summary>
    /// Gets or sets a product sorting
    /// </summary>
    public string ViewMode { get; set; }

    /// <summary>
    /// Gets or sets the products
    /// </summary>
    public IList<ProductOverviewModel> Products { get; set; }

    #endregion

    #region Ctor

    public CatalogProductsModel()
    {
		_logger.LogDebug(">>> Entering CatalogProductsModel()");
        PriceRangeFilter = new PriceRangeFilterModel();
        SpecificationFilter = new SpecificationFilterModel();
		_logger.LogDebug("CatalogProductsModel(): PriceRangeFilter → {PriceRangeFilter}", PriceRangeFilter);
		_logger.LogDebug("CatalogProductsModel(): SpecificationFilter → {SpecificationFilter}", SpecificationFilter);
        ManufacturerFilter = new ManufacturerFilterModel();
        AvailableSortOptions = new List<SelectListItem>();
		_logger.LogDebug("CatalogProductsModel(): ManufacturerFilter → {ManufacturerFilter}", ManufacturerFilter);
		_logger.LogDebug("CatalogProductsModel(): AvailableSortOptions → {AvailableSortOptions}", AvailableSortOptions);
        AvailableViewModes = new List<SelectListItem>();
        PageSizeOptions = new List<SelectListItem>();
		_logger.LogDebug("CatalogProductsModel(): AvailableViewModes → {AvailableViewModes}", AvailableViewModes);
		_logger.LogDebug("CatalogProductsModel(): PageSizeOptions → {PageSizeOptions}", PageSizeOptions);
        Products = new List<ProductOverviewModel>();
		_logger.LogDebug("<<< Exiting CatalogProductsModel()");
		_logger.LogDebug("CatalogProductsModel(): Products → {Products}", Products);
    }

    #endregion
}