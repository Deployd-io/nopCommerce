using Microsoft.Extensions.Logging;
namespace Nop.Services.Customers;

/// <summary>
/// Customer multi-factor authentication info
/// </summary>
public partial class CustomerMultiFactorAuthenticationInfo
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CustomerMultiFactorAuthenticationInfo()
    {
		_logger.LogDebug(">>> Entering CustomerMultiFactorAuthenticationInfo()");
        CustomValues = new Dictionary<string, object>();
		_logger.LogDebug("CustomerMultiFactorAuthenticationInfo(): CustomValues → {CustomValues}", CustomValues);
    }
    public string UserName { get; set; }

    public bool RememberMe { get; set; }

    public string ReturnUrl { get; set; }

    /// <summary>
    /// You can store any custom value in this property
    /// </summary>
    public Dictionary<string, object> CustomValues { get; set; }
}