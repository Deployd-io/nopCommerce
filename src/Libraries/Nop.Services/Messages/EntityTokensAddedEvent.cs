using Nop.Core;

namespace Nop.Services.Messages;

/// <summary>
/// A container for tokens that are added.
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public partial class EntityTokensAddedEvent<T> where T : BaseEntity
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="tokens">Tokens</param>
    public EntityTokensAddedEvent(T entity, IList<Token> tokens)
    {
		logger.LogDebug(">>> Entering EntityTokensAddedEvent(entity={Entity},tokens={Tokens})", entity, tokens);
        Entity = entity;
        Tokens = tokens;
		logger.LogDebug("<<< Exiting EntityTokensAddedEvent(entity={Entity},tokens={Tokens})", entity, tokens);
		logger.LogDebug("EntityTokensAddedEvent(entity={Entity},tokens={Tokens}): Tokens → {Tokens1}", entity, tokens, Tokens);
		logger.LogDebug("EntityTokensAddedEvent(entity={Entity},tokens={Tokens}): Entity → {Entity1}", entity, tokens, Entity);
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
