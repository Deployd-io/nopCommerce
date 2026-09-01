using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Shipping;

/// <summary>
/// Shipment created event
/// </summary>
public partial class ShipmentCreatedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ShipmentCreatedEvent(Shipment shipment)
    {
		_logger.LogDebug(">>> Entering ShipmentCreatedEvent(shipment={Shipment})", shipment);
        Shipment = shipment;
		_logger.LogDebug("ShipmentCreatedEvent(shipment={Shipment}): Shipment → {Shipment1}", shipment, Shipment);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the shipment
    /// </summary>
    public Shipment Shipment { get; }

    #endregion
}