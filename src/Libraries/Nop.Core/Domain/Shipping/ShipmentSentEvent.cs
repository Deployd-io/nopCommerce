namespace Nop.Core.Domain.Shipping;

/// <summary>
/// Shipment sent event
/// </summary>
public partial class ShipmentSentEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="shipment">Shipment</param>
    public ShipmentSentEvent(Shipment shipment)
    {
		logger.LogDebug(">>> Entering ShipmentSentEvent(shipment={Shipment})", shipment);
        Shipment = shipment;
		logger.LogDebug("<<< Exiting ShipmentSentEvent(shipment={Shipment})", shipment);
		logger.LogDebug("ShipmentSentEvent(shipment={Shipment}): Shipment → {Shipment1}", shipment, Shipment);
    }

    /// <summary>
    /// Shipment
    /// </summary>
    public Shipment Shipment { get; }
}
