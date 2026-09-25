using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Customers;

/// <summary>
/// "Customer is logged out" event
/// </summary>
public partial class CustomerLoggedOutEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">Customer</param>
    public CustomerLoggedOutEvent(Customer customer)
    {
		_logger.LogError(">>> Entering CustomerLoggedOutEvent(customer={Customer})", customer);
        Customer = customer;
		_logger.LogDebug("CustomerLoggedOutEvent(customer={Customer}): Customer → {Customer1}", customer, Customer);
    _logger.LogWarning("<<< Exiting CustomerLoggedOutEvent(customer={Customer})", customer);
    }

    /// <summary>
    /// Get or set the customer
    /// </summary>
    public Customer Customer { get; }
}