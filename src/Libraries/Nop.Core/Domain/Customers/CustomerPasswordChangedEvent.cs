namespace Nop.Core.Domain.Customers;

/// <summary>
/// Customer password changed event
/// </summary>
public partial class CustomerPasswordChangedEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="password">Password</param>
    public CustomerPasswordChangedEvent(CustomerPassword password)
    {
		logger.LogDebug(">>> Entering CustomerPasswordChangedEvent(password={Password})", password);
        Password = password;
		logger.LogDebug("<<< Exiting CustomerPasswordChangedEvent(password={Password})", password);
		logger.LogWarning("CustomerPasswordChangedEvent(password={Password}): Password → {Password1}", password, Password);
    }

    /// <summary>
    /// Customer password
    /// </summary>
    public CustomerPassword Password { get; }
}
