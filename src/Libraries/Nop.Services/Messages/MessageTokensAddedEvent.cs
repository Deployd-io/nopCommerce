using Microsoft.Extensions.Logging;
﻿using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages;

/// <summary>
/// A container for tokens that are added.
/// </summary>
public partial class MessageTokensAddedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="tokens">Tokens</param>
    public MessageTokensAddedEvent(MessageTemplate message, IList<Token> tokens)
    {
        Message = message;
        Tokens = tokens;
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