using System.Globalization;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Http;
using Nop.Plugin.Shipping.UPS.API.OAuth;
using Nop.Plugin.Shipping.UPS.API.Rates;
using Nop.Plugin.Shipping.UPS.API.Track;
using Nop.Plugin.Shipping.UPS.Domain;
using Nop.Services;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.UPS.Services;

/// <summary>
/// Represents UPS service
/// </summary>
public class UPSService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Constants

    /// <summary>
    /// Package weight limit (lbs) https://www.ups.com/us/en/help-center/packaging-and-supplies/prepare-overize.page
    /// </summary>
    private const int WEIGHT_LIMIT = 150;

    /// <summary>
    /// Packahe size limit (inches in length and girth combined) https://www.ups.com/us/en/help-center/packaging-and-supplies/prepare-overize.page
    /// </summary>
    private const int SIZE_LIMIT = 165;

    /// <summary>
    /// Package length limit (inches) https://www.ups.com/us/en/help-center/packaging-and-supplies/prepare-overize.page
    /// </summary>
    private const int LENGTH_LIMIT = 108;

    #endregion

    #region Fields

    private readonly CurrencySettings _currencySettings;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICountryService _countryService;
    private readonly ICurrencyService _currencyService;
    private readonly ILocalizationService _localizationService;
    private readonly ILogger _logger;
    private readonly IMeasureService _measureService;
    private readonly IOrderTotalCalculationService _orderTotalCalculationService;
    private readonly IShippingService _shippingService;
    private readonly IStateProvinceService _stateProvinceService;
    private readonly IWorkContext _workContext;
    private readonly UPSSettings _upsSettings;

    private MeasureDimension _measureDimension;
    private MeasureWeight _measureWeight;
    private MeasureDimension _inchesDimension;
    private MeasureWeight _lbWeight;

    private string _accessToken;

    #endregion

    #region Ctor

    public UPSService(CurrencySettings currencySettings,
        IHttpClientFactory httpClientFactory,
        ICountryService countryService,
        ICurrencyService currencyService,
        ILocalizationService localizationService,
        ILogger logger,
        IMeasureService measureService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IShippingService shippingService,
        IStateProvinceService stateProvinceService,
        IWorkContext workContext,
        UPSSettings upsSettings)
    {
        _currencySettings = currencySettings;
		_logger.LogDebug("UPSService(currencySettings={CurrencySettings},httpClientFactory={HttpClientFactory},countryService={CountryService},currencyService={CurrencyService},localizationService={LocalizationService},logger={Logger},measureService={MeasureService},orderTotalCalculationService={OrderTotalCalculationService},shippingService=<redacted>,stateProvinceService={StateProvinceService},workContext={WorkContext},upsSettings={UpsSettings}): _currencySettings → {_currencySettings}", currencySettings, httpClientFactory, countryService, currencyService, localizationService, logger, measureService, orderTotalCalculationService, stateProvinceService, workContext, upsSettings, _currencySettings);
        _httpClientFactory = httpClientFactory;
		_logger.LogDebug("UPSService(currencySettings={CurrencySettings},httpClientFactory={HttpClientFactory},countryService={CountryService},currencyService={CurrencyService},localizationService={LocalizationService},logger={Logger},measureService={MeasureService},orderTotalCalculationService={OrderTotalCalculationService},shippingService=<redacted>,stateProvinceService={StateProvinceService},workContext={WorkContext},upsSettings={UpsSettings}): _httpClientFactory → {_httpClientFactory}", currencySettings, httpClientFactory, countryService, currencyService, localizationService, logger, measureService, orderTotalCalculationService, stateProvinceService, workContext, upsSettings, _httpClientFactory);
        _countryService = countryService;
		_logger.LogDebug("UPSService(currencySettings={CurrencySettings},httpClientFactory={HttpClientFactory},countryService={CountryService},currencyService={CurrencyService},localizationService={LocalizationService},logger={Logger},measureService={MeasureService},orderTotalCalculationService={OrderTotalCalculationService},shippingService=<redacted>,stateProvinceService={StateProvinceService},workContext={WorkContext},upsSettings={UpsSettings}): _countryService → {_countryService}", currencySettings, httpClientFactory, countryService, currencyService, localizationService, logger, measureService, orderTotalCalculationService, stateProvinceService, workContext, upsSettings, _countryService);
        _currencyService = currencyService;
		_logger.LogDebug("UPSService(currencySettings={CurrencySettings},httpClientFactory={HttpClientFactory},countryService={CountryService},currencyService={CurrencyService},localizationService={LocalizationService},logger={Logger},measureService={MeasureService},orderTotalCalculationService={OrderTotalCalculationService},shippingService=<redacted>,stateProvinceService={StateProvinceService},workContext={WorkContext},upsSettings={UpsSettings}): _currencyService → {_currencyService}", currencySettings, httpClientFactory, countryService, currencyService, localizationService, logger, measureService, orderTotalCalculationService, stateProvinceService, workContext, upsSettings, _currencyService);
        _localizationService = localizationService;
		_logger.LogDebug("UPSService(currencySettings={CurrencySettings},httpClientFactory={HttpClientFactory},countryService={CountryService},currencyService={CurrencyService},localizationService={LocalizationService},logger={Logger},measureService={MeasureService},orderTotalCalculationService={OrderTotalCalculationService},shippingService=<redacted>,stateProvinceService={StateProvinceService},workContext={WorkContext},upsSettings={UpsSettings}): _localizationService → {_localizationService}", currencySettings, httpClientFactory, countryService, currencyService, localizationService, logger, measureService, orderTotalCalculationService, stateProvinceService, workContext, upsSettings, _localizationService);
        _logger = logger;
		_logger.LogDebug(">>> Entering UPSService(currencySettings={CurrencySettings},httpClientFactory={HttpClientFactory},countryService={CountryService},currencyService={CurrencyService},localizationService={LocalizationService},logger={Logger},measureService={MeasureService},orderTotalCalculationService={OrderTotalCalculationService},shippingService=<redacted>,stateProvinceService={StateProvinceService},workContext={WorkContext},upsSettings={UpsSettings})", currencySettings, httpClientFactory, countryService, currencyService, localizationService, logger, measureService, orderTotalCalculationService, stateProvinceService, workContext, upsSettings);
        _measureService = measureService;
		_logger.LogDebug("UPSService(currencySettings={CurrencySettings},httpClientFactory={HttpClientFactory},countryService={CountryService},currencyService={CurrencyService},localizationService={LocalizationService},logger={Logger},measureService={MeasureService},orderTotalCalculationService={OrderTotalCalculationService},shippingService=<redacted>,stateProvinceService={StateProvinceService},workContext={WorkContext},upsSettings={UpsSettings}): _measureService → {_measureService}", currencySettings, httpClientFactory, countryService, currencyService, localizationService, logger, measureService, orderTotalCalculationService, stateProvinceService, workContext, upsSettings, _measureService);
        _orderTotalCalculationService = orderTotalCalculationService;
        _shippingService = shippingService;
		_logger.LogDebug("UPSService(currencySettings={CurrencySettings},httpClientFactory={HttpClientFactory},countryService={CountryService},currencyService={CurrencyService},localizationService={LocalizationService},logger={Logger},measureService={MeasureService},orderTotalCalculationService={OrderTotalCalculationService},shippingService=<redacted>,stateProvinceService={StateProvinceService},workContext={WorkContext},upsSettings={UpsSettings}): _orderTotalCalculationService → {_orderTotalCalculationService}", currencySettings, httpClientFactory, countryService, currencyService, localizationService, logger, measureService, orderTotalCalculationService, stateProvinceService, workContext, upsSettings, _orderTotalCalculationService);
		_logger.LogDebug("UPSService(currencySettings={CurrencySettings},httpClientFactory={HttpClientFactory},countryService={CountryService},currencyService={CurrencyService},localizationService={LocalizationService},logger={Logger},measureService={MeasureService},orderTotalCalculationService={OrderTotalCalculationService},shippingService=<redacted>,stateProvinceService={StateProvinceService},workContext={WorkContext},upsSettings={UpsSettings}): _shippingService → {_shippingService}", currencySettings, httpClientFactory, countryService, currencyService, localizationService, logger, measureService, orderTotalCalculationService, stateProvinceService, workContext, upsSettings, _shippingService);
        _stateProvinceService = stateProvinceService;
        _workContext = workContext;
		_logger.LogDebug("UPSService(currencySettings={CurrencySettings},httpClientFactory={HttpClientFactory},countryService={CountryService},currencyService={CurrencyService},localizationService={LocalizationService},logger={Logger},measureService={MeasureService},orderTotalCalculationService={OrderTotalCalculationService},shippingService=<redacted>,stateProvinceService={StateProvinceService},workContext={WorkContext},upsSettings={UpsSettings}): _stateProvinceService → {_stateProvinceService}", currencySettings, httpClientFactory, countryService, currencyService, localizationService, logger, measureService, orderTotalCalculationService, stateProvinceService, workContext, upsSettings, _stateProvinceService);
		_logger.LogDebug("UPSService(currencySettings={CurrencySettings},httpClientFactory={HttpClientFactory},countryService={CountryService},currencyService={CurrencyService},localizationService={LocalizationService},logger={Logger},measureService={MeasureService},orderTotalCalculationService={OrderTotalCalculationService},shippingService=<redacted>,stateProvinceService={StateProvinceService},workContext={WorkContext},upsSettings={UpsSettings}): _workContext → {_workContext}", currencySettings, httpClientFactory, countryService, currencyService, localizationService, logger, measureService, orderTotalCalculationService, stateProvinceService, workContext, upsSettings, _workContext);
		_logger.LogDebug("<<< Exiting UPSService(currencySettings={CurrencySettings},httpClientFactory={HttpClientFactory},countryService={CountryService},currencyService={CurrencyService},localizationService={LocalizationService},logger={Logger},measureService={MeasureService},orderTotalCalculationService={OrderTotalCalculationService},shippingService=<redacted>,stateProvinceService={StateProvinceService},workContext={WorkContext},upsSettings={UpsSettings})", currencySettings, httpClientFactory, countryService, currencyService, localizationService, logger, measureService, orderTotalCalculationService, stateProvinceService, workContext, upsSettings);
        _upsSettings = upsSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get access token
    /// </summary>
    /// <returns>The asynchronous task whose result contains access token</returns>
    private async Task<string> GetAccessTokenAsync()
    {
		_logger.LogDebug(">>> Entering GetAccessTokenAsync()");
        if (!string.IsNullOrEmpty(_accessToken))
            return _accessToken;

        if (string.IsNullOrEmpty(_upsSettings.ClientId))
			_logger.LogWarning("Throwing NopException from GetAccessTokenAsync()");
            throw new NopException("Client ID is not set");

        if (string.IsNullOrEmpty(_upsSettings.ClientSecret))
            throw new NopException("Client secret is not set");

        var client = new OAuthClient(_httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient), _upsSettings);
		_logger.LogDebug("GetAccessTokenAsync(): client → {Client}", client);

        var response = await client.GenerateTokenAsync();
		_logger.LogDebug("GetAccessTokenAsync(): response → {Response}", response);
        _accessToken = response.Access_token;
		_logger.LogDebug("GetAccessTokenAsync(): _accessToken → {_accessToken}", _accessToken);

		_logger.LogDebug("<<< Exiting GetAccessTokenAsync()");
        return _accessToken;
    }

    /// <summary>
    /// Get the weight limit for the selected weight measure
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the value
    /// </returns>
    private async Task<decimal> GetWeightLimitAsync()
    {
		_logger.LogDebug(">>> Entering GetWeightLimitAsync()");
        return await _measureService.ConvertWeightAsync(WEIGHT_LIMIT, _lbWeight, _measureWeight);
    }

    /// <summary>
    /// Get the size limit for the selected dimension measure
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the value
    /// </returns>
    private async Task<decimal> GetSizeLimitAsync()
    {
		_logger.LogDebug(">>> Entering GetSizeLimitAsync()");
        return await _measureService.ConvertDimensionAsync(SIZE_LIMIT, _inchesDimension, _measureDimension);
    }

    /// <summary>
    /// Get the length limit for the selected dimension measure
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the value
    /// </returns>
    private async Task<decimal> GetLengthLimitAsync()
    {
		_logger.LogDebug(">>> Entering GetLengthLimitAsync()");
        return await _measureService.ConvertDimensionAsync(LENGTH_LIMIT, _inchesDimension, _measureDimension);
    }

    /// <summary>
    /// Gets an attribute value on an enum field value
    /// </summary>
    /// <typeparam name="TAttribute">Type of the attribute</typeparam>
    /// <param name="enumValue">Enum value</param>
    /// <returns>The attribute value</returns>
    private TAttribute GetAttributeValue<TAttribute>(Enum enumValue) where TAttribute : Attribute
    {
		_logger.LogDebug(">>> Entering GetAttributeValue(enumValue={EnumValue})", enumValue);
        var enumType = enumValue.GetType();
		_logger.LogDebug("<<< Exiting GetAttributeValue(enumValue={EnumValue})", enumValue);
        var enumValueInfo = enumType.GetMember(enumValue.ToString()).FirstOrDefault();
		_logger.LogDebug("GetAttributeValue(enumValue={EnumValue}): enumValueInfo → {EnumValueInfo}", enumValue, enumValueInfo);
        var attribute = enumValueInfo?.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault();
		_logger.LogDebug("GetAttributeValue(enumValue={EnumValue}): attribute → {Attribute}", enumValue, attribute);
		_logger.LogDebug("GetAttributeValue(enumValue={EnumValue}): enumType → {EnumType}", enumValue, enumType);
        return attribute as TAttribute;
    }

    /// <summary>
    /// Get tracking info
    /// </summary>
    /// <param name="trackingNumber">The tracking number</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the tracking info
    /// </returns>
    private async Task<TrackResponse> TrackAsync(string trackingNumber)
    {
		_logger.LogDebug(">>> Entering TrackAsync(trackingNumber={TrackingNumber})", trackingNumber);
        var client = new TrackClient(_httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient), _upsSettings, await GetAccessTokenAsync());

		_logger.LogDebug("TrackAsync(trackingNumber={TrackingNumber}): client → {Client}", trackingNumber, client);
        var trackResponse = await client.TrackAsync(trackingNumber);
		_logger.LogDebug("TrackAsync(trackingNumber={TrackingNumber}): trackResponse → {TrackResponse}", trackingNumber, trackResponse);

		_logger.LogDebug("<<< Exiting TrackAsync(trackingNumber={TrackingNumber})", trackingNumber);
        return trackResponse;
    }

    /// <summary>
    /// Prepare shipment status event by the passed track activity
    /// </summary>
    /// <param name="activity">Track activity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment status event 
    /// </returns>
    private async Task<ShipmentStatusEvent> PrepareShipmentStatusEventAsync(Activity activity)
    {
		_logger.LogDebug(">>> Entering PrepareShipmentStatusEventAsync(activity={Activity})", activity);
        var shipmentStatusEvent = new ShipmentStatusEvent();

		_logger.LogDebug("PrepareShipmentStatusEventAsync(activity={Activity}): shipmentStatusEvent → {ShipmentStatusEvent}", activity, shipmentStatusEvent);
        try
        {
            //prepare date
            shipmentStatusEvent.Date = DateTime
                .ParseExact($"{activity.Date} {activity.Time}", "yyyyMMdd HHmmss", CultureInfo.InvariantCulture);
				_logger.LogDebug("PrepareShipmentStatusEventAsync(activity={Activity}): shipmentStatusEvent.Date → {Arg1}", activity, shipmentStatusEvent.Date);

            //prepare address
            var addressDetails = new List<string>();
			_logger.LogDebug("PrepareShipmentStatusEventAsync(activity={Activity}): addressDetails → {AddressDetails}", activity, addressDetails);

            var address = activity.Location?.Address;

            if (address != null)
            {
                if (!string.IsNullOrEmpty(address.CountryCode))
                    addressDetails.Add(address.CountryCode);
                if (!string.IsNullOrEmpty(address.StateProvince))
                    addressDetails.Add(address.StateProvince);
                if (!string.IsNullOrEmpty(address.City))
                    addressDetails.Add(address.City);
                if (!string.IsNullOrEmpty(address.AddressLine1))
                    addressDetails.Add(address.AddressLine1);
                if (!string.IsNullOrEmpty(address.AddressLine2))
                    addressDetails.Add(address.AddressLine2);
                if (!string.IsNullOrEmpty(address.AddressLine3))
                    addressDetails.Add(address.AddressLine3);
                if (!string.IsNullOrEmpty(address.PostalCode))
                    addressDetails.Add(address.PostalCode);
            }

            shipmentStatusEvent.CountryCode = activity.Location?.Address?.CountryCode;
			_logger.LogDebug("PrepareShipmentStatusEventAsync(activity={Activity}): shipmentStatusEvent.CountryCode → {Arg1}", activity, shipmentStatusEvent.CountryCode);
            shipmentStatusEvent.Location = string.Join(", ", addressDetails);

            if (activity.Status == null)
                return shipmentStatusEvent;

            //prepare description
            var eventName = string.Empty;
			_logger.LogDebug("PrepareShipmentStatusEventAsync(activity={Activity}): eventName → {EventName}", activity, eventName);
            switch (activity.Status.Type)
            {
                case "I":
                    eventName = activity.Status.Code switch
                    {
                        "DP" => "Plugins.Shipping.Tracker.Departed",
                        "EP" => "Plugins.Shipping.Tracker.ExportScanned",
                        "OR" => "Plugins.Shipping.Tracker.OriginScanned",
                        _ => "Plugins.Shipping.Tracker.Arrived",
                    };
                    break;

                case "X":
                    eventName = "Plugins.Shipping.Tracker.NotDelivered";
                    break;

                case "M":
                    eventName = "Plugins.Shipping.Tracker.Booked";
                    break;

                case "D":
                    eventName = "Plugins.Shipping.Tracker.Delivered";
                    break;

                case "P":
                    eventName = "Plugins.Shipping.Tracker.Pickup";
                    break;
            }
            shipmentStatusEvent.EventName = await _localizationService.GetResourceAsync(eventName);
        }
        catch
        {
			_logger.LogError("Exception in PrepareShipmentStatusEventAsync(activity={Activity}): {Arg1}", activity, ex.Message);
            // ignored
        }

		_logger.LogDebug("<<< Exiting PrepareShipmentStatusEventAsync(activity={Activity})", activity);
        return shipmentStatusEvent;
    }

    /// <summary>
    /// Get rates
    /// </summary>
    /// <param name="request">Request details</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the rates info
    /// </returns>
    private async Task<RateResponse> GetRatesAsync(RateRequest request)
    {
		_logger.LogDebug(">>> Entering GetRatesAsync(request={Request})", request);
        var client = new RateClient(_httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient), _upsSettings, await GetAccessTokenAsync());

		_logger.LogDebug("GetRatesAsync(request={Request}): client → {Client}", request, client);
        //try to get response details
        var response = await client.ProcessRateAsync(request);
		_logger.LogDebug("GetRatesAsync(request={Request}): response → {Response}", request, response);

		_logger.LogDebug("<<< Exiting GetRatesAsync(request={Request})", request);
		_logger.LogDebug("<<< Exiting GetRatesAsync(shippingOptionRequest=<redacted>)");
        return response;
    }

    /// <summary>
    /// Create request details to get shipping rates
    /// </summary>
    /// <param name="shippingOptionRequest">Shipping option request</param>
    /// <param name="saturdayDelivery">Whether to get rates for Saturday Delivery</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the rate request details
    /// </returns>
    private async Task<RateRequest> CreateRateRequestAsync(GetShippingOptionRequest shippingOptionRequest, bool saturdayDelivery = false)
    {
		_logger.LogDebug(">>> Entering CreateRateRequestAsync(shippingOptionRequest=<redacted>,saturdayDelivery={SaturdayDelivery})", saturdayDelivery);
        //set request details
        var request = new RateRequest
        {
            Request = new RateRequest_Request
            {
                //used to define the request type.
                //valid values:
                //  * Rate = the server rates(The default Request option is Rate if a Request Option is not provided).
                //  * Shop = the server validates the shipment, and returns rates for all UPS products from the ShipFrom to the ShipTo addresses.
                //  * Ratetimeintransit = the server rates with transit time information
                //  * Shoptimeintransit = the server validates the shipment, and returns rates and transit times for all UPS products from the ShipFrom to the ShipTo addresses.
                //Rate is the only valid request option for UPS Ground Freight Pricing requests.
                RequestOption = "Shoptimeintransit"
            }
        };

        //prepare addresses details
        var stateCodeTo = (await _stateProvinceService.GetStateProvinceByAddressAsync(shippingOptionRequest.ShippingAddress))?.Abbreviation;
		_logger.LogDebug("CreateRateRequestAsync(shippingOptionRequest=<redacted>,saturdayDelivery={SaturdayDelivery}): stateCodeTo → {StateCodeTo}", saturdayDelivery, stateCodeTo);
        var stateCodeFrom = shippingOptionRequest.StateProvinceFrom?.Abbreviation;
        var countryCodeFrom = (shippingOptionRequest.CountryFrom ?? (await _countryService.GetAllCountriesAsync()).FirstOrDefault())?.TwoLetterIsoCode ?? string.Empty;
		_logger.LogDebug("CreateRateRequestAsync(shippingOptionRequest=<redacted>,saturdayDelivery={SaturdayDelivery}): countryCodeFrom → {CountryCodeFrom}", saturdayDelivery, countryCodeFrom);

        var addressFrom = new Shipper_Address
        {
            AddressLine = new[] { shippingOptionRequest.AddressFrom },
            City = shippingOptionRequest.CityFrom,
            StateProvinceCode = stateCodeFrom,
            CountryCode = countryCodeFrom,
            PostalCode = shippingOptionRequest.ZipPostalCodeFrom
        };

        var addressFromDetails = new ShipFrom_Address
        {
            AddressLine = new[] { shippingOptionRequest.AddressFrom },
            City = shippingOptionRequest.CityFrom,
            StateProvinceCode = stateCodeFrom,
            CountryCode = countryCodeFrom,
            PostalCode = shippingOptionRequest.ZipPostalCodeFrom
        };

        var addressToDetails = new ShipTo_Address
        {
            AddressLine = new[] { shippingOptionRequest.ShippingAddress.Address1, shippingOptionRequest.ShippingAddress.Address2 },
            City = shippingOptionRequest.ShippingAddress.City,
            StateProvinceCode = stateCodeTo,
            CountryCode = (await _countryService.GetCountryByAddressAsync(shippingOptionRequest.ShippingAddress))?.TwoLetterIsoCode,
            PostalCode = shippingOptionRequest.ShippingAddress.ZipPostalCode,
            ResidentialAddressIndicator = string.Empty
        };

        //set shipment details
        request.Shipment = new RateRequest_Shipment
		_logger.LogDebug("CreateRateRequestAsync(shippingOptionRequest=<redacted>,saturdayDelivery={SaturdayDelivery}): request.Shipment → {Arg2}", saturdayDelivery, request.Shipment);
        {
            Shipper = new Shipment_Shipper
            {
                ShipperNumber = _upsSettings.AccountNumber,
                Address = addressFrom
            },
            ShipFrom = new Shipment_ShipFrom
            {
                Address = addressFromDetails
            },
            ShipTo = new Shipment_ShipTo
            {
                Address = addressToDetails
            },
            DeliveryTimeInformation = new Shipment_DeliveryTimeInformation
            {
                //valid values are:
                //  * 02 - Document only
                //  * 03 - Non-Document
                //  * 04 - WWEF Pallet
                //  * 07 - Domestic Pallet
                //if 04 is included, Worldwide Express Freight and UPS Worldwide Express Freight Midday services (if applicable)
                //will be included in the response.
                PackageBillType = "03",
                Pickup = new DeliveryTimeInformation_Pickup
                {
                    Date = DateTime.UtcNow.ToLocalTime().Date.AddDays(1).ToString("yyyyMMdd"),
                }
            }
        };

        //set pickup options and customer classification for US shipments
        if (countryCodeFrom.Equals("US", StringComparison.InvariantCultureIgnoreCase))
        {
            request.PickupType = new RateRequest_PickupType
			_logger.LogDebug("CreateRateRequestAsync(shippingOptionRequest=<redacted>,saturdayDelivery={SaturdayDelivery}): request.PickupType → {Arg2}", saturdayDelivery, request.PickupType);
            {
                Code = GetUpsCode(_upsSettings.PickupType)
            };
            request.CustomerClassification = new RateRequest_CustomerClassification
			_logger.LogDebug("CreateRateRequestAsync(shippingOptionRequest=<redacted>,saturdayDelivery={SaturdayDelivery}): request.CustomerClassification → {Arg2}", saturdayDelivery, request.CustomerClassification);
            {
                Code = GetUpsCode(_upsSettings.CustomerClassification)
            };
        }

        //set negotiated rates details
        if (!string.IsNullOrEmpty(_upsSettings.AccountNumber) && !string.IsNullOrEmpty(stateCodeTo))
            request.Shipment.ShipmentRatingOptions = new Shipment_ShipmentRatingOptions
            {
                NegotiatedRatesIndicator = string.Empty,
                UserLevelDiscountIndicator = string.Empty
            };

        //set Saturday delivery details
        if (saturdayDelivery)
            request.Shipment.ShipmentServiceOptions = new Shipment_ShipmentServiceOptions
            {
                SaturdayDeliveryIndicator = string.Empty
            };

        //set packages details
        request.Shipment.Package = _upsSettings.PackingType switch
        {
            PackingType.PackByOneItemPerPackage => (await GetPackagesForOneItemPerPackageAsync(shippingOptionRequest)).ToArray(),
            PackingType.PackByVolume => (await GetPackagesByCubicRootAsync(shippingOptionRequest)).ToArray(),
            _ => (await GetPackagesByDimensionsAsync(shippingOptionRequest)).ToArray()
        };

        request.Shipment.ShipmentTotalWeight = new Shipment_ShipmentTotalWeight
        {
            UnitOfMeasurement = new ShipmentTotalWeight_UnitOfMeasurement
            {
                Code = _upsSettings.WeightType,
                Description = _upsSettings.WeightType
            },
            Weight = request.Shipment.Package.Sum(x => decimal.TryParse(x.PackageWeight.Weight, out var wt) ? wt : 0).ToString("F2", CultureInfo.InvariantCulture)
        };

        var currencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
		_logger.LogDebug("CreateRateRequestAsync(shippingOptionRequest=<redacted>,saturdayDelivery={SaturdayDelivery}): currencyCode → {CurrencyCode}", saturdayDelivery, currencyCode);
        request.Shipment.InvoiceLineTotal = new Shipment_InvoiceLineTotal
        {
            CurrencyCode = currencyCode,
            MonetaryValue = shippingOptionRequest.Items.Sum(x => x.Product.Price * x.GetQuantity()).ToString("F2", CultureInfo.InvariantCulture)
        };

		_logger.LogDebug("<<< Exiting CreateRateRequestAsync(shippingOptionRequest=<redacted>,saturdayDelivery={SaturdayDelivery})", saturdayDelivery);
        return request;
    }

    /// <summary>
    /// Create package details
    /// </summary>
    /// <param name="width">Width</param>
    /// <param name="length">Length</param>
    /// <param name="height">Height</param>
    /// <param name="weight">Weight</param>
    /// <param name="insuranceAmount">Insurance amount</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the package details
    /// </returns>
    private async Task<Shipment_Package> CreatePackageAsync(decimal width, decimal length, decimal height, decimal weight, decimal insuranceAmount)
    {
		_logger.LogDebug(">>> Entering CreatePackageAsync(width={Width},length={Length},height={Height},weight={Weight},insuranceAmount={InsuranceAmount})", width, length, height, weight, insuranceAmount);
        //set package details
        var package = new Shipment_Package
        {
            PackagingType = new Package_PackagingType
            {
                Code = GetUpsCode(_upsSettings.PackagingType)
            }
        };

        //set dimensions and weight details
        if (!_upsSettings.PassDimensions)
            width = length = height = 0;
        package.Dimensions = new Package_Dimensions
		_logger.LogDebug("CreatePackageAsync(width={Width},length={Length},height={Height},weight={Weight},insuranceAmount={InsuranceAmount}): package.Dimensions → {Arg5}", width, length, height, weight, insuranceAmount, package.Dimensions);
        {
            Width = width.ToString("0.00", CultureInfo.InvariantCulture),
            Length = length.ToString("0.00", CultureInfo.InvariantCulture),
            Height = height.ToString("0.00", CultureInfo.InvariantCulture),
            UnitOfMeasurement = new Dimensions_UnitOfMeasurement { Code = _upsSettings.DimensionsType, Description = _upsSettings.DimensionsType }
        };
        package.PackageWeight = new Package_PackageWeight
		_logger.LogDebug("CreatePackageAsync(width={Width},length={Length},height={Height},weight={Weight},insuranceAmount={InsuranceAmount}): package.PackageWeight → {Arg5}", width, length, height, weight, insuranceAmount, package.PackageWeight);
        {
            Weight = weight.ToString("0.00", CultureInfo.InvariantCulture),
            UnitOfMeasurement = new PackageWeight_UnitOfMeasurement { Code = _upsSettings.WeightType, Description = _upsSettings.WeightType },
        };

        //set insurance details
        if (_upsSettings.InsurePackage && insuranceAmount > decimal.Zero)
        {
            var currencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
			_logger.LogDebug("CreatePackageAsync(width={Width},length={Length},height={Height},weight={Weight},insuranceAmount={InsuranceAmount}): currencyCode → {CurrencyCode}", width, length, height, weight, insuranceAmount, currencyCode);
			_logger.LogDebug("CreatePackageAsync(width={Width},length={Length},height={Height},weight={Weight},insuranceAmount={InsuranceAmount}): package.PackageServiceOptions → {Arg5}", width, length, height, weight, insuranceAmount, package.PackageServiceOptions);
            var monetaryValue = insuranceAmount.ToString("0.00", CultureInfo.InvariantCulture);
			_logger.LogDebug("CreatePackageAsync(width={Width},length={Length},height={Height},weight={Weight},insuranceAmount={InsuranceAmount}): monetaryValue → {MonetaryValue}", width, length, height, weight, insuranceAmount, monetaryValue);
            package.PackageServiceOptions = new Package_PackageServiceOptions
            {
                DeclaredValue = new PackageServiceOptions_DeclaredValue
                {
                    CurrencyCode = currencyCode,
                    MonetaryValue = monetaryValue
                }
            };
        }

		_logger.LogDebug("<<< Exiting CreatePackageAsync(width={Width},length={Length},height={Height},weight={Weight},insuranceAmount={InsuranceAmount})", width, length, height, weight, insuranceAmount);
        return package;
    }

    /// <summary>
    /// Create packages (each shopping cart item is a separate package)
    /// </summary>
    /// <param name="shippingOptionRequest">shipping option request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the packages
    /// </returns>
    private async Task<IEnumerable<Shipment_Package>> GetPackagesForOneItemPerPackageAsync(GetShippingOptionRequest shippingOptionRequest)
    {
        return await shippingOptionRequest.Items.SelectManyAwait(async packageItem =>
        {
			_logger.LogDebug(">>> Entering GetPackagesForOneItemPerPackageAsync(shippingOptionRequest=<redacted>)");
            //get dimensions and weight of the single item
            var (width, length, height) = await GetDimensionsForSingleItemAsync(packageItem.ShoppingCartItem, packageItem.Product);
            var weight = await GetWeightForSingleItemAsync(packageItem.ShoppingCartItem, shippingOptionRequest.Customer, packageItem.Product);

            var insuranceAmount = 0;
            if (_upsSettings.InsurePackage)
                //The maximum declared amount per package: 50000 USD.
                insuranceAmount = Convert.ToInt32(packageItem.Product.Price);

            //create packages according to item quantity
            var package = await CreatePackageAsync(width, length, height, weight, insuranceAmount);

			_logger.LogInformation("GetPackagesForOneItemPerPackageAsync(shippingOptionRequest)=<redacted>: database query executed in {Arg1} ms", __logStart.ElapsedMilliseconds);
			_logger.LogDebug("<<< Exiting GetPackagesForOneItemPerPackageAsync(shippingOptionRequest=<redacted>)");
            return Enumerable.Repeat(package, packageItem.GetQuantity());
        }).ToListAsync();
    }

    /// <summary>
    /// Create packages (total dimensions of shopping cart items determines number of packages)
    /// </summary>
    /// <param name="shippingOptionRequest">shipping option request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the packages
    /// </returns>
    private async Task<IEnumerable<Shipment_Package>> GetPackagesByDimensionsAsync(GetShippingOptionRequest shippingOptionRequest)
    {
		_logger.LogDebug(">>> Entering GetPackagesByDimensionsAsync(shippingOptionRequest=<redacted>)");
        //get dimensions and weight of the whole package
        var (width, length, height) = await GetDimensionsAsync(shippingOptionRequest.Items);
        var weight = await GetWeightAsync(shippingOptionRequest);
		_logger.LogDebug("GetPackagesByDimensionsAsync(shippingOptionRequest=<redacted>): weight → {Weight}", weight);

        //whether the package doesn't exceed the weight and size limits
        var weightLimit = await GetWeightLimitAsync();
		_logger.LogDebug("GetPackagesByDimensionsAsync(shippingOptionRequest=<redacted>): weightLimit → {WeightLimit}", weightLimit);
        var sizeLimit = await GetSizeLimitAsync();
		_logger.LogDebug("GetPackagesByDimensionsAsync(shippingOptionRequest=<redacted>): sizeLimit → {SizeLimit}", sizeLimit);
        if (weight <= weightLimit && GetPackageSize(width, length, height) <= sizeLimit)
        {
            var insuranceAmount = 0;
			_logger.LogDebug("GetPackagesByDimensionsAsync(shippingOptionRequest=<redacted>): insuranceAmount → {InsuranceAmount}", insuranceAmount);
            if (_upsSettings.InsurePackage)
            {
                //The maximum declared amount per package: 50000 USD.
                //use subTotalWithoutDiscount as insured value
                var cart = shippingOptionRequest.Items.Select(item =>
                {
                    var shoppingCartItem = item.ShoppingCartItem;
                    shoppingCartItem.Quantity = item.GetQuantity();
                    return shoppingCartItem;
                }).ToList();
                var (_, _, subTotalWithoutDiscount, _, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, false);
                insuranceAmount = Convert.ToInt32(subTotalWithoutDiscount);
            }

			_logger.LogDebug(">>> Entering GetPackageSize(width={Width},length={Length},height={Height})", width, length, height);
			_logger.LogDebug("<<< Exiting GetPackageSize(width={Width},length={Length},height={Height})", width, length, height);
            return new[] { await CreatePackageAsync(width, length, height, weight, insuranceAmount) };
        }

        //get total packages number according to package limits
        var totalPackagesByWeightLimit = weight > weightLimit
            ? Convert.ToInt32(Math.Ceiling(weight / weightLimit))
            : 1;
        var totalPackagesBySizeLimit = GetPackageSize(width, length, height) > sizeLimit
            ? Convert.ToInt32(Math.Ceiling(GetPackageSize(width, length, height) / await GetLengthLimitAsync()))
            : 1;
        var totalPackages = Math.Max(Math.Max(totalPackagesBySizeLimit, totalPackagesByWeightLimit), 1);
		_logger.LogDebug("GetPackagesByDimensionsAsync(shippingOptionRequest=<redacted>): totalPackages → {TotalPackages}", totalPackages);

        width = Math.Max(width / totalPackages, 1);
        length = Math.Max(length / totalPackages, 1);
        height = Math.Max(height / totalPackages, 1);
        weight = Math.Max(weight / totalPackages, 1);

		_logger.LogDebug("GetPackagesByDimensionsAsync(shippingOptionRequest=<redacted>): width → {Width}", width);
		_logger.LogDebug("GetPackagesByDimensionsAsync(shippingOptionRequest=<redacted>): length → {Length}", length);
		_logger.LogDebug("GetPackagesByDimensionsAsync(shippingOptionRequest=<redacted>): height → {Height}", height);
        var insuranceAmountPerPackage = 0;
		_logger.LogDebug("GetPackagesByDimensionsAsync(shippingOptionRequest=<redacted>): insuranceAmountPerPackage → {InsuranceAmountPerPackage}", insuranceAmountPerPackage);
        if (_upsSettings.InsurePackage)
        {
            //The maximum declared amount per package: 50000 USD.
            //use subTotalWithoutDiscount as insured value
            var cart = shippingOptionRequest.Items.Select(item =>
            {
                var shoppingCartItem = item.ShoppingCartItem;
                shoppingCartItem.Quantity = item.GetQuantity();
                return shoppingCartItem;
            }).ToList();
            var (_, _, subTotalWithoutDiscount, _, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, false);
            insuranceAmountPerPackage = Convert.ToInt32(subTotalWithoutDiscount / totalPackages);
        }

        //create packages according to calculated value
        var package = await CreatePackageAsync(width, length, height, weight, insuranceAmountPerPackage);
		_logger.LogDebug("GetPackagesByDimensionsAsync(shippingOptionRequest=<redacted>): package → {Package}", package);
		_logger.LogDebug("<<< Exiting GetPackagesByDimensionsAsync(shippingOptionRequest=<redacted>)");
        return Enumerable.Repeat(package, totalPackages);
    }

    /// <summary>
    /// Create packages (total volume of shopping cart items determines number of packages)
    /// </summary>
    /// <param name="shippingOptionRequest">shipping option request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the packages
    /// </returns>
    private async Task<IEnumerable<Shipment_Package>> GetPackagesByCubicRootAsync(GetShippingOptionRequest shippingOptionRequest)
    {
		_logger.LogDebug(">>> Entering GetPackagesByCubicRootAsync(shippingOptionRequest=<redacted>)");
        ArgumentNullException.ThrowIfNull(shippingOptionRequest);

        //Dimensional weight is based on volume (the amount of space a package occupies in relation to its actual weight). 
        //If the cubic size of package measures three cubic feet (5,184 cubic inches or 84,951 cubic centimetres) or greater, you will be charged the greater of the dimensional weight or the actual weight.
        //This algorithm devides total package volume by the UPS settings PackingPackageVolume so that no package requires dimensional weight; this could result in an under-charge.

        var totalPackagesBySizeLimit = 1;
		_logger.LogDebug("GetPackagesByCubicRootAsync(shippingOptionRequest=<redacted>): totalPackagesBySizeLimit → {TotalPackagesBySizeLimit}", totalPackagesBySizeLimit);
        decimal width;
        decimal length;
        decimal height;

        //if there is only one item, no need to calculate dimensions
        if (shippingOptionRequest.Items.Count == 1 && shippingOptionRequest.Items?.FirstOrDefault()?.GetQuantity() == 1)
        {
            //get dimensions and weight of the single cubic size of package
            var item = shippingOptionRequest.Items.FirstOrDefault();
			_logger.LogDebug("GetPackagesByCubicRootAsync(shippingOptionRequest=<redacted>): item → {Item}", item);
            (width, length, height) = await GetDimensionsForSingleItemAsync(item.ShoppingCartItem, item.Product);
        }
        else
        {
            //or try to get them
            var dimension = 0;
			_logger.LogDebug("GetPackagesByCubicRootAsync(shippingOptionRequest=<redacted>): dimension → {Dimension}", dimension);

            //get total volume of the package
            var totalVolume = await shippingOptionRequest.Items.SumAwaitAsync(async item =>
            {
                //get dimensions and weight of the single item
                var (itemWidth, itemLength, itemHeight) = await GetDimensionsForSingleItemAsync(item.ShoppingCartItem, item.Product);
                return item.GetQuantity() * itemWidth * itemLength * itemHeight;
            });
            if (totalVolume > decimal.Zero)
            {
                //use default value (in cubic inches) if not specified
                var packageVolume = _upsSettings.PackingPackageVolume;
				_logger.LogDebug("GetPackagesByCubicRootAsync(shippingOptionRequest=<redacted>): packageVolume → {PackageVolume}", packageVolume);
                if (packageVolume <= 0)
                    packageVolume = 5184;

                //calculate cube root (floor)
                dimension = Convert.ToInt32(Math.Floor(Math.Pow(Convert.ToDouble(packageVolume), 1.0 / 3.0)));
                if (GetPackageSize(dimension, dimension, dimension) > await GetSizeLimitAsync())
					_logger.LogWarning("Throwing NopException from GetPackagesByCubicRootAsync(shippingOptionRequest=<redacted>)");
                    throw new NopException("PackingPackageVolume exceeds max package size");

                //adjust package volume for dimensions calculated
                packageVolume = dimension * dimension * dimension;

                totalPackagesBySizeLimit = Convert.ToInt32(Math.Ceiling(totalVolume / packageVolume));
            }

            width = length = height = dimension;
			_logger.LogDebug("GetPackagesByCubicRootAsync(shippingOptionRequest=<redacted>): width → {Width}", width);
        }

        //get total packages number according to package limits
        var weight = await GetWeightAsync(shippingOptionRequest);
		_logger.LogDebug("GetPackagesByCubicRootAsync(shippingOptionRequest=<redacted>): weight → {Weight}", weight);
        var weightLimit = await GetWeightLimitAsync();
		_logger.LogDebug("GetPackagesByCubicRootAsync(shippingOptionRequest=<redacted>): weightLimit → {WeightLimit}", weightLimit);
        var totalPackagesByWeightLimit = weight > weightLimit
            ? Convert.ToInt32(Math.Ceiling(weight / weightLimit))
            : 1;
        var totalPackages = Math.Max(Math.Max(totalPackagesBySizeLimit, totalPackagesByWeightLimit), 1);
		_logger.LogDebug("GetPackagesByCubicRootAsync(shippingOptionRequest=<redacted>): totalPackages → {TotalPackages}", totalPackages);

        var insuranceAmountPerPackage = 0;
		_logger.LogDebug("GetPackagesByCubicRootAsync(shippingOptionRequest=<redacted>): insuranceAmountPerPackage → {InsuranceAmountPerPackage}", insuranceAmountPerPackage);
        if (_upsSettings.InsurePackage)
        {
            //The maximum declared amount per package: 50000 USD.
            //use subTotalWithoutDiscount as insured value
            var cart = shippingOptionRequest.Items.Select(item =>
            {
                var shoppingCartItem = item.ShoppingCartItem;
                shoppingCartItem.Quantity = item.GetQuantity();
                return shoppingCartItem;
            }).ToList();
            var (_, _, subTotalWithoutDiscount, _, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, false);
            insuranceAmountPerPackage = Convert.ToInt32(subTotalWithoutDiscount / totalPackages);
        }

        //create packages according to calculated value
        var package = await CreatePackageAsync(width, length, height, weight / totalPackages, insuranceAmountPerPackage);
		_logger.LogDebug("GetPackagesByCubicRootAsync(shippingOptionRequest=<redacted>): package → {Package}", package);

		_logger.LogDebug("<<< Exiting GetPackagesByCubicRootAsync(shippingOptionRequest=<redacted>)");
        return Enumerable.Repeat(package, totalPackages);
    }

    /// <summary>
    /// Get dimensions values of the single shopping cart item
    /// </summary>
    /// <param name="item">Shopping cart item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dimensions values
    /// </returns>
    private async Task<(decimal width, decimal length, decimal height)> GetDimensionsForSingleItemAsync(ShoppingCartItem item, Product product)
    {
        var items = new[] { new GetShippingOptionRequest.PackageItem(item, product, 1) };

        return await GetDimensionsAsync(items);
    }

    /// <summary>
    /// Get dimensions values of the package
    /// </summary>
    /// <param name="items">Package items</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dimensions values
    /// </returns>
    private async Task<(decimal width, decimal length, decimal height)> GetDimensionsAsync(IList<GetShippingOptionRequest.PackageItem> items)
    {
        async Task<decimal> convertAndRoundDimensionAsync(decimal dimension)
        {
			_logger.LogDebug(">>> Entering convertAndRoundDimensionAsync(dimension={Dimension})", dimension);
            dimension = await _measureService.ConvertFromPrimaryMeasureDimensionAsync(dimension, _measureDimension);
            dimension = Convert.ToInt32(Math.Ceiling(dimension));
			_logger.LogDebug("convertAndRoundDimensionAsync(dimension={Dimension}): dimension → {Dimension1}", dimension, dimension);
			_logger.LogDebug("<<< Exiting convertAndRoundDimensionAsync(dimension={Dimension})", dimension);
            return Math.Max(dimension, 1);
        }

        var (width, length, height) = await _shippingService.GetDimensionsAsync(items, true);
        width = await convertAndRoundDimensionAsync(width);
        length = await convertAndRoundDimensionAsync(length);
        height = await convertAndRoundDimensionAsync(height);

        return (width, length, height);
    }

    /// <summary>
    /// Get weight value of the single shopping cart item
    /// </summary>
    /// <param name="item">Shopping cart item</param>
    /// <param name="customer">Customer</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the weight value
    /// </returns>
    private async Task<decimal> GetWeightForSingleItemAsync(ShoppingCartItem item, Customer customer, Product product)
    {
		_logger.LogDebug(">>> Entering GetWeightForSingleItemAsync(item={Item},customer={Customer},product={Product})", item, customer, product);
        var shippingOptionRequest = new GetShippingOptionRequest
        {
            Customer = customer,
            Items = new[] { new GetShippingOptionRequest.PackageItem(item, product, 1) }
        };

		_logger.LogDebug("<<< Exiting GetWeightForSingleItemAsync(item={Item},customer={Customer},product={Product})", item, customer, product);
        return await GetWeightAsync(shippingOptionRequest);
    }

    /// <summary>
    /// Get weight value of the package
    /// </summary>
    /// <param name="shippingOptionRequest">Shipping option request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the weight value
    /// </returns>
    private async Task<decimal> GetWeightAsync(GetShippingOptionRequest shippingOptionRequest)
    {
		_logger.LogDebug(">>> Entering GetWeightAsync(shippingOptionRequest=<redacted>)");
        var weight = await _shippingService.GetTotalWeightAsync(shippingOptionRequest, ignoreFreeShippedItems: true);
        weight = await _measureService.ConvertFromPrimaryMeasureWeightAsync(weight, _measureWeight);
		_logger.LogDebug("GetWeightAsync(shippingOptionRequest=<redacted>): weight → {Weight}", weight);
        weight = Convert.ToInt32(Math.Ceiling(weight));
		_logger.LogDebug("<<< Exiting GetWeightAsync(shippingOptionRequest=<redacted>)");
        return Math.Max(weight, 1);
    }

    /// <summary>
    /// Get package size
    /// </summary>
    /// <param name="length">Length</param>
    /// <param name="height">Height</param>
    /// <param name="width">Width</param>
    /// <returns>Package size</returns>
    private decimal GetPackageSize(decimal width, decimal length, decimal height)
    {
		_logger.LogDebug(">>> Entering GetPackageSize(width={Width},length={Length},height={Height})", width, length, height);
        //To measure ground packages use the following formula: Length + 2x Width +2x Height. Details: https://www.ups.com/us/en/help-center/packaging-and-supplies/prepare-overize.page
		_logger.LogDebug("<<< Exiting GetPackageSize(width={Width},length={Length},height={Height})", width, length, height);
        return length + width * 2 + height * 2;
    }

    /// <summary>
    /// Gets shipping rates
    /// </summary>
    /// <param name="shippingOptionRequest">Shipping option request details</param>
    /// <param name="saturdayDelivery">Whether to get rates for Saturday Delivery</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping options; errors if exist
    /// </returns>
    private async Task<(IList<ShippingOption> shippingOptions, string error)> GetShippingOptionsAsync(GetShippingOptionRequest shippingOptionRequest,
        bool saturdayDelivery = false)
    {
        try
        {
            //create request details
            var request = await CreateRateRequestAsync(shippingOptionRequest, saturdayDelivery);

            //get rate response
            var rateResponse = await GetRatesAsync(request);

            //prepare shipping options
            return ((await PrepareShippingOptionsAsync(rateResponse)).Select(shippingOption =>
            {
                //correct option name
                if (!shippingOption.Name.ToLowerInvariant().StartsWith("ups"))
                    shippingOption.Name = $"UPS {shippingOption.Name}";
                if (saturdayDelivery)
                    shippingOption.Name = $"{shippingOption.Name} - Saturday Delivery";

                //add additional handling charge
                shippingOption.Rate += _upsSettings.AdditionalHandlingCharge;

                return shippingOption;
            }).ToList(), null);
			_logger.LogDebug(">>> Entering PrepareShippingOptionsAsync(rateResponse={RateResponse})", rateResponse);
			_logger.LogDebug("<<< Exiting PrepareShippingOptionsAsync(rateResponse={RateResponse})", rateResponse);
        }
        catch (API.Rates.ApiException<ErrorResponse> exception)
        {
            //log errors
            var message = $"Error while getting UPS rates{Environment.NewLine}{string.Join(", ", exception.Result.Response.Errors.Select(p => $"{p.Code}: {p.Message}"))}";
            await _logger.ErrorAsync(message, exception, shippingOptionRequest.Customer);

            return (new List<ShippingOption>(), message);
        }
        catch (Exception exception)
        {
            //log errors
            var message = $"Error while getting UPS rates{Environment.NewLine}{exception.Message}";
            await _logger.ErrorAsync(message, exception, shippingOptionRequest.Customer);

            return (new List<ShippingOption>(), message);
        }
    }

    /// <summary>
    /// Prepare shipping options
    /// </summary>
    /// <param name="rateResponse">Rate response</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping options
    /// </returns>
    private async Task<IEnumerable<ShippingOption>> PrepareShippingOptionsAsync(RateResponse rateResponse)
    {
		_logger.LogDebug(">>> Entering PrepareShippingOptionsAsync(rateResponse={RateResponse})", rateResponse);
        var shippingOptions = new List<ShippingOption>();

		_logger.LogDebug("PrepareShippingOptionsAsync(rateResponse={RateResponse}): shippingOptions → {ShippingOptions}", rateResponse, shippingOptions);
        if (!rateResponse?.RatedShipment?.Any() ?? true)
            return shippingOptions;

        //prepare offered delivery services
        var servicesCodes = _upsSettings.CarrierServicesOffered.Split(':', StringSplitOptions.RemoveEmptyEntries)
            .Select(idValue => idValue.Trim('[', ']')).ToList();
        var allServices = (await DeliveryService.Standard.ToSelectListAsync(false)).Select(item =>
        {
            var serviceCode = GetUpsCode((DeliveryService)int.Parse(item.Value));
			_logger.LogDebug("PrepareShippingOptionsAsync(rateResponse={RateResponse}): serviceCode → {ServiceCode}", rateResponse, serviceCode);
            return new { Name = $"UPS {item.Text?.TrimStart('_')}", Code = serviceCode, Offered = servicesCodes.Contains(serviceCode) };
        }).ToList();

        //get shipping options
        foreach (var rate in rateResponse.RatedShipment)
        {
            //weed out unwanted or unknown service rates
            var serviceCode = rate.Service?.Code;
            var deliveryService = allServices.FirstOrDefault(service => service.Code == serviceCode);
			_logger.LogDebug("PrepareShippingOptionsAsync(rateResponse={RateResponse}): deliveryService → {DeliveryService}", rateResponse, deliveryService);
            if (!deliveryService?.Offered ?? true)
                continue;

            //get rate value
            var regularValue = decimal.TryParse(rate.TotalCharges?.MonetaryValue, NumberStyles.Any, new CultureInfo("en-US"), out var value)
                ? (decimal?)value
                : null;
            var negotiatedValue = decimal.TryParse(rate.NegotiatedRateCharges?.TotalCharge?.MonetaryValue, NumberStyles.Any, new CultureInfo("en-US"), out value)
                ? (decimal?)value
                : null;
            var monetaryValue = negotiatedValue ?? regularValue;
            if (!monetaryValue.HasValue)
                continue;

            //parse transit days
            int? transitDays = null;

            var serviceSummary = rate.TimeInTransit?.ServiceSummary;
			_logger.LogDebug("PrepareShippingOptionsAsync(rateResponse={RateResponse}): serviceSummary → {ServiceSummary}", rateResponse, serviceSummary);

            if (serviceSummary != null)
            {
                if (!string.IsNullOrWhiteSpace(serviceSummary.EstimatedArrival.TotalTransitDays) &&
                    int.TryParse(serviceSummary.EstimatedArrival.TotalTransitDays, out var totalTransitDays))
                    transitDays = totalTransitDays;
                else if (!string.IsNullOrEmpty(serviceSummary.EstimatedArrival.BusinessDaysInTransit) &&
                         int.TryParse(serviceSummary.EstimatedArrival.BusinessDaysInTransit, out var businessDaysInTransit))
                    transitDays = businessDaysInTransit;
						_logger.LogDebug("PrepareShippingOptionsAsync(rateResponse={RateResponse}): transitDays → {TransitDays}", rateResponse, transitDays);
            }

            //add shipping option based on service rate
            shippingOptions.Add(new ShippingOption
            {
                Rate = monetaryValue.Value,
                Name = deliveryService.Name,
                TransitDays = transitDays
            });
        }

        return shippingOptions;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get UPS code of enum value
    /// </summary>
    /// <param name="enumValue">Enum value</param>
    /// <returns>UPS code</returns>
    public string GetUpsCode(Enum enumValue)
    {
		_logger.LogDebug(">>> Entering GetUpsCode(enumValue={EnumValue})", enumValue);
        return GetAttributeValue<UPSCodeAttribute>(enumValue)?.Code;
    }

    /// <summary>
    /// Gets all events for a tracking number
    /// </summary>
    /// <param name="trackingNumber">The tracking number to track</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment events
    /// </returns>
    public virtual async Task<IEnumerable<ShipmentStatusEvent>> GetShipmentEventsAsync(string trackingNumber)
    {
		_logger.LogDebug(">>> Entering GetShipmentEventsAsync(trackingNumber={TrackingNumber})", trackingNumber);
        try
        {
            //get tracking info
            var response = await TrackAsync(trackingNumber);
			_logger.LogDebug("GetShipmentEventsAsync(trackingNumber={TrackingNumber}): response → {Response}", trackingNumber, response);

            if (response.Shipment == null)
                return null;

            return await response.Shipment
                .SelectMany(shipment => shipment.Package?
                    .SelectMany(package => package?.Activity)).Where(activity => activity != null)
                .SelectAwait(async activity => await PrepareShipmentStatusEventAsync(activity)).ToListAsync();
        }
        catch (Exception exception)
		_logger.LogDebug("GetShipmentEventsAsync(trackingNumber={TrackingNumber}): activity → {Activity}", trackingNumber, activity);
        {
            //log errors
            var message = $"Error while getting UPS shipment tracking info - {trackingNumber}{Environment.NewLine}{exception.Message}";
			_logger.LogDebug("GetShipmentEventsAsync(trackingNumber={TrackingNumber}): message → {Message}", trackingNumber, message);
            await _logger.ErrorAsync(message, exception, await _workContext.GetCurrentCustomerAsync());

			_logger.LogDebug("<<< Exiting GetShipmentEventsAsync(trackingNumber={TrackingNumber})", trackingNumber);
            return new List<ShipmentStatusEvent>();
        }
    }

    /// <summary>
    /// Gets shipping rates
    /// </summary>
    /// <param name="shippingOptionRequest">Shipping option request details</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the represents a response of getting shipping rate options
    /// </returns>
    public virtual async Task<GetShippingOptionResponse> GetRatesAsync(GetShippingOptionRequest shippingOptionRequest)
    {
        var weightSystemName = _upsSettings.WeightType switch { "LBS" => "lb", "KGS" => "kg", _ => null };
        _measureWeight = await _measureService.GetMeasureWeightBySystemKeywordAsync(weightSystemName)
                         ?? throw new NopException($"UPS shipping service. Could not load \"{weightSystemName}\" measure weight");

        _lbWeight = await _measureService.GetMeasureWeightBySystemKeywordAsync("lb")
                    ?? throw new NopException($"UPS shipping service. Could not load 'lb' measure weight (used to find limits)");

        var dimensionSystemName = _upsSettings.DimensionsType switch { "IN" => "inches", "CM" => "centimeters", _ => null };
        _measureDimension = await _measureService.GetMeasureDimensionBySystemKeywordAsync(dimensionSystemName)
                            ?? throw new NopException($"UPS shipping service. Could not load \"{dimensionSystemName}\" measure dimension");

        _inchesDimension = await _measureService.GetMeasureDimensionBySystemKeywordAsync("inches")
                           ?? throw new NopException($"UPS shipping service. Could not load 'inches' measure dimension (used to find limits)");

        var response = new GetShippingOptionResponse();

        //get regular rates
        var (shippingOptions, error) = await GetShippingOptionsAsync(shippingOptionRequest);
        response.ShippingOptions = shippingOptions;
        if (!string.IsNullOrEmpty(error))
            response.Errors.Add(error);

        //get rates for Saturday delivery
        if (_upsSettings.SaturdayDeliveryEnabled)
        {
            var (saturdayShippingOptions, saturdayError) = await GetShippingOptionsAsync(shippingOptionRequest, true);
            foreach (var shippingOption in saturdayShippingOptions)
            {
                response.ShippingOptions.Add(shippingOption);
            }
            if (!string.IsNullOrEmpty(saturdayError))
                response.Errors.Add(saturdayError);
        }

        if (response.ShippingOptions.Any())
            response.Errors.Clear();

        return response;
    }

    #endregion
}