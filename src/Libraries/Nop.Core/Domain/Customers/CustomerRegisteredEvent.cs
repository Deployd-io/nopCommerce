namespace Nop.Core.Domain.Customers;

/// <summary>
/// Customer registered event
/// </summary>
public partial class CustomerRegisteredEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">customer</param>
    public CustomerRegisteredEvent(Customer customer)
    {
		logger.LogDebug(">>> Entering CustomerRegisteredEvent(customer={Customer})", customer);
        Customer = customer;
		logger.LogDebug("<<< Exiting CustomerRegisteredEvent(customer={Customer})", customer);
		logger.LogDebug("CustomerRegisteredEvent(customer={Customer}): Customer → {Customer1}", customer, Customer);
    }

    /// <summary>
    /// Customer
    /// </summary>
    public Customer Customer
    {
        get;
    }
}
