using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Customers;

/// <summary>
/// Customer password changed event
/// </summary>
public partial class CustomerPasswordChangedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="password">Password</param>
    public CustomerPasswordChangedEvent(CustomerPassword password)
    {
		_logger.LogDebug(">>> Entering CustomerPasswordChangedEvent(password=<redacted>)");
        Password = password;
		_logger.LogWarning("CustomerPasswordChangedEvent(password=<redacted>): Password → {Password1}", Password);
    }

    /// <summary>
    /// Customer password
    /// </summary>
    public CustomerPassword Password { get; }
}