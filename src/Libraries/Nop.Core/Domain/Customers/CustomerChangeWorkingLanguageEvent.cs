namespace Nop.Core.Domain.Customers;

/// <summary>
/// Customer change working language event
/// </summary>
public partial class CustomerChangeWorkingLanguageEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">Customer</param>
    public CustomerChangeWorkingLanguageEvent(Customer customer)
    {
		logger.LogDebug(">>> Entering CustomerChangeWorkingLanguageEvent(customer={Customer})", customer);
        Customer = customer;
		logger.LogDebug("<<< Exiting CustomerChangeWorkingLanguageEvent(customer={Customer})", customer);
		logger.LogDebug("CustomerChangeWorkingLanguageEvent(customer={Customer}): Customer → {Customer1}", customer, Customer);
    }

    /// <summary>
    /// Customer
    /// </summary>
    public Customer Customer
    {
        get;
    }
}
