using Microsoft.Extensions.Logging;
﻿using Nop.Services.Events;

namespace Nop.Tests.Nop.Web.Tests.Events;

public class DateTimeConsumer : IConsumer<DateTime>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public Task HandleEventAsync(DateTime eventMessage)
    {
		_logger.LogDebug(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        DateTime = eventMessage;

		_logger.LogDebug("HandleEventAsync(eventMessage={EventMessage}): DateTime → {DateTime}", eventMessage, DateTime);
		_logger.LogDebug("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        return Task.CompletedTask;
    }

    // For testing
    public static DateTime DateTime { get; set; }
}