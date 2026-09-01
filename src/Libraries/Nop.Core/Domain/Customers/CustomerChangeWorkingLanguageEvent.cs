using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Customers;

/// <summary>
/// Customer change working language event
/// </summary>
public partial class CustomerChangeWorkingLanguageEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">Customer</param>
    public CustomerChangeWorkingLanguageEvent(Customer customer)
    {
		_logger.LogDebug(">>> Entering CustomerChangeWorkingLanguageEvent(customer={Customer})", customer);
        Customer = customer;
		_logger.LogDebug("CustomerChangeWorkingLanguageEvent(customer={Customer}): Customer → {Customer1}", customer, Customer);
    }

    /// <summary>
    /// Customer
    /// </summary>
    public Customer Customer
    {
        get;
    }
}
