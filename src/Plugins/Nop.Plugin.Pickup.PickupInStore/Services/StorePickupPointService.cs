using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.Pickup.PickupInStore.Domain;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Pickup.PickupInStore.Services;

/// <summary>
/// Store pickup point service
/// </summary>
public class StorePickupPointService : IStorePickupPointService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Constants

    /// <summary>
    /// Cache key for pickup points
    /// </summary>
    /// <remarks>
    /// {0} : current store ID
    /// </remarks>
    protected readonly CacheKey _pickupPointAllKey = new("Nop.pickuppoint.all-{0}");
    protected const string PICKUP_POINT_PATTERN_KEY = "Nop.pickuppoint.";

    #endregion

    #region Fields

    protected readonly IRepository<StorePickupPoint> _storePickupPointRepository;
    protected readonly IShortTermCacheManager _shortTermCacheManager;
    protected readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="storePickupPointRepository">Store pickup point repository</param>
    /// <param name="shortTermCacheManager">Short term cache manager</param>
    /// <param name="staticCacheManager">Cache manager</param>
    public StorePickupPointService(IRepository<StorePickupPoint> storePickupPointRepository,
        IShortTermCacheManager shortTermCacheManager,
        IStaticCacheManager staticCacheManager)
    {
		_logger.LogDebug(">>> Entering StorePickupPointService(storePickupPointRepository={StorePickupPointRepository},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager})", storePickupPointRepository, shortTermCacheManager, staticCacheManager);
        _storePickupPointRepository = storePickupPointRepository;
        _shortTermCacheManager = shortTermCacheManager;
		_logger.LogDebug("StorePickupPointService(storePickupPointRepository={StorePickupPointRepository},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager}): _shortTermCacheManager → {_shortTermCacheManager}", storePickupPointRepository, shortTermCacheManager, staticCacheManager, _shortTermCacheManager);
		_logger.LogDebug("StorePickupPointService(storePickupPointRepository={StorePickupPointRepository},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager}): _storePickupPointRepository → {_storePickupPointRepository}", storePickupPointRepository, shortTermCacheManager, staticCacheManager, _storePickupPointRepository);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("StorePickupPointService(storePickupPointRepository={StorePickupPointRepository},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager}): _staticCacheManager → {_staticCacheManager}", storePickupPointRepository, shortTermCacheManager, staticCacheManager, _staticCacheManager);
		_logger.LogDebug("<<< Exiting StorePickupPointService(storePickupPointRepository={StorePickupPointRepository},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager})", storePickupPointRepository, shortTermCacheManager, staticCacheManager);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets all pickup points
    /// </summary>
    /// <param name="storeId">The store identifier; pass 0 to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the pickup points
    /// </returns>
    public virtual async Task<IPagedList<StorePickupPoint>> GetAllStorePickupPointsAsync(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering GetAllStorePickupPointsAsync(storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize})", storeId, pageIndex, pageSize);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var rez = await _shortTermCacheManager.GetAsync(async () => await _storePickupPointRepository.GetAllAsync(query =>
        {
            if (storeId > 0)
                query = query.Where(point => point.StoreId == storeId || point.StoreId == 0);
            query = query.OrderBy(point => point.DisplayOrder).ThenBy(point => point.Name);
			_logger.LogInformation("GetAllStorePickupPointsAsync(storeId,pageIndex,pageSize)={StoreId},{PageIndex},{PageSize}: find query executed in {Arg3} ms", storeId, pageIndex, pageSize, __logStart.ElapsedMilliseconds);

            return query;
        }), _pickupPointAllKey, storeId);

		_logger.LogInformation("GetAllStorePickupPointsAsync(storeId,pageIndex,pageSize)={StoreId},{PageIndex},{PageSize}: external service call took {Arg3} ms", storeId, pageIndex, pageSize, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetAllStorePickupPointsAsync(storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize})", storeId, pageIndex, pageSize);
        return new PagedList<StorePickupPoint>(rez, pageIndex, pageSize);
    }

    /// <summary>
    /// Gets a pickup point
    /// </summary>
    /// <param name="pickupPointId">Pickup point identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the pickup point
    /// </returns>
    public virtual async Task<StorePickupPoint> GetStorePickupPointByIdAsync(int pickupPointId)
    {
		_logger.LogDebug(">>> Entering GetStorePickupPointByIdAsync(pickupPointId={PickupPointId})", pickupPointId);
		_logger.LogDebug("<<< Exiting GetStorePickupPointByIdAsync(pickupPointId={PickupPointId})", pickupPointId);
        return await _storePickupPointRepository.GetByIdAsync(pickupPointId);
    }

    /// <summary>
    /// Inserts a pickup point
    /// </summary>
    /// <param name="pickupPoint">Pickup point</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertStorePickupPointAsync(StorePickupPoint pickupPoint)
    {
		_logger.LogDebug(">>> Entering InsertStorePickupPointAsync(pickupPoint={PickupPoint})", pickupPoint);
        await _storePickupPointRepository.InsertAsync(pickupPoint, false);
        await _staticCacheManager.RemoveByPrefixAsync(PICKUP_POINT_PATTERN_KEY);
		_logger.LogInformation("InsertStorePickupPointAsync(pickupPoint)={PickupPoint}: save query executed in {Arg1} ms", pickupPoint, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting InsertStorePickupPointAsync(pickupPoint={PickupPoint})", pickupPoint);
    }

    /// <summary>
    /// Updates the pickup point
    /// </summary>
    /// <param name="pickupPoint">Pickup point</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateStorePickupPointAsync(StorePickupPoint pickupPoint)
    {
		_logger.LogDebug(">>> Entering UpdateStorePickupPointAsync(pickupPoint={PickupPoint})", pickupPoint);
        await _storePickupPointRepository.UpdateAsync(pickupPoint, false);
        await _staticCacheManager.RemoveByPrefixAsync(PICKUP_POINT_PATTERN_KEY);
		_logger.LogInformation("UpdateStorePickupPointAsync(pickupPoint)={PickupPoint}: update query executed in {Arg1} ms", pickupPoint, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting UpdateStorePickupPointAsync(pickupPoint={PickupPoint})", pickupPoint);
    }

    /// <summary>
    /// Deletes a pickup point
    /// </summary>
    /// <param name="pickupPoint">Pickup point</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteStorePickupPointAsync(StorePickupPoint pickupPoint)
    {
		_logger.LogDebug(">>> Entering DeleteStorePickupPointAsync(pickupPoint={PickupPoint})", pickupPoint);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _storePickupPointRepository.DeleteAsync(pickupPoint, false);
        await _staticCacheManager.RemoveByPrefixAsync(PICKUP_POINT_PATTERN_KEY);
		_logger.LogInformation("DeleteStorePickupPointAsync(pickupPoint)={PickupPoint}: external service call took {Arg1} ms", pickupPoint, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting DeleteStorePickupPointAsync(pickupPoint={PickupPoint})", pickupPoint);
    }

    #endregion
}