using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Services.Orders;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a best customers report search model
/// </summary>
public partial record BestCustomersReportSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public BestCustomersReportSearchModel()
    {
		_logger.LogDebug(">>> Entering BestCustomersReportSearchModel()");
        AvailableOrderStatuses = new List<SelectListItem>();
        AvailablePaymentStatuses = new List<SelectListItem>();
		_logger.LogDebug("BestCustomersReportSearchModel(): AvailableOrderStatuses → {AvailableOrderStatuses}", AvailableOrderStatuses);
		_logger.LogDebug("BestCustomersReportSearchModel(): AvailablePaymentStatuses → {AvailablePaymentStatuses}", AvailablePaymentStatuses);
        AvailableShippingStatuses = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting BestCustomersReportSearchModel()");
		_logger.LogDebug("BestCustomersReportSearchModel(): AvailableShippingStatuses → {AvailableShippingStatuses}", AvailableShippingStatuses);
    }

    #endregion

    #region Properties

    public OrderByEnum OrderBy { get; set; }

    [NopResourceDisplayName("Admin.Reports.Customers.BestBy.StartDate")]
    [UIHint("DateNullable")]
    public DateTime? StartDate { get; set; }

    [NopResourceDisplayName("Admin.Reports.Customers.BestBy.EndDate")]
    [UIHint("DateNullable")]
    public DateTime? EndDate { get; set; }

    [NopResourceDisplayName("Admin.Reports.Customers.BestBy.OrderStatus")]
    public int OrderStatusId { get; set; }

    [NopResourceDisplayName("Admin.Reports.Customers.BestBy.PaymentStatus")]
    public int PaymentStatusId { get; set; }

    [NopResourceDisplayName("Admin.Reports.Customers.BestBy.ShippingStatus")]
    public int ShippingStatusId { get; set; }

    public IList<SelectListItem> AvailableOrderStatuses { get; set; }

    public IList<SelectListItem> AvailablePaymentStatuses { get; set; }

    public IList<SelectListItem> AvailableShippingStatuses { get; set; }

    #endregion
}