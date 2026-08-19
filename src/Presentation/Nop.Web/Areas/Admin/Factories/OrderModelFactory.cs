using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Models.Reports;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the order model factory implementation
/// </summary>
public partial class OrderModelFactory : IOrderModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly CurrencySettings _currencySettings;
    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly IAffiliateService _affiliateService;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICountryService _countryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IDiscountService _discountService;
    protected readonly IDownloadService _downloadService;
    protected readonly IEncryptionService _encryptionService;
    protected readonly IGiftCardService _giftCardService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IMeasureService _measureService;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderReportService _orderReportService;
    protected readonly IOrderService _orderService;
    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IPictureService _pictureService;
    protected readonly IPriceCalculationService _priceCalculationService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;
    protected readonly IReturnRequestService _returnRequestService;
    protected readonly IRewardPointService _rewardPointService;
    protected readonly ISettingService _settingService;
    protected readonly IShipmentService _shipmentService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreService _storeService;
    protected readonly ITaxService _taxService;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IVendorService _vendorService;
    protected readonly IWarehouseService _warehouseService;
    protected readonly IWorkContext _workContext;
    protected readonly MeasureSettings _measureSettings;
    protected readonly NopHttpClient _nopHttpClient;
    protected readonly OrderSettings _orderSettings;
    protected readonly ShippingSettings _shippingSettings;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly TaxSettings _taxSettings;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public OrderModelFactory(AddressSettings addressSettings,
        CatalogSettings catalogSettings,
        CurrencySettings currencySettings,
        IActionContextAccessor actionContextAccessor,
        IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        IAffiliateService affiliateService,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICountryService countryService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IDiscountService discountService,
        IDownloadService downloadService,
        IEncryptionService encryptionService,
        IGiftCardService giftCardService,
        ILocalizationService localizationService,
        IMeasureService measureService,
        IOrderProcessingService orderProcessingService,
        IOrderReportService orderReportService,
        IOrderService orderService,
        IPaymentPluginManager paymentPluginManager,
        IPictureService pictureService,
        IPriceCalculationService priceCalculationService,
        IPriceFormatter priceFormatter,
        IProductAttributeService productAttributeService,
        IProductService productService,
        IReturnRequestService returnRequestService,
        IRewardPointService rewardPointService,
        ISettingService settingService,
        IShipmentService shipmentService,
        IStateProvinceService stateProvinceService,
        IStoreService storeService,
        ITaxService taxService,
        IUrlHelperFactory urlHelperFactory,
        IVendorService vendorService,
        IWarehouseService warehouseService,
        IWorkContext workContext,
        MeasureSettings measureSettings,
        NopHttpClient nopHttpClient,
        OrderSettings orderSettings,
        ShippingSettings shippingSettings,
        IUrlRecordService urlRecordService,
        TaxSettings taxSettings)
    {
        _addressSettings = addressSettings;
        _catalogSettings = catalogSettings;
        _currencySettings = currencySettings;
        _actionContextAccessor = actionContextAccessor;
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
        _affiliateService = affiliateService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _countryService = countryService;
        _currencyService = currencyService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _discountService = discountService;
        _downloadService = downloadService;
        _encryptionService = encryptionService;
        _giftCardService = giftCardService;
        _localizationService = localizationService;
        _measureService = measureService;
        _orderProcessingService = orderProcessingService;
        _orderReportService = orderReportService;
        _orderService = orderService;
        _paymentPluginManager = paymentPluginManager;
        _pictureService = pictureService;
        _priceCalculationService = priceCalculationService;
        _priceFormatter = priceFormatter;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _returnRequestService = returnRequestService;
        _rewardPointService = rewardPointService;
        _settingService = settingService;
        _shipmentService = shipmentService;
        _stateProvinceService = stateProvinceService;
        _storeService = storeService;
        _taxService = taxService;
        _urlHelperFactory = urlHelperFactory;
        _vendorService = vendorService;
        _warehouseService = warehouseService;
        _workContext = workContext;
        _measureSettings = measureSettings;
        _nopHttpClient = nopHttpClient;
        _orderSettings = orderSettings;
        _shippingSettings = shippingSettings;
        _urlRecordService = urlRecordService;
        _taxSettings = taxSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepares the shipment model
    /// </summary>
    /// <param name="shipment">Shipment</param>
    /// <param name="model">Predefined shipment model if any</param>
    /// <returns>The <see cref="Task"/> containing the <see cref="ShipmentModel"/></returns>
    protected virtual async Task<ShipmentModel> PrepareShipmentModelAsync(Shipment shipment, ShipmentModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareShipmentModelAsync(shipment={Shipment},model={Model})", shipment, model);
        //fill in model values from the entity
        var shipmentModel = model ?? shipment.ToModel<ShipmentModel>();
		_logger.LogDebug("PrepareShipmentModelAsync(shipment={Shipment},model={Model}): shipmentModel → {ShipmentModel}", shipment, model, shipmentModel);

        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);
		_logger.LogDebug("PrepareShipmentModelAsync(shipment={Shipment},model={Model}): order → {Order}", shipment, model, order);

        shipmentModel.PickupInStore = order.PickupInStore;
		_logger.LogDebug("PrepareShipmentModelAsync(shipment={Shipment},model={Model}): shipmentModel.PickupInStore → {Arg2}", shipment, model, shipmentModel.PickupInStore);
		_logger.LogDebug("PrepareShipmentModelAsync(shipment={Shipment},model={Model}): shipmentModel.CustomOrderNumber → {Arg2}", shipment, model, shipmentModel.CustomOrderNumber);
        shipmentModel.CustomOrderNumber = order.CustomOrderNumber;

        //convert dates to the user time
        if (order.PickupInStore)
        {
            shipmentModel.ShippedDate = await _localizationService.GetResourceAsync("Admin.Orders.Shipments.DateNotAvailable");
            shipmentModel.ReadyForPickupDate = shipment.ReadyForPickupDateUtc.HasValue
                ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ReadyForPickupDateUtc.Value, DateTimeKind.Utc)).ToString()
                : await _localizationService.GetResourceAsync("Admin.Orders.Shipments.ReadyForPickupDate.NotYet");
				_logger.LogDebug("PrepareShipmentModelAsync(shipment={Shipment},model={Model}): shipmentModel.ReadyForPickupDate → {Arg2}", shipment, model, shipmentModel.ReadyForPickupDate);
        }
        else
        {
            shipmentModel.ReadyForPickupDate = await _localizationService.GetResourceAsync("Admin.Orders.Shipments.DateNotAvailable");
            shipmentModel.ShippedDate = shipment.ShippedDateUtc.HasValue
                ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ShippedDateUtc.Value, DateTimeKind.Utc)).ToString()
                : await _localizationService.GetResourceAsync("Admin.Orders.Shipments.ShippedDate.NotYet");
				_logger.LogDebug("PrepareShipmentModelAsync(shipment={Shipment},model={Model}): shipmentModel.ShippedDate → {Arg2}", shipment, model, shipmentModel.ShippedDate);
        }

        shipmentModel.DeliveryDate = shipment.DeliveryDateUtc.HasValue
            ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc)).ToString()
            : await _localizationService.GetResourceAsync("Admin.Orders.Shipments.DeliveryDate.NotYet");
			_logger.LogDebug("PrepareShipmentModelAsync(shipment={Shipment},model={Model}): shipmentModel.DeliveryDate → {Arg2}", shipment, model, shipmentModel.DeliveryDate);

        //fill in additional values (not existing in the entity)
        shipmentModel.CanShip = !order.PickupInStore && !shipment.ShippedDateUtc.HasValue;
		_logger.LogDebug("PrepareShipmentModelAsync(shipment={Shipment},model={Model}): shipmentModel.CanShip → {Arg2}", shipment, model, shipmentModel.CanShip);
		_logger.LogDebug("PrepareShipmentModelAsync(shipment={Shipment},model={Model}): shipmentModel.CanMarkAsReadyForPickup → {Arg2}", shipment, model, shipmentModel.CanMarkAsReadyForPickup);
        shipmentModel.CanMarkAsReadyForPickup = order.PickupInStore && !shipment.ReadyForPickupDateUtc.HasValue;
        shipmentModel.CanDeliver = (shipment.ShippedDateUtc.HasValue || shipment.ReadyForPickupDateUtc.HasValue) && !shipment.DeliveryDateUtc.HasValue;

        if (shipment.TotalWeight.HasValue)
            shipmentModel.TotalWeight = $"{shipment.TotalWeight:F2} [{(await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId))?.Name}]";

		_logger.LogDebug("<<< Exiting PrepareShipmentModelAsync(shipment={Shipment},model={Model})", shipment, model);
        return shipmentModel;
    }

    /// <summary>
    /// Set some address fields as required
    /// </summary>
    /// <param name="model">Address model</param>
    protected virtual void SetAddressFieldsAsRequired(AddressModel model)
    {
		_logger.LogDebug(">>> Entering SetAddressFieldsAsRequired(model={Model})", model);
        model.FirstNameRequired = true;
        model.LastNameRequired = true;
		_logger.LogDebug("SetAddressFieldsAsRequired(model={Model}): model.LastNameRequired → {Arg1}", model, model.LastNameRequired);
		_logger.LogDebug("SetAddressFieldsAsRequired(model={Model}): model.EmailRequired → {Arg1}", model, model.EmailRequired);
		_logger.LogDebug("SetAddressFieldsAsRequired(model={Model}): model.CompanyRequired → {Arg1}", model, model.CompanyRequired);
		_logger.LogDebug("SetAddressFieldsAsRequired(model={Model}): model.CountyRequired → {Arg1}", model, model.CountyRequired);
		_logger.LogDebug("SetAddressFieldsAsRequired(model={Model}): model.CityRequired → {Arg1}", model, model.CityRequired);
		_logger.LogDebug("SetAddressFieldsAsRequired(model={Model}): model.StreetAddressRequired → {Arg1}", model, model.StreetAddressRequired);
		_logger.LogDebug("SetAddressFieldsAsRequired(model={Model}): model.StreetAddress2Required → {Arg1}", model, model.StreetAddress2Required);
		_logger.LogDebug("SetAddressFieldsAsRequired(model={Model}): model.ZipPostalCodeRequired → {Arg1}", model, model.ZipPostalCodeRequired);
		_logger.LogDebug("SetAddressFieldsAsRequired(model={Model}): model.PhoneRequired → {Arg1}", model, model.PhoneRequired);
		_logger.LogDebug("SetAddressFieldsAsRequired(model={Model}): model.FaxRequired → {Arg1}", model, model.FaxRequired);
		_logger.LogDebug("SetAddressFieldsAsRequired(model={Model}): model.FirstNameRequired → {Arg1}", model, model.FirstNameRequired);
        model.EmailRequired = true;
        model.CompanyRequired = _addressSettings.CompanyRequired;
        model.CountyRequired = _addressSettings.CountyRequired;
        model.CityRequired = _addressSettings.CityRequired;
        model.StreetAddressRequired = _addressSettings.StreetAddressRequired;
        model.StreetAddress2Required = _addressSettings.StreetAddress2Required;
        model.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
        model.PhoneRequired = _addressSettings.PhoneRequired;
        model.FaxRequired = _addressSettings.FaxRequired;
		_logger.LogDebug("<<< Exiting SetAddressFieldsAsRequired(model={Model})", model);
    }

    /// <summary>
    /// Prepare order item models
    /// </summary>
    /// <param name="models">List of order item models</param>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareOrderItemModelsAsync(IList<OrderItemModel> models, Order order)
    {
		_logger.LogDebug(">>> Entering PrepareOrderItemModelsAsync(models={Models},order={Order})", models, order);
        ArgumentNullException.ThrowIfNull(models);

        ArgumentNullException.ThrowIfNull(order);

        var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
		_logger.LogDebug("PrepareOrderItemModelsAsync(models={Models},order={Order}): primaryStoreCurrency → {PrimaryStoreCurrency}", models, order, primaryStoreCurrency);

        //get order items
        var vendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("PrepareOrderItemModelsAsync(models={Models},order={Order}): vendor → {Vendor}", models, order, vendor);
        var orderItems = await _orderService.GetOrderItemsAsync(order.Id, vendorId: vendor?.Id ?? 0);
		_logger.LogDebug("PrepareOrderItemModelsAsync(models={Models},order={Order}): orderItems → {OrderItems}", models, order, orderItems);

        foreach (var orderItem in orderItems)
        {
            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
			_logger.LogDebug("PrepareOrderItemModelsAsync(models={Models},order={Order}): product → {Product}", models, order, product);

            //fill in model values from the entity
            var orderItemModel = new OrderItemModel
            {
                Id = orderItem.Id,
                ProductId = orderItem.ProductId,
                ProductName = product.Name,
                Quantity = orderItem.Quantity,
                IsDownload = product.IsDownload,
                DownloadCount = orderItem.DownloadCount,
                DownloadActivationType = product.DownloadActivationType,
                IsDownloadActivated = orderItem.IsDownloadActivated,
                UnitPriceInclTaxValue = orderItem.UnitPriceInclTax,
                UnitPriceExclTaxValue = orderItem.UnitPriceExclTax,
                DiscountInclTaxValue = orderItem.DiscountAmountInclTax,
                DiscountExclTaxValue = orderItem.DiscountAmountExclTax,
                SubTotalInclTaxValue = orderItem.PriceInclTax,
                SubTotalExclTaxValue = orderItem.PriceExclTax,
                AttributeInfo = orderItem.AttributeDescription
            };

            //fill in additional values (not existing in the entity)
            orderItemModel.Sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml);
            orderItemModel.VendorName = (await _vendorService.GetVendorByIdAsync(product.VendorId))?.Name;

            //picture
            var orderItemPicture = await _pictureService.GetProductPictureAsync(product, orderItem.AttributesXml);
			_logger.LogDebug("PrepareOrderItemModelsAsync(models={Models},order={Order}): orderItemPicture → {OrderItemPicture}", models, order, orderItemPicture);
            (orderItemModel.PictureThumbnailUrl, _) = await _pictureService.GetPictureUrlAsync(orderItemPicture, 75);

            //license file
            if (orderItem.LicenseDownloadId.HasValue)
            {
                orderItemModel.LicenseDownloadGuid = (await _downloadService
                    .GetDownloadByIdAsync(orderItem.LicenseDownloadId.Value))?.DownloadGuid ?? Guid.Empty;
            }

            var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
			_logger.LogDebug("PrepareOrderItemModelsAsync(models={Models},order={Order}): languageId → {LanguageId}", models, order, languageId);

            //unit price
            orderItemModel.UnitPriceInclTax = await _priceFormatter
                .FormatOrderPriceAsync(orderItem.UnitPriceInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true, true);
					_logger.LogDebug("PrepareOrderItemModelsAsync(models={Models},order={Order}): orderItemModel.UnitPriceInclTax → {Arg2}", models, order, orderItemModel.UnitPriceInclTax);
            orderItemModel.UnitPriceExclTax = await _priceFormatter
                .FormatOrderPriceAsync(orderItem.UnitPriceExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false, true);
					_logger.LogDebug("PrepareOrderItemModelsAsync(models={Models},order={Order}): orderItemModel.UnitPriceExclTax → {Arg2}", models, order, orderItemModel.UnitPriceExclTax);

            //discounts
            orderItemModel.DiscountInclTax = await _priceFormatter
                .FormatOrderPriceAsync(orderItem.DiscountAmountInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true, true);
					_logger.LogDebug("PrepareOrderItemModelsAsync(models={Models},order={Order}): orderItemModel.DiscountInclTax → {Arg2}", models, order, orderItemModel.DiscountInclTax);
            orderItemModel.DiscountExclTax = await _priceFormatter
                .FormatOrderPriceAsync(orderItem.DiscountAmountExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false, true);
					_logger.LogDebug("PrepareOrderItemModelsAsync(models={Models},order={Order}): orderItemModel.DiscountExclTax → {Arg2}", models, order, orderItemModel.DiscountExclTax);

            //subtotal
            orderItemModel.SubTotalInclTax = await _priceFormatter
                .FormatOrderPriceAsync(orderItem.PriceInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true, true);
					_logger.LogDebug("PrepareOrderItemModelsAsync(models={Models},order={Order}): orderItemModel.SubTotalInclTax → {Arg2}", models, order, orderItemModel.SubTotalInclTax);
            orderItemModel.SubTotalExclTax = await _priceFormatter
                .FormatOrderPriceAsync(orderItem.PriceExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false, true);
					_logger.LogDebug("PrepareOrderItemModelsAsync(models={Models},order={Order}): orderItemModel.SubTotalExclTax → {Arg2}", models, order, orderItemModel.SubTotalExclTax);

            //recurring info
            if (product.IsRecurring)
            {
                orderItemModel.RecurringInfo = string.Format(await _localizationService.GetResourceAsync("Admin.Orders.Products.RecurringPeriod"),
                    product.RecurringCycleLength, await _localizationService.GetLocalizedEnumAsync(product.RecurringCyclePeriod));
            }

            //rental info
            if (product.IsRental)
            {
                var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                    ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : string.Empty;
                var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                    ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : string.Empty;
                orderItemModel.RentalInfo = string.Format(await _localizationService.GetResourceAsync("Order.Rental.FormattedDate"),
                    rentalStartDate, rentalEndDate);
            }

            //prepare return request models
            await PrepareReturnRequestBriefModelsAsync(orderItemModel.ReturnRequests, orderItem);

            //gift card identifiers
            orderItemModel.PurchasedGiftCardIds = (await _giftCardService
                .GetGiftCardsByPurchasedWithOrderItemIdAsync(orderItem.Id)).Select(card => card.Id).ToList();

            models.Add(orderItemModel);
        }
		_logger.LogDebug("<<< Exiting PrepareOrderItemModelsAsync(models={Models},order={Order})", models, order);
    }

    /// <summary>
    /// Prepare return request brief models
    /// </summary>
    /// <param name="models">List of return request brief models</param>
    /// <param name="orderItem">Order item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareReturnRequestBriefModelsAsync(IList<OrderItemModel.ReturnRequestBriefModel> models, OrderItem orderItem)
    {
		_logger.LogDebug(">>> Entering PrepareReturnRequestBriefModelsAsync(models={Models},orderItem={OrderItem})", models, orderItem);
        ArgumentNullException.ThrowIfNull(models);

        ArgumentNullException.ThrowIfNull(orderItem);

        var returnRequests = await _returnRequestService.SearchReturnRequestsAsync(orderItemId: orderItem.Id);
		_logger.LogDebug("PrepareReturnRequestBriefModelsAsync(models={Models},orderItem={OrderItem}): returnRequests → {ReturnRequests}", models, orderItem, returnRequests);
        foreach (var returnRequest in returnRequests)
        {
            models.Add(new OrderItemModel.ReturnRequestBriefModel
            {
                CustomNumber = returnRequest.CustomNumber,
                Id = returnRequest.Id
            });
        }
		_logger.LogDebug("<<< Exiting PrepareReturnRequestBriefModelsAsync(models={Models},orderItem={OrderItem})", models, orderItem);
    }

    /// <summary>
    /// Prepare order model totals
    /// </summary>
    /// <param name="model">Order model</param>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareOrderModelTotalsAsync(OrderModel model, Order order)
    {
		_logger.LogDebug(">>> Entering PrepareOrderModelTotalsAsync(model={Model},order={Order})", model, order);
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(order);

        var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
		_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): primaryStoreCurrency → {PrimaryStoreCurrency}", model, order, primaryStoreCurrency);
        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
		_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): languageId → {LanguageId}", model, order, languageId);

        //subtotal
        model.OrderSubtotalInclTax = await _priceFormatter
            .FormatOrderPriceAsync(order.OrderSubtotalInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true);
				_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.OrderSubtotalInclTax → {Arg2}", model, order, model.OrderSubtotalInclTax);
        model.OrderSubtotalExclTax = await _priceFormatter
            .FormatOrderPriceAsync(order.OrderSubtotalExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false);
				_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.OrderSubtotalExclTax → {Arg2}", model, order, model.OrderSubtotalExclTax);
				_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.OrderSubtotalInclTaxValue → {Arg2}", model, order, model.OrderSubtotalInclTaxValue);
				_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.OrderSubtotalExclTaxValue → {Arg2}", model, order, model.OrderSubtotalExclTaxValue);
        model.OrderSubtotalInclTaxValue = order.OrderSubtotalInclTax;
        model.OrderSubtotalExclTaxValue = order.OrderSubtotalExclTax;

        //discount (applied to order subtotal)
        var orderSubtotalDiscountInclTaxStr = await _priceFormatter
            .FormatOrderPriceAsync(order.OrderSubTotalDiscountInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true);
        var orderSubtotalDiscountExclTaxStr = await _priceFormatter
            .FormatOrderPriceAsync(order.OrderSubTotalDiscountExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false);
        if (order.OrderSubTotalDiscountInclTax > decimal.Zero)
            model.OrderSubTotalDiscountInclTax = orderSubtotalDiscountInclTaxStr;
			_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.OrderSubTotalDiscountInclTax → {Arg2}", model, order, model.OrderSubTotalDiscountInclTax);
        if (order.OrderSubTotalDiscountExclTax > decimal.Zero)
            model.OrderSubTotalDiscountExclTax = orderSubtotalDiscountExclTaxStr;
			_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.OrderSubTotalDiscountExclTax → {Arg2}", model, order, model.OrderSubTotalDiscountExclTax);
			_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.OrderSubTotalDiscountInclTaxValue → {Arg2}", model, order, model.OrderSubTotalDiscountInclTaxValue);
			_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.OrderSubTotalDiscountExclTaxValue → {Arg2}", model, order, model.OrderSubTotalDiscountExclTaxValue);
        model.OrderSubTotalDiscountInclTaxValue = order.OrderSubTotalDiscountInclTax;
        model.OrderSubTotalDiscountExclTaxValue = order.OrderSubTotalDiscountExclTax;

        //shipping
        model.OrderShippingInclTax = await _priceFormatter
            .FormatOrderPriceAsync(order.OrderShippingInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true,
                _taxSettings.ShippingIsTaxable && _taxSettings.DisplayTaxSuffix);
				_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.OrderShippingInclTax → {Arg2}", model, order, model.OrderShippingInclTax);
        model.OrderShippingExclTax = await _priceFormatter
            .FormatOrderPriceAsync(order.OrderShippingExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false,
                _taxSettings.ShippingIsTaxable && _taxSettings.DisplayTaxSuffix);
				_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.OrderShippingExclTax → {Arg2}", model, order, model.OrderShippingExclTax);
				_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.OrderShippingInclTaxValue → {Arg2}", model, order, model.OrderShippingInclTaxValue);
				_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.OrderShippingExclTaxValue → {Arg2}", model, order, model.OrderShippingExclTaxValue);
        model.OrderShippingInclTaxValue = order.OrderShippingInclTax;
        model.OrderShippingExclTaxValue = order.OrderShippingExclTax;

        //payment method additional fee
        if (order.PaymentMethodAdditionalFeeInclTax > decimal.Zero)
        {
            model.PaymentMethodAdditionalFeeInclTax = await _priceFormatter
                .FormatOrderPriceAsync(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true,
                    _taxSettings.PaymentMethodAdditionalFeeIsTaxable && _taxSettings.DisplayTaxSuffix);
					_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.PaymentMethodAdditionalFeeInclTax → {Arg2}", model, order, model.PaymentMethodAdditionalFeeInclTax);
            model.PaymentMethodAdditionalFeeExclTax = await _priceFormatter
                .FormatOrderPriceAsync(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false,
                    _taxSettings.PaymentMethodAdditionalFeeIsTaxable && _taxSettings.DisplayTaxSuffix);
					_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.PaymentMethodAdditionalFeeExclTax → {Arg2}", model, order, model.PaymentMethodAdditionalFeeExclTax);
        }

        model.PaymentMethodAdditionalFeeInclTaxValue = order.PaymentMethodAdditionalFeeInclTax;
		_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.PaymentMethodAdditionalFeeInclTaxValue → {Arg2}", model, order, model.PaymentMethodAdditionalFeeInclTaxValue);
		_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.PaymentMethodAdditionalFeeExclTaxValue → {Arg2}", model, order, model.PaymentMethodAdditionalFeeExclTaxValue);
        model.PaymentMethodAdditionalFeeExclTaxValue = order.PaymentMethodAdditionalFeeExclTax;

        //tax
        model.Tax = await _priceFormatter
            .FormatOrderPriceAsync(order.OrderTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, null, false);
				_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.Tax → {Arg2}", model, order, model.Tax);
        var taxRates = _orderService.ParseTaxRates(order, order.TaxRates);
		_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): taxRates → {TaxRates}", model, order, taxRates);
        var displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
		_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): displayTaxRates → {DisplayTaxRates}", model, order, displayTaxRates);
        var displayTax = !displayTaxRates;
        foreach (var tr in taxRates)
        {
            model.TaxRates.Add(new OrderModel.TaxRate
            {
                Rate = _priceFormatter.FormatTaxRate(tr.Key),
                Value = await _priceFormatter
                    .FormatOrderPriceAsync(tr.Value, order.CurrencyRate, order.CustomerCurrencyCode,
                        _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, null, false)
            });
        }

        model.DisplayTaxRates = displayTaxRates;
		_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.DisplayTaxRates → {Arg2}", model, order, model.DisplayTaxRates);
		_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.DisplayTax → {Arg2}", model, order, model.DisplayTax);
		_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.TaxValue → {Arg2}", model, order, model.TaxValue);
		_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.TaxRatesValue → {Arg2}", model, order, model.TaxRatesValue);
        model.DisplayTax = displayTax;
        model.TaxValue = order.OrderTax;
        model.TaxRatesValue = order.TaxRates;

        //discount
        if (order.OrderDiscount > 0)
        {
            model.OrderTotalDiscount = await _priceFormatter
                .FormatOrderPriceAsync(-order.OrderDiscount, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, null, false);
					_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.OrderTotalDiscount → {Arg2}", model, order, model.OrderTotalDiscount);
        }
        model.OrderTotalDiscountValue = order.OrderDiscount;
		_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.OrderTotalDiscountValue → {Arg2}", model, order, model.OrderTotalDiscountValue);

        //gift cards
        foreach (var gcuh in await _giftCardService.GetGiftCardUsageHistoryAsync(order))
        {
            model.GiftCards.Add(new OrderModel.GiftCard
            {
                CouponCode = (await _giftCardService.GetGiftCardByIdAsync(gcuh.GiftCardId)).GiftCardCouponCode,
                Amount = await _priceFormatter.FormatPriceAsync(-gcuh.UsedValue, true, false)
            });
        }

        //reward points
        if (order.RedeemedRewardPointsEntryId.HasValue && await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
        {
            model.RedeemedRewardPoints = -redeemedRewardPointsEntry.Points;
			_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.RedeemedRewardPoints → {Arg2}", model, order, model.RedeemedRewardPoints);
            model.RedeemedRewardPointsAmount =
                await _priceFormatter.FormatPriceAsync(-redeemedRewardPointsEntry.UsedAmount, true, false);
        }

        //total
        model.OrderTotal = await _priceFormatter
            .FormatOrderPriceAsync(order.OrderTotal, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, null, false);
				_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.OrderTotal → {Arg2}", model, order, model.OrderTotal);
				_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.OrderTotalValue → {Arg2}", model, order, model.OrderTotalValue);
        model.OrderTotalValue = order.OrderTotal;

        //refunded amount
        if (order.RefundedAmount > decimal.Zero)
            model.RefundedAmount = await _priceFormatter.FormatPriceAsync(order.RefundedAmount, true, false);

        //used discounts
        var duh = await _discountService.GetAllDiscountUsageHistoryAsync(orderId: order.Id);
		_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): duh → {Duh}", model, order, duh);
        foreach (var d in duh)
        {
            var discount = await _discountService.GetDiscountByIdAsync(d.DiscountId);
			_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): discount → {Discount}", model, order, discount);

            model.UsedDiscounts.Add(new OrderModel.UsedDiscountModel
            {
                DiscountId = d.DiscountId,
                DiscountName = discount.Name
            });
        }

        //profit (hide for vendors)
        if (await _workContext.GetCurrentVendorAsync() != null)
            return;

        var profit = await _orderReportService.ProfitReportAsync(orderId: order.Id);
		_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): profit → {Profit}", model, order, profit);
        model.Profit = await _priceFormatter
            .FormatOrderPriceAsync(profit, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, null, false);
				_logger.LogDebug("PrepareOrderModelTotalsAsync(model={Model},order={Order}): model.Profit → {Arg2}", model, order, model.Profit);
				_logger.LogDebug("<<< Exiting PrepareOrderModelTotalsAsync(model={Model},order={Order})", model, order);
    }

    /// <summary>
    /// Prepare order model payment info
    /// </summary>
    /// <param name="model">Order model</param>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareOrderModelPaymentInfoAsync(OrderModel model, Order order)
    {
		_logger.LogDebug(">>> Entering PrepareOrderModelPaymentInfoAsync(model={Model},order={Order})", model, order);
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(order);

        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
		_logger.LogDebug("PrepareOrderModelPaymentInfoAsync(model={Model},order={Order}): billingAddress → {BillingAddress}", model, order, billingAddress);

        //prepare billing address
        model.BillingAddress = billingAddress.ToModel(model.BillingAddress);

        model.BillingAddress.CountryName = (await _countryService.GetCountryByAddressAsync(billingAddress))?.Name;
        model.BillingAddress.StateProvinceName = (await _stateProvinceService.GetStateProvinceByAddressAsync(billingAddress))?.Name;

        await _addressModelFactory.PrepareAddressModelAsync(model.BillingAddress, billingAddress);
        SetAddressFieldsAsRequired(model.BillingAddress);

        if (order.AllowStoringCreditCardNumber)
        {
            //card type
            model.CardType = _encryptionService.DecryptText(order.CardType);
            //cardholder name
            model.CardName = _encryptionService.DecryptText(order.CardName);
            //card number
            model.CardNumber = _encryptionService.DecryptText(order.CardNumber);
            //cvv
            model.CardCvv2 = _encryptionService.DecryptText(order.CardCvv2);
            //expiry date
            var cardExpirationMonthDecrypted = _encryptionService.DecryptText(order.CardExpirationMonth);
			_logger.LogDebug("PrepareOrderModelPaymentInfoAsync(model={Model},order={Order}): cardExpirationMonthDecrypted → {CardExpirationMonthDecrypted}", model, order, cardExpirationMonthDecrypted);
            if (!string.IsNullOrEmpty(cardExpirationMonthDecrypted) && cardExpirationMonthDecrypted != "0")
                model.CardExpirationMonth = cardExpirationMonthDecrypted;
				_logger.LogDebug("PrepareOrderModelPaymentInfoAsync(model={Model},order={Order}): model.CardExpirationMonth → {Arg2}", model, order, model.CardExpirationMonth);
            var cardExpirationYearDecrypted = _encryptionService.DecryptText(order.CardExpirationYear);
			_logger.LogDebug("PrepareOrderModelPaymentInfoAsync(model={Model},order={Order}): cardExpirationYearDecrypted → {CardExpirationYearDecrypted}", model, order, cardExpirationYearDecrypted);
            if (!string.IsNullOrEmpty(cardExpirationYearDecrypted) && cardExpirationYearDecrypted != "0")
                model.CardExpirationYear = cardExpirationYearDecrypted;
				_logger.LogDebug("PrepareOrderModelPaymentInfoAsync(model={Model},order={Order}): model.CardExpirationYear → {Arg2}", model, order, model.CardExpirationYear);

            model.AllowStoringCreditCardNumber = true;
			_logger.LogDebug("PrepareOrderModelPaymentInfoAsync(model={Model},order={Order}): model.AllowStoringCreditCardNumber → {Arg2}", model, order, model.AllowStoringCreditCardNumber);
        }
        else
        {
            var maskedCreditCardNumberDecrypted = _encryptionService.DecryptText(order.MaskedCreditCardNumber);
			_logger.LogDebug("PrepareOrderModelPaymentInfoAsync(model={Model},order={Order}): maskedCreditCardNumberDecrypted → {MaskedCreditCardNumberDecrypted}", model, order, maskedCreditCardNumberDecrypted);
            if (!string.IsNullOrEmpty(maskedCreditCardNumberDecrypted))
                model.CardNumber = maskedCreditCardNumberDecrypted;
				_logger.LogDebug("PrepareOrderModelPaymentInfoAsync(model={Model},order={Order}): model.CardNumber → {Arg2}", model, order, model.CardNumber);
        }

        //payment transaction info
        model.AuthorizationTransactionId = order.AuthorizationTransactionId;
		_logger.LogDebug("PrepareOrderModelPaymentInfoAsync(model={Model},order={Order}): model.AuthorizationTransactionId → {Arg2}", model, order, model.AuthorizationTransactionId);
		_logger.LogDebug("PrepareOrderModelPaymentInfoAsync(model={Model},order={Order}): model.CaptureTransactionId → {Arg2}", model, order, model.CaptureTransactionId);
		_logger.LogDebug("PrepareOrderModelPaymentInfoAsync(model={Model},order={Order}): model.SubscriptionTransactionId → {Arg2}", model, order, model.SubscriptionTransactionId);
        model.CaptureTransactionId = order.CaptureTransactionId;
        model.SubscriptionTransactionId = order.SubscriptionTransactionId;

        //payment method info
        var pm = await _paymentPluginManager.LoadPluginBySystemNameAsync(order.PaymentMethodSystemName);
		_logger.LogDebug("PrepareOrderModelPaymentInfoAsync(model={Model},order={Order}): pm → {Pm}", model, order, pm);
        model.PaymentMethod = pm != null ? pm.PluginDescriptor.FriendlyName : order.PaymentMethodSystemName;
        model.PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);

        //payment method buttons
        model.CanCancelOrder = _orderProcessingService.CanCancelOrder(order);
        model.CanCapture = await _orderProcessingService.CanCaptureAsync(order);
        model.CanMarkOrderAsPaid = _orderProcessingService.CanMarkOrderAsPaid(order);
        model.CanRefund = await _orderProcessingService.CanRefundAsync(order);
        model.CanRefundOffline = _orderProcessingService.CanRefundOffline(order);
        model.CanPartiallyRefund = await _orderProcessingService.CanPartiallyRefundAsync(order, decimal.Zero);
        model.CanPartiallyRefundOffline = _orderProcessingService.CanPartiallyRefundOffline(order, decimal.Zero);
        model.CanVoid = await _orderProcessingService.CanVoidAsync(order);
        model.CanVoidOffline = _orderProcessingService.CanVoidOffline(order);

        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
        model.MaxAmountToRefund = order.OrderTotal - order.RefundedAmount;
		_logger.LogDebug("PrepareOrderModelPaymentInfoAsync(model={Model},order={Order}): model.MaxAmountToRefund → {Arg2}", model, order, model.MaxAmountToRefund);

        //recurring payment record
        model.RecurringPaymentId = (await _orderService.SearchRecurringPaymentsAsync(initialOrderId: order.Id, showHidden: true)).FirstOrDefault()?.Id ?? 0;
		_logger.LogDebug("<<< Exiting PrepareOrderModelPaymentInfoAsync(model={Model},order={Order})", model, order);
    }

    /// <summary>
    /// Prepare order model shipping info
    /// </summary>
    /// <param name="model">Order model</param>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareOrderModelShippingInfoAsync(OrderModel model, Order order)
    {
		_logger.LogDebug(">>> Entering PrepareOrderModelShippingInfoAsync(model={Model},order={Order})", model, order);
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(order);

        model.ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus);
        if (order.ShippingStatus == ShippingStatus.ShippingNotRequired)
            return;

        model.IsShippable = true;
		_logger.LogDebug("PrepareOrderModelShippingInfoAsync(model={Model},order={Order}): model.IsShippable → {Arg2}", model, order, model.IsShippable);
		_logger.LogDebug("PrepareOrderModelShippingInfoAsync(model={Model},order={Order}): model.ShippingMethod → {Arg2}", model, order, model.ShippingMethod);
        model.ShippingMethod = order.ShippingMethod;
        model.CanAddNewShipments = await _orderService.HasItemsToAddToShipmentAsync(order);
        model.PickupInStore = order.PickupInStore;
		_logger.LogDebug("PrepareOrderModelShippingInfoAsync(model={Model},order={Order}): model.PickupInStore → {Arg2}", model, order, model.PickupInStore);
        if (!order.PickupInStore)
        {
            var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value);
			_logger.LogDebug("PrepareOrderModelShippingInfoAsync(model={Model},order={Order}): shippingAddress → {ShippingAddress}", model, order, shippingAddress);
            var shippingCountry = await _countryService.GetCountryByAddressAsync(shippingAddress);
			_logger.LogDebug("PrepareOrderModelShippingInfoAsync(model={Model},order={Order}): shippingCountry → {ShippingCountry}", model, order, shippingCountry);

            model.ShippingAddress = shippingAddress.ToModel(model.ShippingAddress);
            model.ShippingAddress.CountryName = shippingCountry?.Name;
            model.ShippingAddress.StateProvinceName = (await _stateProvinceService.GetStateProvinceByAddressAsync(shippingAddress))?.Name;
            await _addressModelFactory.PrepareAddressModelAsync(model.ShippingAddress, shippingAddress);
            SetAddressFieldsAsRequired(model.ShippingAddress);
            model.ShippingAddressGoogleMapsUrl = "https://maps.google.com/maps?f=q&hl=en&ie=UTF8&oe=UTF8&geocode=&q=" +
                                                 $"{WebUtility.UrlEncode(shippingAddress.Address1 + " " + shippingAddress.ZipPostalCode + " " + shippingAddress.City + " " + (shippingCountry?.Name ?? string.Empty))}";
												_logger.LogDebug("PrepareOrderModelShippingInfoAsync(model={Model},order={Order}): model.ShippingAddressGoogleMapsUrl → {Arg2}", model, order, model.ShippingAddressGoogleMapsUrl);
        }
        else
        {
            if (order.PickupAddressId is null)
                return;

            var pickupAddress = await _addressService.GetAddressByIdAsync(order.PickupAddressId.Value);
			_logger.LogDebug("PrepareOrderModelShippingInfoAsync(model={Model},order={Order}): pickupAddress → {PickupAddress}", model, order, pickupAddress);

            var pickupCountry = await _countryService.GetCountryByAddressAsync(pickupAddress);
			_logger.LogDebug("PrepareOrderModelShippingInfoAsync(model={Model},order={Order}): pickupCountry → {PickupCountry}", model, order, pickupCountry);

            model.PickupAddress = pickupAddress.ToModel(model.PickupAddress);
            model.PickupAddressGoogleMapsUrl = $"https://maps.google.com/maps?f=q&hl=en&ie=UTF8&oe=UTF8&geocode=&q=" +
                                               $"{WebUtility.UrlEncode($"{pickupAddress.Address1} {pickupAddress.ZipPostalCode} {pickupAddress.City} {(pickupCountry?.Name ?? string.Empty)}")}";
											_logger.LogDebug("PrepareOrderModelShippingInfoAsync(model={Model},order={Order}): model.PickupAddressGoogleMapsUrl → {Arg2}", model, order, model.PickupAddressGoogleMapsUrl);
        }
		_logger.LogDebug("<<< Exiting PrepareOrderModelShippingInfoAsync(model={Model},order={Order})", model, order);
    }

    /// <summary>
    /// Prepare product attribute models
    /// </summary>
    /// <param name="models">List of product attribute models</param>
    /// <param name="order">Order</param>
    /// <param name="product">Product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareProductAttributeModelsAsync(IList<AddProductToOrderModel.ProductAttributeModel> models, Order order, Product product)
    {
		_logger.LogDebug(">>> Entering PrepareProductAttributeModelsAsync(models={Models},order={Order},product={Product})", models, order, product);
        ArgumentNullException.ThrowIfNull(models);

        ArgumentNullException.ThrowIfNull(order);

        ArgumentNullException.ThrowIfNull(product);

        var attributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
		_logger.LogDebug("PrepareProductAttributeModelsAsync(models={Models},order={Order},product={Product}): attributes → {Attributes}", models, order, product, attributes);
        foreach (var attribute in attributes)
        {
            var attributeModel = new AddProductToOrderModel.ProductAttributeModel
            {
                Id = attribute.Id,
                ProductAttributeId = attribute.ProductAttributeId,
                Name = (await _productAttributeService.GetProductAttributeByIdAsync(attribute.ProductAttributeId)).Name,
                TextPrompt = attribute.TextPrompt,
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType,
                HasCondition = !string.IsNullOrEmpty(attribute.ConditionAttributeXml)
            };
            if (!string.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
            {
                attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                    .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
					_logger.LogDebug("PrepareProductAttributeModelsAsync(models={Models},order={Order},product={Product}): attributeModel.AllowedFileExtensions → {Arg3}", models, order, product, attributeModel.AllowedFileExtensions);
            }

            if (attribute.ShouldHaveValues())
            {
                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
				_logger.LogDebug("PrepareProductAttributeModelsAsync(models={Models},order={Order},product={Product}): customer → {Customer}", models, order, product, customer);
                var store = await _storeService.GetStoreByIdAsync(order.StoreId);
				_logger.LogDebug("PrepareProductAttributeModelsAsync(models={Models},order={Order},product={Product}): store → {Store}", models, order, product, store);

                //values
                var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
				_logger.LogDebug("PrepareProductAttributeModelsAsync(models={Models},order={Order},product={Product}): attributeValues → {AttributeValues}", models, order, product, attributeValues);
                foreach (var attributeValue in attributeValues)
                {
                    //price adjustment
                    var (priceAdjustment, _) = await _taxService.GetProductPriceAsync(product,
                        await _priceCalculationService.GetProductAttributeValuePriceAdjustmentAsync(product, attributeValue, customer, store));

                    var priceAdjustmentStr = string.Empty;
					_logger.LogDebug("PrepareProductAttributeModelsAsync(models={Models},order={Order},product={Product}): priceAdjustmentStr → {PriceAdjustmentStr}", models, order, product, priceAdjustmentStr);
                    if (priceAdjustment != 0)
                    {
                        if (attributeValue.PriceAdjustmentUsePercentage)
                        {
                            priceAdjustmentStr = attributeValue.PriceAdjustment.ToString("G29");
                            priceAdjustmentStr = priceAdjustment > 0 ? $"+{priceAdjustmentStr}%" : $"{priceAdjustmentStr}%";
                        }
                        else
                        {
                            priceAdjustmentStr = priceAdjustment > 0 ? $"+{await _priceFormatter.FormatPriceAsync(priceAdjustment, false, false)}" : $"-{await _priceFormatter.FormatPriceAsync(-priceAdjustment, false, false)}";
                        }
                    }

                    attributeModel.Values.Add(new AddProductToOrderModel.ProductAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = attributeValue.Name,
                        IsPreSelected = attributeValue.IsPreSelected,
                        CustomerEntersQty = attributeValue.CustomerEntersQty,
                        Quantity = attributeValue.Quantity,
                        PriceAdjustment = priceAdjustmentStr,
                        PriceAdjustmentValue = priceAdjustment
                    });
                }
            }

            models.Add(attributeModel);
        }
		_logger.LogDebug("<<< Exiting PrepareProductAttributeModelsAsync(models={Models},order={Order},product={Product})", models, order, product);
    }

    /// <summary>
    /// Prepare shipment item model
    /// </summary>
    /// <param name="model">Shipment item model</param>
    /// <param name="orderItem">Order item</param>
    /// <param name="product">Product item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareShipmentItemModelAsync(ShipmentItemModel model, OrderItem orderItem, Product product)
    {
		_logger.LogDebug(">>> Entering PrepareShipmentItemModelAsync(model={Model},orderItem={OrderItem},product={Product})", model, orderItem, product);
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(orderItem);

        ArgumentNullException.ThrowIfNull(product);

        if (orderItem.ProductId != product.Id)
			_logger.LogWarning("Throwing ArgumentException from PrepareShipmentItemModelAsync(model={Model},orderItem={OrderItem},product={Product})", model, orderItem, product);
            throw new ArgumentException($"{nameof(orderItem.ProductId)} != {nameof(product.Id)}");

        //fill in additional values (not existing in the entity)
        model.OrderItemId = orderItem.Id;
		_logger.LogDebug("PrepareShipmentItemModelAsync(model={Model},orderItem={OrderItem},product={Product}): model.OrderItemId → {Arg3}", model, orderItem, product, model.OrderItemId);
		_logger.LogDebug("PrepareShipmentItemModelAsync(model={Model},orderItem={OrderItem},product={Product}): model.ProductId → {Arg3}", model, orderItem, product, model.ProductId);
		_logger.LogDebug("PrepareShipmentItemModelAsync(model={Model},orderItem={OrderItem},product={Product}): model.ProductName → {Arg3}", model, orderItem, product, model.ProductName);
        model.ProductId = orderItem.ProductId;
        model.ProductName = product.Name;
        model.Sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml);
        model.AttributeInfo = orderItem.AttributeDescription;
		_logger.LogDebug("PrepareShipmentItemModelAsync(model={Model},orderItem={OrderItem},product={Product}): model.AttributeInfo → {Arg3}", model, orderItem, product, model.AttributeInfo);
		_logger.LogDebug("PrepareShipmentItemModelAsync(model={Model},orderItem={OrderItem},product={Product}): model.ShipSeparately → {Arg3}", model, orderItem, product, model.ShipSeparately);
		_logger.LogDebug("PrepareShipmentItemModelAsync(model={Model},orderItem={OrderItem},product={Product}): model.QuantityOrdered → {Arg3}", model, orderItem, product, model.QuantityOrdered);
        model.ShipSeparately = product.ShipSeparately;
        model.QuantityOrdered = orderItem.Quantity;
        model.QuantityInAllShipments = await _orderService.GetTotalNumberOfItemsInAllShipmentsAsync(orderItem);
        model.QuantityToAdd = await _orderService.GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem);

        var baseWeight = (await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId))?.Name;
		_logger.LogDebug("PrepareShipmentItemModelAsync(model={Model},orderItem={OrderItem},product={Product}): baseWeight → {BaseWeight}", model, orderItem, product, baseWeight);
        var baseDimension = (await _measureService.GetMeasureDimensionByIdAsync(_measureSettings.BaseDimensionId))?.Name;
		_logger.LogDebug("PrepareShipmentItemModelAsync(model={Model},orderItem={OrderItem},product={Product}): baseDimension → {BaseDimension}", model, orderItem, product, baseDimension);
        if (orderItem.ItemWeight.HasValue)
            model.ItemWeight = $"{orderItem.ItemWeight:F2} [{baseWeight}]";
			_logger.LogDebug("PrepareShipmentItemModelAsync(model={Model},orderItem={OrderItem},product={Product}): model.ItemWeight → {Arg3}", model, orderItem, product, model.ItemWeight);
        model.ItemDimensions =
            $"{product.Length:F2} x {product.Width:F2} x {product.Height:F2} [{baseDimension}]";

        if (!product.IsRental)
            return;

        var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
            ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : string.Empty;
        var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
            ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : string.Empty;
        model.RentalInfo = string.Format(await _localizationService.GetResourceAsync("Order.Rental.FormattedDate"), rentalStartDate, rentalEndDate);
		_logger.LogDebug("<<< Exiting PrepareShipmentItemModelAsync(model={Model},orderItem={OrderItem},product={Product})", model, orderItem, product);
    }

    /// <summary>
    /// Prepare shipment status event models
    /// </summary>
    /// <param name="models">List of shipment status event models</param>
    /// <param name="shipment">Shipment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareShipmentStatusEventModelsAsync(IList<ShipmentStatusEventModel> models, Shipment shipment)
    {
		_logger.LogDebug(">>> Entering PrepareShipmentStatusEventModelsAsync(models={Models},shipment={Shipment})", models, shipment);
        ArgumentNullException.ThrowIfNull(models);

        var shipmentTracker = await _shipmentService.GetShipmentTrackerAsync(shipment);
		_logger.LogDebug("PrepareShipmentStatusEventModelsAsync(models={Models},shipment={Shipment}): shipmentTracker → {ShipmentTracker}", models, shipment, shipmentTracker);
        var shipmentEvents = await shipmentTracker?.GetShipmentEventsAsync(shipment.TrackingNumber, shipment);
		_logger.LogDebug("PrepareShipmentStatusEventModelsAsync(models={Models},shipment={Shipment}): shipmentEvents → {ShipmentEvents}", models, shipment, shipmentEvents);
        if (shipmentEvents == null)
            return;

        foreach (var shipmentEvent in shipmentEvents)
        {
            var shipmentStatusEventModel = new ShipmentStatusEventModel
            {
                Status = shipmentEvent.Status,
                Date = shipmentEvent.Date,
                EventName = shipmentEvent.EventName,
                Location = shipmentEvent.Location
            };
            var shipmentEventCountry = await _countryService.GetCountryByTwoLetterIsoCodeAsync(shipmentEvent.CountryCode);
			_logger.LogDebug("PrepareShipmentStatusEventModelsAsync(models={Models},shipment={Shipment}): shipmentEventCountry → {ShipmentEventCountry}", models, shipment, shipmentEventCountry);
            shipmentStatusEventModel.Country = shipmentEventCountry != null
                ? await _localizationService.GetLocalizedAsync(shipmentEventCountry, x => x.Name) : shipmentEvent.CountryCode;
            models.Add(shipmentStatusEventModel);
        }
		_logger.LogDebug("<<< Exiting PrepareShipmentStatusEventModelsAsync(models={Models},shipment={Shipment})", models, shipment);
    }

    /// <summary>
    /// Prepare order shipment search model
    /// </summary>
    /// <param name="searchModel">Order shipment search model</param>
    /// <param name="order">Order</param>
    /// <returns>Order shipment search model</returns>
    protected virtual OrderShipmentSearchModel PrepareOrderShipmentSearchModel(OrderShipmentSearchModel searchModel, Order order)
    {
		_logger.LogDebug(">>> Entering PrepareOrderShipmentSearchModel(searchModel={SearchModel},order={Order})", searchModel, order);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(order);

        searchModel.OrderId = order.Id;
		_logger.LogDebug("PrepareOrderShipmentSearchModel(searchModel={SearchModel},order={Order}): searchModel.OrderId → {Arg2}", searchModel, order, searchModel.OrderId);

        //prepare nested search model
        PrepareShipmentItemSearchModel(searchModel.ShipmentItemSearchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareOrderShipmentSearchModel(searchModel={SearchModel},order={Order})", searchModel, order);
        return searchModel;
    }

    /// <summary>
    /// Prepare shipment item search model
    /// </summary>
    /// <param name="searchModel">Shipment item search model</param>
    /// <returns>Shipment item search model</returns>
    protected virtual ShipmentItemSearchModel PrepareShipmentItemSearchModel(ShipmentItemSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareShipmentItemSearchModel(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareShipmentItemSearchModel(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare order note search model
    /// </summary>
    /// <param name="searchModel">Order note search model</param>
    /// <param name="order">Order</param>
    /// <returns>Order note search model</returns>
    protected virtual OrderNoteSearchModel PrepareOrderNoteSearchModel(OrderNoteSearchModel searchModel, Order order)
    {
		_logger.LogDebug(">>> Entering PrepareOrderNoteSearchModel(searchModel={SearchModel},order={Order})", searchModel, order);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(order);

        searchModel.OrderId = order.Id;
		_logger.LogDebug("PrepareOrderNoteSearchModel(searchModel={SearchModel},order={Order}): searchModel.OrderId → {Arg2}", searchModel, order, searchModel.OrderId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareOrderNoteSearchModel(searchModel={SearchModel},order={Order})", searchModel, order);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare order search model
    /// </summary>
    /// <param name="searchModel">Order search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order search model
    /// </returns>
    public virtual async Task<OrderSearchModel> PrepareOrderSearchModelAsync(OrderSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareOrderSearchModelAsync(searchModel={SearchModel})", searchModel);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;
        searchModel.BillingPhoneEnabled = _addressSettings.PhoneEnabled;
		_logger.LogDebug("PrepareOrderSearchModelAsync(searchModel={SearchModel}): searchModel.BillingPhoneEnabled → {Arg1}", searchModel, searchModel.BillingPhoneEnabled);

        var licenseCheckModel = new LicenseCheckModel();
		_logger.LogDebug("PrepareOrderSearchModelAsync(searchModel={SearchModel}): licenseCheckModel → {LicenseCheckModel}", searchModel, licenseCheckModel);
        try
        {
            var result = await _nopHttpClient.GetLicenseCheckDetailsAsync();
			_logger.LogDebug("PrepareOrderSearchModelAsync(searchModel={SearchModel}): result → {Result}", searchModel, result);
            if (!string.IsNullOrEmpty(result))
            {
                licenseCheckModel = JsonConvert.DeserializeObject<LicenseCheckModel>(result);
                if (licenseCheckModel.DisplayWarning == false && licenseCheckModel.BlockPages == false)
                    await _settingService.SetSettingAsync($"{nameof(AdminAreaSettings)}.{nameof(AdminAreaSettings.CheckLicense)}", false);
            }
        }
        catch { }
		_logger.LogError("Exception in PrepareOrderSearchModelAsync(searchModel={SearchModel}): {Arg1}", searchModel, ex.Message);
		_logger.LogDebug("PrepareOrderSearchModelAsync(searchModel={SearchModel}): searchModel.LicenseCheckModel → {Arg1}", searchModel, searchModel.LicenseCheckModel);
        searchModel.LicenseCheckModel = licenseCheckModel;

        //prepare available order, payment and shipping statuses
        await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);
        if (searchModel.AvailableOrderStatuses.Any())
        {
            if (searchModel.OrderStatusIds?.Any() ?? false)
            {
                var ids = searchModel.OrderStatusIds.Select(id => id.ToString());
				_logger.LogDebug("PrepareOrderSearchModelAsync(searchModel={SearchModel}): ids → {Ids}", searchModel, ids);
                var statusItems = searchModel.AvailableOrderStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList();
				_logger.LogDebug("PrepareOrderSearchModelAsync(searchModel={SearchModel}): statusItems → {StatusItems}", searchModel, statusItems);
                foreach (var statusItem in statusItems)
                {
                    statusItem.Selected = true;
					_logger.LogDebug("PrepareOrderSearchModelAsync(searchModel={SearchModel}): statusItem.Selected → {Arg1}", searchModel, statusItem.Selected);
                }
            }
            else
                searchModel.AvailableOrderStatuses.FirstOrDefault().Selected = true;
        }

        await _baseAdminModelFactory.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);
        if (searchModel.AvailablePaymentStatuses.Any())
        {
            if (searchModel.PaymentStatusIds?.Any() ?? false)
            {
                var ids = searchModel.PaymentStatusIds.Select(id => id.ToString());
                var statusItems = searchModel.AvailablePaymentStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList();
                foreach (var statusItem in statusItems)
                {
                    statusItem.Selected = true;
                }
            }
            else
                searchModel.AvailablePaymentStatuses.FirstOrDefault().Selected = true;
        }

        await _baseAdminModelFactory.PrepareShippingStatusesAsync(searchModel.AvailableShippingStatuses);
        if (searchModel.AvailableShippingStatuses.Any())
        {
            if (searchModel.ShippingStatusIds?.Any() ?? false)
            {
                var ids = searchModel.ShippingStatusIds.Select(id => id.ToString());
                var statusItems = searchModel.AvailableShippingStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList();
                foreach (var statusItem in statusItems)
                {
                    statusItem.Selected = true;
                }
            }
            else
                searchModel.AvailableShippingStatuses.FirstOrDefault().Selected = true;
        }

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        //prepare available vendors
        await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

        //prepare available warehouses
        await _baseAdminModelFactory.PrepareWarehousesAsync(searchModel.AvailableWarehouses);

        //prepare available payment methods
        searchModel.AvailablePaymentMethods = (await _paymentPluginManager.LoadAllPluginsAsync()).Select(method =>
            new SelectListItem { Text = method.PluginDescriptor.FriendlyName, Value = method.PluginDescriptor.SystemName }).ToList();
        searchModel.AvailablePaymentMethods.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = string.Empty });

        //prepare available billing countries
        searchModel.AvailableCountries = (await _countryService.GetAllCountriesForBillingAsync(showHidden: true))
            .Select(country => new SelectListItem { Text = country.Name, Value = country.Id.ToString() }).ToList();
        searchModel.AvailableCountries.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });

        //prepare grid
        searchModel.SetGridPageSize();

        searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

		_logger.LogInformation("PrepareOrderSearchModelAsync(searchModel)={SearchModel}: external service call took {Arg1} ms", searchModel, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting PrepareOrderSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged order list model
    /// </summary>
    /// <param name="searchModel">Order search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order list model
    /// </returns>
    public virtual async Task<OrderListModel> PrepareOrderListModelAsync(OrderSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareOrderListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter orders
        var orderStatusIds = (searchModel.OrderStatusIds?.Contains(0) ?? true) ? null : searchModel.OrderStatusIds.ToList();
		_logger.LogDebug("PrepareOrderListModelAsync(searchModel={SearchModel}): orderStatusIds → {OrderStatusIds}", searchModel, orderStatusIds);
        var paymentStatusIds = (searchModel.PaymentStatusIds?.Contains(0) ?? true) ? null : searchModel.PaymentStatusIds.ToList();
		_logger.LogDebug("PrepareOrderListModelAsync(searchModel={SearchModel}): paymentStatusIds → {PaymentStatusIds}", searchModel, paymentStatusIds);
        var shippingStatusIds = (searchModel.ShippingStatusIds?.Contains(0) ?? true) ? null : searchModel.ShippingStatusIds.ToList();
		_logger.LogDebug("PrepareOrderListModelAsync(searchModel={SearchModel}): shippingStatusIds → {ShippingStatusIds}", searchModel, shippingStatusIds);
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("PrepareOrderListModelAsync(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
        if (currentVendor != null)
            searchModel.VendorId = currentVendor.Id;
			_logger.LogDebug("PrepareOrderListModelAsync(searchModel={SearchModel}): searchModel.VendorId → {Arg1}", searchModel, searchModel.VendorId);
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var product = await _productService.GetProductByIdAsync(searchModel.ProductId);
		_logger.LogDebug("PrepareOrderListModelAsync(searchModel={SearchModel}): product → {Product}", searchModel, product);
        var filterByProductId = product != null && (currentVendor == null || product.VendorId == currentVendor.Id)
            ? searchModel.ProductId : 0;

        //get orders
        var orders = await _orderService.SearchOrdersAsync(storeId: searchModel.StoreId,
            vendorId: searchModel.VendorId,
            productId: filterByProductId,
            warehouseId: searchModel.WarehouseId,
            paymentMethodSystemName: searchModel.PaymentMethodSystemName,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            osIds: orderStatusIds,
            psIds: paymentStatusIds,
            ssIds: shippingStatusIds,
            billingPhone: searchModel.BillingPhone,
            billingEmail: searchModel.BillingEmail,
            billingLastName: searchModel.BillingLastName,
            billingCountryId: searchModel.BillingCountryId,
            orderNotes: searchModel.OrderNotes,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new OrderListModel().PrepareToGridAsync(searchModel, orders, () =>
        {
            //fill in model values from the entity
            return orders.SelectAwait(async order =>
            {
                var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
				_logger.LogDebug("PrepareOrderListModelAsync(searchModel={SearchModel}): billingAddress → {BillingAddress}", searchModel, billingAddress);

                //fill in model values from the entity
                var orderModel = new OrderModel
                {
                    Id = order.Id,
                    OrderStatusId = order.OrderStatusId,
                    PaymentStatusId = order.PaymentStatusId,
                    ShippingStatusId = order.ShippingStatusId,
                    CustomerEmail = billingAddress.Email,
                    CustomerFullName = $"{billingAddress.FirstName} {billingAddress.LastName}",
                    CustomerId = order.CustomerId,
                    CustomOrderNumber = order.CustomOrderNumber
                };

                //convert dates to the user time
                orderModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                orderModel.StoreName = (await _storeService.GetStoreByIdAsync(order.StoreId))?.Name ?? "Deleted";
                orderModel.OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus);
                orderModel.PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);
                orderModel.ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus);
                orderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(order.OrderTotal, true, false);

                return orderModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareOrderListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare order aggregator model
    /// </summary>
    /// <param name="searchModel">Order search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order aggregator model
    /// </returns>
    public virtual async Task<OrderAggreratorModel> PrepareOrderAggregatorModelAsync(OrderSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareOrderAggregatorModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        if (!_orderSettings.DisplayOrderSummary)
            return null;

        //get parameters to filter orders
        var orderStatusIds = (searchModel.OrderStatusIds?.Contains(0) ?? true) ? null : searchModel.OrderStatusIds.ToList();
		_logger.LogDebug("PrepareOrderAggregatorModelAsync(searchModel={SearchModel}): orderStatusIds → {OrderStatusIds}", searchModel, orderStatusIds);
        var paymentStatusIds = (searchModel.PaymentStatusIds?.Contains(0) ?? true) ? null : searchModel.PaymentStatusIds.ToList();
		_logger.LogDebug("PrepareOrderAggregatorModelAsync(searchModel={SearchModel}): paymentStatusIds → {PaymentStatusIds}", searchModel, paymentStatusIds);
        var shippingStatusIds = (searchModel.ShippingStatusIds?.Contains(0) ?? true) ? null : searchModel.ShippingStatusIds.ToList();
		_logger.LogDebug("PrepareOrderAggregatorModelAsync(searchModel={SearchModel}): shippingStatusIds → {ShippingStatusIds}", searchModel, shippingStatusIds);
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("PrepareOrderAggregatorModelAsync(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
        if (currentVendor != null)
            searchModel.VendorId = currentVendor.Id;
			_logger.LogDebug("PrepareOrderAggregatorModelAsync(searchModel={SearchModel}): searchModel.VendorId → {Arg1}", searchModel, searchModel.VendorId);
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var product = await _productService.GetProductByIdAsync(searchModel.ProductId);
		_logger.LogDebug("PrepareOrderAggregatorModelAsync(searchModel={SearchModel}): product → {Product}", searchModel, product);
        var filterByProductId = product != null && (currentVendor == null || product.VendorId == currentVendor.Id)
            ? searchModel.ProductId : 0;

        //prepare additional model data
        var reportSummary = await _orderReportService.GetOrderAverageReportLineAsync(storeId: searchModel.StoreId,
            vendorId: searchModel.VendorId,
            productId: filterByProductId,
            warehouseId: searchModel.WarehouseId,
            paymentMethodSystemName: searchModel.PaymentMethodSystemName,
            osIds: orderStatusIds,
            psIds: paymentStatusIds,
            ssIds: shippingStatusIds,
            startTimeUtc: startDateValue,
            endTimeUtc: endDateValue,
            billingPhone: searchModel.BillingPhone,
            billingEmail: searchModel.BillingEmail,
            billingLastName: searchModel.BillingLastName,
            billingCountryId: searchModel.BillingCountryId,
            orderNotes: searchModel.OrderNotes);

        var profit = await _orderReportService.ProfitReportAsync(storeId: searchModel.StoreId,
            vendorId: searchModel.VendorId,
            productId: filterByProductId,
            warehouseId: searchModel.WarehouseId,
            paymentMethodSystemName: searchModel.PaymentMethodSystemName,
            osIds: orderStatusIds,
            psIds: paymentStatusIds,
            ssIds: shippingStatusIds,
            startTimeUtc: startDateValue,
            endTimeUtc: endDateValue,
            billingPhone: searchModel.BillingPhone,
            billingEmail: searchModel.BillingEmail,
            billingLastName: searchModel.BillingLastName,
            billingCountryId: searchModel.BillingCountryId,
            orderNotes: searchModel.OrderNotes);

        var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
		_logger.LogDebug("PrepareOrderAggregatorModelAsync(searchModel={SearchModel}): primaryStoreCurrency → {PrimaryStoreCurrency}", searchModel, primaryStoreCurrency);
        var shippingSum = await _priceFormatter
            .FormatShippingPriceAsync(reportSummary.SumShippingExclTax, true, primaryStoreCurrency, (await _workContext.GetWorkingLanguageAsync()).Id, false);
        var taxSum = await _priceFormatter.FormatPriceAsync(reportSummary.SumTax, true, false);
		_logger.LogDebug("PrepareOrderAggregatorModelAsync(searchModel={SearchModel}): taxSum → {TaxSum}", searchModel, taxSum);
        var totalSum = await _priceFormatter.FormatPriceAsync(reportSummary.SumOrders, true, false);
		_logger.LogDebug("PrepareOrderAggregatorModelAsync(searchModel={SearchModel}): totalSum → {TotalSum}", searchModel, totalSum);
        var profitSum = await _priceFormatter.FormatPriceAsync(profit, true, false);
		_logger.LogDebug("PrepareOrderAggregatorModelAsync(searchModel={SearchModel}): profitSum → {ProfitSum}", searchModel, profitSum);

        var model = new OrderAggreratorModel
        {
            AggregatorProfit = profitSum,
            AggregatorShipping = shippingSum,
            AggregatorTax = taxSum,
            AggregatorTotal = totalSum
        };

		_logger.LogDebug("<<< Exiting PrepareOrderAggregatorModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare order model
    /// </summary>
    /// <param name="model">Order model</param>
    /// <param name="order">Order</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order model
    /// </returns>
    public virtual async Task<OrderModel> PrepareOrderModelAsync(OrderModel model, Order order, bool excludeProperties = false)
    {
		_logger.LogDebug(">>> Entering PrepareOrderModelAsync(model={Model},order={Order},excludeProperties={ExcludeProperties})", model, order, excludeProperties);
        if (order != null)
        {
            //fill in model values from the entity
            model ??= new OrderModel
            {
                Id = order.Id,
                OrderStatusId = order.OrderStatusId,
                VatNumber = order.VatNumber,
                CheckoutAttributeInfo = order.CheckoutAttributeDescription
            };

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
			_logger.LogDebug("PrepareOrderModelAsync(model={Model},order={Order},excludeProperties={ExcludeProperties}): customer → {Customer}", model, order, excludeProperties, customer);

            model.OrderGuid = order.OrderGuid;
			_logger.LogDebug("PrepareOrderModelAsync(model={Model},order={Order},excludeProperties={ExcludeProperties}): model.OrderGuid → {Arg3}", model, order, excludeProperties, model.OrderGuid);
			_logger.LogDebug("PrepareOrderModelAsync(model={Model},order={Order},excludeProperties={ExcludeProperties}): model.CustomOrderNumber → {Arg3}", model, order, excludeProperties, model.CustomOrderNumber);
			_logger.LogDebug("PrepareOrderModelAsync(model={Model},order={Order},excludeProperties={ExcludeProperties}): model.CustomerIp → {Arg3}", model, order, excludeProperties, model.CustomerIp);
			_logger.LogDebug("PrepareOrderModelAsync(model={Model},order={Order},excludeProperties={ExcludeProperties}): model.CustomerId → {Arg3}", model, order, excludeProperties, model.CustomerId);
            model.CustomOrderNumber = order.CustomOrderNumber;
            model.CustomerIp = order.CustomerIp;
            model.CustomerId = customer.Id;
            model.OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus);
            model.StoreName = (await _storeService.GetStoreByIdAsync(order.StoreId))?.Name ?? "Deleted";
            model.CustomerInfo = await _customerService.IsRegisteredAsync(customer) ? customer.Email : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
            model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);
            model.CustomValues.FillByXml(order.CustomValuesXml);

            var affiliate = await _affiliateService.GetAffiliateByIdAsync(order.AffiliateId);
			_logger.LogDebug("PrepareOrderModelAsync(model={Model},order={Order},excludeProperties={ExcludeProperties}): affiliate → {Affiliate}", model, order, excludeProperties, affiliate);
            if (affiliate != null)
            {
                model.AffiliateId = affiliate.Id;
				_logger.LogDebug("PrepareOrderModelAsync(model={Model},order={Order},excludeProperties={ExcludeProperties}): model.AffiliateId → {Arg3}", model, order, excludeProperties, model.AffiliateId);
                model.AffiliateName = await _affiliateService.GetAffiliateFullNameAsync(affiliate);
            }

            //prepare order totals
            await PrepareOrderModelTotalsAsync(model, order);

            //prepare order items
            await PrepareOrderItemModelsAsync(model.Items, order);
            model.HasDownloadableProducts = model.Items.Any(item => item.IsDownload);

            //prepare payment info
            await PrepareOrderModelPaymentInfoAsync(model, order);

            //prepare shipping info
            await PrepareOrderModelShippingInfoAsync(model, order);

            //prepare nested search model
            PrepareOrderShipmentSearchModel(model.OrderShipmentSearchModel, order);
            PrepareOrderNoteSearchModel(model.OrderNoteSearchModel, order);
        }

        model.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;
        model.AllowCustomersToSelectTaxDisplayType = _taxSettings.AllowCustomersToSelectTaxDisplayType;
		_logger.LogDebug("PrepareOrderModelAsync(model={Model},order={Order},excludeProperties={ExcludeProperties}): model.AllowCustomersToSelectTaxDisplayType → {Arg3}", model, order, excludeProperties, model.AllowCustomersToSelectTaxDisplayType);
		_logger.LogDebug("PrepareOrderModelAsync(model={Model},order={Order},excludeProperties={ExcludeProperties}): model.TaxDisplayType → {Arg3}", model, order, excludeProperties, model.TaxDisplayType);
        model.TaxDisplayType = _taxSettings.TaxDisplayType;

		_logger.LogDebug("<<< Exiting PrepareOrderModelAsync(model={Model},order={Order},excludeProperties={ExcludeProperties})", model, order, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare upload license model
    /// </summary>
    /// <param name="model">Upload license model</param>
    /// <param name="order">Order</param>
    /// <param name="orderItem">Order item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the upload license model
    /// </returns>
    public virtual Task<UploadLicenseModel> PrepareUploadLicenseModelAsync(UploadLicenseModel model, Order order, OrderItem orderItem)
    {
		_logger.LogDebug(">>> Entering PrepareUploadLicenseModelAsync(model={Model},order={Order},orderItem={OrderItem})", model, order, orderItem);
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(order);

        ArgumentNullException.ThrowIfNull(orderItem);

        model.LicenseDownloadId = orderItem.LicenseDownloadId ?? 0;
		_logger.LogDebug("PrepareUploadLicenseModelAsync(model={Model},order={Order},orderItem={OrderItem}): model.LicenseDownloadId → {Arg3}", model, order, orderItem, model.LicenseDownloadId);
		_logger.LogDebug("PrepareUploadLicenseModelAsync(model={Model},order={Order},orderItem={OrderItem}): model.OrderId → {Arg3}", model, order, orderItem, model.OrderId);
		_logger.LogDebug("PrepareUploadLicenseModelAsync(model={Model},order={Order},orderItem={OrderItem}): model.OrderItemId → {Arg3}", model, order, orderItem, model.OrderItemId);
        model.OrderId = order.Id;
        model.OrderItemId = orderItem.Id;

		_logger.LogDebug("<<< Exiting PrepareUploadLicenseModelAsync(model={Model},order={Order},orderItem={OrderItem})", model, order, orderItem);
        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare product search model to add to the order
    /// </summary>
    /// <param name="searchModel">Product search model to add to the order</param>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product search model to add to the order
    /// </returns>
    public virtual async Task<AddProductToOrderSearchModel> PrepareAddProductToOrderSearchModelAsync(AddProductToOrderSearchModel searchModel, Order order)
    {
		_logger.LogDebug(">>> Entering PrepareAddProductToOrderSearchModelAsync(searchModel={SearchModel},order={Order})", searchModel, order);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(order);

        searchModel.OrderId = order.Id;
		_logger.LogDebug("PrepareAddProductToOrderSearchModelAsync(searchModel={SearchModel},order={Order}): searchModel.OrderId → {Arg2}", searchModel, order, searchModel.OrderId);

        //prepare available categories
        await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

        //prepare available manufacturers
        await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

        //prepare available product types
        await _baseAdminModelFactory.PrepareProductTypesAsync(searchModel.AvailableProductTypes);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareAddProductToOrderSearchModelAsync(searchModel={SearchModel},order={Order})", searchModel, order);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged product list model to add to the order
    /// </summary>
    /// <param name="searchModel">Product search model to add to the order</param>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product search model to add to the order
    /// </returns>
    public virtual async Task<AddProductToOrderListModel> PrepareAddProductToOrderListModelAsync(AddProductToOrderSearchModel searchModel, Order order)
    {
		_logger.LogDebug(">>> Entering PrepareAddProductToOrderListModelAsync(searchModel={SearchModel},order={Order})", searchModel, order);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get products
        var products = await _productService.SearchProductsAsync(showHidden: true,
            categoryIds: new List<int> { searchModel.SearchCategoryId },
            manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
            productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
            keywords: searchModel.SearchProductName,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new AddProductToOrderListModel().PrepareToGridAsync(searchModel, products, () =>
        {
            //fill in model values from the entity
            return products.SelectAwait(async product =>
            {
                var productModel = product.ToModel<ProductModel>();
				_logger.LogDebug("PrepareAddProductToOrderListModelAsync(searchModel={SearchModel},order={Order}): productModel → {ProductModel}", searchModel, order, productModel);

                productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);

                return productModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareAddProductToOrderListModelAsync(searchModel={SearchModel},order={Order})", searchModel, order);
        return model;
    }

    /// <summary>
    /// Prepare product model to add to the order
    /// </summary>
    /// <param name="model">Product model to add to the order</param>
    /// <param name="order">Order</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product model to add to the order
    /// </returns>
    public virtual async Task<AddProductToOrderModel> PrepareAddProductToOrderModelAsync(AddProductToOrderModel model, Order order, Product product)
    {
		_logger.LogDebug(">>> Entering PrepareAddProductToOrderModelAsync(model={Model},order={Order},product={Product})", model, order, product);
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(order);

        ArgumentNullException.ThrowIfNull(product);

        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
		_logger.LogDebug("PrepareAddProductToOrderModelAsync(model={Model},order={Order},product={Product}): customer → {Customer}", model, order, product, customer);
        var store = await _storeService.GetStoreByIdAsync(order.StoreId);
		_logger.LogDebug("PrepareAddProductToOrderModelAsync(model={Model},order={Order},product={Product}): store → {Store}", model, order, product, store);

        model.ProductId = product.Id;
		_logger.LogDebug("PrepareAddProductToOrderModelAsync(model={Model},order={Order},product={Product}): model.ProductId → {Arg3}", model, order, product, model.ProductId);
		_logger.LogDebug("PrepareAddProductToOrderModelAsync(model={Model},order={Order},product={Product}): model.OrderId → {Arg3}", model, order, product, model.OrderId);
		_logger.LogDebug("PrepareAddProductToOrderModelAsync(model={Model},order={Order},product={Product}): model.Name → {Arg3}", model, order, product, model.Name);
		_logger.LogDebug("PrepareAddProductToOrderModelAsync(model={Model},order={Order},product={Product}): model.IsRental → {Arg3}", model, order, product, model.IsRental);
		_logger.LogDebug("PrepareAddProductToOrderModelAsync(model={Model},order={Order},product={Product}): model.ProductType → {Arg3}", model, order, product, model.ProductType);
		_logger.LogDebug("PrepareAddProductToOrderModelAsync(model={Model},order={Order},product={Product}): model.AutoUpdateOrderTotals → {Arg3}", model, order, product, model.AutoUpdateOrderTotals);
        model.OrderId = order.Id;
        model.Name = product.Name;
        model.IsRental = product.IsRental;
        model.ProductType = product.ProductType;
        model.AutoUpdateOrderTotals = _orderSettings.AutoUpdateOrderTotalsOnEditingOrder;

        var presetQty = 1;
        var (_, presetPrice, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, store, decimal.Zero, true, presetQty);

        var (presetPriceInclTax, _) = await _taxService.GetProductPriceAsync(product, presetPrice, true, customer);
        var (presetPriceExclTax, _) = await _taxService.GetProductPriceAsync(product, presetPrice, false, customer);
        model.UnitPriceExclTax = presetPriceExclTax;
		_logger.LogDebug("PrepareAddProductToOrderModelAsync(model={Model},order={Order},product={Product}): model.UnitPriceExclTax → {Arg3}", model, order, product, model.UnitPriceExclTax);
		_logger.LogDebug("PrepareAddProductToOrderModelAsync(model={Model},order={Order},product={Product}): model.UnitPriceInclTax → {Arg3}", model, order, product, model.UnitPriceInclTax);
		_logger.LogDebug("PrepareAddProductToOrderModelAsync(model={Model},order={Order},product={Product}): model.Quantity → {Arg3}", model, order, product, model.Quantity);
		_logger.LogDebug("PrepareAddProductToOrderModelAsync(model={Model},order={Order},product={Product}): model.SubTotalExclTax → {Arg3}", model, order, product, model.SubTotalExclTax);
		_logger.LogDebug("PrepareAddProductToOrderModelAsync(model={Model},order={Order},product={Product}): model.SubTotalInclTax → {Arg3}", model, order, product, model.SubTotalInclTax);
        model.UnitPriceInclTax = presetPriceInclTax;
        model.Quantity = presetQty;
        model.SubTotalExclTax = presetPriceExclTax;
        model.SubTotalInclTax = presetPriceInclTax;

        //attributes
        await PrepareProductAttributeModelsAsync(model.ProductAttributes, order, product);
        model.HasCondition = model.ProductAttributes.Any(attribute => attribute.HasCondition);

        //gift card
        model.GiftCard.IsGiftCard = product.IsGiftCard;
        if (model.GiftCard.IsGiftCard)
            model.GiftCard.GiftCardType = product.GiftCardType;

		_logger.LogDebug("<<< Exiting PrepareAddProductToOrderModelAsync(model={Model},order={Order},product={Product})", model, order, product);
        return model;
    }

    /// <summary>
    /// Prepare order address model
    /// </summary>
    /// <param name="model">Order address model</param>
    /// <param name="order">Order</param>
    /// <param name="address">Address</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order address model
    /// </returns>
    public virtual async Task<OrderAddressModel> PrepareOrderAddressModelAsync(OrderAddressModel model, Order order, Address address)
    {
		_logger.LogDebug(">>> Entering PrepareOrderAddressModelAsync(model={Model},order={Order},address={Address})", model, order, address);
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(order);

        ArgumentNullException.ThrowIfNull(address);

        model.OrderId = order.Id;
		_logger.LogDebug("PrepareOrderAddressModelAsync(model={Model},order={Order},address={Address}): model.OrderId → {Arg3}", model, order, address, model.OrderId);

        //prepare address model
        model.Address = address.ToModel(model.Address);
        await _addressModelFactory.PrepareAddressModelAsync(model.Address, address);
        SetAddressFieldsAsRequired(model.Address);

		_logger.LogDebug("<<< Exiting PrepareOrderAddressModelAsync(model={Model},order={Order},address={Address})", model, order, address);
        return model;
    }

    /// <summary>
    /// Prepare shipment search model
    /// </summary>
    /// <param name="searchModel">Shipment search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment search model
    /// </returns>
    public virtual async Task<ShipmentSearchModel> PrepareShipmentSearchModelAsync(ShipmentSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareShipmentSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available countries
        await _baseAdminModelFactory.PrepareCountriesAsync(searchModel.AvailableCountries);

        //prepare available states and provinces
        await _baseAdminModelFactory.PrepareStatesAndProvincesAsync(searchModel.AvailableStates, searchModel.CountryId);

        //prepare available warehouses
        await _baseAdminModelFactory.PrepareWarehousesAsync(searchModel.AvailableWarehouses);

        //prepare nested search model
        PrepareShipmentItemSearchModel(searchModel.ShipmentItemSearchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareShipmentSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged shipment list model
    /// </summary>
    /// <param name="searchModel">Shipment search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment list model
    /// </returns>
    public virtual async Task<ShipmentListModel> PrepareShipmentListModelAsync(ShipmentSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareShipmentListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter shipments
        var vendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("PrepareShipmentListModelAsync(searchModel={SearchModel}): vendor → {Vendor}", searchModel, vendor);
        var vendorId = vendor?.Id ?? 0;
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //get shipments
        var shipments = await _shipmentService.GetAllShipmentsAsync(vendorId,
            searchModel.WarehouseId,
            searchModel.CountryId,
            searchModel.StateProvinceId,
            searchModel.County,
            searchModel.City,
            searchModel.TrackingNumber,
            searchModel.LoadNotShipped,
            searchModel.LoadNotReadyForPickup,
            searchModel.LoadNotDelivered,
            0,
            startDateValue,
            endDateValue,
            searchModel.Page - 1,
            searchModel.PageSize);

        //prepare list model
        var model = await new ShipmentListModel().PrepareToGridAsync(searchModel, shipments, () =>
        {
            //fill in model values from the entity
			_logger.LogDebug("PrepareShipmentListModelAsync(searchModel={SearchModel}): shipment → {Shipment}", searchModel, shipment);
		_logger.LogDebug("<<< Exiting PrepareShipmentListModelAsync(searchModel={SearchModel})", searchModel);
            return shipments.SelectAwait(async shipment => await PrepareShipmentModelAsync(shipment));
        });

        return model;
    }

    /// <summary>
    /// Prepare shipment model
    /// </summary>
    /// <param name="model">Shipment model</param>
    /// <param name="shipment">Shipment</param>
    /// <param name="order">Order</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment model
    /// </returns>
    public virtual async Task<ShipmentModel> PrepareShipmentModelAsync(ShipmentModel model, Shipment shipment, Order order,
        bool excludeProperties = false)
    {
        if (shipment != null)
        {
            //fill in model values from the entity
            model = await PrepareShipmentModelAsync(shipment, model);

            //prepare shipment items
            foreach (var item in await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id))
            {
                var orderItem = await _orderService.GetOrderItemByIdAsync(item.OrderItemId);
                if (orderItem == null)
                    continue;

                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                //fill in model values from the entity
                var shipmentItemModel = new ShipmentItemModel
                {
                    Id = item.Id,
                    QuantityInThisShipment = item.Quantity,
                    ShippedFromWarehouse = (await _warehouseService.GetWarehouseByIdAsync(item.WarehouseId))?.Name
                };

                await PrepareShipmentItemModelAsync(shipmentItemModel, orderItem, product);

                model.Items.Add(shipmentItemModel);
            }

            //prepare shipment events
            if (!string.IsNullOrEmpty(shipment.TrackingNumber))
            {
                var shipmentTracker = await _shipmentService.GetShipmentTrackerAsync(shipment);
                if (shipmentTracker != null)
                {
                    model.TrackingNumberUrl = await shipmentTracker.GetUrlAsync(shipment.TrackingNumber, shipment);
                    if (_shippingSettings.DisplayShipmentEventsToStoreOwner)
                        await PrepareShipmentStatusEventModelsAsync(model.ShipmentStatusEvents, shipment);
                }
            }
        }

        if (shipment != null)
            return model;

        model.OrderId = order.Id;
        model.PickupInStore = order.PickupInStore;
        model.CustomOrderNumber = order.CustomOrderNumber;

        var vendor = await _workContext.GetCurrentVendorAsync();
        var orderItems = (await _orderService.GetOrderItemsAsync(order.Id, isShipEnabled: true, vendorId: vendor?.Id ?? 0)).ToList();

        foreach (var orderItem in orderItems)
        {
            var shipmentItemModel = new ShipmentItemModel();

            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

            await PrepareShipmentItemModelAsync(shipmentItemModel, orderItem, product);

            //ensure that this product can be added to a shipment
            if (shipmentItemModel.QuantityToAdd <= 0)
                continue;

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.UseMultipleWarehouses)
            {
                //multiple warehouses supported
                shipmentItemModel.AllowToChooseWarehouse = true;
                foreach (var pwi in (await _productService.GetAllProductWarehouseInventoryRecordsAsync(orderItem.ProductId)).OrderBy(w => w.WarehouseId).ToList())
                {
                    if (await _warehouseService.GetWarehouseByIdAsync(pwi.WarehouseId) is Warehouse warehouse)
                    {
                        shipmentItemModel.AvailableWarehouses.Add(new ShipmentItemModel.WarehouseInfo
                        {
                            WarehouseId = warehouse.Id,
                            WarehouseName = warehouse.Name,
                            StockQuantity = pwi.StockQuantity,
                            ReservedQuantity = pwi.ReservedQuantity,
                            PlannedQuantity =
                                await _shipmentService.GetQuantityInShipmentsAsync(product, warehouse.Id, true, true)
                        });
                    }
                }
            }
            else
            {
                //multiple warehouses are not supported
                var warehouse = await _warehouseService.GetWarehouseByIdAsync(product.WarehouseId);
                if (warehouse != null)
                {
                    shipmentItemModel.AvailableWarehouses.Add(new ShipmentItemModel.WarehouseInfo
                    {
                        WarehouseId = warehouse.Id,
                        WarehouseName = warehouse.Name,
                        StockQuantity = product.StockQuantity
                    });
                }
            }

            model.Items.Add(shipmentItemModel);
        }

        return model;
    }

    /// <summary>
    /// Prepare paged order shipment list model
    /// </summary>
    /// <param name="searchModel">Order shipment search model</param>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order shipment list model
    /// </returns>
    public virtual async Task<OrderShipmentListModel> PrepareOrderShipmentListModelAsync(OrderShipmentSearchModel searchModel, Order order)
    {
		_logger.LogDebug(">>> Entering PrepareOrderShipmentListModelAsync(searchModel={SearchModel},order={Order})", searchModel, order);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(order);

        var vendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("PrepareOrderShipmentListModelAsync(searchModel={SearchModel},order={Order}): vendor → {Vendor}", searchModel, order, vendor);
        //get shipments
        var shipments = (await _shipmentService.GetAllShipmentsAsync(
                orderId: order.Id,
                //a vendor should have access only to his products
                vendorId: vendor?.Id ?? 0))
            .OrderBy(shipment => shipment.CreatedOnUtc)
            .ToList();

        var pagedShipments = shipments.ToPagedList(searchModel);
		_logger.LogDebug("PrepareOrderShipmentListModelAsync(searchModel={SearchModel},order={Order}): pagedShipments → {PagedShipments}", searchModel, order, pagedShipments);

        //prepare list model
        var model = await new OrderShipmentListModel().PrepareToGridAsync(searchModel, pagedShipments, () =>
        {
            //fill in model values from the entity
			_logger.LogDebug("PrepareOrderShipmentListModelAsync(searchModel={SearchModel},order={Order}): shipment → {Shipment}", searchModel, order, shipment);
		_logger.LogDebug("<<< Exiting PrepareOrderShipmentListModelAsync(searchModel={SearchModel},order={Order})", searchModel, order);
            return pagedShipments.SelectAwait(async shipment => await PrepareShipmentModelAsync(shipment));
        });

        return model;
    }

    /// <summary>
    /// Prepare paged shipment item list model
    /// </summary>
    /// <param name="searchModel">Shipment item search model</param>
    /// <param name="shipment">Shipment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment item list model
    /// </returns>
    public virtual async Task<ShipmentItemListModel> PrepareShipmentItemListModelAsync(ShipmentItemSearchModel searchModel, Shipment shipment)
    {
		_logger.LogDebug(">>> Entering PrepareShipmentItemListModelAsync(searchModel={SearchModel},shipment={Shipment})", searchModel, shipment);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(shipment);

        //get shipments
        var shipmentItems = (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).ToPagedList(searchModel);
		_logger.LogDebug("PrepareShipmentItemListModelAsync(searchModel={SearchModel},shipment={Shipment}): shipmentItems → {ShipmentItems}", searchModel, shipment, shipmentItems);

        //prepare list model
        var model = await new ShipmentItemListModel().PrepareToGridAsync(searchModel, shipmentItems, () =>
        {
            //fill in model values from the entity
            return shipmentItems.SelectAwait(async item =>
            {
                //fill in model values from the entity
                var shipmentItemModel = new ShipmentItemModel
                {
                    Id = item.Id,
                    QuantityInThisShipment = item.Quantity
                };

                //fill in additional values (not existing in the entity)
                var orderItem = await _orderService.GetOrderItemByIdAsync(item.OrderItemId);
				_logger.LogDebug("PrepareShipmentItemListModelAsync(searchModel={SearchModel},shipment={Shipment}): orderItem → {OrderItem}", searchModel, shipment, orderItem);
                if (orderItem == null)
                    return shipmentItemModel;

                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
				_logger.LogDebug("PrepareShipmentItemListModelAsync(searchModel={SearchModel},shipment={Shipment}): product → {Product}", searchModel, shipment, product);

                shipmentItemModel.OrderItemId = orderItem.Id;
				_logger.LogDebug("PrepareShipmentItemListModelAsync(searchModel={SearchModel},shipment={Shipment}): shipmentItemModel.OrderItemId → {Arg2}", searchModel, shipment, shipmentItemModel.OrderItemId);
				_logger.LogDebug("PrepareShipmentItemListModelAsync(searchModel={SearchModel},shipment={Shipment}): shipmentItemModel.ProductId → {Arg2}", searchModel, shipment, shipmentItemModel.ProductId);
				_logger.LogDebug("PrepareShipmentItemListModelAsync(searchModel={SearchModel},shipment={Shipment}): shipmentItemModel.ProductName → {Arg2}", searchModel, shipment, shipmentItemModel.ProductName);
                shipmentItemModel.ProductId = orderItem.ProductId;
                shipmentItemModel.ProductName = product.Name;

                shipmentItemModel.ShippedFromWarehouse = (await _warehouseService.GetWarehouseByIdAsync(item.WarehouseId))?.Name;

                var baseWeight = (await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId))?.Name;
				_logger.LogDebug("PrepareShipmentItemListModelAsync(searchModel={SearchModel},shipment={Shipment}): baseWeight → {BaseWeight}", searchModel, shipment, baseWeight);
                var baseDimension = (await _measureService.GetMeasureDimensionByIdAsync(_measureSettings.BaseDimensionId))?.Name;
				_logger.LogDebug("PrepareShipmentItemListModelAsync(searchModel={SearchModel},shipment={Shipment}): baseDimension → {BaseDimension}", searchModel, shipment, baseDimension);

                if (orderItem.ItemWeight.HasValue)
                    shipmentItemModel.ItemWeight = $"{orderItem.ItemWeight:F2} [{baseWeight}]";
					_logger.LogDebug("PrepareShipmentItemListModelAsync(searchModel={SearchModel},shipment={Shipment}): shipmentItemModel.ItemWeight → {Arg2}", searchModel, shipment, shipmentItemModel.ItemWeight);

                shipmentItemModel.ItemDimensions =
                    $"{product.Length:F2} x {product.Width:F2} x {product.Height:F2} [{baseDimension}]";

                return shipmentItemModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareShipmentItemListModelAsync(searchModel={SearchModel},shipment={Shipment})", searchModel, shipment);
        return model;
    }

    /// <summary>
    /// Prepare paged order note list model
    /// </summary>
    /// <param name="searchModel">Order note search model</param>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order note list model
    /// </returns>
    public virtual async Task<OrderNoteListModel> PrepareOrderNoteListModelAsync(OrderNoteSearchModel searchModel, Order order)
    {
		_logger.LogDebug(">>> Entering PrepareOrderNoteListModelAsync(searchModel={SearchModel},order={Order})", searchModel, order);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(order);

        //get notes
        var orderNotes = (await _orderService.GetOrderNotesByOrderIdAsync(order.Id)).OrderByDescending(on => on.CreatedOnUtc).ToList().ToPagedList(searchModel);
		_logger.LogDebug("PrepareOrderNoteListModelAsync(searchModel={SearchModel},order={Order}): orderNotes → {OrderNotes}", searchModel, order, orderNotes);

        //prepare list model
        var model = await new OrderNoteListModel().PrepareToGridAsync(searchModel, orderNotes, () =>
        {
            //fill in model values from the entity
            return orderNotes.SelectAwait(async orderNote =>
            {
                //fill in model values from the entity
                var orderNoteModel = orderNote.ToModel<OrderNoteModel>();
				_logger.LogDebug("PrepareOrderNoteListModelAsync(searchModel={SearchModel},order={Order}): orderNoteModel → {OrderNoteModel}", searchModel, order, orderNoteModel);

                //convert dates to the user time
                orderNoteModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(orderNote.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                orderNoteModel.Note = _orderService.FormatOrderNoteText(orderNote);

                orderNoteModel.DownloadGuid = (await _downloadService.GetDownloadByIdAsync(orderNote.DownloadId))?.DownloadGuid ?? Guid.Empty;

                return orderNoteModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareOrderNoteListModelAsync(searchModel={SearchModel},order={Order})", searchModel, order);
        return model;
    }

    /// <summary>
    /// Prepare bestseller brief search model
    /// </summary>
    /// <param name="searchModel">Bestseller brief search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the bestseller brief search model
    /// </returns>
    public virtual Task<BestsellerBriefSearchModel> PrepareBestsellerBriefSearchModelAsync(BestsellerBriefSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareBestsellerBriefSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize(5);

		_logger.LogDebug("<<< Exiting PrepareBestsellerBriefSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged bestseller brief list model
    /// </summary>
    /// <param name="searchModel">Bestseller brief search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the bestseller brief list model
    /// </returns>
    public virtual async Task<BestsellerBriefListModel> PrepareBestsellerBriefListModelAsync(BestsellerBriefSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareBestsellerBriefListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        var vendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("PrepareBestsellerBriefListModelAsync(searchModel={SearchModel}): vendor → {Vendor}", searchModel, vendor);
        //get bestsellers
        var bestsellers = await _orderReportService.BestSellersReportAsync(showHidden: true,
            vendorId: vendor?.Id ?? 0,
            orderBy: searchModel.OrderBy,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new BestsellerBriefListModel().PrepareToGridAsync(searchModel, bestsellers, () =>
        {
            //fill in model values from the entity
            return bestsellers.SelectAwait(async bestseller =>
            {
                //fill in model values from the entity
                var bestsellerModel = new BestsellerModel
                {
                    ProductId = bestseller.ProductId,
                    TotalQuantity = bestseller.TotalQuantity,
                    ProductName = bestseller.ProductName
                };

                //fill in additional values (not existing in the entity)
                bestsellerModel.TotalAmount = await _priceFormatter.FormatPriceAsync(bestseller.TotalAmount, true, false);

                return bestsellerModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareBestsellerBriefListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare order average line summary report list model
    /// </summary>
    /// <param name="searchModel">Order average line summary report search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order average line summary report list model
    /// </returns>
    public virtual async Task<OrderAverageReportListModel> PrepareOrderAverageReportListModelAsync(OrderAverageReportSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareOrderAverageReportListModelAsync(searchModel={SearchModel})", searchModel);
        //get report
        var report = new List<OrderAverageReportLineSummary>
        {
            await _orderReportService.OrderAverageReportAsync(0, OrderStatus.Pending),
            await _orderReportService.OrderAverageReportAsync(0, OrderStatus.Processing),
            await _orderReportService.OrderAverageReportAsync(0, OrderStatus.Complete),
            await _orderReportService.OrderAverageReportAsync(0, OrderStatus.Cancelled)
        };

        var pagedList = new PagedList<OrderAverageReportLineSummary>(report, 0, int.MaxValue);
		_logger.LogDebug("PrepareOrderAverageReportListModelAsync(searchModel={SearchModel}): pagedList → {PagedList}", searchModel, pagedList);

        //prepare list model
        var model = await new OrderAverageReportListModel().PrepareToGridAsync(searchModel, pagedList, () =>
        {
            //fill in model values from the entity
            return pagedList.SelectAwait(async reportItem => new OrderAverageReportModel
            {
                OrderStatus = await _localizationService.GetLocalizedEnumAsync(reportItem.OrderStatus),
                SumTodayOrders = await _priceFormatter.FormatPriceAsync(reportItem.SumTodayOrders, true, false),
                SumThisWeekOrders = await _priceFormatter.FormatPriceAsync(reportItem.SumThisWeekOrders, true, false),
                SumThisMonthOrders = await _priceFormatter.FormatPriceAsync(reportItem.SumThisMonthOrders, true, false),
                SumThisYearOrders = await _priceFormatter.FormatPriceAsync(reportItem.SumThisYearOrders, true, false),
                SumAllTimeOrders = await _priceFormatter.FormatPriceAsync(reportItem.SumAllTimeOrders, true, false)
            });
        });

		_logger.LogDebug("<<< Exiting PrepareOrderAverageReportListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare incomplete order report list model
    /// </summary>
    /// <param name="searchModel">Incomplete order report search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the incomplete order report list model
    /// </returns>
    public virtual async Task<OrderIncompleteReportListModel> PrepareOrderIncompleteReportListModelAsync(OrderIncompleteReportSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareOrderIncompleteReportListModelAsync(searchModel={SearchModel})", searchModel);
        var orderIncompleteReportModels = new List<OrderIncompleteReportModel>();

		_logger.LogDebug("PrepareOrderIncompleteReportListModelAsync(searchModel={SearchModel}): orderIncompleteReportModels → {OrderIncompleteReportModels}", searchModel, orderIncompleteReportModels);
        //get URL helper
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
		_logger.LogDebug("PrepareOrderIncompleteReportListModelAsync(searchModel={SearchModel}): urlHelper → {UrlHelper}", searchModel, urlHelper);

        //not paid
        var orderStatuses = Enum.GetValues(typeof(OrderStatus)).Cast<int>().Where(os => os != (int)OrderStatus.Cancelled).ToList();
		_logger.LogDebug("PrepareOrderIncompleteReportListModelAsync(searchModel={SearchModel}): orderStatuses → {OrderStatuses}", searchModel, orderStatuses);
        var paymentStatuses = new List<int> { (int)PaymentStatus.Pending };
		_logger.LogDebug("PrepareOrderIncompleteReportListModelAsync(searchModel={SearchModel}): paymentStatuses → {PaymentStatuses}", searchModel, paymentStatuses);
        var psPending = await _orderReportService.GetOrderAverageReportLineAsync(psIds: paymentStatuses, osIds: orderStatuses);
		_logger.LogDebug("PrepareOrderIncompleteReportListModelAsync(searchModel={SearchModel}): psPending → {PsPending}", searchModel, psPending);
        orderIncompleteReportModels.Add(new OrderIncompleteReportModel
        {
            Item = await _localizationService.GetResourceAsync("Admin.SalesReport.Incomplete.TotalUnpaidOrders"),
            Count = psPending.CountOrders,
            Total = await _priceFormatter.FormatPriceAsync(psPending.SumOrders, true, false),
            ViewLink = urlHelper.Action("List", "Order", new
            {
                orderStatuses = string.Join(",", orderStatuses),
                paymentStatuses = string.Join(",", paymentStatuses)
            })
        });

        //not shipped
        var shippingStatuses = new List<int> { (int)ShippingStatus.NotYetShipped };
		_logger.LogDebug("PrepareOrderIncompleteReportListModelAsync(searchModel={SearchModel}): shippingStatuses → {ShippingStatuses}", searchModel, shippingStatuses);
        var ssPending = await _orderReportService.GetOrderAverageReportLineAsync(osIds: orderStatuses, ssIds: shippingStatuses);
		_logger.LogDebug("PrepareOrderIncompleteReportListModelAsync(searchModel={SearchModel}): ssPending → {SsPending}", searchModel, ssPending);
        orderIncompleteReportModels.Add(new OrderIncompleteReportModel
        {
            Item = await _localizationService.GetResourceAsync("Admin.SalesReport.Incomplete.TotalNotShippedOrders"),
            Count = ssPending.CountOrders,
            Total = await _priceFormatter.FormatPriceAsync(ssPending.SumOrders, true, false),
            ViewLink = urlHelper.Action("List", "Order", new
            {
                orderStatuses = string.Join(",", orderStatuses),
                shippingStatuses = string.Join(",", shippingStatuses)
            })
        });

        //pending
        orderStatuses = [(int)OrderStatus.Pending];
        var osPending = await _orderReportService.GetOrderAverageReportLineAsync(osIds: orderStatuses);
		_logger.LogDebug("PrepareOrderIncompleteReportListModelAsync(searchModel={SearchModel}): osPending → {OsPending}", searchModel, osPending);
        orderIncompleteReportModels.Add(new OrderIncompleteReportModel
        {
            Item = await _localizationService.GetResourceAsync("Admin.SalesReport.Incomplete.TotalIncompleteOrders"),
            Count = osPending.CountOrders,
            Total = await _priceFormatter.FormatPriceAsync(osPending.SumOrders, true, false),
            ViewLink = urlHelper.Action("List", "Order", new { orderStatuses = string.Join(",", orderStatuses) })
        });

        var pagedList = new PagedList<OrderIncompleteReportModel>(orderIncompleteReportModels, 0, int.MaxValue);
		_logger.LogDebug("PrepareOrderIncompleteReportListModelAsync(searchModel={SearchModel}): pagedList → {PagedList}", searchModel, pagedList);

        //prepare list model
        var model = new OrderIncompleteReportListModel().PrepareToGrid(searchModel, pagedList, () => pagedList);
		_logger.LogDebug("PrepareOrderIncompleteReportListModelAsync(searchModel={SearchModel}): model → {Model}", searchModel, model);
		_logger.LogDebug("<<< Exiting PrepareOrderIncompleteReportListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    #endregion
}