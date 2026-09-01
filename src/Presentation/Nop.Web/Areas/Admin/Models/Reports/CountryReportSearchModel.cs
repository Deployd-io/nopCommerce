using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a country report search model
/// </summary>
public partial record CountryReportSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CountryReportSearchModel()
    {
		_logger.LogDebug(">>> Entering CountryReportSearchModel()");
        AvailableOrderStatuses = new List<SelectListItem>();
		_logger.LogDebug("CountryReportSearchModel(): AvailableOrderStatuses → {AvailableOrderStatuses}", AvailableOrderStatuses);
        AvailablePaymentStatuses = new List<SelectListItem>();
		_logger.LogDebug("CountryReportSearchModel(): AvailablePaymentStatuses → {AvailablePaymentStatuses}", AvailablePaymentStatuses);
        AvailableStores = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Reports.Sales.Country.StartDate")]
    [UIHint("DateNullable")]
    public DateTime? StartDate { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.Country.EndDate")]
    [UIHint("DateNullable")]
    public DateTime? EndDate { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.Country.OrderStatus")]
    public int OrderStatusId { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.Country.PaymentStatus")]
    public int PaymentStatusId { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.Country.SearchStore")]
    public int SearchStoreId { get; set; }

    public IList<SelectListItem> AvailableOrderStatuses { get; set; }

    public IList<SelectListItem> AvailablePaymentStatuses { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    #endregion
}