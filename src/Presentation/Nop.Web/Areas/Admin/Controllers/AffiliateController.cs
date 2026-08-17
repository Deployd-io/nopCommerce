using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Services.Affiliates;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Affiliates;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class AffiliateController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly IAffiliateModelFactory _affiliateModelFactory;
    protected readonly IAffiliateService _affiliateService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;

    #endregion

    #region Ctor

    public AffiliateController(IAddressService addressService,
        IAffiliateModelFactory affiliateModelFactory,
        IAffiliateService affiliateService,
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService)
    {
		_logger.LogDebug(">>> Entering AffiliateController(addressService={AddressService},affiliateModelFactory={AffiliateModelFactory},affiliateService={AffiliateService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService})", addressService, affiliateModelFactory, affiliateService, customerActivityService, localizationService, notificationService, permissionService);
        _addressService = addressService;
		_logger.LogDebug("AffiliateController(addressService={AddressService},affiliateModelFactory={AffiliateModelFactory},affiliateService={AffiliateService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService}): _addressService → {_addressService}", addressService, affiliateModelFactory, affiliateService, customerActivityService, localizationService, notificationService, permissionService, _addressService);
        _affiliateModelFactory = affiliateModelFactory;
		_logger.LogDebug("AffiliateController(addressService={AddressService},affiliateModelFactory={AffiliateModelFactory},affiliateService={AffiliateService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService}): _affiliateModelFactory → {_affiliateModelFactory}", addressService, affiliateModelFactory, affiliateService, customerActivityService, localizationService, notificationService, permissionService, _affiliateModelFactory);
        _affiliateService = affiliateService;
		_logger.LogDebug("AffiliateController(addressService={AddressService},affiliateModelFactory={AffiliateModelFactory},affiliateService={AffiliateService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService}): _affiliateService → {_affiliateService}", addressService, affiliateModelFactory, affiliateService, customerActivityService, localizationService, notificationService, permissionService, _affiliateService);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("AffiliateController(addressService={AddressService},affiliateModelFactory={AffiliateModelFactory},affiliateService={AffiliateService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService}): _customerActivityService → {_customerActivityService}", addressService, affiliateModelFactory, affiliateService, customerActivityService, localizationService, notificationService, permissionService, _customerActivityService);
        _localizationService = localizationService;
		_logger.LogDebug("AffiliateController(addressService={AddressService},affiliateModelFactory={AffiliateModelFactory},affiliateService={AffiliateService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService}): _localizationService → {_localizationService}", addressService, affiliateModelFactory, affiliateService, customerActivityService, localizationService, notificationService, permissionService, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("AffiliateController(addressService={AddressService},affiliateModelFactory={AffiliateModelFactory},affiliateService={AffiliateService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService}): _notificationService → {_notificationService}", addressService, affiliateModelFactory, affiliateService, customerActivityService, localizationService, notificationService, permissionService, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("<<< Exiting AffiliateController(addressService={AddressService},affiliateModelFactory={AffiliateModelFactory},affiliateService={AffiliateService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService})", addressService, affiliateModelFactory, affiliateService, customerActivityService, localizationService, notificationService, permissionService);
		_logger.LogDebug("AffiliateController(addressService={AddressService},affiliateModelFactory={AffiliateModelFactory},affiliateService={AffiliateService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService}): _permissionService → {_permissionService}", addressService, affiliateModelFactory, affiliateService, customerActivityService, localizationService, notificationService, permissionService, _permissionService);
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
		_logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Promotions.AFFILIATES_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _affiliateModelFactory.PrepareAffiliateSearchModelAsync(new AffiliateSearchModel());
		_logger.LogDebug("List(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_VIEW)]
    public virtual async Task<IActionResult> List(AffiliateSearchModel searchModel)
    {
        //prepare model
        var model = await _affiliateModelFactory.PrepareAffiliateListModelAsync(searchModel);

		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = await _affiliateModelFactory.PrepareAffiliateModelAsync(new AffiliateModel(), null);
		_logger.LogDebug("Create(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting Create()");
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(AffiliateModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var address = model.Address.ToEntity<Address>();

            address.CreatedOnUtc = DateTime.UtcNow;

            //some validation
            if (address.CountryId == 0)
                address.CountryId = null;
            if (address.StateProvinceId == 0)
                address.StateProvinceId = null;

            await _addressService.InsertAddressAsync(address);

            var affiliate = model.ToEntity<Affiliate>();

            //validate friendly URL name
            var friendlyUrlName = await _affiliateService.ValidateFriendlyUrlNameAsync(affiliate, model.FriendlyUrlName);
            affiliate.FriendlyUrlName = friendlyUrlName;
            affiliate.AddressId = address.Id;

            await _affiliateService.InsertAffiliateAsync(affiliate);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewAffiliate",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewAffiliate"), affiliate.Id), affiliate);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Affiliates.Added"));

            return continueEditing ? RedirectToAction("Edit", new { id = affiliate.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _affiliateModelFactory.PrepareAffiliateModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
		_logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [CheckPermission(StandardPermission.Promotions.AFFILIATES_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get an affiliate with the specified id
        var affiliate = await _affiliateService.GetAffiliateByIdAsync(id);
		_logger.LogDebug("Edit(id={Id}): affiliate → {Affiliate}", id, affiliate);
        if (affiliate == null || affiliate.Deleted)
            return RedirectToAction("List");

        //prepare model
        var model = await _affiliateModelFactory.PrepareAffiliateModelAsync(null, affiliate);
		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(AffiliateModel model, bool continueEditing)
    {
        //try to get an affiliate with the specified id
        var affiliate = await _affiliateService.GetAffiliateByIdAsync(model.Id);
        if (affiliate == null || affiliate.Deleted)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            var address = await _addressService.GetAddressByIdAsync(affiliate.AddressId);
            address = model.Address.ToEntity(address);

            //some validation
            if (address.CountryId == 0)
                address.CountryId = null;
            if (address.StateProvinceId == 0)
                address.StateProvinceId = null;

            await _addressService.UpdateAddressAsync(address);

            affiliate = model.ToEntity(affiliate);

            //validate friendly URL name
            var friendlyUrlName = await _affiliateService.ValidateFriendlyUrlNameAsync(affiliate, model.FriendlyUrlName);
            affiliate.FriendlyUrlName = friendlyUrlName;
            affiliate.AddressId = address.Id;

            await _affiliateService.UpdateAffiliateAsync(affiliate);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditAffiliate",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditAffiliate"), affiliate.Id), affiliate);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Affiliates.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = affiliate.Id });
        }

        //prepare model
        model = await _affiliateModelFactory.PrepareAffiliateModelAsync(model, affiliate, true);

        //if we got this far, something failed, redisplay form
		_logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    //delete
    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get an affiliate with the specified id
        var affiliate = await _affiliateService.GetAffiliateByIdAsync(id);
		_logger.LogDebug("Delete(id={Id}): affiliate → {Affiliate}", id, affiliate);
        if (affiliate == null)
            return RedirectToAction("List");

        await _affiliateService.DeleteAffiliateAsync(affiliate);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteAffiliate",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteAffiliate"), affiliate.Id), affiliate);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Affiliates.Deleted"));

		_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_VIEW)]
    public virtual async Task<IActionResult> AffiliatedOrderListGrid(AffiliatedOrderSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering AffiliatedOrderListGrid(searchModel={SearchModel})", searchModel);
        //try to get an affiliate with the specified id
        var affiliate = await _affiliateService.GetAffiliateByIdAsync(searchModel.AffliateId)
                        ?? throw new ArgumentException("No affiliate found with the specified id");

        //prepare model
        var model = await _affiliateModelFactory.PrepareAffiliatedOrderListModelAsync(searchModel, affiliate);
		_logger.LogDebug("AffiliatedOrderListGrid(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting AffiliatedOrderListGrid(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_VIEW)]
    public virtual async Task<IActionResult> AffiliatedCustomerList(AffiliatedCustomerSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering AffiliatedCustomerList(searchModel={SearchModel})", searchModel);
        //try to get an affiliate with the specified id
        var affiliate = await _affiliateService.GetAffiliateByIdAsync(searchModel.AffliateId)
                        ?? throw new ArgumentException("No affiliate found with the specified id");

        //prepare model
        var model = await _affiliateModelFactory.PrepareAffiliatedCustomerListModelAsync(searchModel, affiliate);
		_logger.LogDebug("AffiliatedCustomerList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting AffiliatedCustomerList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion
}