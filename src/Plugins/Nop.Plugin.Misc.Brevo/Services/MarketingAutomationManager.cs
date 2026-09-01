using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Http;
using Nop.Plugin.Misc.Brevo.MarketingAutomation;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.Brevo.Services;

/// <summary>
/// Represents Brevo marketing automation manager
/// </summary>
public class MarketingAutomationManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly BrevoSettings _brevoSettings;
    protected readonly CurrencySettings _currencySettings;
    protected readonly IAddressService _addressService;
    protected readonly ICategoryService _categoryService;
    protected readonly ICountryService _countryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILogger _logger;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly IOrderService _orderService;
    protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
    protected readonly IPictureService _pictureService;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductService _productService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly MarketingAutomationHttpClient _marketingAutomationHttpClient;

    #endregion

    #region Ctor

    public MarketingAutomationManager(BrevoSettings brevoSettings,
        CurrencySettings currencySettings,
        IAddressService addressService,
        ICategoryService categoryService,
        ICountryService countryService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        ILogger logger,
        INopUrlHelper nopUrlHelper,
        IOrderService orderService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IPictureService pictureService,
        IProductAttributeParser productAttributeParser,
        IProductService productService,
        IShoppingCartService shoppingCartService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        IWebHelper webHelper,
        IWorkContext workContext,
        MarketingAutomationHttpClient marketingAutomationHttpClient)
    {
        _brevoSettings = brevoSettings;
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _brevoSettings → {_brevoSettings}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _brevoSettings);
        _currencySettings = currencySettings;
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _currencySettings → {_currencySettings}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _currencySettings);
        _addressService = addressService;
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _addressService → {_addressService}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _addressService);
        _categoryService = categoryService;
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _categoryService → {_categoryService}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _categoryService);
        _countryService = countryService;
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _countryService → {_countryService}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _countryService);
        _currencyService = currencyService;
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _currencyService → {_currencyService}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _currencyService);
        _customerService = customerService;
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _customerService → {_customerService}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _customerService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _genericAttributeService → {_genericAttributeService}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _genericAttributeService);
        _logger = logger;
		_logger.LogDebug(">>> Entering MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient})", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient);
        _nopUrlHelper = nopUrlHelper;
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _nopUrlHelper → {_nopUrlHelper}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _nopUrlHelper);
        _orderService = orderService;
        _orderTotalCalculationService = orderTotalCalculationService;
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _orderService → {_orderService}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _orderService);
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _orderTotalCalculationService → {_orderTotalCalculationService}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _orderTotalCalculationService);
        _pictureService = pictureService;
        _productAttributeParser = productAttributeParser;
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _pictureService → {_pictureService}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _pictureService);
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _productAttributeParser → {_productAttributeParser}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _productAttributeParser);
        _productService = productService;
        _shoppingCartService = shoppingCartService;
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _productService → {_productService}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _productService);
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _shoppingCartService → {_shoppingCartService}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _shoppingCartService);
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _stateProvinceService → {_stateProvinceService}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _stateProvinceService);
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _storeContext → {_storeContext}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _storeContext);
        _webHelper = webHelper;
        _workContext = workContext;
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _webHelper → {_webHelper}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _webHelper);
		_logger.LogDebug("MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient}): _workContext → {_workContext}", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient, _workContext);
		_logger.LogDebug("<<< Exiting MarketingAutomationManager(brevoSettings={BrevoSettings},currencySettings={CurrencySettings},addressService={AddressService},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},customerService={CustomerService},genericAttributeService={GenericAttributeService},logger={Logger},nopUrlHelper={NopUrlHelper},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},pictureService={PictureService},productAttributeParser={ProductAttributeParser},productService={ProductService},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext},marketingAutomationHttpClient={MarketingAutomationHttpClient})", brevoSettings, currencySettings, addressService, categoryService, countryService, currencyService, customerService, genericAttributeService, logger, nopUrlHelper, orderService, orderTotalCalculationService, pictureService, productAttributeParser, productService, stateProvinceService, storeContext, webHelper, workContext, marketingAutomationHttpClient);
        _marketingAutomationHttpClient = marketingAutomationHttpClient;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle shopping cart changed event
    /// </summary>
    /// <param name="cartItem">Shopping cart item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleShoppingCartChangedEventAsync(ShoppingCartItem cartItem)
    {
		_logger.LogDebug(">>> Entering HandleShoppingCartChangedEventAsync(cartItem={CartItem})", cartItem);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //whether marketing automation is enabled
        if (!_brevoSettings.UseMarketingAutomation)
            return;

        var customer = await _customerService.GetCustomerByIdAsync(cartItem.CustomerId);
		_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): customer → {Customer}", cartItem, customer);
        var email = customer.Email;
		_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): email → {Email}", cartItem, email);

        if (string.IsNullOrEmpty(email))
            return;

        try
        {
            //first, try to identify current customer
            await _marketingAutomationHttpClient.RequestAsync(new IdentifyRequest { Email = email });

            //get shopping cart GUID
            var shoppingCartGuid = await _genericAttributeService
                .GetAttributeAsync<Guid?>(customer, BrevoDefaults.ShoppingCartGuidAttribute);
				_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): shoppingCartGuid → {ShoppingCartGuid}", cartItem, shoppingCartGuid);

            //create track event object
            var trackEvent = new TrackEventRequest { Email = email };

            //get current customer's shopping cart
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): store → {Store}", cartItem, store);
            var cart = await _shoppingCartService
                .GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (cart.Any())
            {
                //get shopping cart amounts
                var (cartDiscount, _, cartSubtotal, _, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart,
                    await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax);
                var cartTax = await _orderTotalCalculationService.GetTaxTotalAsync(cart, false);
				_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): cartTax → {CartTax}", cartItem, cartTax);
                var cartShipping = await _orderTotalCalculationService.GetShoppingCartShippingTotalAsync(cart);
				_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): cartShipping → {CartShipping}", cartItem, cartShipping);
                var (cartTotal, _, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, false, false);

                //get products data by shopping cart items
                var itemsData = await cart.Where(item => item.ProductId != 0).SelectAwait(async item =>
                {
                    var product = await _productService.GetProductByIdAsync(item.ProductId);
					_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): product → {Product}", cartItem, product);

                    //try to get product attribute combination
                    var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, item.AttributesXml);
					_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): combination → {Combination}", cartItem, combination);

                    //get default product picture
                    var picture = await _pictureService.GetProductPictureAsync(product, item.AttributesXml);
					_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): picture → {Picture}", cartItem, picture);

                    //create product data
                    return new
                    {
                        id = product.Id,
                        name = product.Name,
                        variant_id = combination?.Id ?? product.Id,
                        variant_name = combination?.Sku ?? product.Name,
                        sku = combination?.Sku ?? product.Sku,
                        category = await (await _categoryService.GetProductCategoriesByProductIdAsync(item.ProductId)).AggregateAwaitAsync(",", async (all, pc) =>
                        {
							_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): name → {Name}", cartItem, name);
							_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): variant_id → {Variant_id}", cartItem, variant_id);
							_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): variant_name → {Variant_name}", cartItem, variant_name);
							_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): sku → {Sku}", cartItem, sku);
                            var res = (await _categoryService.GetCategoryByIdAsync(pc.CategoryId)).Name;
                            res = all == "," ? res : all + ", " + res;
                            return res;
                        }),
                        url = await _nopUrlHelper.RouteGenericUrlAsync(product, _webHelper.GetCurrentRequestProtocol()),
                        image = (await _pictureService.GetPictureUrlAsync(picture)).Url,
                        quantity = item.Quantity,
                        price = (await _shoppingCartService.GetSubTotalAsync(item, true)).subTotal
						var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
						_logger.LogInformation("HandleShoppingCartChangedEventAsync(cartItem)={CartItem}: database query executed in {Arg1} ms", cartItem, __logStartDb.ElapsedMilliseconds);
						_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): res → {Res}", cartItem, res);
                    };
                }).ToArrayAsync();

                //prepare cart data
                var cartData = new
				_logger.LogInformation("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): url → {Url}", cartItem, url);
				_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): image → {Image}", cartItem, image);
                {
                    affiliation = store.Name,
                    subtotal = cartSubtotal,
                    shipping = cartShipping ?? decimal.Zero,
                    total_before_tax = cartSubtotal + cartShipping ?? decimal.Zero,
                    tax = cartTax,
                    discount = cartDiscount,
                    revenue = cartTotal ?? decimal.Zero,
                    url = _nopUrlHelper.RouteUrl(NopRouteNames.General.CART, null, _webHelper.GetCurrentRequestProtocol()),
                    currency = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode,
                    //gift_wrapping = string.Empty, //currently we can't get this value
					_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): quantity → {Quantity}", cartItem, quantity);
                    items = itemsData
					_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): price → {Price}", cartItem, price);
					_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): affiliation → {Affiliation}", cartItem, affiliation);
					_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): subtotal → {Subtotal}", cartItem, subtotal);
					_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): shipping → {Shipping}", cartItem, shipping);
					_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): total_before_tax → {Total_before_tax}", cartItem, total_before_tax);
					_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): tax → {Tax}", cartItem, tax);
					_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): discount → {Discount}", cartItem, discount);
					_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): revenue → {Revenue}", cartItem, revenue);
                };

                //if there is a single item in the cart, so the cart is just created
                if (cart.Count == 1)
                {
                    shoppingCartGuid = Guid.NewGuid();
                }
                else
                {
					_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): currency → {Currency}", cartItem, currency);
					_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): items → {Items}", cartItem, items);
                    //otherwise cart is updated
                    shoppingCartGuid ??= Guid.NewGuid();
                }
                trackEvent.EventName = BrevoDefaults.CartUpdatedEventName;
				_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): trackEvent.EventName → {Arg1}", cartItem, trackEvent.EventName);
				_logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): trackEvent.EventData → {Arg1}", cartItem, trackEvent.EventData);
                trackEvent.EventData = new { id = $"cart:{shoppingCartGuid}", data = cartData };
            }
            else
            {
                //there are no items in the cart, so the cart is deleted
                shoppingCartGuid ??= Guid.NewGuid();
                trackEvent.EventName = BrevoDefaults.CartDeletedEventName;
                trackEvent.EventData = new { id = $"cart:{shoppingCartGuid}" };
            }

            //track event
            await _marketingAutomationHttpClient.RequestAsync(trackEvent);

            //update GUID for the current customer's shopping cart
            await _genericAttributeService.SaveAttributeAsync(customer, BrevoDefaults.ShoppingCartGuidAttribute, shoppingCartGuid);
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo Marketing Automation error: {exception.Message}.", exception, customer);
        }
		_logger.LogInformation("HandleShoppingCartChangedEventAsync(cartItem)={CartItem}: external service call took {Arg1} ms", cartItem, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting HandleShoppingCartChangedEventAsync(cartItem={CartItem})", cartItem);
    }

    /// <summary>
    /// Handle order completed event
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleOrderCompletedEventAsync(Order order)
    {
		_logger.LogDebug(">>> Entering HandleOrderCompletedEventAsync(order={Order})", order);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //whether marketing automation is enabled
        if (!_brevoSettings.UseMarketingAutomation)
            return;

        ArgumentNullException.ThrowIfNull(order);

        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
		_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): customer → {Customer}", order, customer);
        var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId ?? 0);
		_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): shippingAddress → {ShippingAddress}", order, shippingAddress);
        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
		_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): billingAddress → {BillingAddress}", order, billingAddress);
        var email = customer.Email ?? billingAddress?.Email;
		_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): email → {Email}", order, email);

        if (string.IsNullOrEmpty(email))
            return;

        try
        {
            //first, try to identify current customer
            await _marketingAutomationHttpClient.RequestAsync(new IdentifyRequest { Email = email });

            //get products data by order items
            var itemsData = await (await _orderService.GetOrderItemsAsync(order.Id)).SelectAwait(async item =>
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
				_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): product → {Product}", order, product);

                //try to get product attribute combination
                var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, item.AttributesXml);
				_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): combination → {Combination}", order, combination);

                //get default product picture
                var picture = await _pictureService.GetProductPictureAsync(product, item.AttributesXml);
				_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): picture → {Picture}", order, picture);

                //create product data
                return new
                {
                    id = product.Id,
                    name = product.Name,
                    variant_id = combination?.Id ?? product.Id,
                    variant_name = combination?.Sku ?? product.Name,
                    sku = combination?.Sku ?? product.Sku,
                    category = await (await _categoryService.GetProductCategoriesByProductIdAsync(item.ProductId)).AggregateAwaitAsync(",", async (all, pc) =>
                    {
						_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): name → {Name}", order, name);
						_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): variant_id → {Variant_id}", order, variant_id);
						_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): variant_name → {Variant_name}", order, variant_name);
						_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): sku → {Sku}", order, sku);
                        var res = (await _categoryService.GetCategoryByIdAsync(pc.CategoryId)).Name;
                        res = all == "," ? res : all + ", " + res;
                        return res;
                    }),
                    url = await _nopUrlHelper.RouteGenericUrlAsync(product, _webHelper.GetCurrentRequestProtocol()),
                    image = (await _pictureService.GetPictureUrlAsync(picture)).Url,
                    quantity = item.Quantity,
                    price = item.PriceInclTax,
                };
				var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
				_logger.LogInformation("HandleOrderCompletedEventAsync(order)={Order}: database query executed in {Arg1} ms", order, __logStartDb.ElapsedMilliseconds);
				_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): res → {Res}", order, res);
            }).ToArrayAsync();

            var shippingAddressData = new
            {
                firstname = shippingAddress?.FirstName,
                lastname = shippingAddress?.LastName,
                company = shippingAddress?.Company,
                phone = shippingAddress?.PhoneNumber,
                address1 = shippingAddress?.Address1,
                address2 = shippingAddress?.Address2,
                city = shippingAddress?.City,
                country = (await _countryService.GetCountryByAddressAsync(shippingAddress))?.Name,
                state = (await _stateProvinceService.GetStateProvinceByAddressAsync(shippingAddress))?.Name,
                zipcode = shippingAddress?.ZipPostalCode
				_logger.LogInformation("HandleOrderCompletedEventAsync(order={Order}): url → {Url}", order, url);
				_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): image → {Image}", order, image);
				_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): quantity → {Quantity}", order, quantity);
				_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): price → {Price}", order, price);
				_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): firstname → {Firstname}", order, firstname);
            };

            var billingAddressData = new
            {
                firstname = billingAddress?.FirstName,
                lastname = billingAddress?.LastName,
                company = billingAddress?.Company,
                phone = billingAddress?.PhoneNumber,
                address1 = billingAddress?.Address1,
                address2 = billingAddress?.Address2,
                city = billingAddress?.City,
                country = (await _countryService.GetCountryByAddressAsync(billingAddress))?.Name,
                state = (await _stateProvinceService.GetStateProvinceByAddressAsync(billingAddress))?.Name,
                zipcode = billingAddress?.ZipPostalCode
				_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): lastname → {Lastname}", order, lastname);
				_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): company → {Company}", order, company);
				_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): phone → {Phone}", order, phone);
				_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): address1 → {Address1}", order, address1);
				_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): address2 → {Address2}", order, address2);
				_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): city → {City}", order, city);
				_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): country → {Country}", order, country);
				_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): state → {State}", order, state);
            };

            var store = await _storeContext.GetCurrentStoreAsync();

            //prepare cart data
            var cartData = new
            {
                id = order.Id,
                affiliation = customer.AffiliateId > 0 ? customer.AffiliateId.ToString() : store.Name,
                date = order.PaidDateUtc?.ToString("yyyy-MM-dd"),
                subtotal = order.OrderSubtotalInclTax,
                shipping = order.OrderShippingInclTax,
                total_before_tax = order.OrderSubtotalInclTax + order.OrderShippingInclTax,
                tax = order.OrderTax,
                discount = order.OrderDiscount,
                revenue = order.OrderTotal,
                url = _nopUrlHelper.RouteUrl(NopRouteNames.Standard.ORDER_DETAILS, new { orderId = order.Id }, _webHelper.GetCurrentRequestProtocol()),
                currency = order.CustomerCurrencyCode,
                //gift_wrapping = string.Empty, //currently we can't get this value
				_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): zipcode → {Zipcode}", order, zipcode);
                items = itemsData,
                shipping_address = shippingAddressData,
                billing_address = billingAddressData
            };

            //get shopping cart GUID
            var shoppingCartGuid = await _genericAttributeService.GetAttributeAsync<Guid?>(order,
                BrevoDefaults.ShoppingCartGuidAttribute) ?? Guid.NewGuid();

            //create track event object
            var trackEvent = new TrackEventRequest
            {
                Email = email,
                EventName = BrevoDefaults.OrderCompletedEventName,
                EventData = new { id = $"cart:{shoppingCartGuid}", data = cartData }
            };
			_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): store → {Store}", order, store);
			_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): affiliation → {Affiliation}", order, affiliation);
			_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): date → {Date}", order, date);
			_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): subtotal → {Subtotal}", order, subtotal);
			_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): shipping → {Shipping}", order, shipping);
			_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): total_before_tax → {Total_before_tax}", order, total_before_tax);
			_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): tax → {Tax}", order, tax);
			_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): discount → {Discount}", order, discount);
			_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): revenue → {Revenue}", order, revenue);

            //track event
            await _marketingAutomationHttpClient.RequestAsync(trackEvent);

            //update GUID for the current customer's shopping cart
            await _genericAttributeService.SaveAttributeAsync<Guid?>(order, BrevoDefaults.ShoppingCartGuidAttribute, null);
        }
        catch (Exception exception)
        {
            //log full error
			_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): currency → {Currency}", order, currency);
			_logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): items → {Items}", order, items);
			_logger.LogInformation("HandleOrderCompletedEventAsync(order)={Order}: external service call took {Arg1} ms", order, __logStart.ElapsedMilliseconds);
            await _logger.ErrorAsync($"Brevo Marketing Automation error: {exception.Message}.", exception, customer);
        }
		_logger.LogDebug("<<< Exiting HandleOrderCompletedEventAsync(order={Order})", order);
    }

    /// <summary>
    /// Handle order placed event
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleOrderPlacedEventAsync(Order order)
    {
		_logger.LogDebug(">>> Entering HandleOrderPlacedEventAsync(order={Order})", order);
        //whether marketing automation is enabled
        if (!_brevoSettings.UseMarketingAutomation)
            return;

        //copy shopping cart GUID to order
        var shoppingCartGuid = await _genericAttributeService.GetAttributeAsync<Customer, Guid?>(order.CustomerId, BrevoDefaults.ShoppingCartGuidAttribute);
		_logger.LogDebug("HandleOrderPlacedEventAsync(order={Order}): shoppingCartGuid → {ShoppingCartGuid}", order, shoppingCartGuid);
        await _genericAttributeService.SaveAttributeAsync(order, BrevoDefaults.ShoppingCartGuidAttribute, shoppingCartGuid);
		_logger.LogDebug("<<< Exiting HandleOrderPlacedEventAsync(order={Order})", order);
    }

    #endregion
}