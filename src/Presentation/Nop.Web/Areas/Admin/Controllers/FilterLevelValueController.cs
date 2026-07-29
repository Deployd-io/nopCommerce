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

namespace Nop.Web.Areas.Admin.Controllers;

public partial class FilterLevelValueController : BaseAdminController
{
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
        _customerActivityService = customerActivityService;
        _exportManager = exportManager;
        _filterLevelValueModelFactory = filterLevelValueModelFactory;
        _filterLevelValueService = filterLevelValueService;
        _importManager = importManager;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _productService = productService;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    #region List

    public virtual IActionResult Index()
    {
		logger.LogDebug(">>> Entering Index()");
		logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _filterLevelValueModelFactory.PrepareFilterLevelValueSearchModelAsync(new FilterLevelValueSearchModel());

		logger.LogDebug("List(): model → {Model}", model);
		logger.LogDebug("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_VIEW)]
    public virtual async Task<IActionResult> List(FilterLevelValueSearchModel searchModel)
    {
        //prepare model
        var model = await _filterLevelValueModelFactory.PrepareFilterLevelValueListModelAsync(searchModel);

		logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Create / Edit / Delete

    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual IActionResult Create()
    {
		logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = _filterLevelValueModelFactory.PrepareFilterLevelValueModel(new FilterLevelValueModel(), null);

		logger.LogDebug("Create(): model → {Model}", model);
		logger.LogDebug("<<< Exiting Create()");
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
		logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get a filter level value with the specified id
        var filterLevelValue = await _filterLevelValueService.GetFilterLevelValueByIdAsync(id);
        if (filterLevelValue == null)
			logger.LogDebug("Edit(id={Id}): filterLevelValue → {FilterLevelValue}", id, filterLevelValue);
            return RedirectToAction("List");

        //prepare model
        var model = _filterLevelValueModelFactory.PrepareFilterLevelValueModel(null, filterLevelValue);

		logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);
		logger.LogDebug("<<< Exiting Edit(id={Id})", id);
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
		logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a filter level value with the specified id
        var filterLevelValue = await _filterLevelValueService.GetFilterLevelValueByIdAsync(id);
        if (filterLevelValue == null)
			logger.LogDebug("Delete(id={Id}): filterLevelValue → {FilterLevelValue}", id, filterLevelValue);
            return RedirectToAction("List");

        await _filterLevelValueService.DeleteFilterLevelValueAsync(filterLevelValue);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteFilterLevelValue",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteFilterLevelValue"), filterLevelValue.Id), filterLevelValue);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.FilterLevelValues.Deleted"));

		logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
		logger.LogDebug(">>> Entering DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var filterLevelValues = await _filterLevelValueService.GetFilterLevelValuesByIdsAsync(selectedIds.ToArray());

		logger.LogDebug("DeleteSelected(selectedIds={SelectedIds}): filterLevelValues → {FilterLevelValues}", selectedIds, filterLevelValues);
        await _filterLevelValueService.DeleteFilterLevelValuesAsync(filterLevelValues);

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteFilterLevelValue");
        await _customerActivityService.InsertActivitiesAsync("DeleteFilterLevelValue", filterLevelValues, filterLevelValue => string.Format(activityLogFormat, filterLevelValue.Id));
		logger.LogDebug("DeleteSelected(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);

		logger.LogDebug("<<< Exiting DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    #endregion

    #region Export / Import

    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXlsx()
    {
		logger.LogDebug(">>> Entering ExportXlsx()");
        try
        {
            var filterLevelValues = (await _filterLevelValueService.GetAllFilterLevelValuesAsync()).ToList();
            var bytes = await _exportManager.ExportFilterLevelValuesToXlsxAsync(filterLevelValues);
			logger.LogDebug("ExportXlsx(): filterLevelValues → {FilterLevelValues}", filterLevelValues);
			logger.LogDebug("ExportXlsx(): bytes → {Bytes}", bytes);

            return File(bytes, MimeTypes.TextXlsx, "FilterLevelValues.xlsx");
        }
        catch (Exception exc)
        {
			logger.LogError("Exception in ExportXlsx(): {Arg0}", ex.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			logger.LogDebug("<<< Exiting ExportXlsx()");
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ImportFromXlsx(IFormFile importexcelfile)
    {
		logger.LogDebug(">>> Entering ImportFromXlsx(importexcelfile={Importexcelfile})", importexcelfile);
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
			logger.LogError("Exception in ImportFromXlsx(importexcelfile={Importexcelfile}): {Arg1}", importexcelfile, ex.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			logger.LogDebug("<<< Exiting ImportFromXlsx(importexcelfile={Importexcelfile})", importexcelfile);
            return RedirectToAction("List");
        }
    }

    #endregion

    #region Products

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductList(FilterLevelValueProductSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ProductList(searchModel={SearchModel})", searchModel);
        //try to get a filter level value with the specified id
        var filterLevelValue = await _filterLevelValueService.GetFilterLevelValueByIdAsync(searchModel.FilterLevelValueId)
            ?? throw new ArgumentException("No filter level value found with the specified id");

        //prepare model
        var model = await _filterLevelValueModelFactory.PrepareFilterLevelValueProductListModelAsync(searchModel, filterLevelValue);

		logger.LogDebug("ProductList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ProductList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductDelete(int id)
    {
		logger.LogDebug(">>> Entering ProductDelete(id={Id})", id);
        //try to get a filter level value product mapping with the specified id
        var filterLevelValueProduct = await _filterLevelValueService.GetFilterLevelValueProductByIdAsync(id)
            ?? throw new ArgumentException("No filter level value product mapping found with the specified id", nameof(id));

        await _filterLevelValueService.DeleteFilterLevelValueProductAsync(filterLevelValueProduct);

		logger.LogDebug("<<< Exiting ProductDelete(id={Id})", id);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopup(int filterLevelValueId)
    {
		logger.LogDebug(">>> Entering ProductAddPopup(filterLevelValueId={FilterLevelValueId})", filterLevelValueId);
        //prepare model
        var model = await _filterLevelValueModelFactory.PrepareAddProductToFilterLevelValueSearchModelAsync(new AddProductToFilterLevelValueSearchModel());

		logger.LogDebug("ProductAddPopup(filterLevelValueId={FilterLevelValueId}): model → {Model}", filterLevelValueId, model);
		logger.LogDebug("<<< Exiting ProductAddPopup(filterLevelValueId={FilterLevelValueId})", filterLevelValueId);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopupList(AddProductToFilterLevelValueSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ProductAddPopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _filterLevelValueModelFactory.PrepareAddProductToFilterLevelValueListModelAsync(searchModel);

		logger.LogDebug("ProductAddPopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ProductAddPopupList(searchModel={SearchModel})", searchModel);
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
