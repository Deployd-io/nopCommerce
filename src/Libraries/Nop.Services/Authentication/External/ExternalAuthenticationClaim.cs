using Microsoft.Extensions.Logging;
namespace Nop.Services.Authentication.External;

/// <summary>
/// External authentication claim
/// </summary>
[Serializable]
public partial class ExternalAuthenticationClaim
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="value">Value</param>
    public ExternalAuthenticationClaim(string type, string value)
    {
		_logger.LogDebug(">>> Entering ExternalAuthenticationClaim(type={Type},value={Value})", type, value);
        Type = type;
		_logger.LogDebug("ExternalAuthenticationClaim(type={Type},value={Value}): Type → {Type1}", type, value, Type);
        Value = value;
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