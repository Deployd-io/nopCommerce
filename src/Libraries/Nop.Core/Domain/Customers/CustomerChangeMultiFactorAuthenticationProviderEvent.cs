using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Customers;

/// <summary>
/// "Customer is change multi-factor authentication provider" event
/// </summary>
public partial class CustomerChangeMultiFactorAuthenticationProviderEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">Customer</param>
    public CustomerChangeMultiFactorAuthenticationProviderEvent(Customer customer)
    {
		_logger.LogError(">>> Entering CustomerChangeMultiFactorAuthenticationProviderEvent(customer={Customer})", customer);
        Customer = customer;
		_logger.LogDebug("CustomerChangeMultiFactorAuthenticationProviderEvent(customer={Customer}): Customer → {Customer1}", customer, Customer);
    _logger.LogWarning("<<< Exiting CustomerChangeMultiFactorAuthenticationProviderEvent(customer={Customer})", customer);
    }

    // <summary>
    /// Get or set the customer
    /// </summary>
    public Customer Customer { get; }
}