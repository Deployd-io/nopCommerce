using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Shipping;

/// <summary>
/// Event publisher extensions
/// </summary>
public static class EventPublisherExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Publishes the shipment sent event.
    /// </summary>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="shipment">The shipment.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task PublishShipmentSentAsync(this IEventPublisher eventPublisher, Shipment shipment)
    {
		_logger.LogDebug(">>> Entering PublishShipmentSentAsync(eventPublisher={EventPublisher},shipment={Shipment})", eventPublisher, shipment);
        await eventPublisher.PublishAsync(new ShipmentSentEvent(shipment));
		_logger.LogDebug("<<< Exiting PublishShipmentSentAsync(eventPublisher={EventPublisher},shipment={Shipment})", eventPublisher, shipment);
    }

    /// <summary>
    /// Publishes the shipment ready for pickup event.
    /// </summary>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="shipment">The shipment.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task PublishShipmentReadyForPickupAsync(this IEventPublisher eventPublisher, Shipment shipment)
    {
		_logger.LogDebug(">>> Entering PublishShipmentReadyForPickupAsync(eventPublisher={EventPublisher},shipment={Shipment})", eventPublisher, shipment);
        await eventPublisher.PublishAsync(new ShipmentReadyForPickupEvent(shipment));
		_logger.LogDebug("<<< Exiting PublishShipmentReadyForPickupAsync(eventPublisher={EventPublisher},shipment={Shipment})", eventPublisher, shipment);
    }

    /// <summary>
    /// Publishes the shipment delivered event.
    /// </summary>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="shipment">The shipment.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task PublishShipmentDeliveredAsync(this IEventPublisher eventPublisher, Shipment shipment)
    {
		_logger.LogDebug(">>> Entering PublishShipmentDeliveredAsync(eventPublisher={EventPublisher},shipment={Shipment})", eventPublisher, shipment);
        await eventPublisher.PublishAsync(new ShipmentDeliveredEvent(shipment));
		_logger.LogDebug("<<< Exiting PublishShipmentDeliveredAsync(eventPublisher={EventPublisher},shipment={Shipment})", eventPublisher, shipment);
    }
}