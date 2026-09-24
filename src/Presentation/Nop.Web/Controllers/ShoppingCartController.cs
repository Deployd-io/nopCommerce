using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Http;
using Nop.Core.Http.Extensions;
using Nop.Core.Infrastructure;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Components;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class ShoppingCartController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
    protected readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly ICustomWishlistService _customWishlistService;
    protected readonly IDiscountService _discountService;
    protected readonly IDownloadService _downloadService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IGiftCardService _giftCardService;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPictureService _pictureService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;
    protected readonly IShippingService _shippingService;
    protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly ITaxService _taxService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly MediaSettings _mediaSettings;
    protected readonly OrderSettings _orderSettings;
    protected readonly ShoppingCartSettings _shoppingCartSettings;
    protected readonly ShippingSettings _shippingSettings;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public ShoppingCartController(CaptchaSettings captchaSettings,
        CustomerSettings customerSettings,
        IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
        IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService,
        ICurrencyService currencyService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        ICustomWishlistService customWishlistService,
        IDiscountService discountService,
        IDownloadService downloadService,
        IGenericAttributeService genericAttributeService,
        IGiftCardService giftCardService,
        IHtmlFormatter htmlFormatter,
        ILocalizationService localizationService,
        INopFileProvider fileProvider,
        INopUrlHelper nopUrlHelper,
        INotificationService notificationService,
        IPermissionService permissionService,
        IPictureService pictureService,
        IPriceFormatter priceFormatter,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IProductService productService,
        IShippingService shippingService,
        IShoppingCartModelFactory shoppingCartModelFactory,
        IShoppingCartService shoppingCartService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        ITaxService taxService,
        IWebHelper webHelper,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        MediaSettings mediaSettings,
        OrderSettings orderSettings,
        ShoppingCartSettings shoppingCartSettings,
        ShippingSettings shippingSettings)
    {
        _captchaSettings = captchaSettings;
        _customerSettings = customerSettings;
        _checkoutAttributeParser = checkoutAttributeParser;
        _checkoutAttributeService = checkoutAttributeService;
        _currencyService = currencyService;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _customWishlistService = customWishlistService;
        _discountService = discountService;
        _downloadService = downloadService;
        _genericAttributeService = genericAttributeService;
        _giftCardService = giftCardService;
        _htmlFormatter = htmlFormatter;
        _localizationService = localizationService;
        _fileProvider = fileProvider;
        _nopUrlHelper = nopUrlHelper;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _pictureService = pictureService;
        _priceFormatter = priceFormatter;
        _productAttributeParser = productAttributeParser;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _shippingService = shippingService;
        _shoppingCartModelFactory = shoppingCartModelFactory;
        _shoppingCartService = shoppingCartService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _taxService = taxService;
        _webHelper = webHelper;
        _workContext = workContext;
        _workflowMessageService = workflowMessageService;
        _mediaSettings = mediaSettings;
        _orderSettings = orderSettings;
        _shoppingCartSettings = shoppingCartSettings;
        _shippingSettings = shippingSettings;
    }

    #endregion

    #region Utilities

    protected virtual async Task ParseAndSaveCheckoutAttributesAsync(IList<ShoppingCartItem> cart, IFormCollection form)
    {
		_logger.LogError(">>> Entering ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form})", cart, form);
        ArgumentNullException.ThrowIfNull(cart);

        ArgumentNullException.ThrowIfNull(form);

        var attributesXml = string.Empty;
		_logger.LogError("ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form}): attributesXml → {AttributesXml}", cart, form, attributesXml);
        var excludeShippableAttributes = !await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
		_logger.LogError("ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form}): excludeShippableAttributes → {ExcludeShippableAttributes}", cart, form, excludeShippableAttributes);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form}): store → {Store}", cart, form, store);
        var checkoutAttributes = await _checkoutAttributeService.GetAllAttributesAsync(_staticCacheManager, _storeMappingService, store.Id, excludeShippableAttributes);
		_logger.LogError("ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form}): checkoutAttributes → {CheckoutAttributes}", cart, form, checkoutAttributes);
        foreach (var attribute in checkoutAttributes)
        {
            var controlId = $"checkout_attribute_{attribute.Id}";
			_logger.LogError("ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form}): controlId → {ControlId}", cart, form, controlId);
            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                case AttributeControlType.ColorSquares:
                case AttributeControlType.ImageSquares:
                {
                    var ctrlAttributes = form[controlId];
                    if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                    {
                        var selectedAttributeId = int.Parse(ctrlAttributes);
						_logger.LogError("ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form}): selectedAttributeId → {SelectedAttributeId}", cart, form, selectedAttributeId);
                        if (selectedAttributeId > 0)
                            attributesXml = _checkoutAttributeParser.AddAttribute(attributesXml,
                                attribute, selectedAttributeId.ToString());
                    }
                }

                break;
                case AttributeControlType.Checkboxes:
                {
                    var cblAttributes = form[controlId];
                    if (!StringValues.IsNullOrEmpty(cblAttributes))
                    {
                        foreach (var item in cblAttributes.ToString().Split(_separator, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var selectedAttributeId = int.Parse(item);
                            if (selectedAttributeId > 0)
                                attributesXml = _checkoutAttributeParser.AddAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                        }
                    }
                }

                break;
                case AttributeControlType.ReadonlyCheckboxes:
                {
                    //load read-only (already server-side selected) values
                    var attributeValues = await _checkoutAttributeService.GetAttributeValuesAsync(attribute.Id);
					_logger.LogError("ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form}): attributeValues → {AttributeValues}", cart, form, attributeValues);
                    foreach (var selectedAttributeId in attributeValues
                                 .Where(v => v.IsPreSelected)
                                 .Select(v => v.Id)
                                 .ToList())
                    {
                        attributesXml = _checkoutAttributeParser.AddAttribute(attributesXml,
                            attribute, selectedAttributeId.ToString());
                    }
                }

                break;
                case AttributeControlType.TextBox:
                case AttributeControlType.MultilineTextbox:
                {
                    var ctrlAttributes = form[controlId];
                    if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                    {
                        var enteredText = ctrlAttributes.ToString().Trim();
						_logger.LogError("ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form}): enteredText → {EnteredText}", cart, form, enteredText);
                        attributesXml = _checkoutAttributeParser.AddAttribute(attributesXml,
                            attribute, enteredText);
                    }
                }

                break;
                case AttributeControlType.Datepicker:
                {
                    var date = form[controlId + "_day"];
					_logger.LogError("ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form}): date → {Date}", cart, form, date);
                    var month = form[controlId + "_month"];
					_logger.LogError("ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form}): month → {Month}", cart, form, month);
                    var year = form[controlId + "_year"];
					_logger.LogError("ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form}): year → {Year}", cart, form, year);
                    DateTime? selectedDate = null;
                    try
                    {
                        selectedDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(date));
						_logger.LogError("ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form}): selectedDate → {SelectedDate}", cart, form, selectedDate);
                    }
                    catch
                    {
						_logger.LogError("Exception in ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form}): {Arg2}", cart, form, ex.Message);
						_logger.LogDebug("ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form}): selectedDate → {SelectedDate}", cart, form, selectedDate);
                        // ignored
                    }

                    if (selectedDate.HasValue)
                        attributesXml = _checkoutAttributeParser.AddAttribute(attributesXml,
                            attribute, selectedDate.Value.ToString("D"));
                }

                break;
                case AttributeControlType.FileUpload:
                {
                    _ = Guid.TryParse(form[controlId], out var downloadGuid);
					_logger.LogError("ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form}): _ → {_}", cart, form, _);
                    var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
					_logger.LogError("ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form}): download → {Download}", cart, form, download);
                    if (download != null)
                    {
                        attributesXml = _checkoutAttributeParser.AddAttribute(attributesXml,
                            attribute, download.DownloadGuid.ToString());
                    }
                }

                break;
                default:
                    break;
            }
        }

        //validate conditional attributes (if specified)
        foreach (var attribute in checkoutAttributes)
        {
            var conditionMet = await _checkoutAttributeParser.IsConditionMetAsync(attribute.ConditionAttributeXml, attributesXml);
			_logger.LogError("ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form}): conditionMet → {ConditionMet}", cart, form, conditionMet);
            if (conditionMet.HasValue && !conditionMet.Value)
                attributesXml = _checkoutAttributeParser.RemoveAttribute(attributesXml, attribute.Id);
        }

        //save checkout attributes
        await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.CheckoutAttributes, attributesXml, store.Id);
		_logger.LogWarning("<<< Exiting ParseAndSaveCheckoutAttributesAsync(cart={Cart},form={Form})", cart, form);
    }

    protected virtual async Task SaveItemAsync(ShoppingCartItem updatecartitem, List<string> addToCartWarnings, Product product,
        ShoppingCartType cartType, string attributes, decimal customerEnteredPriceConverted, DateTime? rentalStartDate,
        DateTime? rentalEndDate, int quantity)
    {
		_logger.LogError(">>> Entering SaveItemAsync(updatecartitem={Updatecartitem},addToCartWarnings={AddToCartWarnings},product={Product},cartType={CartType},attributes={Attributes},customerEnteredPriceConverted={CustomerEnteredPriceConverted},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity})", updatecartitem, addToCartWarnings, product, cartType, attributes, customerEnteredPriceConverted, rentalStartDate, rentalEndDate, quantity);
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("SaveItemAsync(updatecartitem={Updatecartitem},addToCartWarnings={AddToCartWarnings},product={Product},cartType={CartType},attributes={Attributes},customerEnteredPriceConverted={CustomerEnteredPriceConverted},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity}): customer → {Customer}", updatecartitem, addToCartWarnings, product, cartType, attributes, customerEnteredPriceConverted, rentalStartDate, rentalEndDate, quantity, customer);
        if (updatecartitem == null)
        {
			_logger.LogError("SaveItemAsync(updatecartitem={Updatecartitem},addToCartWarnings={AddToCartWarnings},product={Product},cartType={CartType},attributes={Attributes},customerEnteredPriceConverted={CustomerEnteredPriceConverted},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity}): store → {Store}", updatecartitem, addToCartWarnings, product, cartType, attributes, customerEnteredPriceConverted, rentalStartDate, rentalEndDate, quantity, store);
            //add to the cart
            addToCartWarnings.AddRange(await _shoppingCartService.AddToCartAsync(customer,
                product, cartType, store.Id,
                attributes, customerEnteredPriceConverted,
                rentalStartDate, rentalEndDate, quantity, true));
        }
        else
        {
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, updatecartitem.ShoppingCartType, store.Id);
			_logger.LogError("SaveItemAsync(updatecartitem={Updatecartitem},addToCartWarnings={AddToCartWarnings},product={Product},cartType={CartType},attributes={Attributes},customerEnteredPriceConverted={CustomerEnteredPriceConverted},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity}): cart → {Cart}", updatecartitem, addToCartWarnings, product, cartType, attributes, customerEnteredPriceConverted, rentalStartDate, rentalEndDate, quantity, cart);

            var otherCartItemWithSameParameters = await _shoppingCartService.FindShoppingCartItemInTheCartAsync(
                cart, updatecartitem.ShoppingCartType, product, attributes, customerEnteredPriceConverted,
                rentalStartDate, rentalEndDate);
            if (otherCartItemWithSameParameters != null &&
                otherCartItemWithSameParameters.Id == updatecartitem.Id)
            {
                //ensure it's some other shopping cart item
                otherCartItemWithSameParameters = null;
            }
            //update existing item
            addToCartWarnings.AddRange(await _shoppingCartService.UpdateShoppingCartItemAsync(customer,
                updatecartitem.Id, attributes, customerEnteredPriceConverted,
                rentalStartDate, rentalEndDate, quantity + (otherCartItemWithSameParameters?.Quantity ?? 0), true));
            if (otherCartItemWithSameParameters != null && !addToCartWarnings.Any())
            {
                //delete the same shopping cart item (the other one)
                await _shoppingCartService.DeleteShoppingCartItemAsync(otherCartItemWithSameParameters);
            }
        }
		_logger.LogWarning("<<< Exiting SaveItemAsync(updatecartitem={Updatecartitem},addToCartWarnings={AddToCartWarnings},product={Product},cartType={CartType},attributes={Attributes},customerEnteredPriceConverted={CustomerEnteredPriceConverted},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity})", updatecartitem, addToCartWarnings, product, cartType, attributes, customerEnteredPriceConverted, rentalStartDate, rentalEndDate, quantity);
    }

    protected virtual async Task<IActionResult> GetProductToCartDetailsAsync(List<string> addToCartWarnings, ShoppingCartType cartType,
        Product product)
    {
        if (addToCartWarnings.Any())
        {
            //cannot be added to the cart/wishlist
            //let's display warnings
            return Json(new
            {
                success = false,
                message = addToCartWarnings.ToArray()
            });
        }

        //added to the cart/wishlist
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        switch (cartType)
        {
            case ShoppingCartType.Wishlist:
            {
                var wishlistRouteUrl = Url.RouteUrl(NopRouteNames.General.WISHLIST);
                //activity log
                await _customerActivityService.InsertActivityAsync("PublicStore.AddToWishlist",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddToWishlist"), product.Name), product);

                if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct)
                {
                    //redirect to the wishlist page
                    return Json(new
                    {
                        redirect = wishlistRouteUrl
                    });
                }

                //display notification message and update appropriate blocks
                var shoppingCarts = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, store.Id);

                var updateTopWishlistSectionHtml = string.Format(
                    await _localizationService.GetResourceAsync("Wishlist.HeaderQuantity"),
                    shoppingCarts.Sum(item => item.Quantity));

                var customWishlists = await _customWishlistService.GetAllCustomWishlistsAsync(customer.Id);
                var isGuest = await _customerService.IsGuestAsync(customer);

                if (!isGuest && customWishlists.Any())
                {
                    var onclick = $"showMoveToWishlistModal({product.Id}); return false;";
                    return Json(new
                    {
                        success = true,
                        message = string.Format(await _localizationService.GetResourceAsync("Products.ProductHasBeenAddedToTheWishlistAndMoved.Link"), wishlistRouteUrl, onclick),
                        updatetopwishlistsectionhtml = updateTopWishlistSectionHtml
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        message = string.Format(await _localizationService.GetResourceAsync("Products.ProductHasBeenAddedToTheWishlist.Link"), wishlistRouteUrl),
                        updatetopwishlistsectionhtml = updateTopWishlistSectionHtml
                    });
                }
            }

            case ShoppingCartType.ShoppingCart:
            default:
            {
                //activity log
                await _customerActivityService.InsertActivityAsync("PublicStore.AddToShoppingCart",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddToShoppingCart"), product.Name), product);

                if (_shoppingCartSettings.DisplayCartAfterAddingProduct)
                {
                    //redirect to the shopping cart page
                    return Json(new
                    {
                        redirect = Url.RouteUrl(NopRouteNames.General.CART)
                    });
                }

                //display notification message and update appropriate blocks
                var shoppingCarts = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

                var updateTopCartSectionHtml = string.Format(
                    await _localizationService.GetResourceAsync("ShoppingCart.HeaderQuantity"),
                    shoppingCarts.Sum(item => item.Quantity));

                var updateFlyoutCartSectionHtml = _shoppingCartSettings.MiniShoppingCartEnabled
                    ? await RenderViewComponentToStringAsync(typeof(FlyoutShoppingCartViewComponent))
                    : string.Empty;

                return Json(new
                {
                    success = true,
                    message = string.Format(await _localizationService.GetResourceAsync("Products.ProductHasBeenAddedToTheCart.Link"),
                        Url.RouteUrl(NopRouteNames.General.CART)),
                    updatetopcartsectionhtml = updateTopCartSectionHtml,
                    updateflyoutcartsectionhtml = updateFlyoutCartSectionHtml
                });
            }
        }
    }

    protected virtual async Task<string> GetGiftCardValidationErrorAsync(IList<ShoppingCartItem> cart, string giftcardcouponcode)
    {
		_logger.LogError(">>> Entering GetGiftCardValidationErrorAsync(cart={Cart},giftcardcouponcode={Giftcardcouponcode})", cart, giftcardcouponcode);
        if (string.IsNullOrWhiteSpace(giftcardcouponcode))
            return await _localizationService.GetResourceAsync("ShoppingCart.GiftCardCouponCode.WrongGiftCard");

        if (await _shoppingCartService.ShoppingCartIsRecurringAsync(cart))
            return await _localizationService.GetResourceAsync("ShoppingCart.GiftCardCouponCode.DontWorkWithAutoshipProducts");

        var giftCard = (await _giftCardService.GetAllGiftCardsAsync(giftCardCouponCode: giftcardcouponcode)).FirstOrDefault();
		_logger.LogError("GetGiftCardValidationErrorAsync(cart={Cart},giftcardcouponcode={Giftcardcouponcode}): giftCard → {GiftCard}", cart, giftcardcouponcode, giftCard);

        if (giftCard == null || !await _giftCardService.IsGiftCardValidAsync(giftCard))
            return await _localizationService.GetResourceAsync("ShoppingCart.GiftCardCouponCode.WrongGiftCard");

        if (await _productService.HasAnyGiftCardProductAsync(cart.Select(c => c.ProductId).ToArray()))
            return await _localizationService.GetResourceAsync("ShoppingCart.GiftCardCouponCode.DontWorkWithGiftCards");

        return string.Empty;
    }

    #endregion

    #region Shopping cart

    [HttpPost]
    public virtual async Task<IActionResult> SelectShippingOption([FromQuery] string name, [FromQuery] EstimateShippingModel model, IFormCollection form)
    {
		_logger.LogError(">>> Entering SelectShippingOption(name={Name},model={Model},form={Form})", name, model, form);
        if (model == null)
            model = new EstimateShippingModel();
			_logger.LogError("SelectShippingOption(name={Name},model={Model},form={Form}): model → {Model1}", name, model, form, model);

        var errors = new List<string>();
		_logger.LogError("SelectShippingOption(name={Name},model={Model},form={Form}): errors → {Errors}", name, model, form, errors);
        if (string.IsNullOrEmpty(model.ZipPostalCode) && !_shippingSettings.EstimateShippingCityNameEnabled)
            errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShipping.ZipPostalCode.Required"));

        if (model.CountryId == null || model.CountryId == 0)
            errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShipping.Country.Required"));

        if (errors.Count > 0)
            return Json(new
            {
                success = false,
                errors = errors
            });

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("SelectShippingOption(name={Name},model={Model},form={Form}): customer → {Customer}", name, model, form, customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("SelectShippingOption(name={Name},model={Model},form={Form}): store → {Store}", name, model, form, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogError("SelectShippingOption(name={Name},model={Model},form={Form}): cart → {Cart}", name, model, form, cart);
        //parse and save checkout attributes
        await ParseAndSaveCheckoutAttributesAsync(cart, form);

        var shippingOptions = new List<ShippingOption>();
		_logger.LogError("SelectShippingOption(name={Name},model={Model},form={Form}): shippingOptions → {ShippingOptions}", name, model, form, shippingOptions);
        ShippingOption selectedShippingOption = null;
		_logger.LogError("SelectShippingOption(name={Name},model={Model},form={Form}): selectedShippingOption → {SelectedShippingOption}", name, model, form, selectedShippingOption);

        if (!string.IsNullOrWhiteSpace(name))
        {
            //find shipping options
            //performance optimization. try cache first
            shippingOptions = await _genericAttributeService.GetAttributeAsync<List<ShippingOption>>(customer,
                NopCustomerDefaults.OfferedShippingOptionsAttribute, store.Id);

            if (shippingOptions == null || !shippingOptions.Any())
            {
                var address = new Address
                {
                    CountryId = model.CountryId,
                    StateProvinceId = model.StateProvinceId,
                    ZipPostalCode = model.ZipPostalCode,
                };

                //not found? let's load them using shipping service
                var getShippingOptionResponse = await _shippingService.GetShippingOptionsAsync(cart, address,
                    customer, storeId: store.Id);

                if (getShippingOptionResponse.Success)
                    shippingOptions = getShippingOptionResponse.ShippingOptions.ToList();
                else
                    foreach (var error in getShippingOptionResponse.Errors)
                        errors.Add(error);
            }
        }

        selectedShippingOption = shippingOptions.Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        if (selectedShippingOption == null)
            errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShippingPopUp.ShippingOption.IsNotFound"));

        if (errors.Count > 0)
            return Json(new
            {
                success = false,
                errors = errors
            });

        //reset pickup point
        await _genericAttributeService.SaveAttributeAsync<PickupPoint>(customer,
            NopCustomerDefaults.SelectedPickupPointAttribute, null, store.Id);

        //cache shipping option
        await _genericAttributeService.SaveAttributeAsync(customer,
            NopCustomerDefaults.SelectedShippingOptionAttribute, selectedShippingOption, store.Id);

        var orderTotalsSectionHtml = await RenderViewComponentToStringAsync(typeof(OrderTotalsViewComponent), new { isEditable = true });
		_logger.LogError("SelectShippingOption(name={Name},model={Model},form={Form}): orderTotalsSectionHtml → {OrderTotalsSectionHtml}", name, model, form, orderTotalsSectionHtml);

		_logger.LogWarning("<<< Exiting SelectShippingOption(name={Name},model={Model},form={Form})", name, model, form);
        return Json(new
        {
            success = true,
            ordertotalssectionhtml = orderTotalsSectionHtml
        });
    }

    //add product to cart using AJAX
    //currently we use this method on catalog pages (category/manufacturer/etc)
    [HttpPost]
    public virtual async Task<IActionResult> AddProductToCart_Catalog(int productId, int shoppingCartTypeId,
        int quantity, bool forceredirection = false)
    {
        var cartType = (ShoppingCartType)shoppingCartTypeId;

        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
            //no product found
            return Json(new
            {
                success = false,
                message = "No product found with the specified ID"
            });

        var redirectUrl = await _nopUrlHelper.RouteGenericUrlAsync(product);

        //we can add only simple products
        if (product.ProductType != ProductType.SimpleProduct)
            return Json(new { redirect = redirectUrl });

        //products with "minimum order quantity" more than a specified qty
        if (product.OrderMinimumQuantity > quantity)
        {
            //we cannot add to the cart such products from category pages
            //it can confuse customers. That's why we redirect customers to the product details page
            return Json(new { redirect = redirectUrl });
        }

        if (product.CustomerEntersPrice)
        {
            //cannot be added to the cart (requires a customer to enter price)
            return Json(new { redirect = redirectUrl });
        }

        if (product.IsRental)
        {
            //rental products require start/end dates to be entered
            return Json(new { redirect = redirectUrl });
        }

        var allowedQuantities = _productService.ParseAllowedQuantities(product);
        if (allowedQuantities.Length > 0)
        {
            //cannot be added to the cart (requires a customer to select a quantity from dropdownlist)
            return Json(new { redirect = redirectUrl });
        }

        //allow a product to be added to the cart when all attributes are with "read-only checkboxes" type
        var productAttributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
        if (productAttributes.Any(pam => pam.AttributeControlType != AttributeControlType.ReadonlyCheckboxes))
        {
            //product has some attributes. let a customer see them
            return Json(new { redirect = redirectUrl });
        }

        //creating XML for "read-only checkboxes" attributes
        var attXml = await productAttributes.AggregateAwaitAsync(string.Empty, async (attributesXml, attribute) =>
        {
            var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
            foreach (var selectedAttributeId in attributeValues
                         .Where(v => v.IsPreSelected)
                         .Select(v => v.Id)
                         .ToList())
            {
                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                    attribute, selectedAttributeId.ToString());
            }

            return attributesXml;
        });

        //get standard warnings without attribute validations
        //first, try to find existing shopping cart item
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, cartType, store.Id);
        var shoppingCartItem = await _shoppingCartService.FindShoppingCartItemInTheCartAsync(cart, cartType, product);
        //if we already have the same product in the cart, then use the total quantity to validate
        var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + quantity : quantity;
        var addToCartWarnings = await _shoppingCartService
            .GetShoppingCartItemWarningsAsync(customer, cartType,
                product, store.Id, string.Empty,
                decimal.Zero, null, null, quantityToValidate, false, shoppingCartItem?.Id ?? 0, true, false, false, false);
        if (addToCartWarnings.Any())
        {
            //cannot be added to the cart
            //let's display standard warnings
            return Json(new
            {
                success = false,
                message = addToCartWarnings.ToArray()
            });
        }

        //now let's try adding product to the cart (now including product attribute validation, etc)
        addToCartWarnings = await _shoppingCartService.AddToCartAsync(customer: customer,
            product: product,
            shoppingCartType: cartType,
            storeId: store.Id,
            attributesXml: attXml,
            quantity: quantity);
        if (addToCartWarnings.Any())
        {
            //cannot be added to the cart
            //but we do not display attribute and gift card warnings here. let's do it on the product details page
            return Json(new { redirect = redirectUrl });
        }

        //added to the cart/wishlist
        switch (cartType)
        {
            case ShoppingCartType.Wishlist:
            {

                var wishlistRouteUrl = Url.RouteUrl(NopRouteNames.General.WISHLIST);
                //activity log
                await _customerActivityService.InsertActivityAsync("PublicStore.AddToWishlist",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddToWishlist"), product.Name), product);

                if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct || forceredirection)
                {
                    //redirect to the wishlist page
                    return Json(new
                    {
                        redirect = wishlistRouteUrl
                    });
                }

                //display notification message and update appropriate blocks
                var shoppingCarts = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, store.Id);

                var updatetopwishlistsectionhtml = string.Format(await _localizationService.GetResourceAsync("Wishlist.HeaderQuantity"),
                    shoppingCarts.Sum(item => item.Quantity));

                var customWishlists = await _customWishlistService.GetAllCustomWishlistsAsync(customer.Id);
                var isGuest = await _customerService.IsGuestAsync(customer);

                if (!isGuest && customWishlists.Any())
                {
                    var onclick = $"showMoveToWishlistModal({productId}); return false;";
                    return Json(new
                    {
                        success = true,
                        message = string.Format(await _localizationService.GetResourceAsync("Products.ProductHasBeenAddedToTheWishlistAndMoved.Link"), wishlistRouteUrl, onclick),
                        updatetopwishlistsectionhtml
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        message = string.Format(await _localizationService.GetResourceAsync("Products.ProductHasBeenAddedToTheWishlist.Link"), wishlistRouteUrl),
                        updatetopwishlistsectionhtml
                    });
                }
            }

            case ShoppingCartType.ShoppingCart:
            default:
            {
                //activity log
                await _customerActivityService.InsertActivityAsync("PublicStore.AddToShoppingCart",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddToShoppingCart"), product.Name), product);

                if (_shoppingCartSettings.DisplayCartAfterAddingProduct || forceredirection)
                {
                    //redirect to the shopping cart page
                    return Json(new
                    {
                        redirect = Url.RouteUrl(NopRouteNames.General.CART)
                    });
                }

                //display notification message and update appropriate blocks
                var shoppingCarts = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

                var updatetopcartsectionhtml = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.HeaderQuantity"),
                    shoppingCarts.Sum(item => item.Quantity));

                var updateflyoutcartsectionhtml = _shoppingCartSettings.MiniShoppingCartEnabled
                    ? await RenderViewComponentToStringAsync(typeof(FlyoutShoppingCartViewComponent))
                    : string.Empty;

                return Json(new
                {
                    success = true,
                    message = string.Format(await _localizationService.GetResourceAsync("Products.ProductHasBeenAddedToTheCart.Link"), Url.RouteUrl(NopRouteNames.General.CART)),
                    updatetopcartsectionhtml,
                    updateflyoutcartsectionhtml
                });
            }
        }
    }

    //add product to cart using AJAX
    //currently we use this method on the product details pages
    [HttpPost]
    public virtual async Task<IActionResult> AddProductToCart_Details(int productId, int shoppingCartTypeId, IFormCollection form)
    {
		_logger.LogError(">>> Entering AddProductToCart_Details(productId={ProductId},shoppingCartTypeId=<redacted>,form={Form})", productId, form);
        var product = await _productService.GetProductByIdAsync(productId);
		_logger.LogError("AddProductToCart_Details(productId={ProductId},shoppingCartTypeId=<redacted>,form={Form}): product → {Product}", productId, form, product);
        if (product == null)
        {
            return Json(new
            {
                redirect = Url.RouteUrl(NopRouteNames.General.HOMEPAGE)
            });
				_logger.LogDebug("AddProductToCart_Details(productId={ProductId},shoppingCartTypeId=<redacted>,form={Form}): redirect → {Redirect}", productId, form, redirect);
        }

        //we can add only simple products
        if (product.ProductType != ProductType.SimpleProduct)
        {
            return Json(new
            {
                success = false,
                message = "Only simple products could be added to the cart"
				_logger.LogDebug("AddProductToCart_Details(productId={ProductId},shoppingCartTypeId=<redacted>,form={Form}): message → {Message}", productId, form, message);
            });
        }

        //update existing shopping cart item
        var updatecartitemid = 0;
        foreach (var formKey in form.Keys)
            if (formKey.Equals($"addtocart_{productId}.UpdatedShoppingCartItemId", StringComparison.InvariantCultureIgnoreCase))
            {
                _ = int.TryParse(form[formKey], out updatecartitemid);
				_logger.LogError("AddProductToCart_Details(productId={ProductId},shoppingCartTypeId=<redacted>,form={Form}): _ → {_}", productId, form, _);
                break;
            }

        ShoppingCartItem updatecartitem = null;
		_logger.LogError("AddProductToCart_Details(productId={ProductId},shoppingCartTypeId=<redacted>,form={Form}): updatecartitem → {Updatecartitem}", productId, form, updatecartitem);
        if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogError("AddProductToCart_Details(productId={ProductId},shoppingCartTypeId=<redacted>,form={Form}): store → {Store}", productId, form, store);
            //search with the same cart type as specified
            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), (ShoppingCartType)shoppingCartTypeId, store.Id);
			_logger.LogError("AddProductToCart_Details(productId={ProductId},shoppingCartTypeId=<redacted>,form={Form}): cart → {Cart}", productId, form, cart);

            updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
            //not found? let's ignore it. in this case we'll add a new item
            //if (updatecartitem == null)
            //{
            //    return Json(new
            //    {
            //        success = false,
            //        message = "No shopping cart item found to update"
            //    });
            //}
            //is it this product?
            if (updatecartitem != null && product.Id != updatecartitem.ProductId)
            {
                return Json(new
                {
                    success = false,
                    message = "This product does not match a passed shopping cart item identifier"
                });
            }
        }

        var addToCartWarnings = new List<string>();
		_logger.LogError("AddProductToCart_Details(productId={ProductId},shoppingCartTypeId=<redacted>,form={Form}): addToCartWarnings → {AddToCartWarnings}", productId, form, addToCartWarnings);

        //customer entered price
        var customerEnteredPriceConverted = await _productAttributeParser.ParseCustomerEnteredPriceAsync(product, form);
		_logger.LogError("AddProductToCart_Details(productId={ProductId},shoppingCartTypeId=<redacted>,form={Form}): customerEnteredPriceConverted → {CustomerEnteredPriceConverted}", productId, form, customerEnteredPriceConverted);

        //entered quantity
        var quantity = _productAttributeParser.ParseEnteredQuantity(product, form);
		_logger.LogError("AddProductToCart_Details(productId={ProductId},shoppingCartTypeId=<redacted>,form={Form}): quantity → {Quantity}", productId, form, quantity);

        //product and gift card attributes
        var attributes = await _productAttributeParser.ParseProductAttributesAsync(product, form, addToCartWarnings);
		_logger.LogError("AddProductToCart_Details(productId={ProductId},shoppingCartTypeId=<redacted>,form={Form}): attributes → {Attributes}", productId, form, attributes);

        //rental attributes
        _productAttributeParser.ParseRentalDates(product, form, out var rentalStartDate, out var rentalEndDate);

        var cartType = updatecartitem == null ? (ShoppingCartType)shoppingCartTypeId :
            //if the item to update is found, then we ignore the specified "shoppingCartTypeId" parameter
            updatecartitem.ShoppingCartType;

        await SaveItemAsync(updatecartitem, addToCartWarnings, product, cartType, attributes, customerEnteredPriceConverted, rentalStartDate, rentalEndDate, quantity);

        //return result
		_logger.LogWarning("<<< Exiting AddProductToCart_Details(productId={ProductId},shoppingCartTypeId=<redacted>,form={Form})", productId, form);
        return await GetProductToCartDetailsAsync(addToCartWarnings, cartType, product);
    }

    //handle product attribute selection event. this way we return new price, overridden gtin/sku/mpn
    //currently we use this method on the product details pages
    [HttpPost]
    public virtual async Task<IActionResult> ProductDetails_AttributeChange(int productId, bool validateAttributeConditions,
        bool loadPicture, IFormCollection form)
    {
		_logger.LogError(">>> Entering ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form})", productId, validateAttributeConditions, loadPicture, form);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
            return new NullJsonResult();

        var errors = new List<string>();
		_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): errors → {Errors}", productId, validateAttributeConditions, loadPicture, form, errors);
        var attributeXml = await _productAttributeParser.ParseProductAttributesAsync(product, form, errors);
		_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): attributeXml → {AttributeXml}", productId, validateAttributeConditions, loadPicture, form, attributeXml);

        //rental attributes
        DateTime? rentalStartDate = null;
        DateTime? rentalEndDate = null;
        if (product.IsRental)
        {
            _productAttributeParser.ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
        }

        //sku, mpn, gtin
        var sku = await _productService.FormatSkuAsync(product, attributeXml);
		_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): sku → {Sku}", productId, validateAttributeConditions, loadPicture, form, sku);
        var mpn = await _productService.FormatMpnAsync(product, attributeXml);
		_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): mpn → {Mpn}", productId, validateAttributeConditions, loadPicture, form, mpn);
        var gtin = await _productService.FormatGtinAsync(product, attributeXml);
		_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): gtin → {Gtin}", productId, validateAttributeConditions, loadPicture, form, gtin);

        // calculating weight adjustment
        var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributeXml);
		_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): attributeValues → {AttributeValues}", productId, validateAttributeConditions, loadPicture, form, attributeValues);
        var totalWeight = product.BasepriceAmount;

        foreach (var attributeValue in attributeValues)
        {
            switch (attributeValue.AttributeValueType)
            {
                case AttributeValueType.Simple:
                    //simple attribute
                    totalWeight += attributeValue.WeightAdjustment;
                    break;
                case AttributeValueType.AssociatedToProduct:
                    //bundled product
                    var associatedProduct = await _productService.GetProductByIdAsync(attributeValue.AssociatedProductId);
					_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): associatedProduct → {AssociatedProduct}", productId, validateAttributeConditions, loadPicture, form, associatedProduct);
                    if (associatedProduct != null)
                        totalWeight += associatedProduct.BasepriceAmount * attributeValue.Quantity;
                    break;
            }
        }

        //price
        var price = string.Empty;
		_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): price → {Price}", productId, validateAttributeConditions, loadPicture, form, price);
        //base price
        var basepricepangv = string.Empty;
		_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): basepricepangv → {Basepricepangv}", productId, validateAttributeConditions, loadPicture, form, basepricepangv);
        if (!product.CustomerEntersPrice && await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES))
        {
            var currentStore = await _storeContext.GetCurrentStoreAsync();
			_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): currentStore → {CurrentStore}", productId, validateAttributeConditions, loadPicture, form, currentStore);
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): currentCustomer → {CurrentCustomer}", productId, validateAttributeConditions, loadPicture, form, currentCustomer);

            //we do not calculate price of "customer enters price" option is enabled
            var (finalPrice, _, _) = await _shoppingCartService.GetUnitPriceAsync(product,
                currentCustomer,
                currentStore,
                ShoppingCartType.ShoppingCart,
                1, attributeXml, 0,
                rentalStartDate, rentalEndDate, true);
            var (finalPriceWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, finalPrice);
            var finalPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithDiscountBase, await _workContext.GetWorkingCurrencyAsync());
			_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): finalPriceWithDiscount → {FinalPriceWithDiscount}", productId, validateAttributeConditions, loadPicture, form, finalPriceWithDiscount);
            price = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
            basepricepangv = await _priceFormatter.FormatBasePriceAsync(product, finalPriceWithDiscountBase, totalWeight);
        }

        //stock
        var stockAvailability = await _productService.FormatStockMessageAsync(product, attributeXml);
		_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): stockAvailability → {StockAvailability}", productId, validateAttributeConditions, loadPicture, form, stockAvailability);

        //conditional attributes
        var enabledAttributeMappingIds = new List<int>();
		_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): enabledAttributeMappingIds → {EnabledAttributeMappingIds}", productId, validateAttributeConditions, loadPicture, form, enabledAttributeMappingIds);
        var disabledAttributeMappingIds = new List<int>();
		_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): disabledAttributeMappingIds → {DisabledAttributeMappingIds}", productId, validateAttributeConditions, loadPicture, form, disabledAttributeMappingIds);
        if (validateAttributeConditions)
        {
            var attributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
			_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): attributes → {Attributes}", productId, validateAttributeConditions, loadPicture, form, attributes);
            foreach (var attribute in attributes)
            {
                var conditionMet = await _productAttributeParser.IsConditionMetAsync(attribute, attributeXml);
				_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): conditionMet → {ConditionMet}", productId, validateAttributeConditions, loadPicture, form, conditionMet);
                if (conditionMet.HasValue)
                {
                    if (conditionMet.Value)
                        enabledAttributeMappingIds.Add(attribute.Id);
                    else
                        disabledAttributeMappingIds.Add(attribute.Id);
                }
            }
        }

        //picture. used when we want to override a default product picture when some attribute is selected
        var pictureFullSizeUrl = string.Empty;
		_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): pictureFullSizeUrl → {PictureFullSizeUrl}", productId, validateAttributeConditions, loadPicture, form, pictureFullSizeUrl);
        var pictureDefaultSizeUrl = string.Empty;
		_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): pictureDefaultSizeUrl → {PictureDefaultSizeUrl}", productId, validateAttributeConditions, loadPicture, form, pictureDefaultSizeUrl);
        var pictureIds = new List<int>();
		_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): pictureIds → {PictureIds}", productId, validateAttributeConditions, loadPicture, form, pictureIds);
        if (loadPicture)
        {
            //first, try to get product attribute combination picture
            var pictureId = 0;
			_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): pictureId → {PictureId}", productId, validateAttributeConditions, loadPicture, form, pictureId);
            var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributeXml);
			_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): combination → {Combination}", productId, validateAttributeConditions, loadPicture, form, combination);
            if (combination != null)
            {
                var combinationPictures = await _productAttributeService.GetProductAttributeCombinationPicturesAsync(combination.Id);
				_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): combinationPictures → {CombinationPictures}", productId, validateAttributeConditions, loadPicture, form, combinationPictures);
                pictureIds = combinationPictures.Select(cp => cp.PictureId).ToList();
                pictureId = combinationPictures.FirstOrDefault()?.PictureId ?? 0;
            }

            //then, let's see whether we have attribute values with pictures
            if (pictureId == 0)
            {
                var valuePictures = await (await _productAttributeParser.ParseProductAttributeValuesAsync(attributeXml))
                    .SelectManyAwait(async attributeValue => await _productAttributeService.GetProductAttributeValuePicturesAsync(attributeValue.Id))
                    .ToListAsync();
					var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
                pictureIds = valuePictures.Select(vp => vp.PictureId).ToList();
                pictureId = valuePictures.FirstOrDefault()?.PictureId ?? 0;
				_logger.LogError("ProductDetails_AttributeChange(productId,validateAttributeConditions,loadPicture,form)={ProductId},{ValidateAttributeConditions},{LoadPicture},{Form}: database query executed in {Arg4} ms", productId, validateAttributeConditions, loadPicture, form, __logStartDb.ElapsedMilliseconds);
            }

            if (pictureId > 0)
            {
                var productAttributePictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductAttributePictureModelKey,
                    pictureId, _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());
                var pictureModel = await _staticCacheManager.GetAsync(productAttributePictureCacheKey, async () =>
                {
                    var picture = await _pictureService.GetPictureByIdAsync(pictureId);
					_logger.LogError("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): picture → {Picture}", productId, validateAttributeConditions, loadPicture, form, picture);
                    string fullSizeImageUrl, imageUrl;

                    (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                    (imageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture, _mediaSettings.ProductDetailsPictureSize);

                    return picture == null ? new PictureModel() : new PictureModel
                    {
                        FullSizeImageUrl = fullSizeImageUrl,
                        ImageUrl = imageUrl
                    };
                });
                pictureFullSizeUrl = pictureModel.FullSizeImageUrl;
                pictureDefaultSizeUrl = pictureModel.ImageUrl;
            }
        }

        var isFreeShipping = product.IsFreeShipping;
        if (isFreeShipping && !string.IsNullOrEmpty(attributeXml))
        {
            isFreeShipping = await (await _productAttributeParser.ParseProductAttributeValuesAsync(attributeXml))
                .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                .SelectAwait(async attributeValue => await _productService.GetProductByIdAsync(attributeValue.AssociatedProductId))
                .AllAsync(associatedProduct => associatedProduct == null || !associatedProduct.IsShipEnabled || associatedProduct.IsFreeShipping);
        }

		_logger.LogDebug("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form}): message → {Message}", productId, validateAttributeConditions, loadPicture, form, message);
		_logger.LogError("ProductDetails_AttributeChange(productId,validateAttributeConditions,loadPicture,form)={ProductId},{ValidateAttributeConditions},{LoadPicture},{Form}: external service call {PictureFullSizeUrl} took {Arg5} ms", productId, validateAttributeConditions, loadPicture, form, pictureFullSizeUrl, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},loadPicture={LoadPicture},form={Form})", productId, validateAttributeConditions, loadPicture, form);
        return Json(new
        {
            productId,
            gtin,
            mpn,
            sku,
            price,
            basepricepangv,
            stockAvailability,
            enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
            disabledattributemappingids = disabledAttributeMappingIds.ToArray(),
            pictureFullSizeUrl,
            pictureDefaultSizeUrl,
            pictureIds,
            isFreeShipping,
            message = errors.Any() ? errors.ToArray() : null
        });
    }

    [HttpPost]
    public virtual async Task<IActionResult> CheckoutAttributeChange(IFormCollection form, bool isEditable)
    {
		_logger.LogError(">>> Entering CheckoutAttributeChange(form={Form},isEditable={IsEditable})", form, isEditable);
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("CheckoutAttributeChange(form={Form},isEditable={IsEditable}): store → {Store}", form, isEditable, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogError("CheckoutAttributeChange(form={Form},isEditable={IsEditable}): cart → {Cart}", form, isEditable, cart);
		_logger.LogError("CheckoutAttributeChange(form={Form},isEditable={IsEditable}): customer → {Customer}", form, isEditable, customer);

        //save selected attributes
        await ParseAndSaveCheckoutAttributesAsync(cart, form);
        var attributeXml = await _genericAttributeService.GetAttributeAsync<string>(customer,
            NopCustomerDefaults.CheckoutAttributes, store.Id);

        //conditions
        var enabledAttributeIds = new List<int>();
		_logger.LogError("CheckoutAttributeChange(form={Form},isEditable={IsEditable}): enabledAttributeIds → {EnabledAttributeIds}", form, isEditable, enabledAttributeIds);
        var disabledAttributeIds = new List<int>();
		_logger.LogError("CheckoutAttributeChange(form={Form},isEditable={IsEditable}): disabledAttributeIds → {DisabledAttributeIds}", form, isEditable, disabledAttributeIds);
        var excludeShippableAttributes = !await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
		_logger.LogError("CheckoutAttributeChange(form={Form},isEditable={IsEditable}): excludeShippableAttributes → {ExcludeShippableAttributes}", form, isEditable, excludeShippableAttributes);
        var attributes = await _checkoutAttributeService.GetAllAttributesAsync(_staticCacheManager, _storeMappingService, store.Id, excludeShippableAttributes);
		_logger.LogError("CheckoutAttributeChange(form={Form},isEditable={IsEditable}): attributes → {Attributes}", form, isEditable, attributes);
        foreach (var attribute in attributes)
        {
            var conditionMet = await _checkoutAttributeParser.IsConditionMetAsync(attribute.ConditionAttributeXml, attributeXml);
			_logger.LogError("CheckoutAttributeChange(form={Form},isEditable={IsEditable}): conditionMet → {ConditionMet}", form, isEditable, conditionMet);
            if (conditionMet.HasValue)
            {
                if (conditionMet.Value)
                    enabledAttributeIds.Add(attribute.Id);
                else
                    disabledAttributeIds.Add(attribute.Id);
            }
        }

        //update blocks
        var ordetotalssectionhtml = await RenderViewComponentToStringAsync(typeof(OrderTotalsViewComponent), new { isEditable });
		_logger.LogError("CheckoutAttributeChange(form={Form},isEditable={IsEditable}): ordetotalssectionhtml → {Ordetotalssectionhtml}", form, isEditable, ordetotalssectionhtml);
        var selectedcheckoutattributesssectionhtml = await RenderViewComponentToStringAsync(typeof(SelectedCheckoutAttributesViewComponent));
		_logger.LogError("CheckoutAttributeChange(form={Form},isEditable={IsEditable}): selectedcheckoutattributesssectionhtml → {Selectedcheckoutattributesssectionhtml}", form, isEditable, selectedcheckoutattributesssectionhtml);

		_logger.LogWarning("<<< Exiting CheckoutAttributeChange(form={Form},isEditable={IsEditable})", form, isEditable);
        return Json(new
        {
            ordetotalssectionhtml,
            selectedcheckoutattributesssectionhtml,
            enabledattributeids = enabledAttributeIds.ToArray(),
            disabledattributeids = disabledAttributeIds.ToArray()
        });
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> UploadFileProductAttribute(int attributeId)
    {
		_logger.LogError(">>> Entering UploadFileProductAttribute(attributeId={AttributeId})", attributeId);
        var attribute = await _productAttributeService.GetProductAttributeMappingByIdAsync(attributeId);
		_logger.LogError("UploadFileProductAttribute(attributeId={AttributeId}): attribute → {Attribute}", attributeId, attribute);
        if (attribute == null || attribute.AttributeControlType != AttributeControlType.FileUpload)
        {
            return Json(new
            {
                success = false,
                downloadGuid = Guid.Empty
            });
			_logger.LogDebug("UploadFileProductAttribute(attributeId={AttributeId}): downloadGuid → {DownloadGuid}", attributeId, downloadGuid);
        }

        var httpPostedFile = await Request.GetFirstOrDefaultFileAsync();
		_logger.LogError("UploadFileProductAttribute(attributeId={AttributeId}): httpPostedFile → {HttpPostedFile}", attributeId, httpPostedFile);
        if (httpPostedFile == null)
        {
            return Json(new
            {
                success = false,
                message = "No file uploaded",
                downloadGuid = Guid.Empty
            });
        }

        var fileBinary = await _downloadService.GetDownloadBitsAsync(httpPostedFile);
		_logger.LogError("UploadFileProductAttribute(attributeId={AttributeId}): fileBinary → {FileBinary}", attributeId, fileBinary);

        var fileName = httpPostedFile.FileName;
		_logger.LogError("UploadFileProductAttribute(attributeId={AttributeId}): fileName → {FileName}", attributeId, fileName);

        //remove path (passed in IE)
        fileName = _fileProvider.GetFileName(fileName);

        var contentType = httpPostedFile.ContentType;

        var fileExtension = _fileProvider.GetFileExtension(fileName);
		_logger.LogError("UploadFileProductAttribute(attributeId={AttributeId}): fileExtension → {FileExtension}", attributeId, fileExtension);
        if (!string.IsNullOrEmpty(fileExtension))
            fileExtension = fileExtension.ToLowerInvariant();

        if (attribute.ValidationFileMaximumSize.HasValue)
        {
            //compare in bytes
            var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
            if (fileBinary.Length > maxFileSizeBytes)
            {
                //when returning JSON the mime-type must be set to text/plain
                //otherwise some browsers will pop-up a "Save As" dialog.
                return Json(new
                {
                    success = false,
                    message = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value),
                    downloadGuid = Guid.Empty
                });
            }
        }

        var download = new Download
        {
            DownloadGuid = Guid.NewGuid(),
            UseDownloadUrl = false,
            DownloadUrl = string.Empty,
            DownloadBinary = fileBinary,
            ContentType = contentType,
            //we store filename without extension for downloads
            Filename = _fileProvider.GetFileNameWithoutExtension(fileName),
            Extension = fileExtension,
            IsNew = true
        };
        await _downloadService.InsertDownloadAsync(download);

        //when returning JSON the mime-type must be set to text/plain
        //otherwise some browsers will pop-up a "Save As" dialog.
		_logger.LogWarning("<<< Exiting UploadFileProductAttribute(attributeId={AttributeId})", attributeId);
        return Json(new
        {
            success = true,
            message = await _localizationService.GetResourceAsync("ShoppingCart.FileUploaded"),
            downloadUrl = Url.RouteUrl(NopRouteNames.Standard.DOWNLOAD_GET_FILE_UPLOAD, new { downloadId = download.DownloadGuid }),
            downloadGuid = download.DownloadGuid
        });
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> UploadFileCheckoutAttribute(int attributeId)
    {
		_logger.LogError(">>> Entering UploadFileCheckoutAttribute(attributeId={AttributeId})", attributeId);
        var attribute = await _checkoutAttributeService.GetAttributeByIdAsync(attributeId);
		_logger.LogError("UploadFileCheckoutAttribute(attributeId={AttributeId}): attribute → {Attribute}", attributeId, attribute);
        if (attribute == null || attribute.AttributeControlType != AttributeControlType.FileUpload)
        {
            return Json(new
            {
                success = false,
                downloadGuid = Guid.Empty
            });
			_logger.LogDebug("UploadFileCheckoutAttribute(attributeId={AttributeId}): downloadGuid → {DownloadGuid}", attributeId, downloadGuid);
        }

        var httpPostedFile = await Request.GetFirstOrDefaultFileAsync();
		_logger.LogError("UploadFileCheckoutAttribute(attributeId={AttributeId}): httpPostedFile → {HttpPostedFile}", attributeId, httpPostedFile);
        if (httpPostedFile == null)
        {
            return Json(new
            {
                success = false,
                message = "No file uploaded",
                downloadGuid = Guid.Empty
            });
        }

        var fileBinary = await _downloadService.GetDownloadBitsAsync(httpPostedFile);
		_logger.LogError("UploadFileCheckoutAttribute(attributeId={AttributeId}): fileBinary → {FileBinary}", attributeId, fileBinary);

        var fileName = httpPostedFile.FileName;
		_logger.LogError("UploadFileCheckoutAttribute(attributeId={AttributeId}): fileName → {FileName}", attributeId, fileName);

        //remove path (passed in IE)
        fileName = _fileProvider.GetFileName(fileName);

        var contentType = httpPostedFile.ContentType;

        var fileExtension = _fileProvider.GetFileExtension(fileName);
		_logger.LogError("UploadFileCheckoutAttribute(attributeId={AttributeId}): fileExtension → {FileExtension}", attributeId, fileExtension);
        if (!string.IsNullOrEmpty(fileExtension))
            fileExtension = fileExtension.ToLowerInvariant();

        if (attribute.ValidationFileMaximumSize.HasValue)
        {
            //compare in bytes
            var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
            if (fileBinary.Length > maxFileSizeBytes)
            {
                //when returning JSON the mime-type must be set to text/plain
                //otherwise some browsers will pop-up a "Save As" dialog.
                return Json(new
                {
                    success = false,
                    message = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value),
                    downloadGuid = Guid.Empty
                });
            }
        }

        var download = new Download
        {
            DownloadGuid = Guid.NewGuid(),
            UseDownloadUrl = false,
            DownloadUrl = string.Empty,
            DownloadBinary = fileBinary,
            ContentType = contentType,
            //we store filename without extension for downloads
            Filename = _fileProvider.GetFileNameWithoutExtension(fileName),
            Extension = fileExtension,
            IsNew = true
        };
        await _downloadService.InsertDownloadAsync(download);

        //when returning JSON the mime-type must be set to text/plain
        //otherwise some browsers will pop-up a "Save As" dialog.
		_logger.LogWarning("<<< Exiting UploadFileCheckoutAttribute(attributeId={AttributeId})", attributeId);
        return Json(new
        {
            success = true,
            message = await _localizationService.GetResourceAsync("ShoppingCart.FileUploaded"),
            downloadUrl = Url.RouteUrl(NopRouteNames.Standard.DOWNLOAD_GET_FILE_UPLOAD, new { downloadId = download.DownloadGuid }),
            downloadGuid = download.DownloadGuid
        });
    }

    public virtual async Task<IActionResult> Cart()
    {
		_logger.LogError(">>> Entering Cart()");
        if (!await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("Cart(): store → {Store}", store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogError("Cart(): cart → {Cart}", cart);
        var model = new ShoppingCartModel();
		_logger.LogError("Cart(): model → {Model}", model);
        model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);
		_logger.LogWarning("<<< Exiting Cart()");
        return View(model);
    }

    [HttpPost, ActionName("Cart")]
    [FormValueRequired("updatecart")]
    public virtual async Task<IActionResult> UpdateCart(IFormCollection form)
    {
		_logger.LogError(">>> Entering UpdateCart(form={Form})", form);
        if (!await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("UpdateCart(form={Form}): customer → {Customer}", form, customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("UpdateCart(form={Form}): store → {Store}", form, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogError("UpdateCart(form={Form}): cart → {Cart}", form, cart);

        //get identifiers of items to remove
        var itemIdsToRemove = form["removefromcart"]
            .SelectMany(value => value.Split(_separator, StringSplitOptions.RemoveEmptyEntries))
            .Select(idString => int.TryParse(idString, out var id) ? id : 0)
            .Distinct().ToList();

        var products = (await _productService.GetProductsByIdsAsync(cart.Select(item => item.ProductId).Distinct().ToArray()))
            .ToDictionary(item => item.Id, item => item);

        //get order items with changed quantity
        var itemsWithNewQuantity = cart.Select(item => new
        {
            //try to get a new quantity for the item, set 0 for items to remove
            NewQuantity = itemIdsToRemove.Contains(item.Id) ? 0 : int.TryParse(form[$"itemquantity{item.Id}"], out var quantity) ? quantity : item.Quantity,
            Item = item,
            Product = products.TryGetValue(item.ProductId, out var value) ? value : null
        }).Where(item => item.NewQuantity != item.Item.Quantity);
		_logger.LogDebug("UpdateCart(form={Form}): Product → {Product}", form, Product);

        //order cart items
        //first should be items with a reduced quantity and that require other products; or items with an increased quantity and are required for other products
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var orderedCart = await itemsWithNewQuantity
            .OrderByDescendingAwait(async cartItem =>
                (cartItem.NewQuantity < cartItem.Item.Quantity &&
                 (cartItem.Product?.RequireOtherProducts ?? false)) ||
                (cartItem.NewQuantity > cartItem.Item.Quantity && cartItem.Product != null && (await _shoppingCartService
                    .GetProductsRequiringProductAsync(cart, cartItem.Product)).Any()))
            .ToListAsync();
			_logger.LogError("UpdateCart(form)={Form}: database query executed in {Arg1} ms", form, __logStart.ElapsedMilliseconds);

        //try to update cart items with new quantities and get warnings
        var warnings = await orderedCart.SelectAwait(async cartItem => new
        {
            ItemId = cartItem.Item.Id,
            Warnings = await _shoppingCartService.UpdateShoppingCartItemAsync(customer,
                cartItem.Item.Id, cartItem.Item.AttributesXml, cartItem.Item.CustomerEnteredPrice,
                cartItem.Item.RentalStartDateUtc, cartItem.Item.RentalEndDateUtc, cartItem.NewQuantity, true)
        }).ToListAsync();

        //updated cart
        cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        //parse and save checkout attributes
        await ParseAndSaveCheckoutAttributesAsync(cart, form);

        //prepare model
        var model = new ShoppingCartModel();
		_logger.LogError("UpdateCart(form={Form}): model → {Model}", form, model);
        model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);

        //update current warnings
        foreach (var warningItem in warnings.Where(warningItem => warningItem.Warnings.Any()))
        {
            //find shopping cart item model to display appropriate warnings
            var itemModel = model.Items.FirstOrDefault(item => item.Id == warningItem.ItemId);
			_logger.LogError("UpdateCart(form={Form}): itemModel → {ItemModel}", form, itemModel);
            if (itemModel != null)
                itemModel.Warnings = warningItem.Warnings.Concat(itemModel.Warnings).Distinct().ToList();
        }

		_logger.LogWarning("<<< Exiting UpdateCart(form={Form})", form);
        return View(model);
    }

    [HttpPost, ActionName("Cart")]
    [FormValueRequired("continueshopping")]
    public virtual async Task<IActionResult> ContinueShopping()
    {
		_logger.LogError(">>> Entering ContinueShopping()");
        var store = await _storeContext.GetCurrentStoreAsync();
        var returnUrl = await _genericAttributeService.GetAttributeAsync<string>(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.LastContinueShoppingPageAttribute, store.Id);
		_logger.LogError("ContinueShopping(): returnUrl → {ReturnUrl}", returnUrl);
		_logger.LogError("ContinueShopping(): store → {Store}", store);

        if (!string.IsNullOrEmpty(returnUrl))
            return Redirect(returnUrl);

        return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
    }

    [HttpPost, ActionName("Cart")]
    [FormValueRequired("checkout")]
    public virtual async Task<IActionResult> StartCheckout(IFormCollection form)
    {
		_logger.LogError(">>> Entering StartCheckout(form={Form})", form);
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("StartCheckout(form={Form}): store → {Store}", form, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogError("StartCheckout(form={Form}): cart → {Cart}", form, cart);
		_logger.LogError("StartCheckout(form={Form}): customer → {Customer}", form, customer);

        //parse and save checkout attributes
        await ParseAndSaveCheckoutAttributesAsync(cart, form);

        //validate attributes
        var checkoutAttributes = await _genericAttributeService.GetAttributeAsync<string>(customer,
            NopCustomerDefaults.CheckoutAttributes, store.Id);
        var checkoutAttributeWarnings = await _shoppingCartService.GetShoppingCartWarningsAsync(cart, checkoutAttributes, true);
		_logger.LogError("StartCheckout(form={Form}): checkoutAttributeWarnings → {CheckoutAttributeWarnings}", form, checkoutAttributeWarnings);
        if (checkoutAttributeWarnings.Any())
        {
            //something wrong, redisplay the page with warnings
            var model = new ShoppingCartModel();
			_logger.LogError("StartCheckout(form={Form}): model → {Model}", form, model);
            model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart, validateCheckoutAttributes: true);
            return View(model);
        }

        var anonymousPermissed = _orderSettings.AnonymousCheckoutAllowed
                                 && _customerSettings.UserRegistrationType == UserRegistrationType.Disabled;

        if (anonymousPermissed || !await _customerService.IsGuestAsync(customer))
            return RedirectToRoute(NopRouteNames.Standard.CHECKOUT);

        var cartProductIds = cart.Select(ci => ci.ProductId).ToArray();
		_logger.LogError("StartCheckout(form={Form}): cartProductIds → {CartProductIds}", form, cartProductIds);
        var downloadableProductsRequireRegistration =
            _customerSettings.RequireRegistrationForDownloadableProducts && await _productService.HasAnyDownloadableProductAsync(cartProductIds);

        if (!_orderSettings.AnonymousCheckoutAllowed || downloadableProductsRequireRegistration)
        {
            //verify user identity (it may be facebook login page, or google, or local)
            return Challenge();
        }

		_logger.LogWarning("<<< Exiting StartCheckout(form={Form})", form);
        return RedirectToRoute(NopRouteNames.Standard.LOGIN_CHECKOUT_AS_GUEST, new { returnUrl = Url.RouteUrl(NopRouteNames.General.CART) });
    }

    [HttpPost, ActionName("Cart")]
    [FormValueRequired("applydiscountcouponcode")]
    public virtual async Task<IActionResult> ApplyDiscountCoupon(string discountcouponcode, IFormCollection form)
    {
		_logger.LogError(">>> Entering ApplyDiscountCoupon(discountcouponcode={Discountcouponcode},form={Form})", discountcouponcode, form);
        //trim
        if (discountcouponcode != null)
            discountcouponcode = discountcouponcode.Trim();
			_logger.LogError("ApplyDiscountCoupon(discountcouponcode={Discountcouponcode},form={Form}): discountcouponcode → {Discountcouponcode1}", discountcouponcode, form, discountcouponcode);

        //cart
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("ApplyDiscountCoupon(discountcouponcode={Discountcouponcode},form={Form}): customer → {Customer}", discountcouponcode, form, customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("ApplyDiscountCoupon(discountcouponcode={Discountcouponcode},form={Form}): store → {Store}", discountcouponcode, form, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogError("ApplyDiscountCoupon(discountcouponcode={Discountcouponcode},form={Form}): cart → {Cart}", discountcouponcode, form, cart);

        //parse and save checkout attributes
        await ParseAndSaveCheckoutAttributesAsync(cart, form);

        var model = new ShoppingCartModel();
		_logger.LogError("ApplyDiscountCoupon(discountcouponcode={Discountcouponcode},form={Form}): model → {Model}", discountcouponcode, form, model);
        if (!string.IsNullOrWhiteSpace(discountcouponcode))
        {
            //we find even hidden records here. this way we can display a user-friendly message if it's expired
            var discounts = (await _discountService.GetAllDiscountsAsync(couponCode: discountcouponcode, showHidden: true))
                .Where(d => d.RequiresCouponCode)
                .ToList();
            if (discounts.Any())
            {
                var userErrors = new List<string>();
				_logger.LogError("ApplyDiscountCoupon(discountcouponcode={Discountcouponcode},form={Form}): userErrors → {UserErrors}", discountcouponcode, form, userErrors);
                var anyValidDiscount = await discounts.AnyAwaitAsync(async discount =>
                {
                    var validationResult = await _discountService.ValidateDiscountAsync(discount, customer, [discountcouponcode]);
					_logger.LogError("ApplyDiscountCoupon(discountcouponcode={Discountcouponcode},form={Form}): validationResult → {ValidationResult}", discountcouponcode, form, validationResult);
                    userErrors.AddRange(validationResult.Errors);

                    return validationResult.IsValid;
                });

                if (anyValidDiscount)
                {
                    //valid
                    await _customerService.ApplyDiscountCouponCodeAsync(customer, discountcouponcode);
                    model.DiscountBox.Messages.Add(await _localizationService.GetResourceAsync("ShoppingCart.DiscountCouponCode.Applied"));
                    model.DiscountBox.IsApplied = true;
                }
                else
                {
                    if (userErrors.Any())
                        //some user errors
                        model.DiscountBox.Messages = userErrors;
                    else
                        //general error text
                        model.DiscountBox.Messages.Add(await _localizationService.GetResourceAsync("ShoppingCart.DiscountCouponCode.WrongDiscount"));
                }
            }
            else
                //discount cannot be found
                model.DiscountBox.Messages.Add(await _localizationService.GetResourceAsync("ShoppingCart.DiscountCouponCode.CannotBeFound"));
        }
        else
            //empty coupon code
            model.DiscountBox.Messages.Add(await _localizationService.GetResourceAsync("ShoppingCart.DiscountCouponCode.Empty"));

        model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);

		_logger.LogWarning("<<< Exiting ApplyDiscountCoupon(discountcouponcode={Discountcouponcode},form={Form})", discountcouponcode, form);
        return View(model);
    }

    [HttpPost, ActionName("Cart")]
    [FormValueRequired("applygiftcardcouponcode")]
    public virtual async Task<IActionResult> ApplyGiftCard(string giftcardcouponcode, IFormCollection form)
    {
		_logger.LogError(">>> Entering ApplyGiftCard(giftcardcouponcode={Giftcardcouponcode},form={Form})", giftcardcouponcode, form);
        //trim
        if (giftcardcouponcode != null)
            giftcardcouponcode = giftcardcouponcode.Trim();
			_logger.LogError("ApplyGiftCard(giftcardcouponcode={Giftcardcouponcode},form={Form}): giftcardcouponcode → {Giftcardcouponcode1}", giftcardcouponcode, form, giftcardcouponcode);

        //cart
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("ApplyGiftCard(giftcardcouponcode={Giftcardcouponcode},form={Form}): customer → {Customer}", giftcardcouponcode, form, customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("ApplyGiftCard(giftcardcouponcode={Giftcardcouponcode},form={Form}): store → {Store}", giftcardcouponcode, form, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogError("ApplyGiftCard(giftcardcouponcode={Giftcardcouponcode},form={Form}): cart → {Cart}", giftcardcouponcode, form, cart);

        //parse and save checkout attributes
        await ParseAndSaveCheckoutAttributesAsync(cart, form);

        var model = new ShoppingCartModel();
		_logger.LogError("ApplyGiftCard(giftcardcouponcode={Giftcardcouponcode},form={Form}): model → {Model}", giftcardcouponcode, form, model);

        var validationError = await GetGiftCardValidationErrorAsync(cart, giftcardcouponcode);
		_logger.LogError("ApplyGiftCard(giftcardcouponcode={Giftcardcouponcode},form={Form}): validationError → {ValidationError}", giftcardcouponcode, form, validationError);

        if (string.IsNullOrEmpty(validationError))
        {
            await _customerService.ApplyGiftCardCouponCodeAsync(customer, giftcardcouponcode);
            model.GiftCardBox.Message = await _localizationService.GetResourceAsync("ShoppingCart.GiftCardCouponCode.Applied");
            model.GiftCardBox.IsApplied = true;
        }
        else
        {
            model.GiftCardBox.Message = validationError;
            model.GiftCardBox.IsApplied = false;
        }

        model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);
		_logger.LogWarning("<<< Exiting ApplyGiftCard(giftcardcouponcode={Giftcardcouponcode},form={Form})", giftcardcouponcode, form);
        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> GetEstimateShipping(EstimateShippingModel model, IFormCollection form)
    {
		_logger.LogError(">>> Entering GetEstimateShipping(model={Model},form={Form})", model, form);
        if (model == null)
            model = new EstimateShippingModel();
			_logger.LogError("GetEstimateShipping(model={Model},form={Form}): model → {Model1}", model, form, model);

        var errors = new List<string>();
		_logger.LogError("GetEstimateShipping(model={Model},form={Form}): errors → {Errors}", model, form, errors);

        if (!_shippingSettings.EstimateShippingCityNameEnabled && string.IsNullOrEmpty(model.ZipPostalCode))
            errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShipping.ZipPostalCode.Required"));

        if (_shippingSettings.EstimateShippingCityNameEnabled && string.IsNullOrEmpty(model.City))
            errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShipping.City.Required"));

        if (model.CountryId == null || model.CountryId == 0)
            errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShipping.Country.Required"));

        if (errors.Count > 0)
            return Json(new
            {
                Success = false,
                Errors = errors
            });
			_logger.LogDebug("GetEstimateShipping(model={Model},form={Form}): Errors → {Errors}", model, form, Errors);

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("GetEstimateShipping(model={Model},form={Form}): store → {Store}", model, form, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogError("GetEstimateShipping(model={Model},form={Form}): cart → {Cart}", model, form, cart);
        //parse and save checkout attributes
        await ParseAndSaveCheckoutAttributesAsync(cart, form);

        var result = await _shoppingCartModelFactory.PrepareEstimateShippingResultModelAsync(cart, model, true);
		_logger.LogError("GetEstimateShipping(model={Model},form={Form}): result → {Result}", model, form, result);

		_logger.LogWarning("<<< Exiting GetEstimateShipping(model={Model},form={Form})", model, form);
        return Json(result);
    }

    [HttpPost, ActionName("Cart")]
    [FormValueRequired(FormValueRequirement.StartsWith, "removediscount-")]
    public virtual async Task<IActionResult> RemoveDiscountCoupon(IFormCollection form)
    {
		_logger.LogError(">>> Entering RemoveDiscountCoupon(form={Form})", form);
        var model = new ShoppingCartModel();

		_logger.LogError("RemoveDiscountCoupon(form={Form}): model → {Model}", form, model);
        //get discount identifier
        var discountId = 0;
		_logger.LogError("RemoveDiscountCoupon(form={Form}): discountId → {DiscountId}", form, discountId);
        foreach (var formValue in form.Keys)
            if (formValue.StartsWith("removediscount-", StringComparison.InvariantCultureIgnoreCase))
                discountId = Convert.ToInt32(formValue["removediscount-".Length..]);
        var discount = await _discountService.GetDiscountByIdAsync(discountId);
		_logger.LogError("RemoveDiscountCoupon(form={Form}): discount → {Discount}", form, discount);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("RemoveDiscountCoupon(form={Form}): customer → {Customer}", form, customer);
        if (discount != null)
            await _customerService.RemoveDiscountCouponCodeAsync(customer, discount.CouponCode);

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("RemoveDiscountCoupon(form={Form}): store → {Store}", form, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogError("RemoveDiscountCoupon(form={Form}): cart → {Cart}", form, cart);

        model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);
		_logger.LogWarning("<<< Exiting RemoveDiscountCoupon(form={Form})", form);
        return View(model);
    }

    [HttpPost, ActionName("Cart")]
    [FormValueRequired(FormValueRequirement.StartsWith, "removegiftcard-")]
    public virtual async Task<IActionResult> RemoveGiftCardCode(IFormCollection form)
    {
		_logger.LogError(">>> Entering RemoveGiftCardCode(form={Form})", form);
        var model = new ShoppingCartModel();

		_logger.LogError("RemoveGiftCardCode(form={Form}): model → {Model}", form, model);
        //get gift card identifier
        var giftCardId = 0;
		_logger.LogError("RemoveGiftCardCode(form={Form}): giftCardId → {GiftCardId}", form, giftCardId);
        foreach (var formValue in form.Keys)
            if (formValue.StartsWith("removegiftcard-", StringComparison.InvariantCultureIgnoreCase))
                giftCardId = Convert.ToInt32(formValue["removegiftcard-".Length..]);
        var gc = await _giftCardService.GetGiftCardByIdAsync(giftCardId);
		_logger.LogError("RemoveGiftCardCode(form={Form}): gc → {Gc}", form, gc);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("RemoveGiftCardCode(form={Form}): customer → {Customer}", form, customer);
        if (gc != null)
            await _customerService.RemoveGiftCardCouponCodeAsync(customer, gc.GiftCardCouponCode);

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("RemoveGiftCardCode(form={Form}): store → {Store}", form, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogError("RemoveGiftCardCode(form={Form}): cart → {Cart}", form, cart);

        model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);
		_logger.LogWarning("<<< Exiting RemoveGiftCardCode(form={Form})", form);
        return View(model);
    }

    #endregion

    #region Wishlist

    public virtual async Task<IActionResult> Wishlist(Guid? customerGuid, int? list)
    {
		_logger.LogError(">>> Entering Wishlist(customerGuid={CustomerGuid},list={List})", customerGuid, list);
        if (!await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_WISHLIST))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var customer = customerGuid.HasValue
            ? await _customerService.GetCustomerByGuidAsync(customerGuid.Value)
            : await _workContext.GetCurrentCustomerAsync();
        if (customer == null)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("Wishlist(customerGuid={CustomerGuid},list={List}): store → {Store}", customerGuid, list, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, store.Id, customWishlistId: list);
		_logger.LogError("Wishlist(customerGuid={CustomerGuid},list={List}): cart → {Cart}", customerGuid, list, cart);

        var model = new WishlistModel();
		_logger.LogError("Wishlist(customerGuid={CustomerGuid},list={List}): model → {Model}", customerGuid, list, model);
        model = await _shoppingCartModelFactory.PrepareWishlistModelAsync(model, cart, !customerGuid.HasValue, list);
		_logger.LogWarning("<<< Exiting Wishlist(customerGuid={CustomerGuid},list={List})", customerGuid, list);
        return View(model);
    }

    [HttpPost, ActionName("Wishlist")]
    [FormValueRequired("updatecart")]
    public virtual async Task<IActionResult> UpdateWishlist(WishlistModel wishlistModel, IFormCollection form)
    {
		_logger.LogError(">>> Entering UpdateWishlist(wishlistModel={WishlistModel},form={Form})", wishlistModel, form);
        if (!await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_WISHLIST))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("UpdateWishlist(wishlistModel={WishlistModel},form={Form}): customer → {Customer}", wishlistModel, form, customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("UpdateWishlist(wishlistModel={WishlistModel},form={Form}): store → {Store}", wishlistModel, form, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, store.Id, customWishlistId: wishlistModel.ListId);
		_logger.LogError("UpdateWishlist(wishlistModel={WishlistModel},form={Form}): cart → {Cart}", wishlistModel, form, cart);

        var allIdsToRemove = form.ContainsKey("removefromcart")
            ? form["removefromcart"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList()
            : new List<int>();

        //current warnings <cart item identifier, warnings>
        var innerWarnings = new Dictionary<int, IList<string>>();
		_logger.LogError("UpdateWishlist(wishlistModel={WishlistModel},form={Form}): innerWarnings → {InnerWarnings}", wishlistModel, form, innerWarnings);
        foreach (var sci in cart)
        {
            var remove = allIdsToRemove.Contains(sci.Id);
			_logger.LogError("UpdateWishlist(wishlistModel={WishlistModel},form={Form}): remove → {Remove}", wishlistModel, form, remove);
            if (remove)
                await _shoppingCartService.DeleteShoppingCartItemAsync(sci);
            else
            {
                foreach (var formKey in form.Keys)
                    if (formKey.Equals($"itemquantity{sci.Id}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (int.TryParse(form[formKey], out var newQuantity))
                        {
                            var currSciWarnings = await _shoppingCartService.UpdateShoppingCartItemAsync(customer,
                                sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
                                sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                                newQuantity, true);
                            innerWarnings.Add(sci.Id, currSciWarnings);
                        }

                        break;
                    }
            }
        }

        //updated wishlist
        cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, store.Id, customWishlistId: wishlistModel.ListId);
        var model = new WishlistModel();
		_logger.LogError("UpdateWishlist(wishlistModel={WishlistModel},form={Form}): model → {Model}", wishlistModel, form, model);
        model = await _shoppingCartModelFactory.PrepareWishlistModelAsync(model, cart, list: wishlistModel.ListId);
        //update current warnings
        foreach (var kvp in innerWarnings)
        {
            //kvp = <cart item identifier, warnings>
            var sciId = kvp.Key;
            var warnings = kvp.Value;
            //find model
            var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
			_logger.LogError("UpdateWishlist(wishlistModel={WishlistModel},form={Form}): sciModel → {SciModel}", wishlistModel, form, sciModel);
            if (sciModel != null)
                foreach (var w in warnings)
                    if (!sciModel.Warnings.Contains(w))
                        sciModel.Warnings.Add(w);
        }

		_logger.LogWarning("<<< Exiting UpdateWishlist(wishlistModel={WishlistModel},form={Form})", wishlistModel, form);
        return View(model);
    }

    [HttpPost, ActionName("Wishlist")]
    [FormValueRequired("addtocartbutton")]
    public virtual async Task<IActionResult> AddItemsToCartFromWishlist(Guid? customerGuid, WishlistModel wishlistModel, IFormCollection form)
    {
		_logger.LogError(">>> Entering AddItemsToCartFromWishlist(customerGuid={CustomerGuid},wishlistModel={WishlistModel},form={Form})", customerGuid, wishlistModel, form);
        if (!await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (!await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_WISHLIST))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("AddItemsToCartFromWishlist(customerGuid={CustomerGuid},wishlistModel={WishlistModel},form={Form}): customer → {Customer}", customerGuid, wishlistModel, form, customer);
        var pageCustomer = customerGuid.HasValue
            ? await _customerService.GetCustomerByGuidAsync(customerGuid.Value)
            : customer;
        if (pageCustomer == null)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("AddItemsToCartFromWishlist(customerGuid={CustomerGuid},wishlistModel={WishlistModel},form={Form}): store → {Store}", customerGuid, wishlistModel, form, store);
        var pageCart = await _shoppingCartService.GetShoppingCartAsync(pageCustomer, ShoppingCartType.Wishlist, store.Id, customWishlistId: wishlistModel.ListId);
		_logger.LogError("AddItemsToCartFromWishlist(customerGuid={CustomerGuid},wishlistModel={WishlistModel},form={Form}): pageCart → {PageCart}", customerGuid, wishlistModel, form, pageCart);

        var allWarnings = new List<string>();
		_logger.LogError("AddItemsToCartFromWishlist(customerGuid={CustomerGuid},wishlistModel={WishlistModel},form={Form}): allWarnings → {AllWarnings}", customerGuid, wishlistModel, form, allWarnings);
        var countOfAddedItems = 0;
        var allIdsToAdd = form.ContainsKey("addtocart")
            ? form["addtocart"].ToString().Split(_separator, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
            : [];
        foreach (var sci in pageCart)
        {
            if (allIdsToAdd.Contains(sci.Id))
            {
                var product = await _productService.GetProductByIdAsync(sci.ProductId);
				_logger.LogError("AddItemsToCartFromWishlist(customerGuid={CustomerGuid},wishlistModel={WishlistModel},form={Form}): product → {Product}", customerGuid, wishlistModel, form, product);

                var warnings = await _shoppingCartService.AddToCartAsync(customer,
                    product, ShoppingCartType.ShoppingCart,
                    store.Id,
                    sci.AttributesXml, sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, true);
                if (!warnings.Any())
                    countOfAddedItems++;
                if (_shoppingCartSettings.MoveItemsFromWishlistToCart && //settings enabled
                    !customerGuid.HasValue && //own wishlist
                    !warnings.Any()) //no warnings ( already in the cart)
                {
                    //let's remove the item from wishlist
                    await _shoppingCartService.DeleteShoppingCartItemAsync(sci);
                }

                allWarnings.AddRange(warnings);
            }
        }

        if (countOfAddedItems > 0)
        {
            //redirect to the shopping cart page

            if (allWarnings.Any())
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Wishlist.AddToCart.Error"));
            }

            return RedirectToRoute(NopRouteNames.General.CART);
        }
        else
        {
            _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Wishlist.AddToCart.NoAddedItems"));
        }
        //no items added. redisplay the wishlist page

        if (allWarnings.Any())
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Wishlist.AddToCart.Error"));
        }

        var cart = await _shoppingCartService.GetShoppingCartAsync(pageCustomer, ShoppingCartType.Wishlist, store.Id, customWishlistId: wishlistModel.ListId);
		_logger.LogError("AddItemsToCartFromWishlist(customerGuid={CustomerGuid},wishlistModel={WishlistModel},form={Form}): cart → {Cart}", customerGuid, wishlistModel, form, cart);

        var model = new WishlistModel();
		_logger.LogError("AddItemsToCartFromWishlist(customerGuid={CustomerGuid},wishlistModel={WishlistModel},form={Form}): model → {Model}", customerGuid, wishlistModel, form, model);
        model = await _shoppingCartModelFactory.PrepareWishlistModelAsync(model, cart, !customerGuid.HasValue, list: wishlistModel.ListId);
        return View(model);
    }

    public virtual async Task<IActionResult> EmailWishlist(int? wishlistId = null)
    {
		_logger.LogError(">>> Entering EmailWishlist(wishlistId={WishlistId})", wishlistId);
        if (!await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_WISHLIST) || !_shoppingCartSettings.EmailWishlistEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("EmailWishlist(wishlistId={WishlistId}): store → {Store}", wishlistId, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.Wishlist, store.Id, customWishlistId: wishlistId);
		_logger.LogError("EmailWishlist(wishlistId={WishlistId}): cart → {Cart}", wishlistId, cart);

        if (!cart.Any())
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = new WishlistEmailAFriendModel();
		_logger.LogError("EmailWishlist(wishlistId={WishlistId}): model → {Model}", wishlistId, model);
        model = await _shoppingCartModelFactory.PrepareWishlistEmailAFriendModelAsync(model, false, wishlistId);
        return View(model);
    }

    [HttpPost, ActionName("EmailWishlist")]
    [FormValueRequired("send-email")]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> EmailWishlistSend(WishlistEmailAFriendModel model, bool captchaValid)
    {
		_logger.LogError(">>> Entering EmailWishlistSend(model={Model},captchaValid={CaptchaValid})", model, captchaValid);
        if (!await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_WISHLIST) || !_shoppingCartSettings.EmailWishlistEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("EmailWishlistSend(model={Model},captchaValid={CaptchaValid}): customer → {Customer}", model, captchaValid, customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("EmailWishlistSend(model={Model},captchaValid={CaptchaValid}): store → {Store}", model, captchaValid, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, store.Id, customWishlistId: model.ListId);
		_logger.LogError("EmailWishlistSend(model={Model},captchaValid={CaptchaValid}): cart → {Cart}", model, captchaValid, cart);

        if (!cart.Any())
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnEmailWishlistToFriendPage && !captchaValid)
        {
            ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        //check whether the current customer is guest and is allowed to email wishlist
        if (await _customerService.IsGuestAsync(customer) && !_shoppingCartSettings.AllowAnonymousUsersToEmailWishlist)
        {
            ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("Wishlist.EmailAFriend.OnlyRegisteredUsers"));
        }

        if (ModelState.IsValid)
        {
            //email
            var wishlistUrl = Url.RouteUrl(NopRouteNames.General.WISHLIST, new { customerGuid = customer.CustomerGuid, list = model.ListId }, _webHelper.GetCurrentRequestProtocol());
			_logger.LogError("EmailWishlistSend(model={Model},captchaValid={CaptchaValid}): wishlistUrl → {WishlistUrl}", model, captchaValid, wishlistUrl);

            await _workflowMessageService.SendWishlistEmailAFriendMessageAsync(customer,
                (await _workContext.GetWorkingLanguageAsync()).Id,
                model.YourEmailAddress,
                model.FriendEmail,
                _htmlFormatter.FormatText(model.PersonalMessage, false, true, false, false, false, false),
                wishlistUrl);

            model.SuccessfullySent = true;
			_logger.LogDebug("EmailWishlistSend(model={Model},captchaValid={CaptchaValid}): model.SuccessfullySent → {Arg2}", model, captchaValid, model.SuccessfullySent);
            model.Result = await _localizationService.GetResourceAsync("Wishlist.EmailAFriend.SuccessfullySent");

            return View(model);
        }

        //If we got this far, something failed, redisplay form
        model = await _shoppingCartModelFactory.PrepareWishlistEmailAFriendModelAsync(model, true);
		_logger.LogError("EmailWishlistSend(model={Model},captchaValid={CaptchaValid}): model → {Model1}", model, captchaValid, model);

		_logger.LogWarning("<<< Exiting EmailWishlistSend(model={Model},captchaValid={CaptchaValid})", model, captchaValid);
        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> AddWishlist(string name, int productId)
    {
		_logger.LogError(">>> Entering AddWishlist(name={Name},productId={ProductId})", name, productId);
        var customer = await _workContext.GetCurrentCustomerAsync();
        var isGuest = await _customerService.IsGuestAsync(customer);
		_logger.LogError("AddWishlist(name={Name},productId={ProductId}): isGuest → {IsGuest}", name, productId, isGuest);
		_logger.LogError("AddWishlist(name={Name},productId={ProductId}): customer → {Customer}", name, productId, customer);

        if (isGuest)
        {
            return Json(new
            {
                success = false,
                message = await _localizationService.GetResourceAsync("Wishlist.MultipleWishlistNotForGuest")
            });
				_logger.LogDebug("AddWishlist(name={Name},productId={ProductId}): message → {Message}", name, productId, message);
        }

        if (!_shoppingCartSettings.AllowMultipleWishlist)
        {
            return Json(new
            {
                success = false,
                message = await _localizationService.GetResourceAsync("Wishlist.NotAllowMultipleWishlist")
            });
        }

        // Check if customer has reached the maximum number of custom wishlists allowed
        var currentWishlists = await _customWishlistService.GetAllCustomWishlistsAsync(customer.Id);
		_logger.LogError("AddWishlist(name={Name},productId={ProductId}): currentWishlists → {CurrentWishlists}", name, productId, currentWishlists);
        var maximumNumberOfCustomWishlist = _shoppingCartSettings.MaximumNumberOfCustomWishlist;
        if (currentWishlists.Count >= maximumNumberOfCustomWishlist)
        {
            return Json(new
            {
                success = false,
                message = string.Format(await _localizationService.GetResourceAsync("Wishlist.MaximumNumberReached"), maximumNumberOfCustomWishlist)
            });
        }

        var customWishlist = new CustomWishlist
        {
            CustomerId = customer.Id,
            Name = name.Trim(),
            CreatedOnUtc = DateTime.UtcNow
        };

        // add a new wishlist
        await _customWishlistService.AddCustomWishlistAsync(customWishlist);

        var product = await _productService.GetProductByIdAsync(productId);
		_logger.LogError("AddWishlist(name={Name},productId={ProductId}): product → {Product}", name, productId, product);
        if (product != null)
        {
            await MoveProductToCustomWishlist(product.Id, customWishlist.Id);
        }

		_logger.LogDebug("AddWishlist(name={Name},productId={ProductId}): redirect → {Redirect}", name, productId, redirect);
		_logger.LogWarning("<<< Exiting AddWishlist(name={Name},productId={ProductId})", name, productId);
        return Json(new
        {
            success = true,
            redirect = Url.RouteUrl(NopRouteNames.General.WISHLIST, new { list = customWishlist.Id })
        });
    }

    [HttpPost]
    public virtual async Task<IActionResult> MoveProductToCustomWishlist(int productId, int wishlistId)
    {
		_logger.LogError(">>> Entering MoveProductToCustomWishlist(productId={ProductId},wishlistId={WishlistId})", productId, wishlistId);
        var customer = await _workContext.GetCurrentCustomerAsync();
        var isGuest = await _customerService.IsGuestAsync(customer);
		_logger.LogError("MoveProductToCustomWishlist(productId={ProductId},wishlistId={WishlistId}): isGuest → {IsGuest}", productId, wishlistId, isGuest);
		_logger.LogError("MoveProductToCustomWishlist(productId={ProductId},wishlistId={WishlistId}): customer → {Customer}", productId, wishlistId, customer);

        if (isGuest)
        {
            return Json(new
            {
                success = false,
                message = await _localizationService.GetResourceAsync("Wishlist.MultipleWishlistNotForGuest")
            });
				_logger.LogDebug("MoveProductToCustomWishlist(productId={ProductId},wishlistId={WishlistId}): message → {Message}", productId, wishlistId, message);
        }

        if (!_shoppingCartSettings.AllowMultipleWishlist)
        {
            return Json(new
            {
                success = false,
                message = await _localizationService.GetResourceAsync("Wishlist.NotAllowMultipleWishlist")
            });
        }

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("MoveProductToCustomWishlist(productId={ProductId},wishlistId={WishlistId}): store → {Store}", productId, wishlistId, store);
        var shoppingCarts = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, store.Id);
		_logger.LogError("MoveProductToCustomWishlist(productId={ProductId},wishlistId={WishlistId}): shoppingCarts → {ShoppingCarts}", productId, wishlistId, shoppingCarts);

        var product = await _productService.GetProductByIdAsync(productId);
		_logger.LogError("MoveProductToCustomWishlist(productId={ProductId},wishlistId={WishlistId}): product → {Product}", productId, wishlistId, product);
        if (product != null)
        {
            var shoppingCartItem = shoppingCarts
                .Where(sci => sci.ProductId == product.Id
                    && sci.ShoppingCartType == ShoppingCartType.Wishlist
                    && sci.CustomWishlistId == null)
                .OrderByDescending(sci => sci.UpdatedOnUtc)
                .FirstOrDefault();

            if (shoppingCartItem != null)
                await MoveToCustomWishlist(shoppingCartItem.Id, wishlistId);
        }
        var redirectUrl = Url.RouteUrl(NopRouteNames.General.WISHLIST, new { list = wishlistId });
		_logger.LogError("MoveProductToCustomWishlist(productId={ProductId},wishlistId={WishlistId}): redirectUrl → {RedirectUrl}", productId, wishlistId, redirectUrl);
        var customWishlist = await _customWishlistService.GetCustomWishlistByIdAsync(wishlistId);
		_logger.LogError("MoveProductToCustomWishlist(productId={ProductId},wishlistId={WishlistId}): customWishlist → {CustomWishlist}", productId, wishlistId, customWishlist);

		_logger.LogWarning("<<< Exiting MoveProductToCustomWishlist(productId={ProductId},wishlistId={WishlistId})", productId, wishlistId);
        return Json(new
        {
            success = true,
            message = string.Format(await _localizationService.GetResourceAsync("Products.ProductHasBeenAddedToTheCustomWishlist.Link"), redirectUrl, WebUtility.HtmlEncode(customWishlist?.Name) ?? "")
        });
    }

    [HttpPost]
    public virtual async Task<IActionResult> MoveToCustomWishlist(int shoppingCartItemId, int customWishlistId)
    {
		_logger.LogError(">>> Entering MoveToCustomWishlist(shoppingCartItemId=<redacted>,customWishlistId={CustomWishlistId})", customWishlistId);
        var customer = await _workContext.GetCurrentCustomerAsync();
        var redirectUrl = Url.RouteUrl(NopRouteNames.General.WISHLIST, new { list = customWishlistId });
		_logger.LogError("MoveToCustomWishlist(shoppingCartItemId=<redacted>,customWishlistId={CustomWishlistId}): customer → {Customer}", customWishlistId, customer);
        if (customWishlistId > 0)
        {
			_logger.LogError("MoveToCustomWishlist(shoppingCartItemId=<redacted>,customWishlistId={CustomWishlistId}): redirectUrl → {RedirectUrl}", customWishlistId, redirectUrl);
            var wishlist = await _customWishlistService.GetCustomWishlistByIdAsync(customWishlistId);
			_logger.LogError("MoveToCustomWishlist(shoppingCartItemId=<redacted>,customWishlistId={CustomWishlistId}): wishlist → {Wishlist}", customWishlistId, wishlist);

            if (wishlist == null || wishlist.CustomerId != customer.Id)
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("Wishlist.NotFound")
                });
					_logger.LogDebug("MoveToCustomWishlist(shoppingCartItemId=<redacted>,customWishlistId={CustomWishlistId}): message → {Message}", customWishlistId, message);

            await _shoppingCartService.MoveItemToCustomWishlistAsync(shoppingCartItemId, wishlist.Id);
        }
        else
        {
            await _shoppingCartService.MoveItemToCustomWishlistAsync(shoppingCartItemId);
            redirectUrl = Url.RouteUrl(NopRouteNames.General.WISHLIST);
        }
		_logger.LogDebug("MoveToCustomWishlist(shoppingCartItemId=<redacted>,customWishlistId={CustomWishlistId}): redirect → {Redirect}", customWishlistId, redirect);
		_logger.LogWarning("<<< Exiting MoveToCustomWishlist(shoppingCartItemId=<redacted>,customWishlistId={CustomWishlistId})", customWishlistId);
        return Json(new
        {
            redirect = redirectUrl
        });
    }

    [HttpPost]
    public virtual async Task<IActionResult> DeleteWishlist(int wishlistId)
    {
		_logger.LogError(">>> Entering DeleteWishlist(wishlistId={WishlistId})", wishlistId);
        var customer = await _workContext.GetCurrentCustomerAsync();
        var wishlist = await _customWishlistService.GetCustomWishlistByIdAsync(wishlistId);
		_logger.LogError("DeleteWishlist(wishlistId={WishlistId}): wishlist → {Wishlist}", wishlistId, wishlist);
		_logger.LogError("DeleteWishlist(wishlistId={WishlistId}): customer → {Customer}", wishlistId, customer);

        if (wishlist == null || wishlist.CustomerId != customer.Id)
        {
            return Json(new
            {
                success = false,
                message = await _localizationService.GetResourceAsync("Wishlist.NotFound")
            });
				_logger.LogDebug("DeleteWishlist(wishlistId={WishlistId}): message → {Message}", wishlistId, message);
        }

        // Delete all shopping cart items associated with this wishlist
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("DeleteWishlist(wishlistId={WishlistId}): store → {Store}", wishlistId, store);
        var cartItems = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, store.Id, customWishlistId: wishlist.Id);
		_logger.LogError("DeleteWishlist(wishlistId={WishlistId}): cartItems → {CartItems}", wishlistId, cartItems);
        foreach (var item in cartItems)
        {
            await _shoppingCartService.DeleteShoppingCartItemAsync(item);
        }

        // Delete the wishlist itself
        await _customWishlistService.RemoveCustomWishlistAsync(wishlist.Id);

		_logger.LogDebug("DeleteWishlist(wishlistId={WishlistId}): redirect → {Redirect}", wishlistId, redirect);
		_logger.LogWarning("<<< Exiting DeleteWishlist(wishlistId={WishlistId})", wishlistId);
        return Json(new
        {
            redirect = Url.RouteUrl(NopRouteNames.General.WISHLIST)
        });
    }

    #endregion
}