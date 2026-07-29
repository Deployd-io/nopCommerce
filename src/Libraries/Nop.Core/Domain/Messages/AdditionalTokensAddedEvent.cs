namespace Nop.Core.Domain.Messages;

/// <summary>
/// Event for "Additional tokens added"
/// </summary>
public partial class AdditionalTokensAddedEvent
{
    public AdditionalTokensAddedEvent()
    {
		logger.LogDebug(">>> Entering AdditionalTokensAddedEvent()");
        AdditionalTokens = new List<string>();
		logger.LogDebug("<<< Exiting AdditionalTokensAddedEvent()");
		logger.LogDebug("AdditionalTokensAddedEvent(): AdditionalTokens → {AdditionalTokens}", AdditionalTokens);
    }

    /// <summary>
    /// Add tokens
    /// </summary>
    /// <param name="additionalTokens">Additional tokens</param>
    public virtual void AddTokens(params string[] additionalTokens)
    {
		logger.LogDebug(">>> Entering AddTokens(additionalTokens={AdditionalTokens})", additionalTokens);
        foreach (var additionalToken in additionalTokens)
        {
            AdditionalTokens.Add(additionalToken);
        }
		logger.LogDebug("<<< Exiting AddTokens(additionalTokens={AdditionalTokens})", additionalTokens);
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