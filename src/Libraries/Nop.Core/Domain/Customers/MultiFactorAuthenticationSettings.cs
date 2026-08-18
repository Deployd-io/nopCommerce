using Microsoft.Extensions.Logging;
﻿using Nop.Core.Configuration;

namespace Nop.Core.Domain.Customers;

/// <summary>
/// Multi-factor authentication settings
/// </summary>
public partial class MultiFactorAuthenticationSettings : ISettings
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public MultiFactorAuthenticationSettings()
    {
		_logger.LogDebug(">>> Entering MultiFactorAuthenticationSettings()");
        ActiveAuthenticationMethodSystemNames = new List<string>();
		_logger.LogDebug("MultiFactorAuthenticationSettings(): ActiveAuthenticationMethodSystemNames → {ActiveAuthenticationMethodSystemNames}", ActiveAuthenticationMethodSystemNames);
		_logger.LogDebug("<<< Exiting MultiFactorAuthenticationSettings()");
    }

    #endregion

    /// <summary>
    /// Gets or sets system names of active multi-factor authentication methods
    /// </summary>
    public List<string> ActiveAuthenticationMethodSystemNames { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to force multi-factor authentication
    /// </summary>
    public bool ForceMultifactorAuthentication { get; set; }
}