using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a low stock product search model
/// </summary>
public partial record LowStockProductSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public LowStockProductSearchModel()
    {
		_logger.LogDebug(">>> Entering LowStockProductSearchModel()");
        AvailablePublishedOptions = new List<SelectListItem>();
		_logger.LogDebug("LowStockProductSearchModel(): AvailablePublishedOptions → {AvailablePublishedOptions}", AvailablePublishedOptions);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Reports.LowStock.SearchPublished")]
    public int SearchPublishedId { get; set; }
    public IList<SelectListItem> AvailablePublishedOptions { get; set; }

    #endregion
}