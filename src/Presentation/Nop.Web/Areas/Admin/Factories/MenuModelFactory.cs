using System.util;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Menus;
using Nop.Core.Http;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Menus;
using Nop.Services.Messages;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Menus;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the menu model factory implementation
/// </summary>
public partial class MenuModelFactory : IMenuModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICategoryService _categoryService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly IMenuService _menuService;
    protected readonly INotificationService _notificationService;
    protected readonly IProductService _productService;
    protected readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
    protected readonly ITopicService _topicService;
    protected readonly IVendorService _vendorService;
    protected readonly MenuSettings _menuSettings;

    #endregion

    #region Ctor

    public MenuModelFactory(
        CatalogSettings catalogSettings,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICategoryService categoryService,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IManufacturerService manufacturerService,
        IMenuService menuService,
        INotificationService notificationService,
        IProductService productService,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
        ITopicService topicService,
        IVendorService vendorService,
        MenuSettings menuSettings)
    {
		_logger.LogError(">>> Entering MenuModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},manufacturerService={ManufacturerService},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},vendorService={VendorService},menuSettings={MenuSettings})", catalogSettings, baseAdminModelFactory, categoryService, localizationService, localizedModelFactory, manufacturerService, menuService, notificationService, productService, topicService, vendorService, menuSettings);
        _catalogSettings = catalogSettings;
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("MenuModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},manufacturerService={ManufacturerService},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},vendorService={VendorService},menuSettings={MenuSettings}): _baseAdminModelFactory → {_baseAdminModelFactory}", catalogSettings, baseAdminModelFactory, categoryService, localizationService, localizedModelFactory, manufacturerService, menuService, notificationService, productService, topicService, vendorService, menuSettings, _baseAdminModelFactory);
        _categoryService = categoryService;
		_logger.LogDebug("MenuModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},manufacturerService={ManufacturerService},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},vendorService={VendorService},menuSettings={MenuSettings}): _categoryService → {_categoryService}", catalogSettings, baseAdminModelFactory, categoryService, localizationService, localizedModelFactory, manufacturerService, menuService, notificationService, productService, topicService, vendorService, menuSettings, _categoryService);
        _localizationService = localizationService;
		_logger.LogDebug("MenuModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},manufacturerService={ManufacturerService},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},vendorService={VendorService},menuSettings={MenuSettings}): _localizationService → {_localizationService}", catalogSettings, baseAdminModelFactory, categoryService, localizationService, localizedModelFactory, manufacturerService, menuService, notificationService, productService, topicService, vendorService, menuSettings, _localizationService);
        _localizedModelFactory = localizedModelFactory;
		_logger.LogDebug("MenuModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},manufacturerService={ManufacturerService},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},vendorService={VendorService},menuSettings={MenuSettings}): _localizedModelFactory → {_localizedModelFactory}", catalogSettings, baseAdminModelFactory, categoryService, localizationService, localizedModelFactory, manufacturerService, menuService, notificationService, productService, topicService, vendorService, menuSettings, _localizedModelFactory);
        _manufacturerService = manufacturerService;
		_logger.LogDebug("MenuModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},manufacturerService={ManufacturerService},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},vendorService={VendorService},menuSettings={MenuSettings}): _manufacturerService → {_manufacturerService}", catalogSettings, baseAdminModelFactory, categoryService, localizationService, localizedModelFactory, manufacturerService, menuService, notificationService, productService, topicService, vendorService, menuSettings, _manufacturerService);
        _menuService = menuService;
		_logger.LogDebug("MenuModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},manufacturerService={ManufacturerService},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},vendorService={VendorService},menuSettings={MenuSettings}): _menuService → {_menuService}", catalogSettings, baseAdminModelFactory, categoryService, localizationService, localizedModelFactory, manufacturerService, menuService, notificationService, productService, topicService, vendorService, menuSettings, _menuService);
        _notificationService = notificationService;
		_logger.LogDebug("MenuModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},manufacturerService={ManufacturerService},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},vendorService={VendorService},menuSettings={MenuSettings}): _notificationService → {_notificationService}", catalogSettings, baseAdminModelFactory, categoryService, localizationService, localizedModelFactory, manufacturerService, menuService, notificationService, productService, topicService, vendorService, menuSettings, _notificationService);
        _productService = productService;
		_logger.LogDebug("MenuModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},manufacturerService={ManufacturerService},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},vendorService={VendorService},menuSettings={MenuSettings}): _productService → {_productService}", catalogSettings, baseAdminModelFactory, categoryService, localizationService, localizedModelFactory, manufacturerService, menuService, notificationService, productService, topicService, vendorService, menuSettings, _productService);
        _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
		_logger.LogDebug("MenuModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},manufacturerService={ManufacturerService},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},vendorService={VendorService},menuSettings={MenuSettings}): _storeMappingSupportedModelFactory → {_storeMappingSupportedModelFactory}", catalogSettings, baseAdminModelFactory, categoryService, localizationService, localizedModelFactory, manufacturerService, menuService, notificationService, productService, topicService, vendorService, menuSettings, _storeMappingSupportedModelFactory);
        _topicService = topicService;
		_logger.LogDebug("MenuModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},manufacturerService={ManufacturerService},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},vendorService={VendorService},menuSettings={MenuSettings}): _topicService → {_topicService}", catalogSettings, baseAdminModelFactory, categoryService, localizationService, localizedModelFactory, manufacturerService, menuService, notificationService, productService, topicService, vendorService, menuSettings, _topicService);
        _vendorService = vendorService;
		_logger.LogDebug("MenuModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},manufacturerService={ManufacturerService},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},vendorService={VendorService},menuSettings={MenuSettings}): _vendorService → {_vendorService}", catalogSettings, baseAdminModelFactory, categoryService, localizationService, localizedModelFactory, manufacturerService, menuService, notificationService, productService, topicService, vendorService, menuSettings, _vendorService);
        _menuSettings = menuSettings;
		_logger.LogDebug("MenuModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},manufacturerService={ManufacturerService},menuService={MenuService},notificationService={NotificationService},productService={ProductService},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},vendorService={VendorService},menuSettings={MenuSettings}): _menuSettings → {_menuSettings}", catalogSettings, baseAdminModelFactory, categoryService, localizationService, localizedModelFactory, manufacturerService, menuService, notificationService, productService, topicService, vendorService, menuSettings, _menuSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get breadcrumb for menu item
    /// </summary>
    /// <param name="item">Menu item</param>
    /// <param name="allItems">All menu items</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains menu item breadcrumb 
    /// </returns>
    protected virtual async Task<string> GetMenuItemBreadcrumbAsync(MenuItem item, IEnumerable<MenuItem> allItems)
    {
		_logger.LogError(">>> Entering GetMenuItemBreadcrumbAsync(item={Item},allItems={AllItems})", item, allItems);
        var titles = new List<string>();
        var current = item;
		_logger.LogError("GetMenuItemBreadcrumbAsync(item={Item},allItems={AllItems}): current → {Current}", item, allItems, current);
		_logger.LogError("GetMenuItemBreadcrumbAsync(item={Item},allItems={AllItems}): titles → {Titles}", item, allItems, titles);

        while (current is not null)
        {
            var entityId = current.EntityId ?? 0;

            titles.Insert(0, current.MenuItemType switch
            {
                MenuItemType.Vendor => (await _vendorService.GetVendorByIdAsync(entityId))?.Name,
                MenuItemType.Category => (await _categoryService.GetCategoryByIdAsync(entityId))?.Name,
                MenuItemType.TopicPage => (await _topicService.GetTopicByIdAsync(entityId))?.Title,
                MenuItemType.Product => (await _productService.GetProductByIdAsync(entityId))?.Name,
                MenuItemType.Manufacturer => (await _manufacturerService.GetManufacturerByIdAsync(entityId))?.Name,
                _ => null
            } ?? current.Title);

            current = allItems.FirstOrDefault(x => x.Id == current.ParentId);
        }

		_logger.LogWarning("<<< Exiting GetMenuItemBreadcrumbAsync(item={Item},allItems={AllItems})", item, allItems);
        return string.Join(" >> ", titles);
    }

    /// <summary>
    /// Initialize an entity identifier of the menu item model
    /// </summary>
    /// <param name="model">Menu item model</param>
    /// <param name="entityId">Entity identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    protected virtual async Task InitMenuItemModelEntityIdAsync(MenuItemModel model, int entityId)
    {
		_logger.LogError(">>> Entering InitMenuItemModelEntityIdAsync(model={Model},entityId={EntityId})", model, entityId);
        ArgumentOutOfRangeException.ThrowIfZero(entityId);

        try
        {
            switch ((MenuItemType)model.MenuItemTypeId)
            {
                case MenuItemType.Product:
                {
                    var product = await _productService.GetProductByIdAsync(entityId);
					_logger.LogError("InitMenuItemModelEntityIdAsync(model={Model},entityId={EntityId}): product → {Product}", model, entityId, product);

                    if (product is null || product.Deleted)
						_logger.LogError("Throwing NopException from InitMenuItemModelEntityIdAsync(model={Model},entityId={EntityId})", model, entityId);
                        throw new NopException("Product not found");

                    model.ProductName = product.Name;
					_logger.LogDebug("InitMenuItemModelEntityIdAsync(model={Model},entityId={EntityId}): model.ProductName → {Arg2}", model, entityId, model.ProductName);
					_logger.LogDebug("InitMenuItemModelEntityIdAsync(model={Model},entityId={EntityId}): model.ProductId → {Arg2}", model, entityId, model.ProductId);
                    model.ProductId = product.Id;
                    break;
                }
                case MenuItemType.TopicPage:
                {
                    var topic = await _topicService.GetTopicByIdAsync(entityId) ?? throw new NopException("Topic not found");
					_logger.LogError("InitMenuItemModelEntityIdAsync(model={Model},entityId={EntityId}): topic → {Topic}", model, entityId, topic);
					_logger.LogDebug("InitMenuItemModelEntityIdAsync(model={Model},entityId={EntityId}): model.TopicId → {Arg2}", model, entityId, model.TopicId);
                    model.TopicId = topic.Id;
                    break;
                }
                case MenuItemType.Category:
                {
                    var category = await _categoryService.GetCategoryByIdAsync(entityId);
					_logger.LogError("InitMenuItemModelEntityIdAsync(model={Model},entityId={EntityId}): category → {Category}", model, entityId, category);

                    if (category is null || category.Deleted)
                        throw new NopException("Category not found");

                    model.CategoryId = category.Id;
					_logger.LogDebug("InitMenuItemModelEntityIdAsync(model={Model},entityId={EntityId}): model.CategoryId → {Arg2}", model, entityId, model.CategoryId);
                    break;
                }
                case MenuItemType.Vendor:
                {
                    var vendor = await _vendorService.GetVendorByIdAsync(entityId);
					_logger.LogError("InitMenuItemModelEntityIdAsync(model={Model},entityId={EntityId}): vendor → {Vendor}", model, entityId, vendor);

                    if (vendor is null || vendor.Deleted)
                        throw new NopException("Manufacturer not found");

                    model.VendorId = vendor.Id;
					_logger.LogDebug("InitMenuItemModelEntityIdAsync(model={Model},entityId={EntityId}): model.VendorId → {Arg2}", model, entityId, model.VendorId);
                    break;
                }
                case MenuItemType.Manufacturer:
                {
                    var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(entityId);
					_logger.LogError("InitMenuItemModelEntityIdAsync(model={Model},entityId={EntityId}): manufacturer → {Manufacturer}", model, entityId, manufacturer);

                    if (manufacturer is null || manufacturer.Deleted)
                        throw new NopException("Manufacturer not found");

                    model.ManufacturerId = manufacturer.Id;
					_logger.LogDebug("InitMenuItemModelEntityIdAsync(model={Model},entityId={EntityId}): model.ManufacturerId → {Arg2}", model, entityId, model.ManufacturerId);
                    break;
                }
                default:
                    break;
            }
        }
        catch (NopException ex)
        {
			_logger.LogError("Exception in InitMenuItemModelEntityIdAsync(model={Model},entityId={EntityId}): {Arg2}", model, entityId, ex.Message);
            _notificationService.WarningNotification(ex.Message);
        }
		_logger.LogWarning("<<< Exiting InitMenuItemModelEntityIdAsync(model={Model},entityId={EntityId})", model, entityId);
    }

    #region Select lists

    /// <summary>
    /// Prepare available menu items
    /// </summary>
    /// <param name="menuItemModel">Menu item model</param>
    /// <param name="depth">Number of menu levels</param>
    /// <param name="menuItemsToAdd">List for adding menu items</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    protected virtual async Task PrepareModelAvailableParentMenuItemsAsync(MenuItemModel menuItemModel, int depth, IList<SelectListItem> menuItemsToAdd)
    {
		_logger.LogError(">>> Entering PrepareModelAvailableParentMenuItemsAsync(menuItemModel={MenuItemModel},depth={Depth},menuItemsToAdd={MenuItemsToAdd})", menuItemModel, depth, menuItemsToAdd);
        var items = await _menuService.GetAllMenuItemsAsync(menuId: menuItemModel.MenuId, treeSorting: true, showHidden: true);
        menuItemsToAdd.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.No"), Value = string.Empty });
		_logger.LogError("PrepareModelAvailableParentMenuItemsAsync(menuItemModel={MenuItemModel},depth={Depth},menuItemsToAdd={MenuItemsToAdd}): items → {Items}", menuItemModel, depth, menuItemsToAdd, items);

        if (menuItemModel.Id > 0 && childrenLevels(items, menuItemModel.Id) >= _menuSettings.MaximumMainMenuLevels - 1)
            return;

        var availableParents = _menuService
            .FilterMenuItemsByDepth(items, depthLimit: depth - 1)
            .Where(item => item.ParentId != item.Id && item.Id != menuItemModel.Id);

        foreach (var item in availableParents)
        {
            menuItemsToAdd.Add(new SelectListItem { Value = item.Id.ToString(), Text = await GetMenuItemBreadcrumbAsync(item, items) });
        }

        int childrenLevels(IEnumerable<MenuItem> elements, int parentId, HashSet<int> visited = null)
        {
            visited ??= new HashSet<int>();

            if (!visited.Add(parentId))
                return 0;

            var children = elements.Where(x => x.ParentId == parentId).ToList();
			_logger.LogError("PrepareModelAvailableParentMenuItemsAsync(menuItemModel={MenuItemModel},depth={Depth},menuItemsToAdd={MenuItemsToAdd}): children → {Children}", menuItemModel, depth, menuItemsToAdd, children);

            if (!children.Any())
                return 0;

            var maxChildDepth = 0;
			_logger.LogError("PrepareModelAvailableParentMenuItemsAsync(menuItemModel={MenuItemModel},depth={Depth},menuItemsToAdd={MenuItemsToAdd}): maxChildDepth → {MaxChildDepth}", menuItemModel, depth, menuItemsToAdd, maxChildDepth);
            foreach (var child in children)
            {
                var childDepth = childrenLevels(elements, child.Id, visited);
				_logger.LogError("PrepareModelAvailableParentMenuItemsAsync(menuItemModel={MenuItemModel},depth={Depth},menuItemsToAdd={MenuItemsToAdd}): childDepth → {ChildDepth}", menuItemModel, depth, menuItemsToAdd, childDepth);
                if (childDepth > maxChildDepth)
                    maxChildDepth = childDepth;
            }

			_logger.LogWarning("<<< Exiting PrepareModelAvailableParentMenuItemsAsync(menuItemModel={MenuItemModel},depth={Depth},menuItemsToAdd={MenuItemsToAdd})", menuItemModel, depth, menuItemsToAdd);
            return maxChildDepth + 1;
        }
    }

    /// <summary>
    /// Prepare topic list
    /// </summary>
    /// <param name="items">List to add available topics</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    protected virtual async Task PrepareAvailableTopicsAsync(IList<SelectListItem> items)
    {
		_logger.LogError(">>> Entering PrepareAvailableTopicsAsync(items={Items})", items);
        ArgumentNullException.ThrowIfNull(items);

        //get topics
        var topics = await _topicService.GetAllTopicsAsync(storeId: 0, showHidden: true);
		_logger.LogError("PrepareAvailableTopicsAsync(items={Items}): topics → {Topics}", items, topics);

        var availableTopics = topics.Select(t => new SelectListItem
        {
            Text = !string.IsNullOrEmpty(t.SystemName) ? t.SystemName : t.Title,
            Value = t.Id.ToString()
        }).ToList();
			_logger.LogDebug("PrepareAvailableTopicsAsync(items={Items}): Value → {Value}", items, Value);

        items.AddRange(availableTopics);
		_logger.LogWarning("<<< Exiting PrepareAvailableTopicsAsync(items={Items})", items);
    }

    #endregion

    #endregion

    #region Methods

    #region Menus

    /// <summary>
    /// Prepare menu search model
    /// </summary>
    /// <param name="searchModel">Menu search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the menu search model
    /// </returns>
    public virtual Task<MenuSearchModel> PrepareMenuSearchModelAsync(MenuSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareMenuSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareMenuSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged menu list model
    /// </summary>
    /// <param name="searchModel">Menu search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the menu list model
    /// </returns>
    public virtual async Task<MenuListModel> PrepareMenuListModelAsync(MenuSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareMenuListModelAsync(searchModel={SearchModel})", searchModel);
        var menus = await _menuService.GetAllMenusAsync(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize, showHidden: true);

		_logger.LogError("PrepareMenuListModelAsync(searchModel={SearchModel}): menus → {Menus}", searchModel, menus);
        //prepare list model
        return await new MenuListModel().PrepareToGridAsync(searchModel, menus, () =>
        {
            //fill in model values from the entity
            return menus.SelectAwait(async menu =>
            {
                var model = menu.ToModel<MenuModel>();
                model.MenuTypeName = await _localizationService.GetLocalizedEnumAsync((MenuType)model.MenuTypeId);

                return model;
            });
        });
		_logger.LogWarning("<<< Exiting PrepareMenuListModelAsync(searchModel={SearchModel})", searchModel);
    }

    /// <summary>
    /// Prepare menu model
    /// </summary>
    /// <param name="model">Menu model</param>
    /// <param name="menu">Menu</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the menu model
    /// </returns>
    public virtual async Task<MenuModel> PrepareMenuModelAsync(MenuModel model, Menu menu, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareMenuModelAsync(model={Model},menu={Menu},excludeProperties={ExcludeProperties})", model, menu, excludeProperties);
        Func<MenuLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (menu != null)
        {
            if (model == null)
                model = menu.ToModel<MenuModel>();
				_logger.LogError("PrepareMenuModelAsync(model={Model},menu={Menu},excludeProperties={ExcludeProperties}): model → {Model1}", model, menu, excludeProperties, model);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(menu, entity => entity.Name, languageId, false, false);
            };

            model.MenuItemSearchModel = await PrepareMenuItemSearchModelAsync(new MenuItemSearchModel { MenuId = menu.Id });
        }

        //prepare model stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, menu, excludeProperties);
        model.AvailableMenuTypes = (await MenuType.Footer.ToSelectListAsync(false)).ToList();

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogWarning("<<< Exiting PrepareMenuModelAsync(model={Model},menu={Menu},excludeProperties={ExcludeProperties})", model, menu, excludeProperties);
        return model;
    }

    #endregion

    #region Menu items

    /// <summary>
    /// Prepare menu items search model
    /// </summary>
    /// <param name="searchModel">Menu item search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the menu item search model
    /// </returns>
    public virtual Task<MenuItemSearchModel> PrepareMenuItemSearchModelAsync(MenuItemSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareMenuItemSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareMenuItemSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged menu item list model
    /// </summary>
    /// <param name="searchModel">Menu item search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the menu item list model
    /// </returns>
    public virtual async Task<MenuItemListModel> PrepareMenuItemListModelAsync(MenuItemSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareMenuItemListModelAsync(searchModel={SearchModel})", searchModel);
        var allItems = await _menuService.GetAllMenuItemsAsync(menuId: searchModel.MenuId, treeSorting: true, showHidden: true);

		_logger.LogError("PrepareMenuItemListModelAsync(searchModel={SearchModel}): allItems → {AllItems}", searchModel, allItems);
        var itemsPaged = allItems.ToPagedList(searchModel);
		_logger.LogError("PrepareMenuItemListModelAsync(searchModel={SearchModel}): itemsPaged → {ItemsPaged}", searchModel, itemsPaged);

        //prepare list model
        return await new MenuItemListModel().PrepareToGridAsync(searchModel, itemsPaged, () =>
        {
            //fill in model values from the entity
            return itemsPaged.SelectAwait(async menuItem =>
            {
                var model = menuItem.ToModel<MenuItemModel>();

                model.Breadcrumb = await GetMenuItemBreadcrumbAsync(menuItem, allItems);
                model.MenuItemTypeName = await _localizationService.GetLocalizedEnumAsync((MenuItemType)model.MenuItemTypeId);

                return model;
            });
        });
		_logger.LogWarning("<<< Exiting PrepareMenuItemListModelAsync(searchModel={SearchModel})", searchModel);
    }

    /// <summary>
    /// Prepare menu item model
    /// </summary>
    /// <param name="menu">Menu</param>
    /// <param name="model">Menu item model</param>
    /// <param name="menuItem">Menu item</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the menu item model
    /// </returns>
    public virtual async Task<MenuItemModel> PrepareMenuItemModelAsync(Menu menu, MenuItemModel model, MenuItem menuItem, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareMenuItemModelAsync(menu={Menu},model={Model},menuItem={MenuItem},excludeProperties={ExcludeProperties})", menu, model, menuItem, excludeProperties);
        ArgumentNullException.ThrowIfNull(menu);
        Func<MenuItemLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (menuItem != null)
        {
            if (model == null)
                model = menuItem.ToModel<MenuItemModel>();
				_logger.LogError("PrepareMenuItemModelAsync(menu={Menu},model={Model},menuItem={MenuItem},excludeProperties={ExcludeProperties}): model → {Model1}", menu, model, menuItem, excludeProperties, model);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Title = await _localizationService.GetLocalizedAsync(menuItem, entity => entity.Title, languageId, false, false);
            };

            if (menuItem.EntityId is int entityId && entityId > 0)
                await InitMenuItemModelEntityIdAsync(model, entityId);
        }
        else
        {
            model.MenuId = menu.Id;
			_logger.LogDebug("PrepareMenuItemModelAsync(menu={Menu},model={Model},menuItem={MenuItem},excludeProperties={ExcludeProperties}): model.MenuId → {Arg4}", menu, model, menuItem, excludeProperties, model.MenuId);
			_logger.LogDebug("PrepareMenuItemModelAsync(menu={Menu},model={Model},menuItem={MenuItem},excludeProperties={ExcludeProperties}): model.Published → {Arg4}", menu, model, menuItem, excludeProperties, model.Published);
			_logger.LogDebug("PrepareMenuItemModelAsync(menu={Menu},model={Model},menuItem={MenuItem},excludeProperties={ExcludeProperties}): model.MaximumNumberEntities → {Arg4}", menu, model, menuItem, excludeProperties, model.MaximumNumberEntities);
			_logger.LogDebug("PrepareMenuItemModelAsync(menu={Menu},model={Model},menuItem={MenuItem},excludeProperties={ExcludeProperties}): model.NumberOfSubItemsPerGridElement → {Arg4}", menu, model, menuItem, excludeProperties, model.NumberOfSubItemsPerGridElement);
			_logger.LogDebug("PrepareMenuItemModelAsync(menu={Menu},model={Model},menuItem={MenuItem},excludeProperties={ExcludeProperties}): model.NumberOfItemsPerGridRow → {Arg4}", menu, model, menuItem, excludeProperties, model.NumberOfItemsPerGridRow);
            model.Published = true;
            model.MaximumNumberEntities = _menuSettings.MaximumNumberEntities;
            model.NumberOfSubItemsPerGridElement = _menuSettings.NumberOfSubItemsPerGridElement;
            model.NumberOfItemsPerGridRow = _menuSettings.NumberOfItemsPerGridRow;
        }

        var isMainMenu = menu.MenuType == MenuType.Main;
        if (isMainMenu)
        {
            model.AvailableMenuItemTemplates.AddRange(await MenuItemTemplate.Simple.ToSelectListAsync(false));
            await PrepareModelAvailableParentMenuItemsAsync(model, _menuSettings.MaximumMainMenuLevels, model.AvailableMenuItems);
        }

        model.AvailableMenuItemTypes = (await MenuItemType.StandardPage.ToSelectListAsync(false)).ToList();
        model.AvailableStandardRoutes = await model.AvailableStandardRoutes.ConstantsToSelectListAsync(typeof(NopRouteNames.General), sortItems: true);

        await PrepareAvailableTopicsAsync(model.AvailableTopics);
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, menuItem, excludeProperties);
        await _baseAdminModelFactory.PrepareCategoriesAsync(model.AvailableCategories, isMainMenu);
        await _baseAdminModelFactory.PrepareManufacturersAsync(model.AvailableManufacturers, isMainMenu);
        await _baseAdminModelFactory.PrepareVendorsAsync(model.AvailableVendors, isMainMenu);

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogWarning("<<< Exiting PrepareMenuItemModelAsync(menu={Menu},model={Model},menuItem={MenuItem},excludeProperties={ExcludeProperties})", menu, model, menuItem, excludeProperties);
        return model;
    }

    #region Products popup

    /// <summary>
    /// Prepare product search model
    /// </summary>
    /// <param name="searchModel">Product search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product search model
    /// </returns>
    public virtual async Task<SelectMenuItemProductSearchModel> PrepareMenuItemSelectProductSearchModelAsync(SelectMenuItemProductSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareMenuItemSelectProductSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareMenuItemSelectProductSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare product list model
    /// </summary>
    /// <param name="searchModel">Product search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product list model
    /// </returns>
    public virtual async Task<SelectMenuItemProductListModel> PrepareMenuItemSelectProductListModelAsync(SelectMenuItemProductSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareMenuItemSelectProductListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get categories
        var products = await _productService.SearchProductsAsync(keywords: searchModel.SearchKeywords,
            showHidden: true,
            storeId: searchModel.SearchStoreId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        return new SelectMenuItemProductListModel().PrepareToGrid(searchModel, products, () =>
        {
            return products.Select(product => new SelectMenuItemProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Published = product.Published
            });
        });
		_logger.LogWarning("<<< Exiting PrepareMenuItemSelectProductListModelAsync(searchModel={SearchModel})", searchModel);
    }

    #endregion

    #endregion

    #endregion
}