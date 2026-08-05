using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Customers;

namespace Nop.Services.Orders;

/// <summary>
/// Gift card service
/// </summary>
public partial class GiftCardService : IGiftCardService
{
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
        _customerService = customerService;
        _eventPublisher = eventPublisher;
        _giftCardRepository = giftCardRepository;
        _giftCardUsageHistoryRepository = giftCardUsageHistoryRepository;
        _orderItemRepository = orderItemRepository;
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
		logger.LogDebug(">>> Entering DeleteGiftCardAsync(giftCard={GiftCard})", giftCard);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _giftCardRepository.DeleteAsync(giftCard);
		logger.LogDebug("<<< Exiting DeleteGiftCardAsync(giftCard={GiftCard})", giftCard);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteGiftCardAsync(giftCard)={GiftCard}: external service call took {Arg1} ms", giftCard, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteGiftCardAsync(giftCard)={GiftCard}: delete query executed in {Arg1} ms", giftCard, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteGiftCardAsync(giftCard)={GiftCard}: external service call took {Arg1} ms", giftCard, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteGiftCardAsync(giftCard)={GiftCard}: delete query executed in {Arg1} ms", giftCard, __logStart.ElapsedMilliseconds);
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
		logger.LogDebug(">>> Entering GetGiftCardByIdAsync(giftCardId={GiftCardId})", giftCardId);
		logger.LogDebug("<<< Exiting GetGiftCardByIdAsync(giftCardId={GiftCardId})", giftCardId);
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

        return giftCards;
    }

    /// <summary>
    /// Inserts a gift card
    /// </summary>
    /// <param name="giftCard">Gift card</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertGiftCardAsync(GiftCard giftCard)
    {
		logger.LogDebug(">>> Entering InsertGiftCardAsync(giftCard={GiftCard})", giftCard);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _giftCardRepository.InsertAsync(giftCard);
		logger.LogDebug("<<< Exiting InsertGiftCardAsync(giftCard={GiftCard})", giftCard);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertGiftCardAsync(giftCard)={GiftCard}: save query executed in {Arg1} ms", giftCard, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertGiftCardAsync(giftCard)={GiftCard}: save query executed in {Arg1} ms", giftCard, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the gift card
    /// </summary>
    /// <param name="giftCard">Gift card</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateGiftCardAsync(GiftCard giftCard)
    {
		logger.LogDebug(">>> Entering UpdateGiftCardAsync(giftCard={GiftCard})", giftCard);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _giftCardRepository.UpdateAsync(giftCard);
		logger.LogDebug("<<< Exiting UpdateGiftCardAsync(giftCard={GiftCard})", giftCard);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateGiftCardAsync(giftCard)={GiftCard}: update query executed in {Arg1} ms", giftCard, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateGiftCardAsync(giftCard)={GiftCard}: update query executed in {Arg1} ms", giftCard, __logStart.ElapsedMilliseconds);
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
		logger.LogDebug(">>> Entering GetGiftCardsByPurchasedWithOrderItemIdAsync(purchasedWithOrderItemId={PurchasedWithOrderItemId})", purchasedWithOrderItemId);
        if (purchasedWithOrderItemId == 0)
            return new List<GiftCard>();

        var query = _giftCardRepository.Table;
        query = query.Where(gc => gc.PurchasedWithOrderItemId.HasValue && gc.PurchasedWithOrderItemId.Value == purchasedWithOrderItemId);
		logger.LogDebug("GetGiftCardsByPurchasedWithOrderItemIdAsync(purchasedWithOrderItemId={PurchasedWithOrderItemId}): query → {Query}", purchasedWithOrderItemId, query);
        query = query.OrderBy(gc => gc.Id);

        var giftCards = await query.ToListAsync();

		logger.LogDebug("GetGiftCardsByPurchasedWithOrderItemIdAsync(purchasedWithOrderItemId={PurchasedWithOrderItemId}): giftCards → {GiftCards}", purchasedWithOrderItemId, giftCards);
		logger.LogDebug("<<< Exiting GetGiftCardsByPurchasedWithOrderItemIdAsync(purchasedWithOrderItemId={PurchasedWithOrderItemId})", purchasedWithOrderItemId);
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
		logger.LogDebug(">>> Entering GetActiveGiftCardsAppliedByCustomerAsync(customer={Customer})", customer);
        var result = new List<GiftCard>();
        if (customer == null)
			logger.LogDebug("GetActiveGiftCardsAppliedByCustomerAsync(customer={Customer}): result → {Result}", customer, result);
            return result;

        var couponCodes = await _customerService.ParseAppliedGiftCardCouponCodesAsync(customer);
        foreach (var couponCode in couponCodes)
        {
		logger.LogDebug("GetActiveGiftCardsAppliedByCustomerAsync(customer={Customer}): couponCodes → {CouponCodes}", customer, couponCodes);
            var giftCards = await GetAllGiftCardsAsync(isGiftCardActivated: true, giftCardCouponCode: couponCode);
            foreach (var gc in giftCards)
                if (await IsGiftCardValidAsync(gc))
				logger.LogDebug("GetActiveGiftCardsAppliedByCustomerAsync(customer={Customer}): giftCards → {GiftCards}", customer, giftCards);
                    result.Add(gc);
        }

		logger.LogDebug("<<< Exiting GetActiveGiftCardsAppliedByCustomerAsync(customer={Customer})", customer);
        return result;
    }

    /// <summary>
    /// Generate new gift card code
    /// </summary>
    /// <returns>Result</returns>
    public virtual string GenerateGiftCardCode()
    {
		logger.LogDebug(">>> Entering GenerateGiftCardCode()");
        var length = 13;
        var result = Guid.NewGuid().ToString();
        if (result.Length > length)
			logger.LogDebug("GenerateGiftCardCode(): result → {Result}", result);
            result = result[0..length];
		logger.LogDebug("<<< Exiting GenerateGiftCardCode()");
        return result;
    }

    /// <summary>
    /// Delete gift card usage history
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteGiftCardUsageHistoryAsync(Order order)
    {
		logger.LogDebug(">>> Entering DeleteGiftCardUsageHistoryAsync(order={Order})", order);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var giftCardUsageHistory = await GetGiftCardUsageHistoryAsync(order);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _giftCardUsageHistoryRepository.DeleteAsync(giftCardUsageHistory);

		logger.LogDebug("DeleteGiftCardUsageHistoryAsync(order={Order}): giftCardUsageHistory → {GiftCardUsageHistory}", order, giftCardUsageHistory);
		logger.LogInformation("DeleteGiftCardUsageHistoryAsync(order)={Order}: delete query executed in {Arg1} ms", order, __logStart.ElapsedMilliseconds);
        var query = _giftCardRepository.Table;

		logger.LogDebug("DeleteGiftCardUsageHistoryAsync(order={Order}): query → {Query}", order, query);
        var giftCardIds = giftCardUsageHistory.Select(gcuh => gcuh.GiftCardId).ToArray();
        var giftCards = await query.Where(bp => giftCardIds.Contains(bp.Id)).ToListAsync();

		logger.LogDebug("DeleteGiftCardUsageHistoryAsync(order={Order}): giftCardIds → {GiftCardIds}", order, giftCardIds);
		logger.LogDebug("DeleteGiftCardUsageHistoryAsync(order={Order}): giftCards → {GiftCards}", order, giftCards);
		logger.LogInformation("DeleteGiftCardUsageHistoryAsync(order)={Order}: database query executed in {Arg1} ms", order, __logStart.ElapsedMilliseconds);
		logger.LogInformation("DeleteGiftCardUsageHistoryAsync(order)={Order}: external service call took {Arg1} ms", order, __logStart.ElapsedMilliseconds);
        //event notification
        foreach (var giftCard in giftCards)
            await _eventPublisher.EntityUpdatedAsync(giftCard);
			logger.LogDebug("<<< Exiting DeleteGiftCardUsageHistoryAsync(order={Order})", order);
			logger.LogInformation("DeleteGiftCardUsageHistoryAsync(order)={Order}: external service call took {Arg1} ms", order, __logStart.ElapsedMilliseconds);
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
		logger.LogDebug(">>> Entering GetGiftCardRemainingAmountAsync(giftCard={GiftCard})", giftCard);
        ArgumentNullException.ThrowIfNull(giftCard);

        var result = giftCard.Amount;

		logger.LogDebug("GetGiftCardRemainingAmountAsync(giftCard={GiftCard}): result → {Result}", giftCard, result);
        foreach (var gcuh in await GetGiftCardUsageHistoryAsync(giftCard))
            result -= gcuh.UsedValue;

        if (result < decimal.Zero)
            result = decimal.Zero;

		logger.LogDebug("<<< Exiting GetGiftCardRemainingAmountAsync(giftCard={GiftCard})", giftCard);
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
		logger.LogDebug(">>> Entering GetGiftCardUsageHistoryAsync(giftCard={GiftCard})", giftCard);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(giftCard);

		logger.LogDebug("<<< Exiting GetGiftCardUsageHistoryAsync(giftCard={GiftCard})", giftCard);
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

		logger.LogDebug("<<< Exiting GetGiftCardUsageHistoryAsync(order={Order})", order);
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
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _giftCardUsageHistoryRepository.InsertAsync(giftCardUsageHistory);
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
		logger.LogDebug(">>> Entering IsGiftCardValidAsync(giftCard={GiftCard})", giftCard);
        ArgumentNullException.ThrowIfNull(giftCard);

        if (!giftCard.IsGiftCardActivated)
            return false;

        var remainingAmount = await GetGiftCardRemainingAmountAsync(giftCard);
        if (remainingAmount > decimal.Zero)
			logger.LogDebug("IsGiftCardValidAsync(giftCard={GiftCard}): remainingAmount → {RemainingAmount}", giftCard, remainingAmount);
            return true;

        return false;
    }

    #endregion
}