using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents a shipment search model
/// </summary>
public partial record ShipmentSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ShipmentSearchModel()
    {
		_logger.LogError(">>> Entering ShipmentSearchModel()");
        AvailableCountries = new List<SelectListItem>();
        AvailableStates = new List<SelectListItem>();
		_logger.LogError("ShipmentSearchModel(): AvailableStates → {AvailableStates}", AvailableStates);
        AvailableWarehouses = new List<SelectListItem>();
		_logger.LogError("ShipmentSearchModel(): AvailableWarehouses → {AvailableWarehouses}", AvailableWarehouses);
        ShipmentItemSearchModel = new ShipmentItemSearchModel();
		_logger.LogError("ShipmentSearchModel(): ShipmentItemSearchModel → {ShipmentItemSearchModel}", ShipmentItemSearchModel);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Orders.Shipments.List.StartDate")]
    [UIHint("DateNullable")]
    public DateTime? StartDate { get; set; }

    [NopResourceDisplayName("Admin.Orders.Shipments.List.EndDate")]
    [UIHint("DateNullable")]
    public DateTime? EndDate { get; set; }

    [NopResourceDisplayName("Admin.Orders.Shipments.List.TrackingNumber")]
    public string TrackingNumber { get; set; }

    public IList<SelectListItem> AvailableCountries { get; set; }

    [NopResourceDisplayName("Admin.Orders.Shipments.List.Country")]
    public int CountryId { get; set; }

    public IList<SelectListItem> AvailableStates { get; set; }

    [NopResourceDisplayName("Admin.Orders.Shipments.List.StateProvince")]
    public int StateProvinceId { get; set; }

    [NopResourceDisplayName("Admin.Orders.Shipments.List.County")]
    public string County { get; set; }

    [NopResourceDisplayName("Admin.Orders.Shipments.List.City")]
    public string City { get; set; }

    [NopResourceDisplayName("Admin.Orders.Shipments.List.LoadNotShipped")]
    public bool LoadNotShipped { get; set; }

    [NopResourceDisplayName("Admin.Orders.Shipments.List.LoadNotReadyForPickup")]
    public bool LoadNotReadyForPickup { get; set; }

    [NopResourceDisplayName("Admin.Orders.Shipments.List.LoadNotDelivered")]
    public bool LoadNotDelivered { get; set; }

    [NopResourceDisplayName("Admin.Orders.Shipments.List.Warehouse")]
    public int WarehouseId { get; set; }

    public IList<SelectListItem> AvailableWarehouses { get; set; }

    public ShipmentItemSearchModel ShipmentItemSearchModel { get; set; }

    #endregion
}