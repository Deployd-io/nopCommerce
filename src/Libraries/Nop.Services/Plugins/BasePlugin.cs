namespace Nop.Services.Plugins;

/// <summary>
/// Base plugin
/// </summary>
public abstract partial class BasePlugin : IPlugin
{
    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public virtual string GetConfigurationPageUrl()
    {
		logger.LogDebug(">>> Entering GetConfigurationPageUrl()");
		logger.LogDebug("<<< Exiting GetConfigurationPageUrl()");
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
		logger.LogDebug(">>> Entering InstallAsync()");
		logger.LogDebug("<<< Exiting InstallAsync()");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Install sample data of plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task InstallSampleDataAsync()
    {
		logger.LogDebug(">>> Entering InstallSampleDataAsync()");
		logger.LogDebug("<<< Exiting InstallSampleDataAsync()");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task UninstallAsync()
    {
		logger.LogDebug(">>> Entering UninstallAsync()");
		logger.LogDebug("<<< Exiting UninstallAsync()");
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
		logger.LogDebug(">>> Entering UpdateAsync(currentVersion={CurrentVersion},targetVersion={TargetVersion})", currentVersion, targetVersion);
		logger.LogDebug("<<< Exiting UpdateAsync(currentVersion={CurrentVersion},targetVersion={TargetVersion})", currentVersion, targetVersion);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Prepare plugin to the uninstallation
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task PreparePluginToUninstallAsync()
    {
		logger.LogDebug(">>> Entering PreparePluginToUninstallAsync()");
        //any can put any custom validation logic here
        //throw an exception if this plugin cannot be uninstalled
        //for example, requires some other certain plugins to be uninstalled first
		logger.LogDebug("<<< Exiting PreparePluginToUninstallAsync()");
        return Task.CompletedTask;
    }
}