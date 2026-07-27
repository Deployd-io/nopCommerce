namespace Nop.Core.Domain.Shipping;

/// <summary>
/// Shipment ready for pickup event
/// </summary>
public partial class ShipmentReadyForPickupEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="shipment">Shipment</param>
    public ShipmentReadyForPickupEvent(Shipment shipment)
    {
		logger.LogDebug(">>> Entering ShipmentReadyForPickupEvent(shipment={Shipment})", shipment);
        Shipment = shipment;
		logger.LogDebug("<<< Exiting ShipmentReadyForPickupEvent(shipment={Shipment})", shipment);
		logger.LogDebug("ShipmentReadyForPickupEvent(shipment={Shipment}): Shipment → {Shipment1}", shipment, Shipment);
    }

    /// <summary>
    /// Shipment
    /// </summary>
    public Shipment Shipment { get; }
}