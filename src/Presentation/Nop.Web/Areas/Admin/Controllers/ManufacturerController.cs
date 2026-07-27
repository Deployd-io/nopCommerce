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

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ManufacturerController : BaseAdminController
{
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
        _customerActivityService = customerActivityService;
        _discountService = discountService;
        _exportManager = exportManager;
        _importManager = importManager;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _manufacturerModelFactory = manufacturerModelFactory;
        _manufacturerService = manufacturerService;
        _notificationService = notificationService;
        _pictureService = pictureService;
        _productService = productService;
        _storeMappingService = storeMappingService;
        _translationModelFactory = translationModelFactory;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(Manufacturer manufacturer, ManufacturerModel model)
    {
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
            await _urlRecordService.SaveSlugAsync(manufacturer, seName, localized.LanguageId);
        }
    }

    protected virtual async Task UpdatePictureSeoNamesAsync(Manufacturer manufacturer)
    {
        var picture = await _pictureService.GetPictureByIdAsync(manufacturer.PictureId);
        if (picture != null)
            await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(manufacturer.Name));
    }

    #endregion

    #region List

    public virtual IActionResult Index()
    {
		logger.LogDebug(">>> Entering Index()");
		logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _manufacturerModelFactory.PrepareManufacturerSearchModelAsync(new ManufacturerSearchModel());

		logger.LogDebug("List(): model → {Model}", model);
		logger.LogDebug("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_VIEW)]
    public virtual async Task<IActionResult> List(ManufacturerSearchModel searchModel)
    {
        //prepare model
        var model = await _manufacturerModelFactory.PrepareManufacturerListModelAsync(searchModel);

		logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Create / Edit / Delete

    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
		logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = await _manufacturerModelFactory.PrepareManufacturerModelAsync(new ManufacturerModel(), null);

		logger.LogDebug("Create(): model → {Model}", model);
		logger.LogDebug("<<< Exiting Create()");
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
		logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get a manufacturer with the specified id
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
        if (manufacturer == null || manufacturer.Deleted)
			logger.LogDebug("Edit(id={Id}): manufacturer → {Manufacturer}", id, manufacturer);
            return RedirectToAction("List");

        //prepare model
        var model = await _manufacturerModelFactory.PrepareManufacturerModelAsync(null, manufacturer);

		logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);
		logger.LogDebug("<<< Exiting Edit(id={Id})", id);
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
		logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PreTranslate(int itemId)
    {
		logger.LogDebug(">>> Entering PreTranslate(itemId={ItemId})", itemId);
        var translationModel = new TranslationModel();

		logger.LogDebug("PreTranslate(itemId={ItemId}): translationModel → {TranslationModel}", itemId, translationModel);
        //try to get a manufacturer with the specified id
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(itemId);
        if (manufacturer == null || manufacturer.Deleted)
			logger.LogDebug("PreTranslate(itemId={ItemId}): manufacturer → {Manufacturer}", itemId, manufacturer);
            return Json(translationModel);

        //prepare model
        var model = await _manufacturerModelFactory.PrepareManufacturerModelAsync(null, manufacturer);

		logger.LogDebug("PreTranslate(itemId={ItemId}): model → {Model}", itemId, model);
        translationModel = await _translationModelFactory.PrepareTranslationModelAsync(model,
            (nameof(ManufacturerLocalizedModel.Name), false),
            (nameof(ManufacturerLocalizedModel.Description), true));

		logger.LogDebug("<<< Exiting PreTranslate(itemId={ItemId})", itemId);
        return Json(translationModel);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a manufacturer with the specified id
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
        if (manufacturer == null)
			logger.LogDebug("Delete(id={Id}): manufacturer → {Manufacturer}", id, manufacturer);
            return RedirectToAction("List");

        await _manufacturerService.DeleteManufacturerAsync(manufacturer);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteManufacturer",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteManufacturer"), manufacturer.Name), manufacturer);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Manufacturers.Deleted"));

		logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
		logger.LogDebug(">>> Entering DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var manufacturers = await _manufacturerService.GetManufacturersByIdsAsync(selectedIds.ToArray());
        await _manufacturerService.DeleteManufacturersAsync(manufacturers);
		logger.LogDebug("DeleteSelected(selectedIds={SelectedIds}): manufacturers → {Manufacturers}", selectedIds, manufacturers);

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteManufacturer");
        await _customerActivityService.InsertActivitiesAsync("DeleteManufacturer", manufacturers, manufacturer => string.Format(activityLogFormat, manufacturer.Name));
		logger.LogDebug("DeleteSelected(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);

		logger.LogDebug("<<< Exiting DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    #endregion

    #region Export / Import

    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXml()
    {
		logger.LogDebug(">>> Entering ExportXml()");
        try
        {
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true);
            var xml = await _exportManager.ExportManufacturersToXmlAsync(manufacturers);
			logger.LogDebug("ExportXml(): manufacturers → {Manufacturers}", manufacturers);
			logger.LogDebug("ExportXml(): xml → {Xml}", xml);
            return File(Encoding.UTF8.GetBytes(xml), "application/xml", "manufacturers.xml");
        }
        catch (Exception exc)
        {
			logger.LogError("Exception in ExportXml(): {Arg0}", ex.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			logger.LogDebug("<<< Exiting ExportXml()");
            return RedirectToAction("List");
        }
    }

    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXlsx()
    {
		logger.LogDebug(">>> Entering ExportXlsx()");
        try
        {
            var bytes = await _exportManager.ExportManufacturersToXlsxAsync((await _manufacturerService.GetAllManufacturersAsync(showHidden: true)).Where(p => !p.Deleted).ToList());

			logger.LogDebug("ExportXlsx(): bytes → {Bytes}", bytes);
            return File(bytes, MimeTypes.TextXlsx, "manufacturers.xlsx");
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
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ImportFromXlsx(IFormFile importexcelfile)
    {
		logger.LogDebug(">>> Entering ImportFromXlsx(importexcelfile={Importexcelfile})", importexcelfile);
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
			logger.LogError("Exception in ImportFromXlsx(importexcelfile={Importexcelfile}): {Arg1}", importexcelfile, ex.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			logger.LogDebug("<<< Exiting ImportFromXlsx(importexcelfile={Importexcelfile})", importexcelfile);
            return RedirectToAction("List");
        }
    }

    #endregion

    #region Products

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_VIEW)]
    public virtual async Task<IActionResult> ProductList(ManufacturerProductSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ProductList(searchModel={SearchModel})", searchModel);
        //try to get a manufacturer with the specified id
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(searchModel.ManufacturerId)
            ?? throw new ArgumentException("No manufacturer found with the specified id");

        //prepare model
        var model = await _manufacturerModelFactory.PrepareManufacturerProductListModelAsync(searchModel, manufacturer);

		logger.LogDebug("ProductList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ProductList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductUpdate(ManufacturerProductModel model)
    {
        //try to get a product manufacturer with the specified id
        var productManufacturer = await _manufacturerService.GetProductManufacturerByIdAsync(model.Id)
            ?? throw new ArgumentException("No product manufacturer mapping found with the specified id");

        //fill entity from model
        productManufacturer = model.ToEntity(productManufacturer);
        await _manufacturerService.UpdateProductManufacturerAsync(productManufacturer);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductDelete(int id)
    {
	logger.LogDebug(">>> Entering ProductDelete(id={Id})", id);

        //try to get a product manufacturer with the specified id
        var productManufacturer = await _manufacturerService.GetProductManufacturerByIdAsync(id)
            ?? throw new ArgumentException("No product manufacturer mapping found with the specified id");

        await _manufacturerService.DeleteProductManufacturerAsync(productManufacturer);

		logger.LogDebug("<<< Exiting ProductDelete(id={Id})", id);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopup(int manufacturerId)
    {
		logger.LogDebug(">>> Entering ProductAddPopup(manufacturerId={ManufacturerId})", manufacturerId);
        //prepare model
        var model = await _manufacturerModelFactory.PrepareAddProductToManufacturerSearchModelAsync(new AddProductToManufacturerSearchModel());

		logger.LogDebug("ProductAddPopup(manufacturerId={ManufacturerId}): model → {Model}", manufacturerId, model);
		logger.LogDebug("<<< Exiting ProductAddPopup(manufacturerId={ManufacturerId})", manufacturerId);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopupList(AddProductToManufacturerSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ProductAddPopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _manufacturerModelFactory.PrepareAddProductToManufacturerListModelAsync(searchModel);

		logger.LogDebug("ProductAddPopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ProductAddPopupList(searchModel={SearchModel})", searchModel);
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