namespace Nop.Core.Domain.Gdpr;

/// <summary>
/// Customer permanently deleted (GDPR)
/// </summary>
public partial class CustomerPermanentlyDeleted
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="email">Email</param>
    public CustomerPermanentlyDeleted(int customerId, string email)
    {
		logger.LogDebug(">>> Entering CustomerPermanentlyDeleted(customerId={CustomerId},email={Email})", customerId, email);
        CustomerId = customerId;
        Email = email;
		logger.LogDebug("<<< Exiting CustomerPermanentlyDeleted(customerId={CustomerId},email={Email})", customerId, email);
		logger.LogDebug("CustomerPermanentlyDeleted(customerId={CustomerId},email={Email}): CustomerId → {CustomerId1}", customerId, email, CustomerId);
		logger.LogDebug("CustomerPermanentlyDeleted(customerId={CustomerId},email={Email}): Email → {Email1}", customerId, email, Email);
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
