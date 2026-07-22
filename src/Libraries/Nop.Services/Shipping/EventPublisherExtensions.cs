using Nop.Core.Domain.Shipping;
using Nop.Core.Events;

namespace Nop.Services.Shipping;

/// <summary>
/// Event publisher extensions
/// </summary>
public static class EventPublisherExtensions
{
    /// <summary>
    /// Publishes the shipment sent event.
    /// </summary>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="shipment">The shipment.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task PublishShipmentSentAsync(this IEventPublisher eventPublisher, Shipment shipment)
    {
		logger.LogDebug(">>> Entering PublishShipmentSentAsync(eventPublisher={EventPublisher},shipment={Shipment})", eventPublisher, shipment);
        await eventPublisher.PublishAsync(new ShipmentSentEvent(shipment));
		logger.LogDebug("<<< Exiting PublishShipmentSentAsync(eventPublisher={EventPublisher},shipment={Shipment})", eventPublisher, shipment);
    }

    /// <summary>
    /// Publishes the shipment ready for pickup event.
    /// </summary>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="shipment">The shipment.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task PublishShipmentReadyForPickupAsync(this IEventPublisher eventPublisher, Shipment shipment)
    {
		logger.LogDebug(">>> Entering PublishShipmentReadyForPickupAsync(eventPublisher={EventPublisher},shipment={Shipment})", eventPublisher, shipment);
        await eventPublisher.PublishAsync(new ShipmentReadyForPickupEvent(shipment));
		logger.LogDebug("<<< Exiting PublishShipmentReadyForPickupAsync(eventPublisher={EventPublisher},shipment={Shipment})", eventPublisher, shipment);
    }

    /// <summary>
    /// Publishes the shipment delivered event.
    /// </summary>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="shipment">The shipment.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task PublishShipmentDeliveredAsync(this IEventPublisher eventPublisher, Shipment shipment)
    {
		logger.LogDebug(">>> Entering PublishShipmentDeliveredAsync(eventPublisher={EventPublisher},shipment={Shipment})", eventPublisher, shipment);
        await eventPublisher.PublishAsync(new ShipmentDeliveredEvent(shipment));
		logger.LogDebug("<<< Exiting PublishShipmentDeliveredAsync(eventPublisher={EventPublisher},shipment={Shipment})", eventPublisher, shipment);
    }
}
