using Nop.Core.Domain.Messages;
using Nop.Services.Events;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.RFQ.Services;

public class RfqMessageTokenEventConsumer : IConsumer<AdditionalTokensAddedEvent>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task HandleEventAsync(AdditionalTokensAddedEvent eventMessage)
    {
		_logger.LogDebug(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        eventMessage.AddTokens("%RequestQuote.Id%", "%RequestQuote.CreatedOn%", "%RequestQuote.URL%", "%Quote.Id%", "%Quote.CreatedOn%", "%Quote.ExpirationOn%", "%Quote.ExpirationOnIsSet%", "%Quote.URL%");

		_logger.LogDebug("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        return Task.CompletedTask;
    }

    #endregion
}
