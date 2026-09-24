using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Payments;
using Nop.Core.Events;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Payments;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class PaymentController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICountryService _countryService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPaymentModelFactory _paymentModelFactory;
    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly PaymentSettings _paymentSettings;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public PaymentController(ICountryService countryService,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPaymentModelFactory paymentModelFactory,
        IPaymentPluginManager paymentPluginManager,
        IPermissionService permissionService,
        ISettingService settingService,
        PaymentSettings paymentSettings)
    {
		_logger.LogError(">>> Entering PaymentController(countryService={CountryService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},paymentModelFactory={PaymentModelFactory},paymentPluginManager={PaymentPluginManager},permissionService={PermissionService},settingService={SettingService},paymentSettings={PaymentSettings})", countryService, eventPublisher, localizationService, notificationService, paymentModelFactory, paymentPluginManager, permissionService, settingService, paymentSettings);
        _countryService = countryService;
        _eventPublisher = eventPublisher;
		_logger.LogDebug("PaymentController(countryService={CountryService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},paymentModelFactory={PaymentModelFactory},paymentPluginManager={PaymentPluginManager},permissionService={PermissionService},settingService={SettingService},paymentSettings={PaymentSettings}): _eventPublisher → {_eventPublisher}", countryService, eventPublisher, localizationService, notificationService, paymentModelFactory, paymentPluginManager, permissionService, settingService, paymentSettings, _eventPublisher);
        _localizationService = localizationService;
		_logger.LogDebug("PaymentController(countryService={CountryService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},paymentModelFactory={PaymentModelFactory},paymentPluginManager={PaymentPluginManager},permissionService={PermissionService},settingService={SettingService},paymentSettings={PaymentSettings}): _localizationService → {_localizationService}", countryService, eventPublisher, localizationService, notificationService, paymentModelFactory, paymentPluginManager, permissionService, settingService, paymentSettings, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("PaymentController(countryService={CountryService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},paymentModelFactory={PaymentModelFactory},paymentPluginManager={PaymentPluginManager},permissionService={PermissionService},settingService={SettingService},paymentSettings={PaymentSettings}): _notificationService → {_notificationService}", countryService, eventPublisher, localizationService, notificationService, paymentModelFactory, paymentPluginManager, permissionService, settingService, paymentSettings, _notificationService);
        _paymentModelFactory = paymentModelFactory;
		_logger.LogDebug("PaymentController(countryService={CountryService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},paymentModelFactory={PaymentModelFactory},paymentPluginManager={PaymentPluginManager},permissionService={PermissionService},settingService={SettingService},paymentSettings={PaymentSettings}): _paymentModelFactory → {_paymentModelFactory}", countryService, eventPublisher, localizationService, notificationService, paymentModelFactory, paymentPluginManager, permissionService, settingService, paymentSettings, _paymentModelFactory);
        _paymentPluginManager = paymentPluginManager;
		_logger.LogDebug("PaymentController(countryService={CountryService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},paymentModelFactory={PaymentModelFactory},paymentPluginManager={PaymentPluginManager},permissionService={PermissionService},settingService={SettingService},paymentSettings={PaymentSettings}): _paymentPluginManager → {_paymentPluginManager}", countryService, eventPublisher, localizationService, notificationService, paymentModelFactory, paymentPluginManager, permissionService, settingService, paymentSettings, _paymentPluginManager);
        _permissionService = permissionService;
		_logger.LogDebug("PaymentController(countryService={CountryService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},paymentModelFactory={PaymentModelFactory},paymentPluginManager={PaymentPluginManager},permissionService={PermissionService},settingService={SettingService},paymentSettings={PaymentSettings}): _permissionService → {_permissionService}", countryService, eventPublisher, localizationService, notificationService, paymentModelFactory, paymentPluginManager, permissionService, settingService, paymentSettings, _permissionService);
        _settingService = settingService;
		_logger.LogDebug("PaymentController(countryService={CountryService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},paymentModelFactory={PaymentModelFactory},paymentPluginManager={PaymentPluginManager},permissionService={PermissionService},settingService={SettingService},paymentSettings={PaymentSettings}): _settingService → {_settingService}", countryService, eventPublisher, localizationService, notificationService, paymentModelFactory, paymentPluginManager, permissionService, settingService, paymentSettings, _settingService);
        _paymentSettings = paymentSettings;
		_logger.LogDebug("PaymentController(countryService={CountryService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},paymentModelFactory={PaymentModelFactory},paymentPluginManager={PaymentPluginManager},permissionService={PermissionService},settingService={SettingService},paymentSettings={PaymentSettings}): _paymentSettings → {_paymentSettings}", countryService, eventPublisher, localizationService, notificationService, paymentModelFactory, paymentPluginManager, permissionService, settingService, paymentSettings, _paymentSettings);
    }

    #endregion

    #region Methods  

    public virtual IActionResult PaymentMethods()
    {
		_logger.LogError(">>> Entering PaymentMethods()");
        _logger.LogWarning("<<< Exiting PaymentMethods()");
		_logger.LogError(">>> Entering Methods()");
		_logger.LogWarning("<<< Exiting Methods()");
		_logger.LogWarning("<<< Exiting Methods(searchModel={SearchModel})", searchModel);
        return RedirectToAction("Methods");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public virtual async Task<IActionResult> Methods()
    {
        //prepare model
        var model = await _paymentModelFactory.PreparePaymentMethodsModelAsync(new PaymentMethodsModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public virtual async Task<IActionResult> Methods(PaymentMethodSearchModel searchModel)
    {
        //prepare model
        var model = await _paymentModelFactory.PreparePaymentMethodListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public virtual async Task<IActionResult> MethodUpdate(PaymentMethodModel model)
    {
		_logger.LogError(">>> Entering MethodUpdate(model={Model})", model);
        var pm = await _paymentPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
		_logger.LogError("MethodUpdate(model={Model}): pm → {Pm}", model, pm);
        if (_paymentPluginManager.IsPluginActive(pm))
        {
            if (!model.IsActive)
            {
                //mark as disabled
                _paymentSettings.ActivePaymentMethodSystemNames.Remove(pm.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_paymentSettings);
            }
        }
        else
        {
            if (model.IsActive)
            {
                //mark as active
                _paymentSettings.ActivePaymentMethodSystemNames.Add(pm.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_paymentSettings);
            }
        }

        var pluginDescriptor = pm.PluginDescriptor;
        pluginDescriptor.FriendlyName = model.FriendlyName;
		_logger.LogDebug("MethodUpdate(model={Model}): pluginDescriptor.FriendlyName → {Arg1}", model, pluginDescriptor.FriendlyName);
		_logger.LogDebug("MethodUpdate(model={Model}): pluginDescriptor.DisplayOrder → {Arg1}", model, pluginDescriptor.DisplayOrder);
        pluginDescriptor.DisplayOrder = model.DisplayOrder;

        //update the description file
        pluginDescriptor.Save();

        //raise event
        await _eventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

		_logger.LogWarning("<<< Exiting MethodUpdate(model={Model})", model);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public virtual async Task<IActionResult> MethodRestrictions()
    {
		_logger.LogError(">>> Entering MethodRestrictions()");
        //prepare model
        var model = await _paymentModelFactory.PreparePaymentMethodsModelAsync(new PaymentMethodsModel());
		_logger.LogError("MethodRestrictions(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting MethodRestrictions()");
        return View(model);
    }

    //we ignore this filter for increase RequestFormLimits
    [IgnoreAntiforgeryToken]
    //we use 2048 value because in some cases default value (1024) is too small for this action
    [RequestFormLimits(ValueCountLimit = 2048)]
    [HttpPost, ActionName("MethodRestrictions")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public virtual async Task<IActionResult> MethodRestrictionsSave(PaymentMethodsModel model, IFormCollection form)
    {
		_logger.LogError(">>> Entering MethodRestrictionsSave(model={Model},form={Form})", model, form);
        var paymentMethods = await _paymentPluginManager.LoadAllPluginsAsync();
        var countries = await _countryService.GetAllCountriesAsync(showHidden: true);
		_logger.LogError("MethodRestrictionsSave(model={Model},form={Form}): countries → {Countries}", model, form, countries);
		_logger.LogError("MethodRestrictionsSave(model={Model},form={Form}): paymentMethods → {PaymentMethods}", model, form, paymentMethods);

        foreach (var pm in paymentMethods)
        {
            var formKey = "restrict_" + pm.PluginDescriptor.SystemName;
			_logger.LogError("MethodRestrictionsSave(model={Model},form={Form}): formKey → {FormKey}", model, form, formKey);
            var countryIdsToRestrict = (!StringValues.IsNullOrEmpty(form[formKey])
                    ? form[formKey].ToString().Split(_separator, StringSplitOptions.RemoveEmptyEntries).ToList()
                    : new List<string>())
                .Select(x => Convert.ToInt32(x)).ToList();

            var newCountryIds = new List<int>();
			_logger.LogError("MethodRestrictionsSave(model={Model},form={Form}): newCountryIds → {NewCountryIds}", model, form, newCountryIds);
            foreach (var c in countries)
            {
                if (countryIdsToRestrict.Contains(c.Id))
                {
                    newCountryIds.Add(c.Id);
                }
            }

            await _paymentPluginManager.SaveRestrictedCountriesAsync(pm, newCountryIds);
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Payment.MethodRestrictions.Updated"));

		_logger.LogWarning("<<< Exiting MethodRestrictionsSave(model={Model},form={Form})", model, form);
        return RedirectToAction("MethodRestrictions");
    }

    #endregion
}