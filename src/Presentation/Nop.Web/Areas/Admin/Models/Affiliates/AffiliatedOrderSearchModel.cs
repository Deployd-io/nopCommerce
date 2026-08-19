using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Affiliates;

/// <summary>
/// Represents an affiliated order search model
/// </summary>
public partial record AffiliatedOrderSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AffiliatedOrderSearchModel()
    {
		_logger.LogDebug(">>> Entering AffiliatedOrderSearchModel()");
        AvailableOrderStatuses = new List<SelectListItem>();
        AvailablePaymentStatuses = new List<SelectListItem>();
		_logger.LogDebug("AffiliatedOrderSearchModel(): AvailablePaymentStatuses → {AvailablePaymentStatuses}", AvailablePaymentStatuses);
		_logger.LogDebug("AffiliatedOrderSearchModel(): AvailableOrderStatuses → {AvailableOrderStatuses}", AvailableOrderStatuses);
        AvailableShippingStatuses = new List<SelectListItem>();
		_logger.LogDebug("AffiliatedOrderSearchModel(): AvailableShippingStatuses → {AvailableShippingStatuses}", AvailableShippingStatuses);
		_logger.LogDebug("<<< Exiting AffiliatedOrderSearchModel()");
    }

    #endregion

    #region Properties

    public int AffliateId { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Orders.StartDate")]
    [UIHint("DateNullable")]
    public DateTime? StartDate { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Orders.EndDate")]
    [UIHint("DateNullable")]
    public DateTime? EndDate { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Orders.OrderStatus")]
    public int OrderStatusId { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Orders.PaymentStatus")]
    public int PaymentStatusId { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Orders.ShippingStatus")]
    public int ShippingStatusId { get; set; }

    public IList<SelectListItem> AvailableOrderStatuses { get; set; }
    public IList<SelectListItem> AvailablePaymentStatuses { get; set; }
    public IList<SelectListItem> AvailableShippingStatuses { get; set; }

    #endregion
}