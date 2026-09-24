using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Pickup.PickupInStore.Domain;
using Nop.Plugin.Pickup.PickupInStore.Services;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Shipping.Tracking;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Pickup.PickupInStore;

public class PickupInStoreProvider : BasePlugin, IPickupPointProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly ICountryService _countryService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStorePickupPointService _storePickupPointService;
    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public PickupInStoreProvider(IAddressService addressService,
        ICountryService countryService,
        ILocalizationService localizationService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        IStorePickupPointService storePickupPointService,
        IWebHelper webHelper)
    {
		_logger.LogError(">>> Entering PickupInStoreProvider(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},stateProvinceService={StateProvinceService},storeContext={StoreContext},storePickupPointService={StorePickupPointService},webHelper={WebHelper})", addressService, countryService, localizationService, stateProvinceService, storeContext, storePickupPointService, webHelper);
        _addressService = addressService;
        _countryService = countryService;
		_logger.LogDebug("PickupInStoreProvider(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},stateProvinceService={StateProvinceService},storeContext={StoreContext},storePickupPointService={StorePickupPointService},webHelper={WebHelper}): _countryService → {_countryService}", addressService, countryService, localizationService, stateProvinceService, storeContext, storePickupPointService, webHelper, _countryService);
        _localizationService = localizationService;
		_logger.LogDebug("PickupInStoreProvider(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},stateProvinceService={StateProvinceService},storeContext={StoreContext},storePickupPointService={StorePickupPointService},webHelper={WebHelper}): _localizationService → {_localizationService}", addressService, countryService, localizationService, stateProvinceService, storeContext, storePickupPointService, webHelper, _localizationService);
        _stateProvinceService = stateProvinceService;
		_logger.LogDebug("PickupInStoreProvider(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},stateProvinceService={StateProvinceService},storeContext={StoreContext},storePickupPointService={StorePickupPointService},webHelper={WebHelper}): _stateProvinceService → {_stateProvinceService}", addressService, countryService, localizationService, stateProvinceService, storeContext, storePickupPointService, webHelper, _stateProvinceService);
        _storeContext = storeContext;
		_logger.LogDebug("PickupInStoreProvider(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},stateProvinceService={StateProvinceService},storeContext={StoreContext},storePickupPointService={StorePickupPointService},webHelper={WebHelper}): _storeContext → {_storeContext}", addressService, countryService, localizationService, stateProvinceService, storeContext, storePickupPointService, webHelper, _storeContext);
        _storePickupPointService = storePickupPointService;
		_logger.LogDebug("PickupInStoreProvider(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},stateProvinceService={StateProvinceService},storeContext={StoreContext},storePickupPointService={StorePickupPointService},webHelper={WebHelper}): _storePickupPointService → {_storePickupPointService}", addressService, countryService, localizationService, stateProvinceService, storeContext, storePickupPointService, webHelper, _storePickupPointService);
        _webHelper = webHelper;
		_logger.LogDebug("PickupInStoreProvider(addressService={AddressService},countryService={CountryService},localizationService={LocalizationService},stateProvinceService={StateProvinceService},storeContext={StoreContext},storePickupPointService={StorePickupPointService},webHelper={WebHelper}): _webHelper → {_webHelper}", addressService, countryService, localizationService, stateProvinceService, storeContext, storePickupPointService, webHelper, _webHelper);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get pickup points for the address
    /// </summary>
    /// <param name="cart">Shopping Cart</param>
    /// <param name="address">Address</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the represents a response of getting pickup points
    /// </returns>
    public async Task<GetPickupPointsResponse> GetPickupPointsAsync(IList<ShoppingCartItem> cart, Address address)
    {
		_logger.LogError(">>> Entering GetPickupPointsAsync(cart={Cart},address={Address})", cart, address);
        var result = new GetPickupPointsResponse();
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("GetPickupPointsAsync(cart={Cart},address={Address}): store → {Store}", cart, address, store);
		_logger.LogError("GetPickupPointsAsync(cart={Cart},address={Address}): result → {Result}", cart, address, result);

        foreach (var point in await _storePickupPointService.GetAllStorePickupPointsAsync(store.Id))
        {
            var pointAddress = await _addressService.GetAddressByIdAsync(point.AddressId);
			_logger.LogError("GetPickupPointsAsync(cart={Cart},address={Address}): pointAddress → {PointAddress}", cart, address, pointAddress);
            if (pointAddress == null)
                continue;

            result.PickupPoints.Add(new PickupPoint
            {
                Id = point.Id.ToString(),
                Name = point.Name,
                Description = point.Description,
                Address = pointAddress.Address1,
                City = pointAddress.City,
                County = pointAddress.County,
                StateAbbreviation = (await _stateProvinceService.GetStateProvinceByAddressAsync(pointAddress))?.Abbreviation ?? string.Empty,
                CountryCode = (await _countryService.GetCountryByAddressAsync(pointAddress))?.TwoLetterIsoCode ?? string.Empty,
                ZipPostalCode = pointAddress.ZipPostalCode,
                OpeningHours = point.OpeningHours,
                PickupFee = point.PickupFee,
                DisplayOrder = point.DisplayOrder,
                ProviderSystemName = PluginDescriptor.SystemName,
                Latitude = point.Latitude,
                Longitude = point.Longitude,
                TransitDays = point.TransitDays
            });
			_logger.LogDebug("GetPickupPointsAsync(cart={Cart},address={Address}): TransitDays → {TransitDays}", cart, address, TransitDays);
        }

        if (!result.PickupPoints.Any())
            result.AddError(await _localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.NoPickupPoints"));

		_logger.LogWarning("<<< Exiting GetPickupPointsAsync(cart={Cart},address={Address})", cart, address);
        return result;
    }

    /// <summary>
    /// Get associated shipment tracker
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment tracker
    /// </returns>
    public Task<IShipmentTracker> GetShipmentTrackerAsync()
    {
		_logger.LogError(">>> Entering GetShipmentTrackerAsync()");
        _logger.LogWarning("<<< Exiting GetShipmentTrackerAsync()");
        return Task.FromResult<IShipmentTracker>(null);
    }

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
		_logger.LogError(">>> Entering GetConfigurationPageUrl()");
        _logger.LogWarning("<<< Exiting GetConfigurationPageUrl()");
        return $"{_webHelper.GetStoreLocation()}Admin/PickupInStore/Configure";
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
		_logger.LogError(">>> Entering InstallAsync()");
        //sample pickup point
        var country = await _countryService.GetCountryByThreeLetterIsoCodeAsync("USA");
		_logger.LogError("InstallAsync(): country → {Country}", country);
        var state = await _stateProvinceService.GetStateProvinceByAbbreviationAsync("NY", country?.Id);
		_logger.LogError("InstallAsync(): state → {State}", state);

        var address = new Address
        {
            Address1 = "21 West 52nd Street",
            City = "New York",
            CountryId = country?.Id,
            StateProvinceId = state?.Id,
            ZipPostalCode = "10021",
            CreatedOnUtc = DateTime.UtcNow
        };
        await _addressService.InsertAddressAsync(address);

        var pickupPoint = new StorePickupPoint
        {
            Name = "New York store",
            AddressId = address.Id,
            OpeningHours = "10.00 - 19.00",
            PickupFee = 1.99m
        };
        await _storePickupPointService.InsertStorePickupPointAsync(pickupPoint);

        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Pickup.PickupInStore.AddNew"] = "Add a new pickup point",
            ["Plugins.Pickup.PickupInStore.Fields.Description"] = "Description",
            ["Plugins.Pickup.PickupInStore.Fields.Description.Hint"] = "Specify a description of the pickup point.",
            ["Plugins.Pickup.PickupInStore.Fields.DisplayOrder"] = "Display order",
            ["Plugins.Pickup.PickupInStore.Fields.DisplayOrder.Hint"] = "Specify the pickup point display order.",
            ["Plugins.Pickup.PickupInStore.Fields.Latitude"] = "Latitude",
            ["Plugins.Pickup.PickupInStore.Fields.Latitude.Hint"] = "Specify a latitude (DD.dddddddd°).",
            ["Plugins.Pickup.PickupInStore.Fields.Latitude.InvalidPrecision"] = "Precision should be less then 8",
            ["Plugins.Pickup.PickupInStore.Fields.Latitude.InvalidRange"] = "Latitude should be in range -90 to 90",
            ["Plugins.Pickup.PickupInStore.Fields.Latitude.IsNullWhenLongitudeHasValue"] = "Latitude and Longitude should be specify together",
            ["Plugins.Pickup.PickupInStore.Fields.Longitude"] = "Longitude",
            ["Plugins.Pickup.PickupInStore.Fields.Longitude.Hint"] = "Specify a longitude (DD.dddddddd°).",
            ["Plugins.Pickup.PickupInStore.Fields.Longitude.InvalidPrecision"] = "Precision should be less then 8",
            ["Plugins.Pickup.PickupInStore.Fields.Longitude.InvalidRange"] = "Longitude should be in range -180 to 180",
            ["Plugins.Pickup.PickupInStore.Fields.Longitude.IsNullWhenLatitudeHasValue"] = "Latitude and Longitude should be specify together",
            ["Plugins.Pickup.PickupInStore.Fields.Name"] = "Name",
            ["Plugins.Pickup.PickupInStore.Fields.Name.Hint"] = "Specify a name of the pickup point.",
            ["Plugins.Pickup.PickupInStore.Fields.OpeningHours"] = "Opening hours",
            ["Plugins.Pickup.PickupInStore.Fields.OpeningHours.Hint"] = "Specify opening hours of the pickup point (Monday - Friday: 09:00 - 19:00 for example).",
            ["Plugins.Pickup.PickupInStore.Fields.PickupFee"] = "Pickup fee",
            ["Plugins.Pickup.PickupInStore.Fields.PickupFee.Hint"] = "Specify a fee for the shipping to the pickup point.",
            ["Plugins.Pickup.PickupInStore.Fields.Store"] = "Store",
            ["Plugins.Pickup.PickupInStore.Fields.Store.Hint"] = "A store name for which this pickup point will be available.",
            ["Plugins.Pickup.PickupInStore.Fields.TransitDays"] = "Transit days",
            ["Plugins.Pickup.PickupInStore.Fields.TransitDays.Hint"] = "The number of days of delivery of the goods to pickup point.",
            ["Plugins.Pickup.PickupInStore.NoPickupPoints"] = "No pickup points are available"
        });

        await base.InstallAsync();
		_logger.LogWarning("<<< Exiting InstallAsync()");
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
		_logger.LogError(">>> Entering UninstallAsync()");
        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Pickup.PickupInStore");

        await base.UninstallAsync();
		_logger.LogWarning("<<< Exiting UninstallAsync()");
    }

    #endregion
}