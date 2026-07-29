using Nop.Web.Areas.Admin.Models.Plugins.Marketplace;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Plugins;

/// <summary>
/// Represents a plugins configuration model
/// </summary>
public partial record PluginsConfigurationModel : BaseNopModel
{
    #region Ctor

    public PluginsConfigurationModel()
    {
		logger.LogDebug(">>> Entering PluginsConfigurationModel()");
        PluginsLocal = new PluginSearchModel();
        AllPluginsAndThemes = new OfficialFeedPluginSearchModel();
		logger.LogDebug("<<< Exiting PluginsConfigurationModel()");
		logger.LogDebug("PluginsConfigurationModel(): PluginsLocal → {PluginsLocal}", PluginsLocal);
		logger.LogDebug("PluginsConfigurationModel(): AllPluginsAndThemes → {AllPluginsAndThemes}", AllPluginsAndThemes);
    }

    #endregion

    #region Properties

    public PluginSearchModel PluginsLocal { get; set; }

    public OfficialFeedPluginSearchModel AllPluginsAndThemes { get; set; }

    #endregion
}