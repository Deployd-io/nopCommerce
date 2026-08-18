using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Stores;
using Nop.Services.Stores;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Factories;

/// <summary>
/// Represents the base store mapping supported model factory implementation
/// </summary>
public partial class StoreMappingSupportedModelFactory : IStoreMappingSupportedModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public StoreMappingSupportedModelFactory(IStoreMappingService storeMappingService,
        IStoreService storeService)
    {
		_logger.LogDebug(">>> Entering StoreMappingSupportedModelFactory(storeMappingService=<redacted>,storeService={StoreService})", storeService);
        _storeMappingService = storeMappingService;
        _storeService = storeService;
		_logger.LogDebug("StoreMappingSupportedModelFactory(storeMappingService=<redacted>,storeService={StoreService}): _storeService → {_storeService}", storeService, _storeService);
		_logger.LogDebug("StoreMappingSupportedModelFactory(storeMappingService=<redacted>,storeService={StoreService}): _storeMappingService → {_storeMappingService}", storeService, _storeMappingService);
		_logger.LogDebug("<<< Exiting StoreMappingSupportedModelFactory(storeMappingService=<redacted>,storeService={StoreService})", storeService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare selected and all available stores for the passed model
    /// </summary>
    /// <typeparam name="TModel">Store mapping supported model type</typeparam>
    /// <param name="model">Model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareModelStoresAsync<TModel>(TModel model) where TModel : IStoreMappingSupportedModel
    {
		_logger.LogDebug(">>> Entering PrepareModelStoresAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        //prepare available stores
        var availableStores = await _storeService.GetAllStoresAsync();
		_logger.LogDebug("PrepareModelStoresAsync(model={Model}): availableStores → {AvailableStores}", model, availableStores);
        model.AvailableStores = availableStores.Select(store => new SelectListItem
        {
            Text = store.Name,
            Value = store.Id.ToString(),
            Selected = model.SelectedStoreIds.Contains(store.Id)
        }).ToList();
		_logger.LogDebug("<<< Exiting PrepareModelStoresAsync(model={Model})", model);
    }

    /// <summary>
    /// Prepare selected and all available stores for the passed model by store mappings
    /// </summary>
    /// <typeparam name="TModel">Store mapping supported model type</typeparam>
    /// <typeparam name="TEntity">Store mapping supported entity type</typeparam>
    /// <param name="model">Model</param>
    /// <param name="entity">Entity</param>
    /// <param name="ignoreStoreMappings">Whether to ignore existing store mappings</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareModelStoresAsync<TModel, TEntity>(TModel model, TEntity entity, bool ignoreStoreMappings)
        where TModel : IStoreMappingSupportedModel where TEntity : BaseEntity, IStoreMappingSupported
    {
        ArgumentNullException.ThrowIfNull(model);

        //prepare stores with granted access
        if (!ignoreStoreMappings && entity != null)
            model.SelectedStoreIds = (await _storeMappingService.GetStoresIdsWithAccessAsync(entity)).ToList();

        await PrepareModelStoresAsync(model);
    }

    #endregion
}