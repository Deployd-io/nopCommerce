using System.Globalization;
using Nop.Core.Caching;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Stores;

namespace Nop.Services.Localization;

/// <summary>
/// Language service
/// </summary>
public partial class LanguageService : ILanguageService
{
    #region Fields

    protected readonly IRepository<Language> _languageRepository;
    protected readonly ISettingService _settingService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public LanguageService(IRepository<Language> languageRepository,
        ISettingService settingService,
        IStaticCacheManager staticCacheManager,
        IStoreMappingService storeMappingService,
        LocalizationSettings localizationSettings)
    {
        _languageRepository = languageRepository;
        _settingService = settingService;
        _staticCacheManager = staticCacheManager;
        _storeMappingService = storeMappingService;
        _localizationSettings = localizationSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a language
    /// </summary>
    /// <param name="language">Language</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteLanguageAsync(Language language)
    {
		logger.LogDebug(">>> Entering DeleteLanguageAsync(language={Language})", language);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(language);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //update default admin area language (if required)
        if (_localizationSettings.DefaultAdminLanguageId == language.Id)
            foreach (var activeLanguage in await GetAllLanguagesAsync())
            {
                if (activeLanguage.Id == language.Id)
                    continue;

                _localizationSettings.DefaultAdminLanguageId = activeLanguage.Id;
				logger.LogDebug("DeleteLanguageAsync(language={Language}): _localizationSettings.DefaultAdminLanguageId → {Arg1}", language, _localizationSettings.DefaultAdminLanguageId);
                await _settingService.SaveSettingAsync(_localizationSettings);
                break;
            }

        await _languageRepository.DeleteAsync(language);
		logger.LogDebug("<<< Exiting DeleteLanguageAsync(language={Language})", language);
		logger.LogInformation("DeleteLanguageAsync(language)={Language}: external service call took {Arg1} ms", language, __logStart.ElapsedMilliseconds);
		logger.LogInformation("DeleteLanguageAsync(language)={Language}: delete query executed in {Arg1} ms", language, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Gets all languages
    /// </summary>
    /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the languages
    /// </returns>
    public virtual async Task<IList<Language>> GetAllLanguagesAsync(bool showHidden = false, int storeId = 0)
    {
		logger.LogDebug(">>> Entering GetAllLanguagesAsync(showHidden={ShowHidden},storeId={StoreId})", showHidden, storeId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopLocalizationDefaults.LanguagesAllCacheKey, storeId, showHidden);

        var languages = await _staticCacheManager.GetAsync(key, async () =>
        {
		logger.LogWarning("GetAllLanguagesAsync(showHidden={ShowHidden},storeId={StoreId}): key → {Key}", showHidden, storeId, key);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
            var allLanguages = await _languageRepository.GetAllAsync(async query =>
            {
				logger.LogDebug("GetAllLanguagesAsync(showHidden={ShowHidden},storeId={StoreId}): query → {Query}", showHidden, storeId, query);
                if (!showHidden)
                    query = query.Where(l => l.Published);

				logger.LogInformation("GetAllLanguagesAsync(showHidden,storeId)={ShowHidden},{StoreId}: find query executed in {Arg2} ms", showHidden, storeId, __logStart.ElapsedMilliseconds);
                if (storeId > 0)
                    query = await _storeMappingService.ApplyStoreMapping(query, storeId);

                query = query.OrderBy(l => l.DisplayOrder).ThenBy(l => l.Id);

                return query;
            });

            return allLanguages;
        });

        return languages;
    }

    /// <summary>
    /// Gets all languages
    /// </summary>
    /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// The languages
    /// </returns>
    public virtual IList<Language> GetAllLanguages(bool showHidden = false, int storeId = 0)
    {
		logger.LogDebug(">>> Entering GetAllLanguages(showHidden={ShowHidden},storeId={StoreId})", showHidden, storeId);
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopLocalizationDefaults.LanguagesAllCacheKey, storeId, showHidden);

		logger.LogWarning("GetAllLanguages(showHidden={ShowHidden},storeId={StoreId}): key → {Key}", showHidden, storeId, key);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var languages = _staticCacheManager.Get(key, () =>
        {
            var allLanguages = _languageRepository.GetAll(query =>
            {
                if (!showHidden)
                    query = query.Where(l => l.Published);
                query = query.OrderBy(l => l.DisplayOrder).ThenBy(l => l.Id);
				logger.LogDebug("GetAllLanguages(showHidden={ShowHidden},storeId={StoreId}): query → {Query}", showHidden, storeId, query);
				logger.LogInformation("GetAllLanguages(showHidden,storeId)={ShowHidden},{StoreId}: find query executed in {Arg2} ms", showHidden, storeId, __logStart.ElapsedMilliseconds);

                return query;
            });

            //store mapping
            if (storeId > 0)
                allLanguages = allLanguages
                    .Where(l => _storeMappingService.Authorize(l, storeId))
                    .ToList();

            return allLanguages;
        });

		logger.LogDebug("<<< Exiting GetAllLanguages(showHidden={ShowHidden},storeId={StoreId})", showHidden, storeId);
        return languages;
    }

    /// <summary>
    /// Gets a language
    /// </summary>
    /// <param name="languageId">Language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the language
    /// </returns>
    public virtual async Task<Language> GetLanguageByIdAsync(int languageId)
    {
		logger.LogDebug(">>> Entering GetLanguageByIdAsync(languageId={LanguageId})", languageId);
		logger.LogDebug("<<< Exiting GetLanguageByIdAsync(languageId={LanguageId})", languageId);
        return await _languageRepository.GetByIdAsync(languageId, cache => default);
    }

    /// <summary>
    /// Inserts a language
    /// </summary>
    /// <param name="language">Language</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertLanguageAsync(Language language)
    {
		logger.LogDebug(">>> Entering InsertLanguageAsync(language={Language})", language);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _languageRepository.InsertAsync(language);
		logger.LogDebug("<<< Exiting InsertLanguageAsync(language={Language})", language);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertLanguageAsync(language)={Language}: save query executed in {Arg1} ms", language, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertLanguageAsync(language)={Language}: save query executed in {Arg1} ms", language, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates a language
    /// </summary>
    /// <param name="language">Language</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateLanguageAsync(Language language)
    {
		logger.LogDebug(">>> Entering UpdateLanguageAsync(language={Language})", language);
        //update language
        await _languageRepository.UpdateAsync(language);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateLanguageAsync(language)={Language}: update query executed in {Arg1} ms", language, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting UpdateLanguageAsync(language={Language})", language);
    }

    /// <summary>
    /// Get 2 letter ISO language code
    /// </summary>
    /// <param name="language">Language</param>
    /// <returns>ISO language code</returns>
    public virtual string GetTwoLetterIsoLanguageName(Language language)
    {
		logger.LogDebug(">>> Entering GetTwoLetterIsoLanguageName(language={Language})", language);
        ArgumentNullException.ThrowIfNull(language);

        if (string.IsNullOrEmpty(language.LanguageCulture))
            return "en";

        var culture = new CultureInfo(language.LanguageCulture);
        var code = culture.TwoLetterISOLanguageName;
		logger.LogDebug("GetTwoLetterIsoLanguageName(language={Language}): culture → {Culture}", language, culture);

		logger.LogDebug("<<< Exiting GetTwoLetterIsoLanguageName(language={Language})", language);
        return string.IsNullOrEmpty(code) ? "en" : code;
    }

    #endregion
}
