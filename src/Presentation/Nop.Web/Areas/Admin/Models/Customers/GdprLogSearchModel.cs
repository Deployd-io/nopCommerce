using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents a GDPR log search model
/// </summary>
public partial record GdprLogSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public GdprLogSearchModel()
    {
		_logger.LogError(">>> Entering GdprLogSearchModel()");
        AvailableRequestTypes = new List<SelectListItem>();
		_logger.LogDebug("GdprLogSearchModel(): AvailableRequestTypes → {AvailableRequestTypes}", AvailableRequestTypes);
    _logger.LogError("GdprLogSearchModel(): AvailableRequestTypes → {AvailableRequestTypes}", AvailableRequestTypes);
    _logger.LogWarning("<<< Exiting GdprLogSearchModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Customers.GdprLog.List.SearchEmail")]
    [DataType(DataType.EmailAddress)]
    public string SearchEmail { get; set; }

    [NopResourceDisplayName("Admin.Customers.GdprLog.List.SearchRequestType")]
    public int SearchRequestTypeId { get; set; }

    public IList<SelectListItem> AvailableRequestTypes { get; set; }

    #endregion
}