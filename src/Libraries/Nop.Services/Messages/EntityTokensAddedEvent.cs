using Microsoft.Extensions.Logging;
using Nop.Core;

namespace Nop.Services.Messages;

/// <summary>
/// A container for tokens that are added.
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public partial class EntityTokensAddedEvent<T> where T : BaseEntity
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="tokens">Tokens</param>
    public EntityTokensAddedEvent(T entity, IList<Token> tokens)
    {
		_logger.LogDebug(">>> Entering EntityTokensAddedEvent(entity={Entity},tokens=<redacted>)", entity);
        Entity = entity;
		_logger.LogDebug("EntityTokensAddedEvent(entity={Entity},tokens=<redacted>): Entity → {Entity1}", entity, Entity);
        Tokens = tokens;
    }

    /// <summary>
    /// Entity
    /// </summary>
    public T Entity { get; }

    /// <summary>
    /// Tokens
    /// </summary>
    public IList<Token> Tokens { get; }
}