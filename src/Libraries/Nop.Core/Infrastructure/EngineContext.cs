using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Nop.Core.Infrastructure;

/// <summary>
/// Provides access to the singleton instance of the Nop engine.
/// </summary>
public partial class EngineContext
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Create a static instance of the Nop engine.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static IEngine Create()
    {
		_logger.LogError(">>> Entering Create()");
        //create NopEngine as engine
		_logger.LogWarning("<<< Exiting Create()");
        return Singleton<IEngine>.Instance ?? (Singleton<IEngine>.Instance = new NopEngine());
    }

    /// <summary>
    /// Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.
    /// </summary>
    /// <param name="engine">The engine to use.</param>
    /// <remarks>Only use this method if you know what you're doing.</remarks>
    public static void Replace(IEngine engine)
    {
		_logger.LogError(">>> Entering Replace(engine={Engine})", engine);
        Singleton<IEngine>.Instance = engine;
		_logger.LogWarning("<<< Exiting Replace(engine={Engine})", engine);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the singleton Nop engine used to access Nop services.
    /// </summary>
    public static IEngine Current
    {
        get
        {
            if (Singleton<IEngine>.Instance == null)
            {
                Create();
            }

            return Singleton<IEngine>.Instance;
        }
    }

    #endregion
}