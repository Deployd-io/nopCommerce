using System.Globalization;
using System.Text;
using Avalara.AvaTax.RestClient;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Tax.Avalara.Domain;
using Nop.Plugin.Tax.Avalara.ItemClassificationAPI;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Tax;

namespace Nop.Plugin.Tax.Avalara.Services;

/// <summary>
/// Represents the manager that operates with requests to the Avalara services
/// </summary>
public class AvalaraTaxManager : IDisposable
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AvalaraTaxSettings _avalaraTaxSettings;
    protected readonly IAddressService _addressService;
    protected readonly IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
    protected readonly ICategoryService _categoryService;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IGeoLookupService _geoLookupService;
    protected readonly ILogger _logger;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IOrderService _orderService;
    protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
    protected readonly IPaymentService _paymentService;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;
    protected readonly IRepository<GenericAttribute> _genericAttributeRepository;
    protected readonly IRepository<TaxCategory> _taxCategoryRepository;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly ITaxCategoryService _taxCategoryService;
    protected readonly ItemClassificationHttpClient _itemClassificationHttpClient;
    protected readonly ItemClassificationService _itemClassificationService;
    protected readonly IWorkContext _workContext;
    protected readonly ShippingSettings _shippingSettings;
    protected readonly TaxSettings _taxSettings;
    protected readonly TaxTransactionLogService _taxTransactionLogService;

    protected AvaTaxClient _serviceClient;
    protected bool _disposed;

    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public AvalaraTaxManager(AvalaraTaxSettings avalaraTaxSettings,
        IAddressService addressService,
        IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
        ICategoryService categoryService,
        ICountryService countryService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        IGeoLookupService geoLookupService,
        ILogger logger,
        INopFileProvider fileProvider,
        IOrderService orderService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IPaymentService paymentService,
        IProductAttributeService productAttributeService,
        IProductService productService,
        IRepository<GenericAttribute> genericAttributeRepository,
        IRepository<TaxCategory> taxCategoryRepository,
        IShoppingCartService shoppingCartService,
        IStateProvinceService stateProvinceService,
        IStaticCacheManager staticCacheManager,
        ITaxCategoryService taxCategoryService,
        ItemClassificationHttpClient itemClassificationHttpClient,
        ItemClassificationService itemClassificationService,
        IWorkContext workContext,
        ShippingSettings shippingSettings,
        TaxSettings taxSettings,
        TaxTransactionLogService taxTransactionLogService)
    {
        _avalaraTaxSettings = avalaraTaxSettings;
        _addressService = addressService;
        _checkoutAttributeParser = checkoutAttributeParser;
        _categoryService = categoryService;
        _countryService = countryService;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _geoLookupService = geoLookupService;
        _logger = logger;
        _fileProvider = fileProvider;
        _orderService = orderService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _paymentService = paymentService;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _genericAttributeRepository = genericAttributeRepository;
        _taxCategoryRepository = taxCategoryRepository;
        _shoppingCartService = shoppingCartService;
        _stateProvinceService = stateProvinceService;
        _staticCacheManager = staticCacheManager;
        _taxCategoryService = taxCategoryService;
        _itemClassificationHttpClient = itemClassificationHttpClient;
        _itemClassificationService = itemClassificationService;
        _workContext = workContext;
        _shippingSettings = shippingSettings;
        _taxSettings = taxSettings;
        _taxTransactionLogService = taxTransactionLogService;
    }

    #endregion

    #region Utilities

    #region Common

    /// <summary>
    /// Event handler
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="args">Event args</param>
    protected async void OnCallCompleted(object sender, EventArgs args)
    {
		_logger.LogError(">>> Entering OnCallCompleted(sender={Sender},args={Args})", sender, args);
        if (args is not AvaTaxCallEventArgs avaTaxCallEventArgs)
            return;

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("OnCallCompleted(sender={Sender},args={Args}): customer → {Customer}", sender, args, customer);

        //log request results
        await _taxTransactionLogService.InsertTaxTransactionLogAsync(new TaxTransactionLog
        {
            StatusCode = (int)avaTaxCallEventArgs.Code,
            Url = avaTaxCallEventArgs.RequestUri.ToString(),
            RequestMessage = avaTaxCallEventArgs.RequestBody,
            ResponseMessage = avaTaxCallEventArgs.ResponseString,
            CustomerId = customer.Id,
            CreatedDateUtc = DateTime.UtcNow
        });
		_logger.LogDebug("OnCallCompleted(sender={Sender},args={Args}): CreatedDateUtc → {CreatedDateUtc}", sender, args, CreatedDateUtc);
		_logger.LogWarning("<<< Exiting OnCallCompleted(sender={Sender},args={Args})", sender, args);
    }

    /// <summary>
    /// Check that tax provider is configured
    /// </summary>
    /// <returns>True if it's configured; otherwise false</returns>
    protected bool IsConfigured()
    {
		_logger.LogError(">>> Entering IsConfigured()");
        _logger.LogWarning("<<< Exiting IsConfigured()");
        return !string.IsNullOrEmpty(_avalaraTaxSettings.AccountId) && !string.IsNullOrEmpty(_avalaraTaxSettings.LicenseKey);
    }

    /// <summary>
    /// Handle function and get result
    /// </summary>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <param name="function">Function</param>
    /// <param name="logErrors">Whether to log errors</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result; error if exists
    /// </returns>
    protected async Task<(TResult Result, string Error)> HandleFunctionAsync<TResult>(Func<Task<TResult>> function, bool logErrors = true)
    {
        try
        {
            //ensure that Avalara tax provider is configured
            if (!IsConfigured())
                throw new NopException("Tax provider is not configured");

            var result = await function();

            return (result, default);
        }
        catch (Exception exception)
        {
            if (!logErrors)
                return (default, exception.Message);

            //compose an error message
            var errorMessage = exception.Message;
            if (exception is AvaTaxError avaTaxError && avaTaxError.error != null)
            {
                var errorInfo = avaTaxError.error.error;
                if (errorInfo != null)
                {
                    errorMessage = $"{errorInfo.code} - {errorInfo.message}{Environment.NewLine}";
                    if (errorInfo.details?.Any() ?? false)
                    {
                        var errorDetails = errorInfo.details.Aggregate(string.Empty, (error, detail) => $"{error}{detail.description}{Environment.NewLine}");
                        errorMessage = $"{errorMessage} Details: {errorDetails}";
                    }
                }
            }

            //log errors
            await _logger.ErrorAsync($"{AvalaraTaxDefaults.SystemName} error. {errorMessage}", exception, await _workContext.GetCurrentCustomerAsync());

            return (default, errorMessage);
        }
    }

    #endregion

    #region Tax calculation

    /// <summary>
    /// Create tax transaction
    /// </summary>
    /// <param name="model">Transaction details</param>
    /// <returns>Created transaction</returns>
    protected TransactionModel CreateTransaction(CreateTransactionModel model)
    {
		_logger.LogError(">>> Entering CreateTransaction(model={Model})", model);
        var transaction = ServiceClient.CreateTransaction(null, model)
            ?? throw new NopException("No response from the service");

        //whether there are any errors
        var errors = transaction.messages?.Where(m => !m.severity?.ToLower().Equals("success") ?? true).ToList() ?? [];
		_logger.LogError("CreateTransaction(model={Model}): errors → {Errors}", model, errors);

        if (!errors.Any())
            return transaction;

        var message = errors.Aggregate(string.Empty, (error, message) => $"{error}{message.summary}{Environment.NewLine}");
		_logger.LogError("Throwing NopException from CreateTransaction(model={Model})", model);
		_logger.LogError("CreateTransaction(model={Model}): message → {Message}", model, message);
		_logger.LogWarning("<<< Exiting CreateTransaction(model={Model})", model);
        throw new NopException(message);
    }

    /// <summary>
    /// Prepare model to create a tax transaction
    /// </summary>
    /// <param name="address">Tax address</param>
    /// <param name="customerCode">Customer code</param>
    /// <param name="documentType">Transaction document type</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the model
    /// </returns>
    protected async Task<CreateTransactionModel> PrepareTransactionModelAsync(Address address, string customerCode, DocumentType documentType)
    {
		_logger.LogError(">>> Entering PrepareTransactionModelAsync(address={Address},customerCode={CustomerCode},documentType={DocumentType})", address, customerCode, documentType);
        var model = new CreateTransactionModel
        {
            customerCode = CommonHelper.EnsureMaximumLength(customerCode, 50),
            date = DateTime.UtcNow,
            type = documentType
        };

        //set company code
        var companyCode = !string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode) && !_avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString())
            ? _avalaraTaxSettings.CompanyCode
            : null;
        model.companyCode = CommonHelper.EnsureMaximumLength(companyCode, 25);

        //set tax addresses
        model.addresses = new AddressesModel();
        var originAddress = _avalaraTaxSettings.TaxOriginAddressType switch
        {
            TaxOriginAddressType.ShippingOrigin => await _addressService.GetAddressByIdAsync(_shippingSettings.ShippingOriginAddressId),
            TaxOriginAddressType.DefaultTaxAddress => await _addressService.GetAddressByIdAsync(_taxSettings.DefaultTaxAddressId),
            _ => null
        };
        var shipFromAddress = await MapAddressAsync(originAddress);
		_logger.LogError("PrepareTransactionModelAsync(address={Address},customerCode={CustomerCode},documentType={DocumentType}): shipFromAddress → {ShipFromAddress}", address, customerCode, documentType, shipFromAddress);
        var shipToAddress = await MapAddressAsync(address);
		_logger.LogError("PrepareTransactionModelAsync(address={Address},customerCode={CustomerCode},documentType={DocumentType}): shipToAddress → {ShipToAddress}", address, customerCode, documentType, shipToAddress);
        if (shipFromAddress != null && shipToAddress != null)
        {
            model.addresses.shipFrom = shipFromAddress;
            model.addresses.shipTo = shipToAddress;
			_logger.LogError(">>> Entering MapAddressAsync(address={Address})", address);
			_logger.LogWarning("<<< Exiting MapAddressAsync(address={Address})", address);
        }
        else
            model.addresses.singleLocation = shipToAddress ?? shipFromAddress;

		_logger.LogWarning("<<< Exiting PrepareTransactionModelAsync(address={Address},customerCode={CustomerCode},documentType={DocumentType})", address, customerCode, documentType);
        return model;
    }

    /// <summary>
    /// Prepare order addresses
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="order">Order</param>
    /// <param name="storeId">Store id</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task PrepareOrderAddressesAsync(Customer customer, Order order, int storeId)
    {
		_logger.LogError(">>> Entering PrepareOrderAddressesAsync(customer={Customer},order={Order},storeId={StoreId})", customer, order, storeId);
        order.BillingAddressId = customer.BillingAddressId ?? 0;
        order.ShippingAddressId = customer.ShippingAddressId;
		_logger.LogDebug("PrepareOrderAddressesAsync(customer={Customer},order={Order},storeId={StoreId}): order.BillingAddressId → {Arg3}", customer, order, storeId, order.BillingAddressId);
        if (_shippingSettings.AllowPickupInStore)
        {
			_logger.LogDebug("PrepareOrderAddressesAsync(customer={Customer},order={Order},storeId={StoreId}): order.ShippingAddressId → {Arg3}", customer, order, storeId, order.ShippingAddressId);
            var pickupPoint = await _genericAttributeService
                .GetAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, storeId);
            if (pickupPoint != null)
            {
                var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(pickupPoint.CountryCode);
				_logger.LogError("PrepareOrderAddressesAsync(customer={Customer},order={Order},storeId={StoreId}): country → {Country}", customer, order, storeId, country);
                var state = await _stateProvinceService.GetStateProvinceByAbbreviationAsync(pickupPoint.StateAbbreviation, country?.Id);
				_logger.LogError("PrepareOrderAddressesAsync(customer={Customer},order={Order},storeId={StoreId}): state → {State}", customer, order, storeId, state);
                var pickupAddress = new Address
                {
                    Address1 = pickupPoint.Address,
                    City = pickupPoint.City,
                    CountryId = country?.Id,
                    StateProvinceId = state?.Id,
                    ZipPostalCode = pickupPoint.ZipPostalCode,
                    CreatedOnUtc = DateTime.UtcNow,
                };
                await _addressService.InsertAddressAsync(pickupAddress);
                order.PickupAddressId = pickupAddress.Id;
				_logger.LogDebug("PrepareOrderAddressesAsync(customer={Customer},order={Order},storeId={StoreId}): order.PickupAddressId → {Arg3}", customer, order, storeId, order.PickupAddressId);
            }
        }
		_logger.LogWarning("<<< Exiting PrepareOrderAddressesAsync(customer={Customer},order={Order},storeId={StoreId})", customer, order, storeId);
    }

    /// <summary>
    /// Get a tax address of the passed order
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the address
    /// </returns>
    protected async Task<Address> GetTaxAddressAsync(Order order)
    {
		_logger.LogError(">>> Entering GetTaxAddressAsync(order={Order})", order);
        Address address = null;

		_logger.LogError("GetTaxAddressAsync(order={Order}): address → {Address}", order, address);
        //tax is based on billing address
        if (_taxSettings.TaxBasedOn == TaxBasedOn.BillingAddress &&
            await _addressService.GetAddressByIdAsync(order.BillingAddressId) is Address billingAddress)
        {
            address = billingAddress;
        }

        //tax is based on shipping address
        if (_taxSettings.TaxBasedOn == TaxBasedOn.ShippingAddress &&
            order.ShippingAddressId.HasValue &&
            await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value) is Address shippingAddress)
        {
            address = shippingAddress;
        }

        //tax is based on pickup point address
        if (_taxSettings.TaxBasedOnPickupPointAddress &&
            order.PickupAddressId.HasValue &&
            await _addressService.GetAddressByIdAsync(order.PickupAddressId.Value) is Address pickupAddress)
        {
            address = pickupAddress;
        }

        //or use default address for tax calculation
        address ??= await _addressService.GetAddressByIdAsync(_taxSettings.DefaultTaxAddressId);

		_logger.LogWarning("<<< Exiting GetTaxAddressAsync(order={Order})", order);
        return address;
    }

    /// <summary>
    /// Map address model
    /// </summary>
    /// <param name="address">Address</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the address model
    /// </returns>
    protected async Task<AddressLocationInfo> MapAddressAsync(Address address)
    {
		_logger.LogError(">>> Entering MapAddressAsync(address={Address})", address);
		_logger.LogWarning("<<< Exiting MapAddressAsync(address={Address})", address);
        return address == null ? null : new AddressLocationInfo
        {
            city = CommonHelper.EnsureMaximumLength(address.City, 50),
            country = CommonHelper.EnsureMaximumLength((await _countryService.GetCountryByAddressAsync(address))?.TwoLetterIsoCode, 2),
            line1 = CommonHelper.EnsureMaximumLength(address.Address1, 50),
            line2 = CommonHelper.EnsureMaximumLength(address.Address2, 100),
            postalCode = CommonHelper.EnsureMaximumLength(address.ZipPostalCode, 11),
            region = CommonHelper.EnsureMaximumLength((await _stateProvinceService.GetStateProvinceByAddressAsync(address))?.Abbreviation, 3)
        };
    }

    /// <summary>
    /// Get item lines to create tax transaction
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="orderItems">Order items</param>
    /// <param name="countryId">Destination country</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of item lines
    /// </returns>
    protected async Task<List<LineItemModel>> GetItemLinesAsync(Order order, IList<OrderItem> orderItems, int? countryId)
    {
		_logger.LogError(">>> Entering GetItemLinesAsync(order={Order},orderItems={OrderItems},countryId={CountryId})", order, orderItems, countryId);
        //get purchased products details
        var items = await CreateLinesForOrderItemsAsync(order, orderItems, countryId);
		_logger.LogError("GetItemLinesAsync(order={Order},orderItems={OrderItems},countryId={CountryId}): items → {Items}", order, orderItems, countryId, items);

        //set payment method additional fee as the separate item line
        if (order.PaymentMethodAdditionalFeeExclTax > decimal.Zero)
            items.Add(await CreateLineForPaymentMethodAsync(order));

        //set shipping rate as the separate item line
        if (order.OrderShippingExclTax > decimal.Zero)
            items.Add(await CreateLineForShippingAsync(order));

        //set checkout attributes as the separate item lines
        if (!string.IsNullOrEmpty(order.CheckoutAttributesXml))
            items.AddRange(await CreateLinesForCheckoutAttributesAsync(order));

		_logger.LogWarning("<<< Exiting GetItemLinesAsync(order={Order},orderItems={OrderItems},countryId={CountryId})", order, orderItems, countryId);
        return items;
    }

    /// <summary>
    /// Create item lines for purchased order items
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="orderItems">Order items</param>
    /// <param name="countryId">Destination country</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the collection of item lines
    /// </returns>
    protected async Task<List<LineItemModel>> CreateLinesForOrderItemsAsync(Order order, IList<OrderItem> orderItems, int? countryId)
    {
		_logger.LogError(">>> Entering CreateLinesForOrderItemsAsync(order={Order},orderItems={OrderItems},countryId={CountryId})", order, orderItems, countryId);
        var itemsClassification = countryId > 0
            ? (await _itemClassificationService.GetItemClassificationAsync(countryId)).ToList()
            : new();

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return await orderItems.SelectAwait(async orderItem =>
        {
            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

            var item = new LineItemModel
            {
                amount = orderItem.PriceExclTax,

                //set name as item description to avoid long values
                description = CommonHelper.EnsureMaximumLength(product?.Name, 2096),

                //whether the discount to the item was applied
                discounted = order.OrderSubTotalDiscountExclTax > decimal.Zero,

                //product exemption
                exemptionCode = product?.IsTaxExempt ?? false
                    ? CommonHelper.EnsureMaximumLength($"Exempt-product-#{product.Id}", 25)
                    : string.Empty,

                //set SKU as item code
                itemCode = product != null
                    ? CommonHelper.EnsureMaximumLength(await _productService.FormatSkuAsync(product, orderItem.AttributesXml), 50)
                    : string.Empty,

                quantity = orderItem.Quantity
            };

            //set tax code
            var productTaxCategory = await _taxCategoryService.GetTaxCategoryByIdAsync(product?.TaxCategoryId ?? 0);
            item.taxCode = CommonHelper.EnsureMaximumLength(productTaxCategory?.Name, 25);

            //whether entity use code is set
            var entityUseCode = product != null
                ? await _genericAttributeService.GetAttributeAsync<string>(product, AvalaraTaxDefaults.EntityUseCodeAttribute)
                : string.Empty;
            item.customerUsageType = CommonHelper.EnsureMaximumLength(entityUseCode, 25);

            //set HS code
            item.hsCode = itemsClassification.FirstOrDefault(x => x.ProductId == orderItem.ProductId)?.HSCode;

            return item;
        }).ToListAsync();
		_logger.LogError("CreateLinesForOrderItemsAsync(order,orderItems,countryId)={Order},{OrderItems},{CountryId}: database query executed in {Arg3} ms", order, orderItems, countryId, __logStart.ElapsedMilliseconds);
		_logger.LogError(">>> Entering CreateLinesForCheckoutAttributesAsync(order={Order})", order);
		_logger.LogError("CreateLinesForCheckoutAttributesAsync(order)={Order}: database query executed in {Arg1} ms", order, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting CreateLinesForCheckoutAttributesAsync(order={Order})", order);
    }

    /// <summary>
    /// Create a separate item line for the order payment method additional fee
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the item line
    /// </returns>
    protected async Task<LineItemModel> CreateLineForPaymentMethodAsync(Order order)
    {
		_logger.LogError(">>> Entering CreateLineForPaymentMethodAsync(order={Order})", order);
        var paymentItem = new LineItemModel
        {
            amount = order.PaymentMethodAdditionalFeeExclTax,

            //item description
            description = "Payment method additional fee",

            //set payment method system name as item code
            itemCode = CommonHelper.EnsureMaximumLength(order.PaymentMethodSystemName, 50),

            quantity = 1
        };

        //whether payment is taxable
        if (_taxSettings.PaymentMethodAdditionalFeeIsTaxable)
        {
            //try to get tax code
            var paymentTaxCategory = await _taxCategoryService.GetTaxCategoryByIdAsync(_taxSettings.PaymentMethodAdditionalFeeTaxClassId);
			_logger.LogError("CreateLineForPaymentMethodAsync(order={Order}): paymentTaxCategory → {PaymentTaxCategory}", order, paymentTaxCategory);
            paymentItem.taxCode = CommonHelper.EnsureMaximumLength(paymentTaxCategory?.Name, 25);
        }
        else
        {
            //if payment is non-taxable, set it as exempt
            paymentItem.exemptionCode = "Payment-fee-non-taxable";
			_logger.LogDebug("CreateLineForPaymentMethodAsync(order={Order}): paymentItem.exemptionCode → {Arg1}", order, paymentItem.exemptionCode);
        }

		_logger.LogWarning("<<< Exiting CreateLineForPaymentMethodAsync(order={Order})", order);
        return paymentItem;
    }

    /// <summary>
    /// Create a separate item line for the order shipping charge
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the item line
    /// </returns>
    protected async Task<LineItemModel> CreateLineForShippingAsync(Order order)
    {
		_logger.LogError(">>> Entering CreateLineForShippingAsync(order={Order})", order);
        var shippingItem = new LineItemModel
        {
            amount = order.OrderShippingExclTax,

            //item description
            description = "Shipping rate",

            //set shipping method name as item code
            itemCode = CommonHelper.EnsureMaximumLength(order.ShippingMethod, 50),

            quantity = 1
        };

        //whether shipping is taxable
        if (_taxSettings.ShippingIsTaxable)
        {
            //try to get tax code
            var shippingTaxCategory = await _taxCategoryService.GetTaxCategoryByIdAsync(_taxSettings.ShippingTaxClassId);
			_logger.LogError("CreateLineForShippingAsync(order={Order}): shippingTaxCategory → {ShippingTaxCategory}", order, shippingTaxCategory);
            shippingItem.taxCode = CommonHelper.EnsureMaximumLength(shippingTaxCategory?.Name, 25);
        }
        else
        {
            //if shipping is non-taxable, set it as exempt
            shippingItem.exemptionCode = "Shipping-rate-non-taxable";
			_logger.LogDebug("CreateLineForShippingAsync(order={Order}): shippingItem.exemptionCode → {Arg1}", order, shippingItem.exemptionCode);
        }

		_logger.LogWarning("<<< Exiting CreateLineForShippingAsync(order={Order})", order);
        return shippingItem;
    }

    /// <summary>
    /// Create item lines for order checkout attributes
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the collection of item lines
    /// </returns>
    protected async Task<IEnumerable<LineItemModel>> CreateLinesForCheckoutAttributesAsync(Order order)
    {
        //get checkout attributes values
        var attributeValues = _checkoutAttributeParser.ParseAttributeValues(order.CheckoutAttributesXml);
		_logger.LogError("CreateLinesForCheckoutAttributesAsync(order={Order}): attributeValues → {AttributeValues}", order, attributeValues);
        return await attributeValues.SelectManyAwait(async attributeWithValues =>
        {
            var attribute = attributeWithValues.attribute;
            return (await attributeWithValues.values.SelectAwait(async value =>
            {
                //create line
                var checkoutAttributeItem = new LineItemModel
                {
                    amount = value.PriceAdjustment,

                    //item description
                    description = CommonHelper.EnsureMaximumLength($"{attribute.Name} ({value.Name})", 2096),

                    //whether the discount to the item was applied
                    discounted = order.OrderSubTotalDiscountExclTax > decimal.Zero,

                    //set checkout attribute name and value as item code
                    itemCode = CommonHelper.EnsureMaximumLength($"{attribute.Name}-{value.Name}", 50),

                    quantity = 1
                };

                //whether checkout attribute is tax exempt
                if (attribute.IsTaxExempt)
                    checkoutAttributeItem.exemptionCode = "Attribute-non-taxable";
                else
                {
					_logger.LogDebug("CreateLinesForCheckoutAttributesAsync(order={Order}): checkoutAttributeItem.exemptionCode → {Arg1}", order, checkoutAttributeItem.exemptionCode);
                    //or try to get tax code
                    var attributeTaxCategory = await _taxCategoryService.GetTaxCategoryByIdAsync(attribute.TaxCategoryId);
                    checkoutAttributeItem.taxCode = CommonHelper.EnsureMaximumLength(attributeTaxCategory?.Name, 25);
                }

                //whether entity use code is set
                var entityUseCode = await _genericAttributeService.GetAttributeAsync<string>(attribute, AvalaraTaxDefaults.EntityUseCodeAttribute);
                checkoutAttributeItem.customerUsageType = CommonHelper.EnsureMaximumLength(entityUseCode, 25);

                return checkoutAttributeItem;
            }).ToListAsync()).ToAsyncEnumerable();
        }).ToListAsync();
    }

    /// <summary>
    /// Prepare model tax exemption details
    /// </summary>
    /// <param name="model">Model</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the model
    /// </returns>
    protected async Task<CreateTransactionModel> PrepareModelTaxExemptionAsync(CreateTransactionModel model, Customer customer)
    {
		_logger.LogError(">>> Entering PrepareModelTaxExemptionAsync(model={Model},customer={Customer})", model, customer);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (customer.IsTaxExempt)
            model.exemptionNo = CommonHelper.EnsureMaximumLength($"Exempt-customer-#{customer.Id}", 25);
        else
        {
            var customerRole = (await _customerService.GetCustomerRolesAsync(customer)).FirstOrDefault(role => role.TaxExempt);
			_logger.LogError("PrepareModelTaxExemptionAsync(model={Model},customer={Customer}): customerRole → {CustomerRole}", model, customer, customerRole);
            if (customerRole != null)
                model.exemptionNo = CommonHelper.EnsureMaximumLength($"Exempt-{customerRole.Name}", 25);
        }

        var entityUseCode = await _genericAttributeService.GetAttributeAsync<string>(customer, AvalaraTaxDefaults.EntityUseCodeAttribute);
		_logger.LogError("PrepareModelTaxExemptionAsync(model={Model},customer={Customer}): entityUseCode → {EntityUseCode}", model, customer, entityUseCode);
        if (!string.IsNullOrEmpty(entityUseCode))
            model.customerUsageType = CommonHelper.EnsureMaximumLength(entityUseCode, 25);
        else
        {
            entityUseCode = await (await _customerService.GetCustomerRolesAsync(customer))
                .SelectAwait(async customerRole => await _genericAttributeService.GetAttributeAsync<string>(customerRole, AvalaraTaxDefaults.EntityUseCodeAttribute))
                .FirstOrDefaultAsync(code => !string.IsNullOrEmpty(code));
				_logger.LogError("PrepareModelTaxExemptionAsync(model,customer)={Model},{Customer}: database query executed in {Arg2} ms", model, customer, __logStart.ElapsedMilliseconds);
            model.customerUsageType = CommonHelper.EnsureMaximumLength(entityUseCode, 25);
        }

		_logger.LogWarning("<<< Exiting PrepareModelTaxExemptionAsync(model={Model},customer={Customer})", model, customer);
        return model;
    }

    /// <summary>
    /// Get tax rates from the file
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the tax rates list
    /// </returns>
    protected async Task<List<TaxRate>> GetTaxRatesFromFileAsync()
    {
		_logger.LogError(">>> Entering GetTaxRatesFromFileAsync()");
        //try to create file if doesn't exist
        var filePath = _fileProvider.MapPath(AvalaraTaxDefaults.TaxRatesFilePath);
		_logger.LogError("GetTaxRatesFromFileAsync(): filePath → {FilePath}", filePath);
        if (!_fileProvider.FileExists(filePath))
            await DownloadTaxRatesAsync();

        if (!_fileProvider.FileExists(filePath))
			_logger.LogError("Throwing NopException from GetTaxRatesFromFileAsync()");
            throw new NopException($"File {AvalaraTaxDefaults.TaxRatesFilePath} not found");

        //get file lines
        var text = await _fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8);
		_logger.LogError("GetTaxRatesFromFileAsync(): text → {Text}", text);
        if (string.IsNullOrEmpty(text))
            throw new NopException($"File {AvalaraTaxDefaults.TaxRatesFilePath} is empty");

        var lines = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
		_logger.LogError("GetTaxRatesFromFileAsync(): lines → {Lines}", lines);
        if (!lines.Any() || lines[0].Split(',').Length < 14)
            throw new NopException($"Unsupported file {AvalaraTaxDefaults.TaxRatesFilePath} structure");

        //prepare tax rates
        var taxRates = lines.Skip(1).Select(line =>
        {
            try
            {
                var values = line.Split(',', StringSplitOptions.TrimEntries);
				_logger.LogError("GetTaxRatesFromFileAsync(): values → {Values}", values);
                return new TaxRate
                {
                    Zip = values[0], //ZIP_CODE
                    State = values[1], //STATE_ABBREV
                    County = values[2], //COUNTY_NAME
                    City = values[3], //CITY_NAME
                    StateTax = decimal.Parse(values[4], NumberStyles.Any, CultureInfo.InvariantCulture), //STATE_SALES_TAX
                    CountyTax = decimal.Parse(values[6], NumberStyles.Any, CultureInfo.InvariantCulture), //COUNTY_SALES_TAX
                    CityTax = decimal.Parse(values[8], NumberStyles.Any, CultureInfo.InvariantCulture), //CITY_SALES_TAX
                    TotalTax = decimal.Parse(values[10], NumberStyles.Any, CultureInfo.InvariantCulture), //TOTAL_SALES_TAX
                    ShippingTaxable = string.Equals(values[11], "y", StringComparison.InvariantCultureIgnoreCase), //TAX_SHIPPING_ALONE
                    ShippingAndHadlingTaxable = string.Equals(values[12], "y", StringComparison.InvariantCultureIgnoreCase) //TAX_SHIPPING_AND_HANDLING_TOGETHER
                };
            }
            catch
            {
				_logger.LogError("Exception in GetTaxRatesFromFileAsync(): {Arg0}", exception.Message);
                return null;
            }
        }).Where(taxRate => taxRate is not null).ToList();

		_logger.LogWarning("<<< Exiting GetTaxRatesFromFileAsync()");
        return taxRates;
    }

    /// <summary>
    /// Check address details before creating a transaction
    /// </summary>
    /// <param name="address">Address to check</param>
    protected void CheckAddressDetails(AddressLocationInfo address)
    {
		_logger.LogError(">>> Entering CheckAddressDetails(address={Address})", address);
        if (address is null)
			_logger.LogError("Throwing NopException from CheckAddressDetails(address={Address})", address);
            throw new NopException("Address not set");

        //for international transactions, the two digit ISO country code is required to determine tax jurisdictions
        if (!string.IsNullOrEmpty(address.country) &&
            !string.Equals(address.country, "US", StringComparison.InvariantCultureIgnoreCase) &&
            !string.Equals(address.country, "CA", StringComparison.InvariantCultureIgnoreCase))
        {
            return;
        }

        //for US and Canadian addresses a postal code is required
        if (!string.IsNullOrEmpty(address.postalCode))
            return;

        //however a full address (street address, city, state, and zip code) will return the best tax calculation
        if (!string.IsNullOrEmpty(address.line1) && !string.IsNullOrEmpty(address.city) && !string.IsNullOrEmpty(address.region))
            return;

		_logger.LogWarning("<<< Exiting CheckAddressDetails(address={Address})", address);
        throw new NopException("Address is incomplete");
    }

    #endregion

    #region Certificates

    /// <summary>
    /// Create or update the passed customer for the company
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="companyId">Selected company id</param>
    /// <param name="customerExists">Whether the customer is already created</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer details
    /// </returns>
    protected async Task<CustomerModel> CreateOrUpdateCustomerAsync(Customer customer, int companyId, bool customerExists)
    {
		_logger.LogError(">>> Entering CreateOrUpdateCustomerAsync(customer={Customer},companyId={CompanyId},customerExists={CustomerExists})", customer, companyId, customerExists);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var defaultAddress = new Address
        {
            Address1 = customer.StreetAddress,
            Address2 = customer.StreetAddress2,
            City = customer.City,
            ZipPostalCode = customer.ZipPostalCode,
            StateProvinceId = customer.StateProvinceId,
            CountryId = customer.CountryId
        };
        var address = await MapAddressAsync(defaultAddress);
		_logger.LogError("CreateOrUpdateCustomerAsync(customer={Customer},companyId={CompanyId},customerExists={CustomerExists}): address → {Address}", customer, companyId, customerExists, address);
        var model = new CustomerModel
        {
            companyId = companyId,
            customerCode = customer.Id.ToString(),
            alternateId = customer.CustomerGuid.ToString().ToLowerInvariant(),
            name = await _customerService.GetCustomerFullNameAsync(customer),
            emailAddress = customer.Email,
            line1 = address.line1,
            line2 = address.line2,
            city = address.city,
            postalCode = address.postalCode,
            country = address.country,
            region = address.region
        };

        var customerDetails = customerExists
            ? await ServiceClient.UpdateCustomerAsync(companyId, customer.Id.ToString(), model)
            : (await ServiceClient.CreateCustomersAsync(companyId, [model]))?.FirstOrDefault();

		_logger.LogError("CreateOrUpdateCustomerAsync(customer,companyId,customerExists)={Customer},{CompanyId},{CustomerExists}: external service call took {Arg3} ms", customer, companyId, customerExists, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting CreateOrUpdateCustomerAsync(customer={Customer},companyId={CompanyId},customerExists={CustomerExists})", customer, companyId, customerExists);
        return customerDetails;
    }

    #endregion

    #region Item classification

    /// <summary>
    /// Create classification model
    /// </summary>
    /// <param name="item">Item classification</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the classification model
    /// </returns>
    protected async Task<HSClassificationModel> CreateHSClassificationAsync(ItemClassification item)
    {
		_logger.LogError(">>> Entering CreateHSClassificationAsync(item={Item})", item);
        var model = new HSClassificationModel();
        var itemModel = new ItemClassificationModel();
		_logger.LogError("CreateHSClassificationAsync(item={Item}): itemModel → {ItemModel}", item, itemModel);
        var product = await _productService.GetProductByIdAsync(item.ProductId);
		_logger.LogError("CreateHSClassificationAsync(item={Item}): product → {Product}", item, product);
		_logger.LogError("CreateHSClassificationAsync(item={Item}): model → {Model}", item, model);

        model.CountryOfDestination = (await _countryService.GetCountryByIdAsync(item.CountryId))?.TwoLetterIsoCode;
        model.Id = $"{product.Id}-{model.CountryOfDestination}";
		_logger.LogDebug("CreateHSClassificationAsync(item={Item}): model.Id → {Arg1}", item, model.Id);

        itemModel.CompanyId = _avalaraTaxSettings.CompanyId ?? 0;
		_logger.LogDebug("CreateHSClassificationAsync(item={Item}): itemModel.CompanyId → {Arg1}", item, itemModel.CompanyId);
		_logger.LogDebug("CreateHSClassificationAsync(item={Item}): itemModel.ItemCode → {Arg1}", item, itemModel.ItemCode);
		_logger.LogDebug("CreateHSClassificationAsync(item={Item}): itemModel.Description → {Arg1}", item, itemModel.Description);
        itemModel.ItemCode = product.Sku;
        itemModel.Description = $"{product.Name}. {product.ShortDescription}";
        itemModel.ParentCode = product.ParentGroupedProductId > 0 ? product.ParentGroupedProductId.ToString() : "";
        itemModel.Summary = product.FullDescription;
		_logger.LogDebug("CreateHSClassificationAsync(item={Item}): itemModel.Summary → {Arg1}", item, itemModel.Summary);

        //If the product belongs to several categories, then we will ignore it and take only the first one from the list,
        //since there is no way to transfer the entire list of categories where the product is included
        var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(item.ProductId);
		_logger.LogError("CreateHSClassificationAsync(item={Item}): productCategories → {ProductCategories}", item, productCategories);
        var category = await _categoryService.GetCategoryByIdAsync(productCategories[0].CategoryId);
		_logger.LogError("CreateHSClassificationAsync(item={Item}): category → {Category}", item, category);
        itemModel.ItemGroup = await _categoryService.GetFormattedBreadCrumbAsync(category, separator: ">");

        model.Item = itemModel;
		_logger.LogDebug("CreateHSClassificationAsync(item={Item}): model.Item → {Arg1}", item, model.Item);

		_logger.LogWarning("<<< Exiting CreateHSClassificationAsync(item={Item})", item);
        return model;
    }

    #endregion

    #endregion

    #region Methods

    #region Configuration

    /// <summary>
    /// Ping service (test conection)
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ping result
    /// </returns>
    public async Task<PingResultModel> PingAsync()
    {
		_logger.LogError(">>> Entering PingAsync()");
		_logger.LogWarning("<<< Exiting PingAsync()");
        return (await HandleFunctionAsync(() =>
            Task.FromResult(ServiceClient.Ping() ?? throw new NopException("No response from the service")))).Result;
    }

    /// <summary>
    /// Get account companies
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of companies
    /// </returns>
    public async Task<List<CompanyModel>> GetAccountCompaniesAsync()
    {
		_logger.LogError(">>> Entering GetAccountCompaniesAsync()");
        return (await HandleFunctionAsync(() =>
        {
            var result = ServiceClient.QueryCompanies(null, null, null, null, null)
                ?? throw new NopException("No response from the service");

		_logger.LogWarning("<<< Exiting GetAccountCompaniesAsync()");
            return Task.FromResult(result.value);
        })).Result;
    }

    /// <summary>
    /// Get pre-defined entity use codes
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of entity use codes
    /// </returns>
    public async Task<List<EntityUseCodeModel>> GetEntityUseCodesAsync()
    {
		_logger.LogError(">>> Entering GetEntityUseCodesAsync()");
        return (await HandleFunctionAsync(() =>
        {
            var result = ServiceClient.ListEntityUseCodes(null, null, null, null)
                ?? throw new NopException("No response from the service");

		_logger.LogWarning("<<< Exiting GetEntityUseCodesAsync()");
            return Task.FromResult(result.value);
        })).Result;
    }

    /// <summary>
    /// Get pre-defined tax code types
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the key-value pairs of tax code types
    /// </returns>
    public async Task<Dictionary<string, string>> GetTaxCodeTypesAsync()
    {
        return (await HandleFunctionAsync(() =>
        {
            var result = ServiceClient.ListTaxCodeTypes(null, null)
                ?? throw new NopException("No response from the service");

            return Task.FromResult(result.types);
        })).Result;
    }

    /// <summary>
    /// Import tax codes from Avalara services
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of imported tax codes; null in case of error
    /// </returns>
    public async Task<int?> ImportTaxCodesAsync()
    {
		_logger.LogError(">>> Entering ImportTaxCodesAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return (await HandleFunctionAsync<int?>(async () =>
        {
            //get Avalara pre-defined system tax codes (only active)
            var systemTaxCodes = await ServiceClient.ListTaxCodesAsync("isActive eq true", null, null, null)
                ?? throw new NopException("No response from the service");

            if (!systemTaxCodes.value?.Any() ?? true)
                return null;

            //get existing tax categories
            var existingTaxCategories = (await _taxCategoryService.GetAllTaxCategoriesAsync())
                .Select(taxCategory => taxCategory.Name)
                .ToList();

            //remove duplicates
            var taxCodesToImport = systemTaxCodes.value
                .Where(taxCode => !string.IsNullOrEmpty(taxCode?.taxCode) && !existingTaxCategories.Contains(taxCode.taxCode))
                .ToList();

            var importedTaxCodesNumber = 0;
            foreach (var taxCode in taxCodesToImport)
            {
                //create new tax category
                var taxCategory = new TaxCategory { Name = taxCode.taxCode };
                await _taxCategoryService.InsertTaxCategoryAsync(taxCategory);

                //save description and type
                if (!string.IsNullOrEmpty(taxCode.description))
                    await _genericAttributeService.SaveAttributeAsync(taxCategory, AvalaraTaxDefaults.TaxCodeDescriptionAttribute, taxCode.description);
                if (!string.IsNullOrEmpty(taxCode.taxCodeTypeId))
                    await _genericAttributeService.SaveAttributeAsync(taxCategory, AvalaraTaxDefaults.TaxCodeTypeAttribute, taxCode.taxCodeTypeId);

                importedTaxCodesNumber++;
            }

            return importedTaxCodesNumber;
        })).Result;
		_logger.LogError("ImportTaxCodesAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting ImportTaxCodesAsync()");
    }

    /// <summary>
    /// Export current tax codes to Avalara services
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of exported tax codes; null in case of error
    /// </returns>
    public async Task<int?> ExportTaxCodesAsync()
    {
		_logger.LogError(">>> Entering ExportTaxCodesAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return (await HandleFunctionAsync<int?>(async () =>
        {
            if (string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode) || _avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString()))
				_logger.LogError("Throwing NopException from ExportTaxCodesAsync()");
                throw new NopException("Company not selected");

            //get selected company
            var selectedCompany = (await GetAccountCompaniesAsync())
                ?.FirstOrDefault(company => _avalaraTaxSettings.CompanyCode.Equals(company?.companyCode))
                ?? throw new NopException("Failed to retrieve company");

            //get existing tax codes (only active)
            var taxCodes = await ServiceClient.ListTaxCodesByCompanyAsync(selectedCompany.id, "isActive eq true", null, null, null, null)
                ?? throw new NopException("No response from the service");

            var existingTaxCodes = taxCodes.value?.Select(taxCode => taxCode.taxCode).ToList() ?? new List<string>();

            //prepare tax codes to export
            var taxCodesToExport = await (await _taxCategoryService.GetAllTaxCategoriesAsync()).SelectAwait(async taxCategory => new TaxCodeModel
            {
                createdDate = DateTime.UtcNow,
                description = CommonHelper.EnsureMaximumLength(taxCategory.Name, 255),
                isActive = true,
                taxCode = CommonHelper.EnsureMaximumLength(taxCategory.Name, 25),
                taxCodeTypeId = CommonHelper.EnsureMaximumLength(await _genericAttributeService
                    .GetAttributeAsync<string>(taxCategory, AvalaraTaxDefaults.TaxCodeTypeAttribute) ?? "P", 2)
            }).Where(taxCode => !string.IsNullOrEmpty(taxCode.taxCode)).ToListAsync();
					var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
					_logger.LogError("ExportTaxCodesAsync(): database query executed in {Arg0} ms", __logStartDb.ElapsedMilliseconds);

            //add Avalara pre-defined system tax codes
            var systemTaxCodesResult = await ServiceClient.ListTaxCodesAsync("isActive eq true", null, null, null)
                ?? throw new NopException("No response from the service");

            var systemTaxCodes = systemTaxCodesResult.value?.Select(taxCode => taxCode.taxCode).ToList() ?? new List<string>();
            existingTaxCodes.AddRange(systemTaxCodes);

            //remove duplicates
            taxCodesToExport = taxCodesToExport.Where(taxCode => !existingTaxCodes.Contains(taxCode.taxCode)).Distinct().ToList();

            //export tax codes
            if (!taxCodesToExport.Any())
                return 0;

            //create items and get the result
            var createdTaxCodes = await ServiceClient.CreateTaxCodesAsync(selectedCompany.id, taxCodesToExport)
                ?? throw new NopException("No response from the service");

            //display results
            var result = createdTaxCodes?.Count;
            if (result.HasValue && result > 0)
                return result.Value;

            return null;
        })).Result;
		_logger.LogError("ExportTaxCodesAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting ExportTaxCodesAsync()");
    }

    /// <summary>
    /// Delete pre-defined system tax codes
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public async Task<bool> DeleteSystemTaxCodesAsync()
    {
		_logger.LogError(">>> Entering DeleteSystemTaxCodesAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return (await HandleFunctionAsync(async () =>
        {
            //get Avalara pre-defined system tax codes (only active)
            var systemTaxCodesResult = await ServiceClient.ListTaxCodesAsync("isActive eq true", null, null, null)
                ?? throw new NopException("No response from the service");

            var systemTaxCodes = systemTaxCodesResult.value?.Select(taxCode => taxCode.taxCode).ToList();
            if (!systemTaxCodes?.Any() ?? true)
                return false;

            //prepare tax categories to delete
            var categoriesIds = await _taxCategoryRepository.Table
                .Where(taxCategory => systemTaxCodes.Contains(taxCategory.Name))
                .Select(taxCategory => taxCategory.Id)
                .ToListAsync();
				var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
				_logger.LogError("DeleteSystemTaxCodesAsync(): database query executed in {Arg0} ms", __logStartDb.ElapsedMilliseconds);

            //delete tax categories
            await _taxCategoryRepository.DeleteAsync(taxCategory => categoriesIds.Contains(taxCategory.Id));
			_logger.LogError("DeleteSystemTaxCodesAsync(): delete query executed in {Arg0} ms", __logStartDb.ElapsedMilliseconds);
            await _staticCacheManager.RemoveByPrefixAsync(NopEntityCacheDefaults<TaxCategory>.Prefix);

            //delete generic attributes
            await _genericAttributeRepository
                .DeleteAsync(attribute => attribute.KeyGroup == nameof(TaxCategory) && categoriesIds.Contains(attribute.EntityId));

            return true;
        })).Result;
		_logger.LogError("DeleteSystemTaxCodesAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting DeleteSystemTaxCodesAsync()");
    }

    /// <summary>
    /// Delete generic attributes used in the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteAttributesAsync()
    {
		_logger.LogError(">>> Entering DeleteAttributesAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await DeleteSystemTaxCodesAsync();

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
        await _genericAttributeRepository.DeleteAsync(attribute =>
            attribute.Key == AvalaraTaxDefaults.EntityUseCodeAttribute ||
            attribute.Key == AvalaraTaxDefaults.TaxCodeTypeAttribute ||
            attribute.Key == AvalaraTaxDefaults.TaxCodeDescriptionAttribute);
			_logger.LogError("DeleteAttributesAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteAttributesAsync()");
    }

    /// <summary>
    /// Export items (products) with the passed ids to Avalara services
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of exported items; null in case of error
    /// </returns>
    public async Task<int?> ExportProductsAsync(string selectedIds)
    {
		_logger.LogError(">>> Entering ExportProductsAsync(selectedIds={SelectedIds})", selectedIds);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return (await HandleFunctionAsync<int?>(async () =>
        {
            if (string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode) || _avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString()))
				_logger.LogError("Throwing NopException from ExportProductsAsync(selectedIds={SelectedIds})", selectedIds);
                throw new NopException("Company not selected");

            //get selected company
            var selectedCompany = (await GetAccountCompaniesAsync())
                ?.FirstOrDefault(company => _avalaraTaxSettings.CompanyCode.Equals(company?.companyCode))
                ?? throw new NopException("Failed to retrieve company");

            //get existing items
            var items = await ServiceClient.ListItemsByCompanyAsync(selectedCompany.id, null, null, null, null, null, null)
                ?? throw new NopException("No response from the service");

            //return the paginated and filtered list
            var existingItemCodes = items.value?.Select(item => item.itemCode).ToList() ?? new List<string>();

            //prepare exported items
            var productIds = selectedIds?.Split(_separator, StringSplitOptions.RemoveEmptyEntries).Select(id => Convert.ToInt32(id)).ToArray();
            var exportedItems = new List<ItemModel>();
            foreach (var product in await _productService.GetProductsByIdsAsync(productIds))
            {
                //find product combinations
                var combinations = (await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id))
                    .Where(combination => !string.IsNullOrEmpty(combination.Sku));

                //export items with specified SKU only
                if (string.IsNullOrEmpty(product.Sku) && !combinations.Any())
                    continue;

                //prepare common properties
                var taxCategory = await _taxCategoryService.GetTaxCategoryByIdAsync(product.TaxCategoryId);
                var taxCode = CommonHelper.EnsureMaximumLength(taxCategory?.Name, 25);
                var description = CommonHelper.EnsureMaximumLength(product.Name, 255);

                //add the product as exported item
                if (!string.IsNullOrEmpty(product.Sku))
                {
                    exportedItems.Add(new ItemModel
                    {
                        createdDate = DateTime.UtcNow,
                        description = description,
                        itemCode = CommonHelper.EnsureMaximumLength(product.Sku, 50),
                        taxCode = taxCode
                    });
                }

                //add product combinations
                exportedItems.AddRange(combinations.Select(combination => new ItemModel
                {
                    createdDate = DateTime.UtcNow,
                    description = description,
                    itemCode = CommonHelper.EnsureMaximumLength(combination.Sku, 50),
                    taxCode = taxCode
                }));
            }

            //remove duplicates
            exportedItems = exportedItems.Where(item => !existingItemCodes.Contains(item.itemCode)).Distinct().ToList();

            //export items
            if (!exportedItems.Any())
                return 0;

            //create items and get the result
            var createdItems = await ServiceClient.CreateItemsAsync(selectedCompany.id, exportedItems)
                ?? throw new NopException("No response from the service");

            //display results
            var result = createdItems?.Count;
            if (result.HasValue && result > 0)
                return result.Value;

            return null;
        })).Result;
		_logger.LogError("ExportProductsAsync(selectedIds)={SelectedIds}: external service call took {Arg1} ms", selectedIds, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting ExportProductsAsync(selectedIds={SelectedIds})", selectedIds);
    }

    #endregion

    #region Validation

    /// <summary>
    /// Resolve the passed address against Avalara's address-validation system
    /// </summary>
    /// <param name="address">Address to validate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the validated address
    /// </returns>
    public async Task<AddressResolutionModel> ValidateAddressAsync(Address address)
    {
		_logger.LogError(">>> Entering ValidateAddressAsync(address={Address})", address);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogWarning("<<< Exiting ValidateAddressAsync(address={Address})", address);
        return (await HandleFunctionAsync(async () => await ServiceClient.ResolveAddressPostAsync(new AddressValidationInfo
        {
            city = CommonHelper.EnsureMaximumLength(address.City, 50),
            country = CommonHelper.EnsureMaximumLength((await _countryService.GetCountryByAddressAsync(address))?.TwoLetterIsoCode, 2),
            line1 = CommonHelper.EnsureMaximumLength(address.Address1, 50),
            line2 = CommonHelper.EnsureMaximumLength(address.Address2, 100),
            postalCode = CommonHelper.EnsureMaximumLength(address.ZipPostalCode, 11),
            region = CommonHelper.EnsureMaximumLength((await _stateProvinceService.GetStateProvinceByAddressAsync(address))?.Abbreviation, 3),
            textCase = TextCase.Mixed
        }) ?? throw new NopException("No response from the service"), false)).Result;
    }

    #endregion

    #region Tax calculation

    /// <summary>
    /// Create test tax transaction
    /// </summary>
    /// <param name="address">Tax address</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the transaction
    /// </returns>
    public async Task<TransactionModel> CreateTestTaxTransactionAsync(Address address)
    {
		_logger.LogError(">>> Entering CreateTestTaxTransactionAsync(address={Address})", address);
        return (await HandleFunctionAsync(async () =>
        {
            if (_avalaraTaxSettings.UseTaxRateTables)
            {
                var taxRates = await GetTaxRatesFromFileAsync();
                var taxRate = taxRates.FirstOrDefault(record => address.ZipPostalCode?.StartsWith(record.Zip) ?? false);
                if (taxRate?.TotalTax is null)
					_logger.LogError("Throwing NopException from CreateTestTaxTransactionAsync(address={Address})", address);
                    throw new NopException($"No rate found for zip code {address.ZipPostalCode}");

                var summary = new List<TransactionSummary>();
                if (!string.IsNullOrEmpty(taxRate.State))
                    summary.Add(new() { jurisName = taxRate.State, rate = taxRate.StateTax });
                if (!string.IsNullOrEmpty(taxRate.County))
                    summary.Add(new() { jurisName = taxRate.County, rate = taxRate.CountyTax });
                if (!string.IsNullOrEmpty(taxRate.City))
                    summary.Add(new() { jurisName = taxRate.City, rate = taxRate.CityTax });
                return new TransactionModel { totalTax = taxRate.TotalTax * 100, summary = summary };
            }

            var customer = await _workContext.GetCurrentCustomerAsync();
            //create tax transaction for a simplified item and without saving 
            var model = await PrepareTransactionModelAsync(address, customer.Id.ToString(), DocumentType.SalesOrder);
            model.lines = [new LineItemModel { amount = 100, quantity = 1 }];
			_logger.LogDebug("CreateTestTaxTransactionAsync(address={Address}): model.lines → {Arg1}", address, model.lines);
		_logger.LogWarning("<<< Exiting CreateTestTaxTransactionAsync(address={Address})", address);
            return CreateTransaction(model);
        }, false)).Result;
    }

    /// <summary>
    /// Get tax rate by request
    /// </summary>
    /// <param name="taxRateRequest">Tax rate request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the tax rate; error if exists
    /// </returns>
    public async Task<(decimal TaxRate, string Error)> GetTaxRateAsync(TaxRateRequest taxRateRequest)
    {
        if (_avalaraTaxSettings.UseTaxRateTables)
        {
            if (string.IsNullOrEmpty(taxRateRequest.Address.ZipPostalCode))
                return (decimal.Zero, "Zip/postal code not set");

            var key = _staticCacheManager
                .PrepareKeyForDefaultCache(AvalaraTaxDefaults.TaxRateByZipCacheKey, taxRateRequest.Address.ZipPostalCode);
            var taxRateByZip = await _staticCacheManager.GetAsync(key, async () => (await HandleFunctionAsync(async () =>
            {
                var taxRates = await GetTaxRatesFromFileAsync();
                var taxRate = taxRates.FirstOrDefault(record => taxRateRequest.Address.ZipPostalCode.StartsWith(record.Zip));
                return taxRate?.TotalTax * 100;
            }, false)).Result);
            if (taxRateByZip is null)
                return (decimal.Zero, "Tax rate not found for the specified address");

            return (taxRateByZip.Value, null);
        }

        //prepare cache key
        var address = await _addressService.GetAddressByIdAsync(taxRateRequest.Address.Id);
        var customer = taxRateRequest.Customer ?? await _workContext.GetCurrentCustomerAsync();
        var taxCategoryId = taxRateRequest.TaxCategoryId > 0
            ? taxRateRequest.TaxCategoryId
            : taxRateRequest.Product?.TaxCategoryId ?? 0;
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(AvalaraTaxDefaults.TaxRateCacheKey,
            _avalaraTaxSettings.GetTaxRateByAddressOnly ? null : customer,
            _avalaraTaxSettings.GetTaxRateByAddressOnly ? 0 : taxCategoryId,
            taxRateRequest.Address.Address1,
            taxRateRequest.Address.City,
            taxRateRequest.Address.StateProvinceId ?? 0,
            taxRateRequest.Address.CountryId ?? 0,
            taxRateRequest.Address.ZipPostalCode);
        if (_avalaraTaxSettings.GetTaxRateByAddressOnly && _avalaraTaxSettings.TaxRateByAddressCacheTime > 0)
            cacheKey.CacheTime = _avalaraTaxSettings.TaxRateByAddressCacheTime;

        //try to get previously cached tax rate
        var taxRate = await _staticCacheManager.GetAsync<decimal?>(cacheKey);
        if (taxRate is not null)
            return (taxRate.Value, null);

        //or specify it now
        (taxRate, var error) = await HandleFunctionAsync(async () =>
        {
            //create tax transaction for a single item and without saving
            var model = await PrepareTransactionModelAsync(address, customer.Id.ToString(), DocumentType.SalesOrder);

            //check some address details before creating a transaction
            CheckAddressDetails(model.addresses?.shipTo);

            var taxCategory = await _taxCategoryService.GetTaxCategoryByIdAsync(taxCategoryId);
            model.lines =
            [
                new ()
                {
                    amount = 100,
                    quantity = 1,
                    itemCode = CommonHelper.EnsureMaximumLength(taxRateRequest.Product?.Sku, 50),
                    taxCode = CommonHelper.EnsureMaximumLength(taxCategory?.Name, 25),
                    exemptionCode = !_avalaraTaxSettings.GetTaxRateByAddressOnly && (taxRateRequest.Product?.IsTaxExempt ?? false)
                        ? CommonHelper.EnsureMaximumLength($"Exempt-product-#{taxRateRequest.Product.Id}", 25)
                        : string.Empty
                }
            ];

            //prepare tax exemption 
            if (!_avalaraTaxSettings.GetTaxRateByAddressOnly)
                await PrepareModelTaxExemptionAsync(model, customer);

            var transaction = CreateTransaction(model);

            //we return the tax total, since we used the amount of 100 when requesting, so the total is the same as the rate
            return transaction?.totalTax;
        }, false);
        if (!string.IsNullOrEmpty(error))
            return (decimal.Zero, error);

        //and cache it
        await _staticCacheManager.SetAsync(cacheKey, taxRate);

        return (taxRate ?? decimal.Zero, null);
    }

    /// <summary>
    /// Create transaction to get tax total for the passed request
    /// </summary>
    /// <param name="taxTotalRequest">Tax total request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the transaction; error if exists
    /// </returns>
    public async Task<(TransactionModel Transaction, string Error)> CreateTaxTotalTransactionAsync(TaxTotalRequest taxTotalRequest)
    {
        return await HandleFunctionAsync(async () =>
        {
            //create dummy order to create tax transaction
            var customer = taxTotalRequest.Customer;
            var order = new Order { CustomerId = customer.Id };

            //addresses
            await PrepareOrderAddressesAsync(customer, order, taxTotalRequest.StoreId);

            //checkout attributes
            order.CheckoutAttributesXml = await _genericAttributeService
                .GetAttributeAsync<string>(customer, NopCustomerDefaults.CheckoutAttributes, taxTotalRequest.StoreId);

            //shipping method
            order.ShippingMethod = (await _genericAttributeService
                .GetAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, taxTotalRequest.StoreId))?.Name;
            order.OrderShippingExclTax = (await _orderTotalCalculationService.GetShoppingCartShippingTotalAsync(taxTotalRequest.ShoppingCart, false)).shippingTotal ?? 0;

            //payment method
            if (taxTotalRequest.UsePaymentMethodAdditionalFee)
            {
                order.PaymentMethodSystemName = await _genericAttributeService
                    .GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, taxTotalRequest.StoreId);
                if (!string.IsNullOrEmpty(order.PaymentMethodSystemName))
                    order.PaymentMethodAdditionalFeeExclTax = await _paymentService.GetAdditionalHandlingFeeAsync(taxTotalRequest.ShoppingCart, order.PaymentMethodSystemName);
            }

            //discount amount
            var (orderSubTotalDiscountExclTax, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(taxTotalRequest.ShoppingCart, false);
            order.OrderSubTotalDiscountExclTax = orderSubTotalDiscountExclTax;

            //create dummy order items
            var orderItems = await taxTotalRequest.ShoppingCart.SelectAwait(async cartItem => new OrderItem
            {
                AttributesXml = cartItem.AttributesXml,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                PriceExclTax = (await _shoppingCartService.GetSubTotalAsync(cartItem, true)).subTotal
            }).ToListAsync();

            //prepare transaction model
            var address = await GetTaxAddressAsync(order);
            var model = await PrepareTransactionModelAsync(address, customer.Id.ToString(), DocumentType.SalesOrder);
            model.email = CommonHelper.EnsureMaximumLength(customer.Email, 50);
            model.discount = order.OrderSubTotalDiscountExclTax;

            //check some address details before creating a transaction
            CheckAddressDetails(model.addresses?.shipTo);

            //set purchased item lines
            model.lines = await GetItemLinesAsync(order, orderItems, address.CountryId);

            //set whole request tax exemption
            await PrepareModelTaxExemptionAsync(model, customer);

            var transaction = CreateTransaction(model);
            if (transaction?.totalTax is null)
                throw new NopException("No response from the service");

            return transaction;
        });
    }

    /// <summary>
    /// Create tax transaction for the placed order
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the transaction
    /// </returns>
    public async Task<TransactionModel> CreateOrderTaxTransactionAsync(Order order)
    {
		_logger.LogError(">>> Entering CreateOrderTaxTransactionAsync(order={Order})", order);
        return (await HandleFunctionAsync(async () =>
        {
            //prepare transaction model
            var address = await GetTaxAddressAsync(order);
            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            var model = await PrepareTransactionModelAsync(address, customer.Id.ToString(), DocumentType.SalesInvoice);
            model.email = CommonHelper.EnsureMaximumLength(customer.Email, 50);
            model.code = CommonHelper.EnsureMaximumLength(order.CustomOrderNumber, 50);
            model.commit = _avalaraTaxSettings.CommitTransactions;
			_logger.LogDebug("CreateOrderTaxTransactionAsync(order={Order}): model.commit → {Arg1}", order, model.commit);
			_logger.LogDebug("CreateOrderTaxTransactionAsync(order={Order}): model.discount → {Arg1}", order, model.discount);
            model.discount = order.OrderSubTotalDiscountExclTax;

            //set purchased item lines
            var orderItems = await _orderService.GetOrderItemsAsync(order.Id);
            model.lines = await GetItemLinesAsync(order, orderItems, address.CountryId);

            //set whole request tax exemption
            await PrepareModelTaxExemptionAsync(model, customer);

		_logger.LogWarning("<<< Exiting CreateOrderTaxTransactionAsync(order={Order})", order);
            return CreateTransaction(model);
        })).Result;
    }

    /// <summary>
    /// Void tax transaction
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task VoidTaxTransactionAsync(Order order)
    {
		_logger.LogError(">>> Entering VoidTaxTransactionAsync(order={Order})", order);
        await HandleFunctionAsync(() =>
        {
            if (string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode) || _avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString()))
				_logger.LogError("Throwing NopException from VoidTaxTransactionAsync(order={Order})", order);
                throw new NopException("Company not selected");

            var model = new VoidTransactionModel { code = VoidReasonCode.DocVoided };
            var transaction = ServiceClient.VoidTransaction(_avalaraTaxSettings.CompanyCode, order.CustomOrderNumber, null, null, model)
                ?? throw new NopException("No response from the service");

		_logger.LogWarning("<<< Exiting VoidTaxTransactionAsync(order={Order})", order);
            return Task.FromResult(transaction);
        });
    }

    /// <summary>
    /// Delete tax transaction
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteTaxTransactionAsync(Order order)
    {
		_logger.LogError(">>> Entering DeleteTaxTransactionAsync(order={Order})", order);
        await HandleFunctionAsync(() =>
        {
            if (string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode) || _avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString()))
				_logger.LogError("Throwing NopException from DeleteTaxTransactionAsync(order={Order})", order);
                throw new NopException("Company not selected");

            var model = new VoidTransactionModel { code = VoidReasonCode.DocDeleted };
            var transaction = ServiceClient.VoidTransaction(_avalaraTaxSettings.CompanyCode, order.CustomOrderNumber, null, null, model)
                ?? throw new NopException("No response from the service");

		_logger.LogWarning("<<< Exiting DeleteTaxTransactionAsync(order={Order})", order);
            return Task.FromResult(transaction);
        });
    }

    /// <summary>
    /// Refund tax transaction
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="amountToRefund">Amount to refund</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task RefundTaxTransactionAsync(Order order, decimal amountToRefund)
    {
		_logger.LogError(">>> Entering RefundTaxTransactionAsync(order={Order},amountToRefund={AmountToRefund})", order, amountToRefund);
        await HandleFunctionAsync(() =>
        {
            if (string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode) || _avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString()))
				_logger.LogError("Throwing NopException from RefundTaxTransactionAsync(order={Order},amountToRefund={AmountToRefund})", order, amountToRefund);
                throw new NopException("Company not selected");

            //first try to get saved tax transaction
            var transaction = ServiceClient.GetTransactionByCodeAndType(_avalaraTaxSettings.CompanyCode, order.CustomOrderNumber, DocumentType.SalesInvoice, null)
                ?? throw new NopException("No response from the service");

            //create refund transaction model
            var model = new RefundTransactionModel
            {
                referenceCode = CommonHelper.EnsureMaximumLength(transaction.code, 50),
                refundDate = transaction.date ?? DateTime.UtcNow,
                refundType = RefundType.Full
            };

            //whether it's a partial refund
            var isPartialRefund = amountToRefund < order.OrderTotal;
            if (isPartialRefund)
            {
                model.refundType = RefundType.Percentage;
				_logger.LogDebug("RefundTaxTransactionAsync(order={Order},amountToRefund={AmountToRefund}): model.refundType → {Arg2}", order, amountToRefund, model.refundType);
                model.refundPercentage = amountToRefund / (order.OrderTotal - order.OrderTax) * 100;
            }

            transaction = ServiceClient.RefundTransaction(_avalaraTaxSettings.CompanyCode, transaction.code, null, null, null, model)
                ?? throw new NopException("No response from the service");

		_logger.LogWarning("<<< Exiting RefundTaxTransactionAsync(order={Order},amountToRefund={AmountToRefund})", order, amountToRefund);
            return Task.FromResult(transaction);
        });
    }

    /// <summary>
    /// Download a file listing tax rates by postal code
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DownloadTaxRatesAsync()
    {
		_logger.LogError(">>> Entering DownloadTaxRatesAsync()");
        await HandleFunctionAsync(async () =>
        {
            var file = ServiceClient.DownloadTaxRatesByZipCode(DateTime.UtcNow, null)
                ?? throw new NopException("No response from the service");

            var filePath = _fileProvider.MapPath(AvalaraTaxDefaults.TaxRatesFilePath);
			_logger.LogError("DownloadTaxRatesAsync(): filePath → {FilePath}", filePath);
            await _fileProvider.WriteAllBytesAsync(filePath, file.Data);

		_logger.LogWarning("<<< Exiting DownloadTaxRatesAsync()");
            return true;
        });
    }

    #endregion

    #region Certificates

    /// <summary>
    /// Checks whether the company is configured to use exemption certificates
    /// </summary>
    /// <param name="request">Whether to request the certificate setup</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result of check
    /// </returns>
    public async Task<bool?> GetCertificateSetupStatusAsync(bool request = false)
    {
		_logger.LogError(">>> Entering GetCertificateSetupStatusAsync(request={Request})", request);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return (await HandleFunctionAsync<bool?>(async () =>
        {
            if (_avalaraTaxSettings.CompanyId is null)
				_logger.LogError("Throwing NopException from GetCertificateSetupStatusAsync(request={Request})", request);
                throw new NopException("Company not selected");

            var provisionStatus = request
                ? await ServiceClient.RequestCertificateSetupAsync(_avalaraTaxSettings.CompanyId.Value)
                : await ServiceClient.GetCertificateSetupAsync(_avalaraTaxSettings.CompanyId.Value)
                ?? throw new NopException("Failed to get certificate setup status");

            if (provisionStatus.status == CertCaptureProvisionStatus.NotProvisioned)
                return default;

            return provisionStatus.status == CertCaptureProvisionStatus.Provisioned;
        })).Result;
		_logger.LogError("GetCertificateSetupStatusAsync(request)={Request}: external service call took {Arg1} ms", request, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetCertificateSetupStatusAsync(request={Request})", request);
    }

    /// <summary>
    /// Create a new authorization token to launch the certificates services
    /// </summary>
    /// <param name="customer">Customer for which token is creating</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated token
    /// </returns>
    public async Task<string> CreateTokenAsync(Customer customer)
    {
		_logger.LogError(">>> Entering CreateTokenAsync(customer={Customer})", customer);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return (await HandleFunctionAsync(async () =>
        {
            if (_avalaraTaxSettings.CompanyId is null)
				_logger.LogError("Throwing NopException from CreateTokenAsync(customer={Customer})", customer);
                throw new NopException("Company not selected");

            //no need to log the error if the customer is not created yet
            var customerExists = false;
            try
            {
                customerExists = await ServiceClient
                    .GetCustomerAsync(_avalaraTaxSettings.CompanyId.Value, customer.Id.ToString(), null) is not null;
            }
            catch { }
			_logger.LogError("Exception in CreateTokenAsync(customer={Customer})", customer);
            if (!customerExists)
                await CreateOrUpdateCustomerAsync(customer, _avalaraTaxSettings.CompanyId.Value, customerExists);

            var model = new CreateECommerceTokenInputModel { customerNumber = customer.Id.ToString() };
			_logger.LogError("CreateTokenAsync(customer)={Customer}: external service call took {Arg1} ms", customer, __logStart.ElapsedMilliseconds);
			_logger.LogWarning("<<< Exiting CreateTokenAsync(customer={Customer})", customer);
            return (await ServiceClient.CreateECommerceTokenAsync(_avalaraTaxSettings.CompanyId.Value, model))?.token
                ?? throw new NopException("Failed to get token");
        })).Result;
    }

    /// <summary>
    /// Get the certificate exposure zones defined by the company
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of exposure zones
    /// </returns>
    public async Task<List<ExposureZoneModel>> GetExposureZonesAsync()
    {
		_logger.LogError(">>> Entering GetExposureZonesAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return (await HandleFunctionAsync(async () =>
        {
            var result = await ServiceClient.ListCertificateExposureZonesAsync(null, null, null, null)
                ?? throw new NopException("Failed to get exposure zones");

            return result.value;
        })).Result;
		_logger.LogError("GetExposureZonesAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetExposureZonesAsync()");
    }

    /// <summary>
    /// Create or update the passed customer for the company
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer details
    /// </returns>
    public async Task<CustomerModel> CreateOrUpdateCustomerAsync(Customer customer)
    {
        return (await HandleFunctionAsync(async () =>
        {
            if (_avalaraTaxSettings.CompanyId is null)
                throw new NopException("Company not selected");

            //no need to log the error if the customer is not created yet
            var customerExists = false;
            try
            {
                customerExists = await ServiceClient
                    .GetCustomerAsync(_avalaraTaxSettings.CompanyId.Value, customer.Id.ToString(), null) is not null;
            }
            catch { }

            return await CreateOrUpdateCustomerAsync(customer, _avalaraTaxSettings.CompanyId.Value, customerExists)
                ?? throw new NopException("Failed to update customer details");
        })).Result;
    }

    /// <summary>
    /// Delete a customer with the passed identifier
    /// </summary>
    /// <param name="customerId">Customer id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer details
    /// </returns>
    public async Task<CustomerModel> DeleteCustomerAsync(int customerId)
    {
		_logger.LogError(">>> Entering DeleteCustomerAsync(customerId={CustomerId})", customerId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return (await HandleFunctionAsync(async () =>
        {
            if (_avalaraTaxSettings.CompanyId is null)
				_logger.LogError("Throwing NopException from DeleteCustomerAsync(customerId={CustomerId})", customerId);
                throw new NopException("Company not selected");

            return await ServiceClient.DeleteCustomerAsync(_avalaraTaxSettings.CompanyId.Value, customerId.ToString())
                ?? throw new NopException("Failed to delete customer");
        })).Result;
		_logger.LogError("DeleteCustomerAsync(customerId)={CustomerId}: external service call took {Arg1} ms", customerId, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting DeleteCustomerAsync(customerId={CustomerId})", customerId);
    }

    /// <summary>
    /// Get valid certificates linked to a customer in a particular country and region
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="storeId">Current store id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of certificates
    /// </returns>
    public async Task<CertificateModel> GetValidCertificatesAsync(Customer customer, int storeId)
    {
		_logger.LogError(">>> Entering GetValidCertificatesAsync(customer={Customer},storeId={StoreId})", customer, storeId);
        return (await HandleFunctionAsync(async () =>
        {
            if (_avalaraTaxSettings.CompanyId is null)
				_logger.LogError("Throwing NopException from GetValidCertificatesAsync(customer={Customer},storeId={StoreId})", customer, storeId);
                throw new NopException("Company not selected");

            //create dummy order to get selected address
            var order = new Order { CustomerId = customer.Id };
            await PrepareOrderAddressesAsync(customer, order, storeId);
            var address = await GetTaxAddressAsync(order);
            var shipTo = await MapAddressAsync(address);

            //check exemption status
            var exemptionStatus = await ServiceClient
                .ListValidCertificatesForCustomerAsync(_avalaraTaxSettings.CompanyId.Value, customer.Id.ToString(), shipTo.country, shipTo.region)
                ?? throw new NopException("Failed to get customer's certificates");

            var exempt = string.Equals(exemptionStatus.status, "Exempt", StringComparison.InvariantCultureIgnoreCase);
		_logger.LogWarning("<<< Exiting GetValidCertificatesAsync(customer={Customer},storeId={StoreId})", customer, storeId);
            return exempt ? exemptionStatus.certificate : null;
        })).Result;
    }

    /// <summary>
    /// Get all certificates linked to a customer
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of certificates
    /// </returns>
    public async Task<List<CertificateModel>> GetCustomerCertificatesAsync(Customer customer)
    {
		_logger.LogError(">>> Entering GetCustomerCertificatesAsync(customer={Customer})", customer);
        return (await HandleFunctionAsync(async () =>
        {
            if (_avalaraTaxSettings.CompanyId is null)
				_logger.LogError("Throwing NopException from GetCustomerCertificatesAsync(customer={Customer})", customer);
                throw new NopException("Company not selected");

            var certificates = await ServiceClient
                .ListCertificatesForCustomerAsync(_avalaraTaxSettings.CompanyId.Value, customer.Id.ToString(), null, null, null, null, null)
                ?? throw new NopException("Failed to get customer's certificates");

		_logger.LogWarning("<<< Exiting GetCustomerCertificatesAsync(customer={Customer})", customer);
            return certificates.value;
        })).Result;
    }

    /// <summary>
    /// Download a PDF file for the certificate
    /// </summary>
    /// <param name="certificateId">The unique ID number of the certificate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the file details
    /// </returns>
    public async Task<FileResult> DownloadCertificateAsync(int certificateId)
    {
		_logger.LogError(">>> Entering DownloadCertificateAsync(certificateId={CertificateId})", certificateId);
        return (await HandleFunctionAsync(() =>
        {
            if (_avalaraTaxSettings.CompanyId is null)
				_logger.LogError("Throwing NopException from DownloadCertificateAsync(certificateId={CertificateId})", certificateId);
                throw new NopException("Company not selected");

            var file = ServiceClient
                .DownloadCertificateImage(_avalaraTaxSettings.CompanyId.Value, certificateId, null, CertificatePreviewType.Pdf)
                ?? throw new NopException("Failed to download certificate");

		_logger.LogWarning("<<< Exiting DownloadCertificateAsync(certificateId={CertificateId})", certificateId);
            return Task.FromResult(file);
        })).Result;
    }

    /// <summary>
    /// Get an existing invitation to upload certificates on the external website
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the URL to redirect customer
    /// </returns>
    public async Task<string> GetInvitationAsync(Customer customer)
    {
		_logger.LogError(">>> Entering GetInvitationAsync(customer={Customer})", customer);
        return (await HandleFunctionAsync(async () =>
        {
            if (_avalaraTaxSettings.CompanyId is null)
				_logger.LogError("Throwing NopException from GetInvitationAsync(customer={Customer})", customer);
                throw new NopException("Company not selected");

            //create invitation for customer
            var invitationModel = new List<CreateCertExpressInvitationModel>
            {
                new() { deliveryMethod = CertificateRequestDeliveryMethod.Download }
            };
            var invitation = (await ServiceClient
                .CreateCertExpressInvitationAsync(_avalaraTaxSettings.CompanyId.Value, customer.Id.ToString(), invitationModel))
                ?.FirstOrDefault()?.invitation
                ?? throw new NopException("Failed to get invitation");

		_logger.LogWarning("<<< Exiting GetInvitationAsync(customer={Customer})", customer);
            return invitation.requestLink;
        })).Result;
    }

    #endregion

    #region Item classification

    /// <summary>
    /// Get item classification
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the classification results
    /// </returns>
    public async Task<(HSClassificationModel model, string Error)> ClassificationProductsAsync(ItemClassification item)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (_avalaraTaxSettings.CompanyId is null)
                throw new NopException("Company not selected");

            var companyId = _avalaraTaxSettings.CompanyId.ToString();
            var classificationModel = await CreateHSClassificationAsync(item);
            var classification = await _itemClassificationHttpClient
                .RequestAsync<CreateHSClassificationRequest, HSClassificationModel>(new CreateHSClassificationRequest(companyId)
                {
                    Id = classificationModel.Id,
                    CountryOfDestination = classificationModel.CountryOfDestination,
                    Item = classificationModel.Item
                });

            return classification;
        });
    }

    /// <summary>
    /// Handle webhook request
    /// </summary>
    /// <param name="request">HTTP request</param>
    /// <returns>
    /// A task that represents the asynchronous operation</returns>
    public async Task HandleItemClassificationWebhookAsync(Microsoft.AspNetCore.Http.HttpRequest request)
    {
		_logger.LogError(">>> Entering HandleItemClassificationWebhookAsync(request={Request})", request);
        await HandleFunctionAsync(async () =>
        {
            using var streamReader = new StreamReader(request.Body);
			_logger.LogError("HandleItemClassificationWebhookAsync(request={Request}): streamReader → {StreamReader}", request, streamReader);
            var requestContent = await streamReader.ReadToEndAsync();
			_logger.LogError("HandleItemClassificationWebhookAsync(request={Request}): requestContent → {RequestContent}", request, requestContent);
            if (string.IsNullOrEmpty(requestContent))
				_logger.LogError("Throwing NopException from HandleItemClassificationWebhookAsync(request={Request})", request);
                throw new NopException("Webhook request content is empty");

            //log request
            var stringBuilder = new StringBuilder();
			_logger.LogError("HandleItemClassificationWebhookAsync(request={Request}): stringBuilder → {StringBuilder}", request, stringBuilder);
            stringBuilder.Append(nameof(request.Headers));
            foreach (var header in request.Headers)
            {
                stringBuilder.AppendLine($"{header.Key}: {header.Value}");
            }
            stringBuilder.AppendLine($"{nameof(request.Path)}: {request.Path}");
            stringBuilder.AppendLine($"{nameof(request.QueryString)}: {request.QueryString}");
            stringBuilder.AppendLine($"{nameof(request.Body)}: {requestContent}");

            await _logger.InformationAsync($"{AvalaraTaxDefaults.SystemName} webhook request info. {stringBuilder}");

            //get webhook message
            var message = JsonConvert.DeserializeObject<HSClassificationModel>(requestContent);
			_logger.LogError("HandleItemClassificationWebhookAsync(request={Request}): message → {Message}", request, message);
            if (!string.IsNullOrEmpty(message.HsCode))
            {
                var itemClassification = await _itemClassificationService.GetItemClassificationByRequestIdAsync(message.Id);
				_logger.LogError("HandleItemClassificationWebhookAsync(request={Request}): itemClassification → {ItemClassification}", request, itemClassification);
                if (itemClassification is not null)
                {
                    itemClassification.HSCode = message.HsCode;
					_logger.LogDebug("HandleItemClassificationWebhookAsync(request={Request}): itemClassification.HSCode → {Arg1}", request, itemClassification.HSCode);
					_logger.LogDebug("HandleItemClassificationWebhookAsync(request={Request}): itemClassification.UpdatedOnUtc → {Arg1}", request, itemClassification.UpdatedOnUtc);
                    itemClassification.UpdatedOnUtc = DateTime.UtcNow;
                    await _itemClassificationService.UpdateItemClassificationAsync(itemClassification);
                }
            }

		_logger.LogWarning("<<< Exiting HandleItemClassificationWebhookAsync(request={Request})", request);
            return true;
        });
    }

    #endregion

    /// <summary>
    /// Dispose object
    /// </summary>
    public void Dispose()
    {
		_logger.LogError(">>> Entering Dispose()");
        Dispose(true);
        GC.SuppressFinalize(this);
		_logger.LogWarning("<<< Exiting Dispose()");
    }

    // Protected implementation of Dispose pattern.
    protected void Dispose(bool disposing)
    {
		_logger.LogError(">>> Entering Dispose(disposing={Disposing})", disposing);
        if (_disposed)
            return;

        if (disposing)
        {
            if (_serviceClient != null)
                _serviceClient.CallCompleted -= OnCallCompleted;
        }

        _disposed = true;
		_logger.LogDebug("Dispose(disposing={Disposing}): _disposed → {_disposed}", disposing, _disposed);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets client that connects to Avalara services
    /// </summary>
    protected AvaTaxClient ServiceClient
    {
        get
        {
            if (_serviceClient == null)
            {
                //create a client with credentials
                _serviceClient = new AvaTaxClient(AvalaraTaxDefaults.ApplicationName,
                        AvalaraTaxDefaults.ApplicationVersion, Environment.MachineName,
                        _avalaraTaxSettings.UseSandbox ? AvaTaxEnvironment.Sandbox : AvaTaxEnvironment.Production)
                    .WithSecurity(_avalaraTaxSettings.AccountId, _avalaraTaxSettings.LicenseKey);

                //invoke method after each request to services completed
                if (_avalaraTaxSettings.EnableLogging)
                    _serviceClient.CallCompleted += OnCallCompleted;
            }

            return _serviceClient;
        }
    }

    #endregion
}