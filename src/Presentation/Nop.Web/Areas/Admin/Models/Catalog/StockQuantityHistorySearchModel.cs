using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a stock quantity history search model
/// </summary>
public partial record StockQuantityHistorySearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public StockQuantityHistorySearchModel()
    {
		_logger.LogDebug(">>> Entering StockQuantityHistorySearchModel()");
        AvailableWarehouses = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting StockQuantityHistorySearchModel()");
		_logger.LogDebug("StockQuantityHistorySearchModel(): AvailableWarehouses → {AvailableWarehouses}", AvailableWarehouses);
    }

    #endregion

    #region Properties

    public int ProductId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchWarehouse")]
    public int WarehouseId { get; set; }

    public IList<SelectListItem> AvailableWarehouses { get; set; }

    #endregion
}