namespace Nop.Services.Messages;

/// <summary>
/// Represents token
/// </summary>
public sealed partial class Token
{
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    public Token(string key, object value) : this(key, value, false)
    {
	logger.LogDebug(">>> Entering Token(key={Key},value={Value})", key, value);
	logger.LogDebug(">>> Entering Token(key={Key},value={Value},neverHtmlEncoded={NeverHtmlEncoded})", key, value, neverHtmlEncoded);
	logger.LogDebug("<<< Exiting Token(key={Key},value={Value},neverHtmlEncoded={NeverHtmlEncoded})", key, value, neverHtmlEncoded);
    }

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="neverHtmlEncoded">Indicates whether this token should not be HTML encoded</param>
    public Token(string key, object value, bool neverHtmlEncoded)
    {
        Key = key;
        Value = value;
		logger.LogWarning("Token(key={Key},value={Value},neverHtmlEncoded={NeverHtmlEncoded}): Key → {Key1}", key, value, neverHtmlEncoded, Key);
		logger.LogDebug("Token(key={Key},value={Value},neverHtmlEncoded={NeverHtmlEncoded}): Value → {Value1}", key, value, neverHtmlEncoded, Value);
        NeverHtmlEncoded = neverHtmlEncoded;
		logger.LogDebug("<<< Exiting Token(key={Key},value={Value},neverHtmlEncoded={NeverHtmlEncoded})", key, value, neverHtmlEncoded);
		logger.LogDebug("Token(key={Key},value={Value},neverHtmlEncoded={NeverHtmlEncoded}): NeverHtmlEncoded → {NeverHtmlEncoded1}", key, value, neverHtmlEncoded, NeverHtmlEncoded);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Token key
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Token value
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Indicates whether this token should not be HTML encoded
    /// </summary>
    public bool NeverHtmlEncoded { get; }

    #endregion

    #region Methods

    /// <summary>
    /// The string representation of the value of this token
    /// </summary>
    /// <returns>String value</returns>
    public override string ToString()
    {
		logger.LogDebug(">>> Entering ToString()");
		logger.LogDebug("<<< Exiting ToString()");
        return $"{Key}: {Value}";
    }

    #endregion
}