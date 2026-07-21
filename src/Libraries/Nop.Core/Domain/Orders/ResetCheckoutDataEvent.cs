using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Orders;

/// <summary>
/// Reset checkout data event
/// </summary>
public partial class ResetCheckoutDataEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="storeId">Store identifier</param>
    public ResetCheckoutDataEvent(Customer customer, int storeId)
    {
		logger.LogDebug(">>> Entering ResetCheckoutDataEvent(customer={Customer},storeId={StoreId})", customer, storeId);
        Customer = customer;
        StoreId = storeId;
		logger.LogDebug("<<< Exiting ResetCheckoutDataEvent(customer={Customer},storeId={StoreId})", customer, storeId);
		logger.LogDebug("ResetCheckoutDataEvent(customer={Customer},storeId={StoreId}): StoreId → {StoreId1}", customer, storeId, StoreId);
		logger.LogDebug("ResetCheckoutDataEvent(customer={Customer},storeId={StoreId}): Customer → {Customer1}", customer, storeId, Customer);
    }

    /// <summary>
    /// Customer
    /// </summary>
    public Customer Customer { get; }

    /// <summary>
    /// Store identifier
    /// </summary>
    public int StoreId { get; }
}
