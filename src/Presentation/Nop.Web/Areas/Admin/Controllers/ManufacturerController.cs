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
		_logger.LogDebug(">>> Entering ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext})", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _customerActivityService → {_customerActivityService}", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext, _customerActivityService);
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
		_logger.LogDebug("<<< Exiting ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext})", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext);
		_logger.LogDebug("ManufacturerController(customerActivityService={CustomerActivityService},discountService={DiscountService},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},manufacturerModelFactory={ManufacturerModelFactory},manufacturerService={ManufacturerService},notificationService={NotificationService},pictureService={PictureService},productService={ProductService},storeMappingService=<redacted>,translationModelFactory={TranslationModelFactory},urlRecordService={UrlRecordService},workContext={WorkContext}): _workContext → {_workContext}", customerActivityService, discountService, exportManager, importManager, localizationService, localizedEntityService, manufacturerModelFactory, manufacturerService, notificationService, pictureService, productService, translationModelFactory, urlRecordService, workContext, _workContext);
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(Manufacturer manufacturer, ManufacturerModel model)
    {
		_logger.LogDebug(">>> Entering UpdateLocalesAsync(manufacturer={Manufacturer},model={Model})", manufacturer, model);
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
			_logger.LogDebug("UpdateLocalesAsync(manufacturer={Manufacturer},model={Model}): seName → {SeName}", manufacturer, model, seName);
            await _urlRecordService.SaveSlugAsync(manufacturer, seName, localized.LanguageId);
        }
		_logger.LogDebug("<<< Exiting UpdateLocalesAsync(manufacturer={Manufacturer},model={Model})", manufacturer, model);
    }

    protected virtual async Task UpdatePictureSeoNamesAsync(Manufacturer manufacturer)
    {
		_logger.LogDebug(">>> Entering UpdatePictureSeoNamesAsync(manufacturer={Manufacturer})", manufacturer);
        var picture = await _pictureService.GetPictureByIdAsync(manufacturer.PictureId);
        if (picture != null)
			_logger.LogDebug("UpdatePictureSeoNamesAsync(manufacturer={Manufacturer}): picture → {Picture}", manufacturer, picture);
            await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(manufacturer.Name));
			_logger.LogDebug("<<< Exiting UpdatePictureSeoNamesAsync(manufacturer={Manufacturer})", manufacturer);
    }

    #endregion

    #region List

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
		_logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _manufacturerModelFactory.PrepareManufacturerSearchModelAsync(new ManufacturerSearchModel());
		_logger.LogDebug("List(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting List()");
		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
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
		_logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = await _manufacturerModelFactory.PrepareManufacturerModelAsync(new ManufacturerModel(), null);
		_logger.LogDebug("Create(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting Create()");
		_logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
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
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get a manufacturer with the specified id
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
		_logger.LogDebug("Edit(id={Id}): manufacturer → {Manufacturer}", id, manufacturer);
        if (manufacturer == null || manufacturer.Deleted)
            return RedirectToAction("List");

        //prepare model
        var model = await _manufacturerModelFactory.PrepareManufacturerModelAsync(null, manufacturer);
		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
		_logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
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
		_logger.LogDebug(">>> Entering PreTranslate(itemId={ItemId})", itemId);
        var translationModel = new TranslationModel();

		_logger.LogDebug("PreTranslate(itemId={ItemId}): translationModel → {TranslationModel}", itemId, translationModel);
        //try to get a manufacturer with the specified id
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(itemId);
		_logger.LogDebug("PreTranslate(itemId={ItemId}): manufacturer → {Manufacturer}", itemId, manufacturer);
        if (manufacturer == null || manufacturer.Deleted)
            return Json(translationModel);

        //prepare model
        var model = await _manufacturerModelFactory.PrepareManufacturerModelAsync(null, manufacturer);
		_logger.LogDebug("PreTranslate(itemId={ItemId}): model → {Model}", itemId, model);

        translationModel = await _translationModelFactory.PrepareTranslationModelAsync(model,
            (nameof(ManufacturerLocalizedModel.Name), false),
            (nameof(ManufacturerLocalizedModel.Description), true));

		_logger.LogDebug("<<< Exiting PreTranslate(itemId={ItemId})", itemId);
        return Json(translationModel);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a manufacturer with the specified id
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
		_logger.LogDebug("Delete(id={Id}): manufacturer → {Manufacturer}", id, manufacturer);
        if (manufacturer == null)
            return RedirectToAction("List");

        await _manufacturerService.DeleteManufacturerAsync(manufacturer);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteManufacturer",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteManufacturer"), manufacturer.Name), manufacturer);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Manufacturers.Deleted"));

		_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
		_logger.LogDebug(">>> Entering DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var manufacturers = await _manufacturerService.GetManufacturersByIdsAsync(selectedIds.ToArray());
		_logger.LogDebug("DeleteSelected(selectedIds={SelectedIds}): manufacturers → {Manufacturers}", selectedIds, manufacturers);
        await _manufacturerService.DeleteManufacturersAsync(manufacturers);

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteManufacturer");
		_logger.LogDebug("DeleteSelected(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);
        await _customerActivityService.InsertActivitiesAsync("DeleteManufacturer", manufacturers, manufacturer => string.Format(activityLogFormat, manufacturer.Name));

		_logger.LogDebug("<<< Exiting DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    #endregion

    #region Export / Import

    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXml()
    {
		_logger.LogDebug(">>> Entering ExportXml()");
        try
        {
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true);
			_logger.LogDebug("ExportXml(): manufacturers → {Manufacturers}", manufacturers);
            var xml = await _exportManager.ExportManufacturersToXmlAsync(manufacturers);
			_logger.LogDebug("ExportXml(): xml → {Xml}", xml);
            return File(Encoding.UTF8.GetBytes(xml), "application/xml", "manufacturers.xml");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ExportXml(): {Arg0}", exc.getMessage());
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting ExportXml()");
            return RedirectToAction("List");
        }
    }

    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXlsx()
    {
		_logger.LogDebug(">>> Entering ExportXlsx()");
        try
        {
            var bytes = await _exportManager.ExportManufacturersToXlsxAsync((await _manufacturerService.GetAllManufacturersAsync(showHidden: true)).Where(p => !p.Deleted).ToList());
			_logger.LogDebug("ExportXlsx(): bytes → {Bytes}", bytes);

            return File(bytes, MimeTypes.TextXlsx, "manufacturers.xlsx");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ExportXlsx(): {Arg0}", exc.getMessage());
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting ExportXlsx()");
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ImportFromXlsx(IFormFile importexcelfile)
    {
		_logger.LogDebug(">>> Entering ImportFromXlsx(importexcelfile={Importexcelfile})", importexcelfile);
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
			_logger.LogError("Exception in ImportFromXlsx(importexcelfile={Importexcelfile}): {Arg1}", importexcelfile, exc.getMessage());
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting ImportFromXlsx(importexcelfile={Importexcelfile})", importexcelfile);
            return RedirectToAction("List");
        }
    }

    #endregion

    #region Products

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_VIEW)]
    public virtual async Task<IActionResult> ProductList(ManufacturerProductSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ProductList(searchModel={SearchModel})", searchModel);
        //try to get a manufacturer with the specified id
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(searchModel.ManufacturerId)
            ?? throw new ArgumentException("No manufacturer found with the specified id");

        //prepare model
        var model = await _manufacturerModelFactory.PrepareManufacturerProductListModelAsync(searchModel, manufacturer);
		_logger.LogDebug("ProductList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting ProductList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductUpdate(ManufacturerProductModel model)
    {
		_logger.LogDebug(">>> Entering ProductUpdate(model={Model})", model);
        //try to get a product manufacturer with the specified id
        var productManufacturer = await _manufacturerService.GetProductManufacturerByIdAsync(model.Id)
            ?? throw new ArgumentException("No product manufacturer mapping found with the specified id");
			_logger.LogDebug("ProductUpdate(model={Model}): productManufacturer → {ProductManufacturer}", model, productManufacturer);

        //fill entity from model
        productManufacturer = model.ToEntity(productManufacturer);
        await _manufacturerService.UpdateProductManufacturerAsync(productManufacturer);

		_logger.LogDebug("<<< Exiting ProductUpdate(model={Model})", model);
        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductDelete(int id)
    {
	_logger.LogDebug(">>> Entering ProductDelete(id={Id})", id);

        //try to get a product manufacturer with the specified id
        var productManufacturer = await _manufacturerService.GetProductManufacturerByIdAsync(id)
            ?? throw new ArgumentException("No product manufacturer mapping found with the specified id");

        await _manufacturerService.DeleteProductManufacturerAsync(productManufacturer);

		_logger.LogDebug("<<< Exiting ProductDelete(id={Id})", id);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopup(int manufacturerId)
    {
		_logger.LogDebug(">>> Entering ProductAddPopup(manufacturerId={ManufacturerId})", manufacturerId);
        //prepare model
        var model = await _manufacturerModelFactory.PrepareAddProductToManufacturerSearchModelAsync(new AddProductToManufacturerSearchModel());
		_logger.LogDebug("ProductAddPopup(manufacturerId={ManufacturerId}): model → {Model}", manufacturerId, model);

		_logger.LogDebug("<<< Exiting ProductAddPopup(manufacturerId={ManufacturerId})", manufacturerId);
		_logger.LogDebug("<<< Exiting ProductAddPopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopupList(AddProductToManufacturerSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ProductAddPopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _manufacturerModelFactory.PrepareAddProductToManufacturerListModelAsync(searchModel);
		_logger.LogDebug("ProductAddPopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting ProductAddPopupList(searchModel={SearchModel})", searchModel);
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