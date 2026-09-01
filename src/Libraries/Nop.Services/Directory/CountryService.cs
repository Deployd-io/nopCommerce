using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Directory;

/// <summary>
/// Country service
/// </summary>
public partial class CountryService : ICountryService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly IRepository<Country> _countryRepository;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;

    #endregion

    #region Ctor

    public CountryService(
        IStaticCacheManager staticCacheManager,
        ILocalizationService localizationService,
        IRepository<Country> countryRepository,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService)
    {
		_logger.LogDebug(">>> Entering CountryService(staticCacheManager={StaticCacheManager},localizationService={LocalizationService},countryRepository={CountryRepository},storeContext={StoreContext},storeMappingService=<redacted>)", staticCacheManager, localizationService, countryRepository, storeContext);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("CountryService(staticCacheManager={StaticCacheManager},localizationService={LocalizationService},countryRepository={CountryRepository},storeContext={StoreContext},storeMappingService=<redacted>): _staticCacheManager → {_staticCacheManager}", staticCacheManager, localizationService, countryRepository, storeContext, _staticCacheManager);
        _localizationService = localizationService;
		_logger.LogDebug("CountryService(staticCacheManager={StaticCacheManager},localizationService={LocalizationService},countryRepository={CountryRepository},storeContext={StoreContext},storeMappingService=<redacted>): _localizationService → {_localizationService}", staticCacheManager, localizationService, countryRepository, storeContext, _localizationService);
        _countryRepository = countryRepository;
		_logger.LogDebug("CountryService(staticCacheManager={StaticCacheManager},localizationService={LocalizationService},countryRepository={CountryRepository},storeContext={StoreContext},storeMappingService=<redacted>): _countryRepository → {_countryRepository}", staticCacheManager, localizationService, countryRepository, storeContext, _countryRepository);
        _storeContext = storeContext;
		_logger.LogDebug("CountryService(staticCacheManager={StaticCacheManager},localizationService={LocalizationService},countryRepository={CountryRepository},storeContext={StoreContext},storeMappingService=<redacted>): _storeContext → {_storeContext}", staticCacheManager, localizationService, countryRepository, storeContext, _storeContext);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("CountryService(staticCacheManager={StaticCacheManager},localizationService={LocalizationService},countryRepository={CountryRepository},storeContext={StoreContext},storeMappingService=<redacted>): _storeMappingService → {_storeMappingService}", staticCacheManager, localizationService, countryRepository, storeContext, _storeMappingService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a country
    /// </summary>
    /// <param name="country">Country</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteCountryAsync(Country country)
    {
		_logger.LogDebug(">>> Entering DeleteCountryAsync(country={Country})", country);
        await _countryRepository.DeleteAsync(country);
    }

    /// <summary>
    /// Gets all countries
    /// </summary>
    /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the countries
    /// </returns>
    public virtual async Task<IList<Country>> GetAllCountriesAsync(int languageId = 0, bool showHidden = false)
    {
		_logger.LogDebug(">>> Entering GetAllCountriesAsync(languageId={LanguageId},showHidden={ShowHidden})", languageId, showHidden);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var store = await _storeContext.GetCurrentStoreAsync();
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.CountriesAllCacheKey, languageId,
            showHidden, store);
			_logger.LogDebug("GetAllCountriesAsync(languageId={LanguageId},showHidden={ShowHidden}): store → {Store}", languageId, showHidden, store);

        return await _staticCacheManager.GetAsync(key, async () =>
        {
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
            var countries = await _countryRepository.GetAllAsync(async query =>
            {
                if (!showHidden)
                {
                    query = query.Where(c => c.Published);
					_logger.LogInformation("GetAllCountriesAsync(languageId,showHidden)={LanguageId},{ShowHidden}: find query executed in {Arg2} ms", languageId, showHidden, __logStartDb.ElapsedMilliseconds);

                    //apply store mapping constraints
                    query = await _storeMappingService.ApplyStoreMapping(query, store.Id);
                }

                return query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);
            });

            if (languageId > 0)
            {
                //we should sort countries by localized names when they have the same display order
                countries = await countries
                    .ToAsyncEnumerable()
                    .OrderBy(c => c.DisplayOrder)
                    .ThenByAwait(async c => await _localizationService.GetLocalizedAsync(c, x => x.Name, languageId))
                    .ToListAsync();
					_logger.LogInformation("GetAllCountriesAsync(languageId,showHidden)={LanguageId},{ShowHidden}: database query executed in {Arg2} ms", languageId, showHidden, __logStartDb.ElapsedMilliseconds);
            }

            return countries;
        });
		_logger.LogInformation("GetAllCountriesAsync(languageId,showHidden)={LanguageId},{ShowHidden}: external service call took {Arg2} ms", languageId, showHidden, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetAllCountriesAsync(languageId={LanguageId},showHidden={ShowHidden})", languageId, showHidden);
    }

    /// <summary>
    /// Gets all countries that allow billing
    /// </summary>
    /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the countries
    /// </returns>
    public virtual async Task<IList<Country>> GetAllCountriesForBillingAsync(int languageId = 0, bool showHidden = false)
    {
		_logger.LogDebug(">>> Entering GetAllCountriesForBillingAsync(languageId={LanguageId},showHidden={ShowHidden})", languageId, showHidden);
		_logger.LogDebug("<<< Exiting GetAllCountriesForBillingAsync(languageId={LanguageId},showHidden={ShowHidden})", languageId, showHidden);
        return (await GetAllCountriesAsync(languageId, showHidden)).Where(c => c.AllowsBilling).ToList();
    }

    /// <summary>
    /// Gets all countries that allow shipping
    /// </summary>
    /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the countries
    /// </returns>
    public virtual async Task<IList<Country>> GetAllCountriesForShippingAsync(int languageId = 0, bool showHidden = false)
    {
		_logger.LogDebug(">>> Entering GetAllCountriesForShippingAsync(languageId={LanguageId},showHidden={ShowHidden})", languageId, showHidden);
		_logger.LogDebug("<<< Exiting GetAllCountriesForShippingAsync(languageId={LanguageId},showHidden={ShowHidden})", languageId, showHidden);
        return (await GetAllCountriesAsync(languageId, showHidden)).Where(c => c.AllowsShipping).ToList();
    }

    /// <summary>
    /// Gets a country by address 
    /// </summary>
    /// <param name="address">Address</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country
    /// </returns>
    public virtual async Task<Country> GetCountryByAddressAsync(Address address)
    {
		_logger.LogDebug(">>> Entering GetCountryByAddressAsync(address={Address})", address);
        return await GetCountryByIdAsync(address?.CountryId ?? 0);
    }

    /// <summary>
    /// Gets a country 
    /// </summary>
    /// <param name="countryId">Country identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country
    /// </returns>
    public virtual async Task<Country> GetCountryByIdAsync(int countryId)
    {
		_logger.LogDebug(">>> Entering GetCountryByIdAsync(countryId={CountryId})", countryId);
		_logger.LogInformation("GetCountryByIdAsync(countryId)={CountryId}: find query executing", countryId);
        return await _countryRepository.GetByIdAsync(countryId, cache => default);
    }

    /// <summary>
    /// Get countries by identifiers
    /// </summary>
    /// <param name="countryIds">Country identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the countries
    /// </returns>
    public virtual async Task<IList<Country>> GetCountriesByIdsAsync(int[] countryIds)
    {
		_logger.LogDebug(">>> Entering GetCountriesByIdsAsync(countryIds={CountryIds})", countryIds);
        return await _countryRepository.GetByIdsAsync(countryIds);
    }

    /// <summary>
    /// Gets a country by two letter ISO code
    /// </summary>
    /// <param name="twoLetterIsoCode">Country two letter ISO code</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country
    /// </returns>
    public virtual async Task<Country> GetCountryByTwoLetterIsoCodeAsync(string twoLetterIsoCode)
    {
		_logger.LogDebug(">>> Entering GetCountryByTwoLetterIsoCodeAsync(twoLetterIsoCode={TwoLetterIsoCode})", twoLetterIsoCode);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (string.IsNullOrEmpty(twoLetterIsoCode))
            return null;

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.CountriesByTwoLetterCodeCacheKey, twoLetterIsoCode);
		_logger.LogWarning("GetCountryByTwoLetterIsoCodeAsync(twoLetterIsoCode={TwoLetterIsoCode}): key → {Key}", twoLetterIsoCode, key);

        var query = from c in _countryRepository.Table
            where c.TwoLetterIsoCode == twoLetterIsoCode
            select c;

		_logger.LogInformation("GetCountryByTwoLetterIsoCodeAsync(twoLetterIsoCode)={TwoLetterIsoCode}: find query executing", twoLetterIsoCode);
		_logger.LogInformation("GetCountryByTwoLetterIsoCodeAsync(twoLetterIsoCode)={TwoLetterIsoCode}: external service call took {Arg1} ms", twoLetterIsoCode, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetCountryByTwoLetterIsoCodeAsync(twoLetterIsoCode={TwoLetterIsoCode})", twoLetterIsoCode);
        return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
    }

    /// <summary>
    /// Gets a country by three letter ISO code
    /// </summary>
    /// <param name="threeLetterIsoCode">Country three letter ISO code</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country
    /// </returns>
    public virtual async Task<Country> GetCountryByThreeLetterIsoCodeAsync(string threeLetterIsoCode)
    {
		_logger.LogDebug(">>> Entering GetCountryByThreeLetterIsoCodeAsync(threeLetterIsoCode={ThreeLetterIsoCode})", threeLetterIsoCode);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (string.IsNullOrEmpty(threeLetterIsoCode))
            return null;

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.CountriesByThreeLetterCodeCacheKey, threeLetterIsoCode);
		_logger.LogWarning("GetCountryByThreeLetterIsoCodeAsync(threeLetterIsoCode={ThreeLetterIsoCode}): key → {Key}", threeLetterIsoCode, key);

        var query = from c in _countryRepository.Table
            where c.ThreeLetterIsoCode == threeLetterIsoCode
            select c;

		_logger.LogInformation("GetCountryByThreeLetterIsoCodeAsync(threeLetterIsoCode)={ThreeLetterIsoCode}: find query executing", threeLetterIsoCode);
		_logger.LogInformation("GetCountryByThreeLetterIsoCodeAsync(threeLetterIsoCode)={ThreeLetterIsoCode}: external service call took {Arg1} ms", threeLetterIsoCode, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetCountryByThreeLetterIsoCodeAsync(threeLetterIsoCode={ThreeLetterIsoCode})", threeLetterIsoCode);
        return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
    }

    /// <summary>
    /// Inserts a country
    /// </summary>
    /// <param name="country">Country</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertCountryAsync(Country country)
    {
		_logger.LogDebug(">>> Entering InsertCountryAsync(country={Country})", country);
        await _countryRepository.InsertAsync(country);
    }

    /// <summary>
    /// Updates the country
    /// </summary>
    /// <param name="country">Country</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateCountryAsync(Country country)
    {
		_logger.LogDebug(">>> Entering UpdateCountryAsync(country={Country})", country);
        await _countryRepository.UpdateAsync(country);
    }

    #endregion
}