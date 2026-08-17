using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Plugins;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Shipping.Tracking;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Shipping;

internal class PickupPointTestProvider : BasePlugin, IPickupPointProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly ICountryService _countryService;
    protected readonly IStateProvinceService _stateProvinceService;

    #endregion

    #region Ctor

    public PickupPointTestProvider(IAddressService addressService,
        ICountryService countryService,
        IStateProvinceService stateProvinceService)
    {
		_logger.LogDebug(">>> Entering PickupPointTestProvider(addressService={AddressService},countryService={CountryService},stateProvinceService={StateProvinceService})", addressService, countryService, stateProvinceService);
        _addressService = addressService;
        _countryService = countryService;
		_logger.LogDebug("PickupPointTestProvider(addressService={AddressService},countryService={CountryService},stateProvinceService={StateProvinceService}): _countryService → {_countryService}", addressService, countryService, stateProvinceService, _countryService);
		_logger.LogDebug("PickupPointTestProvider(addressService={AddressService},countryService={CountryService},stateProvinceService={StateProvinceService}): _addressService → {_addressService}", addressService, countryService, stateProvinceService, _addressService);
        _stateProvinceService = stateProvinceService;
		_logger.LogDebug("<<< Exiting PickupPointTestProvider(addressService={AddressService},countryService={CountryService},stateProvinceService={StateProvinceService})", addressService, countryService, stateProvinceService);
		_logger.LogDebug("PickupPointTestProvider(addressService={AddressService},countryService={CountryService},stateProvinceService={StateProvinceService}): _stateProvinceService → {_stateProvinceService}", addressService, countryService, stateProvinceService, _stateProvinceService);
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
		_logger.LogDebug(">>> Entering GetPickupPointsAsync(cart={Cart},address={Address})", cart, address);
        var result = new GetPickupPointsResponse();

		_logger.LogDebug("GetPickupPointsAsync(cart={Cart},address={Address}): result → {Result}", cart, address, result);
        var pointAddress = await _addressService.GetAddressByIdAsync(1);
		_logger.LogDebug("GetPickupPointsAsync(cart={Cart},address={Address}): pointAddress → {PointAddress}", cart, address, pointAddress);

        for (var point = 1; point <= 3; point++)
            result.PickupPoints.Add(new PickupPoint
            {
                Id = point.ToString(),
                Name = $"Test pint name {point}",
                Description = $"Test pint description {point}",
                Address = pointAddress.Address1,
                City = pointAddress.City,
                County = pointAddress.County,
                StateAbbreviation = (await _stateProvinceService.GetStateProvinceByAddressAsync(pointAddress))?.Abbreviation ?? string.Empty,
                CountryCode = (await _countryService.GetCountryByAddressAsync(pointAddress))?.TwoLetterIsoCode ?? string.Empty,
                ZipPostalCode = pointAddress.ZipPostalCode,
                DisplayOrder = point,
                ProviderSystemName = PluginDescriptor.SystemName
            });

		_logger.LogDebug("<<< Exiting GetPickupPointsAsync(cart={Cart},address={Address})", cart, address);
        return result;
    }

    public Task<IShipmentTracker> GetShipmentTrackerAsync()
    {
		_logger.LogDebug(">>> Entering GetShipmentTrackerAsync()");
		_logger.LogDebug("<<< Exiting GetShipmentTrackerAsync()");
        return Task.FromResult<IShipmentTracker>(null);
    }

    #endregion
}
