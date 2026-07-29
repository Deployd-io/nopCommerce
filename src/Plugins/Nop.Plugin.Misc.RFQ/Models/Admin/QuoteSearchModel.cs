using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.RFQ.Models.Admin;

/// <summary>
/// Represents a quote search model
/// </summary>
public record QuoteSearchModel : BaseSearchModel
{
    #region Ctor

    public QuoteSearchModel()
    {
		logger.LogDebug(">>> Entering QuoteSearchModel()");
        AvailableQuoteStatuses = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting QuoteSearchModel()");
		logger.LogDebug("QuoteSearchModel(): AvailableQuoteStatuses → {AvailableQuoteStatuses}", AvailableQuoteStatuses);
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
