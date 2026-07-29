using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Translation;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ProductAttributeController : BaseAdminController
{
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IProductAttributeModelFactory _productAttributeModelFactory;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly ITranslationModelFactory _translationModelFactory;

    #endregion

    #region Ctor

    public ProductAttributeController(ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IProductAttributeModelFactory productAttributeModelFactory,
        IProductAttributeService productAttributeService,
        ITranslationModelFactory translationModelFactory)
    {
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _productAttributeModelFactory = productAttributeModelFactory;
        _productAttributeService = productAttributeService;
        _translationModelFactory = translationModelFactory;
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(ProductAttribute productAttribute, ProductAttributeModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(productAttribute,
                x => x.Name,
                localized.Name,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(productAttribute,
                x => x.Description,
                localized.Description,
                localized.LanguageId);
        }
    }

    protected virtual async Task UpdateLocalesAsync(PredefinedProductAttributeValue ppav, PredefinedProductAttributeValueModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(ppav,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
    }

    #endregion

    #region Methods

    #region Attribute list / create / edit / delete

    public virtual IActionResult Index()
    {
		logger.LogDebug(">>> Entering Index()");
		logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _productAttributeModelFactory.PrepareProductAttributeSearchModelAsync(new ProductAttributeSearchModel());

		logger.LogDebug("List(): model → {Model}", model);
		logger.LogDebug("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> List(ProductAttributeSearchModel searchModel)
    {
        //prepare model
        var model = await _productAttributeModelFactory.PrepareProductAttributeListModelAsync(searchModel);

		logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
		logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = await _productAttributeModelFactory.PrepareProductAttributeModelAsync(new ProductAttributeModel(), null);

		logger.LogDebug("Create(): model → {Model}", model);
		logger.LogDebug("<<< Exiting Create()");
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(ProductAttributeModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var productAttribute = model.ToEntity<ProductAttribute>();
            await _productAttributeService.InsertProductAttributeAsync(productAttribute);
            await UpdateLocalesAsync(productAttribute, model);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewProductAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewProductAttribute"), productAttribute.Name), productAttribute);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.ProductAttributes.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = productAttribute.Id });
        }

        //prepare model
        model = await _productAttributeModelFactory.PrepareProductAttributeModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get a product attribute with the specified id
        var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(id);
        if (productAttribute == null)
			logger.LogDebug("Edit(id={Id}): productAttribute → {ProductAttribute}", id, productAttribute);
            return RedirectToAction("List");

        //prepare model
        var model = await _productAttributeModelFactory.PrepareProductAttributeModelAsync(null, productAttribute);

		logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);
		logger.LogDebug("<<< Exiting Edit(id={Id})", id);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(ProductAttributeModel model, bool continueEditing)
    {
        //try to get a product attribute with the specified id
        var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(model.Id);
        if (productAttribute == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            productAttribute = model.ToEntity(productAttribute);
            await _productAttributeService.UpdateProductAttributeAsync(productAttribute);

            await UpdateLocalesAsync(productAttribute, model);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditProductAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditProductAttribute"), productAttribute.Name), productAttribute);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.ProductAttributes.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = productAttribute.Id });
        }

        //prepare model
        model = await _productAttributeModelFactory.PrepareProductAttributeModelAsync(model, productAttribute, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PreTranslate(int itemId)
    {
		logger.LogDebug(">>> Entering PreTranslate(itemId={ItemId})", itemId);
        var translationModel = new TranslationModel();

		logger.LogDebug("PreTranslate(itemId={ItemId}): translationModel → {TranslationModel}", itemId, translationModel);
        //try to get a product attribute with the specified id
        var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(itemId);
        if (productAttribute == null)
			logger.LogDebug("PreTranslate(itemId={ItemId}): productAttribute → {ProductAttribute}", itemId, productAttribute);
            return Json(translationModel);

        //prepare model
        var model = await _productAttributeModelFactory.PrepareProductAttributeModelAsync(null, productAttribute);

		logger.LogDebug("PreTranslate(itemId={ItemId}): model → {Model}", itemId, model);
        translationModel = await _translationModelFactory.PrepareTranslationModelAsync(model,
            (nameof(ProductAttributeLocalizedModel.Name), false),
            (nameof(ProductAttributeLocalizedModel.Description), true));

		logger.LogDebug("<<< Exiting PreTranslate(itemId={ItemId})", itemId);
        return Json(translationModel);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a product attribute with the specified id
        var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(id);
        if (productAttribute == null)
			logger.LogDebug("Delete(id={Id}): productAttribute → {ProductAttribute}", id, productAttribute);
            return RedirectToAction("List");

        await _productAttributeService.DeleteProductAttributeAsync(productAttribute);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteProductAttribute",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteProductAttribute"), productAttribute.Name), productAttribute);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.ProductAttributes.Deleted"));

		logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
		logger.LogDebug(">>> Entering DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var productAttributes = await _productAttributeService.GetProductAttributeByIdsAsync(selectedIds.ToArray());
        await _productAttributeService.DeleteProductAttributesAsync(productAttributes);
		logger.LogDebug("DeleteSelected(selectedIds={SelectedIds}): productAttributes → {ProductAttributes}", selectedIds, productAttributes);

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteProductAttribute");
        await _customerActivityService.InsertActivitiesAsync("DeleteProductAttribute", productAttributes, productAttribute => string.Format(activityLogFormat, productAttribute.Name));
		logger.LogDebug("DeleteSelected(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);

		logger.LogDebug("<<< Exiting DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    #endregion

    #region Used by products

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> UsedByProducts(ProductAttributeProductSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering UsedByProducts(searchModel={SearchModel})", searchModel);
        //try to get a product attribute with the specified id
        var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(searchModel.ProductAttributeId)
            ?? throw new ArgumentException("No product attribute found with the specified id");

        //prepare model
        var model = await _productAttributeModelFactory.PrepareProductAttributeProductListModelAsync(searchModel, productAttribute);

		logger.LogDebug("UsedByProducts(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting UsedByProducts(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Predefined values

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> PredefinedProductAttributeValueList(PredefinedProductAttributeValueSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering PredefinedProductAttributeValueList(searchModel={SearchModel})", searchModel);
        //try to get a product attribute with the specified id
        var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(searchModel.ProductAttributeId)
            ?? throw new ArgumentException("No product attribute found with the specified id");

        //prepare model
        var model = await _productAttributeModelFactory.PreparePredefinedProductAttributeValueListModelAsync(searchModel, productAttribute);

		logger.LogDebug("PredefinedProductAttributeValueList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting PredefinedProductAttributeValueList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PredefinedProductAttributeValueCreatePopup(int productAttributeId)
    {
		logger.LogDebug(">>> Entering PredefinedProductAttributeValueCreatePopup(productAttributeId={ProductAttributeId})", productAttributeId);
        //try to get a product attribute with the specified id
        var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(productAttributeId)
            ?? throw new ArgumentException("No product attribute found with the specified id", nameof(productAttributeId));

        //prepare model
        var model = await _productAttributeModelFactory
            .PreparePredefinedProductAttributeValueModelAsync(new PredefinedProductAttributeValueModel(), productAttribute, null);

		logger.LogDebug("<<< Exiting PredefinedProductAttributeValueCreatePopup(productAttributeId={ProductAttributeId})", productAttributeId);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PredefinedProductAttributeValueCreatePopup(PredefinedProductAttributeValueModel model)
    {
        //try to get a product attribute with the specified id
        var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(model.ProductAttributeId)
            ?? throw new ArgumentException("No product attribute found with the specified id");

        if (ModelState.IsValid)
        {
            //fill entity from model
            var ppav = model.ToEntity<PredefinedProductAttributeValue>();

            await _productAttributeService.InsertPredefinedProductAttributeValueAsync(ppav);
            await UpdateLocalesAsync(ppav, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _productAttributeModelFactory.PreparePredefinedProductAttributeValueModelAsync(model, productAttribute, null, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting PredefinedProductAttributeValueCreatePopup(model={Model})", model);
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> PredefinedProductAttributeValueEditPopup(int id)
    {
		logger.LogDebug(">>> Entering PredefinedProductAttributeValueEditPopup(id={Id})", id);
        //try to get a predefined product attribute value with the specified id
        var productAttributeValue = await _productAttributeService.GetPredefinedProductAttributeValueByIdAsync(id)
            ?? throw new ArgumentException("No predefined product attribute value found with the specified id");

        //try to get a product attribute with the specified id
        var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(productAttributeValue.ProductAttributeId)
            ?? throw new ArgumentException("No product attribute found with the specified id");

        //prepare model
        var model = await _productAttributeModelFactory.PreparePredefinedProductAttributeValueModelAsync(null, productAttribute, productAttributeValue);

		logger.LogDebug("PredefinedProductAttributeValueEditPopup(id={Id}): model → {Model}", id, model);
		logger.LogDebug("<<< Exiting PredefinedProductAttributeValueEditPopup(id={Id})", id);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PredefinedProductAttributeValueEditPopup(PredefinedProductAttributeValueModel model)
    {
        //try to get a predefined product attribute value with the specified id
        var productAttributeValue = await _productAttributeService.GetPredefinedProductAttributeValueByIdAsync(model.Id)
            ?? throw new ArgumentException("No predefined product attribute value found with the specified id");

        //try to get a product attribute with the specified id
        var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(productAttributeValue.ProductAttributeId)
            ?? throw new ArgumentException("No product attribute found with the specified id");

        if (ModelState.IsValid)
        {
            productAttributeValue = model.ToEntity(productAttributeValue);
            await _productAttributeService.UpdatePredefinedProductAttributeValueAsync(productAttributeValue);

            await UpdateLocalesAsync(productAttributeValue, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _productAttributeModelFactory.PreparePredefinedProductAttributeValueModelAsync(model, productAttribute, productAttributeValue, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting PredefinedProductAttributeValueEditPopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PredefinedProductAttributeValueDelete(int id)
    {
		logger.LogDebug(">>> Entering PredefinedProductAttributeValueDelete(id={Id})", id);
        //try to get a predefined product attribute value with the specified id
        var productAttributeValue = await _productAttributeService.GetPredefinedProductAttributeValueByIdAsync(id)
            ?? throw new ArgumentException("No predefined product attribute value found with the specified id", nameof(id));

        await _productAttributeService.DeletePredefinedProductAttributeValueAsync(productAttributeValue);

		logger.LogDebug("<<< Exiting PredefinedProductAttributeValueDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion

    #endregion
}