using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a bestseller search model
/// </summary>
public partial record BestsellerSearchModel : BaseSearchModel
{
    #region Ctor

    public BestsellerSearchModel()
    {
		logger.LogDebug(">>> Entering BestsellerSearchModel()");
        AvailableStores = new List<SelectListItem>();
        AvailableOrderStatuses = new List<SelectListItem>();
		logger.LogDebug("BestsellerSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
		logger.LogDebug("BestsellerSearchModel(): AvailableOrderStatuses → {AvailableOrderStatuses}", AvailableOrderStatuses);
        AvailablePaymentStatuses = new List<SelectListItem>();
        AvailableCategories = new List<SelectListItem>();
		logger.LogDebug("BestsellerSearchModel(): AvailablePaymentStatuses → {AvailablePaymentStatuses}", AvailablePaymentStatuses);
		logger.LogDebug("BestsellerSearchModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
        AvailableManufacturers = new List<SelectListItem>();
        AvailableCountries = new List<SelectListItem>();
		logger.LogDebug("BestsellerSearchModel(): AvailableManufacturers → {AvailableManufacturers}", AvailableManufacturers);
		logger.LogDebug("BestsellerSearchModel(): AvailableCountries → {AvailableCountries}", AvailableCountries);
        AvailableVendors = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting BestsellerSearchModel()");
		logger.LogDebug("BestsellerSearchModel(): AvailableVendors → {AvailableVendors}", AvailableVendors);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.StartDate")]
    [UIHint("DateNullable")]
    public DateTime? StartDate { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.EndDate")]
    [UIHint("DateNullable")]
    public DateTime? EndDate { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.Store")]
    public int StoreId { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.OrderStatus")]
    public int OrderStatusId { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.PaymentStatus")]
    public int PaymentStatusId { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.Category")]
    public int CategoryId { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.Manufacturer")]
    public int ManufacturerId { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.BillingCountry")]
    public int BillingCountryId { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.Vendor")]
    public int VendorId { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    public IList<SelectListItem> AvailableOrderStatuses { get; set; }

    public IList<SelectListItem> AvailablePaymentStatuses { get; set; }

    public IList<SelectListItem> AvailableCategories { get; set; }

    public IList<SelectListItem> AvailableManufacturers { get; set; }

    public IList<SelectListItem> AvailableCountries { get; set; }

    public IList<SelectListItem> AvailableVendors { get; set; }

    public bool IsLoggedInAsVendor { get; set; }

    #endregion
}