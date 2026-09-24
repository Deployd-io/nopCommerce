using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Catalog;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Shipping;

/// <summary>
/// Shipping methods service
/// </summary>
public partial class ShippingMethodsService : IShippingMethodsService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<ShippingMethod> _shippingMethodRepository;
    protected readonly IRepository<ShippingMethodCountryMapping> _shippingMethodCountryMappingRepository;

    #endregion

    #region Ctor

    public ShippingMethodsService(IRepository<ShippingMethod> shippingMethodRepository,
        IRepository<ShippingMethodCountryMapping> shippingMethodCountryMappingRepository)
    {
		_logger.LogError(">>> Entering ShippingMethodsService(shippingMethodRepository=<redacted>,shippingMethodCountryMappingRepository=<redacted>)");
        _shippingMethodRepository = shippingMethodRepository;
        _shippingMethodCountryMappingRepository = shippingMethodCountryMappingRepository;
		_logger.LogDebug("ShippingMethodsService(shippingMethodRepository=<redacted>,shippingMethodCountryMappingRepository=<redacted>): _shippingMethodCountryMappingRepository → {_shippingMethodCountryMappingRepository}", _shippingMethodCountryMappingRepository);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a shipping method
    /// </summary>
    /// <param name="shippingMethod">The shipping method</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteShippingMethodAsync(ShippingMethod shippingMethod)
    {
		_logger.LogError(">>> Entering DeleteShippingMethodAsync(shippingMethod=<redacted>)");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _shippingMethodRepository.DeleteAsync(shippingMethod);
		_logger.LogError("DeleteShippingMethodAsync(shippingMethod)=<redacted>: external service call took {Arg1} ms", __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteShippingMethodAsync(shippingMethod=<redacted>)");
    }

    /// <summary>
    /// Gets a shipping method
    /// </summary>
    /// <param name="shippingMethodId">The shipping method identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping method
    /// </returns>
    public virtual async Task<ShippingMethod> GetShippingMethodByIdAsync(int shippingMethodId)
    {
		_logger.LogError(">>> Entering GetShippingMethodByIdAsync(shippingMethodId=<redacted>)");
		_logger.LogError("GetShippingMethodByIdAsync(shippingMethodId)=<redacted>: find query executing");
    _logger.LogWarning("<<< Exiting GetShippingMethodByIdAsync(shippingMethodId=<redacted>)");
        return await _shippingMethodRepository.GetByIdAsync(shippingMethodId, _ => default);
    }

    /// <summary>
    /// Gets all shipping methods
    /// </summary>
    /// <param name="filterByCountryId">The country identifier to filter by</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping methods
    /// </returns>
    public virtual async Task<IList<ShippingMethod>> GetAllShippingMethodsAsync(int? filterByCountryId = null)
    {
		_logger.LogError(">>> Entering GetAllShippingMethodsAsync(filterByCountryId={FilterByCountryId})", filterByCountryId);
        if (filterByCountryId is > 0)
        {
			_logger.LogError("GetAllShippingMethodsAsync(filterByCountryId)={FilterByCountryId}: find query executing", filterByCountryId);
            return await _shippingMethodRepository.GetAllAsync(query =>
            {
                var query1 =
                    from shippingMethod in query
                    join shippingMethodCountryMapping in _shippingMethodCountryMappingRepository.Table on shippingMethod.Id equals shippingMethodCountryMapping.ShippingMethodId
                    where shippingMethodCountryMapping.CountryId == filterByCountryId.Value
                    select shippingMethod.Id;

                query1 = query1.Distinct();
				_logger.LogError("GetAllShippingMethodsAsync(filterByCountryId={FilterByCountryId}): query1 → {Query1}", filterByCountryId, query1);

                var query2 =
                    from sm in query
                    where !query1.Contains(sm.Id)
                    orderby sm.DisplayOrder, sm.Id
                    select sm;

                return query2;
            }, cache => cache.PrepareKeyForDefaultCache(NopShippingDefaults.ShippingMethodsAllCacheKey, filterByCountryId));
        }

		_logger.LogWarning("<<< Exiting GetAllShippingMethodsAsync(filterByCountryId={FilterByCountryId})", filterByCountryId);
        return await _shippingMethodRepository.GetAllAsync(query =>
            from sm in query
            orderby sm.DisplayOrder, sm.Id
            select sm, _ => default);
    }

    /// <summary>
    /// Inserts a shipping method
    /// </summary>
    /// <param name="shippingMethod">Shipping method</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertShippingMethodAsync(ShippingMethod shippingMethod)
    {
		_logger.LogError(">>> Entering InsertShippingMethodAsync(shippingMethod=<redacted>)");
        await _shippingMethodRepository.InsertAsync(shippingMethod);
		_logger.LogWarning("<<< Exiting InsertShippingMethodAsync(shippingMethod=<redacted>)");
    }

    /// <summary>
    /// Updates the shipping method
    /// </summary>
    /// <param name="shippingMethod">Shipping method</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateShippingMethodAsync(ShippingMethod shippingMethod)
    {
		_logger.LogError(">>> Entering UpdateShippingMethodAsync(shippingMethod=<redacted>)");
        await _shippingMethodRepository.UpdateAsync(shippingMethod);
		_logger.LogWarning("<<< Exiting UpdateShippingMethodAsync(shippingMethod=<redacted>)");
    }

    /// <summary>
    /// Does country restriction exist
    /// </summary>
    /// <param name="shippingMethod">Shipping method</param>
    /// <param name="countryId">Country identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> CountryRestrictionExistsAsync(ShippingMethod shippingMethod, int countryId)
    {
		_logger.LogError(">>> Entering CountryRestrictionExistsAsync(shippingMethod=<redacted>,countryId={CountryId})", countryId);
        ArgumentNullException.ThrowIfNull(shippingMethod);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var result = await _shippingMethodCountryMappingRepository.Table
            .AnyAsync(shippingMethodCountryMapping => shippingMethodCountryMapping.ShippingMethodId == shippingMethod.Id && shippingMethodCountryMapping.CountryId == countryId);
			_logger.LogError("CountryRestrictionExistsAsync(shippingMethod,countryId)=<redacted>,{CountryId}: count query executed in {Arg2} ms", countryId, __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting CountryRestrictionExistsAsync(shippingMethod=<redacted>,countryId={CountryId})", countryId);
        return result;
    }

    /// <summary>
    /// Gets shipping country mappings
    /// </summary>
    /// <param name="shippingMethodId">The shipping method identifier</param>
    /// <param name="countryId">Country identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping country mappings
    /// </returns>
    public virtual async Task<IList<ShippingMethodCountryMapping>> GetShippingMethodCountryMappingAsync(int shippingMethodId,
        int countryId)
    {
		_logger.LogError(">>> Entering GetShippingMethodCountryMappingAsync(shippingMethodId=<redacted>,countryId={CountryId})", countryId);
        var query = _shippingMethodCountryMappingRepository.Table.Where(shippingMethodCountryMapping =>
            shippingMethodCountryMapping.ShippingMethodId == shippingMethodId && shippingMethodCountryMapping.CountryId == countryId);

		_logger.LogWarning("<<< Exiting GetShippingMethodCountryMappingAsync(shippingMethodId=<redacted>,countryId={CountryId})", countryId);
        return await query.ToListAsync();
    }

    /// <summary>
    /// Inserts a shipping country mapping
    /// </summary>
    /// <param name="shippingMethodCountryMapping">Shipping country mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertShippingMethodCountryMappingAsync(ShippingMethodCountryMapping shippingMethodCountryMapping)
    {
		_logger.LogError(">>> Entering InsertShippingMethodCountryMappingAsync(shippingMethodCountryMapping=<redacted>)");
        await _shippingMethodCountryMappingRepository.InsertAsync(shippingMethodCountryMapping);
		_logger.LogWarning("<<< Exiting InsertShippingMethodCountryMappingAsync(shippingMethodCountryMapping=<redacted>)");
    }

    /// <summary>
    /// Delete the shipping country mapping
    /// </summary>
    /// <param name="shippingMethodCountryMapping">Shipping country mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteShippingMethodCountryMappingAsync(ShippingMethodCountryMapping shippingMethodCountryMapping)
    {
		_logger.LogError(">>> Entering DeleteShippingMethodCountryMappingAsync(shippingMethodCountryMapping=<redacted>)");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _shippingMethodCountryMappingRepository.DeleteAsync(shippingMethodCountryMapping);
		_logger.LogError("DeleteShippingMethodCountryMappingAsync(shippingMethodCountryMapping)=<redacted>: external service call took {Arg1} ms", __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteShippingMethodCountryMappingAsync(shippingMethodCountryMapping=<redacted>)");
    }

    #endregion
}