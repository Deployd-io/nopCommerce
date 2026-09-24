using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Logging;

/// <summary>
/// Represents an activity log search model
/// </summary>
public partial record ActivityLogSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ActivityLogSearchModel()
    {
		_logger.LogError(">>> Entering ActivityLogSearchModel()");
        ActivityLogType = new List<SelectListItem>();
		_logger.LogDebug("ActivityLogSearchModel(): ActivityLogType → {ActivityLogType}", ActivityLogType);
    _logger.LogError("ActivityLogSearchModel(): ActivityLogType → {ActivityLogType}", ActivityLogType);
    _logger.LogWarning("<<< Exiting ActivityLogSearchModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Customers.ActivityLog.Fields.CreatedOnFrom")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnFrom { get; set; }

    [NopResourceDisplayName("Admin.Customers.ActivityLog.Fields.CreatedOnTo")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnTo { get; set; }

    [NopResourceDisplayName("Admin.Customers.ActivityLog.Fields.ActivityLogType")]
    public int ActivityLogTypeId { get; set; }

    [NopResourceDisplayName("Admin.Customers.ActivityLog.Fields.ActivityLogType")]
    public IList<SelectListItem> ActivityLogType { get; set; }

    [NopResourceDisplayName("Admin.Customers.ActivityLog.Fields.IpAddress")]
    public string IpAddress { get; set; }

    #endregion
}