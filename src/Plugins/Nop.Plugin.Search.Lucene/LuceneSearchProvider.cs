using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Search.Lucene;

/// <summary>
/// Represents Lucene search provider helper plugin
/// </summary>
public class LuceneSearchProvider : BasePlugin, IMiscPlugin
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public LuceneSearchProvider(IWebHelper webHelper)
    {
		_logger.LogDebug(">>> Entering LuceneSearchProvider(webHelper={WebHelper})", webHelper);
        _webHelper = webHelper;
		_logger.LogDebug("<<< Exiting LuceneSearchProvider(webHelper={WebHelper})", webHelper);
		_logger.LogDebug("LuceneSearchProvider(webHelper={WebHelper}): _webHelper → {_webHelper}", webHelper, _webHelper);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
		_logger.LogDebug(">>> Entering GetConfigurationPageUrl()");
		_logger.LogDebug("<<< Exiting GetConfigurationPageUrl()");
        return $"{_webHelper.GetStoreLocation()}Admin/Lucene/Configure";
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
