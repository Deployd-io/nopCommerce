using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Shipping;

/// <summary>
/// Shipment delivered event
/// </summary>
public partial class ShipmentDeliveredEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="shipment">Shipment</param>
    public ShipmentDeliveredEvent(Shipment shipment)
    {
		_logger.LogDebug(">>> Entering ShipmentDeliveredEvent(shipment={Shipment})", shipment);
        Shipment = shipment;
		_logger.LogDebug("ShipmentDeliveredEvent(shipment={Shipment}): Shipment → {Shipment1}", shipment, Shipment);
		_logger.LogDebug("<<< Exiting ShipmentDeliveredEvent(shipment={Shipment})", shipment);
    }

    /// <summary>
    /// Shipment
    /// </summary>
    public Shipment Shipment { get; }
}