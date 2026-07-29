namespace Nop.Core.Domain.Security;

/// <summary>
/// Security setting changed event
/// </summary>
public partial class SecuritySettingsChangedEvent
{
    #region Ctor

    /// <summary>
    /// Initialize a new instance of the SecuritySettingsChangedEvent
    /// </summary>
    /// <param name="SecuritySettings">Security settings</param>
    /// <param name="oldEncryptionPrivateKey">Previous encryption key value</param>
    public SecuritySettingsChangedEvent(SecuritySettings securitySettings, string oldEncryptionPrivateKey)
    {
		logger.LogDebug(">>> Entering SecuritySettingsChangedEvent(securitySettings={SecuritySettings},oldEncryptionPrivateKey={OldEncryptionPrivateKey})", securitySettings, oldEncryptionPrivateKey);
        SecuritySettings = securitySettings;
        OldEncryptionPrivateKey = oldEncryptionPrivateKey;
		logger.LogDebug("<<< Exiting SecuritySettingsChangedEvent(securitySettings={SecuritySettings},oldEncryptionPrivateKey={OldEncryptionPrivateKey})", securitySettings, oldEncryptionPrivateKey);
		logger.LogDebug("SecuritySettingsChangedEvent(securitySettings={SecuritySettings},oldEncryptionPrivateKey={OldEncryptionPrivateKey}): OldEncryptionPrivateKey → {OldEncryptionPrivateKey1}", securitySettings, oldEncryptionPrivateKey, OldEncryptionPrivateKey);
		logger.LogDebug("SecuritySettingsChangedEvent(securitySettings={SecuritySettings},oldEncryptionPrivateKey={OldEncryptionPrivateKey}): SecuritySettings → {SecuritySettings1}", securitySettings, oldEncryptionPrivateKey, SecuritySettings);
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