namespace Nop.Core.Events;

/// <summary>
/// Event publisher extensions
/// </summary>
public static class EventPublisherExtensions
{
    /// <summary>
    /// Entity inserted
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task EntityInsertedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
    {
		logger.LogDebug(">>> Entering EntityInsertedAsync(eventPublisher={EventPublisher},entity={Entity})", eventPublisher, entity);
        await eventPublisher.PublishAsync(new EntityInsertedEvent<T>(entity));
		logger.LogDebug("<<< Exiting EntityInsertedAsync(eventPublisher={EventPublisher},entity={Entity})", eventPublisher, entity);
    }

    /// <summary>
    /// Entity inserted
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="entity">Entity</param>
    public static void EntityInserted<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
    {
		logger.LogDebug(">>> Entering EntityInserted(eventPublisher={EventPublisher},entity={Entity})", eventPublisher, entity);
        eventPublisher.Publish(new EntityInsertedEvent<T>(entity));
		logger.LogDebug("<<< Exiting EntityInserted(eventPublisher={EventPublisher},entity={Entity})", eventPublisher, entity);
    }

    /// <summary>
    /// Entity updated
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task EntityUpdatedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
    {
		logger.LogDebug(">>> Entering EntityUpdatedAsync(eventPublisher={EventPublisher},entity={Entity})", eventPublisher, entity);
        await eventPublisher.PublishAsync(new EntityUpdatedEvent<T>(entity));
		logger.LogDebug("<<< Exiting EntityUpdatedAsync(eventPublisher={EventPublisher},entity={Entity})", eventPublisher, entity);
    }

    /// <summary>
    /// Entity updated
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="entity">Entity</param>
    public static void EntityUpdated<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
    {
		logger.LogDebug(">>> Entering EntityUpdated(eventPublisher={EventPublisher},entity={Entity})", eventPublisher, entity);
        eventPublisher.Publish(new EntityUpdatedEvent<T>(entity));
		logger.LogDebug("<<< Exiting EntityUpdated(eventPublisher={EventPublisher},entity={Entity})", eventPublisher, entity);
    }

    /// <summary>
    /// Entity deleted
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task EntityDeletedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
    {
		logger.LogDebug(">>> Entering EntityDeletedAsync(eventPublisher={EventPublisher},entity={Entity})", eventPublisher, entity);
        await eventPublisher.PublishAsync(new EntityDeletedEvent<T>(entity));
		logger.LogDebug("<<< Exiting EntityDeletedAsync(eventPublisher={EventPublisher},entity={Entity})", eventPublisher, entity);
    }

    /// <summary>
    /// Entity deleted
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="entity">Entity</param>
    public static void EntityDeleted<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
    {
		logger.LogDebug(">>> Entering EntityDeleted(eventPublisher={EventPublisher},entity={Entity})", eventPublisher, entity);
        eventPublisher.Publish(new EntityDeletedEvent<T>(entity));
		logger.LogDebug("<<< Exiting EntityDeleted(eventPublisher={EventPublisher},entity={Entity})", eventPublisher, entity);
    }
}