namespace Nop.Core.Domain.Customers;

/// <summary>
/// "Customer is change multi-factor authentication provider" event
/// </summary>
public partial class CustomerChangeMultiFactorAuthenticationProviderEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">Customer</param>
    public CustomerChangeMultiFactorAuthenticationProviderEvent(Customer customer)
    {
		logger.LogDebug(">>> Entering CustomerChangeMultiFactorAuthenticationProviderEvent(customer={Customer})", customer);
        Customer = customer;
		logger.LogDebug("<<< Exiting CustomerChangeMultiFactorAuthenticationProviderEvent(customer={Customer})", customer);
		logger.LogDebug("CustomerChangeMultiFactorAuthenticationProviderEvent(customer={Customer}): Customer → {Customer1}", customer, Customer);
    }

    // <summary>
    /// Get or set the customer
    /// </summary>
    public Customer Customer { get; }
}