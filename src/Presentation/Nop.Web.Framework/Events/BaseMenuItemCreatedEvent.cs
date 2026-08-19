using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Menu;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents the base event that occurs after admin menu item create
/// </summary>
public abstract partial class BaseMenuItemCreatedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private readonly IAdminMenu _adminMenu;

    protected BaseMenuItemCreatedEvent(IAdminMenu adminMenu)
    {
		_logger.LogDebug(">>> Entering BaseMenuItemCreatedEvent(adminMenu={AdminMenu})", adminMenu);
        _adminMenu = adminMenu;
		_logger.LogDebug("BaseMenuItemCreatedEvent(adminMenu={AdminMenu}): _adminMenu → {_adminMenu}", adminMenu, _adminMenu);
		_logger.LogDebug("<<< Exiting BaseMenuItemCreatedEvent(adminMenu={AdminMenu})", adminMenu);
    }

    /// <summary>
    /// Generates an admin menu item URL 
    /// </summary>
    /// <param name="controllerName">The name of the controller</param>
    /// <param name="actionName">The name of the action method</param>
    public virtual string GetMenuItemUrl(string controllerName, string actionName)
    {
		_logger.LogDebug(">>> Entering GetMenuItemUrl(controllerName={ControllerName},actionName={ActionName})", controllerName, actionName);
		_logger.LogDebug("<<< Exiting GetMenuItemUrl(controllerName={ControllerName},actionName={ActionName})", controllerName, actionName);
        return _adminMenu.GetMenuItemUrl(controllerName, actionName);
    }
}
