using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Menus;

/// <summary>
/// Represents menu model
/// </summary>
public partial record MenuModel : BaseNopEntityModel, IAclSupportedModel, IStoreMappingSupportedModel, ILocalizedModel<MenuLocalizedModel>
{
    #region Ctor

    public MenuModel()
    {
		logger.LogDebug(">>> Entering MenuModel()");
        AvailableMenuTypes = new List<SelectListItem>();

		logger.LogDebug("MenuModel(): AvailableMenuTypes → {AvailableMenuTypes}", AvailableMenuTypes);
        SelectedStoreIds = new List<int>();
		logger.LogDebug("MenuModel(): SelectedStoreIds → {SelectedStoreIds}", SelectedStoreIds);
        AvailableStores = new List<SelectListItem>();

		logger.LogDebug("MenuModel(): AvailableStores → {AvailableStores}", AvailableStores);
        SelectedCustomerRoleIds = new List<int>();
        AvailableCustomerRoles = new List<SelectListItem>();
		logger.LogDebug("MenuModel(): SelectedCustomerRoleIds → {SelectedCustomerRoleIds}", SelectedCustomerRoleIds);
		logger.LogDebug("MenuModel(): AvailableCustomerRoles → {AvailableCustomerRoles}", AvailableCustomerRoles);

        MenuItemSearchModel = new();
        Locales = new List<MenuLocalizedModel>();
		logger.LogDebug("<<< Exiting MenuModel()");
		logger.LogDebug("MenuModel(): MenuItemSearchModel → {MenuItemSearchModel}", MenuItemSearchModel);
		logger.LogDebug("MenuModel(): Locales → {Locales}", Locales);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.Menus.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.Fields.MenuType")]
    public int MenuTypeId { get; set; }
    public string MenuTypeName { get; set; }
    public IList<SelectListItem> AvailableMenuTypes { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.Fields.DisplayAllCategories")]
    public bool DisplayAllCategories { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.Fields.CssClass")]
    public string CssClass { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.Fields.Published")]
    public bool Published { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; }
    public IList<SelectListItem> AvailableStores { get; set; }

    public IList<int> SelectedCustomerRoleIds { get; set; }
    public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    public MenuItemSearchModel MenuItemSearchModel { get; set; }

    public IList<MenuLocalizedModel> Locales { get; set; }

    #endregion
}

public partial record MenuLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Menus.Fields.Name")]
    public string Name { get; set; }
}
