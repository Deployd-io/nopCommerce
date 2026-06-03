namespace Nop.Web.Models.Menus;

/// <summary>
/// Represents an event that occurs when a menu models is created
/// </summary>
public class MenuModelsCreatedEvent
{
    #region Ctor

    public MenuModelsCreatedEvent(IList<MenuModel> menus)
    {
        Menus = menus;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets list of menu models
    /// </summary>
    public IList<MenuModel> Menus { get; }

    #endregion
}
