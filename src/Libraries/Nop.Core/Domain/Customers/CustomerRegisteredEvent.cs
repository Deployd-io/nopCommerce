using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Customers;

/// <summary>
/// Customer registered event
/// </summary>
public partial class CustomerRegisteredEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">customer</param>
    public CustomerRegisteredEvent(Customer customer)
    {
		_logger.LogError(">>> Entering CustomerRegisteredEvent(customer={Customer})", customer);
        Customer = customer;
		_logger.LogDebug("CustomerRegisteredEvent(customer={Customer}): Customer → {Customer1}", customer, Customer);
    _logger.LogWarning("<<< Exiting CustomerRegisteredEvent(customer={Customer})", customer);
    }

    /// <summary>
    /// Customer
    /// </summary>
    public Customer Customer
    {
        get;
    }
}