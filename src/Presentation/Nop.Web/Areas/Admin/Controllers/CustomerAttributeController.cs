using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Services.Attributes;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class CustomerAttributeController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAttributeService<CustomerAttribute, CustomerAttributeValue> _customerAttributeService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerAttributeModelFactory _customerAttributeModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;

    #endregion

    #region Ctor

    public CustomerAttributeController(IAttributeService<CustomerAttribute, CustomerAttributeValue> customerAttributeService,
        ICustomerActivityService customerActivityService,
        ICustomerAttributeModelFactory customerAttributeModelFactory,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IPermissionService permissionService)
    {
		_logger.LogError(">>> Entering CustomerAttributeController(customerAttributeService={CustomerAttributeService},customerActivityService={CustomerActivityService},customerAttributeModelFactory={CustomerAttributeModelFactory},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService})", customerAttributeService, customerActivityService, customerAttributeModelFactory, localizationService, localizedEntityService, notificationService, permissionService);
        _customerAttributeService = customerAttributeService;
        _customerActivityService = customerActivityService;
		_logger.LogDebug("CustomerAttributeController(customerAttributeService={CustomerAttributeService},customerActivityService={CustomerActivityService},customerAttributeModelFactory={CustomerAttributeModelFactory},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService}): _customerActivityService → {_customerActivityService}", customerAttributeService, customerActivityService, customerAttributeModelFactory, localizationService, localizedEntityService, notificationService, permissionService, _customerActivityService);
		_logger.LogDebug("CustomerAttributeController(customerAttributeService={CustomerAttributeService},customerActivityService={CustomerActivityService},customerAttributeModelFactory={CustomerAttributeModelFactory},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService}): _customerAttributeService → {_customerAttributeService}", customerAttributeService, customerActivityService, customerAttributeModelFactory, localizationService, localizedEntityService, notificationService, permissionService, _customerAttributeService);
    _logger.LogWarning("<<< Exiting CustomerAttributeController(customerAttributeService={CustomerAttributeService},customerActivityService={CustomerActivityService},customerAttributeModelFactory={CustomerAttributeModelFactory},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService})", customerAttributeService, customerActivityService, customerAttributeModelFactory, localizationService, localizedEntityService, notificationService, permissionService);
        _customerAttributeModelFactory = customerAttributeModelFactory;
		_logger.LogDebug("CustomerAttributeController(customerAttributeService={CustomerAttributeService},customerActivityService={CustomerActivityService},customerAttributeModelFactory={CustomerAttributeModelFactory},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService}): _customerAttributeModelFactory → {_customerAttributeModelFactory}", customerAttributeService, customerActivityService, customerAttributeModelFactory, localizationService, localizedEntityService, notificationService, permissionService, _customerAttributeModelFactory);
        _localizationService = localizationService;
		_logger.LogDebug("CustomerAttributeController(customerAttributeService={CustomerAttributeService},customerActivityService={CustomerActivityService},customerAttributeModelFactory={CustomerAttributeModelFactory},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService}): _localizationService → {_localizationService}", customerAttributeService, customerActivityService, customerAttributeModelFactory, localizationService, localizedEntityService, notificationService, permissionService, _localizationService);
        _localizedEntityService = localizedEntityService;
		_logger.LogDebug("CustomerAttributeController(customerAttributeService={CustomerAttributeService},customerActivityService={CustomerActivityService},customerAttributeModelFactory={CustomerAttributeModelFactory},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService}): _localizedEntityService → {_localizedEntityService}", customerAttributeService, customerActivityService, customerAttributeModelFactory, localizationService, localizedEntityService, notificationService, permissionService, _localizedEntityService);
        _notificationService = notificationService;
		_logger.LogDebug("CustomerAttributeController(customerAttributeService={CustomerAttributeService},customerActivityService={CustomerActivityService},customerAttributeModelFactory={CustomerAttributeModelFactory},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService}): _notificationService → {_notificationService}", customerAttributeService, customerActivityService, customerAttributeModelFactory, localizationService, localizedEntityService, notificationService, permissionService, _notificationService);
        _permissionService = permissionService;
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateAttributeLocalesAsync(CustomerAttribute customerAttribute, CustomerAttributeModel model)
    {
		_logger.LogError(">>> Entering UpdateAttributeLocalesAsync(customerAttribute={CustomerAttribute},model={Model})", customerAttribute, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(customerAttribute,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
		_logger.LogWarning("<<< Exiting UpdateAttributeLocalesAsync(customerAttribute={CustomerAttribute},model={Model})", customerAttribute, model);
    }

    protected virtual async Task UpdateValueLocalesAsync(CustomerAttributeValue customerAttributeValue, CustomerAttributeValueModel model)
    {
		_logger.LogError(">>> Entering UpdateValueLocalesAsync(customerAttributeValue={CustomerAttributeValue},model={Model})", customerAttributeValue, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(customerAttributeValue,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
		_logger.LogWarning("<<< Exiting UpdateValueLocalesAsync(customerAttributeValue={CustomerAttributeValue},model={Model})", customerAttributeValue, model);
    }

    #endregion

    #region Customer attributes

    public virtual IActionResult Index()
    {
		_logger.LogError(">>> Entering Index()");
        _logger.LogWarning("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual IActionResult List()
    {
		_logger.LogError(">>> Entering List()");
        //select an appropriate card
        SaveSelectedCardName("customersettings-customerformfields");

        //we just redirect a user to the customer settings page
		_logger.LogWarning("<<< Exiting List()");
		_logger.LogWarning("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return RedirectToAction("CustomerUser", "Setting");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> List(CustomerAttributeSearchModel searchModel)
    {
        //prepare model
        var model = await _customerAttributeModelFactory.PrepareCustomerAttributeListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogError(">>> Entering Create()");
        //prepare model
        var model = await _customerAttributeModelFactory.PrepareCustomerAttributeModelAsync(new CustomerAttributeModel(), null);
		_logger.LogError("Create(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting Create()");
		_logger.LogWarning("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Create(CustomerAttributeModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var customerAttribute = model.ToEntity<CustomerAttribute>();
            await _customerAttributeService.InsertAttributeAsync(customerAttribute);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewCustomerAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCustomerAttribute"), customerAttribute.Id),
                customerAttribute);

            //locales
            await UpdateAttributeLocalesAsync(customerAttribute, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.CustomerAttributes.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = customerAttribute.Id });
        }

        //prepare model
        model = await _customerAttributeModelFactory.PrepareCustomerAttributeModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogError(">>> Entering Edit(id={Id})", id);
        //try to get a customer attribute with the specified id
        var customerAttribute = await _customerAttributeService.GetAttributeByIdAsync(id);
		_logger.LogError("Edit(id={Id}): customerAttribute → {CustomerAttribute}", id, customerAttribute);
        if (customerAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _customerAttributeModelFactory.PrepareCustomerAttributeModelAsync(null, customerAttribute);
		_logger.LogError("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting Edit(id={Id})", id);
		_logger.LogWarning("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Edit(CustomerAttributeModel model, bool continueEditing)
    {
        var customerAttribute = await _customerAttributeService.GetAttributeByIdAsync(model.Id);
        if (customerAttribute == null)
            //no customer attribute found with the specified id
            return RedirectToAction("List");

        if (!ModelState.IsValid)
            //if we got this far, something failed, redisplay form
            return View(model);

        customerAttribute = model.ToEntity(customerAttribute);
        await _customerAttributeService.UpdateAttributeAsync(customerAttribute);

        //activity log
        await _customerActivityService.InsertActivityAsync("EditCustomerAttribute",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCustomerAttribute"), customerAttribute.Id),
            customerAttribute);

        //locales
        await UpdateAttributeLocalesAsync(customerAttribute, model);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.CustomerAttributes.Updated"));

        if (!continueEditing)
            return RedirectToAction("List");

        return RedirectToAction("Edit", new { id = customerAttribute.Id });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogError(">>> Entering Delete(id={Id})", id);
        var customerAttribute = await _customerAttributeService.GetAttributeByIdAsync(id);
        await _customerAttributeService.DeleteAttributeAsync(customerAttribute);
		_logger.LogError("Delete(id={Id}): customerAttribute → {CustomerAttribute}", id, customerAttribute);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteCustomerAttribute",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCustomerAttribute"), customerAttribute.Id),
            customerAttribute);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.CustomerAttributes.Deleted"));
		_logger.LogWarning("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    #endregion

    #region Customer attribute values

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueList(CustomerAttributeValueSearchModel searchModel)
    {
		_logger.LogError(">>> Entering ValueList(searchModel={SearchModel})", searchModel);
        //try to get a customer attribute with the specified id
        var customerAttribute = await _customerAttributeService.GetAttributeByIdAsync(searchModel.CustomerAttributeId)
            ?? throw new ArgumentException("No customer attribute found with the specified id");

        //prepare model
        var model = await _customerAttributeModelFactory.PrepareCustomerAttributeValueListModelAsync(searchModel, customerAttribute);
		_logger.LogError("ValueList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting ValueList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueCreatePopup(int customerAttributeId)
    {
		_logger.LogError(">>> Entering ValueCreatePopup(customerAttributeId={CustomerAttributeId})", customerAttributeId);
        //try to get a customer attribute with the specified id
        var customerAttribute = await _customerAttributeService.GetAttributeByIdAsync(customerAttributeId);
		_logger.LogError("ValueCreatePopup(customerAttributeId={CustomerAttributeId}): customerAttribute → {CustomerAttribute}", customerAttributeId, customerAttribute);
        if (customerAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _customerAttributeModelFactory
            .PrepareCustomerAttributeValueModelAsync(new CustomerAttributeValueModel(), customerAttribute, null);

		_logger.LogWarning("<<< Exiting ValueCreatePopup(customerAttributeId={CustomerAttributeId})", customerAttributeId);
		_logger.LogWarning("<<< Exiting ValueCreatePopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueCreatePopup(CustomerAttributeValueModel model)
    {
        //try to get a customer attribute with the specified id
        var customerAttribute = await _customerAttributeService.GetAttributeByIdAsync(model.AttributeId);
        if (customerAttribute == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            var cav = model.ToEntity<CustomerAttributeValue>();
            await _customerAttributeService.InsertAttributeValueAsync(cav);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewCustomerAttributeValue",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCustomerAttributeValue"), cav.Id), cav);

            await UpdateValueLocalesAsync(cav, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _customerAttributeModelFactory.PrepareCustomerAttributeValueModelAsync(model, customerAttribute, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueEditPopup(int id)
    {
		_logger.LogError(">>> Entering ValueEditPopup(id={Id})", id);
        //try to get a customer attribute value with the specified id
        var customerAttributeValue = await _customerAttributeService.GetAttributeValueByIdAsync(id);
		_logger.LogError("ValueEditPopup(id={Id}): customerAttributeValue → {CustomerAttributeValue}", id, customerAttributeValue);
        if (customerAttributeValue == null)
            return RedirectToAction("List");

        //try to get a customer attribute with the specified id
        var customerAttribute = await _customerAttributeService.GetAttributeByIdAsync(customerAttributeValue.AttributeId);
		_logger.LogError("ValueEditPopup(id={Id}): customerAttribute → {CustomerAttribute}", id, customerAttribute);
        if (customerAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _customerAttributeModelFactory.PrepareCustomerAttributeValueModelAsync(null, customerAttribute, customerAttributeValue);
		_logger.LogError("ValueEditPopup(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting ValueEditPopup(id={Id})", id);
		_logger.LogWarning("<<< Exiting ValueEditPopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueEditPopup(CustomerAttributeValueModel model)
    {
        //try to get a customer attribute value with the specified id
        var customerAttributeValue = await _customerAttributeService.GetAttributeValueByIdAsync(model.Id);
        if (customerAttributeValue == null)
            return RedirectToAction("List");

        //try to get a customer attribute with the specified id
        var customerAttribute = await _customerAttributeService.GetAttributeByIdAsync(customerAttributeValue.AttributeId);
        if (customerAttribute == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            customerAttributeValue = model.ToEntity(customerAttributeValue);
            await _customerAttributeService.UpdateAttributeValueAsync(customerAttributeValue);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditCustomerAttributeValue",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCustomerAttributeValue"), customerAttributeValue.Id),
                customerAttributeValue);

            await UpdateValueLocalesAsync(customerAttributeValue, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _customerAttributeModelFactory.PrepareCustomerAttributeValueModelAsync(model, customerAttribute, customerAttributeValue, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueDelete(int id)
    {
		_logger.LogError(">>> Entering ValueDelete(id={Id})", id);
        //try to get a customer attribute value with the specified id
        var customerAttributeValue = await _customerAttributeService.GetAttributeValueByIdAsync(id)
            ?? throw new ArgumentException("No customer attribute value found with the specified id", nameof(id));

        await _customerAttributeService.DeleteAttributeValueAsync(customerAttributeValue);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteCustomerAttributeValue",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCustomerAttributeValue"), customerAttributeValue.Id),
            customerAttributeValue);

		_logger.LogWarning("<<< Exiting ValueDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion
}