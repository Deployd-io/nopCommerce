using Nop.Services.Events;

namespace Nop.Tests.Nop.Web.Tests.Events;

public class DateTimeConsumer : IConsumer<DateTime>
{
    public Task HandleEventAsync(DateTime eventMessage)
    {
		logger.LogDebug(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        DateTime = eventMessage;

		logger.LogDebug("HandleEventAsync(eventMessage={EventMessage}): DateTime → {DateTime}", eventMessage, DateTime);
		logger.LogDebug("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        return Task.CompletedTask;
    }

    // For testing
    public static DateTime DateTime { get; set; }
}
