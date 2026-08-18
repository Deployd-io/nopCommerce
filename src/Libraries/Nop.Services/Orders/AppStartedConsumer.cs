using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Events;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Orders;

/// <summary>
/// Represents AppStarted event consumer
/// </summary>
public partial class AppStartedConsumer : IConsumer<AppStartedEvent>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IGenericAttributeService _genericAttributeService;

    #endregion

    #region Ctor

    public AppStartedConsumer(IGenericAttributeService genericAttributeService)
    {
		_logger.LogDebug(">>> Entering AppStartedConsumer(genericAttributeService={GenericAttributeService})", genericAttributeService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("AppStartedConsumer(genericAttributeService={GenericAttributeService}): _genericAttributeService → {_genericAttributeService}", genericAttributeService, _genericAttributeService);
		_logger.LogDebug("<<< Exiting AppStartedConsumer(genericAttributeService={GenericAttributeService})", genericAttributeService);
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
		_logger.LogDebug(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //clear payment info requests
        await _genericAttributeService.DeleteAttributesAsync<Customer>(NopCustomerDefaults.ProcessPaymentRequestAttribute);
		_logger.LogDebug("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
    }

    #endregion
}
