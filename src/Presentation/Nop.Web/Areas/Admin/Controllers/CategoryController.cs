using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Translation;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class CategoryController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICategoryModelFactory _categoryModelFactory;
    protected readonly ICategoryService _categoryService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IDiscountService _discountService;
    protected readonly IExportManager _exportManager;
    protected readonly IImportManager _importManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IPictureService _pictureService;
    protected readonly IProductService _productService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly ITranslationModelFactory _translationModelFactory;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public CategoryController(ICategoryModelFactory categoryModelFactory,
        ICategoryService categoryService,
        ICustomerActivityService customerActivityService,
        IDiscountService discountService,
        IExportManager exportManager,
        IImportManager importManager,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IPictureService pictureService,
        IProductService productService,
        IStaticCacheManager staticCacheManager,
        IStoreMappingService storeMappingService,
        ITranslationModelFactory translationModelFactory,
        IUrlRecordService urlRecordService,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering CategoryController(categoryModelFactory={CategoryModelFactory},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext})", categoryModelFactory, categoryService, customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, notificationService, pictureService, productService, staticCacheManager, translationModelFactory, urlRecordService, workContext);
        _categoryModelFactory = categoryModelFactory;
        _categoryService = categoryService;
		_logger.LogDebug("CategoryController(categoryModelFactory={CategoryModelFactory},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _categoryService → {_categoryService}", categoryModelFactory, categoryService, customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, notificationService, pictureService, productService, staticCacheManager, translationModelFactory, urlRecordService, workContext, _categoryService);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("CategoryController(categoryModelFactory={CategoryModelFactory},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _customerActivityService → {_customerActivityService}", categoryModelFactory, categoryService, customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, notificationService, pictureService, productService, staticCacheManager, translationModelFactory, urlRecordService, workContext, _customerActivityService);
        _discountService = discountService;
		_logger.LogDebug("CategoryController(categoryModelFactory={CategoryModelFactory},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _discountService → {_discountService}", categoryModelFactory, categoryService, customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, notificationService, pictureService, productService, staticCacheManager, translationModelFactory, urlRecordService, workContext, _discountService);
        _exportManager = exportManager;
		_logger.LogDebug("CategoryController(categoryModelFactory={CategoryModelFactory},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _exportManager → {_exportManager}", categoryModelFactory, categoryService, customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, notificationService, pictureService, productService, staticCacheManager, translationModelFactory, urlRecordService, workContext, _exportManager);
        _importManager = importManager;
		_logger.LogDebug("CategoryController(categoryModelFactory={CategoryModelFactory},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _importManager → {_importManager}", categoryModelFactory, categoryService, customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, notificationService, pictureService, productService, staticCacheManager, translationModelFactory, urlRecordService, workContext, _importManager);
        _localizationService = localizationService;
		_logger.LogDebug("CategoryController(categoryModelFactory={CategoryModelFactory},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _localizationService → {_localizationService}", categoryModelFactory, categoryService, customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, notificationService, pictureService, productService, staticCacheManager, translationModelFactory, urlRecordService, workContext, _localizationService);
        _localizedEntityService = localizedEntityService;
		_logger.LogDebug("CategoryController(categoryModelFactory={CategoryModelFactory},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _localizedEntityService → {_localizedEntityService}", categoryModelFactory, categoryService, customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, notificationService, pictureService, productService, staticCacheManager, translationModelFactory, urlRecordService, workContext, _localizedEntityService);
        _notificationService = notificationService;
		_logger.LogDebug("CategoryController(categoryModelFactory={CategoryModelFactory},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _notificationService → {_notificationService}", categoryModelFactory, categoryService, customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, notificationService, pictureService, productService, staticCacheManager, translationModelFactory, urlRecordService, workContext, _notificationService);
        _pictureService = pictureService;
		_logger.LogDebug("CategoryController(categoryModelFactory={CategoryModelFactory},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _pictureService → {_pictureService}", categoryModelFactory, categoryService, customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, notificationService, pictureService, productService, staticCacheManager, translationModelFactory, urlRecordService, workContext, _pictureService);
        _productService = productService;
		_logger.LogDebug("CategoryController(categoryModelFactory={CategoryModelFactory},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _productService → {_productService}", categoryModelFactory, categoryService, customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, notificationService, pictureService, productService, staticCacheManager, translationModelFactory, urlRecordService, workContext, _productService);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("CategoryController(categoryModelFactory={CategoryModelFactory},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _staticCacheManager → {_staticCacheManager}", categoryModelFactory, categoryService, customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, notificationService, pictureService, productService, staticCacheManager, translationModelFactory, urlRecordService, workContext, _staticCacheManager);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("CategoryController(categoryModelFactory={CategoryModelFactory},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _storeMappingService → {_storeMappingService}", categoryModelFactory, categoryService, customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, notificationService, pictureService, productService, staticCacheManager, translationModelFactory, urlRecordService, workContext, _storeMappingService);
        _translationModelFactory = translationModelFactory;
		_logger.LogDebug("CategoryController(categoryModelFactory={CategoryModelFactory},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _translationModelFactory → {_translationModelFactory}", categoryModelFactory, categoryService, customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, notificationService, pictureService, productService, staticCacheManager, translationModelFactory, urlRecordService, workContext, _translationModelFactory);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("CategoryController(categoryModelFactory={CategoryModelFactory},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _urlRecordService → {_urlRecordService}", categoryModelFactory, categoryService, customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, notificationService, pictureService, productService, staticCacheManager, translationModelFactory, urlRecordService, workContext, _urlRecordService);
        _workContext = workContext;
		_logger.LogDebug("CategoryController(categoryModelFactory={CategoryModelFactory},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _workContext → {_workContext}", categoryModelFactory, categoryService, customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, notificationService, pictureService, productService, staticCacheManager, translationModelFactory, urlRecordService, workContext, _workContext);
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(Category category, CategoryModel model)
    {
		_logger.LogError(">>> Entering UpdateLocalesAsync(category={Category},model={Model})", category, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(category,
                x => x.Name,
                localized.Name,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(category,
                x => x.Description,
                localized.Description,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(category,
                x => x.MetaKeywords,
                localized.MetaKeywords,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(category,
                x => x.MetaDescription,
                localized.MetaDescription,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(category,
                x => x.MetaTitle,
                localized.MetaTitle,
                localized.LanguageId);

            //search engine name
            var seName = await _urlRecordService.ValidateSeNameAsync(category, localized.SeName, localized.Name, false);
			_logger.LogError("UpdateLocalesAsync(category={Category},model={Model}): seName → {SeName}", category, model, seName);
            await _urlRecordService.SaveSlugAsync(category, seName, localized.LanguageId);
        }
		_logger.LogWarning("<<< Exiting UpdateLocalesAsync(category={Category},model={Model})", category, model);
    }

    protected virtual async Task UpdatePictureSeoNamesAsync(Category category)
    {
		_logger.LogError(">>> Entering UpdatePictureSeoNamesAsync(category={Category})", category);
        var picture = await _pictureService.GetPictureByIdAsync(category.PictureId);
			_logger.LogError("UpdatePictureSeoNamesAsync(category={Category}): picture → {Picture}", category, picture);
        if (picture != null) {
            await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(category.Name));
        }
			_logger.LogWarning("<<< Exiting UpdatePictureSeoNamesAsync(category={Category})", category);
    }

    #endregion

    #region List

    public virtual IActionResult Index()
    {
		_logger.LogError(">>> Entering Index()");
        _logger.LogWarning("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Catalog.CATEGORIES_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogError(">>> Entering List()");
        //prepare model
        var model = await _categoryModelFactory.PrepareCategorySearchModelAsync(new CategorySearchModel());
		_logger.LogError("List(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting List()");
		_logger.LogWarning("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_VIEW)]
    public virtual async Task<IActionResult> List(CategorySearchModel searchModel)
    {
        //prepare model
        var model = await _categoryModelFactory.PrepareCategoryListModelAsync(searchModel);

        return Json(model);
    }

    #endregion

    #region Create / Edit / Delete

    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogError(">>> Entering Create()");
        //prepare model
        var model = await _categoryModelFactory.PrepareCategoryModelAsync(new CategoryModel(), null);
		_logger.LogError("Create(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting Create()");
		_logger.LogWarning("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(CategoryModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var category = model.ToEntity<Category>();
            category.CreatedOnUtc = DateTime.UtcNow;
            category.UpdatedOnUtc = DateTime.UtcNow;
            await _categoryService.InsertCategoryAsync(category);

            //search engine name
            model.SeName = await _urlRecordService.ValidateSeNameAsync(category, model.SeName, category.Name, true);
            await _urlRecordService.SaveSlugAsync(category, model.SeName, 0);

            //locales
            await UpdateLocalesAsync(category, model);

            //discounts
            var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true, isActive: null);
            foreach (var discount in allDiscounts)
            {
                if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    await _categoryService.InsertDiscountCategoryMappingAsync(new DiscountCategoryMapping { DiscountId = discount.Id, EntityId = category.Id });
            }

            await _categoryService.UpdateCategoryAsync(category);

            //update picture seo file name
            await UpdatePictureSeoNamesAsync(category);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(category, model.SelectedStoreIds);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewCategory",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCategory"), category.Name), category);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = category.Id });
        }

        //prepare model
        model = await _categoryModelFactory.PrepareCategoryModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.CATEGORIES_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogError(">>> Entering Edit(id={Id})", id);
        //try to get a category with the specified id
        var category = await _categoryService.GetCategoryByIdAsync(id);
		_logger.LogError("Edit(id={Id}): category → {Category}", id, category);
        if (category == null || category.Deleted)
            return RedirectToAction("List");

        //prepare model
        var model = await _categoryModelFactory.PrepareCategoryModelAsync(null, category);
		_logger.LogError("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting Edit(id={Id})", id);
		_logger.LogWarning("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(CategoryModel model, bool continueEditing)
    {
        //try to get a category with the specified id
        var category = await _categoryService.GetCategoryByIdAsync(model.Id);
        if (category == null || category.Deleted)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            var prevPictureId = category.PictureId;

            //if parent category changes, we need to clear cache for previous parent category
            if (category.ParentCategoryId != model.ParentCategoryId)
            {
                await _staticCacheManager.RemoveByPrefixAsync(NopCatalogDefaults.CategoriesByParentCategoryPrefix, category.ParentCategoryId);
                await _staticCacheManager.RemoveByPrefixAsync(NopCatalogDefaults.CategoriesChildIdsPrefix, category.ParentCategoryId);
                await _staticCacheManager.RemoveByPrefixAsync(NopCatalogDefaults.ChildCategoryIdLookupPrefix);
            }

            category = model.ToEntity(category);
            category.UpdatedOnUtc = DateTime.UtcNow;
            await _categoryService.UpdateCategoryAsync(category);

            //search engine name
            model.SeName = await _urlRecordService.ValidateSeNameAsync(category, model.SeName, category.Name, true);
            await _urlRecordService.SaveSlugAsync(category, model.SeName, 0);

            //locales
            await UpdateLocalesAsync(category, model);

            //discounts
            var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true, isActive: null);
            foreach (var discount in allDiscounts)
            {
                if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                {
                    //new discount
                    if (await _categoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is null)
                        await _categoryService.InsertDiscountCategoryMappingAsync(new DiscountCategoryMapping { DiscountId = discount.Id, EntityId = category.Id });
                }
                else
                {
                    //remove discount
                    if (await _categoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is DiscountCategoryMapping mapping)
                        await _categoryService.DeleteDiscountCategoryMappingAsync(mapping);
                }
            }

            await _categoryService.UpdateCategoryAsync(category);

            //delete an old picture (if deleted or updated)
            if (prevPictureId > 0 && prevPictureId != category.PictureId)
            {
                var prevPicture = await _pictureService.GetPictureByIdAsync(prevPictureId);
                if (prevPicture != null)
                    await _pictureService.DeletePictureAsync(prevPicture);
            }

            //update picture seo file name
            await UpdatePictureSeoNamesAsync(category);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(category, model.SelectedStoreIds);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditCategory",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCategory"), category.Name), category);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = category.Id });
        }

        //prepare model
        model = await _categoryModelFactory.PrepareCategoryModelAsync(model, category, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PreTranslate(int itemId)
    {
		_logger.LogError(">>> Entering PreTranslate(itemId={ItemId})", itemId);
        var translationModel = new TranslationModel();

		_logger.LogError("PreTranslate(itemId={ItemId}): translationModel → {TranslationModel}", itemId, translationModel);
        //try to get a category with the specified id
        var category = await _categoryService.GetCategoryByIdAsync(itemId);
		_logger.LogError("PreTranslate(itemId={ItemId}): category → {Category}", itemId, category);
        if (category == null || category.Deleted)
            return Json(translationModel);

        //prepare model
        var model = await _categoryModelFactory.PrepareCategoryModelAsync(null, category);
		_logger.LogError("PreTranslate(itemId={ItemId}): model → {Model}", itemId, model);

        translationModel = await _translationModelFactory.PrepareTranslationModelAsync(model,
            (nameof(CategoryLocalizedModel.Name), false),
            (nameof(CategoryLocalizedModel.Description), true));

		_logger.LogWarning("<<< Exiting PreTranslate(itemId={ItemId})", itemId);
        return Json(translationModel);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogError(">>> Entering Delete(id={Id})", id);
        //try to get a category with the specified id
        var category = await _categoryService.GetCategoryByIdAsync(id);
		_logger.LogError("Delete(id={Id}): category → {Category}", id, category);
        if (category == null)
            return RedirectToAction("List");

        await _categoryService.DeleteCategoryAsync(category);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteCategory",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCategory"), category.Name), category);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Deleted"));

		_logger.LogWarning("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
		_logger.LogError(">>> Entering DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var categories = await _categoryService.GetCategoriesByIdsAsync(selectedIds.ToArray());
		_logger.LogError("DeleteSelected(selectedIds={SelectedIds}): categories → {Categories}", selectedIds, categories);

        await _categoryService.DeleteCategoriesAsync(categories);

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteCategory");
		_logger.LogError("DeleteSelected(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);
        await _customerActivityService.InsertActivitiesAsync("DeleteCategory", categories, category => string.Format(activityLogFormat, category.Name));

		_logger.LogWarning("<<< Exiting DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    #endregion

    #region Export / Import

    [CheckPermission(StandardPermission.Catalog.CATEGORIES_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXml()
    {
		_logger.LogError(">>> Entering ExportXml()");
        try
        {
            var xml = await _exportManager.ExportCategoriesToXmlAsync();
			_logger.LogError("ExportXml(): xml → {Xml}", xml);

            return File(Encoding.UTF8.GetBytes(xml), "application/xml", "categories.xml");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ExportXml(): {Arg0}", exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogWarning("<<< Exiting ExportXml()");
            return RedirectToAction("List");
        }
    }

    [CheckPermission(StandardPermission.Catalog.CATEGORIES_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXlsx()
    {
		_logger.LogError(">>> Entering ExportXlsx()");
        try
        {
            var bytes = await _exportManager
                .ExportCategoriesToXlsxAsync((await _categoryService.GetAllCategoriesAsync(showHidden: true)).ToList());

            return File(bytes, MimeTypes.TextXlsx, "categories.xlsx");
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
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ImportFromXlsx(IFormFile importexcelfile)
    {
		_logger.LogError(">>> Entering ImportFromXlsx(importexcelfile={Importexcelfile})", importexcelfile);
        //a vendor cannot import categories
        if (await _workContext.GetCurrentVendorAsync() != null)
            return AccessDeniedView();

        try
        {
            if (importexcelfile != null && importexcelfile.Length > 0)
            {
                await _importManager.ImportCategoriesFromXlsxAsync(importexcelfile.OpenReadStream());
            }
            else
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));
                return RedirectToAction("List");
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Imported"));

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
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_VIEW)]
    public virtual async Task<IActionResult> ProductList(CategoryProductSearchModel searchModel)
    {
		_logger.LogError(">>> Entering ProductList(searchModel={SearchModel})", searchModel);
        //try to get a category with the specified id
        var category = await _categoryService.GetCategoryByIdAsync(searchModel.CategoryId)
            ?? throw new ArgumentException("No category found with the specified id");

        //prepare model
        var model = await _categoryModelFactory.PrepareCategoryProductListModelAsync(searchModel, category);
		_logger.LogError("ProductList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting ProductList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductUpdate(CategoryProductModel model)
    {
		_logger.LogError(">>> Entering ProductUpdate(model={Model})", model);
        //try to get a product category with the specified id
        var productCategory = await _categoryService.GetProductCategoryByIdAsync(model.Id)
            ?? throw new ArgumentException("No product category mapping found with the specified id");

        //fill entity from product
        productCategory = model.ToEntity(productCategory);
		_logger.LogError("ProductUpdate(model={Model}): productCategory → {ProductCategory}", model, productCategory);
        await _categoryService.UpdateProductCategoryAsync(productCategory);

		_logger.LogWarning("<<< Exiting ProductUpdate(model={Model})", model);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductDelete(int id)
    {
		_logger.LogError(">>> Entering ProductDelete(id={Id})", id);
        //try to get a product category with the specified id
        var productCategory = await _categoryService.GetProductCategoryByIdAsync(id)
            ?? throw new ArgumentException("No product category mapping found with the specified id", nameof(id));

        await _categoryService.DeleteProductCategoryAsync(productCategory);

		_logger.LogWarning("<<< Exiting ProductDelete(id={Id})", id);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopup(int categoryId)
    {
		_logger.LogError(">>> Entering ProductAddPopup(categoryId={CategoryId})", categoryId);
        //prepare model
        var model = await _categoryModelFactory.PrepareAddProductToCategorySearchModelAsync(new AddProductToCategorySearchModel());
		_logger.LogError("ProductAddPopup(categoryId={CategoryId}): model → {Model}", categoryId, model);

		_logger.LogWarning("<<< Exiting ProductAddPopup(categoryId={CategoryId})", categoryId);
		_logger.LogWarning("<<< Exiting ProductAddPopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopupList(AddProductToCategorySearchModel searchModel)
    {
		_logger.LogError(">>> Entering ProductAddPopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _categoryModelFactory.PrepareAddProductToCategoryListModelAsync(searchModel);
		_logger.LogError("ProductAddPopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting ProductAddPopupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopup(AddProductToCategoryModel model)
    {
        //get selected products
        var selectedProducts = await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
        if (selectedProducts.Any())
        {
            var existingProductCategories = await _categoryService.GetProductCategoriesByCategoryIdAsync(model.CategoryId, showHidden: true);
            foreach (var product in selectedProducts)
            {
                //whether product category with such parameters already exists
                if (_categoryService.FindProductCategory(existingProductCategories, product.Id, model.CategoryId) != null)
                    continue;

                //insert the new product category mapping
                await _categoryService.InsertProductCategoryAsync(new ProductCategory
                {
                    CategoryId = model.CategoryId,
                    ProductId = product.Id,
                    IsFeaturedProduct = false,
                    DisplayOrder = 1
                });
            }
        }

        ViewBag.RefreshPage = true;

        return View(new AddProductToCategorySearchModel());
    }

    #endregion
}