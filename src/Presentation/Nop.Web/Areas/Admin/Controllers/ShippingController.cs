using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Shipping.Pickup;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ShippingController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IDateRangeService _dateRangeService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IPickupPluginManager _pickupPluginManager;
    protected readonly ISettingService _settingService;
    protected readonly IShippingModelFactory _shippingModelFactory;
    protected readonly IShippingPluginManager _shippingPluginManager;
    protected readonly IShippingMethodsService _shippingMethodsService;
    protected readonly IWarehouseService _warehouseService;
    protected readonly ShippingSettings _shippingSettings;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public ShippingController(IAddressService addressService,
        ICountryService countryService,
        ICustomerActivityService customerActivityService,
        IDateRangeService dateRangeService,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IPickupPluginManager pickupPluginManager,
        ISettingService settingService,
        IShippingModelFactory shippingModelFactory,
        IShippingPluginManager shippingPluginManager,
        IShippingMethodsService shippingMethodsService,
        IWarehouseService warehouseService,
        ShippingSettings shippingSettings)
    {
		_logger.LogError(">>> Entering ShippingController(addressService={AddressService},countryService={CountryService},customerActivityService={CustomerActivityService},dateRangeService={DateRangeService},eventPublisher={EventPublisher},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pickupPluginManager={PickupPluginManager},settingService={SettingService},shippingModelFactory=<redacted>,shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,warehouseService={WarehouseService},shippingSettings=<redacted>)", addressService, countryService, customerActivityService, dateRangeService, eventPublisher, localizationService, localizedEntityService, notificationService, pickupPluginManager, settingService, warehouseService);
        _addressService = addressService;
        _countryService = countryService;
		_logger.LogDebug("ShippingController(addressService={AddressService},countryService={CountryService},customerActivityService={CustomerActivityService},dateRangeService={DateRangeService},eventPublisher={EventPublisher},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pickupPluginManager={PickupPluginManager},settingService={SettingService},shippingModelFactory=<redacted>,shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,warehouseService={WarehouseService},shippingSettings=<redacted>): _countryService → {_countryService}", addressService, countryService, customerActivityService, dateRangeService, eventPublisher, localizationService, localizedEntityService, notificationService, pickupPluginManager, settingService, warehouseService, _countryService);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("ShippingController(addressService={AddressService},countryService={CountryService},customerActivityService={CustomerActivityService},dateRangeService={DateRangeService},eventPublisher={EventPublisher},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pickupPluginManager={PickupPluginManager},settingService={SettingService},shippingModelFactory=<redacted>,shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,warehouseService={WarehouseService},shippingSettings=<redacted>): _customerActivityService → {_customerActivityService}", addressService, countryService, customerActivityService, dateRangeService, eventPublisher, localizationService, localizedEntityService, notificationService, pickupPluginManager, settingService, warehouseService, _customerActivityService);
        _dateRangeService = dateRangeService;
		_logger.LogDebug("ShippingController(addressService={AddressService},countryService={CountryService},customerActivityService={CustomerActivityService},dateRangeService={DateRangeService},eventPublisher={EventPublisher},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pickupPluginManager={PickupPluginManager},settingService={SettingService},shippingModelFactory=<redacted>,shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,warehouseService={WarehouseService},shippingSettings=<redacted>): _dateRangeService → {_dateRangeService}", addressService, countryService, customerActivityService, dateRangeService, eventPublisher, localizationService, localizedEntityService, notificationService, pickupPluginManager, settingService, warehouseService, _dateRangeService);
        _eventPublisher = eventPublisher;
		_logger.LogDebug("ShippingController(addressService={AddressService},countryService={CountryService},customerActivityService={CustomerActivityService},dateRangeService={DateRangeService},eventPublisher={EventPublisher},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pickupPluginManager={PickupPluginManager},settingService={SettingService},shippingModelFactory=<redacted>,shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,warehouseService={WarehouseService},shippingSettings=<redacted>): _eventPublisher → {_eventPublisher}", addressService, countryService, customerActivityService, dateRangeService, eventPublisher, localizationService, localizedEntityService, notificationService, pickupPluginManager, settingService, warehouseService, _eventPublisher);
        _localizationService = localizationService;
		_logger.LogDebug("ShippingController(addressService={AddressService},countryService={CountryService},customerActivityService={CustomerActivityService},dateRangeService={DateRangeService},eventPublisher={EventPublisher},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pickupPluginManager={PickupPluginManager},settingService={SettingService},shippingModelFactory=<redacted>,shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,warehouseService={WarehouseService},shippingSettings=<redacted>): _localizationService → {_localizationService}", addressService, countryService, customerActivityService, dateRangeService, eventPublisher, localizationService, localizedEntityService, notificationService, pickupPluginManager, settingService, warehouseService, _localizationService);
        _localizedEntityService = localizedEntityService;
		_logger.LogDebug("ShippingController(addressService={AddressService},countryService={CountryService},customerActivityService={CustomerActivityService},dateRangeService={DateRangeService},eventPublisher={EventPublisher},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pickupPluginManager={PickupPluginManager},settingService={SettingService},shippingModelFactory=<redacted>,shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,warehouseService={WarehouseService},shippingSettings=<redacted>): _localizedEntityService → {_localizedEntityService}", addressService, countryService, customerActivityService, dateRangeService, eventPublisher, localizationService, localizedEntityService, notificationService, pickupPluginManager, settingService, warehouseService, _localizedEntityService);
        _notificationService = notificationService;
		_logger.LogDebug("ShippingController(addressService={AddressService},countryService={CountryService},customerActivityService={CustomerActivityService},dateRangeService={DateRangeService},eventPublisher={EventPublisher},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pickupPluginManager={PickupPluginManager},settingService={SettingService},shippingModelFactory=<redacted>,shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,warehouseService={WarehouseService},shippingSettings=<redacted>): _notificationService → {_notificationService}", addressService, countryService, customerActivityService, dateRangeService, eventPublisher, localizationService, localizedEntityService, notificationService, pickupPluginManager, settingService, warehouseService, _notificationService);
        _pickupPluginManager = pickupPluginManager;
		_logger.LogDebug("ShippingController(addressService={AddressService},countryService={CountryService},customerActivityService={CustomerActivityService},dateRangeService={DateRangeService},eventPublisher={EventPublisher},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pickupPluginManager={PickupPluginManager},settingService={SettingService},shippingModelFactory=<redacted>,shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,warehouseService={WarehouseService},shippingSettings=<redacted>): _pickupPluginManager → {_pickupPluginManager}", addressService, countryService, customerActivityService, dateRangeService, eventPublisher, localizationService, localizedEntityService, notificationService, pickupPluginManager, settingService, warehouseService, _pickupPluginManager);
        _settingService = settingService;
		_logger.LogDebug("ShippingController(addressService={AddressService},countryService={CountryService},customerActivityService={CustomerActivityService},dateRangeService={DateRangeService},eventPublisher={EventPublisher},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pickupPluginManager={PickupPluginManager},settingService={SettingService},shippingModelFactory=<redacted>,shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,warehouseService={WarehouseService},shippingSettings=<redacted>): _settingService → {_settingService}", addressService, countryService, customerActivityService, dateRangeService, eventPublisher, localizationService, localizedEntityService, notificationService, pickupPluginManager, settingService, warehouseService, _settingService);
        _shippingModelFactory = shippingModelFactory;
		_logger.LogDebug("ShippingController(addressService={AddressService},countryService={CountryService},customerActivityService={CustomerActivityService},dateRangeService={DateRangeService},eventPublisher={EventPublisher},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pickupPluginManager={PickupPluginManager},settingService={SettingService},shippingModelFactory=<redacted>,shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,warehouseService={WarehouseService},shippingSettings=<redacted>): _shippingModelFactory → {_shippingModelFactory}", addressService, countryService, customerActivityService, dateRangeService, eventPublisher, localizationService, localizedEntityService, notificationService, pickupPluginManager, settingService, warehouseService, _shippingModelFactory);
        _shippingPluginManager = shippingPluginManager;
		_logger.LogDebug("ShippingController(addressService={AddressService},countryService={CountryService},customerActivityService={CustomerActivityService},dateRangeService={DateRangeService},eventPublisher={EventPublisher},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pickupPluginManager={PickupPluginManager},settingService={SettingService},shippingModelFactory=<redacted>,shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,warehouseService={WarehouseService},shippingSettings=<redacted>): _shippingPluginManager → {_shippingPluginManager}", addressService, countryService, customerActivityService, dateRangeService, eventPublisher, localizationService, localizedEntityService, notificationService, pickupPluginManager, settingService, warehouseService, _shippingPluginManager);
        _shippingMethodsService = shippingMethodsService;
		_logger.LogDebug("ShippingController(addressService={AddressService},countryService={CountryService},customerActivityService={CustomerActivityService},dateRangeService={DateRangeService},eventPublisher={EventPublisher},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pickupPluginManager={PickupPluginManager},settingService={SettingService},shippingModelFactory=<redacted>,shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,warehouseService={WarehouseService},shippingSettings=<redacted>): _shippingMethodsService → {_shippingMethodsService}", addressService, countryService, customerActivityService, dateRangeService, eventPublisher, localizationService, localizedEntityService, notificationService, pickupPluginManager, settingService, warehouseService, _shippingMethodsService);
        _warehouseService = warehouseService;
		_logger.LogDebug("ShippingController(addressService={AddressService},countryService={CountryService},customerActivityService={CustomerActivityService},dateRangeService={DateRangeService},eventPublisher={EventPublisher},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pickupPluginManager={PickupPluginManager},settingService={SettingService},shippingModelFactory=<redacted>,shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,warehouseService={WarehouseService},shippingSettings=<redacted>): _warehouseService → {_warehouseService}", addressService, countryService, customerActivityService, dateRangeService, eventPublisher, localizationService, localizedEntityService, notificationService, pickupPluginManager, settingService, warehouseService, _warehouseService);
        _shippingSettings = shippingSettings;
		_logger.LogDebug("ShippingController(addressService={AddressService},countryService={CountryService},customerActivityService={CustomerActivityService},dateRangeService={DateRangeService},eventPublisher={EventPublisher},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},pickupPluginManager={PickupPluginManager},settingService={SettingService},shippingModelFactory=<redacted>,shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,warehouseService={WarehouseService},shippingSettings=<redacted>): _shippingSettings → {_shippingSettings}", addressService, countryService, customerActivityService, dateRangeService, eventPublisher, localizationService, localizedEntityService, notificationService, pickupPluginManager, settingService, warehouseService, _shippingSettings);
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(ShippingMethod shippingMethod, ShippingMethodModel model)
    {
		_logger.LogError(">>> Entering UpdateLocalesAsync(shippingMethod=<redacted>,model={Model})", model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(shippingMethod, x => x.Name, localized.Name, localized.LanguageId);
            await _localizedEntityService.SaveLocalizedValueAsync(shippingMethod, x => x.Description, localized.Description, localized.LanguageId);
        }
		_logger.LogWarning("<<< Exiting UpdateLocalesAsync(shippingMethod=<redacted>,model={Model})", model);
    }

    protected virtual async Task UpdateLocalesAsync(DeliveryDate deliveryDate, DeliveryDateModel model)
    {
		_logger.LogError(">>> Entering UpdateLocalesAsync(deliveryDate={DeliveryDate},model={Model})", deliveryDate, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(deliveryDate, x => x.Name, localized.Name, localized.LanguageId);
        }
		_logger.LogWarning("<<< Exiting UpdateLocalesAsync(deliveryDate={DeliveryDate},model={Model})", deliveryDate, model);
    }

    protected virtual async Task UpdateLocalesAsync(ProductAvailabilityRange productAvailabilityRange, ProductAvailabilityRangeModel model)
    {
		_logger.LogError(">>> Entering UpdateLocalesAsync(productAvailabilityRange={ProductAvailabilityRange},model={Model})", productAvailabilityRange, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(productAvailabilityRange, x => x.Name, localized.Name, localized.LanguageId);
        }
		_logger.LogWarning("<<< Exiting UpdateLocalesAsync(productAvailabilityRange={ProductAvailabilityRange},model={Model})", productAvailabilityRange, model);
    }

    #endregion

    #region Shipping rate computation methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> Providers()
    {
		_logger.LogError(">>> Entering Providers()");
        //prepare model
        var model = await _shippingModelFactory.PrepareShippingProviderSearchModelAsync(new ShippingProviderSearchModel());
		_logger.LogError("Providers(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting Providers()");
		_logger.LogWarning("<<< Exiting Providers(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> Providers(ShippingProviderSearchModel searchModel)
    {
        //prepare model
        var model = await _shippingModelFactory.PrepareShippingProviderListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> ProviderUpdate(ShippingProviderModel model)
    {
		_logger.LogError(">>> Entering ProviderUpdate(model={Model})", model);
        var srcm = await _shippingPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
		_logger.LogError("ProviderUpdate(model={Model}): srcm → {Srcm}", model, srcm);
        if (_shippingPluginManager.IsPluginActive(srcm))
        {
            if (!model.IsActive)
            {
                //mark as disabled
                _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Remove(srcm.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_shippingSettings);
            }
        }
        else
        {
            if (model.IsActive)
            {
                //mark as active
                _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add(srcm.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_shippingSettings);
            }
        }

        var pluginDescriptor = srcm.PluginDescriptor;

        //display order
        pluginDescriptor.DisplayOrder = model.DisplayOrder;
		_logger.LogDebug("ProviderUpdate(model={Model}): pluginDescriptor.DisplayOrder → {Arg1}", model, pluginDescriptor.DisplayOrder);

        //update the description file
        pluginDescriptor.Save();

        //raise event
        await _eventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

		_logger.LogWarning("<<< Exiting ProviderUpdate(model={Model})", model);
        return new NullJsonResult();
    }

    #endregion

    #region Pickup point providers

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> PickupPointProviders()
    {
		_logger.LogError(">>> Entering PickupPointProviders()");
        //prepare model
        var model = await _shippingModelFactory.PreparePickupPointProviderSearchModelAsync(new PickupPointProviderSearchModel());
		_logger.LogError("PickupPointProviders(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting PickupPointProviders()");
		_logger.LogWarning("<<< Exiting PickupPointProviders(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> PickupPointProviders(PickupPointProviderSearchModel searchModel)
    {
        //prepare model
        var model = await _shippingModelFactory.PreparePickupPointProviderListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> PickupPointProviderUpdate(PickupPointProviderModel model)
    {
		_logger.LogError(">>> Entering PickupPointProviderUpdate(model={Model})", model);
        var pickupPointProvider = await _pickupPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
		_logger.LogError("PickupPointProviderUpdate(model={Model}): pickupPointProvider → {PickupPointProvider}", model, pickupPointProvider);
        if (_pickupPluginManager.IsPluginActive(pickupPointProvider))
        {
            if (!model.IsActive)
            {
                //mark as disabled
                _shippingSettings.ActivePickupPointProviderSystemNames.Remove(pickupPointProvider.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_shippingSettings);
            }
        }
        else
        {
            if (model.IsActive)
            {
                //mark as active
                _shippingSettings.ActivePickupPointProviderSystemNames.Add(pickupPointProvider.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_shippingSettings);
            }
        }

        var pluginDescriptor = pickupPointProvider.PluginDescriptor;
        pluginDescriptor.DisplayOrder = model.DisplayOrder;
		_logger.LogDebug("PickupPointProviderUpdate(model={Model}): pluginDescriptor.DisplayOrder → {Arg1}", model, pluginDescriptor.DisplayOrder);

        //update the description file
        pluginDescriptor.Save();

        //raise event
        await _eventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

		_logger.LogWarning("<<< Exiting PickupPointProviderUpdate(model={Model})", model);
        return new NullJsonResult();
    }

    #endregion

    #region Shipping methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> Methods()
    {
		_logger.LogError(">>> Entering Methods()");
        //prepare model
        var model = await _shippingModelFactory.PrepareShippingMethodSearchModelAsync(new ShippingMethodSearchModel());
		_logger.LogError("Methods(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting Methods()");
		_logger.LogWarning("<<< Exiting Methods(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> Methods(ShippingMethodSearchModel searchModel)
    {
        //prepare model
        var model = await _shippingModelFactory.PrepareShippingMethodListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> CreateMethod()
    {
		_logger.LogError(">>> Entering CreateMethod()");
        //prepare model
        var model = await _shippingModelFactory.PrepareShippingMethodModelAsync(new ShippingMethodModel(), null);
		_logger.LogError("CreateMethod(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting CreateMethod()");
		_logger.LogWarning("<<< Exiting CreateMethod(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> CreateMethod(ShippingMethodModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var sm = model.ToEntity<ShippingMethod>();
            await _shippingMethodsService.InsertShippingMethodAsync(sm);

            //locales
            await UpdateLocalesAsync(sm, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Methods.Added"));
            return continueEditing ? RedirectToAction("EditMethod", new { id = sm.Id }) : RedirectToAction("Methods");
        }

        //prepare model
        model = await _shippingModelFactory.PrepareShippingMethodModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> EditMethod(int id)
    {
		_logger.LogError(">>> Entering EditMethod(id={Id})", id);
        //try to get a shipping method with the specified id
        var shippingMethod = await _shippingMethodsService.GetShippingMethodByIdAsync(id);
		_logger.LogError("EditMethod(id={Id}): shippingMethod → {ShippingMethod}", id, shippingMethod);
        if (shippingMethod == null)
            return RedirectToAction("Methods");

        //prepare model
        var model = await _shippingModelFactory.PrepareShippingMethodModelAsync(null, shippingMethod);
		_logger.LogError("EditMethod(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting EditMethod(id={Id})", id);
		_logger.LogWarning("<<< Exiting EditMethod(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> EditMethod(ShippingMethodModel model, bool continueEditing)
    {
        //try to get a shipping method with the specified id
        var shippingMethod = await _shippingMethodsService.GetShippingMethodByIdAsync(model.Id);
        if (shippingMethod == null)
            return RedirectToAction("Methods");

        if (ModelState.IsValid)
        {
            shippingMethod = model.ToEntity(shippingMethod);
            await _shippingMethodsService.UpdateShippingMethodAsync(shippingMethod);

            //locales
            await UpdateLocalesAsync(shippingMethod, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Methods.Updated"));

            return continueEditing ? RedirectToAction("EditMethod", shippingMethod.Id) : RedirectToAction("Methods");
        }

        //prepare model
        model = await _shippingModelFactory.PrepareShippingMethodModelAsync(model, shippingMethod, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> DeleteMethod(int id)
    {
		_logger.LogError(">>> Entering DeleteMethod(id={Id})", id);
        //try to get a shipping method with the specified id
        var shippingMethod = await _shippingMethodsService.GetShippingMethodByIdAsync(id);
		_logger.LogError("DeleteMethod(id={Id}): shippingMethod → {ShippingMethod}", id, shippingMethod);
        if (shippingMethod == null)
            return RedirectToAction("Methods");

        await _shippingMethodsService.DeleteShippingMethodAsync(shippingMethod);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Methods.Deleted"));

		_logger.LogWarning("<<< Exiting DeleteMethod(id={Id})", id);
        return RedirectToAction("Methods");
    }

    #endregion

    #region Dates and ranges

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> DatesAndRanges()
    {
		_logger.LogError(">>> Entering DatesAndRanges()");
        //prepare model
        var model = await _shippingModelFactory.PrepareDatesRangesSearchModelAsync(new DatesRangesSearchModel());
		_logger.LogError("DatesAndRanges(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting DatesAndRanges()");
        return View(model);
    }

    #endregion

    #region Delivery dates

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> DeliveryDates(DeliveryDateSearchModel searchModel)
    {
		_logger.LogError(">>> Entering DeliveryDates(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _shippingModelFactory.PrepareDeliveryDateListModelAsync(searchModel);
		_logger.LogError("DeliveryDates(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting DeliveryDates(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> CreateDeliveryDate()
    {
		_logger.LogError(">>> Entering CreateDeliveryDate()");
        //prepare model
        var model = await _shippingModelFactory.PrepareDeliveryDateModelAsync(new DeliveryDateModel(), null);
		_logger.LogError("CreateDeliveryDate(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting CreateDeliveryDate()");
		_logger.LogWarning("<<< Exiting CreateDeliveryDate(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> CreateDeliveryDate(DeliveryDateModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var deliveryDate = model.ToEntity<DeliveryDate>();
            await _dateRangeService.InsertDeliveryDateAsync(deliveryDate);

            //locales
            await UpdateLocalesAsync(deliveryDate, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.DeliveryDates.Added"));

            return continueEditing ? RedirectToAction("EditDeliveryDate", new { id = deliveryDate.Id }) : RedirectToAction("DatesAndRanges");
        }

        //prepare model
        model = await _shippingModelFactory.PrepareDeliveryDateModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> EditDeliveryDate(int id)
    {
		_logger.LogError(">>> Entering EditDeliveryDate(id={Id})", id);
        //try to get a delivery date with the specified id
        var deliveryDate = await _dateRangeService.GetDeliveryDateByIdAsync(id);
		_logger.LogError("EditDeliveryDate(id={Id}): deliveryDate → {DeliveryDate}", id, deliveryDate);
        if (deliveryDate == null)
            return RedirectToAction("DatesAndRanges");

        //prepare model
        var model = await _shippingModelFactory.PrepareDeliveryDateModelAsync(null, deliveryDate);
		_logger.LogError("EditDeliveryDate(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting EditDeliveryDate(id={Id})", id);
		_logger.LogWarning("<<< Exiting EditDeliveryDate(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> EditDeliveryDate(DeliveryDateModel model, bool continueEditing)
    {
        //try to get a delivery date with the specified id
        var deliveryDate = await _dateRangeService.GetDeliveryDateByIdAsync(model.Id);
        if (deliveryDate == null)
            return RedirectToAction("DatesAndRanges");

        if (ModelState.IsValid)
        {
            deliveryDate = model.ToEntity(deliveryDate);
            await _dateRangeService.UpdateDeliveryDateAsync(deliveryDate);

            //locales
            await UpdateLocalesAsync(deliveryDate, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.DeliveryDates.Updated"));

            return continueEditing ? RedirectToAction("EditDeliveryDate", deliveryDate.Id) : RedirectToAction("DatesAndRanges");
        }

        //prepare model
        model = await _shippingModelFactory.PrepareDeliveryDateModelAsync(model, deliveryDate, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> DeleteDeliveryDate(int id)
    {
		_logger.LogError(">>> Entering DeleteDeliveryDate(id={Id})", id);
        //try to get a delivery date with the specified id
        var deliveryDate = await _dateRangeService.GetDeliveryDateByIdAsync(id);
		_logger.LogError("DeleteDeliveryDate(id={Id}): deliveryDate → {DeliveryDate}", id, deliveryDate);
        if (deliveryDate == null)
            return RedirectToAction("DatesAndRanges");

        await _dateRangeService.DeleteDeliveryDateAsync(deliveryDate);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.DeliveryDates.Deleted"));

		_logger.LogWarning("<<< Exiting DeleteDeliveryDate(id={Id})", id);
        return RedirectToAction("DatesAndRanges");
    }

    #endregion

    #region Product availability ranges

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> ProductAvailabilityRanges(ProductAvailabilityRangeSearchModel searchModel)
    {
		_logger.LogError(">>> Entering ProductAvailabilityRanges(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _shippingModelFactory.PrepareProductAvailabilityRangeListModelAsync(searchModel);
		_logger.LogError("ProductAvailabilityRanges(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting ProductAvailabilityRanges(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> CreateProductAvailabilityRange()
    {
		_logger.LogError(">>> Entering CreateProductAvailabilityRange()");
        //prepare model
        var model = await _shippingModelFactory.PrepareProductAvailabilityRangeModelAsync(new ProductAvailabilityRangeModel(), null);
		_logger.LogError("CreateProductAvailabilityRange(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting CreateProductAvailabilityRange()");
		_logger.LogWarning("<<< Exiting CreateProductAvailabilityRange(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> CreateProductAvailabilityRange(ProductAvailabilityRangeModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var productAvailabilityRange = model.ToEntity<ProductAvailabilityRange>();
            await _dateRangeService.InsertProductAvailabilityRangeAsync(productAvailabilityRange);

            //locales
            await UpdateLocalesAsync(productAvailabilityRange, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.ProductAvailabilityRanges.Added"));

            return continueEditing ? RedirectToAction("EditProductAvailabilityRange", new { id = productAvailabilityRange.Id }) : RedirectToAction("DatesAndRanges");
        }

        //prepare model
        model = await _shippingModelFactory.PrepareProductAvailabilityRangeModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> EditProductAvailabilityRange(int id)
    {
		_logger.LogError(">>> Entering EditProductAvailabilityRange(id={Id})", id);
        //try to get a product availability range with the specified id
        var productAvailabilityRange = await _dateRangeService.GetProductAvailabilityRangeByIdAsync(id);
		_logger.LogError("EditProductAvailabilityRange(id={Id}): productAvailabilityRange → {ProductAvailabilityRange}", id, productAvailabilityRange);
        if (productAvailabilityRange == null)
            return RedirectToAction("DatesAndRanges");

        //prepare model
        var model = await _shippingModelFactory.PrepareProductAvailabilityRangeModelAsync(null, productAvailabilityRange);
		_logger.LogError("EditProductAvailabilityRange(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting EditProductAvailabilityRange(id={Id})", id);
		_logger.LogWarning("<<< Exiting EditProductAvailabilityRange(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> EditProductAvailabilityRange(ProductAvailabilityRangeModel model, bool continueEditing)
    {
        //try to get a product availability range with the specified id
        var productAvailabilityRange = await _dateRangeService.GetProductAvailabilityRangeByIdAsync(model.Id);
        if (productAvailabilityRange == null)
            return RedirectToAction("DatesAndRanges");

        if (ModelState.IsValid)
        {
            productAvailabilityRange = model.ToEntity(productAvailabilityRange);
            await _dateRangeService.UpdateProductAvailabilityRangeAsync(productAvailabilityRange);

            //locales
            await UpdateLocalesAsync(productAvailabilityRange, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.ProductAvailabilityRanges.Updated"));

            return continueEditing ? RedirectToAction("EditProductAvailabilityRange", productAvailabilityRange.Id) : RedirectToAction("DatesAndRanges");
        }

        //prepare model
        model = await _shippingModelFactory.PrepareProductAvailabilityRangeModelAsync(model, productAvailabilityRange, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> DeleteProductAvailabilityRange(int id)
    {
		_logger.LogError(">>> Entering DeleteProductAvailabilityRange(id={Id})", id);
        //try to get a product availability range with the specified id
        var productAvailabilityRange = await _dateRangeService.GetProductAvailabilityRangeByIdAsync(id);
		_logger.LogError("DeleteProductAvailabilityRange(id={Id}): productAvailabilityRange → {ProductAvailabilityRange}", id, productAvailabilityRange);
        if (productAvailabilityRange == null)
            return RedirectToAction("DatesAndRanges");

        await _dateRangeService.DeleteProductAvailabilityRangeAsync(productAvailabilityRange);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.ProductAvailabilityRanges.Deleted"));

		_logger.LogWarning("<<< Exiting DeleteProductAvailabilityRange(id={Id})", id);
        return RedirectToAction("DatesAndRanges");
    }

    #endregion

    #region Warehouses

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> Warehouses()
    {
		_logger.LogError(">>> Entering Warehouses()");
        //prepare model
        var model = await _shippingModelFactory.PrepareWarehouseSearchModelAsync(new WarehouseSearchModel());
		_logger.LogError("Warehouses(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting Warehouses()");
		_logger.LogWarning("<<< Exiting Warehouses(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> Warehouses(WarehouseSearchModel searchModel)
    {
        //prepare model
        var model = await _shippingModelFactory.PrepareWarehouseListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> CreateWarehouse()
    {
		_logger.LogError(">>> Entering CreateWarehouse()");
        //prepare model
        var model = await _shippingModelFactory.PrepareWarehouseModelAsync(new WarehouseModel(), null);
		_logger.LogError("CreateWarehouse(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting CreateWarehouse()");
		_logger.LogWarning("<<< Exiting CreateWarehouse(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> CreateWarehouse(WarehouseModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var address = model.Address.ToEntity<Address>();
            address.CreatedOnUtc = DateTime.UtcNow;
            await _addressService.InsertAddressAsync(address);

            //fill entity from model
            var warehouse = model.ToEntity<Warehouse>();
            warehouse.AddressId = address.Id;

            await _warehouseService.InsertWarehouseAsync(warehouse);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewWarehouse",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewWarehouse"), warehouse.Id), warehouse);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Warehouses.Added"));

            return continueEditing ? RedirectToAction("EditWarehouse", new { id = warehouse.Id }) : RedirectToAction("Warehouses");
        }

        //prepare model
        model = await _shippingModelFactory.PrepareWarehouseModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> EditWarehouse(int id)
    {
		_logger.LogError(">>> Entering EditWarehouse(id={Id})", id);
        //try to get a warehouse with the specified id
        var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
		_logger.LogError("EditWarehouse(id={Id}): warehouse → {Warehouse}", id, warehouse);
        if (warehouse == null)
            return RedirectToAction("Warehouses");

        //prepare model
        var model = await _shippingModelFactory.PrepareWarehouseModelAsync(null, warehouse);
		_logger.LogError("EditWarehouse(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting EditWarehouse(id={Id})", id);
		_logger.LogWarning("<<< Exiting EditWarehouse(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> EditWarehouse(WarehouseModel model, bool continueEditing)
    {
        //try to get a warehouse with the specified id
        var warehouse = await _warehouseService.GetWarehouseByIdAsync(model.Id);
        if (warehouse == null)
            return RedirectToAction("Warehouses");

        if (ModelState.IsValid)
        {
            var address = await _addressService.GetAddressByIdAsync(warehouse.AddressId) ??
                          new Address
                          {
                              CreatedOnUtc = DateTime.UtcNow
                          };
            address = model.Address.ToEntity(address);
            if (address.Id > 0)
                await _addressService.UpdateAddressAsync(address);
            else
                await _addressService.InsertAddressAsync(address);

            //fill entity from model
            warehouse = model.ToEntity(warehouse);

            warehouse.AddressId = address.Id;

            await _warehouseService.UpdateWarehouseAsync(warehouse);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditWarehouse",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditWarehouse"), warehouse.Id), warehouse);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Warehouses.Updated"));

            return continueEditing ? RedirectToAction("EditWarehouse", warehouse.Id) : RedirectToAction("Warehouses");
        }

        //prepare model
        model = await _shippingModelFactory.PrepareWarehouseModelAsync(model, warehouse, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> DeleteWarehouse(int id)
    {
		_logger.LogError(">>> Entering DeleteWarehouse(id={Id})", id);
        //try to get a warehouse with the specified id
        var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
		_logger.LogError("DeleteWarehouse(id={Id}): warehouse → {Warehouse}", id, warehouse);
        if (warehouse == null)
            return RedirectToAction("Warehouses");

        await _warehouseService.DeleteWarehouseAsync(warehouse);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteWarehouse",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteWarehouse"), warehouse.Id), warehouse);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.warehouses.Deleted"));

		_logger.LogWarning("<<< Exiting DeleteWarehouse(id={Id})", id);
        return RedirectToAction("Warehouses");
    }

    #endregion

    #region Restrictions

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> Restrictions()
    {
		_logger.LogError(">>> Entering Restrictions()");
        //prepare model
        var model = await _shippingModelFactory.PrepareShippingMethodRestrictionModelAsync(new ShippingMethodRestrictionModel());
		_logger.LogError("Restrictions(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting Restrictions()");
        return View(model);
    }

    //we ignore this filter for increase RequestFormLimits
    [IgnoreAntiforgeryToken]
    //we use 2048 value because in some cases default value (1024) is too small for this action
    [RequestFormLimits(ValueCountLimit = 2048)]
    [HttpPost, ActionName("Restrictions")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> RestrictionSave(ShippingMethodRestrictionModel model, IFormCollection form)
    {
		_logger.LogError(">>> Entering RestrictionSave(model={Model},form={Form})", model, form);
        var countries = await _countryService.GetAllCountriesAsync(showHidden: true);
        var shippingMethods = await _shippingMethodsService.GetAllShippingMethodsAsync();
		_logger.LogError("RestrictionSave(model={Model},form={Form}): shippingMethods → {ShippingMethods}", model, form, shippingMethods);
		_logger.LogError("RestrictionSave(model={Model},form={Form}): countries → {Countries}", model, form, countries);

        foreach (var shippingMethod in shippingMethods)
        {
            var formKey = "restrict_" + shippingMethod.Id;
			_logger.LogError("RestrictionSave(model={Model},form={Form}): formKey → {FormKey}", model, form, formKey);
            var countryIdsToRestrict = !StringValues.IsNullOrEmpty(form[formKey])
                ? form[formKey].ToString().Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList()
                : new List<int>();

            foreach (var country in countries)
            {
                var restrict = countryIdsToRestrict.Contains(country.Id);
				_logger.LogError("RestrictionSave(model={Model},form={Form}): restrict → {Restrict}", model, form, restrict);
                var shippingMethodCountryMappings =
                    await _shippingMethodsService.GetShippingMethodCountryMappingAsync(shippingMethod.Id, country.Id);

                if (restrict)
                {
                    if (shippingMethodCountryMappings.Any())
                        continue;

                    await _shippingMethodsService.InsertShippingMethodCountryMappingAsync(new ShippingMethodCountryMapping { CountryId = country.Id, ShippingMethodId = shippingMethod.Id });
                    await _shippingMethodsService.UpdateShippingMethodAsync(shippingMethod);
                }
                else
                {
                    if (!shippingMethodCountryMappings.Any())
                        continue;

                    await _shippingMethodsService.DeleteShippingMethodCountryMappingAsync(shippingMethodCountryMappings.FirstOrDefault());
                    await _shippingMethodsService.UpdateShippingMethodAsync(shippingMethod);
                }
            }
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Restrictions.Updated"));

		_logger.LogWarning("<<< Exiting RestrictionSave(model={Model},form={Form})", model, form);
        return RedirectToAction("Restrictions");
    }

    #endregion
}