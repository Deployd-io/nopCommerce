using Microsoft.Extensions.Logging;
﻿namespace Nop.Core.Events;

/// <summary>
/// A container for entities that are updated.
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class EntityUpdatedEvent<T> where T : BaseEntity
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="entity">Entity</param>
    public EntityUpdatedEvent(T entity)
    {
		_logger.LogDebug(">>> Entering EntityUpdatedEvent(entity={Entity})", entity);
        Entity = entity;
		_logger.LogDebug("<<< Exiting EntityUpdatedEvent(entity={Entity})", entity);
		_logger.LogDebug("EntityUpdatedEvent(entity={Entity}): Entity → {Entity1}", entity, Entity);
    }

    /// <summary>
    /// Entity
    /// </summary>
    public T Entity { get; }
}