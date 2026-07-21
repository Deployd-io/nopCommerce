using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Plugins;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.OmnibusDirective;

/// <summary>
/// Represents EU Omnibus Directive plugin
/// </summary>
public class OmnibusDirectivePlugin : BasePlugin, IMiscPlugin
{
    #region Fields

    private readonly IPermissionService _permissionService;
    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public OmnibusDirectivePlugin(IPermissionService permissionService,
        IWebHelper webHelper)
    {
        _permissionService = permissionService;
        _webHelper = webHelper;
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
        return $"{_webHelper.GetStoreLocation()}Admin/OmnibusDirective/Configure";
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
