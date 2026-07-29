namespace Nop.Core.Domain.Customers;

/// <summary>
/// "Customer is logged out" event
/// </summary>
public partial class CustomerLoggedOutEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">Customer</param>
    public CustomerLoggedOutEvent(Customer customer)
    {
		logger.LogDebug(">>> Entering CustomerLoggedOutEvent(customer={Customer})", customer);
        Customer = customer;
		logger.LogDebug("<<< Exiting CustomerLoggedOutEvent(customer={Customer})", customer);
		logger.LogDebug("CustomerLoggedOutEvent(customer={Customer}): Customer → {Customer1}", customer, Customer);
    }

    /// <summary>
    /// Get or set the customer
    /// </summary>
    public Customer Customer { get; }
}