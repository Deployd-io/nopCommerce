using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Plugins;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Shipping.Tracking;

namespace Nop.Tests.Nop.Services.Tests.Shipping;

internal class PickupPointTestProvider : BasePlugin, IPickupPointProvider
{
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
        _addressService = addressService;
        _countryService = countryService;
        _stateProvinceService = stateProvinceService;
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
		logger.LogDebug(">>> Entering GetPickupPointsAsync(cart={Cart},address={Address})", cart, address);
        var result = new GetPickupPointsResponse();

		logger.LogDebug("GetPickupPointsAsync(cart={Cart},address={Address}): result → {Result}", cart, address, result);
        var pointAddress = await _addressService.GetAddressByIdAsync(1);

		logger.LogDebug("GetPickupPointsAsync(cart={Cart},address={Address}): pointAddress → {PointAddress}", cart, address, pointAddress);
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

		logger.LogDebug("<<< Exiting GetPickupPointsAsync(cart={Cart},address={Address})", cart, address);
        return result;
    }

    public Task<IShipmentTracker> GetShipmentTrackerAsync()
    {
		logger.LogDebug(">>> Entering GetShipmentTrackerAsync()");
		logger.LogDebug("<<< Exiting GetShipmentTrackerAsync()");
        return Task.FromResult<IShipmentTracker>(null);
    }

    #endregion
}
