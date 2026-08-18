using Microsoft.Extensions.Logging;
﻿namespace Nop.Core.Domain.Customers;

/// <summary>
/// Customer activated event
/// </summary>
public partial class CustomerActivatedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">customer</param>
    public CustomerActivatedEvent(Customer customer)
    {
		_logger.LogDebug(">>> Entering CustomerActivatedEvent(customer={Customer})", customer);
        Customer = customer;
		_logger.LogDebug("CustomerActivatedEvent(customer={Customer}): Customer → {Customer1}", customer, Customer);
		_logger.LogDebug("<<< Exiting CustomerActivatedEvent(customer={Customer})", customer);
    }

    /// <summary>
    /// Customer
    /// </summary>
    public Customer Customer
    {
        get;
    }
}