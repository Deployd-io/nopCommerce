using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Tax.Avalara.Models.ItemClassification;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Controllers;

public class ItemClassificationController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AvalaraTaxManager _avalaraTaxManager;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICountryService _countryService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IProductService _productService;
    protected readonly ITaxPluginManager _taxPluginManager;
    protected readonly ItemClassificationService _itemClassificationService;

    #endregion

    #region Ctor

    public ItemClassificationController(AvalaraTaxManager avalaraTaxManager,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICountryService countryService,
        IDateTimeHelper dateTimeHelper,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IProductService productService,
        ITaxPluginManager taxPluginManager,
        ItemClassificationService itemClassificationService)
    {
		_logger.LogDebug(">>> Entering ItemClassificationController(avalaraTaxManager={AvalaraTaxManager},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},taxPluginManager={TaxPluginManager},itemClassificationService={ItemClassificationService})", avalaraTaxManager, baseAdminModelFactory, countryService, dateTimeHelper, localizationService, notificationService, permissionService, productService, taxPluginManager, itemClassificationService);
        _avalaraTaxManager = avalaraTaxManager;
		_logger.LogDebug("ItemClassificationController(avalaraTaxManager={AvalaraTaxManager},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},taxPluginManager={TaxPluginManager},itemClassificationService={ItemClassificationService}): _avalaraTaxManager → {_avalaraTaxManager}", avalaraTaxManager, baseAdminModelFactory, countryService, dateTimeHelper, localizationService, notificationService, permissionService, productService, taxPluginManager, itemClassificationService, _avalaraTaxManager);
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("ItemClassificationController(avalaraTaxManager={AvalaraTaxManager},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},taxPluginManager={TaxPluginManager},itemClassificationService={ItemClassificationService}): _baseAdminModelFactory → {_baseAdminModelFactory}", avalaraTaxManager, baseAdminModelFactory, countryService, dateTimeHelper, localizationService, notificationService, permissionService, productService, taxPluginManager, itemClassificationService, _baseAdminModelFactory);
        _countryService = countryService;
		_logger.LogDebug("ItemClassificationController(avalaraTaxManager={AvalaraTaxManager},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},taxPluginManager={TaxPluginManager},itemClassificationService={ItemClassificationService}): _countryService → {_countryService}", avalaraTaxManager, baseAdminModelFactory, countryService, dateTimeHelper, localizationService, notificationService, permissionService, productService, taxPluginManager, itemClassificationService, _countryService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("ItemClassificationController(avalaraTaxManager={AvalaraTaxManager},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},taxPluginManager={TaxPluginManager},itemClassificationService={ItemClassificationService}): _dateTimeHelper → {_dateTimeHelper}", avalaraTaxManager, baseAdminModelFactory, countryService, dateTimeHelper, localizationService, notificationService, permissionService, productService, taxPluginManager, itemClassificationService, _dateTimeHelper);
        _localizationService = localizationService;
		_logger.LogDebug("ItemClassificationController(avalaraTaxManager={AvalaraTaxManager},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},taxPluginManager={TaxPluginManager},itemClassificationService={ItemClassificationService}): _localizationService → {_localizationService}", avalaraTaxManager, baseAdminModelFactory, countryService, dateTimeHelper, localizationService, notificationService, permissionService, productService, taxPluginManager, itemClassificationService, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("ItemClassificationController(avalaraTaxManager={AvalaraTaxManager},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},taxPluginManager={TaxPluginManager},itemClassificationService={ItemClassificationService}): _notificationService → {_notificationService}", avalaraTaxManager, baseAdminModelFactory, countryService, dateTimeHelper, localizationService, notificationService, permissionService, productService, taxPluginManager, itemClassificationService, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("ItemClassificationController(avalaraTaxManager={AvalaraTaxManager},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},taxPluginManager={TaxPluginManager},itemClassificationService={ItemClassificationService}): _permissionService → {_permissionService}", avalaraTaxManager, baseAdminModelFactory, countryService, dateTimeHelper, localizationService, notificationService, permissionService, productService, taxPluginManager, itemClassificationService, _permissionService);
        _productService = productService;
		_logger.LogDebug("ItemClassificationController(avalaraTaxManager={AvalaraTaxManager},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},taxPluginManager={TaxPluginManager},itemClassificationService={ItemClassificationService}): _productService → {_productService}", avalaraTaxManager, baseAdminModelFactory, countryService, dateTimeHelper, localizationService, notificationService, permissionService, productService, taxPluginManager, itemClassificationService, _productService);
        _taxPluginManager = taxPluginManager;
		_logger.LogDebug("ItemClassificationController(avalaraTaxManager={AvalaraTaxManager},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},taxPluginManager={TaxPluginManager},itemClassificationService={ItemClassificationService}): _taxPluginManager → {_taxPluginManager}", avalaraTaxManager, baseAdminModelFactory, countryService, dateTimeHelper, localizationService, notificationService, permissionService, productService, taxPluginManager, itemClassificationService, _taxPluginManager);
        _itemClassificationService = itemClassificationService;
		_logger.LogDebug("ItemClassificationController(avalaraTaxManager={AvalaraTaxManager},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},taxPluginManager={TaxPluginManager},itemClassificationService={ItemClassificationService}): _itemClassificationService → {_itemClassificationService}", avalaraTaxManager, baseAdminModelFactory, countryService, dateTimeHelper, localizationService, notificationService, permissionService, productService, taxPluginManager, itemClassificationService, _itemClassificationService);
    }

    #endregion

    #region Methods

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> List(ItemClassificationSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering List(searchModel={SearchModel})", searchModel);
        //get item classification
        var itemClassification = await _itemClassificationService.GetItemClassificationAsync(
            pageIndex: searchModel.Page - 1,
            pageSize: searchModel.PageSize,
            countryId: searchModel.SearchCountryId,
            productId: null);

        //prepare grid model
        var model = await new ItemClassificationListModel().PrepareToGridAsync(searchModel, itemClassification, () =>
        {
            return itemClassification.SelectAwait(async item => new ItemClassificationModel
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = (await _productService.GetProductByIdAsync(item.ProductId))?.Name ?? "",
                HSClassificationRequestId = item.HSClassificationRequestId,
                CountryId = item.CountryId,
                CountryName = (await _countryService.GetCountryByIdAsync(item.CountryId))?.Name ?? "*",
                HSCode = item.HSCode,
                UpdatedDate = await _dateTimeHelper.ConvertToUserTimeAsync(item.UpdatedOnUtc, DateTimeKind.Utc)
            });
        });

		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> Update(ItemClassificationModel model)
    {
		_logger.LogDebug(">>> Entering Update(model={Model})", model);
        var item = await _itemClassificationService.GetItemClassificationByIdAsync(model.Id)
            ?? throw new ArgumentException("No record found");

        item.HSCode = model.HSCode;
		_logger.LogDebug("Update(model={Model}): item.HSCode → {Arg1}", model, item.HSCode);

        await _itemClassificationService.UpdateItemClassificationAsync(item);

		_logger.LogDebug("<<< Exiting Update(model={Model})", model);
        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> DeleteSelected(List<int> selectedIds)
    {
		_logger.LogDebug(">>> Entering DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        if (!selectedIds?.Any() ?? true)
            return NoContent();

        var recordsToDelete = new List<int>();
		_logger.LogDebug("DeleteSelected(selectedIds={SelectedIds}): recordsToDelete → {RecordsToDelete}", selectedIds, recordsToDelete);

        foreach (var id in selectedIds)
        {
            var item = await _itemClassificationService.GetItemClassificationByIdAsync(id);
			_logger.LogDebug("DeleteSelected(selectedIds={SelectedIds}): item → {Item}", selectedIds, item);
            if (item is null)
                continue;

            recordsToDelete.Add(item.Id);
        }
        await _itemClassificationService.DeleteItemsAsync(recordsToDelete);

		_logger.LogDebug("<<< Exiting DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> ClearAll()
    {
		_logger.LogDebug(">>> Entering ClearAll()");
        await _itemClassificationService.ClearItemClassificationAsync();

		_logger.LogDebug("<<< Exiting ClearAll()");
        return Json(new { Result = true });
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> ProductToClassification()
    {
		_logger.LogDebug(">>> Entering ProductToClassification()");
        var model = new AddProductToClassificationSearchModel();
        await _baseAdminModelFactory.PrepareProductTypesAsync(model.AvailableProductTypes);
		_logger.LogDebug("ProductToClassification(): model → {Model}", model);
        await _baseAdminModelFactory.PrepareCategoriesAsync(model.AvailableCategories);
        await _baseAdminModelFactory.PrepareManufacturersAsync(model.AvailableManufacturers);
        await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores);
        await _baseAdminModelFactory.PrepareVendorsAsync(model.AvailableVendors);
        model.SetPopupGridPageSize();

		_logger.LogDebug("<<< Exiting ProductToClassification()");
		_logger.LogDebug("<<< Exiting ProductToClassification(model={Model})", model);
        return View("~/Plugins/Tax.Avalara/Views/ItemClassification/ProductToClassification.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> ProductListToClassification(AddProductToClassificationSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ProductListToClassification(searchModel={SearchModel})", searchModel);
        var products = await _productService.SearchProductsAsync(showHidden: true,
            keywords: searchModel.SearchProductName,
            productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
            categoryIds: new List<int> { searchModel.SearchCategoryId },
            manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
            storeId: searchModel.SearchStoreId,
            vendorId: searchModel.SearchVendorId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        var model = new AddProductToClassificationListModel().PrepareToGrid(searchModel, products, () =>
        {
            return products.Select(product => new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Sku = product.Sku,
                Price = product.Price,
                Published = product.Published
            }).ToList();
        });

		_logger.LogDebug("<<< Exiting ProductListToClassification(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> ProductToClassification(AddProductToClassificationModel model)
    {
        await _itemClassificationService.AddItemClassificationAsync(model.SelectedProductIds?.ToList());

        ViewBag.RefreshPage = true;

        return View("~/Plugins/Tax.Avalara/Views/ItemClassification/ProductToClassification.cshtml", new AddProductToClassificationSearchModel());
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> StartClassification()
    {
		_logger.LogDebug(">>> Entering StartClassification()");
        var items = (await _itemClassificationService.GetItemClassificationAsync())
            .Where(x => string.IsNullOrEmpty(x.HSClassificationRequestId))
            .ToList();
        foreach (var item in items)
        {
            var (classification, error) = await _avalaraTaxManager.ClassificationProductsAsync(item);

            if (!string.IsNullOrEmpty(error))
                return Json(new { success = false, message = error });

            if (!string.IsNullOrEmpty(classification?.Id))
            {
                //save classification id for future use (get hsCode)
                item.HSClassificationRequestId = classification.Id;
                item.UpdatedOnUtc = DateTime.UtcNow;
                await _itemClassificationService.UpdateItemClassificationAsync(item);
            }
        }

		_logger.LogDebug("<<< Exiting StartClassification()");
        return Json(new { success = true, message = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.ItemClassification.Sync.Success") });
    }

    #endregion
}