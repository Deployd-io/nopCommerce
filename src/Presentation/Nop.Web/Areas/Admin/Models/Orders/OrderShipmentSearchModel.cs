using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents an order shipment search model
/// </summary>
public partial record OrderShipmentSearchModel : BaseSearchModel
{
    #region Ctor

    public OrderShipmentSearchModel()
    {
		logger.LogDebug(">>> Entering OrderShipmentSearchModel()");
        ShipmentItemSearchModel = new ShipmentItemSearchModel();
		logger.LogDebug("<<< Exiting OrderShipmentSearchModel()");
		logger.LogDebug("OrderShipmentSearchModel(): ShipmentItemSearchModel → {ShipmentItemSearchModel}", ShipmentItemSearchModel);
    }

    #endregion

    #region Properties

    public int OrderId { get; set; }

    public ShipmentItemSearchModel ShipmentItemSearchModel { get; set; }

    #endregion
}