using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a low stock product search model
/// </summary>
public partial record LowStockProductSearchModel : BaseSearchModel
{
    #region Ctor

    public LowStockProductSearchModel()
    {
		logger.LogDebug(">>> Entering LowStockProductSearchModel()");
        AvailablePublishedOptions = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting LowStockProductSearchModel()");
		logger.LogDebug("LowStockProductSearchModel(): AvailablePublishedOptions → {AvailablePublishedOptions}", AvailablePublishedOptions);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Reports.LowStock.SearchPublished")]
    public int SearchPublishedId { get; set; }
    public IList<SelectListItem> AvailablePublishedOptions { get; set; }

    #endregion
}