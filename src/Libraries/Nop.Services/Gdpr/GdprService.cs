using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Authentication.External;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Orders;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Gdpr;

/// <summary>
/// Represents the GDPR service
/// </summary>
public partial class GdprService : IGdprService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
    protected readonly IBlogService _blogService;
    protected readonly ICustomerService _customerService;
    protected readonly IExternalAuthenticationService _externalAuthenticationService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IForumService _forumService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly INewsService _newsService;
    protected readonly IProductReviewService _productReviewService;
    protected readonly IProductService _productService;
    protected readonly IRepository<GdprConsent> _gdprConsentRepository;
    protected readonly IRepository<GdprLog> _gdprLogRepository;
    protected readonly IShoppingCartService _shoppingCartService;

    #endregion

    #region Ctor

    public GdprService(IAddressService addressService,
        IBackInStockSubscriptionService backInStockSubscriptionService,
        IBlogService blogService,
        ICustomerService customerService,
        IExternalAuthenticationService externalAuthenticationService,
        IEventPublisher eventPublisher,
        IForumService forumService,
        IGenericAttributeService genericAttributeService,
        INewsService newsService,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        IProductReviewService productReviewService,
        IProductService productService,
        IRepository<GdprConsent> gdprConsentRepository,
        IRepository<GdprLog> gdprLogRepository,
        IShoppingCartService shoppingCartService)
    {
		_logger.LogDebug(">>> Entering GdprService(addressService={AddressService},backInStockSubscriptionService={BackInStockSubscriptionService},blogService={BlogService},customerService={CustomerService},externalAuthenticationService={ExternalAuthenticationService},eventPublisher={EventPublisher},forumService={ForumService},genericAttributeService={GenericAttributeService},newsService={NewsService},newsLetterSubscriptionService={NewsLetterSubscriptionService},productReviewService={ProductReviewService},productService={ProductService},gdprConsentRepository={GdprConsentRepository},gdprLogRepository={GdprLogRepository},shoppingCartService=<redacted>)", addressService, backInStockSubscriptionService, blogService, customerService, externalAuthenticationService, eventPublisher, forumService, genericAttributeService, newsService, newsLetterSubscriptionService, productReviewService, productService, gdprConsentRepository, gdprLogRepository);
        _addressService = addressService;
		_logger.LogDebug("GdprService(addressService={AddressService},backInStockSubscriptionService={BackInStockSubscriptionService},blogService={BlogService},customerService={CustomerService},externalAuthenticationService={ExternalAuthenticationService},eventPublisher={EventPublisher},forumService={ForumService},genericAttributeService={GenericAttributeService},newsService={NewsService},newsLetterSubscriptionService={NewsLetterSubscriptionService},productReviewService={ProductReviewService},productService={ProductService},gdprConsentRepository={GdprConsentRepository},gdprLogRepository={GdprLogRepository},shoppingCartService=<redacted>): _addressService → {_addressService}", addressService, backInStockSubscriptionService, blogService, customerService, externalAuthenticationService, eventPublisher, forumService, genericAttributeService, newsService, newsLetterSubscriptionService, productReviewService, productService, gdprConsentRepository, gdprLogRepository, _addressService);
        _backInStockSubscriptionService = backInStockSubscriptionService;
		_logger.LogDebug("GdprService(addressService={AddressService},backInStockSubscriptionService={BackInStockSubscriptionService},blogService={BlogService},customerService={CustomerService},externalAuthenticationService={ExternalAuthenticationService},eventPublisher={EventPublisher},forumService={ForumService},genericAttributeService={GenericAttributeService},newsService={NewsService},newsLetterSubscriptionService={NewsLetterSubscriptionService},productReviewService={ProductReviewService},productService={ProductService},gdprConsentRepository={GdprConsentRepository},gdprLogRepository={GdprLogRepository},shoppingCartService=<redacted>): _backInStockSubscriptionService → {_backInStockSubscriptionService}", addressService, backInStockSubscriptionService, blogService, customerService, externalAuthenticationService, eventPublisher, forumService, genericAttributeService, newsService, newsLetterSubscriptionService, productReviewService, productService, gdprConsentRepository, gdprLogRepository, _backInStockSubscriptionService);
        _blogService = blogService;
		_logger.LogDebug("GdprService(addressService={AddressService},backInStockSubscriptionService={BackInStockSubscriptionService},blogService={BlogService},customerService={CustomerService},externalAuthenticationService={ExternalAuthenticationService},eventPublisher={EventPublisher},forumService={ForumService},genericAttributeService={GenericAttributeService},newsService={NewsService},newsLetterSubscriptionService={NewsLetterSubscriptionService},productReviewService={ProductReviewService},productService={ProductService},gdprConsentRepository={GdprConsentRepository},gdprLogRepository={GdprLogRepository},shoppingCartService=<redacted>): _blogService → {_blogService}", addressService, backInStockSubscriptionService, blogService, customerService, externalAuthenticationService, eventPublisher, forumService, genericAttributeService, newsService, newsLetterSubscriptionService, productReviewService, productService, gdprConsentRepository, gdprLogRepository, _blogService);
        _customerService = customerService;
		_logger.LogDebug("GdprService(addressService={AddressService},backInStockSubscriptionService={BackInStockSubscriptionService},blogService={BlogService},customerService={CustomerService},externalAuthenticationService={ExternalAuthenticationService},eventPublisher={EventPublisher},forumService={ForumService},genericAttributeService={GenericAttributeService},newsService={NewsService},newsLetterSubscriptionService={NewsLetterSubscriptionService},productReviewService={ProductReviewService},productService={ProductService},gdprConsentRepository={GdprConsentRepository},gdprLogRepository={GdprLogRepository},shoppingCartService=<redacted>): _customerService → {_customerService}", addressService, backInStockSubscriptionService, blogService, customerService, externalAuthenticationService, eventPublisher, forumService, genericAttributeService, newsService, newsLetterSubscriptionService, productReviewService, productService, gdprConsentRepository, gdprLogRepository, _customerService);
        _externalAuthenticationService = externalAuthenticationService;
		_logger.LogDebug("GdprService(addressService={AddressService},backInStockSubscriptionService={BackInStockSubscriptionService},blogService={BlogService},customerService={CustomerService},externalAuthenticationService={ExternalAuthenticationService},eventPublisher={EventPublisher},forumService={ForumService},genericAttributeService={GenericAttributeService},newsService={NewsService},newsLetterSubscriptionService={NewsLetterSubscriptionService},productReviewService={ProductReviewService},productService={ProductService},gdprConsentRepository={GdprConsentRepository},gdprLogRepository={GdprLogRepository},shoppingCartService=<redacted>): _externalAuthenticationService → {_externalAuthenticationService}", addressService, backInStockSubscriptionService, blogService, customerService, externalAuthenticationService, eventPublisher, forumService, genericAttributeService, newsService, newsLetterSubscriptionService, productReviewService, productService, gdprConsentRepository, gdprLogRepository, _externalAuthenticationService);
        _eventPublisher = eventPublisher;
		_logger.LogDebug("GdprService(addressService={AddressService},backInStockSubscriptionService={BackInStockSubscriptionService},blogService={BlogService},customerService={CustomerService},externalAuthenticationService={ExternalAuthenticationService},eventPublisher={EventPublisher},forumService={ForumService},genericAttributeService={GenericAttributeService},newsService={NewsService},newsLetterSubscriptionService={NewsLetterSubscriptionService},productReviewService={ProductReviewService},productService={ProductService},gdprConsentRepository={GdprConsentRepository},gdprLogRepository={GdprLogRepository},shoppingCartService=<redacted>): _eventPublisher → {_eventPublisher}", addressService, backInStockSubscriptionService, blogService, customerService, externalAuthenticationService, eventPublisher, forumService, genericAttributeService, newsService, newsLetterSubscriptionService, productReviewService, productService, gdprConsentRepository, gdprLogRepository, _eventPublisher);
        _forumService = forumService;
		_logger.LogDebug("GdprService(addressService={AddressService},backInStockSubscriptionService={BackInStockSubscriptionService},blogService={BlogService},customerService={CustomerService},externalAuthenticationService={ExternalAuthenticationService},eventPublisher={EventPublisher},forumService={ForumService},genericAttributeService={GenericAttributeService},newsService={NewsService},newsLetterSubscriptionService={NewsLetterSubscriptionService},productReviewService={ProductReviewService},productService={ProductService},gdprConsentRepository={GdprConsentRepository},gdprLogRepository={GdprLogRepository},shoppingCartService=<redacted>): _forumService → {_forumService}", addressService, backInStockSubscriptionService, blogService, customerService, externalAuthenticationService, eventPublisher, forumService, genericAttributeService, newsService, newsLetterSubscriptionService, productReviewService, productService, gdprConsentRepository, gdprLogRepository, _forumService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("GdprService(addressService={AddressService},backInStockSubscriptionService={BackInStockSubscriptionService},blogService={BlogService},customerService={CustomerService},externalAuthenticationService={ExternalAuthenticationService},eventPublisher={EventPublisher},forumService={ForumService},genericAttributeService={GenericAttributeService},newsService={NewsService},newsLetterSubscriptionService={NewsLetterSubscriptionService},productReviewService={ProductReviewService},productService={ProductService},gdprConsentRepository={GdprConsentRepository},gdprLogRepository={GdprLogRepository},shoppingCartService=<redacted>): _genericAttributeService → {_genericAttributeService}", addressService, backInStockSubscriptionService, blogService, customerService, externalAuthenticationService, eventPublisher, forumService, genericAttributeService, newsService, newsLetterSubscriptionService, productReviewService, productService, gdprConsentRepository, gdprLogRepository, _genericAttributeService);
        _newsService = newsService;
		_logger.LogDebug("GdprService(addressService={AddressService},backInStockSubscriptionService={BackInStockSubscriptionService},blogService={BlogService},customerService={CustomerService},externalAuthenticationService={ExternalAuthenticationService},eventPublisher={EventPublisher},forumService={ForumService},genericAttributeService={GenericAttributeService},newsService={NewsService},newsLetterSubscriptionService={NewsLetterSubscriptionService},productReviewService={ProductReviewService},productService={ProductService},gdprConsentRepository={GdprConsentRepository},gdprLogRepository={GdprLogRepository},shoppingCartService=<redacted>): _newsService → {_newsService}", addressService, backInStockSubscriptionService, blogService, customerService, externalAuthenticationService, eventPublisher, forumService, genericAttributeService, newsService, newsLetterSubscriptionService, productReviewService, productService, gdprConsentRepository, gdprLogRepository, _newsService);
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
		_logger.LogDebug("GdprService(addressService={AddressService},backInStockSubscriptionService={BackInStockSubscriptionService},blogService={BlogService},customerService={CustomerService},externalAuthenticationService={ExternalAuthenticationService},eventPublisher={EventPublisher},forumService={ForumService},genericAttributeService={GenericAttributeService},newsService={NewsService},newsLetterSubscriptionService={NewsLetterSubscriptionService},productReviewService={ProductReviewService},productService={ProductService},gdprConsentRepository={GdprConsentRepository},gdprLogRepository={GdprLogRepository},shoppingCartService=<redacted>): _newsLetterSubscriptionService → {_newsLetterSubscriptionService}", addressService, backInStockSubscriptionService, blogService, customerService, externalAuthenticationService, eventPublisher, forumService, genericAttributeService, newsService, newsLetterSubscriptionService, productReviewService, productService, gdprConsentRepository, gdprLogRepository, _newsLetterSubscriptionService);
        _productReviewService = productReviewService;
		_logger.LogDebug("GdprService(addressService={AddressService},backInStockSubscriptionService={BackInStockSubscriptionService},blogService={BlogService},customerService={CustomerService},externalAuthenticationService={ExternalAuthenticationService},eventPublisher={EventPublisher},forumService={ForumService},genericAttributeService={GenericAttributeService},newsService={NewsService},newsLetterSubscriptionService={NewsLetterSubscriptionService},productReviewService={ProductReviewService},productService={ProductService},gdprConsentRepository={GdprConsentRepository},gdprLogRepository={GdprLogRepository},shoppingCartService=<redacted>): _productReviewService → {_productReviewService}", addressService, backInStockSubscriptionService, blogService, customerService, externalAuthenticationService, eventPublisher, forumService, genericAttributeService, newsService, newsLetterSubscriptionService, productReviewService, productService, gdprConsentRepository, gdprLogRepository, _productReviewService);
        _productService = productService;
		_logger.LogDebug("GdprService(addressService={AddressService},backInStockSubscriptionService={BackInStockSubscriptionService},blogService={BlogService},customerService={CustomerService},externalAuthenticationService={ExternalAuthenticationService},eventPublisher={EventPublisher},forumService={ForumService},genericAttributeService={GenericAttributeService},newsService={NewsService},newsLetterSubscriptionService={NewsLetterSubscriptionService},productReviewService={ProductReviewService},productService={ProductService},gdprConsentRepository={GdprConsentRepository},gdprLogRepository={GdprLogRepository},shoppingCartService=<redacted>): _productService → {_productService}", addressService, backInStockSubscriptionService, blogService, customerService, externalAuthenticationService, eventPublisher, forumService, genericAttributeService, newsService, newsLetterSubscriptionService, productReviewService, productService, gdprConsentRepository, gdprLogRepository, _productService);
        _gdprConsentRepository = gdprConsentRepository;
		_logger.LogDebug("GdprService(addressService={AddressService},backInStockSubscriptionService={BackInStockSubscriptionService},blogService={BlogService},customerService={CustomerService},externalAuthenticationService={ExternalAuthenticationService},eventPublisher={EventPublisher},forumService={ForumService},genericAttributeService={GenericAttributeService},newsService={NewsService},newsLetterSubscriptionService={NewsLetterSubscriptionService},productReviewService={ProductReviewService},productService={ProductService},gdprConsentRepository={GdprConsentRepository},gdprLogRepository={GdprLogRepository},shoppingCartService=<redacted>): _gdprConsentRepository → {_gdprConsentRepository}", addressService, backInStockSubscriptionService, blogService, customerService, externalAuthenticationService, eventPublisher, forumService, genericAttributeService, newsService, newsLetterSubscriptionService, productReviewService, productService, gdprConsentRepository, gdprLogRepository, _gdprConsentRepository);
        _gdprLogRepository = gdprLogRepository;
		_logger.LogDebug("GdprService(addressService={AddressService},backInStockSubscriptionService={BackInStockSubscriptionService},blogService={BlogService},customerService={CustomerService},externalAuthenticationService={ExternalAuthenticationService},eventPublisher={EventPublisher},forumService={ForumService},genericAttributeService={GenericAttributeService},newsService={NewsService},newsLetterSubscriptionService={NewsLetterSubscriptionService},productReviewService={ProductReviewService},productService={ProductService},gdprConsentRepository={GdprConsentRepository},gdprLogRepository={GdprLogRepository},shoppingCartService=<redacted>): _gdprLogRepository → {_gdprLogRepository}", addressService, backInStockSubscriptionService, blogService, customerService, externalAuthenticationService, eventPublisher, forumService, genericAttributeService, newsService, newsLetterSubscriptionService, productReviewService, productService, gdprConsentRepository, gdprLogRepository, _gdprLogRepository);
        _shoppingCartService = shoppingCartService;
		_logger.LogDebug("<<< Exiting GdprService(addressService={AddressService},backInStockSubscriptionService={BackInStockSubscriptionService},blogService={BlogService},customerService={CustomerService},externalAuthenticationService={ExternalAuthenticationService},eventPublisher={EventPublisher},forumService={ForumService},genericAttributeService={GenericAttributeService},newsService={NewsService},newsLetterSubscriptionService={NewsLetterSubscriptionService},productReviewService={ProductReviewService},productService={ProductService},gdprConsentRepository={GdprConsentRepository},gdprLogRepository={GdprLogRepository},shoppingCartService=<redacted>)", addressService, backInStockSubscriptionService, blogService, customerService, externalAuthenticationService, eventPublisher, forumService, genericAttributeService, newsService, newsLetterSubscriptionService, productReviewService, productService, gdprConsentRepository, gdprLogRepository);
		_logger.LogDebug("GdprService(addressService={AddressService},backInStockSubscriptionService={BackInStockSubscriptionService},blogService={BlogService},customerService={CustomerService},externalAuthenticationService={ExternalAuthenticationService},eventPublisher={EventPublisher},forumService={ForumService},genericAttributeService={GenericAttributeService},newsService={NewsService},newsLetterSubscriptionService={NewsLetterSubscriptionService},productReviewService={ProductReviewService},productService={ProductService},gdprConsentRepository={GdprConsentRepository},gdprLogRepository={GdprLogRepository},shoppingCartService=<redacted>): _shoppingCartService → {_shoppingCartService}", addressService, backInStockSubscriptionService, blogService, customerService, externalAuthenticationService, eventPublisher, forumService, genericAttributeService, newsService, newsLetterSubscriptionService, productReviewService, productService, gdprConsentRepository, gdprLogRepository, _shoppingCartService);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Insert a GDPR log
    /// </summary>
    /// <param name="gdprLog">GDPR log</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InsertLogAsync(GdprLog gdprLog)
    {
		_logger.LogDebug(">>> Entering InsertLogAsync(gdprLog={GdprLog})", gdprLog);
        await _gdprLogRepository.InsertAsync(gdprLog);
		_logger.LogDebug("<<< Exiting InsertLogAsync(gdprLog={GdprLog})", gdprLog);
    }

    #endregion

    #region Methods

    #region GDPR consent

    /// <summary>
    /// Get a GDPR consent
    /// </summary>
    /// <param name="gdprConsentId">The GDPR consent identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gDPR consent
    /// </returns>
    public virtual async Task<GdprConsent> GetConsentByIdAsync(int gdprConsentId)
    {
		_logger.LogDebug(">>> Entering GetConsentByIdAsync(gdprConsentId={GdprConsentId})", gdprConsentId);
		_logger.LogDebug("<<< Exiting GetConsentByIdAsync(gdprConsentId={GdprConsentId})", gdprConsentId);
        return await _gdprConsentRepository.GetByIdAsync(gdprConsentId, cache => default);
    }

    /// <summary>
    /// Get all GDPR consents
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gDPR consent
    /// </returns>
    public virtual async Task<IList<GdprConsent>> GetAllConsentsAsync()
    {
		_logger.LogDebug(">>> Entering GetAllConsentsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var gdprConsents = await _gdprConsentRepository.GetAllAsync(query =>
        {
            return from c in query
                orderby c.DisplayOrder, c.Id
                select c;
				_logger.LogInformation("GetAllConsentsAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

		_logger.LogDebug("<<< Exiting GetAllConsentsAsync()");
        return gdprConsents;
    }

    /// <summary>
    /// Insert a GDPR consent
    /// </summary>
    /// <param name="gdprConsent">GDPR consent</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertConsentAsync(GdprConsent gdprConsent)
    {
		_logger.LogDebug(">>> Entering InsertConsentAsync(gdprConsent={GdprConsent})", gdprConsent);
        await _gdprConsentRepository.InsertAsync(gdprConsent);
		_logger.LogDebug("<<< Exiting InsertConsentAsync(gdprConsent={GdprConsent})", gdprConsent);
    }

    /// <summary>
    /// Update the GDPR consent
    /// </summary>
    /// <param name="gdprConsent">GDPR consent</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateConsentAsync(GdprConsent gdprConsent)
    {
		_logger.LogDebug(">>> Entering UpdateConsentAsync(gdprConsent={GdprConsent})", gdprConsent);
        await _gdprConsentRepository.UpdateAsync(gdprConsent);
		_logger.LogDebug("<<< Exiting UpdateConsentAsync(gdprConsent={GdprConsent})", gdprConsent);
    }

    /// <summary>
    /// Delete a GDPR consent
    /// </summary>
    /// <param name="gdprConsent">GDPR consent</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteConsentAsync(GdprConsent gdprConsent)
    {
		_logger.LogDebug(">>> Entering DeleteConsentAsync(gdprConsent={GdprConsent})", gdprConsent);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _gdprConsentRepository.DeleteAsync(gdprConsent);
		_logger.LogInformation("DeleteConsentAsync(gdprConsent)={GdprConsent}: external service call took {Arg1} ms", gdprConsent, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting DeleteConsentAsync(gdprConsent={GdprConsent})", gdprConsent);
    }

    /// <summary>
    /// Gets the latest selected value (a consent is accepted or not by a customer)
    /// </summary>
    /// <param name="consentId">Consent identifier</param>
    /// <param name="customerId">Customer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result; null if previous a customer hasn't been asked
    /// </returns>
    public virtual async Task<bool?> IsConsentAcceptedAsync(int consentId, int customerId)
    {
		_logger.LogDebug(">>> Entering IsConsentAcceptedAsync(consentId={ConsentId},customerId={CustomerId})", consentId, customerId);
        //get latest record
        var log = (await GetAllLogAsync(customerId: customerId, consentId: consentId, pageIndex: 0, pageSize: 1)).FirstOrDefault();
		_logger.LogDebug("IsConsentAcceptedAsync(consentId={ConsentId},customerId={CustomerId}): log → {Log}", consentId, customerId, log);
        if (log == null)
            return null;

        return log.RequestType switch
        {
            GdprRequestType.ConsentAgree => true,
            GdprRequestType.ConsentDisagree => false,
            _ => null,
        };
    }

    #endregion

    #region GDPR log

    /// <summary>
    /// Get all GDPR log records
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="consentId">Consent identifier</param>
    /// <param name="customerInfo">Customer info (Exact match)</param>
    /// <param name="requestType">GDPR request type</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gDPR log records
    /// </returns>
    public virtual async Task<IPagedList<GdprLog>> GetAllLogAsync(int customerId = 0, int consentId = 0,
        string customerInfo = "", GdprRequestType? requestType = null,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering GetAllLogAsync(customerId={CustomerId},consentId={ConsentId},customerInfo={CustomerInfo},requestType={RequestType},pageIndex={PageIndex},pageSize={PageSize})", customerId, consentId, customerInfo, requestType, pageIndex, pageSize);
        return await _gdprLogRepository.GetAllPagedAsync(query =>
        {
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogInformation("GetAllLogAsync(customerId,consentId,customerInfo,requestType,pageIndex,pageSize)={CustomerId},{ConsentId},{CustomerInfo},{RequestType},{PageIndex},{PageSize}: find query executed in {Arg6} ms", customerId, consentId, customerInfo, requestType, pageIndex, pageSize, __logStart.ElapsedMilliseconds);
            if (customerId > 0)
                query = query.Where(log => log.CustomerId == customerId);

            if (consentId > 0)
                query = query.Where(log => log.ConsentId == consentId);

            if (!string.IsNullOrEmpty(customerInfo))
                query = query.Where(log => log.CustomerInfo == customerInfo);

            if (requestType != null)
            {
                var requestTypeId = (int)requestType;
                query = query.Where(log => log.RequestTypeId == requestTypeId);
            }

            query = query.OrderByDescending(log => log.CreatedOnUtc).ThenByDescending(log => log.Id);

            return query;
        }, pageIndex, pageSize);
    }

    /// <summary>
    /// Insert a GDPR log
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="consentId">Consent identifier</param>
    /// <param name="requestType">Request type</param>
    /// <param name="requestDetails">Request details</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertLogAsync(Customer customer, int consentId, GdprRequestType requestType, string requestDetails)
    {
		_logger.LogDebug(">>> Entering InsertLogAsync(customer={Customer},consentId={ConsentId},requestType={RequestType},requestDetails={RequestDetails})", customer, consentId, requestType, requestDetails);
        ArgumentNullException.ThrowIfNull(customer);

        var gdprLog = new GdprLog
        {
            CustomerId = customer.Id,
            ConsentId = consentId,
            CustomerInfo = customer.Email,
            RequestType = requestType,
            RequestDetails = requestDetails,
            CreatedOnUtc = DateTime.UtcNow
        };

        await InsertLogAsync(gdprLog);
		_logger.LogDebug("<<< Exiting InsertLogAsync(customer={Customer},consentId={ConsentId},requestType={RequestType},requestDetails={RequestDetails})", customer, consentId, requestType, requestDetails);
    }

    #endregion

    #region Customer

    /// <summary>
    /// Permanent delete of customer
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PermanentDeleteCustomerAsync(Customer customer)
    {
		_logger.LogDebug(">>> Entering PermanentDeleteCustomerAsync(customer={Customer})", customer);
        ArgumentNullException.ThrowIfNull(customer);

        //blog comments
        var blogComments = await _blogService.GetAllCommentsAsync(customerId: customer.Id);
		_logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): blogComments → {BlogComments}", customer, blogComments);
        await _blogService.DeleteBlogCommentsAsync(blogComments);

        //news comments
        var newsComments = await _newsService.GetAllCommentsAsync(customerId: customer.Id);
		_logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): newsComments → {NewsComments}", customer, newsComments);
        await _newsService.DeleteNewsCommentsAsync(newsComments);

        //back in stock subscriptions
        var backInStockSubscriptions = await _backInStockSubscriptionService.GetAllSubscriptionsByCustomerIdAsync(customer.Id);
		_logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): backInStockSubscriptions → {BackInStockSubscriptions}", customer, backInStockSubscriptions);
        foreach (var backInStockSubscription in backInStockSubscriptions)
            await _backInStockSubscriptionService.DeleteSubscriptionAsync(backInStockSubscription);

        //product review
        var productReviews = await _productReviewService.GetAllProductReviewsAsync(customer.Id);
		_logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): productReviews → {ProductReviews}", customer, productReviews);
        var reviewedProducts = await _productService.GetProductsByIdsAsync(productReviews.Select(p => p.ProductId).Distinct().ToArray());
		_logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): reviewedProducts → {ReviewedProducts}", customer, reviewedProducts);
        await _productReviewService.DeleteProductReviewsAsync(productReviews);
        //update product totals
        foreach (var product in reviewedProducts)
            await _productReviewService.UpdateProductReviewTotalsAsync(product);

        //external authentication record
        foreach (var ear in await _externalAuthenticationService.GetCustomerExternalAuthenticationRecordsAsync(customer))
            await _externalAuthenticationService.DeleteExternalAuthenticationRecordAsync(ear);

        //forum subscriptions
        var forumSubscriptions = await _forumService.GetAllSubscriptionsAsync(customer.Id);
		_logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): forumSubscriptions → {ForumSubscriptions}", customer, forumSubscriptions);
        foreach (var forumSubscription in forumSubscriptions)
            await _forumService.DeleteSubscriptionAsync(forumSubscription);

        //shopping cart items
        foreach (var sci in await _shoppingCartService.GetShoppingCartAsync(customer))
            await _shoppingCartService.DeleteShoppingCartItemAsync(sci);

        //private messages (sent)
        foreach (var pm in await _forumService.GetAllPrivateMessagesAsync(0, customer.Id, 0, null, null, null, null))
            await _forumService.DeletePrivateMessageAsync(pm);

        //private messages (received)
        foreach (var pm in await _forumService.GetAllPrivateMessagesAsync(0, 0, customer.Id, null, null, null, null))
            await _forumService.DeletePrivateMessageAsync(pm);

        //newsletter
        var newsletters = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionsByEmailAsync(customer.Email);
		_logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): newsletters → {Newsletters}", customer, newsletters);
        foreach (var newsletter in newsletters)
            await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(newsletter);

        //addresses
        foreach (var address in await _customerService.GetAddressesByCustomerIdAsync(customer.Id))
        {
            await _customerService.RemoveCustomerAddressAsync(customer, address);
            await _customerService.UpdateCustomerAsync(customer);
            //now delete the address record
            await _addressService.DeleteAddressAsync(address);
        }

        //generic attributes
        var keyGroup = customer.GetType().Name;
		_logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): keyGroup → {KeyGroup}", customer, keyGroup);
        var genericAttributes = await _genericAttributeService.GetAttributesForEntityAsync(customer.Id, keyGroup);
		_logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): genericAttributes → {GenericAttributes}", customer, genericAttributes);
        await _genericAttributeService.DeleteAttributesAsync(genericAttributes);

        //ignore ActivityLog
        //ignore ForumPost, ForumTopic, ignore ForumPostVote
        //ignore Log
        //ignore PollVotingRecord
        //ignore ProductReviewHelpfulness
        //ignore RecurringPayment 
        //ignore ReturnRequest
        //ignore RewardPointsHistory
        //and we do not delete orders

        //remove from Registered role, add to Guest one
        if (await _customerService.IsRegisteredAsync(customer))
        {
            var registeredRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
			_logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): registeredRole → {RegisteredRole}", customer, registeredRole);
            await _customerService.RemoveCustomerRoleMappingAsync(customer, registeredRole);
        }

        if (!await _customerService.IsGuestAsync(customer))
        {
            var guestRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GuestsRoleName);
			_logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): guestRole → {GuestRole}", customer, guestRole);
            await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = guestRole.Id });
        }

        var email = customer.Email;
		_logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): email → {Email}", customer, email);

        //clear other information
        customer.Email = string.Empty;
		_logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): customer.Email → {Arg1}", customer, customer.Email);
		_logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): customer.EmailToRevalidate → {Arg1}", customer, customer.EmailToRevalidate);
		_logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): customer.Username → {Arg1}", customer, customer.Username);
		_logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): customer.Active → {Arg1}", customer, customer.Active);
		_logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): customer.Deleted → {Arg1}", customer, customer.Deleted);
        customer.EmailToRevalidate = string.Empty;
        customer.Username = string.Empty;
        customer.Active = false;
        customer.Deleted = true;

        await _customerService.UpdateCustomerAsync(customer);

        //raise event
        await _eventPublisher.PublishAsync(new CustomerPermanentlyDeleted(customer.Id, email));
		_logger.LogDebug("<<< Exiting PermanentDeleteCustomerAsync(customer={Customer})", customer);
    }

    #endregion

    #endregion
}