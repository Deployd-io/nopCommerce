using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents an order search model
/// </summary>
public partial record OrderSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public OrderSearchModel()
    {
		_logger.LogDebug(">>> Entering OrderSearchModel()");
        AvailableOrderStatuses = new List<SelectListItem>();
        AvailablePaymentStatuses = new List<SelectListItem>();
		_logger.LogDebug("OrderSearchModel(): AvailablePaymentStatuses → {AvailablePaymentStatuses}", AvailablePaymentStatuses);
		_logger.LogDebug("OrderSearchModel(): AvailableOrderStatuses → {AvailableOrderStatuses}", AvailableOrderStatuses);
        AvailableShippingStatuses = new List<SelectListItem>();
		_logger.LogDebug("OrderSearchModel(): AvailableShippingStatuses → {AvailableShippingStatuses}", AvailableShippingStatuses);
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("OrderSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
        AvailableVendors = new List<SelectListItem>();
		_logger.LogDebug("OrderSearchModel(): AvailableVendors → {AvailableVendors}", AvailableVendors);
        AvailableWarehouses = new List<SelectListItem>();
		_logger.LogDebug("OrderSearchModel(): AvailableWarehouses → {AvailableWarehouses}", AvailableWarehouses);
        AvailablePaymentMethods = new List<SelectListItem>();
		_logger.LogDebug("OrderSearchModel(): AvailablePaymentMethods → {AvailablePaymentMethods}", AvailablePaymentMethods);
        AvailableCountries = new List<SelectListItem>();
		_logger.LogDebug("OrderSearchModel(): AvailableCountries → {AvailableCountries}", AvailableCountries);
        OrderStatusIds = new List<int>();
		_logger.LogDebug("OrderSearchModel(): OrderStatusIds → {OrderStatusIds}", OrderStatusIds);
        PaymentStatusIds = new List<int>();
		_logger.LogDebug("OrderSearchModel(): PaymentStatusIds → {PaymentStatusIds}", PaymentStatusIds);
        ShippingStatusIds = new List<int>();
		_logger.LogDebug("OrderSearchModel(): ShippingStatusIds → {ShippingStatusIds}", ShippingStatusIds);
        LicenseCheckModel = new();
		_logger.LogDebug("OrderSearchModel(): LicenseCheckModel → {LicenseCheckModel}", LicenseCheckModel);
		_logger.LogDebug("<<< Exiting OrderSearchModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Orders.List.StartDate")]
    [UIHint("DateNullable")]
    public DateTime? StartDate { get; set; }

    [NopResourceDisplayName("Admin.Orders.List.EndDate")]
    [UIHint("DateNullable")]
    public DateTime? EndDate { get; set; }

    [NopResourceDisplayName("Admin.Orders.List.OrderStatus")]
    public IList<int> OrderStatusIds { get; set; }

    [NopResourceDisplayName("Admin.Orders.List.PaymentStatus")]
    public IList<int> PaymentStatusIds { get; set; }

    [NopResourceDisplayName("Admin.Orders.List.ShippingStatus")]
    public IList<int> ShippingStatusIds { get; set; }

    [NopResourceDisplayName("Admin.Orders.List.PaymentMethod")]
    public string PaymentMethodSystemName { get; set; }

    [NopResourceDisplayName("Admin.Orders.List.Store")]
    public int StoreId { get; set; }

    [NopResourceDisplayName("Admin.Orders.List.Vendor")]
    public int VendorId { get; set; }

    [NopResourceDisplayName("Admin.Orders.List.Warehouse")]
    public int WarehouseId { get; set; }

    [NopResourceDisplayName("Admin.Orders.List.Product")]
    public int ProductId { get; set; }

    [NopResourceDisplayName("Admin.Orders.List.BillingEmail")]
    public string BillingEmail { get; set; }

    [NopResourceDisplayName("Admin.Orders.List.BillingPhone")]
    public string BillingPhone { get; set; }

    public bool BillingPhoneEnabled { get; set; }

    [NopResourceDisplayName("Admin.Orders.List.BillingLastName")]
    public string BillingLastName { get; set; }

    [NopResourceDisplayName("Admin.Orders.List.BillingCountry")]
    public int BillingCountryId { get; set; }

    [NopResourceDisplayName("Admin.Orders.List.OrderNotes")]
    public string OrderNotes { get; set; }

    [NopResourceDisplayName("Admin.Orders.List.GoDirectlyToNumber")]
    public string GoDirectlyToCustomOrderNumber { get; set; }

    public bool IsLoggedInAsVendor { get; set; }

    public IList<SelectListItem> AvailableOrderStatuses { get; set; }

    public IList<SelectListItem> AvailablePaymentStatuses { get; set; }

    public IList<SelectListItem> AvailableShippingStatuses { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    public IList<SelectListItem> AvailableVendors { get; set; }

    public IList<SelectListItem> AvailableWarehouses { get; set; }

    public IList<SelectListItem> AvailablePaymentMethods { get; set; }

    public IList<SelectListItem> AvailableCountries { get; set; }

    public LicenseCheckModel LicenseCheckModel { get; set; }

    public bool HideStoresList { get; set; }

    #endregion
}