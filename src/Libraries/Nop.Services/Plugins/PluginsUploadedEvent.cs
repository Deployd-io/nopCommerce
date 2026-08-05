namespace Nop.Services.Plugins;

/// <summary>
/// Plugins uploaded event
/// </summary>
public partial class PluginsUploadedEvent
{
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="uploadedPlugins">Uploaded plugins</param>
    public PluginsUploadedEvent(IList<PluginDescriptor> uploadedPlugins)
    {
		logger.LogDebug(">>> Entering PluginsUploadedEvent(uploadedPlugins={UploadedPlugins})", uploadedPlugins);
        UploadedPlugins = uploadedPlugins;
		logger.LogDebug("<<< Exiting PluginsUploadedEvent(uploadedPlugins={UploadedPlugins})", uploadedPlugins);
		logger.LogDebug("PluginsUploadedEvent(uploadedPlugins={UploadedPlugins}): UploadedPlugins → {UploadedPlugins1}", uploadedPlugins, UploadedPlugins);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Uploaded plugins
    /// </summary>
    public IList<PluginDescriptor> UploadedPlugins { get; }

    #endregion
}