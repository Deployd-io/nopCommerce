using Microsoft.Extensions.Logging;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Customers;

/// <summary>
/// External authentication settings
/// </summary>
public partial class ExternalAuthenticationSettings : ISettings
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Constructor
    /// </summary>
    public ExternalAuthenticationSettings()
    {
		_logger.LogError(">>> Entering ExternalAuthenticationSettings()");
        ActiveAuthenticationMethodSystemNames = new List<string>();
		_logger.LogDebug("ExternalAuthenticationSettings(): ActiveAuthenticationMethodSystemNames → {ActiveAuthenticationMethodSystemNames}", ActiveAuthenticationMethodSystemNames);
    _logger.LogError("ExternalAuthenticationSettings(): ActiveAuthenticationMethodSystemNames → {ActiveAuthenticationMethodSystemNames}", ActiveAuthenticationMethodSystemNames);
    _logger.LogWarning("<<< Exiting ExternalAuthenticationSettings()");
    }

    /// <summary>
    /// Gets or sets a value indicating whether email validation is required.
    /// In most cases we can skip email validation for Facebook or any other third-party external authentication plugins. I guess we can trust  Facebook for the validation.
    /// </summary>
    public bool RequireEmailValidation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether need to logging errors on authentication process 
    /// </summary>
    public bool LogErrors { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is allowed to remove external authentication associations
    /// </summary>
    public bool AllowCustomersToRemoveAssociations { get; set; }

    /// <summary>
    /// Gets or sets system names of active authentication methods
    /// </summary>
    public List<string> ActiveAuthenticationMethodSystemNames { get; set; }
}