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
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
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
		_logger.LogError(">>> Entering AttributeService(attributeRepository={AttributeRepository},attributeValueRepository={AttributeValueRepository},staticCacheManager={StaticCacheManager})", attributeRepository, attributeValueRepository, staticCacheManager);
        _attributeRepository = attributeRepository;
        _attributeValueRepository = attributeValueRepository;
		_logger.LogDebug("AttributeService(attributeRepository={AttributeRepository},attributeValueRepository={AttributeValueRepository},staticCacheManager={StaticCacheManager}): _attributeValueRepository → {_attributeValueRepository}", attributeRepository, attributeValueRepository, staticCacheManager, _attributeValueRepository);
		_logger.LogDebug("AttributeService(attributeRepository={AttributeRepository},attributeValueRepository={AttributeValueRepository},staticCacheManager={StaticCacheManager}): _attributeRepository → {_attributeRepository}", attributeRepository, attributeValueRepository, staticCacheManager, _attributeRepository);
    _logger.LogWarning("<<< Exiting AttributeService(attributeRepository={AttributeRepository},attributeValueRepository={AttributeValueRepository},staticCacheManager={StaticCacheManager})", attributeRepository, attributeValueRepository, staticCacheManager);
        _staticCacheManager = staticCacheManager;
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
		_logger.LogError(">>> Entering GetAllAttributesAsync()");
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
		_logger.LogError(">>> Entering GetAttributeByIdAsync(attributeId={AttributeId})", attributeId);
		_logger.LogError("GetAttributeByIdAsync(attributeId)={AttributeId}: find query executing", attributeId);
    _logger.LogWarning("<<< Exiting GetAttributeByIdAsync(attributeId={AttributeId})", attributeId);
        return await _attributeRepository.GetByIdAsync(attributeId, _ => default);
    }

    /// <summary>
    /// Inserts a attribute
    /// </summary>
    /// <param name="attribute"> attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAttributeAsync(TAttribute attribute)
    {
		_logger.LogError(">>> Entering InsertAttributeAsync(attribute={Attribute})", attribute);
        await _attributeRepository.InsertAsync(attribute);
		_logger.LogWarning("<<< Exiting InsertAttributeAsync(attribute={Attribute})", attribute);
    }

    /// <summary>
    /// Updates a attribute
    /// </summary>
    /// <param name="attribute"> attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateAttributeAsync(TAttribute attribute)
    {
		_logger.LogError(">>> Entering UpdateAttributeAsync(attribute={Attribute})", attribute);
        await _attributeRepository.UpdateAsync(attribute);
		_logger.LogWarning("<<< Exiting UpdateAttributeAsync(attribute={Attribute})", attribute);
    }

    /// <summary>
    /// Deletes a attribute
    /// </summary>
    /// <param name="attribute"> attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAttributeAsync(TAttribute attribute)
    {
		_logger.LogError(">>> Entering DeleteAttributeAsync(attribute={Attribute})", attribute);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _attributeRepository.DeleteAsync(attribute);
		_logger.LogError("DeleteAttributeAsync(attribute)={Attribute}: external service call took {Arg1} ms", attribute, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteAttributeAsync(attribute={Attribute})", attribute);
    }

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
		_logger.LogError(">>> Entering GetAttributeByIdsAsync(attributeIds={AttributeIds})", attributeIds);
        _logger.LogWarning("<<< Exiting GetAttributeByIdsAsync(attributeIds={AttributeIds})", attributeIds);
        return await _attributeRepository.GetByIdsAsync(attributeIds);
    }

    /// <summary>
    /// Deletes attributes
    /// </summary>
    /// <param name="attributes">Attributes</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAttributesAsync(IList<TAttribute> attributes)
    {
		_logger.LogError(">>> Entering DeleteAttributesAsync(attributes={Attributes})", attributes);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(attributes);

		_logger.LogError("DeleteAttributesAsync(attributes)={Attributes}: external service call took {Arg1} ms", attributes, __logStart.ElapsedMilliseconds);
		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
    _logger.LogWarning("<<< Exiting DeleteAttributesAsync(attributes={Attributes})", attributes);
        await _attributeRepository.DeleteAsync(attributes);
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
		_logger.LogError(">>> Entering GetAttributeValuesAsync(attributeId={AttributeId})", attributeId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = _staticCacheManager.PrepareKeyForDefaultCache(
            NopAttributeDefaults.AttributeValuesByAttributeCacheKey, typeof(TAttribute).Name, attributeId);

        var query = _attributeValueRepository.Table
            .Where(attributeValue => attributeValue.AttributeId == attributeId)
            .OrderBy(attributeValue => attributeValue.DisplayOrder)
            .ThenBy(attributeValue => attributeValue.Id);

		_logger.LogError("GetAttributeValuesAsync(attributeId)={AttributeId}: find query executing", attributeId);
		_logger.LogError("GetAttributeValuesAsync(attributeId)={AttributeId}: external service call took {Arg1} ms", attributeId, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetAttributeValuesAsync(attributeId={AttributeId})", attributeId);
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
		_logger.LogError(">>> Entering GetAttributeValueByIdAsync(attributeValueId={AttributeValueId})", attributeValueId);
		_logger.LogError("GetAttributeValueByIdAsync(attributeValueId)={AttributeValueId}: find query executing", attributeValueId);
    _logger.LogWarning("<<< Exiting GetAttributeValueByIdAsync(attributeValueId={AttributeValueId})", attributeValueId);
        return await _attributeValueRepository.GetByIdAsync(attributeValueId, _ => default);
    }

    /// <summary>
    /// Inserts a attribute value
    /// </summary>
    /// <param name="attributeValue"> attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAttributeValueAsync(TAttributeValue attributeValue)
    {
		_logger.LogError(">>> Entering InsertAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
        await _attributeValueRepository.InsertAsync(attributeValue);
		_logger.LogWarning("<<< Exiting InsertAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
    }

    /// <summary>
    /// Updates a attribute value
    /// </summary>
    /// <param name="attributeValue"> attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateAttributeValueAsync(TAttributeValue attributeValue)
    {
		_logger.LogError(">>> Entering UpdateAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
        await _attributeValueRepository.UpdateAsync(attributeValue);
		_logger.LogWarning("<<< Exiting UpdateAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
    }

    /// <summary>
    /// Deletes a attribute value
    /// </summary>
    /// <param name="attributeValue"> attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAttributeValueAsync(TAttributeValue attributeValue)
    {
		_logger.LogError(">>> Entering DeleteAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _attributeValueRepository.DeleteAsync(attributeValue);
		_logger.LogError("DeleteAttributeValueAsync(attributeValue)={AttributeValue}: external service call took {Arg1} ms", attributeValue, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
    }

    #endregion

    #endregion
}