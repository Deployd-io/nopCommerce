using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Customers;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Orders;

/// <summary>
/// Gift card service
/// </summary>
public partial class GiftCardService : IGiftCardService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IRepository<GiftCard> _giftCardRepository;
    protected readonly IRepository<GiftCardUsageHistory> _giftCardUsageHistoryRepository;
    protected readonly IRepository<OrderItem> _orderItemRepository;
    #endregion

    #region Ctor

    public GiftCardService(ICustomerService customerService,
        IEventPublisher eventPublisher,
        IRepository<GiftCard> giftCardRepository,
        IRepository<GiftCardUsageHistory> giftCardUsageHistoryRepository,
        IRepository<OrderItem> orderItemRepository)
    {
		_logger.LogError(">>> Entering GiftCardService(customerService={CustomerService},eventPublisher={EventPublisher},giftCardRepository={GiftCardRepository},giftCardUsageHistoryRepository={GiftCardUsageHistoryRepository},orderItemRepository={OrderItemRepository})", customerService, eventPublisher, giftCardRepository, giftCardUsageHistoryRepository, orderItemRepository);
        _customerService = customerService;
        _eventPublisher = eventPublisher;
		_logger.LogDebug("GiftCardService(customerService={CustomerService},eventPublisher={EventPublisher},giftCardRepository={GiftCardRepository},giftCardUsageHistoryRepository={GiftCardUsageHistoryRepository},orderItemRepository={OrderItemRepository}): _eventPublisher → {_eventPublisher}", customerService, eventPublisher, giftCardRepository, giftCardUsageHistoryRepository, orderItemRepository, _eventPublisher);
        _giftCardRepository = giftCardRepository;
		_logger.LogDebug("GiftCardService(customerService={CustomerService},eventPublisher={EventPublisher},giftCardRepository={GiftCardRepository},giftCardUsageHistoryRepository={GiftCardUsageHistoryRepository},orderItemRepository={OrderItemRepository}): _giftCardRepository → {_giftCardRepository}", customerService, eventPublisher, giftCardRepository, giftCardUsageHistoryRepository, orderItemRepository, _giftCardRepository);
        _giftCardUsageHistoryRepository = giftCardUsageHistoryRepository;
		_logger.LogDebug("GiftCardService(customerService={CustomerService},eventPublisher={EventPublisher},giftCardRepository={GiftCardRepository},giftCardUsageHistoryRepository={GiftCardUsageHistoryRepository},orderItemRepository={OrderItemRepository}): _giftCardUsageHistoryRepository → {_giftCardUsageHistoryRepository}", customerService, eventPublisher, giftCardRepository, giftCardUsageHistoryRepository, orderItemRepository, _giftCardUsageHistoryRepository);
        _orderItemRepository = orderItemRepository;
		_logger.LogDebug("GiftCardService(customerService={CustomerService},eventPublisher={EventPublisher},giftCardRepository={GiftCardRepository},giftCardUsageHistoryRepository={GiftCardUsageHistoryRepository},orderItemRepository={OrderItemRepository}): _orderItemRepository → {_orderItemRepository}", customerService, eventPublisher, giftCardRepository, giftCardUsageHistoryRepository, orderItemRepository, _orderItemRepository);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a gift card
    /// </summary>
    /// <param name="giftCard">Gift card</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteGiftCardAsync(GiftCard giftCard)
    {
		_logger.LogError(">>> Entering DeleteGiftCardAsync(giftCard={GiftCard})", giftCard);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _giftCardRepository.DeleteAsync(giftCard);
		_logger.LogError("DeleteGiftCardAsync(giftCard)={GiftCard}: external service call took {Arg1} ms", giftCard, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteGiftCardAsync(giftCard={GiftCard})", giftCard);
    }

    /// <summary>
    /// Gets a gift card
    /// </summary>
    /// <param name="giftCardId">Gift card identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gift card entry
    /// </returns>
    public virtual async Task<GiftCard> GetGiftCardByIdAsync(int giftCardId)
    {
		_logger.LogError(">>> Entering GetGiftCardByIdAsync(giftCardId={GiftCardId})", giftCardId);
		_logger.LogError("GetGiftCardByIdAsync(giftCardId)={GiftCardId}: find query executing", giftCardId);
    _logger.LogWarning("<<< Exiting GetGiftCardByIdAsync(giftCardId={GiftCardId})", giftCardId);
        return await _giftCardRepository.GetByIdAsync(giftCardId, cache => default);
    }

    /// <summary>
    /// Gets all gift cards
    /// </summary>
    /// <param name="purchasedWithOrderId">Associated order ID; null to load all records</param>
    /// <param name="usedWithOrderId">The order ID in which the gift card was used; null to load all records</param>
    /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
    /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
    /// <param name="isGiftCardActivated">Value indicating whether gift card is activated; null to load all records</param>
    /// <param name="giftCardCouponCode">Gift card coupon code; null to load all records</param>
    /// <param name="recipientName">Recipient name; null to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gift cards
    /// </returns>
    public virtual async Task<IPagedList<GiftCard>> GetAllGiftCardsAsync(int? purchasedWithOrderId = null, int? usedWithOrderId = null,
        DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
        bool? isGiftCardActivated = null, string giftCardCouponCode = null,
        string recipientName = null,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogError(">>> Entering GetAllGiftCardsAsync(purchasedWithOrderId={PurchasedWithOrderId},usedWithOrderId={UsedWithOrderId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},isGiftCardActivated={IsGiftCardActivated},giftCardCouponCode={GiftCardCouponCode},recipientName={RecipientName},pageIndex={PageIndex},pageSize={PageSize})", purchasedWithOrderId, usedWithOrderId, createdFromUtc, createdToUtc, isGiftCardActivated, giftCardCouponCode, recipientName, pageIndex, pageSize);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var giftCards = await _giftCardRepository.GetAllPagedAsync(query =>
        {
            if (purchasedWithOrderId.HasValue)
            {
                query = from gc in query
                    join oi in _orderItemRepository.Table on gc.PurchasedWithOrderItemId equals oi.Id
                    where oi.OrderId == purchasedWithOrderId.Value
                    select gc;
            }

            if (usedWithOrderId.HasValue)
                query = from gc in query
                    join gcuh in _giftCardUsageHistoryRepository.Table on gc.Id equals gcuh.GiftCardId
                    where gcuh.UsedWithOrderId == usedWithOrderId
                    select gc;

            if (createdFromUtc.HasValue)
                query = query.Where(gc => createdFromUtc.Value <= gc.CreatedOnUtc);
				_logger.LogError("GetAllGiftCardsAsync(purchasedWithOrderId={PurchasedWithOrderId},usedWithOrderId={UsedWithOrderId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},isGiftCardActivated={IsGiftCardActivated},giftCardCouponCode={GiftCardCouponCode},recipientName={RecipientName},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", purchasedWithOrderId, usedWithOrderId, createdFromUtc, createdToUtc, isGiftCardActivated, giftCardCouponCode, recipientName, pageIndex, pageSize, query);
            if (createdToUtc.HasValue)
                query = query.Where(gc => createdToUtc.Value >= gc.CreatedOnUtc);
            if (isGiftCardActivated.HasValue)
                query = query.Where(gc => gc.IsGiftCardActivated == isGiftCardActivated.Value);
            if (!string.IsNullOrEmpty(giftCardCouponCode))
                query = query.Where(gc => gc.GiftCardCouponCode == giftCardCouponCode);
            if (!string.IsNullOrWhiteSpace(recipientName))
                query = query.Where(c => c.RecipientName.Contains(recipientName));
            query = query.OrderByDescending(gc => gc.CreatedOnUtc);

            return query;
        }, pageIndex, pageSize);
		_logger.LogError("GetAllGiftCardsAsync(purchasedWithOrderId,usedWithOrderId,createdFromUtc,createdToUtc,isGiftCardActivated,giftCardCouponCode,recipientName,pageIndex,pageSize)={PurchasedWithOrderId},{UsedWithOrderId},{CreatedFromUtc},{CreatedToUtc},{IsGiftCardActivated},{GiftCardCouponCode},{RecipientName},{PageIndex},{PageSize}: find query executed in {Arg9} ms", purchasedWithOrderId, usedWithOrderId, createdFromUtc, createdToUtc, isGiftCardActivated, giftCardCouponCode, recipientName, pageIndex, pageSize, __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting GetAllGiftCardsAsync(purchasedWithOrderId={PurchasedWithOrderId},usedWithOrderId={UsedWithOrderId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},isGiftCardActivated={IsGiftCardActivated},giftCardCouponCode={GiftCardCouponCode},recipientName={RecipientName},pageIndex={PageIndex},pageSize={PageSize})", purchasedWithOrderId, usedWithOrderId, createdFromUtc, createdToUtc, isGiftCardActivated, giftCardCouponCode, recipientName, pageIndex, pageSize);
        return giftCards;
    }

    /// <summary>
    /// Inserts a gift card
    /// </summary>
    /// <param name="giftCard">Gift card</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertGiftCardAsync(GiftCard giftCard)
    {
		_logger.LogError(">>> Entering InsertGiftCardAsync(giftCard={GiftCard})", giftCard);
        await _giftCardRepository.InsertAsync(giftCard);
		_logger.LogWarning("<<< Exiting InsertGiftCardAsync(giftCard={GiftCard})", giftCard);
    }

    /// <summary>
    /// Updates the gift card
    /// </summary>
    /// <param name="giftCard">Gift card</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateGiftCardAsync(GiftCard giftCard)
    {
		_logger.LogError(">>> Entering UpdateGiftCardAsync(giftCard={GiftCard})", giftCard);
        await _giftCardRepository.UpdateAsync(giftCard);
		_logger.LogWarning("<<< Exiting UpdateGiftCardAsync(giftCard={GiftCard})", giftCard);
    }

    /// <summary>
    /// Gets gift cards by 'PurchasedWithOrderItemId'
    /// </summary>
    /// <param name="purchasedWithOrderItemId">Purchased with order item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gift card entries
    /// </returns>
    public virtual async Task<IList<GiftCard>> GetGiftCardsByPurchasedWithOrderItemIdAsync(int purchasedWithOrderItemId)
    {
		_logger.LogError(">>> Entering GetGiftCardsByPurchasedWithOrderItemIdAsync(purchasedWithOrderItemId={PurchasedWithOrderItemId})", purchasedWithOrderItemId);
        if (purchasedWithOrderItemId == 0)
            return new List<GiftCard>();

        var query = _giftCardRepository.Table;
		_logger.LogError("GetGiftCardsByPurchasedWithOrderItemIdAsync(purchasedWithOrderItemId={PurchasedWithOrderItemId}): query → {Query}", purchasedWithOrderItemId, query);
        query = query.Where(gc => gc.PurchasedWithOrderItemId.HasValue && gc.PurchasedWithOrderItemId.Value == purchasedWithOrderItemId);
        query = query.OrderBy(gc => gc.Id);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var giftCards = await query.ToListAsync();

		_logger.LogError("GetGiftCardsByPurchasedWithOrderItemIdAsync(purchasedWithOrderItemId={PurchasedWithOrderItemId}): giftCards → {GiftCards}", purchasedWithOrderItemId, giftCards);
		_logger.LogError("GetGiftCardsByPurchasedWithOrderItemIdAsync(purchasedWithOrderItemId)={PurchasedWithOrderItemId}: database query executed in {Arg1} ms", purchasedWithOrderItemId, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetGiftCardsByPurchasedWithOrderItemIdAsync(purchasedWithOrderItemId={PurchasedWithOrderItemId})", purchasedWithOrderItemId);
        return giftCards;
    }

    /// <summary>
    /// Get active gift cards that are applied by a customer
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the active gift cards
    /// </returns>
    public virtual async Task<IList<GiftCard>> GetActiveGiftCardsAppliedByCustomerAsync(Customer customer)
    {
		_logger.LogError(">>> Entering GetActiveGiftCardsAppliedByCustomerAsync(customer={Customer})", customer);
        var result = new List<GiftCard>();
        if (customer == null)
            return result;

        var couponCodes = await _customerService.ParseAppliedGiftCardCouponCodesAsync(customer);
		_logger.LogError("GetActiveGiftCardsAppliedByCustomerAsync(customer={Customer}): couponCodes → {CouponCodes}", customer, couponCodes);
        foreach (var couponCode in couponCodes)
        {
            var giftCards = await GetAllGiftCardsAsync(isGiftCardActivated: true, giftCardCouponCode: couponCode);
			_logger.LogError("GetActiveGiftCardsAppliedByCustomerAsync(customer={Customer}): giftCards → {GiftCards}", customer, giftCards);
            foreach (var gc in giftCards)
                if (await IsGiftCardValidAsync(gc))
                    result.Add(gc);
        }

		_logger.LogWarning("<<< Exiting GetActiveGiftCardsAppliedByCustomerAsync(customer={Customer})", customer);
        return result;
    }

    /// <summary>
    /// Generate new gift card code
    /// </summary>
    /// <returns>Result</returns>
    public virtual string GenerateGiftCardCode()
    {
		_logger.LogError(">>> Entering GenerateGiftCardCode()");
        var length = 13;
        var result = Guid.NewGuid().ToString();
		_logger.LogError("GenerateGiftCardCode(): result → {Result}", result);
        if (result.Length > length)
            result = result[0..length];
		_logger.LogWarning("<<< Exiting GenerateGiftCardCode()");
        return result;
    }

    /// <summary>
    /// Delete gift card usage history
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteGiftCardUsageHistoryAsync(Order order)
    {
		_logger.LogError(">>> Entering DeleteGiftCardUsageHistoryAsync(order={Order})", order);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var giftCardUsageHistory = await GetGiftCardUsageHistoryAsync(order);

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
        await _giftCardUsageHistoryRepository.DeleteAsync(giftCardUsageHistory);

		_logger.LogError("DeleteGiftCardUsageHistoryAsync(order={Order}): giftCardUsageHistory → {GiftCardUsageHistory}", order, giftCardUsageHistory);
		_logger.LogError("DeleteGiftCardUsageHistoryAsync(order)={Order}: delete query executed in {Arg1} ms", order, __logStartDb.ElapsedMilliseconds);
        var query = _giftCardRepository.Table;

		_logger.LogError("DeleteGiftCardUsageHistoryAsync(order={Order}): query → {Query}", order, query);
        var giftCardIds = giftCardUsageHistory.Select(gcuh => gcuh.GiftCardId).ToArray();
        var giftCards = await query.Where(bp => giftCardIds.Contains(bp.Id)).ToListAsync();
		_logger.LogError("DeleteGiftCardUsageHistoryAsync(order={Order}): giftCards → {GiftCards}", order, giftCards);
		_logger.LogError("DeleteGiftCardUsageHistoryAsync(order={Order}): giftCardIds → {GiftCardIds}", order, giftCardIds);
		_logger.LogError("DeleteGiftCardUsageHistoryAsync(order)={Order}: database query executed in {Arg1} ms", order, __logStartDb.ElapsedMilliseconds);

        //event notification
        foreach (var giftCard in giftCards)
            await _eventPublisher.EntityUpdatedAsync(giftCard);
			_logger.LogError("DeleteGiftCardUsageHistoryAsync(order)={Order}: external service call took {Arg1} ms", order, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteGiftCardUsageHistoryAsync(order={Order})", order);
    }

    /// <summary>
    /// Gets a gift card remaining amount
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gift card remaining amount
    /// </returns>
    public virtual async Task<decimal> GetGiftCardRemainingAmountAsync(GiftCard giftCard)
    {
		_logger.LogError(">>> Entering GetGiftCardRemainingAmountAsync(giftCard={GiftCard})", giftCard);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(giftCard);

        var result = giftCard.Amount;
		_logger.LogError("GetGiftCardRemainingAmountAsync(giftCard={GiftCard}): result → {Result}", giftCard, result);

        foreach (var gcuh in await GetGiftCardUsageHistoryAsync(giftCard))
            result -= gcuh.UsedValue;

        if (result < decimal.Zero)
            result = decimal.Zero;

		_logger.LogWarning("<<< Exiting GetGiftCardRemainingAmountAsync(giftCard={GiftCard})", giftCard);
		_logger.LogError(">>> Entering GetGiftCardUsageHistoryAsync(giftCard={GiftCard})", giftCard);
		_logger.LogError("GetGiftCardUsageHistoryAsync(giftCard)={GiftCard}: database query executed in {Arg1} ms", giftCard, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting GetGiftCardUsageHistoryAsync(giftCard={GiftCard})", giftCard);
        return result;
    }

    /// <summary>
    /// Gets a gift card usage history entries
    /// </summary>
    /// <param name="giftCard">Gift card</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<IList<GiftCardUsageHistory>> GetGiftCardUsageHistoryAsync(GiftCard giftCard)
    {
        ArgumentNullException.ThrowIfNull(giftCard);

        return await _giftCardUsageHistoryRepository.Table
            .Where(gcuh => gcuh.GiftCardId == giftCard.Id)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a gift card usage history entries
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<IList<GiftCardUsageHistory>> GetGiftCardUsageHistoryAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        return await _giftCardUsageHistoryRepository.Table
            .Where(gcuh => gcuh.UsedWithOrderId == order.Id)
            .ToListAsync();
    }

    /// <summary>
    /// Inserts a gift card usage history entry
    /// </summary>
    /// <param name="giftCardUsageHistory">Gift card usage history entry</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertGiftCardUsageHistoryAsync(GiftCardUsageHistory giftCardUsageHistory)
    {
		_logger.LogError(">>> Entering InsertGiftCardUsageHistoryAsync(giftCardUsageHistory={GiftCardUsageHistory})", giftCardUsageHistory);
        await _giftCardUsageHistoryRepository.InsertAsync(giftCardUsageHistory);
		_logger.LogWarning("<<< Exiting InsertGiftCardUsageHistoryAsync(giftCardUsageHistory={GiftCardUsageHistory})", giftCardUsageHistory);
    }

    /// <summary>
    /// Is gift card valid
    /// </summary>
    /// <param name="giftCard">Gift card</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsGiftCardValidAsync(GiftCard giftCard)
    {
		_logger.LogError(">>> Entering IsGiftCardValidAsync(giftCard={GiftCard})", giftCard);
        ArgumentNullException.ThrowIfNull(giftCard);

        if (!giftCard.IsGiftCardActivated)
            return false;

        var remainingAmount = await GetGiftCardRemainingAmountAsync(giftCard);
		_logger.LogError("IsGiftCardValidAsync(giftCard={GiftCard}): remainingAmount → {RemainingAmount}", giftCard, remainingAmount);
        if (remainingAmount > decimal.Zero)
            return true;

		_logger.LogWarning("<<< Exiting IsGiftCardValidAsync(giftCard={GiftCard})", giftCard);
        return false;
    }

    #endregion
}