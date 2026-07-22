namespace Nop.Core.Domain.Shipping;

/// <summary>
/// Shipment created event
/// </summary>
public partial class ShipmentCreatedEvent
{
    #region Ctor

    public ShipmentCreatedEvent(Shipment shipment)
    {
		logger.LogDebug(">>> Entering ShipmentCreatedEvent(shipment={Shipment})", shipment);
        Shipment = shipment;
		logger.LogDebug("<<< Exiting ShipmentCreatedEvent(shipment={Shipment})", shipment);
		logger.LogDebug("ShipmentCreatedEvent(shipment={Shipment}): Shipment → {Shipment1}", shipment, Shipment);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the shipment
    /// </summary>
    public Shipment Shipment { get; }

    #endregion
}
