using System.Globalization;
using Nop.Core.Caching;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Localization;

/// <summary>
/// Language service
/// </summary>
public partial class LanguageService : ILanguageService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogError(">>> Entering LanguageService(languageRepository={LanguageRepository},settingService={SettingService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,localizationSettings={LocalizationSettings})", languageRepository, settingService, staticCacheManager, localizationSettings);
        _languageRepository = languageRepository;
        _settingService = settingService;
		_logger.LogDebug("LanguageService(languageRepository={LanguageRepository},settingService={SettingService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,localizationSettings={LocalizationSettings}): _settingService → {_settingService}", languageRepository, settingService, staticCacheManager, localizationSettings, _settingService);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("LanguageService(languageRepository={LanguageRepository},settingService={SettingService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,localizationSettings={LocalizationSettings}): _staticCacheManager → {_staticCacheManager}", languageRepository, settingService, staticCacheManager, localizationSettings, _staticCacheManager);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("LanguageService(languageRepository={LanguageRepository},settingService={SettingService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,localizationSettings={LocalizationSettings}): _storeMappingService → {_storeMappingService}", languageRepository, settingService, staticCacheManager, localizationSettings, _storeMappingService);
        _localizationSettings = localizationSettings;
		_logger.LogDebug("LanguageService(languageRepository={LanguageRepository},settingService={SettingService},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,localizationSettings={LocalizationSettings}): _localizationSettings → {_localizationSettings}", languageRepository, settingService, staticCacheManager, localizationSettings, _localizationSettings);
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
		_logger.LogError(">>> Entering DeleteLanguageAsync(language={Language})", language);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(language);

        //update default admin area language (if required)
        if (_localizationSettings.DefaultAdminLanguageId == language.Id)
            foreach (var activeLanguage in await GetAllLanguagesAsync())
            {
                if (activeLanguage.Id == language.Id)
                    continue;

                _localizationSettings.DefaultAdminLanguageId = activeLanguage.Id;
				_logger.LogDebug("DeleteLanguageAsync(language={Language}): _localizationSettings.DefaultAdminLanguageId → {Arg1}", language, _localizationSettings.DefaultAdminLanguageId);
                await _settingService.SaveSettingAsync(_localizationSettings);
				_logger.LogError(">>> Entering GetAllLanguagesAsync(showHidden={ShowHidden},storeId={StoreId})", showHidden, storeId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
				var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
				_logger.LogError("GetAllLanguagesAsync(showHidden={ShowHidden},storeId={StoreId}): query → {Query}", showHidden, storeId);
				_logger.LogError("GetAllLanguagesAsync(showHidden,storeId)={ShowHidden},{StoreId}: find query executed in {Arg2} ms", showHidden, storeId, __logStartDb.ElapsedMilliseconds);
				_logger.LogWarning("<<< Exiting GetAllLanguagesAsync(showHidden={ShowHidden},storeId={StoreId})", showHidden, storeId);
				_logger.LogError("GetAllLanguagesAsync(showHidden,storeId)={ShowHidden},{StoreId}: external service call took {Arg2} ms", showHidden, storeId, __logStart.ElapsedMilliseconds);
                break;
            }

		_logger.LogError("DeleteLanguageAsync(language)={Language}: external service call took {Arg1} ms", language, __logStart.ElapsedMilliseconds);
		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
    _logger.LogWarning("<<< Exiting DeleteLanguageAsync(language={Language})", language);
        await _languageRepository.DeleteAsync(language);
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
		_logger.LogError(">>> Entering GetAllLanguagesAsync(showHidden={ShowHidden},storeId={StoreId})", showHidden, storeId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopLocalizationDefaults.LanguagesAllCacheKey, storeId, showHidden);

		_logger.LogError("GetAllLanguagesAsync(showHidden={ShowHidden},storeId={StoreId}): key → {Key}", showHidden, storeId, key);
        var languages = await _staticCacheManager.GetAsync(key, async () =>
        {
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
            var allLanguages = await _languageRepository.GetAllAsync(async query =>
            {
                if (!showHidden)
                    query = query.Where(l => l.Published);

				_logger.LogError("GetAllLanguagesAsync(showHidden={ShowHidden},storeId={StoreId}): query → {Query}", showHidden, storeId, query);
                if (storeId > 0)
                    query = await _storeMappingService.ApplyStoreMapping(query, storeId);

                query = query.OrderBy(l => l.DisplayOrder).ThenBy(l => l.Id);

                return query;
            });
			_logger.LogError("GetAllLanguagesAsync(showHidden,storeId)={ShowHidden},{StoreId}: find query executed in {Arg2} ms", showHidden, storeId, __logStartDb.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting GetAllLanguagesAsync(showHidden={ShowHidden},storeId={StoreId})", showHidden, storeId);
            return allLanguages;
        });

		_logger.LogError("GetAllLanguagesAsync(showHidden,storeId)={ShowHidden},{StoreId}: external service call took {Arg2} ms", showHidden, storeId, __logStart.ElapsedMilliseconds);
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
		_logger.LogError(">>> Entering GetAllLanguages(showHidden={ShowHidden},storeId={StoreId})", showHidden, storeId);
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopLocalizationDefaults.LanguagesAllCacheKey, storeId, showHidden);

		_logger.LogError("GetAllLanguages(showHidden={ShowHidden},storeId={StoreId}): key → {Key}", showHidden, storeId, key);
        var languages = _staticCacheManager.Get(key, () =>
        {
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            var allLanguages = _languageRepository.GetAll(query =>
            {
                if (!showHidden)
                    query = query.Where(l => l.Published);
                query = query.OrderBy(l => l.DisplayOrder).ThenBy(l => l.Id);
				_logger.LogError("GetAllLanguages(showHidden={ShowHidden},storeId={StoreId}): query → {Query}", showHidden, storeId, query);

                return query;
            });
			_logger.LogError("GetAllLanguages(showHidden,storeId)={ShowHidden},{StoreId}: find query executed in {Arg2} ms", showHidden, storeId, __logStart.ElapsedMilliseconds);

            //store mapping
            if (storeId > 0)
                allLanguages = allLanguages
                    .Where(l => _storeMappingService.Authorize(l, storeId))
                    .ToList();

		_logger.LogWarning("<<< Exiting GetAllLanguages(showHidden={ShowHidden},storeId={StoreId})", showHidden, storeId);
            return allLanguages;
        });

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
		_logger.LogError(">>> Entering GetLanguageByIdAsync(languageId={LanguageId})", languageId);
		_logger.LogError("GetLanguageByIdAsync(languageId)={LanguageId}: find query executing", languageId);
    _logger.LogWarning("<<< Exiting GetLanguageByIdAsync(languageId={LanguageId})", languageId);
        return await _languageRepository.GetByIdAsync(languageId, cache => default);
    }

    /// <summary>
    /// Inserts a language
    /// </summary>
    /// <param name="language">Language</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertLanguageAsync(Language language)
    {
		_logger.LogError(">>> Entering InsertLanguageAsync(language={Language})", language);
        await _languageRepository.InsertAsync(language);
		_logger.LogWarning("<<< Exiting InsertLanguageAsync(language={Language})", language);
    }

    /// <summary>
    /// Updates a language
    /// </summary>
    /// <param name="language">Language</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateLanguageAsync(Language language)
    {
		_logger.LogError(">>> Entering UpdateLanguageAsync(language={Language})", language);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //update language
        await _languageRepository.UpdateAsync(language);
		_logger.LogError("UpdateLanguageAsync(language)={Language}: update query executed in {Arg1} ms", language, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting UpdateLanguageAsync(language={Language})", language);
    }

    /// <summary>
    /// Get 2 letter ISO language code
    /// </summary>
    /// <param name="language">Language</param>
    /// <returns>ISO language code</returns>
    public virtual string GetTwoLetterIsoLanguageName(Language language)
    {
		_logger.LogError(">>> Entering GetTwoLetterIsoLanguageName(language={Language})", language);
        ArgumentNullException.ThrowIfNull(language);

        if (string.IsNullOrEmpty(language.LanguageCulture))
            return "en";

        var culture = new CultureInfo(language.LanguageCulture);
		_logger.LogError("GetTwoLetterIsoLanguageName(language={Language}): culture → {Culture}", language, culture);
        var code = culture.TwoLetterISOLanguageName;

		_logger.LogWarning("<<< Exiting GetTwoLetterIsoLanguageName(language={Language})", language);
        return string.IsNullOrEmpty(code) ? "en" : code;
    }

    #endregion
}