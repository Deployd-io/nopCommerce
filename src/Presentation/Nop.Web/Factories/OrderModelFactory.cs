using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Vendors;
using Nop.Web.Infrastructure;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;
using Nop.Web.Models.Order;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the order model factory
/// </summary>
public partial class OrderModelFactory : IOrderModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly ICountryService _countryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IGiftCardService _giftCardService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderService _orderService;
    protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IPaymentService _paymentService;
    protected readonly IPictureService _pictureService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductService _productService;
    protected readonly IRewardPointService _rewardPointService;
    protected readonly IShipmentService _shipmentService;
    protected readonly IShortTermCacheManager _shortTermCacheManager;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVendorService _vendorService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly OrderSettings _orderSettings;
    protected readonly PdfSettings _pdfSettings;
    protected readonly RewardPointsSettings _rewardPointsSettings;
    protected readonly ShippingSettings _shippingSettings;
    protected readonly TaxSettings _taxSettings;
    protected readonly VendorSettings _vendorSettings;

    #endregion

    #region Ctor

    public OrderModelFactory(AddressSettings addressSettings,
        CatalogSettings catalogSettings,
        IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        ICountryService countryService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IGiftCardService giftCardService,
        ILocalizationService localizationService,
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IPaymentPluginManager paymentPluginManager,
        IPaymentService paymentService,
        IPictureService pictureService,
        IPriceFormatter priceFormatter,
        IProductService productService,
        IRewardPointService rewardPointService,
        IShipmentService shipmentService,
        IShortTermCacheManager shortTermCacheManager,
        IStateProvinceService stateProvinceService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IUrlRecordService urlRecordService,
        IVendorService vendorService,
        IWebHelper webHelper,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        OrderSettings orderSettings,
        PdfSettings pdfSettings,
        RewardPointsSettings rewardPointsSettings,
        ShippingSettings shippingSettings,
        TaxSettings taxSettings,
        VendorSettings vendorSettings)
    {
        _addressSettings = addressSettings;
        _catalogSettings = catalogSettings;
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
        _countryService = countryService;
        _currencyService = currencyService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _giftCardService = giftCardService;
        _localizationService = localizationService;
        _orderProcessingService = orderProcessingService;
        _orderService = orderService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _paymentPluginManager = paymentPluginManager;
        _paymentService = paymentService;
        _pictureService = pictureService;
        _priceFormatter = priceFormatter;
        _productService = productService;
        _rewardPointService = rewardPointService;
        _shipmentService = shipmentService;
        _shortTermCacheManager = shortTermCacheManager;
        _stateProvinceService = stateProvinceService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _urlRecordService = urlRecordService;
        _vendorService = vendorService;
        _webHelper = webHelper;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _orderSettings = orderSettings;
        _pdfSettings = pdfSettings;
        _rewardPointsSettings = rewardPointsSettings;
        _shippingSettings = shippingSettings;
        _taxSettings = taxSettings;
        _vendorSettings = vendorSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare the order item picture model
    /// </summary>
    /// <param name="orderItem">Order item</param>
    /// <param name="pictureSize">Picture size</param>
    /// <param name="showDefaultPicture">Whether to show the default picture</param>
    /// <param name="productName">Product name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture model
    /// </returns>
    protected virtual async Task<PictureModel> PrepareOrderItemPictureModelAsync(OrderItem orderItem, int pictureSize, bool showDefaultPicture, string productName)
    {
		_logger.LogError(">>> Entering PrepareOrderItemPictureModelAsync(orderItem={OrderItem},pictureSize={PictureSize},showDefaultPicture={ShowDefaultPicture},productName={ProductName})", orderItem, pictureSize, showDefaultPicture, productName);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var language = await _workContext.GetWorkingLanguageAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareOrderItemPictureModelAsync(orderItem={OrderItem},pictureSize={PictureSize},showDefaultPicture={ShowDefaultPicture},productName={ProductName}): language → {Language}", orderItem, pictureSize, showDefaultPicture, productName, language);
		_logger.LogError("PrepareOrderItemPictureModelAsync(orderItem={OrderItem},pictureSize={PictureSize},showDefaultPicture={ShowDefaultPicture},productName={ProductName}): store → {Store}", orderItem, pictureSize, showDefaultPicture, productName, store);

        var model = await _shortTermCacheManager.GetAsync(async () =>
        {
            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
			_logger.LogError("PrepareOrderItemPictureModelAsync(orderItem={OrderItem},pictureSize={PictureSize},showDefaultPicture={ShowDefaultPicture},productName={ProductName}): product → {Product}", orderItem, pictureSize, showDefaultPicture, productName, product);

            //order item picture
            var orderItemPicture = await _pictureService.GetProductPictureAsync(product, orderItem.AttributesXml);
			_logger.LogError("PrepareOrderItemPictureModelAsync(orderItem={OrderItem},pictureSize={PictureSize},showDefaultPicture={ShowDefaultPicture},productName={ProductName}): orderItemPicture → {OrderItemPicture}", orderItem, pictureSize, showDefaultPicture, productName, orderItemPicture);

            return new PictureModel
            {
                ImageUrl = (await _pictureService.GetPictureUrlAsync(orderItemPicture, pictureSize, showDefaultPicture)).Url,
                Title = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat"), productName),
                AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat"), productName),
            };
        }, NopModelCacheDefaults.OrderPictureModelKey, orderItem, pictureSize, showDefaultPicture, language, _webHelper.IsCurrentConnectionSecured(), store);

		_logger.LogError("PrepareOrderItemPictureModelAsync(orderItem,pictureSize,showDefaultPicture,productName)={OrderItem},{PictureSize},{ShowDefaultPicture},{ProductName}: external service call took {Arg4} ms", orderItem, pictureSize, showDefaultPicture, productName, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting PrepareOrderItemPictureModelAsync(orderItem={OrderItem},pictureSize={PictureSize},showDefaultPicture={ShowDefaultPicture},productName={ProductName})", orderItem, pictureSize, showDefaultPicture, productName);
        return model;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the customer order list model
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="limit">Order filtering period</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer order list model
    /// </returns>
    public virtual async Task<CustomerOrderListModel> PrepareCustomerOrderListModelAsync(int? page, OrderHistoryPeriods limit)
    {
		_logger.LogError(">>> Entering PrepareCustomerOrderListModelAsync(page={Page},limit={Limit})", page, limit);
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareCustomerOrderListModelAsync(page={Page},limit={Limit}): store → {Store}", page, limit, store);
		_logger.LogError("PrepareCustomerOrderListModelAsync(page={Page},limit={Limit}): customer → {Customer}", page, limit, customer);
        var pageSize = _orderSettings.CustomerOrdersPageSize;
        var pageIndex = Math.Max((page ?? 0) - 1, 0);
		_logger.LogError("PrepareCustomerOrderListModelAsync(page={Page},limit={Limit}): pageIndex → {PageIndex}", page, limit, pageIndex);

        var orders = await _orderService.SearchOrdersAsync(storeId: store.Id,
            customerId: customer.Id,
            createdFromUtc: limit == OrderHistoryPeriods.All ? null : DateTime.UtcNow.AddDays((int)limit * -1),
            createdToUtc: limit > 0 ? DateTime.UtcNow : null,
            pageIndex: pageIndex,
            pageSize: pageSize);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var periods = await Enum.GetValues<OrderHistoryPeriods>()
            .SelectAwait(async enumValue => new
            {
                ID = enumValue.ToString().ToLower(),
                Name = await _localizationService.GetLocalizedEnumAsync(enumValue)
            }).ToListAsync();

		_logger.LogError("PrepareCustomerOrderListModelAsync(page,limit)={Page},{Limit}: database query executed in {Arg2} ms", page, limit, __logStart.ElapsedMilliseconds);
        var model = new CustomerOrderListModel
        {
			_logger.LogDebug("PrepareCustomerOrderListModelAsync(page={Page},limit={Limit}): Name → {Name}", page, limit, Name);
            AvailableLimits = new SelectList(periods, "ID", "Name", limit.ToString()).ToList(),
            PagerModel = new PagerModel(_localizationService)
            {
                PageSize = orders.PageSize,
                TotalRecords = orders.TotalCount,
                PageIndex = orders.PageIndex,
                ShowTotalSummary = true,
                RouteActionName = NopRouteNames.Standard.CUSTOMER_ORDERS_PAGED,
                UseRouteLinks = true,
                RouteValues = new CustomerOrdersRouteValues { PageNumber = orders.PageIndex, Limit = limit.ToString().ToLower() }
            }
        };

        foreach (var order in orders)
        {
            var orderModel = new CustomerOrderModel
            {
                Id = order.Id,
                CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc),
                OrderStatusEnum = order.OrderStatus,
                OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus),
                ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus),
                IsReturnRequestAllowed = await _orderProcessingService.IsReturnRequestAllowedAsync(order),
                CustomOrderNumber = order.CustomOrderNumber
            };
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
			_logger.LogError("PrepareCustomerOrderListModelAsync(page={Page},limit={Limit}): orderTotalInCustomerCurrency → {OrderTotalInCustomerCurrency}", page, limit, orderTotalInCustomerCurrency);
            orderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, (await _workContext.GetWorkingLanguageAsync()).Id);

            model.Orders.Add(orderModel);
        }

		_logger.LogWarning("<<< Exiting PrepareCustomerOrderListModelAsync(page={Page},limit={Limit})", page, limit);
        return model;
    }

    /// <summary>
    /// Prepare the customer recurring payment list model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer recurring payment list model
    /// </returns>
    public virtual async Task<CustomerRecurringPaymentListModel> PrepareCustomerRecurringPaymentListModelAsync()
    {
		_logger.LogError(">>> Entering PrepareCustomerRecurringPaymentListModelAsync()");
        var model = new CustomerRecurringPaymentListModel();
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareCustomerRecurringPaymentListModelAsync(): customer → {Customer}", customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareCustomerRecurringPaymentListModelAsync(): store → {Store}", store);
		_logger.LogError("PrepareCustomerRecurringPaymentListModelAsync(): model → {Model}", model);

        var recurringPayments = await _orderService.SearchRecurringPaymentsAsync(store.Id,
            customer.Id);
        foreach (var recurringPayment in recurringPayments)
        {
            var order = await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId);
			_logger.LogError("PrepareCustomerRecurringPaymentListModelAsync(): order → {Order}", order);

            var recurringPaymentModel = new CustomerRecurringPaymentModel
            {
                Id = recurringPayment.Id,
                StartDate = (await _dateTimeHelper.ConvertToUserTimeAsync(recurringPayment.StartDateUtc, DateTimeKind.Utc)).ToString(),
                CycleInfo = $"{recurringPayment.CycleLength} {await _localizationService.GetLocalizedEnumAsync(recurringPayment.CyclePeriod)}",
                NextPayment = await _orderProcessingService.GetNextPaymentDateAsync(recurringPayment) is DateTime nextPaymentDate ? (await _dateTimeHelper.ConvertToUserTimeAsync(nextPaymentDate, DateTimeKind.Utc)).ToString() : "",
                TotalCycles = recurringPayment.TotalCycles,
                CyclesRemaining = await _orderProcessingService.GetCyclesRemainingAsync(recurringPayment),
                InitialOrderId = order.Id,
                InitialOrderNumber = order.CustomOrderNumber,
                CanCancel = await _orderProcessingService.CanCancelRecurringPaymentAsync(customer, recurringPayment),
                CanRetryLastPayment = await _orderProcessingService.CanRetryLastRecurringPaymentAsync(customer, recurringPayment)
            };

            model.RecurringPayments.Add(recurringPaymentModel);
        }

		_logger.LogWarning("<<< Exiting PrepareCustomerRecurringPaymentListModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare the order details model
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order details model
    /// </returns>
    public virtual async Task<OrderDetailsModel> PrepareOrderDetailsModelAsync(Order order)
    {
		_logger.LogError(">>> Entering PrepareOrderDetailsModelAsync(order={Order})", order);
        ArgumentNullException.ThrowIfNull(order);
        var model = new OrderDetailsModel
        {
            Id = order.Id,
            CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc),
            OrderStatus = order.OrderStatus,
            OrderStatusText = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
            IsReOrderAllowed = _orderSettings.IsReOrderAllowed,
            IsReturnRequestAllowed = await _orderProcessingService.IsReturnRequestAllowedAsync(order),
            PdfInvoiceDisabled = _pdfSettings.DisablePdfInvoicesForPendingOrders && order.OrderStatus == OrderStatus.Pending,
            CustomOrderNumber = order.CustomOrderNumber,

            //shipping info
            ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus)
        };

        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
		_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): languageId → {LanguageId}", order, languageId);

        if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
        {
            model.IsShippable = true;
			_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.IsShippable → {Arg1}", order, model.IsShippable);
			_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.PickupInStore → {Arg1}", order, model.PickupInStore);
            model.PickupInStore = order.PickupInStore;
            if (!order.PickupInStore)
            {
                var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId ?? 0);
				_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): shippingAddress → {ShippingAddress}", order, shippingAddress);

                await _addressModelFactory.PrepareAddressModelAsync(model.ShippingAddress,
                    address: shippingAddress,
                    excludeProperties: false,
                    addressSettings: _addressSettings);
            }
            else if (order.PickupAddressId.HasValue && await _addressService.GetAddressByIdAsync(order.PickupAddressId.Value) is Address pickupAddress)
            {
                var (addressLine, addressFields) = await _addressService.FormatAddressAsync(pickupAddress, languageId);
				_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.PickupAddress → {Arg1}", order, model.PickupAddress);
                model.PickupAddress = new AddressModel
                {
                    Address1 = pickupAddress.Address1,
                    City = pickupAddress.City,
                    County = pickupAddress.County,
                    StateProvinceName = await _stateProvinceService.GetStateProvinceByAddressAsync(pickupAddress) is StateProvince stateProvince
                        ? await _localizationService.GetLocalizedAsync(stateProvince, entity => entity.Name)
                        : string.Empty,
                    CountryName = await _countryService.GetCountryByAddressAsync(pickupAddress) is Country country
                        ? await _localizationService.GetLocalizedAsync(country, entity => entity.Name)
                        : string.Empty,
                    ZipPostalCode = pickupAddress.ZipPostalCode,
                    AddressFields = addressFields,
                    AddressLine = addressLine
                };
            }

            model.ShippingMethod = order.ShippingMethod;
			_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.ShippingMethod → {Arg1}", order, model.ShippingMethod);

            //shipments (only already shipped or ready for pickup)
            var shipments = (await _shipmentService.GetShipmentsByOrderIdAsync(order.Id, !order.PickupInStore, order.PickupInStore)).OrderBy(x => x.CreatedOnUtc).ToList();
			_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): shipments → {Shipments}", order, shipments);
            foreach (var shipment in shipments)
            {
                var shipmentModel = new OrderDetailsModel.ShipmentBriefModel
                {
                    Id = shipment.Id,
                    TrackingNumber = shipment.TrackingNumber,
                };
                if (shipment.ShippedDateUtc.HasValue)
                    shipmentModel.ShippedDate = await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ShippedDateUtc.Value, DateTimeKind.Utc);
                if (shipment.ReadyForPickupDateUtc.HasValue)
                    shipmentModel.ReadyForPickupDate = await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ReadyForPickupDateUtc.Value, DateTimeKind.Utc);
                if (shipment.DeliveryDateUtc.HasValue)
                    shipmentModel.DeliveryDate = await _dateTimeHelper.ConvertToUserTimeAsync(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc);
                model.Shipments.Add(shipmentModel);
            }
        }

        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
		_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): billingAddress → {BillingAddress}", order, billingAddress);

        //billing info
        await _addressModelFactory.PrepareAddressModelAsync(model.BillingAddress,
            address: billingAddress,
            excludeProperties: false,
            addressSettings: _addressSettings);

        //VAT number
        model.VatNumber = order.VatNumber;
		_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.VatNumber → {Arg1}", order, model.VatNumber);

        //payment method
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
		_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): customer → {Customer}", order, customer);
        var paymentMethod = await _paymentPluginManager
            .LoadPluginBySystemNameAsync(order.PaymentMethodSystemName, customer, order.StoreId);
        model.PaymentMethod = paymentMethod != null ? await _localizationService.GetLocalizedFriendlyNameAsync(paymentMethod, languageId) : order.PaymentMethodSystemName;
        model.PaymentStatus = order.PaymentStatus;
		_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.PaymentStatus → {Arg1}", order, model.PaymentStatus);
        model.PaymentMethodStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);
        model.CanRePostProcessPayment = await _paymentService.CanRePostProcessPaymentAsync(order);

        //custom values
        model.CustomValues.FillByXml(order.CustomValuesXml, true);

        //order subtotal
        if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
        {
            //including tax

            //order subtotal
            var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
			_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): orderSubtotalInclTaxInCustomerCurrency → {OrderSubtotalInclTaxInCustomerCurrency}", order, orderSubtotalInclTaxInCustomerCurrency);
            model.OrderSubtotal = await _priceFormatter.FormatPriceAsync(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            model.OrderSubtotalValue = orderSubtotalInclTaxInCustomerCurrency;
			_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.OrderSubtotalValue → {Arg1}", order, model.OrderSubtotalValue);
            //discount (applied to order subtotal)
            var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
			_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): orderSubTotalDiscountInclTaxInCustomerCurrency → {OrderSubTotalDiscountInclTaxInCustomerCurrency}", order, orderSubTotalDiscountInclTaxInCustomerCurrency);
            if (orderSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
            {
                model.OrderSubTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                model.OrderSubTotalDiscountValue = orderSubTotalDiscountInclTaxInCustomerCurrency;
				_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.OrderSubTotalDiscountValue → {Arg1}", order, model.OrderSubTotalDiscountValue);
            }
        }
        else
        {
            //excluding tax

            //order subtotal
            var orderSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
			_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): orderSubtotalExclTaxInCustomerCurrency → {OrderSubtotalExclTaxInCustomerCurrency}", order, orderSubtotalExclTaxInCustomerCurrency);
            model.OrderSubtotal = await _priceFormatter.FormatPriceAsync(orderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            model.OrderSubtotalValue = orderSubtotalExclTaxInCustomerCurrency;
            //discount (applied to order subtotal)
            var orderSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
			_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): orderSubTotalDiscountExclTaxInCustomerCurrency → {OrderSubTotalDiscountExclTaxInCustomerCurrency}", order, orderSubTotalDiscountExclTaxInCustomerCurrency);
            if (orderSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
            {
                model.OrderSubTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                model.OrderSubTotalDiscountValue = orderSubTotalDiscountExclTaxInCustomerCurrency;
            }
        }

        if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
        {
            //including tax

            //order shipping
            var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
			_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): orderShippingInclTaxInCustomerCurrency → {OrderShippingInclTaxInCustomerCurrency}", order, orderShippingInclTaxInCustomerCurrency);
            model.OrderShipping = await _priceFormatter.FormatShippingPriceAsync(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            model.OrderShippingValue = orderShippingInclTaxInCustomerCurrency;
			_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.OrderShippingValue → {Arg1}", order, model.OrderShippingValue);
            //payment method additional fee
            var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
			_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): paymentMethodAdditionalFeeInclTaxInCustomerCurrency → {PaymentMethodAdditionalFeeInclTaxInCustomerCurrency}", order, paymentMethodAdditionalFeeInclTaxInCustomerCurrency);
            if (paymentMethodAdditionalFeeInclTaxInCustomerCurrency > decimal.Zero)
            {
                model.PaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                model.PaymentMethodAdditionalFeeValue = paymentMethodAdditionalFeeInclTaxInCustomerCurrency;
				_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.PaymentMethodAdditionalFeeValue → {Arg1}", order, model.PaymentMethodAdditionalFeeValue);
            }
        }
        else
        {
            //excluding tax

            //order shipping
            var orderShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
			_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): orderShippingExclTaxInCustomerCurrency → {OrderShippingExclTaxInCustomerCurrency}", order, orderShippingExclTaxInCustomerCurrency);
            model.OrderShipping = await _priceFormatter.FormatShippingPriceAsync(orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            model.OrderShippingValue = orderShippingExclTaxInCustomerCurrency;
            //payment method additional fee
            var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
			_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): paymentMethodAdditionalFeeExclTaxInCustomerCurrency → {PaymentMethodAdditionalFeeExclTaxInCustomerCurrency}", order, paymentMethodAdditionalFeeExclTaxInCustomerCurrency);
            if (paymentMethodAdditionalFeeExclTaxInCustomerCurrency > decimal.Zero)
            {
                model.PaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                model.PaymentMethodAdditionalFeeValue = paymentMethodAdditionalFeeExclTaxInCustomerCurrency;
            }
        }

        //tax
        var displayTax = true;
		_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): displayTax → {DisplayTax}", order, displayTax);
        var displayTaxRates = true;
		_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): displayTaxRates → {DisplayTaxRates}", order, displayTaxRates);
        if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
        {
            displayTax = false;
            displayTaxRates = false;
        }
        else
        {
            if (order.OrderTax == 0 && _taxSettings.HideZeroTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                var taxRates = _orderService.ParseTaxRates(order, order.TaxRates);
				_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): taxRates → {TaxRates}", order, taxRates);
                displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                displayTax = !displayTaxRates;

                var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
				_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): orderTaxInCustomerCurrency → {OrderTaxInCustomerCurrency}", order, orderTaxInCustomerCurrency);
                model.Tax = await _priceFormatter.FormatPriceAsync(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);

                foreach (var tr in taxRates)
                {
                    model.TaxRates.Add(new OrderDetailsModel.TaxRate
                    {
                        Rate = _priceFormatter.FormatTaxRate(tr.Key),
                        Value = await _priceFormatter.FormatPriceAsync(_currencyService.ConvertCurrency(tr.Value, order.CurrencyRate), true, order.CustomerCurrencyCode, false, languageId),
                    });
                }
            }
        }
        model.DisplayTaxRates = displayTaxRates;
		_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.DisplayTaxRates → {Arg1}", order, model.DisplayTaxRates);
		_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.DisplayTax → {Arg1}", order, model.DisplayTax);
		_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.DisplayTaxShippingInfo → {Arg1}", order, model.DisplayTaxShippingInfo);
        model.DisplayTax = displayTax;
        model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoOrderDetailsPage;
        model.PricesIncludeTax = order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax;

        //discount (applied to order total)
        var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
		_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): orderDiscountInCustomerCurrency → {OrderDiscountInCustomerCurrency}", order, orderDiscountInCustomerCurrency);
        if (orderDiscountInCustomerCurrency > decimal.Zero)
        {
            model.OrderTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);
            model.OrderTotalDiscountValue = orderDiscountInCustomerCurrency;
			_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.OrderTotalDiscountValue → {Arg1}", order, model.OrderTotalDiscountValue);
        }

        //gift cards
        foreach (var gcuh in await _giftCardService.GetGiftCardUsageHistoryAsync(order))
        {
            model.GiftCards.Add(new OrderDetailsModel.GiftCard
            {
                CouponCode = (await _giftCardService.GetGiftCardByIdAsync(gcuh.GiftCardId)).GiftCardCouponCode,
                Amount = await _priceFormatter.FormatPriceAsync(-(_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, languageId),
            });
        }

        //reward points           
        if (order.RedeemedRewardPointsEntryId.HasValue && await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
        {
            model.RedeemedRewardPoints = -redeemedRewardPointsEntry.Points;
			_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.RedeemedRewardPoints → {Arg1}", order, model.RedeemedRewardPoints);
            model.RedeemedRewardPointsAmount = await _priceFormatter.FormatPriceAsync(-(_currencyService.ConvertCurrency(redeemedRewardPointsEntry.UsedAmount, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, languageId);
        }

        //total
        var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
		_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): orderTotalInCustomerCurrency → {OrderTotalInCustomerCurrency}", order, orderTotalInCustomerCurrency);
        model.OrderTotal = await _priceFormatter.FormatPriceAsync(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);
        model.OrderTotalValue = orderTotalInCustomerCurrency;
		_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.OrderTotalValue → {Arg1}", order, model.OrderTotalValue);

        //checkout attributes
        model.CheckoutAttributeInfo = order.CheckoutAttributeDescription;
		_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.CheckoutAttributeInfo → {Arg1}", order, model.CheckoutAttributeInfo);

        //order notes
        foreach (var orderNote in (await _orderService.GetOrderNotesByOrderIdAsync(order.Id, true))
                 .OrderByDescending(on => on.CreatedOnUtc)
                 .ToList())
        {
            model.OrderNotes.Add(new OrderDetailsModel.OrderNote
            {
                Id = orderNote.Id,
                HasDownload = orderNote.DownloadId > 0,
                Note = _orderService.FormatOrderNoteText(orderNote),
                CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(orderNote.CreatedOnUtc, DateTimeKind.Utc)
            });
				_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): CreatedOn → {CreatedOn}", order, CreatedOn);
        }

        //can user cancel the order
        model.CanCancelOrder = order.PaymentStatus == PaymentStatus.Pending
            && order.OrderStatus != OrderStatus.Cancelled
            && _orderSettings.AllowCustomersCancelOrders;

        //purchased products
        model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
		_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.ShowSku → {Arg1}", order, model.ShowSku);
		_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.ShowVendorName → {Arg1}", order, model.ShowVendorName);
		_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): model.ShowProductThumbnail → {Arg1}", order, model.ShowProductThumbnail);
        model.ShowVendorName = _vendorSettings.ShowVendorOnOrderDetailsPage;
        model.ShowProductThumbnail = _orderSettings.ShowProductThumbnailInOrderDetailsPage;

        var orderItems = await _orderService.GetOrderItemsAsync(order.Id);
		_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): orderItems → {OrderItems}", order, orderItems);

        foreach (var orderItem in orderItems)
        {
            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
			_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): product → {Product}", order, product);

            var orderItemModel = new OrderDetailsModel.OrderItemModel
            {
                Id = orderItem.Id,
                OrderItemGuid = orderItem.OrderItemGuid,
                Sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml),
                VendorName = (await _vendorService.GetVendorByIdAsync(product.VendorId))?.Name ?? string.Empty,
                ProductId = (product.ParentGroupedProductId > 0 && !product.VisibleIndividually) ? product.ParentGroupedProductId : product.Id,
                ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                Quantity = orderItem.Quantity,
                AttributeInfo = orderItem.AttributeDescription,
            };
            //rental info
            if (product.IsRental)
            {
                var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                    ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : "";
                var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                    ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : "";
                orderItemModel.RentalInfo = string.Format(await _localizationService.GetResourceAsync("Order.Rental.FormattedDate"),
                    rentalStartDate, rentalEndDate);
            }
            model.Items.Add(orderItemModel);

            //unit price, subtotal
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax
                var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
				_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): unitPriceInclTaxInCustomerCurrency → {UnitPriceInclTaxInCustomerCurrency}", order, unitPriceInclTaxInCustomerCurrency);
                orderItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                orderItemModel.UnitPriceValue = unitPriceInclTaxInCustomerCurrency;
				_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): orderItemModel.UnitPriceValue → {Arg1}", order, orderItemModel.UnitPriceValue);

                var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
				_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): priceInclTaxInCustomerCurrency → {PriceInclTaxInCustomerCurrency}", order, priceInclTaxInCustomerCurrency);
                orderItemModel.SubTotal = await _priceFormatter.FormatPriceAsync(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                orderItemModel.SubTotalValue = priceInclTaxInCustomerCurrency;
				_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): orderItemModel.SubTotalValue → {Arg1}", order, orderItemModel.SubTotalValue);
            }
            else
            {
                //excluding tax
                var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
				_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): unitPriceExclTaxInCustomerCurrency → {UnitPriceExclTaxInCustomerCurrency}", order, unitPriceExclTaxInCustomerCurrency);
                orderItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                orderItemModel.UnitPriceValue = unitPriceExclTaxInCustomerCurrency;

                var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
				_logger.LogError("PrepareOrderDetailsModelAsync(order={Order}): priceExclTaxInCustomerCurrency → {PriceExclTaxInCustomerCurrency}", order, priceExclTaxInCustomerCurrency);
                orderItemModel.SubTotal = await _priceFormatter.FormatPriceAsync(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                orderItemModel.SubTotalValue = priceExclTaxInCustomerCurrency;
            }

            //downloadable products
            if (await _orderService.IsDownloadAllowedAsync(orderItem))
                orderItemModel.DownloadId = product.DownloadId;
				_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): orderItemModel.DownloadId → {Arg1}", order, orderItemModel.DownloadId);
            if (await _orderService.IsLicenseDownloadAllowedAsync(orderItem))
                orderItemModel.LicenseId = orderItem.LicenseDownloadId ?? 0;
				_logger.LogDebug("PrepareOrderDetailsModelAsync(order={Order}): orderItemModel.LicenseId → {Arg1}", order, orderItemModel.LicenseId);

            if (_orderSettings.ShowProductThumbnailInOrderDetailsPage)
            {
                orderItemModel.Picture = await PrepareOrderItemPictureModelAsync(orderItem, _mediaSettings.OrderThumbPictureSize, true, orderItemModel.ProductName);
            }
        }

		_logger.LogWarning("<<< Exiting PrepareOrderDetailsModelAsync(order={Order})", order);
        return model;
    }

    /// <summary>
    /// Prepare the shipment details model
    /// </summary>
    /// <param name="shipment">Shipment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment details model
    /// </returns>
    public virtual async Task<ShipmentDetailsModel> PrepareShipmentDetailsModelAsync(Shipment shipment)
    {
		_logger.LogError(">>> Entering PrepareShipmentDetailsModelAsync(shipment={Shipment})", shipment);
        ArgumentNullException.ThrowIfNull(shipment);

        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId) ?? throw new Exception("order cannot be loaded");
		_logger.LogError("PrepareShipmentDetailsModelAsync(shipment={Shipment}): order → {Order}", shipment, order);
        var model = new ShipmentDetailsModel
        {
            Id = shipment.Id
        };
        if (shipment.ShippedDateUtc.HasValue)
            model.ShippedDate = await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ShippedDateUtc.Value, DateTimeKind.Utc);
        if (shipment.ReadyForPickupDateUtc.HasValue)
            model.ReadyForPickupDate = await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ReadyForPickupDateUtc.Value, DateTimeKind.Utc);
        if (shipment.DeliveryDateUtc.HasValue)
            model.DeliveryDate = await _dateTimeHelper.ConvertToUserTimeAsync(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc);

        //tracking number and shipment information
        if (!string.IsNullOrEmpty(shipment.TrackingNumber))
        {
            model.TrackingNumber = shipment.TrackingNumber;
			_logger.LogDebug("PrepareShipmentDetailsModelAsync(shipment={Shipment}): model.TrackingNumber → {Arg1}", shipment, model.TrackingNumber);
            var shipmentTracker = await _shipmentService.GetShipmentTrackerAsync(shipment);
			_logger.LogError("PrepareShipmentDetailsModelAsync(shipment={Shipment}): shipmentTracker → {ShipmentTracker}", shipment, shipmentTracker);
            if (shipmentTracker != null)
            {
                model.TrackingNumberUrl = await shipmentTracker.GetUrlAsync(shipment.TrackingNumber, shipment);
                if (_shippingSettings.DisplayShipmentEventsToCustomers)
                {
                    var shipmentEvents = await shipmentTracker.GetShipmentEventsAsync(shipment.TrackingNumber, shipment);
					_logger.LogError("PrepareShipmentDetailsModelAsync(shipment={Shipment}): shipmentEvents → {ShipmentEvents}", shipment, shipmentEvents);
                    if (shipmentEvents != null)
                    {
                        foreach (var shipmentEvent in shipmentEvents)
                        {
                            var shipmentStatusEventModel = new ShipmentDetailsModel.ShipmentStatusEventModel();
							_logger.LogError("PrepareShipmentDetailsModelAsync(shipment={Shipment}): shipmentStatusEventModel → {ShipmentStatusEventModel}", shipment, shipmentStatusEventModel);
                            var shipmentEventCountry = await _countryService.GetCountryByTwoLetterIsoCodeAsync(shipmentEvent.CountryCode);
							_logger.LogError("PrepareShipmentDetailsModelAsync(shipment={Shipment}): shipmentEventCountry → {ShipmentEventCountry}", shipment, shipmentEventCountry);
                            shipmentStatusEventModel.Country = shipmentEventCountry != null
                                ? await _localizationService.GetLocalizedAsync(shipmentEventCountry, x => x.Name) : shipmentEvent.CountryCode;
                            shipmentStatusEventModel.Status = shipmentEvent.Status;
							_logger.LogDebug("PrepareShipmentDetailsModelAsync(shipment={Shipment}): shipmentStatusEventModel.Status → {Arg1}", shipment, shipmentStatusEventModel.Status);
							_logger.LogDebug("PrepareShipmentDetailsModelAsync(shipment={Shipment}): shipmentStatusEventModel.Date → {Arg1}", shipment, shipmentStatusEventModel.Date);
							_logger.LogDebug("PrepareShipmentDetailsModelAsync(shipment={Shipment}): shipmentStatusEventModel.EventName → {Arg1}", shipment, shipmentStatusEventModel.EventName);
							_logger.LogDebug("PrepareShipmentDetailsModelAsync(shipment={Shipment}): shipmentStatusEventModel.Location → {Arg1}", shipment, shipmentStatusEventModel.Location);
                            shipmentStatusEventModel.Date = shipmentEvent.Date;
                            shipmentStatusEventModel.EventName = shipmentEvent.EventName;
                            shipmentStatusEventModel.Location = shipmentEvent.Location;
                            model.ShipmentStatusEvents.Add(shipmentStatusEventModel);
                        }
                    }
                }
            }
        }

        //products in this shipment
        model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
		_logger.LogDebug("PrepareShipmentDetailsModelAsync(shipment={Shipment}): model.ShowSku → {Arg1}", shipment, model.ShowSku);
        foreach (var shipmentItem in await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id))
        {
            var orderItem = await _orderService.GetOrderItemByIdAsync(shipmentItem.OrderItemId);
			_logger.LogError("PrepareShipmentDetailsModelAsync(shipment={Shipment}): orderItem → {OrderItem}", shipment, orderItem);
            if (orderItem == null)
                continue;

            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
			_logger.LogError("PrepareShipmentDetailsModelAsync(shipment={Shipment}): product → {Product}", shipment, product);

            var shipmentItemModel = new ShipmentDetailsModel.ShipmentItemModel
            {
                Id = shipmentItem.Id,
                Sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml),
                ProductId = product.Id,
                ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                AttributeInfo = orderItem.AttributeDescription,
                QuantityOrdered = orderItem.Quantity,
                QuantityShipped = shipmentItem.Quantity,
            };
            //rental info
            if (product.IsRental)
            {
                var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                    ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : "";
                var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                    ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : "";
                shipmentItemModel.RentalInfo = string.Format(await _localizationService.GetResourceAsync("Order.Rental.FormattedDate"),
                    rentalStartDate, rentalEndDate);
            }
            model.Items.Add(shipmentItemModel);
        }

        //order details model
        model.Order = await PrepareOrderDetailsModelAsync(order);

		_logger.LogWarning("<<< Exiting PrepareShipmentDetailsModelAsync(shipment={Shipment})", shipment);
        return model;
    }

    /// <summary>
    /// Prepare the customer reward points model
    /// </summary>
    /// <param name="page">Number of items page; pass null to load the first page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer reward points model
    /// </returns>
    public virtual async Task<CustomerRewardPointsModel> PrepareCustomerRewardPointsAsync(int? page)
    {
		_logger.LogError(">>> Entering PrepareCustomerRewardPointsAsync(page={Page})", page);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //get reward points history
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareCustomerRewardPointsAsync(page={Page}): customer → {Customer}", page, customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareCustomerRewardPointsAsync(page={Page}): store → {Store}", page, store);
        var pageSize = _rewardPointsSettings.PageSize;
        var rewardPoints = await _rewardPointService.GetRewardPointsHistoryAsync(customer.Id, store.Id, true, pageIndex: --page ?? 0, pageSize: pageSize);
		_logger.LogError("PrepareCustomerRewardPointsAsync(page={Page}): rewardPoints → {RewardPoints}", page, rewardPoints);

        //prepare model
        var model = new CustomerRewardPointsModel
        {
            RewardPoints = await rewardPoints.SelectAwait(async historyEntry =>
            {
                var activatingDate = await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.CreatedOnUtc, DateTimeKind.Utc);
                return new CustomerRewardPointsModel.RewardPointsHistoryModel
                {
                    Points = historyEntry.Points,
                    PointsBalance = historyEntry.PointsBalance.HasValue ? historyEntry.PointsBalance.ToString()
                        : string.Format(await _localizationService.GetResourceAsync("RewardPoints.ActivatedLater"), activatingDate),
                    Message = historyEntry.Message,
                    CreatedOn = activatingDate,
                    EndDate = !historyEntry.EndDateUtc.HasValue ? null :
                        (DateTime?)(await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.EndDateUtc.Value, DateTimeKind.Utc))
                };
            }).ToListAsync(),

            PagerModel = new PagerModel(_localizationService)
            {
				_logger.LogDebug("PrepareCustomerRewardPointsAsync(page={Page}): PagerModel → {PagerModel}", page, PagerModel);
                PageSize = rewardPoints.PageSize,
                TotalRecords = rewardPoints.TotalCount,
                PageIndex = rewardPoints.PageIndex,
                ShowTotalSummary = true,
                RouteActionName = NopRouteNames.Standard.CUSTOMER_REWARD_POINTS_PAGED,
                UseRouteLinks = true,
                RouteValues = new RewardPointsRouteValues { PageNumber = page ?? 0 }
            }
			_logger.LogError("PrepareCustomerRewardPointsAsync(page)={Page}: database query executed in {Arg1} ms", page, __logStart.ElapsedMilliseconds);
        };

        //current amount/balance
        var rewardPointsBalance = await _rewardPointService.GetRewardPointsBalanceAsync(customer.Id, store.Id);
		_logger.LogError("PrepareCustomerRewardPointsAsync(page={Page}): rewardPointsBalance → {RewardPointsBalance}", page, rewardPointsBalance);
		_logger.LogDebug("PrepareCustomerRewardPointsAsync(page={Page}): model.RewardPointsBalance → {Arg1}", page, model.RewardPointsBalance);
        var rewardPointsAmountBase = await _orderTotalCalculationService.ConvertRewardPointsToAmountAsync(rewardPointsBalance);
		_logger.LogError("PrepareCustomerRewardPointsAsync(page={Page}): rewardPointsAmountBase → {RewardPointsAmountBase}", page, rewardPointsAmountBase);
        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
		_logger.LogError("PrepareCustomerRewardPointsAsync(page={Page}): currentCurrency → {CurrentCurrency}", page, currentCurrency);
        var rewardPointsAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(rewardPointsAmountBase, currentCurrency);
		_logger.LogError("PrepareCustomerRewardPointsAsync(page={Page}): rewardPointsAmount → {RewardPointsAmount}", page, rewardPointsAmount);
        model.RewardPointsBalance = rewardPointsBalance;
        model.RewardPointsAmount = await _priceFormatter.FormatPriceAsync(rewardPointsAmount, true, false);

        //minimum amount/balance
        var minimumRewardPointsBalance = _rewardPointsSettings.MinimumRewardPointsToUse;
        var minimumRewardPointsAmountBase = await _orderTotalCalculationService.ConvertRewardPointsToAmountAsync(minimumRewardPointsBalance);
		_logger.LogError("PrepareCustomerRewardPointsAsync(page={Page}): minimumRewardPointsAmountBase → {MinimumRewardPointsAmountBase}", page, minimumRewardPointsAmountBase);
		_logger.LogDebug("PrepareCustomerRewardPointsAsync(page={Page}): model.MinimumRewardPointsBalance → {Arg1}", page, model.MinimumRewardPointsBalance);
        var minimumRewardPointsAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(minimumRewardPointsAmountBase, currentCurrency);
		_logger.LogError("PrepareCustomerRewardPointsAsync(page={Page}): minimumRewardPointsAmount → {MinimumRewardPointsAmount}", page, minimumRewardPointsAmount);
        model.MinimumRewardPointsBalance = minimumRewardPointsBalance;
        model.MinimumRewardPointsAmount = await _priceFormatter.FormatPriceAsync(minimumRewardPointsAmount, true, false);

		_logger.LogWarning("<<< Exiting PrepareCustomerRewardPointsAsync(page={Page})", page);
        return model;
    }

    #endregion

    #region nested class

    /// <summary>
    /// record that has only page for route value. Used for (My Account) Reward Points pagination
    /// </summary>
    public partial record RewardPointsRouteValues : BaseRouteValues;

    /// <summary>
    /// Record that has filter options for route values. Used for Customer orders pagination (My Account)
    /// </summary>
    public partial record CustomerOrdersRouteValues : BaseRouteValues
    {
        public string Limit { get; set; }
    }

    #endregion
}