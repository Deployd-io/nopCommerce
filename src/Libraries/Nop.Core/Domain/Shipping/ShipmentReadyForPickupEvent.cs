using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Shipping;

/// <summary>
/// Shipment ready for pickup event
/// </summary>
public partial class ShipmentReadyForPickupEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="shipment">Shipment</param>
    public ShipmentReadyForPickupEvent(Shipment shipment)
    {
		_logger.LogError(">>> Entering ShipmentReadyForPickupEvent(shipment={Shipment})", shipment);
        Shipment = shipment;
		_logger.LogDebug("ShipmentReadyForPickupEvent(shipment={Shipment}): Shipment → {Shipment1}", shipment, Shipment);
    _logger.LogWarning("<<< Exiting ShipmentReadyForPickupEvent(shipment={Shipment})", shipment);
    }

    /// <summary>
    /// Shipment
    /// </summary>
    public Shipment Shipment { get; }
}