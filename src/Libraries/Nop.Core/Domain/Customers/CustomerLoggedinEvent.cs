namespace Nop.Core.Domain.Customers;

/// <summary>
/// Customer logged-in event
/// </summary>
public partial class CustomerLoggedinEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="guestCustomer">Guest customer</param>
    public CustomerLoggedinEvent(Customer customer, Customer guestCustomer = null)
    {
		logger.LogDebug(">>> Entering CustomerLoggedinEvent(customer={Customer},guestCustomer={GuestCustomer})", customer, guestCustomer);
        Customer = customer;
        GuestCustomer = guestCustomer;
		logger.LogDebug("<<< Exiting CustomerLoggedinEvent(customer={Customer},guestCustomer={GuestCustomer})", customer, guestCustomer);
		logger.LogDebug("CustomerLoggedinEvent(customer={Customer},guestCustomer={GuestCustomer}): GuestCustomer → {GuestCustomer1}", customer, guestCustomer, GuestCustomer);
		logger.LogDebug("CustomerLoggedinEvent(customer={Customer},guestCustomer={GuestCustomer}): Customer → {Customer1}", customer, guestCustomer, Customer);
    }

    /// <summary>
    /// Customer
    /// </summary>
    public Customer Customer
    {
        get;
    }

    /// <summary>
    /// Guest customer
    /// </summary>
    public Customer GuestCustomer
    {
        get;
    }
}