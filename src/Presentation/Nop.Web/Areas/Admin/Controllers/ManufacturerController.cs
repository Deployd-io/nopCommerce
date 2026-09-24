using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
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

public partial class ManufacturerController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IDiscountService _discountService;
    protected readonly IExportManager _exportManager;
    protected readonly IImportManager _importManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly IManufacturerModelFactory _manufacturerModelFactory;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly INotificationService _notificationService;
    protected readonly IPictureService _pictureService;
    protected readonly IProductService _productService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly ITranslationModelFactory _translationModelFactory;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public ManufacturerController(ICustomerActivityService customerActivityService,
        IDiscountService discountService,
        IExportManager exportManager,
        IImportManager importManager,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        IManufacturerModelFactory manufacturerModelFactory,
        IManufacturerService manufacturerService,
        INotificationService notificationService,
        IPictureService pictureService,
        IProductService productService,
        IStoreMappingService storeMappingService,
        ITranslationModelFactory translationModelFactory,
        IUrlRecordService urlRecordService,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext})", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext);
        _customerActivityService = customerActivityService;
        _discountService = discountService;
		_logger.LogDebug("ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _discountService → {_discountService}", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext, _discountService);
        _exportManager = exportManager;
		_logger.LogDebug("ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _exportManager → {_exportManager}", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext, _exportManager);
        _importManager = importManager;
		_logger.LogDebug("ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _importManager → {_importManager}", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext, _importManager);
        _localizationService = localizationService;
		_logger.LogDebug("ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _localizationService → {_localizationService}", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext, _localizationService);
        _localizedEntityService = localizedEntityService;
		_logger.LogDebug("ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _localizedEntityService → {_localizedEntityService}", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext, _localizedEntityService);
        _manufacturerModelFactory = manufacturerModelFactory;
		_logger.LogDebug("ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _manufacturerModelFactory → {_manufacturerModelFactory}", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext, _manufacturerModelFactory);
        _manufacturerService = manufacturerService;
		_logger.LogDebug("ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _manufacturerService → {_manufacturerService}", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext, _manufacturerService);
        _notificationService = notificationService;
		_logger.LogDebug("ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _notificationService → {_notificationService}", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext, _notificationService);
        _pictureService = pictureService;
		_logger.LogDebug("ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _pictureService → {_pictureService}", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext, _pictureService);
        _productService = productService;
		_logger.LogDebug("ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _productService → {_productService}", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext, _productService);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _storeMappingService → {_storeMappingService}", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext, _storeMappingService);
        _translationModelFactory = translationModelFactory;
		_logger.LogDebug("ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _translationModelFactory → {_translationModelFactory}", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext, _translationModelFactory);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _urlRecordService → {_urlRecordService}", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext, _urlRecordService);
        _workContext = workContext;
		_logger.LogDebug("ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _workContext → {_workContext}", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext, _workContext);
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(Manufacturer manufacturer, ManufacturerModel model)
    {
		_logger.LogError(">>> Entering UpdateLocalesAsync(manufacturer={Manufacturer},model={Model})", manufacturer, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(manufacturer,
                x => x.Name,
                localized.Name,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(manufacturer,
                x => x.Description,
                localized.Description,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(manufacturer,
                x => x.MetaKeywords,
                localized.MetaKeywords,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(manufacturer,
                x => x.MetaDescription,
                localized.MetaDescription,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(manufacturer,
                x => x.MetaTitle,
                localized.MetaTitle,
                localized.LanguageId);

            //search engine name
            var seName = await _urlRecordService.ValidateSeNameAsync(manufacturer, localized.SeName, localized.Name, false);
			_logger.LogError("UpdateLocalesAsync(manufacturer={Manufacturer},model={Model}): seName → {SeName}", manufacturer, model, seName);
            await _urlRecordService.SaveSlugAsync(manufacturer, seName, localized.LanguageId);
        }
		_logger.LogWarning("<<< Exiting UpdateLocalesAsync(manufacturer={Manufacturer},model={Model})", manufacturer, model);
    }

    protected virtual async Task UpdatePictureSeoNamesAsync(Manufacturer manufacturer)
    {
		_logger.LogError(">>> Entering UpdatePictureSeoNamesAsync(manufacturer={Manufacturer})", manufacturer);
        var picture = await _pictureService.GetPictureByIdAsync(manufacturer.PictureId);
		_logger.LogWarning("<<< Exiting UpdatePictureSeoNamesAsync(manufacturer={Manufacturer})", manufacturer);
        if (picture != null)
            await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(manufacturer.Name));
			_logger.LogError("UpdatePictureSeoNamesAsync(manufacturer={Manufacturer}): picture → {Picture}", manufacturer, picture);
    }

    #endregion

    #region List

    public virtual IActionResult Index()
    {
		_logger.LogError(">>> Entering Index()");
        _logger.LogWarning("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogError(">>> Entering List()");
        //prepare model
        var model = await _manufacturerModelFactory.PrepareManufacturerSearchModelAsync(new ManufacturerSearchModel());
		_logger.LogError("List(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting List()");
		_logger.LogWarning("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_VIEW)]
    public virtual async Task<IActionResult> List(ManufacturerSearchModel searchModel)
    {
        //prepare model
        var model = await _manufacturerModelFactory.PrepareManufacturerListModelAsync(searchModel);

        return Json(model);
    }

    #endregion

    #region Create / Edit / Delete

    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogError(">>> Entering Create()");
        //prepare model
        var model = await _manufacturerModelFactory.PrepareManufacturerModelAsync(new ManufacturerModel(), null);
		_logger.LogError("Create(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting Create()");
		_logger.LogWarning("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(ManufacturerModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var manufacturer = model.ToEntity<Manufacturer>();
            manufacturer.CreatedOnUtc = DateTime.UtcNow;
            manufacturer.UpdatedOnUtc = DateTime.UtcNow;
            await _manufacturerService.InsertManufacturerAsync(manufacturer);

            //search engine name
            model.SeName = await _urlRecordService.ValidateSeNameAsync(manufacturer, model.SeName, manufacturer.Name, true);
            await _urlRecordService.SaveSlugAsync(manufacturer, model.SeName, 0);

            //locales
            await UpdateLocalesAsync(manufacturer, model);

            //discounts
            var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToManufacturers, showHidden: true, isActive: null);
            foreach (var discount in allDiscounts)
            {
                if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    //manufacturer.AppliedDiscounts.Add(discount);
                    await _manufacturerService.InsertDiscountManufacturerMappingAsync(new DiscountManufacturerMapping { EntityId = manufacturer.Id, DiscountId = discount.Id });

            }

            await _manufacturerService.UpdateManufacturerAsync(manufacturer);

            //update picture seo file name
            await UpdatePictureSeoNamesAsync(manufacturer);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(manufacturer, model.SelectedStoreIds);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewManufacturer",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewManufacturer"), manufacturer.Name), manufacturer);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Manufacturers.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = manufacturer.Id });
        }

        //prepare model
        model = await _manufacturerModelFactory.PrepareManufacturerModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogError(">>> Entering Edit(id={Id})", id);
        //try to get a manufacturer with the specified id
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
		_logger.LogError("Edit(id={Id}): manufacturer → {Manufacturer}", id, manufacturer);
        if (manufacturer == null || manufacturer.Deleted)
            return RedirectToAction("List");

        //prepare model
        var model = await _manufacturerModelFactory.PrepareManufacturerModelAsync(null, manufacturer);
		_logger.LogError("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting Edit(id={Id})", id);
		_logger.LogWarning("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(ManufacturerModel model, bool continueEditing)
    {
        //try to get a manufacturer with the specified id
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(model.Id);
        if (manufacturer == null || manufacturer.Deleted)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            var prevPictureId = manufacturer.PictureId;
            manufacturer = model.ToEntity(manufacturer);
            manufacturer.UpdatedOnUtc = DateTime.UtcNow;
            await _manufacturerService.UpdateManufacturerAsync(manufacturer);

            //search engine name
            model.SeName = await _urlRecordService.ValidateSeNameAsync(manufacturer, model.SeName, manufacturer.Name, true);
            await _urlRecordService.SaveSlugAsync(manufacturer, model.SeName, 0);

            //locales
            await UpdateLocalesAsync(manufacturer, model);

            //discounts
            var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToManufacturers, showHidden: true, isActive: null);
            foreach (var discount in allDiscounts)
            {
                if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                {
                    //new discount
                    if (await _manufacturerService.GetDiscountAppliedToManufacturerAsync(manufacturer.Id, discount.Id) is null)
                        await _manufacturerService.InsertDiscountManufacturerMappingAsync(new DiscountManufacturerMapping { EntityId = manufacturer.Id, DiscountId = discount.Id });
                }
                else
                {
                    //remove discount
                    if (await _manufacturerService.GetDiscountAppliedToManufacturerAsync(manufacturer.Id, discount.Id) is DiscountManufacturerMapping discountManufacturerMapping)
                        await _manufacturerService.DeleteDiscountManufacturerMappingAsync(discountManufacturerMapping);
                }
            }

            await _manufacturerService.UpdateManufacturerAsync(manufacturer);

            //delete an old picture (if deleted or updated)
            if (prevPictureId > 0 && prevPictureId != manufacturer.PictureId)
            {
                var prevPicture = await _pictureService.GetPictureByIdAsync(prevPictureId);
                if (prevPicture != null)
                    await _pictureService.DeletePictureAsync(prevPicture);
            }

            //update picture seo file name
            await UpdatePictureSeoNamesAsync(manufacturer);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(manufacturer, model.SelectedStoreIds);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditManufacturer",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditManufacturer"), manufacturer.Name), manufacturer);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Manufacturers.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = manufacturer.Id });
        }

        //prepare model
        model = await _manufacturerModelFactory.PrepareManufacturerModelAsync(model, manufacturer, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PreTranslate(int itemId)
    {
		_logger.LogError(">>> Entering PreTranslate(itemId={ItemId})", itemId);
        var translationModel = new TranslationModel();

		_logger.LogError("PreTranslate(itemId={ItemId}): translationModel → {TranslationModel}", itemId, translationModel);
        //try to get a manufacturer with the specified id
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(itemId);
		_logger.LogError("PreTranslate(itemId={ItemId}): manufacturer → {Manufacturer}", itemId, manufacturer);
        if (manufacturer == null || manufacturer.Deleted)
            return Json(translationModel);

        //prepare model
        var model = await _manufacturerModelFactory.PrepareManufacturerModelAsync(null, manufacturer);
		_logger.LogError("PreTranslate(itemId={ItemId}): model → {Model}", itemId, model);

        translationModel = await _translationModelFactory.PrepareTranslationModelAsync(model,
            (nameof(ManufacturerLocalizedModel.Name), false),
            (nameof(ManufacturerLocalizedModel.Description), true));

		_logger.LogWarning("<<< Exiting PreTranslate(itemId={ItemId})", itemId);
        return Json(translationModel);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogError(">>> Entering Delete(id={Id})", id);
        //try to get a manufacturer with the specified id
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
		_logger.LogError("Delete(id={Id}): manufacturer → {Manufacturer}", id, manufacturer);
        if (manufacturer == null)
            return RedirectToAction("List");

        await _manufacturerService.DeleteManufacturerAsync(manufacturer);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteManufacturer",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteManufacturer"), manufacturer.Name), manufacturer);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Manufacturers.Deleted"));

		_logger.LogWarning("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
		_logger.LogError(">>> Entering DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var manufacturers = await _manufacturerService.GetManufacturersByIdsAsync(selectedIds.ToArray());
		_logger.LogError("DeleteSelected(selectedIds={SelectedIds}): manufacturers → {Manufacturers}", selectedIds, manufacturers);
        await _manufacturerService.DeleteManufacturersAsync(manufacturers);

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteManufacturer");
		_logger.LogError("DeleteSelected(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);
        await _customerActivityService.InsertActivitiesAsync("DeleteManufacturer", manufacturers, manufacturer => string.Format(activityLogFormat, manufacturer.Name));

		_logger.LogWarning("<<< Exiting DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    #endregion

    #region Export / Import

    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXml()
    {
		_logger.LogError(">>> Entering ExportXml()");
        try
        {
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true);
			_logger.LogError("ExportXml(): manufacturers → {Manufacturers}", manufacturers);
            var xml = await _exportManager.ExportManufacturersToXmlAsync(manufacturers);
			_logger.LogError("ExportXml(): xml → {Xml}", xml);
            return File(Encoding.UTF8.GetBytes(xml), "application/xml", "manufacturers.xml");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ExportXml(): {Arg0}", exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogWarning("<<< Exiting ExportXml()");
            return RedirectToAction("List");
        }
    }

    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXlsx()
    {
		_logger.LogError(">>> Entering ExportXlsx()");
        try
        {
            var bytes = await _exportManager.ExportManufacturersToXlsxAsync((await _manufacturerService.GetAllManufacturersAsync(showHidden: true)).Where(p => !p.Deleted).ToList());
			_logger.LogError("ExportXlsx(): bytes → {Bytes}", bytes);

            return File(bytes, MimeTypes.TextXlsx, "manufacturers.xlsx");
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
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ImportFromXlsx(IFormFile importexcelfile)
    {
		_logger.LogError(">>> Entering ImportFromXlsx(importexcelfile={Importexcelfile})", importexcelfile);
        //a vendor cannot import manufacturers
        if (await _workContext.GetCurrentVendorAsync() != null)
            return AccessDeniedView();

        try
        {
            if (importexcelfile != null && importexcelfile.Length > 0)
            {
                await _importManager.ImportManufacturersFromXlsxAsync(importexcelfile.OpenReadStream());
            }
            else
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));
                return RedirectToAction("List");
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Manufacturers.Imported"));
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
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_VIEW)]
    public virtual async Task<IActionResult> ProductList(ManufacturerProductSearchModel searchModel)
    {
		_logger.LogError(">>> Entering ProductList(searchModel={SearchModel})", searchModel);
        //try to get a manufacturer with the specified id
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(searchModel.ManufacturerId)
            ?? throw new ArgumentException("No manufacturer found with the specified id");

        //prepare model
        var model = await _manufacturerModelFactory.PrepareManufacturerProductListModelAsync(searchModel, manufacturer);
		_logger.LogError("ProductList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting ProductList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductUpdate(ManufacturerProductModel model)
    {
		_logger.LogError(">>> Entering ProductUpdate(model={Model})", model);
        //try to get a product manufacturer with the specified id
        var productManufacturer = await _manufacturerService.GetProductManufacturerByIdAsync(model.Id)
            ?? throw new ArgumentException("No product manufacturer mapping found with the specified id");

        //fill entity from model
        productManufacturer = model.ToEntity(productManufacturer);
		_logger.LogError("ProductUpdate(model={Model}): productManufacturer → {ProductManufacturer}", model, productManufacturer);
        await _manufacturerService.UpdateProductManufacturerAsync(productManufacturer);

		_logger.LogWarning("<<< Exiting ProductUpdate(model={Model})", model);
        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductDelete(int id)
    {
	_logger.LogError(">>> Entering ProductDelete(id={Id})", id);

        //try to get a product manufacturer with the specified id
        var productManufacturer = await _manufacturerService.GetProductManufacturerByIdAsync(id)
            ?? throw new ArgumentException("No product manufacturer mapping found with the specified id");

        await _manufacturerService.DeleteProductManufacturerAsync(productManufacturer);

		_logger.LogWarning("<<< Exiting ProductDelete(id={Id})", id);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopup(int manufacturerId)
    {
		_logger.LogError(">>> Entering ProductAddPopup(manufacturerId={ManufacturerId})", manufacturerId);
        //prepare model
        var model = await _manufacturerModelFactory.PrepareAddProductToManufacturerSearchModelAsync(new AddProductToManufacturerSearchModel());
		_logger.LogError("ProductAddPopup(manufacturerId={ManufacturerId}): model → {Model}", manufacturerId, model);

		_logger.LogWarning("<<< Exiting ProductAddPopup(manufacturerId={ManufacturerId})", manufacturerId);
		_logger.LogWarning("<<< Exiting ProductAddPopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopupList(AddProductToManufacturerSearchModel searchModel)
    {
		_logger.LogError(">>> Entering ProductAddPopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _manufacturerModelFactory.PrepareAddProductToManufacturerListModelAsync(searchModel);
		_logger.LogError("ProductAddPopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting ProductAddPopupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopup(AddProductToManufacturerModel model)
    {
        //get selected products
        var selectedProducts = await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
        if (selectedProducts.Any())
        {
            var existingProductmanufacturers = await _manufacturerService
                .GetProductManufacturersByManufacturerIdAsync(model.ManufacturerId, showHidden: true);
            foreach (var product in selectedProducts)
            {
                //whether product manufacturer with such parameters already exists
                if (_manufacturerService.FindProductManufacturer(existingProductmanufacturers, product.Id, model.ManufacturerId) != null)
                    continue;

                //insert the new product manufacturer mapping
                await _manufacturerService.InsertProductManufacturerAsync(new ProductManufacturer
                {
                    ManufacturerId = model.ManufacturerId,
                    ProductId = product.Id,
                    IsFeaturedProduct = false,
                    DisplayOrder = 1
                });
            }
        }

        ViewBag.RefreshPage = true;

        return View(new AddProductToManufacturerSearchModel());
    }

    #endregion
}