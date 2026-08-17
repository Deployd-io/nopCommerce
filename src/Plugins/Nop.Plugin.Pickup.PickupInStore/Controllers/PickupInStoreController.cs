using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Pickup.PickupInStore.Domain;
using Nop.Plugin.Pickup.PickupInStore.Factories;
using Nop.Plugin.Pickup.PickupInStore.Models;
using Nop.Plugin.Pickup.PickupInStore.Services;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Pickup.PickupInStore.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class PickupInStoreController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly ICountryService _countryService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStorePickupPointModelFactory _storePickupPointModelFactory;
    protected readonly IStorePickupPointService _storePickupPointService;
    protected readonly IStoreService _storeService;
    protected readonly AddressSettings _addressSettings;

    #endregion

    #region Ctor

    public PickupInStoreController(IAddressService addressService,
        ICountryService countryService,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        IStateProvinceService stateProvinceService,
        IStorePickupPointModelFactory storePickupPointModelFactory,
        IStorePickupPointService storePickupPointService,
        IStoreService storeService,
        AddressSettings customerSettings)
    {
		_logger.LogDebug(">>> Entering PickupInStoreController(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},permissionService={PermissionService},stateProvinceService={StateProvinceService},storePickupPointModelFactory={StorePickupPointModelFactory},storePickupPointService={StorePickupPointService},storeService={StoreService},customerSettings={CustomerSettings})", addressService, countryService, localizationService, permissionService, stateProvinceService, storePickupPointModelFactory, storePickupPointService, storeService, customerSettings);
        _addressService = addressService;
		_logger.LogDebug("PickupInStoreController(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},permissionService={PermissionService},stateProvinceService={StateProvinceService},storePickupPointModelFactory={StorePickupPointModelFactory},storePickupPointService={StorePickupPointService},storeService={StoreService},customerSettings={CustomerSettings}): _addressService → {_addressService}", addressService, countryService, localizationService, permissionService, stateProvinceService, storePickupPointModelFactory, storePickupPointService, storeService, customerSettings, _addressService);
        _countryService = countryService;
		_logger.LogDebug("PickupInStoreController(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},permissionService={PermissionService},stateProvinceService={StateProvinceService},storePickupPointModelFactory={StorePickupPointModelFactory},storePickupPointService={StorePickupPointService},storeService={StoreService},customerSettings={CustomerSettings}): _countryService → {_countryService}", addressService, countryService, localizationService, permissionService, stateProvinceService, storePickupPointModelFactory, storePickupPointService, storeService, customerSettings, _countryService);
        _localizationService = localizationService;
		_logger.LogDebug("PickupInStoreController(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},permissionService={PermissionService},stateProvinceService={StateProvinceService},storePickupPointModelFactory={StorePickupPointModelFactory},storePickupPointService={StorePickupPointService},storeService={StoreService},customerSettings={CustomerSettings}): _localizationService → {_localizationService}", addressService, countryService, localizationService, permissionService, stateProvinceService, storePickupPointModelFactory, storePickupPointService, storeService, customerSettings, _localizationService);
        _permissionService = permissionService;
		_logger.LogDebug("PickupInStoreController(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},permissionService={PermissionService},stateProvinceService={StateProvinceService},storePickupPointModelFactory={StorePickupPointModelFactory},storePickupPointService={StorePickupPointService},storeService={StoreService},customerSettings={CustomerSettings}): _permissionService → {_permissionService}", addressService, countryService, localizationService, permissionService, stateProvinceService, storePickupPointModelFactory, storePickupPointService, storeService, customerSettings, _permissionService);
        _stateProvinceService = stateProvinceService;
		_logger.LogDebug("PickupInStoreController(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},permissionService={PermissionService},stateProvinceService={StateProvinceService},storePickupPointModelFactory={StorePickupPointModelFactory},storePickupPointService={StorePickupPointService},storeService={StoreService},customerSettings={CustomerSettings}): _stateProvinceService → {_stateProvinceService}", addressService, countryService, localizationService, permissionService, stateProvinceService, storePickupPointModelFactory, storePickupPointService, storeService, customerSettings, _stateProvinceService);
        _storePickupPointModelFactory = storePickupPointModelFactory;
		_logger.LogDebug("PickupInStoreController(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},permissionService={PermissionService},stateProvinceService={StateProvinceService},storePickupPointModelFactory={StorePickupPointModelFactory},storePickupPointService={StorePickupPointService},storeService={StoreService},customerSettings={CustomerSettings}): _storePickupPointModelFactory → {_storePickupPointModelFactory}", addressService, countryService, localizationService, permissionService, stateProvinceService, storePickupPointModelFactory, storePickupPointService, storeService, customerSettings, _storePickupPointModelFactory);
        _storePickupPointService = storePickupPointService;
		_logger.LogDebug("PickupInStoreController(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},permissionService={PermissionService},stateProvinceService={StateProvinceService},storePickupPointModelFactory={StorePickupPointModelFactory},storePickupPointService={StorePickupPointService},storeService={StoreService},customerSettings={CustomerSettings}): _storePickupPointService → {_storePickupPointService}", addressService, countryService, localizationService, permissionService, stateProvinceService, storePickupPointModelFactory, storePickupPointService, storeService, customerSettings, _storePickupPointService);
        _storeService = storeService;
		_logger.LogDebug("PickupInStoreController(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},permissionService={PermissionService},stateProvinceService={StateProvinceService},storePickupPointModelFactory={StorePickupPointModelFactory},storePickupPointService={StorePickupPointService},storeService={StoreService},customerSettings={CustomerSettings}): _storeService → {_storeService}", addressService, countryService, localizationService, permissionService, stateProvinceService, storePickupPointModelFactory, storePickupPointService, storeService, customerSettings, _storeService);
        _addressSettings = customerSettings;
		_logger.LogDebug("<<< Exiting PickupInStoreController(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},permissionService={PermissionService},stateProvinceService={StateProvinceService},storePickupPointModelFactory={StorePickupPointModelFactory},storePickupPointService={StorePickupPointService},storeService={StoreService},customerSettings={CustomerSettings})", addressService, countryService, localizationService, permissionService, stateProvinceService, storePickupPointModelFactory, storePickupPointService, storeService, customerSettings);
		_logger.LogDebug("PickupInStoreController(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},permissionService={PermissionService},stateProvinceService={StateProvinceService},storePickupPointModelFactory={StorePickupPointModelFactory},storePickupPointService={StorePickupPointService},storeService={StoreService},customerSettings={CustomerSettings}): _addressSettings → {_addressSettings}", addressService, countryService, localizationService, permissionService, stateProvinceService, storePickupPointModelFactory, storePickupPointService, storeService, customerSettings, _addressSettings);
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public async Task<IActionResult> Configure()
    {
		_logger.LogDebug(">>> Entering Configure()");
        //prepare model
        var model = await _storePickupPointModelFactory.PrepareStorePickupPointSearchModelAsync(new StorePickupPointSearchModel());
		_logger.LogDebug("Configure(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting Configure()");
        return View("~/Plugins/Pickup.PickupInStore/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public async Task<IActionResult> List(StorePickupPointSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering List(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _storePickupPointModelFactory.PrepareStorePickupPointListModelAsync(searchModel);
		_logger.LogDebug("List(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public async Task<IActionResult> Create()
    {
		_logger.LogDebug(">>> Entering Create()");
        var model = new StorePickupPointModel
        {
            Address = new AddressModel()
        };

        model.Address.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Address.SelectCountry"), Value = "0" });
        foreach (var country in await _countryService.GetAllCountriesAsync(showHidden: true))
            model.Address.AvailableCountries.Add(new SelectListItem { Text = country.Name, Value = country.Id.ToString() });

        var states = !model.Address.CountryId.HasValue ? new List<StateProvince>()
            : await _stateProvinceService.GetStateProvincesByCountryIdAsync(model.Address.CountryId.Value, showHidden: true);
        if (states.Any())
        {
            model.Address.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Address.SelectState"), Value = "0" });
            foreach (var state in states)
                model.Address.AvailableStates.Add(new SelectListItem { Text = state.Name, Value = state.Id.ToString() });
        }
        else
            model.Address.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Address.Other"), Value = "0" });

        model.AvailableStores.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.StoreScope.AllStores"), Value = "0" });
        foreach (var store in await _storeService.GetAllStoresAsync())
            model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString() });

		_logger.LogDebug("<<< Exiting Create()");
        return View("~/Plugins/Pickup.PickupInStore/Views/Create.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public async Task<IActionResult> Create(StorePickupPointModel model)
    {
        var address = new Address
        {
            Address1 = model.Address.Address1,
            City = model.Address.City,
            County = model.Address.County,
            CountryId = model.Address.CountryId,
            StateProvinceId = model.Address.StateProvinceId,
            ZipPostalCode = model.Address.ZipPostalCode,
            CreatedOnUtc = DateTime.UtcNow
        };
        await _addressService.InsertAddressAsync(address);

        var pickupPoint = new StorePickupPoint
        {
            Name = model.Name,
            Description = model.Description,
            AddressId = address.Id,
            OpeningHours = model.OpeningHours,
            PickupFee = model.PickupFee,
            DisplayOrder = model.DisplayOrder,
            StoreId = model.StoreId,
            Latitude = model.Latitude,
            Longitude = model.Longitude,
            TransitDays = model.TransitDays
        };
        await _storePickupPointService.InsertStorePickupPointAsync(pickupPoint);

        ViewBag.RefreshPage = true;

        return View("~/Plugins/Pickup.PickupInStore/Views/Create.cshtml", model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        var pickupPoint = await _storePickupPointService.GetStorePickupPointByIdAsync(id);
        if (pickupPoint == null)
			_logger.LogDebug("Edit(id={Id}): pickupPoint → {PickupPoint}", id, pickupPoint);
            return RedirectToAction("Configure");

        var model = new StorePickupPointModel
        {
            Id = pickupPoint.Id,
            Name = pickupPoint.Name,
            Description = pickupPoint.Description,
            OpeningHours = pickupPoint.OpeningHours,
            PickupFee = pickupPoint.PickupFee,
            DisplayOrder = pickupPoint.DisplayOrder,
            StoreId = pickupPoint.StoreId,
            Latitude = pickupPoint.Latitude,
            Longitude = pickupPoint.Longitude,
            TransitDays = pickupPoint.TransitDays
        };

        var address = await _addressService.GetAddressByIdAsync(pickupPoint.AddressId);
		_logger.LogDebug("Edit(id={Id}): address → {Address}", id, address);
        if (address != null)
        {
            model.Address = new AddressModel
			_logger.LogDebug("Edit(id={Id}): model.Address → {Arg1}", id, model.Address);
            {
                Address1 = address.Address1,
                City = address.City,
                County = address.County,
                CountryId = address.CountryId,
                StateProvinceId = address.StateProvinceId,
                ZipPostalCode = address.ZipPostalCode,
            };
        }

        model.Address.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Address.SelectCountry"), Value = "0" });
        foreach (var country in await _countryService.GetAllCountriesAsync(showHidden: true))
            model.Address.AvailableCountries.Add(new SelectListItem { Text = country.Name, Value = country.Id.ToString(), Selected = (address != null && country.Id == address.CountryId) });

        var states = !model.Address.CountryId.HasValue ? new List<StateProvince>()
            : await _stateProvinceService.GetStateProvincesByCountryIdAsync(model.Address.CountryId.Value, showHidden: true);
        if (states.Any())
        {
            model.Address.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Address.SelectState"), Value = "0" });
            foreach (var state in states)
                model.Address.AvailableStates.Add(new SelectListItem { Text = state.Name, Value = state.Id.ToString(), Selected = (address != null && state.Id == address.StateProvinceId) });
        }
        else
            model.Address.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Address.Other"), Value = "0" });

        model.AvailableStores.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.StoreScope.AllStores"), Value = "0" });
        foreach (var store in await _storeService.GetAllStoresAsync())
            model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString(), Selected = store.Id == model.StoreId });

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
        return View("~/Plugins/Pickup.PickupInStore/Views/Edit.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public async Task<IActionResult> Edit(StorePickupPointModel model)
    {
        if (!ModelState.IsValid)
            return await Edit(model.Id);

        var pickupPoint = await _storePickupPointService.GetStorePickupPointByIdAsync(model.Id);
        if (pickupPoint == null)
            return RedirectToAction("Configure");

        var address = await _addressService.GetAddressByIdAsync(pickupPoint.AddressId) ?? new Address { CreatedOnUtc = DateTime.UtcNow };
        address.Address1 = model.Address.Address1;
        address.City = model.Address.City;
        address.County = model.Address.County;
        address.CountryId = model.Address.CountryId;
        address.StateProvinceId = model.Address.StateProvinceId;
        address.ZipPostalCode = model.Address.ZipPostalCode;
        if (address.Id > 0)
            await _addressService.UpdateAddressAsync(address);
        else
            await _addressService.InsertAddressAsync(address);

        pickupPoint.Name = model.Name;
        pickupPoint.Description = model.Description;
        pickupPoint.AddressId = address.Id;
        pickupPoint.OpeningHours = model.OpeningHours;
        pickupPoint.PickupFee = model.PickupFee;
        pickupPoint.DisplayOrder = model.DisplayOrder;
        pickupPoint.StoreId = model.StoreId;
        pickupPoint.Latitude = model.Latitude;
        pickupPoint.Longitude = model.Longitude;
        pickupPoint.TransitDays = model.TransitDays;
        await _storePickupPointService.UpdateStorePickupPointAsync(pickupPoint);

        ViewBag.RefreshPage = true;

        return View("~/Plugins/Pickup.PickupInStore/Views/Edit.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        var pickupPoint = await _storePickupPointService.GetStorePickupPointByIdAsync(id);
        if (pickupPoint == null)
			_logger.LogDebug("Delete(id={Id}): pickupPoint → {PickupPoint}", id, pickupPoint);
            return RedirectToAction("Configure");

        var address = await _addressService.GetAddressByIdAsync(pickupPoint.AddressId);
		_logger.LogDebug("Delete(id={Id}): address → {Address}", id, address);
        if (address != null)
            await _addressService.DeleteAddressAsync(address);

        await _storePickupPointService.DeleteStorePickupPointAsync(pickupPoint);

		_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion
}