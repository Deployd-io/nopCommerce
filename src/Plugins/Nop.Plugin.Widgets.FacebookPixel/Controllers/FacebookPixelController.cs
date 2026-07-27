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

namespace Nop.Plugin.Widgets.FacebookPixel.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class FacebookPixelController : BasePluginController
{
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
        _facebookPixelService = facebookPixelService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _storeContext = storeContext;
        _storeService = storeService;
        _workContext = workContext;
        _storeInformationSettings = storeInformationSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare list of available public widget zones
    /// </summary>
    /// <returns>Available widget zones</returns>
    public IList<SelectListItem> PreparePublicWidgetZones()
    {
		logger.LogDebug(">>> Entering PreparePublicWidgetZones()");
		logger.LogDebug("<<< Exiting PreparePublicWidgetZones()");
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
		logger.LogDebug(">>> Entering Configure()");
        //prepare plugin configuration model
        var model = new ConfigurationModel();
        await _baseAdminModelFactory.PrepareStoresAsync(model.FacebookPixelSearchModel.AvailableStores);
		logger.LogDebug("Configure(): model → {Model}", model);
        model.FacebookPixelSearchModel.HideStoresList = model.FacebookPixelSearchModel.AvailableStores.SelectionIsNotPossible();
        model.FacebookPixelSearchModel.HideSearchBlock = await _genericAttributeService
            .GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), FacebookPixelDefaults.HideSearchBlockAttribute);
        model.FacebookPixelSearchModel.SetGridPageSize();
        model.HideList = !(await _facebookPixelService.GetPagedConfigurationsAsync()).Any();

		logger.LogDebug("<<< Exiting Configure()");
        return View("~/Plugins/Widgets.FacebookPixel/Views/Configuration/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> List(FacebookPixelSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering List(searchModel={SearchModel})", searchModel);
        var configurations = await _facebookPixelService.GetPagedConfigurationsAsync(searchModel.StoreId, searchModel.Page - 1, searchModel.PageSize);
        var model = await new FacebookPixelListModel().PrepareToGridAsync(searchModel, configurations, () =>
        {
		logger.LogDebug("List(searchModel={SearchModel}): configurations → {Configurations}", searchModel, configurations);
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

		logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> Create()
    {
		logger.LogDebug(">>> Entering Create()");
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

		logger.LogDebug("<<< Exiting Create()");
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
		logger.LogDebug(">>> Entering Edit(id={Id})", id);
        var configuration = await _facebookPixelService.GetConfigurationByIdAsync(id);
        if (configuration == null)
			logger.LogDebug("Edit(id={Id}): configuration → {Configuration}", id, configuration);
            return RedirectToAction("Configure", "FacebookPixel");

        //fill in model values from the configuration
        var model = configuration.ToModel<FacebookPixelModel>();

		logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);
        //prepare other model properties
        await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores, false);
        model.HideStoresList = model.AvailableStores.SelectionIsNotPossible();
        model.HideCustomEventsSearch = !(await _facebookPixelService.GetCustomEventsAsync(configuration.Id)).Any();
        model.CustomEventSearchModel.ConfigurationId = configuration.Id;
        model.CustomEventSearchModel.AddCustomEvent.AvailableWidgetZones = PreparePublicWidgetZones();
        model.CustomEventSearchModel.SetGridPageSize();

		logger.LogDebug("<<< Exiting Edit(id={Id})", id);
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
		logger.LogDebug(">>> Entering Delete(id={Id})", id);
        var configuration = await _facebookPixelService.GetConfigurationByIdAsync(id);
        if (configuration == null)
			logger.LogDebug("Delete(id={Id}): configuration → {Configuration}", id, configuration);
            return RedirectToAction("Configure", "FacebookPixel");

        //delete configuration
        await _facebookPixelService.DeleteConfigurationAsync(configuration);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

		logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("Configure", "FacebookPixel");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> CustomEventList(CustomEventSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering CustomEventList(searchModel={SearchModel})", searchModel);
        var configuration = await _facebookPixelService.GetConfigurationByIdAsync(searchModel.ConfigurationId)
            ?? throw new ArgumentException("No configuration found with the specified id", nameof(searchModel.ConfigurationId));

        var customEvents = (await _facebookPixelService.GetCustomEventsAsync(configuration.Id, searchModel.WidgetZone)).ToPagedList(searchModel);
        var model = new CustomEventListModel().PrepareToGrid(searchModel, customEvents, () =>
        {
		logger.LogDebug("CustomEventList(searchModel={SearchModel}): customEvents → {CustomEvents}", searchModel, customEvents);
            //fill in model values from the configuration
            return customEvents.Select(customEvent => new CustomEventModel
            {
                ConfigurationId = configuration.Id,
                EventName = customEvent.EventName,
                WidgetZonesName = string.Join(", ", customEvent.WidgetZones)
            });
        });

		logger.LogDebug("<<< Exiting CustomEventList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    //ValidateAttribute is used to force model validation
    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> CustomEventAdd(int configurationId, [Validate] CustomEventModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //save custom events configuration
        await _facebookPixelService.SaveCustomEventsAsync(configurationId, model.EventName, model.WidgetZones);

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public virtual async Task<IActionResult> CustomEventDelete(int configurationId, string id)
    {
		logger.LogDebug(">>> Entering CustomEventDelete(configurationId={ConfigurationId},id={Id})", configurationId, id);
        //save custom events configuration
        var eventName = id;
        await _facebookPixelService.SaveCustomEventsAsync(configurationId, eventName, null);

		logger.LogDebug("<<< Exiting CustomEventDelete(configurationId={ConfigurationId},id={Id})", configurationId, id);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public async Task<IActionResult> CookieSettingsWarning(bool disableForUsersNotAcceptingCookieConsent)
    {
		logger.LogDebug(">>> Entering CookieSettingsWarning(disableForUsersNotAcceptingCookieConsent={DisableForUsersNotAcceptingCookieConsent})", disableForUsersNotAcceptingCookieConsent);
        if (!disableForUsersNotAcceptingCookieConsent || _storeInformationSettings.DisplayEuCookieLawWarning)
            return Json(new { Result = string.Empty });

        //display a warning when "DisplayEuCookieLawWarning" setting is disabled, 
        //but merchant want to disable Facebook Pixel for users not accepting Cookie Consent
        var url = Url.Action("GeneralCommon", "Setting");
        var warning = string.Format(await _localizationService.GetResourceAsync("Plugins.Widgets.FacebookPixel.Configuration.CookieSettingsWarning"), url);
		logger.LogDebug("CookieSettingsWarning(disableForUsersNotAcceptingCookieConsent={DisableForUsersNotAcceptingCookieConsent}): url → {Url}", disableForUsersNotAcceptingCookieConsent, url);
		logger.LogDebug("CookieSettingsWarning(disableForUsersNotAcceptingCookieConsent={DisableForUsersNotAcceptingCookieConsent}): warning → {Warning}", disableForUsersNotAcceptingCookieConsent, warning);
		logger.LogDebug("<<< Exiting CookieSettingsWarning(disableForUsersNotAcceptingCookieConsent={DisableForUsersNotAcceptingCookieConsent})", disableForUsersNotAcceptingCookieConsent);
        return Json(new { Result = warning });
    }

    #endregion
}