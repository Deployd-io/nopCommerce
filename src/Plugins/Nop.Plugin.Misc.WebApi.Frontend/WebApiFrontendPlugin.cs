using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.WebApi.Frontend;

/// <summary>
/// Represents the Web API frontend plugin
/// </summary>
public class WebApiFrontendPlugin : BasePlugin, IMiscPlugin
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public WebApiFrontendPlugin(IWebHelper webHelper)
    {
		_logger.LogError(">>> Entering WebApiFrontendPlugin(webHelper={WebHelper})", webHelper);
        _webHelper = webHelper;
		_logger.LogDebug("WebApiFrontendPlugin(webHelper={WebHelper}): _webHelper → {_webHelper}", webHelper, _webHelper);
    _logger.LogWarning("<<< Exiting WebApiFrontendPlugin(webHelper={WebHelper})", webHelper);
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
        return $"{_webHelper.GetStoreLocation()}Admin/WebApiFrontend/Configure";
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