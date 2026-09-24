using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Menus;

/// <summary>
/// Represents menu model
/// </summary>
public partial record MenuModel : BaseNopEntityModel, IAclSupportedModel, IStoreMappingSupportedModel, ILocalizedModel<MenuLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public MenuModel()
    {
		_logger.LogError(">>> Entering MenuModel()");
        AvailableMenuTypes = new List<SelectListItem>();

		_logger.LogDebug("MenuModel(): AvailableMenuTypes → {AvailableMenuTypes}", AvailableMenuTypes);
        SelectedStoreIds = new List<int>();
		_logger.LogError("MenuModel(): SelectedStoreIds → {SelectedStoreIds}", SelectedStoreIds);
		_logger.LogDebug("MenuModel(): SelectedStoreIds → {SelectedStoreIds}", SelectedStoreIds);
        AvailableStores = new List<SelectListItem>();
		_logger.LogError("MenuModel(): AvailableStores → {AvailableStores}", AvailableStores);

        SelectedCustomerRoleIds = new List<int>();
		_logger.LogError("MenuModel(): SelectedCustomerRoleIds → {SelectedCustomerRoleIds}", SelectedCustomerRoleIds);
		_logger.LogDebug("MenuModel(): AvailableStores → {AvailableStores}", AvailableStores);
        AvailableCustomerRoles = new List<SelectListItem>();
		_logger.LogError("MenuModel(): AvailableCustomerRoles → {AvailableCustomerRoles}", AvailableCustomerRoles);

        MenuItemSearchModel = new();
		_logger.LogError("MenuModel(): MenuItemSearchModel → {MenuItemSearchModel}", MenuItemSearchModel);
        Locales = new List<MenuLocalizedModel>();
		_logger.LogError("MenuModel(): Locales → {Locales}", Locales);
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