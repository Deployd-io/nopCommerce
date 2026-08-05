using Microsoft.Extensions.Logging;
﻿namespace Nop.Core.Domain.Security;

/// <summary>
/// Security setting changed event
/// </summary>
public partial class SecuritySettingsChangedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Initialize a new instance of the SecuritySettingsChangedEvent
    /// </summary>
    /// <param name="SecuritySettings">Security settings</param>
    /// <param name="oldEncryptionPrivateKey">Previous encryption key value</param>
    public SecuritySettingsChangedEvent(SecuritySettings securitySettings, string oldEncryptionPrivateKey)
    {
		_logger.LogDebug(">>> Entering SecuritySettingsChangedEvent(securitySettings={SecuritySettings},oldEncryptionPrivateKey={OldEncryptionPrivateKey})", securitySettings, oldEncryptionPrivateKey);
        SecuritySettings = securitySettings;
        OldEncryptionPrivateKey = oldEncryptionPrivateKey;
		_logger.LogDebug("<<< Exiting SecuritySettingsChangedEvent(securitySettings={SecuritySettings},oldEncryptionPrivateKey={OldEncryptionPrivateKey})", securitySettings, oldEncryptionPrivateKey);
		_logger.LogDebug("SecuritySettingsChangedEvent(securitySettings={SecuritySettings},oldEncryptionPrivateKey={OldEncryptionPrivateKey}): OldEncryptionPrivateKey → {OldEncryptionPrivateKey1}", securitySettings, oldEncryptionPrivateKey, OldEncryptionPrivateKey);
		_logger.LogDebug("SecuritySettingsChangedEvent(securitySettings={SecuritySettings},oldEncryptionPrivateKey={OldEncryptionPrivateKey}): SecuritySettings → {SecuritySettings1}", securitySettings, oldEncryptionPrivateKey, SecuritySettings);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Security settings
    /// </summary>
    public SecuritySettings SecuritySettings { get; set; }

    /// <summary>
    /// Previous encryption key value
    /// </summary>
    public string OldEncryptionPrivateKey { get; set; }

    #endregion
}