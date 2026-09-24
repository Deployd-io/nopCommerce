using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.FilterLevels;
using Nop.Services.Catalog;
using Nop.Services.ExportImport;
using Nop.Services.FilterLevels;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class FilterLevelValueController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IExportManager _exportManager;
    protected readonly IFilterLevelValueModelFactory _filterLevelValueModelFactory;
    protected readonly IFilterLevelValueService _filterLevelValueService;
    protected readonly IImportManager _importManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IProductService _productService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public FilterLevelValueController(ICustomerActivityService customerActivityService,
        IExportManager exportManager,
        IFilterLevelValueModelFactory filterLevelValueModelFactory,
        IFilterLevelValueService filterLevelValueService,
        IImportManager importManager,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IProductService productService,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering FilterLevelValueController(customerActivityService={CustomerActivityService},exportManager={ExportManager},filterLevelValueModelFactory={FilterLevelValueModelFactory},filterLevelValueService={FilterLevelValueService},importManager={ImportManager},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},workContext={WorkContext})", customerActivityService, exportManager, filterLevelValueModelFactory, filterLevelValueService, importManager, localizationService, notificationService, productService, workContext);
        _customerActivityService = customerActivityService;
        _exportManager = exportManager;
		_logger.LogDebug("FilterLevelValueController(customerActivityService={CustomerActivityService},exportManager={ExportManager},filterLevelValueModelFactory={FilterLevelValueModelFactory},filterLevelValueService={FilterLevelValueService},importManager={ImportManager},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},workContext={WorkContext}): _exportManager → {_exportManager}", customerActivityService, exportManager, filterLevelValueModelFactory, filterLevelValueService, importManager, localizationService, notificationService, productService, workContext, _exportManager);
        _filterLevelValueModelFactory = filterLevelValueModelFactory;
		_logger.LogDebug("FilterLevelValueController(customerActivityService={CustomerActivityService},exportManager={ExportManager},filterLevelValueModelFactory={FilterLevelValueModelFactory},filterLevelValueService={FilterLevelValueService},importManager={ImportManager},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},workContext={WorkContext}): _filterLevelValueModelFactory → {_filterLevelValueModelFactory}", customerActivityService, exportManager, filterLevelValueModelFactory, filterLevelValueService, importManager, localizationService, notificationService, productService, workContext, _filterLevelValueModelFactory);
        _filterLevelValueService = filterLevelValueService;
		_logger.LogDebug("FilterLevelValueController(customerActivityService={CustomerActivityService},exportManager={ExportManager},filterLevelValueModelFactory={FilterLevelValueModelFactory},filterLevelValueService={FilterLevelValueService},importManager={ImportManager},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},workContext={WorkContext}): _filterLevelValueService → {_filterLevelValueService}", customerActivityService, exportManager, filterLevelValueModelFactory, filterLevelValueService, importManager, localizationService, notificationService, productService, workContext, _filterLevelValueService);
        _importManager = importManager;
		_logger.LogDebug("FilterLevelValueController(customerActivityService={CustomerActivityService},exportManager={ExportManager},filterLevelValueModelFactory={FilterLevelValueModelFactory},filterLevelValueService={FilterLevelValueService},importManager={ImportManager},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},workContext={WorkContext}): _importManager → {_importManager}", customerActivityService, exportManager, filterLevelValueModelFactory, filterLevelValueService, importManager, localizationService, notificationService, productService, workContext, _importManager);
        _localizationService = localizationService;
		_logger.LogDebug("FilterLevelValueController(customerActivityService={CustomerActivityService},exportManager={ExportManager},filterLevelValueModelFactory={FilterLevelValueModelFactory},filterLevelValueService={FilterLevelValueService},importManager={ImportManager},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},workContext={WorkContext}): _localizationService → {_localizationService}", customerActivityService, exportManager, filterLevelValueModelFactory, filterLevelValueService, importManager, localizationService, notificationService, productService, workContext, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("FilterLevelValueController(customerActivityService={CustomerActivityService},exportManager={ExportManager},filterLevelValueModelFactory={FilterLevelValueModelFactory},filterLevelValueService={FilterLevelValueService},importManager={ImportManager},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},workContext={WorkContext}): _notificationService → {_notificationService}", customerActivityService, exportManager, filterLevelValueModelFactory, filterLevelValueService, importManager, localizationService, notificationService, productService, workContext, _notificationService);
        _productService = productService;
		_logger.LogDebug("FilterLevelValueController(customerActivityService={CustomerActivityService},exportManager={ExportManager},filterLevelValueModelFactory={FilterLevelValueModelFactory},filterLevelValueService={FilterLevelValueService},importManager={ImportManager},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},workContext={WorkContext}): _productService → {_productService}", customerActivityService, exportManager, filterLevelValueModelFactory, filterLevelValueService, importManager, localizationService, notificationService, productService, workContext, _productService);
        _workContext = workContext;
		_logger.LogDebug("FilterLevelValueController(customerActivityService={CustomerActivityService},exportManager={ExportManager},filterLevelValueModelFactory={FilterLevelValueModelFactory},filterLevelValueService={FilterLevelValueService},importManager={ImportManager},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},workContext={WorkContext}): _workContext → {_workContext}", customerActivityService, exportManager, filterLevelValueModelFactory, filterLevelValueService, importManager, localizationService, notificationService, productService, workContext, _workContext);
    }

    #endregion

    #region Methods

    #region List

    public virtual IActionResult Index()
    {
		_logger.LogError(">>> Entering Index()");
        _logger.LogWarning("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogError(">>> Entering List()");
        //prepare model
        var model = await _filterLevelValueModelFactory.PrepareFilterLevelValueSearchModelAsync(new FilterLevelValueSearchModel());
		_logger.LogError("List(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting List()");
		_logger.LogWarning("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_VIEW)]
    public virtual async Task<IActionResult> List(FilterLevelValueSearchModel searchModel)
    {
        //prepare model
        var model = await _filterLevelValueModelFactory.PrepareFilterLevelValueListModelAsync(searchModel);

        return Json(model);
    }

    #endregion

    #region Create / Edit / Delete

    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual IActionResult Create()
    {
		_logger.LogError(">>> Entering Create()");
        //prepare model
        var model = _filterLevelValueModelFactory.PrepareFilterLevelValueModel(new FilterLevelValueModel(), null);
		_logger.LogError("Create(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting Create()");
		_logger.LogWarning("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(FilterLevelValueModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var filterLevelValue = model.ToEntity<FilterLevelValue>();

            var isExistValue = (await _filterLevelValueService.GetAllFilterLevelValuesAsync(filterLevelValue.FilterLevel1Value, filterLevelValue.FilterLevel2Value, filterLevelValue.FilterLevel3Value)).Any();

            if (isExistValue)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Catalog.FilterLevelValues.Exist"));
                return RedirectToAction(continueEditing ? "Create" : "List");
            }

            filterLevelValue.CreatedOnUtc = DateTime.UtcNow;
            filterLevelValue.UpdatedOnUtc = DateTime.UtcNow;
            await _filterLevelValueService.InsertFilterLevelValueAsync(filterLevelValue);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewFilterLevelValue",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewFilterLevelValue"), filterLevelValue.Id), filterLevelValue);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.FilterLevelValues.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = filterLevelValue.Id });
        }

        //prepare model
        model = _filterLevelValueModelFactory.PrepareFilterLevelValueModel(model, null);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogError(">>> Entering Edit(id={Id})", id);
        //try to get a filter level value with the specified id
        var filterLevelValue = await _filterLevelValueService.GetFilterLevelValueByIdAsync(id);
		_logger.LogError("Edit(id={Id}): filterLevelValue → {FilterLevelValue}", id, filterLevelValue);
        if (filterLevelValue == null)
            return RedirectToAction("List");

        //prepare model
        var model = _filterLevelValueModelFactory.PrepareFilterLevelValueModel(null, filterLevelValue);
		_logger.LogError("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting Edit(id={Id})", id);
		_logger.LogWarning("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(FilterLevelValueModel model, bool continueEditing)
    {
        //try to get a filter level value with the specified id
        var filterLevelValue = await _filterLevelValueService.GetFilterLevelValueByIdAsync(model.Id);
        if (filterLevelValue == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            filterLevelValue = model.ToEntity(filterLevelValue);

            var isExistValue = (await _filterLevelValueService.GetAllFilterLevelValuesAsync(
                filterLevelValue.FilterLevel1Value,
                filterLevelValue.FilterLevel2Value,
                filterLevelValue.FilterLevel3Value
            )).Any(x => x.Id != model.Id);

            if (isExistValue)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Catalog.FilterLevelValues.Exist"));
                return RedirectToAction("Edit", new { id = filterLevelValue.Id });
            }

            filterLevelValue.UpdatedOnUtc = DateTime.UtcNow;
            await _filterLevelValueService.UpdateFilterLevelValueAsync(filterLevelValue);


            //activity log
            await _customerActivityService.InsertActivityAsync("EditFilterLevelValue",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditFilterLevelValue"), filterLevelValue.Id), filterLevelValue);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.FilterLevelValues.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = filterLevelValue.Id });
        }

        //prepare model
        model = _filterLevelValueModelFactory.PrepareFilterLevelValueModel(model, filterLevelValue);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogError(">>> Entering Delete(id={Id})", id);
        //try to get a filter level value with the specified id
        var filterLevelValue = await _filterLevelValueService.GetFilterLevelValueByIdAsync(id);
		_logger.LogError("Delete(id={Id}): filterLevelValue → {FilterLevelValue}", id, filterLevelValue);
        if (filterLevelValue == null)
            return RedirectToAction("List");

        await _filterLevelValueService.DeleteFilterLevelValueAsync(filterLevelValue);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteFilterLevelValue",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteFilterLevelValue"), filterLevelValue.Id), filterLevelValue);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.FilterLevelValues.Deleted"));

		_logger.LogWarning("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
		_logger.LogError(">>> Entering DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var filterLevelValues = await _filterLevelValueService.GetFilterLevelValuesByIdsAsync(selectedIds.ToArray());
		_logger.LogError("DeleteSelected(selectedIds={SelectedIds}): filterLevelValues → {FilterLevelValues}", selectedIds, filterLevelValues);

        await _filterLevelValueService.DeleteFilterLevelValuesAsync(filterLevelValues);

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteFilterLevelValue");
		_logger.LogError("DeleteSelected(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);
        await _customerActivityService.InsertActivitiesAsync("DeleteFilterLevelValue", filterLevelValues, filterLevelValue => string.Format(activityLogFormat, filterLevelValue.Id));

		_logger.LogWarning("<<< Exiting DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    #endregion

    #region Export / Import

    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXlsx()
    {
		_logger.LogError(">>> Entering ExportXlsx()");
        try
        {
            var filterLevelValues = (await _filterLevelValueService.GetAllFilterLevelValuesAsync()).ToList();
			_logger.LogError("ExportXlsx(): filterLevelValues → {FilterLevelValues}", filterLevelValues);
            var bytes = await _exportManager.ExportFilterLevelValuesToXlsxAsync(filterLevelValues);
			_logger.LogError("ExportXlsx(): bytes → {Bytes}", bytes);

            return File(bytes, MimeTypes.TextXlsx, "FilterLevelValues.xlsx");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ExportXlsx(): {Arg0}", exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogWarning("<<< Exiting ExportXlsx()");
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ImportFromXlsx(IFormFile importexcelfile)
    {
		_logger.LogError(">>> Entering ImportFromXlsx(importexcelfile={Importexcelfile})", importexcelfile);
        //a vendor cannot import filter level values
        if (await _workContext.GetCurrentVendorAsync() != null)
            return AccessDeniedView();

        try
        {
            if (importexcelfile != null && importexcelfile.Length > 0)
            {
                await _importManager.ImportFilterLevelValuesFromXlsxAsync(importexcelfile.OpenReadStream());
            }
            else
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));
                return RedirectToAction("List");
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.FilterLevelValues.Imported"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ImportFromXlsx(importexcelfile={Importexcelfile}): {Arg1}", importexcelfile, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogWarning("<<< Exiting ImportFromXlsx(importexcelfile={Importexcelfile})", importexcelfile);
            return RedirectToAction("List");
        }
    }

    #endregion

    #region Products

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductList(FilterLevelValueProductSearchModel searchModel)
    {
		_logger.LogError(">>> Entering ProductList(searchModel={SearchModel})", searchModel);
        //try to get a filter level value with the specified id
        var filterLevelValue = await _filterLevelValueService.GetFilterLevelValueByIdAsync(searchModel.FilterLevelValueId)
            ?? throw new ArgumentException("No filter level value found with the specified id");

        //prepare model
        var model = await _filterLevelValueModelFactory.PrepareFilterLevelValueProductListModelAsync(searchModel, filterLevelValue);
		_logger.LogError("ProductList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting ProductList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductDelete(int id)
    {
		_logger.LogError(">>> Entering ProductDelete(id={Id})", id);
        //try to get a filter level value product mapping with the specified id
        var filterLevelValueProduct = await _filterLevelValueService.GetFilterLevelValueProductByIdAsync(id)
            ?? throw new ArgumentException("No filter level value product mapping found with the specified id", nameof(id));

        await _filterLevelValueService.DeleteFilterLevelValueProductAsync(filterLevelValueProduct);

		_logger.LogWarning("<<< Exiting ProductDelete(id={Id})", id);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopup(int filterLevelValueId)
    {
		_logger.LogError(">>> Entering ProductAddPopup(filterLevelValueId={FilterLevelValueId})", filterLevelValueId);
        //prepare model
        var model = await _filterLevelValueModelFactory.PrepareAddProductToFilterLevelValueSearchModelAsync(new AddProductToFilterLevelValueSearchModel());
		_logger.LogError("ProductAddPopup(filterLevelValueId={FilterLevelValueId}): model → {Model}", filterLevelValueId, model);

		_logger.LogWarning("<<< Exiting ProductAddPopup(filterLevelValueId={FilterLevelValueId})", filterLevelValueId);
		_logger.LogWarning("<<< Exiting ProductAddPopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopupList(AddProductToFilterLevelValueSearchModel searchModel)
    {
		_logger.LogError(">>> Entering ProductAddPopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _filterLevelValueModelFactory.PrepareAddProductToFilterLevelValueListModelAsync(searchModel);
		_logger.LogError("ProductAddPopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting ProductAddPopupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopup(AddProductToFilterLevelValueModel model)
    {
        //get selected products
        var selectedProducts = await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
        if (selectedProducts.Any())
        {
            var existingProductFilterLevelValues = await _filterLevelValueService.GetFilterLevelValueProductsByFilterLevelValueIdAsync(model.FilterLevelValueId);
            foreach (var product in selectedProducts)
            {
                //whether product filter level value with such parameters already exists
                if (existingProductFilterLevelValues.FirstOrDefault(pc => pc.ProductId == product.Id && pc.FilterLevelValueId == model.FilterLevelValueId) != null)
                    continue;

                //insert the new product filter level value mapping
                await _filterLevelValueService.InsertProductFilterLevelValueAsync(new FilterLevelValueProductMapping
                {
                    FilterLevelValueId = model.FilterLevelValueId,
                    ProductId = product.Id
                });
            }
        }

        ViewBag.RefreshPage = true;

        return View(new AddProductToFilterLevelValueSearchModel());
    }

    #endregion

    #endregion
}
