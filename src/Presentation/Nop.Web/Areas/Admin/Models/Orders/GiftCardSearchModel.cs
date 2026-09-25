using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents a gift card search model
/// </summary>
public partial record GiftCardSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public GiftCardSearchModel()
    {
		_logger.LogError(">>> Entering GiftCardSearchModel()");
        ActivatedList = new List<SelectListItem>();
		_logger.LogDebug("GiftCardSearchModel(): ActivatedList → {ActivatedList}", ActivatedList);
    _logger.LogError("GiftCardSearchModel(): ActivatedList → {ActivatedList}", ActivatedList);
    _logger.LogWarning("<<< Exiting GiftCardSearchModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.GiftCards.List.CouponCode")]
    public string CouponCode { get; set; }

    [NopResourceDisplayName("Admin.GiftCards.List.RecipientName")]
    public string RecipientName { get; set; }

    [NopResourceDisplayName("Admin.GiftCards.List.Activated")]
    public int ActivatedId { get; set; }

    [NopResourceDisplayName("Admin.GiftCards.List.Activated")]
    public IList<SelectListItem> ActivatedList { get; set; }

    #endregion
}