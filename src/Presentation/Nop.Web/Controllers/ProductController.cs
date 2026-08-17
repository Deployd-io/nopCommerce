using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Catalog;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class ProductController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly IAclService _aclService;
    protected readonly ICompareProductsService _compareProductsService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly INotificationService _notificationService;
    protected readonly IOrderService _orderService;
    protected readonly IPermissionService _permissionService;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductModelFactory _productModelFactory;
    protected readonly IProductReviewService _productReviewService;
    protected readonly IProductService _productService;
    protected readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
    protected readonly IReviewTypeService _reviewTypeService;
    protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly ShoppingCartSettings _shoppingCartSettings;
    protected readonly ShippingSettings _shippingSettings;

    #endregion

    #region Ctor

    public ProductController(CaptchaSettings captchaSettings,
        CatalogSettings catalogSettings,
        IAclService aclService,
        ICompareProductsService compareProductsService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IEventPublisher eventPublisher,
        IHtmlFormatter htmlFormatter,
        ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        INotificationService notificationService,
        IOrderService orderService,
        IPermissionService permissionService,
        IProductAttributeParser productAttributeParser,
        IProductModelFactory productModelFactory,
        IProductReviewService productReviewService,
        IProductService productService,
        IRecentlyViewedProductsService recentlyViewedProductsService,
        IReviewTypeService reviewTypeService,
        IShoppingCartModelFactory shoppingCartModelFactory,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings,
        ShoppingCartSettings shoppingCartSettings,
        ShippingSettings shippingSettings)
    {
        _captchaSettings = captchaSettings;
        _catalogSettings = catalogSettings;
        _aclService = aclService;
        _compareProductsService = compareProductsService;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _eventPublisher = eventPublisher;
        _htmlFormatter = htmlFormatter;
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _notificationService = notificationService;
        _orderService = orderService;
        _permissionService = permissionService;
        _productAttributeParser = productAttributeParser;
        _productModelFactory = productModelFactory;
        _productReviewService = productReviewService;
        _productService = productService;
        _reviewTypeService = reviewTypeService;
        _recentlyViewedProductsService = recentlyViewedProductsService;
        _shoppingCartModelFactory = shoppingCartModelFactory;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _workContext = workContext;
        _workflowMessageService = workflowMessageService;
        _localizationSettings = localizationSettings;
        _shoppingCartSettings = shoppingCartSettings;
        _shippingSettings = shippingSettings;
    }

    #endregion
    
    #region Product details page

    public virtual async Task<IActionResult> ProductDetails(int productId, int updatecartitemid = 0, int? customwishlistid = null)
    {
		_logger.LogDebug(">>> Entering ProductDetails(productId={ProductId},updatecartitemid={Updatecartitemid},customwishlistid={Customwishlistid})", productId, updatecartitemid, customwishlistid);
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null || product.Deleted)
			_logger.LogDebug("ProductDetails(productId={ProductId},updatecartitemid={Updatecartitemid},customwishlistid={Customwishlistid}): product → {Product}", productId, updatecartitemid, customwishlistid, product);
            return InvokeHttp404();

        var notAvailable =
            //published?
            (!product.Published && !_catalogSettings.AllowViewUnpublishedProductPage) ||
            //ACL (access control list) 
            !await _aclService.AuthorizeAsync(product) ||
            //Store mapping
            !await _storeMappingService.AuthorizeAsync(product) ||
            //availability dates
            !_productService.ProductIsAvailable(product);
        //Check whether the current user has a "Manage products" permission (usually a store owner)
        //We should allows him (her) to use "Preview" functionality
        var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(StandardPermission.Catalog.PRODUCTS_VIEW);
		_logger.LogDebug("ProductDetails(productId={ProductId},updatecartitemid={Updatecartitemid},customwishlistid={Customwishlistid}): hasAdminAccess → {HasAdminAccess}", productId, updatecartitemid, customwishlistid, hasAdminAccess);
        if (notAvailable && !hasAdminAccess)
            return InvokeHttp404();

        //visible individually?
        if (!product.VisibleIndividually)
        {
            //is this one an associated products?
            var parentGroupedProduct = await _productService.GetProductByIdAsync(product.ParentGroupedProductId);
			_logger.LogDebug("ProductDetails(productId={ProductId},updatecartitemid={Updatecartitemid},customwishlistid={Customwishlistid}): parentGroupedProduct → {ParentGroupedProduct}", productId, updatecartitemid, customwishlistid, parentGroupedProduct);
            if (parentGroupedProduct == null)
                return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

            var productUrl = await _nopUrlHelper.RouteGenericUrlAsync(parentGroupedProduct);
			_logger.LogDebug("ProductDetails(productId={ProductId},updatecartitemid={Updatecartitemid},customwishlistid={Customwishlistid}): productUrl → {ProductUrl}", productId, updatecartitemid, customwishlistid, productUrl);
            return LocalRedirectPermanent(productUrl);
        }

        //update existing shopping cart or wishlist  item?
        ShoppingCartItem updatecartitem = null;
		_logger.LogDebug("ProductDetails(productId={ProductId},updatecartitemid={Updatecartitemid},customwishlistid={Customwishlistid}): updatecartitem → {Updatecartitem}", productId, updatecartitemid, customwishlistid, updatecartitem);
        if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
        {
            var productUrl = await _nopUrlHelper.RouteGenericUrlAsync(product);
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogDebug("ProductDetails(productId={ProductId},updatecartitemid={Updatecartitemid},customwishlistid={Customwishlistid}): store → {Store}", productId, updatecartitemid, customwishlistid, store);
            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), storeId: store.Id, customWishlistId: customwishlistid);
			_logger.LogDebug("ProductDetails(productId={ProductId},updatecartitemid={Updatecartitemid},customwishlistid={Customwishlistid}): cart → {Cart}", productId, updatecartitemid, customwishlistid, cart);
            updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);

            //not found?
            if (updatecartitem == null)
                return LocalRedirect(productUrl);

            //is it this product?
            if (product.Id != updatecartitem.ProductId)
                return LocalRedirect(productUrl);
        }

        //save as recently viewed
        await _recentlyViewedProductsService.AddProductToRecentlyViewedListAsync(product.Id);

        //display "edit" (manage) link
        if (await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) &&
            await _permissionService.AuthorizeAsync(StandardPermission.Catalog.PRODUCTS_VIEW))
        {
            //a vendor should have access only to his products
            var currentVendor = await _workContext.GetCurrentVendorAsync();
			_logger.LogDebug("ProductDetails(productId={ProductId},updatecartitemid={Updatecartitemid},customwishlistid={Customwishlistid}): currentVendor → {CurrentVendor}", productId, updatecartitemid, customwishlistid, currentVendor);
            if (currentVendor == null || currentVendor.Id == product.VendorId)
            {
                DisplayEditLink(Url.Action("Edit", "Product", new { id = product.Id, area = AreaNames.ADMIN }));
            }
        }

        //activity log
        await _customerActivityService.InsertActivityAsync("PublicStore.ViewProduct",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ViewProduct"), product.Name), product);

        //model
        var model = await _productModelFactory.PrepareProductDetailsModelAsync(product, updatecartitem, false);
		_logger.LogDebug("ProductDetails(productId={ProductId},updatecartitemid={Updatecartitemid},customwishlistid={Customwishlistid}): model → {Model}", productId, updatecartitemid, customwishlistid, model);
        //template
        var productTemplateViewPath = await _productModelFactory.PrepareProductTemplateViewPathAsync(product);
		_logger.LogDebug("ProductDetails(productId={ProductId},updatecartitemid={Updatecartitemid},customwishlistid={Customwishlistid}): productTemplateViewPath → {ProductTemplateViewPath}", productId, updatecartitemid, customwishlistid, productTemplateViewPath);

		_logger.LogDebug("<<< Exiting ProductDetails(productId={ProductId},updatecartitemid={Updatecartitemid},customwishlistid={Customwishlistid})", productId, updatecartitemid, customwishlistid);
        return View(productTemplateViewPath, model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> EstimateShipping([FromQuery] ProductDetailsModel.ProductEstimateShippingModel model, IFormCollection form)
    {
        if (model == null)
            model = new ProductDetailsModel.ProductEstimateShippingModel();
			_logger.LogDebug("EstimateShipping(model={Model},form={Form}): model → {Model1}", model, form, model);

        var errors = new List<string>();
		_logger.LogDebug("EstimateShipping(model={Model},form={Form}): errors → {Errors}", model, form, errors);

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
				_logger.LogDebug("EstimateShipping(model={Model},form={Form}): Success → {Success}", model, form, Success);
				_logger.LogDebug("EstimateShipping(model={Model},form={Form}): Errors → {Errors}", model, form, Errors);
            });

        var product = await _productService.GetProductByIdAsync(model.ProductId);
		_logger.LogDebug("EstimateShipping(model={Model},form={Form}): product → {Product}", model, form, product);
        if (product == null || product.Deleted)
        {
            errors.Add(await _localizationService.GetResourceAsync("Shipping.EstimateShippingPopUp.Product.IsNotFound"));
            return Json(new
            {
                Success = false,
                Errors = errors
            });
        }

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("EstimateShipping(model={Model},form={Form}): store → {Store}", model, form, store);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("EstimateShipping(model={Model},form={Form}): customer → {Customer}", model, form, customer);

        var wrappedProduct = new ShoppingCartItem()
        {
            StoreId = store.Id,
            ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart,
            CustomerId = customer.Id,
            ProductId = product.Id,
            CreatedOnUtc = DateTime.UtcNow
        };

        var addToCartWarnings = new List<string>();
		_logger.LogDebug("EstimateShipping(model={Model},form={Form}): addToCartWarnings → {AddToCartWarnings}", model, form, addToCartWarnings);
        //customer entered price
        wrappedProduct.CustomerEnteredPrice = await _productAttributeParser.ParseCustomerEnteredPriceAsync(product, form);

        //entered quantity
        wrappedProduct.Quantity = _productAttributeParser.ParseEnteredQuantity(product, form);

        //product and gift card attributes
        wrappedProduct.AttributesXml = await _productAttributeParser.ParseProductAttributesAsync(product, form, addToCartWarnings);

        //rental attributes
        _productAttributeParser.ParseRentalDates(product, form, out var rentalStartDate, out var rentalEndDate);
        wrappedProduct.RentalStartDateUtc = rentalStartDate;
		_logger.LogDebug("EstimateShipping(model={Model},form={Form}): wrappedProduct.RentalStartDateUtc → {Arg2}", model, form, wrappedProduct.RentalStartDateUtc);
		_logger.LogDebug("EstimateShipping(model={Model},form={Form}): wrappedProduct.RentalEndDateUtc → {Arg2}", model, form, wrappedProduct.RentalEndDateUtc);
        wrappedProduct.RentalEndDateUtc = rentalEndDate;

        var result = await _shoppingCartModelFactory.PrepareEstimateShippingResultModelAsync(new[] { wrappedProduct }, model, false);
		_logger.LogDebug("EstimateShipping(model={Model},form={Form}): result → {Result}", model, form, result);

		_logger.LogDebug("<<< Exiting EstimateShipping(model={Model},form={Form})", model, form);
        return Json(result);
    }

    //ignore SEO friendly URLs checks
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> GetProductCombinations(int productId)
    {
		_logger.LogDebug(">>> Entering GetProductCombinations(productId={ProductId})", productId);
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
			_logger.LogDebug("GetProductCombinations(productId={ProductId}): product → {Product}", productId, product);
            return NotFound();

        var model = await _productModelFactory.PrepareProductCombinationModelsAsync(product);
		_logger.LogDebug("GetProductCombinations(productId={ProductId}): model → {Model}", productId, model);
		_logger.LogDebug("<<< Exiting GetProductCombinations(productId={ProductId})", productId);
        return Ok(model);
    }

    #endregion

    #region Recently viewed products

    public virtual async Task<IActionResult> RecentlyViewedProducts()
    {
		_logger.LogDebug(">>> Entering RecentlyViewedProducts()");
        if (!_catalogSettings.RecentlyViewedProductsEnabled)
            return Content("");

        var products = await _recentlyViewedProductsService.GetRecentlyViewedProductsAsync(_catalogSettings.RecentlyViewedProductsNumber);
		_logger.LogDebug("RecentlyViewedProducts(): products → {Products}", products);

        var model = new List<ProductOverviewModel>();
		_logger.LogDebug("RecentlyViewedProducts(): model → {Model}", model);
        model.AddRange(await _productModelFactory.PrepareProductOverviewModelsAsync(products));

		_logger.LogDebug("<<< Exiting RecentlyViewedProducts()");
        return View(model);
    }

    #endregion

    #region Product reviews

    [HttpPost, ActionName("ProductReviews")]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> ProductReviewsAdd(int productId, ProductReviewsModel model, bool captchaValid)
    {
        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnProductReviewPage && !captchaValid) 
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

        var product = await _productService.GetProductByIdAsync(productId);
        var currentStore = await _storeContext.GetCurrentStoreAsync();

        if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews ||
            !await _productReviewService.CanAddReviewAsync(product.Id, _catalogSettings.ShowProductReviewsPerStore ? currentStore.Id : 0))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
        
        foreach (var error in await _productReviewService.ValidateProductReviewAvailabilityAsync(product)) 
            ModelState.AddModelError(string.Empty, error);

        if (ModelState.IsValid)
        {
            //save review
            var rating = model.AddProductReview.Rating;

            if (rating is < 1 or > 5)
                rating = _catalogSettings.DefaultProductRatingValue;

            var isApproved = !_catalogSettings.ProductReviewsMustBeApproved;
            var customer = await _workContext.GetCurrentCustomerAsync();

            var productReview = new ProductReview
            {
                ProductId = product.Id,
                CustomerId = customer.Id,
                Title = model.AddProductReview.Title,
                ReviewText = model.AddProductReview.ReviewText,
                Rating = rating,
                HelpfulYesTotal = 0,
                HelpfulNoTotal = 0,
                IsApproved = isApproved,
                CreatedOnUtc = DateTime.UtcNow,
                StoreId = currentStore.Id,
            };

            var productReviewReviewTypeMappings = model.AddAdditionalProductReviewList.Select(additionalReview =>
                new ProductReviewReviewTypeMapping
                {
                    ReviewTypeId = additionalReview.ReviewTypeId,
                    Rating = additionalReview.Rating
                }).ToList();

            await _productReviewService.InsertProductReviewAsync(productReview, productReviewReviewTypeMappings);

            model = await _productModelFactory.PrepareProductReviewsModelAsync(product);
            model.AddProductReview.Title = null;
            model.AddProductReview.ReviewText = null;

            if (!isApproved)
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Reviews.SeeAfterApproving"));
            else
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Reviews.SuccessfullyAdded"));

            var productUrl = await _nopUrlHelper.RouteGenericUrlAsync(product);

            return LocalRedirect(productUrl);
        }

        //if we got this far, something failed, redisplay form
        RouteData.Values["action"] = "ProductDetails";

        //model
        var productModel = await _productModelFactory.PrepareProductDetailsModelAsync(product);
        //template
        var productTemplateViewPath = await _productModelFactory.PrepareProductTemplateViewPathAsync(product);

        return View(productTemplateViewPath, productModel);
    }

    [HttpPost]
    public virtual async Task<IActionResult> SetProductReviewHelpfulness(int productReviewId, bool washelpful)
    {
		_logger.LogDebug(">>> Entering SetProductReviewHelpfulness(productReviewId={ProductReviewId},washelpful={Washelpful})", productReviewId, washelpful);
        var productReview = await _productReviewService.GetProductReviewByIdAsync(productReviewId) ?? throw new ArgumentException("No product review found with the specified id");

		_logger.LogDebug("SetProductReviewHelpfulness(productReviewId={ProductReviewId},washelpful={Washelpful}): productReview → {ProductReview}", productReviewId, washelpful, productReview);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("SetProductReviewHelpfulness(productReviewId={ProductReviewId},washelpful={Washelpful}): customer → {Customer}", productReviewId, washelpful, customer);
        if (await _customerService.IsGuestAsync(customer) && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
        {
            return Json(new
            {
                Result = await _localizationService.GetResourceAsync("Reviews.Helpfulness.OnlyRegistered"),
                TotalYes = productReview.HelpfulYesTotal,
                TotalNo = productReview.HelpfulNoTotal
				_logger.LogDebug("SetProductReviewHelpfulness(productReviewId={ProductReviewId},washelpful={Washelpful}): TotalYes → {TotalYes}", productReviewId, washelpful, TotalYes);
				_logger.LogDebug("SetProductReviewHelpfulness(productReviewId={ProductReviewId},washelpful={Washelpful}): TotalNo → {TotalNo}", productReviewId, washelpful, TotalNo);
            });
        }

        //customers aren't allowed to vote for their own reviews
        if (productReview.CustomerId == customer.Id)
        {
            return Json(new
            {
                Result = await _localizationService.GetResourceAsync("Reviews.Helpfulness.YourOwnReview"),
                TotalYes = productReview.HelpfulYesTotal,
                TotalNo = productReview.HelpfulNoTotal
            });
        }

        await _productReviewService.SetProductReviewHelpfulnessAsync(productReview, washelpful);

        //new totals
        await _productReviewService.UpdateProductReviewHelpfulnessTotalsAsync(productReview);

		_logger.LogDebug("<<< Exiting SetProductReviewHelpfulness(productReviewId={ProductReviewId},washelpful={Washelpful})", productReviewId, washelpful);
        return Json(new
        {
            Result = await _localizationService.GetResourceAsync("Reviews.Helpfulness.SuccessfullyVoted"),
            TotalYes = productReview.HelpfulYesTotal,
            TotalNo = productReview.HelpfulNoTotal
        });
    }

    public virtual async Task<IActionResult> CustomerProductReviews(int? pageNumber)
    {
		_logger.LogDebug(">>> Entering CustomerProductReviews(pageNumber={PageNumber})", pageNumber);
        if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        if (!_catalogSettings.ShowProductReviewsTabOnAccountPage)
        {
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);
        }

        var model = await _productModelFactory.PrepareCustomerProductReviewsModelAsync(pageNumber);
		_logger.LogDebug("CustomerProductReviews(pageNumber={PageNumber}): model → {Model}", pageNumber, model);

		_logger.LogDebug("<<< Exiting CustomerProductReviews(pageNumber={PageNumber})", pageNumber);
        return View(model);
    }

    #endregion

    #region Email a friend

    public virtual async Task<IActionResult> ProductEmailAFriend(int productId)
    {
		_logger.LogDebug(">>> Entering ProductEmailAFriend(productId={ProductId})", productId);
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null || product.Deleted || !product.Published || !_catalogSettings.EmailAFriendEnabled)
			_logger.LogDebug("ProductEmailAFriend(productId={ProductId}): product → {Product}", productId, product);
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = new ProductEmailAFriendModel();
		_logger.LogDebug("ProductEmailAFriend(productId={ProductId}): model → {Model}", productId, model);
        model = await _productModelFactory.PrepareProductEmailAFriendModelAsync(model, product, false);
		_logger.LogDebug("<<< Exiting ProductEmailAFriend(productId={ProductId})", productId);
        return View(model);
    }

    [HttpPost, ActionName("ProductEmailAFriend")]
    [FormValueRequired("send-email")]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> ProductEmailAFriendSend(ProductEmailAFriendModel model, bool captchaValid)
    {
        var product = await _productService.GetProductByIdAsync(model.ProductId);
        if (product == null || product.Deleted || !product.Published || !_catalogSettings.EmailAFriendEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnEmailProductToFriendPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        //check whether the current customer is guest and ia allowed to email a friend
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer) && !_catalogSettings.AllowAnonymousUsersToEmailAFriend)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Products.EmailAFriend.OnlyRegisteredUsers"));
        }

        if (ModelState.IsValid)
        {
            //email
            await _workflowMessageService.SendProductEmailAFriendMessageAsync(customer,
                (await _workContext.GetWorkingLanguageAsync()).Id, product,
                model.YourEmailAddress, model.FriendEmail,
                _htmlFormatter.FormatText(model.PersonalMessage, false, true, false, false, false, false));

            model = await _productModelFactory.PrepareProductEmailAFriendModelAsync(model, product, true);
			_logger.LogDebug("ProductEmailAFriendSend(model={Model},captchaValid={CaptchaValid}): model → {Model1}", model, captchaValid, model);
			_logger.LogDebug("ProductEmailAFriendSend(model={Model},captchaValid={CaptchaValid}): model.SuccessfullySent → {Arg2}", model, captchaValid, model.SuccessfullySent);
            model.SuccessfullySent = true;
            model.Result = await _localizationService.GetResourceAsync("Products.EmailAFriend.SuccessfullySent");

            return View(model);
        }

        //If we got this far, something failed, redisplay form
        model = await _productModelFactory.PrepareProductEmailAFriendModelAsync(model, product, true);
		_logger.LogDebug("<<< Exiting ProductEmailAFriendSend(model={Model},captchaValid={CaptchaValid})", model, captchaValid);
        return View(model);
    }

    #endregion

    #region Comparing products

    [HttpPost]
    public virtual async Task<IActionResult> AddProductToCompareList(int productId)
    {
		_logger.LogDebug(">>> Entering AddProductToCompareList(productId={ProductId})", productId);
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null || product.Deleted || !product.Published)
			_logger.LogDebug("AddProductToCompareList(productId={ProductId}): product → {Product}", productId, product);
            return Json(new
            {
                success = false,
                message = "No product found with the specified ID"
				_logger.LogDebug("AddProductToCompareList(productId={ProductId}): success → {Success}", productId, success);
				_logger.LogDebug("AddProductToCompareList(productId={ProductId}): message → {Message}", productId, message);
            });

        if (!_catalogSettings.CompareProductsEnabled)
            return Json(new
            {
                success = false,
                message = "Product comparison is disabled"
            });

        await _compareProductsService.AddProductToCompareListAsync(productId);

        //activity log
        await _customerActivityService.InsertActivityAsync("PublicStore.AddToCompareList",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddToCompareList"), product.Name), product);

		_logger.LogDebug("<<< Exiting AddProductToCompareList(productId={ProductId})", productId);
        return Json(new
        {
            success = true,
            message = string.Format(await _localizationService.GetResourceAsync("Products.ProductHasBeenAddedToCompareList.Link"), Url.RouteUrl(NopRouteNames.General.COMPARE_PRODUCTS))
            //use the code below (commented) if you want a customer to be automatically redirected to the compare products page
            //redirect = Url.RouteUrl(NopRouteNames.General.COMPARE_PRODUCTS),
        });
    }

    public virtual async Task<IActionResult> RemoveProductFromCompareList(int productId)
    {
		_logger.LogDebug(">>> Entering RemoveProductFromCompareList(productId={ProductId})", productId);
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
			_logger.LogDebug("RemoveProductFromCompareList(productId={ProductId}): product → {Product}", productId, product);
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (!_catalogSettings.CompareProductsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        await _compareProductsService.RemoveProductFromCompareListAsync(productId);

		_logger.LogDebug("<<< Exiting RemoveProductFromCompareList(productId={ProductId})", productId);
        return RedirectToRoute(NopRouteNames.General.COMPARE_PRODUCTS);
    }

    public virtual async Task<IActionResult> CompareProducts()
    {
		_logger.LogDebug(">>> Entering CompareProducts()");
        if (!_catalogSettings.CompareProductsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = new CompareProductsModel
        {
            IncludeShortDescriptionInCompareProducts = _catalogSettings.IncludeShortDescriptionInCompareProducts,
            IncludeFullDescriptionInCompareProducts = _catalogSettings.IncludeFullDescriptionInCompareProducts,
        };

        var products = await (await _compareProductsService.GetComparedProductsAsync())
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogInformation("CompareProducts(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
            .Where(p => _productService.ProductIsAvailable(p)).ToListAsync();

        //prepare model
        var poModels = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, prepareSpecificationAttributes: true))
            .ToList();
        foreach (var poModel in poModels)
        {
            model.Products.Add(poModel);
        }

		_logger.LogDebug("<<< Exiting CompareProducts()");
        return View(model);
    }

    public virtual IActionResult ClearCompareList()
    {
		_logger.LogDebug(">>> Entering ClearCompareList()");
        if (!_catalogSettings.CompareProductsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        _compareProductsService.ClearCompareProducts();

		_logger.LogDebug("<<< Exiting ClearCompareList()");
        return RedirectToRoute(NopRouteNames.General.COMPARE_PRODUCTS);
    }

    #endregion
}