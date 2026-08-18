using Nop.Services.Plugins;
using Nop.Web.Framework.Events;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.NopMobileApp.Services;

/// <summary>
/// Represents plugin event consumer
/// </summary>
public class EventConsumer : BaseAdminMenuCreatedEventConsumer
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public EventConsumer(IPluginManager<IPlugin> pluginManager) :
		_logger.LogDebug(">>> Entering EventConsumer(pluginManager={PluginManager})", pluginManager);
		_logger.LogDebug("<<< Exiting EventConsumer(pluginManager={PluginManager})", pluginManager);
        base(pluginManager)
    {
    }

    /// <summary>
    /// Gets the plugin system name
    /// </summary>
    protected override string PluginSystemName => NopMobileAppDefaults.SystemName;

    /// <summary>
    /// The system name of the menu item before with need to insert the current one
    /// </summary>
    protected override string BeforeMenuSystemName => "Local plugins";
}
