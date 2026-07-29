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
        _shippingMethodRepository = shippingMethodRepository;
        _shippingMethodCountryMappingRepository = shippingMethodCountryMappingRepository;
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
		logger.LogDebug(">>> Entering DeleteShippingMethodAsync(shippingMethod={ShippingMethod})", shippingMethod);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _shippingMethodRepository.DeleteAsync(shippingMethod);
		logger.LogDebug("<<< Exiting DeleteShippingMethodAsync(shippingMethod={ShippingMethod})", shippingMethod);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteShippingMethodAsync(shippingMethod)={ShippingMethod}: external service call took {Arg1} ms", shippingMethod, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteShippingMethodAsync(shippingMethod)={ShippingMethod}: delete query executed in {Arg1} ms", shippingMethod, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteShippingMethodAsync(shippingMethod)={ShippingMethod}: external service call took {Arg1} ms", shippingMethod, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteShippingMethodAsync(shippingMethod)={ShippingMethod}: delete query executed in {Arg1} ms", shippingMethod, __logStart.ElapsedMilliseconds);
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
		logger.LogDebug(">>> Entering GetShippingMethodByIdAsync(shippingMethodId={ShippingMethodId})", shippingMethodId);
		logger.LogDebug("<<< Exiting GetShippingMethodByIdAsync(shippingMethodId={ShippingMethodId})", shippingMethodId);
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
		logger.LogDebug(">>> Entering GetAllShippingMethodsAsync(filterByCountryId={FilterByCountryId})", filterByCountryId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (filterByCountryId is > 0)
        {
            return await _shippingMethodRepository.GetAllAsync(query =>
            {
                var query1 =
                    from shippingMethod in query
					logger.LogDebug("GetAllShippingMethodsAsync(filterByCountryId={FilterByCountryId}): query1 → {Query1}", filterByCountryId, query1);
                    join shippingMethodCountryMapping in _shippingMethodCountryMappingRepository.Table on shippingMethod.Id equals shippingMethodCountryMapping.ShippingMethodId
                    where shippingMethodCountryMapping.CountryId == filterByCountryId.Value
                    select shippingMethod.Id;
					logger.LogInformation("GetAllShippingMethodsAsync(filterByCountryId)={FilterByCountryId}: find query executed in {Arg1} ms", filterByCountryId, __logStart.ElapsedMilliseconds);

                query1 = query1.Distinct();

                var query2 =
                    from sm in query
                    where !query1.Contains(sm.Id)
                    orderby sm.DisplayOrder, sm.Id
                    select sm;

                return query2;
            }, cache => cache.PrepareKeyForDefaultCache(NopShippingDefaults.ShippingMethodsAllCacheKey, filterByCountryId));
        }

		logger.LogDebug("<<< Exiting GetAllShippingMethodsAsync(filterByCountryId={FilterByCountryId})", filterByCountryId);
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
        await _shippingMethodRepository.InsertAsync(shippingMethod);
		logger.LogDebug(">>> Entering InsertShippingMethodAsync(shippingMethod={ShippingMethod})", shippingMethod);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogDebug("<<< Exiting InsertShippingMethodAsync(shippingMethod={ShippingMethod})", shippingMethod);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertShippingMethodAsync(shippingMethod)={ShippingMethod}: save query executed in {Arg1} ms", shippingMethod, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
	logger.LogInformation("InsertShippingMethodAsync(shippingMethod)={ShippingMethod}: save query executed in {Arg1} ms", shippingMethod, __logStart.ElapsedMilliseconds);
    /// Updates the shipping method
    /// </summary>
    /// <param name="shippingMethod">Shipping method</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateShippingMethodAsync(ShippingMethod shippingMethod)
    {
		logger.LogDebug(">>> Entering UpdateShippingMethodAsync(shippingMethod={ShippingMethod})", shippingMethod);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _shippingMethodRepository.UpdateAsync(shippingMethod);
		logger.LogDebug("<<< Exiting UpdateShippingMethodAsync(shippingMethod={ShippingMethod})", shippingMethod);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateShippingMethodAsync(shippingMethod)={ShippingMethod}: update query executed in {Arg1} ms", shippingMethod, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateShippingMethodAsync(shippingMethod)={ShippingMethod}: update query executed in {Arg1} ms", shippingMethod, __logStart.ElapsedMilliseconds);
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
		logger.LogDebug(">>> Entering CountryRestrictionExistsAsync(shippingMethod={ShippingMethod},countryId={CountryId})", shippingMethod, countryId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(shippingMethod);

        var result = await _shippingMethodCountryMappingRepository.Table
            .AnyAsync(shippingMethodCountryMapping => shippingMethodCountryMapping.ShippingMethodId == shippingMethod.Id && shippingMethodCountryMapping.CountryId == countryId);
			logger.LogInformation("CountryRestrictionExistsAsync(shippingMethod,countryId)={ShippingMethod},{CountryId}: count query executed in {Arg2} ms", shippingMethod, countryId, __logStart.ElapsedMilliseconds);

		logger.LogDebug("<<< Exiting CountryRestrictionExistsAsync(shippingMethod={ShippingMethod},countryId={CountryId})", shippingMethod, countryId);
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
        var query = _shippingMethodCountryMappingRepository.Table.Where(shippingMethodCountryMapping =>
            shippingMethodCountryMapping.ShippingMethodId == shippingMethodId && shippingMethodCountryMapping.CountryId == countryId);

        return await query.ToListAsync();
    }

    /// <summary>
    /// Inserts a shipping country mapping
    /// </summary>
    /// <param name="shippingMethodCountryMapping">Shipping country mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertShippingMethodCountryMappingAsync(ShippingMethodCountryMapping shippingMethodCountryMapping)
    {
		logger.LogDebug(">>> Entering InsertShippingMethodCountryMappingAsync(shippingMethodCountryMapping={ShippingMethodCountryMapping})", shippingMethodCountryMapping);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _shippingMethodCountryMappingRepository.InsertAsync(shippingMethodCountryMapping);
		logger.LogDebug("<<< Exiting InsertShippingMethodCountryMappingAsync(shippingMethodCountryMapping={ShippingMethodCountryMapping})", shippingMethodCountryMapping);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertShippingMethodCountryMappingAsync(shippingMethodCountryMapping)={ShippingMethodCountryMapping}: save query executed in {Arg1} ms", shippingMethodCountryMapping, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertShippingMethodCountryMappingAsync(shippingMethodCountryMapping)={ShippingMethodCountryMapping}: save query executed in {Arg1} ms", shippingMethodCountryMapping, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Delete the shipping country mapping
    /// </summary>
    /// <param name="shippingMethodCountryMapping">Shipping country mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteShippingMethodCountryMappingAsync(ShippingMethodCountryMapping shippingMethodCountryMapping)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogDebug(">>> Entering DeleteShippingMethodCountryMappingAsync(shippingMethodCountryMapping={ShippingMethodCountryMapping})", shippingMethodCountryMapping);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _shippingMethodCountryMappingRepository.DeleteAsync(shippingMethodCountryMapping);
		logger.LogDebug("<<< Exiting DeleteShippingMethodCountryMappingAsync(shippingMethodCountryMapping={ShippingMethodCountryMapping})", shippingMethodCountryMapping);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteShippingMethodCountryMappingAsync(shippingMethodCountryMapping)={ShippingMethodCountryMapping}: external service call took {Arg1} ms", shippingMethodCountryMapping, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteShippingMethodCountryMappingAsync(shippingMethodCountryMapping)={ShippingMethodCountryMapping}: external service call took {Arg1} ms", shippingMethodCountryMapping, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
    #endregion
}