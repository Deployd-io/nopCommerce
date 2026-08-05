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

namespace Nop.Services.Gdpr;

/// <summary>
/// Represents the GDPR service
/// </summary>
public partial class GdprService : IGdprService
{
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
        _addressService = addressService;
        _backInStockSubscriptionService = backInStockSubscriptionService;
        _blogService = blogService;
        _customerService = customerService;
        _externalAuthenticationService = externalAuthenticationService;
        _eventPublisher = eventPublisher;
        _forumService = forumService;
        _genericAttributeService = genericAttributeService;
        _newsService = newsService;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _productReviewService = productReviewService;
        _productService = productService;
        _gdprConsentRepository = gdprConsentRepository;
        _gdprLogRepository = gdprLogRepository;
        _shoppingCartService = shoppingCartService;
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
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _gdprLogRepository.InsertAsync(gdprLog);
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
		logger.LogDebug(">>> Entering GetConsentByIdAsync(gdprConsentId={GdprConsentId})", gdprConsentId);
		logger.LogDebug("<<< Exiting GetConsentByIdAsync(gdprConsentId={GdprConsentId})", gdprConsentId);
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
		logger.LogDebug(">>> Entering GetAllConsentsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var gdprConsents = await _gdprConsentRepository.GetAllAsync(query =>
        {
            return from c in query
                orderby c.DisplayOrder, c.Id
                select c;
				logger.LogInformation("GetAllConsentsAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

		logger.LogDebug("<<< Exiting GetAllConsentsAsync()");
        return gdprConsents;
    }

    /// <summary>
    /// Insert a GDPR consent
    /// </summary>
    /// <param name="gdprConsent">GDPR consent</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertConsentAsync(GdprConsent gdprConsent)
    {
		logger.LogDebug(">>> Entering InsertConsentAsync(gdprConsent={GdprConsent})", gdprConsent);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _gdprConsentRepository.InsertAsync(gdprConsent);
		logger.LogDebug("<<< Exiting InsertConsentAsync(gdprConsent={GdprConsent})", gdprConsent);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertConsentAsync(gdprConsent)={GdprConsent}: save query executed in {Arg1} ms", gdprConsent, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertConsentAsync(gdprConsent)={GdprConsent}: save query executed in {Arg1} ms", gdprConsent, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Update the GDPR consent
    /// </summary>
    /// <param name="gdprConsent">GDPR consent</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateConsentAsync(GdprConsent gdprConsent)
    {
		logger.LogDebug(">>> Entering UpdateConsentAsync(gdprConsent={GdprConsent})", gdprConsent);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _gdprConsentRepository.UpdateAsync(gdprConsent);
		logger.LogDebug("<<< Exiting UpdateConsentAsync(gdprConsent={GdprConsent})", gdprConsent);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateConsentAsync(gdprConsent)={GdprConsent}: update query executed in {Arg1} ms", gdprConsent, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateConsentAsync(gdprConsent)={GdprConsent}: update query executed in {Arg1} ms", gdprConsent, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Delete a GDPR consent
    /// </summary>
    /// <param name="gdprConsent">GDPR consent</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteConsentAsync(GdprConsent gdprConsent)
    {
		logger.LogDebug(">>> Entering DeleteConsentAsync(gdprConsent={GdprConsent})", gdprConsent);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _gdprConsentRepository.DeleteAsync(gdprConsent);
		logger.LogDebug("<<< Exiting DeleteConsentAsync(gdprConsent={GdprConsent})", gdprConsent);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteConsentAsync(gdprConsent)={GdprConsent}: external service call took {Arg1} ms", gdprConsent, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteConsentAsync(gdprConsent)={GdprConsent}: delete query executed in {Arg1} ms", gdprConsent, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteConsentAsync(gdprConsent)={GdprConsent}: external service call took {Arg1} ms", gdprConsent, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteConsentAsync(gdprConsent)={GdprConsent}: delete query executed in {Arg1} ms", gdprConsent, __logStart.ElapsedMilliseconds);
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
		logger.LogDebug(">>> Entering IsConsentAcceptedAsync(consentId={ConsentId},customerId={CustomerId})", consentId, customerId);
        //get latest record
        var log = (await GetAllLogAsync(customerId: customerId, consentId: consentId, pageIndex: 0, pageSize: 1)).FirstOrDefault();
        if (log == null)
			logger.LogDebug("IsConsentAcceptedAsync(consentId={ConsentId},customerId={CustomerId}): log → {Log}", consentId, customerId, log);
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
        return await _gdprLogRepository.GetAllPagedAsync(query =>
        {
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
		logger.LogDebug(">>> Entering InsertLogAsync(customer={Customer},consentId={ConsentId},requestType={RequestType},requestDetails={RequestDetails})", customer, consentId, requestType, requestDetails);
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
		logger.LogDebug("<<< Exiting InsertLogAsync(customer={Customer},consentId={ConsentId},requestType={RequestType},requestDetails={RequestDetails})", customer, consentId, requestType, requestDetails);
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
		logger.LogDebug(">>> Entering PermanentDeleteCustomerAsync(customer={Customer})", customer);
        ArgumentNullException.ThrowIfNull(customer);

        //blog comments
        var blogComments = await _blogService.GetAllCommentsAsync(customerId: customer.Id);
        await _blogService.DeleteBlogCommentsAsync(blogComments);
		logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): blogComments → {BlogComments}", customer, blogComments);

        //news comments
        var newsComments = await _newsService.GetAllCommentsAsync(customerId: customer.Id);
        await _newsService.DeleteNewsCommentsAsync(newsComments);
		logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): newsComments → {NewsComments}", customer, newsComments);

        //back in stock subscriptions
        var backInStockSubscriptions = await _backInStockSubscriptionService.GetAllSubscriptionsByCustomerIdAsync(customer.Id);
        foreach (var backInStockSubscription in backInStockSubscriptions)
            await _backInStockSubscriptionService.DeleteSubscriptionAsync(backInStockSubscription);
			logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): backInStockSubscriptions → {BackInStockSubscriptions}", customer, backInStockSubscriptions);

        //product review
        var productReviews = await _productReviewService.GetAllProductReviewsAsync(customer.Id);
        var reviewedProducts = await _productService.GetProductsByIdsAsync(productReviews.Select(p => p.ProductId).Distinct().ToArray());
		logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): productReviews → {ProductReviews}", customer, productReviews);
		logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): reviewedProducts → {ReviewedProducts}", customer, reviewedProducts);
        await _productReviewService.DeleteProductReviewsAsync(productReviews);
        //update product totals
        foreach (var product in reviewedProducts)
            await _productReviewService.UpdateProductReviewTotalsAsync(product);

        //external authentication record
        foreach (var ear in await _externalAuthenticationService.GetCustomerExternalAuthenticationRecordsAsync(customer))
            await _externalAuthenticationService.DeleteExternalAuthenticationRecordAsync(ear);

        //forum subscriptions
        var forumSubscriptions = await _forumService.GetAllSubscriptionsAsync(customer.Id);
        foreach (var forumSubscription in forumSubscriptions)
            await _forumService.DeleteSubscriptionAsync(forumSubscription);
			logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): forumSubscriptions → {ForumSubscriptions}", customer, forumSubscriptions);

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
        foreach (var newsletter in newsletters)
            await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(newsletter);
			logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): newsletters → {Newsletters}", customer, newsletters);

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
        var genericAttributes = await _genericAttributeService.GetAttributesForEntityAsync(customer.Id, keyGroup);
		logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): keyGroup → {KeyGroup}", customer, keyGroup);
		logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): genericAttributes → {GenericAttributes}", customer, genericAttributes);
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
            await _customerService.RemoveCustomerRoleMappingAsync(customer, registeredRole);
			logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): registeredRole → {RegisteredRole}", customer, registeredRole);
        }

        if (!await _customerService.IsGuestAsync(customer))
        {
            var guestRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GuestsRoleName);
            await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = guestRole.Id });
			logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): guestRole → {GuestRole}", customer, guestRole);
        }

        var email = customer.Email;

		logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): email → {Email}", customer, email);
        //clear other information
        customer.Email = string.Empty;
		logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): customer.Email → {Arg1}", customer, customer.Email);
		logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): customer.EmailToRevalidate → {Arg1}", customer, customer.EmailToRevalidate);
		logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): customer.Username → {Arg1}", customer, customer.Username);
		logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): customer.Active → {Arg1}", customer, customer.Active);
		logger.LogDebug("PermanentDeleteCustomerAsync(customer={Customer}): customer.Deleted → {Arg1}", customer, customer.Deleted);
        customer.EmailToRevalidate = string.Empty;
        customer.Username = string.Empty;
        customer.Active = false;
        customer.Deleted = true;

        await _customerService.UpdateCustomerAsync(customer);

        //raise event
        await _eventPublisher.PublishAsync(new CustomerPermanentlyDeleted(customer.Id, email));
		logger.LogDebug("<<< Exiting PermanentDeleteCustomerAsync(customer={Customer})", customer);
    }

    #endregion

    #endregion
}