using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Security;

/// <summary>
/// Permission service
/// </summary>
public partial class PermissionService : IPermissionService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IRepository<CustomerRole> _customerRoleRepository;
    protected readonly IRepository<PermissionRecord> _permissionRecordRepository;
    protected readonly IRepository<PermissionRecordCustomerRoleMapping> _permissionRecordCustomerRoleMappingRepository;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly ITypeFinder _typeFinder;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public PermissionService(ICustomerService customerService,
        ILocalizationService localizationService,
        IRepository<CustomerRole> customerRoleRepository,
        IRepository<PermissionRecord> permissionRecordRepository,
        IRepository<PermissionRecordCustomerRoleMapping> permissionRecordCustomerRoleMappingRepository,
        IStaticCacheManager staticCacheManager,
        ITypeFinder typeFinder,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering PermissionService(customerService={CustomerService},localizationService={LocalizationService},customerRoleRepository={CustomerRoleRepository},permissionRecordRepository={PermissionRecordRepository},permissionRecordCustomerRoleMappingRepository=<redacted>,staticCacheManager={StaticCacheManager},typeFinder={TypeFinder},workContext={WorkContext})", customerService, localizationService, customerRoleRepository, permissionRecordRepository, staticCacheManager, typeFinder, workContext);
        _customerService = customerService;
        _localizationService = localizationService;
		_logger.LogDebug("PermissionService(customerService={CustomerService},localizationService={LocalizationService},customerRoleRepository={CustomerRoleRepository},permissionRecordRepository={PermissionRecordRepository},permissionRecordCustomerRoleMappingRepository=<redacted>,staticCacheManager={StaticCacheManager},typeFinder={TypeFinder},workContext={WorkContext}): _localizationService → {_localizationService}", customerService, localizationService, customerRoleRepository, permissionRecordRepository, staticCacheManager, typeFinder, workContext, _localizationService);
		_logger.LogDebug("PermissionService(customerService={CustomerService},localizationService={LocalizationService},customerRoleRepository={CustomerRoleRepository},permissionRecordRepository={PermissionRecordRepository},permissionRecordCustomerRoleMappingRepository=<redacted>,staticCacheManager={StaticCacheManager},typeFinder={TypeFinder},workContext={WorkContext}): _customerService → {_customerService}", customerService, localizationService, customerRoleRepository, permissionRecordRepository, staticCacheManager, typeFinder, workContext, _customerService);
    _logger.LogWarning("<<< Exiting PermissionService(customerService={CustomerService},localizationService={LocalizationService},customerRoleRepository={CustomerRoleRepository},permissionRecordRepository={PermissionRecordRepository},permissionRecordCustomerRoleMappingRepository=<redacted>,staticCacheManager={StaticCacheManager},typeFinder={TypeFinder},workContext={WorkContext})", customerService, localizationService, customerRoleRepository, permissionRecordRepository, staticCacheManager, typeFinder, workContext);
        _customerRoleRepository = customerRoleRepository;
		_logger.LogDebug("PermissionService(customerService={CustomerService},localizationService={LocalizationService},customerRoleRepository={CustomerRoleRepository},permissionRecordRepository={PermissionRecordRepository},permissionRecordCustomerRoleMappingRepository=<redacted>,staticCacheManager={StaticCacheManager},typeFinder={TypeFinder},workContext={WorkContext}): _customerRoleRepository → {_customerRoleRepository}", customerService, localizationService, customerRoleRepository, permissionRecordRepository, staticCacheManager, typeFinder, workContext, _customerRoleRepository);
        _permissionRecordRepository = permissionRecordRepository;
		_logger.LogDebug("PermissionService(customerService={CustomerService},localizationService={LocalizationService},customerRoleRepository={CustomerRoleRepository},permissionRecordRepository={PermissionRecordRepository},permissionRecordCustomerRoleMappingRepository=<redacted>,staticCacheManager={StaticCacheManager},typeFinder={TypeFinder},workContext={WorkContext}): _permissionRecordRepository → {_permissionRecordRepository}", customerService, localizationService, customerRoleRepository, permissionRecordRepository, staticCacheManager, typeFinder, workContext, _permissionRecordRepository);
        _permissionRecordCustomerRoleMappingRepository = permissionRecordCustomerRoleMappingRepository;
		_logger.LogDebug("PermissionService(customerService={CustomerService},localizationService={LocalizationService},customerRoleRepository={CustomerRoleRepository},permissionRecordRepository={PermissionRecordRepository},permissionRecordCustomerRoleMappingRepository=<redacted>,staticCacheManager={StaticCacheManager},typeFinder={TypeFinder},workContext={WorkContext}): _permissionRecordCustomerRoleMappingRepository → {_permissionRecordCustomerRoleMappingRepository}", customerService, localizationService, customerRoleRepository, permissionRecordRepository, staticCacheManager, typeFinder, workContext, _permissionRecordCustomerRoleMappingRepository);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("PermissionService(customerService={CustomerService},localizationService={LocalizationService},customerRoleRepository={CustomerRoleRepository},permissionRecordRepository={PermissionRecordRepository},permissionRecordCustomerRoleMappingRepository=<redacted>,staticCacheManager={StaticCacheManager},typeFinder={TypeFinder},workContext={WorkContext}): _staticCacheManager → {_staticCacheManager}", customerService, localizationService, customerRoleRepository, permissionRecordRepository, staticCacheManager, typeFinder, workContext, _staticCacheManager);
        _typeFinder = typeFinder;
		_logger.LogDebug("PermissionService(customerService={CustomerService},localizationService={LocalizationService},customerRoleRepository={CustomerRoleRepository},permissionRecordRepository={PermissionRecordRepository},permissionRecordCustomerRoleMappingRepository=<redacted>,staticCacheManager={StaticCacheManager},typeFinder={TypeFinder},workContext={WorkContext}): _typeFinder → {_typeFinder}", customerService, localizationService, customerRoleRepository, permissionRecordRepository, staticCacheManager, typeFinder, workContext, _typeFinder);
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get permission records by customer role identifier
    /// </summary>
    /// <param name="customerRoleId">Customer role identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the permissions
    /// </returns>
    protected virtual async Task<IList<PermissionRecord>> GetPermissionRecordsByCustomerRoleIdAsync(int customerRoleId)
    {
		_logger.LogError(">>> Entering GetPermissionRecordsByCustomerRoleIdAsync(customerRoleId={CustomerRoleId})", customerRoleId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopSecurityDefaults.PermissionRecordsAllCacheKey, customerRoleId);

		_logger.LogError("GetPermissionRecordsByCustomerRoleIdAsync(customerRoleId={CustomerRoleId}): key → {Key}", customerRoleId, key);
        var query = from pr in _permissionRecordRepository.Table
                    join prcrm in _permissionRecordCustomerRoleMappingRepository.Table on pr.Id equals prcrm
                        .PermissionRecordId
                    where prcrm.CustomerRoleId == customerRoleId
                    orderby pr.Id
                    select pr;

		_logger.LogError("GetPermissionRecordsByCustomerRoleIdAsync(customerRoleId)={CustomerRoleId}: find query executing", customerRoleId);
		_logger.LogError("GetPermissionRecordsByCustomerRoleIdAsync(customerRoleId)={CustomerRoleId}: external service call took {Arg1} ms", customerRoleId, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetPermissionRecordsByCustomerRoleIdAsync(customerRoleId={CustomerRoleId})", customerRoleId);
        return await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
    }

    /// <summary>
    /// Gets a permission
    /// </summary>
    /// <param name="systemName">Permission system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the permission
    /// </returns>
    protected virtual async Task<PermissionRecord> GetPermissionRecordBySystemNameAsync(string systemName)
    {
		_logger.LogError(">>> Entering GetPermissionRecordBySystemNameAsync(systemName={SystemName})", systemName);
        if (string.IsNullOrWhiteSpace(systemName))
            return null;

        var query = from pr in _permissionRecordRepository.Table
                    where pr.SystemName == systemName
                    orderby pr.Id
                    select pr;

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("GetPermissionRecordBySystemNameAsync(systemName)={SystemName}: database query executed in {Arg1} ms", systemName, __logStart.ElapsedMilliseconds);
        var permissionRecord = await query.FirstOrDefaultAsync();
		_logger.LogError("GetPermissionRecordBySystemNameAsync(systemName={SystemName}): permissionRecord → {PermissionRecord}", systemName, permissionRecord);
		_logger.LogWarning("<<< Exiting GetPermissionRecordBySystemNameAsync(systemName={SystemName})", systemName);
        return permissionRecord;
    }

    /// <summary>
    /// Insert permissions by list of permission configs
    /// </summary>
    /// <param name="configs">Permission configs</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallPermissionsAsync(IList<PermissionConfig> configs)
    {
		_logger.LogError(">>> Entering InstallPermissionsAsync(configs={Configs})", configs);
        if (!configs?.Any() ?? true)
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var exists =
            await _permissionRecordCustomerRoleMappingRepository.GetAllAsync(query => query, getCacheKey: _ => default);
			_logger.LogError("InstallPermissionsAsync(configs)={Configs}: find query executed in {Arg1} ms", configs, __logStart.ElapsedMilliseconds);

        async Task addPermissionRecordCustomerRoleMappingIfNotExists(
            PermissionRecordCustomerRoleMapping permissionRecordCustomerRoleMapping)
        {
			_logger.LogError(">>> Entering addPermissionRecordCustomerRoleMappingIfNotExists(permissionRecordCustomerRoleMapping=<redacted>)");
            var mapping = exists.FirstOrDefault(m =>
                m.CustomerRoleId == permissionRecordCustomerRoleMapping.CustomerRoleId &&
                m.PermissionRecordId == permissionRecordCustomerRoleMapping.PermissionRecordId);

            if (mapping != null)
            {
                permissionRecordCustomerRoleMapping.Id = mapping.Id;
				_logger.LogDebug("InstallPermissionsAsync(configs={Configs}): permissionRecordCustomerRoleMapping.Id → {Arg1}", configs, permissionRecordCustomerRoleMapping.Id);

				_logger.LogDebug("addPermissionRecordCustomerRoleMappingIfNotExists(permissionRecordCustomerRoleMapping=<redacted>): permissionRecordCustomerRoleMapping.Id → {Arg1}", permissionRecordCustomerRoleMapping.Id);
                return;
            }

            await _permissionRecordCustomerRoleMappingRepository.InsertAsync(permissionRecordCustomerRoleMapping, false);
			_logger.LogError("InstallPermissionsAsync(configs)={Configs}: save query executed in {Arg1} ms", configs, __logStart.ElapsedMilliseconds);
            exists.Add(permissionRecordCustomerRoleMapping);
			        _logger.LogWarning("<<< Exiting addPermissionRecordCustomerRoleMappingIfNotExists(permissionRecordCustomerRoleMapping=<redacted>)");
        }

        foreach (var config in configs)
        {
            //new permission (install it)
            var permission = new PermissionRecord
            {
                Name = config.Name,
                SystemName = config.SystemName,
                Category = config.Category
            };

            //save new permission
            await _permissionRecordRepository.InsertAsync(permission);

            foreach (var systemRoleName in config.DefaultCustomerRoles)
            {
                var customerRole = await _customerService.GetCustomerRoleBySystemNameAsync(systemRoleName);
				_logger.LogError("InstallPermissionsAsync(configs={Configs}): customerRole → {CustomerRole}", configs, customerRole);

                if (customerRole == null)
                {
                    //new role (save it)
                    customerRole = new CustomerRole
                    {
                        Name = systemRoleName,
                        Active = true,
                        SystemName = systemRoleName
                    };

                    await _customerRoleRepository.InsertAsync(customerRole);
                }

                await addPermissionRecordCustomerRoleMappingIfNotExists(new PermissionRecordCustomerRoleMapping { CustomerRoleId = customerRole.Id, PermissionRecordId = permission.Id });
            }

            //save localization
            await _localizationService.SaveLocalizedPermissionNameAsync(permission);
        }
		_logger.LogWarning("<<< Exiting InstallPermissionsAsync(configs={Configs})", configs);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets all permissions
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the permissions
    /// </returns>
    public virtual async Task<IList<PermissionRecord>> GetAllPermissionRecordsAsync()
    {
		_logger.LogError(">>> Entering GetAllPermissionRecordsAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var permissions = await _permissionRecordRepository.GetAllAsync(query => from pr in query
                                                                                 orderby pr.Name
                                                                                 select pr);
																				_logger.LogError("GetAllPermissionRecordsAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting GetAllPermissionRecordsAsync()");
        return permissions;
    }

    /// <summary>
    /// Inserts a permission
    /// </summary>
    /// <param name="permission">Permission</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPermissionRecordAsync(PermissionRecord permission)
    {
		_logger.LogError(">>> Entering InsertPermissionRecordAsync(permission={Permission})", permission);
        await _permissionRecordRepository.InsertAsync(permission);
		_logger.LogWarning("<<< Exiting InsertPermissionRecordAsync(permission={Permission})", permission);
    }

    /// <summary>
    /// Gets a permission record by identifier
    /// </summary>
    /// <param name="permissionId">Permission identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a permission record
    /// </returns>
    public virtual async Task<PermissionRecord> GetPermissionRecordByIdAsync(int permissionId)
    {
		_logger.LogError(">>> Entering GetPermissionRecordByIdAsync(permissionId={PermissionId})", permissionId);
        _logger.LogWarning("<<< Exiting GetPermissionRecordByIdAsync(permissionId={PermissionId})", permissionId);
        return await _permissionRecordRepository.GetByIdAsync(permissionId);
    }

    /// <summary>
    /// Updates the permission
    /// </summary>
    /// <param name="permission">Permission</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdatePermissionRecordAsync(PermissionRecord permission)
    {
		_logger.LogError(">>> Entering UpdatePermissionRecordAsync(permission={Permission})", permission);
        await _permissionRecordRepository.UpdateAsync(permission);
		_logger.LogWarning("<<< Exiting UpdatePermissionRecordAsync(permission={Permission})", permission);
    }

    /// <summary>
    /// Delete a permission
    /// </summary>
    /// <param name="permission">Permission</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePermissionRecordAsync(PermissionRecord permission)
    {
		_logger.LogError(">>> Entering DeletePermissionRecordAsync(permission={Permission})", permission);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _permissionRecordRepository.DeleteAsync(permission);
		_logger.LogError("DeletePermissionRecordAsync(permission)={Permission}: external service call took {Arg1} ms", permission, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeletePermissionRecordAsync(permission={Permission})", permission);
    }

    /// <summary>
    /// Delete a permission
    /// </summary>
    /// <param name="permissionSystemName">Permission system name</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePermissionAsync(string permissionSystemName)
    {
		_logger.LogError(">>> Entering DeletePermissionAsync(permissionSystemName={PermissionSystemName})", permissionSystemName);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var permission = await GetPermissionRecordBySystemNameAsync(permissionSystemName);

		_logger.LogError("DeletePermissionAsync(permissionSystemName={PermissionSystemName}): permission → {Permission}", permissionSystemName, permission);
        if (permission == null)
            return;

        var mapping = await GetMappingByPermissionRecordIdAsync(permission.Id);
		_logger.LogError("DeletePermissionAsync(permissionSystemName={PermissionSystemName}): mapping → {Mapping}", permissionSystemName, mapping);
		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("DeletePermissionAsync(permissionSystemName)={PermissionSystemName}: delete query executed in {Arg1} ms", permissionSystemName, __logStartDb.ElapsedMilliseconds);

        await _permissionRecordCustomerRoleMappingRepository.DeleteAsync(mapping);
        await _localizationService.DeleteLocalizedPermissionNameAsync(permission);
        await _permissionRecordRepository.DeleteAsync(permission);
		_logger.LogError("DeletePermissionAsync(permissionSystemName)={PermissionSystemName}: external service call took {Arg1} ms", permissionSystemName, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting DeletePermissionAsync(permissionSystemName={PermissionSystemName})", permissionSystemName);
    }

    /// <summary>
    /// Authorize permission
    /// </summary>
    /// <param name="permission">Permission record</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync(PermissionRecord permission)
    {
		_logger.LogError(">>> Entering AuthorizeAsync(permission={Permission})", permission);
        _logger.LogWarning("<<< Exiting AuthorizeAsync(permission={Permission})", permission);
        return await AuthorizeAsync(permission, await _workContext.GetCurrentCustomerAsync());
    }

    /// <summary>
    /// Authorize permission
    /// </summary>
    /// <param name="permission">Permission record</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync(PermissionRecord permission, Customer customer)
    {
        if (permission == null)
            return false;

        if (customer == null)
            return false;

        return await AuthorizeAsync(permission.SystemName, customer);
    }

    /// <summary>
    /// Authorize permission
    /// </summary>
    /// <param name="permissionRecordSystemName">Permission record system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName)
    {
        return await AuthorizeAsync(permissionRecordSystemName, await _workContext.GetCurrentCustomerAsync());
    }

    /// <summary>
    /// Authorize permission
    /// </summary>
    /// <param name="permissionRecordSystemName">Permission record system name</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName, Customer customer)
    {
        if (string.IsNullOrEmpty(permissionRecordSystemName))
            return false;

        var customerRoles = await _customerService.GetCustomerRolesAsync(customer);
        foreach (var role in customerRoles)
            if (await AuthorizeAsync(permissionRecordSystemName, role.Id))
                //yes, we have such permission
                return true;

        //no permission found
        return false;
    }

    /// <summary>
    /// Authorize permission
    /// </summary>
    /// <param name="permissionRecordSystemName">Permission record system name</param>
    /// <param name="customerRoleId">Customer role identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the true - authorized; otherwise, false
    /// </returns>
    public virtual async Task<bool> AuthorizeAsync(string permissionRecordSystemName, int customerRoleId)
    {
        if (string.IsNullOrEmpty(permissionRecordSystemName))
            return false;

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopSecurityDefaults.PermissionAllowedCacheKey, permissionRecordSystemName, customerRoleId);

        return await _staticCacheManager.GetAsync(key, async () =>
        {
            var permissions = await GetPermissionRecordsByCustomerRoleIdAsync(customerRoleId);
            foreach (var permission in permissions)
                if (permission.SystemName.Equals(permissionRecordSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        });
    }

    /// <summary>
    /// Gets a permission record-customer role mapping
    /// </summary>
    /// <param name="permissionId">Permission identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a list of mappings
    /// </returns>
    public virtual async Task<IList<PermissionRecordCustomerRoleMapping>> GetMappingByPermissionRecordIdAsync(int permissionId)
    {
		_logger.LogError(">>> Entering GetMappingByPermissionRecordIdAsync(permissionId={PermissionId})", permissionId);
        var query = _permissionRecordCustomerRoleMappingRepository.Table;

        query = query.Where(x => x.PermissionRecordId == permissionId);
		_logger.LogError("GetMappingByPermissionRecordIdAsync(permissionId={PermissionId}): query → {Query}", permissionId, query);

		_logger.LogWarning("<<< Exiting GetMappingByPermissionRecordIdAsync(permissionId={PermissionId})", permissionId);
        return await query.ToListAsync();
    }

    /// <summary>
    /// Delete a permission record-customer role mapping
    /// </summary>
    /// <param name="permissionId">Permission identifier</param>
    /// <param name="customerRoleId">Customer role identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePermissionRecordCustomerRoleMappingAsync(int permissionId, int customerRoleId)
    {
		_logger.LogError(">>> Entering DeletePermissionRecordCustomerRoleMappingAsync(permissionId={PermissionId},customerRoleId={CustomerRoleId})", permissionId, customerRoleId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var mapping = await _permissionRecordCustomerRoleMappingRepository.Table
            .FirstOrDefaultAsync(prcm => prcm.CustomerRoleId == customerRoleId && prcm.PermissionRecordId == permissionId);
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogError("DeletePermissionRecordCustomerRoleMappingAsync(permissionId,customerRoleId)={PermissionId},{CustomerRoleId}: database query executed in {Arg2} ms", permissionId, customerRoleId, __logStartDb.ElapsedMilliseconds);
			_logger.LogError("DeletePermissionRecordCustomerRoleMappingAsync(permissionId,customerRoleId)={PermissionId},{CustomerRoleId}: external service call took {Arg2} ms", permissionId, customerRoleId, __logStart.ElapsedMilliseconds);
			_logger.LogError("DeletePermissionRecordCustomerRoleMappingAsync(permissionId,customerRoleId)={PermissionId},{CustomerRoleId}: delete query executed in {Arg2} ms", permissionId, customerRoleId, __logStartDb.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting DeletePermissionRecordCustomerRoleMappingAsync(permissionId={PermissionId},customerRoleId={CustomerRoleId})", permissionId, customerRoleId);
        if (mapping is null) {
            return;
        }

        await _permissionRecordCustomerRoleMappingRepository.DeleteAsync(mapping);
    }

    /// <summary>
    /// Inserts a permission record-customer role mapping
    /// </summary>
    /// <param name="permissionRecordCustomerRoleMapping">Permission record-customer role mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPermissionRecordCustomerRoleMappingAsync(PermissionRecordCustomerRoleMapping permissionRecordCustomerRoleMapping)
    {
		_logger.LogError(">>> Entering InsertPermissionRecordCustomerRoleMappingAsync(permissionRecordCustomerRoleMapping=<redacted>)");
        await _permissionRecordCustomerRoleMappingRepository.InsertAsync(permissionRecordCustomerRoleMapping);
		_logger.LogWarning("<<< Exiting InsertPermissionRecordCustomerRoleMappingAsync(permissionRecordCustomerRoleMapping=<redacted>)");
    }

    /// <summary>
    /// Configure permission manager
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPermissionsAsync()
    {
		_logger.LogError(">>> Entering InsertPermissionsAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var permissionRecords = (await _permissionRecordRepository.GetAllAsync(query => query, getCacheKey: _ => null)).Distinct().ToHashSet();
        var exists = permissionRecords.Select(p => p.SystemName).ToHashSet();

		_logger.LogError("InsertPermissionsAsync(): permissionRecords → {PermissionRecords}", permissionRecords);
        var configs = _typeFinder.FindClassesOfType<IPermissionConfigManager>()
            .Select(configType => (IPermissionConfigManager)Activator.CreateInstance(configType))
            .SelectMany(config => config?.AllConfigs ?? new List<PermissionConfig>())
            .Where(c => !exists.Contains(c.SystemName))
            .ToList();
			_logger.LogError("InsertPermissionsAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
			_logger.LogError("InsertPermissionsAsync(): exists → {Exists}", exists);

        await InstallPermissionsAsync(configs);
		_logger.LogWarning("<<< Exiting InsertPermissionsAsync()");
    }

    /// <summary>
    /// Inserts a permission record-customer role mappings
    /// </summary>
    /// <param name="customerRoleId">Customer role ID</param>
    /// <param name="permissions">Permissions</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPermissionMappingAsync(int customerRoleId, params string[] permissions)
    {
		_logger.LogError(">>> Entering InsertPermissionMappingAsync(customerRoleId={CustomerRoleId},permissions={Permissions})", customerRoleId, permissions);
        var permissionRecords = await GetAllPermissionRecordsAsync();

		_logger.LogError("InsertPermissionMappingAsync(customerRoleId={CustomerRoleId},permissions={Permissions}): permissionRecords → {PermissionRecords}", customerRoleId, permissions, permissionRecords);
        foreach (var permissionSystemName in permissions)
        {
            var permission = permissionRecords.FirstOrDefault(p =>
                p.SystemName.Equals(permissionSystemName, StringComparison.CurrentCultureIgnoreCase));

            if (permission == null)
                continue;

            await InsertPermissionRecordCustomerRoleMappingAsync(
                new PermissionRecordCustomerRoleMapping
                {
                    CustomerRoleId = customerRoleId,
                    PermissionRecordId = permission.Id
                });
				_logger.LogDebug("InsertPermissionMappingAsync(customerRoleId={CustomerRoleId},permissions={Permissions}): PermissionRecordId → {PermissionRecordId}", customerRoleId, permissions, PermissionRecordId);
        }
		_logger.LogWarning("<<< Exiting InsertPermissionMappingAsync(customerRoleId={CustomerRoleId},permissions={Permissions})", customerRoleId, permissions);
    }

    #endregion
}
