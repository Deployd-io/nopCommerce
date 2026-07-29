using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Localization;

namespace Nop.Services.Directory;

/// <summary>
/// State province service
/// </summary>
public partial class StateProvinceService : IStateProvinceService
{
    #region Fields

    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly IRepository<StateProvince> _stateProvinceRepository;

    #endregion

    #region Ctor

    public StateProvinceService(IStaticCacheManager staticCacheManager,
        ILocalizationService localizationService,
        IRepository<StateProvince> stateProvinceRepository)
    {
        _staticCacheManager = staticCacheManager;
        _localizationService = localizationService;
        _stateProvinceRepository = stateProvinceRepository;
    }

    #endregion

    #region Methods
    /// <summary>
    /// Deletes a state/province
    /// </summary>
    /// <param name="stateProvince">The state/province</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteStateProvinceAsync(StateProvince stateProvince)
    {
		logger.LogDebug(">>> Entering DeleteStateProvinceAsync(stateProvince={StateProvince})", stateProvince);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _stateProvinceRepository.DeleteAsync(stateProvince);
		logger.LogDebug("<<< Exiting DeleteStateProvinceAsync(stateProvince={StateProvince})", stateProvince);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteStateProvinceAsync(stateProvince)={StateProvince}: external service call took {Arg1} ms", stateProvince, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteStateProvinceAsync(stateProvince)={StateProvince}: delete query executed in {Arg1} ms", stateProvince, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteStateProvinceAsync(stateProvince)={StateProvince}: external service call took {Arg1} ms", stateProvince, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteStateProvinceAsync(stateProvince)={StateProvince}: delete query executed in {Arg1} ms", stateProvince, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets a state/province
    /// </summary>
    /// <param name="stateProvinceId">The state/province identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the state/province
    /// </returns>
    public virtual async Task<StateProvince> GetStateProvinceByIdAsync(int stateProvinceId)
    {
		logger.LogDebug(">>> Entering GetStateProvinceByIdAsync(stateProvinceId={StateProvinceId})", stateProvinceId);
		logger.LogDebug("<<< Exiting GetStateProvinceByIdAsync(stateProvinceId={StateProvinceId})", stateProvinceId);
        return await _stateProvinceRepository.GetByIdAsync(stateProvinceId, cache => default);
    }

    /// <summary>
    /// Gets a state/province by abbreviation
    /// </summary>
    /// <param name="abbreviation">The state/province abbreviation</param>
    /// <param name="countryId">Country identifier; pass null to load the state regardless of a country</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the state/province
    /// </returns>
    public virtual async Task<StateProvince> GetStateProvinceByAbbreviationAsync(string abbreviation, int? countryId = null)
    {
		logger.LogDebug(">>> Entering GetStateProvinceByAbbreviationAsync(abbreviation={Abbreviation},countryId={CountryId})", abbreviation, countryId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (string.IsNullOrEmpty(abbreviation))
            return null;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.StateProvincesByAbbreviationCacheKey
            , abbreviation, countryId ?? 0);

        var query = _stateProvinceRepository.Table.Where(state => state.Abbreviation == abbreviation);

		logger.LogDebug("GetStateProvinceByAbbreviationAsync(abbreviation={Abbreviation},countryId={CountryId}): query → {Query}", abbreviation, countryId, query);
        //filter by country
        if (countryId.HasValue)
            query = query.Where(state => state.CountryId == countryId);

		logger.LogInformation("GetStateProvinceByAbbreviationAsync(abbreviation,countryId)={Abbreviation},{CountryId}: external service call took {Arg2} ms", abbreviation, countryId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetStateProvinceByAbbreviationAsync(abbreviation={Abbreviation},countryId={CountryId})", abbreviation, countryId);
        return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
    }

    /// <summary>
    /// Gets a state/province by address 
    /// </summary>
    /// <param name="address">Address</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country
    /// </returns>
    public virtual async Task<StateProvince> GetStateProvinceByAddressAsync(Address address)
    {
		logger.LogDebug(">>> Entering GetStateProvinceByAddressAsync(address={Address})", address);
		logger.LogDebug("<<< Exiting GetStateProvinceByAddressAsync(address={Address})", address);
        return await GetStateProvinceByIdAsync(address?.StateProvinceId ?? 0);
    }

    /// <summary>
    /// Gets a state/province collection by country identifier
    /// </summary>
    /// <param name="countryId">Country identifier</param>
    /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the states
    /// </returns>
    public virtual async Task<IList<StateProvince>> GetStateProvincesByCountryIdAsync(int countryId, int languageId = 0, bool showHidden = false)
    {
		logger.LogDebug(">>> Entering GetStateProvincesByCountryIdAsync(countryId={CountryId},languageId={LanguageId},showHidden={ShowHidden})", countryId, languageId, showHidden);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.StateProvincesByCountryCacheKey, countryId, languageId, showHidden);

        return await _staticCacheManager.GetAsync(key, async () =>
        {
		logger.LogWarning("GetStateProvincesByCountryIdAsync(countryId={CountryId},languageId={LanguageId},showHidden={ShowHidden}): key → {Key}", countryId, languageId, showHidden, key);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
            var query = from sp in _stateProvinceRepository.Table
                orderby sp.DisplayOrder, sp.Name
                where sp.CountryId == countryId &&
                      (showHidden || sp.Published)
                select sp;
            var stateProvinces = await query.ToListAsync();
			logger.LogInformation("GetStateProvincesByCountryIdAsync(countryId,languageId,showHidden)={CountryId},{LanguageId},{ShowHidden}: database query executed in {Arg3} ms", countryId, languageId, showHidden, __logStart.ElapsedMilliseconds);

            if (languageId > 0)
                //we should sort states by localized names when they have the same display order
                stateProvinces = await stateProvinces.ToAsyncEnumerable()
                    .OrderBy(c => c.DisplayOrder)
                    .ThenByAwait(async c => await _localizationService.GetLocalizedAsync(c, x => x.Name, languageId))
                    .ToListAsync();

            return stateProvinces;
        });
		logger.LogInformation("GetStateProvincesByCountryIdAsync(countryId,languageId,showHidden)={CountryId},{LanguageId},{ShowHidden}: external service call took {Arg3} ms", countryId, languageId, showHidden, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetStateProvincesByCountryIdAsync(countryId={CountryId},languageId={LanguageId},showHidden={ShowHidden})", countryId, languageId, showHidden);
    }

    /// <summary>
    /// Gets all states/provinces
    /// </summary>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the states
    /// </returns>
    public virtual async Task<IList<StateProvince>> GetStateProvincesAsync(bool showHidden = false)
    {
		logger.LogDebug(">>> Entering GetStateProvincesAsync(showHidden={ShowHidden})", showHidden);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var query = from sp in _stateProvinceRepository.Table
            orderby sp.CountryId, sp.DisplayOrder, sp.Name
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            where showHidden || sp.Published
            select sp;


        var stateProvinces = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.StateProvincesAllCacheKey, showHidden), async () => await query.ToListAsync());

		logger.LogDebug("GetStateProvincesAsync(showHidden={ShowHidden}): stateProvinces → {StateProvinces}", showHidden, stateProvinces);
		logger.LogInformation("GetStateProvincesAsync(showHidden)={ShowHidden}: external service call took {Arg1} ms", showHidden, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetStateProvincesAsync(showHidden={ShowHidden})", showHidden);
        return stateProvinces;
    }

    /// <summary>
    /// Inserts a state/province
    /// </summary>
    /// <param name="stateProvince">State/province</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertStateProvinceAsync(StateProvince stateProvince)
    {
		logger.LogDebug(">>> Entering InsertStateProvinceAsync(stateProvince={StateProvince})", stateProvince);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _stateProvinceRepository.InsertAsync(stateProvince);
		logger.LogDebug("<<< Exiting InsertStateProvinceAsync(stateProvince={StateProvince})", stateProvince);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertStateProvinceAsync(stateProvince)={StateProvince}: save query executed in {Arg1} ms", stateProvince, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertStateProvinceAsync(stateProvince)={StateProvince}: save query executed in {Arg1} ms", stateProvince, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates a state/province
    /// </summary>
    /// <param name="stateProvince">State/province</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateStateProvinceAsync(StateProvince stateProvince)
    {
		logger.LogDebug(">>> Entering UpdateStateProvinceAsync(stateProvince={StateProvince})", stateProvince);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _stateProvinceRepository.UpdateAsync(stateProvince);
		logger.LogDebug("<<< Exiting UpdateStateProvinceAsync(stateProvince={StateProvince})", stateProvince);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateStateProvinceAsync(stateProvince)={StateProvince}: update query executed in {Arg1} ms", stateProvince, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateStateProvinceAsync(stateProvince)={StateProvince}: update query executed in {Arg1} ms", stateProvince, __logStart.ElapsedMilliseconds);
    #endregion
}