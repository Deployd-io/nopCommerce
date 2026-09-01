using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Shipping;

/// <summary>
/// Shipment tracking number set event
/// </summary>
public partial class ShipmentTrackingNumberSetEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="shipment">Shipment</param>
    public ShipmentTrackingNumberSetEvent(Shipment shipment)
    {
		_logger.LogDebug(">>> Entering ShipmentTrackingNumberSetEvent(shipment={Shipment})", shipment);
        Shipment = shipment;
		_logger.LogDebug("ShipmentTrackingNumberSetEvent(shipment={Shipment}): Shipment → {Shipment1}", shipment, Shipment);
    }

    /// <summary>
    /// Shipment
    /// </summary>
    public Shipment Shipment { get; }
}