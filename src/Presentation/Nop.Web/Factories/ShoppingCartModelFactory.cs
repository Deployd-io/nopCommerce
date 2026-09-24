using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
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
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the shopping cart model factory
/// </summary>
public partial class ShoppingCartModelFactory : IShoppingCartModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly CommonSettings _commonSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
    protected readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;
    protected readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
    protected readonly ICountryService _countryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly ICustomWishlistService _customWishlistService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IDiscountService _discountService;
    protected readonly IDownloadService _downloadService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IGiftCardService _giftCardService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IPaymentService _paymentService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPictureService _pictureService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductAttributeFormatter _productAttributeFormatter;
    protected readonly IProductService _productService;
    protected readonly IShippingService _shippingService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IShortTermCacheManager _shortTermCacheManager;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly ITaxService _taxService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVendorService _vendorService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly OrderSettings _orderSettings;
    protected readonly RewardPointsSettings _rewardPointsSettings;
    protected readonly ShippingSettings _shippingSettings;
    protected readonly ShoppingCartSettings _shoppingCartSettings;
    protected readonly TaxSettings _taxSettings;
    protected readonly VendorSettings _vendorSettings;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public ShoppingCartModelFactory(AddressSettings addressSettings,
        CaptchaSettings captchaSettings,
        CatalogSettings catalogSettings,
        CommonSettings commonSettings,
        CustomerSettings customerSettings,
        IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
        IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService,
        ICheckoutAttributeFormatter checkoutAttributeFormatter,
        ICountryService countryService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        ICustomWishlistService customWishlistService,
        IDateTimeHelper dateTimeHelper,
        IDiscountService discountService,
        IDownloadService downloadService,
        IGenericAttributeService genericAttributeService,
        IGiftCardService giftCardService,
        IHttpContextAccessor httpContextAccessor,
        ILocalizationService localizationService,
        IOrderProcessingService orderProcessingService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IPaymentPluginManager paymentPluginManager,
        IPaymentService paymentService,
        IPermissionService permissionService,
        IPictureService pictureService,
        IPriceFormatter priceFormatter,
        IProductAttributeFormatter productAttributeFormatter,
        IProductService productService,
        IShippingService shippingService,
        IShoppingCartService shoppingCartService,
        IShortTermCacheManager shortTermCacheManager,
        IStateProvinceService stateProvinceService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        ITaxService taxService,
        IUrlRecordService urlRecordService,
        IVendorService vendorService,
        IWebHelper webHelper,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        OrderSettings orderSettings,
        RewardPointsSettings rewardPointsSettings,
        ShippingSettings shippingSettings,
        ShoppingCartSettings shoppingCartSettings,
        TaxSettings taxSettings,
        VendorSettings vendorSettings)
    {
        _addressSettings = addressSettings;
        _addressService = addressService;
        _captchaSettings = captchaSettings;
        _catalogSettings = catalogSettings;
        _commonSettings = commonSettings;
        _customerSettings = customerSettings;
        _customWishlistService = customWishlistService;
        _addressModelFactory = addressModelFactory;
        _checkoutAttributeParser = checkoutAttributeParser;
        _checkoutAttributeService = checkoutAttributeService;
        _checkoutAttributeFormatter = checkoutAttributeFormatter;
        _countryService = countryService;
        _currencyService = currencyService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _discountService = discountService;
        _downloadService = downloadService;
        _genericAttributeService = genericAttributeService;
        _giftCardService = giftCardService;
        _httpContextAccessor = httpContextAccessor;
        _localizationService = localizationService;
        _orderProcessingService = orderProcessingService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _paymentPluginManager = paymentPluginManager;
        _paymentService = paymentService;
        _permissionService = permissionService;
        _pictureService = pictureService;
        _priceFormatter = priceFormatter;
        _productAttributeFormatter = productAttributeFormatter;
        _productService = productService;
        _shippingService = shippingService;
        _shoppingCartService = shoppingCartService;
        _shortTermCacheManager = shortTermCacheManager;
        _stateProvinceService = stateProvinceService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _taxService = taxService;
        _urlRecordService = urlRecordService;
        _vendorService = vendorService;
        _webHelper = webHelper;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _orderSettings = orderSettings;
        _rewardPointsSettings = rewardPointsSettings;
        _shippingSettings = shippingSettings;
        _shoppingCartSettings = shoppingCartSettings;
        _taxSettings = taxSettings;
        _vendorSettings = vendorSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare the checkout attribute models
    /// </summary>
    /// <param name="cart">List of the shopping cart item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of the checkout attribute model
    /// </returns>
    protected virtual async Task<IList<ShoppingCartModel.CheckoutAttributeModel>> PrepareCheckoutAttributeModelsAsync(
        IList<ShoppingCartItem> cart)
    {
		_logger.LogError(">>> Entering PrepareCheckoutAttributeModelsAsync(cart={Cart})", cart);
        ArgumentNullException.ThrowIfNull(cart);

        var model = new List<ShoppingCartModel.CheckoutAttributeModel>();
		_logger.LogError("PrepareCheckoutAttributeModelsAsync(cart={Cart}): model → {Model}", cart, model);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareCheckoutAttributeModelsAsync(cart={Cart}): store → {Store}", cart, store);
        var excludeShippableAttributes = !await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
		_logger.LogError("PrepareCheckoutAttributeModelsAsync(cart={Cart}): excludeShippableAttributes → {ExcludeShippableAttributes}", cart, excludeShippableAttributes);
        var checkoutAttributes =
            await _checkoutAttributeService.GetAllAttributesAsync(_staticCacheManager, _storeMappingService, store.Id, excludeShippableAttributes);
        foreach (var attribute in checkoutAttributes)
        {
            var attributeModel = new ShoppingCartModel.CheckoutAttributeModel
            {
                Id = attribute.Id,
                Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name),
                TextPrompt = await _localizationService.GetLocalizedAsync(attribute, x => x.TextPrompt),
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType,
                DefaultValue = await _localizationService.GetLocalizedAsync(attribute, x => x.DefaultValue)
            };
            if (!string.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
            {
                attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                    .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
					_logger.LogDebug("PrepareCheckoutAttributeModelsAsync(cart={Cart}): attributeModel.AllowedFileExtensions → {Arg1}", cart, attributeModel.AllowedFileExtensions);
            }

            if (attribute.ShouldHaveValues)
            {
                //values
                var attributeValues = await _checkoutAttributeService.GetAttributeValuesAsync(attribute.Id);
				_logger.LogError("PrepareCheckoutAttributeModelsAsync(cart={Cart}): attributeValues → {AttributeValues}", cart, attributeValues);
                foreach (var attributeValue in attributeValues)
                {
                    var attributeValueModel = new ShoppingCartModel.CheckoutAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                        ColorSquaresRgb = attributeValue.ColorSquaresRgb,
                        IsPreSelected = attributeValue.IsPreSelected,
                    };
                    attributeModel.Values.Add(attributeValueModel);

                    //display price if allowed
                    if (await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES))
                    {
                        var (priceAdjustmentBase, _) = await _taxService.GetCheckoutAttributePriceAsync(attribute, attributeValue);
                        var priceAdjustment =
                            await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(priceAdjustmentBase,
                                await _workContext.GetWorkingCurrencyAsync());
                        if (priceAdjustmentBase > decimal.Zero)
                            attributeValueModel.PriceAdjustment =
                                "+" + await _priceFormatter.FormatPriceAsync(priceAdjustment);
                        else if (priceAdjustmentBase < decimal.Zero)
                            attributeValueModel.PriceAdjustment =
                                "-" + await _priceFormatter.FormatPriceAsync(-priceAdjustment);
                    }
                }
            }

            //set already selected attributes
            var selectedCheckoutAttributes = await _genericAttributeService.GetAttributeAsync<string>(
                await _workContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.CheckoutAttributes, store.Id);
            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                case AttributeControlType.Checkboxes:
                case AttributeControlType.ColorSquares:
                case AttributeControlType.ImageSquares:
                {
					var __logStart = System.Diagnostics.Stopwatch.StartNew();
                    if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                    {
                        //clear default selection
                        foreach (var item in attributeModel.Values)
                            item.IsPreSelected = false;
							_logger.LogDebug("PrepareCheckoutAttributeModelsAsync(cart={Cart}): item.IsPreSelected → {Arg1}", cart, item.IsPreSelected);

                        //select new values
                        var selectedValues =
                            _checkoutAttributeParser.ParseAttributeValues(selectedCheckoutAttributes);
                        foreach (var attributeValue in await selectedValues.SelectMany(x => x.values).ToListAsync())
                        foreach (var item in attributeModel.Values)
                            if (attributeValue.Id == item.Id)
                                item.IsPreSelected = true;
                    }
					_logger.LogError("PrepareCheckoutAttributeModelsAsync(cart)={Cart}: database query executed in {Arg1} ms", cart, __logStart.ElapsedMilliseconds);
                }

                    break;
                case AttributeControlType.ReadonlyCheckboxes:
                {
                    //do nothing
                    //values are already pre-set
                }

                    break;
                case AttributeControlType.TextBox:
                case AttributeControlType.MultilineTextbox:
                {
                    if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                    {
                        var enteredText =
                            _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                        if (enteredText.Any())
                            attributeModel.DefaultValue = enteredText[0];
							_logger.LogDebug("PrepareCheckoutAttributeModelsAsync(cart={Cart}): attributeModel.DefaultValue → {Arg1}", cart, attributeModel.DefaultValue);
                    }
                }

                    break;
                case AttributeControlType.Datepicker:
                {
                    //keep in mind my that the code below works only in the current culture
                    var selectedDateStr =
                        _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                    if (selectedDateStr.Any())
                    {
                        if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                DateTimeStyles.None, out var selectedDate))
                        {
                            //successfully parsed
                            attributeModel.SelectedDay = selectedDate.Day;
							_logger.LogDebug("PrepareCheckoutAttributeModelsAsync(cart={Cart}): attributeModel.SelectedDay → {Arg1}", cart, attributeModel.SelectedDay);
							_logger.LogDebug("PrepareCheckoutAttributeModelsAsync(cart={Cart}): attributeModel.SelectedMonth → {Arg1}", cart, attributeModel.SelectedMonth);
							_logger.LogDebug("PrepareCheckoutAttributeModelsAsync(cart={Cart}): attributeModel.SelectedYear → {Arg1}", cart, attributeModel.SelectedYear);
                            attributeModel.SelectedMonth = selectedDate.Month;
                            attributeModel.SelectedYear = selectedDate.Year;
                        }
                    }
                }

                    break;
                case AttributeControlType.FileUpload:
                {
                    if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                    {
                        var downloadGuidStr = _checkoutAttributeParser
                            .ParseValues(selectedCheckoutAttributes, attribute.Id).FirstOrDefault();
                        _ = Guid.TryParse(downloadGuidStr, out var downloadGuid);
						_logger.LogError("PrepareCheckoutAttributeModelsAsync(cart={Cart}): _ → {_}", cart, _);
                        var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
						_logger.LogError("PrepareCheckoutAttributeModelsAsync(cart={Cart}): download → {Download}", cart, download);
                        if (download != null)
                            attributeModel.DefaultValue = download.DownloadGuid.ToString();
                    }
                }

                    break;
                default:
                    break;
            }

            model.Add(attributeModel);
        }

		_logger.LogWarning("<<< Exiting PrepareCheckoutAttributeModelsAsync(cart={Cart})", cart);
        return model;
    }

    /// <summary>
    /// Prepare the shopping cart item model
    /// </summary>
    /// <param name="cart">List of the shopping cart item</param>
    /// <param name="sci">Shopping cart item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping cart item model
    /// </returns>
    protected virtual async Task<ShoppingCartModel.ShoppingCartItemModel> PrepareShoppingCartItemModelAsync(IList<ShoppingCartItem> cart, ShoppingCartItem sci)
    {
		_logger.LogError(">>> Entering PrepareShoppingCartItemModelAsync(cart={Cart},sci={Sci})", cart, sci);
        ArgumentNullException.ThrowIfNull(cart);

        ArgumentNullException.ThrowIfNull(sci);

        var product = await _productService.GetProductByIdAsync(sci.ProductId);
		_logger.LogError("PrepareShoppingCartItemModelAsync(cart={Cart},sci={Sci}): product → {Product}", cart, sci, product);

        var cartItemModel = new ShoppingCartModel.ShoppingCartItemModel
        {
            Id = sci.Id,
            Sku = await _productService.FormatSkuAsync(product, sci.AttributesXml),
            VendorName = _vendorSettings.ShowVendorOnOrderDetailsPage ? (await _vendorService.GetVendorByProductIdAsync(product.Id))?.Name : string.Empty,
            ProductId = sci.ProductId,
            ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
            ProductSeName = await _urlRecordService.GetSeNameAsync(product),
            Quantity = sci.Quantity,
            AttributeInfo = await _productAttributeFormatter.FormatAttributesAsync(product, sci.AttributesXml),
        };

        //allow editing?
        //1. setting enabled?
        //2. simple product?
        //3. has attribute or gift card?
        //4. visible individually?
        cartItemModel.AllowItemEditing = _shoppingCartSettings.AllowCartItemEditing &&
                                         product.ProductType == ProductType.SimpleProduct &&
                                         (!string.IsNullOrEmpty(cartItemModel.AttributeInfo) ||
                                          product.IsGiftCard) &&
                                         product.VisibleIndividually;
										_logger.LogDebug("PrepareShoppingCartItemModelAsync(cart={Cart},sci={Sci}): cartItemModel.AllowItemEditing → {Arg2}", cart, sci, cartItemModel.AllowItemEditing);

        //disable removal?
        //1. do other items require this one?
        cartItemModel.DisableRemoval = (await _shoppingCartService.GetProductsRequiringProductAsync(cart, product)).Any();

        //allowed quantities
        var allowedQuantities = _productService.ParseAllowedQuantities(product);
		_logger.LogError("PrepareShoppingCartItemModelAsync(cart={Cart},sci={Sci}): allowedQuantities → {AllowedQuantities}", cart, sci, allowedQuantities);
        foreach (var qty in allowedQuantities)
        {
            cartItemModel.AllowedQuantities.Add(new SelectListItem
            {
                Text = qty.ToString(),
                Value = qty.ToString(),
                Selected = sci.Quantity == qty
            });
        }

        //recurring info
        if (product.IsRecurring)
            cartItemModel.RecurringInfo = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.RecurringPeriod"),
                product.RecurringCycleLength, await _localizationService.GetLocalizedEnumAsync(product.RecurringCyclePeriod));

        //rental info
        if (product.IsRental)
        {
            var rentalStartDate = sci.RentalStartDateUtc.HasValue
                ? _productService.FormatRentalDate(product, sci.RentalStartDateUtc.Value)
                : string.Empty;
            var rentalEndDate = sci.RentalEndDateUtc.HasValue
                ? _productService.FormatRentalDate(product, sci.RentalEndDateUtc.Value)
                : string.Empty;
            cartItemModel.RentalInfo =
                string.Format(await _localizationService.GetResourceAsync("ShoppingCart.Rental.FormattedDate"),
                    rentalStartDate, rentalEndDate);
        }

        //unit prices
        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
		_logger.LogError("PrepareShoppingCartItemModelAsync(cart={Cart},sci={Sci}): currentCurrency → {CurrentCurrency}", cart, sci, currentCurrency);
        if (product.CallForPrice &&
            //also check whether the current user is impersonated
            (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
        {
            cartItemModel.UnitPrice = await _localizationService.GetResourceAsync("Products.CallForPrice");
            cartItemModel.UnitPriceValue = 0;
			_logger.LogDebug("PrepareShoppingCartItemModelAsync(cart={Cart},sci={Sci}): cartItemModel.UnitPriceValue → {Arg2}", cart, sci, cartItemModel.UnitPriceValue);
        }
        else
        {
            var (shoppingCartUnitPriceWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, (await _shoppingCartService.GetUnitPriceAsync(sci, true)).unitPrice);
            var shoppingCartUnitPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartUnitPriceWithDiscountBase, currentCurrency);
			_logger.LogError("PrepareShoppingCartItemModelAsync(cart={Cart},sci={Sci}): shoppingCartUnitPriceWithDiscount → {ShoppingCartUnitPriceWithDiscount}", cart, sci, shoppingCartUnitPriceWithDiscount);
            cartItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(shoppingCartUnitPriceWithDiscount);
            cartItemModel.UnitPriceValue = shoppingCartUnitPriceWithDiscount;
        }
        //subtotal, discount
        if (product.CallForPrice &&
            //also check whether the current user is impersonated
            (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
        {
            cartItemModel.SubTotal = await _localizationService.GetResourceAsync("Products.CallForPrice");
            cartItemModel.SubTotalValue = 0;
			_logger.LogDebug("PrepareShoppingCartItemModelAsync(cart={Cart},sci={Sci}): cartItemModel.SubTotalValue → {Arg2}", cart, sci, cartItemModel.SubTotalValue);
        }
        else
        {
            //sub total
            var (subTotal, shoppingCartItemDiscountBase, _, maximumDiscountQty) = await _shoppingCartService.GetSubTotalAsync(sci, true);
            var (shoppingCartItemSubTotalWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, subTotal);
            var shoppingCartItemSubTotalWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartItemSubTotalWithDiscountBase, currentCurrency);
			_logger.LogError("PrepareShoppingCartItemModelAsync(cart={Cart},sci={Sci}): shoppingCartItemSubTotalWithDiscount → {ShoppingCartItemSubTotalWithDiscount}", cart, sci, shoppingCartItemSubTotalWithDiscount);
            cartItemModel.SubTotal = await _priceFormatter.FormatPriceAsync(shoppingCartItemSubTotalWithDiscount);
            cartItemModel.SubTotalValue = shoppingCartItemSubTotalWithDiscount;
            cartItemModel.MaximumDiscountedQty = maximumDiscountQty;
			_logger.LogDebug("PrepareShoppingCartItemModelAsync(cart={Cart},sci={Sci}): cartItemModel.MaximumDiscountedQty → {Arg2}", cart, sci, cartItemModel.MaximumDiscountedQty);

            //display an applied discount amount
            if (shoppingCartItemDiscountBase > decimal.Zero)
            {
                (shoppingCartItemDiscountBase, _) = await _taxService.GetProductPriceAsync(product, shoppingCartItemDiscountBase);
                if (shoppingCartItemDiscountBase > decimal.Zero)
                {
                    var shoppingCartItemDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartItemDiscountBase, currentCurrency);
					_logger.LogError("PrepareShoppingCartItemModelAsync(cart={Cart},sci={Sci}): shoppingCartItemDiscount → {ShoppingCartItemDiscount}", cart, sci, shoppingCartItemDiscount);
                    cartItemModel.Discount = await _priceFormatter.FormatPriceAsync(shoppingCartItemDiscount);
                    cartItemModel.DiscountValue = shoppingCartItemDiscount;
					_logger.LogDebug("PrepareShoppingCartItemModelAsync(cart={Cart},sci={Sci}): cartItemModel.DiscountValue → {Arg2}", cart, sci, cartItemModel.DiscountValue);
                }
            }
        }

        //picture
        if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
        {
            cartItemModel.Picture = await PrepareCartItemPictureModelAsync(sci,
                _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
        }

        //item warnings
        var itemWarnings = await _shoppingCartService.GetShoppingCartItemWarningsAsync(
            await _workContext.GetCurrentCustomerAsync(),
            sci.ShoppingCartType,
            product,
            sci.StoreId,
            sci.AttributesXml,
            sci.CustomerEnteredPrice,
            sci.RentalStartDateUtc,
            sci.RentalEndDateUtc,
            sci.Quantity,
            false,
            sci.Id);
        foreach (var warning in itemWarnings)
            cartItemModel.Warnings.Add(warning);

		_logger.LogWarning("<<< Exiting PrepareShoppingCartItemModelAsync(cart={Cart},sci={Sci})", cart, sci);
        return cartItemModel;
    }

    /// <summary>
    /// Prepare the wishlist item model
    /// </summary>
    /// <param name="sci">Shopping cart item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping cart item model
    /// </returns>
    protected virtual async Task<WishlistModel.ShoppingCartItemModel> PrepareWishlistItemModelAsync(ShoppingCartItem sci)
    {
		_logger.LogError(">>> Entering PrepareWishlistItemModelAsync(sci={Sci})", sci);
        ArgumentNullException.ThrowIfNull(sci);

        var product = await _productService.GetProductByIdAsync(sci.ProductId);
		_logger.LogError("PrepareWishlistItemModelAsync(sci={Sci}): product → {Product}", sci, product);

        var cartItemModel = new WishlistModel.ShoppingCartItemModel
        {
            Id = sci.Id,
            Sku = await _productService.FormatSkuAsync(product, sci.AttributesXml),
            ProductId = product.Id,
            ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
            ProductSeName = await _urlRecordService.GetSeNameAsync(product),
            Quantity = sci.Quantity,
            AttributeInfo = await _productAttributeFormatter.FormatAttributesAsync(product, sci.AttributesXml),
        };

        //allow editing?
        //1. setting enabled?
        //2. simple product?
        //3. has attribute or gift card?
        //4. visible individually?
        cartItemModel.AllowItemEditing = _shoppingCartSettings.AllowCartItemEditing &&
                                         product.ProductType == ProductType.SimpleProduct &&
                                         (!string.IsNullOrEmpty(cartItemModel.AttributeInfo) ||
                                          product.IsGiftCard) &&
                                         product.VisibleIndividually;
										_logger.LogDebug("PrepareWishlistItemModelAsync(sci={Sci}): cartItemModel.AllowItemEditing → {Arg1}", sci, cartItemModel.AllowItemEditing);

        //allowed quantities
        var allowedQuantities = _productService.ParseAllowedQuantities(product);
		_logger.LogError("PrepareWishlistItemModelAsync(sci={Sci}): allowedQuantities → {AllowedQuantities}", sci, allowedQuantities);
        foreach (var qty in allowedQuantities)
        {
            cartItemModel.AllowedQuantities.Add(new SelectListItem
            {
                Text = qty.ToString(),
                Value = qty.ToString(),
                Selected = sci.Quantity == qty
            });
        }

        //recurring info
        if (product.IsRecurring)
            cartItemModel.RecurringInfo = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.RecurringPeriod"),
                product.RecurringCycleLength, await _localizationService.GetLocalizedEnumAsync(product.RecurringCyclePeriod));

        //rental info
        if (product.IsRental)
        {
            var rentalStartDate = sci.RentalStartDateUtc.HasValue
                ? _productService.FormatRentalDate(product, sci.RentalStartDateUtc.Value)
                : string.Empty;
            var rentalEndDate = sci.RentalEndDateUtc.HasValue
                ? _productService.FormatRentalDate(product, sci.RentalEndDateUtc.Value)
                : string.Empty;
            cartItemModel.RentalInfo =
                string.Format(await _localizationService.GetResourceAsync("ShoppingCart.Rental.FormattedDate"),
                    rentalStartDate, rentalEndDate);
        }

        //unit prices
        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
		_logger.LogError("PrepareWishlistItemModelAsync(sci={Sci}): currentCurrency → {CurrentCurrency}", sci, currentCurrency);
        if (product.CallForPrice &&
            //also check whether the current user is impersonated
            (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
        {
            cartItemModel.UnitPrice = await _localizationService.GetResourceAsync("Products.CallForPrice");
            cartItemModel.UnitPriceValue = 0;
			_logger.LogDebug("PrepareWishlistItemModelAsync(sci={Sci}): cartItemModel.UnitPriceValue → {Arg1}", sci, cartItemModel.UnitPriceValue);
        }
        else
        {
            var (shoppingCartUnitPriceWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, (await _shoppingCartService.GetUnitPriceAsync(sci, true)).unitPrice);
            var shoppingCartUnitPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartUnitPriceWithDiscountBase, currentCurrency);
			_logger.LogError("PrepareWishlistItemModelAsync(sci={Sci}): shoppingCartUnitPriceWithDiscount → {ShoppingCartUnitPriceWithDiscount}", sci, shoppingCartUnitPriceWithDiscount);
            cartItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(shoppingCartUnitPriceWithDiscount);
            cartItemModel.UnitPriceValue = shoppingCartUnitPriceWithDiscount;
        }
        //subtotal, discount
        if (product.CallForPrice &&
            //also check whether the current user is impersonated
            (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
        {
            cartItemModel.SubTotal = await _localizationService.GetResourceAsync("Products.CallForPrice");
            cartItemModel.SubTotalValue = 0;
			_logger.LogDebug("PrepareWishlistItemModelAsync(sci={Sci}): cartItemModel.SubTotalValue → {Arg1}", sci, cartItemModel.SubTotalValue);
        }
        else
        {
            //sub total
            var (subTotal, shoppingCartItemDiscountBase, _, maximumDiscountQty) = await _shoppingCartService.GetSubTotalAsync(sci, true);
            var (shoppingCartItemSubTotalWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, subTotal);
            var shoppingCartItemSubTotalWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartItemSubTotalWithDiscountBase, currentCurrency);
			_logger.LogError("PrepareWishlistItemModelAsync(sci={Sci}): shoppingCartItemSubTotalWithDiscount → {ShoppingCartItemSubTotalWithDiscount}", sci, shoppingCartItemSubTotalWithDiscount);
            cartItemModel.SubTotal = await _priceFormatter.FormatPriceAsync(shoppingCartItemSubTotalWithDiscount);
            cartItemModel.SubTotalValue = shoppingCartItemSubTotalWithDiscount;
            cartItemModel.MaximumDiscountedQty = maximumDiscountQty;
			_logger.LogDebug("PrepareWishlistItemModelAsync(sci={Sci}): cartItemModel.MaximumDiscountedQty → {Arg1}", sci, cartItemModel.MaximumDiscountedQty);

            //display an applied discount amount
            if (shoppingCartItemDiscountBase > decimal.Zero)
            {
                (shoppingCartItemDiscountBase, _) = await _taxService.GetProductPriceAsync(product, shoppingCartItemDiscountBase);
                if (shoppingCartItemDiscountBase > decimal.Zero)
                {
                    var shoppingCartItemDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartItemDiscountBase, currentCurrency);
					_logger.LogError("PrepareWishlistItemModelAsync(sci={Sci}): shoppingCartItemDiscount → {ShoppingCartItemDiscount}", sci, shoppingCartItemDiscount);
                    cartItemModel.Discount = await _priceFormatter.FormatPriceAsync(shoppingCartItemDiscount);
                    cartItemModel.DiscountValue = shoppingCartItemDiscount;
					_logger.LogDebug("PrepareWishlistItemModelAsync(sci={Sci}): cartItemModel.DiscountValue → {Arg1}", sci, cartItemModel.DiscountValue);
                }
            }
        }

        //picture
        if (_shoppingCartSettings.ShowProductImagesOnWishList)
        {
            cartItemModel.Picture = await PrepareCartItemPictureModelAsync(sci,
                _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
        }

        //item warnings
        var itemWarnings = await _shoppingCartService.GetShoppingCartItemWarningsAsync(
            await _workContext.GetCurrentCustomerAsync(),
            sci.ShoppingCartType,
            product,
            sci.StoreId,
            sci.AttributesXml,
            sci.CustomerEnteredPrice,
            sci.RentalStartDateUtc,
            sci.RentalEndDateUtc,
            sci.Quantity,
            false,
            sci.Id);
        foreach (var warning in itemWarnings)
            cartItemModel.Warnings.Add(warning);

		_logger.LogWarning("<<< Exiting PrepareWishlistItemModelAsync(sci={Sci})", sci);
        return cartItemModel;
    }

    /// <summary>
    /// Prepare the order review data model
    /// </summary>
    /// <param name="cart">List of the shopping cart item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order review data model
    /// </returns>
    protected virtual async Task<ShoppingCartModel.OrderReviewDataModel> PrepareOrderReviewDataModelAsync(IList<ShoppingCartItem> cart)
    {
		_logger.LogError(">>> Entering PrepareOrderReviewDataModelAsync(cart={Cart})", cart);
        ArgumentNullException.ThrowIfNull(cart);

        var model = new ShoppingCartModel.OrderReviewDataModel
        {
            Display = true
        };

        //billing info
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareOrderReviewDataModelAsync(cart={Cart}): customer → {Customer}", cart, customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareOrderReviewDataModelAsync(cart={Cart}): store → {Store}", cart, store);
        var billingAddress = await _customerService.GetCustomerBillingAddressAsync(customer);
		_logger.LogError("PrepareOrderReviewDataModelAsync(cart={Cart}): billingAddress → {BillingAddress}", cart, billingAddress);
        if (billingAddress != null)
        {
            await _addressModelFactory.PrepareAddressModelAsync(model.BillingAddress,
                address: billingAddress,
                excludeProperties: false,
                addressSettings: _addressSettings);
        }

        //shipping info
        if (await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
        {
            model.IsShippable = true;
			_logger.LogDebug("PrepareOrderReviewDataModelAsync(cart={Cart}): model.IsShippable → {Arg1}", cart, model.IsShippable);

            var pickupPoint = await _genericAttributeService.GetAttributeAsync<PickupPoint>(customer,
                NopCustomerDefaults.SelectedPickupPointAttribute, store.Id);
            model.SelectedPickupInStore = _shippingSettings.AllowPickupInStore && pickupPoint != null;
            if (!model.SelectedPickupInStore)
            {
                if (await _customerService.GetCustomerShippingAddressAsync(customer) is Address address)
                {
                    await _addressModelFactory.PrepareAddressModelAsync(model.ShippingAddress,
                        address: address,
                        excludeProperties: false,
                        addressSettings: _addressSettings);
                }
            }
            else
            {
                var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(pickupPoint.CountryCode);
				_logger.LogError("PrepareOrderReviewDataModelAsync(cart={Cart}): country → {Country}", cart, country);
				_logger.LogDebug("PrepareOrderReviewDataModelAsync(cart={Cart}): model.PickupAddress → {Arg1}", cart, model.PickupAddress);
                var state = await _stateProvinceService.GetStateProvinceByAbbreviationAsync(pickupPoint.StateAbbreviation, country?.Id);
				_logger.LogError("PrepareOrderReviewDataModelAsync(cart={Cart}): state → {State}", cart, state);

                model.PickupAddress = new AddressModel
                {
                    Address1 = pickupPoint.Address,
                    City = pickupPoint.City,
                    County = pickupPoint.County,
                    CountryName = country?.Name ?? string.Empty,
                    StateProvinceName = state?.Name ?? string.Empty,
                    ZipPostalCode = pickupPoint.ZipPostalCode,
                    CountryId = country?.Id,
                    StateProvinceId = state?.Id
                };

                var address = new Address
                {
                    CountryId = model.PickupAddress.CountryId,
                    StateProvinceId = model.PickupAddress.StateProvinceId,
                    City = model.PickupAddress.City,
                    County = model.PickupAddress.County,
                    Address1 = model.PickupAddress.Address1,

                    ZipPostalCode = model.PickupAddress.ZipPostalCode
                };
                var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
				_logger.LogError("PrepareOrderReviewDataModelAsync(cart={Cart}): languageId → {LanguageId}", cart, languageId);
                var (_, addressFields) = await _addressService.FormatAddressAsync(address, languageId);

                model.PickupAddress.AddressFields = addressFields;
            }

            //selected shipping method
            var shippingOption = await _genericAttributeService.GetAttributeAsync<ShippingOption>(customer,
                NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);
            if (shippingOption != null)
                model.ShippingMethod = shippingOption.Name;
				_logger.LogDebug("PrepareOrderReviewDataModelAsync(cart={Cart}): model.ShippingMethod → {Arg1}", cart, model.ShippingMethod);
        }

        //payment info
        var selectedPaymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
		_logger.LogError("PrepareOrderReviewDataModelAsync(cart={Cart}): selectedPaymentMethodSystemName → {SelectedPaymentMethodSystemName}", cart, selectedPaymentMethodSystemName);
        var paymentMethod = await _paymentPluginManager
            .LoadPluginBySystemNameAsync(selectedPaymentMethodSystemName, customer, store.Id);
        model.PaymentMethod = paymentMethod != null
            ? await _localizationService.GetLocalizedFriendlyNameAsync(paymentMethod, (await _workContext.GetWorkingLanguageAsync()).Id)
            : string.Empty;

        //custom values
        var processPaymentRequest = await _orderProcessingService.GetProcessPaymentRequestAsync();
		_logger.LogError("PrepareOrderReviewDataModelAsync(cart={Cart}): processPaymentRequest → {ProcessPaymentRequest}", cart, processPaymentRequest);
        model.CustomValues.AddRange(processPaymentRequest?.CustomValues?.Where(value => value.DisplayToCustomer).ToList() ?? new());

		_logger.LogWarning("<<< Exiting PrepareOrderReviewDataModelAsync(cart={Cart})", cart);
        return model;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the estimate shipping model
    /// </summary>
    /// <param name="cart">List of the shopping cart item</param>
    /// <param name="setEstimateShippingDefaultAddress">Whether to use customer default shipping address for estimating</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the estimate shipping model
    /// </returns>
    public virtual async Task<EstimateShippingModel> PrepareEstimateShippingModelAsync(IList<ShoppingCartItem> cart, bool setEstimateShippingDefaultAddress = true)
    {
		_logger.LogError(">>> Entering PrepareEstimateShippingModelAsync(cart={Cart},setEstimateShippingDefaultAddress=<redacted>)", cart);
        ArgumentNullException.ThrowIfNull(cart);

        var model = new EstimateShippingModel
        {
            RequestDelay = _shippingSettings.RequestDelay,
            UseCity = _shippingSettings.EstimateShippingCityNameEnabled,
            Enabled = cart.Any() && await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart)
        };
        if (model.Enabled)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogError("PrepareEstimateShippingModelAsync(cart={Cart},setEstimateShippingDefaultAddress=<redacted>): customer → {Customer}", cart, customer);
            var shippingAddress = await _customerService.GetCustomerShippingAddressAsync(customer) ?? await (await _customerService.GetAddressesByCustomerIdAsync(customer.Id))
                //enabled for the current store
                .FirstOrDefaultAwaitAsync(async a => a.CountryId == null || await _storeMappingService.AuthorizeAsync(await _countryService.GetCountryByAddressAsync(a)));

            //countries
            var defaultEstimateCountryId = (setEstimateShippingDefaultAddress && shippingAddress != null)
                ? shippingAddress.CountryId
                : model.CountryId;
            model.AvailableCountries.Add(new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Address.SelectCountry"),
                Value = "0"
				_logger.LogDebug("PrepareEstimateShippingModelAsync(cart={Cart},setEstimateShippingDefaultAddress=<redacted>): Value → {Value}", cart, Value);
            });

            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
			_logger.LogError("PrepareEstimateShippingModelAsync(cart={Cart},setEstimateShippingDefaultAddress=<redacted>): currentLanguage → {CurrentLanguage}", cart, currentLanguage);
            foreach (var c in await _countryService.GetAllCountriesForShippingAsync(currentLanguage.Id))
                model.AvailableCountries.Add(new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                    Value = c.Id.ToString(),
                    Selected = c.Id == defaultEstimateCountryId
                });

            //states
            var defaultEstimateStateId = (setEstimateShippingDefaultAddress && shippingAddress != null)
                ? shippingAddress.StateProvinceId
                : model.StateProvinceId;
            var states = defaultEstimateCountryId.HasValue
                ? (await _stateProvinceService.GetStateProvincesByCountryIdAsync(defaultEstimateCountryId.Value, currentLanguage.Id)).ToList()
                : new List<StateProvince>();
            if (states.Any())
            {
                foreach (var s in states)
                {
                    model.AvailableStates.Add(new SelectListItem
                    {
                        Text = await _localizationService.GetLocalizedAsync(s, x => x.Name),
                        Value = s.Id.ToString(),
                        Selected = s.Id == defaultEstimateStateId
                    });
                }
            }
            else
            {
                model.AvailableStates.Add(new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Address.Other"),
                    Value = "0"
                });
            }

            if (setEstimateShippingDefaultAddress && shippingAddress != null)
            {
                if (!_shippingSettings.EstimateShippingCityNameEnabled)
                    model.ZipPostalCode = shippingAddress.ZipPostalCode;
                else
                    model.City = shippingAddress.City;
					_logger.LogDebug("PrepareEstimateShippingModelAsync(cart={Cart},setEstimateShippingDefaultAddress=<redacted>): model.ZipPostalCode → {Arg2}", cart, model.ZipPostalCode);
					_logger.LogDebug("PrepareEstimateShippingModelAsync(cart={Cart},setEstimateShippingDefaultAddress=<redacted>): model.City → {Arg2}", cart, model.City);
            }
        }

		_logger.LogWarning("<<< Exiting PrepareEstimateShippingModelAsync(cart={Cart},setEstimateShippingDefaultAddress=<redacted>)", cart);
        return model;
    }

    /// <summary>
    /// Prepare the shopping cart model
    /// </summary>
    /// <param name="model">Shopping cart model</param>
    /// <param name="cart">List of the shopping cart item</param>
    /// <param name="isEditable">Whether model is editable</param>
    /// <param name="validateCheckoutAttributes">Whether to validate checkout attributes</param>
    /// <param name="prepareAndDisplayOrderReviewData">Whether to prepare and display order review data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping cart model
    /// </returns>
    public virtual async Task<ShoppingCartModel> PrepareShoppingCartModelAsync(ShoppingCartModel model,
        IList<ShoppingCartItem> cart, bool isEditable = true,
        bool validateCheckoutAttributes = false,
        bool prepareAndDisplayOrderReviewData = false)
    {
        ArgumentNullException.ThrowIfNull(cart);

        ArgumentNullException.ThrowIfNull(model);

        //simple properties
        model.OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled;

        if (!cart.Any())
            return model;

        model.IsEditable = isEditable;
        model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
        model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
        model.ShowVendorName = _vendorSettings.ShowVendorOnOrderDetailsPage;
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var checkoutAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(customer,
            NopCustomerDefaults.CheckoutAttributes, store.Id);
        var minOrderSubtotalAmountOk = await _orderProcessingService.ValidateMinOrderSubtotalAmountAsync(cart);
        if (!minOrderSubtotalAmountOk)
        {
            var minOrderSubtotalAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(_orderSettings.MinOrderSubtotalAmount, await _workContext.GetWorkingCurrencyAsync());
            model.MinOrderSubtotalWarning = string.Format(await _localizationService.GetResourceAsync("Checkout.MinOrderSubtotalAmount"), await _priceFormatter.FormatPriceAsync(minOrderSubtotalAmount, true, false));
        }

        model.TermsOfServiceOnShoppingCartPage = _orderSettings.TermsOfServiceOnShoppingCartPage;
        model.TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage;
        model.TermsOfServicePopup = _commonSettings.PopupForTermsOfServiceLinks;
        model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoShoppingCart;

        //discount and gift card boxes
        model.DiscountBox.Display = _shoppingCartSettings.ShowDiscountBox;
        var discountCouponCodes = await _customerService.ParseAppliedDiscountCouponCodesAsync(customer);

        foreach (var couponCode in discountCouponCodes)
        {
            var discount = await (await _discountService.GetAllDiscountsAsync(couponCode: couponCode))
                .FirstOrDefaultAwaitAsync(async d => d.RequiresCouponCode && (await _discountService.ValidateDiscountAsync(d, customer, discountCouponCodes)).IsValid);

            if (discount != null)
            {
                model.DiscountBox.AppliedDiscountsWithCodes.Add(new ShoppingCartModel.DiscountBoxModel.DiscountInfoModel
                {
                    Id = discount.Id,
                    CouponCode = discount.CouponCode
                });
            }
        }

        model.GiftCardBox.Display = _shoppingCartSettings.ShowGiftCardBox;

        //cart warnings
        var cartWarnings = await _shoppingCartService.GetShoppingCartWarningsAsync(cart, checkoutAttributesXml, validateCheckoutAttributes);
        foreach (var warning in cartWarnings)
            model.Warnings.Add(warning);

        //checkout attributes
        model.CheckoutAttributes = await PrepareCheckoutAttributeModelsAsync(cart);

        //cart items
        foreach (var sci in cart)
        {
            var cartItemModel = await PrepareShoppingCartItemModelAsync(cart, sci);
            model.Items.Add(cartItemModel);
        }

        //payment methods
        //all payment methods (do not filter by country here as it could be not specified yet)
        var paymentMethods = await (await _paymentPluginManager
                .LoadActivePluginsAsync(customer, store.Id))
            .WhereAwait(async pm => !await pm.HidePaymentMethodAsync(cart)).ToListAsync();
        //payment methods displayed during checkout (not with "Button" type)
        var nonButtonPaymentMethods = paymentMethods
            .Where(pm => pm.PaymentMethodType != PaymentMethodType.Button)
            .ToList();
        //"button" payment methods(*displayed on the shopping cart page)
        var buttonPaymentMethods = paymentMethods
            .Where(pm => pm.PaymentMethodType == PaymentMethodType.Button)
            .ToList();
        foreach (var pm in buttonPaymentMethods)
        {
            if (await _shoppingCartService.ShoppingCartIsRecurringAsync(cart) && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                continue;

            var viewComponent = pm.GetPublicViewComponent();
            model.ButtonPaymentMethodViewComponents.Add(viewComponent);
        }
        //hide "Checkout" button if we have only "Button" payment methods
        model.HideCheckoutButton = !nonButtonPaymentMethods.Any() && model.ButtonPaymentMethodViewComponents.Any();

        //order review data
        if (prepareAndDisplayOrderReviewData)
        {
            model.OrderReviewData = await PrepareOrderReviewDataModelAsync(cart);
        }

		_logger.LogWarning("<<< Exiting PrepareShoppingCartModelAsync(model={Model},cart={Cart},isEditable={IsEditable},validateCheckoutAttributes={ValidateCheckoutAttributes},prepareAndDisplayOrderReviewData={PrepareAndDisplayOrderReviewData})", model, cart, isEditable, validateCheckoutAttributes, prepareAndDisplayOrderReviewData);
        return model;
    }

    /// <summary>
    /// Prepare the wishlist model
    /// </summary>
    /// <param name="model">Wishlist model</param>
    /// <param name="cart">List of the shopping cart item</param>
    /// <param name="isEditable">Whether model is editable</param>
    /// <param name="list">Custom wishlist identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the wishlist model
    /// </returns>
    public virtual async Task<WishlistModel> PrepareWishlistModelAsync(WishlistModel model, IList<ShoppingCartItem> cart, bool isEditable = true, int? list = null)
    {
		_logger.LogError(">>> Entering PrepareWishlistModelAsync(model={Model},cart={Cart},isEditable={IsEditable},list={List})", model, cart, isEditable, list);
        ArgumentNullException.ThrowIfNull(cart);
        ArgumentNullException.ThrowIfNull(model);

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareWishlistModelAsync(model={Model},cart={Cart},isEditable={IsEditable},list={List}): currentCustomer → {CurrentCustomer}", model, cart, isEditable, list, currentCustomer);
        var isGuest = await _customerService.IsGuestAsync(currentCustomer);
		_logger.LogError("PrepareWishlistModelAsync(model={Model},cart={Cart},isEditable={IsEditable},list={List}): isGuest → {IsGuest}", model, cart, isEditable, list, isGuest);

        model.EmailWishlistEnabled = _shoppingCartSettings.EmailWishlistEnabled;
		_logger.LogDebug("PrepareWishlistModelAsync(model={Model},cart={Cart},isEditable={IsEditable},list={List}): model.EmailWishlistEnabled → {Arg4}", model, cart, isEditable, list, model.EmailWishlistEnabled);
		_logger.LogDebug("PrepareWishlistModelAsync(model={Model},cart={Cart},isEditable={IsEditable},list={List}): model.ListId → {Arg4}", model, cart, isEditable, list, model.ListId);
		_logger.LogDebug("PrepareWishlistModelAsync(model={Model},cart={Cart},isEditable={IsEditable},list={List}): model.AllowMultipleWishlist → {Arg4}", model, cart, isEditable, list, model.AllowMultipleWishlist);
		_logger.LogDebug("PrepareWishlistModelAsync(model={Model},cart={Cart},isEditable={IsEditable},list={List}): model.IsEditable → {Arg4}", model, cart, isEditable, list, model.IsEditable);
        model.ListId = list;
        model.AllowMultipleWishlist = _shoppingCartSettings.AllowMultipleWishlist && !isGuest;
        model.IsEditable = isEditable;
        model.DisplayAddToCart = await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART);
        model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoWishlist;
		_logger.LogDebug("PrepareWishlistModelAsync(model={Model},cart={Cart},isEditable={IsEditable},list={List}): model.DisplayTaxShippingInfo → {Arg4}", model, cart, isEditable, list, model.DisplayTaxShippingInfo);

        //custom wishlist items
        var currentWishlists = await _customWishlistService.GetAllCustomWishlistsAsync(currentCustomer.Id);
		_logger.LogError("PrepareWishlistModelAsync(model={Model},cart={Cart},isEditable={IsEditable},list={List}): currentWishlists → {CurrentWishlists}", model, cart, isEditable, list, currentWishlists);
        foreach (var wishlist in currentWishlists)
        {
            var customWishlistModel = new CustomWishlistModel
            {
                Id = wishlist.Id,
                Name = wishlist.Name
            };
            model.CustomWishlistItems.Add(customWishlistModel);
        }

        if (list != null)
        {
            var customWishlist = await _customWishlistService.GetCustomWishlistByIdAsync(list.Value);
			_logger.LogError("PrepareWishlistModelAsync(model={Model},cart={Cart},isEditable={IsEditable},list={List}): customWishlist → {CustomWishlist}", model, cart, isEditable, list, customWishlist);
            if (customWishlist != null)
                model.CustomWishlistName = customWishlist.Name;
				_logger.LogDebug("PrepareWishlistModelAsync(model={Model},cart={Cart},isEditable={IsEditable},list={List}): model.CustomWishlistName → {Arg4}", model, cart, isEditable, list, model.CustomWishlistName);
        }

        if (!cart.Any())
            return model;

        //simple properties
        var customer = await _customerService.GetShoppingCartCustomerAsync(cart);
		_logger.LogError("PrepareWishlistModelAsync(model={Model},cart={Cart},isEditable={IsEditable},list={List}): customer → {Customer}", model, cart, isEditable, list, customer);

        model.CustomerGuid = customer.CustomerGuid;
		_logger.LogDebug("PrepareWishlistModelAsync(model={Model},cart={Cart},isEditable={IsEditable},list={List}): model.CustomerGuid → {Arg4}", model, cart, isEditable, list, model.CustomerGuid);
        model.CustomerFullname = await _customerService.GetCustomerFullNameAsync(customer);
        model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnWishList;
		_logger.LogDebug("PrepareWishlistModelAsync(model={Model},cart={Cart},isEditable={IsEditable},list={List}): model.ShowProductImages → {Arg4}", model, cart, isEditable, list, model.ShowProductImages);
		_logger.LogDebug("PrepareWishlistModelAsync(model={Model},cart={Cart},isEditable={IsEditable},list={List}): model.ShowSku → {Arg4}", model, cart, isEditable, list, model.ShowSku);
        model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;

        //cart warnings
        var cartWarnings = await _shoppingCartService.GetShoppingCartWarningsAsync(cart, string.Empty, false);
		_logger.LogError("PrepareWishlistModelAsync(model={Model},cart={Cart},isEditable={IsEditable},list={List}): cartWarnings → {CartWarnings}", model, cart, isEditable, list, cartWarnings);
        foreach (var warning in cartWarnings)
            model.Warnings.Add(warning);

        //cart items
        foreach (var sci in cart)
        {
            var cartItemModel = await PrepareWishlistItemModelAsync(sci);
			_logger.LogError("PrepareWishlistModelAsync(model={Model},cart={Cart},isEditable={IsEditable},list={List}): cartItemModel → {CartItemModel}", model, cart, isEditable, list, cartItemModel);
            model.Items.Add(cartItemModel);
        }

        return model;
    }

    /// <summary>
    /// Prepare the mini shopping cart model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the mini shopping cart model
    /// </returns>
    public virtual async Task<MiniShoppingCartModel> PrepareMiniShoppingCartModelAsync()
    {
		_logger.LogError(">>> Entering PrepareMiniShoppingCartModelAsync()");
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareMiniShoppingCartModelAsync(): customer → {Customer}", customer);
        var model = new MiniShoppingCartModel
        {
            ShowProductImages = _shoppingCartSettings.ShowProductImagesInMiniShoppingCart,
            //let's always display it
            DisplayShoppingCartButton = true,
            CurrentCustomerIsGuest = await _customerService.IsGuestAsync(customer),
            AnonymousCheckoutAllowed = _orderSettings.AnonymousCheckoutAllowed,
        };

        //performance optimization (use "HasShoppingCartItems" property)
        if (customer.HasShoppingCartItems)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogError("PrepareMiniShoppingCartModelAsync(): store → {Store}", store);
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
			_logger.LogError("PrepareMiniShoppingCartModelAsync(): cart → {Cart}", cart);

            if (cart.Any())
            {
                model.TotalProducts = cart.Sum(item => item.Quantity);

                //subtotal
                var subTotalIncludingTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
				_logger.LogError("PrepareMiniShoppingCartModelAsync(): subTotalIncludingTax → {SubTotalIncludingTax}", subTotalIncludingTax);
                var (_, _, subTotalWithoutDiscountBase, _, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, subTotalIncludingTax);
                var subtotalBase = subTotalWithoutDiscountBase;
                var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
				_logger.LogError("PrepareMiniShoppingCartModelAsync(): currentCurrency → {CurrentCurrency}", currentCurrency);
                var subtotal = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(subtotalBase, currentCurrency);
				_logger.LogError("PrepareMiniShoppingCartModelAsync(): subtotal → {Subtotal}", subtotal);
                model.SubTotal = await _priceFormatter.FormatPriceAsync(subtotal, false, currentCurrency, (await _workContext.GetWorkingLanguageAsync()).Id, subTotalIncludingTax);
                model.SubTotalValue = subtotal;
				_logger.LogDebug("PrepareMiniShoppingCartModelAsync(): model.SubTotalValue → {Arg0}", model.SubTotalValue);

                var requiresShipping = await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
				_logger.LogError("PrepareMiniShoppingCartModelAsync(): requiresShipping → {RequiresShipping}", requiresShipping);
                //a customer should visit the shopping cart page (hide checkout button) before going to checkout if:
                //1. "terms of service" are enabled
                //2. min order sub-total is OK
                //3. we have at least one checkout attribute
                var checkoutAttributesExist = (await _checkoutAttributeService
                        .GetAllAttributesAsync(_staticCacheManager, _storeMappingService, store.Id, !requiresShipping))
                    .Any();

                var minOrderSubtotalAmountOk = await _orderProcessingService.ValidateMinOrderSubtotalAmountAsync(cart);
				_logger.LogError("PrepareMiniShoppingCartModelAsync(): minOrderSubtotalAmountOk → {MinOrderSubtotalAmountOk}", minOrderSubtotalAmountOk);

                var cartProductIds = cart.Select(ci => ci.ProductId).ToArray();
				_logger.LogError("PrepareMiniShoppingCartModelAsync(): cartProductIds → {CartProductIds}", cartProductIds);

                var downloadableProductsRequireRegistration =
                    _customerSettings.RequireRegistrationForDownloadableProducts && await _productService.HasAnyDownloadableProductAsync(cartProductIds);

                model.DisplayCheckoutButton = !_orderSettings.TermsOfServiceOnShoppingCartPage &&
                                              minOrderSubtotalAmountOk &&
                                              !checkoutAttributesExist &&
                                              !(downloadableProductsRequireRegistration
                                                && await _customerService.IsGuestAsync(customer));
												_logger.LogDebug("PrepareMiniShoppingCartModelAsync(): model.DisplayCheckoutButton → {Arg0}", model.DisplayCheckoutButton);

                //products. sort descending (recently added products)
                foreach (var sci in cart
                             .OrderByDescending(x => x.Id)
                             .Take(_shoppingCartSettings.MiniShoppingCartProductNumber)
                             .ToList())
                {
                    var product = await _productService.GetProductByIdAsync(sci.ProductId);
					_logger.LogError("PrepareMiniShoppingCartModelAsync(): product → {Product}", product);

                    var cartItemModel = new MiniShoppingCartModel.ShoppingCartItemModel
                    {
                        Id = sci.Id,
                        ProductId = sci.ProductId,
                        ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                        ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                        Quantity = sci.Quantity,
                        AttributeInfo = await _productAttributeFormatter.FormatAttributesAsync(product, sci.AttributesXml)
                    };

                    //unit prices
                    if (product.CallForPrice &&
                        //also check whether the current user is impersonated
                        (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
                    {
                        cartItemModel.UnitPrice = await _localizationService.GetResourceAsync("Products.CallForPrice");
                        cartItemModel.UnitPriceValue = 0;
						_logger.LogDebug("PrepareMiniShoppingCartModelAsync(): cartItemModel.UnitPriceValue → {Arg0}", cartItemModel.UnitPriceValue);
                    }
                    else
                    {
                        var (shoppingCartUnitPriceWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, (await _shoppingCartService.GetUnitPriceAsync(sci, true)).unitPrice);
                        var shoppingCartUnitPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartUnitPriceWithDiscountBase, currentCurrency);
						_logger.LogError("PrepareMiniShoppingCartModelAsync(): shoppingCartUnitPriceWithDiscount → {ShoppingCartUnitPriceWithDiscount}", shoppingCartUnitPriceWithDiscount);
                        cartItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(shoppingCartUnitPriceWithDiscount);
                        cartItemModel.UnitPriceValue = shoppingCartUnitPriceWithDiscount;
                    }

                    //picture
                    if (_shoppingCartSettings.ShowProductImagesInMiniShoppingCart)
                    {
                        cartItemModel.Picture = await PrepareCartItemPictureModelAsync(sci,
                            _mediaSettings.MiniCartThumbPictureSize, true, cartItemModel.ProductName);
                    }

                    model.Items.Add(cartItemModel);
                }
            }
        }

		_logger.LogWarning("<<< Exiting PrepareMiniShoppingCartModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare selected checkout attributes
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the formatted attributes
    /// </returns>
    public virtual async Task<string> FormatSelectedCheckoutAttributesAsync()
    {
		_logger.LogError(">>> Entering FormatSelectedCheckoutAttributesAsync()");
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("FormatSelectedCheckoutAttributesAsync(): store → {Store}", store);
        var checkoutAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(customer,
            NopCustomerDefaults.CheckoutAttributes, store.Id);
			_logger.LogError("FormatSelectedCheckoutAttributesAsync(): customer → {Customer}", customer);

		_logger.LogWarning("<<< Exiting FormatSelectedCheckoutAttributesAsync()");
        return await _checkoutAttributeFormatter.FormatAttributesAsync(checkoutAttributesXml, customer);
    }

    /// <summary>
    /// Prepare the order totals model
    /// </summary>
    /// <param name="cart">List of the shopping cart item</param>
    /// <param name="isEditable">Whether model is editable</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order totals model
    /// </returns>
    public virtual async Task<OrderTotalsModel> PrepareOrderTotalsModelAsync(IList<ShoppingCartItem> cart, bool isEditable)
    {
		_logger.LogError(">>> Entering PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable})", cart, isEditable);
        var model = new OrderTotalsModel
        {
            IsEditable = isEditable
        };

        if (cart.Any())
        {
            //subtotal
            var subTotalIncludingTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
			_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): subTotalIncludingTax → {SubTotalIncludingTax}", cart, isEditable, subTotalIncludingTax);
            var (orderSubTotalDiscountAmountBase, _, subTotalWithoutDiscountBase, _, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, subTotalIncludingTax);
            var subtotalBase = subTotalWithoutDiscountBase;
            var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
			_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): currentCurrency → {CurrentCurrency}", cart, isEditable, currentCurrency);
            var subtotal = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(subtotalBase, currentCurrency);
			_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): subtotal → {Subtotal}", cart, isEditable, subtotal);
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
			_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): currentLanguage → {CurrentLanguage}", cart, isEditable, currentLanguage);
            model.SubTotal = await _priceFormatter.FormatPriceAsync(subtotal, true, currentCurrency, currentLanguage.Id, subTotalIncludingTax);

            if (orderSubTotalDiscountAmountBase > decimal.Zero)
            {
                var orderSubTotalDiscountAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(orderSubTotalDiscountAmountBase, currentCurrency);
				_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): orderSubTotalDiscountAmount → {OrderSubTotalDiscountAmount}", cart, isEditable, orderSubTotalDiscountAmount);
                model.SubTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderSubTotalDiscountAmount, true, currentCurrency, currentLanguage.Id, subTotalIncludingTax);
            }

            //shipping info
            model.RequiresShipping = await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): customer → {Customer}", cart, isEditable, customer);
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): store → {Store}", cart, isEditable, store);
            if (model.RequiresShipping)
            {
                var shoppingCartShippingBase = await _orderTotalCalculationService.GetShoppingCartShippingTotalAsync(cart);
				_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): shoppingCartShippingBase → {ShoppingCartShippingBase}", cart, isEditable, shoppingCartShippingBase);
                if (shoppingCartShippingBase.HasValue)
                {
                    var shoppingCartShipping = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartShippingBase.Value, currentCurrency);
					_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): shoppingCartShipping → {ShoppingCartShipping}", cart, isEditable, shoppingCartShipping);
                    model.Shipping = await _priceFormatter.FormatShippingPriceAsync(shoppingCartShipping, true);

                    //selected shipping method
                    var shippingOption = await _genericAttributeService.GetAttributeAsync<ShippingOption>(customer,
                        NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);
                    if (shippingOption != null)
                        model.SelectedShippingMethod = shippingOption.Name;
						_logger.LogDebug("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): model.SelectedShippingMethod → {Arg2}", cart, isEditable, model.SelectedShippingMethod);
                }
            }
            else
            {
                model.HideShippingTotal = _shippingSettings.HideShippingTotal;
				_logger.LogDebug("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): model.HideShippingTotal → {Arg2}", cart, isEditable, model.HideShippingTotal);
            }

            //payment method fee
            var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
			_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): paymentMethodSystemName → {PaymentMethodSystemName}", cart, isEditable, paymentMethodSystemName);
            var paymentMethodAdditionalFee = await _paymentService.GetAdditionalHandlingFeeAsync(cart, paymentMethodSystemName);
			_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): paymentMethodAdditionalFee → {PaymentMethodAdditionalFee}", cart, isEditable, paymentMethodAdditionalFee);
            var (paymentMethodAdditionalFeeWithTaxBase, _) = await _taxService.GetPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFee, customer);
            if (paymentMethodAdditionalFeeWithTaxBase > decimal.Zero)
            {
                var paymentMethodAdditionalFeeWithTax = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(paymentMethodAdditionalFeeWithTaxBase, currentCurrency);
				_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): paymentMethodAdditionalFeeWithTax → {PaymentMethodAdditionalFeeWithTax}", cart, isEditable, paymentMethodAdditionalFeeWithTax);
                model.PaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFeeWithTax, true);
            }

            //tax
            bool displayTax;
            bool displayTaxRates;
            if (_taxSettings.HideTaxInOrderSummary && await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
				_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): displayTax → {DisplayTax}", cart, isEditable, displayTax);
                displayTaxRates = false;
				_logger.LogDebug("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): displayTaxRates → {DisplayTaxRates}", cart, isEditable, displayTaxRates);
            }
            else
            {
				_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): displayTaxRates → {DisplayTaxRates}", cart, isEditable, displayTaxRates);
				_logger.LogDebug("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): displayTax → {DisplayTax}", cart, isEditable, displayTax);
                var (shoppingCartTaxBase, taxRates) = await _orderTotalCalculationService.GetTaxTotalAsync(cart);
                var shoppingCartTax = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartTaxBase, currentCurrency);
				_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): shoppingCartTax → {ShoppingCartTax}", cart, isEditable, shoppingCartTax);

                if (shoppingCartTaxBase == 0 && _taxSettings.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                    displayTax = !displayTaxRates;

                    model.Tax = await _priceFormatter.FormatPriceAsync(shoppingCartTax, true, false);
                    foreach (var tr in taxRates)
                    {
                        model.TaxRates.Add(new OrderTotalsModel.TaxRate
                        {
                            Rate = _priceFormatter.FormatTaxRate(tr.Key),
                            Value = await _priceFormatter.FormatPriceAsync(await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(tr.Value, currentCurrency), true, false),
                        });
                    }
                }
            }

            model.DisplayTaxRates = displayTaxRates;
			_logger.LogDebug("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): model.DisplayTaxRates → {Arg2}", cart, isEditable, model.DisplayTaxRates);
			_logger.LogDebug("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): model.DisplayTax → {Arg2}", cart, isEditable, model.DisplayTax);
            model.DisplayTax = displayTax;

            //total
            var (shoppingCartTotalBase, orderTotalDiscountAmountBase, _, appliedGiftCards, redeemedRewardPoints, redeemedRewardPointsAmount) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart);
            if (shoppingCartTotalBase.HasValue)
            {
                var shoppingCartTotal = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartTotalBase.Value, currentCurrency);
				_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): shoppingCartTotal → {ShoppingCartTotal}", cart, isEditable, shoppingCartTotal);
                model.OrderTotal = await _priceFormatter.FormatPriceAsync(shoppingCartTotal, true, false);
            }

            //discount
            if (orderTotalDiscountAmountBase > decimal.Zero)
            {
                var orderTotalDiscountAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(orderTotalDiscountAmountBase, currentCurrency);
				_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): orderTotalDiscountAmount → {OrderTotalDiscountAmount}", cart, isEditable, orderTotalDiscountAmount);
                model.OrderTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderTotalDiscountAmount, true, false);
            }

            //gift cards
            if (appliedGiftCards != null && appliedGiftCards.Any())
            {
                foreach (var appliedGiftCard in appliedGiftCards)
                {
                    var gcModel = new OrderTotalsModel.GiftCard
                    {
                        Id = appliedGiftCard.GiftCard.Id,
                        CouponCode = appliedGiftCard.GiftCard.GiftCardCouponCode,
                    };
                    var amountCanBeUsed = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(appliedGiftCard.AmountCanBeUsed, currentCurrency);
					_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): amountCanBeUsed → {AmountCanBeUsed}", cart, isEditable, amountCanBeUsed);
                    gcModel.Amount = await _priceFormatter.FormatPriceAsync(-amountCanBeUsed, true, false);

                    var remainingAmountBase = await _giftCardService.GetGiftCardRemainingAmountAsync(appliedGiftCard.GiftCard) - appliedGiftCard.AmountCanBeUsed;
					_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): remainingAmountBase → {RemainingAmountBase}", cart, isEditable, remainingAmountBase);
                    var remainingAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(remainingAmountBase, currentCurrency);
					_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): remainingAmount → {RemainingAmount}", cart, isEditable, remainingAmount);
                    gcModel.Remaining = await _priceFormatter.FormatPriceAsync(remainingAmount, true, false);

                    model.GiftCards.Add(gcModel);
                }
            }

            //reward points to be spent (redeemed)
            if (redeemedRewardPointsAmount > decimal.Zero)
            {
                var redeemedRewardPointsAmountInCustomerCurrency = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(redeemedRewardPointsAmount, currentCurrency);
				_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): redeemedRewardPointsAmountInCustomerCurrency → {RedeemedRewardPointsAmountInCustomerCurrency}", cart, isEditable, redeemedRewardPointsAmountInCustomerCurrency);
				_logger.LogDebug("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): model.RedeemedRewardPoints → {Arg2}", cart, isEditable, model.RedeemedRewardPoints);
                model.RedeemedRewardPoints = redeemedRewardPoints;
                model.RedeemedRewardPointsAmount = await _priceFormatter.FormatPriceAsync(-redeemedRewardPointsAmountInCustomerCurrency, true, false);
            }

            //reward points to be earned
            if (_rewardPointsSettings.Enabled && _rewardPointsSettings.DisplayHowMuchWillBeEarned && shoppingCartTotalBase.HasValue)
            {
                //get shipping total
                var shippingBaseInclTax = !model.RequiresShipping ? 0 : (await _orderTotalCalculationService.GetShoppingCartShippingTotalAsync(cart, true)).shippingTotal ?? 0;
				_logger.LogError("PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable}): shippingBaseInclTax → {ShippingBaseInclTax}", cart, isEditable, shippingBaseInclTax);

                //get total for reward points
                var totalForRewardPoints = _orderTotalCalculationService
                    .CalculateApplicableOrderTotalForRewardPoints(shippingBaseInclTax, shoppingCartTotalBase.Value);
                if (totalForRewardPoints > decimal.Zero)
                    model.WillEarnRewardPoints = await _orderTotalCalculationService.CalculateRewardPointsAsync(customer, totalForRewardPoints);
            }
        }

		_logger.LogWarning("<<< Exiting PrepareOrderTotalsModelAsync(cart={Cart},isEditable={IsEditable})", cart, isEditable);
        return model;
    }

    /// <summary>
    /// Prepare the estimate shipping result model
    /// </summary>
    /// <param name="cart">List of the shopping cart item</param>
    /// <param name="request">Request to get shipping options</param>
    /// <param name="cacheShippingOptions">Indicates whether to cache offered shipping options</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the estimate shipping result model
    /// </returns>
    public virtual async Task<EstimateShippingResultModel> PrepareEstimateShippingResultModelAsync(IList<ShoppingCartItem> cart, EstimateShippingModel request, bool cacheShippingOptions)
    {
		_logger.LogError(">>> Entering PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>)", cart, request);
        ArgumentNullException.ThrowIfNull(request);

        var model = new EstimateShippingResultModel();
		_logger.LogError("PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>): model → {Model}", cart, request, model);

        if (await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
        {
            var address = new Address
            {
                CountryId = request.CountryId,
                StateProvinceId = request.StateProvinceId,
                ZipPostalCode = request.ZipPostalCode,
                City = request.City
            };

            var rawShippingOptions = new List<ShippingOption>();
			_logger.LogError("PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>): rawShippingOptions → {RawShippingOptions}", cart, request, rawShippingOptions);
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogError("PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>): customer → {Customer}", cart, request, customer);
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogError("PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>): store → {Store}", cart, request, store);
            var getShippingOptionResponse = await _shippingService.GetShippingOptionsAsync(cart, address, customer, storeId: store.Id);
			_logger.LogError("PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>): getShippingOptionResponse → {GetShippingOptionResponse}", cart, request, getShippingOptionResponse);
            if (getShippingOptionResponse.Success)
            {
                if (getShippingOptionResponse.ShippingOptions.Any())
                {
                    foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                    {
                        rawShippingOptions.Add(new ShippingOption
                        {
                            Name = shippingOption.Name,
                            Description = shippingOption.Description,
                            Rate = shippingOption.Rate,
                            TransitDays = shippingOption.TransitDays,
                            ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName
                        });
						_logger.LogDebug("PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>): ShippingRateComputationMethodSystemName → {ShippingRateComputationMethodSystemName}", cart, request, ShippingRateComputationMethodSystemName);
                    }
                }
            }
            else
            {
                foreach (var error in getShippingOptionResponse.Errors)
                    model.Errors.Add(error);
            }

            var pickupPointsNumber = 0;
			_logger.LogError("PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>): pickupPointsNumber → {PickupPointsNumber}", cart, request, pickupPointsNumber);
            if (_shippingSettings.AllowPickupInStore)
            {
                var pickupPointsResponse = await _shippingService.GetPickupPointsAsync(cart, address,
                    customer, storeId: store.Id);
                if (pickupPointsResponse.Success)
                {
                    if (pickupPointsResponse.PickupPoints.Any())
                    {
                        pickupPointsNumber = pickupPointsResponse.PickupPoints.Count;
                        var pickupPoint = pickupPointsResponse.PickupPoints.OrderBy(p => p.PickupFee).First();
						_logger.LogError("PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>): pickupPoint → {PickupPoint}", cart, request, pickupPoint);

                        rawShippingOptions.Add(new ShippingOption
                        {
                            Name = await _localizationService.GetResourceAsync("Checkout.PickupPoints"),
                            Description = await _localizationService.GetResourceAsync("Checkout.PickupPoints.Description"),
                            Rate = pickupPoint.PickupFee,
                            TransitDays = pickupPoint.TransitDays,
                            ShippingRateComputationMethodSystemName = pickupPoint.ProviderSystemName,
                            IsPickupInStore = true
                        });
						_logger.LogDebug("PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>): IsPickupInStore → {IsPickupInStore}", cart, request, IsPickupInStore);
                    }
                }
                else
                {
                    foreach (var error in pickupPointsResponse.Errors)
                        model.Errors.Add(error);
                }
            }

            ShippingOption selectedShippingOption = null;
            if (cacheShippingOptions)
            {
                //performance optimization. cache returned shipping options.
                //we'll use them later (after a customer has selected an option).
                await _genericAttributeService.SaveAttributeAsync(customer,
                    NopCustomerDefaults.OfferedShippingOptionsAttribute,
                    rawShippingOptions,
                    store.Id);

                //find a selected (previously) shipping option
                selectedShippingOption = await _genericAttributeService.GetAttributeAsync<ShippingOption>(customer,
                    NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);
            }

            if (rawShippingOptions.Any())
            {
                foreach (var option in rawShippingOptions)
                {
                    var (shippingRate, _) = await _orderTotalCalculationService.AdjustShippingRateAsync(option.Rate, cart, option.IsPickupInStore);
                    (shippingRate, _) = await _taxService.GetShippingPriceAsync(shippingRate, customer);
                    shippingRate = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shippingRate, await _workContext.GetWorkingCurrencyAsync());
					_logger.LogError("PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>): shippingRate → {ShippingRate}", cart, request, shippingRate);
					_logger.LogDebug("PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>): shippingRate → {ShippingRate}", cart, request, shippingRate);
                    var shippingRateString = await _priceFormatter.FormatShippingPriceAsync(shippingRate, true);
					_logger.LogError("PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>): shippingRateString → {ShippingRateString}", cart, request, shippingRateString);

                    if (option.IsPickupInStore && pickupPointsNumber > 1)
                        shippingRateString = string.Format(await _localizationService.GetResourceAsync("Shipping.EstimateShippingPopUp.Pickup.PriceFrom"), shippingRateString);

                    string deliveryDateFormat = null;
					_logger.LogError("PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>): deliveryDateFormat → {DeliveryDateFormat}", cart, request, deliveryDateFormat);
                    if (option.TransitDays.HasValue)
                    {
                        var currentCulture = CultureInfo.GetCultureInfo((await _workContext.GetWorkingLanguageAsync()).LanguageCulture);
						_logger.LogError("PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>): currentCulture → {CurrentCulture}", cart, request, currentCulture);
                        var customerDateTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
						_logger.LogError("PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>): customerDateTime → {CustomerDateTime}", cart, request, customerDateTime);
                        deliveryDateFormat = customerDateTime.AddDays(option.TransitDays.Value).ToString("d", currentCulture);
                    }

                    var selected = selectedShippingOption != null &&
                                   !string.IsNullOrEmpty(option.ShippingRateComputationMethodSystemName) &&
                                   option.ShippingRateComputationMethodSystemName.Equals(selectedShippingOption.ShippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase) &&
                                   (!string.IsNullOrEmpty(option.Name) &&
                                    option.Name.Equals(selectedShippingOption.Name, StringComparison.InvariantCultureIgnoreCase) ||
                                    (option.IsPickupInStore && option.IsPickupInStore == selectedShippingOption.IsPickupInStore));

                    model.ShippingOptions.Add(new EstimateShippingResultModel.ShippingOptionModel
                    {
                        Name = option.Name,
                        ShippingRateComputationMethodSystemName = option.ShippingRateComputationMethodSystemName,
                        Description = option.Description,
                        DisplayOrder = option.DisplayOrder ?? 0,
                        Price = shippingRateString,
                        Rate = shippingRate,
                        DeliveryDateFormat = deliveryDateFormat,
                        Selected = selected
                    });
					_logger.LogDebug("PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>): Selected → {Selected}", cart, request, Selected);
                }

                //sort shipping methods
                if (model.ShippingOptions.Count > 1)
                {
                    model.ShippingOptions = (_shippingSettings.ShippingSorting switch
                    {
                        ShippingSortingEnum.ShippingCost => model.ShippingOptions.OrderBy(option => option.Rate),
                        _ => model.ShippingOptions.OrderBy(option => option.DisplayOrder)
                    }).ToList();
                }

                //if no option has been selected, let's do it for the first one
                if (!model.ShippingOptions.Any(so => so.Selected))
                    model.ShippingOptions.First().Selected = true;
            }
        }

		_logger.LogWarning("<<< Exiting PrepareEstimateShippingResultModelAsync(cart={Cart},request={Request},cacheShippingOptions=<redacted>)", cart, request);
        return model;
    }

    /// <summary>
    /// Prepare the wishlist email a friend model
    /// </summary>
    /// <param name="model">Wishlist email a friend model</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <param name="wishlistId">Custom wishlist identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the wishlist email a friend model
    /// </returns>
    public virtual async Task<WishlistEmailAFriendModel> PrepareWishlistEmailAFriendModelAsync(WishlistEmailAFriendModel model, bool excludeProperties, int? wishlistId = null)
    {
		_logger.LogError(">>> Entering PrepareWishlistEmailAFriendModelAsync(model={Model},excludeProperties={ExcludeProperties},wishlistId={WishlistId})", model, excludeProperties, wishlistId);
        ArgumentNullException.ThrowIfNull(model);

        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailWishlistToFriendPage;
		_logger.LogDebug("PrepareWishlistEmailAFriendModelAsync(model={Model},excludeProperties={ExcludeProperties},wishlistId={WishlistId}): model.DisplayCaptcha → {Arg3}", model, excludeProperties, wishlistId, model.DisplayCaptcha);
        if (!excludeProperties)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogError("PrepareWishlistEmailAFriendModelAsync(model={Model},excludeProperties={ExcludeProperties},wishlistId={WishlistId}): customer → {Customer}", model, excludeProperties, wishlistId, customer);
			_logger.LogDebug("PrepareWishlistEmailAFriendModelAsync(model={Model},excludeProperties={ExcludeProperties},wishlistId={WishlistId}): model.YourEmailAddress → {Arg3}", model, excludeProperties, wishlistId, model.YourEmailAddress);
            model.YourEmailAddress = customer.Email;
        }

        model.ListId = wishlistId;
		_logger.LogDebug("PrepareWishlistEmailAFriendModelAsync(model={Model},excludeProperties={ExcludeProperties},wishlistId={WishlistId}): model.ListId → {Arg3}", model, excludeProperties, wishlistId, model.ListId);

		_logger.LogWarning("<<< Exiting PrepareWishlistEmailAFriendModelAsync(model={Model},excludeProperties={ExcludeProperties},wishlistId={WishlistId})", model, excludeProperties, wishlistId);
        return model;
    }

    /// <summary>
    /// Prepare the cart item picture model
    /// </summary>
    /// <param name="sci">Shopping cart item</param>
    /// <param name="pictureSize">Picture size</param>
    /// <param name="showDefaultPicture">Whether to show the default picture</param>
    /// <param name="productName">Product name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture model
    /// </returns>
    public virtual async Task<PictureModel> PrepareCartItemPictureModelAsync(ShoppingCartItem sci, int pictureSize, bool showDefaultPicture, string productName)
    {
		_logger.LogError(">>> Entering PrepareCartItemPictureModelAsync(sci={Sci},pictureSize={PictureSize},showDefaultPicture={ShowDefaultPicture},productName={ProductName})", sci, pictureSize, showDefaultPicture, productName);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var model = await _shortTermCacheManager.GetAsync(async () =>
        {
            var product = await _productService.GetProductByIdAsync(sci.ProductId);
			_logger.LogError("PrepareCartItemPictureModelAsync(sci={Sci},pictureSize={PictureSize},showDefaultPicture={ShowDefaultPicture},productName={ProductName}): product → {Product}", sci, pictureSize, showDefaultPicture, productName, product);

            //shopping cart item picture
            var sciPicture = await _pictureService.GetProductPictureAsync(product, sci.AttributesXml);
			_logger.LogError("PrepareCartItemPictureModelAsync(sci={Sci},pictureSize={PictureSize},showDefaultPicture={ShowDefaultPicture},productName={ProductName}): sciPicture → {SciPicture}", sci, pictureSize, showDefaultPicture, productName, sciPicture);

            return new PictureModel
            {
                ImageUrl = (await _pictureService.GetPictureUrlAsync(sciPicture, pictureSize, showDefaultPicture)).Url,
                FullSizeImageUrl = (await _pictureService.GetPictureUrlAsync(sciPicture)).Url,
                Title = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat"), productName),
                AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat"), productName),
            };
        }, NopModelCacheDefaults.CartPictureModelKey, sci, pictureSize, true, await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());

		_logger.LogError("PrepareCartItemPictureModelAsync(sci,pictureSize,showDefaultPicture,productName)={Sci},{PictureSize},{ShowDefaultPicture},{ProductName}: external service call took {Arg4} ms", sci, pictureSize, showDefaultPicture, productName, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting PrepareCartItemPictureModelAsync(sci={Sci},pictureSize={PictureSize},showDefaultPicture={ShowDefaultPicture},productName={ProductName})", sci, pictureSize, showDefaultPicture, productName);
        return model;
    }

    #endregion
}