using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog;

/// <summary>
/// Product review service
/// </summary>
public partial class ProductReviewService : IProductReviewService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IAclService _aclService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderService _orderService;
    protected readonly IProductService _productService;
    protected readonly IRepository<Product> _productRepository;
    protected readonly IRepository<ProductReview> _productReviewRepository;
    protected readonly IRepository<ProductReviewHelpfulness> _productReviewHelpfulnessRepository;
    protected readonly IReviewTypeService _reviewTypeService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public ProductReviewService(CatalogSettings catalogSettings,
        IAclService aclService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        IOrderService orderService,
        IProductService productService,
        IRepository<Product> productRepository,
        IRepository<ProductReview> productReviewRepository,
        IRepository<ProductReviewHelpfulness> productReviewHelpfulnessRepository,
        IReviewTypeService reviewTypeService,
        IStoreMappingService storeMappingService,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings)
    {
		_logger.LogDebug(">>> Entering ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings})", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings);
        _catalogSettings = catalogSettings;
		_logger.LogDebug("ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _catalogSettings → {_catalogSettings}", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings, _catalogSettings);
        _aclService = aclService;
		_logger.LogDebug("ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _aclService → {_aclService}", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings, _aclService);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _customerActivityService → {_customerActivityService}", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings, _customerActivityService);
        _customerService = customerService;
		_logger.LogDebug("ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _customerService → {_customerService}", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings, _customerService);
        _eventPublisher = eventPublisher;
		_logger.LogDebug("ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _eventPublisher → {_eventPublisher}", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings, _eventPublisher);
        _localizationService = localizationService;
		_logger.LogDebug("ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _localizationService → {_localizationService}", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings, _localizationService);
        _orderService = orderService;
		_logger.LogDebug("ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _orderService → {_orderService}", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings, _orderService);
        _productService = productService;
		_logger.LogDebug("ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _productService → {_productService}", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings, _productService);
        _productRepository = productRepository;
		_logger.LogDebug("ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _productRepository → {_productRepository}", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings, _productRepository);
        _productReviewRepository = productReviewRepository;
		_logger.LogDebug("ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _productReviewRepository → {_productReviewRepository}", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings, _productReviewRepository);
        _productReviewHelpfulnessRepository = productReviewHelpfulnessRepository;
		_logger.LogDebug("ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _productReviewHelpfulnessRepository → {_productReviewHelpfulnessRepository}", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings, _productReviewHelpfulnessRepository);
        _reviewTypeService = reviewTypeService;
		_logger.LogDebug("ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _reviewTypeService → {_reviewTypeService}", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings, _reviewTypeService);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _storeMappingService → {_storeMappingService}", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings, _storeMappingService);
        _workContext = workContext;
		_logger.LogDebug("ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _workContext → {_workContext}", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings, _workContext);
        _workflowMessageService = workflowMessageService;
		_logger.LogDebug("ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _workflowMessageService → {_workflowMessageService}", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings, _workflowMessageService);
        _localizationSettings = localizationSettings;
		_logger.LogDebug("<<< Exiting ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings})", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings);
		_logger.LogDebug("ProductReviewService(catalogSettings={CatalogSettings},aclService={AclService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},orderService={OrderService},productService={ProductService},productRepository={ProductRepository},productReviewRepository={ProductReviewRepository},productReviewHelpfulnessRepository={ProductReviewHelpfulnessRepository},reviewTypeService={ReviewTypeService},storeMappingService=<redacted>,workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _localizationSettings → {_localizationSettings}", catalogSettings, aclService, customerActivityService, customerService, eventPublisher, localizationService, orderService, productService, productRepository, productReviewRepository, productReviewHelpfulnessRepository, reviewTypeService, workContext, workflowMessageService, localizationSettings, _localizationSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Checks if customer has completed orders with specified product
    /// </summary>
    /// <param name="product">Product to check</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the check result
    /// </returns>
    protected virtual async ValueTask<bool> HasCompletedOrdersAsync(Product product)
    {
		_logger.LogDebug(">>> Entering HasCompletedOrdersAsync(product={Product})", product);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("HasCompletedOrdersAsync(product={Product}): customer → {Customer}", product, customer);
		_logger.LogDebug("<<< Exiting HasCompletedOrdersAsync(product={Product})", product);
        return (await _orderService.SearchOrdersAsync(customerId: customer.Id,
            productId: product.Id,
            osIds: [(int)OrderStatus.Complete],
            pageSize: 1)).Any();
    }


    /// <summary>
    /// Gets ratio of useful and not useful product reviews 
    /// </summary>
    /// <param name="productReview">Product review</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    protected virtual async Task<(int usefulCount, int notUsefulCount)> GetHelpfulnessCountsAsync(ProductReview productReview)
    {
        ArgumentNullException.ThrowIfNull(productReview);

        var productReviewHelpfulness = _productReviewHelpfulnessRepository.Table.Where(prh => prh.ProductReviewId == productReview.Id);

        return (await productReviewHelpfulness.CountAsync(prh => prh.WasHelpful),
            await productReviewHelpfulness.CountAsync(prh => !prh.WasHelpful));
    }

    /// <summary>
    /// Inserts a product review helpfulness record
    /// </summary>
    /// <param name="productReviewHelpfulness">Product review helpfulness record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InsertProductReviewHelpfulnessAsync(ProductReviewHelpfulness productReviewHelpfulness)
    {
		_logger.LogDebug(">>> Entering InsertProductReviewHelpfulnessAsync(productReviewHelpfulness={ProductReviewHelpfulness})", productReviewHelpfulness);
        await _productReviewHelpfulnessRepository.InsertAsync(productReviewHelpfulness);
		_logger.LogDebug("<<< Exiting InsertProductReviewHelpfulnessAsync(productReviewHelpfulness={ProductReviewHelpfulness})", productReviewHelpfulness);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Update product review totals
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductReviewTotalsAsync(Product product)
    {
		_logger.LogDebug(">>> Entering UpdateProductReviewTotalsAsync(product={Product})", product);
        ArgumentNullException.ThrowIfNull(product);

        var approvedRatingSum = 0;
        var notApprovedRatingSum = 0;
        var approvedTotalReviews = 0;
        var notApprovedTotalReviews = 0;

        var reviews = _productReviewRepository.Table
            .Where(r => r.ProductId == product.Id)
            .ToAsyncEnumerable();
        await foreach (var pr in reviews)
            if (pr.IsApproved)
            {
                approvedRatingSum += pr.Rating;
                approvedTotalReviews++;
            }
            else
            {
                notApprovedRatingSum += pr.Rating;
                notApprovedTotalReviews++;
            }

        product.ApprovedRatingSum = approvedRatingSum;
		_logger.LogDebug("UpdateProductReviewTotalsAsync(product={Product}): product.ApprovedRatingSum → {Arg1}", product, product.ApprovedRatingSum);
		_logger.LogDebug("UpdateProductReviewTotalsAsync(product={Product}): product.NotApprovedRatingSum → {Arg1}", product, product.NotApprovedRatingSum);
		_logger.LogDebug("UpdateProductReviewTotalsAsync(product={Product}): product.ApprovedTotalReviews → {Arg1}", product, product.ApprovedTotalReviews);
		_logger.LogDebug("UpdateProductReviewTotalsAsync(product={Product}): product.NotApprovedTotalReviews → {Arg1}", product, product.NotApprovedTotalReviews);
        product.NotApprovedRatingSum = notApprovedRatingSum;
        product.ApprovedTotalReviews = approvedTotalReviews;
        product.NotApprovedTotalReviews = notApprovedTotalReviews;
        await _productService.UpdateProductAsync(product);
		_logger.LogDebug("<<< Exiting UpdateProductReviewTotalsAsync(product={Product})", product);
    }

    /// <summary>
    /// Validate product review availability
    /// </summary>
    /// <param name="product">Product to validate review availability</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the validation error list if found
    /// </returns>
    public virtual async Task<IList<string>> ValidateProductReviewAvailabilityAsync(Product product)
    {
		_logger.LogDebug(">>> Entering ValidateProductReviewAvailabilityAsync(product={Product})", product);
        var error = new List<string>();
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("ValidateProductReviewAvailabilityAsync(product={Product}): customer → {Customer}", product, customer);
		_logger.LogDebug("ValidateProductReviewAvailabilityAsync(product={Product}): error → {Error}", product, error);
        if (await _customerService.IsGuestAsync(customer) && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
            error.Add(await _localizationService.GetResourceAsync("Reviews.OnlyRegisteredUsersCanWriteReviews"));

        if (!_catalogSettings.ProductReviewPossibleOnlyAfterPurchasing)
            return error;

        var hasCompletedOrders = product.ProductType == ProductType.SimpleProduct
            ? await HasCompletedOrdersAsync(product)
            : await (await _productService.GetAssociatedProductsAsync(product.Id)).AnyAwaitAsync(HasCompletedOrdersAsync);

        if (!hasCompletedOrders)
            error.Add(await _localizationService.GetResourceAsync("Reviews.ProductReviewPossibleOnlyAfterPurchasing"));

		_logger.LogDebug("<<< Exiting ValidateProductReviewAvailabilityAsync(product={Product})", product);
        return error;
    }

    /// <summary>
    /// Gets all product reviews
    /// </summary>
    /// <param name="customerId">Customer identifier (who wrote a review); 0 to load all records</param>
    /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
    /// <param name="fromUtc">Item creation from; null to load all records</param>
    /// <param name="toUtc">Item creation to; null to load all records</param>
    /// <param name="message">Search title or review text; null to load all records</param>
    /// <param name="storeId">The store identifier, where a review has been created; pass 0 to load all records</param>
    /// <param name="productId">The product identifier; pass 0 to load all records</param>
    /// <param name="vendorId">The vendor identifier (limit to products of this vendor); pass 0 to load all records</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the reviews
    /// </returns>
    public virtual async Task<IPagedList<ProductReview>> GetAllProductReviewsAsync(int customerId = 0, bool? approved = null,
        DateTime? fromUtc = null, DateTime? toUtc = null,
        string message = null, int storeId = 0, int productId = 0, int vendorId = 0, bool showHidden = false,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering GetAllProductReviewsAsync(customerId={CustomerId},approved={Approved},fromUtc={FromUtc},toUtc={ToUtc},message={Message},storeId={StoreId},productId={ProductId},vendorId={VendorId},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize})", customerId, approved, fromUtc, toUtc, message, storeId, productId, vendorId, showHidden, pageIndex, pageSize);
        var productReviews = await _productReviewRepository.GetAllPagedAsync(async query =>
        {
			_logger.LogDebug("GetAllProductReviewsAsync(customerId={CustomerId},approved={Approved},fromUtc={FromUtc},toUtc={ToUtc},message={Message},storeId={StoreId},productId={ProductId},vendorId={VendorId},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", customerId, approved, fromUtc, toUtc, message, storeId, productId, vendorId, showHidden, pageIndex, pageSize, query);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            if (!showHidden)
            {
                var productsQuery = _productRepository.Table.Where(p => p.Published);

				_logger.LogDebug("GetAllProductReviewsAsync(customerId={CustomerId},approved={Approved},fromUtc={FromUtc},toUtc={ToUtc},message={Message},storeId={StoreId},productId={ProductId},vendorId={VendorId},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize}): productsQuery → {ProductsQuery}", customerId, approved, fromUtc, toUtc, message, storeId, productId, vendorId, showHidden, pageIndex, pageSize, productsQuery);
				_logger.LogInformation("GetAllProductReviewsAsync(customerId,approved,fromUtc,toUtc,message,storeId,productId,vendorId,showHidden,pageIndex,pageSize)={CustomerId},{Approved},{FromUtc},{ToUtc},{Message},{StoreId},{ProductId},{VendorId},{ShowHidden},{PageIndex},{PageSize}: find query executed in {Arg11} ms", customerId, approved, fromUtc, toUtc, message, storeId, productId, vendorId, showHidden, pageIndex, pageSize, __logStart.ElapsedMilliseconds);
                //apply store mapping constraints
                productsQuery = await _storeMappingService.ApplyStoreMapping(productsQuery, storeId);

                //apply ACL constraints
                var customer = await _workContext.GetCurrentCustomerAsync();
				_logger.LogDebug("GetAllProductReviewsAsync(customerId={CustomerId},approved={Approved},fromUtc={FromUtc},toUtc={ToUtc},message={Message},storeId={StoreId},productId={ProductId},vendorId={VendorId},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize}): customer → {Customer}", customerId, approved, fromUtc, toUtc, message, storeId, productId, vendorId, showHidden, pageIndex, pageSize, customer);
                productsQuery = await _aclService.ApplyAcl(productsQuery, customer);

                query = query.Where(review => productsQuery.Any(product => product.Id == review.ProductId));
            }

            if (approved.HasValue)
                query = query.Where(pr => pr.IsApproved == approved);
            if (customerId > 0)
                query = query.Where(pr => pr.CustomerId == customerId);
            if (fromUtc.HasValue)
                query = query.Where(pr => fromUtc.Value <= pr.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(pr => toUtc.Value >= pr.CreatedOnUtc);
            if (!string.IsNullOrEmpty(message))
                query = query.Where(pr => pr.Title.Contains(message) || pr.ReviewText.Contains(message));
            if (storeId > 0)
                query = query.Where(pr => pr.StoreId == storeId);
            if (productId > 0)
                query = query.Where(pr => pr.ProductId == productId);

            query = from productReview in query
                    join product in _productRepository.Table on productReview.ProductId equals product.Id
                    where
                        (vendorId == 0 || product.VendorId == vendorId) &&
                        //ignore deleted products
                        !product.Deleted
                    select productReview;

            query = _catalogSettings.ProductReviewsSortByCreatedDateAscending
                ? query.OrderBy(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id)
                : query.OrderByDescending(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id);

            return query;
        }, pageIndex, pageSize);

		_logger.LogDebug("<<< Exiting GetAllProductReviewsAsync(customerId={CustomerId},approved={Approved},fromUtc={FromUtc},toUtc={ToUtc},message={Message},storeId={StoreId},productId={ProductId},vendorId={VendorId},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize})", customerId, approved, fromUtc, toUtc, message, storeId, productId, vendorId, showHidden, pageIndex, pageSize);
        return productReviews;
    }

    /// <summary>
    /// Gets product review
    /// </summary>
    /// <param name="productReviewId">Product review identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product review
    /// </returns>
    public virtual async Task<ProductReview> GetProductReviewByIdAsync(int productReviewId)
    {
		_logger.LogDebug(">>> Entering GetProductReviewByIdAsync(productReviewId={ProductReviewId})", productReviewId);
		_logger.LogDebug("<<< Exiting GetProductReviewByIdAsync(productReviewId={ProductReviewId})", productReviewId);
        return await _productReviewRepository.GetByIdAsync(productReviewId, _ => default);
    }

    /// <summary>
    /// Get product reviews by identifiers
    /// </summary>
    /// <param name="productReviewIds">Product review identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product reviews
    /// </returns>
    public virtual async Task<IList<ProductReview>> GetProductReviewsByIdsAsync(int[] productReviewIds)
    {
		_logger.LogDebug(">>> Entering GetProductReviewsByIdsAsync(productReviewIds={ProductReviewIds})", productReviewIds);
		_logger.LogDebug("<<< Exiting GetProductReviewsByIdsAsync(productReviewIds={ProductReviewIds})", productReviewIds);
        return await _productReviewRepository.GetByIdsAsync(productReviewIds);
    }

    /// <summary>
    /// Inserts a product review
    /// </summary>
    /// <param name="productReview">Product review</param>
    /// <param name="productReviewReviewTypeMappings"></param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductReviewAsync(ProductReview productReview, IList<ProductReviewReviewTypeMapping> productReviewReviewTypeMappings = null)
    {
		_logger.LogDebug(">>> Entering InsertProductReviewAsync(productReview={ProductReview},productReviewReviewTypeMappings=<redacted>)", productReview);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productReviewRepository.InsertAsync(productReview);

		_logger.LogInformation("InsertProductReviewAsync(productReview,productReviewReviewTypeMappings)={ProductReview},{ProductReviewReviewTypeMappings}: save query executed in {Arg2} ms", productReview, productReviewReviewTypeMappings, __logStart.ElapsedMilliseconds);
        //add product review and review type mapping
        if (productReviewReviewTypeMappings != null && productReviewReviewTypeMappings.Any())
        {
            foreach (var additionalProductReview in productReviewReviewTypeMappings)
            {
                additionalProductReview.ProductReviewId = productReview.Id;
				_logger.LogDebug("InsertProductReviewAsync(productReview={ProductReview},productReviewReviewTypeMappings=<redacted>): additionalProductReview.ProductReviewId → {Arg2}", productReview, additionalProductReview.ProductReviewId);
                await _reviewTypeService.InsertProductReviewReviewTypeMappingsAsync(additionalProductReview);
            }
        }

        //update product totals
        var product = await _productService.GetProductByIdAsync(productReview.ProductId);
		_logger.LogDebug("InsertProductReviewAsync(productReview={ProductReview},productReviewReviewTypeMappings=<redacted>): product → {Product}", productReview, product);
        await UpdateProductReviewTotalsAsync(product);

        //notify store owner
        if (_catalogSettings.NotifyStoreOwnerAboutNewProductReviews)
            await _workflowMessageService.SendProductReviewStoreOwnerNotificationMessageAsync(productReview, _localizationSettings.DefaultAdminLanguageId);

        //activity log
        await _customerActivityService.InsertActivityAsync("PublicStore.AddProductReview",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddProductReview"), product.Name), product);

        //raise event
        if (productReview.IsApproved)
            await _eventPublisher.PublishAsync(new ProductReviewApprovedEvent(productReview));
			_logger.LogDebug("<<< Exiting InsertProductReviewAsync(productReview={ProductReview},productReviewReviewTypeMappings=<redacted>)", productReview);
    }

    /// <summary>
    /// Deletes a product review
    /// </summary>
    /// <param name="productReview">Product review</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductReviewAsync(ProductReview productReview)
    {
		_logger.LogDebug(">>> Entering DeleteProductReviewAsync(productReview={ProductReview})", productReview);
		_logger.LogInformation("DeleteProductReviewAsync(productReview)={ProductReview}: external service call took {Arg1} ms", productReview, __logStart.ElapsedMilliseconds);
        await _productReviewRepository.DeleteAsync(productReview);
		_logger.LogDebug("<<< Exiting DeleteProductReviewAsync(productReview={ProductReview})", productReview);
    }

    /// <summary>
    /// Deletes product reviews
    /// </summary>
    /// <param name="productReviews">Product reviews</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductReviewsAsync(IList<ProductReview> productReviews)
    {
		_logger.LogDebug(">>> Entering DeleteProductReviewsAsync(productReviews={ProductReviews})", productReviews);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productReviewRepository.DeleteAsync(productReviews);
		_logger.LogInformation("DeleteProductReviewsAsync(productReviews)={ProductReviews}: external service call took {Arg1} ms", productReviews, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting DeleteProductReviewsAsync(productReviews={ProductReviews})", productReviews);
    }

    /// <summary>
    /// Sets or create a product review helpfulness record
    /// </summary>
    /// <param name="productReview">Product review</param>
    /// <param name="helpfulness">Value indicating whether a review a helpful</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetProductReviewHelpfulnessAsync(ProductReview productReview, bool helpfulness)
    {
		_logger.LogDebug(">>> Entering SetProductReviewHelpfulnessAsync(productReview={ProductReview},helpfulness={Helpfulness})", productReview, helpfulness);
        ArgumentNullException.ThrowIfNull(productReview);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("SetProductReviewHelpfulnessAsync(productReview={ProductReview},helpfulness={Helpfulness}): customer → {Customer}", productReview, helpfulness, customer);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var prh = await _productReviewHelpfulnessRepository.Table
            .SingleOrDefaultAsync(h => h.ProductReviewId == productReview.Id && h.CustomerId == customer.Id);
			_logger.LogInformation("SetProductReviewHelpfulnessAsync(productReview,helpfulness)={ProductReview},{Helpfulness}: database query executed in {Arg2} ms", productReview, helpfulness, __logStart.ElapsedMilliseconds);

        if (prh is null)
        {
            //insert new helpfulness
            prh = new ProductReviewHelpfulness
            {
                ProductReviewId = productReview.Id,
                CustomerId = customer.Id,
                WasHelpful = helpfulness,
            };

            await InsertProductReviewHelpfulnessAsync(prh);
        }
        else
        {
            //existing one
            prh.WasHelpful = helpfulness;
			_logger.LogDebug("SetProductReviewHelpfulnessAsync(productReview={ProductReview},helpfulness={Helpfulness}): prh.WasHelpful → {Arg2}", productReview, helpfulness, prh.WasHelpful);

            await _productReviewHelpfulnessRepository.UpdateAsync(prh);
        }
		_logger.LogDebug("<<< Exiting SetProductReviewHelpfulnessAsync(productReview={ProductReview},helpfulness={Helpfulness})", productReview, helpfulness);
    }

    /// <summary>
    /// Updates a product review
    /// </summary>
    /// <param name="productReview">Product review</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductReviewAsync(ProductReview productReview)
    {
		_logger.LogDebug(">>> Entering UpdateProductReviewAsync(productReview={ProductReview})", productReview);
        await _productReviewRepository.UpdateAsync(productReview);
		_logger.LogDebug("<<< Exiting UpdateProductReviewAsync(productReview={ProductReview})", productReview);
    }

    /// <summary>
    /// Updates a totals helpfulness count for product review
    /// </summary>
    /// <param name="productReview">Product review</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task UpdateProductReviewHelpfulnessTotalsAsync(ProductReview productReview)
    {
		_logger.LogDebug(">>> Entering UpdateProductReviewHelpfulnessTotalsAsync(productReview={ProductReview})", productReview);
        ArgumentNullException.ThrowIfNull(productReview);

        (productReview.HelpfulYesTotal, productReview.HelpfulNoTotal) = await GetHelpfulnessCountsAsync(productReview);

        await _productReviewRepository.UpdateAsync(productReview);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("UpdateProductReviewHelpfulnessTotalsAsync(productReview)={ProductReview}: update query executed in {Arg1} ms", productReview, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting UpdateProductReviewHelpfulnessTotalsAsync(productReview={ProductReview})", productReview);
    }

    /// <summary>
    /// Check possibility added review for current customer
    /// </summary>
    /// <param name="productId">Current product</param>
    /// <param name="storeId">The store identifier; pass 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the 
    /// </returns>
    public virtual async Task<bool> CanAddReviewAsync(int productId, int storeId = 0)
    {
		_logger.LogDebug(">>> Entering CanAddReviewAsync(productId={ProductId},storeId={StoreId})", productId, storeId);
        var customer = await _workContext.GetCurrentCustomerAsync();

		_logger.LogDebug("CanAddReviewAsync(productId={ProductId},storeId={StoreId}): customer → {Customer}", productId, storeId, customer);
        if (_catalogSettings.OneReviewPerProductFromCustomer)
            return (await GetAllProductReviewsAsync(customerId: customer.Id, productId: productId, storeId: storeId)).TotalCount == 0;

        return true;
    }

    #endregion
}