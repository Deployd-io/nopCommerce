namespace Nop.Services.Plugins;

/// <summary>
/// Represents the plugin updated event
/// </summary>
public partial class PluginUpdatedEvent
{
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="plugin">Updated plugin</param>
    public PluginUpdatedEvent(PluginDescriptor plugin)
    {
		logger.LogDebug(">>> Entering PluginUpdatedEvent(plugin={Plugin})", plugin);
        Plugin = plugin;
		logger.LogDebug("<<< Exiting PluginUpdatedEvent(plugin={Plugin})", plugin);
		logger.LogDebug("PluginUpdatedEvent(plugin={Plugin}): Plugin → {Plugin1}", plugin, Plugin);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Updated plugin
    /// </summary>
    public PluginDescriptor Plugin { get; }

    #endregion
}