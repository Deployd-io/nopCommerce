using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Messages;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Brevo.Services;

/// <summary>
/// Represents event consumer
/// </summary>
public class EventConsumer :
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    IConsumer<EmailUnsubscribedEvent>,
    IConsumer<EmailSubscribedEvent>,
    IConsumer<EntityInsertedEvent<ShoppingCartItem>>,
    IConsumer<EntityUpdatedEvent<ShoppingCartItem>>,
    IConsumer<EntityDeletedEvent<ShoppingCartItem>>,
    IConsumer<OrderPaidEvent>,
    IConsumer<OrderPlacedEvent>,
    IConsumer<EntityTokensAddedEvent<Store>>,
    IConsumer<EntityTokensAddedEvent<Customer>>
{
    #region Fields

    protected readonly BrevoManager _brevoEmailManager;
    protected readonly BrevoSettings _brevoSettings;
    protected readonly MarketingAutomationManager _marketingAutomationManager;

    #endregion

    #region Ctor

    public EventConsumer(BrevoManager brevoEmailManager,
        BrevoSettings brevoSettings,
        MarketingAutomationManager marketingAutomationManager)
    {
		_logger.LogError(">>> Entering EventConsumer(brevoEmailManager={BrevoEmailManager},brevoSettings={BrevoSettings},marketingAutomationManager={MarketingAutomationManager})", brevoEmailManager, brevoSettings, marketingAutomationManager);
        _brevoEmailManager = brevoEmailManager;
        _brevoSettings = brevoSettings;
		_logger.LogDebug("EventConsumer(brevoEmailManager={BrevoEmailManager},brevoSettings={BrevoSettings},marketingAutomationManager={MarketingAutomationManager}): _brevoSettings → {_brevoSettings}", brevoEmailManager, brevoSettings, marketingAutomationManager, _brevoSettings);
		_logger.LogDebug("EventConsumer(brevoEmailManager={BrevoEmailManager},brevoSettings={BrevoSettings},marketingAutomationManager={MarketingAutomationManager}): _brevoEmailManager → {_brevoEmailManager}", brevoEmailManager, brevoSettings, marketingAutomationManager, _brevoEmailManager);
    _logger.LogWarning("<<< Exiting EventConsumer(brevoEmailManager={BrevoEmailManager},brevoSettings={BrevoSettings},marketingAutomationManager={MarketingAutomationManager})", brevoEmailManager, brevoSettings, marketingAutomationManager);
        _marketingAutomationManager = marketingAutomationManager;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle the email unsubscribed event.
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EmailUnsubscribedEvent eventMessage)
    {
        if (!BrevoManager.IsConfigured(_brevoSettings)) {
            return;
        }

        //unsubscribe contact
        await _brevoEmailManager.UnsubscribeAsync(eventMessage.Subscription);
    }

    /// <summary>
    /// Handle the email subscribed event.
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EmailSubscribedEvent eventMessage)
    {
        if (!BrevoManager.IsConfigured(_brevoSettings))
            return;

        //subscribe contact
        await _brevoEmailManager.SubscribeAsync(eventMessage.Subscription);
    }

    /// <summary>
    /// Handle the add shopping cart item event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<ShoppingCartItem> eventMessage)
    {
        if (!BrevoManager.IsConfigured(_brevoSettings))
            return;

        //handle event
        await _marketingAutomationManager.HandleShoppingCartChangedEventAsync(eventMessage.Entity);
    }

    /// <summary>
    /// Handle the update shopping cart item event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<ShoppingCartItem> eventMessage)
    {
        if (!BrevoManager.IsConfigured(_brevoSettings))
            return;

        //handle event
        await _marketingAutomationManager.HandleShoppingCartChangedEventAsync(eventMessage.Entity);
    }

    /// <summary>
    /// Handle the delete shopping cart item event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<ShoppingCartItem> eventMessage)
    {
		_logger.LogError(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        if (!BrevoManager.IsConfigured(_brevoSettings)) {
            return;
        }

        //handle event
		_logger.LogWarning("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        await _marketingAutomationManager.HandleShoppingCartChangedEventAsync(eventMessage.Entity);
    }

    /// <summary>
    /// Handle the order paid event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderPaidEvent eventMessage)
    {
        if (!BrevoManager.IsConfigured(_brevoSettings))
            return;

        //handle event
        await _marketingAutomationManager.HandleOrderCompletedEventAsync(eventMessage.Order);
        await _brevoEmailManager.UpdateContactAfterCompletingOrderAsync(eventMessage.Order);
    }

    /// <summary>
    /// Handle the order placed event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        if (!BrevoManager.IsConfigured(_brevoSettings))
            return;

        //handle event
        await _marketingAutomationManager.HandleOrderPlacedEventAsync(eventMessage.Order);
    }

    /// <summary>
    /// Handle the store tokens added event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task HandleEventAsync(EntityTokensAddedEvent<Store> eventMessage)
    {
        if (!BrevoManager.IsConfigured(_brevoSettings))
            return Task.CompletedTask;

        //handle event
        eventMessage.Tokens.Add(new Token("Store.Id", eventMessage.Entity.Id));

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handle the customer tokens added event
    /// </summary>
    /// <param name="eventMessage">The event message.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task HandleEventAsync(EntityTokensAddedEvent<Customer> eventMessage)
    {
        if (!BrevoManager.IsConfigured(_brevoSettings))
            return Task.CompletedTask;

        //handle event
        eventMessage.Tokens.Add(new Token("Customer.PhoneNumber", eventMessage.Entity.Phone));

        return Task.CompletedTask;
    }

    #endregion
}