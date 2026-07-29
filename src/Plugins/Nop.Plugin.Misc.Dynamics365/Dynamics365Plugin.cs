using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.Dynamics365;

/// <summary>
/// Represents the Dynamics 365 helper plugin
/// </summary>
public class Dynamics365Plugin : BasePlugin, IMiscPlugin
{
    #region Fields

    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public Dynamics365Plugin(IWebHelper webHelper)
    {
		logger.LogDebug(">>> Entering Dynamics365Plugin(webHelper={WebHelper})", webHelper);
        _webHelper = webHelper;
		logger.LogDebug("<<< Exiting Dynamics365Plugin(webHelper={WebHelper})", webHelper);
		logger.LogDebug("Dynamics365Plugin(webHelper={WebHelper}): _webHelper → {_webHelper}", webHelper, _webHelper);
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
        return $"{_webHelper.GetStoreLocation()}Admin/Dynamics365/Configure";
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
