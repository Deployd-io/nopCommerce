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
		_logger.LogDebug(">>> Entering OmnibusDirectivePlugin(permissionService={PermissionService},webHelper={WebHelper})", permissionService, webHelper);
        _permissionService = permissionService;
		_logger.LogDebug("OmnibusDirectivePlugin(permissionService={PermissionService},webHelper={WebHelper}): _permissionService → {_permissionService}", permissionService, webHelper, _permissionService);
        _webHelper = webHelper;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
		_logger.LogDebug(">>> Entering GetConfigurationPageUrl()");
        return $"{_webHelper.GetStoreLocation()}Admin/OmnibusDirective/Configure";
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
		_logger.LogDebug(">>> Entering InstallAsync()");
        await base.InstallAsync();
		_logger.LogDebug("<<< Exiting InstallAsync()");
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
		_logger.LogDebug(">>> Entering UninstallAsync()");
        await base.UninstallAsync();
		_logger.LogDebug("<<< Exiting UninstallAsync()");
    }

    #endregion
}
