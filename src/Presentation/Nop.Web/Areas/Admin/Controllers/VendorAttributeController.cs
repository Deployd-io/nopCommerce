using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Vendors;
using Nop.Services.Attributes;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class VendorAttributeController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAttributeService<VendorAttribute, VendorAttributeValue> _vendorAttributeService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IVendorAttributeModelFactory _vendorAttributeModelFactory;

    #endregion

    #region Ctor

    public VendorAttributeController(IAttributeService<VendorAttribute, VendorAttributeValue> vendorAttributeService,
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IVendorAttributeModelFactory vendorAttributeModelFactory)
    {
		_logger.LogDebug(">>> Entering VendorAttributeController(vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},vendorAttributeModelFactory={VendorAttributeModelFactory})", vendorAttributeService, customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, vendorAttributeModelFactory);
        _vendorAttributeService = vendorAttributeService;
		_logger.LogDebug("VendorAttributeController(vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},vendorAttributeModelFactory={VendorAttributeModelFactory}): _vendorAttributeService → {_vendorAttributeService}", vendorAttributeService, customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, vendorAttributeModelFactory, _vendorAttributeService);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("VendorAttributeController(vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},vendorAttributeModelFactory={VendorAttributeModelFactory}): _customerActivityService → {_customerActivityService}", vendorAttributeService, customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, vendorAttributeModelFactory, _customerActivityService);
        _localizationService = localizationService;
		_logger.LogDebug("VendorAttributeController(vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},vendorAttributeModelFactory={VendorAttributeModelFactory}): _localizationService → {_localizationService}", vendorAttributeService, customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, vendorAttributeModelFactory, _localizationService);
        _localizedEntityService = localizedEntityService;
		_logger.LogDebug("VendorAttributeController(vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},vendorAttributeModelFactory={VendorAttributeModelFactory}): _localizedEntityService → {_localizedEntityService}", vendorAttributeService, customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, vendorAttributeModelFactory, _localizedEntityService);
        _notificationService = notificationService;
		_logger.LogDebug("VendorAttributeController(vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},vendorAttributeModelFactory={VendorAttributeModelFactory}): _notificationService → {_notificationService}", vendorAttributeService, customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, vendorAttributeModelFactory, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("VendorAttributeController(vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},vendorAttributeModelFactory={VendorAttributeModelFactory}): _permissionService → {_permissionService}", vendorAttributeService, customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, vendorAttributeModelFactory, _permissionService);
        _vendorAttributeModelFactory = vendorAttributeModelFactory;
		_logger.LogDebug("VendorAttributeController(vendorAttributeService={VendorAttributeService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},vendorAttributeModelFactory={VendorAttributeModelFactory}): _vendorAttributeModelFactory → {_vendorAttributeModelFactory}", vendorAttributeService, customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, vendorAttributeModelFactory, _vendorAttributeModelFactory);
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateAttributeLocalesAsync(VendorAttribute vendorAttribute, VendorAttributeModel model)
    {
		_logger.LogDebug(">>> Entering UpdateAttributeLocalesAsync(vendorAttribute={VendorAttribute},model={Model})", vendorAttribute, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(vendorAttribute,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
		_logger.LogDebug("<<< Exiting UpdateAttributeLocalesAsync(vendorAttribute={VendorAttribute},model={Model})", vendorAttribute, model);
    }

    protected virtual async Task UpdateValueLocalesAsync(VendorAttributeValue vendorAttributeValue, VendorAttributeValueModel model)
    {
		_logger.LogDebug(">>> Entering UpdateValueLocalesAsync(vendorAttributeValue={VendorAttributeValue},model={Model})", vendorAttributeValue, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(vendorAttributeValue,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
		_logger.LogDebug("<<< Exiting UpdateValueLocalesAsync(vendorAttributeValue={VendorAttributeValue},model={Model})", vendorAttributeValue, model);
    }

    #endregion

    #region Vendor attributes

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual IActionResult List()
    {
		_logger.LogDebug(">>> Entering List()");
        //we just redirect a user to the vendor settings page
		_logger.LogDebug("<<< Exiting List()");
		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return RedirectToAction("Vendor", "Setting");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> List(VendorAttributeSearchModel searchModel)
    {
        //prepare model
        var model = await _vendorAttributeModelFactory.PrepareVendorAttributeListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = await _vendorAttributeModelFactory.PrepareVendorAttributeModelAsync(new VendorAttributeModel(), null);
		_logger.LogDebug("Create(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting Create()");
		_logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Create(VendorAttributeModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var vendorAttribute = model.ToEntity<VendorAttribute>();
            await _vendorAttributeService.InsertAttributeAsync(vendorAttribute);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewVendorAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewVendorAttribute"), vendorAttribute.Id), vendorAttribute);

            //locales
            await UpdateAttributeLocalesAsync(vendorAttribute, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.VendorAttributes.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = vendorAttribute.Id });
        }

        //prepare model
        model = await _vendorAttributeModelFactory.PrepareVendorAttributeModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get a vendor attribute with the specified id
        var vendorAttribute = await _vendorAttributeService.GetAttributeByIdAsync(id);
		_logger.LogDebug("Edit(id={Id}): vendorAttribute → {VendorAttribute}", id, vendorAttribute);
        if (vendorAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _vendorAttributeModelFactory.PrepareVendorAttributeModelAsync(null, vendorAttribute);
		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
		_logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Edit(VendorAttributeModel model, bool continueEditing)
    {
        //try to get a vendor attribute with the specified id
        var vendorAttribute = await _vendorAttributeService.GetAttributeByIdAsync(model.Id);
        if (vendorAttribute == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            vendorAttribute = model.ToEntity(vendorAttribute);
            await _vendorAttributeService.UpdateAttributeAsync(vendorAttribute);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditVendorAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditVendorAttribute"), vendorAttribute.Id), vendorAttribute);

            //locales
            await UpdateAttributeLocalesAsync(vendorAttribute, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.VendorAttributes.Updated"));
            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = vendorAttribute.Id });
        }

        //prepare model
        model = await _vendorAttributeModelFactory.PrepareVendorAttributeModelAsync(model, vendorAttribute, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a vendor attribute with the specified id
        var vendorAttribute = await _vendorAttributeService.GetAttributeByIdAsync(id);
		_logger.LogDebug("Delete(id={Id}): vendorAttribute → {VendorAttribute}", id, vendorAttribute);
        if (vendorAttribute == null)
            return RedirectToAction("List");

        await _vendorAttributeService.DeleteAttributeAsync(vendorAttribute);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteVendorAttribute",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteVendorAttribute"), vendorAttribute.Id), vendorAttribute);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.VendorAttributes.Deleted"));

		_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    #endregion

    #region Vendor attribute values

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueList(VendorAttributeValueSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ValueList(searchModel={SearchModel})", searchModel);
        //try to get a vendor attribute with the specified id
        var vendorAttribute = await _vendorAttributeService.GetAttributeByIdAsync(searchModel.VendorAttributeId)
            ?? throw new ArgumentException("No vendor attribute found with the specified id");

        //prepare model
        var model = await _vendorAttributeModelFactory.PrepareVendorAttributeValueListModelAsync(searchModel, vendorAttribute);
		_logger.LogDebug("ValueList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting ValueList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueCreatePopup(int vendorAttributeId)
    {
		_logger.LogDebug(">>> Entering ValueCreatePopup(vendorAttributeId={VendorAttributeId})", vendorAttributeId);
        //try to get a vendor attribute with the specified id
        var vendorAttribute = await _vendorAttributeService.GetAttributeByIdAsync(vendorAttributeId);
		_logger.LogDebug("ValueCreatePopup(vendorAttributeId={VendorAttributeId}): vendorAttribute → {VendorAttribute}", vendorAttributeId, vendorAttribute);
        if (vendorAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _vendorAttributeModelFactory.PrepareVendorAttributeValueModelAsync(new VendorAttributeValueModel(), vendorAttribute, null);
		_logger.LogDebug("ValueCreatePopup(vendorAttributeId={VendorAttributeId}): model → {Model}", vendorAttributeId, model);

		_logger.LogDebug("<<< Exiting ValueCreatePopup(vendorAttributeId={VendorAttributeId})", vendorAttributeId);
		_logger.LogDebug("<<< Exiting ValueCreatePopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueCreatePopup(VendorAttributeValueModel model)
    {
        //try to get a vendor attribute with the specified id
        var vendorAttribute = await _vendorAttributeService.GetAttributeByIdAsync(model.AttributeId);
        if (vendorAttribute == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            var value = model.ToEntity<VendorAttributeValue>();

            await _vendorAttributeService.InsertAttributeValueAsync(value);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewVendorAttributeValue",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewVendorAttributeValue"), value.Id), value);

            await UpdateValueLocalesAsync(value, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _vendorAttributeModelFactory.PrepareVendorAttributeValueModelAsync(model, vendorAttribute, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    //edit
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueEditPopup(int id)
    {
		_logger.LogDebug(">>> Entering ValueEditPopup(id={Id})", id);
        //try to get a vendor attribute value with the specified id
        var vendorAttributeValue = await _vendorAttributeService.GetAttributeValueByIdAsync(id);
		_logger.LogDebug("ValueEditPopup(id={Id}): vendorAttributeValue → {VendorAttributeValue}", id, vendorAttributeValue);
        if (vendorAttributeValue == null)
            return RedirectToAction("List");

        //try to get a vendor attribute with the specified id
        var vendorAttribute = await _vendorAttributeService.GetAttributeByIdAsync(vendorAttributeValue.AttributeId);
		_logger.LogDebug("ValueEditPopup(id={Id}): vendorAttribute → {VendorAttribute}", id, vendorAttribute);
        if (vendorAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _vendorAttributeModelFactory.PrepareVendorAttributeValueModelAsync(null, vendorAttribute, vendorAttributeValue);
		_logger.LogDebug("ValueEditPopup(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting ValueEditPopup(id={Id})", id);
		_logger.LogDebug("<<< Exiting ValueEditPopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueEditPopup(VendorAttributeValueModel model)
    {
        //try to get a vendor attribute value with the specified id
        var vendorAttributeValue = await _vendorAttributeService.GetAttributeValueByIdAsync(model.Id);
        if (vendorAttributeValue == null)
            return RedirectToAction("List");

        //try to get a vendor attribute with the specified id
        var vendorAttribute = await _vendorAttributeService.GetAttributeByIdAsync(vendorAttributeValue.AttributeId);
        if (vendorAttribute == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            vendorAttributeValue = model.ToEntity(vendorAttributeValue);
            await _vendorAttributeService.UpdateAttributeValueAsync(vendorAttributeValue);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditVendorAttributeValue",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditVendorAttributeValue"), vendorAttributeValue.Id),
                vendorAttributeValue);

            await UpdateValueLocalesAsync(vendorAttributeValue, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _vendorAttributeModelFactory.PrepareVendorAttributeValueModelAsync(model, vendorAttribute, vendorAttributeValue, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueDelete(int id)
    {
		_logger.LogDebug(">>> Entering ValueDelete(id={Id})", id);
        //try to get a vendor attribute value with the specified id
        var value = await _vendorAttributeService.GetAttributeValueByIdAsync(id)
            ?? throw new ArgumentException("No vendor attribute value found with the specified id", nameof(id));

        await _vendorAttributeService.DeleteAttributeValueAsync(value);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteVendorAttributeValue",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteVendorAttributeValue"), value.Id), value);

		_logger.LogDebug("<<< Exiting ValueDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion
}