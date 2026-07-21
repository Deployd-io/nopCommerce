using Nop.Web.Framework.Menu;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents an event that occurs after admin menu created
/// </summary>
public partial class AdminMenuCreatedEvent : BaseMenuItemCreatedEvent
{
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="adminMenu">Admin menu</param>
    /// <param name="rootMenuItem">Root menu item</param>
    public AdminMenuCreatedEvent(IAdminMenu adminMenu, AdminMenuItem rootMenuItem) : base(adminMenu)
    {
		logger.LogDebug(">>> Entering AdminMenuCreatedEvent(adminMenu={AdminMenu},rootMenuItem={RootMenuItem})", adminMenu, rootMenuItem);
        RootMenuItem = rootMenuItem;
		logger.LogDebug("<<< Exiting AdminMenuCreatedEvent(adminMenu={AdminMenu},rootMenuItem={RootMenuItem})", adminMenu, rootMenuItem);
		logger.LogDebug("AdminMenuCreatedEvent(adminMenu={AdminMenu},rootMenuItem={RootMenuItem}): RootMenuItem → {RootMenuItem1}", adminMenu, rootMenuItem, RootMenuItem);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a root menu item
    /// </summary>
    public AdminMenuItem RootMenuItem { get; protected set; }

    #endregion
}
