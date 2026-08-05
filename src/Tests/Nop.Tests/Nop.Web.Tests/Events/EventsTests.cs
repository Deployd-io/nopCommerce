using FluentAssertions;
using Nop.Core.Events;
using Nop.Services.Events;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Events;

[TestFixture]
public class EventsTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IEventPublisher _eventPublisher;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _eventPublisher = GetService<IEventPublisher>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _eventPublisher → {_eventPublisher}", _eventPublisher);
    }

    [Test]
    public async Task CanPublishEvent()
    {
		_logger.LogDebug(">>> Entering CanPublishEvent()");
        var oldDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(7));
        DateTimeConsumer.DateTime = oldDateTime;
		_logger.LogDebug("CanPublishEvent(): oldDateTime → {OldDateTime}", oldDateTime);
		_logger.LogDebug("CanPublishEvent(): DateTimeConsumer.DateTime → {Arg0}", DateTimeConsumer.DateTime);

        var newDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(5));
        await _eventPublisher.PublishAsync(newDateTime);
		_logger.LogDebug("CanPublishEvent(): newDateTime → {NewDateTime}", newDateTime);
        newDateTime.Should().Be(DateTimeConsumer.DateTime);
		_logger.LogDebug("<<< Exiting CanPublishEvent()");
    }

    public class DateTimeConsumer : IConsumer<DateTime>
    {
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
}