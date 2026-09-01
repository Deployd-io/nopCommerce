using Nop.Web.Areas.Admin.Models.Plugins.Marketplace;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Plugins;

/// <summary>
/// Represents a plugins configuration model
/// </summary>
public partial record PluginsConfigurationModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public PluginsConfigurationModel()
    {
		_logger.LogDebug(">>> Entering PluginsConfigurationModel()");
        PluginsLocal = new PluginSearchModel();
		_logger.LogDebug("PluginsConfigurationModel(): PluginsLocal → {PluginsLocal}", PluginsLocal);
        AllPluginsAndThemes = new OfficialFeedPluginSearchModel();
    }

    #endregion

    #region Properties

    public PluginSearchModel PluginsLocal { get; set; }

    public OfficialFeedPluginSearchModel AllPluginsAndThemes { get; set; }

    #endregion
}