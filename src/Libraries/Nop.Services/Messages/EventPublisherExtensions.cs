using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Messages;

/// <summary>
/// Event publisher extensions
/// </summary>
public static class EventPublisherExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Publishes the newsletter subscribe event.
    /// </summary>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="subscription">The newsletter subscription.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task PublishNewsLetterSubscribeAsync(this IEventPublisher eventPublisher, NewsLetterSubscription subscription)
    {
		_logger.LogDebug(">>> Entering PublishNewsLetterSubscribeAsync(eventPublisher={EventPublisher},subscription={Subscription})", eventPublisher, subscription);
        await eventPublisher.PublishAsync(new EmailSubscribedEvent(subscription));
		_logger.LogDebug("<<< Exiting PublishNewsLetterSubscribeAsync(eventPublisher={EventPublisher},subscription={Subscription})", eventPublisher, subscription);
    }

    /// <summary>
    /// Publishes the newsletter unsubscribe event.
    /// </summary>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="subscription">The newsletter subscription.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task PublishNewsLetterUnsubscribeAsync(this IEventPublisher eventPublisher, NewsLetterSubscription subscription)
    {
		_logger.LogDebug(">>> Entering PublishNewsLetterUnsubscribeAsync(eventPublisher={EventPublisher},subscription={Subscription})", eventPublisher, subscription);
        await eventPublisher.PublishAsync(new EmailUnsubscribedEvent(subscription));
		_logger.LogDebug("<<< Exiting PublishNewsLetterUnsubscribeAsync(eventPublisher={EventPublisher},subscription={Subscription})", eventPublisher, subscription);
    }

    /// <summary>
    /// Entity tokens added
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="entity">Entity</param>
    /// <param name="tokens">Tokens</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task EntityTokensAddedAsync<T>(this IEventPublisher eventPublisher, T entity, IList<Token> tokens) where T : BaseEntity
    {
		_logger.LogDebug(">>> Entering EntityTokensAddedAsync(eventPublisher={EventPublisher},entity={Entity},tokens=<redacted>)", eventPublisher, entity);
        await eventPublisher.PublishAsync(new EntityTokensAddedEvent<T>(entity, tokens));
		_logger.LogDebug("<<< Exiting EntityTokensAddedAsync(eventPublisher={EventPublisher},entity={Entity},tokens=<redacted>)", eventPublisher, entity);
    }

    /// <summary>
    /// Message token added
    /// </summary>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="message">Message</param>
    /// <param name="tokens">Tokens</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task MessageTokensAddedAsync(this IEventPublisher eventPublisher, MessageTemplate message, IList<Token> tokens)
    {
		_logger.LogDebug(">>> Entering MessageTokensAddedAsync(eventPublisher={EventPublisher},message={Message},tokens=<redacted>)", eventPublisher, message);
        await eventPublisher.PublishAsync(new MessageTokensAddedEvent(message, tokens));
		_logger.LogDebug("<<< Exiting MessageTokensAddedAsync(eventPublisher={EventPublisher},message={Message},tokens=<redacted>)", eventPublisher, message);
    }
}