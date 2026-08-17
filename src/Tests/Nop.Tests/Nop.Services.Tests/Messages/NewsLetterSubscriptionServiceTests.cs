using FluentAssertions;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Messages;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Messages;

[TestFixture]
public class NewsLetterSubscriptionServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private INewsLetterSubscriptionService _newsLetterSubscriptionService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _newsLetterSubscriptionService = GetService<INewsLetterSubscriptionService>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _newsLetterSubscriptionService → {_newsLetterSubscriptionService}", _newsLetterSubscriptionService);
    }

    /// <summary>
    /// Verifies the active insert triggers subscribe event.
    /// </summary>
    [Test]
    public async Task VerifyActiveInsertTriggersSubscribeEvent()
    {
		_logger.LogDebug(">>> Entering VerifyActiveInsertTriggersSubscribeEvent()");
        var subscription = new NewsLetterSubscription
        { 
            Active = true, 
            Email = "test@test.com",
            TypeId = 1
        };
        await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);

        var eventType = NewsLetterSubscriptionConsumer.LastEventType;

        await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);

        eventType.Should().Be(typeof(EmailSubscribedEvent));
		_logger.LogDebug("<<< Exiting VerifyActiveInsertTriggersSubscribeEvent()");
    }

    /// <summary>
    /// Verifies the delete triggers unsubscribe event.
    /// </summary>
    [Test]
    public async Task VerifyDeleteTriggersUnsubscribeEvent()
    {
		_logger.LogDebug(">>> Entering VerifyDeleteTriggersUnsubscribeEvent()");
        var subscription = new NewsLetterSubscription
        {
            Active = true,
            Email = "test@test.com",
            TypeId = 1
        };
        await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);
        await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);

        NewsLetterSubscriptionConsumer.LastEventType.Should().Be(typeof(EmailUnsubscribedEvent));
		_logger.LogDebug("<<< Exiting VerifyDeleteTriggersUnsubscribeEvent()");
    }

    /// <summary>
    /// Verifies the insert event is fired.
    /// </summary>
    [Test]
    public async Task VerifyInsertEventIsFired()
    {
		_logger.LogDebug(">>> Entering VerifyInsertEventIsFired()");
        var subscription = new NewsLetterSubscription
        { 
            Email = "test@test.com",
            TypeId = 1
        };

        await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);

        var eventType = NewsLetterSubscriptionConsumer.LastEventType;

        await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);

        eventType.Should().Be(typeof(EntityInsertedEvent<NewsLetterSubscription>));
		_logger.LogDebug("<<< Exiting VerifyInsertEventIsFired()");
    }

    [Test]
    public async Task CanCRUD()
    {
		_logger.LogDebug(">>> Entering CanCRUD()");
        var guid = Guid.NewGuid();

		_logger.LogDebug("CanCRUD(): guid → {Guid}", guid);
        var subscription = new NewsLetterSubscription
		_logger.LogDebug("CanCRUD(): subscription → {Subscription}", subscription);
        {
            Active = true,
            Email = NopTestsDefaults.AdminEmail,
            NewsLetterSubscriptionGuid = guid,
            TypeId = 1
        };

        await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);
        subscription.Id.Should().BeGreaterThan(0);
        subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByIdAsync(subscription.Id);
        subscription.Active.Should().BeTrue();
        subscription.Active = false;
		_logger.LogDebug("CanCRUD(): subscription.Active → {Arg0}", subscription.Active);
        await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
        subscription = (await _newsLetterSubscriptionService.GetNewsLetterSubscriptionsByGuidAsync(guid)).FirstOrDefault();
        subscription.Active.Should().BeFalse();

        subscription = (await _newsLetterSubscriptionService.GetNewsLetterSubscriptionsByEmailAsync(NopTestsDefaults.AdminEmail)).FirstOrDefault();
        subscription.Should().NotBeNull();

        await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);
        subscription = (await _newsLetterSubscriptionService.GetNewsLetterSubscriptionsByGuidAsync(guid)).FirstOrDefault();
        subscription.Should().BeNull();
        subscription = (await _newsLetterSubscriptionService.GetNewsLetterSubscriptionsByEmailAsync(NopTestsDefaults.AdminEmail)).FirstOrDefault();
        subscription.Should().BeNull();
		_logger.LogDebug("<<< Exiting CanCRUD()");
    }

    [Test]
    public async Task CanGetAllNewsLetterSubscriptions()
    {
		_logger.LogDebug(">>> Entering CanGetAllNewsLetterSubscriptions()");
        var guid = Guid.NewGuid();

		_logger.LogDebug("CanGetAllNewsLetterSubscriptions(): guid → {Guid}", guid);
        var subscription = new NewsLetterSubscription
        {
            Active = true,
            Email = NopTestsDefaults.AdminEmail,
            NewsLetterSubscriptionGuid = guid,
            TypeId = 1
        };
        await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);

        var subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync();
		_logger.LogDebug("CanGetAllNewsLetterSubscriptions(): subscriptions → {Subscriptions}", subscriptions);
        subscriptions.Count.Should().Be(1);
        subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(NopTestsDefaults.AdminEmail);
        subscriptions.Count.Should().Be(1);
        subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(isActive: true);
        subscriptions.Count.Should().Be(1);
        subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(isActive: false);
        subscriptions.Count.Should().Be(0);

        subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(customerRoleId: 1);
        subscriptions.Count.Should().Be(1);
        subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(customerRoleId: 1, storeId: 1);
        subscriptions.Count.Should().Be(0);
        subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(NopTestsDefaults.AdminEmail, customerRoleId: 4);
        subscriptions.Count.Should().Be(0);


        await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);
		_logger.LogDebug("<<< Exiting CanGetAllNewsLetterSubscriptions()");
    }

    public class NewsLetterSubscriptionConsumer : IConsumer<EmailSubscribedEvent>, IConsumer<EmailUnsubscribedEvent>, IConsumer<EntityInsertedEvent<NewsLetterSubscription>>
    {
        public static Type LastEventType { get; set; }

        public Task HandleEventAsync(EmailSubscribedEvent eventMessage)
        {
            LastEventType = typeof(EmailSubscribedEvent);

			_logger.LogDebug("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
            return Task.CompletedTask;
        }

        public Task HandleEventAsync(EmailUnsubscribedEvent eventMessage)
        {
            LastEventType = typeof(EmailUnsubscribedEvent);

            return Task.CompletedTask;
        }

        public Task HandleEventAsync(EntityInsertedEvent<NewsLetterSubscription> eventMessage)
        {
            LastEventType = typeof(EntityInsertedEvent<NewsLetterSubscription>);

			_logger.LogDebug("HandleEventAsync(eventMessage={EventMessage}): LastEventType → {LastEventType}", eventMessage, LastEventType);
            return Task.CompletedTask;
        }
    }
}