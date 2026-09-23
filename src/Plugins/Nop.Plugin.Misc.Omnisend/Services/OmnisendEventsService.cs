using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Http;
using Nop.Plugin.Misc.Omnisend.DTO;
using Nop.Plugin.Misc.Omnisend.DTO.Events;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Mvc.Routing;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Omnisend.Services;

/// <summary>
/// Events based Platform integration service
/// </summary>
public class OmnisendEventsService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly IAddressService _addressService;
    private readonly ICategoryService _categoryService;
    private readonly ICountryService _countryService;
    private readonly ICustomerService _customerService;
    private readonly IDiscountService _discountService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly ILocalizationService _localizationService;
    private readonly IManufacturerService _manufacturerService;
    private readonly IMeasureService _measureService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IOrderService _orderService;
    private readonly IOrderTotalCalculationService _orderTotalCalculationService;
    private readonly IPaymentPluginManager _paymentPluginManager;
    private readonly IPictureService _pictureService;
    private readonly IProductService _productService;
    private readonly IProductTagService _productTagService;
    private readonly IShipmentService _shipmentService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IStateProvinceService _stateProvinceService;
    private readonly IStoreContext _storeContext;
    private readonly ITaxService _taxService;
    private readonly IWebHelper _webHelper;
    private readonly IWorkContext _workContext;
    private readonly OmnisendCustomerService _omnisendCustomerService;
    private readonly OmnisendHelper _omnisendHelper;
    private readonly OmnisendHttpClient _omnisendHttpClient;

    #endregion

    #region Ctor

    public OmnisendEventsService(IAddressService addressService,
        ICategoryService categoryService,
        ICountryService countryService,
        ICustomerService customerService,
        IDiscountService discountService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        IManufacturerService manufacturerService,
        IMeasureService measureService,
        INopUrlHelper nopUrlHelper,
        IOrderService orderService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IPaymentPluginManager paymentPluginManager,
        IPictureService pictureService,
        IProductService productService,
        IProductTagService productTagService,
        IShipmentService shipmentService,
        IShoppingCartService shoppingCartService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        ITaxService taxService,
        IWebHelper webHelper,
        IWorkContext workContext,
        OmnisendCustomerService omnisendCustomerService,
        OmnisendHelper omnisendHelper,
        OmnisendHttpClient omnisendHttpClient)
    {
        _addressService = addressService;
        _categoryService = categoryService;
        _countryService = countryService;
        _customerService = customerService;
        _discountService = discountService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _manufacturerService = manufacturerService;
        _measureService = measureService;
        _nopUrlHelper = nopUrlHelper;
        _orderService = orderService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _paymentPluginManager = paymentPluginManager;
        _pictureService = pictureService;
        _productService = productService;
        _productTagService = productTagService;
        _shipmentService = shipmentService;
        _shoppingCartService = shoppingCartService;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _taxService = taxService;
        _webHelper = webHelper;
        _workContext = workContext;
        _omnisendCustomerService = omnisendCustomerService;
        _omnisendHelper = omnisendHelper;
        _omnisendHttpClient = omnisendHttpClient;
    }

    #endregion

    #region Utilities

    private async Task SendEventAsync(CustomerEvents customerEvent)
    {
		_logger.LogError(">>> Entering SendEventAsync(customerEvent={CustomerEvent})", customerEvent);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (customerEvent == null)
            return;

        var data = JsonConvert.SerializeObject(customerEvent);
		_logger.LogError("SendEventAsync(customerEvent={CustomerEvent}): data → {Data}", customerEvent, data);
        await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.CustomerEventsApiUrl, data, HttpMethod.Post);
		_logger.LogError("SendEventAsync(customerEvent)={CustomerEvent}: external service call took {Arg1} ms", customerEvent, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting SendEventAsync(customerEvent={CustomerEvent})", customerEvent);
    }

    private async Task<CustomerEvents> CreateAddedProductToCartEventAsync(ShoppingCartItem shoppingCartItem)
    {
		_logger.LogError(">>> Entering CreateAddedProductToCartEventAsync(shoppingCartItem=<redacted>)");
        var customer = await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);

		_logger.LogError("CreateAddedProductToCartEventAsync(shoppingCartItem=<redacted>): customer → {Customer}", customer);
        var customerEvent = await _omnisendCustomerService.CreateCustomerEventAsync(customer,
            await PrepareAddedProductToCartPropertyAsync(shoppingCartItem));

		_logger.LogWarning("<<< Exiting CreateAddedProductToCartEventAsync(shoppingCartItem=<redacted>)");
        return customerEvent;
    }

    private async Task<CustomerEvents> CreateStartedCheckoutEventAsync()
    {
		_logger.LogError(">>> Entering CreateStartedCheckoutEventAsync()");
        var customer = await _workContext.GetCurrentCustomerAsync();
        var customerEvent =
            await _omnisendCustomerService.CreateCustomerEventAsync(customer,
                await PrepareStartedCheckoutPropertyAsync(customer));
				_logger.LogError("CreateStartedCheckoutEventAsync(): customer → {Customer}", customer);

		_logger.LogWarning("<<< Exiting CreateStartedCheckoutEventAsync()");
        return customerEvent;
    }

    private async Task<CustomerEvents> CreateOrderPlacedEventAsync(Order order)
    {
		_logger.LogError(">>> Entering CreateOrderPlacedEventAsync(order={Order})", order);
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

		_logger.LogError("CreateOrderPlacedEventAsync(order={Order}): customer → {Customer}", order, customer);
        var customerEvent =
            await _omnisendCustomerService.CreateCustomerEventAsync(customer,
                await PreparePlacedOrderPropertyAsync(order));

		_logger.LogWarning("<<< Exiting CreateOrderPlacedEventAsync(order={Order})", order);
		_logger.LogError(">>> Entering PrepareStartedCheckoutPropertyAsync(customer={Customer})", customer);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("PrepareStartedCheckoutPropertyAsync(customer)={Customer}: database query executed in {Arg1} ms", customer, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting PrepareStartedCheckoutPropertyAsync(customer={Customer})", customer);
        return customerEvent;
    }

    private async Task<CustomerEvents> CreateOrderPaidEventAsync(Order order)
    {
		_logger.LogError(">>> Entering CreateOrderPaidEventAsync(order={Order})", order);
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

		_logger.LogError("CreateOrderPaidEventAsync(order={Order}): customer → {Customer}", order, customer);
        var customerEvent =
            await _omnisendCustomerService.CreateCustomerEventAsync(customer,
                await PreparePlacedPaidPropertyAsync(order));

		_logger.LogWarning("<<< Exiting CreateOrderPaidEventAsync(order={Order})", order);
        return customerEvent;
    }

    private async Task<CustomerEvents> CreateOrderCanceledEventAsync(Order order)
    {
		_logger.LogError(">>> Entering CreateOrderCanceledEventAsync(order={Order})", order);
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

		_logger.LogError("CreateOrderCanceledEventAsync(order={Order}): customer → {Customer}", order, customer);
        var customerEvent =
            await _omnisendCustomerService.CreateCustomerEventAsync(customer,
                await PrepareOrderCanceledPropertyAsync(order));

		_logger.LogWarning("<<< Exiting CreateOrderCanceledEventAsync(order={Order})", order);
		_logger.LogError(">>> Entering PreparePlacedPaidPropertyAsync(order={Order})", order);
		_logger.LogWarning("<<< Exiting PreparePlacedPaidPropertyAsync(order={Order})", order);
        return customerEvent;
    }

    private async Task<CustomerEvents> CreateOrderFulfilledEventAsync(Order order)
    {
		_logger.LogError(">>> Entering CreateOrderFulfilledEventAsync(order={Order})", order);
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

		_logger.LogError("CreateOrderFulfilledEventAsync(order={Order}): customer → {Customer}", order, customer);
        var customerEvent =
            await _omnisendCustomerService.CreateCustomerEventAsync(customer,
                await PrepareOrderFulfilledPropertyAsync(order));

		_logger.LogWarning("<<< Exiting CreateOrderFulfilledEventAsync(order={Order})", order);
        return customerEvent;
    }

    private async Task<CustomerEvents> CreateOrderRefundedEventAsync(Order order)
    {
		_logger.LogError(">>> Entering CreateOrderRefundedEventAsync(order={Order})", order);
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

		_logger.LogError("CreateOrderRefundedEventAsync(order={Order}): customer → {Customer}", order, customer);
        var customerEvent =
            await _omnisendCustomerService.CreateCustomerEventAsync(customer,
                await PrepareOrderRefundedPropertyAsync(order));

		_logger.LogWarning("<<< Exiting CreateOrderRefundedEventAsync(order={Order})", order);
		_logger.LogError(">>> Entering PrepareOrderFulfilledPropertyAsync(order={Order})", order);
		_logger.LogWarning("<<< Exiting PrepareOrderFulfilledPropertyAsync(order={Order})", order);
        return customerEvent;
    }

    private async Task<AddedProductToCartProperty> PrepareAddedProductToCartPropertyAsync(
        ShoppingCartItem shoppingCartItem)
    {
		_logger.LogError(">>> Entering PrepareAddedProductToCartPropertyAsync(shoppingCartItem=<redacted>)");
        var customer = await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart,
            shoppingCartItem.StoreId);
			_logger.LogError("PrepareAddedProductToCartPropertyAsync(shoppingCartItem=<redacted>): customer → {Customer}", customer);

        var cartId = await _omnisendCustomerService.GetCartIdAsync(customer);
		_logger.LogError("PrepareAddedProductToCartPropertyAsync(shoppingCartItem=<redacted>): cartId → {CartId}", cartId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var property = new AddedProductToCartProperty
        {
            AbandonedCheckoutURL = _omnisendCustomerService.GetAbandonedCheckoutUrl(cartId),
            CartId = cartId,
            Currency = await _omnisendHelper.GetPrimaryStoreCurrencyCodeAsync(),
            LineItems =
                await cart.SelectAwait(async sci => await ShoppingCartItemToProductItemAsync(sci))
                    .ToListAsync(),
            Value = (await GetShoppingCartItemPriceAsync(shoppingCartItem)).price,
            AddedItem = await ShoppingCartItemToProductItemAsync(shoppingCartItem)
        };
		_logger.LogError("PrepareAddedProductToCartPropertyAsync(shoppingCartItem)=<redacted>: database query executed in {Arg1} ms", __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting PrepareAddedProductToCartPropertyAsync(shoppingCartItem=<redacted>)");
        return property;
    }

    private async Task<StartedCheckoutProperty> PrepareStartedCheckoutPropertyAsync(Customer customer)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogError("PrepareStartedCheckoutPropertyAsync(customer={Customer}): cart → {Cart}", customer, cart);
		_logger.LogError("PrepareStartedCheckoutPropertyAsync(customer={Customer}): store → {Store}", customer, store);

        var cartSum = (await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart)).shoppingCartTotal ?? 0;
		_logger.LogError("PrepareStartedCheckoutPropertyAsync(customer={Customer}): cartSum → {CartSum}", customer, cartSum);
        var cartId = await _omnisendCustomerService.GetCartIdAsync(customer);
		_logger.LogError("PrepareStartedCheckoutPropertyAsync(customer={Customer}): cartId → {CartId}", customer, cartId);

        var property = new StartedCheckoutProperty
        {
            AbandonedCheckoutURL = _omnisendCustomerService.GetAbandonedCheckoutUrl(cartId),
            CartId = cartId,
            Currency = await _omnisendHelper.GetPrimaryStoreCurrencyCodeAsync(),
            LineItems = await cart.SelectAwait(async sci => await ShoppingCartItemToProductItemAsync(sci))
                .ToListAsync(),
            Value = (float)cartSum
        };

        return property;
    }

    private async Task<PlacedOrderProperty> PreparePlacedOrderPropertyAsync(Order order)
    {
		_logger.LogError(">>> Entering PreparePlacedOrderPropertyAsync(order={Order})", order);
        var property = new PlacedOrderProperty();
        await FillOrderEventBaseAsync(property, order);
		_logger.LogError("PreparePlacedOrderPropertyAsync(order={Order}): property → {Property}", order, property);

		_logger.LogWarning("<<< Exiting PreparePlacedOrderPropertyAsync(order={Order})", order);
        return property;
    }

    private async Task<PaidForOrderProperty> PreparePlacedPaidPropertyAsync(Order order)
    {
        var property = new PaidForOrderProperty();
        await FillOrderEventBaseAsync(property, order);
		_logger.LogError("PreparePlacedPaidPropertyAsync(order={Order}): property → {Property}", order, property);

        return property;
    }

    private async Task<OrderCanceledProperty> PrepareOrderCanceledPropertyAsync(Order order)
    {
		_logger.LogError(">>> Entering PrepareOrderCanceledPropertyAsync(order={Order})", order);
        var property = new OrderCanceledProperty();
        await FillOrderEventBaseAsync(property, order);
		_logger.LogError("PrepareOrderCanceledPropertyAsync(order={Order}): property → {Property}", order, property);
		_logger.LogDebug("PrepareOrderCanceledPropertyAsync(order={Order}): property.CancelReason → {Arg1}", order, property.CancelReason);
        property.CancelReason = null;

		_logger.LogWarning("<<< Exiting PrepareOrderCanceledPropertyAsync(order={Order})", order);
        return property;
    }

    private async Task<OrderFulfilledProperty> PrepareOrderFulfilledPropertyAsync(Order order)
    {
        var property = new OrderFulfilledProperty();
        await FillOrderEventBaseAsync(property, order);
		_logger.LogError("PrepareOrderFulfilledPropertyAsync(order={Order}): property → {Property}", order, property);

        return property;
    }

    private async Task<OrderRefundedProperty> PrepareOrderRefundedPropertyAsync(Order order)
    {
		_logger.LogError(">>> Entering PrepareOrderRefundedPropertyAsync(order={Order})", order);
        var property = new OrderRefundedProperty();
        await FillOrderEventBaseAsync(property, order);
		_logger.LogError("PrepareOrderRefundedPropertyAsync(order={Order}): property → {Property}", order, property);
        property.TotalRefundedAmount = (float)order.RefundedAmount;

		_logger.LogWarning("<<< Exiting PrepareOrderRefundedPropertyAsync(order={Order})", order);
        return property;
    }

    private async Task<ProductItem> ShoppingCartItemToProductItemAsync(ShoppingCartItem shoppingCartItem)
    {
		_logger.LogError(">>> Entering ShoppingCartItemToProductItemAsync(shoppingCartItem=<redacted>)");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);

		_logger.LogError("ShoppingCartItemToProductItemAsync(shoppingCartItem=<redacted>): product → {Product}", product);
        var (sku, variantId) =
            await _omnisendHelper.GetSkuAndVariantIdAsync(product, shoppingCartItem.AttributesXml);
        var (price, discount) = await GetShoppingCartItemPriceAsync(shoppingCartItem);
        var picture = await _pictureService.GetProductPictureAsync(product, shoppingCartItem.AttributesXml);
		_logger.LogError("ShoppingCartItemToProductItemAsync(shoppingCartItem=<redacted>): picture → {Picture}", picture);
        var (pictureUrl, _) = await _pictureService.GetPictureUrlAsync(picture);

        var productItem = new ProductItem
        {
            ProductCategories = await GetProductCategoriesAsync(product),
            ProductDescription = product.ShortDescription,
            ProductDiscount = discount,
            ProductId = product.Id,
            ProductImageURL = pictureUrl,
            ProductPrice = price,
            ProductQuantity = shoppingCartItem.Quantity,
            ProductSku = sku,
            ProductStrikeThroughPrice = (float)product.OldPrice,
            ProductTitle = product.Name,
            ProductURL = await _omnisendHelper.GetProductUrlAsync(product),
            ProductVariantId = variantId,
            ProductVariantImageURL = pictureUrl
        };

		_logger.LogWarning("<<< Exiting ShoppingCartItemToProductItemAsync(shoppingCartItem=<redacted>)");
		_logger.LogError(">>> Entering FillOrderEventBaseAsync(property={Property},order={Order})", property, order);
		_logger.LogError("FillOrderEventBaseAsync(property,order)={Property},{Order}: database query executed in {Arg2} ms", property, order, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("FillOrderEventBaseAsync(property={Property},order={Order}): property.Note → {Arg2}", property, order, property.Note);
		_logger.LogDebug("FillOrderEventBaseAsync(property={Property},order={Order}): property.OrderId → {Arg2}", property, order, property.OrderId);
		_logger.LogDebug("FillOrderEventBaseAsync(property={Property},order={Order}): property.OrderNumber → {Arg2}", property, order, property.OrderNumber);
		_logger.LogDebug("FillOrderEventBaseAsync(property={Property},order={Order}): property.PaymentMethod → {Arg2}", property, order, property.PaymentMethod);
		_logger.LogDebug("FillOrderEventBaseAsync(property={Property},order={Order}): property.ShippingMethod → {Arg2}", property, order, property.ShippingMethod);
		_logger.LogDebug("FillOrderEventBaseAsync(property={Property},order={Order}): property.SubTotalTaxIncluded → {Arg2}", property, order, property.SubTotalTaxIncluded);
		_logger.LogDebug("FillOrderEventBaseAsync(property={Property},order={Order}): property.Tags → {Arg2}", property, order, property.Tags);
		_logger.LogDebug("FillOrderEventBaseAsync(property={Property},order={Order}): property.Tracking → {Arg2}", property, order, property.Tracking);
		_logger.LogWarning("<<< Exiting FillOrderEventBaseAsync(property={Property},order={Order})", property, order);
        return productItem;
    }

    private async Task FillOrderEventBaseAsync(OrderEventBaseProperty property, Order order)
    {
        var items = await _orderService.GetOrderItemsAsync(order.Id);
        var appliedDiscounts = await _discountService.GetAllDiscountUsageHistoryAsync(orderId: order.Id);
		_logger.LogError("FillOrderEventBaseAsync(property={Property},order={Order}): appliedDiscounts → {AppliedDiscounts}", property, order, appliedDiscounts);
		_logger.LogError("FillOrderEventBaseAsync(property={Property},order={Order}): items → {Items}", property, order, items);

        var paymentMethodName = await _paymentPluginManager.LoadPluginBySystemNameAsync(order.PaymentMethodSystemName) is { } plugin
            ? await _localizationService.GetLocalizedFriendlyNameAsync(plugin, order.CustomerLanguageId)
            : order.PaymentMethodSystemName;

        property.BillingAddress = await GetAddressItemDataAsync(order.BillingAddressId);
        property.CreatedAt = order.CreatedOnUtc.ToDtoString();
        property.Currency = await _omnisendHelper.GetPrimaryStoreCurrencyCodeAsync();
        property.Discounts = await appliedDiscounts.SelectAwait(async duh =>
        {
            var discount = await _discountService.GetDiscountByIdAsync(duh.DiscountId);
			_logger.LogError("FillOrderEventBaseAsync(property={Property},order={Order}): discount → {Discount}", property, order, discount);

            return new DiscountItem
            {
                Amount = (float)discount.DiscountAmount,
                Code = discount.CouponCode,
                Type = discount.DiscountType.ToString()
            };
        }).ToListAsync();
        property.FulfillmentStatus = order.OrderStatus.ToString();
        property.LineItems =
            await items.SelectAwait(async oi => await OrderItemToProductItemAsync(oi)).ToListAsync();
        property.Note = null;
        property.OrderId = order.CustomOrderNumber;
        property.OrderNumber = order.Id;
        property.OrderStatusURL = _nopUrlHelper.RouteUrl(NopRouteNames.Standard.ORDER_DETAILS, new { orderId = order.Id }, _webHelper.GetCurrentRequestProtocol());
        property.PaymentMethod = paymentMethodName;
        property.PaymentStatus = order.PaymentStatus.ToString();
        property.ShippingAddress = await GetAddressItemDataAsync(order.ShippingAddressId);
        property.ShippingMethod = order.ShippingMethod;
        property.ShippingPrice = (float)order.OrderShippingInclTax;
        property.SubTotalPrice = (float)order.OrderSubtotalInclTax;
        property.SubTotalTaxIncluded = true;
        property.Tags = null;
        property.TotalDiscount = (float)order.OrderDiscount;
        property.TotalPrice = (float)order.OrderTotal;
        property.TotalTax = (float)order.OrderTax;

        if ((await _shipmentService.GetShipmentsByOrderIdAsync(order.Id)).LastOrDefault() is { } shipment &&
            await _shipmentService.GetShipmentTrackerAsync(shipment) is { } shipmentTracker)
            property.Tracking = new TrackingItem
            {
                Code = shipment.TrackingNumber,
                CourierURL = await shipmentTracker.GetUrlAsync(shipment.TrackingNumber, shipment)
            };
    }

    private async Task<OrderProductItem> OrderItemToProductItemAsync(OrderItem orderItem)
    {
		_logger.LogError(">>> Entering OrderItemToProductItemAsync(orderItem={OrderItem})", orderItem);
        var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

		_logger.LogError("OrderItemToProductItemAsync(orderItem={OrderItem}): product → {Product}", orderItem, product);
        var (sku, variantId) = await _omnisendHelper.GetSkuAndVariantIdAsync(product, orderItem.AttributesXml);

        var picture = await _pictureService.GetProductPictureAsync(product, orderItem.AttributesXml);
		_logger.LogError("OrderItemToProductItemAsync(orderItem={OrderItem}): picture → {Picture}", orderItem, picture);
        var (pictureUrl, _) = await _pictureService.GetPictureUrlAsync(picture);

        var productManufacturer = (await _manufacturerService.GetProductManufacturersByProductIdAsync(orderItem.ProductId)).FirstOrDefault();
		_logger.LogError("OrderItemToProductItemAsync(orderItem={OrderItem}): productManufacturer → {ProductManufacturer}", orderItem, productManufacturer);
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(productManufacturer?.ManufacturerId ?? 0);
		_logger.LogError("OrderItemToProductItemAsync(orderItem={OrderItem}): manufacturer → {Manufacturer}", orderItem, manufacturer);
        var productsTags = await _productTagService.GetAllProductTagsByProductIdAsync(product.Id);
		_logger.LogError("OrderItemToProductItemAsync(orderItem={OrderItem}): productsTags → {ProductsTags}", orderItem, productsTags);

        var weight = await _measureService.GetMeasureWeightBySystemKeywordAsync("grams") is { } measureWeight
            ? await _measureService.ConvertFromPrimaryMeasureWeightAsync(orderItem.ItemWeight ?? 0, measureWeight)
            : 0;

        float discount = 0;
		_logger.LogError("OrderItemToProductItemAsync(orderItem={OrderItem}): discount → {Discount}", orderItem, discount);

        if (orderItem.DiscountAmountInclTax > 0 && orderItem.Quantity > 0)
            discount = (float)orderItem.DiscountAmountInclTax / orderItem.Quantity;

        var productItem = new OrderProductItem
        {
            ProductCategories = await GetProductCategoriesAsync(product),
            ProductDescription = product.ShortDescription,
            ProductDiscount = discount,
            ProductId = product.Id,
            ProductImageURL = pictureUrl,
            ProductPrice = (float)orderItem.UnitPriceInclTax + discount,
            ProductQuantity = orderItem.Quantity,
            ProductSku = sku,
            ProductStrikeThroughPrice = (float)product.OldPrice,
            ProductTags = productsTags.Select(tag => tag.Name).ToList(),
            ProductTitle = product.Name,
            ProductURL = await _omnisendHelper.GetProductUrlAsync(product),
            ProductVariantId = variantId,
            ProductVariantImageURL = pictureUrl,
            ProductVendor = manufacturer?.Name,
            ProductWeight = (int)weight
        };

		_logger.LogWarning("<<< Exiting OrderItemToProductItemAsync(orderItem={OrderItem})", orderItem);
        return productItem;
    }

    private async Task<AddressItem> GetAddressItemDataAsync(int? addressId)
    {
		_logger.LogError(">>> Entering GetAddressItemDataAsync(addressId={AddressId})", addressId);
        var address = await _addressService.GetAddressByIdAsync(addressId ?? 0);

		_logger.LogError("GetAddressItemDataAsync(addressId={AddressId}): address → {Address}", addressId, address);
        if (address == null)
            return null;

        var country = await _countryService.GetCountryByIdAsync(address.CountryId ?? 0);
		_logger.LogError("GetAddressItemDataAsync(addressId={AddressId}): country → {Country}", addressId, country);
        var state = await _stateProvinceService.GetStateProvinceByIdAsync(address.StateProvinceId ?? 0);
		_logger.LogError("GetAddressItemDataAsync(addressId={AddressId}): state → {State}", addressId, state);

		_logger.LogWarning("<<< Exiting GetAddressItemDataAsync(addressId={AddressId})", addressId);
        return new AddressItem
        {
            Address1 = address.Address1,
            Address2 = address.Address2,
            City = address.City,
            Company = address.Company,
            Country = country?.Name,
            CountryCode = country?.TwoLetterIsoCode,
            FirstName = address.FirstName,
            LastName = address.LastName,
            Phone = address.PhoneNumber,
            State = state?.Name,
            StateCode = state?.Abbreviation,
            Zip = address.ZipPostalCode,
        };
    }

    private async Task<(float price, float discountAmount)> GetShoppingCartItemPriceAsync(
        ShoppingCartItem shoppingCartItem)
    {
        var customer = await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);
        var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);

        var (scSubTotal, discountAmount, _, _) =
            await _shoppingCartService.GetSubTotalAsync(shoppingCartItem, true);
        var price = (float)(await _taxService.GetProductPriceAsync(product, scSubTotal, true, customer)).price;

        return (price, (float)discountAmount);
    }

    private async Task<List<ProductItem.ProductItemCategories>> GetProductCategoriesAsync(Product product)
    {
		_logger.LogError(">>> Entering GetProductCategoriesAsync(product={Product})", product);
        var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id);

		_logger.LogError("GetProductCategoriesAsync(product={Product}): productCategories → {ProductCategories}", product, productCategories);
		_logger.LogWarning("<<< Exiting GetProductCategoriesAsync(product={Product})", product);
        return await productCategories.SelectAwait(async pc => new ProductItem.ProductItemCategories
        {
            Id = pc.Id,
            Title = (await _categoryService.GetCategoryByIdAsync(pc.CategoryId)).Name
        }).ToListAsync();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Send "added product to cart" event
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    public async Task SendAddedProductToCartEventAsync(ShoppingCartItem shoppingCartItem)
    {
		_logger.LogError(">>> Entering SendAddedProductToCartEventAsync(shoppingCartItem=<redacted>)");
        await SendEventAsync(await CreateAddedProductToCartEventAsync(shoppingCartItem));
		_logger.LogWarning("<<< Exiting SendAddedProductToCartEventAsync(shoppingCartItem=<redacted>)");
    }

    /// <summary>
    /// Send "order placed" event
    /// </summary>
    /// <param name="order">Order</param>
    public async Task SendOrderPlacedEventAsync(Order order)
    {
		_logger.LogError(">>> Entering SendOrderPlacedEventAsync(order={Order})", order);
        await SendEventAsync(await CreateOrderPlacedEventAsync(order));
		_logger.LogWarning("<<< Exiting SendOrderPlacedEventAsync(order={Order})", order);
    }

    /// <summary>
    /// Send "order paid" event
    /// </summary>
    /// <param name="eventMessage">Order paid event</param>
    public async Task SendOrderPaidEventAsync(OrderPaidEvent eventMessage)
    {
		_logger.LogError(">>> Entering SendOrderPaidEventAsync(eventMessage={EventMessage})", eventMessage);
        await SendEventAsync(await CreateOrderPaidEventAsync(eventMessage.Order));
		_logger.LogWarning("<<< Exiting SendOrderPaidEventAsync(eventMessage={EventMessage})", eventMessage);
    }

    /// <summary>
    /// Send "order refunded" event
    /// </summary>
    /// <param name="eventMessage">Order refunded event</param>
    public async Task SendOrderRefundedEventAsync(OrderRefundedEvent eventMessage)
    {
		_logger.LogError(">>> Entering SendOrderRefundedEventAsync(eventMessage={EventMessage})", eventMessage);
        if (eventMessage.Order.PaymentStatus == PaymentStatus.Refunded)
            await SendEventAsync(await CreateOrderRefundedEventAsync(eventMessage.Order));
			_logger.LogWarning("<<< Exiting SendOrderRefundedEventAsync(eventMessage={EventMessage})", eventMessage);
    }

    /// <summary>
    /// Send "order canceled" or "order fulfilled" events
    /// </summary>
    /// <param name="eventMessage">Order status changed event</param>
    public async Task SendOrderStatusChangedEventAsync(OrderStatusChangedEvent eventMessage)
    {
		_logger.LogError(">>> Entering SendOrderStatusChangedEventAsync(eventMessage={EventMessage})", eventMessage);
        var order = eventMessage.Order;

        if (eventMessage.PreviousOrderStatus == order.OrderStatus)
            return;

        switch (order.OrderStatus)
        {
            case OrderStatus.Cancelled:
            {
                var sent = await _genericAttributeService.GetAttributeAsync<bool>(order, OmnisendDefaults.OrderCanceledAttribute);
				_logger.LogError("SendOrderStatusChangedEventAsync(eventMessage={EventMessage}): sent → {Sent}", eventMessage, sent);

                if (sent)
                    return;

                await SendEventAsync(await CreateOrderCanceledEventAsync(order));

                await _genericAttributeService.SaveAttributeAsync(order, OmnisendDefaults.OrderCanceledAttribute, true);

                break;
            }
            case OrderStatus.Complete:
            {
                var sent = await _genericAttributeService.GetAttributeAsync<bool>(order, OmnisendDefaults.OrderFulfilledAttribute);

                if (sent)
                    return;

                await SendEventAsync(await CreateOrderFulfilledEventAsync(order));

                await _genericAttributeService.SaveAttributeAsync(order, OmnisendDefaults.OrderFulfilledAttribute, true);

                break;
            }
        }
		_logger.LogWarning("<<< Exiting SendOrderStatusChangedEventAsync(eventMessage={EventMessage})", eventMessage);
    }

    /// <summary>
    /// Send "started checkout" event
    /// </summary>
    /// <param name="eventMessage">Page rendering event</param>
    public async Task SendStartedCheckoutEventAsync(PageRenderingEvent eventMessage)
    {
		_logger.LogError(">>> Entering SendStartedCheckoutEventAsync(eventMessage={EventMessage})", eventMessage);
        var routeName = eventMessage.GetRouteName();
        if (!routeName.Equals(NopRouteNames.Standard.CHECKOUT_ONE_PAGE, StringComparison.InvariantCultureIgnoreCase) &&
            !routeName.Equals(NopRouteNames.Standard.CHECKOUT_BILLING_ADDRESS, StringComparison.InvariantCultureIgnoreCase))
            return;

        await SendEventAsync(await CreateStartedCheckoutEventAsync());
		_logger.LogWarning("<<< Exiting SendStartedCheckoutEventAsync(eventMessage={EventMessage})", eventMessage);
    }

    #endregion
}