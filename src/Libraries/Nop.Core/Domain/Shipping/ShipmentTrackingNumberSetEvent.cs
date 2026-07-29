namespace Nop.Core.Domain.Shipping;

/// <summary>
/// Shipment tracking number set event
/// </summary>
public partial class ShipmentTrackingNumberSetEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="shipment">Shipment</param>
    public ShipmentTrackingNumberSetEvent(Shipment shipment)
    {
		logger.LogDebug(">>> Entering ShipmentTrackingNumberSetEvent(shipment={Shipment})", shipment);
        Shipment = shipment;
		logger.LogDebug("<<< Exiting ShipmentTrackingNumberSetEvent(shipment={Shipment})", shipment);
		logger.LogDebug("ShipmentTrackingNumberSetEvent(shipment={Shipment}): Shipment → {Shipment1}", shipment, Shipment);
    }

    /// <summary>
    /// Shipment
    /// </summary>
    public Shipment Shipment { get; }
}