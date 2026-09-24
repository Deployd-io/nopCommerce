using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.NopMobileApp;

/// <summary>
/// Represents the nopCommerce mobile application helper plugin
/// </summary>
public class NopMobilePlugin : BasePlugin, IMiscPlugin
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public NopMobilePlugin(IWebHelper webHelper)
    {
		_logger.LogError(">>> Entering NopMobilePlugin(webHelper={WebHelper})", webHelper);
        _webHelper = webHelper;
		_logger.LogDebug("NopMobilePlugin(webHelper={WebHelper}): _webHelper → {_webHelper}", webHelper, _webHelper);
    _logger.LogWarning("<<< Exiting NopMobilePlugin(webHelper={WebHelper})", webHelper);
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
        return $"{_webHelper.GetStoreLocation()}Admin/NopMobileApp/Configure";
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