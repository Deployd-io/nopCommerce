using System.Linq.Expressions;
using System.Reflection;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Microsoft.Extensions.Logging;


namespace Nop.Services.Localization;

/// <summary>
/// Provides information about localizable entities
/// </summary>
public partial class LocalizedEntityService : ILocalizedEntityService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public LocalizedEntityService(IRepository<LocalizedProperty> localizedPropertyRepository,
        IStaticCacheManager staticCacheManager,
        LocalizationSettings localizationSettings)
    {
		_logger.LogError(">>> Entering LocalizedEntityService(localizedPropertyRepository={LocalizedPropertyRepository},staticCacheManager={StaticCacheManager},localizationSettings={LocalizationSettings})", localizedPropertyRepository, staticCacheManager, localizationSettings);
        _localizedPropertyRepository = localizedPropertyRepository;
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("LocalizedEntityService(localizedPropertyRepository={LocalizedPropertyRepository},staticCacheManager={StaticCacheManager},localizationSettings={LocalizationSettings}): _staticCacheManager → {_staticCacheManager}", localizedPropertyRepository, staticCacheManager, localizationSettings, _staticCacheManager);
        _localizationSettings = localizationSettings;
		_logger.LogDebug("LocalizedEntityService(localizedPropertyRepository={LocalizedPropertyRepository},staticCacheManager={StaticCacheManager},localizationSettings={LocalizationSettings}): _localizationSettings → {_localizationSettings}", localizedPropertyRepository, staticCacheManager, localizationSettings, _localizationSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets localized properties
    /// </summary>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="localeKeyGroup">Locale key group</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the localized properties
    /// </returns>
    protected virtual async Task<IList<LocalizedProperty>> GetLocalizedPropertiesAsync(int entityId, string localeKeyGroup)
    {
		_logger.LogError(">>> Entering GetLocalizedPropertiesAsync(entityId={EntityId},localeKeyGroup={LocaleKeyGroup})", entityId, localeKeyGroup);
        if (entityId == 0 || string.IsNullOrEmpty(localeKeyGroup))
            return new List<LocalizedProperty>();

        var query = from lp in _localizedPropertyRepository.Table
            orderby lp.Id
            where lp.EntityId == entityId &&
                  lp.LocaleKeyGroup == localeKeyGroup
            select lp;

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var props = await query.ToListAsync();

		_logger.LogError("GetLocalizedPropertiesAsync(entityId={EntityId},localeKeyGroup={LocaleKeyGroup}): props → {Props}", entityId, localeKeyGroup, props);
		_logger.LogError("GetLocalizedPropertiesAsync(entityId,localeKeyGroup)={EntityId},{LocaleKeyGroup}: database query executed in {Arg2} ms", entityId, localeKeyGroup, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetLocalizedPropertiesAsync(entityId={EntityId},localeKeyGroup={LocaleKeyGroup})", entityId, localeKeyGroup);
        return props;
    }

    /// <summary>
    /// Gets all cached localized properties
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cached localized properties
    /// </returns>
    protected virtual async Task<IList<LocalizedProperty>> GetAllLocalizedPropertiesAsync()
    {
		_logger.LogError(">>> Entering GetAllLocalizedPropertiesAsync()");
        return await _localizedPropertyRepository.GetAllAsync(query =>
        {
            return from lp in query
                select lp;
				_logger.LogError("GetAllLocalizedPropertiesAsync(): find query executing");
        }, cache => default);
		_logger.LogWarning("<<< Exiting GetAllLocalizedPropertiesAsync()");
    }

    /// <summary>
    /// Gets all cached localized properties by language
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the uncached localized properties
    /// </returns>
    protected virtual async Task<IList<LocalizedProperty>> GetAllLocalizedPropertiesAsync(int languageId)
    {
		_logger.LogError(">>> Entering GetAllLocalizedPropertiesAsync(languageId={LanguageId})", languageId);
        // do not cache here
        return await _localizedPropertyRepository.GetAllAsync(query => query.Where(lp => lp.LanguageId == languageId));
    }

    /// <summary>
    /// Deletes a localized property
    /// </summary>
    /// <param name="localizedProperty">Localized property</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task DeleteLocalizedPropertyAsync(LocalizedProperty localizedProperty)
    {
		_logger.LogError(">>> Entering DeleteLocalizedPropertyAsync(localizedProperty={LocalizedProperty})", localizedProperty);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _localizedPropertyRepository.DeleteAsync(localizedProperty);
		_logger.LogError("DeleteLocalizedPropertyAsync(localizedProperty)={LocalizedProperty}: external service call took {Arg1} ms", localizedProperty, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteLocalizedPropertyAsync(localizedProperty={LocalizedProperty})", localizedProperty);
    }

    /// <summary>
    /// Inserts a localized property
    /// </summary>
    /// <param name="localizedProperty">Localized property</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InsertLocalizedPropertyAsync(LocalizedProperty localizedProperty)
    {
		_logger.LogError(">>> Entering InsertLocalizedPropertyAsync(localizedProperty={LocalizedProperty})", localizedProperty);
        await _localizedPropertyRepository.InsertAsync(localizedProperty);
		_logger.LogWarning("<<< Exiting InsertLocalizedPropertyAsync(localizedProperty={LocalizedProperty})", localizedProperty);
    }

    /// <summary>
    /// Updates the localized property
    /// </summary>
    /// <param name="localizedProperty">Localized property</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task UpdateLocalizedPropertyAsync(LocalizedProperty localizedProperty)
    {
		_logger.LogError(">>> Entering UpdateLocalizedPropertyAsync(localizedProperty={LocalizedProperty})", localizedProperty);
        await _localizedPropertyRepository.UpdateAsync(localizedProperty);
		_logger.LogWarning("<<< Exiting UpdateLocalizedPropertyAsync(localizedProperty={LocalizedProperty})", localizedProperty);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Find localized properties
    /// </summary>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="localeKeyGroup">Locale key group</param>
    /// <param name="localeKey">Locale key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the found localized properties
    /// </returns>
    public virtual async Task<IList<LocalizedProperty>> GetEntityLocalizedPropertiesAsync(int entityId, string localeKeyGroup, string localeKey)
    {
		_logger.LogError(">>> Entering GetEntityLocalizedPropertiesAsync(entityId={EntityId},localeKeyGroup={LocaleKeyGroup},localeKey={LocaleKey})", entityId, localeKeyGroup, localeKey);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopLocalizationDefaults.LocalizedPropertiesCacheKey,
            entityId, localeKeyGroup, localeKey);

        return await _staticCacheManager.GetAsync(key, async () =>
        {
            var source = _localizationSettings.LoadAllLocalizedPropertiesOnStartup
                //load all records (we know they are cached)
                ? (await GetAllLocalizedPropertiesAsync()).AsQueryable()
                //gradual loading
                : _localizedPropertyRepository.Table;

            var query = from lp in source
                where lp.EntityId == entityId &&
                      lp.LocaleKeyGroup == localeKeyGroup &&
                      lp.LocaleKey == localeKey
                select lp;

			_logger.LogError("GetEntityLocalizedPropertiesAsync(entityId,localeKeyGroup,localeKey)={EntityId},{LocaleKeyGroup},{LocaleKey}: database query executing", entityId, localeKeyGroup, localeKey);
            return await query.ToListAsync();
        });
		_logger.LogError("GetEntityLocalizedPropertiesAsync(entityId,localeKeyGroup,localeKey)={EntityId},{LocaleKeyGroup},{LocaleKey}: external service call took {Arg3} ms", entityId, localeKeyGroup, localeKey, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetEntityLocalizedPropertiesAsync(entityId={EntityId},localeKeyGroup={LocaleKeyGroup},localeKey={LocaleKey})", entityId, localeKeyGroup, localeKey);
    }

    /// <summary>
    /// Find localized value
    /// </summary>
    /// <param name="languageId">Language identifier</param>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="localeKeyGroup">Locale key group</param>
    /// <param name="localeKey">Locale key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the found localized value
    /// </returns>
    public virtual async Task<string> GetLocalizedValueAsync(int languageId, int entityId, string localeKeyGroup, string localeKey)
    {
		_logger.LogError(">>> Entering GetLocalizedValueAsync(languageId={LanguageId},entityId={EntityId},localeKeyGroup={LocaleKeyGroup},localeKey={LocaleKey})", languageId, entityId, localeKeyGroup, localeKey);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (_localizationSettings.LoadAllLocalizedPropertiesOnStartup)
        {
            //value tuples aren't json-serializable by default, so we use a string key
            static string formatKey(string keyGroup, string key, int id) => $"{keyGroup}:{key}:{id}";

            var localizedValues = await _staticCacheManager.GetAsync(
                _staticCacheManager.PrepareKeyForDefaultCache(NopLocalizationDefaults.LocalizedPropertyLookupCacheKey, languageId),
                async () => (await GetAllLocalizedPropertiesAsync(languageId))
                    .GroupBy(p => formatKey(p.LocaleKeyGroup, p.LocaleKey, p.EntityId))
                    .ToDictionary(g => g.Key, g => g.First().LocaleValue));

            return localizedValues.TryGetValue(formatKey(localeKeyGroup, localeKey, entityId), out var localeValue)
                ? localeValue
                : string.Empty;
        }

        //gradual loading
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopLocalizationDefaults.LocalizedPropertyCacheKey
            , languageId, entityId, localeKeyGroup, localeKey);

        return await _staticCacheManager.GetAsync(key, async () =>
        {
            var query = from lp in _localizedPropertyRepository.Table
                where lp.LanguageId == languageId &&
                      lp.EntityId == entityId &&
                      lp.LocaleKeyGroup == localeKeyGroup &&
                      lp.LocaleKey == localeKey
                select lp.LocaleValue;

            //little hack here. nulls aren't cacheable so set it to ""
			_logger.LogError("GetLocalizedValueAsync(languageId,entityId,localeKeyGroup,localeKey)={LanguageId},{EntityId},{LocaleKeyGroup},{LocaleKey}: database query executing", languageId, entityId, localeKeyGroup, localeKey);
			_logger.LogError("GetLocalizedValueAsync(languageId,entityId,localeKeyGroup,localeKey)={LanguageId},{EntityId},{LocaleKeyGroup},{LocaleKey}: external service call took {Arg4} ms", languageId, entityId, localeKeyGroup, localeKey, __logStart.ElapsedMilliseconds);
            return await query.FirstOrDefaultAsync() ?? string.Empty;
        });
		_logger.LogError("GetLocalizedValueAsync(languageId,entityId,localeKeyGroup,localeKey)={LanguageId},{EntityId},{LocaleKeyGroup},{LocaleKey}: external service call took {Arg4} ms", languageId, entityId, localeKeyGroup, localeKey, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetLocalizedValueAsync(languageId={LanguageId},entityId={EntityId},localeKeyGroup={LocaleKeyGroup},localeKey={LocaleKey})", languageId, entityId, localeKeyGroup, localeKey);
    }

    /// <summary>
    /// Save localized value
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="localeValue">Locale value</param>
    /// <param name="languageId">Language ID</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SaveLocalizedValueAsync<T>(T entity,
        Expression<Func<T, string>> keySelector,
        string localeValue,
        int languageId) where T : BaseEntity, ILocalizedEntity
    {
        await SaveLocalizedValueAsync<T, string>(entity, keySelector, localeValue, languageId);
    }

    /// <summary>
    /// Save localized value
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="localeValue">Locale value</param>
    /// <param name="languageId">Language ID</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SaveLocalizedValueAsync<T, TPropType>(T entity,
        Expression<Func<T, TPropType>> keySelector,
        TPropType localeValue,
        int languageId) where T : BaseEntity, ILocalizedEntity
    {
		_logger.LogError(">>> Entering SaveLocalizedValueAsync(entity={Entity},keySelector={KeySelector},localeValue={LocaleValue},languageId={LanguageId})", entity, keySelector, localeValue, languageId);
        ArgumentNullException.ThrowIfNull(entity);

			_logger.LogWarning("<<< Exiting SaveLocalizedValueAsync(entity={Entity},keySelector={KeySelector},localeValue={LocaleValue},languageId={LanguageId})", entity, keySelector, localeValue, languageId);
			_logger.LogError("Throwing ArgumentOutOfRangeException from SaveLocalizedValueAsync(entity={Entity},keySelector={KeySelector},localeValue={LocaleValue},languageId={LanguageId})", entity, keySelector, localeValue, languageId);
        if (languageId == 0) {
            throw new ArgumentOutOfRangeException(nameof(languageId), "Language ID should not be 0");
        }

        if (keySelector.Body is not MemberExpression member)
        {
			_logger.LogError("Throwing ArgumentException from SaveLocalizedValueAsync(entity={Entity},keySelector={KeySelector},localeValue={LocaleValue},languageId={LanguageId})", entity, keySelector, localeValue, languageId);
            throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a method, not a property.",
                keySelector));
        }

        var propInfo = member.Member as PropertyInfo ?? throw new ArgumentException(string.Format(
            "Expression '{0}' refers to a field, not a property.",
            keySelector));

        //load localized value (check whether it's a cacheable entity. In such cases we load its original entity type)
        var localeKeyGroup = entity.GetType().Name;
		_logger.LogError("SaveLocalizedValueAsync(entity={Entity},keySelector={KeySelector},localeValue={LocaleValue},languageId={LanguageId}): localeKeyGroup → {LocaleKeyGroup}", entity, keySelector, localeValue, languageId, localeKeyGroup);
        var localeKey = propInfo.Name;

        var props = await GetLocalizedPropertiesAsync(entity.Id, localeKeyGroup);
		_logger.LogError("SaveLocalizedValueAsync(entity={Entity},keySelector={KeySelector},localeValue={LocaleValue},languageId={LanguageId}): props → {Props}", entity, keySelector, localeValue, languageId, props);
        var prop = props.FirstOrDefault(lp => lp.LanguageId == languageId &&
                                              lp.LocaleKey.Equals(localeKey, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

        var localeValueStr = CommonHelper.To<string>(localeValue);
		_logger.LogError("SaveLocalizedValueAsync(entity={Entity},keySelector={KeySelector},localeValue={LocaleValue},languageId={LanguageId}): localeValueStr → {LocaleValueStr}", entity, keySelector, localeValue, languageId, localeValueStr);

        if (prop != null)
        {
            if (string.IsNullOrWhiteSpace(localeValueStr))
            {
                //delete
                await DeleteLocalizedPropertyAsync(prop);
            }
            else
            {
                //update
                prop.LocaleValue = localeValueStr;
				_logger.LogDebug("SaveLocalizedValueAsync(entity={Entity},keySelector={KeySelector},localeValue={LocaleValue},languageId={LanguageId}): prop.LocaleValue → {Arg4}", entity, keySelector, localeValue, languageId, prop.LocaleValue);
                await UpdateLocalizedPropertyAsync(prop);
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(localeValueStr))
                return;

            //insert
            prop = new LocalizedProperty
            {
                EntityId = entity.Id,
                LanguageId = languageId,
                LocaleKey = localeKey,
                LocaleKeyGroup = localeKeyGroup,
                LocaleValue = localeValueStr
            };
            await InsertLocalizedPropertyAsync(prop);
        }
    }

    #endregion
}