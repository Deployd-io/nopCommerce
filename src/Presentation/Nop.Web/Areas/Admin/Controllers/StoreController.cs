using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Stores;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class StoreController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreModelFactory _storeModelFactory;
    protected readonly IStoreService _storeService;
    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public StoreController(ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        IStoreModelFactory storeModelFactory,
        IStoreService storeService,
        IWebHelper webHelper)
    {
		_logger.LogError(">>> Entering StoreController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeModelFactory={StoreModelFactory},storeService={StoreService},webHelper={WebHelper})", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, settingService, storeModelFactory, storeService, webHelper);
        _customerActivityService = customerActivityService;
		_logger.LogWarning("<<< Exiting StoreController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeModelFactory={StoreModelFactory},storeService={StoreService},webHelper={WebHelper})", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, settingService, storeModelFactory, storeService, webHelper);
		_logger.LogDebug("StoreController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeModelFactory={StoreModelFactory},storeService={StoreService},webHelper={WebHelper}): _customerActivityService → {_customerActivityService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, settingService, storeModelFactory, storeService, webHelper, _customerActivityService);
        _localizationService = localizationService;
		_logger.LogDebug("StoreController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeModelFactory={StoreModelFactory},storeService={StoreService},webHelper={WebHelper}): _localizationService → {_localizationService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, settingService, storeModelFactory, storeService, webHelper, _localizationService);
        _localizedEntityService = localizedEntityService;
		_logger.LogDebug("StoreController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeModelFactory={StoreModelFactory},storeService={StoreService},webHelper={WebHelper}): _localizedEntityService → {_localizedEntityService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, settingService, storeModelFactory, storeService, webHelper, _localizedEntityService);
        _notificationService = notificationService;
		_logger.LogDebug("StoreController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeModelFactory={StoreModelFactory},storeService={StoreService},webHelper={WebHelper}): _notificationService → {_notificationService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, settingService, storeModelFactory, storeService, webHelper, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("StoreController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeModelFactory={StoreModelFactory},storeService={StoreService},webHelper={WebHelper}): _permissionService → {_permissionService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, settingService, storeModelFactory, storeService, webHelper, _permissionService);
        _settingService = settingService;
		_logger.LogDebug("StoreController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeModelFactory={StoreModelFactory},storeService={StoreService},webHelper={WebHelper}): _settingService → {_settingService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, settingService, storeModelFactory, storeService, webHelper, _settingService);
        _storeModelFactory = storeModelFactory;
		_logger.LogDebug("StoreController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeModelFactory={StoreModelFactory},storeService={StoreService},webHelper={WebHelper}): _storeModelFactory → {_storeModelFactory}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, settingService, storeModelFactory, storeService, webHelper, _storeModelFactory);
        _storeService = storeService;
		_logger.LogDebug("StoreController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeModelFactory={StoreModelFactory},storeService={StoreService},webHelper={WebHelper}): _storeService → {_storeService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, settingService, storeModelFactory, storeService, webHelper, _storeService);
        _webHelper = webHelper;
		_logger.LogDebug("StoreController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},storeModelFactory={StoreModelFactory},storeService={StoreService},webHelper={WebHelper}): _webHelper → {_webHelper}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, settingService, storeModelFactory, storeService, webHelper, _webHelper);

    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(Store store, StoreModel model)
    {
		_logger.LogError(">>> Entering UpdateLocalesAsync(store={Store},model={Model})", store, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(store,
                x => x.Name,
                localized.Name,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(store,
                x => x.DefaultTitle,
                localized.DefaultTitle,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(store,
                x => x.DefaultMetaDescription,
                localized.DefaultMetaDescription,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(store,
                x => x.DefaultMetaKeywords,
                localized.DefaultMetaKeywords,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(store,
                x => x.HomepageDescription,
                localized.HomepageDescription,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(store,
                x => x.HomepageTitle,
                localized.HomepageTitle,
                localized.LanguageId);
        }
		_logger.LogWarning("<<< Exiting UpdateLocalesAsync(store={Store},model={Model})", store, model);
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_STORES)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogError(">>> Entering List()");
        //prepare model
        var model = await _storeModelFactory.PrepareStoreSearchModelAsync(new StoreSearchModel());
		_logger.LogError("List(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting List()");
		_logger.LogWarning("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_STORES)]
    public virtual async Task<IActionResult> List(StoreSearchModel searchModel)
    {
        //prepare model
        var model = await _storeModelFactory.PrepareStoreListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_STORES)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogError(">>> Entering Create()");
        //prepare model
        var model = await _storeModelFactory.PrepareStoreModelAsync(new StoreModel(), null);
		_logger.LogError("Create(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting Create()");
		_logger.LogWarning("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_STORES)]
    public virtual async Task<IActionResult> Create(StoreModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var store = model.ToEntity<Store>();

            //ensure we have "/" at the end
            if (!store.Url.EndsWith("/"))
                store.Url += "/";

            await _storeService.InsertStoreAsync(store);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewStore",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewStore"), store.Id), store);

            //locales
            await UpdateLocalesAsync(store, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Stores.Added"));

            return continueEditing ? RedirectToAction("Edit", new { id = store.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _storeModelFactory.PrepareStoreModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpsRequirement(ignore: true)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_STORES)]
    public virtual async Task<IActionResult> SetStoreSslByCurrentRequestScheme(int id)
    {
		_logger.LogError(">>> Entering SetStoreSslByCurrentRequestScheme(id={Id})", id);
        //try to get a store with the specified id
        var store = await _storeService.GetStoreByIdAsync(id);
		_logger.LogError("SetStoreSslByCurrentRequestScheme(id={Id}): store → {Store}", id, store);
        if (store == null)
            return RedirectToAction("List");

        var value = _webHelper.IsCurrentConnectionSecured();
		_logger.LogError("SetStoreSslByCurrentRequestScheme(id={Id}): value → {Value}", id, value);

        if (store.SslEnabled != value)
        {
            store.SslEnabled = value;
			_logger.LogDebug("SetStoreSslByCurrentRequestScheme(id={Id}): store.SslEnabled → {Arg1}", id, store.SslEnabled);
            await _storeService.UpdateStoreAsync(store);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Stores.Ssl.Updated"));
        }

		_logger.LogWarning("<<< Exiting SetStoreSslByCurrentRequestScheme(id={Id})", id);
        return RedirectToAction("Edit", new { id = id });
    }

    [HttpsRequirement(ignore: true)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_STORES)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogError(">>> Entering Edit(id={Id})", id);
        //try to get a store with the specified id
        var store = await _storeService.GetStoreByIdAsync(id);
		_logger.LogError("Edit(id={Id}): store → {Store}", id, store);
        if (store == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _storeModelFactory.PrepareStoreModelAsync(null, store);
		_logger.LogError("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting Edit(id={Id})", id);
		_logger.LogWarning("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_STORES)]
    public virtual async Task<IActionResult> Edit(StoreModel model, bool continueEditing)
    {
        //try to get a store with the specified id
        var store = await _storeService.GetStoreByIdAsync(model.Id);
        if (store == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            store = model.ToEntity(store);

            //ensure we have "/" at the end
            if (!store.Url.EndsWith("/"))
                store.Url += "/";

            await _storeService.UpdateStoreAsync(store);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditStore",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditStore"), store.Id), store);

            //locales
            await UpdateLocalesAsync(store, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Stores.Updated"));

            return continueEditing ? RedirectToAction("Edit", new { id = store.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _storeModelFactory.PrepareStoreModelAsync(model, store, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_STORES)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogError(">>> Entering Delete(id={Id})", id);
        //try to get a store with the specified id
        var store = await _storeService.GetStoreByIdAsync(id);
		_logger.LogError("Delete(id={Id}): store → {Store}", id, store);
        if (store == null)
            return RedirectToAction("List");

        try
        {
            await _storeService.DeleteStoreAsync(store);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteStore",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteStore"), store.Id), store);

            //when we delete a store we should also ensure that all "per store" settings will also be deleted
            var settingsToDelete = (await _settingService
                    .GetAllSettingsAsync())
                .Where(s => s.StoreId == id)
                .ToList();
            await _settingService.DeleteSettingsAsync(settingsToDelete);

            //when we had two stores and now have only one store, we also should delete all "per store" settings
            var allStores = await _storeService.GetAllStoresAsync();
			_logger.LogError("Delete(id={Id}): allStores → {AllStores}", id, allStores);
            if (allStores.Count == 1)
            {
                settingsToDelete = (await _settingService
                        .GetAllSettingsAsync())
                    .Where(s => s.StoreId == allStores[0].Id)
                    .ToList();
                await _settingService.DeleteSettingsAsync(settingsToDelete);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Stores.Deleted"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in Delete(id={Id}): {Arg1}", id, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogWarning("<<< Exiting Delete(id={Id})", id);
            return RedirectToAction("Edit", new { id = store.Id });
        }
    }

    #endregion
}