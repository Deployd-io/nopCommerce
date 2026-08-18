using System.Globalization;
using System.IO.Compression;
using iTextSharp.text;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common.Pdf;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using PdfRpt.Core.Contracts;

namespace Nop.Services.Common;

/// <summary>
/// PDF service
/// </summary>
public partial class PdfService : IPdfService
{
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly CurrencySettings _currencySettings;
    protected readonly IAddressService _addressService;
    protected readonly IAttributeFormatter<AddressAttribute, AddressAttributeValue> _addressAttributeFormatter;
    protected readonly ICountryService _countryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IGiftCardService _giftCardService;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IMeasureService _measureService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IOrderService _orderService;
    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IPaymentService _paymentService;
    protected readonly IPictureService _pictureService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductService _productService;
    protected readonly IRewardPointService _rewardPointService;
    protected readonly ISettingService _settingService;
    protected readonly IShipmentService _shipmentService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly IThumbService _thumbService;
    protected readonly IVendorService _vendorService;
    protected readonly IWorkContext _workContext;
    protected readonly MeasureSettings _measureSettings;
    protected readonly TaxSettings _taxSettings;
    protected readonly VendorSettings _vendorSettings;

    #endregion

    #region Ctor

    public PdfService(AddressSettings addressSettings,
        CatalogSettings catalogSettings,
        CurrencySettings currencySettings,
        IAddressService addressService,
        IAttributeFormatter<AddressAttribute, AddressAttributeValue> addressAttributeFormatter,
        ICountryService countryService,
        ICurrencyService currencyService,
        IDateTimeHelper dateTimeHelper,
        IGiftCardService giftCardService,
        IHtmlFormatter htmlFormatter,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IMeasureService measureService,
        INopFileProvider fileProvider,
        IOrderService orderService,
        IPaymentPluginManager paymentPluginManager,
        IPaymentService paymentService,
        IPictureService pictureService,
        IPriceFormatter priceFormatter,
        IProductService productService,
        IRewardPointService rewardPointService,
        ISettingService settingService,
        IShipmentService shipmentService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        IStoreService storeService,
        IThumbService thumbService,
        IVendorService vendorService,
        IWorkContext workContext,
        MeasureSettings measureSettings,
        TaxSettings taxSettings,
        VendorSettings vendorSettings)
    {
        _addressSettings = addressSettings;
        _catalogSettings = catalogSettings;
        _currencySettings = currencySettings;
        _addressService = addressService;
        _addressAttributeFormatter = addressAttributeFormatter;
        _countryService = countryService;
        _currencyService = currencyService;
        _dateTimeHelper = dateTimeHelper;
        _giftCardService = giftCardService;
        _htmlFormatter = htmlFormatter;
        _languageService = languageService;
        _localizationService = localizationService;
        _measureService = measureService;
        _fileProvider = fileProvider;
        _orderService = orderService;
        _paymentPluginManager = paymentPluginManager;
        _paymentService = paymentService;
        _pictureService = pictureService;
        _priceFormatter = priceFormatter;
        _productService = productService;
        _rewardPointService = rewardPointService;
        _settingService = settingService;
        _shipmentService = shipmentService;
        _storeContext = storeContext;
        _stateProvinceService = stateProvinceService;
        _storeService = storeService;
        _thumbService = thumbService;
        _vendorService = vendorService;
        _workContext = workContext;
        _measureSettings = measureSettings;
        _taxSettings = taxSettings;
        _vendorSettings = vendorSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get billing address
    /// </summary>
    /// <param name="vendor">Vendor</param>
    /// <param name="lang">Language</param>
    /// <param name="order">Order</param>
    /// <returns>A task that contains address item</returns>
    protected virtual async Task<AddressItem> GetBillingAddressAsync(Vendor vendor, Language lang, Order order)
    {
		_logger.LogDebug(">>> Entering GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order})", vendor, lang, order);
        var addressResult = new AddressItem();

		_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): addressResult → {AddressResult}", vendor, lang, order, addressResult);
        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
		_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): billingAddress → {BillingAddress}", vendor, lang, order, billingAddress);

        if (_addressSettings.CompanyEnabled && !string.IsNullOrEmpty(billingAddress.Company))
            addressResult.Company = billingAddress.Company;
			_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): addressResult.Company → {Arg3}", vendor, lang, order, addressResult.Company);

        addressResult.Name = $"{billingAddress.FirstName} {billingAddress.LastName}";
		_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): addressResult.Name → {Arg3}", vendor, lang, order, addressResult.Name);

        if (_addressSettings.PhoneEnabled)
            addressResult.Phone = billingAddress.PhoneNumber;
			_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): addressResult.Phone → {Arg3}", vendor, lang, order, addressResult.Phone);

        if (_addressSettings.FaxEnabled && !string.IsNullOrEmpty(billingAddress.FaxNumber))
            addressResult.Fax = billingAddress.FaxNumber;
			_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): addressResult.Fax → {Arg3}", vendor, lang, order, addressResult.Fax);

        if (_addressSettings.StreetAddressEnabled)
            addressResult.Address = billingAddress.Address1;
			_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): addressResult.Address → {Arg3}", vendor, lang, order, addressResult.Address);

        if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(billingAddress.Address2))
            addressResult.Address2 = billingAddress.Address2;
			_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): addressResult.Address2 → {Arg3}", vendor, lang, order, addressResult.Address2);

        if (_addressSettings.CityEnabled && !string.IsNullOrEmpty(billingAddress.City))
            addressResult.City = billingAddress.City;
			_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): addressResult.City → {Arg3}", vendor, lang, order, addressResult.City);

        if (_addressSettings.CountyEnabled && !string.IsNullOrEmpty(billingAddress.County))
            addressResult.County = billingAddress.County;
			_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): addressResult.County → {Arg3}", vendor, lang, order, addressResult.County);

        if (_addressSettings.ZipPostalCodeEnabled && !string.IsNullOrEmpty(billingAddress.ZipPostalCode))
            addressResult.ZipPostalCode = billingAddress.ZipPostalCode;
			_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): addressResult.ZipPostalCode → {Arg3}", vendor, lang, order, addressResult.ZipPostalCode);

        var stateProvince = await _stateProvinceService.GetStateProvinceByAddressAsync(billingAddress);
		_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): stateProvince → {StateProvince}", vendor, lang, order, stateProvince);
        addressResult.StateProvinceName = stateProvince != null ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name, lang.Id) : string.Empty;

        if (_addressSettings.CountryEnabled && await _countryService.GetCountryByAddressAsync(billingAddress) is Country country)
            addressResult.Country = await _localizationService.GetLocalizedAsync(country, x => x.Name, lang.Id);

        var (addressLine, _) = await _addressService.FormatAddressAsync(billingAddress, lang.Id);
        addressResult.AddressLine = addressLine;
		_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): addressResult.AddressLine → {Arg3}", vendor, lang, order, addressResult.AddressLine);

        //VAT number
        if (!string.IsNullOrEmpty(order.VatNumber))
            addressResult.VATNumber = order.VatNumber;
			_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): addressResult.VATNumber → {Arg3}", vendor, lang, order, addressResult.VATNumber);

        //custom attributes
        var customBillingAddressAttributes = await _addressAttributeFormatter
            .FormatAttributesAsync(billingAddress.CustomAttributes, "<br />");

        if (!string.IsNullOrEmpty(customBillingAddressAttributes))
        {
            var text = _htmlFormatter.ConvertHtmlToPlainText(customBillingAddressAttributes, true, true);
			_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): text → {Text}", vendor, lang, order, text);
            addressResult.AddressAttributes = text.Split('\n').ToList();
        }

        //billing address custom values
        var customValues = new CustomValues();
		_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): customValues → {CustomValues}", vendor, lang, order, customValues);
        customValues.FillByXml(order.CustomValuesXml, true);
        addressResult.CustomValues.AddRange(customValues.Where(value => value.DisplayLocation == CustomValueDisplayLocation.BillingAddress));

        //vendors payment details
        if (vendor is null)
        {
            //payment method
            var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(order.PaymentMethodSystemName);
			_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): paymentMethod → {PaymentMethod}", vendor, lang, order, paymentMethod);
            var paymentMethodStr = paymentMethod != null
                ? await _localizationService.GetLocalizedFriendlyNameAsync(paymentMethod, lang.Id)
                : order.PaymentMethodSystemName;
            if (!string.IsNullOrEmpty(paymentMethodStr))
            {
                addressResult.PaymentMethod = paymentMethodStr;
				_logger.LogDebug("GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order}): addressResult.PaymentMethod → {Arg3}", vendor, lang, order, addressResult.PaymentMethod);
            }

            //payment custom values
            addressResult.CustomValues.AddRange(customValues.Where(value => value.DisplayLocation == CustomValueDisplayLocation.Payment));
        }

		_logger.LogDebug("<<< Exiting GetBillingAddressAsync(vendor={Vendor},lang={Lang},order={Order})", vendor, lang, order);
        return addressResult;
    }

    /// <summary>
    /// Get shipping address
    /// </summary>
    /// <param name="lang">Language</param>
    /// <param name="order">Order</param>
    /// <returns>A task that contains address item</returns>
    protected virtual async Task<AddressItem> GetShippingAddressAsync(Language lang, Order order)
    {
		_logger.LogDebug(">>> Entering GetShippingAddressAsync(lang={Lang},order={Order})", lang, order);
        var addressResult = new AddressItem();

		_logger.LogDebug("GetShippingAddressAsync(lang={Lang},order={Order}): addressResult → {AddressResult}", lang, order, addressResult);
        if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
        {
            if (!order.PickupInStore)
            {
                if (order.ShippingAddressId == null || await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value) is not Address shippingAddress)
					_logger.LogWarning("Throwing NopException from GetShippingAddressAsync(lang={Lang},order={Order})", lang, order);
					_logger.LogDebug("GetShippingAddressAsync(lang={Lang},order={Order}): ID → {ID}", lang, order, ID);
                    throw new NopException($"Shipping is required, but address is not available. Order ID = {order.Id}");

                if (!string.IsNullOrEmpty(shippingAddress.Company))
                    addressResult.Company = shippingAddress.Company;
					_logger.LogDebug("GetShippingAddressAsync(lang={Lang},order={Order}): addressResult.Company → {Arg2}", lang, order, addressResult.Company);

                addressResult.Name = $"{shippingAddress.FirstName} {shippingAddress.LastName}";
				_logger.LogDebug("GetShippingAddressAsync(lang={Lang},order={Order}): addressResult.Name → {Arg2}", lang, order, addressResult.Name);

                if (_addressSettings.PhoneEnabled)
                    addressResult.Phone = shippingAddress.PhoneNumber;
					_logger.LogDebug("GetShippingAddressAsync(lang={Lang},order={Order}): addressResult.Phone → {Arg2}", lang, order, addressResult.Phone);

                if (_addressSettings.FaxEnabled && !string.IsNullOrEmpty(shippingAddress.FaxNumber))
                    addressResult.Fax = shippingAddress.FaxNumber;
					_logger.LogDebug("GetShippingAddressAsync(lang={Lang},order={Order}): addressResult.Fax → {Arg2}", lang, order, addressResult.Fax);

                if (_addressSettings.StreetAddressEnabled)
                    addressResult.Address = shippingAddress.Address1;
					_logger.LogDebug("GetShippingAddressAsync(lang={Lang},order={Order}): addressResult.Address → {Arg2}", lang, order, addressResult.Address);

                if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(shippingAddress.Address2))
                    addressResult.Address2 = shippingAddress.Address2;
					_logger.LogDebug("GetShippingAddressAsync(lang={Lang},order={Order}): addressResult.Address2 → {Arg2}", lang, order, addressResult.Address2);

                if (_addressSettings.CityEnabled && !string.IsNullOrEmpty(shippingAddress.City))
                    addressResult.City = shippingAddress.City;
					_logger.LogDebug("GetShippingAddressAsync(lang={Lang},order={Order}): addressResult.City → {Arg2}", lang, order, addressResult.City);

                if (_addressSettings.CountyEnabled && !string.IsNullOrEmpty(shippingAddress.County))
                    addressResult.County = shippingAddress.County;
					_logger.LogDebug("GetShippingAddressAsync(lang={Lang},order={Order}): addressResult.County → {Arg2}", lang, order, addressResult.County);

                if (_addressSettings.ZipPostalCodeEnabled && !string.IsNullOrEmpty(shippingAddress.ZipPostalCode))
                    addressResult.ZipPostalCode = shippingAddress.ZipPostalCode;
					_logger.LogDebug("GetShippingAddressAsync(lang={Lang},order={Order}): addressResult.ZipPostalCode → {Arg2}", lang, order, addressResult.ZipPostalCode);

                var stateProvince = await _stateProvinceService.GetStateProvinceByAddressAsync(shippingAddress);
				_logger.LogDebug("GetShippingAddressAsync(lang={Lang},order={Order}): stateProvince → {StateProvince}", lang, order, stateProvince);
                addressResult.StateProvinceName = stateProvince != null ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name, lang.Id) : string.Empty;

                if (_addressSettings.CountryEnabled && await _countryService.GetCountryByAddressAsync(shippingAddress) is Country country)
                {
                    addressResult.Country = await _localizationService.GetLocalizedAsync(country, x => x.Name, lang.Id);
                }

                var (addressLine, _) = await _addressService.FormatAddressAsync(shippingAddress, lang.Id);
                addressResult.AddressLine = addressLine;
				_logger.LogDebug("GetShippingAddressAsync(lang={Lang},order={Order}): addressResult.AddressLine → {Arg2}", lang, order, addressResult.AddressLine);

                //custom attributes
                var customShippingAddressAttributes = await _addressAttributeFormatter
                    .FormatAttributesAsync(shippingAddress.CustomAttributes, "<br />");
                if (!string.IsNullOrEmpty(customShippingAddressAttributes))
                {
                    var text = _htmlFormatter.ConvertHtmlToPlainText(customShippingAddressAttributes, true, true);
					_logger.LogDebug("GetShippingAddressAsync(lang={Lang},order={Order}): text → {Text}", lang, order, text);
                    addressResult.AddressAttributes = text.Split('\n').ToList();
                }
            }
            else if (order.PickupAddressId.HasValue && await _addressService.GetAddressByIdAsync(order.PickupAddressId.Value) is Address pickupAddress)
            {
                if (!string.IsNullOrEmpty(pickupAddress.Address1))
                    addressResult.Address = pickupAddress.Address1;

                if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(pickupAddress.Address2))
                    addressResult.Address2 = pickupAddress.Address2;

                if (_addressSettings.CityEnabled && !string.IsNullOrEmpty(pickupAddress.City))
                    addressResult.City = pickupAddress.City;

                if (_addressSettings.CountyEnabled && !string.IsNullOrEmpty(pickupAddress.County))
                    addressResult.County = pickupAddress.County;

                if (_addressSettings.ZipPostalCodeEnabled && !string.IsNullOrEmpty(pickupAddress.ZipPostalCode))
                    addressResult.ZipPostalCode = pickupAddress.ZipPostalCode;

                var (addressLine, _) = await _addressService.FormatAddressAsync(pickupAddress, lang.Id);
                addressResult.AddressLine = addressLine;

                var stateProvince = await _stateProvinceService.GetStateProvinceByAddressAsync(pickupAddress);
                addressResult.StateProvinceName = stateProvince != null ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name, lang.Id) : string.Empty;

                if (await _countryService.GetCountryByAddressAsync(pickupAddress) is Country country)
                    addressResult.Country = await _localizationService.GetLocalizedAsync(country, x => x.Name, lang.Id);
            }

            //shipping address custom values
            var customValues = new CustomValues();
			_logger.LogDebug("GetShippingAddressAsync(lang={Lang},order={Order}): customValues → {CustomValues}", lang, order, customValues);
            customValues.FillByXml(order.CustomValuesXml, true);
            addressResult.CustomValues.AddRange(customValues.Where(value => value.DisplayLocation == CustomValueDisplayLocation.ShippingAddress));

            addressResult.ShippingMethod = order.ShippingMethod;
			_logger.LogDebug("GetShippingAddressAsync(lang={Lang},order={Order}): addressResult.ShippingMethod → {Arg2}", lang, order, addressResult.ShippingMethod);

            //shipping custom values
            addressResult.CustomValues.AddRange(customValues.Where(value => value.DisplayLocation == CustomValueDisplayLocation.Shipping));
        }

		_logger.LogDebug("<<< Exiting GetShippingAddressAsync(lang={Lang},order={Order})", lang, order);
        return addressResult;
    }

    /// <summary>
    /// Get order notes
    /// </summary>
    /// <param name="pdfSettingsByStore">PDF settings</param>
    /// <param name="order">Order</param>
    /// <param name="lang">Language</param>
    /// <returns>A task that contains collection of date/note pairs</returns>
    protected virtual async Task<List<(string, string)>> GetOrderNotesAsync(PdfSettings pdfSettingsByStore, Order order, Language lang)
    {
        var notesResult = new List<(string, string)>();

        if (!pdfSettingsByStore.RenderOrderNotes)
            return notesResult;

        var orderNotes = (await _orderService.GetOrderNotesByOrderIdAsync(order.Id, true))
            .OrderByDescending(on => on.CreatedOnUtc)
            .ToList();

        if (!orderNotes.Any())
            return notesResult;

        foreach (var orderNote in orderNotes)
        {
            var createdOn = (await _dateTimeHelper.ConvertToUserTimeAsync(orderNote.CreatedOnUtc, DateTimeKind.Utc)).ToString();
            var note = _htmlFormatter.ConvertHtmlToPlainText(_orderService.FormatOrderNoteText(orderNote), true, true);

            notesResult.Add((createdOn, note));

            //should we display a link to downloadable files here?
            //I think, no. Anyway, PDFs are printable documents and links (files) are useful here
        }

        return notesResult;
    }

    /// <summary>
    /// Get product entries for document data source
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="orderItems">Collection of order items</param>
    /// <param name="language">Language</param>
    /// <param name="shipmentItems">Collection of shipment items; when using to prepare shipment items</param>
    /// <returns>A task that contains collection of product entries</returns>
    protected virtual async Task<List<ProductItem>> GetOrderProductItemsAsync(Order order, IList<OrderItem> orderItems, Language language, IList<ShipmentItem> shipmentItems = null)
    {
		_logger.LogDebug(">>> Entering GetOrderProductItemsAsync(order={Order},orderItems={OrderItems},language={Language},shipmentItems={ShipmentItems})", order, orderItems, language, shipmentItems);
        var vendors = _vendorSettings.ShowVendorOnOrderDetailsPage ? await _vendorService.GetVendorsByProductIdsAsync(orderItems.Select(item => item.ProductId).ToArray()) : new List<Vendor>();

		_logger.LogDebug("GetOrderProductItemsAsync(order={Order},orderItems={OrderItems},language={Language},shipmentItems={ShipmentItems}): vendors → {Vendors}", order, orderItems, language, shipmentItems, vendors);
        var result = new List<ProductItem>();
		_logger.LogDebug("GetOrderProductItemsAsync(order={Order},orderItems={OrderItems},language={Language},shipmentItems={ShipmentItems}): result → {Result}", order, orderItems, language, shipmentItems, result);

        foreach (var oi in orderItems)
        {
            var productItem = new ProductItem();
			_logger.LogDebug("GetOrderProductItemsAsync(order={Order},orderItems={OrderItems},language={Language},shipmentItems={ShipmentItems}): productItem → {ProductItem}", order, orderItems, language, shipmentItems, productItem);
            var product = await _productService.GetProductByIdAsync(oi.ProductId);
			_logger.LogDebug("GetOrderProductItemsAsync(order={Order},orderItems={OrderItems},language={Language},shipmentItems={ShipmentItems}): product → {Product}", order, orderItems, language, shipmentItems, product);

            //product name
            productItem.Name = await _localizationService.GetLocalizedAsync(product, x => x.Name, language.Id);

            //attributes
            if (!string.IsNullOrEmpty(oi.AttributeDescription))
            {
                var attributes = _htmlFormatter.ConvertHtmlToPlainText(oi.AttributeDescription, true, true);
				_logger.LogDebug("GetOrderProductItemsAsync(order={Order},orderItems={OrderItems},language={Language},shipmentItems={ShipmentItems}): attributes → {Attributes}", order, orderItems, language, shipmentItems, attributes);
                productItem.ProductAttributes = attributes.Split('\n').ToList();
            }

            //SKU
            if (_catalogSettings.ShowSkuOnProductDetailsPage)
                productItem.Sku = await _productService.FormatSkuAsync(product, oi.AttributesXml);

            //Vendor name
            if (_vendorSettings.ShowVendorOnOrderDetailsPage)
                productItem.VendorName = vendors.FirstOrDefault(v => v.Id == product.VendorId)?.Name ?? string.Empty;

            //price
            string unitPrice;
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax
                var unitPriceInclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(oi.UnitPriceInclTax, order.CurrencyRate);
                unitPrice = await _priceFormatter.FormatPriceAsync(unitPriceInclTaxInCustomerCurrency, true,
                    order.CustomerCurrencyCode, language.Id, true);
					_logger.LogDebug("GetOrderProductItemsAsync(order={Order},orderItems={OrderItems},language={Language},shipmentItems={ShipmentItems}): unitPrice → {UnitPrice}", order, orderItems, language, shipmentItems, unitPrice);
            }
            else
            {
                //excluding tax
                var unitPriceExclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(oi.UnitPriceExclTax, order.CurrencyRate);
                unitPrice = await _priceFormatter.FormatPriceAsync(unitPriceExclTaxInCustomerCurrency, true,
                    order.CustomerCurrencyCode, language.Id, false);
            }

            productItem.Price = unitPrice;
			_logger.LogDebug("GetOrderProductItemsAsync(order={Order},orderItems={OrderItems},language={Language},shipmentItems={ShipmentItems}): productItem.Price → {Arg4}", order, orderItems, language, shipmentItems, productItem.Price);

            //qty
            productItem.Quantity = shipmentItems is null ?
				_logger.LogDebug("GetOrderProductItemsAsync(order={Order},orderItems={OrderItems},language={Language},shipmentItems={ShipmentItems}): productItem.Quantity → {Arg4}", order, orderItems, language, shipmentItems, productItem.Quantity);
                oi.Quantity.ToString() :
                shipmentItems.FirstOrDefault(x => x.OrderItemId == oi.Id).Quantity.ToString();

            //total
            string subTotal;
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax
                var priceInclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(oi.PriceInclTax, order.CurrencyRate);
                subTotal = await _priceFormatter.FormatPriceAsync(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode,
                    language.Id, true);
					_logger.LogDebug("GetOrderProductItemsAsync(order={Order},orderItems={OrderItems},language={Language},shipmentItems={ShipmentItems}): subTotal → {SubTotal}", order, orderItems, language, shipmentItems, subTotal);
            }
            else
            {
                //excluding tax
                var priceExclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(oi.PriceExclTax, order.CurrencyRate);
                subTotal = await _priceFormatter.FormatPriceAsync(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode,
                    language.Id, false);
            }

            productItem.Total = subTotal;
			_logger.LogDebug("GetOrderProductItemsAsync(order={Order},orderItems={OrderItems},language={Language},shipmentItems={ShipmentItems}): productItem.Total → {Arg4}", order, orderItems, language, shipmentItems, productItem.Total);

            result.Add(productItem);
        }

		_logger.LogDebug("<<< Exiting GetOrderProductItemsAsync(order={Order},orderItems={OrderItems},language={Language},shipmentItems={ShipmentItems})", order, orderItems, language, shipmentItems);
        return result;
    }

    /// <summary>
    /// Get invoice totals
    /// </summary>
    /// <param name="lang">Language</param>
    /// <param name="order">Order</param>
    /// <returns>A task that contains invoice totals</returns>
    protected virtual async Task<InvoiceTotals> GetTotalsAsync(Language lang, Order order)
    {
		_logger.LogDebug(">>> Entering GetTotalsAsync(lang={Lang},order={Order})", lang, order);
        var result = new InvoiceTotals();
        var languageId = lang.Id;
		_logger.LogDebug("GetTotalsAsync(lang={Lang},order={Order}): result → {Result}", lang, order, result);

        //order subtotal
        if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax &&
            !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
        {
            //including tax
            var orderSubtotalInclTaxInCustomerCurrency =
                _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
            result.SubTotal = await _priceFormatter.FormatPriceAsync(orderSubtotalInclTaxInCustomerCurrency, true,
                order.CustomerCurrencyCode, languageId, true);
        }
        else
        {
            //excluding tax
            var orderSubtotalExclTaxInCustomerCurrency =
                _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
            result.SubTotal = await _priceFormatter.FormatPriceAsync(orderSubtotalExclTaxInCustomerCurrency, true,
                order.CustomerCurrencyCode, languageId, false);
        }

        //discount (applied to order subtotal)
        if (order.OrderSubTotalDiscountExclTax > decimal.Zero)
        {
            //order subtotal
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax &&
                !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
            {
                //including tax
                var orderSubTotalDiscountInclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                result.Discount = await _priceFormatter.FormatPriceAsync(
                    -orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            }
            else
            {
                //excluding tax
                var orderSubTotalDiscountExclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                result.Discount = await _priceFormatter.FormatPriceAsync(
                    -orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            }
        }

        //shipping
        if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
        {
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax
                var orderShippingInclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                result.Shipping = await _priceFormatter.FormatShippingPriceAsync(
                    orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            }
            else
            {
                //excluding tax
                var orderShippingExclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                result.Shipping = await _priceFormatter.FormatShippingPriceAsync(
                    orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            }
        }

        //payment fee
        if (order.PaymentMethodAdditionalFeeExclTax > decimal.Zero)
        {
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax
                var paymentMethodAdditionalFeeInclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                result.PaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(
                    paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            }
            else
            {
                //excluding tax
                var paymentMethodAdditionalFeeExclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                result.PaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(
                    paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            }
        }

        //tax
        var taxStr = string.Empty;
        var taxRates = new SortedDictionary<decimal, decimal>();
		_logger.LogDebug("GetTotalsAsync(lang={Lang},order={Order}): taxRates → {TaxRates}", lang, order, taxRates);
        bool displayTax;
        var displayTaxRates = true;
		_logger.LogDebug("GetTotalsAsync(lang={Lang},order={Order}): displayTaxRates → {DisplayTaxRates}", lang, order, displayTaxRates);
        if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
        {
            displayTax = false;
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
                taxRates = _orderService.ParseTaxRates(order, order.TaxRates);

                displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                displayTax = !displayTaxRates;

                var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
				_logger.LogDebug("GetTotalsAsync(lang={Lang},order={Order}): orderTaxInCustomerCurrency → {OrderTaxInCustomerCurrency}", lang, order, orderTaxInCustomerCurrency);
                taxStr = await _priceFormatter.FormatPriceAsync(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode,
                    false, languageId);
            }
        }

        if (displayTax)
        {
            result.Tax = taxStr;
			_logger.LogDebug("GetTotalsAsync(lang={Lang},order={Order}): result.Tax → {Arg2}", lang, order, result.Tax);
        }

        if (displayTaxRates)
        {
            foreach (var item in taxRates)
            {
                var taxRate = string.Format(await _localizationService.GetResourceAsync("Pdf.TaxRate", languageId),
                    _priceFormatter.FormatTaxRate(item.Key));
                var taxValue = await _priceFormatter.FormatPriceAsync(
                    _currencyService.ConvertCurrency(item.Value, order.CurrencyRate), true, order.CustomerCurrencyCode,
                    false, languageId);

                result.TaxRates.Add($"{taxRate} {taxValue}");
            }
        }

        //discount (applied to order total)
        if (order.OrderDiscount > decimal.Zero)
        {
            var orderDiscountInCustomerCurrency =
                _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
            result.Discount = await _priceFormatter.FormatPriceAsync(-orderDiscountInCustomerCurrency,
                true, order.CustomerCurrencyCode, false, languageId);
        }

        //gift cards
        foreach (var gcuh in await _giftCardService.GetGiftCardUsageHistoryAsync(order))
        {
            var gcTitle = string.Format(await _localizationService.GetResourceAsync("Pdf.GiftCardInfo", languageId),
                (await _giftCardService.GetGiftCardByIdAsync(gcuh.GiftCardId))?.GiftCardCouponCode);
            var gcAmountStr = await _priceFormatter.FormatPriceAsync(
                -_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate), true,
                order.CustomerCurrencyCode, false, languageId);

            result.GiftCards.Add($"{gcTitle} {gcAmountStr}");
        }

        //reward points
        if (order.RedeemedRewardPointsEntryId.HasValue && await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
        {
            var rpTitle = string.Format(await _localizationService.GetResourceAsync("Pdf.RewardPoints", languageId),
                -redeemedRewardPointsEntry.Points);
            var rpAmount = await _priceFormatter.FormatPriceAsync(
                -_currencyService.ConvertCurrency(redeemedRewardPointsEntry.UsedAmount, order.CurrencyRate),
                true, order.CustomerCurrencyCode, false, languageId);

            result.RewardPoints = $"{rpTitle} {rpAmount}";
			_logger.LogDebug("GetTotalsAsync(lang={Lang},order={Order}): result.RewardPoints → {Arg2}", lang, order, result.RewardPoints);
        }

        //order total
        var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
		_logger.LogDebug("GetTotalsAsync(lang={Lang},order={Order}): orderTotalInCustomerCurrency → {OrderTotalInCustomerCurrency}", lang, order, orderTotalInCustomerCurrency);
        result.OrderTotal = await _priceFormatter.FormatPriceAsync(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);

		_logger.LogDebug("<<< Exiting GetTotalsAsync(lang={Lang},order={Order})", lang, order);
        return result;
    }

    /// <summary>
    /// Resolve font for PDF document
    /// </summary>
    /// <param name="language">Language</param>
    /// <param name="settings">PDF settings</param>
    /// <returns>A font object</returns>
    protected virtual Font ResolvePdfFont(Language language, PdfSettings settings)
    {
		_logger.LogDebug(">>> Entering ResolvePdfFont(language={Language},settings={Settings})", language, settings);
        ArgumentNullException.ThrowIfNull(settings);

        var fontName = language?.Rtl == true
            ? !string.IsNullOrEmpty(settings.RtlFontName) ? settings.RtlFontName : NopCommonDefaults.PdfRtlFontName
            : !string.IsNullOrEmpty(settings.LtrFontName) ? settings.LtrFontName : NopCommonDefaults.PdfLtrFontName;

        var fontSize = settings.BaseFontSize >= 0 ? settings.BaseFontSize : 10;

		_logger.LogDebug("<<< Exiting ResolvePdfFont(language={Language},settings={Settings})", language, settings);
        return PdfDocumentHelper.GetFont(fontName, fontSize);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Write PDF invoice to the specified stream
    /// </summary>
    /// <param name="stream">Stream to save PDF</param>
    /// <param name="order">Order</param>
    /// <param name="language">Language; null to use a language used when placing an order</param>
    /// <param name="store">Store</param>
    /// <param name="vendor">Vendor to limit products; null to print all products. If specified, then totals won't be printed</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    public virtual async Task PrintOrderToPdfAsync(Stream stream, Order order, Language language = null, Store store = null, Vendor vendor = null)
    {
		_logger.LogDebug(">>> Entering PrintOrderToPdfAsync(stream={Stream},order={Order},language={Language},store={Store},vendor={Vendor})", stream, order, language, store, vendor);
        ArgumentNullException.ThrowIfNull(order);

        //store info
        store ??= await _storeContext.GetCurrentStoreAsync();

        var orderStore = order.StoreId == 0 || order.StoreId == store?.Id ?
            store : await _storeService.GetStoreByIdAsync(order.StoreId);

        //language info
        language ??= await _languageService.GetLanguageByIdAsync(order.CustomerLanguageId);

        if (language?.Published != true)
            language = await _workContext.GetWorkingLanguageAsync();
			_logger.LogDebug("PrintOrderToPdfAsync(stream={Stream},order={Order},language={Language},store={Store},vendor={Vendor}): language → {Language1}", stream, order, language, store, vendor, language);

        //by default _pdfSettings contains settings for the current active store
        //and we need PdfSettings for the store which was used to place an order
        //so let's load it based on a store of the current order
        var pdfSettingsByStore = await _settingService.LoadSettingAsync<PdfSettings>(orderStore.Id);
		_logger.LogDebug("PrintOrderToPdfAsync(stream={Stream},order={Order},language={Language},store={Store},vendor={Vendor}): pdfSettingsByStore → {PdfSettingsByStore}", stream, order, language, store, vendor, pdfSettingsByStore);

        byte[] logo = null;
        var logoPicture = await _pictureService.GetPictureByIdAsync(pdfSettingsByStore.LogoPictureId);
		_logger.LogDebug("PrintOrderToPdfAsync(stream={Stream},order={Order},language={Language},store={Store},vendor={Vendor}): logoPicture → {LogoPicture}", stream, order, language, store, vendor, logoPicture);
        if (logoPicture != null)
        {
            logo = await _pictureService.LoadPictureBinaryAsync(logoPicture);

			_logger.LogDebug("PrintOrderToPdfAsync(stream={Stream},order={Order},language={Language},store={Store},vendor={Vendor}): logo → {Logo}", stream, order, language, store, vendor, logo);
            if (logoPicture.MimeType == MimeTypes.ImageSvg)
            {
                await using var logoStream = new MemoryStream(logo);
				_logger.LogDebug("PrintOrderToPdfAsync(stream={Stream},order={Order},language={Language},store={Store},vendor={Vendor}): logoStream → {LogoStream}", stream, order, language, store, vendor, logoStream);
                logo = await _pictureService.ConvertSvgToPngAsync(logoStream);
            }
        }

        var date = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);
		_logger.LogDebug("PrintOrderToPdfAsync(stream={Stream},order={Order},language={Language},store={Store},vendor={Vendor}): date → {Date}", stream, order, language, store, vendor, date);

        //a vendor should have access only to products
        var orderItems = await _orderService.GetOrderItemsAsync(order.Id, vendorId: vendor?.Id ?? 0);
		_logger.LogDebug("PrintOrderToPdfAsync(stream={Stream},order={Order},language={Language},store={Store},vendor={Vendor}): orderItems → {OrderItems}", stream, order, language, store, vendor, orderItems);

        var column1Lines = string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn1) ?
            new List<string>()
            : pdfSettingsByStore.InvoiceFooterTextColumn1
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

        var column2Lines = string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn2) ?
            new List<string>()
            : pdfSettingsByStore.InvoiceFooterTextColumn2
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

        var document = new InvoiceDocument
        {
            StoreUrl = orderStore.Url?.Trim('/'),
            Language = language,
            Font = ResolvePdfFont(language, pdfSettingsByStore),
            ImageTargetSize = pdfSettingsByStore.ImageTargetSize,
            OrderDateUser = date.ToString("D", new CultureInfo(language.LanguageCulture)),
            LogoData = logo,
            OrderNumberText = order.CustomOrderNumber,
            PageSize = pdfSettingsByStore.LetterPageSizeEnabled ? PdfPageSize.Letter : PdfPageSize.A4,
            BillingAddress = await GetBillingAddressAsync(vendor, language, order),
            ShippingAddress = await GetShippingAddressAsync(language, order),
            Products = await GetOrderProductItemsAsync(order, orderItems, language),
            ShowSkuInProductList = _catalogSettings.ShowSkuOnProductDetailsPage,
            ShowVendorInProductList = _vendorSettings.ShowVendorOnOrderDetailsPage,
            CheckoutAttributes = vendor is null ? _htmlFormatter.ConvertHtmlToPlainText(order.CheckoutAttributeDescription, true, true) : string.Empty, //vendors cannot see checkout attributes
            Totals = vendor is null ? await GetTotalsAsync(language, order) : new(), //vendors cannot see totals
            OrderNotes = await GetOrderNotesAsync(pdfSettingsByStore, order, language),
            FooterTextColumn1 = column1Lines,
            FooterTextColumn2 = column2Lines,
            GetResourceAsync = async (string resourceKey, int languageId) => await _localizationService.GetResourceAsync(resourceKey, languageId)
        };

        await using var pdfStream = new MemoryStream();
		_logger.LogDebug("PrintOrderToPdfAsync(stream={Stream},order={Order},language={Language},store={Store},vendor={Vendor}): pdfStream → {PdfStream}", stream, order, language, store, vendor, pdfStream);
        document.Generate(pdfStream);

        pdfStream.Position = 0;
		_logger.LogDebug("PrintOrderToPdfAsync(stream={Stream},order={Order},language={Language},store={Store},vendor={Vendor}): pdfStream.Position → {Arg5}", stream, order, language, store, vendor, pdfStream.Position);
        await pdfStream.CopyToAsync(stream);
		_logger.LogDebug("<<< Exiting PrintOrderToPdfAsync(stream={Stream},order={Order},language={Language},store={Store},vendor={Vendor})", stream, order, language, store, vendor);
    }

    /// <summary>
    /// Write ZIP archive with invoices to the specified stream
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="orders">Orders</param>
    /// <param name="language">Language; null to use a language used when placing an order</param>
    /// <param name="vendor">Vendor to limit products; null to print all products. If specified, then totals won't be printed</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrintOrdersToPdfAsync(Stream stream, IList<Order> orders, Language language = null, Vendor vendor = null)
    {
		_logger.LogDebug(">>> Entering PrintOrdersToPdfAsync(stream={Stream},orders={Orders},language={Language},vendor={Vendor})", stream, orders, language, vendor);
        ArgumentNullException.ThrowIfNull(stream);

        ArgumentNullException.ThrowIfNull(orders);

        var currentStore = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("PrintOrdersToPdfAsync(stream={Stream},orders={Orders},language={Language},vendor={Vendor}): currentStore → {CurrentStore}", stream, orders, language, vendor, currentStore);

        using var archive = new ZipArchive(stream, ZipArchiveMode.Create, true);
		_logger.LogDebug("PrintOrdersToPdfAsync(stream={Stream},orders={Orders},language={Language},vendor={Vendor}): archive → {Archive}", stream, orders, language, vendor, archive);

        foreach (var order in orders)
        {
            var entryName = string.Format(await _localizationService.GetResourceAsync("Pdf.Order"), order.CustomOrderNumber);
			_logger.LogDebug("PrintOrdersToPdfAsync(stream={Stream},orders={Orders},language={Language},vendor={Vendor}): entryName → {EntryName}", stream, orders, language, vendor, entryName);

            await using var fileStreamInZip = archive.CreateEntry($"{entryName}.pdf").Open();
			_logger.LogDebug("PrintOrdersToPdfAsync(stream={Stream},orders={Orders},language={Language},vendor={Vendor}): fileStreamInZip → {FileStreamInZip}", stream, orders, language, vendor, fileStreamInZip);
            await using var pdfStream = new MemoryStream();
			_logger.LogDebug("PrintOrdersToPdfAsync(stream={Stream},orders={Orders},language={Language},vendor={Vendor}): pdfStream → {PdfStream}", stream, orders, language, vendor, pdfStream);
            await PrintOrderToPdfAsync(pdfStream, order, language, currentStore, vendor);
            pdfStream.Position = 0;
			_logger.LogDebug("PrintOrdersToPdfAsync(stream={Stream},orders={Orders},language={Language},vendor={Vendor}): pdfStream.Position → {Arg4}", stream, orders, language, vendor, pdfStream.Position);
            await pdfStream.CopyToAsync(fileStreamInZip);
        }
		_logger.LogDebug("<<< Exiting PrintOrdersToPdfAsync(stream={Stream},orders={Orders},language={Language},vendor={Vendor})", stream, orders, language, vendor);
    }

    /// <summary>
    /// Write ZIP archive with packaging slips to the specified stream
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="shipments">Shipments</param>
    /// <param name="language">Language; null to use a language used when placing an order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrintPackagingSlipsToPdfAsync(Stream stream, IList<Shipment> shipments, Language language = null)
    {
		_logger.LogDebug(">>> Entering PrintPackagingSlipsToPdfAsync(stream={Stream},shipments={Shipments},language={Language})", stream, shipments, language);
        ArgumentNullException.ThrowIfNull(stream);

        ArgumentNullException.ThrowIfNull(shipments);

        using var archive = new ZipArchive(stream, ZipArchiveMode.Create, true);
		_logger.LogDebug("PrintPackagingSlipsToPdfAsync(stream={Stream},shipments={Shipments},language={Language}): archive → {Archive}", stream, shipments, language, archive);

        foreach (var shipment in shipments)
        {
            var entryName = string.Format(await _localizationService.GetResourceAsync("Pdf.Shipment"), shipment.Id);
			_logger.LogDebug("PrintPackagingSlipsToPdfAsync(stream={Stream},shipments={Shipments},language={Language}): entryName → {EntryName}", stream, shipments, language, entryName);

            await using var fileStreamInZip = archive.CreateEntry($"{entryName}.pdf").Open();
			_logger.LogDebug("PrintPackagingSlipsToPdfAsync(stream={Stream},shipments={Shipments},language={Language}): fileStreamInZip → {FileStreamInZip}", stream, shipments, language, fileStreamInZip);
            await using var pdfStream = new MemoryStream();
			_logger.LogDebug("PrintPackagingSlipsToPdfAsync(stream={Stream},shipments={Shipments},language={Language}): pdfStream → {PdfStream}", stream, shipments, language, pdfStream);
            await PrintPackagingSlipToPdfAsync(pdfStream, shipment, language);

            pdfStream.Position = 0;
			_logger.LogDebug("PrintPackagingSlipsToPdfAsync(stream={Stream},shipments={Shipments},language={Language}): pdfStream.Position → {Arg3}", stream, shipments, language, pdfStream.Position);
            await pdfStream.CopyToAsync(fileStreamInZip);
        }
		_logger.LogDebug("<<< Exiting PrintPackagingSlipsToPdfAsync(stream={Stream},shipments={Shipments},language={Language})", stream, shipments, language);
    }

    /// <summary>
    /// Write packaging slip to the specified stream
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="shipment">Shipment</param>
    /// <param name="language">Language; null to use a language used when placing an order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrintPackagingSlipToPdfAsync(Stream stream, Shipment shipment, Language language = null)
    {
		_logger.LogDebug(">>> Entering PrintPackagingSlipToPdfAsync(stream={Stream},shipment={Shipment},language={Language})", stream, shipment, language);
        ArgumentNullException.ThrowIfNull(stream);

        ArgumentNullException.ThrowIfNull(shipment);

        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);
		_logger.LogDebug("PrintPackagingSlipToPdfAsync(stream={Stream},shipment={Shipment},language={Language}): order → {Order}", stream, shipment, language, order);

        var pdfSettingsByStore = await _settingService.LoadSettingAsync<PdfSettings>(order.StoreId);
		_logger.LogDebug("PrintPackagingSlipToPdfAsync(stream={Stream},shipment={Shipment},language={Language}): pdfSettingsByStore → {PdfSettingsByStore}", stream, shipment, language, pdfSettingsByStore);

        //language info
        language ??= await _languageService.GetLanguageByIdAsync(order.CustomerLanguageId);

        if (language?.Published != true)
            language = await _workContext.GetWorkingLanguageAsync();
			_logger.LogDebug("PrintPackagingSlipToPdfAsync(stream={Stream},shipment={Shipment},language={Language}): language → {Language1}", stream, shipment, language, language);

        var shipmentItems = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id);
		_logger.LogDebug("PrintPackagingSlipToPdfAsync(stream={Stream},shipment={Shipment},language={Language}): shipmentItems → {ShipmentItems}", stream, shipment, language, shipmentItems);

        if (shipmentItems?.Any() != true)
            return;

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var orderItems = await shipmentItems
            .SelectAwait(async si => await _orderService.GetOrderItemByIdAsync(si.OrderItemId))
            .Where(pi => pi != null)
            .ToListAsync();
			_logger.LogInformation("PrintPackagingSlipToPdfAsync(stream,shipment,language)={Stream},{Shipment},{Language}: database query executed in {Arg3} ms", stream, shipment, language, __logStart.ElapsedMilliseconds);

        if (orderItems?.Any() != true)
            return;

        await using var pdfStream = new MemoryStream();
		_logger.LogDebug("PrintPackagingSlipToPdfAsync(stream={Stream},shipment={Shipment},language={Language}): pdfStream → {PdfStream}", stream, shipment, language, pdfStream);
        var document = new ShipmentDocument
        {
            PageSize = pdfSettingsByStore.LetterPageSizeEnabled ? PdfPageSize.Letter : PdfPageSize.A4,
            Language = language,
            Font = ResolvePdfFont(language, pdfSettingsByStore),
            ImageTargetSize = pdfSettingsByStore.ImageTargetSize,
            ShipmentNumberText = shipment.Id.ToString(),
            OrderNumberText = order.CustomOrderNumber,
            Address = await GetShippingAddressAsync(language, order),
            Products = await GetOrderProductItemsAsync(order, orderItems, language, shipmentItems),
            GetResourceAsync = async (string resourceKey, int languageId) => await _localizationService.GetResourceAsync(resourceKey, languageId)
        };

        document.Generate(pdfStream);

        pdfStream.Position = 0;
		_logger.LogDebug("PrintPackagingSlipToPdfAsync(stream={Stream},shipment={Shipment},language={Language}): pdfStream.Position → {Arg3}", stream, shipment, language, pdfStream.Position);
        await pdfStream.CopyToAsync(stream);
		_logger.LogDebug("<<< Exiting PrintPackagingSlipToPdfAsync(stream={Stream},shipment={Shipment},language={Language})", stream, shipment, language);
    }

    /// <summary>
    /// Write PDF catalog to the specified stream
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="products">Products</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrintProductsToPdfAsync(Stream stream, IList<Product> products)
    {
		_logger.LogDebug(">>> Entering PrintProductsToPdfAsync(stream={Stream},products={Products})", stream, products);
        ArgumentNullException.ThrowIfNull(stream);

        ArgumentNullException.ThrowIfNull(products);

        var currentStore = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("PrintProductsToPdfAsync(stream={Stream},products={Products}): currentStore → {CurrentStore}", stream, products, currentStore);
        var pdfSettingsByStore = await _settingService.LoadSettingAsync<PdfSettings>(currentStore.Id);
		_logger.LogDebug("PrintProductsToPdfAsync(stream={Stream},products={Products}): pdfSettingsByStore → {PdfSettingsByStore}", stream, products, pdfSettingsByStore);
        var lang = await _workContext.GetWorkingLanguageAsync();
		_logger.LogDebug("PrintProductsToPdfAsync(stream={Stream},products={Products}): lang → {Lang}", stream, products, lang);

        var productItems = new List<CatalogItem>();
		_logger.LogDebug("PrintProductsToPdfAsync(stream={Stream},products={Products}): productItems → {ProductItems}", stream, products, productItems);

        foreach (var product in products)
        {
            var priceStr = $"{product.Price:0.00} {(await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode}";
			_logger.LogDebug("PrintProductsToPdfAsync(stream={Stream},products={Products}): priceStr → {PriceStr}", stream, products, priceStr);
            if (product.IsRental)
                priceStr = await _priceFormatter.FormatRentalProductPeriodAsync(product, priceStr);

            var rawDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription, lang.Id);
			_logger.LogDebug("PrintProductsToPdfAsync(stream={Stream},products={Products}): rawDescription → {RawDescription}", stream, products, rawDescription);

            var productNumber = products.IndexOf(product) + 1;
			_logger.LogDebug("PrintProductsToPdfAsync(stream={Stream},products={Products}): productNumber → {ProductNumber}", stream, products, productNumber);
            var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name, lang.Id);
			_logger.LogDebug("PrintProductsToPdfAsync(stream={Stream},products={Products}): productName → {ProductName}", stream, products, productName);

            var item = new CatalogItem()
            {
                Name = $"{productNumber}. {productName}",
                Description = _htmlFormatter.StripTags(_htmlFormatter.ConvertHtmlToPlainText(rawDescription, decode: true)),
                Price = priceStr,
                Sku = product.Sku,
                Weight = product.IsShipEnabled && product.Weight > decimal.Zero ?
                    $"{product.Weight:0.00} {(await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId)).Name}" :
                    string.Empty,
                Stock = product.ManageInventoryMethod == ManageInventoryMethod.ManageStock ?
                    $"{await _productService.GetTotalStockQuantityAsync(product)}" :
                    string.Empty
            };

            var pictures = await _pictureService.GetPicturesByProductIdAsync(product.Id);
			_logger.LogDebug("PrintProductsToPdfAsync(stream={Stream},products={Products}): pictures → {Pictures}", stream, products, pictures);

            //WebP images are not supported
            pictures = pictures.Where(picture => picture.MimeType != MimeTypes.ImageWebp).ToList();

            if (pictures.Any())
            {
                var picturePaths = new HashSet<string>();
				_logger.LogDebug("PrintProductsToPdfAsync(stream={Stream},products={Products}): picturePaths → {PicturePaths}", stream, products, picturePaths);

                foreach (var pic in pictures)
                {
                    var (pictureUrl, _) = await _pictureService.GetPictureUrlAsync(pic, pdfSettingsByStore.ImageTargetSize, false);
                    var picPath = await _thumbService.GetThumbLocalPathAsync(pictureUrl);
					_logger.LogDebug("PrintProductsToPdfAsync(stream={Stream},products={Products}): picPath → {PicPath}", stream, products, picPath);

                    if (!string.IsNullOrEmpty(picPath))
                        picturePaths.Add(picPath);
                }

                item.PicturePaths = picturePaths;
				_logger.LogDebug("PrintProductsToPdfAsync(stream={Stream},products={Products}): item.PicturePaths → {Arg2}", stream, products, item.PicturePaths);
            }

            productItems.Add(item);
        }

        var catalogDocument = new CatalogDocument
        {
            Language = lang,
            ImageTargetSize = pdfSettingsByStore.ImageTargetSize,
            PageSize = pdfSettingsByStore.LetterPageSizeEnabled ? PdfPageSize.Letter : PdfPageSize.A4,
            Font = ResolvePdfFont(lang, pdfSettingsByStore),
            Products = productItems,
            GetImageAsync = async (string path) =>
            {
                ArgumentNullException.ThrowIfNullOrEmpty(path);

                var imageType = _pictureService.GetPictureContentTypeByFileExtension(_fileProvider.GetFileExtension(path));
                if (imageType == MimeTypes.ImageSvg)
                {
                    using var svgFileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					using Microsoft.Extensions.Logging;
                    return await _pictureService.ConvertSvgToPngAsync(svgFileStream);
                }

                return await _fileProvider.ReadAllBytesAsync(path);
            },
            GetResourceAsync = async (string resourceKey, int languageId) => await _localizationService.GetResourceAsync(resourceKey, languageId)
        };


        await using var pdfStream = new MemoryStream();
		_logger.LogDebug("PrintProductsToPdfAsync(stream={Stream},products={Products}): pdfStream → {PdfStream}", stream, products, pdfStream);
        catalogDocument.Generate(pdfStream);

        pdfStream.Position = 0;
		_logger.LogDebug("PrintProductsToPdfAsync(stream={Stream},products={Products}): pdfStream.Position → {Arg2}", stream, products, pdfStream.Position);
        await pdfStream.CopyToAsync(stream);
		_logger.LogDebug("<<< Exiting PrintProductsToPdfAsync(stream={Stream},products={Products})", stream, products);
    }

    /// <summary>
    /// Export an order to PDF and save to disk
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="language">Language identifier; null to use a language used when placing an order</param>
    /// <param name="vendor">Vendor to limit products; null to print all products. If specified, then totals won't be printed</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a path of generated file
    /// </returns>
    public virtual async Task<string> SaveOrderPdfToDiskAsync(Order order, Language language = null, Vendor vendor = null)
    {
		private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        var fileName = $"order_{order.OrderGuid}_{CommonHelper.GenerateRandomDigitCode(4)}.pdf";
		_logger.LogDebug(">>> Entering SaveOrderPdfToDiskAsync(order={Order},language={Language},vendor={Vendor})", order, language, vendor);
        var filePath = _fileProvider.Combine(_fileProvider.MapPath("~/wwwroot/files/exportimport"), fileName);
		_logger.LogDebug("SaveOrderPdfToDiskAsync(order={Order},language={Language},vendor={Vendor}): filePath → {FilePath}", order, language, vendor, filePath);
        await using var fileStream = new FileStream(filePath, FileMode.Create);
		_logger.LogDebug("SaveOrderPdfToDiskAsync(order={Order},language={Language},vendor={Vendor}): fileStream → {FileStream}", order, language, vendor, fileStream);

        await PrintOrderToPdfAsync(fileStream, order, language, store: null, vendor: vendor);

		_logger.LogDebug("<<< Exiting SaveOrderPdfToDiskAsync(order={Order},language={Language},vendor={Vendor})", order, language, vendor);
        return filePath;
    }

    #endregion
}