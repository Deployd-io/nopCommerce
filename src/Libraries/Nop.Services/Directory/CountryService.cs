using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Stores;

namespace Nop.Services.Directory;

/// <summary>
/// Country service
/// </summary>
public partial class CountryService : ICountryService
{
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
        _staticCacheManager = staticCacheManager;
        _localizationService = localizationService;
        _countryRepository = countryRepository;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
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
		logger.LogDebug(">>> Entering DeleteCountryAsync(country={Country})", country);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _countryRepository.DeleteAsync(country);
		logger.LogDebug("<<< Exiting DeleteCountryAsync(country={Country})", country);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteCountryAsync(country)={Country}: external service call took {Arg1} ms", country, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteCountryAsync(country)={Country}: delete query executed in {Arg1} ms", country, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteCountryAsync(country)={Country}: external service call took {Arg1} ms", country, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteCountryAsync(country)={Country}: delete query executed in {Arg1} ms", country, __logStart.ElapsedMilliseconds);
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
		logger.LogDebug(">>> Entering GetAllCountriesAsync(languageId={LanguageId},showHidden={ShowHidden})", languageId, showHidden);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var store = await _storeContext.GetCurrentStoreAsync();
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.CountriesAllCacheKey, languageId,
            showHidden, store);
			logger.LogDebug("GetAllCountriesAsync(languageId={LanguageId},showHidden={ShowHidden}): store → {Store}", languageId, showHidden, store);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        return await _staticCacheManager.GetAsync(key, async () =>
        {
            var countries = await _countryRepository.GetAllAsync(async query =>
            {
                if (!showHidden)
                {
                    query = query.Where(c => c.Published);

					logger.LogDebug("GetAllCountriesAsync(languageId={LanguageId},showHidden={ShowHidden}): query → {Query}", languageId, showHidden, query);
					logger.LogInformation("GetAllCountriesAsync(languageId,showHidden)={LanguageId},{ShowHidden}: find query executed in {Arg2} ms", languageId, showHidden, __logStart.ElapsedMilliseconds);
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
					logger.LogInformation("GetAllCountriesAsync(languageId,showHidden)={LanguageId},{ShowHidden}: database query executed in {Arg2} ms", languageId, showHidden, __logStart.ElapsedMilliseconds);
            }

            return countries;
        });
		logger.LogInformation("GetAllCountriesAsync(languageId,showHidden)={LanguageId},{ShowHidden}: external service call took {Arg2} ms", languageId, showHidden, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetAllCountriesAsync(languageId={LanguageId},showHidden={ShowHidden})", languageId, showHidden);
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
		logger.LogDebug(">>> Entering GetAllCountriesForBillingAsync(languageId={LanguageId},showHidden={ShowHidden})", languageId, showHidden);
		logger.LogDebug("<<< Exiting GetAllCountriesForBillingAsync(languageId={LanguageId},showHidden={ShowHidden})", languageId, showHidden);
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
		logger.LogDebug(">>> Entering GetAllCountriesForShippingAsync(languageId={LanguageId},showHidden={ShowHidden})", languageId, showHidden);
		logger.LogDebug("<<< Exiting GetAllCountriesForShippingAsync(languageId={LanguageId},showHidden={ShowHidden})", languageId, showHidden);
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
		logger.LogDebug(">>> Entering GetCountryByAddressAsync(address={Address})", address);
		logger.LogDebug("<<< Exiting GetCountryByAddressAsync(address={Address})", address);
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
		logger.LogDebug(">>> Entering GetCountryByIdAsync(countryId={CountryId})", countryId);
		logger.LogDebug("<<< Exiting GetCountryByIdAsync(countryId={CountryId})", countryId);
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
		logger.LogDebug(">>> Entering GetCountriesByIdsAsync(countryIds={CountryIds})", countryIds);
		logger.LogDebug("<<< Exiting GetCountriesByIdsAsync(countryIds={CountryIds})", countryIds);
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
		logger.LogDebug(">>> Entering GetCountryByTwoLetterIsoCodeAsync(twoLetterIsoCode={TwoLetterIsoCode})", twoLetterIsoCode);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (string.IsNullOrEmpty(twoLetterIsoCode))
            return null;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.CountriesByTwoLetterCodeCacheKey, twoLetterIsoCode);

		logger.LogWarning("GetCountryByTwoLetterIsoCodeAsync(twoLetterIsoCode={TwoLetterIsoCode}): key → {Key}", twoLetterIsoCode, key);
        var query = from c in _countryRepository.Table
            where c.TwoLetterIsoCode == twoLetterIsoCode
            select c;

		logger.LogInformation("GetCountryByTwoLetterIsoCodeAsync(twoLetterIsoCode)={TwoLetterIsoCode}: external service call took {Arg1} ms", twoLetterIsoCode, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetCountryByTwoLetterIsoCodeAsync(twoLetterIsoCode={TwoLetterIsoCode})", twoLetterIsoCode);
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
		logger.LogDebug(">>> Entering GetCountryByThreeLetterIsoCodeAsync(threeLetterIsoCode={ThreeLetterIsoCode})", threeLetterIsoCode);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (string.IsNullOrEmpty(threeLetterIsoCode))
            return null;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.CountriesByThreeLetterCodeCacheKey, threeLetterIsoCode);

		logger.LogWarning("GetCountryByThreeLetterIsoCodeAsync(threeLetterIsoCode={ThreeLetterIsoCode}): key → {Key}", threeLetterIsoCode, key);
        var query = from c in _countryRepository.Table
            where c.ThreeLetterIsoCode == threeLetterIsoCode
            select c;

		logger.LogInformation("GetCountryByThreeLetterIsoCodeAsync(threeLetterIsoCode)={ThreeLetterIsoCode}: external service call took {Arg1} ms", threeLetterIsoCode, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetCountryByThreeLetterIsoCodeAsync(threeLetterIsoCode={ThreeLetterIsoCode})", threeLetterIsoCode);
        return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
    }

    /// <summary>
    /// Inserts a country
    /// </summary>
    /// <param name="country">Country</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertCountryAsync(Country country)
    {
		logger.LogDebug(">>> Entering InsertCountryAsync(country={Country})", country);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _countryRepository.InsertAsync(country);
		logger.LogDebug("<<< Exiting InsertCountryAsync(country={Country})", country);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertCountryAsync(country)={Country}: save query executed in {Arg1} ms", country, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertCountryAsync(country)={Country}: save query executed in {Arg1} ms", country, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the country
    /// </summary>
    /// <param name="country">Country</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateCountryAsync(Country country)
    {
		logger.LogDebug(">>> Entering UpdateCountryAsync(country={Country})", country);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _countryRepository.UpdateAsync(country);
		logger.LogDebug("<<< Exiting UpdateCountryAsync(country={Country})", country);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateCountryAsync(country)={Country}: update query executed in {Arg1} ms", country, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateCountryAsync(country)={Country}: update query executed in {Arg1} ms", country, __logStart.ElapsedMilliseconds);
    #endregion
}