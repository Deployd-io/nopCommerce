using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Orders;

/// <summary>
/// Reward point service
/// </summary>
public partial class RewardPointService : IRewardPointService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ILocalizationService _localizationService;
    protected readonly IRepository<RewardPointsHistory> _rewardPointsHistoryRepository;
    protected readonly RewardPointsSettings _rewardPointsSettings;

    #endregion

    #region Ctor

    public RewardPointService(IDateTimeHelper dateTimeHelper,
        ILocalizationService localizationService,
        IRepository<RewardPointsHistory> rewardPointsHistoryRepository,
        RewardPointsSettings rewardPointsSettings)
    {
		_logger.LogError(">>> Entering RewardPointService(dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},rewardPointsHistoryRepository={RewardPointsHistoryRepository},rewardPointsSettings={RewardPointsSettings})", dateTimeHelper, localizationService, rewardPointsHistoryRepository, rewardPointsSettings);
        _dateTimeHelper = dateTimeHelper;
        _localizationService = localizationService;
		_logger.LogDebug("RewardPointService(dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},rewardPointsHistoryRepository={RewardPointsHistoryRepository},rewardPointsSettings={RewardPointsSettings}): _localizationService → {_localizationService}", dateTimeHelper, localizationService, rewardPointsHistoryRepository, rewardPointsSettings, _localizationService);
        _rewardPointsHistoryRepository = rewardPointsHistoryRepository;
		_logger.LogDebug("RewardPointService(dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},rewardPointsHistoryRepository={RewardPointsHistoryRepository},rewardPointsSettings={RewardPointsSettings}): _rewardPointsHistoryRepository → {_rewardPointsHistoryRepository}", dateTimeHelper, localizationService, rewardPointsHistoryRepository, rewardPointsSettings, _rewardPointsHistoryRepository);
        _rewardPointsSettings = rewardPointsSettings;
		_logger.LogDebug("RewardPointService(dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},rewardPointsHistoryRepository={RewardPointsHistoryRepository},rewardPointsSettings={RewardPointsSettings}): _rewardPointsSettings → {_rewardPointsSettings}", dateTimeHelper, localizationService, rewardPointsHistoryRepository, rewardPointsSettings, _rewardPointsSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get query to load reward points history
    /// </summary>
    /// <param name="customerId">Customer identifier; pass 0 to load all records</param>
    /// <param name="storeId">Store identifier; pass null to load all records</param>
    /// <param name="showNotActivated">Whether to load reward points that did not yet activated</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the query to load reward points history
    /// </returns>
    protected virtual async Task<IQueryable<RewardPointsHistory>> GetRewardPointsQueryAsync(int customerId, int? storeId, bool showNotActivated = false)
    {
		_logger.LogError(">>> Entering GetRewardPointsQueryAsync(customerId={CustomerId},storeId={StoreId},showNotActivated={ShowNotActivated})", customerId, storeId, showNotActivated);
        var query = _rewardPointsHistoryRepository.Table;

		_logger.LogError("GetRewardPointsQueryAsync(customerId={CustomerId},storeId={StoreId},showNotActivated={ShowNotActivated}): query → {Query}", customerId, storeId, showNotActivated, query);
        //filter by customer
        if (customerId > 0)
            query = query.Where(historyEntry => historyEntry.CustomerId == customerId);

        //filter by store
        if (!_rewardPointsSettings.PointsAccumulatedForAllStores && storeId > 0)
            query = query.Where(historyEntry => historyEntry.StoreId == storeId);

        //whether to show only the points that already activated
        if (!showNotActivated)
            query = query.Where(historyEntry => historyEntry.CreatedOnUtc < DateTime.UtcNow);

        //update points balance
        await UpdateRewardPointsBalanceAsync(query);

		_logger.LogWarning("<<< Exiting GetRewardPointsQueryAsync(customerId={CustomerId},storeId={StoreId},showNotActivated={ShowNotActivated})", customerId, storeId, showNotActivated);
        return query;
    }

    /// <summary>
    /// Update reward points balance if necessary
    /// </summary>
    /// <param name="query">Input query</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task UpdateRewardPointsBalanceAsync(IQueryable<RewardPointsHistory> query)
    {
		_logger.LogError(">>> Entering UpdateRewardPointsBalanceAsync(query={Query})", query);
        //get expired points
        var nowUtc = DateTime.UtcNow;
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var expiredPoints = await query
            .Where(historyEntry => historyEntry.EndDateUtc < nowUtc && historyEntry.ValidPoints > 0)
            .OrderBy(historyEntry => historyEntry.CreatedOnUtc).ThenBy(historyEntry => historyEntry.Id).ToListAsync();
			_logger.LogError("UpdateRewardPointsBalanceAsync(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

        //reduce the balance for these points
        foreach (var historyEntry in expiredPoints)
        {
            await InsertRewardPointsHistoryEntryAsync(new RewardPointsHistory
            {
                CustomerId = historyEntry.CustomerId,
                StoreId = historyEntry.StoreId,
                Points = -historyEntry.ValidPoints.Value,
                Message = string.Format(await _localizationService.GetResourceAsync("RewardPoints.Expired"),
                    await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.CreatedOnUtc, DateTimeKind.Utc)),
                CreatedOnUtc = historyEntry.EndDateUtc.Value
            });
			_logger.LogDebug("UpdateRewardPointsBalanceAsync(query={Query}): CreatedOnUtc → {CreatedOnUtc}", query, CreatedOnUtc);

            historyEntry.ValidPoints = 0;
			_logger.LogDebug("UpdateRewardPointsBalanceAsync(query={Query}): historyEntry.ValidPoints → {Arg1}", query, historyEntry.ValidPoints);
            await UpdateRewardPointsHistoryEntryAsync(historyEntry);
        }

        //get has not yet activated points, but it's time to do it
        var notActivatedPoints = await query
            .Where(historyEntry => !historyEntry.PointsBalance.HasValue && historyEntry.CreatedOnUtc < nowUtc)
            .OrderBy(historyEntry => historyEntry.CreatedOnUtc).ThenBy(historyEntry => historyEntry.Id).ToListAsync();

        if (!notActivatedPoints.Any())
            return;

        //get current points balance
        //LINQ to entities does not support Last method, thus order by desc and use First one
        var currentPointsBalance = (await query
            .OrderByDescending(historyEntry => historyEntry.CreatedOnUtc).ThenByDescending(historyEntry => historyEntry.Id)
            .FirstOrDefaultAsync(historyEntry => historyEntry.PointsBalance.HasValue))
            ?.PointsBalance ?? 0;

        //update appropriate records
        foreach (var historyEntry in notActivatedPoints)
        {
            currentPointsBalance += historyEntry.Points;
            historyEntry.PointsBalance = currentPointsBalance;
			_logger.LogDebug("UpdateRewardPointsBalanceAsync(query={Query}): historyEntry.PointsBalance → {Arg1}", query, historyEntry.PointsBalance);
            await UpdateRewardPointsHistoryEntryAsync(historyEntry);
        }
		_logger.LogWarning("<<< Exiting UpdateRewardPointsBalanceAsync(query={Query})", query);
    }

    /// <summary>
    /// Insert the reward point history entry
    /// </summary>
    /// <param name="rewardPointsHistory">Reward point history entry</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InsertRewardPointsHistoryEntryAsync(RewardPointsHistory rewardPointsHistory)
    {
		_logger.LogError(">>> Entering InsertRewardPointsHistoryEntryAsync(rewardPointsHistory={RewardPointsHistory})", rewardPointsHistory);
        await _rewardPointsHistoryRepository.InsertAsync(rewardPointsHistory);
		_logger.LogWarning("<<< Exiting InsertRewardPointsHistoryEntryAsync(rewardPointsHistory={RewardPointsHistory})", rewardPointsHistory);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Load reward point history records
    /// </summary>
    /// <param name="customerId">Customer identifier; 0 to load all records</param>
    /// <param name="storeId">Store identifier; pass null to load all records</param>
    /// <param name="showNotActivated">A value indicating whether to show reward points that did not yet activated</param>
    /// <param name="orderGuid">Order Guid; pass null to load all record</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the reward point history records
    /// </returns>
    public virtual async Task<IPagedList<RewardPointsHistory>> GetRewardPointsHistoryAsync(int customerId = 0, int? storeId = null,
        bool showNotActivated = false, Guid? orderGuid = null, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogError(">>> Entering GetRewardPointsHistoryAsync(customerId={CustomerId},storeId={StoreId},showNotActivated={ShowNotActivated},orderGuid={OrderGuid},pageIndex={PageIndex},pageSize={PageSize})", customerId, storeId, showNotActivated, orderGuid, pageIndex, pageSize);
        var query = await GetRewardPointsQueryAsync(customerId, storeId, showNotActivated);

		_logger.LogError("GetRewardPointsHistoryAsync(customerId={CustomerId},storeId={StoreId},showNotActivated={ShowNotActivated},orderGuid={OrderGuid},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", customerId, storeId, showNotActivated, orderGuid, pageIndex, pageSize, query);
        if (orderGuid.HasValue)
            query = query.Where(historyEntry => historyEntry.UsedWithOrder == orderGuid.Value);

        query = query.OrderByDescending(historyEntry => historyEntry.CreatedOnUtc)
            .ThenByDescending(historyEntry => historyEntry.Id);

        //return paged reward points history
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Gets reward points balance
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the balance
    /// </returns>
    public virtual async Task<int> GetRewardPointsBalanceAsync(int customerId, int storeId)
    {
		_logger.LogError(">>> Entering GetRewardPointsBalanceAsync(customerId={CustomerId},storeId={StoreId})", customerId, storeId);
        var query = (await GetRewardPointsQueryAsync(customerId, storeId))
            .OrderByDescending(historyEntry => historyEntry.CreatedOnUtc).ThenByDescending(historyEntry => historyEntry.Id);

        //return point balance of the first actual history entry
		_logger.LogWarning("<<< Exiting GetRewardPointsBalanceAsync(customerId={CustomerId},storeId={StoreId})", customerId, storeId);
        return (await query.FirstOrDefaultAsync())?.PointsBalance ?? 0;
    }

    /// <summary>
    /// Add reward points history record
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="points">Number of points to add</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="message">Message</param>
    /// <param name="usedWithOrder">The order for which points were redeemed (spent) as a payment</param>
    /// <param name="usedAmount">Used amount</param>
    /// <param name="activatingDate">Date and time of activating reward points; pass null to immediately activating</param>
    /// <param name="endDate">Date and time when the reward points will no longer be valid; pass null to add date termless points</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the reward points history entry identifier
    /// </returns>
    public virtual async Task<int> AddRewardPointsHistoryEntryAsync(Customer customer, int points, int storeId, string message = "",
        Order usedWithOrder = null, decimal usedAmount = 0M, DateTime? activatingDate = null, DateTime? endDate = null)
    {
		_logger.LogError(">>> Entering AddRewardPointsHistoryEntryAsync(customer={Customer},points={Points},storeId={StoreId},message={Message},usedWithOrder={UsedWithOrder},usedAmount={UsedAmount},activatingDate={ActivatingDate},endDate={EndDate})", customer, points, storeId, message, usedWithOrder, usedAmount, activatingDate, endDate);
        ArgumentNullException.ThrowIfNull(customer);

			_logger.LogError("Throwing ArgumentException from AddRewardPointsHistoryEntryAsync(customer={Customer},points={Points},storeId={StoreId},message={Message},usedWithOrder={UsedWithOrder},usedAmount={UsedAmount},activatingDate={ActivatingDate},endDate={EndDate})", customer, points, storeId, message, usedWithOrder, usedAmount, activatingDate, endDate);
        if (storeId == 0) {
            throw new ArgumentException("Store ID should be valid");
        }

        if (points < 0 && endDate.HasValue)
            throw new ArgumentException("End date is available only for positive points amount");

        //insert new history entry
        var newHistoryEntry = new RewardPointsHistory
        {
            CustomerId = customer.Id,
            StoreId = storeId,
            Points = points,
            PointsBalance = activatingDate.HasValue ? null : (int?)(await GetRewardPointsBalanceAsync(customer.Id, storeId) + points),
            UsedAmount = usedAmount,
            Message = message,
            CreatedOnUtc = activatingDate ?? DateTime.UtcNow,
            EndDateUtc = endDate,
            ValidPoints = points > 0 ? (int?)points : null,
            UsedWithOrder = usedWithOrder?.OrderGuid
        };
        await InsertRewardPointsHistoryEntryAsync(newHistoryEntry);

        //reduce valid points of previous entries
        if (points >= 0)
            return newHistoryEntry.Id;

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var withValidPoints = await (await GetRewardPointsQueryAsync(customer.Id, storeId))
            .Where(historyEntry => historyEntry.ValidPoints > 0)
            .OrderBy(historyEntry => historyEntry.CreatedOnUtc).ThenBy(historyEntry => historyEntry.Id).ToListAsync();
			_logger.LogError("AddRewardPointsHistoryEntryAsync(customer,points,storeId,message,usedWithOrder,usedAmount,activatingDate,endDate)={Customer},{Points},{StoreId},{Message},{UsedWithOrder},{UsedAmount},{ActivatingDate},{EndDate}: database query executed in {Arg8} ms", customer, points, storeId, message, usedWithOrder, usedAmount, activatingDate, endDate, __logStart.ElapsedMilliseconds);

        foreach (var historyEntry in withValidPoints)
        {
            points += historyEntry.ValidPoints.Value;
            historyEntry.ValidPoints = Math.Max(points, 0);
            await UpdateRewardPointsHistoryEntryAsync(historyEntry);

            if (points >= 0)
                break;
        }

		_logger.LogWarning("<<< Exiting AddRewardPointsHistoryEntryAsync(customer={Customer},points={Points},storeId={StoreId},message={Message},usedWithOrder={UsedWithOrder},usedAmount={UsedAmount},activatingDate={ActivatingDate},endDate={EndDate})", customer, points, storeId, message, usedWithOrder, usedAmount, activatingDate, endDate);
        return newHistoryEntry.Id;
    }

    /// <summary>
    /// Gets a reward point history entry
    /// </summary>
    /// <param name="rewardPointsHistoryId">Reward point history entry identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the reward point history entry
    /// </returns>
    public virtual async Task<RewardPointsHistory> GetRewardPointsHistoryEntryByIdAsync(int rewardPointsHistoryId)
    {
		_logger.LogError(">>> Entering GetRewardPointsHistoryEntryByIdAsync(rewardPointsHistoryId={RewardPointsHistoryId})", rewardPointsHistoryId);
        _logger.LogWarning("<<< Exiting GetRewardPointsHistoryEntryByIdAsync(rewardPointsHistoryId={RewardPointsHistoryId})", rewardPointsHistoryId);
        return await _rewardPointsHistoryRepository.GetByIdAsync(rewardPointsHistoryId);
    }

    /// <summary>
    /// Update the reward point history entry
    /// </summary>
    /// <param name="rewardPointsHistory">Reward point history entry</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateRewardPointsHistoryEntryAsync(RewardPointsHistory rewardPointsHistory)
    {
		_logger.LogError(">>> Entering UpdateRewardPointsHistoryEntryAsync(rewardPointsHistory={RewardPointsHistory})", rewardPointsHistory);
		_logger.LogWarning("<<< Exiting UpdateRewardPointsHistoryEntryAsync(rewardPointsHistory={RewardPointsHistory})", rewardPointsHistory);
        await _rewardPointsHistoryRepository.UpdateAsync(rewardPointsHistory);
    }

    /// <summary>
    /// Delete the reward point history entry
    /// </summary>
    /// <param name="rewardPointsHistory">Reward point history entry</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteRewardPointsHistoryEntryAsync(RewardPointsHistory rewardPointsHistory)
    {
		_logger.LogError(">>> Entering DeleteRewardPointsHistoryEntryAsync(rewardPointsHistory={RewardPointsHistory})", rewardPointsHistory);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _rewardPointsHistoryRepository.DeleteAsync(rewardPointsHistory);
		_logger.LogError("DeleteRewardPointsHistoryEntryAsync(rewardPointsHistory)={RewardPointsHistory}: external service call took {Arg1} ms", rewardPointsHistory, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteRewardPointsHistoryEntryAsync(rewardPointsHistory={RewardPointsHistory})", rewardPointsHistory);
    }

    #endregion
}