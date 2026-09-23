using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Menus;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Topics;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Menus;
using Nop.Services.Security;
using Nop.Services.Topics;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Menus;
using Nop.Web.Areas.Admin.Models.Plugins;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Infrastructure;

/// <summary>
/// Represents ACL event consumer
/// </summary>
public partial class AclEventConsumer : IConsumer<ModelPreparedEvent<BaseNopModel>>,
    IConsumer<ModelReceivedEvent<BaseNopModel>>,
    IConsumer<EntityInsertedEvent<Manufacturer>>,
    IConsumer<EntityInsertedEvent<Product>>,
    IConsumer<EntityInsertedEvent<Topic>>,
    IConsumer<EntityInsertedEvent<Category>>,
    IConsumer<EntityInsertedEvent<Menu>>,
    IConsumer<EntityInsertedEvent<MenuItem>>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IAclService _aclService;
    protected readonly IAclSupportedModelFactory _aclSupportedModelFactory;
    protected readonly ICategoryService _categoryService;
    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly IMenuService _menuService;
    protected readonly IProductService _productService;
    protected readonly ITopicService _topicService;

    private static readonly Dictionary<string, IList<int>> _tempData = new(comparer: StringComparer.InvariantCultureIgnoreCase);

    #endregion

    #region Ctor

    public AclEventConsumer(CatalogSettings catalogSettings,
        IAclService aclService,
        IAclSupportedModelFactory aclSupportedModelFactory,
        ICategoryService categoryService,
        ICustomerService customerService,
        ILocalizationService localizationService,
        IManufacturerService manufacturerService,
        IMenuService menuService,
        IProductService productService,
        ITopicService topicService)
    {
		_logger.LogError(">>> Entering AclEventConsumer(catalogSettings={CatalogSettings},aclService={AclService},aclSupportedModelFactory={AclSupportedModelFactory},categoryService={CategoryService},customerService={CustomerService},localizationService={LocalizationService},manufacturerService={ManufacturerService},menuService={MenuService},productService={ProductService},topicService={TopicService})", catalogSettings, aclService, aclSupportedModelFactory, categoryService, customerService, localizationService, manufacturerService, menuService, productService, topicService);
        _aclService = aclService;
        _catalogSettings = catalogSettings;
		_logger.LogDebug("AclEventConsumer(catalogSettings={CatalogSettings},aclService={AclService},aclSupportedModelFactory={AclSupportedModelFactory},categoryService={CategoryService},customerService={CustomerService},localizationService={LocalizationService},manufacturerService={ManufacturerService},menuService={MenuService},productService={ProductService},topicService={TopicService}): _catalogSettings → {_catalogSettings}", catalogSettings, aclService, aclSupportedModelFactory, categoryService, customerService, localizationService, manufacturerService, menuService, productService, topicService, _catalogSettings);
		_logger.LogDebug("AclEventConsumer(catalogSettings={CatalogSettings},aclService={AclService},aclSupportedModelFactory={AclSupportedModelFactory},categoryService={CategoryService},customerService={CustomerService},localizationService={LocalizationService},manufacturerService={ManufacturerService},menuService={MenuService},productService={ProductService},topicService={TopicService}): _aclService → {_aclService}", catalogSettings, aclService, aclSupportedModelFactory, categoryService, customerService, localizationService, manufacturerService, menuService, productService, topicService, _aclService);
    _logger.LogWarning("<<< Exiting AclEventConsumer(catalogSettings={CatalogSettings},aclService={AclService},aclSupportedModelFactory={AclSupportedModelFactory},categoryService={CategoryService},customerService={CustomerService},localizationService={LocalizationService},manufacturerService={ManufacturerService},menuService={MenuService},productService={ProductService},topicService={TopicService})", catalogSettings, aclService, aclSupportedModelFactory, categoryService, customerService, localizationService, manufacturerService, menuService, productService, topicService);
        _aclSupportedModelFactory = aclSupportedModelFactory;
		_logger.LogDebug("AclEventConsumer(catalogSettings={CatalogSettings},aclService={AclService},aclSupportedModelFactory={AclSupportedModelFactory},categoryService={CategoryService},customerService={CustomerService},localizationService={LocalizationService},manufacturerService={ManufacturerService},menuService={MenuService},productService={ProductService},topicService={TopicService}): _aclSupportedModelFactory → {_aclSupportedModelFactory}", catalogSettings, aclService, aclSupportedModelFactory, categoryService, customerService, localizationService, manufacturerService, menuService, productService, topicService, _aclSupportedModelFactory);
        _categoryService = categoryService;
		_logger.LogDebug("AclEventConsumer(catalogSettings={CatalogSettings},aclService={AclService},aclSupportedModelFactory={AclSupportedModelFactory},categoryService={CategoryService},customerService={CustomerService},localizationService={LocalizationService},manufacturerService={ManufacturerService},menuService={MenuService},productService={ProductService},topicService={TopicService}): _categoryService → {_categoryService}", catalogSettings, aclService, aclSupportedModelFactory, categoryService, customerService, localizationService, manufacturerService, menuService, productService, topicService, _categoryService);
        _customerService = customerService;
		_logger.LogDebug("AclEventConsumer(catalogSettings={CatalogSettings},aclService={AclService},aclSupportedModelFactory={AclSupportedModelFactory},categoryService={CategoryService},customerService={CustomerService},localizationService={LocalizationService},manufacturerService={ManufacturerService},menuService={MenuService},productService={ProductService},topicService={TopicService}): _customerService → {_customerService}", catalogSettings, aclService, aclSupportedModelFactory, categoryService, customerService, localizationService, manufacturerService, menuService, productService, topicService, _customerService);
        _localizationService = localizationService;
		_logger.LogDebug("AclEventConsumer(catalogSettings={CatalogSettings},aclService={AclService},aclSupportedModelFactory={AclSupportedModelFactory},categoryService={CategoryService},customerService={CustomerService},localizationService={LocalizationService},manufacturerService={ManufacturerService},menuService={MenuService},productService={ProductService},topicService={TopicService}): _localizationService → {_localizationService}", catalogSettings, aclService, aclSupportedModelFactory, categoryService, customerService, localizationService, manufacturerService, menuService, productService, topicService, _localizationService);
        _manufacturerService = manufacturerService;
		_logger.LogDebug("AclEventConsumer(catalogSettings={CatalogSettings},aclService={AclService},aclSupportedModelFactory={AclSupportedModelFactory},categoryService={CategoryService},customerService={CustomerService},localizationService={LocalizationService},manufacturerService={ManufacturerService},menuService={MenuService},productService={ProductService},topicService={TopicService}): _manufacturerService → {_manufacturerService}", catalogSettings, aclService, aclSupportedModelFactory, categoryService, customerService, localizationService, manufacturerService, menuService, productService, topicService, _manufacturerService);
        _menuService = menuService;
		_logger.LogDebug("AclEventConsumer(catalogSettings={CatalogSettings},aclService={AclService},aclSupportedModelFactory={AclSupportedModelFactory},categoryService={CategoryService},customerService={CustomerService},localizationService={LocalizationService},manufacturerService={ManufacturerService},menuService={MenuService},productService={ProductService},topicService={TopicService}): _menuService → {_menuService}", catalogSettings, aclService, aclSupportedModelFactory, categoryService, customerService, localizationService, manufacturerService, menuService, productService, topicService, _menuService);
        _productService = productService;
		_logger.LogDebug("AclEventConsumer(catalogSettings={CatalogSettings},aclService={AclService},aclSupportedModelFactory={AclSupportedModelFactory},categoryService={CategoryService},customerService={CustomerService},localizationService={LocalizationService},manufacturerService={ManufacturerService},menuService={MenuService},productService={ProductService},topicService={TopicService}): _productService → {_productService}", catalogSettings, aclService, aclSupportedModelFactory, categoryService, customerService, localizationService, manufacturerService, menuService, productService, topicService, _productService);
        _topicService = topicService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Save ACL mapping
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <param name="key">Key to load mapping</param>
    /// <param name="entity">Entity to store mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SaveStoredDataAsync<TEntity>(string key, TEntity entity) where TEntity : BaseEntity, IAclSupported
    {
        if (!_tempData.ContainsKey(key))
            return;

        await _aclService.SaveAclAsync(entity, _tempData[key]);
        _tempData.Remove(key);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle model prepared event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(ModelPreparedEvent<BaseNopModel> eventMessage)
    {
        if (eventMessage.Model is not IAclSupportedModel)
            return;

        switch (eventMessage.Model)
        {
            case CategoryModel categoryModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(categoryModel, nameof(Category));
                break;
            case ManufacturerModel manufacturerModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(manufacturerModel, nameof(Manufacturer));
                break;
            case MenuModel menuModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(menuModel, nameof(Menu));
                break;
            case MenuItemModel menuItemModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(menuItemModel, nameof(MenuItem));
                break;
            case PluginModel pluginModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(pluginModel);
                break;
            case ProductModel productModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(productModel, nameof(Product));
                break;
            case TopicModel topicModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(topicModel, nameof(Topic));
                break;
            case CustomerSearchModel customerSearchModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(customerSearchModel);
                break;
            case OnlineCustomerSearchModel onlineCustomerSearchModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(onlineCustomerSearchModel);
                break;
        }
		_logger.LogError(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
		_logger.LogWarning("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
    }

    /// <summary>
    /// Handle model received event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(ModelReceivedEvent<BaseNopModel> eventMessage)
    {
        if (eventMessage.Model is not IAclSupportedModel model)
            return;

        var key = string.Empty;
		_logger.LogError("HandleEventAsync(eventMessage={EventMessage}): key → {Key}", eventMessage, key);

        switch (eventMessage.Model)
        {
            case ManufacturerModel manufacturerModel:
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerModel.Id);
				_logger.LogError("HandleEventAsync(eventMessage={EventMessage}): manufacturer → {Manufacturer}", eventMessage, manufacturer);
                await _aclService.SaveAclAsync(manufacturer, manufacturerModel.SelectedCustomerRoleIds);
                key = manufacturer == null ? manufacturerModel.Name : string.Empty;
                break;

            case MenuModel menuModel:
                var menu = await _menuService.GetMenuByIdAsync(menuModel.Id);
				_logger.LogError("HandleEventAsync(eventMessage={EventMessage}): menu → {Menu}", eventMessage, menu);
                await _aclService.SaveAclAsync(menu, menuModel.SelectedCustomerRoleIds);
                key = menu == null ? menuModel.Name : string.Empty;
                break;

            case MenuItemModel menuItemModel:
                var menuItem = await _menuService.GetMenuItemByIdAsync(menuItemModel.Id);
				_logger.LogError("HandleEventAsync(eventMessage={EventMessage}): menuItem → {MenuItem}", eventMessage, menuItem);
                await _aclService.SaveAclAsync(menuItem, menuItemModel.SelectedCustomerRoleIds);
                key = menuItem == null ? $"{nameof(MenuItem)}:{menuItemModel.Title}:{menuItemModel.MenuId}:{menuItemModel.MenuItemTypeId}" : string.Empty;
                break;

            case ProductModel productModel:
                var product = await _productService.GetProductByIdAsync(productModel.Id);
				_logger.LogError("HandleEventAsync(eventMessage={EventMessage}): product → {Product}", eventMessage, product);
                await _aclService.SaveAclAsync(product, productModel.SelectedCustomerRoleIds);
                key = product == null ? productModel.Name : string.Empty;
                break;

            case TopicModel topicModel:
                var topic = await _topicService.GetTopicByIdAsync(topicModel.Id);
				_logger.LogError("HandleEventAsync(eventMessage={EventMessage}): topic → {Topic}", eventMessage, topic);
                await _aclService.SaveAclAsync(topic, topicModel.SelectedCustomerRoleIds);
                key = topic == null ? topicModel.Title : string.Empty;
                break;

            case CategoryModel categoryModel:
                var category = await _categoryService.GetCategoryByIdAsync(categoryModel.Id);
				_logger.LogError("HandleEventAsync(eventMessage={EventMessage}): category → {Category}", eventMessage, category);
                await _aclService.SaveAclAsync(category, categoryModel.SelectedCustomerRoleIds);
                key = category == null ? categoryModel.Name : string.Empty;
                break;
        }

        if (!string.IsNullOrEmpty(key))
            _tempData[key] = model.SelectedCustomerRoleIds;
    }

    /// <summary>
    /// Handle manufacturer inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<Manufacturer> eventMessage)
    {
        var entity = eventMessage.Entity;
        await SaveStoredDataAsync(entity.Name, entity);
    }

    /// <summary>
    /// Handle product inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<Product> eventMessage)
    {
        var entity = eventMessage.Entity;
        await SaveStoredDataAsync(entity.Name, entity);
    }

    /// <summary>
    /// Handle topic inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<Topic> eventMessage)
    {
        var entity = eventMessage.Entity;
        await SaveStoredDataAsync(entity.Title, entity);
    }

    /// <summary>
    /// Handle category inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<Category> eventMessage)
    {
        var entity = eventMessage.Entity;
        await SaveStoredDataAsync(entity.Name, entity);
    }

    /// <summary>
    /// Handle menu inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<Menu> eventMessage)
    {
        var entity = eventMessage.Entity;
        await SaveStoredDataAsync(entity.Name, entity);
    }

    /// <summary>
    /// Handle menu item inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<MenuItem> eventMessage)
    {
        var entity = eventMessage.Entity;
        await SaveStoredDataAsync($"{nameof(MenuItem)}:{entity.Title}:{entity.MenuId}:{entity.MenuItemTypeId}", entity);
    }

    #endregion
}