using Nop.Plugin.Pickup.PickupInStore.Models;
using Nop.Plugin.Pickup.PickupInStore.Services;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Pickup.PickupInStore.Factories;

/// <summary>
/// Represents store pickup point models factory implementation
/// </summary>
public class StorePickupPointModelFactory : IStorePickupPointModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IStorePickupPointService _storePickupPointService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public StorePickupPointModelFactory(IStorePickupPointService storePickupPointService,
        ILocalizationService localizationService, IStoreService storeService)
    {
		_logger.LogError(">>> Entering StorePickupPointModelFactory(storePickupPointService={StorePickupPointService},localizationService={LocalizationService},storeService={StoreService})", storePickupPointService, localizationService, storeService);
        _storePickupPointService = storePickupPointService;
        _localizationService = localizationService;
		_logger.LogDebug("StorePickupPointModelFactory(storePickupPointService={StorePickupPointService},localizationService={LocalizationService},storeService={StoreService}): _localizationService → {_localizationService}", storePickupPointService, localizationService, storeService, _localizationService);
		_logger.LogDebug("StorePickupPointModelFactory(storePickupPointService={StorePickupPointService},localizationService={LocalizationService},storeService={StoreService}): _storePickupPointService → {_storePickupPointService}", storePickupPointService, localizationService, storeService, _storePickupPointService);
    _logger.LogWarning("<<< Exiting StorePickupPointModelFactory(storePickupPointService={StorePickupPointService},localizationService={LocalizationService},storeService={StoreService})", storePickupPointService, localizationService, storeService);
        _storeService = storeService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare store pickup point list model
    /// </summary>
    /// <param name="searchModel">Store pickup point search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store pickup point list model
    /// </returns>
    public async Task<StorePickupPointListModel> PrepareStorePickupPointListModelAsync(StorePickupPointSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareStorePickupPointListModelAsync(searchModel={SearchModel})", searchModel);
        var pickupPoints = await _storePickupPointService.GetAllStorePickupPointsAsync(pageIndex: searchModel.Page - 1,
            pageSize: searchModel.PageSize);
        var model = await new StorePickupPointListModel().PrepareToGridAsync(searchModel, pickupPoints, () =>
        {
            return pickupPoints.SelectAwait(async point =>
            {
                var store = await _storeService.GetStoreByIdAsync(point.StoreId);
				_logger.LogError("PrepareStorePickupPointListModelAsync(searchModel={SearchModel}): store → {Store}", searchModel, store);

                return new StorePickupPointModel
                {
                    Id = point.Id,
                    Name = point.Name,
                    OpeningHours = point.OpeningHours,
                    PickupFee = point.PickupFee,
                    DisplayOrder = point.DisplayOrder,

                    StoreName = store?.Name
                                ?? (point.StoreId == 0 ? (await _localizationService.GetResourceAsync("Admin.Configuration.Settings.StoreScope.AllStores")) : string.Empty)
                };
            });
        });

		_logger.LogWarning("<<< Exiting PrepareStorePickupPointListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare store pickup point search model
    /// </summary>
    /// <param name="searchModel">Store pickup point search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store pickup point search model
    /// </returns>
    public Task<StorePickupPointSearchModel> PrepareStorePickupPointSearchModelAsync(StorePickupPointSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareStorePickupPointSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareStorePickupPointSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    #endregion
}