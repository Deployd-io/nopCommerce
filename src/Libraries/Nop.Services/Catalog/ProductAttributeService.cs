using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data;

namespace Nop.Services.Catalog;

/// <summary>
/// Product attribute service
/// </summary>
public partial class ProductAttributeService : IProductAttributeService
{
    #region Fields

    protected readonly IRepository<Picture> _pictureRepository;
    protected readonly IRepository<PredefinedProductAttributeValue> _predefinedProductAttributeValueRepository;
    protected readonly IRepository<Product> _productRepository;
    protected readonly IRepository<ProductAttribute> _productAttributeRepository;
    protected readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
    protected readonly IRepository<ProductAttributeCombinationPicture> _productAttributeCombinationPictureRepository;
    protected readonly IRepository<ProductAttributeMapping> _productAttributeMappingRepository;
    protected readonly IRepository<ProductAttributeValue> _productAttributeValueRepository;
    protected readonly IRepository<ProductAttributeValuePicture> _productAttributeValuePictureRepository;
    protected readonly IRepository<ProductPicture> _productPictureRepository;
    protected readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public ProductAttributeService(IRepository<Picture> pictureRepository,
        IRepository<PredefinedProductAttributeValue> predefinedProductAttributeValueRepository,
        IRepository<Product> productRepository,
        IRepository<ProductAttribute> productAttributeRepository,
        IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
        IRepository<ProductAttributeCombinationPicture> productAttributeCombinationPictureRepository,
        IRepository<ProductAttributeMapping> productAttributeMappingRepository,
        IRepository<ProductAttributeValue> productAttributeValueRepository,
        IRepository<ProductAttributeValuePicture> productAttributeValuePictureRepository,
        IRepository<ProductPicture> productPictureRepository,
        IStaticCacheManager staticCacheManager)
    {
        _pictureRepository = pictureRepository;
        _predefinedProductAttributeValueRepository = predefinedProductAttributeValueRepository;
        _productRepository = productRepository;
        _productAttributeRepository = productAttributeRepository;
        _productAttributeCombinationRepository = productAttributeCombinationRepository;
        _productAttributeCombinationPictureRepository = productAttributeCombinationPictureRepository;
        _productAttributeMappingRepository = productAttributeMappingRepository;
        _productAttributeValueRepository = productAttributeValueRepository;
        _productAttributeValuePictureRepository = productAttributeValuePictureRepository;
        _productPictureRepository = productPictureRepository;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods

    #region Product attributes

    /// <summary>
    /// Deletes a product attribute
    /// </summary>
    /// <param name="productAttribute">Product attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributeAsync(ProductAttribute productAttribute)
    {
		logger.LogDebug(">>> Entering DeleteProductAttributeAsync(productAttribute={ProductAttribute})", productAttribute);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeRepository.DeleteAsync(productAttribute);
		logger.LogDebug("<<< Exiting DeleteProductAttributeAsync(productAttribute={ProductAttribute})", productAttribute);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteProductAttributeAsync(productAttribute)={ProductAttribute}: external service call took {Arg1} ms", productAttribute, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteProductAttributeAsync(productAttribute)={ProductAttribute}: delete query executed in {Arg1} ms", productAttribute, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteProductAttributeAsync(productAttribute)={ProductAttribute}: external service call took {Arg1} ms", productAttribute, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteProductAttributeAsync(productAttribute)={ProductAttribute}: delete query executed in {Arg1} ms", productAttribute, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Deletes product attributes
    /// </summary>
    /// <param name="productAttributes">Product attributes</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributesAsync(IList<ProductAttribute> productAttributes)
    {
		logger.LogDebug(">>> Entering DeleteProductAttributesAsync(productAttributes={ProductAttributes})", productAttributes);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(productAttributes);

        await _productAttributeRepository.DeleteAsync(productAttributes);
		logger.LogDebug("<<< Exiting DeleteProductAttributesAsync(productAttributes={ProductAttributes})", productAttributes);
		logger.LogInformation("DeleteProductAttributesAsync(productAttributes)={ProductAttributes}: external service call took {Arg1} ms", productAttributes, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteProductAttributesAsync(productAttributes)={ProductAttributes}: delete query executed in {Arg1} ms", productAttributes, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Gets all product attributes
    /// </summary>
    /// <param name="name">Filter by name</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attributes
    /// </returns>
    public virtual async Task<IPagedList<ProductAttribute>> GetAllProductAttributesAsync(string name = null, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		logger.LogDebug(">>> Entering GetAllProductAttributesAsync(name={Name},pageIndex={PageIndex},pageSize={PageSize})", name, pageIndex, pageSize);
        var query = _productAttributeRepository.Table;

		logger.LogDebug("GetAllProductAttributesAsync(name={Name},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", name, pageIndex, pageSize, query);
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(pa => pa.Name.Contains(name));

		logger.LogDebug("<<< Exiting GetAllProductAttributesAsync(name={Name},pageIndex={PageIndex},pageSize={PageSize})", name, pageIndex, pageSize);
        return await query.OrderBy(x => x.Name).ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Gets a product attribute 
    /// </summary>
    /// <param name="productAttributeId">Product attribute identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute 
    /// </returns>
    public virtual async Task<ProductAttribute> GetProductAttributeByIdAsync(int productAttributeId)
    {
		logger.LogDebug(">>> Entering GetProductAttributeByIdAsync(productAttributeId={ProductAttributeId})", productAttributeId);
		logger.LogDebug("<<< Exiting GetProductAttributeByIdAsync(productAttributeId={ProductAttributeId})", productAttributeId);
        return await _productAttributeRepository.GetByIdAsync(productAttributeId, cache => default);
    }

    /// <summary>
    /// Gets product attributes 
    /// </summary>
    /// <param name="productAttributeIds">Product attribute identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attributes 
    /// </returns>
    public virtual async Task<IList<ProductAttribute>> GetProductAttributeByIdsAsync(int[] productAttributeIds)
    {
		logger.LogDebug(">>> Entering GetProductAttributeByIdsAsync(productAttributeIds={ProductAttributeIds})", productAttributeIds);
		logger.LogDebug("<<< Exiting GetProductAttributeByIdsAsync(productAttributeIds={ProductAttributeIds})", productAttributeIds);
        return await _productAttributeRepository.GetByIdsAsync(productAttributeIds);
    }

    /// <summary>
    /// Inserts a product attribute
    /// </summary>
    /// <param name="productAttribute">Product attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeAsync(ProductAttribute productAttribute)
    {
		logger.LogDebug(">>> Entering InsertProductAttributeAsync(productAttribute={ProductAttribute})", productAttribute);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeRepository.InsertAsync(productAttribute);
		logger.LogDebug("<<< Exiting InsertProductAttributeAsync(productAttribute={ProductAttribute})", productAttribute);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertProductAttributeAsync(productAttribute)={ProductAttribute}: save query executed in {Arg1} ms", productAttribute, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertProductAttributeAsync(productAttribute)={ProductAttribute}: save query executed in {Arg1} ms", productAttribute, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the product attribute
    /// </summary>
    /// <param name="productAttribute">Product attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeAsync(ProductAttribute productAttribute)
    {
		logger.LogDebug(">>> Entering UpdateProductAttributeAsync(productAttribute={ProductAttribute})", productAttribute);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeRepository.UpdateAsync(productAttribute);
		logger.LogDebug("<<< Exiting UpdateProductAttributeAsync(productAttribute={ProductAttribute})", productAttribute);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateProductAttributeAsync(productAttribute)={ProductAttribute}: update query executed in {Arg1} ms", productAttribute, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateProductAttributeAsync(productAttribute)={ProductAttribute}: update query executed in {Arg1} ms", productAttribute, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Returns a list of IDs of not existing attributes
    /// </summary>
    /// <param name="attributeId">The IDs of the attributes to check</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of IDs not existing attributes
    /// </returns>
    public virtual async Task<int[]> GetNotExistingAttributesAsync(int[] attributeId)
    {
		logger.LogDebug(">>> Entering GetNotExistingAttributesAsync(attributeId={AttributeId})", attributeId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(attributeId);

        var query = _productAttributeRepository.Table;
        var queryFilter = attributeId.Distinct().ToArray();
		logger.LogDebug("GetNotExistingAttributesAsync(attributeId={AttributeId}): query → {Query}", attributeId, query);
		logger.LogDebug("GetNotExistingAttributesAsync(attributeId={AttributeId}): queryFilter → {QueryFilter}", attributeId, queryFilter);
        var filter = await query.Select(a => a.Id)
            .Where(m => queryFilter.Contains(m))
            .ToListAsync();
			logger.LogInformation("GetNotExistingAttributesAsync(attributeId)={AttributeId}: database query executed in {Arg1} ms", attributeId, __logStart.ElapsedMilliseconds);

		logger.LogDebug("<<< Exiting GetNotExistingAttributesAsync(attributeId={AttributeId})", attributeId);
        return queryFilter.Except(filter).ToArray();
    }

    #endregion

    #region Product attributes mappings

    /// <summary>
    /// Deletes a product attribute mapping
    /// </summary>
    /// <param name="productAttributeMapping">Product attribute mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
    {
		logger.LogDebug(">>> Entering DeleteProductAttributeMappingAsync(productAttributeMapping={ProductAttributeMapping})", productAttributeMapping);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeMappingRepository.DeleteAsync(productAttributeMapping);
		logger.LogDebug("<<< Exiting DeleteProductAttributeMappingAsync(productAttributeMapping={ProductAttributeMapping})", productAttributeMapping);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteProductAttributeMappingAsync(productAttributeMapping)={ProductAttributeMapping}: external service call took {Arg1} ms", productAttributeMapping, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteProductAttributeMappingAsync(productAttributeMapping)={ProductAttributeMapping}: delete query executed in {Arg1} ms", productAttributeMapping, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteProductAttributeMappingAsync(productAttributeMapping)={ProductAttributeMapping}: external service call took {Arg1} ms", productAttributeMapping, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteProductAttributeMappingAsync(productAttributeMapping)={ProductAttributeMapping}: delete query executed in {Arg1} ms", productAttributeMapping, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets product attribute mappings by product identifier
    /// </summary>
    /// <param name="productId">The product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute mapping collection
    /// </returns>
    public virtual async Task<IList<ProductAttributeMapping>> GetProductAttributeMappingsByProductIdAsync(int productId)
    {
		logger.LogDebug(">>> Entering GetProductAttributeMappingsByProductIdAsync(productId={ProductId})", productId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var allCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeMappingsByProductCacheKey, productId);

        var query = from pam in _productAttributeMappingRepository.Table
			logger.LogDebug("GetProductAttributeMappingsByProductIdAsync(productId={ProductId}): allCacheKey → {AllCacheKey}", productId, allCacheKey);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            orderby pam.DisplayOrder, pam.Id
            where pam.ProductId == productId
            select pam;

        var attributes = await _staticCacheManager.GetAsync(allCacheKey, async () => await query.ToListAsync()) ?? new List<ProductAttributeMapping>();

		logger.LogDebug("GetProductAttributeMappingsByProductIdAsync(productId={ProductId}): attributes → {Attributes}", productId, attributes);
		logger.LogInformation("GetProductAttributeMappingsByProductIdAsync(productId)={ProductId}: external service call took {Arg1} ms", productId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetProductAttributeMappingsByProductIdAsync(productId={ProductId})", productId);
        return attributes;
    }

    /// <summary>
    /// Gets a product attribute mapping
    /// </summary>
    /// <param name="productAttributeMappingId">Product attribute mapping identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute mapping
    /// </returns>
    public virtual async Task<ProductAttributeMapping> GetProductAttributeMappingByIdAsync(int productAttributeMappingId)
    {
		logger.LogDebug(">>> Entering GetProductAttributeMappingByIdAsync(productAttributeMappingId={ProductAttributeMappingId})", productAttributeMappingId);
		logger.LogDebug("<<< Exiting GetProductAttributeMappingByIdAsync(productAttributeMappingId={ProductAttributeMappingId})", productAttributeMappingId);
        return await _productAttributeMappingRepository.GetByIdAsync(productAttributeMappingId, cache => default);
    }

    /// <summary>
    /// Inserts a product attribute mapping
    /// </summary>
    /// <param name="productAttributeMapping">The product attribute mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
    {
		logger.LogDebug(">>> Entering InsertProductAttributeMappingAsync(productAttributeMapping={ProductAttributeMapping})", productAttributeMapping);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeMappingRepository.InsertAsync(productAttributeMapping);
		logger.LogDebug("<<< Exiting InsertProductAttributeMappingAsync(productAttributeMapping={ProductAttributeMapping})", productAttributeMapping);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertProductAttributeMappingAsync(productAttributeMapping)={ProductAttributeMapping}: save query executed in {Arg1} ms", productAttributeMapping, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertProductAttributeMappingAsync(productAttributeMapping)={ProductAttributeMapping}: save query executed in {Arg1} ms", productAttributeMapping, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the product attribute mapping
    /// </summary>
    /// <param name="productAttributeMapping">The product attribute mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
    {
		logger.LogDebug(">>> Entering UpdateProductAttributeMappingAsync(productAttributeMapping={ProductAttributeMapping})", productAttributeMapping);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeMappingRepository.UpdateAsync(productAttributeMapping);
		logger.LogDebug("<<< Exiting UpdateProductAttributeMappingAsync(productAttributeMapping={ProductAttributeMapping})", productAttributeMapping);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateProductAttributeMappingAsync(productAttributeMapping)={ProductAttributeMapping}: update query executed in {Arg1} ms", productAttributeMapping, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateProductAttributeMappingAsync(productAttributeMapping)={ProductAttributeMapping}: update query executed in {Arg1} ms", productAttributeMapping, __logStart.ElapsedMilliseconds);
    #endregion

    #region Product attribute values

    /// <summary>
    /// Deletes a product attribute value
    /// </summary>
    /// <param name="productAttributeValue">Product attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
    {
		logger.LogDebug(">>> Entering DeleteProductAttributeValueAsync(productAttributeValue={ProductAttributeValue})", productAttributeValue);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeValueRepository.DeleteAsync(productAttributeValue);
		logger.LogDebug("<<< Exiting DeleteProductAttributeValueAsync(productAttributeValue={ProductAttributeValue})", productAttributeValue);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteProductAttributeValueAsync(productAttributeValue)={ProductAttributeValue}: external service call took {Arg1} ms", productAttributeValue, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteProductAttributeValueAsync(productAttributeValue)={ProductAttributeValue}: delete query executed in {Arg1} ms", productAttributeValue, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteProductAttributeValueAsync(productAttributeValue)={ProductAttributeValue}: external service call took {Arg1} ms", productAttributeValue, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteProductAttributeValueAsync(productAttributeValue)={ProductAttributeValue}: delete query executed in {Arg1} ms", productAttributeValue, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets product attribute values by product attribute mapping identifier
    /// </summary>
    /// <param name="productAttributeMappingId">The product attribute mapping identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute mapping collection
    /// </returns>
    public virtual async Task<IList<ProductAttributeValue>> GetProductAttributeValuesAsync(int productAttributeMappingId)
    {
		logger.LogDebug(">>> Entering GetProductAttributeValuesAsync(productAttributeMappingId={ProductAttributeMappingId})", productAttributeMappingId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeValuesByAttributeCacheKey, productAttributeMappingId);

        var query = from pav in _productAttributeValueRepository.Table
			logger.LogWarning("GetProductAttributeValuesAsync(productAttributeMappingId={ProductAttributeMappingId}): key → {Key}", productAttributeMappingId, key);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            orderby pav.DisplayOrder, pav.Id
            where pav.ProductAttributeMappingId == productAttributeMappingId
            select pav;
        var productAttributeValues = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

		logger.LogDebug("GetProductAttributeValuesAsync(productAttributeMappingId={ProductAttributeMappingId}): productAttributeValues → {ProductAttributeValues}", productAttributeMappingId, productAttributeValues);
		logger.LogInformation("GetProductAttributeValuesAsync(productAttributeMappingId)={ProductAttributeMappingId}: external service call took {Arg1} ms", productAttributeMappingId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetProductAttributeValuesAsync(productAttributeMappingId={ProductAttributeMappingId})", productAttributeMappingId);
        return productAttributeValues;
    }

    /// <summary>
    /// Gets a product attribute value
    /// </summary>
    /// <param name="productAttributeValueId">Product attribute value identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute value
    /// </returns>
    public virtual async Task<ProductAttributeValue> GetProductAttributeValueByIdAsync(int productAttributeValueId)
    {
		logger.LogDebug(">>> Entering GetProductAttributeValueByIdAsync(productAttributeValueId={ProductAttributeValueId})", productAttributeValueId);
		logger.LogDebug("<<< Exiting GetProductAttributeValueByIdAsync(productAttributeValueId={ProductAttributeValueId})", productAttributeValueId);
        return await _productAttributeValueRepository.GetByIdAsync(productAttributeValueId, cache => default);
    }

    /// <summary>
    /// Inserts a product attribute value
    /// </summary>
    /// <param name="productAttributeValue">The product attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
    {
		logger.LogDebug(">>> Entering InsertProductAttributeValueAsync(productAttributeValue={ProductAttributeValue})", productAttributeValue);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeValueRepository.InsertAsync(productAttributeValue);
		logger.LogDebug("<<< Exiting InsertProductAttributeValueAsync(productAttributeValue={ProductAttributeValue})", productAttributeValue);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertProductAttributeValueAsync(productAttributeValue)={ProductAttributeValue}: save query executed in {Arg1} ms", productAttributeValue, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertProductAttributeValueAsync(productAttributeValue)={ProductAttributeValue}: save query executed in {Arg1} ms", productAttributeValue, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the product attribute value
    /// </summary>
    /// <param name="productAttributeValue">The product attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
    {
		logger.LogDebug(">>> Entering UpdateProductAttributeValueAsync(productAttributeValue={ProductAttributeValue})", productAttributeValue);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeValueRepository.UpdateAsync(productAttributeValue);
		logger.LogDebug("<<< Exiting UpdateProductAttributeValueAsync(productAttributeValue={ProductAttributeValue})", productAttributeValue);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateProductAttributeValueAsync(productAttributeValue)={ProductAttributeValue}: update query executed in {Arg1} ms", productAttributeValue, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateProductAttributeValueAsync(productAttributeValue)={ProductAttributeValue}: update query executed in {Arg1} ms", productAttributeValue, __logStart.ElapsedMilliseconds);
    #endregion

    #region Product attribute value pictures

    /// <summary>
    /// Deletes a list of product attribute value picture
    /// </summary>
    /// <param name="value">Product attribute value pictures</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributeValuePicturesAsync(IList<ProductAttributeValuePicture> valuePictures)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeValuePictureRepository.DeleteAsync(valuePictures);
    }

    /// <summary>
    /// Inserts a product attribute value picture
    /// </summary>
    /// <param name="value">Product attribute value picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeValuePictureAsync(ProductAttributeValuePicture valuePicture)
    {
		logger.LogDebug(">>> Entering InsertProductAttributeValuePictureAsync(valuePicture={ValuePicture})", valuePicture);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeValuePictureRepository.InsertAsync(valuePicture);
		logger.LogDebug("<<< Exiting InsertProductAttributeValuePictureAsync(valuePicture={ValuePicture})", valuePicture);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertProductAttributeValuePictureAsync(valuePicture)={ValuePicture}: save query executed in {Arg1} ms", valuePicture, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertProductAttributeValuePictureAsync(valuePicture)={ValuePicture}: save query executed in {Arg1} ms", valuePicture, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates a product attribute value picture
    /// </summary>
    /// <param name="value">Product attribute value picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeValuePictureAsync(ProductAttributeValuePicture valuePicture)
    {
		logger.LogDebug(">>> Entering UpdateProductAttributeValuePictureAsync(valuePicture={ValuePicture})", valuePicture);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeValuePictureRepository.UpdateAsync(valuePicture);
		logger.LogDebug("<<< Exiting UpdateProductAttributeValuePictureAsync(valuePicture={ValuePicture})", valuePicture);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateProductAttributeValuePictureAsync(valuePicture)={ValuePicture}: update query executed in {Arg1} ms", valuePicture, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateProductAttributeValuePictureAsync(valuePicture)={ValuePicture}: update query executed in {Arg1} ms", valuePicture, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Get product attribute value pictures
    /// </summary>
    /// <param name="valueId">Value id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute value pictures
    /// </returns>
    public virtual async Task<IList<ProductAttributeValuePicture>> GetProductAttributeValuePicturesAsync(int valueId)
    {
		logger.LogDebug(">>> Entering GetProductAttributeValuePicturesAsync(valueId={ValueId})", valueId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var allCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeValuePicturesByValueCacheKey, valueId);

        var query = from pacp in _productAttributeValuePictureRepository.Table
			logger.LogDebug("GetProductAttributeValuePicturesAsync(valueId={ValueId}): allCacheKey → {AllCacheKey}", valueId, allCacheKey);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            join p in _pictureRepository.Table on pacp.PictureId equals p.Id
            join pp in _productPictureRepository.Table on p.Id equals pp.PictureId
            where pacp.ProductAttributeValueId == valueId
            orderby pp.DisplayOrder, pacp.PictureId
            select pacp;

        var valuePictures = await _staticCacheManager.GetAsync(allCacheKey, async () => await query.ToListAsync())
                            ?? new List<ProductAttributeValuePicture>();
							logger.LogInformation("GetProductAttributeValuePicturesAsync(valueId)={ValueId}: find query executed in {Arg1} ms", valueId, __logStart.ElapsedMilliseconds);

		logger.LogInformation("GetProductAttributeValuePicturesAsync(valueId)={ValueId}: external service call took {Arg1} ms", valueId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetProductAttributeValuePicturesAsync(valueId={ValueId})", valueId);
        return valuePictures;
    }

    /// <summary>
    /// Returns a ProductAttributeValuePicture that has the specified values
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="valueId">Product attribute value identifier</param>
    /// <param name="pictureId">Picture identifier</param>
    /// <returns>A ProductAttributeValuePicture that has the specified values; otherwise null</returns>
    public virtual ProductAttributeValuePicture FindProductAttributeValuePicture(IList<ProductAttributeValuePicture> source, int valueId, int pictureId)
    {
		logger.LogDebug(">>> Entering FindProductAttributeValuePicture(source={Source},valueId={ValueId},pictureId={PictureId})", source, valueId, pictureId);
		logger.LogDebug("<<< Exiting FindProductAttributeValuePicture(source={Source},valueId={ValueId},pictureId={PictureId})", source, valueId, pictureId);
        return source.FirstOrDefault(vp => vp.ProductAttributeValueId == valueId && vp.PictureId == pictureId);
    }

    #endregion

    #region Predefined product attribute values

    /// <summary>
    /// Deletes a predefined product attribute value
    /// </summary>
    /// <param name="ppav">Predefined product attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePredefinedProductAttributeValueAsync(PredefinedProductAttributeValue ppav)
    {
		logger.LogDebug(">>> Entering DeletePredefinedProductAttributeValueAsync(ppav={Ppav})", ppav);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _predefinedProductAttributeValueRepository.DeleteAsync(ppav);
		logger.LogDebug("<<< Exiting DeletePredefinedProductAttributeValueAsync(ppav={Ppav})", ppav);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeletePredefinedProductAttributeValueAsync(ppav)={Ppav}: external service call took {Arg1} ms", ppav, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeletePredefinedProductAttributeValueAsync(ppav)={Ppav}: delete query executed in {Arg1} ms", ppav, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeletePredefinedProductAttributeValueAsync(ppav)={Ppav}: external service call took {Arg1} ms", ppav, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeletePredefinedProductAttributeValueAsync(ppav)={Ppav}: delete query executed in {Arg1} ms", ppav, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets predefined product attribute values by product attribute identifier
    /// </summary>
    /// <param name="productAttributeId">The product attribute identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute mapping collection
    /// </returns>
    public virtual async Task<IList<PredefinedProductAttributeValue>> GetPredefinedProductAttributeValuesAsync(int productAttributeId)
    {
		logger.LogDebug(">>> Entering GetPredefinedProductAttributeValuesAsync(productAttributeId={ProductAttributeId})", productAttributeId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.PredefinedProductAttributeValuesByAttributeCacheKey, productAttributeId);

        var query = from ppav in _predefinedProductAttributeValueRepository.Table
			logger.LogWarning("GetPredefinedProductAttributeValuesAsync(productAttributeId={ProductAttributeId}): key → {Key}", productAttributeId, key);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            orderby ppav.DisplayOrder, ppav.Id
            where ppav.ProductAttributeId == productAttributeId
            select ppav;

        var values = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

		logger.LogDebug("GetPredefinedProductAttributeValuesAsync(productAttributeId={ProductAttributeId}): values → {Values}", productAttributeId, values);
		logger.LogInformation("GetPredefinedProductAttributeValuesAsync(productAttributeId)={ProductAttributeId}: external service call took {Arg1} ms", productAttributeId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetPredefinedProductAttributeValuesAsync(productAttributeId={ProductAttributeId})", productAttributeId);
        return values;
    }

    /// <summary>
    /// Gets a predefined product attribute value
    /// </summary>
    /// <param name="id">Predefined product attribute value identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the predefined product attribute value
    /// </returns>
    public virtual async Task<PredefinedProductAttributeValue> GetPredefinedProductAttributeValueByIdAsync(int id)
    {
		logger.LogDebug(">>> Entering GetPredefinedProductAttributeValueByIdAsync(id={Id})", id);
		logger.LogDebug("<<< Exiting GetPredefinedProductAttributeValueByIdAsync(id={Id})", id);
        return await _predefinedProductAttributeValueRepository.GetByIdAsync(id, cache => default);
    }

    /// <summary>
    /// Inserts a predefined product attribute value
    /// </summary>
    /// <param name="ppav">The predefined product attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPredefinedProductAttributeValueAsync(PredefinedProductAttributeValue ppav)
    {
		logger.LogDebug(">>> Entering InsertPredefinedProductAttributeValueAsync(ppav={Ppav})", ppav);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _predefinedProductAttributeValueRepository.InsertAsync(ppav);
		logger.LogDebug("<<< Exiting InsertPredefinedProductAttributeValueAsync(ppav={Ppav})", ppav);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertPredefinedProductAttributeValueAsync(ppav)={Ppav}: save query executed in {Arg1} ms", ppav, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertPredefinedProductAttributeValueAsync(ppav)={Ppav}: save query executed in {Arg1} ms", ppav, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the predefined product attribute value
    /// </summary>
    /// <param name="ppav">The predefined product attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdatePredefinedProductAttributeValueAsync(PredefinedProductAttributeValue ppav)
    {
		logger.LogDebug(">>> Entering UpdatePredefinedProductAttributeValueAsync(ppav={Ppav})", ppav);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _predefinedProductAttributeValueRepository.UpdateAsync(ppav);
		logger.LogDebug("<<< Exiting UpdatePredefinedProductAttributeValueAsync(ppav={Ppav})", ppav);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdatePredefinedProductAttributeValueAsync(ppav)={Ppav}: update query executed in {Arg1} ms", ppav, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdatePredefinedProductAttributeValueAsync(ppav)={Ppav}: update query executed in {Arg1} ms", ppav, __logStart.ElapsedMilliseconds);
    #endregion

    #region Product attribute combinations

    /// <summary>
    /// Deletes a product attribute combination
    /// </summary>
    /// <param name="combination">Product attribute combination</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributeCombinationAsync(ProductAttributeCombination combination)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeCombinationRepository.DeleteAsync(combination);
    }

    /// <summary>
    /// Gets all product attribute combinations
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute combinations
    /// </returns>
    public virtual async Task<IList<ProductAttributeCombination>> GetAllProductAttributeCombinationsAsync(int productId)
    {
		logger.LogDebug(">>> Entering GetAllProductAttributeCombinationsAsync(productId={ProductId})", productId);
        if (productId == 0)
            return new List<ProductAttributeCombination>();
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var combinations = await _productAttributeCombinationRepository.GetAllAsync(query =>
        {
            return from c in query
                orderby c.Id
                where c.ProductId == productId
                select c;
				logger.LogInformation("GetAllProductAttributeCombinationsAsync(productId)={ProductId}: find query executed in {Arg1} ms", productId, __logStart.ElapsedMilliseconds);
        }, cache => cache.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeCombinationsByProductCacheKey, productId));

		logger.LogDebug("<<< Exiting GetAllProductAttributeCombinationsAsync(productId={ProductId})", productId);
        return combinations;
    }

    /// <summary>
    /// Gets a product attribute combination
    /// </summary>
    /// <param name="productAttributeCombinationId">Product attribute combination identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute combination
    /// </returns>
    public virtual async Task<ProductAttributeCombination> GetProductAttributeCombinationByIdAsync(int productAttributeCombinationId)
    {
		logger.LogDebug(">>> Entering GetProductAttributeCombinationByIdAsync(productAttributeCombinationId={ProductAttributeCombinationId})", productAttributeCombinationId);
		logger.LogDebug("<<< Exiting GetProductAttributeCombinationByIdAsync(productAttributeCombinationId={ProductAttributeCombinationId})", productAttributeCombinationId);
        return await _productAttributeCombinationRepository.GetByIdAsync(productAttributeCombinationId, cache => default);
    }

    /// <summary>
    /// Gets a product attribute combination by SKU
    /// </summary>
    /// <param name="sku">SKU</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute combination
    /// </returns>
    public virtual async Task<ProductAttributeCombination> GetProductAttributeCombinationBySkuAsync(string sku)
    {
		logger.LogDebug(">>> Entering GetProductAttributeCombinationBySkuAsync(sku={Sku})", sku);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (string.IsNullOrEmpty(sku))
            return null;

        sku = sku.Trim();

		logger.LogDebug("GetProductAttributeCombinationBySkuAsync(sku={Sku}): sku → {Sku1}", sku, sku);
        var query = from pac in _productAttributeCombinationRepository.Table
            join p in _productRepository.Table on pac.ProductId equals p.Id
            orderby pac.Id
            where !p.Deleted && pac.Sku == sku
            select pac;
        var combination = await query.FirstOrDefaultAsync();

		logger.LogDebug("GetProductAttributeCombinationBySkuAsync(sku={Sku}): combination → {Combination}", sku, combination);
		logger.LogDebug("<<< Exiting GetProductAttributeCombinationBySkuAsync(sku={Sku})", sku);
        return combination;
    }

    /// <summary>
    /// Inserts a product attribute combination
    /// </summary>
    /// <param name="combination">Product attribute combination</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeCombinationAsync(ProductAttributeCombination combination)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeCombinationRepository.InsertAsync(combination);
    }

    /// <summary>
    /// Updates a product attribute combination
    /// </summary>
    /// <param name="combination">Product attribute combination</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeCombinationAsync(ProductAttributeCombination combination)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeCombinationRepository.UpdateAsync(combination);
    }

    #endregion

    #region Product attribute combination pictures

    /// <summary>
    /// Deletes a product attribute combination picture
    /// </summary>
    /// <param name="combination">Product attribute combination picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributeCombinationPictureAsync(ProductAttributeCombinationPicture combinationPicture)
    {
		logger.LogDebug(">>> Entering DeleteProductAttributeCombinationPictureAsync(combinationPicture={CombinationPicture})", combinationPicture);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeCombinationPictureRepository.DeleteAsync(combinationPicture);
		logger.LogDebug("<<< Exiting DeleteProductAttributeCombinationPictureAsync(combinationPicture={CombinationPicture})", combinationPicture);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteProductAttributeCombinationPictureAsync(combinationPicture)={CombinationPicture}: external service call took {Arg1} ms", combinationPicture, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteProductAttributeCombinationPictureAsync(combinationPicture)={CombinationPicture}: delete query executed in {Arg1} ms", combinationPicture, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteProductAttributeCombinationPictureAsync(combinationPicture)={CombinationPicture}: external service call took {Arg1} ms", combinationPicture, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteProductAttributeCombinationPictureAsync(combinationPicture)={CombinationPicture}: delete query executed in {Arg1} ms", combinationPicture, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Inserts a product attribute combination picture
    /// </summary>
    /// <param name="combination">Product attribute combination picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeCombinationPictureAsync(ProductAttributeCombinationPicture combinationPicture)
    {
		logger.LogDebug(">>> Entering InsertProductAttributeCombinationPictureAsync(combinationPicture={CombinationPicture})", combinationPicture);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeCombinationPictureRepository.InsertAsync(combinationPicture);
		logger.LogDebug("<<< Exiting InsertProductAttributeCombinationPictureAsync(combinationPicture={CombinationPicture})", combinationPicture);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertProductAttributeCombinationPictureAsync(combinationPicture)={CombinationPicture}: save query executed in {Arg1} ms", combinationPicture, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertProductAttributeCombinationPictureAsync(combinationPicture)={CombinationPicture}: save query executed in {Arg1} ms", combinationPicture, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates a product attribute combination picture
    /// </summary>
    /// <param name="combination">Product attribute combination picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeCombinationPictureAsync(ProductAttributeCombinationPicture combinationPicture)
    {
		logger.LogDebug(">>> Entering UpdateProductAttributeCombinationPictureAsync(combinationPicture={CombinationPicture})", combinationPicture);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeCombinationPictureRepository.UpdateAsync(combinationPicture);
		logger.LogDebug("<<< Exiting UpdateProductAttributeCombinationPictureAsync(combinationPicture={CombinationPicture})", combinationPicture);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateProductAttributeCombinationPictureAsync(combinationPicture)={CombinationPicture}: update query executed in {Arg1} ms", combinationPicture, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateProductAttributeCombinationPictureAsync(combinationPicture)={CombinationPicture}: update query executed in {Arg1} ms", combinationPicture, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Get product attribute combination pictures
    /// </summary>
    /// <param name="combinationId">Combination id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute combination pictures
    /// </returns>
    public virtual async Task<IList<ProductAttributeCombinationPicture>> GetProductAttributeCombinationPicturesAsync(int combinationId)
    {
		logger.LogDebug(">>> Entering GetProductAttributeCombinationPicturesAsync(combinationId={CombinationId})", combinationId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var allCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeCombinationPicturesByCombinationCacheKey, combinationId);

        var query = from pacp in _productAttributeCombinationPictureRepository.Table
			logger.LogDebug("GetProductAttributeCombinationPicturesAsync(combinationId={CombinationId}): allCacheKey → {AllCacheKey}", combinationId, allCacheKey);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            join p in _pictureRepository.Table on pacp.PictureId equals p.Id
            join pp in _productPictureRepository.Table on p.Id equals pp.PictureId
            where pacp.ProductAttributeCombinationId == combinationId
            orderby pp.DisplayOrder, pacp.PictureId
            select pacp;

        var combinationPictures = await _staticCacheManager.GetAsync(allCacheKey, async () => await query.ToListAsync()) 
                                  ?? new List<ProductAttributeCombinationPicture>();
								logger.LogInformation("GetProductAttributeCombinationPicturesAsync(combinationId)={CombinationId}: find query executed in {Arg1} ms", combinationId, __logStart.ElapsedMilliseconds);

		logger.LogInformation("GetProductAttributeCombinationPicturesAsync(combinationId)={CombinationId}: external service call took {Arg1} ms", combinationId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetProductAttributeCombinationPicturesAsync(combinationId={CombinationId})", combinationId);
        return combinationPictures;
    }

    /// <summary>
    /// Returns a ProductAttributeCombinationPicture that has the specified values
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="combinationId">Product attribute combination identifier</param>
    /// <param name="pictureId">Picture identifier</param>
    /// <returns>A ProductAttributeCombinationPicture that has the specified values; otherwise null</returns>
    public virtual ProductAttributeCombinationPicture FindProductAttributeCombinationPicture(IList<ProductAttributeCombinationPicture> source, int combinationId, int pictureId)
    {
		logger.LogDebug(">>> Entering FindProductAttributeCombinationPicture(source={Source},combinationId={CombinationId},pictureId={PictureId})", source, combinationId, pictureId);
		logger.LogDebug("<<< Exiting FindProductAttributeCombinationPicture(source={Source},combinationId={CombinationId},pictureId={PictureId})", source, combinationId, pictureId);
        return source.FirstOrDefault(pacp => pacp.ProductAttributeCombinationId == combinationId && pacp.PictureId == pictureId);
    }

    #endregion

    #endregion
}