using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Stores;

/// <summary>
/// Store mapping service
/// </summary>
public partial class StoreMappingService : IStoreMappingService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly INopDataProvider _dataProvider;
    protected readonly IRepository<StoreMapping> _storeMappingRepository;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public StoreMappingService(CatalogSettings catalogSettings,
        INopDataProvider dataProvider,
        IRepository<StoreMapping> storeMappingRepository,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreService storeService)
    {
		_logger.LogError(">>> Entering StoreMappingService(catalogSettings={CatalogSettings},dataProvider={DataProvider},storeMappingRepository=<redacted>,staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeService={StoreService})", catalogSettings, dataProvider, staticCacheManager, storeContext, storeService);
        _catalogSettings = catalogSettings;
        _dataProvider = dataProvider;
		_logger.LogError("StoreMappingService(catalogSettings={CatalogSettings},dataProvider={DataProvider},storeMappingRepository=<redacted>,staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeService={StoreService}): _dataProvider → {_dataProvider}", catalogSettings, dataProvider, staticCacheManager, storeContext, storeService, _dataProvider);
		_logger.LogDebug("StoreMappingService(catalogSettings={CatalogSettings},dataProvider={DataProvider},storeMappingRepository=<redacted>,staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeService={StoreService}): _dataProvider → {_dataProvider}", catalogSettings, dataProvider, staticCacheManager, storeContext, storeService, _dataProvider);
        _storeMappingRepository = storeMappingRepository;
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("StoreMappingService(catalogSettings={CatalogSettings},dataProvider={DataProvider},storeMappingRepository=<redacted>,staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeService={StoreService}): _storeMappingRepository → {_storeMappingRepository}", catalogSettings, dataProvider, staticCacheManager, storeContext, storeService, _storeMappingRepository);
		_logger.LogDebug("StoreMappingService(catalogSettings={CatalogSettings},dataProvider={DataProvider},storeMappingRepository=<redacted>,staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeService={StoreService}): _staticCacheManager → {_staticCacheManager}", catalogSettings, dataProvider, staticCacheManager, storeContext, storeService, _staticCacheManager);
        _storeContext = storeContext;
        _storeService = storeService;
		_logger.LogDebug("StoreMappingService(catalogSettings={CatalogSettings},dataProvider={DataProvider},storeMappingRepository=<redacted>,staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeService={StoreService}): _storeContext → {_storeContext}", catalogSettings, dataProvider, staticCacheManager, storeContext, storeService, _storeContext);
    _logger.LogWarning("<<< Exiting StoreMappingService(catalogSettings={CatalogSettings},dataProvider={DataProvider},storeMappingRepository=<redacted>,staticCacheManager={StaticCacheManager},storeContext={StoreContext},storeService={StoreService})", catalogSettings, dataProvider, staticCacheManager, storeContext, storeService);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Inserts a store mapping record
    /// </summary>
    /// <param name="storeMapping">Store mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InsertStoreMappingAsync(StoreMapping storeMapping)
    {
		_logger.LogError(">>> Entering InsertStoreMappingAsync(storeMapping=<redacted>)");
        await _storeMappingRepository.InsertAsync(storeMapping);
		_logger.LogWarning("<<< Exiting InsertStoreMappingAsync(storeMapping=<redacted>)");
		_logger.LogError(">>> Entering InsertStoreMappingAsync(entity={Entity},storeId={StoreId})", entity, storeId);
		_logger.LogWarning("<<< Exiting InsertStoreMappingAsync(entity={Entity},storeId={StoreId})", entity, storeId);
		_logger.LogError("Throwing ArgumentOutOfRangeException from InsertStoreMappingAsync(entity={Entity},storeId={StoreId})", entity, storeId);
    }

    /// <summary>
    /// Get a value indicating whether a store mapping exists for an entity type
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if exists; otherwise false
    /// </returns>
    protected virtual async Task<bool> IsEntityMappingExistsAsync<TEntity>() where TEntity : BaseEntity, IStoreMappingSupported
    {
		_logger.LogError(">>> Entering IsEntityMappingExistsAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var entityName = typeof(TEntity).Name;
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopStoreDefaults.StoreMappingExistsCacheKey, entityName);
		_logger.LogError("IsEntityMappingExistsAsync(): entityName → {EntityName}", entityName);
		_logger.LogError("IsEntityMappingExistsAsync(): key → {Key}", key);

        var query = from sm in _storeMappingRepository.Table
            where sm.EntityName == entityName
            select sm.StoreId;
			_logger.LogWarning("<<< Exiting IsEntityMappingExistsAsync()");
			_logger.LogError("IsEntityMappingExistsAsync(): find query executing");
			_logger.LogError("IsEntityMappingExistsAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);

        return await _staticCacheManager.GetAsync(key, async () => await query.AnyAsync());
    }

    #endregion

    #region Methods

    /// <summary>
    /// Apply store mapping to the passed query
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
    /// <param name="query">Query to filter</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filtered query
    /// </returns>
    public virtual async Task<IQueryable<TEntity>> ApplyStoreMapping<TEntity>(IQueryable<TEntity> query, int storeId)
        where TEntity : BaseEntity, IStoreMappingSupported
    {
		_logger.LogError(">>> Entering ApplyStoreMapping(query={Query},storeId={StoreId})", query, storeId);
        ArgumentNullException.ThrowIfNull(query);

        if (storeId == 0 || _catalogSettings.IgnoreStoreLimitations || !await IsEntityMappingExistsAsync<TEntity>())
            return query;

			_logger.LogWarning("<<< Exiting ApplyStoreMapping(query={Query},storeId={StoreId})", query, storeId);
        return from entity in query
            where !entity.LimitedToStores || _storeMappingRepository.Table.Any(sm =>
                sm.EntityName == typeof(TEntity).Name && sm.EntityId == entity.Id && sm.StoreId == storeId)
            select entity;
    }

    /// <summary>
    /// Deletes a store mapping record
    /// </summary>
    /// <param name="storeMapping">Store mapping record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteStoreMappingAsync(StoreMapping storeMapping)
    {
		_logger.LogError(">>> Entering DeleteStoreMappingAsync(storeMapping=<redacted>)");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _storeMappingRepository.DeleteAsync(storeMapping);
		_logger.LogError("DeleteStoreMappingAsync(storeMapping)=<redacted>: external service call took {Arg1} ms", __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteStoreMappingAsync(storeMapping=<redacted>)");
    }

    /// <summary>
    /// Gets store mapping records
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
    /// <param name="entity">Entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store mapping records
    /// </returns>
    public virtual async Task<IList<StoreMapping>> GetStoreMappingsAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported
    {
		_logger.LogError(">>> Entering GetStoreMappingsAsync(entity={Entity})", entity);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(entity);

        var entityId = entity.Id;
        var entityName = entity.GetType().Name;
		_logger.LogError("GetStoreMappingsAsync(entity={Entity}): entityName → {EntityName}", entity, entityName);

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopStoreDefaults.StoreMappingsCacheKey, entityId, entityName);
		_logger.LogError("GetStoreMappingsAsync(entity={Entity}): key → {Key}", entity, key);

        var query = from sm in _storeMappingRepository.Table
            where sm.EntityId == entityId &&
                  sm.EntityName == entityName
            select sm;
			_logger.LogWarning("<<< Exiting GetStoreMappingsAsync(entity={Entity})", entity);
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogError("GetStoreMappingsAsync(entity)={Entity}: external service call took {Arg1} ms", entity, __logStart.ElapsedMilliseconds);
			_logger.LogError("GetStoreMappingsAsync(entity)={Entity}: find query executed in {Arg1} ms", entity, __logStartDb.ElapsedMilliseconds);

        var storeMappings = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
		_logger.LogError("GetStoreMappingsAsync(entity={Entity}): storeMappings → {StoreMappings}", entity, storeMappings);

        return storeMappings;
    }

    /// <summary>
    /// Inserts a store mapping record
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="storeId">Store id</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertStoreMappingAsync<TEntity>(TEntity entity, int storeId) where TEntity : BaseEntity, IStoreMappingSupported
    {
        ArgumentNullException.ThrowIfNull(entity);

			_logger.LogWarning("<<< Exiting InsertStoreMappingAsync(entity={Entity},storeId={StoreId})", entity, storeId);
			_logger.LogError("Throwing ArgumentOutOfRangeException from InsertStoreMappingAsync(entity={Entity},storeId={StoreId})", entity, storeId);
        if (storeId == 0) {
            throw new ArgumentOutOfRangeException(nameof(storeId));
        }

        var entityId = entity.Id;
        var entityName = entity.GetType().Name;
		_logger.LogError("InsertStoreMappingAsync(entity={Entity},storeId={StoreId}): entityName → {EntityName}", entity, storeId, entityName);

        var storeMapping = new StoreMapping
        {
            EntityId = entityId,
            EntityName = entityName,
            StoreId = storeId
        };

        await InsertStoreMappingAsync(storeMapping);
    }

    /// <summary>
    /// Find store identifiers with granted access (mapped to the entity)
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
    /// <param name="entity">Entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store identifiers
    /// </returns>
    public virtual async Task<int[]> GetStoresIdsWithAccessAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported
    {
		_logger.LogError(">>> Entering GetStoresIdsWithAccessAsync(entity={Entity})", entity);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(entity);

        var entityId = entity.Id;
        var entityName = entity.GetType().Name;
		_logger.LogError("GetStoresIdsWithAccessAsync(entity={Entity}): entityName → {EntityName}", entity, entityName);

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopStoreDefaults.StoreMappingIdsCacheKey, entityId, entityName);
		_logger.LogError("GetStoresIdsWithAccessAsync(entity={Entity}): key → {Key}", entity, key);

        var query = from sm in _storeMappingRepository.Table
            where sm.EntityId == entityId &&
                  sm.EntityName == entityName
            select sm.StoreId;
			_logger.LogWarning("<<< Exiting GetStoresIdsWithAccessAsync(entity={Entity})", entity);
			_logger.LogError("GetStoresIdsWithAccessAsync(entity)={Entity}: find query executing", entity);
			_logger.LogError("GetStoresIdsWithAccessAsync(entity)={Entity}: external service call took {Arg1} ms", entity, __logStart.ElapsedMilliseconds);

        return await _staticCacheManager.GetAsync(key, () => query.ToArrayAsync());
    }

    /// <summary>
    /// Find store identifiers with granted access (mapped to the entity)
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
    /// <param name="entity">Entity</param>
    /// <returns>
    /// The store identifiers
    /// </returns>
    public virtual int[] GetStoresIdsWithAccess<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported
    {
		_logger.LogError(">>> Entering GetStoresIdsWithAccess(entity={Entity})", entity);
        ArgumentNullException.ThrowIfNull(entity);

        var entityId = entity.Id;
        var entityName = entity.GetType().Name;
		_logger.LogError("GetStoresIdsWithAccess(entity={Entity}): entityName → {EntityName}", entity, entityName);

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopStoreDefaults.StoreMappingIdsCacheKey, entityId, entityName);
		_logger.LogError("GetStoresIdsWithAccess(entity={Entity}): key → {Key}", entity, key);

        var query = from sm in _storeMappingRepository.Table
            where sm.EntityId == entityId &&
                  sm.EntityName == entityName
            select sm.StoreId;

		_logger.LogWarning("<<< Exiting GetStoresIdsWithAccess(entity={Entity})", entity);
        return _staticCacheManager.Get(key, () => query.ToArray());
    }

    /// <summary>
    /// Authorize whether entity could be accessed in the current store (mapped to this store)
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
    /// <param name="entity">Entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported
    {
		_logger.LogError(">>> Entering AuthorizeAsync(entity={Entity})", entity);
        var store = await _storeContext.GetCurrentStoreAsync();

		_logger.LogError("AuthorizeAsync(entity={Entity}): store → {Store}", entity, store);
		_logger.LogWarning("<<< Exiting AuthorizeAsync(entity={Entity})", entity);
		_logger.LogWarning("<<< Exiting AuthorizeAsync(entity={Entity},storeId={StoreId})", entity, storeId);
        return await AuthorizeAsync(entity, store.Id);
    }

    /// <summary>
    /// Authorize whether entity could be accessed in a store (mapped to this store)
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync<TEntity>(TEntity entity, int storeId) where TEntity : BaseEntity, IStoreMappingSupported
    {
        if (entity == null)
            return false;

        if (storeId == 0)
            //return true if no store specified/found
            return true;

        if (_catalogSettings.IgnoreStoreLimitations)
            return true;

        if (!entity.LimitedToStores)
            return true;

        foreach (var storeIdWithAccess in await GetStoresIdsWithAccessAsync(entity))
            if (storeId == storeIdWithAccess)
                //yes, we have such permission
                return true;

        //no permission found
        return false;
    }

    /// <summary>
    /// Authorize whether entity could be accessed in a store (mapped to this store)
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// True - authorized; otherwise, false
    /// </returns>
    public virtual bool Authorize<TEntity>(TEntity entity, int storeId) where TEntity : BaseEntity, IStoreMappingSupported
    {
		_logger.LogError(">>> Entering Authorize(entity={Entity},storeId={StoreId})", entity, storeId);
        if (entity == null)
            return false;

        if (storeId == 0)
            //return true if no store specified/found
            return true;

        if (_catalogSettings.IgnoreStoreLimitations)
            return true;

        if (!entity.LimitedToStores)
            return true;

        foreach (var storeIdWithAccess in GetStoresIdsWithAccess(entity))
            if (storeId == storeIdWithAccess)
                //yes, we have such permission
                return true;

        //no permission found
		_logger.LogWarning("<<< Exiting Authorize(entity={Entity},storeId={StoreId})", entity, storeId);
        return false;
    }

    /// <summary>
    /// Save store mappings for the passed entity
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports store mapping</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="storeIds">Store identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    public virtual async Task SaveStoreMappingsAsync<TEntity>(TEntity entity, IEnumerable<int> storeIds) where TEntity : BaseEntity, IStoreMappingSupported
    {
		_logger.LogError(">>> Entering SaveStoreMappingsAsync(entity={Entity},storeIds={StoreIds})", entity, storeIds);
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(storeIds);

        ArgumentOutOfRangeException.ThrowIfZero(entity.Id);

        var allStores = await _storeService.GetAllStoresAsync();
		_logger.LogError("SaveStoreMappingsAsync(entity={Entity},storeIds={StoreIds}): allStores → {AllStores}", entity, storeIds, allStores);
        var existingStoreMappings = await GetStoreMappingsAsync(entity);
		_logger.LogError("SaveStoreMappingsAsync(entity={Entity},storeIds={StoreIds}): existingStoreMappings → {ExistingStoreMappings}", entity, storeIds, existingStoreMappings);

        if (entity.LimitedToStores != storeIds.Any())
        {
            entity.LimitedToStores = storeIds.Any();
            await _dataProvider.UpdateEntityAsync(entity);
        }

        foreach (var store in allStores)
        {
            if (storeIds.Contains(store.Id))
            {
                //new store
                if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                    await InsertStoreMappingAsync(entity, store.Id);
            }
            else
            {
                //remove store
                var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
				_logger.LogError("SaveStoreMappingsAsync(entity={Entity},storeIds={StoreIds}): storeMappingToDelete → {StoreMappingToDelete}", entity, storeIds, storeMappingToDelete);
                if (storeMappingToDelete != null)
                    await DeleteStoreMappingAsync(storeMappingToDelete);
            }
        }
		_logger.LogWarning("<<< Exiting SaveStoreMappingsAsync(entity={Entity},storeIds={StoreIds})", entity, storeIds);
    }

    #endregion
}