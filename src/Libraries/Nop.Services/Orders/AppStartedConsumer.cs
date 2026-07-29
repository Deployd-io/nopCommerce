using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Events;

namespace Nop.Services.Orders;

/// <summary>
/// Represents AppStarted event consumer
/// </summary>
public partial class AppStartedConsumer : IConsumer<AppStartedEvent>
{
    #region Fields

    protected readonly IGenericAttributeService _genericAttributeService;

    #endregion

    #region Ctor

    public AppStartedConsumer(IGenericAttributeService genericAttributeService)
    {
		logger.LogDebug(">>> Entering AppStartedConsumer(genericAttributeService={GenericAttributeService})", genericAttributeService);
        _genericAttributeService = genericAttributeService;
		logger.LogDebug("<<< Exiting AppStartedConsumer(genericAttributeService={GenericAttributeService})", genericAttributeService);
		logger.LogDebug("AppStartedConsumer(genericAttributeService={GenericAttributeService}): _genericAttributeService → {_genericAttributeService}", genericAttributeService, _genericAttributeService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(AppStartedEvent eventMessage)
    {
		logger.LogDebug(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //clear payment info requests
        await _genericAttributeService.DeleteAttributesAsync<Customer>(NopCustomerDefaults.ProcessPaymentRequestAttribute);
		logger.LogDebug("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
    }

    #endregion
}
