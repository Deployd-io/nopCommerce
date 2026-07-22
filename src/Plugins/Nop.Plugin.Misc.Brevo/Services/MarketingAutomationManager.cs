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
        _currencySettings = currencySettings;
        _addressService = addressService;
        _categoryService = categoryService;
        _countryService = countryService;
        _currencyService = currencyService;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _logger = logger;
        _nopUrlHelper = nopUrlHelper;
        _orderService = orderService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _pictureService = pictureService;
        _productAttributeParser = productAttributeParser;
        _productService = productService;
        _shoppingCartService = shoppingCartService;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _webHelper = webHelper;
        _workContext = workContext;
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
		logger.LogDebug(">>> Entering HandleShoppingCartChangedEventAsync(cartItem={CartItem})", cartItem);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //whether marketing automation is enabled
        if (!_brevoSettings.UseMarketingAutomation)
            return;

        var customer = await _customerService.GetCustomerByIdAsync(cartItem.CustomerId);
        var email = customer.Email;
		logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): customer → {Customer}", cartItem, customer);
		logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): email → {Email}", cartItem, email);

        if (string.IsNullOrEmpty(email))
            return;

        try
        {
            //first, try to identify current customer
            await _marketingAutomationHttpClient.RequestAsync(new IdentifyRequest { Email = email });

            //get shopping cart GUID
            var shoppingCartGuid = await _genericAttributeService
                .GetAttributeAsync<Guid?>(customer, BrevoDefaults.ShoppingCartGuidAttribute);
				logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): shoppingCartGuid → {ShoppingCartGuid}", cartItem, shoppingCartGuid);

            //create track event object
            var trackEvent = new TrackEventRequest { Email = email };

            //get current customer's shopping cart
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService
				logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): store → {Store}", cartItem, store);
                .GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (cart.Any())
            {
                //get shopping cart amounts
                var (cartDiscount, _, cartSubtotal, _, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart,
                    await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax);
                var cartTax = await _orderTotalCalculationService.GetTaxTotalAsync(cart, false);
                var cartShipping = await _orderTotalCalculationService.GetShoppingCartShippingTotalAsync(cart);
				logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): cartTax → {CartTax}", cartItem, cartTax);
				logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): cartShipping → {CartShipping}", cartItem, cartShipping);
                var (cartTotal, _, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, false, false);

                //get products data by shopping cart items
                var itemsData = await cart.Where(item => item.ProductId != 0).SelectAwait(async item =>
                {
                    var product = await _productService.GetProductByIdAsync(item.ProductId);

					logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): product → {Product}", cartItem, product);
                    //try to get product attribute combination
                    var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, item.AttributesXml);

					logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): combination → {Combination}", cartItem, combination);
                    //get default product picture
                    var picture = await _pictureService.GetProductPictureAsync(product, item.AttributesXml);

					logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): picture → {Picture}", cartItem, picture);
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
						logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): name → {Name}", cartItem, name);
						logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): variant_id → {Variant_id}", cartItem, variant_id);
						logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): variant_name → {Variant_name}", cartItem, variant_name);
						logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): sku → {Sku}", cartItem, sku);
                            var res = (await _categoryService.GetCategoryByIdAsync(pc.CategoryId)).Name;
                            res = all == "," ? res : all + ", " + res;
							logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): res → {Res}", cartItem, res);
                            return res;
                        }),
                        url = await _nopUrlHelper.RouteGenericUrlAsync(product, _webHelper.GetCurrentRequestProtocol()),
                        image = (await _pictureService.GetPictureUrlAsync(picture)).Url,
                        quantity = item.Quantity,
                        price = (await _shoppingCartService.GetSubTotalAsync(item, true)).subTotal
						logger.LogInformation("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): url → {Url}", cartItem, url);
						logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): image → {Image}", cartItem, image);
						logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): quantity → {Quantity}", cartItem, quantity);
						logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): price → {Price}", cartItem, price);
                    };
                }).ToArrayAsync();

                //prepare cart data
                var cartData = new
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
					logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): affiliation → {Affiliation}", cartItem, affiliation);
					logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): subtotal → {Subtotal}", cartItem, subtotal);
					logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): shipping → {Shipping}", cartItem, shipping);
					logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): total_before_tax → {Total_before_tax}", cartItem, total_before_tax);
					logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): tax → {Tax}", cartItem, tax);
					logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): discount → {Discount}", cartItem, discount);
					logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): revenue → {Revenue}", cartItem, revenue);
					logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): currency → {Currency}", cartItem, currency);
                    items = itemsData
					logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): items → {Items}", cartItem, items);
                };

                //if there is a single item in the cart, so the cart is just created
                if (cart.Count == 1)
                {
                    shoppingCartGuid = Guid.NewGuid();
                }
                else
                {
                    //otherwise cart is updated
                    shoppingCartGuid ??= Guid.NewGuid();
                }
                trackEvent.EventName = BrevoDefaults.CartUpdatedEventName;
				logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): trackEvent.EventName → {Arg1}", cartItem, trackEvent.EventName);
				logger.LogDebug("HandleShoppingCartChangedEventAsync(cartItem={CartItem}): trackEvent.EventData → {Arg1}", cartItem, trackEvent.EventData);
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
		logger.LogInformation("HandleShoppingCartChangedEventAsync(cartItem)={CartItem}: external service call took {Arg1} ms", cartItem, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting HandleShoppingCartChangedEventAsync(cartItem={CartItem})", cartItem);
    }

    /// <summary>
    /// Handle order completed event
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleOrderCompletedEventAsync(Order order)
    {
		logger.LogDebug(">>> Entering HandleOrderCompletedEventAsync(order={Order})", order);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //whether marketing automation is enabled
        if (!_brevoSettings.UseMarketingAutomation)
            return;

        ArgumentNullException.ThrowIfNull(order);

        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
        var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId ?? 0);
		logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): customer → {Customer}", order, customer);
		logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): shippingAddress → {ShippingAddress}", order, shippingAddress);
        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
        var email = customer.Email ?? billingAddress?.Email;
		logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): billingAddress → {BillingAddress}", order, billingAddress);
		logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): email → {Email}", order, email);

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

				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): product → {Product}", order, product);
                //try to get product attribute combination
                var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, item.AttributesXml);

				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): combination → {Combination}", order, combination);
                //get default product picture
                var picture = await _pictureService.GetProductPictureAsync(product, item.AttributesXml);

				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): picture → {Picture}", order, picture);
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
					logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): name → {Name}", order, name);
					logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): variant_id → {Variant_id}", order, variant_id);
					logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): variant_name → {Variant_name}", order, variant_name);
					logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): sku → {Sku}", order, sku);
                        var res = (await _categoryService.GetCategoryByIdAsync(pc.CategoryId)).Name;
                        res = all == "," ? res : all + ", " + res;
						logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): res → {Res}", order, res);
                        return res;
                    }),
                    url = await _nopUrlHelper.RouteGenericUrlAsync(product, _webHelper.GetCurrentRequestProtocol()),
                    image = (await _pictureService.GetPictureUrlAsync(picture)).Url,
                    quantity = item.Quantity,
                    price = item.PriceInclTax,
                };
					logger.LogInformation("HandleOrderCompletedEventAsync(order={Order}): url → {Url}", order, url);
					logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): image → {Image}", order, image);
					logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): quantity → {Quantity}", order, quantity);
					logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): price → {Price}", order, price);
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
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): firstname → {Firstname}", order, firstname);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): lastname → {Lastname}", order, lastname);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): company → {Company}", order, company);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): phone → {Phone}", order, phone);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): address1 → {Address1}", order, address1);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): address2 → {Address2}", order, address2);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): city → {City}", order, city);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): country → {Country}", order, country);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): state → {State}", order, state);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): zipcode → {Zipcode}", order, zipcode);
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
            };

            var store = await _storeContext.GetCurrentStoreAsync();

			logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): store → {Store}", order, store);
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
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): affiliation → {Affiliation}", order, affiliation);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): date → {Date}", order, date);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): subtotal → {Subtotal}", order, subtotal);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): shipping → {Shipping}", order, shipping);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): total_before_tax → {Total_before_tax}", order, total_before_tax);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): tax → {Tax}", order, tax);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): discount → {Discount}", order, discount);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): revenue → {Revenue}", order, revenue);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): currency → {Currency}", order, currency);
                items = itemsData,
                shipping_address = shippingAddressData,
                billing_address = billingAddressData
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): items → {Items}", order, items);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): shipping_address → {Shipping_address}", order, shipping_address);
				logger.LogDebug("HandleOrderCompletedEventAsync(order={Order}): billing_address → {Billing_address}", order, billing_address);
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

            //track event
            await _marketingAutomationHttpClient.RequestAsync(trackEvent);

            //update GUID for the current customer's shopping cart
            await _genericAttributeService.SaveAttributeAsync<Guid?>(order, BrevoDefaults.ShoppingCartGuidAttribute, null);
        }
        catch (Exception exception)
        {
            //log full error
            await _logger.ErrorAsync($"Brevo Marketing Automation error: {exception.Message}.", exception, customer);
        }
		logger.LogInformation("HandleOrderCompletedEventAsync(order)={Order}: external service call took {Arg1} ms", order, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting HandleOrderCompletedEventAsync(order={Order})", order);
    }

    /// <summary>
    /// Handle order placed event
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleOrderPlacedEventAsync(Order order)
    {
		logger.LogDebug(">>> Entering HandleOrderPlacedEventAsync(order={Order})", order);
        //whether marketing automation is enabled
        if (!_brevoSettings.UseMarketingAutomation)
            return;

        //copy shopping cart GUID to order
        var shoppingCartGuid = await _genericAttributeService.GetAttributeAsync<Customer, Guid?>(order.CustomerId, BrevoDefaults.ShoppingCartGuidAttribute);
        await _genericAttributeService.SaveAttributeAsync(order, BrevoDefaults.ShoppingCartGuidAttribute, shoppingCartGuid);
		logger.LogDebug("HandleOrderPlacedEventAsync(order={Order}): shoppingCartGuid → {ShoppingCartGuid}", order, shoppingCartGuid);
		logger.LogDebug("<<< Exiting HandleOrderPlacedEventAsync(order={Order})", order);
    }

    #endregion
}
