using Nop.Core.Caching;
using Nop.Core.Domain.Attributes;
using Nop.Data;

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
        _attributeRepository = attributeRepository;
        _attributeValueRepository = attributeValueRepository;
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
		logger.LogDebug(">>> Entering GetAllAttributesAsync()");
		logger.LogDebug("GetAllAttributesAsync(): query → {Query}");
		logger.LogDebug("<<< Exiting GetAllAttributesAsync()");
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
		logger.LogDebug(">>> Entering GetAttributeByIdAsync(attributeId={AttributeId})", attributeId);
		logger.LogDebug("<<< Exiting GetAttributeByIdAsync(attributeId={AttributeId})", attributeId);
        return await _attributeRepository.GetByIdAsync(attributeId, _ => default);
    }

    /// <summary>
    /// Inserts a attribute
    /// </summary>
    /// <param name="attribute"> attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAttributeAsync(TAttribute attribute)
    {
		logger.LogDebug(">>> Entering InsertAttributeAsync(attribute={Attribute})", attribute);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _attributeRepository.InsertAsync(attribute);
		logger.LogDebug("<<< Exiting InsertAttributeAsync(attribute={Attribute})", attribute);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertAttributeAsync(attribute)={Attribute}: save query executed in {Arg1} ms", attribute, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertAttributeAsync(attribute)={Attribute}: save query executed in {Arg1} ms", attribute, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates a attribute
    /// </summary>
    /// <param name="attribute"> attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateAttributeAsync(TAttribute attribute)
    {
		logger.LogDebug(">>> Entering UpdateAttributeAsync(attribute={Attribute})", attribute);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _attributeRepository.UpdateAsync(attribute);
		logger.LogDebug("<<< Exiting UpdateAttributeAsync(attribute={Attribute})", attribute);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateAttributeAsync(attribute)={Attribute}: update query executed in {Arg1} ms", attribute, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateAttributeAsync(attribute)={Attribute}: update query executed in {Arg1} ms", attribute, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Deletes a attribute
    /// </summary>
    /// <param name="attribute"> attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAttributeAsync(TAttribute attribute)
    {
		logger.LogDebug(">>> Entering DeleteAttributeAsync(attribute={Attribute})", attribute);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _attributeRepository.DeleteAsync(attribute);
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
    /// Gets attributes 
    /// </summary>
    /// <param name="attributeIds">Attribute identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attributes
    /// </returns>
    public virtual async Task<IList<TAttribute>> GetAttributeByIdsAsync(int[] attributeIds)
    {
		logger.LogDebug(">>> Entering GetAttributeByIdsAsync(attributeIds={AttributeIds})", attributeIds);
		logger.LogDebug("<<< Exiting GetAttributeByIdsAsync(attributeIds={AttributeIds})", attributeIds);
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
		logger.LogDebug(">>> Entering DeleteAttributesAsync(attributes={Attributes})", attributes);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(attributes);

        await _attributeRepository.DeleteAsync(attributes);
		logger.LogDebug("<<< Exiting DeleteAttributesAsync(attributes={Attributes})", attributes);
		logger.LogInformation("DeleteAttributesAsync(attributes)={Attributes}: external service call took {Arg1} ms", attributes, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteAttributesAsync(attributes)={Attributes}: delete query executed in {Arg1} ms", attributes, __logStart.ElapsedMilliseconds);
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
		logger.LogDebug(">>> Entering GetAttributeValuesAsync(attributeId={AttributeId})", attributeId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = _staticCacheManager.PrepareKeyForDefaultCache(
            NopAttributeDefaults.AttributeValuesByAttributeCacheKey, typeof(TAttribute).Name, attributeId);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var query = _attributeValueRepository.Table
            .Where(attributeValue => attributeValue.AttributeId == attributeId)
            .OrderBy(attributeValue => attributeValue.DisplayOrder)
            .ThenBy(attributeValue => attributeValue.Id);

		logger.LogInformation("GetAttributeValuesAsync(attributeId)={AttributeId}: external service call took {Arg1} ms", attributeId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetAttributeValuesAsync(attributeId={AttributeId})", attributeId);
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
		logger.LogDebug(">>> Entering GetAttributeValueByIdAsync(attributeValueId={AttributeValueId})", attributeValueId);
		logger.LogDebug("<<< Exiting GetAttributeValueByIdAsync(attributeValueId={AttributeValueId})", attributeValueId);
        return await _attributeValueRepository.GetByIdAsync(attributeValueId, _ => default);
    }

    /// <summary>
    /// Inserts a attribute value
    /// </summary>
    /// <param name="attributeValue"> attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAttributeValueAsync(TAttributeValue attributeValue)
    {
		logger.LogDebug(">>> Entering InsertAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _attributeValueRepository.InsertAsync(attributeValue);
		logger.LogDebug("<<< Exiting InsertAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertAttributeValueAsync(attributeValue)={AttributeValue}: save query executed in {Arg1} ms", attributeValue, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertAttributeValueAsync(attributeValue)={AttributeValue}: save query executed in {Arg1} ms", attributeValue, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates a attribute value
    /// </summary>
    /// <param name="attributeValue"> attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateAttributeValueAsync(TAttributeValue attributeValue)
    {
		logger.LogDebug(">>> Entering UpdateAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _attributeValueRepository.UpdateAsync(attributeValue);
		logger.LogDebug("<<< Exiting UpdateAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateAttributeValueAsync(attributeValue)={AttributeValue}: update query executed in {Arg1} ms", attributeValue, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateAttributeValueAsync(attributeValue)={AttributeValue}: update query executed in {Arg1} ms", attributeValue, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Deletes a attribute value
    /// </summary>
    /// <param name="attributeValue"> attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAttributeValueAsync(TAttributeValue attributeValue)
    {
		logger.LogDebug(">>> Entering DeleteAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _attributeValueRepository.DeleteAsync(attributeValue);
		logger.LogDebug("<<< Exiting DeleteAttributeValueAsync(attributeValue={AttributeValue})", attributeValue);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteAttributeValueAsync(attributeValue)={AttributeValue}: external service call took {Arg1} ms", attributeValue, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteAttributeValueAsync(attributeValue)={AttributeValue}: delete query executed in {Arg1} ms", attributeValue, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteAttributeValueAsync(attributeValue)={AttributeValue}: external service call took {Arg1} ms", attributeValue, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteAttributeValueAsync(attributeValue)={AttributeValue}: delete query executed in {Arg1} ms", attributeValue, __logStart.ElapsedMilliseconds);
    #endregion

    #endregion
}