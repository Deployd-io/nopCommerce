using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
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
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the product model factory
/// </summary>
public partial class ProductModelFactory : IProductModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly ICategoryService _categoryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly ICustomWishlistService _customWishlistService;
    protected readonly IDateRangeService _dateRangeService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IDownloadService _downloadService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IJsonLdModelFactory _jsonLdModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPictureService _pictureService;
    protected readonly IPriceCalculationService _priceCalculationService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductReviewService _productReviewService;
    protected readonly IProductService _productService;
    protected readonly IProductTagService _productTagService;
    protected readonly IProductTemplateService _productTemplateService;
    protected readonly IReviewTypeService _reviewTypeService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly ISpecificationAttributeService _specificationAttributeService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;
    protected readonly ITaxService _taxService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVendorService _vendorService;
    protected readonly IVideoService _videoService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly OrderSettings _orderSettings;
    protected readonly SeoSettings _seoSettings;
    protected readonly ShippingSettings _shippingSettings;
    protected readonly VendorSettings _vendorSettings;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public ProductModelFactory(CaptchaSettings captchaSettings,
        CatalogSettings catalogSettings,
        CustomerSettings customerSettings,
        ICategoryService categoryService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        ICustomWishlistService customWishlistService,
        IDateRangeService dateRangeService,
        IDateTimeHelper dateTimeHelper,
        IDownloadService downloadService,
        IGenericAttributeService genericAttributeService,
        IJsonLdModelFactory jsonLdModelFactory,
        ILocalizationService localizationService,
        IManufacturerService manufacturerService,
        IPermissionService permissionService,
        IPictureService pictureService,
        IPriceCalculationService priceCalculationService,
        IPriceFormatter priceFormatter,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IProductReviewService productReviewService,
        IProductService productService,
        IProductTagService productTagService,
        IProductTemplateService productTemplateService,
        IReviewTypeService reviewTypeService,
        IShoppingCartService shoppingCartService,
        ISpecificationAttributeService specificationAttributeService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreService storeService,
        IShoppingCartModelFactory shoppingCartModelFactory,
        ITaxService taxService,
        IUrlRecordService urlRecordService,
        IVendorService vendorService,
        IVideoService videoService,
        IWebHelper webHelper,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        OrderSettings orderSettings,
        SeoSettings seoSettings,
        ShippingSettings shippingSettings,
        VendorSettings vendorSettings)
    {
        _captchaSettings = captchaSettings;
        _catalogSettings = catalogSettings;
        _customerSettings = customerSettings;
        _categoryService = categoryService;
        _currencyService = currencyService;
        _customerService = customerService;
        _customWishlistService = customWishlistService;
        _dateRangeService = dateRangeService;
        _dateTimeHelper = dateTimeHelper;
        _downloadService = downloadService;
        _genericAttributeService = genericAttributeService;
        _jsonLdModelFactory = jsonLdModelFactory;
        _localizationService = localizationService;
        _manufacturerService = manufacturerService;
        _permissionService = permissionService;
        _pictureService = pictureService;
        _priceCalculationService = priceCalculationService;
        _priceFormatter = priceFormatter;
        _productAttributeParser = productAttributeParser;
        _productAttributeService = productAttributeService;
        _productReviewService = productReviewService;
        _productService = productService;
        _productTagService = productTagService;
        _productTemplateService = productTemplateService;
        _reviewTypeService = reviewTypeService;
        _shoppingCartService = shoppingCartService;
        _specificationAttributeService = specificationAttributeService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _storeService = storeService;
        _shoppingCartModelFactory = shoppingCartModelFactory;
        _taxService = taxService;
        _urlRecordService = urlRecordService;
        _vendorService = vendorService;
        _webHelper = webHelper;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _orderSettings = orderSettings;
        _seoSettings = seoSettings;
        _shippingSettings = shippingSettings;
        _vendorSettings = vendorSettings;
        _videoService = videoService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare the grouped product overview price model
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="priceModel">Price model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareGroupedProductPriceModelAsync(Product product, ProductPriceModel priceModel)
    {
		_logger.LogError(">>> Entering PrepareGroupedProductPriceModelAsync(product={Product},priceModel={PriceModel})", product, priceModel);
        var store = await _storeContext.GetCurrentStoreAsync();
        var associatedProducts = await _productService.GetAssociatedProductsAsync(product.Id,
            store.Id);
			_logger.LogError("PrepareGroupedProductPriceModelAsync(product={Product},priceModel={PriceModel}): store → {Store}", product, priceModel, store);

        if (!associatedProducts.Any())
            return;

        //we have at least one associated product
        //find a minimum possible price
        decimal? minPossiblePrice = null;
        Product minPriceProduct = null;
		_logger.LogError("PrepareGroupedProductPriceModelAsync(product={Product},priceModel={PriceModel}): minPriceProduct → {MinPriceProduct}", product, priceModel, minPriceProduct);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareGroupedProductPriceModelAsync(product={Product},priceModel={PriceModel}): customer → {Customer}", product, priceModel, customer);
        foreach (var associatedProduct in associatedProducts)
        {
            var (_, tmpMinPossiblePrice, _, _) = await _priceCalculationService.GetFinalPriceAsync(associatedProduct, customer, store);

            //calculate price for the maximum quantity if we have tier prices, and choose minimal
            tmpMinPossiblePrice = Math.Min(tmpMinPossiblePrice,
                (await _priceCalculationService.GetFinalPriceAsync(associatedProduct, customer, store, quantity: int.MaxValue)).finalPrice);

            if (minPossiblePrice.HasValue && tmpMinPossiblePrice >= minPossiblePrice.Value)
                continue;

            minPriceProduct = associatedProduct;
            minPossiblePrice = tmpMinPossiblePrice;
			_logger.LogDebug("PrepareGroupedProductPriceModelAsync(product={Product},priceModel={PriceModel}): minPossiblePrice → {MinPossiblePrice}", product, priceModel, minPossiblePrice);
        }

        if (minPriceProduct == null || minPriceProduct.CustomerEntersPrice)
            return;

        if (minPriceProduct.CallForPrice &&
            //also check whether the current user is impersonated
            (!_orderSettings.AllowAdminsToBuyCallForPriceProducts ||
             _workContext.OriginalCustomerIfImpersonated == null))
        {
            priceModel.OldPrice = null;
			_logger.LogDebug("PrepareGroupedProductPriceModelAsync(product={Product},priceModel={PriceModel}): priceModel.OldPrice → {Arg2}", product, priceModel, priceModel.OldPrice);
			_logger.LogDebug("PrepareGroupedProductPriceModelAsync(product={Product},priceModel={PriceModel}): priceModel.OldPriceValue → {Arg2}", product, priceModel, priceModel.OldPriceValue);
            priceModel.OldPriceValue = null;
            priceModel.Price = await _localizationService.GetResourceAsync("Products.CallForPrice");
            priceModel.PriceValue = null;
			_logger.LogDebug("PrepareGroupedProductPriceModelAsync(product={Product},priceModel={PriceModel}): priceModel.PriceValue → {Arg2}", product, priceModel, priceModel.PriceValue);
        }
        else
        {
            //calculate prices
            var (finalPriceBase, _) = await _taxService.GetProductPriceAsync(minPriceProduct, minPossiblePrice.Value);
            var finalPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceBase, await _workContext.GetWorkingCurrencyAsync());
			_logger.LogError("PrepareGroupedProductPriceModelAsync(product={Product},priceModel={PriceModel}): finalPrice → {FinalPrice}", product, priceModel, finalPrice);

            priceModel.OldPrice = null;
            priceModel.OldPriceValue = null;
            priceModel.Price = string.Format(await _localizationService.GetResourceAsync("Products.PriceRangeFrom"), await _priceFormatter.FormatPriceAsync(finalPrice));
            priceModel.PriceValue = finalPrice;

            //PAngV default baseprice (used in Germany)
            priceModel.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(product, finalPriceBase);
            priceModel.BasePricePAngVValue = finalPriceBase;
			_logger.LogDebug("PrepareGroupedProductPriceModelAsync(product={Product},priceModel={PriceModel}): priceModel.BasePricePAngVValue → {Arg2}", product, priceModel, priceModel.BasePricePAngVValue);
        }
		_logger.LogWarning("<<< Exiting PrepareGroupedProductPriceModelAsync(product={Product},priceModel={PriceModel})", product, priceModel);
    }

    /// <summary>
    /// Gets the minimum possible product price
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="customer">Customer</param>
    /// <param name="store">Store</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the minimum possible product price
    /// </returns>
    protected virtual async Task<(bool hasMultiplePrices, decimal minPossiblePriceWithoutDiscount, decimal minPossiblePriceWithDiscount)> GetFromPriceAsync(Product product, Customer customer, Store store)
    {
        var hasMultiplePrices = false;

        //calculate the base product price
        var (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount, _, _) = await _priceCalculationService
            .GetFinalPriceAsync(product, customer, store);

        //calculate price based on price adjustments of combinations and attributes
        if (_catalogSettings.DisplayFromPrices)
        {
            //prepare cache key
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
            var cacheKey = _staticCacheManager
                .PrepareKeyForDefaultCache(NopCatalogDefaults.ProductMultiplePriceCacheKey, product, customerRoleIds, store);
            if (!_catalogSettings.CacheProductPrices || product.IsRental)
                cacheKey.CacheTime = 0;

            //try to cache the min price
            var cachedPrice = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var prices = new List<(decimal PriceWithoutDiscount, decimal PriceWithDiscount)>();

                //we shouldn't use the base product price if there is at least one required attribute
                var ignoreBasePrice = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id))
                    .Any(am => !am.IsNonCombinable() && am.IsRequired);

                //check all possible attribute combinations for min price
                var allAttributesXml = await _productAttributeParser.GenerateAllCombinationsAsync(product, true);
                foreach (var attributesXml in allAttributesXml)
                {
                    var warnings = await _shoppingCartService
                        .GetShoppingCartItemAttributeWarningsAsync(customer, ShoppingCartType.ShoppingCart, product, 1, attributesXml, true, true, true);
                    if (warnings.Any())
                        continue;

                    //check for overridden combination price, we should use it instead of the base product price if it exists
                    var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
                    if (combination?.OverriddenPrice.HasValue ?? false)
                    {
                        var (priceWithoutDiscount, priceWithDiscount, _, _) = await _priceCalculationService
                            .GetFinalPriceAsync(product, customer, store, combination.OverriddenPrice.Value, decimal.Zero, true, 1, null, null);
                        prices.Add((priceWithoutDiscount, priceWithDiscount));
                        continue;
                    }

                    //or check for attribute price adjustment, in this case we should add it to the base product price
                    var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
                    var additionalCharge = await attributeValues.SumAwaitAsync(async attributeValue =>
                        await _priceCalculationService.GetProductAttributeValuePriceAdjustmentAsync(product, attributeValue, customer, store));
                    if (additionalCharge != decimal.Zero)
                    {
                        var (priceWithoutDiscount, priceWithDiscount, _, _) = await _priceCalculationService
                            .GetFinalPriceAsync(product, customer, store, additionalCharge);
                        prices.Add((priceWithoutDiscount, priceWithDiscount));
                        continue;
                    }

                    //there are no price adjustments of combinations and attributes, so add the base product price in the list as possible
                    prices.Add((minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount));
                }

                //don't cache (return null) if there are no multiple prices
                if (prices.Distinct().Count() < 2)
                    return null;

                //find the min price
                var (minPriceWithoutDiscount, minPriceWithDiscount) = prices.OrderBy(p => p.PriceWithDiscount).First();
                return new { PriceWithoutDiscount = minPriceWithoutDiscount, PriceWithDiscount = minPriceWithDiscount, IgnoreBasePrice = ignoreBasePrice };
            });

            if (cachedPrice is not null)
            {
                hasMultiplePrices = true;

                //change min product price
                if (cachedPrice.IgnoreBasePrice || cachedPrice.PriceWithDiscount < minPossiblePriceWithDiscount)
                {
                    minPossiblePriceWithoutDiscount = cachedPrice.PriceWithoutDiscount;
                    minPossiblePriceWithDiscount = cachedPrice.PriceWithDiscount;
                }
            }
        }

        //calculate price for the maximum quantity (in case if we have tier prices)
        //when there is just one tier price (with  qty 1), there are no actual savings in the list.
        var tierPrices = await _productService.GetTierPricesAsync(product, customer, store);
        if (tierPrices.Any() && (tierPrices.Count > 1 || tierPrices[0].Quantity > 1))
        {
            hasMultiplePrices = true;

            var (minTierPriceWithoutDiscount, minTierPriceWithDiscount, _, _) = await _priceCalculationService
                .GetFinalPriceAsync(product, customer, store, quantity: int.MaxValue);
            if (minTierPriceWithDiscount < minPossiblePriceWithDiscount)
            {
                //change min product price
                minPossiblePriceWithoutDiscount = minTierPriceWithoutDiscount;
                minPossiblePriceWithDiscount = minTierPriceWithDiscount;
            }
        }

        return (hasMultiplePrices, minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount);
    }

    /// <summary>
    /// Prepare the product price model
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="addPriceRangeFrom">Indicates whether to add a price range information</param>
    /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product price model
    /// </returns>
    protected virtual async Task<ProductPriceModel> PrepareProductPriceModelAsync(Product product, bool addPriceRangeFrom = false, bool forceRedirectionAfterAddingToCart = false)
    {
		_logger.LogError(">>> Entering PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart})", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart);
        ArgumentNullException.ThrowIfNull(product);

        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
		_logger.LogError("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): currentCurrency → {CurrentCurrency}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, currentCurrency);

        var model = new ProductPriceModel
        {
            ProductId = product.Id,
            //add to cart button (ignore "DisableBuyButton" property for grouped products)
            DisableBuyButton = (product.ProductType != ProductType.GroupedProduct && product.DisableBuyButton) ||
                !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART) ||
                !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES),
            //add to wishlist button (ignore "DisableWishlistButton" property for grouped products)
            DisableWishlistButton = (product.ProductType != ProductType.GroupedProduct && product.DisableWishlistButton) ||
                !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_WISHLIST) ||
                !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES),
            //compare products
            DisableAddToCompareListButton = !_catalogSettings.CompareProductsEnabled,
            //currency code
            CurrencyCode = currentCurrency.CurrencyCode,
            ForceRedirectionAfterAddingToCart = forceRedirectionAfterAddingToCart
        };

        if (!await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES))
        {
            model.OldPrice = null;
			_logger.LogDebug("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): model.OldPrice → {Arg3}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, model.OldPrice);
			_logger.LogDebug("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): model.OldPriceValue → {Arg3}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, model.OldPriceValue);
			_logger.LogDebug("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): model.Price → {Arg3}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, model.Price);
			_logger.LogDebug("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): model.PriceValue → {Arg3}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, model.PriceValue);
			_logger.LogDebug("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): model.HidePrices → {Arg3}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, model.HidePrices);
            model.OldPriceValue = null;
            model.Price = null;
            model.PriceValue = null;
            model.HidePrices = true;

            return model;
        }

        if (addPriceRangeFrom && product.ProductType == ProductType.GroupedProduct)
        {
            await PrepareGroupedProductPriceModelAsync(product, model);

            return model;
        }

        //pre-order
        if (product.AvailableForPreOrder)
        {
            model.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                product.PreOrderAvailabilityStartDateTimeUtc.Value >=
                DateTime.UtcNow;
				_logger.LogDebug("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): model.AvailableForPreOrder → {Arg3}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, model.AvailableForPreOrder);

            model.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;
			_logger.LogDebug("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): model.PreOrderAvailabilityStartDateTimeUtc → {Arg3}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, model.PreOrderAvailabilityStartDateTimeUtc);
        }

        if (product.CustomerEntersPrice)
        {
            model.CustomerEntersPrice = true;
			_logger.LogDebug("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): model.CustomerEntersPrice → {Arg3}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, model.CustomerEntersPrice);

            return model;
        }

        if (product.CallForPrice &&
            //also check whether the current user is impersonated
            (!_orderSettings.AllowAdminsToBuyCallForPriceProducts ||
                _workContext.OriginalCustomerIfImpersonated == null))
        {
            model.CallForPrice = true;
			_logger.LogDebug("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): model.CallForPrice → {Arg3}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, model.CallForPrice);

            //call for price
            model.OldPrice = null;
            model.OldPriceValue = null;
            model.Price = await _localizationService.GetResourceAsync("Products.CallForPrice");
            model.PriceValue = null;

            return model;
        }

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): store → {Store}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, store);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): customer → {Customer}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, customer);

        //prices
        decimal minPossiblePriceWithoutDiscount;
        decimal minPossiblePriceWithDiscount;
        var hasMultiplePrices = false;

        if (addPriceRangeFrom)
            (hasMultiplePrices, minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount) = await GetFromPriceAsync(product, customer, store);
        else
            (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, store);

        var (oldPriceBase, _) = await _taxService.GetProductPriceAsync(product, product.OldPrice);
        var (finalPriceWithoutDiscountBase, _) = await _taxService.GetProductPriceAsync(product, minPossiblePriceWithoutDiscount);
        var (finalPriceWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, minPossiblePriceWithDiscount);
        var oldPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(oldPriceBase, currentCurrency);
		_logger.LogError("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): oldPrice → {OldPrice}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, oldPrice);
        var finalPriceWithoutDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithoutDiscountBase, currentCurrency);
		_logger.LogError("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): finalPriceWithoutDiscount → {FinalPriceWithoutDiscount}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, finalPriceWithoutDiscount);
        var finalPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithDiscountBase, currentCurrency);
		_logger.LogError("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): finalPriceWithDiscount → {FinalPriceWithDiscount}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, finalPriceWithDiscount);

        model.Price = await _priceFormatter.FormatPriceAsync(finalPriceWithoutDiscount);
        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
        {
            model.OldPrice = await _priceFormatter.FormatPriceAsync(oldPrice);
            model.OldPriceValue = oldPrice;
        }
        else
        {
            model.OldPrice = null;
            model.OldPriceValue = null;
        }

        if (addPriceRangeFrom)
        {
            var strikeThroughPrice = decimal.Zero;
			_logger.LogError("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): strikeThroughPrice → {StrikeThroughPrice}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, strikeThroughPrice);

            if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                strikeThroughPrice = finalPriceWithoutDiscount;

            if (strikeThroughPrice > decimal.Zero)
            {
                model.OldPrice = await _priceFormatter.FormatPriceAsync(strikeThroughPrice);
                model.OldPriceValue = strikeThroughPrice;
            }

            model.Price = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
        }

        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
        {
            model.PriceWithDiscount = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
            model.PriceWithDiscountValue = finalPriceWithDiscount;
			_logger.LogDebug("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): model.PriceWithDiscountValue → {Arg3}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, model.PriceWithDiscountValue);
        }

        model.PriceValue = finalPriceWithDiscount;

        if (hasMultiplePrices)
            model.Price = string.Format(await _localizationService.GetResourceAsync("Products.PriceRangeFrom"), model.Price);

        //property for German market
        //we display tax/shipping info only with "shipping enabled" for this product
        //we also ensure this it's not free shipping
        model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoProductBoxes
            && product.IsShipEnabled
            && !product.IsFreeShipping;
			_logger.LogDebug("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): model.DisplayTaxShippingInfo → {Arg3}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, model.DisplayTaxShippingInfo);

        if (product.IsRental)
        {
            model.IsRental = true;
			_logger.LogDebug("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): model.IsRental → {Arg3}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, model.IsRental);
            model.OldPrice = await _priceFormatter.FormatRentalProductPeriodAsync(product, model.OldPrice);
            model.Price = await _priceFormatter.FormatRentalProductPeriodAsync(product, model.Price);
            var priceStr = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
			_logger.LogError("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): priceStr → {PriceStr}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, priceStr);
            model.RentalPrice = await _priceFormatter.FormatRentalProductPeriodAsync(product, priceStr);
            model.RentalPriceValue = finalPriceWithDiscount;
			_logger.LogDebug("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): model.RentalPriceValue → {Arg3}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, model.RentalPriceValue);
        }

        //PAngV default base price (used in Germany)
        model.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(product, finalPriceWithDiscountBase);
        model.BasePricePAngVValue = finalPriceWithDiscountBase;
		_logger.LogDebug("PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): model.BasePricePAngVValue → {Arg3}", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart, model.BasePricePAngVValue);

		_logger.LogWarning("<<< Exiting PrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart})", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart);
        return model;
    }

    /// <summary>
    /// Prepare the product specification models
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="group">Specification attribute group</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of product specification model
    /// </returns>
    protected virtual async Task<IList<ProductSpecificationAttributeModel>> PrepareProductSpecificationAttributeModelAsync(Product product, SpecificationAttributeGroup group)
    {
		_logger.LogError(">>> Entering PrepareProductSpecificationAttributeModelAsync(product={Product},group={Group})", product, group);
        ArgumentNullException.ThrowIfNull(product);

        var productSpecificationAttributes = await _specificationAttributeService.GetProductSpecificationAttributesAsync(
            product.Id, specificationAttributeGroupId: group?.Id, showOnProductPage: true);

        var result = new List<ProductSpecificationAttributeModel>();
		_logger.LogError("PrepareProductSpecificationAttributeModelAsync(product={Product},group={Group}): result → {Result}", product, group, result);

        foreach (var psa in productSpecificationAttributes)
        {
            var option = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(psa.SpecificationAttributeOptionId);
			_logger.LogError("PrepareProductSpecificationAttributeModelAsync(product={Product},group={Group}): option → {Option}", product, group, option);

            var model = result.FirstOrDefault(model => model.Id == option.SpecificationAttributeId);
			_logger.LogError("PrepareProductSpecificationAttributeModelAsync(product={Product},group={Group}): model → {Model}", product, group, model);
            if (model == null)
            {
                var attribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(option.SpecificationAttributeId);
				_logger.LogError("PrepareProductSpecificationAttributeModelAsync(product={Product},group={Group}): attribute → {Attribute}", product, group, attribute);
                model = new ProductSpecificationAttributeModel
                {
                    Id = attribute.Id,
                    Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name)
                };
                result.Add(model);
            }

            var value = new ProductSpecificationAttributeValueModel
            {
                AttributeTypeId = psa.AttributeTypeId,
                ColorSquaresRgb = option.ColorSquaresRgb,
                ValueRaw = psa.AttributeType switch
                {
                    SpecificationAttributeType.Option => WebUtility.HtmlEncode(await _localizationService.GetLocalizedAsync(option, x => x.Name)),
                    SpecificationAttributeType.CustomText => WebUtility.HtmlEncode(await _localizationService.GetLocalizedAsync(psa, x => x.CustomValue)),
                    SpecificationAttributeType.CustomHtmlText => await _localizationService.GetLocalizedAsync(psa, x => x.CustomValue),
                    SpecificationAttributeType.Hyperlink => $"<a href='{psa.CustomValue}' target='_blank'>{psa.CustomValue}</a>",
                    _ => null
                }
            };

            model.Values.Add(value);
        }

		_logger.LogWarning("<<< Exiting PrepareProductSpecificationAttributeModelAsync(product={Product},group={Group})", product, group);
        return result;
    }

    /// <summary>
    /// Prepare the product review overview model
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product review overview model
    /// </returns>
    protected virtual async Task<ProductReviewOverviewModel> PrepareProductReviewOverviewModelAsync(Product product)
    {
		_logger.LogError(">>> Entering PrepareProductReviewOverviewModelAsync(product={Product})", product);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ProductReviewOverviewModel productReview;
        var currentStore = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareProductReviewOverviewModelAsync(product={Product}): currentStore → {CurrentStore}", product, currentStore);

        if (_catalogSettings.ShowProductReviewsPerStore)
        {
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductReviewsModelKey, product, currentStore);
			_logger.LogError("PrepareProductReviewOverviewModelAsync(product={Product}): cacheKey → {CacheKey}", product, cacheKey);

            productReview = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var productReviews = await _productReviewService.GetAllProductReviewsAsync(productId: product.Id, approved: true, storeId: currentStore.Id);
				_logger.LogError("PrepareProductReviewOverviewModelAsync(product={Product}): productReviews → {ProductReviews}", product, productReviews);

                return new ProductReviewOverviewModel
                {
                    RatingSum = productReviews.Sum(pr => pr.Rating),
                    TotalReviews = productReviews.Count
                };
            });
        }
        else
        {
            productReview = new ProductReviewOverviewModel
            {
                RatingSum = product.ApprovedRatingSum,
                TotalReviews = product.ApprovedTotalReviews
            };
        }

        if (productReview != null)
        {
            productReview.ProductId = product.Id;
			_logger.LogDebug("PrepareProductReviewOverviewModelAsync(product={Product}): productReview.ProductId → {Arg1}", product, productReview.ProductId);
			_logger.LogDebug("PrepareProductReviewOverviewModelAsync(product={Product}): productReview.AllowCustomerReviews → {Arg1}", product, productReview.AllowCustomerReviews);
            productReview.AllowCustomerReviews = product.AllowCustomerReviews;
            productReview.CanCurrentCustomerLeaveReview = _catalogSettings.AllowAnonymousUsersToReviewProduct || !await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync());
            productReview.CanAddNewReview = await _productReviewService.CanAddReviewAsync(product.Id, _catalogSettings.ShowProductReviewsPerStore ? currentStore.Id : 0);
        }

		_logger.LogError("PrepareProductReviewOverviewModelAsync(product)={Product}: external service call took {Arg1} ms", product, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting PrepareProductReviewOverviewModelAsync(product={Product})", product);
        return productReview;
    }

    /// <summary>
    /// Prepare the product overview picture model
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="productThumbPictureSize">Product thumb picture size (longest side); pass null to use the default value of media settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains picture models
    /// </returns>
    protected virtual async Task<IList<PictureModel>> PrepareProductOverviewPicturesModelAsync(Product product, int? productThumbPictureSize = null)
    {
		_logger.LogError(">>> Entering PrepareProductOverviewPicturesModelAsync(product={Product},productThumbPictureSize={ProductThumbPictureSize})", product, productThumbPictureSize);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(product);

        var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);
		_logger.LogError("PrepareProductOverviewPicturesModelAsync(product={Product},productThumbPictureSize={ProductThumbPictureSize}): productName → {ProductName}", product, productThumbPictureSize, productName);
        //If a size has been set in the view, we use it in priority
        var pictureSize = productThumbPictureSize ?? _mediaSettings.ProductThumbPictureSize;

        //prepare picture model
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductOverviewPicturesModelKey,
            product, pictureSize, true, _catalogSettings.DisplayAllPicturesOnCatalogPages, await _workContext.GetWorkingLanguageAsync(),
            _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());

        var cachedPictures = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            async Task<PictureModel> preparePictureModelAsync(Picture picture)
            {
				_logger.LogError(">>> Entering preparePictureModelAsync(picture={Picture})", picture);
                //we have to keep the url generation order "full size -> preview" because picture can be updated twice
                //this section of code requires detailed analysis in the future
                (var fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                (var imageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);

				_logger.LogWarning("<<< Exiting preparePictureModelAsync(picture={Picture})", picture);
                return new PictureModel
                {
                    ImageUrl = imageUrl,
                    FullSizeImageUrl = fullSizeImageUrl,
                    //"title" attribute
                    Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute))
                        ? picture.TitleAttribute
                        : string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat"),
                            productName),
                    //"alt" attribute
                    AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute))
                        ? picture.AltAttribute
                        : string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat"),
                            productName)
                };
            }

            //all pictures
            var pictures = (await _pictureService
                    .GetPicturesByProductIdAsync(product.Id, _catalogSettings.DisplayAllPicturesOnCatalogPages ? 0 : 1))
                .DefaultIfEmpty(null);
            var pictureModels = await pictures
                .SelectAwait(async picture => await preparePictureModelAsync(picture))
                .ToListAsync();
				var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
				_logger.LogError("PrepareProductOverviewPicturesModelAsync(product,productThumbPictureSize)={Product},{ProductThumbPictureSize}: database query executed in {Arg2} ms", product, productThumbPictureSize, __logStartDb.ElapsedMilliseconds);
            return pictureModels;
        });

		_logger.LogWarning("<<< Exiting PrepareProductOverviewPicturesModelAsync(product={Product},productThumbPictureSize={ProductThumbPictureSize})", product, productThumbPictureSize);
        return cachedPictures;
    }

    /// <summary>
    /// Prepare the product breadcrumb model
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product breadcrumb model
    /// </returns>
    protected virtual async Task<ProductDetailsModel.ProductBreadcrumbModel> PrepareProductBreadcrumbModelAsync(Product product)
    {
		_logger.LogError(">>> Entering PrepareProductBreadcrumbModelAsync(product={Product})", product);
        ArgumentNullException.ThrowIfNull(product);

        var breadcrumbModel = new ProductDetailsModel.ProductBreadcrumbModel
        {
            Enabled = _catalogSettings.CategoryBreadcrumbEnabled,
            ProductId = product.Id,
            ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
            ProductSeName = await _urlRecordService.GetSeNameAsync(product)
        };
        var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id);
		_logger.LogError("PrepareProductBreadcrumbModelAsync(product={Product}): productCategories → {ProductCategories}", product, productCategories);
        if (!productCategories.Any())
            return breadcrumbModel;

        var category = await _categoryService.GetCategoryByIdAsync(productCategories[0].CategoryId);
		_logger.LogError("PrepareProductBreadcrumbModelAsync(product={Product}): category → {Category}", product, category);
        if (category == null)
            return breadcrumbModel;

        foreach (var catBr in await _categoryService.GetCategoryBreadCrumbAsync(category))
        {
            breadcrumbModel.CategoryBreadcrumb.Add(new CategorySimpleModel
            {
                Id = catBr.Id,
                Name = await _localizationService.GetLocalizedAsync(catBr, x => x.Name),
                SeName = await _urlRecordService.GetSeNameAsync(catBr),
            });
        }

        if (_seoSettings.MicrodataEnabled)
        {
            var jsonLdModel = await _jsonLdModelFactory.PrepareJsonLdProductBreadcrumbAsync(breadcrumbModel);
			_logger.LogError("PrepareProductBreadcrumbModelAsync(product={Product}): jsonLdModel → {JsonLdModel}", product, jsonLdModel);
            breadcrumbModel.JsonLd = JsonConvert
                .SerializeObject(jsonLdModel, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
				_logger.LogDebug("PrepareProductBreadcrumbModelAsync(product={Product}): breadcrumbModel.JsonLd → {Arg1}", product, breadcrumbModel.JsonLd);
        }

		_logger.LogWarning("<<< Exiting PrepareProductBreadcrumbModelAsync(product={Product})", product);
        return breadcrumbModel;
    }

    /// <summary>
    /// Prepare the product tag models
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of product tag model
    /// </returns>
    protected virtual async Task<IList<ProductTagModel>> PrepareProductTagModelsAsync(Product product)
    {
		_logger.LogError(">>> Entering PrepareProductTagModelsAsync(product={Product})", product);
        ArgumentNullException.ThrowIfNull(product);

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareProductTagModelsAsync(product={Product}): store → {Store}", product, store);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var productsTags = await _productTagService.GetAllProductTagsByProductIdAsync(product.Id);
		_logger.LogError("PrepareProductTagModelsAsync(product={Product}): productsTags → {ProductsTags}", product, productsTags);

        var model = await productsTags
            //filter by store
            .WhereAwait(async x => await _productTagService.GetProductCountByProductTagIdAsync(x.Id, store.Id) > 0)
            .SelectAwait(async x => new ProductTagModel
            {
                Id = x.Id,
                Name = await _localizationService.GetLocalizedAsync(x, y => y.Name),
                SeName = await _urlRecordService.GetSeNameAsync(x),
                ProductCount = await _productTagService.GetProductCountByProductTagIdAsync(x.Id, store.Id)
            }).ToListAsync();

		_logger.LogDebug("PrepareProductTagModelsAsync(product={Product}): ProductCount → {ProductCount}", product, ProductCount);
		_logger.LogError("PrepareProductTagModelsAsync(product)={Product}: database query executed in {Arg1} ms", product, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting PrepareProductTagModelsAsync(product={Product})", product);
        return model;
    }

    /// <summary>
    /// Prepare the product to wishlist model
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product add to wishlist model
    /// </returns>
    protected virtual async Task<ProductToWishlistModel> PrepareProductToWishlistModelAsync(Product product)
    {
		_logger.LogError(">>> Entering PrepareProductToWishlistModelAsync(product={Product})", product);
        ArgumentNullException.ThrowIfNull(product);

        var model = new ProductToWishlistModel
        {
            ProductId = product.Id
        };
        //custom wishlists
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareProductToWishlistModelAsync(product={Product}): currentCustomer → {CurrentCustomer}", product, currentCustomer);
        var currentWishlists = await _customWishlistService.GetAllCustomWishlistsAsync(currentCustomer.Id);
		_logger.LogError("PrepareProductToWishlistModelAsync(product={Product}): currentWishlists → {CurrentWishlists}", product, currentWishlists);
        foreach (var wishlist in currentWishlists)
        {
            var customWishlistModel = new CustomWishlistModel
            {
                Id = wishlist.Id,
                Name = wishlist.Name
            };
            model.CustomWishlistItems.Add(customWishlistModel);
        }
		_logger.LogWarning("<<< Exiting PrepareProductToWishlistModelAsync(product={Product})", product);
        return model;
    }

    /// <summary>
    /// Prepare the product add to cart model
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="updatecartitem">Updated shopping cart item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product add to cart model
    /// </returns>
    protected virtual async Task<ProductDetailsModel.AddToCartModel> PrepareProductAddToCartModelAsync(Product product, ShoppingCartItem updatecartitem)
    {
		_logger.LogError(">>> Entering PrepareProductAddToCartModelAsync(product={Product},updatecartitem={Updatecartitem})", product, updatecartitem);
        ArgumentNullException.ThrowIfNull(product);

        var model = new ProductDetailsModel.AddToCartModel
        {
            ProductId = product.Id
        };

        if (updatecartitem != null)
        {
            model.UpdatedShoppingCartItemId = updatecartitem.Id;
			_logger.LogDebug("PrepareProductAddToCartModelAsync(product={Product},updatecartitem={Updatecartitem}): model.UpdatedShoppingCartItemId → {Arg2}", product, updatecartitem, model.UpdatedShoppingCartItemId);
			_logger.LogDebug("PrepareProductAddToCartModelAsync(product={Product},updatecartitem={Updatecartitem}): model.UpdateShoppingCartItemType → {Arg2}", product, updatecartitem, model.UpdateShoppingCartItemType);
            model.UpdateShoppingCartItemType = updatecartitem.ShoppingCartType;
        }

        //quantity
        model.EnteredQuantity = updatecartitem != null ? updatecartitem.Quantity : product.OrderMinimumQuantity;
        //allowed quantities
        var allowedQuantities = _productService.ParseAllowedQuantities(product);
		_logger.LogError("PrepareProductAddToCartModelAsync(product={Product},updatecartitem={Updatecartitem}): allowedQuantities → {AllowedQuantities}", product, updatecartitem, allowedQuantities);
        foreach (var qty in allowedQuantities)
        {
            model.AllowedQuantities.Add(new SelectListItem
            {
                Text = qty.ToString(),
                Value = qty.ToString(),
                Selected = updatecartitem != null && updatecartitem.Quantity == qty
            });
        }
        //minimum quantity notification
        if (product.OrderMinimumQuantity > 1)
        {
            model.MinimumQuantityNotification = string.Format(await _localizationService.GetResourceAsync("Products.MinimumQuantityNotification"), product.OrderMinimumQuantity);
        }

        //'add to cart', 'add to wishlist' buttons
        model.DisableBuyButton = product.DisableBuyButton || !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART);
        model.DisableWishlistButton = product.DisableWishlistButton || !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_WISHLIST);
        if (!await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES))
        {
            model.DisableBuyButton = true;
			_logger.LogDebug("PrepareProductAddToCartModelAsync(product={Product},updatecartitem={Updatecartitem}): model.DisableBuyButton → {Arg2}", product, updatecartitem, model.DisableBuyButton);
			_logger.LogDebug("PrepareProductAddToCartModelAsync(product={Product},updatecartitem={Updatecartitem}): model.DisableWishlistButton → {Arg2}", product, updatecartitem, model.DisableWishlistButton);
            model.DisableWishlistButton = true;
        }

        //custom wishlist items
        model.ProductToWishlist = await PrepareProductToWishlistModelAsync(product);

        //pre-order
        if (product.AvailableForPreOrder)
        {
            model.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                                         product.PreOrderAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
										_logger.LogDebug("PrepareProductAddToCartModelAsync(product={Product},updatecartitem={Updatecartitem}): model.AvailableForPreOrder → {Arg2}", product, updatecartitem, model.AvailableForPreOrder);
										_logger.LogDebug("PrepareProductAddToCartModelAsync(product={Product},updatecartitem={Updatecartitem}): model.PreOrderAvailabilityStartDateTimeUtc → {Arg2}", product, updatecartitem, model.PreOrderAvailabilityStartDateTimeUtc);
            model.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;

            if (model.AvailableForPreOrder &&
                model.PreOrderAvailabilityStartDateTimeUtc.HasValue &&
                _catalogSettings.DisplayDatePreOrderAvailability)
            {
                model.PreOrderAvailabilityStartDateTimeUserTime =
                    (await _dateTimeHelper.ConvertToUserTimeAsync(model.PreOrderAvailabilityStartDateTimeUtc.Value)).ToString("D");
            }
        }
        //rental
        model.IsRental = product.IsRental;
		_logger.LogDebug("PrepareProductAddToCartModelAsync(product={Product},updatecartitem={Updatecartitem}): model.IsRental → {Arg2}", product, updatecartitem, model.IsRental);

        //customer entered price
        model.CustomerEntersPrice = product.CustomerEntersPrice;
		_logger.LogDebug("PrepareProductAddToCartModelAsync(product={Product},updatecartitem={Updatecartitem}): model.CustomerEntersPrice → {Arg2}", product, updatecartitem, model.CustomerEntersPrice);
        if (!model.CustomerEntersPrice)
            return model;

        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
		_logger.LogError("PrepareProductAddToCartModelAsync(product={Product},updatecartitem={Updatecartitem}): currentCurrency → {CurrentCurrency}", product, updatecartitem, currentCurrency);
        var minimumCustomerEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(product.MinimumCustomerEnteredPrice, currentCurrency);
		_logger.LogError("PrepareProductAddToCartModelAsync(product={Product},updatecartitem={Updatecartitem}): minimumCustomerEnteredPrice → {MinimumCustomerEnteredPrice}", product, updatecartitem, minimumCustomerEnteredPrice);
        var maximumCustomerEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(product.MaximumCustomerEnteredPrice, currentCurrency);
		_logger.LogError("PrepareProductAddToCartModelAsync(product={Product},updatecartitem={Updatecartitem}): maximumCustomerEnteredPrice → {MaximumCustomerEnteredPrice}", product, updatecartitem, maximumCustomerEnteredPrice);

        model.CustomerEnteredPrice = updatecartitem != null ? updatecartitem.CustomerEnteredPrice : minimumCustomerEnteredPrice;
        model.CustomerEnteredPriceRange = string.Format(await _localizationService.GetResourceAsync("Products.EnterProductPrice.Range"),
            await _priceFormatter.FormatPriceAsync(minimumCustomerEnteredPrice, false, false),
            await _priceFormatter.FormatPriceAsync(maximumCustomerEnteredPrice, false, false));

		_logger.LogWarning("<<< Exiting PrepareProductAddToCartModelAsync(product={Product},updatecartitem={Updatecartitem})", product, updatecartitem);
        return model;
    }

    /// <summary>
    /// Prepare the product attribute models
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="updatecartitem">Updated shopping cart item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of product attribute model
    /// </returns>
    protected virtual async Task<IList<ProductDetailsModel.ProductAttributeModel>> PrepareProductAttributeModelsAsync(Product product, ShoppingCartItem updatecartitem)
    {
		_logger.LogError(">>> Entering PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem})", product, updatecartitem);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(product);

        var model = new List<ProductDetailsModel.ProductAttributeModel>();
		_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): model → {Model}", product, updatecartitem, model);
        var store = updatecartitem != null ? await _storeService.GetStoreByIdAsync(updatecartitem.StoreId) : await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): store → {Store}", product, updatecartitem, store);

        var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
		_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): productAttributeMapping → {ProductAttributeMapping}", product, updatecartitem, productAttributeMapping);
        foreach (var attribute in productAttributeMapping)
        {
            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(attribute.ProductAttributeId);
			_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): productAttribute → {ProductAttribute}", product, updatecartitem, productAttribute);

            var attributeModel = new ProductDetailsModel.ProductAttributeModel
            {
                Id = attribute.Id,
                ProductId = product.Id,
                ProductAttributeId = attribute.ProductAttributeId,
                Name = await _localizationService.GetLocalizedAsync(productAttribute, x => x.Name),
                Description = await _localizationService.GetLocalizedAsync(productAttribute, x => x.Description),
                TextPrompt = await _localizationService.GetLocalizedAsync(attribute, x => x.TextPrompt),
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType,
                DefaultValue = updatecartitem != null ? null : await _localizationService.GetLocalizedAsync(attribute, x => x.DefaultValue),
                HasCondition = !string.IsNullOrEmpty(attribute.ConditionAttributeXml)
            };
            if (!string.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
            {
                attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                    .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
					_logger.LogDebug("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): attributeModel.AllowedFileExtensions → {Arg2}", product, updatecartitem, attributeModel.AllowedFileExtensions);
            }

            if (attribute.ShouldHaveValues())
            {
                //values
                var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
				_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): attributeValues → {AttributeValues}", product, updatecartitem, attributeValues);
                foreach (var attributeValue in attributeValues)
                {
                    var valueModel = new ProductDetailsModel.ProductAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                        ColorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
                        IsPreSelected = attributeValue.IsPreSelected,
                        CustomerEntersQty = attributeValue.CustomerEntersQty,
                        Quantity = attributeValue.Quantity
                    };
                    attributeModel.Values.Add(valueModel);

                    //display price if allowed
                    if (await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES))
                    {
                        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
						_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): currentCustomer → {CurrentCustomer}", product, updatecartitem, currentCustomer);
                        var customer = updatecartitem?.CustomerId is null ? currentCustomer : await _customerService.GetCustomerByIdAsync(updatecartitem.CustomerId);
						_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): customer → {Customer}", product, updatecartitem, customer);

                        var attributeValuePriceAdjustment = await _priceCalculationService.GetProductAttributeValuePriceAdjustmentAsync(product, attributeValue, customer, store, quantity: updatecartitem?.Quantity ?? 1);
						_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): attributeValuePriceAdjustment → {AttributeValuePriceAdjustment}", product, updatecartitem, attributeValuePriceAdjustment);
                        var (priceAdjustmentBase, _) = await _taxService.GetProductPriceAsync(product, attributeValuePriceAdjustment);
                        var priceAdjustment = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(priceAdjustmentBase, await _workContext.GetWorkingCurrencyAsync());
						_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): priceAdjustment → {PriceAdjustment}", product, updatecartitem, priceAdjustment);

                        if (attributeValue.PriceAdjustmentUsePercentage)
                        {
                            var priceAdjustmentStr = attributeValue.PriceAdjustment.ToString("G29");
							_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): priceAdjustmentStr → {PriceAdjustmentStr}", product, updatecartitem, priceAdjustmentStr);
                            if (attributeValue.PriceAdjustment > decimal.Zero)
                                valueModel.PriceAdjustment = "+";
								_logger.LogDebug("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): valueModel.PriceAdjustment → {Arg2}", product, updatecartitem, valueModel.PriceAdjustment);
                            valueModel.PriceAdjustment += priceAdjustmentStr + "%";
                        }
                        else
                        {
                            if (priceAdjustmentBase > decimal.Zero)
                                valueModel.PriceAdjustment = "+" + await _priceFormatter.FormatPriceAsync(priceAdjustment, false, false);
                            else if (priceAdjustmentBase < decimal.Zero)
                                valueModel.PriceAdjustment = "-" + await _priceFormatter.FormatPriceAsync(-priceAdjustment, false, false);
                        }

                        valueModel.PriceAdjustmentValue = priceAdjustment;
						_logger.LogDebug("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): valueModel.PriceAdjustmentValue → {Arg2}", product, updatecartitem, valueModel.PriceAdjustmentValue);
                    }

                    //"image square" picture (with with "image squares" attribute type only)
                    if (attributeValue.ImageSquaresPictureId > 0)
                    {
                        var productAttributeImageSquarePictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductAttributeImageSquarePictureModelKey
                            , attributeValue.ImageSquaresPictureId,
                            _webHelper.IsCurrentConnectionSecured(),
                            await _storeContext.GetCurrentStoreAsync());
                        valueModel.ImageSquaresPictureModel = await _staticCacheManager.GetAsync(productAttributeImageSquarePictureCacheKey, async () =>
                        {
                            var imageSquaresPicture = await _pictureService.GetPictureByIdAsync(attributeValue.ImageSquaresPictureId);
							_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): imageSquaresPicture → {ImageSquaresPicture}", product, updatecartitem, imageSquaresPicture);

                            (var fullSizeImageUrl, imageSquaresPicture) = await _pictureService.GetPictureUrlAsync(imageSquaresPicture);
                            (var imageUrl, imageSquaresPicture) = await _pictureService.GetPictureUrlAsync(imageSquaresPicture, _mediaSettings.ImageSquarePictureSize);

                            if (imageSquaresPicture != null)
                            {

                                return new PictureModel
                                {
                                    FullSizeImageUrl = fullSizeImageUrl,
                                    ImageUrl = imageUrl
                                };
                            }

                            return new PictureModel();
                        });
                    }

                    //picture of a product attribute value
                    valueModel.PictureId = (await _productAttributeService.GetProductAttributeValuePicturesAsync(attributeValue.Id)).FirstOrDefault()?.PictureId ?? 0;
                }
            }

            //set already selected attributes (if we're going to update the existing shopping cart item)
            if (updatecartitem != null)
            {
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    {
                        if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                        {
                            //clear default selection
                            foreach (var item in attributeModel.Values)
                                item.IsPreSelected = false;

                            //select new values
                            var selectedValues = await _productAttributeParser.ParseProductAttributeValuesAsync(updatecartitem.AttributesXml);
							_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): selectedValues → {SelectedValues}", product, updatecartitem, selectedValues);
                            foreach (var attributeValue in selectedValues)
                                foreach (var item in attributeModel.Values)
                                    if (attributeValue.Id == item.Id)
                                    {
                                        item.IsPreSelected = true;

                                        //set customer entered quantity
                                        if (attributeValue.CustomerEntersQty)
                                            item.Quantity = attributeValue.Quantity;
                                    }
                        }
                    }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                    {
                        //values are already pre-set

                        //set customer entered quantity
                        if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                        {
                            foreach (var attributeValue in (await _productAttributeParser.ParseProductAttributeValuesAsync(updatecartitem.AttributesXml))
                                     .Where(value => value.CustomerEntersQty))
                            {
                                var item = attributeModel.Values.FirstOrDefault(value => value.Id == attributeValue.Id);
								_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): item → {Item}", product, updatecartitem, item);
                                if (item != null)
                                    item.Quantity = attributeValue.Quantity;
                            }
                        }
                    }

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                    {
                        if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                        {
                            var enteredText = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
							_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): enteredText → {EnteredText}", product, updatecartitem, enteredText);
                            if (enteredText.Any())
                                attributeModel.DefaultValue = enteredText[0];
								_logger.LogDebug("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): attributeModel.DefaultValue → {Arg2}", product, updatecartitem, attributeModel.DefaultValue);
                        }
                    }

                        break;
                    case AttributeControlType.Datepicker:
                    {
                        //keep in mind my that the code below works only in the current culture
                        var selectedDateStr = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
						_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): selectedDateStr → {SelectedDateStr}", product, updatecartitem, selectedDateStr);
                        if (selectedDateStr.Any())
                        {
                            if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture, DateTimeStyles.None, out var selectedDate))
                            {
                                //successfully parsed
                                attributeModel.SelectedDay = selectedDate.Day;
								_logger.LogDebug("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): attributeModel.SelectedDay → {Arg2}", product, updatecartitem, attributeModel.SelectedDay);
								_logger.LogDebug("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): attributeModel.SelectedMonth → {Arg2}", product, updatecartitem, attributeModel.SelectedMonth);
								_logger.LogDebug("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): attributeModel.SelectedYear → {Arg2}", product, updatecartitem, attributeModel.SelectedYear);
                                attributeModel.SelectedMonth = selectedDate.Month;
                                attributeModel.SelectedYear = selectedDate.Year;
                            }
                        }
                    }

                        break;
                    case AttributeControlType.FileUpload:
                    {
                        if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                        {
                            var downloadGuidStr = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id).FirstOrDefault();
							_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): downloadGuidStr → {DownloadGuidStr}", product, updatecartitem, downloadGuidStr);
                            _ = Guid.TryParse(downloadGuidStr, out var downloadGuid);
							_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): _ → {_}", product, updatecartitem, _);
                            var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
							_logger.LogError("PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem}): download → {Download}", product, updatecartitem, download);
                            if (download != null)
                                attributeModel.DefaultValue = download.DownloadGuid.ToString();
                        }
                    }

                        break;
                    default:
                        break;
                }
            }

            model.Add(attributeModel);
        }

		_logger.LogError("PrepareProductAttributeModelsAsync(product,updatecartitem)={Product},{Updatecartitem}: external service call took {Arg2} ms", product, updatecartitem, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting PrepareProductAttributeModelsAsync(product={Product},updatecartitem={Updatecartitem})", product, updatecartitem);
        return model;
    }

    /// <summary>
    /// Prepare the product tier price models
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of tier price model
    /// </returns>
    protected virtual async Task<IList<ProductDetailsModel.TierPriceModel>> PrepareProductTierPriceModelsAsync(Product product)
    {
		_logger.LogError(">>> Entering PrepareProductTierPriceModelsAsync(product={Product})", product);
        ArgumentNullException.ThrowIfNull(product);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareProductTierPriceModelsAsync(product={Product}): customer → {Customer}", product, customer);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var store = await _storeContext.GetCurrentStoreAsync();
        var model = await (await _productService.GetTierPricesAsync(product, customer, store))
            .SelectAwait(async tierPrice =>
            {
				_logger.LogError("PrepareProductTierPriceModelsAsync(product={Product}): store → {Store}", product, store);
                var priceBase = (await _taxService.GetProductPriceAsync(product, (await _priceCalculationService.GetFinalPriceAsync(product,
                    customer, store, decimal.Zero, _catalogSettings.DisplayTierPricesWithDiscounts,
                    tierPrice.Quantity)).finalPrice)).price;

                var price = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(priceBase, await _workContext.GetWorkingCurrencyAsync());
				_logger.LogError("PrepareProductTierPriceModelsAsync(product={Product}): price → {Price}", product, price);

                return new ProductDetailsModel.TierPriceModel
                {
                    Quantity = tierPrice.Quantity,
                    Price = await _priceFormatter.FormatPriceAsync(price, false, false),
                    PriceValue = price
                };
            }).ToListAsync();
			_logger.LogError("PrepareProductTierPriceModelsAsync(product)={Product}: database query executed in {Arg1} ms", product, __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting PrepareProductTierPriceModelsAsync(product={Product})", product);
        return model;
    }

    /// <summary>
    /// Prepare the product manufacturer models
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of manufacturer brief info model
    /// </returns>
    protected virtual async Task<IList<ManufacturerBriefInfoModel>> PrepareProductManufacturerModelsAsync(Product product)
    {
		_logger.LogError(">>> Entering PrepareProductManufacturerModelsAsync(product={Product})", product);
        ArgumentNullException.ThrowIfNull(product);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var model = await (await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id))
            .SelectAwait(async pm =>
            {
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(pm.ManufacturerId);
				_logger.LogError("PrepareProductManufacturerModelsAsync(product={Product}): manufacturer → {Manufacturer}", product, manufacturer);
                var modelMan = new ManufacturerBriefInfoModel
                {
                    Id = manufacturer.Id,
                    Name = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name),
                    SeName = await _urlRecordService.GetSeNameAsync(manufacturer)
                };

                return modelMan;
            }).ToListAsync();
			_logger.LogError("PrepareProductManufacturerModelsAsync(product)={Product}: database query executed in {Arg1} ms", product, __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting PrepareProductManufacturerModelsAsync(product={Product})", product);
        return model;
    }

    /// <summary>
    /// Prepare the product details picture model
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="isAssociatedProduct">Whether the product is associated</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture model for the default picture; All picture models
    /// </returns>
    protected virtual async Task<(PictureModel pictureModel, IList<PictureModel> allPictureModels, IList<VideoModel> allVideoModels)> PrepareProductDetailsPictureModelAsync(Product product, bool isAssociatedProduct)
    {
        ArgumentNullException.ThrowIfNull(product);

        //default picture size
        var defaultPictureSize = isAssociatedProduct ?
            _mediaSettings.AssociatedProductPictureSize :
            _mediaSettings.ProductDetailsPictureSize;

        //prepare picture models
        var productPicturesCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductDetailsPicturesModelKey
            , product, defaultPictureSize, isAssociatedProduct,
            await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());
        var cachedPictures = await _staticCacheManager.GetAsync(productPicturesCacheKey, async () =>
        {
            var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);

            var pictures = await _pictureService.GetPicturesByProductIdAsync(product.Id);
            var defaultPicture = pictures.FirstOrDefault();

            (var fullSizeImageUrl, defaultPicture) = await _pictureService.GetPictureUrlAsync(defaultPicture, 0, !isAssociatedProduct);
            (var imageUrl, defaultPicture) = await _pictureService.GetPictureUrlAsync(defaultPicture, defaultPictureSize, !isAssociatedProduct);

            var defaultPictureModel = new PictureModel
            {
                ImageUrl = imageUrl,
                FullSizeImageUrl = fullSizeImageUrl,
                //"title" attribute
                Title = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.TitleAttribute)) ?
                    defaultPicture.TitleAttribute :
                    string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat.Details"), productName),
                //"alt" attribute
                AlternateText = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.AltAttribute)) ?
                    defaultPicture.AltAttribute :
                    string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat.Details"), productName)
            };

            //all pictures
            var pictureModels = new List<PictureModel>();
            for (var i = 0; i < pictures.Count; i++)
            {
                var picture = pictures[i];

                (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                (imageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture, defaultPictureSize, !isAssociatedProduct);
                (var thumbImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage);

                var pictureModel = new PictureModel
                {
                    Id = picture.Id,
                    ImageUrl = imageUrl,
                    ThumbImageUrl = thumbImageUrl,
                    FullSizeImageUrl = fullSizeImageUrl,
                    Title = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat.Details"), productName),
                    AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat.Details"), productName),
                };
                //"title" attribute
                pictureModel.Title = !string.IsNullOrEmpty(picture.TitleAttribute) ?
                    picture.TitleAttribute :
                    string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat.Details"), productName);
                //"alt" attribute
                pictureModel.AlternateText = !string.IsNullOrEmpty(picture.AltAttribute) ?
                    picture.AltAttribute :
                    string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat.Details"), productName);

                pictureModels.Add(pictureModel);
            }

            return new { DefaultPictureModel = defaultPictureModel, PictureModels = pictureModels };
        });

        var allPictureModels = cachedPictures.PictureModels;

        //all videos
        var allvideoModels = new List<VideoModel>();
        var videos = await _videoService.GetVideosByProductIdAsync(product.Id);
        foreach (var video in videos)
        {
            var videoModel = new VideoModel
            {
                VideoUrl = video.VideoUrl,
                Allow = _mediaSettings.VideoIframeAllow,
                Width = _mediaSettings.VideoIframeWidth,
                Height = _mediaSettings.VideoIframeHeight
            };

            allvideoModels.Add(videoModel);
        }
        return (cachedPictures.DefaultPictureModel, allPictureModels, allvideoModels);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get the product template view path
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view path
    /// </returns>
    public virtual async Task<string> PrepareProductTemplateViewPathAsync(Product product)
    {
		_logger.LogError(">>> Entering PrepareProductTemplateViewPathAsync(product={Product})", product);
        ArgumentNullException.ThrowIfNull(product);

        var template = (await _productTemplateService.GetProductTemplateByIdAsync(product.ProductTemplateId) ??
                        (await _productTemplateService.GetAllProductTemplatesAsync()).FirstOrDefault()) ?? throw new Exception("No default template could be loaded");

		_logger.LogWarning("<<< Exiting PrepareProductTemplateViewPathAsync(product={Product})", product);
        return template.ViewPath;
    }

    /// <summary>
    /// Prepare the product overview models
    /// </summary>
    /// <param name="products">Collection of products</param>
    /// <param name="preparePriceModel">Whether to prepare the price model</param>
    /// <param name="preparePictureModel">Whether to prepare the picture model</param>
    /// <param name="productThumbPictureSize">Product thumb picture size (longest side); pass null to use the default value of media settings</param>
    /// <param name="prepareSpecificationAttributes">Whether to prepare the specification attribute models</param>
    /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the collection of product overview model
    /// </returns>
    public virtual async Task<IEnumerable<ProductOverviewModel>> PrepareProductOverviewModelsAsync(IEnumerable<Product> products,
        bool preparePriceModel = true, bool preparePictureModel = true,
        int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
        bool forceRedirectionAfterAddingToCart = false)
    {
		_logger.LogError(">>> Entering PrepareProductOverviewModelsAsync(products={Products},preparePriceModel={PreparePriceModel},preparePictureModel={PreparePictureModel},productThumbPictureSize={ProductThumbPictureSize},prepareSpecificationAttributes={PrepareSpecificationAttributes},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart})", products, preparePriceModel, preparePictureModel, productThumbPictureSize, prepareSpecificationAttributes, forceRedirectionAfterAddingToCart);
        ArgumentNullException.ThrowIfNull(products);

        var models = new List<ProductOverviewModel>();
		_logger.LogError("PrepareProductOverviewModelsAsync(products={Products},preparePriceModel={PreparePriceModel},preparePictureModel={PreparePictureModel},productThumbPictureSize={ProductThumbPictureSize},prepareSpecificationAttributes={PrepareSpecificationAttributes},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart}): models → {Models}", products, preparePriceModel, preparePictureModel, productThumbPictureSize, prepareSpecificationAttributes, forceRedirectionAfterAddingToCart, models);
        foreach (var product in products)
        {
            var model = new ProductOverviewModel
            {
                Id = product.Id,
                Name = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                ShortDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription),
                FullDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription),
                SeName = await _urlRecordService.GetSeNameAsync(product),
                Sku = product.Sku,
                ProductType = product.ProductType,
                MarkAsNew = product.MarkAsNew &&
                            (!product.MarkAsNewStartDateTimeUtc.HasValue || product.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) &&
                            (!product.MarkAsNewEndDateTimeUtc.HasValue || product.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow)
            };

            //price
            if (preparePriceModel)
            {
                model.ProductPrice = await PrepareProductPriceModelAsync(product, true, forceRedirectionAfterAddingToCart);
            }

            //picture
            if (preparePictureModel)
            {
                model.PictureModels = await PrepareProductOverviewPicturesModelAsync(product, productThumbPictureSize);
            }

            //specs
            if (prepareSpecificationAttributes)
            {
                model.ProductSpecificationModel = await PrepareProductSpecificationModelAsync(product);
            }

            //reviews
            model.ReviewOverviewModel = await PrepareProductReviewOverviewModelAsync(product);

            //custom wishlist items
            model.ProductToWishlist = await PrepareProductToWishlistModelAsync(product);

            models.Add(model);
        }

		_logger.LogWarning("<<< Exiting PrepareProductOverviewModelsAsync(products={Products},preparePriceModel={PreparePriceModel},preparePictureModel={PreparePictureModel},productThumbPictureSize={ProductThumbPictureSize},prepareSpecificationAttributes={PrepareSpecificationAttributes},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart})", products, preparePriceModel, preparePictureModel, productThumbPictureSize, prepareSpecificationAttributes, forceRedirectionAfterAddingToCart);
        return models;
    }

    /// <summary>
    /// Prepare the product combination models
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product combination models
    /// </returns>
    public virtual async Task<IList<ProductCombinationModel>> PrepareProductCombinationModelsAsync(Product product)
    {
		_logger.LogError(">>> Entering PrepareProductCombinationModelsAsync(product={Product})", product);
        ArgumentNullException.ThrowIfNull(product);

        var result = new List<ProductCombinationModel>();
		_logger.LogError("PrepareProductCombinationModelsAsync(product={Product}): result → {Result}", product, result);

        var combinations = await _productAttributeService
            .GetAllProductAttributeCombinationsAsync(product.Id);
        if (combinations?.Any() == true)
        {
            foreach (var combination in combinations)
            {
                var combinationModel = new ProductCombinationModel
                {
                    InStock = combination.StockQuantity > 0 || combination.AllowOutOfStockOrders
                };

                var mappings = await _productAttributeParser
                    .ParseProductAttributeMappingsAsync(combination.AttributesXml);
                if (mappings == null || !mappings.Any())
                    continue;

                foreach (var mapping in mappings)
                {
                    var attributeModel = new ProductAttributeModel
                    {
                        Id = mapping.Id
                    };

                    var values = await _productAttributeParser
                        .ParseProductAttributeValuesAsync(combination.AttributesXml, mapping.Id);
                    if (values == null || !values.Any())
                        continue;

                    foreach (var value in values)
                        attributeModel.ValueIds.Add(value.Id);

                    combinationModel.Attributes.Add(attributeModel);
                }

                result.Add(combinationModel);
            }
        }

		_logger.LogWarning("<<< Exiting PrepareProductCombinationModelsAsync(product={Product})", product);
        return result;
    }

    /// <summary>
    /// Prepare the product details model
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="updatecartitem">Updated shopping cart item</param>
    /// <param name="isAssociatedProduct">Whether the product is associated</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product details model
    /// </returns>
    public virtual async Task<ProductDetailsModel> PrepareProductDetailsModelAsync(Product product,
        ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false)
    {
		_logger.LogError(">>> Entering PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct})", product, updatecartitem, isAssociatedProduct);
        ArgumentNullException.ThrowIfNull(product);

        //standard properties
        var model = new ProductDetailsModel
        {
            Id = product.Id,
            Name = await _localizationService.GetLocalizedAsync(product, x => x.Name),
            ShortDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription),
            FullDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription),
            MetaKeywords = await _localizationService.GetLocalizedAsync(product, x => x.MetaKeywords),
            MetaDescription = await _localizationService.GetLocalizedAsync(product, x => x.MetaDescription),
            MetaTitle = await _localizationService.GetLocalizedAsync(product, x => x.MetaTitle),
            SeName = await _urlRecordService.GetSeNameAsync(product),
            ProductType = product.ProductType,
            ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage,
            Sku = product.Sku,
            ShowManufacturerPartNumber = _catalogSettings.ShowManufacturerPartNumber,
            FreeShippingNotificationEnabled = _catalogSettings.ShowFreeShippingNotification,
            ManufacturerPartNumber = product.ManufacturerPartNumber,
            ShowGtin = _catalogSettings.ShowGtin,
            Gtin = product.Gtin,
            ManageInventoryMethod = product.ManageInventoryMethod,
            StockAvailability = await _productService.FormatStockMessageAsync(product, string.Empty),
            HasSampleDownload = product.IsDownload && product.HasSampleDownload,
            DisplayDiscontinuedMessage = !product.Published && _catalogSettings.DisplayDiscontinuedMessageForUnpublishedProducts,
            AvailableEndDate = product.AvailableEndDateTimeUtc,
            VisibleIndividually = product.VisibleIndividually,
            AllowAddingOnlyExistingAttributeCombinations = product.AllowAddingOnlyExistingAttributeCombinations,
            DisplayAttributeCombinationImagesOnly = product.DisplayAttributeCombinationImagesOnly
        };

        //automatically generate product description?
        if (_seoSettings.GenerateProductMetaDescription && string.IsNullOrEmpty(model.MetaDescription))
        {
            //based on short description
            model.MetaDescription = model.ShortDescription;
			_logger.LogDebug("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): model.MetaDescription → {Arg3}", product, updatecartitem, isAssociatedProduct, model.MetaDescription);
        }

        //shipping info
        model.IsShipEnabled = product.IsShipEnabled;
		_logger.LogDebug("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): model.IsShipEnabled → {Arg3}", product, updatecartitem, isAssociatedProduct, model.IsShipEnabled);
        if (product.IsShipEnabled)
        {
            model.IsFreeShipping = product.IsFreeShipping;
			_logger.LogDebug("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): model.IsFreeShipping → {Arg3}", product, updatecartitem, isAssociatedProduct, model.IsFreeShipping);
            //delivery date
            var deliveryDate = await _dateRangeService.GetDeliveryDateByIdAsync(product.DeliveryDateId);
			_logger.LogError("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): deliveryDate → {DeliveryDate}", product, updatecartitem, isAssociatedProduct, deliveryDate);
            if (deliveryDate != null)
            {
                model.DeliveryDate = await _localizationService.GetLocalizedAsync(deliveryDate, dd => dd.Name);
            }
        }

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): store → {Store}", product, updatecartitem, isAssociatedProduct, store);
        //email a friend
        model.EmailAFriendEnabled = _catalogSettings.EmailAFriendEnabled;
		_logger.LogDebug("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): model.EmailAFriendEnabled → {Arg3}", product, updatecartitem, isAssociatedProduct, model.EmailAFriendEnabled);
        //compare products
        model.CompareProductsEnabled = _catalogSettings.CompareProductsEnabled;
		_logger.LogDebug("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): model.CompareProductsEnabled → {Arg3}", product, updatecartitem, isAssociatedProduct, model.CompareProductsEnabled);
        //store name
        model.CurrentStoreName = await _localizationService.GetLocalizedAsync(store, x => x.Name);

        //vendor details
        if (_vendorSettings.ShowVendorOnProductDetailsPage)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(product.VendorId);
			_logger.LogError("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): vendor → {Vendor}", product, updatecartitem, isAssociatedProduct, vendor);
            if (vendor != null && !vendor.Deleted && vendor.Active)
            {
                model.ShowVendor = true;
				_logger.LogDebug("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): model.ShowVendor → {Arg3}", product, updatecartitem, isAssociatedProduct, model.ShowVendor);
				_logger.LogDebug("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): model.VendorModel → {Arg3}", product, updatecartitem, isAssociatedProduct, model.VendorModel);

                model.VendorModel = new VendorBriefInfoModel
                {
                    Id = vendor.Id,
                    Name = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
                    SeName = await _urlRecordService.GetSeNameAsync(vendor),
                };
            }
        }

        //page sharing
        if (_catalogSettings.ShowShareButton && !string.IsNullOrEmpty(_catalogSettings.PageShareCode))
        {
            var shareCode = _catalogSettings.PageShareCode;
			_logger.LogError("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): shareCode → {ShareCode}", product, updatecartitem, isAssociatedProduct, shareCode);
            if (_webHelper.IsCurrentConnectionSecured())
            {
                //need to change the add this link to be https linked when the page is, so that the page doesn't ask about mixed mode when viewed in https...
                shareCode = shareCode.Replace("http://", "https://");
            }

            model.PageShareCode = shareCode;
			_logger.LogDebug("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): model.PageShareCode → {Arg3}", product, updatecartitem, isAssociatedProduct, model.PageShareCode);
        }

        switch (product.ManageInventoryMethod)
        {
            case ManageInventoryMethod.DontManageStock:
                model.InStock = true;
				_logger.LogDebug("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): model.InStock → {Arg3}", product, updatecartitem, isAssociatedProduct, model.InStock);
                break;

            case ManageInventoryMethod.ManageStock:
                model.InStock = product.BackorderMode != BackorderMode.NoBackorders
                                || await _productService.GetTotalStockQuantityAsync(product) > 0;
                model.DisplayBackInStockSubscription = !model.InStock && product.AllowBackInStockSubscriptions;
				_logger.LogDebug("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): model.DisplayBackInStockSubscription → {Arg3}", product, updatecartitem, isAssociatedProduct, model.DisplayBackInStockSubscription);
                break;

            case ManageInventoryMethod.ManageStockByAttributes:
                model.InStock = (await _productAttributeService
                                    .GetAllProductAttributeCombinationsAsync(product.Id))
                                ?.Any(c => c.StockQuantity > 0 || c.AllowOutOfStockOrders)
                                ?? false;
                break;
        }

        //breadcrumb
        //do not prepare this model for the associated products. anyway it's not used
        if (_catalogSettings.CategoryBreadcrumbEnabled && !isAssociatedProduct)
        {
            model.Breadcrumb = await PrepareProductBreadcrumbModelAsync(product);
        }

        //product tags
        //do not prepare this model for the associated products. anyway it's not used
        if (!isAssociatedProduct)
        {
            model.ProductTags = await PrepareProductTagModelsAsync(product);
        }

        //pictures and videos
        model.DefaultPictureZoomEnabled = _mediaSettings.DefaultPictureZoomEnabled;
		_logger.LogDebug("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): model.DefaultPictureZoomEnabled → {Arg3}", product, updatecartitem, isAssociatedProduct, model.DefaultPictureZoomEnabled);
        IList<PictureModel> allPictureModels;
        IList<VideoModel> allVideoModels;
        (model.DefaultPictureModel, allPictureModels, allVideoModels) = await PrepareProductDetailsPictureModelAsync(product, isAssociatedProduct);
        model.PictureModels = allPictureModels;
		_logger.LogDebug("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): model.PictureModels → {Arg3}", product, updatecartitem, isAssociatedProduct, model.PictureModels);
		_logger.LogDebug("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): model.VideoModels → {Arg3}", product, updatecartitem, isAssociatedProduct, model.VideoModels);
        model.VideoModels = allVideoModels;

        //price
        model.ProductPrice = await PrepareProductPriceModelAsync(product);

        //'Add to cart' model
        model.AddToCart = await PrepareProductAddToCartModelAsync(product, updatecartitem);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): customer → {Customer}", product, updatecartitem, isAssociatedProduct, customer);
        //gift card
        if (product.IsGiftCard)
        {
            model.GiftCard.IsGiftCard = true;
            model.GiftCard.GiftCardType = product.GiftCardType;

            if (updatecartitem == null)
            {
                model.GiftCard.SenderName = await _customerService.GetCustomerFullNameAsync(customer);
                model.GiftCard.SenderEmail = customer.Email;
            }
            else
            {
                _productAttributeParser.GetGiftCardAttribute(updatecartitem.AttributesXml,
                    out var giftCardRecipientName, out var giftCardRecipientEmail,
                    out var giftCardSenderName, out var giftCardSenderEmail, out var giftCardMessage);

                model.GiftCard.RecipientName = giftCardRecipientName;
                model.GiftCard.RecipientEmail = giftCardRecipientEmail;
                model.GiftCard.SenderName = giftCardSenderName;
                model.GiftCard.SenderEmail = giftCardSenderEmail;
                model.GiftCard.Message = giftCardMessage;
            }
        }

        //product attributes
        model.ProductAttributes = await PrepareProductAttributeModelsAsync(product, updatecartitem);

        //product specifications
        //do not prepare this model for the associated products. anyway it's not used
        if (!isAssociatedProduct)
        {
            model.ProductSpecificationModel = await PrepareProductSpecificationModelAsync(product);
        }

        //product review overview
        model.ProductReviewOverview = await PrepareProductReviewOverviewModelAsync(product);

        model.ProductReviews = await PrepareProductReviewsModelAsync(product);

        if (await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES))
            model.TierPrices = await PrepareProductTierPriceModelsAsync(product);

        //manufacturers
        model.ProductManufacturers = await PrepareProductManufacturerModelsAsync(product);

        //rental products
        if (product.IsRental)
        {
            model.IsRental = true;
			_logger.LogDebug("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): model.IsRental → {Arg3}", product, updatecartitem, isAssociatedProduct, model.IsRental);
            //set already entered dates attributes (if we're going to update the existing shopping cart item)
            if (updatecartitem != null)
            {
                model.RentalStartDate = updatecartitem.RentalStartDateUtc;
				_logger.LogDebug("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): model.RentalStartDate → {Arg3}", product, updatecartitem, isAssociatedProduct, model.RentalStartDate);
				_logger.LogDebug("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): model.RentalEndDate → {Arg3}", product, updatecartitem, isAssociatedProduct, model.RentalEndDate);
                model.RentalEndDate = updatecartitem.RentalEndDateUtc;
            }
			_logger.LogError(">>> Entering PrepareProductSpecificationModelAsync(product={Product})", product);
			_logger.LogDebug("PrepareProductSpecificationModelAsync(product={Product}): Attributes → {Attributes}", product, Attributes);
			_logger.LogWarning("<<< Exiting PrepareProductSpecificationModelAsync(product={Product})", product);
        }

        //estimate shipping
        if (_shippingSettings.EstimateShippingProductPageEnabled && !model.IsFreeShipping)
        {
            var wrappedProduct = new ShoppingCartItem
            {
                StoreId = store.Id,
                ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart,
                CustomerId = customer.Id,
                ProductId = product.Id,
                CreatedOnUtc = DateTime.UtcNow
            };

            var estimateShippingModel = await _shoppingCartModelFactory.PrepareEstimateShippingModelAsync(new[] { wrappedProduct });
			_logger.LogError("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): estimateShippingModel → {EstimateShippingModel}", product, updatecartitem, isAssociatedProduct, estimateShippingModel);

            model.ProductEstimateShipping.ProductId = product.Id;
            model.ProductEstimateShipping.RequestDelay = estimateShippingModel.RequestDelay;
            model.ProductEstimateShipping.Enabled = estimateShippingModel.Enabled;
            model.ProductEstimateShipping.CountryId = estimateShippingModel.CountryId;
            model.ProductEstimateShipping.StateProvinceId = estimateShippingModel.StateProvinceId;
            model.ProductEstimateShipping.ZipPostalCode = estimateShippingModel.ZipPostalCode;
            model.ProductEstimateShipping.UseCity = estimateShippingModel.UseCity;
            model.ProductEstimateShipping.City = estimateShippingModel.City;
            model.ProductEstimateShipping.AvailableCountries = estimateShippingModel.AvailableCountries;
            model.ProductEstimateShipping.AvailableStates = estimateShippingModel.AvailableStates;
        }

        //associated products
        if (product.ProductType == ProductType.GroupedProduct)
        {
            //ensure no circular references
            if (!isAssociatedProduct)
            {
                var associatedProducts = await _productService.GetAssociatedProductsAsync(product.Id, store.Id);
				_logger.LogError("PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct}): associatedProducts → {AssociatedProducts}", product, updatecartitem, isAssociatedProduct, associatedProducts);
                foreach (var associatedProduct in associatedProducts)
                    model.AssociatedProducts.Add(await PrepareProductDetailsModelAsync(associatedProduct, null, true));
            }
            model.InStock = model.AssociatedProducts.Any(associatedProduct => associatedProduct.InStock);
        }
        if (_seoSettings.MicrodataEnabled)
        {
            var jsonLdModel = await _jsonLdModelFactory.PrepareJsonLdProductAsync(model);
            model.JsonLd = JsonConvert.SerializeObject(jsonLdModel, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			_logger.LogWarning("<<< Exiting PrepareProductDetailsModelAsync(product={Product},updatecartitem={Updatecartitem},isAssociatedProduct={IsAssociatedProduct})", product, updatecartitem, isAssociatedProduct);
        }

        return model;
    }

    /// <summary>
    /// Prepare the product reviews model
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product reviews model
    /// </returns>
    public virtual async Task<ProductReviewsModel> PrepareProductReviewsModelAsync(Product product)
    {
		_logger.LogError(">>> Entering PrepareProductReviewsModelAsync(product={Product})", product);
        ArgumentNullException.ThrowIfNull(product);

        var model = new ProductReviewsModel
        {
            ProductId = product.Id
        };

        var currentStore = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareProductReviewsModelAsync(product={Product}): currentStore → {CurrentStore}", product, currentStore);

        var productReviews = await _productReviewService.GetAllProductReviewsAsync(
            approved: true,
            productId: product.Id,
            storeId: _catalogSettings.ShowProductReviewsPerStore ? currentStore.Id : 0);

        //get all review types
        foreach (var reviewType in await _reviewTypeService.GetAllReviewTypesAsync())
        {
            model.ReviewTypeList.Add(new ReviewTypeModel
            {
                Id = reviewType.Id,
                Name = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Name),
                Description = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Description),
                VisibleToAllCustomers = reviewType.VisibleToAllCustomers,
                DisplayOrder = reviewType.DisplayOrder,
                IsRequired = reviewType.IsRequired,
            });
        }

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareProductReviewsModelAsync(product={Product}): currentCustomer → {CurrentCustomer}", product, currentCustomer);

        //filling data from db
        foreach (var pr in productReviews)
        {
            var customer = await _customerService.GetCustomerByIdAsync(pr.CustomerId);
			_logger.LogError("PrepareProductReviewsModelAsync(product={Product}): customer → {Customer}", product, customer);

            var writeOn = await _dateTimeHelper.ConvertToUserTimeAsync(pr.CreatedOnUtc, DateTimeKind.Utc);
			_logger.LogError("PrepareProductReviewsModelAsync(product={Product}): writeOn → {WriteOn}", product, writeOn);

            var productReviewModel = new ProductReviewModel
            {
                Id = pr.Id,
                CustomerId = pr.CustomerId,
                CustomerName = await _customerService.FormatUsernameAsync(customer),
                AllowViewingProfiles = _customerSettings.AllowViewingProfiles && customer != null && !await _customerService.IsGuestAsync(customer),
                Title = pr.Title,
                ReviewText = pr.ReviewText,
                ReplyText = pr.ReplyText,
                Rating = pr.Rating,
                Helpfulness = new ProductReviewHelpfulnessModel
                {
                    ProductReviewId = pr.Id,
                    HelpfulYesTotal = pr.HelpfulYesTotal,
                    HelpfulNoTotal = pr.HelpfulNoTotal,
                },
                WrittenOnStr = writeOn.ToString("g"),
                WrittenOn = writeOn
            };

            if (_customerSettings.AllowCustomersToUploadAvatars)
            {
                productReviewModel.CustomerAvatarUrl = await _pictureService.GetPictureUrlAsync(
                    await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                    _mediaSettings.AvatarPictureSize, _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
            }

            foreach (var q in await _reviewTypeService.GetProductReviewReviewTypeMappingsByProductReviewIdAsync(pr.Id))
            {
                var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(q.ReviewTypeId);
				_logger.LogError("PrepareProductReviewsModelAsync(product={Product}): reviewType → {ReviewType}", product, reviewType);

                productReviewModel.AdditionalProductReviewList.Add(new ProductReviewReviewTypeMappingModel
                {
                    ReviewTypeId = q.ReviewTypeId,
                    ProductReviewId = pr.Id,
                    Rating = q.Rating,
                    Name = await _localizationService.GetLocalizedAsync(reviewType, x => x.Name),
                    VisibleToAllCustomers = reviewType.VisibleToAllCustomers || currentCustomer.Id == pr.CustomerId,
                });
            }

            model.Items.Add(productReviewModel);
        }

        foreach (var rt in model.ReviewTypeList)
        {
            if (model.ReviewTypeList.Count <= model.AddAdditionalProductReviewList.Count)
                continue;
            var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(rt.Id);
            var reviewTypeMappingModel = new AddProductReviewReviewTypeMappingModel
            {
                ReviewTypeId = rt.Id,
                Name = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Name),
                Description = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Description),
                DisplayOrder = rt.DisplayOrder,
                IsRequired = rt.IsRequired,
            };

            model.AddAdditionalProductReviewList.Add(reviewTypeMappingModel);
        }

        //Average rating
        foreach (var rtm in model.ReviewTypeList)
        {
            var totalRating = 0;
            var totalCount = 0;
			_logger.LogError("PrepareProductReviewsModelAsync(product={Product}): totalCount → {TotalCount}", product, totalCount);
            foreach (var item in model.Items)
            {
                foreach (var q in item.AdditionalProductReviewList.Where(w => w.ReviewTypeId == rtm.Id))
                {
                    totalRating += q.Rating;
                    totalCount = ++totalCount;
                }
            }

            rtm.AverageRating = (double)totalRating / (totalCount > 0 ? totalCount : 1);
        }

        model.AddProductReview.CanCurrentCustomerLeaveReview = _catalogSettings.AllowAnonymousUsersToReviewProduct || !await _customerService.IsGuestAsync(currentCustomer);
        model.AddProductReview.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnProductReviewPage;
        model.AddProductReview.CanAddNewReview = await _productReviewService.CanAddReviewAsync(product.Id, _catalogSettings.ShowProductReviewsPerStore ? currentStore.Id : 0);
        model.AddProductReview.Rating = _catalogSettings.DefaultProductRatingValue;

		_logger.LogWarning("<<< Exiting PrepareProductReviewsModelAsync(product={Product})", product);
        return model;
    }

    /// <summary>
    /// Prepare the customer product reviews model
    /// </summary>
    /// <param name="page">Number of items page; pass null to load the first page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer product reviews model
    /// </returns>
    public virtual async Task<CustomerProductReviewsModel> PrepareCustomerProductReviewsModelAsync(int? page)
    {
		_logger.LogError(">>> Entering PrepareCustomerProductReviewsModelAsync(page={Page})", page);
        var pageSize = _catalogSettings.ProductReviewsPageSizeOnAccountPage;
        var pageIndex = 0;
		_logger.LogError("PrepareCustomerProductReviewsModelAsync(page={Page}): pageIndex → {PageIndex}", page, pageIndex);

        if (page > 0)
        {
            pageIndex = page.Value - 1;
        }

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareCustomerProductReviewsModelAsync(page={Page}): store → {Store}", page, store);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareCustomerProductReviewsModelAsync(page={Page}): customer → {Customer}", page, customer);

        var list = await _productReviewService.GetAllProductReviewsAsync(
            customerId: customer.Id,
            approved: null,
            storeId: _catalogSettings.ShowProductReviewsPerStore ? store.Id : 0,
            pageIndex: pageIndex,
            pageSize: pageSize);

        var productReviews = new List<CustomerProductReviewModel>();
		_logger.LogError("PrepareCustomerProductReviewsModelAsync(page={Page}): productReviews → {ProductReviews}", page, productReviews);

        foreach (var review in list)
        {
            var product = await _productService.GetProductByIdAsync(review.ProductId);
			_logger.LogError("PrepareCustomerProductReviewsModelAsync(page={Page}): product → {Product}", page, product);

            var productReviewModel = new CustomerProductReviewModel
            {
                Title = review.Title,
                ProductId = product.Id,
                ProductName = await _localizationService.GetLocalizedAsync(product, p => p.Name),
                ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                Rating = review.Rating,
                ReviewText = review.ReviewText,
                ReplyText = review.ReplyText,
                WrittenOnStr = (await _dateTimeHelper.ConvertToUserTimeAsync(review.CreatedOnUtc, DateTimeKind.Utc)).ToString("g")
            };

            if (_catalogSettings.ProductReviewsMustBeApproved)
            {
                productReviewModel.ApprovalStatus = review.IsApproved
                    ? await _localizationService.GetResourceAsync("Account.CustomerProductReviews.ApprovalStatus.Approved")
                    : await _localizationService.GetResourceAsync("Account.CustomerProductReviews.ApprovalStatus.Pending");
					_logger.LogDebug("PrepareCustomerProductReviewsModelAsync(page={Page}): productReviewModel.ApprovalStatus → {Arg1}", page, productReviewModel.ApprovalStatus);
            }

            foreach (var q in await _reviewTypeService.GetProductReviewReviewTypeMappingsByProductReviewIdAsync(review.Id))
            {
                var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(q.ReviewTypeId);
				_logger.LogError("PrepareCustomerProductReviewsModelAsync(page={Page}): reviewType → {ReviewType}", page, reviewType);

                productReviewModel.AdditionalProductReviewList.Add(new ProductReviewReviewTypeMappingModel
                {
                    ReviewTypeId = q.ReviewTypeId,
                    ProductReviewId = review.Id,
                    Rating = q.Rating,
                    Name = await _localizationService.GetLocalizedAsync(reviewType, x => x.Name),
                });
            }

            productReviews.Add(productReviewModel);
        }

        var pagerModel = new PagerModel(_localizationService)
        {
            PageSize = list.PageSize,
            TotalRecords = list.TotalCount,
            PageIndex = list.PageIndex,
            ShowTotalSummary = false,
            RouteActionName = NopRouteNames.Standard.CUSTOMER_PRODUCT_REVIEWS_PAGED,
            UseRouteLinks = true,
            RouteValues = new CustomerProductReviewsModel.CustomerProductReviewsRouteValues { PageNumber = pageIndex }
        };

        var model = new CustomerProductReviewsModel
        {
            ProductReviews = productReviews,
            PagerModel = pagerModel
        };

		_logger.LogWarning("<<< Exiting PrepareCustomerProductReviewsModelAsync(page={Page})", page);
        return model;
    }

    /// <summary>
    /// Prepare the product email a friend model
    /// </summary>
    /// <param name="model">Product email a friend model</param>
    /// <param name="product">Product</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product email a friend model
    /// </returns>
    public virtual async Task<ProductEmailAFriendModel> PrepareProductEmailAFriendModelAsync(ProductEmailAFriendModel model, Product product, bool excludeProperties)
    {
		_logger.LogError(">>> Entering PrepareProductEmailAFriendModelAsync(model={Model},product={Product},excludeProperties={ExcludeProperties})", model, product, excludeProperties);
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(product);

        model.ProductId = product.Id;
		_logger.LogDebug("PrepareProductEmailAFriendModelAsync(model={Model},product={Product},excludeProperties={ExcludeProperties}): model.ProductId → {Arg3}", model, product, excludeProperties, model.ProductId);
        model.ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name);
        model.ProductSeName = await _urlRecordService.GetSeNameAsync(product);
        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailProductToFriendPage;
		_logger.LogDebug("PrepareProductEmailAFriendModelAsync(model={Model},product={Product},excludeProperties={ExcludeProperties}): model.DisplayCaptcha → {Arg3}", model, product, excludeProperties, model.DisplayCaptcha);
        if (!excludeProperties)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogError("PrepareProductEmailAFriendModelAsync(model={Model},product={Product},excludeProperties={ExcludeProperties}): customer → {Customer}", model, product, excludeProperties, customer);
			_logger.LogDebug("PrepareProductEmailAFriendModelAsync(model={Model},product={Product},excludeProperties={ExcludeProperties}): model.YourEmailAddress → {Arg3}", model, product, excludeProperties, model.YourEmailAddress);
            model.YourEmailAddress = customer.Email;
        }

		_logger.LogWarning("<<< Exiting PrepareProductEmailAFriendModelAsync(model={Model},product={Product},excludeProperties={ExcludeProperties})", model, product, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare the product specification model
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product specification model
    /// </returns>
    public virtual async Task<ProductSpecificationModel> PrepareProductSpecificationModelAsync(Product product)
    {
		_logger.LogError(">>> Entering PrepareProductSpecificationModelAsync(product={Product})", product);
        ArgumentNullException.ThrowIfNull(product);

        var model = new ProductSpecificationModel();
		_logger.LogError("PrepareProductSpecificationModelAsync(product={Product}): model → {Model}", product, model);

        // Add non-grouped attributes first
        model.Groups.Add(new ProductSpecificationAttributeGroupModel
        {
            Attributes = await PrepareProductSpecificationAttributeModelAsync(product, null)
        });

        // Add grouped attributes
        var groups = await _specificationAttributeService.GetProductSpecificationAttributeGroupsAsync(product.Id);
		_logger.LogError("PrepareProductSpecificationModelAsync(product={Product}): groups → {Groups}", product, groups);
        foreach (var group in groups)
        {
            model.Groups.Add(new ProductSpecificationAttributeGroupModel
            {
                Id = group.Id,
                Name = await _localizationService.GetLocalizedAsync(group, x => x.Name),
                Attributes = await PrepareProductSpecificationAttributeModelAsync(product, group)
            });
        }

		_logger.LogWarning("<<< Exiting PrepareProductSpecificationModelAsync(product={Product})", product);
        return model;
    }

    #endregion
}