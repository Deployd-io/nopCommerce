using Nop.Core.Caching;
using Nop.Core.Domain.Attributes;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Attributes;

/// <summary>
/// Attribute service
/// </summary>
/// <typeparam name="TAttribute">Type of the attribute (see <see cref="BaseAttribute"/>)</typeparam>
/// <typeparam name="TAttributeValue">Type of the attribute value (see <see cref="BaseAttributeValue"/>)</typeparam>
public partial class AttributeService<TAttribute, TAttributeValue> : IAttributeService<TAttribute, TAttributeValue>
    where TAttribute : BaseAttribute
    where TAttributeValue : BaseAttributeValue
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<TAttribute> _attributeRepository;
    protected readonly IRepository<TAttributeValue> _attributeValueRepository;
    protected readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public AttributeService(IRepository<TAttribute> attributeRepository,
        IRepository<TAttributeValue> attributeValueRepository,
        IStaticCacheManager staticCacheManager)
    {
		_logger.LogDebug(">>> Entering AttributeService(attributeRepository={AttributeRepository},attributeValueRepository={AttributeValueRepository},staticCacheManager={StaticCacheManager})", attributeRepository, attributeValueRepository, staticCacheManager);
        _attributeRepository = attributeRepository;
        _attributeValueRepository = attributeValueRepository;
		_logger.LogDebug("AttributeService(attributeRepository={AttributeRepository},attributeValueRepository={AttributeValueRepository},staticCacheManager={StaticCacheManager}): _attributeValueRepository → {_attributeValueRepository}", attributeRepository, attributeValueRepository, staticCacheManager, _attributeValueRepository);
		_logger.LogDebug("AttributeService(attributeRepository={AttributeRepository},attributeValueRepository={AttributeValueRepository},staticCacheManager={StaticCacheManager}): _attributeRepository → {_attributeRepository}", attributeRepository, attributeValueRepository, staticCacheManager, _attributeRepository);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("<<< Exiting AttributeService(attributeRepository={AttributeRepository},attributeValueRepository={AttributeValueRepository},staticCacheManager={StaticCacheManager})", attributeRepository, attributeValueRepository, staticCacheManager);
		_logger.LogDebug("AttributeService(attributeRepository={AttributeRepository},attributeValueRepository={AttributeValueRepository},staticCacheManager={StaticCacheManager}): _staticCacheManager → {_staticCacheManager}", attributeRepository, attributeValueRepository, staticCacheManager, _staticCacheManager);
    }

    #endregion

    #region Methods

    #region Attributes

    /// <summary>
    /// Gets all attributes
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attributes
    /// </returns>
    public virtual async Task<IList<TAttribute>> GetAllAttributesAsync()
    {
		_logger.LogDebug(">>> Entering GetAllAttributesAsync()");
		_logger.LogDebug("GetAllAttributesAsync(): query → {Query}");
		_logger.LogDebug("<<< Exiting GetAllAttributesAsync()");
        return await _attributeRepository.GetAllAsync(
            query => query.OrderBy(attribute => attribute.DisplayOrder)
                .ThenBy(attribute => attribute.Id),
            _ => default);
    }

    /// <summary>
    /// Gets a attribute 
    /// </summary>
    /// <param name="attributeId"> attribute identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attribute
    /// </returns>
    public virtual async Task<TAttribute> GetAttributeByIdAsync(int attributeId)
    {
		_logger.LogDebug(">>> Entering GetAttributeByIdAsync(attributeId={AttributeId})", attributeId);
		_logger.LogDebug("<<< Exiting GetAttributeByIdAsync(attributeId={AttributeId})", attributeId);
        return await _attributeRepository.GetByIdAsync(attributeId, _ => default);
    }

    /// <summary>
    /// Inserts a attribute
    /// </summary>
    /// <param name="attribute"> attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAttributeAsync(TAttribute attribute)
    {
		_logger.LogDebug(">>> Entering InsertAttributeAsync(attribute={Attribute})", attribute);
        await _attributeRepository.InsertAsync(attribute);
		_logger.LogDebug("<<< Exiting InsertAttributeAsync(attribute={Attribute})", attribute);
    }

    /// <summary>
    /// Updates a attribute
    /// </summary>
    /// <param name="attribute"> attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateAttributeAsync(TAttribute attribute)
    {
		_logger.LogDebug(">>> Entering UpdateAttributeAsync(attribute={Attribute})", attribute);
        await _attributeRepository.UpdateAsync(attribute);
		_logger.LogDebug("<<< Exiting UpdateAttributeAsync(attribute={Attribute})", attribute);
    }

    /// <summary>
    /// Deletes a attribute
    /// </summary>
    /// <param name="attribute"> attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAttributeAsync(TAttribute attribute)
    {
		_logger.LogDebug(">>> Entering DeleteAttributeAsync(attribute={Attribute})", attribute);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _attributeRepository.DeleteAsync(attribute);
		_logger.LogDebug("<<< Exiting DeleteAttributeAsync(attribute={Attribute})", attribute);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteAttributeAsync(attribute)={Attribute}: external service call took {Arg1} ms", attribute, __logStart.ElapsedMilliseconds);
    }

	_logger.LogInformation("DeleteAttributeAsync(attribute)={Attribute}: external service call took {Arg1} ms", attribute, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets attributes 
    /// </summary>
    /// <param name="attributeIds">Attribute identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attributes
    /// </returns>
    public virtual async Task<IList<TAttribute>> GetAttributeByIdsAsync(int[] attributeIds)
    {
		_logger.LogDebug(">>> Entering GetAttributeByIdsAsync(attributeIds={AttributeIds})", attributeIds);
		_logger.LogDebug("<<< Exiting GetAttributeByIdsAsync(attributeIds={AttributeIds})", attributeIds);
        return await _attributeRepository.GetByIdsAsync(attributeIds);
    }

    /// <summary>
    /// Deletes attributes
    /// </summary>
    /// <param name="attributes">Attributes</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAttributesAsync(IList<TAttribute> attributes)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogDebug(">>> Entering DeleteAttributesAsync(attributes={Attributes})", attributes);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(attributes);

        await _attributeRepository.DeleteAsync(attributes);
		_logger.LogDebug("<<< Exiting DeleteAttributesAsync(attributes={Attributes})", attributes);
		_logger.LogInformation("DeleteAttributesAsync(attributes)={Attributes}: external service call took {Arg1} ms", attributes, __logStart.ElapsedMilliseconds);
    }

    #endregion

    #region Attribute values

    /// <summary>
    /// Gets attribute values by attribute identifier
    /// </summary>
    /// <param name="attributeId">The attribute identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attribute values
    /// </returns>
    public virtual async Task<IList<TAttributeValue>> GetAttributeValuesAsync(int attributeId)
    {
		_logger.LogDebug(">>> Entering GetAttributeValuesAsync(attributeId={AttributeId})", attributeId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = _staticCacheManager.PrepareKeyForDefaultCache(
            NopAttributeDefaults.AttributeValuesByAttributeCacheKey, typeof(TAttribute).Name, attributeId);

        var query = _attributeValueRepository.Table
            .Where(attributeValue => attributeValue.AttributeId == attributeId)
            .OrderBy(attributeValue => attributeValue.DisplayOrder)
            .ThenBy(attributeValue => attributeValue.Id);

		_logger.LogInformation("GetAttributeValuesAsync(attributeId)={AttributeId}: external service call took {Arg1} ms", attributeId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetAttributeValuesAsync(attributeId={AttributeId})", attributeId);
        return await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
    }

    /// <summary>
    /// Gets a attribute value
    /// </summary>
    /// <param name="attributeValueId"> attribute value identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attribute value
    /// </returns>
    public virtual async Task<TAttributeValue> GetAttributeValueByIdAsync(int attributeValueId)
    {
		_logger.LogDebug(">>> Entering GetAttributeValueByIdAsync(attributeValueId={AttributeValueId})", attributeValueId);
		_logger.LogDebug("<<< Exiting GetAttributeValueByIdAsync(attributeValueId={AttributeValueId})", attributeValueId);
        return await _attributeValueRepository.GetByIdAsync(attributeValueId, _ => default);
    }

    /// <summary>
    /// Inserts a attribute value
    /// </summary>
    /// <param name="attributeValue"> attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAttributeValueAsync(TAttributeValue attributeValue)
    {
		_logger.LogDebug(">>> Entering InsertAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
        await _attributeValueRepository.InsertAsync(attributeValue);
		_logger.LogDebug("<<< Exiting InsertAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
    }

    /// <summary>
    /// Updates a attribute value
    /// </summary>
    /// <param name="attributeValue"> attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateAttributeValueAsync(TAttributeValue attributeValue)
    {
		_logger.LogDebug(">>> Entering UpdateAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
        await _attributeValueRepository.UpdateAsync(attributeValue);
		_logger.LogDebug("<<< Exiting UpdateAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
    }

    /// <summary>
    /// Deletes a attribute value
    /// </summary>
    /// <param name="attributeValue"> attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAttributeValueAsync(TAttributeValue attributeValue)
    {
		_logger.LogDebug(">>> Entering DeleteAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _attributeValueRepository.DeleteAsync(attributeValue);
		_logger.LogDebug("<<< Exiting DeleteAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteAttributeValueAsync(attributeValue)={AttributeValue}: external service call took {Arg1} ms", attributeValue, __logStart.ElapsedMilliseconds);
    }

	_logger.LogInformation("DeleteAttributeValueAsync(attributeValue)={AttributeValue}: external service call took {Arg1} ms", attributeValue, __logStart.ElapsedMilliseconds);
    #endregion

    #endregion
}