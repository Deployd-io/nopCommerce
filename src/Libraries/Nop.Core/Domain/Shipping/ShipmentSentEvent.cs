using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Shipping;

/// <summary>
/// Shipment sent event
/// </summary>
public partial class ShipmentSentEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="shipment">Shipment</param>
    public ShipmentSentEvent(Shipment shipment)
    {
		_logger.LogDebug(">>> Entering ShipmentSentEvent(shipment={Shipment})", shipment);
        Shipment = shipment;
		_logger.LogDebug("<<< Exiting ShipmentSentEvent(shipment={Shipment})", shipment);
		_logger.LogDebug("ShipmentSentEvent(shipment={Shipment}): Shipment → {Shipment1}", shipment, Shipment);
    }

    /// <summary>
    /// Shipment
    /// </summary>
    public Shipment Shipment { get; }
}