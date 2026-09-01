using Microsoft.Extensions.Logging;
namespace Nop.Services.Plugins;

/// <summary>
/// Base plugin
/// </summary>
public abstract partial class BasePlugin : IPlugin
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public virtual string GetConfigurationPageUrl()
    {
		_logger.LogDebug(">>> Entering GetConfigurationPageUrl()");
		_logger.LogDebug("<<< Exiting GetConfigurationPageUrl()");
        return null;
    }

    /// <summary>
    /// Gets or sets the plugin descriptor
    /// </summary>
    public virtual PluginDescriptor PluginDescriptor { get; set; }

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task InstallAsync()
    {
		_logger.LogDebug(">>> Entering InstallAsync()");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Install sample data of plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task InstallSampleDataAsync()
    {
		_logger.LogDebug(">>> Entering InstallSampleDataAsync()");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task UninstallAsync()
    {
		_logger.LogDebug(">>> Entering UninstallAsync()");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Update plugin
    /// </summary>
    /// <param name="currentVersion">Current version of plugin</param>
    /// <param name="targetVersion">New version of plugin</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task UpdateAsync(string currentVersion, string targetVersion)
    {
		_logger.LogDebug(">>> Entering UpdateAsync(currentVersion={CurrentVersion},targetVersion={TargetVersion})", currentVersion, targetVersion);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Prepare plugin to the uninstallation
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task PreparePluginToUninstallAsync()
    {
		_logger.LogDebug(">>> Entering PreparePluginToUninstallAsync()");
        //any can put any custom validation logic here
        //throw an exception if this plugin cannot be uninstalled
        //for example, requires some other certain plugins to be uninstalled first
		_logger.LogDebug("<<< Exiting PreparePluginToUninstallAsync()");
        return Task.CompletedTask;
    }
}