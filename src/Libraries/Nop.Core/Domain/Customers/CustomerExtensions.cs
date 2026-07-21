namespace Nop.Core.Domain.Customers;

/// <summary>
/// Customer extensions
/// </summary>
public static class CustomerExtensions
{
    /// <summary>
    /// Gets a value indicating whether customer a search engine
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>Result</returns>
    public static bool IsSearchEngineAccount(this Customer customer)
    {
		logger.LogDebug(">>> Entering IsSearchEngineAccount(customer={Customer})", customer);
        ArgumentNullException.ThrowIfNull(customer);

        if (!customer.IsSystemAccount || string.IsNullOrEmpty(customer.SystemName))
            return false;

        var result = customer.SystemName.Equals(NopCustomerDefaults.SearchEngineCustomerName, StringComparison.InvariantCultureIgnoreCase);

		logger.LogDebug("IsSearchEngineAccount(customer={Customer}): result → {Result}", customer, result);
		logger.LogDebug("<<< Exiting IsSearchEngineAccount(customer={Customer})", customer);
        return result;
    }

    /// <summary>
    /// Gets a value indicating whether the customer is a built-in record for background tasks
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>Result</returns>
    public static bool IsBackgroundTaskAccount(this Customer customer)
    {
		logger.LogDebug(">>> Entering IsBackgroundTaskAccount(customer={Customer})", customer);
        ArgumentNullException.ThrowIfNull(customer);

        if (!customer.IsSystemAccount || string.IsNullOrEmpty(customer.SystemName))
            return false;

        var result = customer.SystemName.Equals(NopCustomerDefaults.BackgroundTaskCustomerName, StringComparison.InvariantCultureIgnoreCase);

		logger.LogDebug("IsBackgroundTaskAccount(customer={Customer}): result → {Result}", customer, result);
		logger.LogDebug("<<< Exiting IsBackgroundTaskAccount(customer={Customer})", customer);
        return result;
    }
}
