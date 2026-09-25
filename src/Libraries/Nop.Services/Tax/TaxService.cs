using System.Text.RegularExpressions;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Tax.Events;

namespace Nop.Services.Tax;

/// <summary>
/// Tax service
/// </summary>
public partial class TaxService : ITaxService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly IAddressService _addressService;
    protected readonly ICheckVatService _checkVatService;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerService _customerService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IGeoLookupService _geoLookupService;
    protected readonly ILogger _logger;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly ITaxPluginManager _taxPluginManager;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly ShippingSettings _shippingSettings;
    protected readonly TaxSettings _taxSettings;

    #endregion

    #region Ctor

    public TaxService(AddressSettings addressSettings,
        CustomerSettings customerSettings,
        IAddressService addressService,
        ICheckVatService checkVatService,
        ICountryService countryService,
        ICustomerService customerService,
        IEventPublisher eventPublisher,
        IGenericAttributeService genericAttributeService,
        IGeoLookupService geoLookupService,
        ILogger logger,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        ITaxPluginManager taxPluginManager,
        IWebHelper webHelper,
        IWorkContext workContext,
        ShippingSettings shippingSettings,
        TaxSettings taxSettings)
    {
        _addressSettings = addressSettings;
		_logger.LogDebug("TaxService(addressSettings={AddressSettings},customerSettings={CustomerSettings},addressService={AddressService},checkVatService={CheckVatService},countryService={CountryService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},geoLookupService={GeoLookupService},logger={Logger},stateProvinceService={StateProvinceService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},webHelper={WebHelper},workContext={WorkContext},shippingSettings=<redacted>,taxSettings={TaxSettings}): _addressSettings → {_addressSettings}", addressSettings, customerSettings, addressService, checkVatService, countryService, customerService, eventPublisher, genericAttributeService, geoLookupService, logger, stateProvinceService, storeContext, taxPluginManager, webHelper, workContext, taxSettings, _addressSettings);
        _customerSettings = customerSettings;
		_logger.LogDebug("TaxService(addressSettings={AddressSettings},customerSettings={CustomerSettings},addressService={AddressService},checkVatService={CheckVatService},countryService={CountryService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},geoLookupService={GeoLookupService},logger={Logger},stateProvinceService={StateProvinceService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},webHelper={WebHelper},workContext={WorkContext},shippingSettings=<redacted>,taxSettings={TaxSettings}): _customerSettings → {_customerSettings}", addressSettings, customerSettings, addressService, checkVatService, countryService, customerService, eventPublisher, genericAttributeService, geoLookupService, logger, stateProvinceService, storeContext, taxPluginManager, webHelper, workContext, taxSettings, _customerSettings);
        _addressService = addressService;
		_logger.LogDebug("TaxService(addressSettings={AddressSettings},customerSettings={CustomerSettings},addressService={AddressService},checkVatService={CheckVatService},countryService={CountryService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},geoLookupService={GeoLookupService},logger={Logger},stateProvinceService={StateProvinceService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},webHelper={WebHelper},workContext={WorkContext},shippingSettings=<redacted>,taxSettings={TaxSettings}): _addressService → {_addressService}", addressSettings, customerSettings, addressService, checkVatService, countryService, customerService, eventPublisher, genericAttributeService, geoLookupService, logger, stateProvinceService, storeContext, taxPluginManager, webHelper, workContext, taxSettings, _addressService);
        _checkVatService = checkVatService;
		_logger.LogDebug("TaxService(addressSettings={AddressSettings},customerSettings={CustomerSettings},addressService={AddressService},checkVatService={CheckVatService},countryService={CountryService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},geoLookupService={GeoLookupService},logger={Logger},stateProvinceService={StateProvinceService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},webHelper={WebHelper},workContext={WorkContext},shippingSettings=<redacted>,taxSettings={TaxSettings}): _checkVatService → {_checkVatService}", addressSettings, customerSettings, addressService, checkVatService, countryService, customerService, eventPublisher, genericAttributeService, geoLookupService, logger, stateProvinceService, storeContext, taxPluginManager, webHelper, workContext, taxSettings, _checkVatService);
        _countryService = countryService;
		_logger.LogDebug("TaxService(addressSettings={AddressSettings},customerSettings={CustomerSettings},addressService={AddressService},checkVatService={CheckVatService},countryService={CountryService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},geoLookupService={GeoLookupService},logger={Logger},stateProvinceService={StateProvinceService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},webHelper={WebHelper},workContext={WorkContext},shippingSettings=<redacted>,taxSettings={TaxSettings}): _countryService → {_countryService}", addressSettings, customerSettings, addressService, checkVatService, countryService, customerService, eventPublisher, genericAttributeService, geoLookupService, logger, stateProvinceService, storeContext, taxPluginManager, webHelper, workContext, taxSettings, _countryService);
        _customerService = customerService;
		_logger.LogDebug("TaxService(addressSettings={AddressSettings},customerSettings={CustomerSettings},addressService={AddressService},checkVatService={CheckVatService},countryService={CountryService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},geoLookupService={GeoLookupService},logger={Logger},stateProvinceService={StateProvinceService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},webHelper={WebHelper},workContext={WorkContext},shippingSettings=<redacted>,taxSettings={TaxSettings}): _customerService → {_customerService}", addressSettings, customerSettings, addressService, checkVatService, countryService, customerService, eventPublisher, genericAttributeService, geoLookupService, logger, stateProvinceService, storeContext, taxPluginManager, webHelper, workContext, taxSettings, _customerService);
        _eventPublisher = eventPublisher;
		_logger.LogDebug("TaxService(addressSettings={AddressSettings},customerSettings={CustomerSettings},addressService={AddressService},checkVatService={CheckVatService},countryService={CountryService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},geoLookupService={GeoLookupService},logger={Logger},stateProvinceService={StateProvinceService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},webHelper={WebHelper},workContext={WorkContext},shippingSettings=<redacted>,taxSettings={TaxSettings}): _eventPublisher → {_eventPublisher}", addressSettings, customerSettings, addressService, checkVatService, countryService, customerService, eventPublisher, genericAttributeService, geoLookupService, logger, stateProvinceService, storeContext, taxPluginManager, webHelper, workContext, taxSettings, _eventPublisher);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("TaxService(addressSettings={AddressSettings},customerSettings={CustomerSettings},addressService={AddressService},checkVatService={CheckVatService},countryService={CountryService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},geoLookupService={GeoLookupService},logger={Logger},stateProvinceService={StateProvinceService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},webHelper={WebHelper},workContext={WorkContext},shippingSettings=<redacted>,taxSettings={TaxSettings}): _genericAttributeService → {_genericAttributeService}", addressSettings, customerSettings, addressService, checkVatService, countryService, customerService, eventPublisher, genericAttributeService, geoLookupService, logger, stateProvinceService, storeContext, taxPluginManager, webHelper, workContext, taxSettings, _genericAttributeService);
        _geoLookupService = geoLookupService;
		_logger.LogDebug("TaxService(addressSettings={AddressSettings},customerSettings={CustomerSettings},addressService={AddressService},checkVatService={CheckVatService},countryService={CountryService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},geoLookupService={GeoLookupService},logger={Logger},stateProvinceService={StateProvinceService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},webHelper={WebHelper},workContext={WorkContext},shippingSettings=<redacted>,taxSettings={TaxSettings}): _geoLookupService → {_geoLookupService}", addressSettings, customerSettings, addressService, checkVatService, countryService, customerService, eventPublisher, genericAttributeService, geoLookupService, logger, stateProvinceService, storeContext, taxPluginManager, webHelper, workContext, taxSettings, _geoLookupService);
        _logger = logger;
		_logger.LogError(">>> Entering TaxService(addressSettings={AddressSettings},customerSettings={CustomerSettings},addressService={AddressService},checkVatService={CheckVatService},countryService={CountryService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},geoLookupService={GeoLookupService},logger={Logger},stateProvinceService={StateProvinceService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},webHelper={WebHelper},workContext={WorkContext},shippingSettings=<redacted>,taxSettings={TaxSettings})", addressSettings, customerSettings, addressService, checkVatService, countryService, customerService, eventPublisher, genericAttributeService, geoLookupService, logger, stateProvinceService, storeContext, taxPluginManager, webHelper, workContext, taxSettings);
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
		_logger.LogDebug("TaxService(addressSettings={AddressSettings},customerSettings={CustomerSettings},addressService={AddressService},checkVatService={CheckVatService},countryService={CountryService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},geoLookupService={GeoLookupService},logger={Logger},stateProvinceService={StateProvinceService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},webHelper={WebHelper},workContext={WorkContext},shippingSettings=<redacted>,taxSettings={TaxSettings}): _stateProvinceService → {_stateProvinceService}", addressSettings, customerSettings, addressService, checkVatService, countryService, customerService, eventPublisher, genericAttributeService, geoLookupService, logger, stateProvinceService, storeContext, taxPluginManager, webHelper, workContext, taxSettings, _stateProvinceService);
		_logger.LogDebug("TaxService(addressSettings={AddressSettings},customerSettings={CustomerSettings},addressService={AddressService},checkVatService={CheckVatService},countryService={CountryService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},geoLookupService={GeoLookupService},logger={Logger},stateProvinceService={StateProvinceService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},webHelper={WebHelper},workContext={WorkContext},shippingSettings=<redacted>,taxSettings={TaxSettings}): _storeContext → {_storeContext}", addressSettings, customerSettings, addressService, checkVatService, countryService, customerService, eventPublisher, genericAttributeService, geoLookupService, logger, stateProvinceService, storeContext, taxPluginManager, webHelper, workContext, taxSettings, _storeContext);
        _taxPluginManager = taxPluginManager;
        _webHelper = webHelper;
		_logger.LogDebug("TaxService(addressSettings={AddressSettings},customerSettings={CustomerSettings},addressService={AddressService},checkVatService={CheckVatService},countryService={CountryService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},geoLookupService={GeoLookupService},logger={Logger},stateProvinceService={StateProvinceService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},webHelper={WebHelper},workContext={WorkContext},shippingSettings=<redacted>,taxSettings={TaxSettings}): _taxPluginManager → {_taxPluginManager}", addressSettings, customerSettings, addressService, checkVatService, countryService, customerService, eventPublisher, genericAttributeService, geoLookupService, logger, stateProvinceService, storeContext, taxPluginManager, webHelper, workContext, taxSettings, _taxPluginManager);
		_logger.LogDebug("TaxService(addressSettings={AddressSettings},customerSettings={CustomerSettings},addressService={AddressService},checkVatService={CheckVatService},countryService={CountryService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},geoLookupService={GeoLookupService},logger={Logger},stateProvinceService={StateProvinceService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},webHelper={WebHelper},workContext={WorkContext},shippingSettings=<redacted>,taxSettings={TaxSettings}): _webHelper → {_webHelper}", addressSettings, customerSettings, addressService, checkVatService, countryService, customerService, eventPublisher, genericAttributeService, geoLookupService, logger, stateProvinceService, storeContext, taxPluginManager, webHelper, workContext, taxSettings, _webHelper);
        _workContext = workContext;
        _shippingSettings = shippingSettings;
		_logger.LogDebug("TaxService(addressSettings={AddressSettings},customerSettings={CustomerSettings},addressService={AddressService},checkVatService={CheckVatService},countryService={CountryService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},geoLookupService={GeoLookupService},logger={Logger},stateProvinceService={StateProvinceService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},webHelper={WebHelper},workContext={WorkContext},shippingSettings=<redacted>,taxSettings={TaxSettings}): _workContext → {_workContext}", addressSettings, customerSettings, addressService, checkVatService, countryService, customerService, eventPublisher, genericAttributeService, geoLookupService, logger, stateProvinceService, storeContext, taxPluginManager, webHelper, workContext, taxSettings, _workContext);
        _taxSettings = taxSettings;
		_logger.LogWarning("<<< Exiting TaxService(addressSettings={AddressSettings},customerSettings={CustomerSettings},addressService={AddressService},checkVatService={CheckVatService},countryService={CountryService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},geoLookupService={GeoLookupService},logger={Logger},stateProvinceService={StateProvinceService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},webHelper={WebHelper},workContext={WorkContext},shippingSettings=<redacted>,taxSettings={TaxSettings})", addressSettings, customerSettings, addressService, checkVatService, countryService, customerService, eventPublisher, genericAttributeService, geoLookupService, logger, stateProvinceService, storeContext, taxPluginManager, webHelper, workContext, taxSettings);
		_logger.LogDebug("TaxService(addressSettings={AddressSettings},customerSettings={CustomerSettings},addressService={AddressService},checkVatService={CheckVatService},countryService={CountryService},customerService={CustomerService},eventPublisher={EventPublisher},genericAttributeService={GenericAttributeService},geoLookupService={GeoLookupService},logger={Logger},stateProvinceService={StateProvinceService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},webHelper={WebHelper},workContext={WorkContext},shippingSettings=<redacted>,taxSettings={TaxSettings}): _taxSettings → {_taxSettings}", addressSettings, customerSettings, addressService, checkVatService, countryService, customerService, eventPublisher, genericAttributeService, geoLookupService, logger, stateProvinceService, storeContext, taxPluginManager, webHelper, workContext, taxSettings, _taxSettings);
    }

    #endregion

    #region Utilities               

    /// <summary>
    /// Gets a default tax address
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the address
    /// </returns>
    protected virtual async Task<Address> LoadDefaultTaxAddressAsync()
    {
		_logger.LogError(">>> Entering LoadDefaultTaxAddressAsync()");
        var addressId = _taxSettings.DefaultTaxAddressId;

		_logger.LogWarning("<<< Exiting LoadDefaultTaxAddressAsync()");
        return await _addressService.GetAddressByIdAsync(addressId);
    }

    /// <summary>
    /// Gets or sets a pickup point address for tax calculation
    /// </summary>
    /// <param name="pickupPoint">Pickup point</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the address
    /// </returns>
    protected virtual async Task<Address> LoadPickupPointTaxAddressAsync(PickupPoint pickupPoint)
    {
		_logger.LogError(">>> Entering LoadPickupPointTaxAddressAsync(pickupPoint={PickupPoint})", pickupPoint);
        ArgumentNullException.ThrowIfNull(pickupPoint);

        var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(pickupPoint.CountryCode);
		_logger.LogError("LoadPickupPointTaxAddressAsync(pickupPoint={PickupPoint}): country → {Country}", pickupPoint, country);
        var state = await _stateProvinceService.GetStateProvinceByAbbreviationAsync(pickupPoint.StateAbbreviation, country?.Id);
		_logger.LogError("LoadPickupPointTaxAddressAsync(pickupPoint={PickupPoint}): state → {State}", pickupPoint, state);

		_logger.LogWarning("<<< Exiting LoadPickupPointTaxAddressAsync(pickupPoint={PickupPoint})", pickupPoint);
        return new Address
        {
            CountryId = country?.Id ?? 0,
            StateProvinceId = state?.Id ?? 0,
            County = pickupPoint.County,
            City = pickupPoint.City,
            Address1 = pickupPoint.Address,
            ZipPostalCode = pickupPoint.ZipPostalCode
        };
    }

    /// <summary>
    /// Prepare request to get tax rate
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="taxCategoryId">Tax category identifier</param>
    /// <param name="customer">Customer</param>
    /// <param name="price">Price</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the package for tax calculation
    /// </returns>
    protected virtual async Task<TaxRateRequest> PrepareTaxRateRequestAsync(Product product, int taxCategoryId, Customer customer, decimal price)
    {
		_logger.LogError(">>> Entering PrepareTaxRateRequestAsync(product={Product},taxCategoryId={TaxCategoryId},customer={Customer},price={Price})", product, taxCategoryId, customer, price);
        ArgumentNullException.ThrowIfNull(customer);

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareTaxRateRequestAsync(product={Product},taxCategoryId={TaxCategoryId},customer={Customer},price={Price}): store → {Store}", product, taxCategoryId, customer, price, store);
        var taxRateRequest = new TaxRateRequest
        {
            Customer = customer,
            Product = product,
            Price = price,
            TaxCategoryId = taxCategoryId > 0 ? taxCategoryId : product?.TaxCategoryId ?? 0,
            CurrentStoreId = store.Id
        };

        var basedOn = _taxSettings.TaxBasedOn;
		_logger.LogError("PrepareTaxRateRequestAsync(product={Product},taxCategoryId={TaxCategoryId},customer={Customer},price={Price}): basedOn → {BasedOn}", product, taxCategoryId, customer, price, basedOn);

        //tax is based on pickup point address
        if (_taxSettings.TaxBasedOnPickupPointAddress && _shippingSettings.AllowPickupInStore)
        {
            var pickupPoint = await _genericAttributeService.GetAttributeAsync<PickupPoint>(customer,
                NopCustomerDefaults.SelectedPickupPointAttribute, store.Id);
            if (pickupPoint != null)
            {
                taxRateRequest.Address = await LoadPickupPointTaxAddressAsync(pickupPoint);
                return taxRateRequest;
            }
        }

        var autodetectedCountry = false;
		_logger.LogError("PrepareTaxRateRequestAsync(product={Product},taxCategoryId={TaxCategoryId},customer={Customer},price={Price}): autodetectedCountry → {AutodetectedCountry}", product, taxCategoryId, customer, price, autodetectedCountry);
        var detectedAddress = new Address
        {
            CreatedOnUtc = DateTime.UtcNow
        };

        if (basedOn == TaxBasedOn.BillingAddress && customer.BillingAddressId == null ||
            basedOn == TaxBasedOn.ShippingAddress && customer.ShippingAddressId == null)
        {
            if (_taxSettings.AutomaticallyDetectCountry)
            {
                var ipAddress = _webHelper.GetCurrentIpAddress();
				_logger.LogError("PrepareTaxRateRequestAsync(product={Product},taxCategoryId={TaxCategoryId},customer={Customer},price={Price}): ipAddress → {IpAddress}", product, taxCategoryId, customer, price, ipAddress);
                var countryIsoCode = _geoLookupService.LookupCountryIsoCode(ipAddress);
				_logger.LogError("PrepareTaxRateRequestAsync(product={Product},taxCategoryId={TaxCategoryId},customer={Customer},price={Price}): countryIsoCode → {CountryIsoCode}", product, taxCategoryId, customer, price, countryIsoCode);
                var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(countryIsoCode);
				_logger.LogError("PrepareTaxRateRequestAsync(product={Product},taxCategoryId={TaxCategoryId},customer={Customer},price={Price}): country → {Country}", product, taxCategoryId, customer, price, country);

                if (country != null)
                {
                    detectedAddress.CountryId = country.Id;
					_logger.LogDebug("PrepareTaxRateRequestAsync(product={Product},taxCategoryId={TaxCategoryId},customer={Customer},price={Price}): detectedAddress.CountryId → {Arg4}", product, taxCategoryId, customer, price, detectedAddress.CountryId);
                    autodetectedCountry = true;
                }
                else
                    basedOn = TaxBasedOn.DefaultAddress;
            }
            else
                basedOn = TaxBasedOn.DefaultAddress;
        }

        taxRateRequest.Address = basedOn switch
		_logger.LogDebug("PrepareTaxRateRequestAsync(product={Product},taxCategoryId={TaxCategoryId},customer={Customer},price={Price}): taxRateRequest.Address → {Arg4}", product, taxCategoryId, customer, price, taxRateRequest.Address);
        {
            TaxBasedOn.BillingAddress => autodetectedCountry ? detectedAddress : await _customerService.GetCustomerBillingAddressAsync(customer),
            TaxBasedOn.ShippingAddress => autodetectedCountry ? detectedAddress : await _customerService.GetCustomerShippingAddressAsync(customer),
            _ => await LoadDefaultTaxAddressAsync(),
        };

		_logger.LogWarning("<<< Exiting PrepareTaxRateRequestAsync(product={Product},taxCategoryId={TaxCategoryId},customer={Customer},price={Price})", product, taxCategoryId, customer, price);
        return taxRateRequest;
    }

    /// <summary>
    /// Calculated price
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="percent">Percent</param>
    /// <param name="increase">Increase</param>
    /// <returns>New price</returns>
    protected virtual decimal CalculatePrice(decimal price, decimal percent, bool increase)
    {
		_logger.LogError(">>> Entering CalculatePrice(price={Price},percent={Percent},increase={Increase})", price, percent, increase);
        if (percent == decimal.Zero)
            return price;

        decimal result;
        if (increase)
            result = price * (1 + percent / 100);
        else
            result = price - price / (100 + percent) * percent;
			_logger.LogError("CalculatePrice(price={Price},percent={Percent},increase={Increase}): result → {Result}", price, percent, increase, result);

		_logger.LogWarning("<<< Exiting CalculatePrice(price={Price},percent={Percent},increase={Increase})", price, percent, increase);
        return result;
    }

    /// <summary>
    /// Gets tax rate
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="taxCategoryId">Tax category identifier</param>
    /// <param name="customer">Customer</param>
    /// <param name="price">Price (taxable value)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the calculated tax rate. A value indicating whether a request is taxable
    /// </returns>
    protected virtual async Task<(decimal taxRate, bool isTaxable)> GetTaxRateAsync(Product product, int taxCategoryId,
        Customer customer, decimal price)
    {
        var taxRate = decimal.Zero;

        //active tax provider
        var store = await _storeContext.GetCurrentStoreAsync();
        var activeTaxProvider = await _taxPluginManager.LoadPrimaryPluginAsync(customer, store.Id);
        if (activeTaxProvider == null)
            return (taxRate, true);

        //tax request
        var taxRateRequest = await PrepareTaxRateRequestAsync(product, taxCategoryId, customer, price);

        var isTaxable = !await IsTaxExemptAsync(product, taxRateRequest.Customer);

        //tax exempt

        //make EU VAT exempt validation (the European Union Value Added Tax)
        if (isTaxable &&
            _taxSettings.EuVatEnabled &&
            await IsVatExemptAsync(taxRateRequest.Address, taxRateRequest.Customer))
            //VAT is not chargeable
            isTaxable = false;

        //get tax rate
        var taxRateResult = await activeTaxProvider.GetTaxRateAsync(taxRateRequest);

        //tax rate is calculated, now consumers can adjust it
        await _eventPublisher.PublishAsync(new TaxRateCalculatedEvent(taxRateResult));

        if (taxRateResult.Success)
        {
            //ensure that tax is equal or greater than zero
            if (taxRateResult.TaxRate < decimal.Zero)
                taxRateResult.TaxRate = decimal.Zero;

            taxRate = taxRateResult.TaxRate;
        }
        else if (_taxSettings.LogErrors)
            foreach (var error in taxRateResult.Errors)
                await _logger.ErrorAsync($"{activeTaxProvider.PluginDescriptor.FriendlyName} - {error}", null, customer);

        return (taxRate, isTaxable);
    }

    /// <summary>
    /// Gets VAT Number status
    /// </summary>
    /// <param name="twoLetterIsoCode">Two letter ISO code of a country</param>
    /// <param name="vatNumber">VAT number</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vAT Number status. Name (if received). Address (if received)
    /// </returns>
    protected virtual async Task<(VatNumberStatus vatNumberStatus, string name, string address)> GetVatNumberStatusAsync(string twoLetterIsoCode, string vatNumber)
    {
        var name = string.Empty;
        var address = string.Empty;

        if (string.IsNullOrEmpty(twoLetterIsoCode))
            return (VatNumberStatus.Empty, name, address);

        if (string.IsNullOrEmpty(vatNumber))
            return (VatNumberStatus.Empty, name, address);

        if (_taxSettings.EuVatAssumeValid)
            return (VatNumberStatus.Valid, name, address);

        if (!_taxSettings.EuVatUseWebService)
            return (VatNumberStatus.Unknown, name, address);

        var rez = await DoVatCheckAsync(twoLetterIsoCode, vatNumber);

        return (rez.vatNumberStatus, rez.name, rez.address);
    }

    /// <summary>
    /// Performs a basic check of a VAT number for validity
    /// </summary>
    /// <param name="twoLetterIsoCode">Two letter ISO code of a country</param>
    /// <param name="vatNumber">VAT number</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vAT number status. Company name. Address. Exception
    /// </returns>
    protected virtual async Task<(VatNumberStatus vatNumberStatus, string name, string address, Exception exception)> DoVatCheckAsync(string twoLetterIsoCode, string vatNumber)
    {
        vatNumber ??= string.Empty;
        vatNumber = vatNumber.Trim().Replace(" ", string.Empty);

        twoLetterIsoCode ??= string.Empty;

        try
        {
            var (status, name, address) = await _checkVatService.CheckVatAsync(twoLetterIsoCode, vatNumber);
            return (status, name, address, null);
        }
        catch (Exception ex)
        {
            return (VatNumberStatus.Unknown, string.Empty, string.Empty, ex);
        }
    }

    /// <summary>
    /// Gets a value indicating whether EU VAT exempt (the European Union Value Added Tax)
    /// </summary>
    /// <param name="address">Address</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    protected virtual async Task<bool> IsVatExemptAsync(Address address, Customer customer)
    {
		_logger.LogError(">>> Entering IsVatExemptAsync(address={Address},customer={Customer})", address, customer);
        if (!_taxSettings.EuVatEnabled)
            return false;

        if (customer == null || address == null)
            return false;

        var country = await _countryService.GetCountryByIdAsync(address.CountryId ?? 0);
		_logger.LogError("IsVatExemptAsync(address={Address},customer={Customer}): country → {Country}", address, customer, country);
        if (country == null)
            return false;

        if (!country.SubjectToVat)
            // VAT not chargeable if shipping outside VAT zone
            return true;

        // VAT not chargeable if address, customer and config meet our VAT exemption requirements:
        // returns true if this customer is VAT exempt because they are shipping within the EU but outside our shop country, they have supplied a validated VAT number, and the shop is configured to allow VAT exemption
        var customerVatStatus = (VatNumberStatus)customer.VatNumberStatusId;

		_logger.LogWarning("<<< Exiting IsVatExemptAsync(address={Address},customer={Customer})", address, customer);
        return country.Id != _taxSettings.EuVatShopCountryId &&
               customerVatStatus == VatNumberStatus.Valid &&
               _taxSettings.EuVatAllowVatExemption;
    }

    #endregion

    #region Methods

    #region Product price

    /// <summary>
    /// Gets price
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="price">Price</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    public virtual async Task<(decimal price, decimal taxRate)> GetProductPriceAsync(Product product, decimal price)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        return await GetProductPriceAsync(product, price, customer);
    }

    /// <summary>
    /// Gets price
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="price">Price</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    public virtual async Task<(decimal price, decimal taxRate)> GetProductPriceAsync(Product product, decimal price,
        Customer customer)
    {
        var includingTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;
        return await GetProductPriceAsync(product, price, includingTax, customer);
    }

    /// <summary>
    /// Gets price
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="price">Price</param>
    /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    public virtual async Task<(decimal price, decimal taxRate)> GetProductPriceAsync(Product product, decimal price,
        bool includingTax, Customer customer)
    {
        var priceIncludesTax = _taxSettings.PricesIncludeTax;
        var taxCategoryId = 0;
        return await GetProductPriceAsync(product, taxCategoryId, price, includingTax, customer, priceIncludesTax);
    }

    /// <summary>
    /// Gets price
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="taxCategoryId">Tax category identifier</param>
    /// <param name="price">Price</param>
    /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
    /// <param name="customer">Customer</param>
    /// <param name="priceIncludesTax">A value indicating whether price already includes tax</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    public virtual async Task<(decimal price, decimal taxRate)> GetProductPriceAsync(Product product, int taxCategoryId,
        decimal price, bool includingTax, Customer customer,
        bool priceIncludesTax)
    {
        var taxRate = decimal.Zero;

        //no need to calculate tax rate if passed "price" is 0
        if (price == decimal.Zero)
            return (price, taxRate);

        bool isTaxable;

        (taxRate, isTaxable) = await GetTaxRateAsync(product, taxCategoryId, customer, price);

        if (priceIncludesTax)
        {
            //"price" already includes tax
            if (includingTax)
            {
                //we should calculate price WITH tax
                if (!isTaxable)
                {
                    //but our request is not taxable
                    //hence we should calculate price WITHOUT tax
                    price = CalculatePrice(price, taxRate, false);
                }
            }
            else
            {
                //we should calculate price WITHOUT tax
                price = CalculatePrice(price, taxRate, false);
            }
        }
        else
        {
            //"price" doesn't include tax
            if (includingTax)
            {
                //we should calculate price WITH tax
                //do it only when price is taxable
                if (isTaxable)
                {
                    price = CalculatePrice(price, taxRate, true);
                }
            }
        }

        if (!isTaxable)
        {
            //we return 0% tax rate in case a request is not taxable
            taxRate = decimal.Zero;
        }

        //allowed to support negative price adjustments
        //if (price < decimal.Zero)
        //    price = decimal.Zero;

        return (price, taxRate);
    }

    /// <summary>
    /// Gets a value indicating whether a product is tax exempt
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether a product is tax exempt
    /// </returns>
    public virtual async Task<bool> IsTaxExemptAsync(Product product, Customer customer)
    {
		_logger.LogError(">>> Entering IsTaxExemptAsync(product={Product},customer={Customer})", product, customer);
        if (customer != null)
        {
            if (customer.IsTaxExempt)
                return true;

            if ((await _customerService.GetCustomerRolesAsync(customer)).Any(cr => cr.TaxExempt))
                return true;
        }

        if (product == null)
            return false;

		_logger.LogWarning("<<< Exiting IsTaxExemptAsync(product={Product},customer={Customer})", product, customer);
        if (product.IsTaxExempt) {
            return true;
        }

        return false;
    }

    #endregion

    #region Shipping price

    /// <summary>
    /// Gets shipping price
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    public virtual async Task<(decimal price, decimal taxRate)> GetShippingPriceAsync(decimal price, Customer customer)
    {
        var includingTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;

        return await GetShippingPriceAsync(price, includingTax, customer);
    }

    /// <summary>
    /// Gets shipping price
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    public virtual async Task<(decimal price, decimal taxRate)> GetShippingPriceAsync(decimal price, bool includingTax, Customer customer)
    {
        var taxRate = decimal.Zero;

        if (!_taxSettings.ShippingIsTaxable)
        {
            return (price, taxRate);
        }

        var taxClassId = _taxSettings.ShippingTaxClassId;
        var priceIncludesTax = _taxSettings.ShippingPriceIncludesTax;

        return await GetProductPriceAsync(null, taxClassId, price, includingTax, customer, priceIncludesTax);
    }

    #endregion

    #region Payment additional fee

    /// <summary>
    /// Gets payment method additional handling fee
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    public virtual async Task<(decimal price, decimal taxRate)> GetPaymentMethodAdditionalFeeAsync(decimal price, Customer customer)
    {
        var includingTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;

        return await GetPaymentMethodAdditionalFeeAsync(price, includingTax, customer);
    }

    /// <summary>
    /// Gets payment method additional handling fee
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    public virtual async Task<(decimal price, decimal taxRate)> GetPaymentMethodAdditionalFeeAsync(decimal price, bool includingTax, Customer customer)
    {
        var taxRate = decimal.Zero;

        if (!_taxSettings.PaymentMethodAdditionalFeeIsTaxable)
        {
            return (price, taxRate);
        }

        var taxClassId = _taxSettings.PaymentMethodAdditionalFeeTaxClassId;
        var priceIncludesTax = _taxSettings.PaymentMethodAdditionalFeeIncludesTax;
        return await GetProductPriceAsync(null, taxClassId, price, includingTax, customer, priceIncludesTax);
    }

    #endregion

    #region Checkout attribute price

    /// <summary>
    /// Gets checkout attribute value price
    /// </summary>
    /// <param name="ca">Checkout attribute</param>
    /// <param name="cav">Checkout attribute value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    public virtual async Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        return await GetCheckoutAttributePriceAsync(ca, cav, customer);
    }

    /// <summary>
    /// Gets checkout attribute value price
    /// </summary>
    /// <param name="ca">Checkout attribute</param>
    /// <param name="cav">Checkout attribute value</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    public virtual async Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav, Customer customer)
    {
        var includingTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;

        return await GetCheckoutAttributePriceAsync(ca, cav, includingTax, customer);
    }

    /// <summary>
    /// Gets checkout attribute value price
    /// </summary>
    /// <param name="ca">Checkout attribute</param>
    /// <param name="cav">Checkout attribute value</param>
    /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    public virtual async Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav,
        bool includingTax, Customer customer)
    {
        ArgumentNullException.ThrowIfNull(cav);

        var taxRate = decimal.Zero;

        var price = cav.PriceAdjustment;
        if (ca.IsTaxExempt)
            return (price, taxRate);

        var priceIncludesTax = _taxSettings.PricesIncludeTax;
        var taxClassId = ca.TaxCategoryId;

        return await GetProductPriceAsync(null, taxClassId, price, includingTax, customer, priceIncludesTax);
    }

    #endregion

    #region VAT

    /// <summary>
    /// Gets VAT Number status
    /// </summary>
    /// <param name="fullVatNumber">Two letter ISO code of a country and VAT number (e.g. DE 111 1111 111)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vAT Number status. Name (if received). Address (if received)
    /// </returns>
    public virtual async Task<(VatNumberStatus vatNumberStatus, string name, string address)> GetVatNumberStatusAsync(string fullVatNumber)
    {
        var name = string.Empty;
        var address = string.Empty;

        if (string.IsNullOrWhiteSpace(fullVatNumber))
            return (VatNumberStatus.Empty, name, address);
        fullVatNumber = fullVatNumber.Trim();

        //DE 111 1111 111 or DE 1111111111
        var r = new Regex(@"^(\w{2})(.*)");
        var match = r.Match(fullVatNumber);
        if (!match.Success)
            return (VatNumberStatus.Invalid, name, address);

        var twoLetterIsoCode = match.Groups[1].Value;
        var vatNumber = match.Groups[2].Value;

        return await GetVatNumberStatusAsync(twoLetterIsoCode, vatNumber);
    }

    #endregion

    #region Tax total

    /// <summary>
    /// Get tax total for the passed shopping cart
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<TaxTotalResult> GetTaxTotalAsync(IList<ShoppingCartItem> cart, bool usePaymentMethodAdditionalFee = true)
    {
		_logger.LogError(">>> Entering GetTaxTotalAsync(cart={Cart},usePaymentMethodAdditionalFee={UsePaymentMethodAdditionalFee})", cart, usePaymentMethodAdditionalFee);
        var customer = await _customerService.GetShoppingCartCustomerAsync(cart);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("GetTaxTotalAsync(cart={Cart},usePaymentMethodAdditionalFee={UsePaymentMethodAdditionalFee}): store → {Store}", cart, usePaymentMethodAdditionalFee, store);
        var activeTaxProvider = await _taxPluginManager.LoadPrimaryPluginAsync(customer, store.Id);
		_logger.LogError("GetTaxTotalAsync(cart={Cart},usePaymentMethodAdditionalFee={UsePaymentMethodAdditionalFee}): activeTaxProvider → {ActiveTaxProvider}", cart, usePaymentMethodAdditionalFee, activeTaxProvider);
		_logger.LogError("GetTaxTotalAsync(cart={Cart},usePaymentMethodAdditionalFee={UsePaymentMethodAdditionalFee}): customer → {Customer}", cart, usePaymentMethodAdditionalFee, customer);
        if (activeTaxProvider == null)
            return null;

        //get result by using primary tax provider
        var taxTotalRequest = new TaxTotalRequest
        {
            ShoppingCart = cart,
            Customer = customer,
            StoreId = store.Id,
            UsePaymentMethodAdditionalFee = usePaymentMethodAdditionalFee
        };
        var taxTotalResult = await activeTaxProvider.GetTaxTotalAsync(taxTotalRequest);
		_logger.LogError("GetTaxTotalAsync(cart={Cart},usePaymentMethodAdditionalFee={UsePaymentMethodAdditionalFee}): taxTotalResult → {TaxTotalResult}", cart, usePaymentMethodAdditionalFee, taxTotalResult);

        //tax total is calculated, now consumers can adjust it
        await _eventPublisher.PublishAsync(new TaxTotalCalculatedEvent(taxTotalRequest, taxTotalResult));

        //error logging
        if (taxTotalResult != null && !taxTotalResult.Success && _taxSettings.LogErrors)
        {
            foreach (var error in taxTotalResult.Errors)
            {
                await _logger.ErrorAsync($"{activeTaxProvider.PluginDescriptor.FriendlyName} - {error}", null, customer);
            }
        }

		_logger.LogWarning("<<< Exiting GetTaxTotalAsync(cart={Cart},usePaymentMethodAdditionalFee={UsePaymentMethodAdditionalFee})", cart, usePaymentMethodAdditionalFee);
        return taxTotalResult;
    }

    #endregion

    #endregion
}