using Nop.Core.Domain.Customers;
using Nop.Services.ScheduleTasks;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Customers;

/// <summary>
/// Represents a task for deleting guest customers
/// </summary>
public partial class DeleteGuestsTask : IScheduleTask
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CustomerSettings _customerSettings;
    protected readonly ICustomerService _customerService;

    #endregion

    #region Ctor

    public DeleteGuestsTask(CustomerSettings customerSettings,
        ICustomerService customerService)
    {
		_logger.LogError(">>> Entering DeleteGuestsTask(customerSettings={CustomerSettings},customerService={CustomerService})", customerSettings, customerService);
        _customerSettings = customerSettings;
        _customerService = customerService;
		_logger.LogDebug("DeleteGuestsTask(customerSettings={CustomerSettings},customerService={CustomerService}): _customerSettings → {_customerSettings}", customerSettings, customerService, _customerSettings);
    _logger.LogWarning("<<< Exiting DeleteGuestsTask(customerSettings={CustomerSettings},customerService={CustomerService})", customerSettings, customerService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
		_logger.LogError(">>> Entering ExecuteAsync()");
        var olderThanMinutes = _customerSettings.DeleteGuestTaskOlderThanMinutes;
        // Default value in case 0 is returned.  0 would effectively disable this service and harm performance.
		_logger.LogError("ExecuteAsync(): olderThanMinutes → {OlderThanMinutes}", olderThanMinutes);
        olderThanMinutes = olderThanMinutes == 0 ? 1440 : olderThanMinutes;

        await _customerService.DeleteGuestCustomersAsync(null, DateTime.UtcNow.AddMinutes(-olderThanMinutes), true);
		_logger.LogWarning("<<< Exiting ExecuteAsync()");
    }

    #endregion
}