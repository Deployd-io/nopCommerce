using Microsoft.Extensions.Logging;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Orders;

/// <summary>
/// Reset checkout data event
/// </summary>
public partial class ResetCheckoutDataEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="storeId">Store identifier</param>
    public ResetCheckoutDataEvent(Customer customer, int storeId)
    {
		_logger.LogError(">>> Entering ResetCheckoutDataEvent(customer={Customer},storeId={StoreId})", customer, storeId);
        Customer = customer;
        StoreId = storeId;
		_logger.LogDebug("ResetCheckoutDataEvent(customer={Customer},storeId={StoreId}): Customer → {Customer1}", customer, storeId, Customer);
    _logger.LogWarning("<<< Exiting ResetCheckoutDataEvent(customer={Customer},storeId={StoreId})", customer, storeId);
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