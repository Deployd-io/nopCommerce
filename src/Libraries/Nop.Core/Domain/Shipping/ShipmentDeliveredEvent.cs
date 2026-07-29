namespace Nop.Core.Domain.Shipping;

/// <summary>
/// Shipment delivered event
/// </summary>
public partial class ShipmentDeliveredEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="shipment">Shipment</param>
    public ShipmentDeliveredEvent(Shipment shipment)
    {
		logger.LogDebug(">>> Entering ShipmentDeliveredEvent(shipment={Shipment})", shipment);
        Shipment = shipment;
		logger.LogDebug("<<< Exiting ShipmentDeliveredEvent(shipment={Shipment})", shipment);
		logger.LogDebug("ShipmentDeliveredEvent(shipment={Shipment}): Shipment → {Shipment1}", shipment, Shipment);
    }

    /// <summary>
    /// Shipment
    /// </summary>
    public Shipment Shipment { get; }
}