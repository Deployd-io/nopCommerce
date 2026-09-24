using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Reports;

public partial record SalesSummarySearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public SalesSummarySearchModel()
    {
		_logger.LogError(">>> Entering SalesSummarySearchModel()");
        AvailableStores = new List<SelectListItem>();
        AvailableOrderStatuses = new List<SelectListItem>();
		_logger.LogError("SalesSummarySearchModel(): AvailableOrderStatuses → {AvailableOrderStatuses}", AvailableOrderStatuses);
        AvailablePaymentStatuses = new List<SelectListItem>();
		_logger.LogError("SalesSummarySearchModel(): AvailablePaymentStatuses → {AvailablePaymentStatuses}", AvailablePaymentStatuses);
        AvailableCategories = new List<SelectListItem>();
		_logger.LogError("SalesSummarySearchModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
        AvailableManufacturers = new List<SelectListItem>();
		_logger.LogError("SalesSummarySearchModel(): AvailableManufacturers → {AvailableManufacturers}", AvailableManufacturers);
        AvailableCountries = new List<SelectListItem>();
		_logger.LogError("SalesSummarySearchModel(): AvailableCountries → {AvailableCountries}", AvailableCountries);
        OrderStatusIds = new List<int>();
		_logger.LogError("SalesSummarySearchModel(): OrderStatusIds → {OrderStatusIds}", OrderStatusIds);
        PaymentStatusIds = new List<int>();
		_logger.LogError("SalesSummarySearchModel(): PaymentStatusIds → {PaymentStatusIds}", PaymentStatusIds);
        GroupByOptions = new List<SelectListItem>();
		_logger.LogError("SalesSummarySearchModel(): GroupByOptions → {GroupByOptions}", GroupByOptions);
        AvailableVendors = new List<SelectListItem>();
		_logger.LogError("SalesSummarySearchModel(): AvailableVendors → {AvailableVendors}", AvailableVendors);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Reports.SalesSummary.StartDate")]
    [UIHint("DateNullable")]
    public DateTime? StartDate { get; set; }

    [NopResourceDisplayName("Admin.Reports.SalesSummary.EndDate")]
    [UIHint("DateNullable")]
    public DateTime? EndDate { get; set; }

    [NopResourceDisplayName("Admin.Reports.SalesSummary.Store")]
    public int StoreId { get; set; }

    [NopResourceDisplayName("Admin.Reports.SalesSummary.OrderStatus")]
    public IList<int> OrderStatusIds { get; set; }

    [NopResourceDisplayName("Admin.Reports.SalesSummary.PaymentStatus")]
    public IList<int> PaymentStatusIds { get; set; }

    [NopResourceDisplayName("Admin.Reports.SalesSummary.Category")]
    public int CategoryId { get; set; }

    [NopResourceDisplayName("Admin.Reports.SalesSummary.Manufacturer")]
    public int ManufacturerId { get; set; }

    [NopResourceDisplayName("Admin.Reports.SalesSummary.BillingCountry")]
    public int BillingCountryId { get; set; }

    [NopResourceDisplayName("Admin.Reports.SalesSummary.Product")]
    public int ProductId { get; set; }

    [NopResourceDisplayName("Admin.Reports.SalesSummary.GroupBy")]
    public int SearchGroupId { get; set; }

    [NopResourceDisplayName("Admin.Reports.SalesSummary.Vendor")]
    public int VendorId { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    public IList<SelectListItem> AvailableOrderStatuses { get; set; }

    public IList<SelectListItem> AvailablePaymentStatuses { get; set; }

    public IList<SelectListItem> AvailableCategories { get; set; }

    public IList<SelectListItem> AvailableManufacturers { get; set; }

    public IList<SelectListItem> AvailableCountries { get; set; }

    public IList<SelectListItem> GroupByOptions { get; set; }

    public IList<SelectListItem> AvailableVendors { get; set; }

    public bool IsLoggedInAsVendor { get; set; }

    #endregion
}