using Nop.Core.Domain.Gdpr;
using Nop.Services.Customers;
using Nop.Services.ScheduleTasks;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Gdpr;

/// <summary>
/// Represents a task for deleting inactive customers
/// </summary>
public partial class DeleteInactiveCustomersTask : IScheduleTask
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IGdprService _gdprService;
    protected readonly GdprSettings _gdprSettings;

    #endregion

    #region Ctor

    public DeleteInactiveCustomersTask(ICustomerService customerService,
        IGdprService gdprService,
        GdprSettings gdprSettings)
    {
		_logger.LogDebug(">>> Entering DeleteInactiveCustomersTask(customerService={CustomerService},gdprService={GdprService},gdprSettings={GdprSettings})", customerService, gdprService, gdprSettings);
        _customerService = customerService;
        _gdprService = gdprService;
		_logger.LogDebug("DeleteInactiveCustomersTask(customerService={CustomerService},gdprService={GdprService},gdprSettings={GdprSettings}): _gdprService → {_gdprService}", customerService, gdprService, gdprSettings, _gdprService);
		_logger.LogDebug("DeleteInactiveCustomersTask(customerService={CustomerService},gdprService={GdprService},gdprSettings={GdprSettings}): _customerService → {_customerService}", customerService, gdprService, gdprSettings, _customerService);
        _gdprSettings = gdprSettings;
		_logger.LogDebug("<<< Exiting DeleteInactiveCustomersTask(customerService={CustomerService},gdprService={GdprService},gdprSettings={GdprSettings})", customerService, gdprService, gdprSettings);
		_logger.LogDebug("DeleteInactiveCustomersTask(customerService={CustomerService},gdprService={GdprService},gdprSettings={GdprSettings}): _gdprSettings → {_gdprSettings}", customerService, gdprService, gdprSettings, _gdprSettings);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
		_logger.LogDebug(">>> Entering ExecuteAsync()");
        if (!_gdprSettings.GdprEnabled)
            return;

        var lastActivityToUtc = DateTime.UtcNow.AddMonths(-_gdprSettings.DeleteInactiveCustomersAfterMonths);
		_logger.LogDebug("ExecuteAsync(): lastActivityToUtc → {LastActivityToUtc}", lastActivityToUtc);
        var inactiveCustomers = await _customerService.GetAllCustomersAsync(lastActivityToUtc: lastActivityToUtc);
		_logger.LogDebug("ExecuteAsync(): inactiveCustomers → {InactiveCustomers}", inactiveCustomers);

        foreach (var customer in inactiveCustomers.Where(c => !c.IsSystemAccount))
            await _gdprService.PermanentDeleteCustomerAsync(customer);
			_logger.LogDebug("<<< Exiting ExecuteAsync()");
    }

    #endregion
}