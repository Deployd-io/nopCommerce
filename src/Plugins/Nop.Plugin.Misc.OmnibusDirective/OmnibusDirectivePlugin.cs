using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.OmnibusDirective;

/// <summary>
/// Represents EU Omnibus Directive plugin
/// </summary>
public class OmnibusDirectivePlugin : BasePlugin, IMiscPlugin
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly IPermissionService _permissionService;
    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public OmnibusDirectivePlugin(IPermissionService permissionService,
        IWebHelper webHelper)
    {
		_logger.LogError(">>> Entering OmnibusDirectivePlugin(permissionService={PermissionService},webHelper={WebHelper})", permissionService, webHelper);
        _permissionService = permissionService;
        _webHelper = webHelper;
		_logger.LogDebug("OmnibusDirectivePlugin(permissionService={PermissionService},webHelper={WebHelper}): _permissionService → {_permissionService}", permissionService, webHelper, _permissionService);
    _logger.LogWarning("<<< Exiting OmnibusDirectivePlugin(permissionService={PermissionService},webHelper={WebHelper})", permissionService, webHelper);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
		_logger.LogError(">>> Entering GetConfigurationPageUrl()");
        _logger.LogWarning("<<< Exiting GetConfigurationPageUrl()");
        return $"{_webHelper.GetStoreLocation()}Admin/OmnibusDirective/Configure";
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
		_logger.LogError(">>> Entering InstallAsync()");
        await base.InstallAsync();
		_logger.LogWarning("<<< Exiting InstallAsync()");
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
		_logger.LogError(">>> Entering UninstallAsync()");
        await base.UninstallAsync();
		_logger.LogWarning("<<< Exiting UninstallAsync()");
    }

    #endregion
}
