using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Reports;

public partial record SalesSummarySearchModel : BaseSearchModel
{
    #region Ctor

    public SalesSummarySearchModel()
    {
		logger.LogDebug(">>> Entering SalesSummarySearchModel()");
        AvailableStores = new List<SelectListItem>();
        AvailableOrderStatuses = new List<SelectListItem>();
		logger.LogDebug("SalesSummarySearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
		logger.LogDebug("SalesSummarySearchModel(): AvailableOrderStatuses → {AvailableOrderStatuses}", AvailableOrderStatuses);
        AvailablePaymentStatuses = new List<SelectListItem>();
        AvailableCategories = new List<SelectListItem>();
		logger.LogDebug("SalesSummarySearchModel(): AvailablePaymentStatuses → {AvailablePaymentStatuses}", AvailablePaymentStatuses);
		logger.LogDebug("SalesSummarySearchModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
        AvailableManufacturers = new List<SelectListItem>();
        AvailableCountries = new List<SelectListItem>();
		logger.LogDebug("SalesSummarySearchModel(): AvailableManufacturers → {AvailableManufacturers}", AvailableManufacturers);
		logger.LogDebug("SalesSummarySearchModel(): AvailableCountries → {AvailableCountries}", AvailableCountries);
        OrderStatusIds = new List<int>();
        PaymentStatusIds = new List<int>();
		logger.LogDebug("SalesSummarySearchModel(): OrderStatusIds → {OrderStatusIds}", OrderStatusIds);
		logger.LogDebug("SalesSummarySearchModel(): PaymentStatusIds → {PaymentStatusIds}", PaymentStatusIds);
        GroupByOptions = new List<SelectListItem>();
        AvailableVendors = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting SalesSummarySearchModel()");
		logger.LogDebug("SalesSummarySearchModel(): GroupByOptions → {GroupByOptions}", GroupByOptions);
		logger.LogDebug("SalesSummarySearchModel(): AvailableVendors → {AvailableVendors}", AvailableVendors);
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