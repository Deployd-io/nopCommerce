using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Gdpr;

/// <summary>
/// Customer permanently deleted (GDPR)
/// </summary>
public partial class CustomerPermanentlyDeleted
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="email">Email</param>
    public CustomerPermanentlyDeleted(int customerId, string email)
    {
		_logger.LogDebug(">>> Entering CustomerPermanentlyDeleted(customerId={CustomerId},email={Email})", customerId, email);
        CustomerId = customerId;
        Email = email;
		_logger.LogDebug("<<< Exiting CustomerPermanentlyDeleted(customerId={CustomerId},email={Email})", customerId, email);
		_logger.LogDebug("CustomerPermanentlyDeleted(customerId={CustomerId},email={Email}): CustomerId → {CustomerId1}", customerId, email, CustomerId);
		_logger.LogDebug("CustomerPermanentlyDeleted(customerId={CustomerId},email={Email}): Email → {Email1}", customerId, email, Email);
    }

    /// <summary>
    /// Customer identifier
    /// </summary>
    public int CustomerId { get; }

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; }
}