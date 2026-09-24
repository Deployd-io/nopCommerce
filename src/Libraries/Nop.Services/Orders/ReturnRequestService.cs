using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Orders;

/// <summary>
/// Return request service
/// </summary>
public partial class ReturnRequestService : IReturnRequestService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<ReturnRequest> _returnRequestRepository;
    protected readonly IRepository<ReturnRequestAction> _returnRequestActionRepository;
    protected readonly IRepository<ReturnRequestReason> _returnRequestReasonRepository;
    protected readonly IRepository<OrderItem> _orderItemRepository;
    protected readonly IRepository<Product> _productRepository;

    #endregion

    #region Ctor

    public ReturnRequestService(IRepository<ReturnRequest> returnRequestRepository,
        IRepository<ReturnRequestAction> returnRequestActionRepository,
        IRepository<ReturnRequestReason> returnRequestReasonRepository,
        IRepository<OrderItem> orderItemRepository,
        IRepository<Product> productRepository)
    {
		_logger.LogError(">>> Entering ReturnRequestService(returnRequestRepository={ReturnRequestRepository},returnRequestActionRepository={ReturnRequestActionRepository},returnRequestReasonRepository={ReturnRequestReasonRepository},orderItemRepository={OrderItemRepository},productRepository={ProductRepository})", returnRequestRepository, returnRequestActionRepository, returnRequestReasonRepository, orderItemRepository, productRepository);
        _returnRequestRepository = returnRequestRepository;
        _returnRequestActionRepository = returnRequestActionRepository;
		_logger.LogDebug("ReturnRequestService(returnRequestRepository={ReturnRequestRepository},returnRequestActionRepository={ReturnRequestActionRepository},returnRequestReasonRepository={ReturnRequestReasonRepository},orderItemRepository={OrderItemRepository},productRepository={ProductRepository}): _returnRequestActionRepository → {_returnRequestActionRepository}", returnRequestRepository, returnRequestActionRepository, returnRequestReasonRepository, orderItemRepository, productRepository, _returnRequestActionRepository);
		_logger.LogDebug("ReturnRequestService(returnRequestRepository={ReturnRequestRepository},returnRequestActionRepository={ReturnRequestActionRepository},returnRequestReasonRepository={ReturnRequestReasonRepository},orderItemRepository={OrderItemRepository},productRepository={ProductRepository}): _returnRequestRepository → {_returnRequestRepository}", returnRequestRepository, returnRequestActionRepository, returnRequestReasonRepository, orderItemRepository, productRepository, _returnRequestRepository);
    _logger.LogWarning("<<< Exiting ReturnRequestService(returnRequestRepository={ReturnRequestRepository},returnRequestActionRepository={ReturnRequestActionRepository},returnRequestReasonRepository={ReturnRequestReasonRepository},orderItemRepository={OrderItemRepository},productRepository={ProductRepository})", returnRequestRepository, returnRequestActionRepository, returnRequestReasonRepository, orderItemRepository, productRepository);
        _returnRequestReasonRepository = returnRequestReasonRepository;
		_logger.LogDebug("ReturnRequestService(returnRequestRepository={ReturnRequestRepository},returnRequestActionRepository={ReturnRequestActionRepository},returnRequestReasonRepository={ReturnRequestReasonRepository},orderItemRepository={OrderItemRepository},productRepository={ProductRepository}): _returnRequestReasonRepository → {_returnRequestReasonRepository}", returnRequestRepository, returnRequestActionRepository, returnRequestReasonRepository, orderItemRepository, productRepository, _returnRequestReasonRepository);
        _orderItemRepository = orderItemRepository;
		_logger.LogDebug("ReturnRequestService(returnRequestRepository={ReturnRequestRepository},returnRequestActionRepository={ReturnRequestActionRepository},returnRequestReasonRepository={ReturnRequestReasonRepository},orderItemRepository={OrderItemRepository},productRepository={ProductRepository}): _orderItemRepository → {_orderItemRepository}", returnRequestRepository, returnRequestActionRepository, returnRequestReasonRepository, orderItemRepository, productRepository, _orderItemRepository);
        _productRepository = productRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a return request
    /// </summary>
    /// <param name="returnRequest">Return request</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteReturnRequestAsync(ReturnRequest returnRequest)
    {
		_logger.LogError(">>> Entering DeleteReturnRequestAsync(returnRequest={ReturnRequest})", returnRequest);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _returnRequestRepository.DeleteAsync(returnRequest);
		_logger.LogError("DeleteReturnRequestAsync(returnRequest)={ReturnRequest}: external service call took {Arg1} ms", returnRequest, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteReturnRequestAsync(returnRequest={ReturnRequest})", returnRequest);
    }

    /// <summary>
    /// Gets a return request
    /// </summary>
    /// <param name="returnRequestId">Return request identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request
    /// </returns>
    public virtual async Task<ReturnRequest> GetReturnRequestByIdAsync(int returnRequestId)
    {
		_logger.LogError(">>> Entering GetReturnRequestByIdAsync(returnRequestId={ReturnRequestId})", returnRequestId);
        _logger.LogWarning("<<< Exiting GetReturnRequestByIdAsync(returnRequestId={ReturnRequestId})", returnRequestId);
        return await _returnRequestRepository.GetByIdAsync(returnRequestId);
    }

    /// <summary>
    /// Search return requests
    /// </summary>
    /// <param name="storeId">Store identifier; 0 to load all entries</param>
    /// <param name="customerId">Customer identifier; 0 to load all entries</param>
    /// <param name="orderItemId">Order item identifier; 0 to load all entries</param>
    /// <param name="customNumber">Custom number; null or empty to load all entries</param>
    /// <param name="rs">Return request status; null to load all entries</param>
    /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
    /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return requests
    /// </returns>
    public virtual async Task<IPagedList<ReturnRequest>> SearchReturnRequestsAsync(int storeId = 0, int customerId = 0,
        int orderItemId = 0, string customNumber = "", ReturnRequestStatus? rs = null, DateTime? createdFromUtc = null,
        DateTime? createdToUtc = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
    {
		_logger.LogError(">>> Entering SearchReturnRequestsAsync(storeId={StoreId},customerId={CustomerId},orderItemId={OrderItemId},customNumber={CustomNumber},rs={Rs},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},pageIndex={PageIndex},pageSize={PageSize},getOnlyTotalCount={GetOnlyTotalCount})", storeId, customerId, orderItemId, customNumber, rs, createdFromUtc, createdToUtc, pageIndex, pageSize, getOnlyTotalCount);
        var query = _returnRequestRepository.Table;
        if (storeId > 0)
            query = query.Where(rr => storeId == rr.StoreId);
        if (customerId > 0)
            query = query.Where(rr => customerId == rr.CustomerId);
        if (rs.HasValue)
        {
            var returnStatusId = (int)rs.Value;
			_logger.LogError("SearchReturnRequestsAsync(storeId={StoreId},customerId={CustomerId},orderItemId={OrderItemId},customNumber={CustomNumber},rs={Rs},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},pageIndex={PageIndex},pageSize={PageSize},getOnlyTotalCount={GetOnlyTotalCount}): returnStatusId → {ReturnStatusId}", storeId, customerId, orderItemId, customNumber, rs, createdFromUtc, createdToUtc, pageIndex, pageSize, getOnlyTotalCount, returnStatusId);
            query = query.Where(rr => rr.ReturnRequestStatusId == returnStatusId);
        }

        if (orderItemId > 0)
            query = query.Where(rr => rr.OrderItemId == orderItemId);

        if (!string.IsNullOrEmpty(customNumber))
            query = query.Where(rr => rr.CustomNumber == customNumber);

        if (createdFromUtc.HasValue)
            query = query.Where(rr => createdFromUtc.Value <= rr.CreatedOnUtc);
        if (createdToUtc.HasValue)
            query = query.Where(rr => createdToUtc.Value >= rr.CreatedOnUtc);

        query = query.OrderByDescending(rr => rr.CreatedOnUtc).ThenByDescending(rr => rr.Id);

        var returnRequests = await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
		_logger.LogError("SearchReturnRequestsAsync(storeId={StoreId},customerId={CustomerId},orderItemId={OrderItemId},customNumber={CustomNumber},rs={Rs},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},pageIndex={PageIndex},pageSize={PageSize},getOnlyTotalCount={GetOnlyTotalCount}): returnRequests → {ReturnRequests}", storeId, customerId, orderItemId, customNumber, rs, createdFromUtc, createdToUtc, pageIndex, pageSize, getOnlyTotalCount, returnRequests);

		_logger.LogWarning("<<< Exiting SearchReturnRequestsAsync(storeId={StoreId},customerId={CustomerId},orderItemId={OrderItemId},customNumber={CustomNumber},rs={Rs},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},pageIndex={PageIndex},pageSize={PageSize},getOnlyTotalCount={GetOnlyTotalCount})", storeId, customerId, orderItemId, customNumber, rs, createdFromUtc, createdToUtc, pageIndex, pageSize, getOnlyTotalCount);
        return returnRequests;
    }

    /// <summary>
    /// Gets the return request availability
    /// </summary>
    /// <param name="orderId">The order identifier</param>
    /// <returns>The <see cref="Task"/> containing the <see cref="ReturnRequestAvailability"/></returns>
    public virtual async Task<ReturnRequestAvailability> GetReturnRequestAvailabilityAsync(int orderId)
    {
		_logger.LogError(">>> Entering GetReturnRequestAvailabilityAsync(orderId={OrderId})", orderId);
        var result = new ReturnRequestAvailability();

		_logger.LogError("GetReturnRequestAvailabilityAsync(orderId={OrderId}): result → {Result}", orderId, result);
        if (orderId > 0)
        {
            var cancelledStatusId = (int)ReturnRequestStatus.Cancelled;
			_logger.LogError("GetReturnRequestAvailabilityAsync(orderId={OrderId}): cancelledStatusId → {CancelledStatusId}", orderId, cancelledStatusId);
            var requestedOrderItemsForReturn =
                from rr in _returnRequestRepository.Table
                where rr.ReturnRequestStatusId != cancelledStatusId
                group rr by new
                {
                    rr.OrderItemId
                } into g
                select new
                {
                    OrderItemId = g.Key.OrderItemId,
                    RequestedQuantityForReturn = g.Sum(rr => rr.Quantity)
                };

            var query =
                from oi in _orderItemRepository.Table
                join roi in requestedOrderItemsForReturn
                    on oi.Id equals roi.OrderItemId into alreadyRequestedForReturn
                from aroi in alreadyRequestedForReturn.DefaultIfEmpty()
                join p in _productRepository.Table
                    on oi.ProductId equals p.Id
                where !p.NotReturnable && oi.OrderId == orderId
                select new ReturnableOrderItem
                {
                    AvailableQuantityForReturn = aroi != null
                        ? Math.Max(oi.Quantity - aroi.RequestedQuantityForReturn, 0)
                        : oi.Quantity,
                    OrderItem = oi
                };

			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogError("GetReturnRequestAvailabilityAsync(orderId)={OrderId}: database query executed in {Arg1} ms", orderId, __logStart.ElapsedMilliseconds);
            result.ReturnableOrderItems = await query.ToListAsync();
        }

		_logger.LogWarning("<<< Exiting GetReturnRequestAvailabilityAsync(orderId={OrderId})", orderId);
        return result;
    }

    /// <summary>
    /// Delete a return request action
    /// </summary>
    /// <param name="returnRequestAction">Return request action</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteReturnRequestActionAsync(ReturnRequestAction returnRequestAction)
    {
		_logger.LogError(">>> Entering DeleteReturnRequestActionAsync(returnRequestAction={ReturnRequestAction})", returnRequestAction);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _returnRequestActionRepository.DeleteAsync(returnRequestAction);
		_logger.LogError("DeleteReturnRequestActionAsync(returnRequestAction)={ReturnRequestAction}: external service call took {Arg1} ms", returnRequestAction, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteReturnRequestActionAsync(returnRequestAction={ReturnRequestAction})", returnRequestAction);
    }

    /// <summary>
    /// Gets all return request actions
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request actions
    /// </returns>
    public virtual async Task<IList<ReturnRequestAction>> GetAllReturnRequestActionsAsync()
    {
		_logger.LogError(">>> Entering GetAllReturnRequestActionsAsync()");
        return await _returnRequestActionRepository.GetAllAsync(query =>
        {
            return from rra in query
                orderby rra.DisplayOrder, rra.Id
                select rra;
				_logger.LogError("GetAllReturnRequestActionsAsync(): find query executing");
        }, cache => default);
		_logger.LogWarning("<<< Exiting GetAllReturnRequestActionsAsync()");
    }

    /// <summary>
    /// Gets a return request action
    /// </summary>
    /// <param name="returnRequestActionId">Return request action identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request action
    /// </returns>
    public virtual async Task<ReturnRequestAction> GetReturnRequestActionByIdAsync(int returnRequestActionId)
    {
		_logger.LogError(">>> Entering GetReturnRequestActionByIdAsync(returnRequestActionId={ReturnRequestActionId})", returnRequestActionId);
		_logger.LogError("GetReturnRequestActionByIdAsync(returnRequestActionId)={ReturnRequestActionId}: find query executing", returnRequestActionId);
    _logger.LogWarning("<<< Exiting GetReturnRequestActionByIdAsync(returnRequestActionId={ReturnRequestActionId})", returnRequestActionId);
        return await _returnRequestActionRepository.GetByIdAsync(returnRequestActionId, cache => default);
    }

    /// <summary>
    /// Inserts a return request
    /// </summary>
    /// <param name="returnRequest">Return request</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertReturnRequestAsync(ReturnRequest returnRequest)
    {
		_logger.LogError(">>> Entering InsertReturnRequestAsync(returnRequest={ReturnRequest})", returnRequest);
        await _returnRequestRepository.InsertAsync(returnRequest);
		_logger.LogWarning("<<< Exiting InsertReturnRequestAsync(returnRequest={ReturnRequest})", returnRequest);
    }

    /// <summary>
    /// Inserts a return request action
    /// </summary>
    /// <param name="returnRequestAction">Return request action</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertReturnRequestActionAsync(ReturnRequestAction returnRequestAction)
    {
		_logger.LogError(">>> Entering InsertReturnRequestActionAsync(returnRequestAction={ReturnRequestAction})", returnRequestAction);
        await _returnRequestActionRepository.InsertAsync(returnRequestAction);
		_logger.LogWarning("<<< Exiting InsertReturnRequestActionAsync(returnRequestAction={ReturnRequestAction})", returnRequestAction);
    }

    /// <summary>
    /// Updates the return request
    /// </summary>
    /// <param name="returnRequest">Return request</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateReturnRequestAsync(ReturnRequest returnRequest)
    {
		_logger.LogError(">>> Entering UpdateReturnRequestAsync(returnRequest={ReturnRequest})", returnRequest);
        await _returnRequestRepository.UpdateAsync(returnRequest);
		_logger.LogWarning("<<< Exiting UpdateReturnRequestAsync(returnRequest={ReturnRequest})", returnRequest);
    }

    /// <summary>
    /// Updates the return request action
    /// </summary>
    /// <param name="returnRequestAction">Return request action</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateReturnRequestActionAsync(ReturnRequestAction returnRequestAction)
    {
		_logger.LogError(">>> Entering UpdateReturnRequestActionAsync(returnRequestAction={ReturnRequestAction})", returnRequestAction);
        await _returnRequestActionRepository.UpdateAsync(returnRequestAction);
		_logger.LogWarning("<<< Exiting UpdateReturnRequestActionAsync(returnRequestAction={ReturnRequestAction})", returnRequestAction);
    }

    /// <summary>
    /// Delete a return request reason
    /// </summary>
    /// <param name="returnRequestReason">Return request reason</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteReturnRequestReasonAsync(ReturnRequestReason returnRequestReason)
    {
		_logger.LogError(">>> Entering DeleteReturnRequestReasonAsync(returnRequestReason={ReturnRequestReason})", returnRequestReason);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _returnRequestReasonRepository.DeleteAsync(returnRequestReason);
		_logger.LogError("DeleteReturnRequestReasonAsync(returnRequestReason)={ReturnRequestReason}: external service call took {Arg1} ms", returnRequestReason, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteReturnRequestReasonAsync(returnRequestReason={ReturnRequestReason})", returnRequestReason);
    }

    /// <summary>
    /// Gets all return request reasons
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request reasons
    /// </returns>
    public virtual async Task<IList<ReturnRequestReason>> GetAllReturnRequestReasonsAsync()
    {
		_logger.LogError(">>> Entering GetAllReturnRequestReasonsAsync()");
        return await _returnRequestReasonRepository.GetAllAsync(query =>
        {
            return from rra in query
                orderby rra.DisplayOrder, rra.Id
                select rra;
				_logger.LogError("GetAllReturnRequestReasonsAsync(): find query executing");
        }, cache => default);
		_logger.LogWarning("<<< Exiting GetAllReturnRequestReasonsAsync()");
    }

    /// <summary>
    /// Gets a return request reason
    /// </summary>
    /// <param name="returnRequestReasonId">Return request reason identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request reason
    /// </returns>
    public virtual async Task<ReturnRequestReason> GetReturnRequestReasonByIdAsync(int returnRequestReasonId)
    {
		_logger.LogError(">>> Entering GetReturnRequestReasonByIdAsync(returnRequestReasonId={ReturnRequestReasonId})", returnRequestReasonId);
		_logger.LogError("GetReturnRequestReasonByIdAsync(returnRequestReasonId)={ReturnRequestReasonId}: find query executing", returnRequestReasonId);
    _logger.LogWarning("<<< Exiting GetReturnRequestReasonByIdAsync(returnRequestReasonId={ReturnRequestReasonId})", returnRequestReasonId);
        return await _returnRequestReasonRepository.GetByIdAsync(returnRequestReasonId, cache => default);
    }

    /// <summary>
    /// Inserts a return request reason
    /// </summary>
    /// <param name="returnRequestReason">Return request reason</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertReturnRequestReasonAsync(ReturnRequestReason returnRequestReason)
    {
		_logger.LogError(">>> Entering InsertReturnRequestReasonAsync(returnRequestReason={ReturnRequestReason})", returnRequestReason);
        await _returnRequestReasonRepository.InsertAsync(returnRequestReason);
		_logger.LogWarning("<<< Exiting InsertReturnRequestReasonAsync(returnRequestReason={ReturnRequestReason})", returnRequestReason);
    }

    /// <summary>
    /// Updates the  return request reason
    /// </summary>
    /// <param name="returnRequestReason">Return request reason</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateReturnRequestReasonAsync(ReturnRequestReason returnRequestReason)
    {
		_logger.LogError(">>> Entering UpdateReturnRequestReasonAsync(returnRequestReason={ReturnRequestReason})", returnRequestReason);
        await _returnRequestReasonRepository.UpdateAsync(returnRequestReason);
		_logger.LogWarning("<<< Exiting UpdateReturnRequestReasonAsync(returnRequestReason={ReturnRequestReason})", returnRequestReason);
    }

    #endregion
}