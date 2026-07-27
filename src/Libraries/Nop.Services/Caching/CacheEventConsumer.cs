using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Services.Events;

namespace Nop.Services.Caching;

/// <summary>
/// Represents the base entity cache event consumer
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public abstract partial class CacheEventConsumer<TEntity> :
    IConsumer<EntityInsertedEvent<TEntity>>,
    IConsumer<EntityUpdatedEvent<TEntity>>,
    IConsumer<EntityDeletedEvent<TEntity>>
    where TEntity : BaseEntity
{
    #region Fields

    protected readonly IShortTermCacheManager _shortTermCacheManager;
    protected readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    protected CacheEventConsumer()
    {
		logger.LogDebug(">>> Entering CacheEventConsumer()");
        _shortTermCacheManager = EngineContext.Current.Resolve<IShortTermCacheManager>();
        _staticCacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
		logger.LogDebug("<<< Exiting CacheEventConsumer()");
		logger.LogDebug("CacheEventConsumer(): _shortTermCacheManager → {_shortTermCacheManager}", _shortTermCacheManager);
		logger.LogDebug("CacheEventConsumer(): _staticCacheManager → {_staticCacheManager}", _staticCacheManager);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Clear cache by entity event type
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="entityEventType">Entity event type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ClearCacheAsync(TEntity entity, EntityEventType entityEventType)
    {
		logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
        await RemoveByPrefixAsync(NopEntityCacheDefaults<TEntity>.ByIdsPrefix);
        await RemoveByPrefixAsync(NopEntityCacheDefaults<TEntity>.AllPrefix);

        if (entityEventType != EntityEventType.Insert)
            await RemoveAsync(NopEntityCacheDefaults<TEntity>.ByIdCacheKey, entity);

        await ClearCacheAsync(entity);
		logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
		logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
		logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }

    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual Task ClearCacheAsync(TEntity entity)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes items by cache key prefix
    /// </summary>
    /// <param name="prefix">Cache key prefix</param>
    /// <param name="prefixParameters">Parameters to create cache key prefix</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
    {
		logger.LogDebug(">>> Entering RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
        _shortTermCacheManager.RemoveByPrefix(prefix, prefixParameters);
        await _staticCacheManager.RemoveByPrefixAsync(prefix, prefixParameters);
		logger.LogDebug("<<< Exiting RemoveByPrefixAsync(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
    }

    /// <summary>
    /// Remove the value with the specified key from the cache
    /// </summary>
    /// <param name="cacheKey">Cache key</param>
    /// <param name="cacheKeyParameters">Parameters to create cache key</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task RemoveAsync(CacheKey cacheKey, params object[] cacheKeyParameters)
    {
		logger.LogDebug(">>> Entering RemoveAsync(cacheKey={CacheKey},cacheKeyParameters={CacheKeyParameters})", cacheKey, cacheKeyParameters);
        _shortTermCacheManager.Remove(cacheKey.Key, cacheKeyParameters);
        await _staticCacheManager.RemoveAsync(cacheKey, cacheKeyParameters);
		logger.LogDebug("<<< Exiting RemoveAsync(cacheKey={CacheKey},cacheKeyParameters={CacheKeyParameters})", cacheKey, cacheKeyParameters);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle entity inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<TEntity> eventMessage)
    {
		logger.LogDebug(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        await ClearCacheAsync(eventMessage.Entity, EntityEventType.Insert);
		logger.LogDebug("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
    }

    /// <summary>
    /// Handle entity updated event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityUpdatedEvent<TEntity> eventMessage)
    {
        await ClearCacheAsync(eventMessage.Entity, EntityEventType.Update);
    }

    /// <summary>
    /// Handle entity deleted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityDeletedEvent<TEntity> eventMessage)
    {
        await ClearCacheAsync(eventMessage.Entity, EntityEventType.Delete);
    }

    #endregion

    #region Nested

    protected enum EntityEventType
    {
        Insert,
        Update,
        Delete
    }

    #endregion
}