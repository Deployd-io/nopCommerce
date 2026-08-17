using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class CustomerRoleController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerRoleModelFactory _customerRoleModelFactory;
    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IProductService _productService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public CustomerRoleController(ICustomerActivityService customerActivityService,
        ICustomerRoleModelFactory customerRoleModelFactory,
        ICustomerService customerService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IProductService productService,
        IWorkContext workContext)
    {
		_logger.LogDebug(">>> Entering CustomerRoleController(customerActivityService={CustomerActivityService},customerRoleModelFactory={CustomerRoleModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext})", customerActivityService, customerRoleModelFactory, customerService, localizationService, notificationService, permissionService, productService, workContext);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("CustomerRoleController(customerActivityService={CustomerActivityService},customerRoleModelFactory={CustomerRoleModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _customerActivityService → {_customerActivityService}", customerActivityService, customerRoleModelFactory, customerService, localizationService, notificationService, permissionService, productService, workContext, _customerActivityService);
        _customerRoleModelFactory = customerRoleModelFactory;
		_logger.LogDebug("CustomerRoleController(customerActivityService={CustomerActivityService},customerRoleModelFactory={CustomerRoleModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _customerRoleModelFactory → {_customerRoleModelFactory}", customerActivityService, customerRoleModelFactory, customerService, localizationService, notificationService, permissionService, productService, workContext, _customerRoleModelFactory);
        _customerService = customerService;
		_logger.LogDebug("CustomerRoleController(customerActivityService={CustomerActivityService},customerRoleModelFactory={CustomerRoleModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _customerService → {_customerService}", customerActivityService, customerRoleModelFactory, customerService, localizationService, notificationService, permissionService, productService, workContext, _customerService);
        _localizationService = localizationService;
		_logger.LogDebug("CustomerRoleController(customerActivityService={CustomerActivityService},customerRoleModelFactory={CustomerRoleModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _localizationService → {_localizationService}", customerActivityService, customerRoleModelFactory, customerService, localizationService, notificationService, permissionService, productService, workContext, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("CustomerRoleController(customerActivityService={CustomerActivityService},customerRoleModelFactory={CustomerRoleModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _notificationService → {_notificationService}", customerActivityService, customerRoleModelFactory, customerService, localizationService, notificationService, permissionService, productService, workContext, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("CustomerRoleController(customerActivityService={CustomerActivityService},customerRoleModelFactory={CustomerRoleModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _permissionService → {_permissionService}", customerActivityService, customerRoleModelFactory, customerService, localizationService, notificationService, permissionService, productService, workContext, _permissionService);
        _productService = productService;
		_logger.LogDebug("CustomerRoleController(customerActivityService={CustomerActivityService},customerRoleModelFactory={CustomerRoleModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _productService → {_productService}", customerActivityService, customerRoleModelFactory, customerService, localizationService, notificationService, permissionService, productService, workContext, _productService);
        _workContext = workContext;
		_logger.LogDebug("<<< Exiting CustomerRoleController(customerActivityService={CustomerActivityService},customerRoleModelFactory={CustomerRoleModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext})", customerActivityService, customerRoleModelFactory, customerService, localizationService, notificationService, permissionService, productService, workContext);
		_logger.LogDebug("CustomerRoleController(customerActivityService={CustomerActivityService},customerRoleModelFactory={CustomerRoleModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _workContext → {_workContext}", customerActivityService, customerRoleModelFactory, customerService, localizationService, notificationService, permissionService, productService, workContext, _workContext);
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
		_logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMER_ROLES_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _customerRoleModelFactory.PrepareCustomerRoleSearchModelAsync(new CustomerRoleSearchModel());
		_logger.LogDebug("List(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMER_ROLES_VIEW)]
    public virtual async Task<IActionResult> List(CustomerRoleSearchModel searchModel)
    {
        //prepare model
        var model = await _customerRoleModelFactory.PrepareCustomerRoleListModelAsync(searchModel);

		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMER_ROLES_CREATE_EDIT_DELETE)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = await _customerRoleModelFactory.PrepareCustomerRoleModelAsync(new CustomerRoleModel(), null);
		_logger.LogDebug("Create(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting Create()");
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Customers.CUSTOMER_ROLES_CREATE_EDIT_DELETE)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> Create(CustomerRoleModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var customerRole = model.ToEntity<CustomerRole>();
            await _customerService.InsertCustomerRoleAsync(customerRole);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewCustomerRole",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCustomerRole"), customerRole.Name), customerRole);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.CustomerRoles.Added"));

            return continueEditing ? RedirectToAction("Edit", new { id = customerRole.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _customerRoleModelFactory.PrepareCustomerRoleModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
		_logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMER_ROLES_VIEW)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get a customer role with the specified id
        var customerRole = await _customerService.GetCustomerRoleByIdAsync(id);
		_logger.LogDebug("Edit(id={Id}): customerRole → {CustomerRole}", id, customerRole);
        if (customerRole == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _customerRoleModelFactory.PrepareCustomerRoleModelAsync(null, customerRole);
		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Customers.CUSTOMER_ROLES_CREATE_EDIT_DELETE)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> Edit(CustomerRoleModel model, bool continueEditing)
    {
        //try to get a customer role with the specified id
        var customerRole = await _customerService.GetCustomerRoleByIdAsync(model.Id);
        if (customerRole == null)
            return RedirectToAction("List");

        try
        {
            if (ModelState.IsValid)
            {
                if (customerRole.IsSystemRole && !model.Active)
                    throw new NopException(await _localizationService.GetResourceAsync("Admin.Customers.CustomerRoles.Fields.Active.CantEditSystem"));

                if (customerRole.IsSystemRole && !customerRole.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
                    throw new NopException(await _localizationService.GetResourceAsync("Admin.Customers.CustomerRoles.Fields.SystemName.CantEditSystem"));

                if (NopCustomerDefaults.RegisteredRoleName.Equals(customerRole.SystemName, StringComparison.InvariantCultureIgnoreCase) &&
                    model.PurchasedWithProductId > 0)
                    throw new NopException(await _localizationService.GetResourceAsync("Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct.Registered"));

                customerRole = model.ToEntity(customerRole);
                await _customerService.UpdateCustomerRoleAsync(customerRole);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditCustomerRole",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCustomerRole"), customerRole.Name), customerRole);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.CustomerRoles.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = customerRole.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _customerRoleModelFactory.PrepareCustomerRoleModelAsync(model, customerRole, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("Edit", new { id = customerRole.Id });
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMER_ROLES_CREATE_EDIT_DELETE)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a customer role with the specified id
        var customerRole = await _customerService.GetCustomerRoleByIdAsync(id);
		_logger.LogDebug("Delete(id={Id}): customerRole → {CustomerRole}", id, customerRole);
        if (customerRole == null)
            return RedirectToAction("List");

        try
        {
            await _customerService.DeleteCustomerRoleAsync(customerRole);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteCustomerRole",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCustomerRole"), customerRole.Name), customerRole);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.CustomerRoles.Deleted"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in Delete(id={Id}): {Arg1}", id, exc.Message);
            _notificationService.ErrorNotification(exc.Message);
			_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
            return RedirectToAction("Edit", new { id = customerRole.Id });
        }
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMER_ROLES_VIEW)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> AssociateProductToCustomerRolePopup()
    {
		_logger.LogDebug(">>> Entering AssociateProductToCustomerRolePopup()");
        //prepare model
        var model = await _customerRoleModelFactory.PrepareCustomerRoleProductSearchModelAsync(new CustomerRoleProductSearchModel());
		_logger.LogDebug("AssociateProductToCustomerRolePopup(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting AssociateProductToCustomerRolePopup()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMER_ROLES_VIEW)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> AssociateProductToCustomerRolePopupList(CustomerRoleProductSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering AssociateProductToCustomerRolePopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _customerRoleModelFactory.PrepareCustomerRoleProductListModelAsync(searchModel);
		_logger.LogDebug("AssociateProductToCustomerRolePopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting AssociateProductToCustomerRolePopupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Customers.CUSTOMER_ROLES_CREATE_EDIT_DELETE)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> AssociateProductToCustomerRolePopup([Bind(Prefix = nameof(AddProductToCustomerRoleModel))] AddProductToCustomerRoleModel model)
    {
        //try to get a product with the specified id
        var associatedProduct = await _productService.GetProductByIdAsync(model.AssociatedToProductId);
        if (associatedProduct == null)
            return Content("Cannot load a product");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && associatedProduct.VendorId != currentVendor.Id)
            return Content("This is not your product");

        ViewBag.RefreshPage = true;
        ViewBag.productId = associatedProduct.Id;
        ViewBag.productName = associatedProduct.Name;

        return View(new CustomerRoleProductSearchModel());
    }

    #endregion
}