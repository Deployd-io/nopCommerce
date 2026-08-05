using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.RFQ.Models.Admin;

/// <summary>
/// Represents a quote search model
/// </summary>
public record QuoteSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public QuoteSearchModel()
    {
		_logger.LogDebug(">>> Entering QuoteSearchModel()");
        AvailableQuoteStatuses = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting QuoteSearchModel()");
		_logger.LogDebug("QuoteSearchModel(): AvailableQuoteStatuses → {AvailableQuoteStatuses}", AvailableQuoteStatuses);
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

    [NopResourceDisplayName("Plugins.Misc.RFQ.AdminQuote.QuoteStatus")]
    public int QuoteStatus { get; set; }

    public List<SelectListItem> AvailableQuoteStatuses { get; set; }

    #endregion
}
