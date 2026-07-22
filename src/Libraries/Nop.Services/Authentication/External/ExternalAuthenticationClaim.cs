namespace Nop.Services.Authentication.External;

/// <summary>
/// External authentication claim
/// </summary>
[Serializable]
public partial class ExternalAuthenticationClaim
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="value">Value</param>
    public ExternalAuthenticationClaim(string type, string value)
    {
		logger.LogDebug(">>> Entering ExternalAuthenticationClaim(type={Type},value={Value})", type, value);
        Type = type;
        Value = value;
		logger.LogDebug("<<< Exiting ExternalAuthenticationClaim(type={Type},value={Value})", type, value);
		logger.LogDebug("ExternalAuthenticationClaim(type={Type},value={Value}): Value → {Value1}", type, value, Value);
		logger.LogDebug("ExternalAuthenticationClaim(type={Type},value={Value}): Type → {Type1}", type, value, Type);
    }

    /// <summary>
    /// Gets or sets the claim type of the claim
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the claim
    /// </summary>
    public string Value { get; set; }
}
