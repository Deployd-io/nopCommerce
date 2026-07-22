using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages;

/// <summary>
/// A container for tokens that are added.
/// </summary>
public partial class MessageTokensAddedEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="tokens">Tokens</param>
    public MessageTokensAddedEvent(MessageTemplate message, IList<Token> tokens)
    {
		logger.LogDebug(">>> Entering MessageTokensAddedEvent(message={Message},tokens={Tokens})", message, tokens);
        Message = message;
        Tokens = tokens;
		logger.LogDebug("<<< Exiting MessageTokensAddedEvent(message={Message},tokens={Tokens})", message, tokens);
		logger.LogDebug("MessageTokensAddedEvent(message={Message},tokens={Tokens}): Tokens → {Tokens1}", message, tokens, Tokens);
		logger.LogDebug("MessageTokensAddedEvent(message={Message},tokens={Tokens}): Message → {Message1}", message, tokens, Message);
    }

    /// <summary>
    /// Message
    /// </summary>
    public MessageTemplate Message { get; }

    /// <summary>
    /// Tokens
    /// </summary>
    public IList<Token> Tokens { get; }
}
