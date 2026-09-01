using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Messages;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog;

/// <summary>
/// Back in stock subscription service
/// </summary>
public partial class BackInStockSubscriptionService : IBackInStockSubscriptionService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<BackInStockSubscription> _backInStockSubscriptionRepository;
    protected readonly IRepository<Customer> _customerRepository;
    protected readonly IRepository<Product> _productRepository;
    protected readonly IWorkflowMessageService _workflowMessageService;

    #endregion

    #region Ctor

    public BackInStockSubscriptionService(IRepository<BackInStockSubscription> backInStockSubscriptionRepository,
        IRepository<Customer> customerRepository,
        IRepository<Product> productRepository,
        IWorkflowMessageService workflowMessageService)
    {
		_logger.LogDebug(">>> Entering BackInStockSubscriptionService(backInStockSubscriptionRepository={BackInStockSubscriptionRepository},customerRepository={CustomerRepository},productRepository={ProductRepository},workflowMessageService={WorkflowMessageService})", backInStockSubscriptionRepository, customerRepository, productRepository, workflowMessageService);
        _backInStockSubscriptionRepository = backInStockSubscriptionRepository;
		_logger.LogDebug("BackInStockSubscriptionService(backInStockSubscriptionRepository={BackInStockSubscriptionRepository},customerRepository={CustomerRepository},productRepository={ProductRepository},workflowMessageService={WorkflowMessageService}): _backInStockSubscriptionRepository → {_backInStockSubscriptionRepository}", backInStockSubscriptionRepository, customerRepository, productRepository, workflowMessageService, _backInStockSubscriptionRepository);
        _customerRepository = customerRepository;
		_logger.LogDebug("BackInStockSubscriptionService(backInStockSubscriptionRepository={BackInStockSubscriptionRepository},customerRepository={CustomerRepository},productRepository={ProductRepository},workflowMessageService={WorkflowMessageService}): _customerRepository → {_customerRepository}", backInStockSubscriptionRepository, customerRepository, productRepository, workflowMessageService, _customerRepository);
        _productRepository = productRepository;
		_logger.LogDebug("BackInStockSubscriptionService(backInStockSubscriptionRepository={BackInStockSubscriptionRepository},customerRepository={CustomerRepository},productRepository={ProductRepository},workflowMessageService={WorkflowMessageService}): _productRepository → {_productRepository}", backInStockSubscriptionRepository, customerRepository, productRepository, workflowMessageService, _productRepository);
        _workflowMessageService = workflowMessageService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Delete a back in stock subscription
    /// </summary>
    /// <param name="subscription">Subscription</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteSubscriptionAsync(BackInStockSubscription subscription)
    {
		_logger.LogDebug(">>> Entering DeleteSubscriptionAsync(subscription={Subscription})", subscription);
        await _backInStockSubscriptionRepository.DeleteAsync(subscription);
    }

    /// <summary>
    /// Gets all subscriptions
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the subscriptions
    /// </returns>
    public virtual async Task<IPagedList<BackInStockSubscription>> GetAllSubscriptionsByCustomerIdAsync(int customerId,
        int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering GetAllSubscriptionsByCustomerIdAsync(customerId={CustomerId},storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize})", customerId, storeId, pageIndex, pageSize);
        return await _backInStockSubscriptionRepository.GetAllPagedAsync(query =>
        {
			_logger.LogInformation("GetAllSubscriptionsByCustomerIdAsync(customerId,storeId,pageIndex,pageSize)={CustomerId},{StoreId},{PageIndex},{PageSize}: find query executing", customerId, storeId, pageIndex, pageSize);
            //customer
            query = query.Where(biss => biss.CustomerId == customerId);

            //store
            if (storeId > 0)
                query = query.Where(biss => biss.StoreId == storeId);

            //product
            query = from q in query
                join p in _productRepository.Table on q.ProductId equals p.Id
                where !p.Deleted
                select q;

            query = query.OrderByDescending(biss => biss.CreatedOnUtc);

		_logger.LogDebug("<<< Exiting GetAllSubscriptionsByCustomerIdAsync(customerId={CustomerId},storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize})", customerId, storeId, pageIndex, pageSize);
            return query;
        }, pageIndex, pageSize);
    }

    /// <summary>
    /// Gets all subscriptions
    /// </summary>
    /// <param name="customerId">Customer id</param>
    /// <param name="productId">Product identifier</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the subscriptions
    /// </returns>
    public virtual async Task<BackInStockSubscription> FindSubscriptionAsync(int customerId, int productId, int storeId)
    {
		_logger.LogDebug(">>> Entering FindSubscriptionAsync(customerId={CustomerId},productId={ProductId},storeId={StoreId})", customerId, productId, storeId);
        var query = from biss in _backInStockSubscriptionRepository.Table
            orderby biss.CreatedOnUtc descending
            where biss.CustomerId == customerId &&
                  biss.ProductId == productId &&
                  biss.StoreId == storeId
            select biss;

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var subscription = await query.FirstOrDefaultAsync();

		_logger.LogDebug("FindSubscriptionAsync(customerId={CustomerId},productId={ProductId},storeId={StoreId}): subscription → {Subscription}", customerId, productId, storeId, subscription);
		_logger.LogInformation("FindSubscriptionAsync(customerId,productId,storeId)={CustomerId},{ProductId},{StoreId}: database query executed in {Arg3} ms", customerId, productId, storeId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting FindSubscriptionAsync(customerId={CustomerId},productId={ProductId},storeId={StoreId})", customerId, productId, storeId);
        return subscription;
    }

    /// <summary>
    /// Gets a subscription
    /// </summary>
    /// <param name="subscriptionId">Subscription identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the subscription
    /// </returns>
    public virtual async Task<BackInStockSubscription> GetSubscriptionByIdAsync(int subscriptionId)
    {
		_logger.LogDebug(">>> Entering GetSubscriptionByIdAsync(subscriptionId={SubscriptionId})", subscriptionId);
		_logger.LogInformation("GetSubscriptionByIdAsync(subscriptionId)={SubscriptionId}: find query executing", subscriptionId);
        return await _backInStockSubscriptionRepository.GetByIdAsync(subscriptionId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Inserts subscription
    /// </summary>
    /// <param name="subscription">Subscription</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertSubscriptionAsync(BackInStockSubscription subscription)
    {
		_logger.LogDebug(">>> Entering InsertSubscriptionAsync(subscription={Subscription})", subscription);
        await _backInStockSubscriptionRepository.InsertAsync(subscription);
    }

    /// <summary>
    /// Send notification to subscribers
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of sent email
    /// </returns>
    public virtual async Task<int> SendNotificationsToSubscribersAsync(Product product)
    {
		_logger.LogDebug(">>> Entering SendNotificationsToSubscribersAsync(product={Product})", product);
        ArgumentNullException.ThrowIfNull(product);

        var result = 0;
		_logger.LogDebug("SendNotificationsToSubscribersAsync(product={Product}): result → {Result}", product, result);
        var subscriptions = await GetAllSubscriptionsByProductIdAsync(product.Id);
		_logger.LogDebug("SendNotificationsToSubscribersAsync(product={Product}): subscriptions → {Subscriptions}", product, subscriptions);
        foreach (var subscription in subscriptions)
        {
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            var customer = await _customerRepository.GetByIdAsync(subscription.CustomerId);
            result += (await _workflowMessageService.SendBackInStockNotificationAsync(subscription, customer?.LanguageId ?? 0)).Count;
			_logger.LogInformation("SendNotificationsToSubscribersAsync(product)={Product}: find query executed in {Arg1} ms", product, __logStart.ElapsedMilliseconds);
			_logger.LogDebug("SendNotificationsToSubscribersAsync(product={Product}): customer → {Customer}", product, customer);
        }

        for (var i = 0; i <= subscriptions.Count - 1; i++)
            await DeleteSubscriptionAsync(subscriptions[i]);

		_logger.LogDebug("<<< Exiting SendNotificationsToSubscribersAsync(product={Product})", product);
        return result;
    }

    /// <summary>
    /// Gets all subscriptions
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the subscriptions
    /// </returns>
    public virtual async Task<IPagedList<BackInStockSubscription>> GetAllSubscriptionsByProductIdAsync(int productId,
        int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering GetAllSubscriptionsByProductIdAsync(productId={ProductId},storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize})", productId, storeId, pageIndex, pageSize);
        return await _backInStockSubscriptionRepository.GetAllPagedAsync(query =>
        {
			_logger.LogInformation("GetAllSubscriptionsByProductIdAsync(productId,storeId,pageIndex,pageSize)={ProductId},{StoreId},{PageIndex},{PageSize}: find query executing", productId, storeId, pageIndex, pageSize);
            //product
            query = query.Where(biss => biss.ProductId == productId);
            //store
            if (storeId > 0)
                query = query.Where(biss => biss.StoreId == storeId);
            //customer
            query = from biss in query
                join c in _customerRepository.Table on biss.CustomerId equals c.Id
                where c.Active && !c.Deleted
                select biss;

            query = query.OrderByDescending(biss => biss.CreatedOnUtc);

            return query;
		_logger.LogDebug("<<< Exiting GetAllSubscriptionsByProductIdAsync(productId={ProductId},storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize})", productId, storeId, pageIndex, pageSize);
        }, pageIndex, pageSize);
    }

    #endregion
}