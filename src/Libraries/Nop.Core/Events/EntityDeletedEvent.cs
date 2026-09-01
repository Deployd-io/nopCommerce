using Microsoft.Extensions.Logging;
namespace Nop.Core.Events;

/// <summary>
/// A container for passing entities that have been deleted. This is not used for entities that are deleted logically via a bit column.
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class EntityDeletedEvent<T> where T : BaseEntity
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="entity">Entity</param>
    public EntityDeletedEvent(T entity)
    {
		_logger.LogDebug(">>> Entering EntityDeletedEvent(entity={Entity})", entity);
        Entity = entity;
		_logger.LogDebug("EntityDeletedEvent(entity={Entity}): Entity → {Entity1}", entity, Entity);
    }

    /// <summary>
    /// Entity
    /// </summary>
    public T Entity { get; }
}