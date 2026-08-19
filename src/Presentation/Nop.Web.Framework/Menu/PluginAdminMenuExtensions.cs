using Microsoft.Extensions.Logging;
﻿using Nop.Services.Plugins;

namespace Nop.Web.Framework.Menu;

public static partial class PluginAdminMenuExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public static AdminMenuItem GetAdminMenuItem(this IPlugin plugin, string iconClass = "far fa-dot-circle")
    {
		_logger.LogDebug(">>> Entering GetAdminMenuItem(plugin={Plugin},iconClass={IconClass})", plugin, iconClass);
        var description = plugin?.PluginDescriptor;

        if (description == null)
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
