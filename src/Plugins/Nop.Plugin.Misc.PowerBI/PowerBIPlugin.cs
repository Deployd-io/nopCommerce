using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.PowerBI;

/// <summary>
/// Represents the Power BI helper plugin
/// </summary>
public class PowerBIPlugin : BasePlugin, IMiscPlugin
{
    #region Fields

    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public PowerBIPlugin(IWebHelper webHelper)
    {
		logger.LogDebug(">>> Entering PowerBIPlugin(webHelper={WebHelper})", webHelper);
        _webHelper = webHelper;
		logger.LogDebug("<<< Exiting PowerBIPlugin(webHelper={WebHelper})", webHelper);
		logger.LogDebug("PowerBIPlugin(webHelper={WebHelper}): _webHelper → {_webHelper}", webHelper, _webHelper);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
		logger.LogDebug(">>> Entering GetConfigurationPageUrl()");
		logger.LogDebug("<<< Exiting GetConfigurationPageUrl()");
        return $"{_webHelper.GetStoreLocation()}Admin/PowerBI/Configure";
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
		logger.LogDebug(">>> Entering InstallAsync()");
        await base.InstallAsync();
		logger.LogDebug("<<< Exiting InstallAsync()");
    }
    
    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
		logger.LogDebug(">>> Entering UninstallAsync()");
        await base.UninstallAsync();
		logger.LogDebug("<<< Exiting UninstallAsync()");
    }

    #endregion
}
