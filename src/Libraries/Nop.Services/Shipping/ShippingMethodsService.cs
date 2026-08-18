using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Catalog;

namespace Nop.Services.Shipping;

/// <summary>
/// Shipping methods service
/// </summary>
public partial class ShippingMethodsService : IShippingMethodsService
{
    #region Fields

    protected readonly IRepository<ShippingMethod> _shippingMethodRepository;
    protected readonly IRepository<ShippingMethodCountryMapping> _shippingMethodCountryMappingRepository;

    #endregion

    #region Ctor

    public ShippingMethodsService(IRepository<ShippingMethod> shippingMethodRepository,
        IRepository<ShippingMethodCountryMapping> shippingMethodCountryMappingRepository)
    {
		_logger.LogDebug(">>> Entering ShippingMethodsService(shippingMethodRepository=<redacted>,shippingMethodCountryMappingRepository=<redacted>)");
        _shippingMethodRepository = shippingMethodRepository;
        _shippingMethodCountryMappingRepository = shippingMethodCountryMappingRepository;
		_logger.LogDebug("ShippingMethodsService(shippingMethodRepository=<redacted>,shippingMethodCountryMappingRepository=<redacted>): _shippingMethodCountryMappingRepository → {_shippingMethodCountryMappingRepository}", _shippingMethodCountryMappingRepository);
		_logger.LogDebug("ShippingMethodsService(shippingMethodRepository=<redacted>,shippingMethodCountryMappingRepository=<redacted>): _shippingMethodRepository → {_shippingMethodRepository}", _shippingMethodRepository);
		_logger.LogDebug("<<< Exiting ShippingMethodsService(shippingMethodRepository=<redacted>,shippingMethodCountryMappingRepository=<redacted>)");
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
		_logger.LogDebug(">>> Entering DeleteShippingMethodAsync(shippingMethod=<redacted>)");
		_logger.LogInformation("DeleteShippingMethodAsync(shippingMethod)={ShippingMethod}: external service call took {Arg1} ms", shippingMethod, __logStart.ElapsedMilliseconds);
        await _shippingMethodRepository.DeleteAsync(shippingMethod);
		_logger.LogDebug("<<< Exiting DeleteShippingMethodAsync(shippingMethod=<redacted>)");
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
		_logger.LogDebug(">>> Entering GetShippingMethodByIdAsync(shippingMethodId=<redacted>)");
		_logger.LogDebug("<<< Exiting GetShippingMethodByIdAsync(shippingMethodId=<redacted>)");
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
		_logger.LogDebug(">>> Entering GetAllShippingMethodsAsync(filterByCountryId={FilterByCountryId})", filterByCountryId);
        if (filterByCountryId is > 0)
        {
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            return await _shippingMethodRepository.GetAllAsync(query =>
            {
                var query1 =
                    from shippingMethod in query
					_logger.LogDebug("GetAllShippingMethodsAsync(filterByCountryId={FilterByCountryId}): query1 → {Query1}", filterByCountryId, query1);
                    join shippingMethodCountryMapping in _shippingMethodCountryMappingRepository.Table on shippingMethod.Id equals shippingMethodCountryMapping.ShippingMethodId
                    where shippingMethodCountryMapping.CountryId == filterByCountryId.Value
                    select shippingMethod.Id;
					_logger.LogInformation("GetAllShippingMethodsAsync(filterByCountryId)={FilterByCountryId}: find query executed in {Arg1} ms", filterByCountryId, __logStart.ElapsedMilliseconds);

                query1 = query1.Distinct();

                var query2 =
                    from sm in query
                    where !query1.Contains(sm.Id)
                    orderby sm.DisplayOrder, sm.Id
                    select sm;

		_logger.LogDebug("<<< Exiting GetAllShippingMethodsAsync(filterByCountryId={FilterByCountryId})", filterByCountryId);
                return query2;
            }, cache => cache.PrepareKeyForDefaultCache(NopShippingDefaults.ShippingMethodsAllCacheKey, filterByCountryId));
        }

        return await _shippingMethodRepository.GetAllAsync(query =>
            from sm in query
			using Microsoft.Extensions.Logging;
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
		private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        await _shippingMethodRepository.InsertAsync(shippingMethod);
		_logger.LogDebug(">>> Entering InsertShippingMethodAsync(shippingMethod=<redacted>)");
		_logger.LogDebug("<<< Exiting InsertShippingMethodAsync(shippingMethod=<redacted>)");
    }

    /// <summary>
    /// Updates the shipping method
    /// </summary>
    /// <param name="shippingMethod">Shipping method</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateShippingMethodAsync(ShippingMethod shippingMethod)
    {
		_logger.LogDebug(">>> Entering UpdateShippingMethodAsync(shippingMethod=<redacted>)");
        await _shippingMethodRepository.UpdateAsync(shippingMethod);
		_logger.LogDebug("<<< Exiting UpdateShippingMethodAsync(shippingMethod=<redacted>)");
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
		_logger.LogDebug(">>> Entering CountryRestrictionExistsAsync(shippingMethod=<redacted>,countryId={CountryId})", countryId);
        ArgumentNullException.ThrowIfNull(shippingMethod);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var result = await _shippingMethodCountryMappingRepository.Table
            .AnyAsync(shippingMethodCountryMapping => shippingMethodCountryMapping.ShippingMethodId == shippingMethod.Id && shippingMethodCountryMapping.CountryId == countryId);
			_logger.LogInformation("CountryRestrictionExistsAsync(shippingMethod,countryId)={ShippingMethod},{CountryId}: count query executed in {Arg2} ms", shippingMethod, countryId, __logStart.ElapsedMilliseconds);

		_logger.LogDebug("<<< Exiting CountryRestrictionExistsAsync(shippingMethod=<redacted>,countryId={CountryId})", countryId);
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
		_logger.LogDebug(">>> Entering GetShippingMethodCountryMappingAsync(shippingMethodId=<redacted>,countryId={CountryId})", countryId);
        var query = _shippingMethodCountryMappingRepository.Table.Where(shippingMethodCountryMapping =>
            shippingMethodCountryMapping.ShippingMethodId == shippingMethodId && shippingMethodCountryMapping.CountryId == countryId);

		_logger.LogDebug("<<< Exiting GetShippingMethodCountryMappingAsync(shippingMethodId=<redacted>,countryId={CountryId})", countryId);
        return await query.ToListAsync();
    }

    /// <summary>
    /// Inserts a shipping country mapping
    /// </summary>
    /// <param name="shippingMethodCountryMapping">Shipping country mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertShippingMethodCountryMappingAsync(ShippingMethodCountryMapping shippingMethodCountryMapping)
    {
		_logger.LogDebug(">>> Entering InsertShippingMethodCountryMappingAsync(shippingMethodCountryMapping=<redacted>)");
        await _shippingMethodCountryMappingRepository.InsertAsync(shippingMethodCountryMapping);
		_logger.LogDebug("<<< Exiting InsertShippingMethodCountryMappingAsync(shippingMethodCountryMapping=<redacted>)");
    }

    /// <summary>
    /// Delete the shipping country mapping
    /// </summary>
    /// <param name="shippingMethodCountryMapping">Shipping country mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteShippingMethodCountryMappingAsync(ShippingMethodCountryMapping shippingMethodCountryMapping)
    {
		_logger.LogDebug(">>> Entering DeleteShippingMethodCountryMappingAsync(shippingMethodCountryMapping=<redacted>)");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _shippingMethodCountryMappingRepository.DeleteAsync(shippingMethodCountryMapping);
		_logger.LogInformation("DeleteShippingMethodCountryMappingAsync(shippingMethodCountryMapping)={ShippingMethodCountryMapping}: external service call took {Arg1} ms", shippingMethodCountryMapping, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting DeleteShippingMethodCountryMappingAsync(shippingMethodCountryMapping=<redacted>)");
    }

    #endregion
}