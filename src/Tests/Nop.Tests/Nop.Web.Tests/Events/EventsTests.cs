using FluentAssertions;
using Nop.Core.Events;
using Nop.Services.Events;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Events;

[TestFixture]
public class EventsTests : BaseNopTest
{
    private IEventPublisher _eventPublisher;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _eventPublisher = GetService<IEventPublisher>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _eventPublisher → {_eventPublisher}", _eventPublisher);
    }

    [Test]
    public async Task CanPublishEvent()
    {
		logger.LogDebug(">>> Entering CanPublishEvent()");
        var oldDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(7));
        DateTimeConsumer.DateTime = oldDateTime;
		logger.LogDebug("CanPublishEvent(): oldDateTime → {OldDateTime}", oldDateTime);
		logger.LogDebug("CanPublishEvent(): DateTimeConsumer.DateTime → {Arg0}", DateTimeConsumer.DateTime);

        var newDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(5));
        await _eventPublisher.PublishAsync(newDateTime);
		logger.LogDebug("CanPublishEvent(): newDateTime → {NewDateTime}", newDateTime);
        newDateTime.Should().Be(DateTimeConsumer.DateTime);
		logger.LogDebug("<<< Exiting CanPublishEvent()");
    }

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
}