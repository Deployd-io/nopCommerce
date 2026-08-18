using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Plugin.Widgets.FacebookPixel.Domain;
using Nop.Plugin.Widgets.FacebookPixel.Models;
using Nop.Plugin.Widgets.FacebookPixel.Services;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Widgets.FacebookPixel.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class FacebookPixelController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly FacebookPixelService _facebookPixelService;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly IWorkContext _workContext;
    protected readonly StoreInformationSettings _storeInformationSettings;

    #endregion

    #region Ctor

    public FacebookPixelController(FacebookPixelService facebookPixelService,
        IBaseAdminModelFactory baseAdminModelFactory,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IStoreContext storeContext,
        IStoreService storeService,
        IWorkContext workContext,
        StoreInformationSettings storeInformationSettings)
    {
		_logger.LogDebug(">>> Entering FacebookPixelController(facebookPixelService={FacebookPixelService},baseAdminModelFactory={BaseAdminModelFactory},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext},storeInformationSettings={StoreInformationSettings})", facebookPixelService, baseAdminModelFactory, genericAttributeService, localizationService, notificationService, permissionService, storeContext, storeService, workContext, storeInformationSettings);
        _facebookPixelService = facebookPixelService;
		_logger.LogDebug("FacebookPixelController(facebookPixelService={FacebookPixelService},baseAdminModelFactory={BaseAdminModelFactory},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _facebookPixelService → {_facebookPixelService}", facebookPixelService, baseAdminModelFactory, genericAttributeService, localizationService, notificationService, permissionService, storeContext, storeService, workContext, storeInformationSettings, _facebookPixelService);
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("FacebookPixelController(facebookPixelService={FacebookPixelService},baseAdminModelFactory={BaseAdminModelFactory},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _baseAdminModelFactory → {_baseAdminModelFactory}", facebookPixelService, baseAdminModelFactory, genericAttributeService, localizationService, notificationService, permissionService, storeContext, storeService, workContext, storeInformationSettings, _baseAdminModelFactory);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("FacebookPixelController(facebookPixelService={FacebookPixelService},baseAdminModelFactory={BaseAdminModelFactory},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _genericAttributeService → {_genericAttributeService}", facebookPixelService, baseAdminModelFactory, genericAttributeService, localizationService, notificationService, permissionService, storeContext, storeService, workContext, storeInformationSettings, _genericAttributeService);
        _localizationService = localizationService;
		_logger.LogDebug("FacebookPixelController(facebookPixelService={FacebookPixelService},baseAdminModelFactory={BaseAdminModelFactory},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _localizationService → {_localizationService}", facebookPixelService, baseAdminModelFactory, genericAttributeService, localizationService, notificationService, permissionService, storeContext, storeService, workContext, storeInformationSettings, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("FacebookPixelController(facebookPixelService={FacebookPixelService},baseAdminModelFactory={BaseAdminModelFactory},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _notificationService → {_notificationService}", facebookPixelService, baseAdminModelFactory, genericAttributeService, localizationService, notificationService, permissionService, storeContext, storeService, workContext, storeInformationSettings, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("FacebookPixelController(facebookPixelService={FacebookPixelService},baseAdminModelFactory={BaseAdminModelFactory},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _permissionService → {_permissionService}", facebookPixelService, baseAdminModelFactory, genericAttributeService, localizationService, notificationService, permissionService, storeContext, storeService, workContext, storeInformationSettings, _permissionService);
        _storeContext = storeContext;
		_logger.LogDebug("FacebookPixelController(facebookPixelService={FacebookPixelService},baseAdminModelFactory={BaseAdminModelFactory},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _storeContext → {_storeContext}", facebookPixelService, baseAdminModelFactory, genericAttributeService, localizationService, notificationService, permissionService, storeContext, storeService, workContext, storeInformationSettings, _storeContext);
        _storeService = storeService;
		_logger.LogDebug("FacebookPixelController(facebookPixelService={FacebookPixelService},baseAdminModelFactory={BaseAdminModelFactory},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _storeService → {_storeService}", facebookPixelService, baseAdminModelFactory, genericAttributeService, localizationService, notificationService, permissionService, storeContext, storeService, workContext, storeInformationSettings, _storeService);
        _workContext = workContext;
		_logger.LogDebug("FacebookPixelController(facebookPixelService={FacebookPixelService},baseAdminModelFactory={BaseAdminModelFactory},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _workContext → {_workContext}", facebookPixelService, baseAdminModelFactory, genericAttributeService, localizationService, notificationService, permissionService, storeContext, storeService, workContext, storeInformationSettings, _workContext);
        _storeInformationSettings = storeInformationSettings;
		_logger.LogDebug("<<< Exiting FacebookPixelController(facebookPixelService={FacebookPixelService},baseAdminModelFactory={BaseAdminModelFactory},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext},storeInformationSettings={StoreInformationSettings})", facebookPixelService, baseAdminModelFactory, genericAttributeService, localizationService, notificationService, permissionService, storeContext, storeService, workContext, storeInformationSettings);
		_logger.LogDebug("FacebookPixelController(facebookPixelService={FacebookPixelService},baseAdminModelFactory={BaseAdminModelFactory},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},storeContext={StoreContext},storeService={StoreService},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _storeInformationSettings → {_storeInformationSettings}", facebookPixelService, baseAdminModelFactory, genericAttributeService, localizationService, notificationService, permissionService, storeContext, storeService, workContext, storeInformationSettings, _storeInformationSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare list of available public widget zones
    /// </summary>
    /// <returns>Available widget zones</returns>
    public IList<SelectListItem> PreparePublicWidgetZones()
    {
		_logger.LogDebug(">>> Entering PreparePublicWidgetZones()");
		_logger.LogDebug("<<< Exiting PreparePublicWidgetZones()");
        return typeof(PublicWidgetZones)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(property => property.PropertyType == typeof(string))
            .Select(property => property.GetValue(null) is string value ? new SelectListItem(value, value) : null)
            .Where(item => item != null)
            .ToList();
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public async Task<IActionResult> Configure()
    {
		_logger.LogDebug(">>> Entering Configure()");
        //prepare plugin configuration model
        var model = new ConfigurationModel();
		_logger.LogDebug("Configure(): model → {Model}", model);
        await _baseAdminModelFactory.PrepareStoresAsync(model.FacebookPixelSearchModel.AvailableStores);
        model.FacebookPixelSearchModel.HideStoresList = model.FacebookPixelSearchModel.AvailableStores.SelectionIsNotPossible();
        model.FacebookPixelSearchModel.HideSearchBlock = await _genericAttributeService
            .GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), FacebookPixelDefaults.HideSearchBlockAttribute);
        model.FacebookPixelSearchModel.SetGridPageSize();
        model.HideList = !(await _facebookPixelService.GetPagedConfigurationsAsync()).Any();

		_logger.LogDebug("<<< Exiting Configure()");
        return View("~/Plugins/Widgets.FacebookPixel/Views/Configuration/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> List(FacebookPixelSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering List(searchModel={SearchModel})", searchModel);
        var configurations = await _facebookPixelService.GetPagedConfigurationsAsync(searchModel.StoreId, searchModel.Page - 1, searchModel.PageSize);
        var model = await new FacebookPixelListModel().PrepareToGridAsync(searchModel, configurations, () =>
        {
			_logger.LogDebug("List(searchModel={SearchModel}): configurations → {Configurations}", searchModel, configurations);
            //fill in model values from the configuration
            return configurations.SelectAwait(async configuration => new FacebookPixelModel
            {
                Id = configuration.Id,
                PixelId = configuration.PixelId,
                AccessToken = configuration.AccessToken,
                PixelScriptEnabled = configuration.PixelScriptEnabled,
                ConversionsApiEnabled = configuration.ConversionsApiEnabled,
                StoreId = configuration.StoreId,
                StoreName = (await _storeService.GetStoreByIdAsync(configuration.StoreId))?.Name
            });
        });

		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogDebug(">>> Entering Create()");
        //set default values
        var model = new FacebookPixelModel
        {
            TrackAddToCart = true,
            TrackPageView = true,
            TrackPurchase = true,
            TrackViewContent = true,
            UseAdvancedMatching = true
        };

        //prepare other model properties
        await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores, false);
        model.HideStoresList = model.AvailableStores.SelectionIsNotPossible();

		_logger.LogDebug("<<< Exiting Create()");
		_logger.LogDebug("<<< Exiting Create(model={Model})", model);
        return View("~/Plugins/Widgets.FacebookPixel/Views/Configuration/Create.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> Create(FacebookPixelModel model)
    {
        if (ModelState.IsValid)
        {
            //save configuration
            var store = await _storeContext.GetCurrentStoreAsync();
            model.StoreId = model.StoreId > 0 ? model.StoreId : store.Id;
            var configuration = model.ToEntity<FacebookPixelConfiguration>();
            await _facebookPixelService.InsertConfigurationAsync(configuration);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return RedirectToAction("Configure", "FacebookPixel");
        }

        //if we got this far, something failed, redisplay form
        await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores, false);
        model.HideStoresList = model.AvailableStores.SelectionIsNotPossible();

        return View("~/Plugins/Widgets.FacebookPixel/Views/Configuration/Create.cshtml", model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        var configuration = await _facebookPixelService.GetConfigurationByIdAsync(id);
        if (configuration == null)
			_logger.LogDebug("Edit(id={Id}): configuration → {Configuration}", id, configuration);
            return RedirectToAction("Configure", "FacebookPixel");

        //fill in model values from the configuration
        var model = configuration.ToModel<FacebookPixelModel>();
		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);

        //prepare other model properties
        await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores, false);
        model.HideStoresList = model.AvailableStores.SelectionIsNotPossible();
        model.HideCustomEventsSearch = !(await _facebookPixelService.GetCustomEventsAsync(configuration.Id)).Any();
        model.CustomEventSearchModel.ConfigurationId = configuration.Id;
        model.CustomEventSearchModel.AddCustomEvent.AvailableWidgetZones = PreparePublicWidgetZones();
        model.CustomEventSearchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
		_logger.LogDebug("<<< Exiting Edit(model={Model})", model);
        return View("~/Plugins/Widgets.FacebookPixel/Views/Configuration/Edit.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> Edit(FacebookPixelModel model)
    {
        var configuration = await _facebookPixelService.GetConfigurationByIdAsync(model.Id);
        if (configuration == null)
            return RedirectToAction("Configure", "FacebookPixel");

        if (ModelState.IsValid)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            //save configuration
            var customEvents = configuration.CustomEvents;
            model.StoreId = model.StoreId > 0 ? model.StoreId : store.Id;
            configuration = model.ToEntity<FacebookPixelConfiguration>();
            configuration.CustomEvents = customEvents;
            await _facebookPixelService.UpdateConfigurationAsync(configuration);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return RedirectToAction("Configure", "FacebookPixel");
        }

        //if we got this far, something failed, redisplay form
        await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores, false);
        model.HideStoresList = model.AvailableStores.SelectionIsNotPossible();
        model.HideCustomEventsSearch = !(await _facebookPixelService.GetCustomEventsAsync(configuration.Id)).Any();
        model.CustomEventSearchModel.ConfigurationId = configuration.Id;
        model.CustomEventSearchModel.AddCustomEvent.AvailableWidgetZones = PreparePublicWidgetZones();
        model.CustomEventSearchModel.SetGridPageSize();

        return View("~/Plugins/Widgets.FacebookPixel/Views/Configuration/Edit.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        var configuration = await _facebookPixelService.GetConfigurationByIdAsync(id);
        if (configuration == null)
			_logger.LogDebug("Delete(id={Id}): configuration → {Configuration}", id, configuration);
            return RedirectToAction("Configure", "FacebookPixel");

        //delete configuration
        await _facebookPixelService.DeleteConfigurationAsync(configuration);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

		_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("Configure", "FacebookPixel");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> CustomEventList(CustomEventSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering CustomEventList(searchModel={SearchModel})", searchModel);
        var configuration = await _facebookPixelService.GetConfigurationByIdAsync(searchModel.ConfigurationId)
            ?? throw new ArgumentException("No configuration found with the specified id", nameof(searchModel.ConfigurationId));

        var customEvents = (await _facebookPixelService.GetCustomEventsAsync(configuration.Id, searchModel.WidgetZone)).ToPagedList(searchModel);
		_logger.LogDebug("CustomEventList(searchModel={SearchModel}): customEvents → {CustomEvents}", searchModel, customEvents);
        var model = new CustomEventListModel().PrepareToGrid(searchModel, customEvents, () =>
        {
            //fill in model values from the configuration
            return customEvents.Select(customEvent => new CustomEventModel
            {
                ConfigurationId = configuration.Id,
                EventName = customEvent.EventName,
                WidgetZonesName = string.Join(", ", customEvent.WidgetZones)
            });
        });

		_logger.LogDebug("<<< Exiting CustomEventList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    //ValidateAttribute is used to force model validation
    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> CustomEventAdd(int configurationId, [Validate] CustomEventModel model)
    {
		_logger.LogDebug(">>> Entering CustomEventAdd(configurationId={ConfigurationId},model={Model})", configurationId, model);
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //save custom events configuration
        await _facebookPixelService.SaveCustomEventsAsync(configurationId, model.EventName, model.WidgetZones);

		_logger.LogDebug("<<< Exiting CustomEventAdd(configurationId={ConfigurationId},model={Model})", configurationId, model);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> CustomEventDelete(int configurationId, string id)
    {
		_logger.LogDebug(">>> Entering CustomEventDelete(configurationId={ConfigurationId},id={Id})", configurationId, id);
        //save custom events configuration
        var eventName = id;
        await _facebookPixelService.SaveCustomEventsAsync(configurationId, eventName, null);

		_logger.LogDebug("<<< Exiting CustomEventDelete(configurationId={ConfigurationId},id={Id})", configurationId, id);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public async Task<IActionResult> CookieSettingsWarning(bool disableForUsersNotAcceptingCookieConsent)
    {
		_logger.LogDebug(">>> Entering CookieSettingsWarning(disableForUsersNotAcceptingCookieConsent=<redacted>)");
        if (!disableForUsersNotAcceptingCookieConsent || _storeInformationSettings.DisplayEuCookieLawWarning)
            return Json(new { Result = string.Empty });

        //display a warning when "DisplayEuCookieLawWarning" setting is disabled, 
        //but merchant want to disable Facebook Pixel for users not accepting Cookie Consent
        var url = Url.Action("GeneralCommon", "Setting");
		_logger.LogDebug("CookieSettingsWarning(disableForUsersNotAcceptingCookieConsent=<redacted>): url → {Url}", url);
        var warning = string.Format(await _localizationService.GetResourceAsync("Plugins.Widgets.FacebookPixel.Configuration.CookieSettingsWarning"), url);
		_logger.LogDebug("CookieSettingsWarning(disableForUsersNotAcceptingCookieConsent=<redacted>): warning → {Warning}", warning);
		_logger.LogDebug("<<< Exiting CookieSettingsWarning(disableForUsersNotAcceptingCookieConsent=<redacted>)");
        return Json(new { Result = warning });
    }

    #endregion
}