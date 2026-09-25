using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Orders;

/// <summary>
/// Shopping cart service
/// </summary>
public partial class ShoppingCartService : IShoppingCartService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IAclService _aclService;
    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
    protected readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateRangeService _dateRangeService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IGiftCardService _giftCardService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPriceCalculationService _priceCalculationService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;
    protected readonly IRepository<ShoppingCartItem> _sciRepository;
    protected readonly IShippingService _shippingService;
    protected readonly IShortTermCacheManager _shortTermCacheManager;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;
    protected readonly OrderSettings _orderSettings;
    protected readonly ShoppingCartSettings _shoppingCartSettings;

    #endregion

    #region Ctor

    public ShoppingCartService(CatalogSettings catalogSettings,
        IAclService aclService,
        IActionContextAccessor actionContextAccessor,
        IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
        IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IDateRangeService dateRangeService,
        IDateTimeHelper dateTimeHelper,
        IEventPublisher eventPublisher,
        IGenericAttributeService genericAttributeService,
        IGiftCardService giftCardService,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        IPriceCalculationService priceCalculationService,
        IPriceFormatter priceFormatter,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IProductService productService,
        IRepository<ShoppingCartItem> sciRepository,
        IShippingService shippingService,
        IShortTermCacheManager shortTermCacheManager,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreService storeService,
        IStoreMappingService storeMappingService,
        IUrlHelperFactory urlHelperFactory,
        IUrlRecordService urlRecordService,
        IWorkContext workContext,
        OrderSettings orderSettings,
        ShoppingCartSettings shoppingCartSettings)
    {
        _catalogSettings = catalogSettings;
        _aclService = aclService;
        _actionContextAccessor = actionContextAccessor;
        _checkoutAttributeParser = checkoutAttributeParser;
        _checkoutAttributeService = checkoutAttributeService;
        _currencyService = currencyService;
        _customerService = customerService;
        _dateRangeService = dateRangeService;
        _dateTimeHelper = dateTimeHelper;
        _eventPublisher = eventPublisher;
        _genericAttributeService = genericAttributeService;
        _giftCardService = giftCardService;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _priceCalculationService = priceCalculationService;
        _priceFormatter = priceFormatter;
        _productAttributeParser = productAttributeParser;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _sciRepository = sciRepository;
        _shippingService = shippingService;
        _shortTermCacheManager = shortTermCacheManager;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _storeService = storeService;
        _storeMappingService = storeMappingService;
        _urlHelperFactory = urlHelperFactory;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
        _orderSettings = orderSettings;
        _shoppingCartSettings = shoppingCartSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Determine if the shopping cart item is the same as the one being compared
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="customerEnteredPrice">Price entered by a customer</param>
    /// <param name="rentalStartDate">Rental start date</param>
    /// <param name="rentalEndDate">Rental end date</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping cart item is equal
    /// </returns>
    protected virtual async Task<bool> ShoppingCartItemIsEqualAsync(ShoppingCartItem shoppingCartItem,
        Product product,
        string attributesXml,
        decimal customerEnteredPrice,
        DateTime? rentalStartDate,
        DateTime? rentalEndDate)
    {
		_logger.LogError(">>> Entering ShoppingCartItemIsEqualAsync(shoppingCartItem=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate})", product, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate);
        if (shoppingCartItem.ProductId != product.Id)
            return false;

        //attributes
        var attributesEqual = await _productAttributeParser.AreProductAttributesEqualAsync(shoppingCartItem.AttributesXml, attributesXml, false, false);
		_logger.LogError("ShoppingCartItemIsEqualAsync(shoppingCartItem=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate}): attributesEqual → {AttributesEqual}", product, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, attributesEqual);
        if (!attributesEqual)
            return false;

        //gift cards
        if (product.IsGiftCard)
        {
            _productAttributeParser.GetGiftCardAttribute(attributesXml, out var giftCardRecipientName1, out var _, out var giftCardSenderName1, out var _, out var _);

            _productAttributeParser.GetGiftCardAttribute(shoppingCartItem.AttributesXml, out var giftCardRecipientName2, out var _, out var giftCardSenderName2, out var _, out var _);

            var giftCardsAreEqual = giftCardRecipientName1.Equals(giftCardRecipientName2, StringComparison.InvariantCultureIgnoreCase)
                                    && giftCardSenderName1.Equals(giftCardSenderName2, StringComparison.InvariantCultureIgnoreCase);
            if (!giftCardsAreEqual)
                return false;
        }

        //price is the same (for products which require customers to enter a price)
        if (product.CustomerEntersPrice)
        {
            //we use rounding to eliminate errors associated with storing real numbers in memory when comparing
            var customerEnteredPricesEqual = Math.Round(shoppingCartItem.CustomerEnteredPrice, 2) == Math.Round(customerEnteredPrice, 2);
			_logger.LogError("ShoppingCartItemIsEqualAsync(shoppingCartItem=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate}): customerEnteredPricesEqual → {CustomerEnteredPricesEqual}", product, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, customerEnteredPricesEqual);
            if (!customerEnteredPricesEqual)
                return false;
        }

        if (!product.IsRental)
            return true;

        //rental products
        var rentalInfoEqual = shoppingCartItem.RentalStartDateUtc == rentalStartDate && shoppingCartItem.RentalEndDateUtc == rentalEndDate;

		_logger.LogWarning("<<< Exiting ShoppingCartItemIsEqualAsync(shoppingCartItem=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate})", product, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate);
        return rentalInfoEqual;
    }

    /// <summary>
    /// Gets a value indicating whether customer shopping cart is empty
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>Result</returns>
    protected virtual async Task<bool> IsCustomerShoppingCartEmptyAsync(Customer customer)
    {
		_logger.LogError(">>> Entering IsCustomerShoppingCartEmptyAsync(customer={Customer})", customer);
		_logger.LogError("IsCustomerShoppingCartEmptyAsync(customer)={Customer}: database query executing", customer);
    _logger.LogWarning("<<< Exiting IsCustomerShoppingCartEmptyAsync(customer={Customer})", customer);
        return !await _sciRepository.Table.AnyAsync(sci => sci.CustomerId == customer.Id);
    }

    /// <summary>
    /// Validates required products (products which require some other products to be added to the cart)
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="shoppingCartType">Shopping cart type</param>
    /// <param name="product">Product</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="quantity">Quantity</param>
    /// <param name="addRequiredProducts">Whether to add required products</param>
    /// <param name="shoppingCartItemId">Shopping cart identifier; pass 0 if it's a new item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    protected virtual async Task<IList<string>> GetRequiredProductWarningsAsync(Customer customer, ShoppingCartType shoppingCartType, Product product,
        int storeId, int quantity, bool addRequiredProducts, int shoppingCartItemId)
    {
		_logger.LogError(">>> Entering GetRequiredProductWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},storeId={StoreId},quantity={Quantity},addRequiredProducts={AddRequiredProducts},shoppingCartItemId=<redacted>)", customer, product, storeId, quantity, addRequiredProducts);
        ArgumentNullException.ThrowIfNull(customer);

        ArgumentNullException.ThrowIfNull(product);

        var warnings = new List<string>();
		_logger.LogError("GetRequiredProductWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},storeId={StoreId},quantity={Quantity},addRequiredProducts={AddRequiredProducts},shoppingCartItemId=<redacted>): warnings → {Warnings}", customer, product, storeId, quantity, addRequiredProducts, warnings);

        //at now we ignore quantities of required products and use 1
        var requiredProductQuantity = 1;

        //get customer shopping cart
        var cart = await GetShoppingCartAsync(customer, shoppingCartType, storeId);
		_logger.LogError("GetRequiredProductWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},storeId={StoreId},quantity={Quantity},addRequiredProducts={AddRequiredProducts},shoppingCartItemId=<redacted>): cart → {Cart}", customer, product, storeId, quantity, addRequiredProducts, cart);

        var productsRequiringProduct = await GetProductsRequiringProductAsync(cart, product);
		_logger.LogError("GetRequiredProductWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},storeId={StoreId},quantity={Quantity},addRequiredProducts={AddRequiredProducts},shoppingCartItemId=<redacted>): productsRequiringProduct → {ProductsRequiringProduct}", customer, product, storeId, quantity, addRequiredProducts, productsRequiringProduct);

        //whether other cart items require the passed product
        var passedProductRequiredQuantity = cart.Where(ci => productsRequiringProduct.Any(p => p.Id == ci.ProductId))
            .Sum(item => item.Quantity * requiredProductQuantity);

        if (passedProductRequiredQuantity > quantity)
            warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.RequiredProductUpdateWarning"), passedProductRequiredQuantity));

        //whether the passed product requires other products
        if (!product.RequireOtherProducts)
            return warnings;

        //get these required products
        var requiredProducts = await _productService.GetProductsByIdsAsync(_productService.ParseRequiredProductIds(product));
		_logger.LogError("GetRequiredProductWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},storeId={StoreId},quantity={Quantity},addRequiredProducts={AddRequiredProducts},shoppingCartItemId=<redacted>): requiredProducts → {RequiredProducts}", customer, product, storeId, quantity, addRequiredProducts, requiredProducts);
        if (!requiredProducts.Any())
            return warnings;

        var finalRequiredProducts = requiredProducts.GroupBy(p => p.Id)
            .Select(g => new { Product = g.First(), Count = g.Count() });

        //get warnings
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
		_logger.LogError("GetRequiredProductWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},storeId={StoreId},quantity={Quantity},addRequiredProducts={AddRequiredProducts},shoppingCartItemId=<redacted>): urlHelper → {UrlHelper}", customer, product, storeId, quantity, addRequiredProducts, urlHelper);
        var warningLocale = await _localizationService.GetResourceAsync("ShoppingCart.RequiredProductWarning");
		_logger.LogError("GetRequiredProductWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},storeId={StoreId},quantity={Quantity},addRequiredProducts={AddRequiredProducts},shoppingCartItemId=<redacted>): warningLocale → {WarningLocale}", customer, product, storeId, quantity, addRequiredProducts, warningLocale);
        foreach (var requiredProduct in finalRequiredProducts)
        {
            var productsRequiringRequiredProduct = await GetProductsRequiringProductAsync(cart, requiredProduct.Product);
			_logger.LogError("GetRequiredProductWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},storeId={StoreId},quantity={Quantity},addRequiredProducts={AddRequiredProducts},shoppingCartItemId=<redacted>): productsRequiringRequiredProduct → {ProductsRequiringRequiredProduct}", customer, product, storeId, quantity, addRequiredProducts, productsRequiringRequiredProduct);

            //get the required quantity of the required product
            var requiredProductRequiredQuantity = quantity * requiredProductQuantity +
                                                  cart.Where(ci => productsRequiringRequiredProduct.Any(p => p.Id == ci.ProductId))
                                                      .Where(item => item.Id != shoppingCartItemId)
                                                      .Sum(item => item.Quantity * requiredProductQuantity);

            //whether required product is already in the cart in the required quantity
            var quantityToAdd = requiredProductRequiredQuantity * requiredProduct.Count - (cart.FirstOrDefault(item => item.ProductId == requiredProduct.Product.Id)?.Quantity ?? 0);
			_logger.LogError("GetRequiredProductWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},storeId={StoreId},quantity={Quantity},addRequiredProducts={AddRequiredProducts},shoppingCartItemId=<redacted>): quantityToAdd → {QuantityToAdd}", customer, product, storeId, quantity, addRequiredProducts, quantityToAdd);
            if (quantityToAdd <= 0)
                continue;

            //prepare warning message
            var url = urlHelper.RouteUrl(nameof(Product), new { SeName = await _urlRecordService.GetSeNameAsync(requiredProduct.Product) });
			_logger.LogError("GetRequiredProductWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},storeId={StoreId},quantity={Quantity},addRequiredProducts={AddRequiredProducts},shoppingCartItemId=<redacted>): url → {Url}", customer, product, storeId, quantity, addRequiredProducts, url);
            var requiredProductName = WebUtility.HtmlEncode(await _localizationService.GetLocalizedAsync(requiredProduct.Product, x => x.Name));
			_logger.LogError("GetRequiredProductWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},storeId={StoreId},quantity={Quantity},addRequiredProducts={AddRequiredProducts},shoppingCartItemId=<redacted>): requiredProductName → {RequiredProductName}", customer, product, storeId, quantity, addRequiredProducts, requiredProductName);
            var requiredProductWarning = _catalogSettings.UseLinksInRequiredProductWarnings
                ? string.Format(warningLocale, $"<a href=\"{url}\">{requiredProductName}</a>", requiredProductRequiredQuantity * requiredProduct.Count)
                : string.Format(warningLocale, requiredProductName, requiredProductRequiredQuantity);

            //add to cart (if possible)
            if (addRequiredProducts && product.AutomaticallyAddRequiredProducts)
            {
                //do not add required products to prevent circular references
                var addToCartWarnings = await GetShoppingCartItemWarningsAsync(
                    customer: customer,
                    product: requiredProduct.Product,
                    attributesXml: null,
                    customerEnteredPrice: decimal.Zero,
                    shoppingCartType: shoppingCartType,
                    storeId: storeId,
                    quantity: quantityToAdd,
                    addRequiredProducts: true);

                //don't display all specific errors only the generic one
                if (addToCartWarnings.Any())
                    warnings.Add(requiredProductWarning);
            }
            else
            {
                warnings.Add(requiredProductWarning);
				_logger.LogWarning("<<< Exiting GetShoppingCartItemWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},storeId={StoreId},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},addRequiredProducts={AddRequiredProducts},shoppingCartItemId=<redacted>,getStandardWarnings={GetStandardWarnings},getAttributesWarnings={GetAttributesWarnings},getGiftCardWarnings={GetGiftCardWarnings},getRequiredProductWarnings={GetRequiredProductWarnings},getRentalWarnings={GetRentalWarnings})", customer, product, storeId, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, addRequiredProducts, getStandardWarnings, getAttributesWarnings, getGiftCardWarnings, getRequiredProductWarnings, getRentalWarnings);
            }
        }

		_logger.LogWarning("<<< Exiting GetRequiredProductWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},storeId={StoreId},quantity={Quantity},addRequiredProducts={AddRequiredProducts},shoppingCartItemId=<redacted>)", customer, product, storeId, quantity, addRequiredProducts);
        return warnings;
    }

    /// <summary>
    /// Validates a product for standard properties
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="shoppingCartType">Shopping cart type</param>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="customerEnteredPrice">Customer entered price</param>
    /// <param name="quantity">Quantity</param>
    /// <param name="shoppingCartItemId">Shopping cart identifier; pass 0 if it's a new item</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    protected virtual async Task<IList<string>> GetStandardWarningsAsync(Customer customer, ShoppingCartType shoppingCartType, Product product,
        string attributesXml, decimal customerEnteredPrice, int quantity, int shoppingCartItemId, int storeId)
    {
		_logger.LogError(">>> Entering GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId})", customer, product, attributesXml, customerEnteredPrice, quantity, storeId);
        ArgumentNullException.ThrowIfNull(customer);

        ArgumentNullException.ThrowIfNull(product);

        var warnings = new List<string>();
		_logger.LogError("GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId}): warnings → {Warnings}", customer, product, attributesXml, customerEnteredPrice, quantity, storeId, warnings);

        //deleted
        if (product.Deleted)
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ProductDeleted"));
            return warnings;
        }

        //published
        if (!product.Published)
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ProductUnpublished"));
        }

        //we can add only simple products
        if (product.ProductType != ProductType.SimpleProduct)
        {
            warnings.Add("This is not simple product");
        }

        //ACL
        if (!await _aclService.AuthorizeAsync(product, customer))
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ProductUnpublished"));
        }

        //Store mapping
        if (!await _storeMappingService.AuthorizeAsync(product, storeId))
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ProductUnpublished"));
        }

        //disabled "add to cart" button
        if (shoppingCartType == ShoppingCartType.ShoppingCart && product.DisableBuyButton)
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.BuyingDisabled"));
        }

        //disabled "add to wishlist" button
        if (shoppingCartType == ShoppingCartType.Wishlist && product.DisableWishlistButton)
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.WishlistDisabled"));
        }

        //call for price
        if (shoppingCartType == ShoppingCartType.ShoppingCart && product.CallForPrice &&
            //also check whether the current user is impersonated
            (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
        {
            warnings.Add(await _localizationService.GetResourceAsync("Products.CallForPrice"));
        }

        //customer entered price
        if (product.CustomerEntersPrice)
        {
            if (customerEnteredPrice < product.MinimumCustomerEnteredPrice ||
                customerEnteredPrice > product.MaximumCustomerEnteredPrice)
            {
                var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
				_logger.LogError("GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId}): currentCurrency → {CurrentCurrency}", customer, product, attributesXml, customerEnteredPrice, quantity, storeId, currentCurrency);
                var minimumCustomerEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(product.MinimumCustomerEnteredPrice, currentCurrency);
				_logger.LogError("GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId}): minimumCustomerEnteredPrice → {MinimumCustomerEnteredPrice}", customer, product, attributesXml, customerEnteredPrice, quantity, storeId, minimumCustomerEnteredPrice);
                var maximumCustomerEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(product.MaximumCustomerEnteredPrice, currentCurrency);
				_logger.LogError("GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId}): maximumCustomerEnteredPrice → {MaximumCustomerEnteredPrice}", customer, product, attributesXml, customerEnteredPrice, quantity, storeId, maximumCustomerEnteredPrice);
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.CustomerEnteredPrice.RangeError"),
                    await _priceFormatter.FormatPriceAsync(minimumCustomerEnteredPrice, false, false),
                    await _priceFormatter.FormatPriceAsync(maximumCustomerEnteredPrice, false, false)));
            }
        }

        //quantity validation
        var hasQtyWarnings = false;
		_logger.LogError("GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId}): hasQtyWarnings → {HasQtyWarnings}", customer, product, attributesXml, customerEnteredPrice, quantity, storeId, hasQtyWarnings);
        if (quantity < product.OrderMinimumQuantity)
        {
            warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MinimumQuantity"), product.OrderMinimumQuantity));
            hasQtyWarnings = true;
        }

        if (quantity > product.OrderMaximumQuantity)
        {
            warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumQuantity"), product.OrderMaximumQuantity));
            hasQtyWarnings = true;
        }

        var allowedQuantities = _productService.ParseAllowedQuantities(product);
		_logger.LogError("GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId}): allowedQuantities → {AllowedQuantities}", customer, product, attributesXml, customerEnteredPrice, quantity, storeId, allowedQuantities);
        if (allowedQuantities.Length > 0 && !allowedQuantities.Contains(quantity))
        {
            warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.AllowedQuantities"), string.Join(", ", allowedQuantities)));
        }

        var validateOutOfStock = shoppingCartType == ShoppingCartType.ShoppingCart || !_shoppingCartSettings.AllowOutOfStockItemsToBeAddedToWishlist;
        if (validateOutOfStock && !hasQtyWarnings)
        {
            switch (product.ManageInventoryMethod)
            {
                case ManageInventoryMethod.DontManageStock:
                    //do nothing
                    break;
                case ManageInventoryMethod.ManageStock:
                    if (product.BackorderMode == BackorderMode.NoBackorders)
                    {
                        var maximumQuantityCanBeAdded = await _productService.GetTotalStockQuantityAsync(product);
						_logger.LogError("GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId}): maximumQuantityCanBeAdded → {MaximumQuantityCanBeAdded}", customer, product, attributesXml, customerEnteredPrice, quantity, storeId, maximumQuantityCanBeAdded);

                        warnings.AddRange(await GetQuantityProductWarningsAsync(product, quantity, maximumQuantityCanBeAdded));

                        if (warnings.Any())
                            return warnings;

                        //validate product quantity with non combinable product attributes
                        var productAttributeMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
						_logger.LogError("GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId}): productAttributeMappings → {ProductAttributeMappings}", customer, product, attributesXml, customerEnteredPrice, quantity, storeId, productAttributeMappings);
                        if (productAttributeMappings?.Any() == true)
                        {
                            var onlyCombinableAttributes = productAttributeMappings.All(mapping => !mapping.IsNonCombinable());
							_logger.LogError("GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId}): onlyCombinableAttributes → {OnlyCombinableAttributes}", customer, product, attributesXml, customerEnteredPrice, quantity, storeId, onlyCombinableAttributes);
                            if (!onlyCombinableAttributes)
                            {
                                var cart = await GetShoppingCartAsync(customer, shoppingCartType, storeId);
								_logger.LogError("GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId}): cart → {Cart}", customer, product, attributesXml, customerEnteredPrice, quantity, storeId, cart);
                                var totalAddedQuantity = cart
                                    .Where(item => item.ProductId == product.Id && item.Id != shoppingCartItemId)
                                    .Sum(product => product.Quantity);

                                totalAddedQuantity += quantity;

                                //counting a product into bundles
                                foreach (var bundle in cart.Where(x => x.Id != shoppingCartItemId && !string.IsNullOrEmpty(x.AttributesXml)))
                                {
                                    var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(bundle.AttributesXml);
									_logger.LogError("GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId}): attributeValues → {AttributeValues}", customer, product, attributesXml, customerEnteredPrice, quantity, storeId, attributeValues);
                                    foreach (var attributeValue in attributeValues)
                                    {
                                        if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct && attributeValue.AssociatedProductId == product.Id)
                                            totalAddedQuantity += bundle.Quantity * attributeValue.Quantity;
                                    }
									_logger.LogWarning("<<< Exiting GetShoppingCartAsync(customer={Customer},shoppingCartType=<redacted>,storeId={StoreId},productId={ProductId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},customWishlistId={CustomWishlistId})", customer, storeId, productId, createdFromUtc, createdToUtc, customWishlistId);
                                }

                                warnings.AddRange(await GetQuantityProductWarningsAsync(product, totalAddedQuantity, maximumQuantityCanBeAdded));
                            }
                        }

                        if (warnings.Any())
                            return warnings;

                        //validate product quantity and product quantity into bundles
                        if (string.IsNullOrEmpty(attributesXml))
                        {
                            var cart = await GetShoppingCartAsync(customer, shoppingCartType, storeId);
                            var totalQuantityInCart = cart.Where(item => item.ProductId == product.Id && item.Id != shoppingCartItemId && string.IsNullOrEmpty(item.AttributesXml))
                                .Sum(product => product.Quantity);

                            totalQuantityInCart += quantity;

                            foreach (var bundle in cart.Where(x => x.Id != shoppingCartItemId && !string.IsNullOrEmpty(x.AttributesXml)))
                            {
                                var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(bundle.AttributesXml);
                                foreach (var attributeValue in attributeValues)
                                {
                                    if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct && attributeValue.AssociatedProductId == product.Id)
                                        totalQuantityInCart += bundle.Quantity * attributeValue.Quantity;
                                }
								_logger.LogError(">>> Entering GetShoppingCartAsync(customer={Customer},shoppingCartType=<redacted>,storeId={StoreId},productId={ProductId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},customWishlistId={CustomWishlistId})", customer, storeId, productId, createdFromUtc, createdToUtc, customWishlistId);
								var __logStart = System.Diagnostics.Stopwatch.StartNew();
								_logger.LogWarning("<<< Exiting GetShoppingCartAsync(customer={Customer},shoppingCartType=<redacted>,storeId={StoreId},productId={ProductId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},customWishlistId={CustomWishlistId})", customer, storeId, productId, createdFromUtc, createdToUtc, customWishlistId);
								_logger.LogError("GetShoppingCartAsync(customer,shoppingCartType,storeId,productId,createdFromUtc,createdToUtc,customWishlistId)={Customer},<redacted>,{StoreId},{ProductId},{CreatedFromUtc},{CreatedToUtc},{CustomWishlistId}: find query executing", customer, storeId, productId, createdFromUtc, createdToUtc, customWishlistId);
								_logger.LogError("GetShoppingCartAsync(customer,shoppingCartType,storeId,productId,createdFromUtc,createdToUtc,customWishlistId)={Customer},<redacted>,{StoreId},{ProductId},{CreatedFromUtc},{CreatedToUtc},{CustomWishlistId}: external service call took {Arg7} ms", customer, storeId, productId, createdFromUtc, createdToUtc, customWishlistId, __logStart.ElapsedMilliseconds);
                            }

                            warnings.AddRange(await GetQuantityProductWarningsAsync(product, totalQuantityInCart, maximumQuantityCanBeAdded));
                        }
                    }

                    break;
                case ManageInventoryMethod.ManageStockByAttributes:
                    var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
					_logger.LogError("GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId}): combination → {Combination}", customer, product, attributesXml, customerEnteredPrice, quantity, storeId, combination);
                    if (combination != null)
                    {
                        //combination exists
                        //let's check stock level
                        if (!combination.AllowOutOfStockOrders)
                            warnings.AddRange(await GetQuantityProductWarningsAsync(product, quantity, combination.StockQuantity));
                    }
                    else
                    {
                        //combination doesn't exist
                        if (product.AllowAddingOnlyExistingAttributeCombinations)
                        {
                            //maybe, is it better  to display something like "No such product/combination" message?
                            var productAvailabilityRange = await _dateRangeService.GetProductAvailabilityRangeByIdAsync(product.ProductAvailabilityRangeId);
							_logger.LogError("GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId}): productAvailabilityRange → {ProductAvailabilityRange}", customer, product, attributesXml, customerEnteredPrice, quantity, storeId, productAvailabilityRange);
                            var warning = productAvailabilityRange == null ? await _localizationService.GetResourceAsync("ShoppingCart.OutOfStock")
                                : string.Format(await _localizationService.GetResourceAsync("ShoppingCart.AvailabilityRange"),
                                    await _localizationService.GetLocalizedAsync(productAvailabilityRange, range => range.Name));
                            warnings.Add(warning);
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        //availability dates
        var availableStartDateError = false;
		_logger.LogError("GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId}): availableStartDateError → {AvailableStartDateError}", customer, product, attributesXml, customerEnteredPrice, quantity, storeId, availableStartDateError);
        if (product.AvailableStartDateTimeUtc.HasValue)
        {
            var availableStartDateTime = DateTime.SpecifyKind(product.AvailableStartDateTimeUtc.Value, DateTimeKind.Utc);
			_logger.LogError("GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId}): availableStartDateTime → {AvailableStartDateTime}", customer, product, attributesXml, customerEnteredPrice, quantity, storeId, availableStartDateTime);
            if (availableStartDateTime.CompareTo(DateTime.UtcNow) > 0)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.NotAvailable"));
                availableStartDateError = true;
            }
        }

        if (product.AgeVerification && product.MinimumAgeToPurchase > 0)
        {
            if (!customer.DateOfBirth.HasValue)
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.DateOfBirthRequired"));
            else if (CommonHelper.GetDifferenceInYears(customer.DateOfBirth.Value, DateTime.Today) < product.MinimumAgeToPurchase)
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MinimumAgeToPurchase"), product.MinimumAgeToPurchase));
        }

        if (!product.AvailableEndDateTimeUtc.HasValue || availableStartDateError)
            return warnings;

        var availableEndDateTime = DateTime.SpecifyKind(product.AvailableEndDateTimeUtc.Value, DateTimeKind.Utc);
		_logger.LogError("GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId}): availableEndDateTime → {AvailableEndDateTime}", customer, product, attributesXml, customerEnteredPrice, quantity, storeId, availableEndDateTime);
        if (availableEndDateTime.CompareTo(DateTime.UtcNow) < 0)
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.NotAvailable"));
        }

		_logger.LogWarning("<<< Exiting GetStandardWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},quantity={Quantity},shoppingCartItemId=<redacted>,storeId={StoreId})", customer, product, attributesXml, customerEnteredPrice, quantity, storeId);
        return warnings;
    }

    /// <summary>
    /// Validates the maximum quantity a product can be added 
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="quantity">Quantity</param>
    /// <param name="maximumQuantityCanBeAdded">The maximum quantity a product can be added</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings 
    /// </returns>
    protected virtual async Task<IList<string>> GetQuantityProductWarningsAsync(Product product, int quantity, int maximumQuantityCanBeAdded)
    {
		_logger.LogError(">>> Entering GetQuantityProductWarningsAsync(product={Product},quantity={Quantity},maximumQuantityCanBeAdded={MaximumQuantityCanBeAdded})", product, quantity, maximumQuantityCanBeAdded);
        ArgumentNullException.ThrowIfNull(product);

        var warnings = new List<string>();
		_logger.LogError("GetQuantityProductWarningsAsync(product={Product},quantity={Quantity},maximumQuantityCanBeAdded={MaximumQuantityCanBeAdded}): warnings → {Warnings}", product, quantity, maximumQuantityCanBeAdded, warnings);

        if (maximumQuantityCanBeAdded < quantity)
        {
            if (maximumQuantityCanBeAdded <= 0)
            {
                var productAvailabilityRange = await _dateRangeService.GetProductAvailabilityRangeByIdAsync(product.ProductAvailabilityRangeId);
				_logger.LogError("GetQuantityProductWarningsAsync(product={Product},quantity={Quantity},maximumQuantityCanBeAdded={MaximumQuantityCanBeAdded}): productAvailabilityRange → {ProductAvailabilityRange}", product, quantity, maximumQuantityCanBeAdded, productAvailabilityRange);
                var warning = productAvailabilityRange == null ? await _localizationService.GetResourceAsync("ShoppingCart.OutOfStock")
                    : string.Format(await _localizationService.GetResourceAsync("ShoppingCart.AvailabilityRange"),
                        await _localizationService.GetLocalizedAsync(productAvailabilityRange, range => range.Name));
                warnings.Add(warning);
            }
            else
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.QuantityExceedsStock"), maximumQuantityCanBeAdded));
        }

		_logger.LogWarning("<<< Exiting GetQuantityProductWarningsAsync(product={Product},quantity={Quantity},maximumQuantityCanBeAdded={MaximumQuantityCanBeAdded})", product, quantity, maximumQuantityCanBeAdded);
        return warnings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Delete shopping cart item
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
    /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteShoppingCartItemAsync(ShoppingCartItem shoppingCartItem, bool resetCheckoutData = true,
        bool ensureOnlyActiveCheckoutAttributes = false)
    {
		_logger.LogError(">>> Entering DeleteShoppingCartItemAsync(shoppingCartItem=<redacted>,resetCheckoutData={ResetCheckoutData},ensureOnlyActiveCheckoutAttributes={EnsureOnlyActiveCheckoutAttributes})", resetCheckoutData, ensureOnlyActiveCheckoutAttributes);
        ArgumentNullException.ThrowIfNull(shoppingCartItem);

        var customer = await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);
		_logger.LogError("DeleteShoppingCartItemAsync(shoppingCartItem=<redacted>,resetCheckoutData={ResetCheckoutData},ensureOnlyActiveCheckoutAttributes={EnsureOnlyActiveCheckoutAttributes}): customer → {Customer}", resetCheckoutData, ensureOnlyActiveCheckoutAttributes, customer);
        var storeId = shoppingCartItem.StoreId;

        //reset checkout data
        if (resetCheckoutData)
            await _customerService.ResetCheckoutDataAsync(customer, shoppingCartItem.StoreId);

        //delete item
        await _sciRepository.DeleteAsync(shoppingCartItem);

        //reset "HasShoppingCartItems" property used for performance optimization
        var hasShoppingCartItems = !await IsCustomerShoppingCartEmptyAsync(customer);
		_logger.LogError("DeleteShoppingCartItemAsync(shoppingCartItem=<redacted>,resetCheckoutData={ResetCheckoutData},ensureOnlyActiveCheckoutAttributes={EnsureOnlyActiveCheckoutAttributes}): hasShoppingCartItems → {HasShoppingCartItems}", resetCheckoutData, ensureOnlyActiveCheckoutAttributes, hasShoppingCartItems);
        if (hasShoppingCartItems != customer.HasShoppingCartItems)
        {
            customer.HasShoppingCartItems = hasShoppingCartItems;
			_logger.LogDebug("DeleteShoppingCartItemAsync(shoppingCartItem=<redacted>,resetCheckoutData={ResetCheckoutData},ensureOnlyActiveCheckoutAttributes={EnsureOnlyActiveCheckoutAttributes}): customer.HasShoppingCartItems → {Arg3}", resetCheckoutData, ensureOnlyActiveCheckoutAttributes, customer.HasShoppingCartItems);
            await _customerService.UpdateCustomerAsync(customer);
        }

        //validate checkout attributes
        if (ensureOnlyActiveCheckoutAttributes &&
            //only for shopping cart items (ignore wishlist)
            shoppingCartItem.ShoppingCartType == ShoppingCartType.ShoppingCart)
        {
            var cart = await GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, storeId);
			_logger.LogError("DeleteShoppingCartItemAsync(shoppingCartItem=<redacted>,resetCheckoutData={ResetCheckoutData},ensureOnlyActiveCheckoutAttributes={EnsureOnlyActiveCheckoutAttributes}): cart → {Cart}", resetCheckoutData, ensureOnlyActiveCheckoutAttributes, cart);

            var checkoutAttributesXml =
                await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CheckoutAttributes,
                    storeId);
            checkoutAttributesXml =
                await _checkoutAttributeParser.EnsureOnlyActiveAttributesAsync(checkoutAttributesXml, cart);
            await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.CheckoutAttributes,
                checkoutAttributesXml, storeId);
        }

        if (!_catalogSettings.RemoveRequiredProducts)
            return;

        var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);
		_logger.LogError("DeleteShoppingCartItemAsync(shoppingCartItem=<redacted>,resetCheckoutData={ResetCheckoutData},ensureOnlyActiveCheckoutAttributes={EnsureOnlyActiveCheckoutAttributes}): product → {Product}", resetCheckoutData, ensureOnlyActiveCheckoutAttributes, product);
        if (!product?.RequireOtherProducts ?? true)
            return;

        var requiredProductIds = _productService.ParseRequiredProductIds(product);
		_logger.LogError("DeleteShoppingCartItemAsync(shoppingCartItem=<redacted>,resetCheckoutData={ResetCheckoutData},ensureOnlyActiveCheckoutAttributes={EnsureOnlyActiveCheckoutAttributes}): requiredProductIds → {RequiredProductIds}", resetCheckoutData, ensureOnlyActiveCheckoutAttributes, requiredProductIds);
        var requiredShoppingCartItems =
            (await GetShoppingCartAsync(customer, shoppingCartType: shoppingCartItem.ShoppingCartType))
            .Where(item => requiredProductIds.Any(id => id == item.ProductId))
            .ToList();

        //update quantity of required products in the cart if the main one is removed
        foreach (var cartItem in requiredShoppingCartItems)
        {
            //at now we ignore quantities of required products and use 1
            var requiredProductQuantity = 1;

            await UpdateShoppingCartItemAsync(customer, cartItem.Id, cartItem.AttributesXml, cartItem.CustomerEnteredPrice,
                quantity: cartItem.Quantity - shoppingCartItem.Quantity * requiredProductQuantity,
                resetCheckoutData: false);
        }
		_logger.LogWarning("<<< Exiting DeleteShoppingCartItemAsync(shoppingCartItem=<redacted>,resetCheckoutData={ResetCheckoutData},ensureOnlyActiveCheckoutAttributes={EnsureOnlyActiveCheckoutAttributes})", resetCheckoutData, ensureOnlyActiveCheckoutAttributes);
    }

    /// <summary>
    /// Clear shopping cart
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="storeId">Store ID</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ClearShoppingCartAsync(Customer customer, int storeId)
    {
		_logger.LogError(">>> Entering ClearShoppingCartAsync(customer={Customer},storeId={StoreId})", customer, storeId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customer);

        var cart = await GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, storeId);
		_logger.LogError("ClearShoppingCartAsync(customer={Customer},storeId={StoreId}): cart → {Cart}", customer, storeId, cart);

        //delete items
		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("ClearShoppingCartAsync(customer,storeId)={Customer},{StoreId}: delete query executed in {Arg2} ms", customer, storeId, __logStartDb.ElapsedMilliseconds);
        await _sciRepository.DeleteAsync(cart, publishEvent: false);
        await _eventPublisher.PublishAsync(new ClearShoppingCartEvent(cart));

        //reset "HasShoppingCartItems" property used for performance optimization
        var hasShoppingCartItems = !await IsCustomerShoppingCartEmptyAsync(customer);
		_logger.LogError("ClearShoppingCartAsync(customer={Customer},storeId={StoreId}): hasShoppingCartItems → {HasShoppingCartItems}", customer, storeId, hasShoppingCartItems);
        if (hasShoppingCartItems != customer.HasShoppingCartItems)
        {
            customer.HasShoppingCartItems = hasShoppingCartItems;
			_logger.LogDebug("ClearShoppingCartAsync(customer={Customer},storeId={StoreId}): customer.HasShoppingCartItems → {Arg2}", customer, storeId, customer.HasShoppingCartItems);
			_logger.LogError("ClearShoppingCartAsync(customer,storeId)={Customer},{StoreId}: external service call took {Arg2} ms", customer, storeId, __logStart.ElapsedMilliseconds);
            await _customerService.UpdateCustomerAsync(customer);
			_logger.LogWarning("<<< Exiting ClearShoppingCartAsync(customer={Customer},storeId={StoreId})", customer, storeId);
        }
		__logStart = System.Diagnostics.Stopwatch.StartNew();
    }

    /// <summary>
    /// Delete shopping cart item
    /// </summary>
    /// <param name="shoppingCartItemId">Shopping cart item ID</param>
    /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
    /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteShoppingCartItemAsync(int shoppingCartItemId, bool resetCheckoutData = true,
        bool ensureOnlyActiveCheckoutAttributes = false)
    {
		_logger.LogError(">>> Entering DeleteShoppingCartItemAsync(shoppingCartItemId=<redacted>,resetCheckoutData={ResetCheckoutData},ensureOnlyActiveCheckoutAttributes={EnsureOnlyActiveCheckoutAttributes})", resetCheckoutData, ensureOnlyActiveCheckoutAttributes);
        var shoppingCartItem = await _sciRepository.Table.FirstOrDefaultAsync(sci => sci.Id == shoppingCartItemId);
			_logger.LogError("DeleteShoppingCartItemAsync(shoppingCartItemId=<redacted>,resetCheckoutData={ResetCheckoutData},ensureOnlyActiveCheckoutAttributes={EnsureOnlyActiveCheckoutAttributes}): shoppingCartItem → {ShoppingCartItem}", resetCheckoutData, ensureOnlyActiveCheckoutAttributes, shoppingCartItem);
			    _logger.LogWarning("<<< Exiting DeleteShoppingCartItemAsync(shoppingCartItemId=<redacted>,resetCheckoutData={ResetCheckoutData},ensureOnlyActiveCheckoutAttributes={EnsureOnlyActiveCheckoutAttributes})", resetCheckoutData, ensureOnlyActiveCheckoutAttributes);
        if (shoppingCartItem != null) {
            await DeleteShoppingCartItemAsync(shoppingCartItem, resetCheckoutData, ensureOnlyActiveCheckoutAttributes);
        }
    }

    /// <summary>
    /// Deletes expired shopping cart items
    /// </summary>
    /// <param name="olderThanUtc">Older than date and time</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of deleted items
    /// </returns>
    public virtual async Task<int> DeleteExpiredShoppingCartItemsAsync(DateTime olderThanUtc)
    {
		_logger.LogError(">>> Entering DeleteExpiredShoppingCartItemsAsync(olderThanUtc={OlderThanUtc})", olderThanUtc);
        var query = from sci in _sciRepository.Table
            where sci.UpdatedOnUtc < olderThanUtc
            select sci;

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var cartItems = await query.ToListAsync();

		_logger.LogError("DeleteExpiredShoppingCartItemsAsync(olderThanUtc)={OlderThanUtc}: database query executed in {Arg1} ms", olderThanUtc, __logStart.ElapsedMilliseconds);
		_logger.LogError("DeleteExpiredShoppingCartItemsAsync(olderThanUtc={OlderThanUtc}): cartItems → {CartItems}", olderThanUtc, cartItems);
        foreach (var cartItem in cartItems)
            await DeleteShoppingCartItemAsync(cartItem);

		_logger.LogWarning("<<< Exiting DeleteExpiredShoppingCartItemsAsync(olderThanUtc={OlderThanUtc})", olderThanUtc);
        return cartItems.Count;
    }

    /// <summary>
    /// Get products from shopping cart whether requiring specific product
    /// </summary>
    /// <param name="cart">Shopping cart </param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<IList<Product>> GetProductsRequiringProductAsync(IList<ShoppingCartItem> cart, Product product)
    {
		_logger.LogError(">>> Entering GetProductsRequiringProductAsync(cart={Cart},product={Product})", cart, product);
        ArgumentNullException.ThrowIfNull(cart);

        ArgumentNullException.ThrowIfNull(product);

        if (!cart.Any())
            return new List<Product>();

        var productIds = cart.Select(ci => ci.ProductId).ToArray();
		_logger.LogError("GetProductsRequiringProductAsync(cart={Cart},product={Product}): productIds → {ProductIds}", cart, product, productIds);

        var cartProducts = await _productService.GetProductsByIdsAsync(productIds);
		_logger.LogError("GetProductsRequiringProductAsync(cart={Cart},product={Product}): cartProducts → {CartProducts}", cart, product, cartProducts);

		_logger.LogWarning("<<< Exiting GetProductsRequiringProductAsync(cart={Cart},product={Product})", cart, product);
        return cartProducts.Where(cartProduct =>
            cartProduct.RequireOtherProducts &&
            _productService.ParseRequiredProductIds(cartProduct).Contains(product.Id)).ToList();
    }

    /// <summary>
    /// Gets shopping cart
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="shoppingCartType">Shopping cart type; pass null to load all records</param>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="productId">Product identifier; pass null to load all records</param>
    /// <param name="createdFromUtc">Created date from (UTC); pass null to load all records</param>
    /// <param name="createdToUtc">Created date to (UTC); pass null to load all records</param>
    /// <param name="customWishlistId">Custom wishlist identifier; pass 0 to load all records from all wishlists, pass null to load records from the default wishlist</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping Cart
    /// </returns>
    public virtual async Task<IList<ShoppingCartItem>> GetShoppingCartAsync(Customer customer, ShoppingCartType? shoppingCartType = null,
        int storeId = 0, int? productId = null, DateTime? createdFromUtc = null, DateTime? createdToUtc = null, int? customWishlistId = null)
    {
		_logger.LogError(">>> Entering GetShoppingCartAsync(customer={Customer},shoppingCartType=<redacted>,storeId={StoreId},productId={ProductId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},customWishlistId={CustomWishlistId})", customer, storeId, productId, createdFromUtc, createdToUtc, customWishlistId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customer);

        var items = _sciRepository.Table.Where(sci => sci.CustomerId == customer.Id);
		_logger.LogError("GetShoppingCartAsync(customer={Customer},shoppingCartType=<redacted>,storeId={StoreId},productId={ProductId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},customWishlistId={CustomWishlistId}): items → {Items}", customer, storeId, productId, createdFromUtc, createdToUtc, customWishlistId, items);

        //filter by type
        if (shoppingCartType.HasValue)
            items = items.Where(item => item.ShoppingCartTypeId == (int)shoppingCartType.Value);

        //filter by custom wishlist
        if ((!shoppingCartType.HasValue || shoppingCartType == ShoppingCartType.Wishlist) && (customWishlistId is null || customWishlistId > 0))
            items = items.Where(item => item.CustomWishlistId == customWishlistId);

        //filter shopping cart items by store
        if (storeId > 0 && !_shoppingCartSettings.CartsSharedBetweenStores)
            items = items.Where(item => item.StoreId == storeId);

        //filter shopping cart items by product
        if (productId > 0)
            items = items.Where(item => item.ProductId == productId);

        //filter shopping cart items by date
        if (createdFromUtc.HasValue)
            items = items.Where(item => createdFromUtc.Value <= item.CreatedOnUtc);
        if (createdToUtc.HasValue)
            items = items.Where(item => createdToUtc.Value >= item.CreatedOnUtc);

		_logger.LogError("GetShoppingCartAsync(customer,shoppingCartType,storeId,productId,createdFromUtc,createdToUtc,customWishlistId)={Customer},<redacted>,{StoreId},{ProductId},{CreatedFromUtc},{CreatedToUtc},{CustomWishlistId}: find query executing", customer, storeId, productId, createdFromUtc, createdToUtc, customWishlistId);
		_logger.LogError("GetShoppingCartAsync(customer,shoppingCartType,storeId,productId,createdFromUtc,createdToUtc,customWishlistId)={Customer},<redacted>,{StoreId},{ProductId},{CreatedFromUtc},{CreatedToUtc},{CustomWishlistId}: external service call took {Arg7} ms", customer, storeId, productId, createdFromUtc, createdToUtc, customWishlistId, __logStart.ElapsedMilliseconds);
        return await _shortTermCacheManager.GetAsync(async () => await items.ToListAsync(), NopOrderDefaults.ShoppingCartItemsAllCacheKey, customer, shoppingCartType, storeId, productId, createdFromUtc, createdToUtc, customWishlistId);
    }

    /// <summary>
    /// Validates shopping cart item attributes
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="shoppingCartType">Shopping cart type</param>
    /// <param name="product">Product</param>
    /// <param name="quantity">Quantity</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
    /// <param name="ignoreConditionMet">A value indicating whether we should ignore filtering by "is condition met" property</param>
    /// <param name="ignoreBundledProducts">A value indicating whether we should ignore bundled (associated) products</param>
    /// <param name="shoppingCartItemId">Shopping cart identifier; pass 0 if it's a new item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public virtual async Task<IList<string>> GetShoppingCartItemAttributeWarningsAsync(Customer customer,
        ShoppingCartType shoppingCartType,
        Product product,
        int quantity = 1,
        string attributesXml = "",
        bool ignoreNonCombinableAttributes = false,
        bool ignoreConditionMet = false,
        bool ignoreBundledProducts = false,
        int shoppingCartItemId = 0)
    {
		_logger.LogError(">>> Entering GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>)", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(product);

        var warnings = new List<string>();
		_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): warnings → {Warnings}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, warnings);

        //ensure it's our attributes
        var attributes1 = await _productAttributeParser.ParseProductAttributeMappingsAsync(attributesXml);
		_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): attributes1 → {Attributes1}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, attributes1);
        if (ignoreNonCombinableAttributes)
        {
            attributes1 = attributes1.Where(x => !x.IsNonCombinable()).ToList();
        }

        foreach (var attribute in attributes1)
        {
            if (attribute.ProductId == 0)
            {
                warnings.Add("Attribute error");
                return warnings;
            }

            if (attribute.ProductId != product.Id)
            {
                warnings.Add("Attribute error");
            }
        }

        //validate required product attributes (whether they're chosen/selected/entered)
        var attributes2 = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
		_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): attributes2 → {Attributes2}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, attributes2);
        if (ignoreNonCombinableAttributes)
        {
            attributes2 = attributes2.Where(x => !x.IsNonCombinable()).ToList();
        }

        //validate conditional attributes only (if specified)
        if (!ignoreConditionMet)
        {
            attributes2 = await attributes2.WhereAwait(async x =>
            {
                var conditionMet = await _productAttributeParser.IsConditionMetAsync(x, attributesXml);
				_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): conditionMet → {ConditionMet}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, conditionMet);
			_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer,shoppingCartType,product,quantity,attributesXml,ignoreNonCombinableAttributes,ignoreConditionMet,ignoreBundledProducts,shoppingCartItemId)={Customer},<redacted>,{Product},{Quantity},{AttributesXml},{IgnoreNonCombinableAttributes},{IgnoreConditionMet},{IgnoreBundledProducts},<redacted>: database query executed in {Arg9} ms", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, __logStart.ElapsedMilliseconds);
                return !conditionMet.HasValue || conditionMet.Value;
            }).ToListAsync();
        }

        foreach (var a2 in attributes2)
        {
            var productAttributeValues = await _productAttributeService.GetProductAttributeValuesAsync(a2.Id);
			_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): productAttributeValues → {ProductAttributeValues}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, productAttributeValues);

            if (a2.IsRequired)
            {
                var found = false;
				_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): found → {Found}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, found);
                //selected product attributes
                foreach (var a1 in attributes1)
                {
                    if (a1.Id != a2.Id)
                        continue;

                    var attributeValuesStr = _productAttributeParser.ParseValues(attributesXml, a1.Id);
					_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): attributeValuesStr → {AttributeValuesStr}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, attributeValuesStr);

                    if (a2.ShouldHaveValues() && productAttributeValues.Any() && !productAttributeValues.Any(x => attributeValuesStr.Contains(x.Id.ToString())))
                        break;

                    foreach (var str1 in attributeValuesStr)
                    {
                        if (string.IsNullOrEmpty(str1.Trim()))
                            continue;

                        found = true;
                        break;
                    }
                }

                //if not found
                if (!found)
                {
                    var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(a2.ProductAttributeId);
					_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): productAttribute → {ProductAttribute}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, productAttribute);

                    var textPrompt = await _localizationService.GetLocalizedAsync(a2, x => x.TextPrompt);
					_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): textPrompt → {TextPrompt}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, textPrompt);
                    var notFoundWarning = !string.IsNullOrEmpty(textPrompt) ?
                        textPrompt :
                        string.Format(await _localizationService.GetResourceAsync("ShoppingCart.SelectAttribute"), await _localizationService.GetLocalizedAsync(productAttribute, a => a.Name));

                    warnings.Add(notFoundWarning);
                }
            }

            if (a2.AttributeControlType != AttributeControlType.ReadonlyCheckboxes)
                continue;

            //customers cannot edit read-only attributes
            var allowedReadOnlyValueIds = productAttributeValues
                .Where(x => x.IsPreSelected)
                .Select(x => x.Id)
                .ToArray();

            var selectedReadOnlyValueIds = (await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml))
                .Where(x => x.ProductAttributeMappingId == a2.Id)
                .Select(x => x.Id)
                .ToArray();

            if (!CommonHelper.ArraysEqual(allowedReadOnlyValueIds, selectedReadOnlyValueIds))
            {
                warnings.Add("You cannot change read-only values");
            }
        }

        //validation rules
        foreach (var pam in attributes2)
        {
            if (!pam.ValidationRulesAllowed())
                continue;

            string enteredText;
            int enteredTextLength;

            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId);

            //minimum length
            if (pam.ValidationMinLength.HasValue)
            {
                if (pam.AttributeControlType == AttributeControlType.TextBox ||
                    pam.AttributeControlType == AttributeControlType.MultilineTextbox)
                {
                    enteredText = _productAttributeParser.ParseValues(attributesXml, pam.Id).FirstOrDefault();
					_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): enteredText → {EnteredText}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, enteredText);
					_logger.LogDebug("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): enteredText → {EnteredText}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, enteredText);
                    enteredTextLength = string.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;
					_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): enteredTextLength → {EnteredTextLength}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, enteredTextLength);

                    if (pam.ValidationMinLength.Value > enteredTextLength)
                    {
						_logger.LogDebug("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): enteredTextLength → {EnteredTextLength}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, enteredTextLength);
                        warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.TextboxMinimumLength"), await _localizationService.GetLocalizedAsync(productAttribute, a => a.Name), pam.ValidationMinLength.Value));
                    }
                }
            }

            //maximum length
            if (!pam.ValidationMaxLength.HasValue)
                continue;

            if (pam.AttributeControlType != AttributeControlType.TextBox && pam.AttributeControlType != AttributeControlType.MultilineTextbox)
                continue;

            enteredText = _productAttributeParser.ParseValues(attributesXml, pam.Id).FirstOrDefault();
            enteredTextLength = string.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

            if (pam.ValidationMaxLength.Value < enteredTextLength)
            {
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.TextboxMaximumLength"), await _localizationService.GetLocalizedAsync(productAttribute, a => a.Name), pam.ValidationMaxLength.Value));
            }
        }

        if (warnings.Any() || ignoreBundledProducts)
            return warnings;

        //validate bundled products
        var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
		_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): attributeValues → {AttributeValues}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, attributeValues);
        foreach (var attributeValue in attributeValues)
        {
            if (attributeValue.AttributeValueType != AttributeValueType.AssociatedToProduct)
                continue;

            var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(attributeValue.ProductAttributeMappingId);
			_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): productAttributeMapping → {ProductAttributeMapping}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, productAttributeMapping);

            if (productAttributeMapping == null)
                continue;

            if (ignoreNonCombinableAttributes && productAttributeMapping.IsNonCombinable())
                continue;

            //associated product (bundle)
            var associatedProduct = await _productService.GetProductByIdAsync(attributeValue.AssociatedProductId);
			_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): associatedProduct → {AssociatedProduct}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, associatedProduct);

            if (associatedProduct != null)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
				_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): store → {Store}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, store);
                var totalQty = quantity * attributeValue.Quantity;
                var associatedProductWarnings = await GetShoppingCartItemWarningsAsync(customer,
                    shoppingCartType, associatedProduct, store.Id,
                    string.Empty, decimal.Zero, null, null, totalQty, false, shoppingCartItemId);

                var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(productAttributeMapping.ProductAttributeId);

                foreach (var associatedProductWarning in associatedProductWarnings)
                {
                    var attributeName = await _localizationService.GetLocalizedAsync(productAttribute, a => a.Name);
					_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): attributeName → {AttributeName}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, attributeName);
                    var attributeValueName = await _localizationService.GetLocalizedAsync(attributeValue, a => a.Name);
					_logger.LogError("GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>): attributeValueName → {AttributeValueName}", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts, attributeValueName);
                    warnings.Add(string.Format(
                        await _localizationService.GetResourceAsync("ShoppingCart.AssociatedAttributeWarning"),
                        attributeName, attributeValueName, associatedProductWarning));
                }
            }
            else
                warnings.Add($"Associated product cannot be loaded - {attributeValue.AssociatedProductId}");
        }

		_logger.LogWarning("<<< Exiting GetShoppingCartItemAttributeWarningsAsync(customer={Customer},shoppingCartType=<redacted>,product={Product},quantity={Quantity},attributesXml={AttributesXml},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreConditionMet={IgnoreConditionMet},ignoreBundledProducts={IgnoreBundledProducts},shoppingCartItemId=<redacted>)", customer, product, quantity, attributesXml, ignoreNonCombinableAttributes, ignoreConditionMet, ignoreBundledProducts);
        return warnings;
    }

    /// <summary>
    /// Validates shopping cart item (gift card)
    /// </summary>
    /// <param name="shoppingCartType">Shopping cart type</param>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public virtual async Task<IList<string>> GetShoppingCartItemGiftCardWarningsAsync(ShoppingCartType shoppingCartType,
        Product product, string attributesXml)
    {
		_logger.LogError(">>> Entering GetShoppingCartItemGiftCardWarningsAsync(shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml})", product, attributesXml);
        ArgumentNullException.ThrowIfNull(product);

        var warnings = new List<string>();
		_logger.LogError("GetShoppingCartItemGiftCardWarningsAsync(shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml}): warnings → {Warnings}", product, attributesXml, warnings);

        //gift cards
        if (!product.IsGiftCard)
            return warnings;

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("GetShoppingCartItemGiftCardWarningsAsync(shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml}): customer → {Customer}", product, attributesXml, customer);
        var giftCards = await _giftCardService.GetActiveGiftCardsAppliedByCustomerAsync(customer);
		_logger.LogError("GetShoppingCartItemGiftCardWarningsAsync(shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml}): giftCards → {GiftCards}", product, attributesXml, giftCards);
        if (giftCards.Any())
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.GiftCardCouponCode.DontWorkWithGiftCards"));

        _productAttributeParser.GetGiftCardAttribute(attributesXml, out var giftCardRecipientName, out var giftCardRecipientEmail, out var giftCardSenderName, out var giftCardSenderEmail, out var _);

        if (string.IsNullOrEmpty(giftCardRecipientName))
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.RecipientNameError"));

        if (product.GiftCardType == GiftCardType.Virtual)
        {
            //validate for virtual gift cards only
            if (string.IsNullOrEmpty(giftCardRecipientEmail) || !CommonHelper.IsValidEmail(giftCardRecipientEmail))
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.RecipientEmailError"));
        }

        if (string.IsNullOrEmpty(giftCardSenderName))
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.SenderNameError"));

        if (product.GiftCardType != GiftCardType.Virtual)
            return warnings;

        //validate for virtual gift cards only
        if (string.IsNullOrEmpty(giftCardSenderEmail) || !CommonHelper.IsValidEmail(giftCardSenderEmail))
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.SenderEmailError"));

		_logger.LogWarning("<<< Exiting GetShoppingCartItemGiftCardWarningsAsync(shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml})", product, attributesXml);
        return warnings;
    }

    /// <summary>
    /// Validates shopping cart item for rental products
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="rentalStartDate">Rental start date</param>
    /// <param name="rentalEndDate">Rental end date</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public virtual async Task<IList<string>> GetRentalProductWarningsAsync(Product product,
        DateTime? rentalStartDate = null, DateTime? rentalEndDate = null)
    {
		_logger.LogError(">>> Entering GetRentalProductWarningsAsync(product={Product},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate})", product, rentalStartDate, rentalEndDate);
        ArgumentNullException.ThrowIfNull(product);

        var warnings = new List<string>();
		_logger.LogError("GetRentalProductWarningsAsync(product={Product},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate}): warnings → {Warnings}", product, rentalStartDate, rentalEndDate, warnings);

        if (!product.IsRental)
            return warnings;

        if (!rentalStartDate.HasValue)
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.Rental.EnterStartDate"));
            return warnings;
        }

        if (!rentalEndDate.HasValue)
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.Rental.EnterEndDate"));
            return warnings;
        }

        if (rentalStartDate.Value.CompareTo(rentalEndDate.Value) > 0)
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.Rental.StartDateLessEndDate"));
            return warnings;
        }

        //allowed start date should be the future date
        //we should compare rental start date with a store local time
        //but we what if a store works in distinct timezones? how we should handle it? skip it for now
        //we also ignore hours (anyway not supported yet)
        //today
        var nowDtInStoreTimeZone = _dateTimeHelper.ConvertToUserTime(DateTime.Now, TimeZoneInfo.Local, _dateTimeHelper.DefaultStoreTimeZone);
		_logger.LogError("GetRentalProductWarningsAsync(product={Product},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate}): nowDtInStoreTimeZone → {NowDtInStoreTimeZone}", product, rentalStartDate, rentalEndDate, nowDtInStoreTimeZone);
        var todayDt = new DateTime(nowDtInStoreTimeZone.Year, nowDtInStoreTimeZone.Month, nowDtInStoreTimeZone.Day);
		_logger.LogError("GetRentalProductWarningsAsync(product={Product},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate}): todayDt → {TodayDt}", product, rentalStartDate, rentalEndDate, todayDt);
        var todayDtUtc = _dateTimeHelper.ConvertToUtcTime(todayDt, _dateTimeHelper.DefaultStoreTimeZone);
		_logger.LogError("GetRentalProductWarningsAsync(product={Product},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate}): todayDtUtc → {TodayDtUtc}", product, rentalStartDate, rentalEndDate, todayDtUtc);
        //dates are entered in store timezone (e.g. like in hotels)
        var startDateUtc = _dateTimeHelper.ConvertToUtcTime(rentalStartDate.Value, _dateTimeHelper.DefaultStoreTimeZone);
		_logger.LogError("GetRentalProductWarningsAsync(product={Product},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate}): startDateUtc → {StartDateUtc}", product, rentalStartDate, rentalEndDate, startDateUtc);
        //but we what if dates should be entered in a customer timezone?
        //DateTime startDateUtc = _dateTimeHelper.ConvertToUtcTime(rentalStartDate.Value, _dateTimeHelper.CurrentTimeZone);
        if (todayDtUtc.CompareTo(startDateUtc) <= 0)
            return warnings;

        warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.Rental.StartDateShouldBeFuture"));
        return warnings;
    }

    /// <summary>
    /// Validates shopping cart item
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="shoppingCartType">Shopping cart type</param>
    /// <param name="product">Product</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="customerEnteredPrice">Customer entered price</param>
    /// <param name="rentalStartDate">Rental start date</param>
    /// <param name="rentalEndDate">Rental end date</param>
    /// <param name="quantity">Quantity</param>
    /// <param name="addRequiredProducts">Whether to add required products</param>
    /// <param name="shoppingCartItemId">Shopping cart identifier; pass 0 if it's a new item</param>
    /// <param name="getStandardWarnings">A value indicating whether we should validate a product for standard properties</param>
    /// <param name="getAttributesWarnings">A value indicating whether we should validate product attributes</param>
    /// <param name="getGiftCardWarnings">A value indicating whether we should validate gift card properties</param>
    /// <param name="getRequiredProductWarnings">A value indicating whether we should validate required products (products which require other products to be added to the cart)</param>
    /// <param name="getRentalWarnings">A value indicating whether we should validate rental properties</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public virtual async Task<IList<string>> GetShoppingCartItemWarningsAsync(Customer customer, ShoppingCartType shoppingCartType,
        Product product, int storeId,
        string attributesXml, decimal customerEnteredPrice,
        DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
        int quantity = 1, bool addRequiredProducts = true, int shoppingCartItemId = 0,
        bool getStandardWarnings = true, bool getAttributesWarnings = true,
        bool getGiftCardWarnings = true, bool getRequiredProductWarnings = true,
        bool getRentalWarnings = true)
    {
        ArgumentNullException.ThrowIfNull(product);

        var warnings = new List<string>();

        //standard properties
        if (getStandardWarnings)
            warnings.AddRange(await GetStandardWarningsAsync(customer, shoppingCartType, product, attributesXml, customerEnteredPrice, quantity, shoppingCartItemId, storeId));

        //selected attributes
        if (getAttributesWarnings)
            warnings.AddRange(await GetShoppingCartItemAttributeWarningsAsync(customer, shoppingCartType, product, quantity, attributesXml, false, false, false, shoppingCartItemId));

        //gift cards
        if (getGiftCardWarnings)
            warnings.AddRange(await GetShoppingCartItemGiftCardWarningsAsync(shoppingCartType, product, attributesXml));

        //required products
        if (getRequiredProductWarnings)
            warnings.AddRange(await GetRequiredProductWarningsAsync(customer, shoppingCartType, product, storeId, quantity, addRequiredProducts, shoppingCartItemId));

        //rental products
        if (getRentalWarnings)
            warnings.AddRange(await GetRentalProductWarningsAsync(product, rentalStartDate, rentalEndDate));

        return warnings;
    }

    /// <summary>
    /// Validates whether this shopping cart is valid
    /// </summary>
    /// <param name="shoppingCart">Shopping cart</param>
    /// <param name="checkoutAttributesXml">Checkout attributes in XML format</param>
    /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public virtual async Task<IList<string>> GetShoppingCartWarningsAsync(IList<ShoppingCartItem> shoppingCart,
        string checkoutAttributesXml, bool validateCheckoutAttributes)
    {
        var warnings = new List<string>();

        if (shoppingCart.Count > _shoppingCartSettings.MaximumShoppingCartItems)
            warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumShoppingCartItems"), _shoppingCartSettings.MaximumShoppingCartItems));

        var hasStandardProducts = false;
        var hasRecurringProducts = false;

        foreach (var sci in shoppingCart)
        {
            var product = await _productService.GetProductByIdAsync(sci.ProductId);
            if (product == null)
            {
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.CannotLoadProduct"), sci.ProductId));
                return warnings;
            }

            if (product.IsRecurring)
                hasRecurringProducts = true;
            else
                hasStandardProducts = true;
        }

        //don't mix standard and recurring products
        if (hasStandardProducts && hasRecurringProducts)
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.CannotMixStandardAndAutoshipProducts"));

        //recurring cart validation
        if (hasRecurringProducts)
        {
            var cyclesError = (await GetRecurringCycleInfoAsync(shoppingCart)).error;
            if (!string.IsNullOrEmpty(cyclesError))
            {
                warnings.Add(cyclesError);
                return warnings;
            }
        }

        //validate checkout attributes
        if (!validateCheckoutAttributes)
            return warnings;

        //selected attributes
        var attributes1 = await _checkoutAttributeParser.ParseAttributesAsync(checkoutAttributesXml);

        //existing checkout attributes
        var excludeShippableAttributes = !await ShoppingCartRequiresShippingAsync(shoppingCart);
        var store = await _storeContext.GetCurrentStoreAsync();
        var attributes2 = await _checkoutAttributeService.GetAllAttributesAsync(_staticCacheManager, _storeMappingService, store.Id, excludeShippableAttributes);

        //validate conditional attributes only (if specified)
        attributes2 = await attributes2.WhereAwait(async x =>
        {
            var conditionMet = await _checkoutAttributeParser.IsConditionMetAsync(x.ConditionAttributeXml, checkoutAttributesXml);
            return !conditionMet.HasValue || conditionMet.Value;
        }).ToListAsync();

        foreach (var a2 in attributes2)
        {
            if (!a2.IsRequired)
                continue;

            var found = false;
            //selected checkout attributes
            foreach (var a1 in attributes1)
            {
                if (a1.Id != a2.Id)
                    continue;

                var attributeValuesStr = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, a1.Id);
                foreach (var str1 in attributeValuesStr)
                    if (!string.IsNullOrEmpty(str1.Trim()))
                    {
                        found = true;
                        break;
                    }
            }

            if (found)
                continue;

            //if not found
            warnings.Add(!string.IsNullOrEmpty(await _localizationService.GetLocalizedAsync(a2, a => a.TextPrompt))
                ? await _localizationService.GetLocalizedAsync(a2, a => a.TextPrompt)
                : string.Format(await _localizationService.GetResourceAsync("ShoppingCart.SelectAttribute"),
                    await _localizationService.GetLocalizedAsync(a2, a => a.Name)));
        }

        //now validation rules

        //minimum length
        foreach (var ca in attributes2)
        {
            string enteredText;
            int enteredTextLength;

            if (ca.ValidationMinLength.HasValue)
            {
                if (ca.AttributeControlType == AttributeControlType.TextBox ||
                    ca.AttributeControlType == AttributeControlType.MultilineTextbox)
                {
                    enteredText = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, ca.Id).FirstOrDefault();
                    enteredTextLength = string.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                    if (ca.ValidationMinLength.Value > enteredTextLength)
                    {
                        warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.TextboxMinimumLength"), await _localizationService.GetLocalizedAsync(ca, a => a.Name), ca.ValidationMinLength.Value));
                    }
                }
            }

            //maximum length
            if (!ca.ValidationMaxLength.HasValue)
                continue;

            if (ca.AttributeControlType != AttributeControlType.TextBox && ca.AttributeControlType != AttributeControlType.MultilineTextbox)
                continue;

            enteredText = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, ca.Id).FirstOrDefault();
            enteredTextLength = string.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

            if (ca.ValidationMaxLength.Value < enteredTextLength)
            {
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.TextboxMaximumLength"), await _localizationService.GetLocalizedAsync(ca, a => a.Name), ca.ValidationMaxLength.Value));
            }
        }

        return warnings;
    }

    /// <summary>
    /// Gets the shopping cart item sub total
    /// </summary>
    /// <param name="shoppingCartItem">The shopping cart item</param>
    /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
    /// <returns>Shopping cart item sub total. Applied discount amount. Applied discounts. Maximum discounted qty. Return not nullable value if discount cannot be applied to ALL items</returns>
    public virtual async Task<(decimal subTotal, decimal discountAmount, List<Discount> appliedDiscounts, int? maximumDiscountQty)> GetSubTotalAsync(ShoppingCartItem shoppingCartItem,
        bool includeDiscounts)
    {
        ArgumentNullException.ThrowIfNull(shoppingCartItem);

        decimal subTotal;
        int? maximumDiscountQty = null;

        //unit price
        var (unitPrice, discountAmount, appliedDiscounts) = await GetUnitPriceAsync(shoppingCartItem, includeDiscounts);

        //discount
        if (appliedDiscounts.Any())
        {
            //we can properly use "MaximumDiscountedQuantity" property only for one discount (not cumulative ones)
            Discount oneAndOnlyDiscount = null;
            if (appliedDiscounts.Count == 1)
                oneAndOnlyDiscount = appliedDiscounts.First();

            if ((oneAndOnlyDiscount?.MaximumDiscountedQuantity.HasValue ?? false) &&
                shoppingCartItem.Quantity > oneAndOnlyDiscount.MaximumDiscountedQuantity.Value)
            {
                maximumDiscountQty = oneAndOnlyDiscount.MaximumDiscountedQuantity.Value;
                //we cannot apply discount for all shopping cart items
                var discountedQuantity = oneAndOnlyDiscount.MaximumDiscountedQuantity.Value;
                var discountedSubTotal = unitPrice * discountedQuantity;
                discountAmount *= discountedQuantity;

                var notDiscountedQuantity = shoppingCartItem.Quantity - discountedQuantity;
                var notDiscountedUnitPrice = (await GetUnitPriceAsync(shoppingCartItem, false)).unitPrice;
                var notDiscountedSubTotal = notDiscountedUnitPrice * notDiscountedQuantity;

                subTotal = discountedSubTotal + notDiscountedSubTotal;
            }
            else
            {
                //discount is applied to all items (quantity)
                //calculate discount amount for all items
                discountAmount *= shoppingCartItem.Quantity;

                subTotal = unitPrice * shoppingCartItem.Quantity;
            }
        }
        else
        {
            subTotal = unitPrice * shoppingCartItem.Quantity;
        }

        return (subTotal, discountAmount, appliedDiscounts, maximumDiscountQty);
    }

    /// <summary>
    /// Gets the shopping cart unit price (one item)
    /// </summary>
    /// <param name="shoppingCartItem">The shopping cart item</param>
    /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping cart unit price (one item). Applied discount amount. Applied discounts
    /// </returns>
    public virtual async Task<(decimal unitPrice, decimal discountAmount, List<Discount> appliedDiscounts)> GetUnitPriceAsync(ShoppingCartItem shoppingCartItem,
        bool includeDiscounts)
    {
        ArgumentNullException.ThrowIfNull(shoppingCartItem);

        //allow third-party handlers to select the unit price
        var unitPriceEvent = new GetShoppingCartItemUnitPriceEvent(shoppingCartItem, includeDiscounts);
        await _eventPublisher.PublishAsync(unitPriceEvent);
        if (unitPriceEvent.StopProcessing)
            return (unitPriceEvent.UnitPrice, unitPriceEvent.DiscountAmount, unitPriceEvent.AppliedDiscounts);

        var customer = await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);
        var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);
        var store = await _storeService.GetStoreByIdAsync(shoppingCartItem.StoreId);

        return await GetUnitPriceAsync(product,
            customer,
            store,
            shoppingCartItem.ShoppingCartType,
            shoppingCartItem.Quantity,
            shoppingCartItem.AttributesXml,
            shoppingCartItem.CustomerEnteredPrice,
            shoppingCartItem.RentalStartDateUtc,
            shoppingCartItem.RentalEndDateUtc,
            includeDiscounts);
    }

    /// <summary>
    /// Gets the shopping cart unit price (one item)
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="customer">Customer</param>
    /// <param name="store">Store</param>
    /// <param name="shoppingCartType">Shopping cart type</param>
    /// <param name="quantity">Quantity</param>
    /// <param name="attributesXml">Product attributes (XML format)</param>
    /// <param name="customerEnteredPrice">Customer entered price (if specified)</param>
    /// <param name="rentalStartDate">Rental start date (null for not rental products)</param>
    /// <param name="rentalEndDate">Rental end date (null for not rental products)</param>
    /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping cart unit price (one item). Applied discount amount. Applied discounts
    /// </returns>
    public virtual async Task<(decimal unitPrice, decimal discountAmount, List<Discount> appliedDiscounts)> GetUnitPriceAsync(Product product,
        Customer customer,
        Store store,
        ShoppingCartType shoppingCartType,
        int quantity,
        string attributesXml,
        decimal customerEnteredPrice,
        DateTime? rentalStartDate, DateTime? rentalEndDate,
        bool includeDiscounts)
    {
        ArgumentNullException.ThrowIfNull(product);

        ArgumentNullException.ThrowIfNull(customer);

        var discountAmount = decimal.Zero;
        var appliedDiscounts = new List<Discount>();

        decimal finalPrice;

        var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
        if (combination?.OverriddenPrice.HasValue ?? false)
        {
            (_, finalPrice, discountAmount, appliedDiscounts) = await _priceCalculationService.GetFinalPriceAsync(product,
                customer,
                store,
                combination.OverriddenPrice.Value,
                decimal.Zero,
                includeDiscounts,
                quantity,
                product.IsRental ? rentalStartDate : null,
                product.IsRental ? rentalEndDate : null);
        }
        else
        {
            //summarize price of all attributes
            var attributesTotalPrice = decimal.Zero;
            var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
            if (attributeValues != null)
            {
                foreach (var attributeValue in attributeValues)
                {
                    attributesTotalPrice += await _priceCalculationService.GetProductAttributeValuePriceAdjustmentAsync(product,
                        attributeValue,
                        customer,
                        store,
                        product.CustomerEntersPrice ? (decimal?)customerEnteredPrice : null,
                        quantity);
                }
            }

            //get price of a product (with previously calculated price of all attributes)
            if (product.CustomerEntersPrice)
            {
                finalPrice = customerEnteredPrice;
            }
            else
            {
                int qty;
                if (_shoppingCartSettings.GroupTierPricesForDistinctShoppingCartItems)
                {
                    //the same products with distinct product attributes could be stored as distinct "ShoppingCartItem" records
                    //so let's find how many of the current products are in the cart                        
                    qty = (await GetShoppingCartAsync(customer, shoppingCartType: shoppingCartType, productId: product.Id))
                        .Sum(x => x.Quantity);

                    if (qty == 0)
                    {
                        qty = quantity;
                    }
                }
                else
                {
                    qty = quantity;
                }

                (_, finalPrice, discountAmount, appliedDiscounts) = await _priceCalculationService.GetFinalPriceAsync(product,
                    customer,
                    store,
                    attributesTotalPrice,
                    includeDiscounts,
                    qty,
                    product.IsRental ? rentalStartDate : null,
                    product.IsRental ? rentalEndDate : null);
            }
        }

        //rounding
        if (_shoppingCartSettings.RoundPricesDuringCalculation)
            finalPrice = await _priceCalculationService.RoundPriceAsync(finalPrice);

        return (finalPrice, discountAmount, appliedDiscounts);
    }

    /// <summary>
    /// Finds a shopping cart item in the cart
    /// </summary>
    /// <param name="shoppingCart">Shopping cart</param>
    /// <param name="shoppingCartType">Shopping cart type</param>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="customerEnteredPrice">Price entered by a customer</param>
    /// <param name="rentalStartDate">Rental start date</param>
    /// <param name="rentalEndDate">Rental end date</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the found shopping cart item
    /// </returns>
    public virtual async Task<ShoppingCartItem> FindShoppingCartItemInTheCartAsync(IList<ShoppingCartItem> shoppingCart,
        ShoppingCartType shoppingCartType,
        Product product,
        string attributesXml = "",
        decimal customerEnteredPrice = decimal.Zero,
        DateTime? rentalStartDate = null,
        DateTime? rentalEndDate = null)
    {
        ArgumentNullException.ThrowIfNull(shoppingCart);
        ArgumentNullException.ThrowIfNull(product);

			_logger.LogWarning("<<< Exiting FindShoppingCartItemInTheCartAsync(shoppingCart=<redacted>,shoppingCartType=<redacted>,product={Product},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate})", product, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate);
        return await shoppingCart.Where(sci => sci.ShoppingCartType == shoppingCartType)
            .FirstOrDefaultAwaitAsync(async sci => await ShoppingCartItemIsEqualAsync(sci, product, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate));
    }

    /// <summary>
    /// Add a product to shopping cart
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="product">Product</param>
    /// <param name="shoppingCartType">Shopping cart type</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="customerEnteredPrice">The price enter by a customer</param>
    /// <param name="rentalStartDate">Rental start date</param>
    /// <param name="rentalEndDate">Rental end date</param>
    /// <param name="quantity">Quantity</param>
    /// <param name="addRequiredProducts">Whether to add required products</param>
    /// <param name="wishlistId">Wishlist identifier; pass null if it's default wishlist</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public virtual async Task<IList<string>> AddToCartAsync(Customer customer, Product product,
        ShoppingCartType shoppingCartType, int storeId, string attributesXml = null,
        decimal customerEnteredPrice = decimal.Zero,
        DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
        int quantity = 1, bool addRequiredProducts = true, int? wishlistId = null)
    {
		_logger.LogError(">>> Entering AddToCartAsync(customer={Customer},product={Product},shoppingCartType=<redacted>,storeId={StoreId},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},addRequiredProducts={AddRequiredProducts},wishlistId={WishlistId})", customer, product, storeId, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, addRequiredProducts, wishlistId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(customer);

        ArgumentNullException.ThrowIfNull(product);

        var warnings = new List<string>();
		_logger.LogError("AddToCartAsync(customer={Customer},product={Product},shoppingCartType=<redacted>,storeId={StoreId},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},addRequiredProducts={AddRequiredProducts},wishlistId={WishlistId}): warnings → {Warnings}", customer, product, storeId, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, addRequiredProducts, wishlistId, warnings);
        if (shoppingCartType == ShoppingCartType.ShoppingCart && !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART, customer))
        {
            warnings.Add("Shopping cart is disabled");
            return warnings;
        }

        if (shoppingCartType == ShoppingCartType.Wishlist && !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_WISHLIST, customer))
        {
            warnings.Add("Wishlist is disabled");
            return warnings;
        }

        if (customer.IsSearchEngineAccount())
        {
            warnings.Add("Search engine can't add to cart");
            return warnings;
        }

        if (quantity <= 0)
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.QuantityShouldPositive"));
            return warnings;
        }

        //reset checkout info
        await _customerService.ResetCheckoutDataAsync(customer, storeId);

        var cart = await GetShoppingCartAsync(customer, shoppingCartType, storeId);
		_logger.LogError("AddToCartAsync(customer={Customer},product={Product},shoppingCartType=<redacted>,storeId={StoreId},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},addRequiredProducts={AddRequiredProducts},wishlistId={WishlistId}): cart → {Cart}", customer, product, storeId, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, addRequiredProducts, wishlistId, cart);

        var shoppingCartItem = await FindShoppingCartItemInTheCartAsync(cart,
            shoppingCartType, product, attributesXml, customerEnteredPrice,
            rentalStartDate, rentalEndDate);

        if (shoppingCartItem != null)
        {
            //update existing shopping cart item
            var newQuantity = shoppingCartItem.Quantity + quantity;

            await addRequiredProductsToCartAsync(newQuantity, wishlistId);

            if (warnings.Any())
                return warnings;

            warnings.AddRange(await GetShoppingCartItemWarningsAsync(customer, shoppingCartType, product,
                storeId, attributesXml,
                customerEnteredPrice, rentalStartDate, rentalEndDate,
                newQuantity, addRequiredProducts, shoppingCartItem.Id));

            if (warnings.Any())
                return warnings;

            shoppingCartItem.AttributesXml = attributesXml;
			_logger.LogDebug("AddToCartAsync(customer={Customer},product={Product},shoppingCartType=<redacted>,storeId={StoreId},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},addRequiredProducts={AddRequiredProducts},wishlistId={WishlistId}): shoppingCartItem.AttributesXml → {Arg11}", customer, product, storeId, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, addRequiredProducts, wishlistId, shoppingCartItem.AttributesXml);
			_logger.LogDebug("AddToCartAsync(customer={Customer},product={Product},shoppingCartType=<redacted>,storeId={StoreId},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},addRequiredProducts={AddRequiredProducts},wishlistId={WishlistId}): shoppingCartItem.UpdatedOnUtc → {Arg11}", customer, product, storeId, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, addRequiredProducts, wishlistId, shoppingCartItem.UpdatedOnUtc);
            shoppingCartItem.Quantity = newQuantity;
			_logger.LogDebug("AddToCartAsync(customer={Customer},product={Product},shoppingCartType=<redacted>,storeId={StoreId},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},addRequiredProducts={AddRequiredProducts},wishlistId={WishlistId}): shoppingCartItem.Quantity → {Arg11}", customer, product, storeId, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, addRequiredProducts, wishlistId, shoppingCartItem.Quantity);
            shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;

            await _sciRepository.UpdateAsync(shoppingCartItem);
			_logger.LogError("AddToCartAsync(customer,product,shoppingCartType,storeId,attributesXml,customerEnteredPrice,rentalStartDate,rentalEndDate,quantity,addRequiredProducts,wishlistId)={Customer},{Product},<redacted>,{StoreId},{AttributesXml},{CustomerEnteredPrice},{RentalStartDate},{RentalEndDate},{Quantity},{AddRequiredProducts},{WishlistId}: update query executed in {Arg11} ms", customer, product, storeId, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, addRequiredProducts, wishlistId, __logStart.ElapsedMilliseconds);
        }
        else
        {
            //new shopping cart item
            warnings.AddRange(await GetShoppingCartItemWarningsAsync(customer, shoppingCartType, product,
                storeId, attributesXml, customerEnteredPrice,
                rentalStartDate, rentalEndDate,
                quantity, addRequiredProducts));

            if (warnings.Any())
                return warnings;

            await addRequiredProductsToCartAsync(wishlistId: wishlistId);

            if (warnings.Any())
                return warnings;

            //maximum items validation
            switch (shoppingCartType)
            {
                case ShoppingCartType.ShoppingCart:
                    if (cart.Count >= _shoppingCartSettings.MaximumShoppingCartItems)
                    {
                        warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumShoppingCartItems"), _shoppingCartSettings.MaximumShoppingCartItems));
                        return warnings;
                    }

                    break;
                case ShoppingCartType.Wishlist:
                    if (cart.Count >= _shoppingCartSettings.MaximumWishlistItems)
                    {
                        warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumWishlistItems"), _shoppingCartSettings.MaximumWishlistItems));
                        return warnings;
                    }

                    break;
                default:
					_logger.LogError(">>> Entering addRequiredProductsToCartAsync(qty={Qty},wishlistId={WishlistId})", qty, wishlistId);
					_logger.LogWarning("<<< Exiting addRequiredProductsToCartAsync(qty={Qty},wishlistId={WishlistId})", qty, wishlistId);
                    break;
            }

            var now = DateTime.UtcNow;
            shoppingCartItem = new ShoppingCartItem
            {
                ShoppingCartType = shoppingCartType,
                StoreId = storeId,
                ProductId = product.Id,
                CustomWishlistId = shoppingCartType == ShoppingCartType.Wishlist ? wishlistId : null,
                AttributesXml = attributesXml,
                CustomerEnteredPrice = customerEnteredPrice,
                Quantity = quantity,
                RentalStartDateUtc = rentalStartDate,
                RentalEndDateUtc = rentalEndDate,
                CreatedOnUtc = now,
                UpdatedOnUtc = now,
                CustomerId = customer.Id
            };

            await _sciRepository.InsertAsync(shoppingCartItem);
			_logger.LogError("AddToCartAsync(customer,product,shoppingCartType,storeId,attributesXml,customerEnteredPrice,rentalStartDate,rentalEndDate,quantity,addRequiredProducts,wishlistId)={Customer},{Product},<redacted>,{StoreId},{AttributesXml},{CustomerEnteredPrice},{RentalStartDate},{RentalEndDate},{Quantity},{AddRequiredProducts},{WishlistId}: save query executed in {Arg11} ms", customer, product, storeId, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, addRequiredProducts, wishlistId, __logStart.ElapsedMilliseconds);

            //updated "HasShoppingCartItems" property used for performance optimization
            var hasShoppingCartItems = !await IsCustomerShoppingCartEmptyAsync(customer);
			_logger.LogError("AddToCartAsync(customer={Customer},product={Product},shoppingCartType=<redacted>,storeId={StoreId},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},addRequiredProducts={AddRequiredProducts},wishlistId={WishlistId}): hasShoppingCartItems → {HasShoppingCartItems}", customer, product, storeId, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, addRequiredProducts, wishlistId, hasShoppingCartItems);
            if (hasShoppingCartItems != customer.HasShoppingCartItems)
            {
                customer.HasShoppingCartItems = hasShoppingCartItems;
				_logger.LogDebug("AddToCartAsync(customer={Customer},product={Product},shoppingCartType=<redacted>,storeId={StoreId},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},addRequiredProducts={AddRequiredProducts},wishlistId={WishlistId}): customer.HasShoppingCartItems → {Arg11}", customer, product, storeId, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, addRequiredProducts, wishlistId, customer.HasShoppingCartItems);
                await _customerService.UpdateCustomerAsync(customer);
            }
        }

		_logger.LogWarning("<<< Exiting AddToCartAsync(customer={Customer},product={Product},shoppingCartType=<redacted>,storeId={StoreId},attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},addRequiredProducts={AddRequiredProducts},wishlistId={WishlistId})", customer, product, storeId, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, addRequiredProducts, wishlistId);
        return warnings;

        async Task addRequiredProductsToCartAsync(int qty = 0, int? wishlistId = null)
        {
			_logger.LogError(">>> Entering addRequiredProductsToCartAsync(qty={Qty},wishlistId={WishlistId})", qty, wishlistId);
            if (!product.RequireOtherProducts)
                return;

            //get these required products
            var requiredProducts = await _productService.GetProductsByIdsAsync(_productService.ParseRequiredProductIds(product));
			_logger.LogError("addRequiredProductsToCartAsync(qty={Qty},wishlistId={WishlistId}): requiredProducts → {RequiredProducts}", qty, wishlistId, requiredProducts);
            if (!requiredProducts.Any())
                return;

            var finalRequiredProducts = requiredProducts.GroupBy(p => p.Id)
                .Select(g => new { Product = g.First(), Count = g.Count() });

            foreach (var requiredProduct in finalRequiredProducts)
            {
                var productsRequiringRequiredProduct = await GetProductsRequiringProductAsync(cart, requiredProduct.Product);
				_logger.LogError("addRequiredProductsToCartAsync(qty={Qty},wishlistId={WishlistId}): productsRequiringRequiredProduct → {ProductsRequiringRequiredProduct}", qty, wishlistId, productsRequiringRequiredProduct);

                //get the required quantity of the required product
                var requiredProductRequiredQuantity = (qty > 0 ? qty : quantity) +
                                                      cart.Where(ci => productsRequiringRequiredProduct.Any(p => p.Id == ci.ProductId))
                                                          .Where(item => item.Id != (shoppingCartItem?.Id ?? 0))
                                                          .Sum(item => item.Quantity);

                //whether required product is already in the cart in the required quantity
                var quantityToAdd = requiredProductRequiredQuantity * requiredProduct.Count - (cart.FirstOrDefault(item => item.ProductId == requiredProduct.Product.Id)?.Quantity ?? 0);
				_logger.LogError("addRequiredProductsToCartAsync(qty={Qty},wishlistId={WishlistId}): quantityToAdd → {QuantityToAdd}", qty, wishlistId, quantityToAdd);
                if (quantityToAdd <= 0)
                    continue;

                if (addRequiredProducts && product.AutomaticallyAddRequiredProducts)
                {
                    //do not add required products to prevent circular references
                    var addToCartWarnings = await AddToCartAsync(customer, requiredProduct.Product, shoppingCartType, storeId,
                        quantity: quantityToAdd, addRequiredProducts: requiredProduct.Product.AutomaticallyAddRequiredProducts, wishlistId: wishlistId);

                    if (addToCartWarnings.Any())
                    {
                        warnings.AddRange(addToCartWarnings);
						_logger.LogWarning("<<< Exiting addRequiredProductsToCartAsync(qty={Qty},wishlistId={WishlistId})", qty, wishlistId);
                        return;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Updates the shopping cart item
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="shoppingCartItemId">Shopping cart item identifier</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="customerEnteredPrice">New customer entered price</param>
    /// <param name="rentalStartDate">Rental start date</param>
    /// <param name="rentalEndDate">Rental end date</param>
    /// <param name="quantity">New shopping cart item quantity</param>
    /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public virtual async Task<IList<string>> UpdateShoppingCartItemAsync(Customer customer,
        int shoppingCartItemId, string attributesXml,
        decimal customerEnteredPrice,
        DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
        int quantity = 1, bool resetCheckoutData = true)
    {
		_logger.LogError(">>> Entering UpdateShoppingCartItemAsync(customer={Customer},shoppingCartItemId=<redacted>,attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},resetCheckoutData={ResetCheckoutData})", customer, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, resetCheckoutData);
        ArgumentNullException.ThrowIfNull(customer);

        var warnings = new List<string>();
		_logger.LogError("UpdateShoppingCartItemAsync(customer={Customer},shoppingCartItemId=<redacted>,attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},resetCheckoutData={ResetCheckoutData}): warnings → {Warnings}", customer, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, resetCheckoutData, warnings);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var shoppingCartItem = await _sciRepository.GetByIdAsync(shoppingCartItemId, cache => default);

		_logger.LogError("UpdateShoppingCartItemAsync(customer,shoppingCartItemId,attributesXml,customerEnteredPrice,rentalStartDate,rentalEndDate,quantity,resetCheckoutData)={Customer},<redacted>,{AttributesXml},{CustomerEnteredPrice},{RentalStartDate},{RentalEndDate},{Quantity},{ResetCheckoutData}: find query executed in {Arg8} ms", customer, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, resetCheckoutData, __logStart.ElapsedMilliseconds);
			_logger.LogError("UpdateShoppingCartItemAsync(customer={Customer},shoppingCartItemId=<redacted>,attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},resetCheckoutData={ResetCheckoutData}): shoppingCartItem → {ShoppingCartItem}", customer, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, resetCheckoutData, shoppingCartItem);
        if (shoppingCartItem == null || shoppingCartItem.CustomerId != customer.Id) {
            return warnings;
        }

        if (resetCheckoutData)
        {
            //reset checkout data
            await _customerService.ResetCheckoutDataAsync(customer, shoppingCartItem.StoreId);
        }

        var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);
		_logger.LogError("UpdateShoppingCartItemAsync(customer={Customer},shoppingCartItemId=<redacted>,attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},resetCheckoutData={ResetCheckoutData}): product → {Product}", customer, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, resetCheckoutData, product);

        if (quantity > 0)
        {
            //check warnings
            warnings.AddRange(await GetShoppingCartItemWarningsAsync(customer, shoppingCartItem.ShoppingCartType,
                product, shoppingCartItem.StoreId,
                attributesXml, customerEnteredPrice,
                rentalStartDate, rentalEndDate, quantity, false, shoppingCartItemId));
            if (warnings.Any())
                return warnings;

            //if everything is OK, then update a shopping cart item
            shoppingCartItem.Quantity = quantity;
			_logger.LogDebug("UpdateShoppingCartItemAsync(customer={Customer},shoppingCartItemId=<redacted>,attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},resetCheckoutData={ResetCheckoutData}): shoppingCartItem.Quantity → {Arg8}", customer, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, resetCheckoutData, shoppingCartItem.Quantity);
			_logger.LogDebug("UpdateShoppingCartItemAsync(customer={Customer},shoppingCartItemId=<redacted>,attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},resetCheckoutData={ResetCheckoutData}): shoppingCartItem.CustomerEnteredPrice → {Arg8}", customer, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, resetCheckoutData, shoppingCartItem.CustomerEnteredPrice);
			_logger.LogDebug("UpdateShoppingCartItemAsync(customer={Customer},shoppingCartItemId=<redacted>,attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},resetCheckoutData={ResetCheckoutData}): shoppingCartItem.RentalStartDateUtc → {Arg8}", customer, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, resetCheckoutData, shoppingCartItem.RentalStartDateUtc);
			_logger.LogDebug("UpdateShoppingCartItemAsync(customer={Customer},shoppingCartItemId=<redacted>,attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},resetCheckoutData={ResetCheckoutData}): shoppingCartItem.RentalEndDateUtc → {Arg8}", customer, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, resetCheckoutData, shoppingCartItem.RentalEndDateUtc);
			_logger.LogDebug("UpdateShoppingCartItemAsync(customer={Customer},shoppingCartItemId=<redacted>,attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},resetCheckoutData={ResetCheckoutData}): shoppingCartItem.UpdatedOnUtc → {Arg8}", customer, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, resetCheckoutData, shoppingCartItem.UpdatedOnUtc);
            shoppingCartItem.AttributesXml = attributesXml;
			_logger.LogDebug("UpdateShoppingCartItemAsync(customer={Customer},shoppingCartItemId=<redacted>,attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},resetCheckoutData={ResetCheckoutData}): shoppingCartItem.AttributesXml → {Arg8}", customer, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, resetCheckoutData, shoppingCartItem.AttributesXml);
            shoppingCartItem.CustomerEnteredPrice = customerEnteredPrice;
            shoppingCartItem.RentalStartDateUtc = rentalStartDate;
            shoppingCartItem.RentalEndDateUtc = rentalEndDate;
            shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;

            await _sciRepository.UpdateAsync(shoppingCartItem);
			_logger.LogError("UpdateShoppingCartItemAsync(customer,shoppingCartItemId,attributesXml,customerEnteredPrice,rentalStartDate,rentalEndDate,quantity,resetCheckoutData)={Customer},<redacted>,{AttributesXml},{CustomerEnteredPrice},{RentalStartDate},{RentalEndDate},{Quantity},{ResetCheckoutData}: update query executed in {Arg8} ms", customer, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, resetCheckoutData, __logStart.ElapsedMilliseconds);
        }
        else
        {
            //check warnings for required products
            warnings.AddRange(await GetRequiredProductWarningsAsync(customer, shoppingCartItem.ShoppingCartType,
                product, shoppingCartItem.StoreId, quantity, false, shoppingCartItemId));
            if (warnings.Any())
                return warnings;

            //delete a shopping cart item
            await DeleteShoppingCartItemAsync(shoppingCartItem, resetCheckoutData, true);
        }

		_logger.LogWarning("<<< Exiting UpdateShoppingCartItemAsync(customer={Customer},shoppingCartItemId=<redacted>,attributesXml={AttributesXml},customerEnteredPrice={CustomerEnteredPrice},rentalStartDate={RentalStartDate},rentalEndDate={RentalEndDate},quantity={Quantity},resetCheckoutData={ResetCheckoutData})", customer, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, resetCheckoutData);
        return warnings;
    }

    /// <summary>
    /// Move shopping cart item to a custom wishlist
    /// </summary>
    /// <param name="shoppingCartItemId">Shopping cart item identifier</param>
    /// <param name="wishlistId">Custom wishlist identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task MoveItemToCustomWishlistAsync(int shoppingCartItemId, int? wishlistId = null)
    {
		_logger.LogError(">>> Entering MoveItemToCustomWishlistAsync(shoppingCartItemId=<redacted>,wishlistId={WishlistId})", wishlistId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var shoppingCartItemFrom = await _sciRepository.GetByIdAsync(shoppingCartItemId, cache => default);
        if (shoppingCartItemFrom == null)
            return;
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogError("MoveItemToCustomWishlistAsync(shoppingCartItemId=<redacted>,wishlistId={WishlistId}): shoppingCartItemFrom → {ShoppingCartItemFrom}", wishlistId, shoppingCartItemFrom);
			_logger.LogError("MoveItemToCustomWishlistAsync(shoppingCartItemId,wishlistId)=<redacted>,{WishlistId}: find query executed in {Arg2} ms", wishlistId, __logStartDb.ElapsedMilliseconds);

        var customer = await _customerService.GetCustomerByIdAsync(shoppingCartItemFrom.CustomerId);
        var product = await _productService.GetProductByIdAsync(shoppingCartItemFrom.ProductId);
		_logger.LogError("MoveItemToCustomWishlistAsync(shoppingCartItemId=<redacted>,wishlistId={WishlistId}): customer → {Customer}", wishlistId, customer);
		_logger.LogError("MoveItemToCustomWishlistAsync(shoppingCartItemId=<redacted>,wishlistId={WishlistId}): product → {Product}", wishlistId, product);
        var cart = await GetShoppingCartAsync(customer, shoppingCartItemFrom.ShoppingCartType, shoppingCartItemFrom.StoreId, product.Id, customWishlistId: wishlistId);

		_logger.LogError("MoveItemToCustomWishlistAsync(shoppingCartItemId=<redacted>,wishlistId={WishlistId}): cart → {Cart}", wishlistId, cart);
        var shoppingCartItemTo = await cart.FirstOrDefaultAwaitAsync(async sci => await ShoppingCartItemIsEqualAsync(sci, product, shoppingCartItemFrom.AttributesXml, shoppingCartItemFrom.CustomerEnteredPrice, shoppingCartItemFrom.RentalStartDateUtc, shoppingCartItemFrom.RentalEndDateUtc));

		_logger.LogError("MoveItemToCustomWishlistAsync(shoppingCartItemId=<redacted>,wishlistId={WishlistId}): shoppingCartItemTo → {ShoppingCartItemTo}", wishlistId, shoppingCartItemTo);
        if (shoppingCartItemTo != null)
        {
            //update existing shopping cart item
            var newQuantity = shoppingCartItemTo.Quantity + shoppingCartItemFrom.Quantity;
            shoppingCartItemTo.Quantity = newQuantity;
			_logger.LogDebug("MoveItemToCustomWishlistAsync(shoppingCartItemId=<redacted>,wishlistId={WishlistId}): shoppingCartItemTo.Quantity → {Arg2}", wishlistId, shoppingCartItemTo.Quantity);
            shoppingCartItemTo.UpdatedOnUtc = DateTime.UtcNow;
			_logger.LogDebug("MoveItemToCustomWishlistAsync(shoppingCartItemId=<redacted>,wishlistId={WishlistId}): shoppingCartItemTo.UpdatedOnUtc → {Arg2}", wishlistId, shoppingCartItemTo.UpdatedOnUtc);

            await _sciRepository.UpdateAsync(shoppingCartItemTo);
			_logger.LogError("MoveItemToCustomWishlistAsync(shoppingCartItemId,wishlistId)=<redacted>,{WishlistId}: update query executed in {Arg2} ms", wishlistId, __logStartDb.ElapsedMilliseconds);
            await _sciRepository.DeleteAsync(shoppingCartItemFrom);
        }
        else
        {
			_logger.LogError("MoveItemToCustomWishlistAsync(shoppingCartItemId,wishlistId)=<redacted>,{WishlistId}: delete query executed in {Arg2} ms", wishlistId, __logStartDb.ElapsedMilliseconds);
            //update custom wishlist id
            shoppingCartItemFrom.CustomWishlistId = wishlistId;
			_logger.LogDebug("MoveItemToCustomWishlistAsync(shoppingCartItemId=<redacted>,wishlistId={WishlistId}): shoppingCartItemFrom.CustomWishlistId → {Arg2}", wishlistId, shoppingCartItemFrom.CustomWishlistId);
            shoppingCartItemFrom.UpdatedOnUtc = DateTime.UtcNow;
			_logger.LogDebug("MoveItemToCustomWishlistAsync(shoppingCartItemId=<redacted>,wishlistId={WishlistId}): shoppingCartItemFrom.UpdatedOnUtc → {Arg2}", wishlistId, shoppingCartItemFrom.UpdatedOnUtc);
            await _sciRepository.UpdateAsync(shoppingCartItemFrom);
        }
		_logger.LogError("MoveItemToCustomWishlistAsync(shoppingCartItemId,wishlistId)=<redacted>,{WishlistId}: external service call took {Arg2} ms", wishlistId, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting MoveItemToCustomWishlistAsync(shoppingCartItemId=<redacted>,wishlistId={WishlistId})", wishlistId);
    }

    /// <summary>
    /// Migrate shopping cart
    /// </summary>
    /// <param name="fromCustomer">From customer</param>
    /// <param name="toCustomer">To customer</param>
    /// <param name="includeCouponCodes">A value indicating whether to coupon codes (discount and gift card) should be also re-applied</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task MigrateShoppingCartAsync(Customer fromCustomer, Customer toCustomer, bool includeCouponCodes)
    {
		_logger.LogError(">>> Entering MigrateShoppingCartAsync(fromCustomer={FromCustomer},toCustomer={ToCustomer},includeCouponCodes={IncludeCouponCodes})", fromCustomer, toCustomer, includeCouponCodes);
        ArgumentNullException.ThrowIfNull(fromCustomer);
        ArgumentNullException.ThrowIfNull(toCustomer);

        if (fromCustomer.Id == toCustomer.Id)
            return; //the same customer

        //shopping cart items
        var fromCart = await GetShoppingCartAsync(fromCustomer);
		_logger.LogError("MigrateShoppingCartAsync(fromCustomer={FromCustomer},toCustomer={ToCustomer},includeCouponCodes={IncludeCouponCodes}): fromCart → {FromCart}", fromCustomer, toCustomer, includeCouponCodes, fromCart);

        for (var i = 0; i < fromCart.Count; i++)
        {
            var sci = fromCart[i];
            var product = await _productService.GetProductByIdAsync(sci.ProductId);
			_logger.LogError("MigrateShoppingCartAsync(fromCustomer={FromCustomer},toCustomer={ToCustomer},includeCouponCodes={IncludeCouponCodes}): product → {Product}", fromCustomer, toCustomer, includeCouponCodes, product);

            await AddToCartAsync(toCustomer, product, sci.ShoppingCartType, sci.StoreId,
                sci.AttributesXml, sci.CustomerEnteredPrice,
                sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, false);
        }

        for (var i = 0; i < fromCart.Count; i++)
        {
            var sci = fromCart[i];
            await DeleteShoppingCartItemAsync(sci);
        }

        //copy discount and gift card coupon codes
        if (includeCouponCodes)
        {
            //discount
            foreach (var code in await _customerService.ParseAppliedDiscountCouponCodesAsync(fromCustomer))
                await _customerService.ApplyDiscountCouponCodeAsync(toCustomer, code);

            //gift card
            foreach (var code in await _customerService.ParseAppliedGiftCardCouponCodesAsync(fromCustomer))
                await _customerService.ApplyGiftCardCouponCodeAsync(toCustomer, code);
        }

        //move selected checkout attributes
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("MigrateShoppingCartAsync(fromCustomer={FromCustomer},toCustomer={ToCustomer},includeCouponCodes={IncludeCouponCodes}): store → {Store}", fromCustomer, toCustomer, includeCouponCodes, store);
        var checkoutAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(fromCustomer, NopCustomerDefaults.CheckoutAttributes, store.Id);
		_logger.LogError("MigrateShoppingCartAsync(fromCustomer={FromCustomer},toCustomer={ToCustomer},includeCouponCodes={IncludeCouponCodes}): checkoutAttributesXml → {CheckoutAttributesXml}", fromCustomer, toCustomer, includeCouponCodes, checkoutAttributesXml);
        await _genericAttributeService.SaveAttributeAsync(toCustomer, NopCustomerDefaults.CheckoutAttributes, checkoutAttributesXml, store.Id);
		_logger.LogWarning("<<< Exiting MigrateShoppingCartAsync(fromCustomer={FromCustomer},toCustomer={ToCustomer},includeCouponCodes={IncludeCouponCodes})", fromCustomer, toCustomer, includeCouponCodes);
    }

    /// <summary>
    /// Indicates whether the shopping cart requires shipping
    /// </summary>
    /// <param name="shoppingCart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if the shopping cart requires shipping; otherwise, false.
    /// </returns>
    public virtual async Task<bool> ShoppingCartRequiresShippingAsync(IList<ShoppingCartItem> shoppingCart)
    {
		_logger.LogError(">>> Entering ShoppingCartRequiresShippingAsync(shoppingCart=<redacted>)");
		_logger.LogWarning("<<< Exiting ShoppingCartRequiresShippingAsync(shoppingCart=<redacted>)");
        return await shoppingCart.AnyAwaitAsync(async shoppingCartItem => await _shippingService.IsShipEnabledAsync(shoppingCartItem));
    }

    /// <summary>
    /// Gets a value indicating whether shopping cart is recurring
    /// </summary>
    /// <param name="shoppingCart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> ShoppingCartIsRecurringAsync(IList<ShoppingCartItem> shoppingCart)
    {
		_logger.LogError(">>> Entering ShoppingCartIsRecurringAsync(shoppingCart=<redacted>)");
        ArgumentNullException.ThrowIfNull(shoppingCart);

		_logger.LogWarning("<<< Exiting ShoppingCartIsRecurringAsync(shoppingCart=<redacted>)");
        if (!shoppingCart.Any()) {
            return false;
        }

        return await _productService.HasAnyRecurringProductAsync(shoppingCart.Select(sci => sci.ProductId).ToArray());
    }

    /// <summary>
    /// Get a recurring cycle information
    /// </summary>
    /// <param name="shoppingCart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the error (if exists); otherwise, empty string. Cycle length. Cycle period. Total cycles
    /// </returns>
    public virtual async Task<(string error, int cycleLength, RecurringProductCyclePeriod cyclePeriod, int totalCycles)> GetRecurringCycleInfoAsync(IList<ShoppingCartItem> shoppingCart)
    {
        var rezCycleLength = 0;
        RecurringProductCyclePeriod rezCyclePeriod = 0;
        var rezTotalCycles = 0;

        int? cycleLength = null;
        RecurringProductCyclePeriod? cyclePeriod = null;
        int? totalCycles = null;

        var conflictError = await _localizationService.GetResourceAsync("ShoppingCart.ConflictingShipmentSchedules");

        foreach (var sci in shoppingCart)
        {
            var product = await _productService.GetProductByIdAsync(sci.ProductId) ?? throw new NopException($"Product (Id={sci.ProductId}) cannot be loaded");

            if (!product.IsRecurring)
                continue;

            //cycle length
            if (cycleLength.HasValue && cycleLength.Value != product.RecurringCycleLength)
                return (conflictError, rezCycleLength, rezCyclePeriod, rezTotalCycles);
            cycleLength = product.RecurringCycleLength;

            //cycle period
            if (cyclePeriod.HasValue && cyclePeriod.Value != product.RecurringCyclePeriod)
                return (conflictError, rezCycleLength, rezCyclePeriod, rezTotalCycles);
            cyclePeriod = product.RecurringCyclePeriod;

            //total cycles
            if (totalCycles.HasValue && totalCycles.Value != product.RecurringTotalCycles)
                return (conflictError, rezCycleLength, rezCyclePeriod, rezTotalCycles);
            totalCycles = product.RecurringTotalCycles;
        }

        if (!cycleLength.HasValue)
            return (string.Empty, rezCycleLength, rezCyclePeriod, rezTotalCycles);

        rezCycleLength = cycleLength.Value;
        rezCyclePeriod = cyclePeriod.Value;
        rezTotalCycles = totalCycles.Value;

        return (string.Empty, rezCycleLength, rezCyclePeriod, rezTotalCycles);
    }

    #endregion
}