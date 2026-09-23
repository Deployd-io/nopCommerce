using Microsoft.Extensions.Logging;
namespace Nop.Core.Infrastructure;

/// <summary>
/// Provides access to all "singletons" stored by <see cref="Singleton{T}"/>.
/// </summary>
public partial class BaseSingleton
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    static BaseSingleton()
    {
		_logger.LogError(">>> Entering BaseSingleton()");
        AllSingletons = new Dictionary<Type, object>();
		_logger.LogDebug("BaseSingleton(): AllSingletons → {AllSingletons}", AllSingletons);
    _logger.LogError("BaseSingleton(): AllSingletons → {AllSingletons}", AllSingletons);
    _logger.LogWarning("<<< Exiting BaseSingleton()");
    }

    /// <summary>
    /// Dictionary of type to singleton instances.
    /// </summary>
    public static IDictionary<Type, object> AllSingletons { get; }
}