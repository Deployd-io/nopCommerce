using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Services.Customers;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Security;

/// <summary>
/// ACL service
/// </summary>
public partial class AclService : IAclService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly ICustomerService _customerService;
    protected readonly INopDataProvider _dataProvider;
    protected readonly IRepository<AclRecord> _aclRecordRepository;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly Lazy<IWorkContext> _workContext;

    #endregion

    #region Ctor

    public AclService(CatalogSettings catalogSettings,
        ICustomerService customerService,
        INopDataProvider dataProvider,
        IRepository<AclRecord> aclRecordRepository,
        IStaticCacheManager staticCacheManager,
        Lazy<IWorkContext> workContext)
    {
		_logger.LogError(">>> Entering AclService(catalogSettings={CatalogSettings},customerService={CustomerService},dataProvider={DataProvider},aclRecordRepository={AclRecordRepository},staticCacheManager={StaticCacheManager},workContext={WorkContext})", catalogSettings, customerService, dataProvider, aclRecordRepository, staticCacheManager, workContext);
        _catalogSettings = catalogSettings;
        _customerService = customerService;
		_logger.LogDebug("AclService(catalogSettings={CatalogSettings},customerService={CustomerService},dataProvider={DataProvider},aclRecordRepository={AclRecordRepository},staticCacheManager={StaticCacheManager},workContext={WorkContext}): _customerService → {_customerService}", catalogSettings, customerService, dataProvider, aclRecordRepository, staticCacheManager, workContext, _customerService);
        _dataProvider = dataProvider;
		_logger.LogError("AclService(catalogSettings={CatalogSettings},customerService={CustomerService},dataProvider={DataProvider},aclRecordRepository={AclRecordRepository},staticCacheManager={StaticCacheManager},workContext={WorkContext}): _dataProvider → {_dataProvider}", catalogSettings, customerService, dataProvider, aclRecordRepository, staticCacheManager, workContext, _dataProvider);
		_logger.LogDebug("AclService(catalogSettings={CatalogSettings},customerService={CustomerService},dataProvider={DataProvider},aclRecordRepository={AclRecordRepository},staticCacheManager={StaticCacheManager},workContext={WorkContext}): _dataProvider → {_dataProvider}", catalogSettings, customerService, dataProvider, aclRecordRepository, staticCacheManager, workContext, _dataProvider);
        _aclRecordRepository = aclRecordRepository;
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("AclService(catalogSettings={CatalogSettings},customerService={CustomerService},dataProvider={DataProvider},aclRecordRepository={AclRecordRepository},staticCacheManager={StaticCacheManager},workContext={WorkContext}): _aclRecordRepository → {_aclRecordRepository}", catalogSettings, customerService, dataProvider, aclRecordRepository, staticCacheManager, workContext, _aclRecordRepository);
		_logger.LogDebug("AclService(catalogSettings={CatalogSettings},customerService={CustomerService},dataProvider={DataProvider},aclRecordRepository={AclRecordRepository},staticCacheManager={StaticCacheManager},workContext={WorkContext}): _staticCacheManager → {_staticCacheManager}", catalogSettings, customerService, dataProvider, aclRecordRepository, staticCacheManager, workContext, _staticCacheManager);
    _logger.LogWarning("<<< Exiting AclService(catalogSettings={CatalogSettings},customerService={CustomerService},dataProvider={DataProvider},aclRecordRepository={AclRecordRepository},staticCacheManager={StaticCacheManager},workContext={WorkContext})", catalogSettings, customerService, dataProvider, aclRecordRepository, staticCacheManager, workContext);
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Inserts an ACL record
    /// </summary>
    /// <param name="aclRecord">ACL record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InsertAclRecordAsync(AclRecord aclRecord)
    {
		_logger.LogError(">>> Entering InsertAclRecordAsync(aclRecord={AclRecord})", aclRecord);
        await _aclRecordRepository.InsertAsync(aclRecord);
		_logger.LogWarning("<<< Exiting InsertAclRecordAsync(aclRecord={AclRecord})", aclRecord);
    }

    /// <summary>
    /// Get a value indicating whether any ACL records exist for entity type are related to customer roles
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if exist; otherwise false
    /// </returns>
    protected virtual async Task<bool> IsEntityAclMappingExistAsync<TEntity>() where TEntity : BaseEntity, IAclSupported
    {
		_logger.LogError(">>> Entering IsEntityAclMappingExistAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var entityName = typeof(TEntity).Name;
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopSecurityDefaults.EntityAclRecordExistsCacheKey, entityName);
		_logger.LogError("IsEntityAclMappingExistAsync(): entityName → {EntityName}", entityName);
		_logger.LogError("IsEntityAclMappingExistAsync(): key → {Key}", key);

        var query = from acl in _aclRecordRepository.Table
            where acl.EntityName == entityName
            select acl;
			_logger.LogWarning("<<< Exiting IsEntityAclMappingExistAsync()");
			_logger.LogError("IsEntityAclMappingExistAsync(): find query executing");
			_logger.LogError("IsEntityAclMappingExistAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);

        return await _staticCacheManager.GetAsync(key, async () => await query.AnyAsync());
    }

    #endregion

    #region Methods

    /// <summary>
    /// Apply ACL to the passed query
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="query">Query to filter</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filtered query
    /// </returns>
    public virtual async Task<IQueryable<TEntity>> ApplyAcl<TEntity>(IQueryable<TEntity> query, Customer customer)
        where TEntity : BaseEntity, IAclSupported
    {
		_logger.LogError(">>> Entering ApplyAcl(query={Query},customer={Customer})", query, customer);
        ArgumentNullException.ThrowIfNull(query);

        ArgumentNullException.ThrowIfNull(customer);

        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
		_logger.LogError("ApplyAcl(query={Query},customer={Customer}): customerRoleIds → {CustomerRoleIds}", query, customer, customerRoleIds);
		_logger.LogWarning("<<< Exiting ApplyAcl(query={Query},customer={Customer})", customer);
        return await ApplyAcl(query, customerRoleIds);
    }

    /// <summary>
    /// Apply ACL to the passed query
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="query">Query to filter</param>
    /// <param name="customerRoleIds">Identifiers of customer's roles</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filtered query
    /// </returns>
    public virtual async Task<IQueryable<TEntity>> ApplyAcl<TEntity>(IQueryable<TEntity> query, int[] customerRoleIds)
        where TEntity : BaseEntity, IAclSupported
    {
        ArgumentNullException.ThrowIfNull(query);

        ArgumentNullException.ThrowIfNull(customerRoleIds);

        if (!customerRoleIds.Any() || _catalogSettings.IgnoreAcl || !await IsEntityAclMappingExistAsync<TEntity>())
            return query;

        return from entity in query
            where !entity.SubjectToAcl || _aclRecordRepository.Table.Any(acl =>
                acl.EntityName == typeof(TEntity).Name && acl.EntityId == entity.Id && customerRoleIds.Contains(acl.CustomerRoleId))
            select entity;
    }

    /// <summary>
    /// Deletes an ACL record
    /// </summary>
    /// <param name="aclRecord">ACL record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAclRecordAsync(AclRecord aclRecord)
    {
		_logger.LogError(">>> Entering DeleteAclRecordAsync(aclRecord={AclRecord})", aclRecord);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _aclRecordRepository.DeleteAsync(aclRecord);
		_logger.LogError("DeleteAclRecordAsync(aclRecord)={AclRecord}: external service call took {Arg1} ms", aclRecord, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteAclRecordAsync(aclRecord={AclRecord})", aclRecord);
    }

    /// <summary>
    /// Gets ACL records
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="entity">Entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ACL records
    /// </returns>
    public virtual async Task<IList<AclRecord>> GetAclRecordsAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported
    {
		_logger.LogError(">>> Entering GetAclRecordsAsync(entity={Entity})", entity);
        ArgumentNullException.ThrowIfNull(entity);

        var entityId = entity.Id;
        var entityName = entity.GetType().Name;
		_logger.LogError("GetAclRecordsAsync(entity={Entity}): entityName → {EntityName}", entity, entityName);

        var query = from ur in _aclRecordRepository.Table
            where ur.EntityId == entityId &&
                  ur.EntityName == entityName
            select ur;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogError("GetAclRecordsAsync(entity)={Entity}: database query executed in {Arg1} ms", entity, __logStart.ElapsedMilliseconds);
        var aclRecords = await query.ToListAsync();
		_logger.LogWarning("<<< Exiting GetAclRecordsAsync(entity={Entity})", entity);
		_logger.LogError("GetAclRecordsAsync(entity={Entity}): aclRecords → {AclRecords}", entity, aclRecords);

        return aclRecords;
    }

    /// <summary>
    /// Inserts an ACL record
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="customerRoleId">Customer role id</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAclRecordAsync<TEntity>(TEntity entity, int customerRoleId) where TEntity : BaseEntity, IAclSupported
    {
		_logger.LogError(">>> Entering InsertAclRecordAsync(entity={Entity},customerRoleId={CustomerRoleId})", entity, customerRoleId);
        ArgumentNullException.ThrowIfNull(entity);

        if (customerRoleId == 0)
			_logger.LogError("Throwing ArgumentOutOfRangeException from InsertAclRecordAsync(entity={Entity},customerRoleId={CustomerRoleId})", entity, customerRoleId);
            throw new ArgumentOutOfRangeException(nameof(customerRoleId));

        var entityId = entity.Id;
        var entityName = entity.GetType().Name;
		_logger.LogError("InsertAclRecordAsync(entity={Entity},customerRoleId={CustomerRoleId}): entityName → {EntityName}", entity, customerRoleId, entityName);

        var aclRecord = new AclRecord
        {
            EntityId = entityId,
            EntityName = entityName,
            CustomerRoleId = customerRoleId
        };

        await InsertAclRecordAsync(aclRecord);
    }

    /// <summary>
    /// Find customer role identifiers with granted access
    /// </summary>
    /// <param name="entityId">Entity ID</param>
    /// <param name="entityName">Entity name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer role identifiers
    /// </returns>
    public virtual async Task<int[]> GetCustomerRoleIdsWithAccessAsync(int entityId, string entityName)
    {
		_logger.LogError(">>> Entering GetCustomerRoleIdsWithAccessAsync(entityId={EntityId},entityName={EntityName})", entityId, entityName);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (entityId == 0)
            return [];

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopSecurityDefaults.AclRecordCacheKey, entityId, entityName);
		_logger.LogError("GetCustomerRoleIdsWithAccessAsync(entityId={EntityId},entityName={EntityName}): key → {Key}", entityId, entityName, key);

        var query = from ur in _aclRecordRepository.Table
            where ur.EntityId == entityId &&
                  ur.EntityName == entityName
            select ur.CustomerRoleId;

		_logger.LogError("GetCustomerRoleIdsWithAccessAsync(entityId,entityName)={EntityId},{EntityName}: find query executing", entityId, entityName);
		_logger.LogError("GetCustomerRoleIdsWithAccessAsync(entityId,entityName)={EntityId},{EntityName}: external service call took {Arg2} ms", entityId, entityName, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetCustomerRoleIdsWithAccessAsync(entityId={EntityId},entityName={EntityName})", entityId, entityName);
        return await _staticCacheManager.GetAsync(key, () => query.ToArrayAsync());
    }

    /// <summary>
    /// Authorize ACL permission
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="entity">Entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IAclSupported
    {
		_logger.LogError(">>> Entering AuthorizeAsync(entity={Entity})", entity);
        _logger.LogWarning("<<< Exiting AuthorizeAsync(entity={Entity})", entity);
        return await AuthorizeAsync(entity, await _workContext.Value.GetCurrentCustomerAsync());
    }

    /// <summary>
    /// Authorize ACL permission
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that supports the ACL</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync<TEntity>(TEntity entity, Customer customer) where TEntity : BaseEntity, IAclSupported
    {
		_logger.LogError(">>> Entering AuthorizeAsync(entity={Entity},customer={Customer})", entity, customer);
        if (entity == null)
            return false;

        if (!entity.SubjectToAcl)
            return true;

		_logger.LogError(">>> Entering AuthorizeAsync(entityTypeName={EntityTypeName},entityId={EntityId},customer={Customer})", entityTypeName, entityId, customer);
		_logger.LogWarning("<<< Exiting AuthorizeAsync(entityTypeName={EntityTypeName},entityId={EntityId},customer={Customer})", entityTypeName, entityId, customer);
        return await AuthorizeAsync(entity.GetType().Name, entity.Id, customer);
    }

    /// <summary>
    /// Authorize ACL permission
    /// </summary>
    /// <param name="entityTypeName">Type name of entity that supports the ACL</param>
    /// <param name="entityId">Entity ID</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync(string entityTypeName, int entityId, Customer customer)
    {
        if (string.IsNullOrEmpty(entityTypeName))
            return false;

        if (entityId <= 0)
            return false;

        if (customer == null)
            return false;

        if (_catalogSettings.IgnoreAcl)
            return true;

        foreach (var role1 in await _customerService.GetCustomerRolesAsync(customer))
            foreach (var role2Id in await GetCustomerRoleIdsWithAccessAsync(entityId, entityTypeName))
                if (role1.Id == role2Id)
                    //yes, we have such permission
                    return true;

        //no permission found
        return false;
    }

    /// <summary>
    /// Authorize ACL permission
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="allowedCustomerRoleIds">List of allowed customer role IDs</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync(Customer customer, IList<int> allowedCustomerRoleIds)
    {
        return _catalogSettings.IgnoreAcl || allowedCustomerRoleIds.Intersect(await _customerService.GetCustomerRoleIdsAsync(customer)).Any();
    }

    /// <summary>
    /// Save ACL mapping
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="selectedCustomerRoleIds">Customer roles for mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SaveAclAsync<TEntity>(TEntity entity, IList<int> selectedCustomerRoleIds) where TEntity : BaseEntity, IAclSupported
    {
		_logger.LogError(">>> Entering SaveAclAsync(entity={Entity},selectedCustomerRoleIds={SelectedCustomerRoleIds})", entity, selectedCustomerRoleIds);
        if (entity == null)
            return;

        if (entity.SubjectToAcl != selectedCustomerRoleIds.Any())
        {
            entity.SubjectToAcl = selectedCustomerRoleIds.Any();
            await _dataProvider.UpdateEntityAsync(entity);
        }

        var existingAclRecords = await GetAclRecordsAsync(entity);
		_logger.LogError("SaveAclAsync(entity={Entity},selectedCustomerRoleIds={SelectedCustomerRoleIds}): existingAclRecords → {ExistingAclRecords}", entity, selectedCustomerRoleIds, existingAclRecords);
        var allCustomerRoles = await _customerService.GetAllCustomerRolesAsync(true);
		_logger.LogError("SaveAclAsync(entity={Entity},selectedCustomerRoleIds={SelectedCustomerRoleIds}): allCustomerRoles → {AllCustomerRoles}", entity, selectedCustomerRoleIds, allCustomerRoles);
        foreach (var customerRole in allCustomerRoles)
            if (selectedCustomerRoleIds.Contains(customerRole.Id))
            {
                //new role
                if (existingAclRecords.All(acl => acl.CustomerRoleId != customerRole.Id))
                    await InsertAclRecordAsync(entity, customerRole.Id);
            }
            else
            {
                //remove role
                var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
				_logger.LogError("SaveAclAsync(entity={Entity},selectedCustomerRoleIds={SelectedCustomerRoleIds}): aclRecordToDelete → {AclRecordToDelete}", entity, selectedCustomerRoleIds, aclRecordToDelete);
                if (aclRecordToDelete != null)
                    await DeleteAclRecordAsync(aclRecordToDelete);
            }
    }

    #endregion
}