using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Domain.Configuration;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Configuration;

/// <summary>
/// Setting manager
/// </summary>
public partial class SettingService : ISettingService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<Setting> _settingRepository;
    protected readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public SettingService(IRepository<Setting> settingRepository,
        IStaticCacheManager staticCacheManager)
    {
		_logger.LogError(">>> Entering SettingService(settingRepository={SettingRepository},staticCacheManager={StaticCacheManager})", settingRepository, staticCacheManager);
        _settingRepository = settingRepository;
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("SettingService(settingRepository={SettingRepository},staticCacheManager={StaticCacheManager}): _staticCacheManager → {_staticCacheManager}", settingRepository, staticCacheManager, _staticCacheManager);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets all settings
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the settings
    /// </returns>
    protected virtual async Task<IDictionary<string, IList<Setting>>> GetAllSettingsDictionaryAsync()
    {
        return await _staticCacheManager.GetAsync(NopSettingsDefaults.SettingsAllAsDictionaryCacheKey, async () =>
        {
            var settings = await GetAllSettingsAsync();

            var dictionary = new Dictionary<string, IList<Setting>>();
            foreach (var s in settings)
            {
                var resourceName = s.Name.ToLowerInvariant();
                var settingForCaching = new Setting
                {
                    Id = s.Id,
                    Name = s.Name,
                    Value = s.Value,
                    StoreId = s.StoreId
                };
                if (!dictionary.TryGetValue(resourceName, out var value))
                    //first setting
                    dictionary.Add(resourceName, new List<Setting>
                    {
                        settingForCaching
                    });
                else
                    //already added
                    //most probably it's the setting with the same name but for some certain store (storeId > 0)
                    value.Add(settingForCaching);
					_logger.LogError(">>> Entering GetAllSettingsAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
					_logger.LogError("GetAllSettingsAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
					_logger.LogWarning("<<< Exiting GetAllSettingsAsync()");
            }

            return dictionary;
        });
    }

    /// <summary>
    /// Gets all settings
    /// </summary>
    /// <returns>
    /// Settings
    /// </returns>
    protected virtual IDictionary<string, IList<Setting>> GetAllSettingsDictionary()
    {
        return _staticCacheManager.Get(NopSettingsDefaults.SettingsAllAsDictionaryCacheKey, () =>
        {
            var settings = GetAllSettings();

            var dictionary = new Dictionary<string, IList<Setting>>();
            foreach (var s in settings)
            {
                var resourceName = s.Name.ToLowerInvariant();
                var settingForCaching = new Setting
                {
                    Id = s.Id,
                    Name = s.Name,
                    Value = s.Value,
                    StoreId = s.StoreId
                };
                if (!dictionary.TryGetValue(resourceName, out var value))
                    //first setting
                    dictionary.Add(resourceName, new List<Setting>
                    {
                        settingForCaching
                    });
                else
                    //already added
                    //most probably it's the setting with the same name but for some certain store (storeId > 0)
                    value.Add(settingForCaching);
            }

            return dictionary;
        });
    }

    /// <summary>
    /// Set setting value
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SetSettingAsync(Type type, string key, object value, int storeId = 0, bool clearCache = true)
    {
		_logger.LogError(">>> Entering SetSettingAsync(type={Type},key={Key},value={Value},storeId={StoreId},clearCache={ClearCache})", type, key, value, storeId, clearCache);
        ArgumentNullException.ThrowIfNull(key);
        key = key.Trim().ToLowerInvariant();
		_logger.LogError("SetSettingAsync(type={Type},key={Key},value={Value},storeId={StoreId},clearCache={ClearCache}): key → {Key1}", type, key, value, storeId, clearCache, key);
        var valueStr = TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);
		_logger.LogError("SetSettingAsync(type={Type},key={Key},value={Value},storeId={StoreId},clearCache={ClearCache}): valueStr → {ValueStr}", type, key, value, storeId, clearCache, valueStr);

        var allSettings = await GetAllSettingsDictionaryAsync();
		_logger.LogError("SetSettingAsync(type={Type},key={Key},value={Value},storeId={StoreId},clearCache={ClearCache}): allSettings → {AllSettings}", type, key, value, storeId, clearCache, allSettings);
        var settingForCaching = allSettings.TryGetValue(key, out var settings) ?
            settings.FirstOrDefault(x => x.StoreId == storeId) : null;
        if (settingForCaching != null)
        {
            //update
            var setting = await GetSettingByIdAsync(settingForCaching.Id);
			_logger.LogError("SetSettingAsync(type={Type},key={Key},value={Value},storeId={StoreId},clearCache={ClearCache}): setting → {Setting}", type, key, value, storeId, clearCache, setting);
            setting.Value = valueStr;
			_logger.LogDebug("SetSettingAsync(type={Type},key={Key},value={Value},storeId={StoreId},clearCache={ClearCache}): setting.Value → {Arg5}", type, key, value, storeId, clearCache, setting.Value);
            await UpdateSettingAsync(setting, clearCache);
        }
        else
        {
            //insert
            var setting = new Setting
            {
                Name = key,
                Value = valueStr,
                StoreId = storeId
            };
            await InsertSettingAsync(setting, clearCache);
        }
		_logger.LogError(">>> Entering SetSettingAsync(key={Key},value={Value},storeId={StoreId},clearCache={ClearCache})", key, value, storeId, clearCache);
		_logger.LogWarning("<<< Exiting SetSettingAsync(key={Key},value={Value},storeId={StoreId},clearCache={ClearCache})", key, value, storeId, clearCache);
    }

    /// <summary>
    /// Set setting value
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
    protected virtual void SetSetting(Type type, string key, object value, int storeId = 0, bool clearCache = true)
    {
		_logger.LogError(">>> Entering SetSetting(type={Type},key={Key},value={Value},storeId={StoreId},clearCache={ClearCache})", type, key, value, storeId, clearCache);
        ArgumentNullException.ThrowIfNull(key);
        key = key.Trim().ToLowerInvariant();
		_logger.LogError("SetSetting(type={Type},key={Key},value={Value},storeId={StoreId},clearCache={ClearCache}): key → {Key1}", type, key, value, storeId, clearCache, key);
        var valueStr = TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);
		_logger.LogError("SetSetting(type={Type},key={Key},value={Value},storeId={StoreId},clearCache={ClearCache}): valueStr → {ValueStr}", type, key, value, storeId, clearCache, valueStr);

        var allSettings = GetAllSettingsDictionary();
		_logger.LogError("SetSetting(type={Type},key={Key},value={Value},storeId={StoreId},clearCache={ClearCache}): allSettings → {AllSettings}", type, key, value, storeId, clearCache, allSettings);
        var settingForCaching = allSettings.TryGetValue(key, out var settings) ?
            settings.FirstOrDefault(x => x.StoreId == storeId) : null;
        if (settingForCaching != null)
        {
            //update
            var setting = GetSettingById(settingForCaching.Id);
			_logger.LogError("SetSetting(type={Type},key={Key},value={Value},storeId={StoreId},clearCache={ClearCache}): setting → {Setting}", type, key, value, storeId, clearCache, setting);
            setting.Value = valueStr;
			_logger.LogDebug("SetSetting(type={Type},key={Key},value={Value},storeId={StoreId},clearCache={ClearCache}): setting.Value → {Arg5}", type, key, value, storeId, clearCache, setting.Value);
            UpdateSetting(setting, clearCache);
        }
        else
        {
            //insert
            var setting = new Setting
            {
                Name = key,
                Value = valueStr,
                StoreId = storeId
            };
            InsertSetting(setting, clearCache);
        }
		_logger.LogWarning("<<< Exiting SetSetting(type={Type},key={Key},value={Value},storeId={StoreId},clearCache={ClearCache})", type, key, value, storeId, clearCache);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds a setting
    /// </summary>
    /// <param name="setting">Setting</param>
    /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertSettingAsync(Setting setting, bool clearCache = true)
    {
		_logger.LogError(">>> Entering InsertSettingAsync(setting={Setting},clearCache={ClearCache})", setting, clearCache);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _settingRepository.InsertAsync(setting);

		_logger.LogError("InsertSettingAsync(setting,clearCache)={Setting},{ClearCache}: save query executed in {Arg2} ms", setting, clearCache, __logStart.ElapsedMilliseconds);
        //cache
        if (clearCache)
            await ClearCacheAsync();
			_logger.LogWarning("<<< Exiting InsertSettingAsync(setting={Setting},clearCache={ClearCache})", setting, clearCache);
    }

    /// <summary>
    /// Adds a setting
    /// </summary>
    /// <param name="setting">Setting</param>
    /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
    public virtual void InsertSetting(Setting setting, bool clearCache = true)
    {
		_logger.LogError(">>> Entering InsertSetting(setting={Setting},clearCache={ClearCache})", setting, clearCache);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        _settingRepository.Insert(setting);

		_logger.LogError("InsertSetting(setting,clearCache)={Setting},{ClearCache}: save query executed in {Arg2} ms", setting, clearCache, __logStart.ElapsedMilliseconds);
        //cache
        if (clearCache)
            ClearCache();
			_logger.LogWarning("<<< Exiting InsertSetting(setting={Setting},clearCache={ClearCache})", setting, clearCache);
    }

    /// <summary>
    /// Updates a setting
    /// </summary>
    /// <param name="setting">Setting</param>
    /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateSettingAsync(Setting setting, bool clearCache = true)
    {
		_logger.LogError(">>> Entering UpdateSettingAsync(setting={Setting},clearCache={ClearCache})", setting, clearCache);
        ArgumentNullException.ThrowIfNull(setting);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("UpdateSettingAsync(setting,clearCache)={Setting},{ClearCache}: update query executed in {Arg2} ms", setting, clearCache, __logStart.ElapsedMilliseconds);
        await _settingRepository.UpdateAsync(setting);

        //cache
        if (clearCache)
            await ClearCacheAsync();
			_logger.LogWarning("<<< Exiting UpdateSettingAsync(setting={Setting},clearCache={ClearCache})", setting, clearCache);
    }

    /// <summary>
    /// Updates a setting
    /// </summary>
    /// <param name="setting">Setting</param>
    /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
    public virtual void UpdateSetting(Setting setting, bool clearCache = true)
    {
		_logger.LogError(">>> Entering UpdateSetting(setting={Setting},clearCache={ClearCache})", setting, clearCache);
        ArgumentNullException.ThrowIfNull(setting);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("UpdateSetting(setting,clearCache)={Setting},{ClearCache}: update query executed in {Arg2} ms", setting, clearCache, __logStart.ElapsedMilliseconds);
        _settingRepository.Update(setting);

        //cache
        if (clearCache)
            ClearCache();
			_logger.LogWarning("<<< Exiting UpdateSetting(setting={Setting},clearCache={ClearCache})", setting, clearCache);
    }

    /// <summary>
    /// Deletes a setting
    /// </summary>
    /// <param name="setting">Setting</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteSettingAsync(Setting setting)
    {
		_logger.LogError(">>> Entering DeleteSettingAsync(setting={Setting})", setting);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _settingRepository.DeleteAsync(setting);

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("DeleteSettingAsync(setting)={Setting}: external service call took {Arg1} ms", setting, __logStart.ElapsedMilliseconds);
		_logger.LogError("DeleteSettingAsync(setting)={Setting}: delete query executed in {Arg1} ms", setting, __logStartDb.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteSettingAsync(setting={Setting})", setting);
        //cache
        await ClearCacheAsync();
    }

    /// <summary>
    /// Deletes a setting
    /// </summary>
    /// <param name="setting">Setting</param>
    public virtual void DeleteSetting(Setting setting)
    {
		_logger.LogError(">>> Entering DeleteSetting(setting={Setting})", setting);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        _settingRepository.Delete(setting);

		_logger.LogError("DeleteSetting(setting)={Setting}: delete query executed in {Arg1} ms", setting, __logStart.ElapsedMilliseconds);
        //cache
        ClearCache();
		_logger.LogWarning("<<< Exiting DeleteSetting(setting={Setting})", setting);
    }

    /// <summary>
    /// Deletes settings
    /// </summary>
    /// <param name="settings">Settings</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteSettingsAsync(IList<Setting> settings)
    {
		_logger.LogError(">>> Entering DeleteSettingsAsync(settings={Settings})", settings);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _settingRepository.DeleteAsync(settings);

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("DeleteSettingsAsync(settings)={Settings}: external service call took {Arg1} ms", settings, __logStart.ElapsedMilliseconds);
		_logger.LogError("DeleteSettingsAsync(settings)={Settings}: delete query executed in {Arg1} ms", settings, __logStartDb.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteSettingsAsync(settings={Settings})", settings);
        //cache
        await ClearCacheAsync();
    }

    /// <summary>
    /// Gets a setting by identifier
    /// </summary>
    /// <param name="settingId">Setting identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the setting
    /// </returns>
    public virtual async Task<Setting> GetSettingByIdAsync(int settingId)
    {
		_logger.LogError(">>> Entering GetSettingByIdAsync(settingId={SettingId})", settingId);
		_logger.LogError("GetSettingByIdAsync(settingId)={SettingId}: find query executing", settingId);
    _logger.LogWarning("<<< Exiting GetSettingByIdAsync(settingId={SettingId})", settingId);
		_logger.LogError(">>> Entering ClearCacheAsync()");
		_logger.LogWarning("<<< Exiting ClearCacheAsync()");
        return await _settingRepository.GetByIdAsync(settingId, cache => default);
    }

    /// <summary>
    /// Gets a setting by identifier
    /// </summary>
    /// <param name="settingId">Setting identifier</param>
    /// <returns>
    /// The setting
    /// </returns>
    public virtual Setting GetSettingById(int settingId)
    {
		_logger.LogError(">>> Entering GetSettingById(settingId={SettingId})", settingId);
		_logger.LogError("GetSettingById(settingId)={SettingId}: find query executing", settingId);
    _logger.LogWarning("<<< Exiting GetSettingById(settingId={SettingId})", settingId);
        return _settingRepository.GetById(settingId, cache => default);
    }

    /// <summary>
    /// Get setting by key
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the setting
    /// </returns>
    public virtual async Task<Setting> GetSettingAsync(string key, int storeId = 0, bool loadSharedValueIfNotFound = false)
    {
		_logger.LogError(">>> Entering GetSettingAsync(key={Key},storeId={StoreId},loadSharedValueIfNotFound={LoadSharedValueIfNotFound})", key, storeId, loadSharedValueIfNotFound);
        if (string.IsNullOrEmpty(key))
            return null;

        var settings = await GetAllSettingsDictionaryAsync();
		_logger.LogError("GetSettingAsync(key={Key},storeId={StoreId},loadSharedValueIfNotFound={LoadSharedValueIfNotFound}): settings → {Settings}", key, storeId, loadSharedValueIfNotFound, settings);
        key = key.Trim().ToLowerInvariant();
		_logger.LogError("GetSettingAsync(key={Key},storeId={StoreId},loadSharedValueIfNotFound={LoadSharedValueIfNotFound}): key → {Key1}", key, storeId, loadSharedValueIfNotFound, key);
        if (!settings.TryGetValue(key, out var value))
            return null;

        var settingsByKey = value;
        var setting = settingsByKey.FirstOrDefault(x => x.StoreId == storeId);
		_logger.LogError("GetSettingAsync(key={Key},storeId={StoreId},loadSharedValueIfNotFound={LoadSharedValueIfNotFound}): setting → {Setting}", key, storeId, loadSharedValueIfNotFound, setting);

        //load shared value?
        if (setting == null && storeId > 0 && loadSharedValueIfNotFound)
            setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);

		_logger.LogWarning("<<< Exiting GetSettingAsync(key={Key},storeId={StoreId},loadSharedValueIfNotFound={LoadSharedValueIfNotFound})", key, storeId, loadSharedValueIfNotFound);
        return setting;
    }

    /// <summary>
    /// Get setting by key
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
    /// <returns>
    /// The setting
    /// </returns>
    public virtual Setting GetSetting(string key, int storeId = 0, bool loadSharedValueIfNotFound = false)
    {
		_logger.LogError(">>> Entering GetSetting(key={Key},storeId={StoreId},loadSharedValueIfNotFound={LoadSharedValueIfNotFound})", key, storeId, loadSharedValueIfNotFound);
        if (string.IsNullOrEmpty(key))
            return null;

        var settings = GetAllSettingsDictionary();
		_logger.LogError("GetSetting(key={Key},storeId={StoreId},loadSharedValueIfNotFound={LoadSharedValueIfNotFound}): settings → {Settings}", key, storeId, loadSharedValueIfNotFound, settings);
        key = key.Trim().ToLowerInvariant();
		_logger.LogError("GetSetting(key={Key},storeId={StoreId},loadSharedValueIfNotFound={LoadSharedValueIfNotFound}): key → {Key1}", key, storeId, loadSharedValueIfNotFound, key);
        if (!settings.TryGetValue(key, out var value))
            return null;

        var settingsByKey = value;
        var setting = settingsByKey.FirstOrDefault(x => x.StoreId == storeId);
		_logger.LogError("GetSetting(key={Key},storeId={StoreId},loadSharedValueIfNotFound={LoadSharedValueIfNotFound}): setting → {Setting}", key, storeId, loadSharedValueIfNotFound, setting);

        //load shared value?
        if (setting == null && storeId > 0 && loadSharedValueIfNotFound)
            setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);

		_logger.LogWarning("<<< Exiting GetSetting(key={Key},storeId={StoreId},loadSharedValueIfNotFound={LoadSharedValueIfNotFound})", key, storeId, loadSharedValueIfNotFound);
        return setting;
    }

    /// <summary>
    /// Get setting value by key
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="key">Key</param>
    /// <param name="defaultValue">Default value</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the setting value
    /// </returns>
    public virtual async Task<T> GetSettingByKeyAsync<T>(string key, T defaultValue = default,
        int storeId = 0, bool loadSharedValueIfNotFound = false)
    {
        if (string.IsNullOrEmpty(key))
            return defaultValue;

        var settings = await GetAllSettingsDictionaryAsync();
        key = key.Trim().ToLowerInvariant();
		_logger.LogError("GetSettingByKeyAsync(key={Key},defaultValue={DefaultValue},storeId={StoreId},loadSharedValueIfNotFound={LoadSharedValueIfNotFound}): key → {Key1}", key, defaultValue, storeId, loadSharedValueIfNotFound, key);
        if (!settings.TryGetValue(key, out var value))
            return defaultValue;

        var settingsByKey = value;
        var setting = settingsByKey.FirstOrDefault(x => x.StoreId == storeId);
		_logger.LogError("GetSettingByKeyAsync(key={Key},defaultValue={DefaultValue},storeId={StoreId},loadSharedValueIfNotFound={LoadSharedValueIfNotFound}): setting → {Setting}", key, defaultValue, storeId, loadSharedValueIfNotFound, setting);

        //load shared value?
        if (setting == null && storeId > 0 && loadSharedValueIfNotFound)
            setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);

		_logger.LogWarning("<<< Exiting GetSettingByKeyAsync(key={Key},defaultValue={DefaultValue},storeId={StoreId},loadSharedValueIfNotFound={LoadSharedValueIfNotFound})", key, defaultValue, storeId, loadSharedValueIfNotFound);
        return setting != null ? CommonHelper.To<T>(setting.Value) : defaultValue;
    }

    /// <summary>
    /// Get setting value by key
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="key">Key</param>
    /// <param name="defaultValue">Default value</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
    /// <returns>
    /// Setting value
    /// </returns>
    public virtual T GetSettingByKey<T>(string key, T defaultValue = default,
        int storeId = 0, bool loadSharedValueIfNotFound = false)
    {
        if (string.IsNullOrEmpty(key))
            return defaultValue;

        var settings = GetAllSettingsDictionary();
        key = key.Trim().ToLowerInvariant();
		_logger.LogError("GetSettingByKey(key={Key},defaultValue={DefaultValue},storeId={StoreId},loadSharedValueIfNotFound={LoadSharedValueIfNotFound}): key → {Key1}", key, defaultValue, storeId, loadSharedValueIfNotFound, key);
        if (!settings.TryGetValue(key, out var value))
            return defaultValue;

        var settingsByKey = value;
        var setting = settingsByKey.FirstOrDefault(x => x.StoreId == storeId);
		_logger.LogError("GetSettingByKey(key={Key},defaultValue={DefaultValue},storeId={StoreId},loadSharedValueIfNotFound={LoadSharedValueIfNotFound}): setting → {Setting}", key, defaultValue, storeId, loadSharedValueIfNotFound, setting);

        //load shared value?
        if (setting == null && storeId > 0 && loadSharedValueIfNotFound)
            setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);

		_logger.LogWarning("<<< Exiting GetSettingByKey(key={Key},defaultValue={DefaultValue},storeId={StoreId},loadSharedValueIfNotFound={LoadSharedValueIfNotFound})", key, defaultValue, storeId, loadSharedValueIfNotFound);
        return setting != null ? CommonHelper.To<T>(setting.Value) : defaultValue;
    }

    /// <summary>
    /// Set setting value
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetSettingAsync<T>(string key, T value, int storeId = 0, bool clearCache = true)
    {
        await SetSettingAsync(typeof(T), key, value, storeId, clearCache);
    }

    /// <summary>
    /// Set setting value
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
    public virtual void SetSetting<T>(string key, T value, int storeId = 0, bool clearCache = true)
    {
        SetSetting(typeof(T), key, value, storeId, clearCache);
    }

    /// <summary>
    /// Gets all settings
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the settings
    /// </returns>
    public virtual async Task<IList<Setting>> GetAllSettingsAsync()
    {
		_logger.LogError(">>> Entering GetAllSettingsAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var settings = await _settingRepository.GetAllAsync(query =>
        {
            return from s in query
                orderby s.Name, s.StoreId
                select s;
        }, cache => default);
		_logger.LogError("GetAllSettingsAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting GetAllSettingsAsync()");
        return settings;
    }

    /// <summary>
    /// Gets all settings
    /// </summary>
    /// <returns>
    /// Settings
    /// </returns>
    public virtual IList<Setting> GetAllSettings()
    {
		_logger.LogError(">>> Entering GetAllSettings()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var settings = _settingRepository.GetAll(query => from s in query
            orderby s.Name, s.StoreId
            select s, cache => default);
			_logger.LogError("GetAllSettings(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting GetAllSettings()");
        return settings;
    }

    /// <summary>
    /// Determines whether a setting exists
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="settings">Entity</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the true -setting exists; false - does not exist
    /// </returns>
    public virtual async Task<bool> SettingExistsAsync<T, TPropType>(T settings,
        Expression<Func<T, TPropType>> keySelector, int storeId = 0)
        where T : ISettings, new()
    {
		_logger.LogError(">>> Entering SettingExistsAsync(settings={Settings},keySelector={KeySelector},storeId={StoreId})", settings, keySelector, storeId);
        var key = GetSettingKey(settings, keySelector);

		_logger.LogError("SettingExistsAsync(settings={Settings},keySelector={KeySelector},storeId={StoreId}): key → {Key}", settings, keySelector, storeId, key);
        var setting = await GetSettingByKeyAsync<string>(key, storeId: storeId);
		_logger.LogError("SettingExistsAsync(settings={Settings},keySelector={KeySelector},storeId={StoreId}): setting → {Setting}", settings, keySelector, storeId, setting);
		_logger.LogWarning("<<< Exiting SettingExistsAsync(settings={Settings},keySelector={KeySelector},storeId={StoreId})", settings, keySelector, storeId);
        return setting != null;
    }

    /// <summary>
    /// Determines whether a setting exists
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="settings">Entity</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// The true -setting exists; false - does not exist
    /// </returns>
    public virtual bool SettingExists<T, TPropType>(T settings,
        Expression<Func<T, TPropType>> keySelector, int storeId = 0)
        where T : ISettings, new()
    {
        var key = GetSettingKey(settings, keySelector);

        var setting = GetSettingByKey<string>(key, storeId: storeId);
        return setting != null;
    }

    /// <summary>
    /// Load settings
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="storeId">Store identifier for which settings should be loaded</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<T> LoadSettingAsync<T>(int storeId = 0) where T : ISettings, new()
    {
		_logger.LogError(">>> Entering LoadSettingAsync(storeId={StoreId})", storeId);
        _logger.LogWarning("<<< Exiting LoadSettingAsync(storeId={StoreId})", storeId);
        return (T)await LoadSettingAsync(typeof(T), storeId);
    }

    /// <summary>
    /// Load settings
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="storeId">Store identifier for which settings should be loaded</param>
    public virtual T LoadSetting<T>(int storeId = 0) where T : ISettings, new()
    {
		_logger.LogError(">>> Entering LoadSetting(storeId={StoreId})", storeId);
        _logger.LogWarning("<<< Exiting LoadSetting(storeId={StoreId})", storeId);
        return (T)LoadSetting(typeof(T), storeId);
    }

    /// <summary>
    /// Load settings
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="storeId">Store identifier for which settings should be loaded</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<ISettings> LoadSettingAsync(Type type, int storeId = 0)
    {
		_logger.LogError(">>> Entering LoadSettingAsync(type={Type},storeId={StoreId})", type, storeId);
        var settings = Activator.CreateInstance(type);

		_logger.LogError("LoadSettingAsync(type={Type},storeId={StoreId}): settings → {Settings}", type, storeId, settings);
        if (!DataSettingsManager.IsDatabaseInstalled())
            return settings as ISettings;

        foreach (var prop in type.GetProperties())
        {
            // get properties we can read and write to
            if (!prop.CanRead || !prop.CanWrite)
                continue;

            var key = type.Name + "." + prop.Name;
			_logger.LogError("LoadSettingAsync(type={Type},storeId={StoreId}): key → {Key}", type, storeId, key);
            //load by store
            var setting = await GetSettingByKeyAsync<string>(key, storeId: storeId, loadSharedValueIfNotFound: true);
			_logger.LogError("LoadSettingAsync(type={Type},storeId={StoreId}): setting → {Setting}", type, storeId, setting);
            if (setting == null)
                continue;

            if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                continue;

            if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
                continue;

            var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);
			_logger.LogError("LoadSettingAsync(type={Type},storeId={StoreId}): value → {Value}", type, storeId, value);

            //set property
            prop.SetValue(settings, value, null);
        }

		_logger.LogWarning("<<< Exiting LoadSettingAsync(type={Type},storeId={StoreId})", type, storeId);
        return settings as ISettings;
    }

    /// <summary>
    /// Load settings
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="storeId">Store identifier for which settings should be loaded</param>
    /// <returns>Settings</returns>
    public virtual ISettings LoadSetting(Type type, int storeId = 0)
    {
        var settings = Activator.CreateInstance(type);

		_logger.LogError("LoadSetting(type={Type},storeId={StoreId}): settings → {Settings}", type, storeId, settings);
        if (!DataSettingsManager.IsDatabaseInstalled())
            return settings as ISettings;

        foreach (var prop in type.GetProperties())
        {
            // get properties we can read and write to
            if (!prop.CanRead || !prop.CanWrite)
                continue;

            var key = type.Name + "." + prop.Name;
			_logger.LogError("LoadSetting(type={Type},storeId={StoreId}): key → {Key}", type, storeId, key);
            //load by store
            var setting = GetSettingByKey<string>(key, storeId: storeId, loadSharedValueIfNotFound: true);
			_logger.LogError("LoadSetting(type={Type},storeId={StoreId}): setting → {Setting}", type, storeId, setting);
            if (setting == null)
                continue;

            if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                continue;

            if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
                continue;

            var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);
			_logger.LogError("LoadSetting(type={Type},storeId={StoreId}): value → {Value}", type, storeId, value);

            //set property
            prop.SetValue(settings, value, null);
        }

        return settings as ISettings;
    }

    /// <summary>
    /// Save settings object
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="storeId">Store identifier</param>
    /// <param name="settings">Setting instance</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SaveSettingAsync<T>(T settings, int storeId = 0) where T : ISettings, new()
    {
		_logger.LogError(">>> Entering SaveSettingAsync(settings={Settings},storeId={StoreId})", settings, storeId);
        /* We do not clear cache after each setting update.
         * This behavior can increase performance because cached settings will not be cleared 
         * and loaded from database after each update */
        foreach (var prop in typeof(T).GetProperties())
        {
            // get properties we can read and write to
            if (!prop.CanRead || !prop.CanWrite)
                continue;

            if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                continue;

            var key = typeof(T).Name + "." + prop.Name;
			_logger.LogError("SaveSettingAsync(settings={Settings},storeId={StoreId}): key → {Key}", settings, storeId, key);
            var value = prop.GetValue(settings, null);
			_logger.LogError("SaveSettingAsync(settings={Settings},storeId={StoreId}): value → {Value}", settings, storeId, value);
            if (value != null)
                await SetSettingAsync(prop.PropertyType, key, value, storeId, false);
            else
                await SetSettingAsync(key, string.Empty, storeId, false);
        }

        //and now clear cache
        await ClearCacheAsync();
		_logger.LogWarning("<<< Exiting SaveSettingAsync(settings={Settings},storeId={StoreId})", settings, storeId);
    }

    /// <summary>
    /// Save settings object
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="storeId">Store identifier</param>
    /// <param name="settings">Setting instance</param>
    public virtual void SaveSetting<T>(T settings, int storeId = 0) where T : ISettings, new()
    {
        /* We do not clear cache after each setting update.
         * This behavior can increase performance because cached settings will not be cleared 
         * and loaded from database after each update */
        foreach (var prop in typeof(T).GetProperties())
        {
            // get properties we can read and write to
            if (!prop.CanRead || !prop.CanWrite)
                continue;

            if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                continue;

            var key = typeof(T).Name + "." + prop.Name;
            var value = prop.GetValue(settings, null);
            if (value != null)
                SetSetting(prop.PropertyType, key, value, storeId, false);
            else
                SetSetting(key, string.Empty, storeId, false);
        }

        //and now clear cache
        ClearCache();
    }

    /// <summary>
    /// Save settings object
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="settings">Settings</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="storeId">Store ID</param>
    /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SaveSettingAsync<T, TPropType>(T settings,
        Expression<Func<T, TPropType>> keySelector,
        int storeId = 0, bool clearCache = true) where T : ISettings, new()
    {
			_logger.LogError("Throwing ArgumentException from SaveSettingAsync(settings={Settings},keySelector={KeySelector},storeId={StoreId},clearCache={ClearCache})", settings, keySelector, storeId, clearCache);
        if (keySelector.Body is not MemberExpression member) {
            throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");
        }

        var propInfo = member.Member as PropertyInfo
                       ?? throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

        var key = GetSettingKey(settings, keySelector);
		_logger.LogError("SaveSettingAsync(settings={Settings},keySelector={KeySelector},storeId={StoreId},clearCache={ClearCache}): key → {Key}", settings, keySelector, storeId, clearCache, key);
        var value = (TPropType)propInfo.GetValue(settings, null);
		_logger.LogError("SaveSettingAsync(settings={Settings},keySelector={KeySelector},storeId={StoreId},clearCache={ClearCache}): value → {Value}", settings, keySelector, storeId, clearCache, value);
        if (value != null)
            await SetSettingAsync(key, value, storeId, clearCache);
        else
            await SetSettingAsync(key, string.Empty, storeId, clearCache);
    }

    /// <summary>
    /// Save settings object
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="settings">Settings</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="storeId">Store ID</param>
    /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
    public virtual void SaveSetting<T, TPropType>(T settings,
        Expression<Func<T, TPropType>> keySelector,
        int storeId = 0, bool clearCache = true) where T : ISettings, new()
    {
        if (keySelector.Body is not MemberExpression member)
            throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

        var propInfo = member.Member as PropertyInfo
                       ?? throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

        var key = GetSettingKey(settings, keySelector);
        var value = (TPropType)propInfo.GetValue(settings, null);
        if (value != null)
            SetSetting(key, value, storeId, clearCache);
        else
            SetSetting(key, string.Empty, storeId, clearCache);
    }

    /// <summary>
    /// Save settings object (per store). If the setting is not overridden per store then it'll be delete
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="settings">Settings</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="overrideForStore">A value indicating whether to setting is overridden in some store</param>
    /// <param name="storeId">Store ID</param>
    /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SaveSettingOverridablePerStoreAsync<T, TPropType>(T settings,
        Expression<Func<T, TPropType>> keySelector,
        bool overrideForStore, int storeId = 0, bool clearCache = true) where T : ISettings, new()
    {
		_logger.LogError(">>> Entering SaveSettingOverridablePerStoreAsync(settings={Settings},keySelector={KeySelector},overrideForStore={OverrideForStore},storeId={StoreId},clearCache={ClearCache})", settings, keySelector, overrideForStore, storeId, clearCache);
        if (overrideForStore || storeId == 0)
            await SaveSettingAsync(settings, keySelector, storeId, clearCache);
        else if (storeId > 0)
            await DeleteSettingAsync(settings, keySelector, storeId);
			_logger.LogWarning("<<< Exiting SaveSettingOverridablePerStoreAsync(settings={Settings},keySelector={KeySelector},overrideForStore={OverrideForStore},storeId={StoreId},clearCache={ClearCache})", settings, keySelector, overrideForStore, storeId, clearCache);
    }

    /// <summary>
    /// Delete all settings
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteSettingAsync<T>() where T : ISettings, new()
    {
        var settingsToDelete = new List<Setting>();
        var allSettings = await GetAllSettingsAsync();
		_logger.LogError("DeleteSettingAsync(): settingsToDelete → {SettingsToDelete}", settingsToDelete);
        foreach (var prop in typeof(T).GetProperties())
        {
			_logger.LogError("DeleteSettingAsync(): allSettings → {AllSettings}", allSettings);
            var key = typeof(T).Name + "." + prop.Name;
			_logger.LogError("DeleteSettingAsync(): key → {Key}", key);
            settingsToDelete.AddRange(allSettings.Where(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)));
        }

        await DeleteSettingsAsync(settingsToDelete);
    }

    /// <summary>
    /// Delete settings object
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="settings">Settings</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="storeId">Store ID</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteSettingAsync<T, TPropType>(T settings,
        Expression<Func<T, TPropType>> keySelector, int storeId = 0) where T : ISettings, new()
    {
        var key = GetSettingKey(settings, keySelector);
        key = key.Trim().ToLowerInvariant();
		_logger.LogError("DeleteSettingAsync(settings={Settings},keySelector={KeySelector},storeId={StoreId}): key → {Key}", settings, keySelector, storeId, key);

        var allSettings = await GetAllSettingsDictionaryAsync();
		_logger.LogError("DeleteSettingAsync(settings={Settings},keySelector={KeySelector},storeId={StoreId}): allSettings → {AllSettings}", settings, keySelector, storeId, allSettings);
        var settingForCaching = allSettings.TryGetValue(key, out var settings_) ?
            settings_.FirstOrDefault(x => x.StoreId == storeId) : null;
        if (settingForCaching == null)
            return;

        //update
        var setting = await GetSettingByIdAsync(settingForCaching.Id);
		_logger.LogError("DeleteSettingAsync(settings={Settings},keySelector={KeySelector},storeId={StoreId}): setting → {Setting}", settings, keySelector, storeId, setting);
        await DeleteSettingAsync(setting);
    }

    /// <summary>
    /// Clear cache
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ClearCacheAsync()
    {
        await _staticCacheManager.RemoveByPrefixAsync(NopEntityCacheDefaults<Setting>.Prefix);
    }

    /// <summary>
    /// Clear cache
    /// </summary>
    public virtual void ClearCache()
    {
		_logger.LogError(">>> Entering ClearCache()");
        _staticCacheManager.RemoveByPrefix(NopEntityCacheDefaults<Setting>.Prefix);
		_logger.LogWarning("<<< Exiting ClearCache()");
    }

    /// <summary>
    /// Get setting key (stored into database)
    /// </summary>
    /// <typeparam name="TSettings">Type of settings</typeparam>
    /// <typeparam name="T">Property type</typeparam>
    /// <param name="settings">Settings</param>
    /// <param name="keySelector">Key selector</param>
    /// <returns>Key</returns>
    public virtual string GetSettingKey<TSettings, T>(TSettings settings, Expression<Func<TSettings, T>> keySelector)
        where TSettings : ISettings, new()
    {
		_logger.LogError(">>> Entering GetSettingKey(settings={Settings},keySelector={KeySelector})", settings, keySelector);
			_logger.LogError("Throwing ArgumentException from GetSettingKey(settings={Settings},keySelector={KeySelector})", settings, keySelector);
        if (keySelector.Body is not MemberExpression member) {
            throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");
        }

        if (member.Member is not PropertyInfo propInfo)
            throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

        var key = $"{typeof(TSettings).Name}.{propInfo.Name}";
		_logger.LogError("GetSettingKey(settings={Settings},keySelector={KeySelector}): key → {Key}", settings, keySelector, key);

		_logger.LogWarning("<<< Exiting GetSettingKey(settings={Settings},keySelector={KeySelector})", settings, keySelector);
        return key;
    }
    #endregion
}