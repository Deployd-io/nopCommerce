using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a bestseller search model
/// </summary>
public partial record BestsellerSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public BestsellerSearchModel()
    {
		_logger.LogDebug(">>> Entering BestsellerSearchModel()");
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("BestsellerSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
        AvailableOrderStatuses = new List<SelectListItem>();
		_logger.LogDebug("BestsellerSearchModel(): AvailableOrderStatuses → {AvailableOrderStatuses}", AvailableOrderStatuses);
        AvailablePaymentStatuses = new List<SelectListItem>();
		_logger.LogDebug("BestsellerSearchModel(): AvailablePaymentStatuses → {AvailablePaymentStatuses}", AvailablePaymentStatuses);
        AvailableCategories = new List<SelectListItem>();
		_logger.LogDebug("BestsellerSearchModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
        AvailableManufacturers = new List<SelectListItem>();
		_logger.LogDebug("BestsellerSearchModel(): AvailableManufacturers → {AvailableManufacturers}", AvailableManufacturers);
        AvailableCountries = new List<SelectListItem>();
		_logger.LogDebug("BestsellerSearchModel(): AvailableCountries → {AvailableCountries}", AvailableCountries);
        AvailableVendors = new List<SelectListItem>();
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