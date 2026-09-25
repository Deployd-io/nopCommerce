using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Customers;

/// <summary>
/// Customer extensions
/// </summary>
public static class CustomerExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Gets a value indicating whether customer a search engine
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>Result</returns>
    public static bool IsSearchEngineAccount(this Customer customer)
    {
		_logger.LogError(">>> Entering IsSearchEngineAccount(customer={Customer})", customer);
        ArgumentNullException.ThrowIfNull(customer);

        if (!customer.IsSystemAccount || string.IsNullOrEmpty(customer.SystemName))
            return false;

        var result = customer.SystemName.Equals(NopCustomerDefaults.SearchEngineCustomerName, StringComparison.InvariantCultureIgnoreCase);
		_logger.LogError("IsSearchEngineAccount(customer={Customer}): result → {Result}", customer, result);

		_logger.LogWarning("<<< Exiting IsSearchEngineAccount(customer={Customer})", customer);
        return result;
    }

    /// <summary>
    /// Gets a value indicating whether the customer is a built-in record for background tasks
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>Result</returns>
    public static bool IsBackgroundTaskAccount(this Customer customer)
    {
		_logger.LogError(">>> Entering IsBackgroundTaskAccount(customer={Customer})", customer);
        ArgumentNullException.ThrowIfNull(customer);

        if (!customer.IsSystemAccount || string.IsNullOrEmpty(customer.SystemName))
            return false;

        var result = customer.SystemName.Equals(NopCustomerDefaults.BackgroundTaskCustomerName, StringComparison.InvariantCultureIgnoreCase);
		_logger.LogError("IsBackgroundTaskAccount(customer={Customer}): result → {Result}", customer, result);

		_logger.LogWarning("<<< Exiting IsBackgroundTaskAccount(customer={Customer})", customer);
        return result;
    }
}