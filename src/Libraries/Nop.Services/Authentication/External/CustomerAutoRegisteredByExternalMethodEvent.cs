using Microsoft.Extensions.Logging;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Authentication.External;

/// <summary>
/// Customer auto registered by external authentication method event
/// </summary>
public partial class CustomerAutoRegisteredByExternalMethodEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="parameters">Parameters</param>
    public CustomerAutoRegisteredByExternalMethodEvent(Customer customer, ExternalAuthenticationParameters parameters)
    {
		_logger.LogDebug(">>> Entering CustomerAutoRegisteredByExternalMethodEvent(customer={Customer},parameters={Parameters})", customer, parameters);
        Customer = customer;
		_logger.LogDebug("CustomerAutoRegisteredByExternalMethodEvent(customer={Customer},parameters={Parameters}): Customer → {Customer1}", customer, parameters, Customer);
        AuthenticationParameters = parameters;
    }

    /// <summary>
    /// Gets or sets customer
    /// </summary>
    public Customer Customer { get; }

    /// <summary>
    /// Gets or sets external authentication parameters
    /// </summary>
    public ExternalAuthenticationParameters AuthenticationParameters { get; }
}