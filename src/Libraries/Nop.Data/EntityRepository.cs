using System.Linq.Expressions;
using System.Transactions;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Domain.Common;
using Nop.Core.Events;
using Microsoft.Extensions.Logging;

namespace Nop.Data;

/// <summary>
/// Represents the entity repository implementation
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public partial class EntityRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IEventPublisher _eventPublisher;
    protected readonly INopDataProvider _dataProvider;
    protected readonly IShortTermCacheManager _shortTermCacheManager;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly bool _usingDistributedCache;

    #endregion

    #region Ctor

    public EntityRepository(IEventPublisher eventPublisher,
        INopDataProvider dataProvider,
        IShortTermCacheManager shortTermCacheManager,
        IStaticCacheManager staticCacheManager,
        AppSettings appSettings)
    {
		_logger.LogError(">>> Entering EntityRepository(eventPublisher={EventPublisher},dataProvider={DataProvider},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},appSettings={AppSettings})", eventPublisher, dataProvider, shortTermCacheManager, staticCacheManager, appSettings);
        _eventPublisher = eventPublisher;
		_logger.LogDebug("EntityRepository(eventPublisher={EventPublisher},dataProvider={DataProvider},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},appSettings={AppSettings}): _eventPublisher → {_eventPublisher}", eventPublisher, dataProvider, shortTermCacheManager, staticCacheManager, appSettings, _eventPublisher);
        _dataProvider = dataProvider;
		_logger.LogError("EntityRepository(eventPublisher={EventPublisher},dataProvider={DataProvider},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},appSettings={AppSettings}): _dataProvider → {_dataProvider}", eventPublisher, dataProvider, shortTermCacheManager, staticCacheManager, appSettings, _dataProvider);
		_logger.LogDebug("EntityRepository(eventPublisher={EventPublisher},dataProvider={DataProvider},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},appSettings={AppSettings}): _dataProvider → {_dataProvider}", eventPublisher, dataProvider, shortTermCacheManager, staticCacheManager, appSettings, _dataProvider);
        _shortTermCacheManager = shortTermCacheManager;
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("EntityRepository(eventPublisher={EventPublisher},dataProvider={DataProvider},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},appSettings={AppSettings}): _shortTermCacheManager → {_shortTermCacheManager}", eventPublisher, dataProvider, shortTermCacheManager, staticCacheManager, appSettings, _shortTermCacheManager);
        _usingDistributedCache = appSettings.Get<DistributedCacheConfig>().DistributedCacheType switch
        {
            DistributedCacheType.Redis => true,
            DistributedCacheType.SqlServer => true,
            _ => false
        };
		_logger.LogDebug("EntityRepository(eventPublisher={EventPublisher},dataProvider={DataProvider},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},appSettings={AppSettings}): _staticCacheManager → {_staticCacheManager}", eventPublisher, dataProvider, shortTermCacheManager, staticCacheManager, appSettings, _staticCacheManager);
    _logger.LogDebug("EntityRepository(eventPublisher={EventPublisher},dataProvider={DataProvider},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},appSettings={AppSettings}): _usingDistributedCache → {_usingDistributedCache}", eventPublisher, dataProvider, shortTermCacheManager, staticCacheManager, appSettings, _usingDistributedCache);
    _logger.LogWarning("<<< Exiting EntityRepository(eventPublisher={EventPublisher},dataProvider={DataProvider},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},appSettings={AppSettings})", eventPublisher, dataProvider, shortTermCacheManager, staticCacheManager, appSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="getAllAsync">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    protected virtual async Task<IList<TEntity>> GetEntitiesAsync(Func<Task<IList<TEntity>>> getAllAsync, Func<IStaticCacheManager, CacheKey> getCacheKey)
    {
        if (getCacheKey == null)
            return await getAllAsync();

        //caching
        var cacheKey = getCacheKey(_staticCacheManager)
                       ?? _staticCacheManager.PrepareKeyForDefaultCache(NopEntityCacheDefaults<TEntity>.AllCacheKey);
		_logger.LogWarning("<<< Exiting GetEntitiesAsync(getAllAsync={GetAllAsync},getCacheKey={GetCacheKey})", getAllAsync, getCacheKey);
        return await _staticCacheManager.GetAsync(cacheKey, getAllAsync);
    }

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="getAll">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <returns>Entity entries</returns>
    protected virtual IList<TEntity> GetEntities(Func<IList<TEntity>> getAll, Func<IStaticCacheManager, CacheKey> getCacheKey)
    {
		_logger.LogError(">>> Entering GetEntities(getAll={GetAll},getCacheKey={GetCacheKey})", getAll, getCacheKey);
        if (getCacheKey == null)
            return getAll();

        //caching
        var cacheKey = getCacheKey(_staticCacheManager)
                       ?? _staticCacheManager.PrepareKeyForDefaultCache(NopEntityCacheDefaults<TEntity>.AllCacheKey);

		_logger.LogWarning("<<< Exiting GetEntities(getAll={GetAll},getCacheKey={GetCacheKey})", getAll, getCacheKey);
        return _staticCacheManager.Get(cacheKey, getAll);
    }

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="getAllAsync">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    protected virtual async Task<IList<TEntity>> GetEntitiesAsync(Func<Task<IList<TEntity>>> getAllAsync, Func<IStaticCacheManager, Task<CacheKey>> getCacheKey)
    {
        if (getCacheKey == null)
            return await getAllAsync();

        //caching
        var cacheKey = await getCacheKey(_staticCacheManager)
                       ?? _staticCacheManager.PrepareKeyForDefaultCache(NopEntityCacheDefaults<TEntity>.AllCacheKey);
        return await _staticCacheManager.GetAsync(cacheKey, getAllAsync);
    }

    /// <summary>
    /// Adds "deleted" filter to query which contains <see cref="ISoftDeletedEntity"/> entries, if its need
    /// </summary>
    /// <param name="query">Entity entries</param>
    /// <param name="includeDeleted">Whether to include deleted items</param>
    /// <returns>Entity entries</returns>
    protected virtual IQueryable<TEntity> AddDeletedFilter(IQueryable<TEntity> query, in bool includeDeleted)
    {
		_logger.LogError(">>> Entering AddDeletedFilter(query={Query},includeDeleted={IncludeDeleted})", includeDeleted);
        if (includeDeleted)
            return query;

        if (typeof(TEntity).GetInterface(nameof(ISoftDeletedEntity)) == null)
            return query;

        return query.OfType<ISoftDeletedEntity>().Where(entry => !entry.Deleted).OfType<TEntity>();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get the entity entry
    /// </summary>
    /// <param name="id">Entity entry identifier</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="Nop.Core.Domain.Common.ISoftDeletedEntity"/> entities)</param>
    /// <param name="useShortTermCache">Whether to use short term cache instead of static cache</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entry
    /// </returns>
    public virtual async Task<TEntity> GetByIdAsync(int? id, Func<ICacheKeyService, CacheKey> getCacheKey = null, bool includeDeleted = true, bool useShortTermCache = false)
    {
		_logger.LogError(">>> Entering GetByIdAsync(id={Id},getCacheKey={GetCacheKey},includeDeleted={IncludeDeleted},useShortTermCache={UseShortTermCache})", id, getCacheKey, includeDeleted, useShortTermCache);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (!id.HasValue || id == 0)
            return null;

        async Task<TEntity> getEntityAsync()
        {
			_logger.LogError(">>> Entering getEntityAsync()");
            return await AddDeletedFilter(Table, includeDeleted).FirstOrDefaultAsync(entity => entity.Id == Convert.ToInt32(id));
        }

        if (getCacheKey == null)
            return await getEntityAsync();

        ICacheKeyService cacheKeyService = useShortTermCache ? _shortTermCacheManager : _staticCacheManager;

        //caching
        var cacheKey = getCacheKey(cacheKeyService)
                       ?? cacheKeyService.PrepareKeyForDefaultCache(NopEntityCacheDefaults<TEntity>.ByIdCacheKey, id);

        if (useShortTermCache)
            return await _shortTermCacheManager.GetAsync(getEntityAsync, cacheKey);

		_logger.LogError("GetByIdAsync(id,getCacheKey,includeDeleted,useShortTermCache)={Id},{GetCacheKey},{IncludeDeleted},{UseShortTermCache}: external service call took {Arg4} ms", id, getCacheKey, includeDeleted, useShortTermCache, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetByIdAsync(id={Id},getCacheKey={GetCacheKey},includeDeleted={IncludeDeleted},useShortTermCache={UseShortTermCache})", id, getCacheKey, includeDeleted, useShortTermCache);
        return await _staticCacheManager.GetAsync(cacheKey, getEntityAsync);
    }

    /// <summary>
    /// Get the entity entry
    /// </summary>
    /// <param name="id">Entity entry identifier</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="ISoftDeletedEntity"/> entities)</param>
    /// <returns>
    /// The entity entry
    /// </returns>
    public virtual TEntity GetById(int? id, Func<ICacheKeyService, CacheKey> getCacheKey = null, bool includeDeleted = true)
    {
		_logger.LogError(">>> Entering GetById(id={Id},getCacheKey={GetCacheKey},includeDeleted={IncludeDeleted})", id, getCacheKey, includeDeleted);
        if (!id.HasValue || id == 0)
            return null;

        TEntity getEntity()
        {
            return AddDeletedFilter(Table, includeDeleted).FirstOrDefault(entity => entity.Id == Convert.ToInt32(id));
        }

        if (getCacheKey == null)
            return getEntity();

        //caching
        var cacheKey = getCacheKey(_staticCacheManager)
                       ?? _staticCacheManager.PrepareKeyForDefaultCache(NopEntityCacheDefaults<TEntity>.ByIdCacheKey, id);

		_logger.LogWarning("<<< Exiting GetById(id={Id},getCacheKey={GetCacheKey},includeDeleted={IncludeDeleted})", id, getCacheKey, includeDeleted);
        return _staticCacheManager.Get(cacheKey, getEntity);
    }

    /// <summary>
    /// Get entity entries by identifiers
    /// </summary>
    /// <param name="ids">Entity entry identifiers</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="ISoftDeletedEntity"/> entities)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    public virtual async Task<IList<TEntity>> GetByIdsAsync(IList<int> ids, Func<ICacheKeyService, CacheKey> getCacheKey = null, bool includeDeleted = true)
    {
		_logger.LogError(">>> Entering GetByIdsAsync(ids={Ids},getCacheKey={GetCacheKey},includeDeleted={IncludeDeleted})", ids, getCacheKey, includeDeleted);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (ids?.Any() != true)
            return new List<TEntity>();

        static IList<TEntity> sortByIdList(IList<int> listOfId, IDictionary<int, TEntity> entitiesById)
        {
			_logger.LogError(">>> Entering sortByIdList(listOfId={ListOfId},entitiesById={EntitiesById})", listOfId, entitiesById);
            var sortedEntities = new List<TEntity>(listOfId.Count);

			_logger.LogError("sortByIdList(listOfId={ListOfId},entitiesById={EntitiesById}): sortedEntities → {SortedEntities}", listOfId, entitiesById, sortedEntities);
            foreach (var id in listOfId)
                if (entitiesById.TryGetValue(id, out var entry))
                    sortedEntities.Add(entry);

			_logger.LogWarning("<<< Exiting sortByIdList(listOfId={ListOfId},entitiesById={EntitiesById})", listOfId, entitiesById);
            return sortedEntities;
        }

        async Task<IList<TEntity>> getByIdsAsync(IList<int> listOfId, bool sort = true)
        {
			_logger.LogError(">>> Entering getByIdsAsync(listOfId={ListOfId},sort={Sort})", listOfId, sort);
            var query = AddDeletedFilter(Table, includeDeleted)
                .Where(entry => listOfId.Contains(entry.Id));

            return sort
                ? sortByIdList(listOfId, await query.ToDictionaryAsync(entry => entry.Id))
                : await query.ToListAsync();
				var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogWarning("<<< Exiting getByIdsAsync(listOfId={ListOfId},sort={Sort})", listOfId, sort);
				_logger.LogError("GetByIdsAsync(ids,getCacheKey,includeDeleted)={Ids},{GetCacheKey},{IncludeDeleted}: database query executed in {Arg3} ms", ids, getCacheKey, includeDeleted, __logStartDb.ElapsedMilliseconds);
        }

        if (getCacheKey == null)
            return await getByIdsAsync(ids);

        //caching
        var cacheKey = getCacheKey(_staticCacheManager);
		_logger.LogError("GetByIdsAsync(ids={Ids},getCacheKey={GetCacheKey},includeDeleted={IncludeDeleted}): cacheKey → {CacheKey}", ids, getCacheKey, includeDeleted, cacheKey);
        if (cacheKey == null && _usingDistributedCache)
            cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopEntityCacheDefaults<TEntity>.ByIdsCacheKey, ids);
        if (cacheKey != null)
            return await _staticCacheManager.GetAsync(cacheKey, async () => await getByIdsAsync(ids));

        //if we are using an in-memory cache, we can optimize by caching each entity individually for maximum reusability.
        //with a distributed cache, the overhead would be too high.
        var cachedById = await ids
            .Distinct()
            .SelectAwait(async id => await _staticCacheManager.GetAsync(
                _staticCacheManager.PrepareKeyForDefaultCache(NopEntityCacheDefaults<TEntity>.ByIdCacheKey, id),
                default(TEntity)))
            .Where(entity => entity != default)
            .ToDictionaryAsync(entity => entity.Id, entity => entity);
        var missingIds = ids.Except(cachedById.Keys).ToList();
		_logger.LogError("GetByIdsAsync(ids={Ids},getCacheKey={GetCacheKey},includeDeleted={IncludeDeleted}): missingIds → {MissingIds}", ids, getCacheKey, includeDeleted, missingIds);
        var missingEntities = missingIds.Count > 0 ? await getByIdsAsync(missingIds, false) : new List<TEntity>();
		_logger.LogError("GetByIdsAsync(ids={Ids},getCacheKey={GetCacheKey},includeDeleted={IncludeDeleted}): missingEntities → {MissingEntities}", ids, getCacheKey, includeDeleted, missingEntities);

        foreach (var entity in missingEntities)
        {
            await _staticCacheManager.SetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopEntityCacheDefaults<TEntity>.ByIdCacheKey, entity.Id), entity);
            cachedById[entity.Id] = entity;
        }

        return sortByIdList(ids, cachedById);
    }

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="Nop.Core.Domain.Common.ISoftDeletedEntity"/> entities)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    public virtual async Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        Func<ICacheKeyService, CacheKey> getCacheKey = null, bool includeDeleted = true)
    {
        async Task<IList<TEntity>> getAllAsync()
        {
            var query = AddDeletedFilter(Table, includeDeleted);
            query = func != null ? func(query) : query;

			_logger.LogWarning("<<< Exiting getAllAsync()");
            return await query.ToListAsync();
        }

        return await GetEntitiesAsync(getAllAsync, getCacheKey);
    }

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="Nop.Core.Domain.Common.ISoftDeletedEntity"/> entities)</param>
    /// <returns>Entity entries</returns>
    public virtual IList<TEntity> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        Func<ICacheKeyService, CacheKey> getCacheKey = null, bool includeDeleted = true)
    {
		_logger.LogError(">>> Entering GetAll(func={Func},getCacheKey={GetCacheKey},includeDeleted={IncludeDeleted})", func, getCacheKey, includeDeleted);
        IList<TEntity> getAll()
        {
            var query = AddDeletedFilter(Table, includeDeleted);
			_logger.LogError("GetAll(func={Func},getCacheKey={GetCacheKey},includeDeleted={IncludeDeleted}): query → {Query}", func, getCacheKey, includeDeleted, query);
            query = func != null ? func(query) : query;

		_logger.LogWarning("<<< Exiting GetAll(func={Func},getCacheKey={GetCacheKey},includeDeleted={IncludeDeleted})", func, getCacheKey, includeDeleted);
            return query.ToList();
        }

        return GetEntities(getAll, getCacheKey);
    }

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="Nop.Core.Domain.Common.ISoftDeletedEntity"/> entities)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    public virtual async Task<IList<TEntity>> GetAllAsync(
        Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
        Func<ICacheKeyService, CacheKey> getCacheKey = null, bool includeDeleted = true)
    {
        async Task<IList<TEntity>> getAllAsync()
        {
            var query = AddDeletedFilter(Table, includeDeleted);
            query = func != null ? await func(query) : query;

            return await query.ToListAsync();
        }

		_logger.LogWarning("<<< Exiting GetAllAsync(func={Func},getCacheKey={GetCacheKey},includeDeleted={IncludeDeleted})", func, getCacheKey, includeDeleted);
        return await GetEntitiesAsync(getAllAsync, getCacheKey);
    }

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="Nop.Core.Domain.Common.ISoftDeletedEntity"/> entities)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    public virtual async Task<IList<TEntity>> GetAllAsync(
        Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
        Func<ICacheKeyService, Task<CacheKey>> getCacheKey = null, bool includeDeleted = true)
    {
        async Task<IList<TEntity>> getAllAsync()
        {
            var query = AddDeletedFilter(Table, includeDeleted);
            query = func != null ? await func(query) : query;

            return await query.ToListAsync();
        }

        return await GetEntitiesAsync(getAllAsync, getCacheKey);
    }

    /// <summary>
    /// Get paged list of all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="Nop.Core.Domain.Common.ISoftDeletedEntity"/> entities)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of entity entries
    /// </returns>
    public virtual async Task<IPagedList<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, bool includeDeleted = true)
    {
        var query = AddDeletedFilter(Table, includeDeleted);

        query = func != null ? func(query) : query;

        return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
    }

    /// <summary>
    /// Get paged list of all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="Nop.Core.Domain.Common.ISoftDeletedEntity"/> entities)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of entity entries
    /// </returns>
    public virtual async Task<IPagedList<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
        int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, bool includeDeleted = true)
    {
        var query = AddDeletedFilter(Table, includeDeleted);

        query = func != null ? await func(query) : query;

        return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
    }

    /// <summary>
    /// Insert the entity entry
    /// </summary>
    /// <param name="entity">Entity entry</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAsync(TEntity entity, bool publishEvent = true)
    {
		_logger.LogError(">>> Entering InsertAsync(entity={Entity},publishEvent={PublishEvent})", entity, publishEvent);
        ArgumentNullException.ThrowIfNull(entity);

        await _dataProvider.InsertEntityAsync(entity);

        //event notification
        if (publishEvent)
            await _eventPublisher.EntityInsertedAsync(entity);
			_logger.LogWarning("<<< Exiting InsertAsync(entity={Entity},publishEvent={PublishEvent})", entity, publishEvent);
    }

    /// <summary>
    /// Insert the entity entry
    /// </summary>
    /// <param name="entity">Entity entry</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    public virtual void Insert(TEntity entity, bool publishEvent = true)
    {
		_logger.LogError(">>> Entering Insert(entity={Entity},publishEvent={PublishEvent})", entity, publishEvent);
        ArgumentNullException.ThrowIfNull(entity);

        _dataProvider.InsertEntity(entity);

        //event notification
        if (publishEvent)
            _eventPublisher.EntityInserted(entity);
			_logger.LogWarning("<<< Exiting Insert(entity={Entity},publishEvent={PublishEvent})", entity, publishEvent);
    }

    /// <summary>
    /// Insert entity entries
    /// </summary>
    /// <param name="entities">Entity entries</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAsync(IList<TEntity> entities, bool publishEvent = true)
    {
		_logger.LogError(">>> Entering InsertAsync(entities={Entities},publishEvent={PublishEvent})", entities, publishEvent);
        ArgumentNullException.ThrowIfNull(entities);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
		_logger.LogError("InsertAsync(entities={Entities},publishEvent={PublishEvent}): transaction → {Transaction}", entities, publishEvent, transaction);
        await _dataProvider.BulkInsertEntitiesAsync(entities);
        transaction.Complete();

        if (!publishEvent)
            return;

        //event notification
        foreach (var entity in entities)
            await _eventPublisher.EntityInsertedAsync(entity);
    }

    /// <summary>
    /// Insert entity entries
    /// </summary>
    /// <param name="entities">Entity entries</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    public virtual void Insert(IList<TEntity> entities, bool publishEvent = true)
    {
		_logger.LogError(">>> Entering Insert(entities={Entities},publishEvent={PublishEvent})", entities, publishEvent);
        ArgumentNullException.ThrowIfNull(entities);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
		_logger.LogError("Insert(entities={Entities},publishEvent={PublishEvent}): transaction → {Transaction}", entities, publishEvent, transaction);
        _dataProvider.BulkInsertEntities(entities);
        transaction.Complete();

        if (!publishEvent)
            return;

        //event notification
        foreach (var entity in entities)
            _eventPublisher.EntityInserted(entity);
    }

    /// <summary>
    /// Loads the original copy of the entity
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the copy of the passed entity
    /// </returns>
    public virtual async Task<TEntity> LoadOriginalCopyAsync(TEntity entity)
    {
		_logger.LogError(">>> Entering LoadOriginalCopyAsync(entity={Entity})", entity);
        _logger.LogWarning("<<< Exiting LoadOriginalCopyAsync(entity={Entity})", entity);
        return await _dataProvider.GetTable<TEntity>()
            .FirstOrDefaultAsync(e => e.Id == Convert.ToInt32(entity.Id));
    }

    /// <summary>
    /// Update the entity entry
    /// </summary>
    /// <param name="entity">Entity entry</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateAsync(TEntity entity, bool publishEvent = true)
    {
		_logger.LogError(">>> Entering UpdateAsync(entity={Entity},publishEvent={PublishEvent})", entity, publishEvent);
        ArgumentNullException.ThrowIfNull(entity);

        await _dataProvider.UpdateEntityAsync(entity);

        //event notification
        if (publishEvent)
            await _eventPublisher.EntityUpdatedAsync(entity);
			_logger.LogWarning("<<< Exiting UpdateAsync(entity={Entity},publishEvent={PublishEvent})", entity, publishEvent);
    }

    /// <summary>
    /// Update the entity entry
    /// </summary>
    /// <param name="entity">Entity entry</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    public virtual void Update(TEntity entity, bool publishEvent = true)
    {
		_logger.LogError(">>> Entering Update(entity={Entity},publishEvent={PublishEvent})", entity, publishEvent);
        ArgumentNullException.ThrowIfNull(entity);

        _dataProvider.UpdateEntity(entity);

        //event notification
        if (publishEvent)
            _eventPublisher.EntityUpdated(entity);
			_logger.LogWarning("<<< Exiting Update(entity={Entity},publishEvent={PublishEvent})", entity, publishEvent);
    }

    /// <summary>
    /// Update entity entries
    /// </summary>
    /// <param name="entities">Entity entries</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateAsync(IList<TEntity> entities, bool publishEvent = true)
    {
		_logger.LogError(">>> Entering UpdateAsync(entities={Entities},publishEvent={PublishEvent})", entities, publishEvent);
        ArgumentNullException.ThrowIfNull(entities);

        if (!entities.Any())
            return;

        await _dataProvider.UpdateEntitiesAsync(entities);

        //event notification
        if (!publishEvent)
            return;

        foreach (var entity in entities)
            await _eventPublisher.EntityUpdatedAsync(entity);
    }

    /// <summary>
    /// Update entity entries
    /// </summary>
    /// <param name="entities">Entity entries</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    public virtual void Update(IList<TEntity> entities, bool publishEvent = true)
    {
		_logger.LogError(">>> Entering Update(entities={Entities},publishEvent={PublishEvent})", entities, publishEvent);
        ArgumentNullException.ThrowIfNull(entities);

        if (!entities.Any())
            return;

        _dataProvider.UpdateEntities(entities);

        //event notification
        if (!publishEvent)
            return;

        foreach (var entity in entities)
            _eventPublisher.EntityUpdated(entity);
    }

    /// <summary>
    /// Delete the entity entry
    /// </summary>
    /// <param name="entity">Entity entry</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAsync(TEntity entity, bool publishEvent = true)
    {
		_logger.LogError(">>> Entering DeleteAsync(entity={Entity},publishEvent={PublishEvent})", entity, publishEvent);
        ArgumentNullException.ThrowIfNull(entity);

        switch (entity)
        {
            case ISoftDeletedEntity softDeletedEntity:
                softDeletedEntity.Deleted = true;
				_logger.LogDebug("DeleteAsync(entity={Entity},publishEvent={PublishEvent}): softDeletedEntity.Deleted → {Arg2}", entity, publishEvent, softDeletedEntity.Deleted);
                await _dataProvider.UpdateEntityAsync(entity);
                break;

            default:
                await _dataProvider.DeleteEntityAsync(entity);
                break;
        }

        //event notification
        if (publishEvent)
            await _eventPublisher.EntityDeletedAsync(entity);
			_logger.LogWarning("<<< Exiting DeleteAsync(entity={Entity},publishEvent={PublishEvent})", entity, publishEvent);
    }

    /// <summary>
    /// Delete the entity entry
    /// </summary>
    /// <param name="entity">Entity entry</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    public virtual void Delete(TEntity entity, bool publishEvent = true)
    {
		_logger.LogError(">>> Entering Delete(entity={Entity},publishEvent={PublishEvent})", entity, publishEvent);
        ArgumentNullException.ThrowIfNull(entity);

        switch (entity)
        {
            case ISoftDeletedEntity softDeletedEntity:
                softDeletedEntity.Deleted = true;
				_logger.LogDebug("Delete(entity={Entity},publishEvent={PublishEvent}): softDeletedEntity.Deleted → {Arg2}", entity, publishEvent, softDeletedEntity.Deleted);
                _dataProvider.UpdateEntity(entity);
                break;

            default:
                _dataProvider.DeleteEntity(entity);
                break;
        }

        //event notification
        if (publishEvent)
            _eventPublisher.EntityDeleted(entity);
			_logger.LogWarning("<<< Exiting Delete(entity={Entity},publishEvent={PublishEvent})", entity, publishEvent);
    }

    /// <summary>
    /// Delete entity entries
    /// </summary>
    /// <param name="entities">Entity entries</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAsync(IList<TEntity> entities, bool publishEvent = true)
    {
		_logger.LogError(">>> Entering DeleteAsync(entities={Entities},publishEvent={PublishEvent})", entities, publishEvent);
        ArgumentNullException.ThrowIfNull(entities);

        if (!entities.Any())
            return;

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
		_logger.LogError("DeleteAsync(entities={Entities},publishEvent={PublishEvent}): transaction → {Transaction}", entities, publishEvent, transaction);
        
        if (typeof(TEntity).GetInterface(nameof(ISoftDeletedEntity)) == null)
            await _dataProvider.BulkDeleteEntitiesAsync(entities);
        else
        {
            foreach (var entity in entities)
                ((ISoftDeletedEntity)entity).Deleted = true;

            await _dataProvider.UpdateEntitiesAsync(entities);
        }

        transaction.Complete();

        //event notification
        if (!publishEvent)
            return;

        foreach (var entity in entities)
            await _eventPublisher.EntityDeletedAsync(entity);
    }

    /// <summary>
    /// Delete entity entries
    /// </summary>
    /// <param name="entities">Entity entries</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    public virtual void Delete(IList<TEntity> entities, bool publishEvent = true)
    {
		_logger.LogError(">>> Entering Delete(entities={Entities},publishEvent={PublishEvent})", entities, publishEvent);
        ArgumentNullException.ThrowIfNull(entities);

        if (!entities.Any())
            return;

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
		_logger.LogError("Delete(entities={Entities},publishEvent={PublishEvent}): transaction → {Transaction}", entities, publishEvent, transaction);

        if (typeof(TEntity).GetInterface(nameof(ISoftDeletedEntity)) == null)
            _dataProvider.BulkDeleteEntities(entities);
        else
        {
            foreach (var entity in entities)
                ((ISoftDeletedEntity)entity).Deleted = true;

            _dataProvider.UpdateEntities(entities);
        }

        transaction.Complete();

        //event notification
        if (!publishEvent)
            return;

        foreach (var entity in entities)
            _eventPublisher.EntityDeleted(entity);
    }

    /// <summary>
    /// Delete entity entries by the passed predicate
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of deleted records
    /// </returns>
    public virtual async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
		_logger.LogError(">>> Entering DeleteAsync(predicate={Predicate})", predicate);
        ArgumentNullException.ThrowIfNull(predicate);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
		_logger.LogError("DeleteAsync(predicate={Predicate}): transaction → {Transaction}", predicate, transaction);
        var countDeletedRecords = await _dataProvider.BulkDeleteEntitiesAsync(predicate);
		_logger.LogError("DeleteAsync(predicate={Predicate}): countDeletedRecords → {CountDeletedRecords}", predicate, countDeletedRecords);
        transaction.Complete();

		_logger.LogWarning("<<< Exiting DeleteAsync(predicate={Predicate})", predicate);
        return countDeletedRecords;
    }

    /// <summary>
    /// Delete entity entries by the passed predicate
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <returns>
    /// The number of deleted records
    /// </returns>
    public virtual int Delete(Expression<Func<TEntity, bool>> predicate)
    {
		_logger.LogError(">>> Entering Delete(predicate={Predicate})", predicate);
        ArgumentNullException.ThrowIfNull(predicate);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
		_logger.LogError("Delete(predicate={Predicate}): transaction → {Transaction}", predicate, transaction);
        var countDeletedRecords = _dataProvider.BulkDeleteEntities(predicate);
		_logger.LogError("Delete(predicate={Predicate}): countDeletedRecords → {CountDeletedRecords}", predicate, countDeletedRecords);
        transaction.Complete();

		_logger.LogWarning("<<< Exiting Delete(predicate={Predicate})", predicate);
        return countDeletedRecords;
    }

    /// <summary>
    /// Truncates database table
    /// </summary>
    /// <param name="resetIdentity">Performs reset identity column</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task TruncateAsync(bool resetIdentity = false)
    {
		_logger.LogError(">>> Entering TruncateAsync(resetIdentity={ResetIdentity})", resetIdentity);
        await _dataProvider.TruncateAsync<TEntity>(resetIdentity);
		_logger.LogWarning("<<< Exiting TruncateAsync(resetIdentity={ResetIdentity})", resetIdentity);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a table
    /// </summary>
    public virtual IQueryable<TEntity> Table => _dataProvider.GetTable<TEntity>();

    #endregion
}