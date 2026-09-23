using Microsoft.Extensions.Logging;
namespace Nop.Services.Plugins;

/// <summary>
/// Represents the plugin updated event
/// </summary>
public partial class PluginUpdatedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="plugin">Updated plugin</param>
    public PluginUpdatedEvent(PluginDescriptor plugin)
    {
		_logger.LogError(">>> Entering PluginUpdatedEvent(plugin={Plugin})", plugin);
        Plugin = plugin;
		_logger.LogDebug("PluginUpdatedEvent(plugin={Plugin}): Plugin → {Plugin1}", plugin, Plugin);
    _logger.LogWarning("<<< Exiting PluginUpdatedEvent(plugin={Plugin})", plugin);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Updated plugin
    /// </summary>
    public PluginDescriptor Plugin { get; }

    #endregion
}