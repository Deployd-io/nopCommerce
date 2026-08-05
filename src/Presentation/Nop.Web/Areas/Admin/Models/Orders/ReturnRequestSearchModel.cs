using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents a return request search model
/// </summary>
public partial record ReturnRequestSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ReturnRequestSearchModel()
    {
		_logger.LogDebug(">>> Entering ReturnRequestSearchModel()");
        ReturnRequestStatusList = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting ReturnRequestSearchModel()");
		_logger.LogDebug("ReturnRequestSearchModel(): ReturnRequestStatusList → {ReturnRequestStatusList}", ReturnRequestStatusList);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ReturnRequests.SearchStartDate")]
    [UIHint("DateNullable")]
    public DateTime? StartDate { get; set; }

    [NopResourceDisplayName("Admin.ReturnRequests.SearchEndDate")]
    [UIHint("DateNullable")]
    public DateTime? EndDate { get; set; }

    [NopResourceDisplayName("Admin.ReturnRequests.SearchCustomNumber")]
    public string CustomNumber { get; set; }

    [NopResourceDisplayName("Admin.ReturnRequests.SearchReturnRequestStatus")]
    public int ReturnRequestStatusId { get; set; }

    public IList<SelectListItem> ReturnRequestStatusList { get; set; }

    #endregion
}