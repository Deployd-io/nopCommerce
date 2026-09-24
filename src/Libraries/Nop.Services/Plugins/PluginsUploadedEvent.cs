using Microsoft.Extensions.Logging;
namespace Nop.Services.Plugins;

/// <summary>
/// Plugins uploaded event
/// </summary>
public partial class PluginsUploadedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="uploadedPlugins">Uploaded plugins</param>
    public PluginsUploadedEvent(IList<PluginDescriptor> uploadedPlugins)
    {
		_logger.LogError(">>> Entering PluginsUploadedEvent(uploadedPlugins={UploadedPlugins})", uploadedPlugins);
        UploadedPlugins = uploadedPlugins;
		_logger.LogDebug("PluginsUploadedEvent(uploadedPlugins={UploadedPlugins}): UploadedPlugins → {UploadedPlugins1}", uploadedPlugins, UploadedPlugins);
    _logger.LogWarning("<<< Exiting PluginsUploadedEvent(uploadedPlugins={UploadedPlugins})", uploadedPlugins);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Uploaded plugins
    /// </summary>
    public IList<PluginDescriptor> UploadedPlugins { get; }

    #endregion
}