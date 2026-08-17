using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Menu;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents an event that occurs after third party plugins menu item created
/// </summary>
public partial class ThirdPartyPluginsMenuItemCreatedEvent : BaseMenuItemCreatedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="adminMenu">Admin menu</param>
    /// <param name="rootMenuItem">Third party plugins menu item</param>
    public ThirdPartyPluginsMenuItemCreatedEvent(IAdminMenu adminMenu, AdminMenuItem rootMenuItem) : base(adminMenu)
    {
		_logger.LogDebug(">>> Entering ThirdPartyPluginsMenuItemCreatedEvent(adminMenu={AdminMenu},rootMenuItem={RootMenuItem})", adminMenu, rootMenuItem);
        MenuItem = rootMenuItem;
		_logger.LogDebug("<<< Exiting ThirdPartyPluginsMenuItemCreatedEvent(adminMenu={AdminMenu},rootMenuItem={RootMenuItem})", adminMenu, rootMenuItem);
		_logger.LogDebug("ThirdPartyPluginsMenuItemCreatedEvent(adminMenu={AdminMenu},rootMenuItem={RootMenuItem}): MenuItem → {MenuItem}", adminMenu, rootMenuItem, MenuItem);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a third party plugins menu item
    /// </summary>
    public AdminMenuItem MenuItem { get; protected set; }

    #endregion
}