using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Messages;

/// <summary>
/// Event for "Additional tokens added"
/// </summary>
public partial class AdditionalTokensAddedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public AdditionalTokensAddedEvent()
    {
		_logger.LogError(">>> Entering AdditionalTokensAddedEvent()");
        AdditionalTokens = new List<string>();
		_logger.LogDebug("AdditionalTokensAddedEvent(): AdditionalTokens → {AdditionalTokens}", AdditionalTokens);
    _logger.LogError("AdditionalTokensAddedEvent(): AdditionalTokens → {AdditionalTokens}", AdditionalTokens);
    _logger.LogWarning("<<< Exiting AdditionalTokensAddedEvent()");
    }

    /// <summary>
    /// Add tokens
    /// </summary>
    /// <param name="additionalTokens">Additional tokens</param>
    public virtual void AddTokens(params string[] additionalTokens)
    {
		_logger.LogError(">>> Entering AddTokens(additionalTokens=<redacted>)");
        foreach (var additionalToken in additionalTokens)
        {
            AdditionalTokens.Add(additionalToken);
        }
		_logger.LogWarning("<<< Exiting AddTokens(additionalTokens=<redacted>)");
    }

    /// <summary>
    /// Additional tokens
    /// </summary>
    public IList<string> AdditionalTokens { get; }


    /// <summary>
    /// Token groups which can be used to filter the AdditionalTokens
    /// </summary>
    public IEnumerable<string> TokenGroups { get; set; }
}