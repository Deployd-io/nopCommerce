namespace Nop.Core.Events;

/// <summary>
/// A container for entities that are updated.
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class EntityUpdatedEvent<T> where T : BaseEntity
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="entity">Entity</param>
    public EntityUpdatedEvent(T entity)
    {
		logger.LogDebug(">>> Entering EntityUpdatedEvent(entity={Entity})", entity);
        Entity = entity;
		logger.LogDebug("<<< Exiting EntityUpdatedEvent(entity={Entity})", entity);
		logger.LogDebug("EntityUpdatedEvent(entity={Entity}): Entity → {Entity1}", entity, Entity);
    }

    /// <summary>
    /// Entity
    /// </summary>
    public T Entity { get; }
}
