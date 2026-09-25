using Microsoft.Extensions.Logging;
namespace Nop.Services;

/// <summary>
/// Represents a base class for results and response
/// </summary>
public abstract partial class BaseNopResult
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Gets a value indicating whether request has been completed successfully
    /// </summary>
    public bool Success => !Errors.Any();

    /// <summary>
    /// Add error
    /// </summary>
    /// <param name="error">Error</param>
    public virtual void AddError(string error)
    {
		_logger.LogError(">>> Entering AddError(error={Error})", error);
        Errors.Add(error);
		_logger.LogWarning("<<< Exiting AddError(error={Error})", error);
    }

    /// <summary>
    /// Errors
    /// </summary>
    public IList<string> Errors { get; set; } = new List<string>();
}
