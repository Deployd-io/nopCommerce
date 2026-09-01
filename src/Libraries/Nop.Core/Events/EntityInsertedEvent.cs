using Microsoft.Extensions.Logging;
namespace Nop.Core.Events;

/// <summary>
/// A container for entities that have been inserted.
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class EntityInsertedEvent<T> where T : BaseEntity
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="entity">Entity</param>
    public EntityInsertedEvent(T entity)
    {
		_logger.LogDebug(">>> Entering EntityInsertedEvent(entity={Entity})", entity);
        Entity = entity;
		_logger.LogDebug("EntityInsertedEvent(entity={Entity}): Entity → {Entity1}", entity, Entity);
    }

    /// <summary>
    /// Entity
    /// </summary>
    public T Entity { get; }
}