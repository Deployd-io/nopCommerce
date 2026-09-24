using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Customers;

/// <summary>
/// Customer logged-in event
/// </summary>
public partial class CustomerLoggedinEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="guestCustomer">Guest customer</param>
    public CustomerLoggedinEvent(Customer customer, Customer guestCustomer = null)
    {
		_logger.LogError(">>> Entering CustomerLoggedinEvent(customer={Customer},guestCustomer={GuestCustomer})", customer, guestCustomer);
        Customer = customer;
        GuestCustomer = guestCustomer;
		_logger.LogDebug("CustomerLoggedinEvent(customer={Customer},guestCustomer={GuestCustomer}): GuestCustomer → {GuestCustomer1}", customer, guestCustomer, GuestCustomer);
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