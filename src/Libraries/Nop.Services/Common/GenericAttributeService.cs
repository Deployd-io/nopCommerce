using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Data;

namespace Nop.Services.Common;

/// <summary>
/// Generic attribute service
/// </summary>
public partial class GenericAttributeService : IGenericAttributeService
{
    #region Fields

    protected readonly IRepository<GenericAttribute> _genericAttributeRepository;
    protected readonly IShortTermCacheManager _shortTermCacheManager;
    protected readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public GenericAttributeService(IRepository<GenericAttribute> genericAttributeRepository,
        IShortTermCacheManager shortTermCacheManager,
        IStaticCacheManager staticCacheManager)
    {
        _genericAttributeRepository = genericAttributeRepository;
        _shortTermCacheManager = shortTermCacheManager;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes an attribute
    /// </summary>
    /// <param name="attribute">Attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAttributeAsync(GenericAttribute attribute)
    {
		logger.LogDebug(">>> Entering DeleteAttributeAsync(attribute={Attribute})", attribute);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _genericAttributeRepository.DeleteAsync(attribute);
		logger.LogDebug("<<< Exiting DeleteAttributeAsync(attribute={Attribute})", attribute);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteAttributeAsync(attribute)={Attribute}: external service call took {Arg1} ms", attribute, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteAttributeAsync(attribute)={Attribute}: delete query executed in {Arg1} ms", attribute, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteAttributeAsync(attribute)={Attribute}: external service call took {Arg1} ms", attribute, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteAttributeAsync(attribute)={Attribute}: delete query executed in {Arg1} ms", attribute, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Deletes an attributes
    /// </summary>
    /// <param name="attributes">Attributes</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAttributesAsync(IList<GenericAttribute> attributes)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _genericAttributeRepository.DeleteAsync(attributes);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogDebug(">>> Entering DeleteAttributesAsync(key={Key})", key);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogDebug("<<< Exiting DeleteAttributesAsync(key={Key})", key);
		logger.LogInformation("DeleteAttributesAsync(key)={Key}: external service call took {Arg1} ms", key, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteAttributesAsync(key)={Key}: delete query executed in {Arg1} ms", key, __logStart.ElapsedMilliseconds);
    }

	var __logStart = System.Diagnostics.Stopwatch.StartNew();
    /// <summary>
    /// Deletes an attributes
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAttributesAsync<TEntity>(string key)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var keyGroup = typeof(TEntity).Name;
        await _genericAttributeRepository.DeleteAsync(a => a.Key == key && a.KeyGroup == keyGroup);
    }

    /// <summary>
    /// Inserts an attribute
	logger.LogDebug("DeleteAttributesAsync(key={Key}): keyGroup → {KeyGroup}", key, keyGroup);
	logger.LogInformation("DeleteAttributesAsync(key)={Key}: delete query executed in {Arg1} ms", key, __logStart.ElapsedMilliseconds);
    /// </summary>
    /// <param name="attribute">attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAttributeAsync(GenericAttribute attribute)
    {
		logger.LogDebug(">>> Entering InsertAttributeAsync(attribute={Attribute})", attribute);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(attribute);

        attribute.CreatedOrUpdatedDateUTC = DateTime.UtcNow;
		logger.LogDebug("InsertAttributeAsync(attribute={Attribute}): attribute.CreatedOrUpdatedDateUTC → {Arg1}", attribute, attribute.CreatedOrUpdatedDateUTC);

        await _genericAttributeRepository.InsertAsync(attribute);
		logger.LogDebug("<<< Exiting InsertAttributeAsync(attribute={Attribute})", attribute);
		logger.LogInformation("InsertAttributeAsync(attribute)={Attribute}: save query executed in {Arg1} ms", attribute, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Updates the attribute
    /// </summary>
    /// <param name="attribute">Attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateAttributeAsync(GenericAttribute attribute)
    {
		logger.LogDebug(">>> Entering UpdateAttributeAsync(attribute={Attribute})", attribute);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(attribute);

        attribute.CreatedOrUpdatedDateUTC = DateTime.UtcNow;
		logger.LogDebug("UpdateAttributeAsync(attribute={Attribute}): attribute.CreatedOrUpdatedDateUTC → {Arg1}", attribute, attribute.CreatedOrUpdatedDateUTC);

        await _genericAttributeRepository.UpdateAsync(attribute);
		logger.LogDebug("<<< Exiting UpdateAttributeAsync(attribute={Attribute})", attribute);
		logger.LogInformation("UpdateAttributeAsync(attribute)={Attribute}: update query executed in {Arg1} ms", attribute, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Get attributes
    /// </summary>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="keyGroup">Key group</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the get attributes
    /// </returns>
    public virtual async Task<IList<GenericAttribute>> GetAttributesForEntityAsync(int entityId, string keyGroup)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var query = from ga in _genericAttributeRepository.Table
            where ga.EntityId == entityId &&
                  ga.KeyGroup == keyGroup
				var __logStart = System.Diagnostics.Stopwatch.StartNew();
            select ga;
        var attributes = await _shortTermCacheManager.GetAsync(async () => await query.ToListAsync(), NopCommonDefaults.GenericAttributeCacheKey, entityId, keyGroup);

        return attributes;
    }

    /// <summary>
    /// Save attribute value
    /// </summary>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="storeId">Store identifier; pass 0 if this attribute will be available for all stores</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SaveAttributeAsync<TPropType>(BaseEntity entity, string key, TPropType value, int storeId = 0)
    {
        ArgumentNullException.ThrowIfNull(entity);

        ArgumentNullException.ThrowIfNull(key);

        var keyGroup = entity.GetType().Name;

        var props = (await GetAttributesForEntityAsync(entity.Id, keyGroup))
            .Where(x => x.StoreId == storeId)
            .ToList();
        var prop = props.FirstOrDefault(ga =>
            ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

        var valueStr = CommonHelper.To<string>(value);

        if (prop != null)
        {
            if (string.IsNullOrWhiteSpace(valueStr))
                //delete
                await DeleteAttributeAsync(prop);
            else
            {
                //update
                prop.Value = valueStr;
                await UpdateAttributeAsync(prop);
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(valueStr))
                return;

            //insert
            prop = new GenericAttribute
            {
                EntityId = entity.Id,
                Key = key,
                KeyGroup = keyGroup,
                Value = valueStr,
                StoreId = storeId
            };

            await InsertAttributeAsync(prop);
        }
    }

    /// <summary>
    /// Get an attribute of an entity
    /// </summary>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="key">Key</param>
    /// <param name="storeId">Load a value specific for a certain store; pass 0 to load a value shared for all stores</param>
    /// <param name="defaultValue">Default value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attribute
    /// </returns>
    public virtual async Task<TPropType> GetAttributeAsync<TPropType>(BaseEntity entity, string key, int storeId = 0, TPropType defaultValue = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var keyGroup = entity.GetType().Name;

        var props = await GetAttributesForEntityAsync(entity.Id, keyGroup);

        //little hack here (only for unit testing). we should write expect-return rules in unit tests for such cases
        if (props == null)
            return defaultValue;

        props = props.Where(x => x.StoreId == storeId).ToList();
        if (!props.Any())
            return defaultValue;

        var prop = props.FirstOrDefault(ga =>
            ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

        if (prop == null || string.IsNullOrEmpty(prop.Value))
            return defaultValue;

        return CommonHelper.To<TPropType>(prop.Value);
    }

    /// <summary>
    /// Get an attribute of an entity
    /// </summary>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="key">Key</param>
    /// <param name="storeId">Load a value specific for a certain store; pass 0 to load a value shared for all stores</param>
    /// <param name="defaultValue">Default value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attribute
    /// </returns>
    public virtual async Task<TPropType> GetAttributeAsync<TEntity, TPropType>(int entityId, string key, int storeId = 0, TPropType defaultValue = default)
        where TEntity : BaseEntity
    {
		logger.LogDebug(">>> Entering GetAttributeAsync(entityId={EntityId},key={Key},storeId={StoreId},defaultValue={DefaultValue})", entityId, key, storeId, defaultValue);
        var entity = (TEntity)Activator.CreateInstance(typeof(TEntity));
        entity.Id = entityId;
		logger.LogDebug("GetAttributeAsync(entityId={EntityId},key={Key},storeId={StoreId},defaultValue={DefaultValue}): entity → {Entity}", entityId, key, storeId, defaultValue, entity);
		logger.LogDebug("GetAttributeAsync(entityId={EntityId},key={Key},storeId={StoreId},defaultValue={DefaultValue}): entity.Id → {Arg4}", entityId, key, storeId, defaultValue, entity.Id);

        return await GetAttributeAsync(entity, key, storeId, defaultValue);
    }

    #endregion
}
