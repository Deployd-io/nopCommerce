using Nop.Web.Framework.Menu;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents the base event that occurs after admin menu item create
/// </summary>
public abstract partial class BaseMenuItemCreatedEvent
{
    private readonly IAdminMenu _adminMenu;

    protected BaseMenuItemCreatedEvent(IAdminMenu adminMenu)
    {
		logger.LogDebug(">>> Entering BaseMenuItemCreatedEvent(adminMenu={AdminMenu})", adminMenu);
        _adminMenu = adminMenu;
		logger.LogDebug("<<< Exiting BaseMenuItemCreatedEvent(adminMenu={AdminMenu})", adminMenu);
		logger.LogDebug("BaseMenuItemCreatedEvent(adminMenu={AdminMenu}): _adminMenu → {_adminMenu}", adminMenu, _adminMenu);
    }

    /// <summary>
    /// Generates an admin menu item URL 
    /// </summary>
    /// <param name="controllerName">The name of the controller</param>
    /// <param name="actionName">The name of the action method</param>
    public virtual string GetMenuItemUrl(string controllerName, string actionName)
    {
		logger.LogDebug(">>> Entering GetMenuItemUrl(controllerName={ControllerName},actionName={ActionName})", controllerName, actionName);
		logger.LogDebug("<<< Exiting GetMenuItemUrl(controllerName={ControllerName},actionName={ActionName})", controllerName, actionName);
        return _adminMenu.GetMenuItemUrl(controllerName, actionName);
    }
}
