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
        _customerRepository = customerRepository;
		_logger.LogDebug("BackInStockSubscriptionService(backInStockSubscriptionRepository={BackInStockSubscriptionRepository},customerRepository={CustomerRepository},productRepository={ProductRepository},workflowMessageService={WorkflowMessageService}): _customerRepository → {_customerRepository}", backInStockSubscriptionRepository, customerRepository, productRepository, workflowMessageService, _customerRepository);
		_logger.LogDebug("BackInStockSubscriptionService(backInStockSubscriptionRepository={BackInStockSubscriptionRepository},customerRepository={CustomerRepository},productRepository={ProductRepository},workflowMessageService={WorkflowMessageService}): _backInStockSubscriptionRepository → {_backInStockSubscriptionRepository}", backInStockSubscriptionRepository, customerRepository, productRepository, workflowMessageService, _backInStockSubscriptionRepository);
        _productRepository = productRepository;
		_logger.LogDebug("BackInStockSubscriptionService(backInStockSubscriptionRepository={BackInStockSubscriptionRepository},customerRepository={CustomerRepository},productRepository={ProductRepository},workflowMessageService={WorkflowMessageService}): _productRepository → {_productRepository}", backInStockSubscriptionRepository, customerRepository, productRepository, workflowMessageService, _productRepository);
        _workflowMessageService = workflowMessageService;
		_logger.LogDebug("<<< Exiting BackInStockSubscriptionService(backInStockSubscriptionRepository={BackInStockSubscriptionRepository},customerRepository={CustomerRepository},productRepository={ProductRepository},workflowMessageService={WorkflowMessageService})", backInStockSubscriptionRepository, customerRepository, productRepository, workflowMessageService);
		_logger.LogDebug("BackInStockSubscriptionService(backInStockSubscriptionRepository={BackInStockSubscriptionRepository},customerRepository={CustomerRepository},productRepository={ProductRepository},workflowMessageService={WorkflowMessageService}): _workflowMessageService → {_workflowMessageService}", backInStockSubscriptionRepository, customerRepository, productRepository, workflowMessageService, _workflowMessageService);
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
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
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
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return await _backInStockSubscriptionRepository.GetAllPagedAsync(query =>
        {
            //customer
            query = query.Where(biss => biss.CustomerId == customerId);
			_logger.LogInformation("GetAllSubscriptionsByCustomerIdAsync(customerId,storeId,pageIndex,pageSize)={CustomerId},{StoreId},{PageIndex},{PageSize}: find query executed in {Arg4} ms", customerId, storeId, pageIndex, pageSize, __logStart.ElapsedMilliseconds);

            //store
            if (storeId > 0)
                query = query.Where(biss => biss.StoreId == storeId);

            //product
            query = from q in query
				_logger.LogDebug("GetAllSubscriptionsByCustomerIdAsync(customerId={CustomerId},storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", customerId, storeId, pageIndex, pageSize, query);
                join p in _productRepository.Table on q.ProductId equals p.Id
                where !p.Deleted
                select q;

            query = query.OrderByDescending(biss => biss.CreatedOnUtc);

            return query;
        }, pageIndex, pageSize);
		_logger.LogDebug("<<< Exiting GetAllSubscriptionsByCustomerIdAsync(customerId={CustomerId},storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize})", customerId, storeId, pageIndex, pageSize);
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
        var query = from biss in _backInStockSubscriptionRepository.Table
            orderby biss.CreatedOnUtc descending
            where biss.CustomerId == customerId &&
                  biss.ProductId == productId &&
                  biss.StoreId == storeId
            select biss;

        var subscription = await query.FirstOrDefaultAsync();

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
		_logger.LogDebug("<<< Exiting GetSubscriptionByIdAsync(subscriptionId={SubscriptionId})", subscriptionId);
        return await _backInStockSubscriptionRepository.GetByIdAsync(subscriptionId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Inserts subscription
    /// </summary>
    /// <param name="subscription">Subscription</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertSubscriptionAsync(BackInStockSubscription subscription)
    {
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
			_logger.LogDebug("SendNotificationsToSubscribersAsync(product={Product}): customer → {Customer}", product, customer);
			_logger.LogInformation("SendNotificationsToSubscribersAsync(product)={Product}: find query executed in {Arg1} ms", product, __logStart.ElapsedMilliseconds);
			_logger.LogDebug("<<< Exiting GetAllSubscriptionsByProductIdAsync(productId={ProductId},storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize})", productId, storeId, pageIndex, pageSize);
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
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return await _backInStockSubscriptionRepository.GetAllPagedAsync(query =>
        {
            //product
            query = query.Where(biss => biss.ProductId == productId);
			_logger.LogInformation("GetAllSubscriptionsByProductIdAsync(productId,storeId,pageIndex,pageSize)={ProductId},{StoreId},{PageIndex},{PageSize}: find query executed in {Arg4} ms", productId, storeId, pageIndex, pageSize, __logStart.ElapsedMilliseconds);
            //store
            if (storeId > 0)
                query = query.Where(biss => biss.StoreId == storeId);
            //customer
            query = from biss in query
				_logger.LogDebug("GetAllSubscriptionsByProductIdAsync(productId={ProductId},storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", productId, storeId, pageIndex, pageSize, query);
                join c in _customerRepository.Table on biss.CustomerId equals c.Id
                where c.Active && !c.Deleted
                select biss;

            query = query.OrderByDescending(biss => biss.CreatedOnUtc);

            return query;
        }, pageIndex, pageSize);
    }

    #endregion
}