namespace Nop.Core.Domain.Customers;

/// <summary>
/// Customer activated event
/// </summary>
public partial class CustomerActivatedEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">customer</param>
    public CustomerActivatedEvent(Customer customer)
    {
		logger.LogDebug(">>> Entering CustomerActivatedEvent(customer={Customer})", customer);
        Customer = customer;
		logger.LogDebug("<<< Exiting CustomerActivatedEvent(customer={Customer})", customer);
		logger.LogDebug("CustomerActivatedEvent(customer={Customer}): Customer → {Customer1}", customer, Customer);
    }

    /// <summary>
    /// Customer
    /// </summary>
    public Customer Customer
    {
        get;
    }
}
