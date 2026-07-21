namespace Nop.Core.Events;

/// <summary>
/// A container for entities that have been inserted.
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class EntityInsertedEvent<T> where T : BaseEntity
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="entity">Entity</param>
    public EntityInsertedEvent(T entity)
    {
		logger.LogDebug(">>> Entering EntityInsertedEvent(entity={Entity})", entity);
        Entity = entity;
		logger.LogDebug("<<< Exiting EntityInsertedEvent(entity={Entity})", entity);
		logger.LogDebug("EntityInsertedEvent(entity={Entity}): Entity → {Entity1}", entity, Entity);
    }

    /// <summary>
    /// Entity
    /// </summary>
    public T Entity { get; }
}
