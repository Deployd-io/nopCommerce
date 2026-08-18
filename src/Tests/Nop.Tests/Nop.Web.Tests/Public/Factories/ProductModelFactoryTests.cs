using FluentAssertions;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Data;
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
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class ProductModelFactoryTests : WebTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IProductModelFactory _productModelFactory;
    private IProductReviewService _productReviewService;
    private IProductService _productService;
    private IUrlRecordService _urlRecordService;

    private ProductModelFactoryForTest _productModelFactoryForTest;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _productModelFactory = GetService<IProductModelFactory>();
        _productReviewService = GetService<IProductReviewService>();
		_logger.LogDebug("SetUp(): _productReviewService → {_productReviewService}", _productReviewService);
		_logger.LogDebug("SetUp(): _productModelFactory → {_productModelFactory}", _productModelFactory);
        _productService = GetService<IProductService>();
		_logger.LogDebug("SetUp(): _productService → {_productService}", _productService);
        _urlRecordService = GetService<IUrlRecordService>();
		_logger.LogDebug("SetUp(): _urlRecordService → {_urlRecordService}", _urlRecordService);
        _productModelFactoryForTest = GetService<ProductModelFactoryForTest>();
		_logger.LogDebug("SetUp(): _productModelFactoryForTest → {_productModelFactoryForTest}", _productModelFactoryForTest);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [Test]
    public async Task CanPreparePriceModel()
    {
		_logger.LogDebug(">>> Entering CanPreparePriceModel()");
        foreach (var product in await _productService.SearchProductsAsync())
        {
            PropertiesShouldEqual(await _productModelFactoryForTest.NewPrepareProductPriceModelAsync(product, true), await _productModelFactoryForTest.PrepareProductOverviewPriceModelAsync(product), "CustomProperties");
            PropertiesShouldEqual(await _productModelFactoryForTest.NewPrepareProductPriceModelAsync(product, true, true), await _productModelFactoryForTest.PrepareProductOverviewPriceModelAsync(product, true), "CustomProperties");
            PropertiesShouldEqual(await _productModelFactoryForTest.NewPrepareProductPriceModelAsync(product), await _productModelFactoryForTest.PrepareProductPriceModelAsync(product), "CustomProperties");
        }
		_logger.LogDebug("<<< Exiting CanPreparePriceModel()");
    }

    [Test]
    public async Task CanPrepareProductTemplateViewPath()
    {
		_logger.LogDebug(">>> Entering CanPrepareProductTemplateViewPath()");
        var productTemplateRepository = GetService<IRepository<ProductTemplate>>();
        var productTemplateSimple = productTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Simple product") ?? throw new Exception("Simple product template could not be loaded");
		_logger.LogDebug("CanPrepareProductTemplateViewPath(): productTemplateSimple → {ProductTemplateSimple}", productTemplateSimple);
        var productTemplateGrouped = productTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Grouped product (with variants)") ?? throw new Exception("Grouped product template could not be loaded");
		_logger.LogDebug("CanPrepareProductTemplateViewPath(): productTemplateGrouped → {ProductTemplateGrouped}", productTemplateGrouped);
		_logger.LogDebug("CanPrepareProductTemplateViewPath(): productTemplateRepository → {ProductTemplateRepository}", productTemplateRepository);

        var modelSimple = await _productModelFactory.PrepareProductTemplateViewPathAsync(new Product
        {
            ProductTemplateId = productTemplateSimple.Id
        });

        var modelGrouped = await _productModelFactory.PrepareProductTemplateViewPathAsync(new Product
        {
            ProductTemplateId = productTemplateGrouped.Id
        });

        modelSimple.Should().NotBe(modelGrouped);

        modelSimple.Should().Be(productTemplateSimple.ViewPath);
        modelGrouped.Should().Be(productTemplateGrouped.ViewPath);
		_logger.LogDebug("<<< Exiting CanPrepareProductTemplateViewPath()");
    }

    [Test]
    public async Task CanPrepareProductOverviewModels()
    {
		_logger.LogDebug(">>> Entering CanPrepareProductOverviewModels()");
        var product = await _productService.GetProductByIdAsync(1);
        var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(new[] { product })).FirstOrDefault();
		_logger.LogDebug("CanPrepareProductOverviewModels(): model → {Model}", model);
		_logger.LogDebug("CanPrepareProductOverviewModels(): product → {Product}", product);

        PropertiesShouldEqual(product, model);
		_logger.LogDebug("<<< Exiting CanPrepareProductOverviewModels()");
    }

    [Test]
    public async Task CanPrepareProductDetailsModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareProductDetailsModel()");
        var product = await _productService.GetProductByIdAsync(1);
        var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(new[] { product })).FirstOrDefault();
		_logger.LogDebug("CanPrepareProductDetailsModel(): model → {Model}", model);
		_logger.LogDebug("CanPrepareProductDetailsModel(): product → {Product}", product);

        PropertiesShouldEqual(product, model);
		_logger.LogDebug("<<< Exiting CanPrepareProductDetailsModel()");
    }

    [Test]
    public async Task CanPrepareProductReviewsModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareProductReviewsModel()");
        var pId = (await _productReviewService.GetProductReviewByIdAsync(1)).ProductId;
        var product = await _productService.GetProductByIdAsync(pId);
		_logger.LogDebug("CanPrepareProductReviewsModel(): product → {Product}", product);
        var model = await _productModelFactory.PrepareProductReviewsModelAsync(product);
		_logger.LogDebug("CanPrepareProductReviewsModel(): model → {Model}", model);
		_logger.LogDebug("CanPrepareProductReviewsModel(): pId → {PId}", pId);

        model.ProductId.Should().Be(product.Id);

        model.Items.Any().Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanPrepareProductReviewsModel()");
    }

    [Test]
    public async Task CanPrepareCustomerProductReviewsModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareCustomerProductReviewsModel()");
        var model = await _productModelFactory.PrepareCustomerProductReviewsModelAsync(null);
        var review = model.ProductReviews.FirstOrDefault();
		_logger.LogDebug("CanPrepareCustomerProductReviewsModel(): review → {Review}", review);
		_logger.LogDebug("CanPrepareCustomerProductReviewsModel(): model → {Model}", model);

        review.Should().NotBeNull();

        var product = await _productService.GetProductByIdAsync(review.ProductId);
		_logger.LogDebug("CanPrepareCustomerProductReviewsModel(): product → {Product}", product);

        review.ProductName.Should().Be(product.Name);
        review.ProductSeName.Should().Be(await _urlRecordService.GetSeNameAsync(product));
		_logger.LogDebug("<<< Exiting CanPrepareCustomerProductReviewsModel()");
    }

    [Test]
    public async Task CanPrepareProductEmailAFriendModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareProductEmailAFriendModel()");
        var product = await _productService.GetProductByIdAsync(1);
        var model = await _productModelFactory.PrepareProductEmailAFriendModelAsync(new ProductEmailAFriendModel(), product, false);
		_logger.LogDebug("CanPrepareProductEmailAFriendModel(): model → {Model}", model);
		_logger.LogDebug("CanPrepareProductEmailAFriendModel(): product → {Product}", product);

        model.ProductId.Should().Be(product.Id);
        model.ProductName.Should().Be(product.Name);
        model.ProductSeName.Should().Be(await GetService<IUrlRecordService>().GetSeNameAsync(product));
        model.YourEmailAddress.Should().Be(NopTestsDefaults.AdminEmail);
		_logger.LogDebug("<<< Exiting CanPrepareProductEmailAFriendModel()");
    }

    [Test]
    public async Task CanPrepareProductSpecificationModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareProductSpecificationModel()");
        var product = await _productService.GetProductByIdAsync(1);
        var model = await _productModelFactory.PrepareProductSpecificationModelAsync(product);
		_logger.LogDebug("CanPrepareProductSpecificationModel(): model → {Model}", model);
		_logger.LogDebug("CanPrepareProductSpecificationModel(): product → {Product}", product);

        var group = model.Groups.FirstOrDefault();
		_logger.LogDebug("CanPrepareProductSpecificationModel(): group → {Group}", group);

        group.Should().NotBe(null);
		_logger.LogDebug("<<< Exiting CanPrepareProductSpecificationModel()");
    }

    #region Nested class

    public class ProductModelFactoryForTest : ProductModelFactory
    {
        public ProductModelFactoryForTest(CaptchaSettings captchaSettings, CatalogSettings catalogSettings, CustomerSettings customerSettings, ICategoryService categoryService, ICurrencyService currencyService, ICustomerService customerService, ICustomWishlistService customWishlistService, IDateRangeService dateRangeService, IDateTimeHelper dateTimeHelper, IDownloadService downloadService, IGenericAttributeService genericAttributeService, IJsonLdModelFactory jsonLdModelFactory, ILocalizationService localizationService, IManufacturerService manufacturerService, IPermissionService permissionService, IPictureService pictureService, IPriceCalculationService priceCalculationService, IPriceFormatter priceFormatter, IProductAttributeParser productAttributeParser, IProductAttributeService productAttributeService, IProductReviewService productReviewService, IProductService productService, IProductTagService productTagService, IProductTemplateService productTemplateService, IReviewTypeService reviewTypeService, IShoppingCartService shoppingCartService, ISpecificationAttributeService specificationAttributeService, IStaticCacheManager staticCacheManager, IStoreContext storeContext, IStoreService storeService, IShoppingCartModelFactory shoppingCartModelFactory, ITaxService taxService, IUrlRecordService urlRecordService, IVendorService vendorService, IVideoService videoService, IWebHelper webHelper, IWorkContext workContext, MediaSettings mediaSettings, OrderSettings orderSettings, SeoSettings seoSettings, ShippingSettings shippingSettings, VendorSettings vendorSettings) : base(captchaSettings, catalogSettings, customerSettings, categoryService, currencyService, customerService, customWishlistService, dateRangeService, dateTimeHelper, downloadService, genericAttributeService, jsonLdModelFactory, localizationService, manufacturerService, permissionService, pictureService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, productReviewService, productService, productTagService, productTemplateService, reviewTypeService, shoppingCartService, specificationAttributeService, staticCacheManager, storeContext, storeService, shoppingCartModelFactory, taxService, urlRecordService, vendorService, videoService, webHelper, workContext, mediaSettings, orderSettings, seoSettings, shippingSettings, vendorSettings)
        {
		_logger.LogDebug(">>> Entering ProductModelFactoryForTest(captchaSettings={CaptchaSettings},catalogSettings={CatalogSettings},customerSettings={CustomerSettings},categoryService={CategoryService},currencyService={CurrencyService},customerService={CustomerService},customWishlistService={CustomWishlistService},dateRangeService={DateRangeService},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},manufacturerService={ManufacturerService},permissionService={PermissionService},pictureService={PictureService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productReviewService={ProductReviewService},productService={ProductService},productTagService={ProductTagService},productTemplateService={ProductTemplateService},reviewTypeService={ReviewTypeService},shoppingCartService=<redacted>,specificationAttributeService={SpecificationAttributeService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeService={StoreService},shoppingCartModelFactory=<redacted>,taxService={TaxService},urlRecordService={UrlRecordService},vendorService={VendorService},videoService={VideoService},webHelper={WebHelper},workContext={WorkContext},mediaSettings={MediaSettings},orderSettings={OrderSettings},seoSettings={SeoSettings},shippingSettings=<redacted>,vendorSettings={VendorSettings})", captchaSettings, catalogSettings, customerSettings, categoryService, currencyService, customerService, customWishlistService, dateRangeService, dateTimeHelper, downloadService, genericAttributeService, jsonLdModelFactory, localizationService, manufacturerService, permissionService, pictureService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, productReviewService, productService, productTagService, productTemplateService, reviewTypeService, specificationAttributeService, staticCacheManager, storeContext, storeService, taxService, urlRecordService, vendorService, videoService, webHelper, workContext, mediaSettings, orderSettings, seoSettings, vendorSettings);
		_logger.LogDebug("<<< Exiting ProductModelFactoryForTest(captchaSettings={CaptchaSettings},catalogSettings={CatalogSettings},customerSettings={CustomerSettings},categoryService={CategoryService},currencyService={CurrencyService},customerService={CustomerService},customWishlistService={CustomWishlistService},dateRangeService={DateRangeService},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},manufacturerService={ManufacturerService},permissionService={PermissionService},pictureService={PictureService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productReviewService={ProductReviewService},productService={ProductService},productTagService={ProductTagService},productTemplateService={ProductTemplateService},reviewTypeService={ReviewTypeService},shoppingCartService=<redacted>,specificationAttributeService={SpecificationAttributeService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeService={StoreService},shoppingCartModelFactory=<redacted>,taxService={TaxService},urlRecordService={UrlRecordService},vendorService={VendorService},videoService={VideoService},webHelper={WebHelper},workContext={WorkContext},mediaSettings={MediaSettings},orderSettings={OrderSettings},seoSettings={SeoSettings},shippingSettings=<redacted>,vendorSettings={VendorSettings})", captchaSettings, catalogSettings, customerSettings, categoryService, currencyService, customerService, customWishlistService, dateRangeService, dateTimeHelper, downloadService, genericAttributeService, jsonLdModelFactory, localizationService, manufacturerService, permissionService, pictureService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, productReviewService, productService, productTagService, productTemplateService, reviewTypeService, specificationAttributeService, staticCacheManager, storeContext, storeService, taxService, urlRecordService, vendorService, videoService, webHelper, workContext, mediaSettings, orderSettings, seoSettings, vendorSettings);
        }

        #region Methods

        /// <summary>
        /// Prepare the product overview price model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product overview price model
        /// </returns>
        public virtual async Task<ProductPriceModel> PrepareProductOverviewPriceModelAsync(Product product, bool forceRedirectionAfterAddingToCart = false)
        {
			_logger.LogDebug(">>> Entering PrepareProductOverviewPriceModelAsync(product={Product},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart})", product, forceRedirectionAfterAddingToCart);
            ArgumentNullException.ThrowIfNull(product);

            async Task prepareSimpleProductOverviewPriceModel(ProductPriceModel priceModel)
            {
				_logger.LogDebug(">>> Entering prepareSimpleProductOverviewPriceModel(priceModel={PriceModel})", priceModel);
				var __logStart = System.Diagnostics.Stopwatch.StartNew();
                //add to cart button
                priceModel.DisableBuyButton = product.DisableBuyButton ||
                                              !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART) ||
                                              !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES);
											_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): priceModel.DisableBuyButton → {Arg1}", priceModel, priceModel.DisableBuyButton);

                //add to wishlist button
                priceModel.DisableWishlistButton = product.DisableWishlistButton ||
                                                   !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_WISHLIST) ||
                                                   !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES);
												_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): priceModel.DisableWishlistButton → {Arg1}", priceModel, priceModel.DisableWishlistButton);
                //compare products
                priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareProductsEnabled;
				_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): priceModel.DisableAddToCompareListButton → {Arg1}", priceModel, priceModel.DisableAddToCompareListButton);

                //rental
                priceModel.IsRental = product.IsRental;
				_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): priceModel.IsRental → {Arg1}", priceModel, priceModel.IsRental);

                //pre-order
                if (product.AvailableForPreOrder)
                {
                    priceModel.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                                                      product.PreOrderAvailabilityStartDateTimeUtc.Value >=
                                                      DateTime.UtcNow;
													_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): priceModel.AvailableForPreOrder → {Arg1}", priceModel, priceModel.AvailableForPreOrder);
                    priceModel.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;
					_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): priceModel.PreOrderAvailabilityStartDateTimeUtc → {Arg1}", priceModel, priceModel.PreOrderAvailabilityStartDateTimeUtc);
                }

                //prices
                if (await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES))
                {
                    if (product.CustomerEntersPrice)
                    {
                        priceModel.CustomerEntersPrice = true;
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): priceModel.CustomerEntersPrice → {Arg1}", priceModel, priceModel.CustomerEntersPrice);
                        return;
                    }
                    if (product.CallForPrice &&
                        //also check whether the current user is impersonated
                        (!_orderSettings.AllowAdminsToBuyCallForPriceProducts ||
                            _workContext.OriginalCustomerIfImpersonated == null))
                    {
                        //call for price
                        priceModel.OldPrice = null;
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): priceModel.OldPrice → {Arg1}", priceModel, priceModel.OldPrice);
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): priceModel.OldPriceValue → {Arg1}", priceModel, priceModel.OldPriceValue);
                        priceModel.OldPriceValue = null;
                        priceModel.Price = await _localizationService.GetResourceAsync("Products.CallForPrice");
                        priceModel.PriceValue = null;
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): priceModel.PriceValue → {Arg1}", priceModel, priceModel.PriceValue);
                    }
                    else
                    {
                        var store = await _storeContext.GetCurrentStoreAsync();
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): store → {Store}", priceModel, store);
                        var customer = await _workContext.GetCurrentCustomerAsync();
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): customer → {Customer}", priceModel, customer);

                        //prices
                        var (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount) = (decimal.Zero, decimal.Zero);
                        var hasMultiplePrices = false;
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): hasMultiplePrices → {HasMultiplePrices}", priceModel, hasMultiplePrices);
                        if (_catalogSettings.DisplayFromPrices)
                        {
                            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
							_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): customerRoleIds → {CustomerRoleIds}", priceModel, customerRoleIds);
                            var cacheKey = _staticCacheManager
                                .PrepareKeyForDefaultCache(NopCatalogDefaults.ProductMultiplePriceCacheKey, product, customerRoleIds, store);
                            if (!_catalogSettings.CacheProductPrices || product.IsRental)
                                cacheKey.CacheTime = 0;
								_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): cacheKey.CacheTime → {Arg1}", priceModel, cacheKey.CacheTime);

                            var cachedPrice = await _staticCacheManager.GetAsync(cacheKey, async () =>
                            {
                                var prices = new List<(decimal PriceWithoutDiscount, decimal PriceWithDiscount)>();
								_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): prices → {Prices}", priceModel, prices);

                                // price when there are no required attributes
                                var attributesMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
								_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): attributesMappings → {AttributesMappings}", priceModel, attributesMappings);
                                if (!attributesMappings.Any(am => !am.IsNonCombinable() && am.IsRequired))
                                {
                                    (var priceWithoutDiscount, var priceWithDiscount, _, _) = await _priceCalculationService
                                        .GetFinalPriceAsync(product, customer, store);
                                    prices.Add((priceWithoutDiscount, priceWithDiscount));
                                }

                                var allAttributesXml = await _productAttributeParser.GenerateAllCombinationsAsync(product, true);
								_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): allAttributesXml → {AllAttributesXml}", priceModel, allAttributesXml);
                                foreach (var attributesXml in allAttributesXml)
                                {
                                    var warnings = new List<string>();
									_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): warnings → {Warnings}", priceModel, warnings);
                                    warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(customer,
                                        ShoppingCartType.ShoppingCart, product, 1, attributesXml, true, true, true));
                                    if (warnings.Any())
                                        continue;

                                    //get price with additional charge
                                    var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
									_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): combination → {Combination}", priceModel, combination);
                                    if (combination?.OverriddenPrice.HasValue ?? false)
                                    {
                                        (var priceWithoutDiscount, var priceWithDiscount, _, _) = await _priceCalculationService
                                            .GetFinalPriceAsync(product, customer, store, combination.OverriddenPrice.Value, decimal.Zero, true, 1, null, null);
                                        prices.Add((priceWithoutDiscount, priceWithDiscount));
                                    }
                                    else
                                    {
                                        var additionalCharge = decimal.Zero;
                                        var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
										_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): attributeValues → {AttributeValues}", priceModel, attributeValues);
                                        foreach (var attributeValue in attributeValues)
                                        {
                                            additionalCharge += await _priceCalculationService.
                                                GetProductAttributeValuePriceAdjustmentAsync(product, attributeValue, customer, store);
                                        }
                                        if (additionalCharge != decimal.Zero)
                                        {
                                            (var priceWithoutDiscount, var priceWithDiscount, _, _) = await _priceCalculationService
                                                .GetFinalPriceAsync(product, customer, store, additionalCharge);
                                            prices.Add((priceWithoutDiscount, priceWithDiscount));
                                        }
                                    }
                                }

                                if (prices.Distinct().Count() > 1)
                                {
                                    (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount) = prices.OrderBy(p => p.PriceWithDiscount).First();
                                    return new
                                    {
                                        PriceWithoutDiscount = minPossiblePriceWithoutDiscount,
                                        PriceWithDiscount = minPossiblePriceWithDiscount
										_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): PriceWithoutDiscount → {PriceWithoutDiscount}", priceModel, PriceWithoutDiscount);
										_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): PriceWithDiscount → {PriceWithDiscount}", priceModel, PriceWithDiscount);
                                    };
                                }

                                // show default price when required attributes available but no values added
                                (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, store);

                                //don't cache (return null) if there are no multiple prices
                                return null;
                            });

                            if (cachedPrice is not null)
                            {
                                hasMultiplePrices = true;
                                (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount) = (cachedPrice.PriceWithoutDiscount, cachedPrice.PriceWithDiscount);
                            }
                        }
                        else
                            (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, store);

                        var (tierPriceMinPossiblePriceWithoutDiscount, tierPriceMinPossiblePriceWithDiscount, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, store, quantity: int.MaxValue);

                        //calculate price for the maximum quantity if we have tier prices, and choose minimal
                        minPossiblePriceWithoutDiscount = Math.Min(minPossiblePriceWithoutDiscount, tierPriceMinPossiblePriceWithoutDiscount);
                        minPossiblePriceWithDiscount = Math.Min(minPossiblePriceWithDiscount, tierPriceMinPossiblePriceWithDiscount);

						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): minPossiblePriceWithoutDiscount → {MinPossiblePriceWithoutDiscount}", priceModel, minPossiblePriceWithoutDiscount);
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): minPossiblePriceWithDiscount → {MinPossiblePriceWithDiscount}", priceModel, minPossiblePriceWithDiscount);
                        var (oldPriceBase, _) = await _taxService.GetProductPriceAsync(product, product.OldPrice);
                        var (finalPriceWithoutDiscountBase, _) = await _taxService.GetProductPriceAsync(product, minPossiblePriceWithoutDiscount);
                        var (finalPriceWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, minPossiblePriceWithDiscount);
                        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): currentCurrency → {CurrentCurrency}", priceModel, currentCurrency);
                        var oldPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(oldPriceBase, currentCurrency);
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): oldPrice → {OldPrice}", priceModel, oldPrice);
                        var finalPriceWithoutDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithoutDiscountBase, currentCurrency);
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): finalPriceWithoutDiscount → {FinalPriceWithoutDiscount}", priceModel, finalPriceWithoutDiscount);
                        var finalPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithDiscountBase, currentCurrency);
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): finalPriceWithDiscount → {FinalPriceWithDiscount}", priceModel, finalPriceWithDiscount);

                        var strikeThroughPrice = decimal.Zero;
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): strikeThroughPrice → {StrikeThroughPrice}", priceModel, strikeThroughPrice);

                        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                            strikeThroughPrice = oldPrice;

                        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                            strikeThroughPrice = finalPriceWithoutDiscount;

                        if (strikeThroughPrice > decimal.Zero)
                        {
                            priceModel.OldPrice = await _priceFormatter.FormatPriceAsync(strikeThroughPrice);
                            priceModel.OldPriceValue = strikeThroughPrice;
                        }
                        else
                        {
                            priceModel.OldPrice = null;
                            priceModel.OldPriceValue = null;
                        }

                        //do we have tier prices configured?
                        var tierPrices = await _productService.GetTierPricesAsync(product, customer, store);
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): tierPrices → {TierPrices}", priceModel, tierPrices);

                        //When there is just one tier price (with  qty 1), there are no actual savings in the list.
                        var hasTierPrices = tierPrices.Any() && !(tierPrices.Count == 1 && tierPrices[0].Quantity <= 1);
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): hasTierPrices → {HasTierPrices}", priceModel, hasTierPrices);

                        var price = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): price → {Price}", priceModel, price);
                        priceModel.Price = hasTierPrices || hasMultiplePrices
                            ? string.Format(await _localizationService.GetResourceAsync("Products.PriceRangeFrom"), price)
                            : price;
							_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): priceModel.Price → {Arg1}", priceModel, priceModel.Price);
                        priceModel.PriceValue = finalPriceWithDiscount;

                        if (product.IsRental)
                        {
                            //rental product
                            priceModel.OldPrice = await _priceFormatter.FormatRentalProductPeriodAsync(product, priceModel.OldPrice);
                            priceModel.RentalPrice = priceModel.Price = await _priceFormatter.FormatRentalProductPeriodAsync(product, priceModel.Price);
                            priceModel.RentalPriceValue = finalPriceWithDiscount;
							_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): priceModel.RentalPriceValue → {Arg1}", priceModel, priceModel.RentalPriceValue);
                        }

                        //property for German market
                        //we display tax/shipping info only with "shipping enabled" for this product
                        //we also ensure this it's not free shipping
                        priceModel.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoProductBoxes && product.IsShipEnabled && !product.IsFreeShipping;
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): priceModel.DisplayTaxShippingInfo → {Arg1}", priceModel, priceModel.DisplayTaxShippingInfo);

                        //PAngV default baseprice (used in Germany)
                        priceModel.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(product, finalPriceWithDiscount);
                        priceModel.BasePricePAngVValue = finalPriceWithDiscount;
						_logger.LogDebug("prepareSimpleProductOverviewPriceModel(priceModel={PriceModel}): priceModel.BasePricePAngVValue → {Arg1}", priceModel, priceModel.BasePricePAngVValue);
                    }
                }
                else
                {
                    //hide prices
                    priceModel.OldPrice = null;
                    priceModel.OldPriceValue = null;
                    priceModel.Price = null;
                    priceModel.PriceValue = null;
                }
				_logger.LogInformation("prepareSimpleProductOverviewPriceModel(priceModel)={PriceModel}: external service call took {Arg1} ms", priceModel, __logStart.ElapsedMilliseconds);
				_logger.LogDebug("<<< Exiting prepareSimpleProductOverviewPriceModel(priceModel={PriceModel})", priceModel);
            }

            async Task prepareGroupedProductOverviewPriceModel(ProductPriceModel priceModel)
            {
				_logger.LogDebug(">>> Entering prepareGroupedProductOverviewPriceModel(priceModel={PriceModel})", priceModel);
                var store = await _storeContext.GetCurrentStoreAsync();
                var associatedProducts = await _productService.GetAssociatedProductsAsync(product.Id,
                    store.Id);
					_logger.LogDebug("prepareGroupedProductOverviewPriceModel(priceModel={PriceModel}): store → {Store}", priceModel, store);

                //add to cart button (ignore "DisableBuyButton" property for grouped products)
                priceModel.DisableBuyButton =
                    !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART) ||
                    !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES);

                //add to wishlist button (ignore "DisableWishlistButton" property for grouped products)
                priceModel.DisableWishlistButton =
                    !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_WISHLIST) ||
                    !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES);

                //compare products
                priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareProductsEnabled;
				_logger.LogDebug("prepareGroupedProductOverviewPriceModel(priceModel={PriceModel}): priceModel.DisableAddToCompareListButton → {Arg1}", priceModel, priceModel.DisableAddToCompareListButton);
                if (!associatedProducts.Any())
                    return;

                //we have at least one associated product
                if (await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES))
                {
                    //find a minimum possible price
                    decimal? minPossiblePrice = null;
                    Product minPriceProduct = null;
					_logger.LogDebug("prepareGroupedProductOverviewPriceModel(priceModel={PriceModel}): minPriceProduct → {MinPriceProduct}", priceModel, minPriceProduct);
                    var customer = await _workContext.GetCurrentCustomerAsync();
					_logger.LogDebug("prepareGroupedProductOverviewPriceModel(priceModel={PriceModel}): customer → {Customer}", priceModel, customer);
                    foreach (var associatedProduct in associatedProducts)
                    {
                        var (_, tmpMinPossiblePrice, _, _) = await _priceCalculationService.GetFinalPriceAsync(associatedProduct, customer, store);

                        //calculate price for the maximum quantity if we have tier prices, and choose minimal
                        tmpMinPossiblePrice = Math.Min(tmpMinPossiblePrice,
                            (await _priceCalculationService.GetFinalPriceAsync(associatedProduct, customer, store, quantity: int.MaxValue)).finalPrice);
							_logger.LogDebug("prepareGroupedProductOverviewPriceModel(priceModel={PriceModel}): tmpMinPossiblePrice → {TmpMinPossiblePrice}", priceModel, tmpMinPossiblePrice);

                        if (minPossiblePrice.HasValue && tmpMinPossiblePrice >= minPossiblePrice.Value)
                            continue;
                        minPriceProduct = associatedProduct;
                        minPossiblePrice = tmpMinPossiblePrice;
						_logger.LogDebug("prepareGroupedProductOverviewPriceModel(priceModel={PriceModel}): minPossiblePrice → {MinPossiblePrice}", priceModel, minPossiblePrice);
                    }

                    if (minPriceProduct == null || minPriceProduct.CustomerEntersPrice)
                    {
                        priceModel.CustomerEntersPrice = minPriceProduct?.CustomerEntersPrice ?? product.CustomerEntersPrice;
						_logger.LogDebug("prepareGroupedProductOverviewPriceModel(priceModel={PriceModel}): priceModel.CustomerEntersPrice → {Arg1}", priceModel, priceModel.CustomerEntersPrice);
                        return;
                    }

                    if (minPriceProduct.CallForPrice &&
                        //also check whether the current user is impersonated
                        (!_orderSettings.AllowAdminsToBuyCallForPriceProducts ||
                            _workContext.OriginalCustomerIfImpersonated == null))
                    {
                        priceModel.OldPrice = null;
						_logger.LogDebug("prepareGroupedProductOverviewPriceModel(priceModel={PriceModel}): priceModel.OldPrice → {Arg1}", priceModel, priceModel.OldPrice);
						_logger.LogDebug("prepareGroupedProductOverviewPriceModel(priceModel={PriceModel}): priceModel.OldPriceValue → {Arg1}", priceModel, priceModel.OldPriceValue);
                        priceModel.OldPriceValue = null;
                        priceModel.Price = await _localizationService.GetResourceAsync("Products.CallForPrice");
                        priceModel.PriceValue = null;
						_logger.LogDebug("prepareGroupedProductOverviewPriceModel(priceModel={PriceModel}): priceModel.PriceValue → {Arg1}", priceModel, priceModel.PriceValue);
                    }
                    else
                    {
                        //calculate prices
                        var (finalPriceBase, _) = await _taxService.GetProductPriceAsync(minPriceProduct, minPossiblePrice.Value);
                        var finalPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceBase, await _workContext.GetWorkingCurrencyAsync());
						_logger.LogDebug("prepareGroupedProductOverviewPriceModel(priceModel={PriceModel}): finalPrice → {FinalPrice}", priceModel, finalPrice);

                        priceModel.OldPrice = null;
                        priceModel.OldPriceValue = null;
                        priceModel.Price = string.Format(await _localizationService.GetResourceAsync("Products.PriceRangeFrom"), await _priceFormatter.FormatPriceAsync(finalPrice));
                        priceModel.PriceValue = finalPrice;

                        //PAngV default baseprice (used in Germany)
                        priceModel.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(product, finalPriceBase);
                        priceModel.BasePricePAngVValue = finalPriceBase;
						_logger.LogDebug("prepareGroupedProductOverviewPriceModel(priceModel={PriceModel}): priceModel.BasePricePAngVValue → {Arg1}", priceModel, priceModel.BasePricePAngVValue);
                    }
                }
                else
                {
                    //hide prices
                    priceModel.OldPrice = null;
                    priceModel.OldPriceValue = null;
                    priceModel.Price = null;
					_logger.LogDebug("prepareGroupedProductOverviewPriceModel(priceModel={PriceModel}): priceModel.Price → {Arg1}", priceModel, priceModel.Price);
                    priceModel.PriceValue = null;
                }
				_logger.LogDebug("<<< Exiting prepareGroupedProductOverviewPriceModel(priceModel={PriceModel})", priceModel);
            }

            var currentCurrency = await _workContext.GetWorkingCurrencyAsync();

            var priceModel = new ProductPriceModel
            {
                ForceRedirectionAfterAddingToCart = forceRedirectionAfterAddingToCart,
                CurrencyCode = currentCurrency.CurrencyCode,
                ProductId = product.Id
            };

            switch (product.ProductType)
            {
                case ProductType.GroupedProduct:
                    //grouped product
                    await prepareGroupedProductOverviewPriceModel(priceModel);

                    break;
                case ProductType.SimpleProduct:
                default:
                    //simple product
                    await prepareSimpleProductOverviewPriceModel(priceModel);

                    break;
            }

			_logger.LogDebug("<<< Exiting PrepareProductOverviewPriceModelAsync(product={Product},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart})", product, forceRedirectionAfterAddingToCart);
            return priceModel;
        }

        /// <summary>
        /// Prepare the product price model
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product price model
        /// </returns>
        public virtual async Task<ProductPriceModel> PrepareProductPriceModelAsync(Product product)
        {
			_logger.LogDebug(">>> Entering PrepareProductPriceModelAsync(product={Product})", product);
            ArgumentNullException.ThrowIfNull(product);
            var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
			_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): currentCurrency → {CurrentCurrency}", product, currentCurrency);

            var model = new ProductPriceModel
            {
                ProductId = product.Id,
                //currency code
                CurrencyCode = currentCurrency.CurrencyCode
            };

            if (await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES))
            {
                model.HidePrices = false;
				_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): model.HidePrices → {Arg1}", product, model.HidePrices);
                if (product.CustomerEntersPrice)
                    model.CustomerEntersPrice = true;
					_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): model.CustomerEntersPrice → {Arg1}", product, model.CustomerEntersPrice);
                else
                {
                    if (product.CallForPrice &&
                        //also check whether the current user is impersonated
                        (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
                    {
                        model.CallForPrice = true;
						_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): model.CallForPrice → {Arg1}", product, model.CallForPrice);
                    }
                    else
                    {
                        var customer = await _workContext.GetCurrentCustomerAsync();
						_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): customer → {Customer}", product, customer);
                        var store = await _storeContext.GetCurrentStoreAsync();
						_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): store → {Store}", product, store);

                        var (oldPriceBase, _) = await _taxService.GetProductPriceAsync(product, product.OldPrice);

                        var (finalPriceWithoutDiscountBase, _) = await _taxService.GetProductPriceAsync(product, (await _priceCalculationService.GetFinalPriceAsync(product, customer, store, includeDiscounts: false)).finalPrice);
                        var (finalPriceWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, (await _priceCalculationService.GetFinalPriceAsync(product, customer, store)).finalPrice);

                        var oldPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(oldPriceBase, currentCurrency);
						_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): oldPrice → {OldPrice}", product, oldPrice);
                        var finalPriceWithoutDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithoutDiscountBase, currentCurrency);
						_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): finalPriceWithoutDiscount → {FinalPriceWithoutDiscount}", product, finalPriceWithoutDiscount);
                        var finalPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithDiscountBase, currentCurrency);
						_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): finalPriceWithDiscount → {FinalPriceWithDiscount}", product, finalPriceWithDiscount);

                        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                        {
                            model.OldPrice = await _priceFormatter.FormatPriceAsync(oldPrice);
                            model.OldPriceValue = oldPrice;
							_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): model.OldPriceValue → {Arg1}", product, model.OldPriceValue);
                        }

                        model.Price = await _priceFormatter.FormatPriceAsync(finalPriceWithoutDiscount);

                        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                        {
                            model.PriceWithDiscount = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
                            model.PriceWithDiscountValue = finalPriceWithDiscount;
							_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): model.PriceWithDiscountValue → {Arg1}", product, model.PriceWithDiscountValue);
                        }

                        model.PriceValue = finalPriceWithDiscount;
						_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): model.PriceValue → {Arg1}", product, model.PriceValue);

                        //property for German market
                        //we display tax/shipping info only with "shipping enabled" for this product
                        //we also ensure this it's not free shipping
                        model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoProductDetailsPage
                                                       && product.IsShipEnabled &&
                                                       !product.IsFreeShipping;
													_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): model.DisplayTaxShippingInfo → {Arg1}", product, model.DisplayTaxShippingInfo);

                        //PAngV baseprice (used in Germany)
                        model.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(product, finalPriceWithDiscountBase);
                        model.BasePricePAngVValue = finalPriceWithDiscountBase;
						_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): model.BasePricePAngVValue → {Arg1}", product, model.BasePricePAngVValue);

                        //rental
                        if (product.IsRental)
                        {
                            model.IsRental = true;
							_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): model.IsRental → {Arg1}", product, model.IsRental);
                            var priceStr = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
							_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): priceStr → {PriceStr}", product, priceStr);
                            model.Price = model.RentalPrice = await _priceFormatter.FormatRentalProductPeriodAsync(product, priceStr);
                            model.RentalPriceValue = finalPriceWithDiscount;
							_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): model.RentalPriceValue → {Arg1}", product, model.RentalPriceValue);
                        }
                    }
                }
            }
            else
            {
                model.HidePrices = true;
                model.OldPrice = null;
				_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): model.OldPrice → {Arg1}", product, model.OldPrice);
                model.OldPriceValue = null;
                model.Price = null;
				_logger.LogDebug("PrepareProductPriceModelAsync(product={Product}): model.Price → {Arg1}", product, model.Price);
            }

			_logger.LogDebug("<<< Exiting PrepareProductPriceModelAsync(product={Product})", product);
            return model;
        }

        public async Task<ProductPriceModel> NewPrepareProductPriceModelAsync(Product product,
            bool addPriceRangeFrom = false, bool forceRedirectionAfterAddingToCart = false)
        {
			_logger.LogDebug(">>> Entering NewPrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart})", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart);
			_logger.LogDebug("<<< Exiting NewPrepareProductPriceModelAsync(product={Product},addPriceRangeFrom={AddPriceRangeFrom},forceRedirectionAfterAddingToCart={ForceRedirectionAfterAddingToCart})", product, addPriceRangeFrom, forceRedirectionAfterAddingToCart);
            return await PrepareProductPriceModelAsync(product, addPriceRangeFrom, forceRedirectionAfterAddingToCart);
        }

        #endregion
    }

    #endregion
}