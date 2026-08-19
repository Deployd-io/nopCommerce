using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Menu;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents an event that occurs after admin menu created
/// </summary>
public partial class AdminMenuCreatedEvent : BaseMenuItemCreatedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="adminMenu">Admin menu</param>
    /// <param name="rootMenuItem">Root menu item</param>
    public AdminMenuCreatedEvent(IAdminMenu adminMenu, AdminMenuItem rootMenuItem) : base(adminMenu)
    {
		_logger.LogDebug(">>> Entering AdminMenuCreatedEvent(adminMenu={AdminMenu},rootMenuItem={RootMenuItem})", adminMenu, rootMenuItem);
        RootMenuItem = rootMenuItem;
		_logger.LogDebug("AdminMenuCreatedEvent(adminMenu={AdminMenu},rootMenuItem={RootMenuItem}): RootMenuItem → {RootMenuItem1}", adminMenu, rootMenuItem, RootMenuItem);
		_logger.LogDebug("<<< Exiting AdminMenuCreatedEvent(adminMenu={AdminMenu},rootMenuItem={RootMenuItem})", adminMenu, rootMenuItem);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a root menu item
    /// </summary>
    public AdminMenuItem RootMenuItem { get; protected set; }

    #endregion
}