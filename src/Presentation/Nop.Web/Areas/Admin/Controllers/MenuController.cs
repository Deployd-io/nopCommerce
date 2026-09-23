using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Menus;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Menus;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Menus;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class MenuController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly IMenuModelFactory _menuModelFactory;
    protected readonly IMenuService _menuService;
    protected readonly INotificationService _notificationService;
    protected readonly IProductService _productService;
    protected readonly IStoreMappingService _storeMappingService;

    #endregion

    #region Ctor

    public MenuController(
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        IMenuModelFactory menuModelFactory,
        IMenuService menuService,
        INotificationService notificationService,
        IProductService productService,
        IStoreMappingService storeMappingService)
    {
		_logger.LogError(">>> Entering MenuController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},menuModelFactory={MenuModelFactory},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingService=<redacted>)", customerActivityService, localizationService, localizedEntityService, menuModelFactory, menuService, notificationService, productService);
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
		_logger.LogDebug("MenuController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},menuModelFactory={MenuModelFactory},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingService=<redacted>): _localizationService → {_localizationService}", customerActivityService, localizationService, localizedEntityService, menuModelFactory, menuService, notificationService, productService, _localizationService);
        _localizedEntityService = localizedEntityService;
		_logger.LogDebug("MenuController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},menuModelFactory={MenuModelFactory},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingService=<redacted>): _localizedEntityService → {_localizedEntityService}", customerActivityService, localizationService, localizedEntityService, menuModelFactory, menuService, notificationService, productService, _localizedEntityService);
        _menuModelFactory = menuModelFactory;
		_logger.LogDebug("MenuController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},menuModelFactory={MenuModelFactory},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingService=<redacted>): _menuModelFactory → {_menuModelFactory}", customerActivityService, localizationService, localizedEntityService, menuModelFactory, menuService, notificationService, productService, _menuModelFactory);
        _menuService = menuService;
		_logger.LogDebug("MenuController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},menuModelFactory={MenuModelFactory},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingService=<redacted>): _menuService → {_menuService}", customerActivityService, localizationService, localizedEntityService, menuModelFactory, menuService, notificationService, productService, _menuService);
        _notificationService = notificationService;
		_logger.LogDebug("MenuController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},menuModelFactory={MenuModelFactory},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingService=<redacted>): _notificationService → {_notificationService}", customerActivityService, localizationService, localizedEntityService, menuModelFactory, menuService, notificationService, productService, _notificationService);
        _productService = productService;
		_logger.LogDebug("MenuController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},menuModelFactory={MenuModelFactory},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingService=<redacted>): _productService → {_productService}", customerActivityService, localizationService, localizedEntityService, menuModelFactory, menuService, notificationService, productService, _productService);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("MenuController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},menuModelFactory={MenuModelFactory},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingService=<redacted>): _storeMappingService → {_storeMappingService}", customerActivityService, localizationService, localizedEntityService, menuModelFactory, menuService, notificationService, productService, _storeMappingService);
    }

    #endregion

    #region Utilities

    protected virtual void DefinePopupViewBagData(int entityId, string entityInfo)
    {
		_logger.LogError(">>> Entering DefinePopupViewBagData(entityId={EntityId},entityInfo={EntityInfo})", entityId, entityInfo);
		_logger.LogDebug("DefinePopupViewBagData(entityId={EntityId},entityInfo={EntityInfo}): ViewBag.entityInfo → {Arg2}", entityId, entityInfo, ViewBag.entityInfo);
		_logger.LogDebug("DefinePopupViewBagData(entityId={EntityId},entityInfo={EntityInfo}): ViewBag.entityId → {Arg2}", entityId, entityInfo, ViewBag.entityId);
        ViewBag.RefreshPage = true;
        ViewBag.entityId = entityId;
        ViewBag.entityInfo = entityInfo;
    }

    protected virtual async Task<MenuItem> SaveMenuItemAsync(Menu menu, MenuItemModel model, MenuItem menuItem, Func<MenuItem, Task> serviceAction)
    {
		_logger.LogError(">>> Entering SaveMenuItemAsync(menu={Menu},model={Model},menuItem={MenuItem},serviceAction={ServiceAction})", menu, model, menuItem, serviceAction);
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(menuItem);

        menuItem = model.ToEntity(menuItem);
		_logger.LogError("SaveMenuItemAsync(menu={Menu},model={Model},menuItem={MenuItem},serviceAction={ServiceAction}): menuItem → {MenuItem1}", menu, model, menuItem, serviceAction, menuItem);
		_logger.LogDebug("SaveMenuItemAsync(menu={Menu},model={Model},menuItem={MenuItem},serviceAction={ServiceAction}): menuItem.EntityId → {Arg4}", menu, model, menuItem, serviceAction, menuItem.EntityId);

        menuItem.EntityId = menuItem.MenuItemType switch
        {
            MenuItemType.Category => model.CategoryId,
            MenuItemType.TopicPage => model.TopicId,
            MenuItemType.Manufacturer => model.ManufacturerId,
            MenuItemType.Vendor => model.VendorId,
            MenuItemType.Product => model.ProductId,
            _ => null
        };

        if ((menuItem.MenuItemType != MenuItemType.Category && (menuItem.EntityId ?? 0) != 0) || menu.MenuType == MenuType.Footer)
            menuItem.Template = MenuItemTemplate.Simple;
			_logger.LogDebug("SaveMenuItemAsync(menu={Menu},model={Model},menuItem={MenuItem},serviceAction={ServiceAction}): menuItem.Template → {Arg4}", menu, model, menuItem, serviceAction, menuItem.Template);

        if (menuItem.MenuItemType != MenuItemType.StandardPage)
            menuItem.RouteName = null;
			_logger.LogDebug("SaveMenuItemAsync(menu={Menu},model={Model},menuItem={MenuItem},serviceAction={ServiceAction}): menuItem.RouteName → {Arg4}", menu, model, menuItem, serviceAction, menuItem.RouteName);

        await serviceAction(menuItem);

        //stores
        await _storeMappingService.SaveStoreMappingsAsync(menuItem, model.SelectedStoreIds);

        //locales
        await UpdateMenuItemLocalesAsync(menuItem, model);

		_logger.LogWarning("<<< Exiting SaveMenuItemAsync(menu={Menu},model={Model},menuItem={MenuItem},serviceAction={ServiceAction})", menu, model, menuItem, serviceAction);
        return menuItem;
    }

    protected virtual async Task UpdateMenuLocalesAsync(Menu menu, MenuModel model)
    {
		_logger.LogError(">>> Entering UpdateMenuLocalesAsync(menu={Menu},model={Model})", menu, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(menu,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
		_logger.LogWarning("<<< Exiting UpdateMenuLocalesAsync(menu={Menu},model={Model})", menu, model);
    }

    protected virtual async Task UpdateMenuItemLocalesAsync(MenuItem menuItem, MenuItemModel model)
    {
		_logger.LogError(">>> Entering UpdateMenuItemLocalesAsync(menuItem={MenuItem},model={Model})", menuItem, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(menuItem,
                x => x.Title,
                localized.Title,
                localized.LanguageId);
        }
		_logger.LogWarning("<<< Exiting UpdateMenuItemLocalesAsync(menuItem={MenuItem},model={Model})", menuItem, model);
    }

    #endregion

    #region Methods

    #region Menus

    public virtual IActionResult Index()
    {
		_logger.LogError(">>> Entering Index()");
        _logger.LogWarning("<<< Exiting Index()");
        return RedirectToAction(nameof(List));
    }

    [CheckPermission(StandardPermission.ContentManagement.MENU_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogError(">>> Entering List()");
        //prepare model
        var model = await _menuModelFactory.PrepareMenuSearchModelAsync(new MenuSearchModel());
		_logger.LogError("List(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.MENU_VIEW)]
    public virtual async Task<IActionResult> MenuList(MenuSearchModel searchModel)
    {
		_logger.LogError(">>> Entering MenuList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _menuModelFactory.PrepareMenuListModelAsync(searchModel);
		_logger.LogError("MenuList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting MenuList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.MENU_VIEW)]
    public virtual async Task<IActionResult> MenuItemList(MenuItemSearchModel searchModel)
    {
		_logger.LogError(">>> Entering MenuItemList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _menuModelFactory.PrepareMenuItemListModelAsync(searchModel);
		_logger.LogError("MenuItemList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting MenuItemList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }


    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogError(">>> Entering Create()");
        //prepare model
        var model = await _menuModelFactory.PrepareMenuModelAsync(new MenuModel(), null);
		_logger.LogError("Create(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting Create()");
		_logger.LogWarning("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(MenuModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var menu = model.ToEntity<Menu>();
            await _menuService.InsertMenuAsync(menu);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(menu, model.SelectedCustomerRoleIds);

            await UpdateMenuLocalesAsync(menu, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Menus.Added"));

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewMenu",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewMenu"), menu.Name), menu);

            if (!continueEditing)
                return RedirectToAction(nameof(List));

            return RedirectToAction(nameof(Edit), new { id = menu.Id });
        }

        //prepare model
        model = await _menuModelFactory.PrepareMenuModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.MENU_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogError(">>> Entering Edit(id={Id})", id);
        var menu = await _menuService.GetMenuByIdAsync(id);
        if (menu == null)
            return RedirectToAction(nameof(List));

        //prepare model
        var model = await _menuModelFactory.PrepareMenuModelAsync(null, menu);
		_logger.LogError("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting Edit(id={Id})", id);
		_logger.LogWarning("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(MenuModel model, bool continueEditing)
    {
        var menu = await _menuService.GetMenuByIdAsync(model.Id);
        if (menu == null)
            return RedirectToAction(nameof(List));

        if (ModelState.IsValid)
        {
            menu = model.ToEntity(menu);
            await _menuService.UpdateMenuAsync(menu);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(menu, model.SelectedStoreIds);

            await UpdateMenuLocalesAsync(menu, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Menus.Updated"));

            //activity log
            await _customerActivityService.InsertActivityAsync("EditMenu",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditMenu"), menu.Name), menu);

            if (!continueEditing)
                return RedirectToAction(nameof(List));

            return RedirectToAction(nameof(Edit), new { id = menu.Id });
        }

        //prepare model
        model = await _menuModelFactory.PrepareMenuModelAsync(model, menu, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogError(">>> Entering Delete(id={Id})", id);
        var menu = await _menuService.GetMenuByIdAsync(id);
        if (menu is null)
            return RedirectToAction(nameof(List));

        await _menuService.DeleteMenuAsync(menu);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Menus.Deleted"));

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteMenu",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteMenu"), menu.Name), menu);

		_logger.LogWarning("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction(nameof(List));
    }

    #endregion

    #region Menu items

    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> MenuItemCreate(int menuId)
    {
		_logger.LogError(">>> Entering MenuItemCreate(menuId={MenuId})", menuId);
        var menu = await _menuService.GetMenuByIdAsync(menuId);
        if (menu == null)
            return RedirectToAction(nameof(List));

        //prepare model
        var model = await _menuModelFactory.PrepareMenuItemModelAsync(menu, new MenuItemModel(), null);
		_logger.LogError("MenuItemCreate(menuId={MenuId}): model → {Model}", menuId, model);

		_logger.LogWarning("<<< Exiting MenuItemCreate(menuId={MenuId})", menuId);
		_logger.LogWarning("<<< Exiting MenuItemCreate(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> MenuItemCreate(MenuItemModel model, bool continueEditing)
    {
        var menu = await _menuService.GetMenuByIdAsync(model.MenuId);
        if (menu == null)
            return RedirectToAction(nameof(List));

        if (ModelState.IsValid)
        {
            var menuItem = await SaveMenuItemAsync(menu, model, model.ToEntity<MenuItem>(), _menuService.InsertMenuItemAsync);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Menus.Added"));

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewMenuItem",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewMenuItem"), menuItem.Id), menuItem);

            if (!continueEditing)
                return RedirectToAction(nameof(Edit), new { id = menuItem.MenuId });

            return RedirectToAction(nameof(MenuItemEdit), new { id = menuItem.Id });
        }

        //prepare model
        model = await _menuModelFactory.PrepareMenuItemModelAsync(menu, model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.MENU_VIEW)]
    public virtual async Task<IActionResult> MenuItemEdit(int id)
    {
		_logger.LogError(">>> Entering MenuItemEdit(id={Id})", id);
        var menuItem = await _menuService.GetMenuItemByIdAsync(id);
        if (menuItem == null)
            return RedirectToAction(nameof(List));

        var menu = await _menuService.GetMenuByIdAsync(menuItem.MenuId);
		_logger.LogError("MenuItemEdit(id={Id}): menu → {Menu}", id, menu);
        if (menu == null)
            return RedirectToAction(nameof(List));

        //prepare model
        var model = await _menuModelFactory.PrepareMenuItemModelAsync(menu, null, menuItem);
		_logger.LogError("MenuItemEdit(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting MenuItemEdit(id={Id})", id);
		_logger.LogWarning("<<< Exiting MenuItemEdit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> MenuItemEdit(MenuItemModel model, bool continueEditing)
    {
        var menu = await _menuService.GetMenuByIdAsync(model.MenuId);
        if (menu == null)
            return RedirectToAction(nameof(List));

        var menuItem = await _menuService.GetMenuItemByIdAsync(model.Id);
        if (menuItem == null)
            return RedirectToAction(nameof(List));

        if (ModelState.IsValid)
        {
            await SaveMenuItemAsync(menu, model, menuItem, _menuService.UpdateMenuItemAsync);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Menus.MenuItems.Updated"));

            //activity log
            await _customerActivityService.InsertActivityAsync("EditMenuItem",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditMenuItem"), menuItem.Title), menuItem);

            if (!continueEditing)
                return RedirectToAction(nameof(Edit), new { id = menuItem.MenuId });

            return RedirectToAction(nameof(MenuItemEdit), new { id = menuItem.Id });
        }

        //prepare model
        model = await _menuModelFactory.PrepareMenuItemModelAsync(menu, model, menuItem, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> MenuItemDelete(int id)
    {
		_logger.LogError(">>> Entering MenuItemDelete(id={Id})", id);
        var menuItem = await _menuService.GetMenuItemByIdAsync(id);
        if (menuItem is null)
            return RedirectToAction(nameof(List));

        await _menuService.DeleteMenuItemAsync(menuItem);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Menus.MenuItems.Deleted"));

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteMenuItem",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteMenuItem"), menuItem.Title), menuItem);

		_logger.LogWarning("<<< Exiting MenuItemDelete(id={Id})", id);
        return RedirectToAction(nameof(Edit), new { id = menuItem.MenuId });
    }

    #endregion

    #region Popups

    #region Products

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> MenuItemSelectProductPopup(int menuItemId)
    {
		_logger.LogError(">>> Entering MenuItemSelectProductPopup(menuItemId={MenuItemId})", menuItemId);
        //prepare model
        var model = await _menuModelFactory.PrepareMenuItemSelectProductSearchModelAsync(new SelectMenuItemProductSearchModel());
		_logger.LogError("MenuItemSelectProductPopup(menuItemId={MenuItemId}): model → {Model}", menuItemId, model);

		_logger.LogWarning("<<< Exiting MenuItemSelectProductPopup(menuItemId={MenuItemId})", menuItemId);
		_logger.LogWarning("<<< Exiting MenuItemSelectProductPopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> MenuItemSelectProductPopupList(SelectMenuItemProductSearchModel searchModel)
    {
		_logger.LogError(">>> Entering MenuItemSelectProductPopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _menuModelFactory.PrepareMenuItemSelectProductListModelAsync(searchModel);
		_logger.LogError("MenuItemSelectProductPopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting MenuItemSelectProductPopupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> MenuItemSelectProductPopup(SelectMenuItemEntityModel model)
    {
        var product = await _productService.GetProductByIdAsync(model.EntityId)
            ?? throw new ArgumentException("No products found with the specified id");

        DefinePopupViewBagData(product.Id, product.Name);

        return View(new SelectMenuItemProductSearchModel());
    }
    #endregion

    #endregion

    #endregion
}
