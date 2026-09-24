using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Directory;

/// <summary>
/// State province service
/// </summary>
public partial class StateProvinceService : IStateProvinceService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogError(">>> Entering StateProvinceService(staticCacheManager={StaticCacheManager},localizationService={LocalizationService},stateProvinceRepository={StateProvinceRepository})", staticCacheManager, localizationService, stateProvinceRepository);
        _staticCacheManager = staticCacheManager;
        _localizationService = localizationService;
		_logger.LogDebug("StateProvinceService(staticCacheManager={StaticCacheManager},localizationService={LocalizationService},stateProvinceRepository={StateProvinceRepository}): _localizationService → {_localizationService}", staticCacheManager, localizationService, stateProvinceRepository, _localizationService);
		_logger.LogDebug("StateProvinceService(staticCacheManager={StaticCacheManager},localizationService={LocalizationService},stateProvinceRepository={StateProvinceRepository}): _staticCacheManager → {_staticCacheManager}", staticCacheManager, localizationService, stateProvinceRepository, _staticCacheManager);
    _logger.LogWarning("<<< Exiting StateProvinceService(staticCacheManager={StaticCacheManager},localizationService={LocalizationService},stateProvinceRepository={StateProvinceRepository})", staticCacheManager, localizationService, stateProvinceRepository);
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
		_logger.LogError(">>> Entering DeleteStateProvinceAsync(stateProvince={StateProvince})", stateProvince);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _stateProvinceRepository.DeleteAsync(stateProvince);
		_logger.LogError("DeleteStateProvinceAsync(stateProvince)={StateProvince}: external service call took {Arg1} ms", stateProvince, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteStateProvinceAsync(stateProvince={StateProvince})", stateProvince);
    }

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
		_logger.LogError(">>> Entering GetStateProvinceByIdAsync(stateProvinceId={StateProvinceId})", stateProvinceId);
		_logger.LogError("GetStateProvinceByIdAsync(stateProvinceId)={StateProvinceId}: find query executing", stateProvinceId);
    _logger.LogWarning("<<< Exiting GetStateProvinceByIdAsync(stateProvinceId={StateProvinceId})", stateProvinceId);
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
		_logger.LogError(">>> Entering GetStateProvinceByAbbreviationAsync(abbreviation={Abbreviation},countryId={CountryId})", abbreviation, countryId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (string.IsNullOrEmpty(abbreviation))
            return null;

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.StateProvincesByAbbreviationCacheKey
            , abbreviation, countryId ?? 0);

        var query = _stateProvinceRepository.Table.Where(state => state.Abbreviation == abbreviation);
		_logger.LogError("GetStateProvinceByAbbreviationAsync(abbreviation={Abbreviation},countryId={CountryId}): query → {Query}", abbreviation, countryId, query);

        //filter by country
        if (countryId.HasValue)
            query = query.Where(state => state.CountryId == countryId);

		_logger.LogError("GetStateProvinceByAbbreviationAsync(abbreviation,countryId)={Abbreviation},{CountryId}: find query executing", abbreviation, countryId);
		_logger.LogError("GetStateProvinceByAbbreviationAsync(abbreviation,countryId)={Abbreviation},{CountryId}: external service call took {Arg2} ms", abbreviation, countryId, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetStateProvinceByAbbreviationAsync(abbreviation={Abbreviation},countryId={CountryId})", abbreviation, countryId);
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
		_logger.LogError(">>> Entering GetStateProvinceByAddressAsync(address={Address})", address);
        _logger.LogWarning("<<< Exiting GetStateProvinceByAddressAsync(address={Address})", address);
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
		_logger.LogError(">>> Entering GetStateProvincesByCountryIdAsync(countryId={CountryId},languageId={LanguageId},showHidden={ShowHidden})", countryId, languageId, showHidden);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.StateProvincesByCountryCacheKey, countryId, languageId, showHidden);

		_logger.LogError("GetStateProvincesByCountryIdAsync(countryId={CountryId},languageId={LanguageId},showHidden={ShowHidden}): key → {Key}", countryId, languageId, showHidden, key);
        return await _staticCacheManager.GetAsync(key, async () =>
        {
            var query = from sp in _stateProvinceRepository.Table
                orderby sp.DisplayOrder, sp.Name
                where sp.CountryId == countryId &&
                      (showHidden || sp.Published)
                select sp;
				var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
				_logger.LogError("GetStateProvincesByCountryIdAsync(countryId,languageId,showHidden)={CountryId},{LanguageId},{ShowHidden}: database query executed in {Arg3} ms", countryId, languageId, showHidden, __logStartDb.ElapsedMilliseconds);
            var stateProvinces = await query.ToListAsync();

            if (languageId > 0)
                //we should sort states by localized names when they have the same display order
                stateProvinces = await stateProvinces.ToAsyncEnumerable()
                    .OrderBy(c => c.DisplayOrder)
                    .ThenByAwait(async c => await _localizationService.GetLocalizedAsync(c, x => x.Name, languageId))
                    .ToListAsync();

            return stateProvinces;
        });
		_logger.LogError("GetStateProvincesByCountryIdAsync(countryId,languageId,showHidden)={CountryId},{LanguageId},{ShowHidden}: external service call took {Arg3} ms", countryId, languageId, showHidden, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetStateProvincesByCountryIdAsync(countryId={CountryId},languageId={LanguageId},showHidden={ShowHidden})", countryId, languageId, showHidden);
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
		_logger.LogError(">>> Entering GetStateProvincesAsync(showHidden={ShowHidden})", showHidden);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var query = from sp in _stateProvinceRepository.Table
            orderby sp.CountryId, sp.DisplayOrder, sp.Name
            where showHidden || sp.Published
            select sp;


		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("GetStateProvincesAsync(showHidden)={ShowHidden}: external service call took {Arg1} ms", showHidden, __logStart.ElapsedMilliseconds);
		_logger.LogError("GetStateProvincesAsync(showHidden)={ShowHidden}: find query executed in {Arg1} ms", showHidden, __logStartDb.ElapsedMilliseconds);
        var stateProvinces = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.StateProvincesAllCacheKey, showHidden), async () => await query.ToListAsync());
		_logger.LogError("GetStateProvincesAsync(showHidden={ShowHidden}): stateProvinces → {StateProvinces}", showHidden, stateProvinces);

		_logger.LogWarning("<<< Exiting GetStateProvincesAsync(showHidden={ShowHidden})", showHidden);
        return stateProvinces;
    }

    /// <summary>
    /// Inserts a state/province
    /// </summary>
    /// <param name="stateProvince">State/province</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertStateProvinceAsync(StateProvince stateProvince)
    {
		_logger.LogError(">>> Entering InsertStateProvinceAsync(stateProvince={StateProvince})", stateProvince);
        await _stateProvinceRepository.InsertAsync(stateProvince);
		_logger.LogWarning("<<< Exiting InsertStateProvinceAsync(stateProvince={StateProvince})", stateProvince);
    }

    /// <summary>
    /// Updates a state/province
    /// </summary>
    /// <param name="stateProvince">State/province</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateStateProvinceAsync(StateProvince stateProvince)
    {
		_logger.LogError(">>> Entering UpdateStateProvinceAsync(stateProvince={StateProvince})", stateProvince);
        await _stateProvinceRepository.UpdateAsync(stateProvince);
		_logger.LogWarning("<<< Exiting UpdateStateProvinceAsync(stateProvince={StateProvince})", stateProvince);
    }

    #endregion
}