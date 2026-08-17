using Microsoft.Extensions.Logging;
﻿namespace Nop.Services.Messages;

/// <summary>
/// Represents token
/// </summary>
public sealed partial class Token
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    public Token(string key, object value) : this(key, value, false)
    {
	_logger.LogDebug(">>> Entering Token(key={Key},value={Value})", key, value);
	_logger.LogDebug(">>> Entering Token(key={Key},value={Value},neverHtmlEncoded={NeverHtmlEncoded})", key, value, neverHtmlEncoded);
	_logger.LogDebug("<<< Exiting Token(key={Key},value={Value},neverHtmlEncoded={NeverHtmlEncoded})", key, value, neverHtmlEncoded);
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
		_logger.LogWarning("Token(key={Key},value={Value},neverHtmlEncoded={NeverHtmlEncoded}): Key → {Key1}", key, value, neverHtmlEncoded, Key);
		_logger.LogDebug("Token(key={Key},value={Value},neverHtmlEncoded={NeverHtmlEncoded}): Value → {Value1}", key, value, neverHtmlEncoded, Value);
        NeverHtmlEncoded = neverHtmlEncoded;
		_logger.LogDebug("<<< Exiting Token(key={Key},value={Value},neverHtmlEncoded={NeverHtmlEncoded})", key, value, neverHtmlEncoded);
		_logger.LogDebug("Token(key={Key},value={Value},neverHtmlEncoded={NeverHtmlEncoded}): NeverHtmlEncoded → {NeverHtmlEncoded1}", key, value, neverHtmlEncoded, NeverHtmlEncoded);
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
		_logger.LogDebug(">>> Entering ToString()");
		_logger.LogDebug("<<< Exiting ToString()");
        return $"{Key}: {Value}";
    }

    #endregion
}