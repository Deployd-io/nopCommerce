using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Tax;
using Nop.Services.Themes;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Plugins;
using Nop.Web.Areas.Admin.Models.Plugins.Marketplace;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class PluginController : BaseAdminController
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
    protected readonly IAuthenticationPluginManager _authenticationPluginManager;
    protected readonly ICommonModelFactory _commonModelFactory;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILocalizationService _localizationService;
    protected readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IPickupPluginManager _pickupPluginManager;
    protected readonly IPluginModelFactory _pluginModelFactory;
    protected readonly IPluginService _pluginService;
    protected readonly ISearchPluginManager _searchPluginManager;
    protected readonly ISettingService _settingService;
    protected readonly IShippingPluginManager _shippingPluginManager;
    protected readonly IUploadService _uploadService;
    protected readonly IWidgetPluginManager _widgetPluginManager;
    protected readonly IWorkContext _workContext;
    protected readonly MultiFactorAuthenticationSettings _multiFactorAuthenticationSettings;
    protected readonly PaymentSettings _paymentSettings;
    protected readonly ShippingSettings _shippingSettings;
    protected readonly TaxSettings _taxSettings;
    protected readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public PluginController(CatalogSettings catalogSettings,
        ExternalAuthenticationSettings externalAuthenticationSettings,
        IAuthenticationPluginManager authenticationPluginManager,
        ICommonModelFactory commonModelFactory,
        ICustomerActivityService customerActivityService,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
        INotificationService notificationService,
        IPermissionService permissionService,
        IPaymentPluginManager paymentPluginManager,
        IPickupPluginManager pickupPluginManager,
        IPluginModelFactory pluginModelFactory,
        IPluginService pluginService,
        ISearchPluginManager searchPluginManager,
        ISettingService settingService,
        IShippingPluginManager shippingPluginManager,
        IUploadService uploadService,
        IWidgetPluginManager widgetPluginManager,
        IWorkContext workContext,
        MultiFactorAuthenticationSettings multiFactorAuthenticationSettings,
        PaymentSettings paymentSettings,
        ShippingSettings shippingSettings,
        TaxSettings taxSettings,
        WidgetSettings widgetSettings)
    {
        _catalogSettings = catalogSettings;
        _externalAuthenticationSettings = externalAuthenticationSettings;
        _authenticationPluginManager = authenticationPluginManager;
        _commonModelFactory = commonModelFactory;
        _customerActivityService = customerActivityService;
        _eventPublisher = eventPublisher;
        _localizationService = localizationService;
        _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _paymentPluginManager = paymentPluginManager;
        _pickupPluginManager = pickupPluginManager;
        _pluginModelFactory = pluginModelFactory;
        _pluginService = pluginService;
        _searchPluginManager = searchPluginManager;
        _settingService = settingService;
        _shippingPluginManager = shippingPluginManager;
        _uploadService = uploadService;
        _widgetPluginManager = widgetPluginManager;
        _workContext = workContext;
        _multiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
        _paymentSettings = paymentSettings;
        _shippingSettings = shippingSettings;
        _taxSettings = taxSettings;
        _widgetSettings = widgetSettings;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> List(bool showWarnings = true)
    {
		logger.LogDebug(">>> Entering List(showWarnings={ShowWarnings})", showWarnings);
        var model = await _pluginModelFactory.PreparePluginSearchModelAsync(new PluginSearchModel());

		logger.LogDebug("List(showWarnings={ShowWarnings}): model → {Model}", showWarnings, model);
        if (!showWarnings)
            return View(model);

        var warnings = new List<SystemWarningModel>();
        await _commonModelFactory.PreparePluginsWarningModelAsync(warnings);
		logger.LogDebug("List(showWarnings={ShowWarnings}): warnings → {Warnings}", showWarnings, warnings);

        if (warnings.Any())
            _notificationService.WarningNotification(string.Join("<br />", warnings), false);

		logger.LogDebug("<<< Exiting List(showWarnings={ShowWarnings})", showWarnings);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> ListSelect(PluginSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ListSelect(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _pluginModelFactory.PreparePluginListModelAsync(searchModel);

		logger.LogDebug("ListSelect(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ListSelect(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> AdminNavigationPlugins()
    {
		logger.LogDebug(">>> Entering AdminNavigationPlugins()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //prepare models
        var models = await (await _pluginModelFactory.PrepareAdminNavigationPluginModelsAsync()).SelectAwait(async model => new
        {
            title = model.FriendlyName,
            link = model.ConfigurationUrl,
            parent = await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.Local"),
            grandParent = string.Empty,
            rate = -50 //negative rate is set to move plugins to the end of list
			logger.LogDebug("AdminNavigationPlugins(): title → {Title}", title);
			logger.LogDebug("AdminNavigationPlugins(): link → {Link}", link);
			logger.LogDebug("AdminNavigationPlugins(): parent → {Parent}", parent);
			logger.LogDebug("AdminNavigationPlugins(): grandParent → {GrandParent}", grandParent);
			logger.LogDebug("AdminNavigationPlugins(): rate → {Rate}", rate);
			logger.LogInformation("AdminNavigationPlugins(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }).ToListAsync();

		logger.LogDebug("<<< Exiting AdminNavigationPlugins()");
        return Json(models);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> UploadPluginsAndThemes(IFormFile archivefile)
    {
		logger.LogDebug(">>> Entering UploadPluginsAndThemes(archivefile={Archivefile})", archivefile);
        try
        {
            if (archivefile == null || archivefile.Length == 0)
                throw new NopException(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));

            var descriptors = await _uploadService.UploadPluginsAndThemesAsync(archivefile);
            var pluginDescriptors = descriptors.OfType<PluginDescriptor>().ToList();
			logger.LogDebug("UploadPluginsAndThemes(archivefile={Archivefile}): descriptors → {Descriptors}", archivefile, descriptors);
			logger.LogDebug("UploadPluginsAndThemes(archivefile={Archivefile}): pluginDescriptors → {PluginDescriptors}", archivefile, pluginDescriptors);
            var themeDescriptors = descriptors.OfType<ThemeDescriptor>().ToList();
            
			logger.LogDebug("UploadPluginsAndThemes(archivefile={Archivefile}): themeDescriptors → {ThemeDescriptors}", archivefile, themeDescriptors);
            if (pluginDescriptors.Any())
            {
                //events
                await _eventPublisher.PublishAsync(new PluginsUploadedEvent(pluginDescriptors));

                //activity log
                var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.UploadNewPlugin");
                await _customerActivityService.InsertActivitiesAsync("UploadNewPlugin", pluginDescriptors, descriptor => string.Format(activityLogFormat, descriptor.FriendlyName));
				logger.LogDebug("UploadPluginsAndThemes(archivefile={Archivefile}): activityLogFormat → {ActivityLogFormat}", archivefile, activityLogFormat);
            }

            if (themeDescriptors.Any())
            {
                //events
                await _eventPublisher.PublishAsync(new ThemesUploadedEvent(themeDescriptors));

                //activity log
                var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.UploadNewTheme");
                await _customerActivityService.InsertActivitiesAsync("UploadNewTheme", themeDescriptors, descriptor => string.Format(activityLogFormat, descriptor.FriendlyName));
            }

            var message = string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.Uploaded"), pluginDescriptors.Count, themeDescriptors.Count);
            _notificationService.SuccessNotification(message);
			logger.LogDebug("UploadPluginsAndThemes(archivefile={Archivefile}): message → {Message}", archivefile, message);

            if (themeDescriptors.Any())
                return View("RestartApplication", Url.Action("List", "Plugin"));
        }
        catch (Exception exc)
        {
			logger.LogError("Exception in UploadPluginsAndThemes(archivefile={Archivefile}): {Arg1}", archivefile, ex.Message);
            await _notificationService.ErrorNotificationAsync(exc);
        }

		logger.LogDebug("<<< Exiting UploadPluginsAndThemes(archivefile={Archivefile})", archivefile);
        return RedirectToAction("List", new { showWarnings = false });
    }

    [HttpPost, ActionName("List")]
    [FormValueRequired(FormValueRequirement.StartsWith, "install-plugin-link-")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> Install(IFormCollection form)
    {
		logger.LogDebug(">>> Entering Install(form={Form})", form);
        try
        {
            //get plugin system name
            string systemName = null;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("install-plugin-link-", StringComparison.InvariantCultureIgnoreCase))
				logger.LogDebug("Install(form={Form}): systemName → {SystemName}", form, systemName);
                    systemName = formValue["install-plugin-link-".Length..];

            var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.All);
            if (pluginDescriptor == null)
				logger.LogDebug("Install(form={Form}): pluginDescriptor → {PluginDescriptor}", form, pluginDescriptor);
                //No plugin found with the specified id
                return RedirectToAction("List", new { showWarnings = false });

            //check whether plugin is not installed
            if (pluginDescriptor.Installed)
                return RedirectToAction("List", new { showWarnings = false });

            await _pluginService.PreparePluginToInstallAsync(pluginDescriptor.SystemName, await _workContext.GetCurrentCustomerAsync());
            pluginDescriptor.ShowInPluginsList = false;
			logger.LogDebug("Install(form={Form}): pluginDescriptor.ShowInPluginsList → {Arg1}", form, pluginDescriptor.ShowInPluginsList);
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.ChangesApplyAfterReboot"));
        }
        catch (Exception exc)
        {
			logger.LogError("Exception in Install(form={Form}): {Arg1}", form, ex.Message);
            await _notificationService.ErrorNotificationAsync(exc);
        }

		logger.LogDebug("<<< Exiting Install(form={Form})", form);
        return RedirectToAction("List", new { showWarnings = false });
    }

    [HttpPost, ActionName("List")]
    [FormValueRequired(FormValueRequirement.StartsWith, "uninstall-plugin-link-")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> Uninstall(IFormCollection form)
    {
		logger.LogDebug(">>> Entering Uninstall(form={Form})", form);
        try
        {
            //get plugin system name
            string systemName = null;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("uninstall-plugin-link-", StringComparison.InvariantCultureIgnoreCase))
				logger.LogDebug("Uninstall(form={Form}): systemName → {SystemName}", form, systemName);
                    systemName = formValue["uninstall-plugin-link-".Length..];

            var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.All);
            if (pluginDescriptor == null)
				logger.LogDebug("Uninstall(form={Form}): pluginDescriptor → {PluginDescriptor}", form, pluginDescriptor);
                //No plugin found with the specified id
                return RedirectToAction("List", new { showWarnings = false });

            //check whether plugin is installed
            if (!pluginDescriptor.Installed)
                return RedirectToAction("List", new { showWarnings = false });

            await _pluginService.PreparePluginToUninstallAsync(pluginDescriptor.SystemName);
            pluginDescriptor.ShowInPluginsList = false;
			logger.LogDebug("Uninstall(form={Form}): pluginDescriptor.ShowInPluginsList → {Arg1}", form, pluginDescriptor.ShowInPluginsList);
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.ChangesApplyAfterReboot"));
        }
        catch (Exception exc)
        {
			logger.LogError("Exception in Uninstall(form={Form}): {Arg1}", form, ex.Message);
            await _notificationService.ErrorNotificationAsync(exc);
        }

		logger.LogDebug("<<< Exiting Uninstall(form={Form})", form);
        return RedirectToAction("List", new { showWarnings = false });
    }

    [HttpPost, ActionName("List")]
    [FormValueRequired(FormValueRequirement.StartsWith, "delete-plugin-link-")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> Delete(IFormCollection form)
    {
		logger.LogDebug(">>> Entering Delete(form={Form})", form);
        try
        {
            //get plugin system name
            string systemName = null;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("delete-plugin-link-", StringComparison.InvariantCultureIgnoreCase))
				logger.LogDebug("Delete(form={Form}): systemName → {SystemName}", form, systemName);
                    systemName = formValue["delete-plugin-link-".Length..];

            var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.All);

			logger.LogDebug("Delete(form={Form}): pluginDescriptor → {PluginDescriptor}", form, pluginDescriptor);
            //check whether plugin is not installed
            if (pluginDescriptor.Installed)
                return RedirectToAction("List", new { showWarnings = false });

            await _pluginService.PreparePluginToDeleteAsync(pluginDescriptor.SystemName);
            pluginDescriptor.ShowInPluginsList = false;
			logger.LogDebug("Delete(form={Form}): pluginDescriptor.ShowInPluginsList → {Arg1}", form, pluginDescriptor.ShowInPluginsList);
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.ChangesApplyAfterReboot"));
        }
        catch (Exception exc)
        {
			logger.LogError("Exception in Delete(form={Form}): {Arg1}", form, ex.Message);
            await _notificationService.ErrorNotificationAsync(exc);
        }

		logger.LogDebug("<<< Exiting Delete(form={Form})", form);
        return RedirectToAction("List", new { showWarnings = false });
    }

    [HttpPost, ActionName("List")]
    [FormValueRequired("plugin-reload-grid")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> ReloadList()
    {
		logger.LogDebug(">>> Entering ReloadList()");
        await _pluginService.UninstallPluginsAsync();
        await _pluginService.DeletePluginsAsync();

		logger.LogDebug("<<< Exiting ReloadList()");
        return View("RestartApplication", Url.Action("List", "Plugin"));
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> UninstallAndDeleteUnusedPlugins(string[] names)
    {
		logger.LogDebug(">>> Entering UninstallAndDeleteUnusedPlugins(names={Names})", names);
        foreach (var name in names)
            await _pluginService.PreparePluginToUninstallAsync(name);

        await _pluginService.UninstallPluginsAsync();

        foreach (var name in names)
            await _pluginService.PreparePluginToDeleteAsync(name);

        await _pluginService.DeletePluginsAsync();

		logger.LogDebug("<<< Exiting UninstallAndDeleteUnusedPlugins(names={Names})", names);
        return View("RestartApplication", Url.Action("Warnings", "Common"));
    }

    [HttpPost, ActionName("List")]
    [FormValueRequired("plugin-apply-changes")]
    public virtual async Task<IActionResult> ApplyChanges()
    {
		logger.LogDebug(">>> Entering ApplyChanges()");
		logger.LogDebug("<<< Exiting ApplyChanges()");
        return await ReloadList();
    }

    [HttpPost, ActionName("List")]
    [FormValueRequired("plugin-discard-changes")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual IActionResult DiscardChanges()
    {
		logger.LogDebug(">>> Entering DiscardChanges()");
        _pluginService.ResetChanges();

		logger.LogDebug("<<< Exiting DiscardChanges()");
        return RedirectToAction("List", new { showWarnings = false });
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> EditPopup(string systemName)
    {
        //try to get a plugin with the specified system name
        var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.All);
        if (pluginDescriptor == null)
            return RedirectToAction("List", new { showWarnings = false });

        //prepare model
        var model = await _pluginModelFactory.PreparePluginModelAsync(null, pluginDescriptor);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> EditPopup(PluginModel model)
    {
        //try to get a plugin with the specified system name
        var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(model.SystemName, LoadPluginsMode.All);
        if (pluginDescriptor == null)
            return RedirectToAction("List", new { showWarnings = false });

        if (ModelState.IsValid)
        {
            ViewBag.RefreshPage = true;

            //we allow editing of 'friendly name', 'display order', store mappings
            pluginDescriptor.FriendlyName = model.FriendlyName;
            pluginDescriptor.DisplayOrder = model.DisplayOrder;
            pluginDescriptor.LimitedToStores.Clear();
            if (model.SelectedStoreIds.Any())
                pluginDescriptor.LimitedToStores = model.SelectedStoreIds;
            pluginDescriptor.LimitedToCustomerRoles.Clear();
            if (model.SelectedCustomerRoleIds.Any())
                pluginDescriptor.LimitedToCustomerRoles = model.SelectedCustomerRoleIds;

            //update the description file
            pluginDescriptor.Save();

            //locales
            var pluginInstance = pluginDescriptor.Instance<IPlugin>();
            foreach (var localized in model.Locales)
            {
                await _localizationService.SaveLocalizedFriendlyNameAsync(pluginInstance, localized.LanguageId, localized.FriendlyName);
            }

            //enabled/disabled
            if (!pluginDescriptor.Installed)
                return View(model);

            bool pluginIsActive;
            switch (pluginInstance)
            {
                case IPaymentMethod paymentMethod:
                    pluginIsActive = _paymentPluginManager.IsPluginActive(paymentMethod);
                    if (pluginIsActive && !model.IsEnabled)
                    {
                        //mark as disabled
                        _paymentSettings.ActivePaymentMethodSystemNames.Remove(pluginDescriptor.SystemName);
                        await _settingService.SaveSettingAsync(_paymentSettings);
                        break;
                    }

                    if (!pluginIsActive && model.IsEnabled)
                    {
                        //mark as enabled
                        _paymentSettings.ActivePaymentMethodSystemNames.Add(pluginDescriptor.SystemName);
                        await _settingService.SaveSettingAsync(_paymentSettings);
                    }

                    break;
                case IShippingRateComputationMethod shippingRateComputationMethod:
                    pluginIsActive = _shippingPluginManager.IsPluginActive(shippingRateComputationMethod);
                    if (pluginIsActive && !model.IsEnabled)
                    {
                        //mark as disabled
                        _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Remove(pluginDescriptor.SystemName);
                        await _settingService.SaveSettingAsync(_shippingSettings);
                        break;
                    }

                    if (!pluginIsActive && model.IsEnabled)
                    {
                        //mark as enabled
                        _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add(pluginDescriptor.SystemName);
                        await _settingService.SaveSettingAsync(_shippingSettings);
                    }

                    break;
                case IPickupPointProvider pickupPointProvider:
                    pluginIsActive = _pickupPluginManager.IsPluginActive(pickupPointProvider);
                    if (pluginIsActive && !model.IsEnabled)
                    {
                        //mark as disabled
                        _shippingSettings.ActivePickupPointProviderSystemNames.Remove(pluginDescriptor.SystemName);
                        await _settingService.SaveSettingAsync(_shippingSettings);
                        break;
                    }

                    if (!pluginIsActive && model.IsEnabled)
                    {
                        //mark as enabled
                        _shippingSettings.ActivePickupPointProviderSystemNames.Add(pluginDescriptor.SystemName);
                        await _settingService.SaveSettingAsync(_shippingSettings);
                    }

                    break;
                case ITaxProvider taxProvider:
                    if (!model.IsEnabled)
                    {
                        //mark as disabled
                        _taxSettings.ActiveTaxProviderSystemName = string.Empty;
                        await _settingService.SaveSettingAsync(_taxSettings);
                        break;
                    }

                    //mark as enabled
                    _taxSettings.ActiveTaxProviderSystemName = model.SystemName;
                    await _settingService.SaveSettingAsync(_taxSettings);
                    break;
                case IExternalAuthenticationMethod externalAuthenticationMethod:
                    pluginIsActive = _authenticationPluginManager.IsPluginActive(externalAuthenticationMethod);
                    if (pluginIsActive && !model.IsEnabled)
                    {
                        //mark as disabled
                        _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(pluginDescriptor.SystemName);
                        await _settingService.SaveSettingAsync(_externalAuthenticationSettings);
                        break;
                    }

                    if (!pluginIsActive && model.IsEnabled)
                    {
                        //mark as enabled
                        _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(pluginDescriptor.SystemName);
                        await _settingService.SaveSettingAsync(_externalAuthenticationSettings);
                    }

                    break;
                case IMultiFactorAuthenticationMethod multiFactorAuthenticationMethod:
                    pluginIsActive = _multiFactorAuthenticationPluginManager.IsPluginActive(multiFactorAuthenticationMethod);
                    if (pluginIsActive && !model.IsEnabled)
                    {
                        //mark as disabled
                        _multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(pluginDescriptor.SystemName);
                        await _settingService.SaveSettingAsync(_multiFactorAuthenticationSettings);
                        break;
                    }

                    if (!pluginIsActive && model.IsEnabled)
                    {
                        //mark as enabled
                        _multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(pluginDescriptor.SystemName);
                        await _settingService.SaveSettingAsync(_multiFactorAuthenticationSettings);
                    }

                    break;

                case ISearchProvider _:
                    if (!model.IsEnabled)
                    {
                        //mark as disabled
                        _catalogSettings.ActiveSearchProviderSystemName = string.Empty;
                        await _settingService.SaveSettingAsync(_catalogSettings);
                        break;
                    }

                    //mark as enabled
                    _catalogSettings.ActiveSearchProviderSystemName = model.SystemName;
                    await _settingService.SaveSettingAsync(_catalogSettings);
                    break;

                case IWidgetPlugin widgetPlugin:
                    pluginIsActive = _widgetPluginManager.IsPluginActive(widgetPlugin);
                    if (pluginIsActive && !model.IsEnabled)
                    {
                        //mark as disabled
                        _widgetSettings.ActiveWidgetSystemNames.Remove(pluginDescriptor.SystemName);
                        await _settingService.SaveSettingAsync(_widgetSettings);
                        break;
                    }

                    if (!pluginIsActive && model.IsEnabled)
                    {
                        //mark as enabled
                        _widgetSettings.ActiveWidgetSystemNames.Add(pluginDescriptor.SystemName);
                        await _settingService.SaveSettingAsync(_widgetSettings);
                    }

                    break;
            }

            //activity log
            await _customerActivityService.InsertActivityAsync("EditPlugin",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditPlugin"), pluginDescriptor.FriendlyName));

            //raise event
            await _eventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));
            
            return View(model);
        }

        //prepare model
        model = await _pluginModelFactory.PreparePluginModelAsync(model, pluginDescriptor, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting EditPopup(model={Model})", model);
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> OfficialFeed()
    {
		logger.LogDebug(">>> Entering OfficialFeed()");
        //prepare model
        var model = await _pluginModelFactory.PrepareOfficialFeedPluginSearchModelAsync(new OfficialFeedPluginSearchModel());

		logger.LogDebug("OfficialFeed(): model → {Model}", model);
		logger.LogDebug("<<< Exiting OfficialFeed()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> OfficialFeedSelect(OfficialFeedPluginSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering OfficialFeedSelect(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _pluginModelFactory.PrepareOfficialFeedPluginListModelAsync(searchModel);

		logger.LogDebug("OfficialFeedSelect(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting OfficialFeedSelect(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion
}
