using Nop.Services.Plugins;

namespace Nop.Web.Framework.Menu;

public static partial class PluginAdminMenuExtensions
{
    public static AdminMenuItem GetAdminMenuItem(this IPlugin plugin, string iconClass = "far fa-dot-circle")
    {
		logger.LogDebug(">>> Entering GetAdminMenuItem(plugin={Plugin},iconClass={IconClass})", plugin, iconClass);
        var description = plugin?.PluginDescriptor;

        if (description == null)
		logger.LogDebug("<<< Exiting GetAdminMenuItem(plugin={Plugin},iconClass={IconClass})", plugin, iconClass);
            return new AdminMenuItem { Visible = false };
        
        return new AdminMenuItem
        {
            SystemName = description.SystemName,
            Title = description.FriendlyName,
            IconClass = iconClass,
            Url = plugin.GetConfigurationPageUrl()
        };
    }
}
