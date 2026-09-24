using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.RFQ.Models.Admin;

/// <summary>
/// Represents a request quote search model
/// </summary>
public record RequestQuoteSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public RequestQuoteSearchModel()
    {
		_logger.LogError(">>> Entering RequestQuoteSearchModel()");
        AvailableRequestQuoteStatuses = new List<SelectListItem>();
		_logger.LogDebug("RequestQuoteSearchModel(): AvailableRequestQuoteStatuses → {AvailableRequestQuoteStatuses}", AvailableRequestQuoteStatuses);
    _logger.LogError("RequestQuoteSearchModel(): AvailableRequestQuoteStatuses → {AvailableRequestQuoteStatuses}", AvailableRequestQuoteStatuses);
    _logger.LogWarning("<<< Exiting RequestQuoteSearchModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.CustomerEmail")]
    public string CustomerEmail { get; set; }

    public int CustomerId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.CreatedOnFrom")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnFrom { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.CreatedOnTo")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnTo { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.AdminRequests.RequestQuoteStatus")]
    public int RequestQuoteStatus { get; set; }
    public List<SelectListItem> AvailableRequestQuoteStatuses { get; set; }

    #endregion
}
