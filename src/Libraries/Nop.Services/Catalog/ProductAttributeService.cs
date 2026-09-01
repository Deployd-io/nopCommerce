using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog;

/// <summary>
/// Product attribute service
/// </summary>
public partial class ProductAttributeService : IProductAttributeService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogDebug(">>> Entering ProductAttributeService(pictureRepository={PictureRepository},predefinedProductAttributeValueRepository={PredefinedProductAttributeValueRepository},productRepository={ProductRepository},productAttributeRepository={ProductAttributeRepository},productAttributeCombinationRepository={ProductAttributeCombinationRepository},productAttributeCombinationPictureRepository={ProductAttributeCombinationPictureRepository},productAttributeMappingRepository=<redacted>,productAttributeValueRepository={ProductAttributeValueRepository},productAttributeValuePictureRepository={ProductAttributeValuePictureRepository},productPictureRepository={ProductPictureRepository},staticCacheManager={StaticCacheManager})", pictureRepository, predefinedProductAttributeValueRepository, productRepository, productAttributeRepository, productAttributeCombinationRepository, productAttributeCombinationPictureRepository, productAttributeValueRepository, productAttributeValuePictureRepository, productPictureRepository, staticCacheManager);
        _pictureRepository = pictureRepository;
		_logger.LogDebug("ProductAttributeService(pictureRepository={PictureRepository},predefinedProductAttributeValueRepository={PredefinedProductAttributeValueRepository},productRepository={ProductRepository},productAttributeRepository={ProductAttributeRepository},productAttributeCombinationRepository={ProductAttributeCombinationRepository},productAttributeCombinationPictureRepository={ProductAttributeCombinationPictureRepository},productAttributeMappingRepository=<redacted>,productAttributeValueRepository={ProductAttributeValueRepository},productAttributeValuePictureRepository={ProductAttributeValuePictureRepository},productPictureRepository={ProductPictureRepository},staticCacheManager={StaticCacheManager}): _pictureRepository → {_pictureRepository}", pictureRepository, predefinedProductAttributeValueRepository, productRepository, productAttributeRepository, productAttributeCombinationRepository, productAttributeCombinationPictureRepository, productAttributeValueRepository, productAttributeValuePictureRepository, productPictureRepository, staticCacheManager, _pictureRepository);
        _predefinedProductAttributeValueRepository = predefinedProductAttributeValueRepository;
		_logger.LogDebug("ProductAttributeService(pictureRepository={PictureRepository},predefinedProductAttributeValueRepository={PredefinedProductAttributeValueRepository},productRepository={ProductRepository},productAttributeRepository={ProductAttributeRepository},productAttributeCombinationRepository={ProductAttributeCombinationRepository},productAttributeCombinationPictureRepository={ProductAttributeCombinationPictureRepository},productAttributeMappingRepository=<redacted>,productAttributeValueRepository={ProductAttributeValueRepository},productAttributeValuePictureRepository={ProductAttributeValuePictureRepository},productPictureRepository={ProductPictureRepository},staticCacheManager={StaticCacheManager}): _predefinedProductAttributeValueRepository → {_predefinedProductAttributeValueRepository}", pictureRepository, predefinedProductAttributeValueRepository, productRepository, productAttributeRepository, productAttributeCombinationRepository, productAttributeCombinationPictureRepository, productAttributeValueRepository, productAttributeValuePictureRepository, productPictureRepository, staticCacheManager, _predefinedProductAttributeValueRepository);
        _productRepository = productRepository;
		_logger.LogDebug("ProductAttributeService(pictureRepository={PictureRepository},predefinedProductAttributeValueRepository={PredefinedProductAttributeValueRepository},productRepository={ProductRepository},productAttributeRepository={ProductAttributeRepository},productAttributeCombinationRepository={ProductAttributeCombinationRepository},productAttributeCombinationPictureRepository={ProductAttributeCombinationPictureRepository},productAttributeMappingRepository=<redacted>,productAttributeValueRepository={ProductAttributeValueRepository},productAttributeValuePictureRepository={ProductAttributeValuePictureRepository},productPictureRepository={ProductPictureRepository},staticCacheManager={StaticCacheManager}): _productRepository → {_productRepository}", pictureRepository, predefinedProductAttributeValueRepository, productRepository, productAttributeRepository, productAttributeCombinationRepository, productAttributeCombinationPictureRepository, productAttributeValueRepository, productAttributeValuePictureRepository, productPictureRepository, staticCacheManager, _productRepository);
        _productAttributeRepository = productAttributeRepository;
		_logger.LogDebug("ProductAttributeService(pictureRepository={PictureRepository},predefinedProductAttributeValueRepository={PredefinedProductAttributeValueRepository},productRepository={ProductRepository},productAttributeRepository={ProductAttributeRepository},productAttributeCombinationRepository={ProductAttributeCombinationRepository},productAttributeCombinationPictureRepository={ProductAttributeCombinationPictureRepository},productAttributeMappingRepository=<redacted>,productAttributeValueRepository={ProductAttributeValueRepository},productAttributeValuePictureRepository={ProductAttributeValuePictureRepository},productPictureRepository={ProductPictureRepository},staticCacheManager={StaticCacheManager}): _productAttributeRepository → {_productAttributeRepository}", pictureRepository, predefinedProductAttributeValueRepository, productRepository, productAttributeRepository, productAttributeCombinationRepository, productAttributeCombinationPictureRepository, productAttributeValueRepository, productAttributeValuePictureRepository, productPictureRepository, staticCacheManager, _productAttributeRepository);
        _productAttributeCombinationRepository = productAttributeCombinationRepository;
		_logger.LogDebug("ProductAttributeService(pictureRepository={PictureRepository},predefinedProductAttributeValueRepository={PredefinedProductAttributeValueRepository},productRepository={ProductRepository},productAttributeRepository={ProductAttributeRepository},productAttributeCombinationRepository={ProductAttributeCombinationRepository},productAttributeCombinationPictureRepository={ProductAttributeCombinationPictureRepository},productAttributeMappingRepository=<redacted>,productAttributeValueRepository={ProductAttributeValueRepository},productAttributeValuePictureRepository={ProductAttributeValuePictureRepository},productPictureRepository={ProductPictureRepository},staticCacheManager={StaticCacheManager}): _productAttributeCombinationRepository → {_productAttributeCombinationRepository}", pictureRepository, predefinedProductAttributeValueRepository, productRepository, productAttributeRepository, productAttributeCombinationRepository, productAttributeCombinationPictureRepository, productAttributeValueRepository, productAttributeValuePictureRepository, productPictureRepository, staticCacheManager, _productAttributeCombinationRepository);
        _productAttributeCombinationPictureRepository = productAttributeCombinationPictureRepository;
		_logger.LogDebug("ProductAttributeService(pictureRepository={PictureRepository},predefinedProductAttributeValueRepository={PredefinedProductAttributeValueRepository},productRepository={ProductRepository},productAttributeRepository={ProductAttributeRepository},productAttributeCombinationRepository={ProductAttributeCombinationRepository},productAttributeCombinationPictureRepository={ProductAttributeCombinationPictureRepository},productAttributeMappingRepository=<redacted>,productAttributeValueRepository={ProductAttributeValueRepository},productAttributeValuePictureRepository={ProductAttributeValuePictureRepository},productPictureRepository={ProductPictureRepository},staticCacheManager={StaticCacheManager}): _productAttributeCombinationPictureRepository → {_productAttributeCombinationPictureRepository}", pictureRepository, predefinedProductAttributeValueRepository, productRepository, productAttributeRepository, productAttributeCombinationRepository, productAttributeCombinationPictureRepository, productAttributeValueRepository, productAttributeValuePictureRepository, productPictureRepository, staticCacheManager, _productAttributeCombinationPictureRepository);
        _productAttributeMappingRepository = productAttributeMappingRepository;
		_logger.LogDebug("ProductAttributeService(pictureRepository={PictureRepository},predefinedProductAttributeValueRepository={PredefinedProductAttributeValueRepository},productRepository={ProductRepository},productAttributeRepository={ProductAttributeRepository},productAttributeCombinationRepository={ProductAttributeCombinationRepository},productAttributeCombinationPictureRepository={ProductAttributeCombinationPictureRepository},productAttributeMappingRepository=<redacted>,productAttributeValueRepository={ProductAttributeValueRepository},productAttributeValuePictureRepository={ProductAttributeValuePictureRepository},productPictureRepository={ProductPictureRepository},staticCacheManager={StaticCacheManager}): _productAttributeMappingRepository → {_productAttributeMappingRepository}", pictureRepository, predefinedProductAttributeValueRepository, productRepository, productAttributeRepository, productAttributeCombinationRepository, productAttributeCombinationPictureRepository, productAttributeValueRepository, productAttributeValuePictureRepository, productPictureRepository, staticCacheManager, _productAttributeMappingRepository);
        _productAttributeValueRepository = productAttributeValueRepository;
		_logger.LogDebug("ProductAttributeService(pictureRepository={PictureRepository},predefinedProductAttributeValueRepository={PredefinedProductAttributeValueRepository},productRepository={ProductRepository},productAttributeRepository={ProductAttributeRepository},productAttributeCombinationRepository={ProductAttributeCombinationRepository},productAttributeCombinationPictureRepository={ProductAttributeCombinationPictureRepository},productAttributeMappingRepository=<redacted>,productAttributeValueRepository={ProductAttributeValueRepository},productAttributeValuePictureRepository={ProductAttributeValuePictureRepository},productPictureRepository={ProductPictureRepository},staticCacheManager={StaticCacheManager}): _productAttributeValueRepository → {_productAttributeValueRepository}", pictureRepository, predefinedProductAttributeValueRepository, productRepository, productAttributeRepository, productAttributeCombinationRepository, productAttributeCombinationPictureRepository, productAttributeValueRepository, productAttributeValuePictureRepository, productPictureRepository, staticCacheManager, _productAttributeValueRepository);
        _productAttributeValuePictureRepository = productAttributeValuePictureRepository;
		_logger.LogDebug("ProductAttributeService(pictureRepository={PictureRepository},predefinedProductAttributeValueRepository={PredefinedProductAttributeValueRepository},productRepository={ProductRepository},productAttributeRepository={ProductAttributeRepository},productAttributeCombinationRepository={ProductAttributeCombinationRepository},productAttributeCombinationPictureRepository={ProductAttributeCombinationPictureRepository},productAttributeMappingRepository=<redacted>,productAttributeValueRepository={ProductAttributeValueRepository},productAttributeValuePictureRepository={ProductAttributeValuePictureRepository},productPictureRepository={ProductPictureRepository},staticCacheManager={StaticCacheManager}): _productAttributeValuePictureRepository → {_productAttributeValuePictureRepository}", pictureRepository, predefinedProductAttributeValueRepository, productRepository, productAttributeRepository, productAttributeCombinationRepository, productAttributeCombinationPictureRepository, productAttributeValueRepository, productAttributeValuePictureRepository, productPictureRepository, staticCacheManager, _productAttributeValuePictureRepository);
        _productPictureRepository = productPictureRepository;
		_logger.LogDebug("ProductAttributeService(pictureRepository={PictureRepository},predefinedProductAttributeValueRepository={PredefinedProductAttributeValueRepository},productRepository={ProductRepository},productAttributeRepository={ProductAttributeRepository},productAttributeCombinationRepository={ProductAttributeCombinationRepository},productAttributeCombinationPictureRepository={ProductAttributeCombinationPictureRepository},productAttributeMappingRepository=<redacted>,productAttributeValueRepository={ProductAttributeValueRepository},productAttributeValuePictureRepository={ProductAttributeValuePictureRepository},productPictureRepository={ProductPictureRepository},staticCacheManager={StaticCacheManager}): _productPictureRepository → {_productPictureRepository}", pictureRepository, predefinedProductAttributeValueRepository, productRepository, productAttributeRepository, productAttributeCombinationRepository, productAttributeCombinationPictureRepository, productAttributeValueRepository, productAttributeValuePictureRepository, productPictureRepository, staticCacheManager, _productPictureRepository);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("ProductAttributeService(pictureRepository={PictureRepository},predefinedProductAttributeValueRepository={PredefinedProductAttributeValueRepository},productRepository={ProductRepository},productAttributeRepository={ProductAttributeRepository},productAttributeCombinationRepository={ProductAttributeCombinationRepository},productAttributeCombinationPictureRepository={ProductAttributeCombinationPictureRepository},productAttributeMappingRepository=<redacted>,productAttributeValueRepository={ProductAttributeValueRepository},productAttributeValuePictureRepository={ProductAttributeValuePictureRepository},productPictureRepository={ProductPictureRepository},staticCacheManager={StaticCacheManager}): _staticCacheManager → {_staticCacheManager}", pictureRepository, predefinedProductAttributeValueRepository, productRepository, productAttributeRepository, productAttributeCombinationRepository, productAttributeCombinationPictureRepository, productAttributeValueRepository, productAttributeValuePictureRepository, productPictureRepository, staticCacheManager, _staticCacheManager);
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
		_logger.LogDebug(">>> Entering DeleteProductAttributeAsync(productAttribute={ProductAttribute})", productAttribute);
        await _productAttributeRepository.DeleteAsync(productAttribute);
    }

    /// <summary>
    /// Deletes product attributes
    /// </summary>
    /// <param name="productAttributes">Product attributes</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributesAsync(IList<ProductAttribute> productAttributes)
    {
		_logger.LogDebug(">>> Entering DeleteProductAttributesAsync(productAttributes={ProductAttributes})", productAttributes);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(productAttributes);

		_logger.LogInformation("DeleteProductAttributesAsync(productAttributes)={ProductAttributes}: external service call took {Arg1} ms", productAttributes, __logStart.ElapsedMilliseconds);
		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
        await _productAttributeRepository.DeleteAsync(productAttributes);
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
		_logger.LogDebug(">>> Entering GetAllProductAttributesAsync(name={Name},pageIndex={PageIndex},pageSize={PageSize})", name, pageIndex, pageSize);
        var query = _productAttributeRepository.Table;

		_logger.LogDebug("GetAllProductAttributesAsync(name={Name},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", name, pageIndex, pageSize, query);
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(pa => pa.Name.Contains(name));

		_logger.LogDebug("<<< Exiting GetAllProductAttributesAsync(name={Name},pageIndex={PageIndex},pageSize={PageSize})", name, pageIndex, pageSize);
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
		_logger.LogDebug(">>> Entering GetProductAttributeByIdAsync(productAttributeId={ProductAttributeId})", productAttributeId);
		_logger.LogInformation("GetProductAttributeByIdAsync(productAttributeId)={ProductAttributeId}: find query executing", productAttributeId);
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
		_logger.LogDebug(">>> Entering GetProductAttributeByIdsAsync(productAttributeIds={ProductAttributeIds})", productAttributeIds);
        return await _productAttributeRepository.GetByIdsAsync(productAttributeIds);
    }

    /// <summary>
    /// Inserts a product attribute
    /// </summary>
    /// <param name="productAttribute">Product attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeAsync(ProductAttribute productAttribute)
    {
		_logger.LogDebug(">>> Entering InsertProductAttributeAsync(productAttribute={ProductAttribute})", productAttribute);
        await _productAttributeRepository.InsertAsync(productAttribute);
    }

    /// <summary>
    /// Updates the product attribute
    /// </summary>
    /// <param name="productAttribute">Product attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeAsync(ProductAttribute productAttribute)
    {
		_logger.LogDebug(">>> Entering UpdateProductAttributeAsync(productAttribute={ProductAttribute})", productAttribute);
        await _productAttributeRepository.UpdateAsync(productAttribute);
    }

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
		_logger.LogDebug(">>> Entering GetNotExistingAttributesAsync(attributeId={AttributeId})", attributeId);
        ArgumentNullException.ThrowIfNull(attributeId);

        var query = _productAttributeRepository.Table;
		_logger.LogDebug("GetNotExistingAttributesAsync(attributeId={AttributeId}): query → {Query}", attributeId, query);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var queryFilter = attributeId.Distinct().ToArray();
        var filter = await query.Select(a => a.Id)
            .Where(m => queryFilter.Contains(m))
            .ToListAsync();
			_logger.LogInformation("GetNotExistingAttributesAsync(attributeId)={AttributeId}: database query executed in {Arg1} ms", attributeId, __logStart.ElapsedMilliseconds);
			_logger.LogDebug("GetNotExistingAttributesAsync(attributeId={AttributeId}): queryFilter → {QueryFilter}", attributeId, queryFilter);

		_logger.LogDebug("<<< Exiting GetNotExistingAttributesAsync(attributeId={AttributeId})", attributeId);
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
		_logger.LogDebug(">>> Entering DeleteProductAttributeMappingAsync(productAttributeMapping=<redacted>)");
        await _productAttributeMappingRepository.DeleteAsync(productAttributeMapping);
    }

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
		_logger.LogDebug(">>> Entering GetProductAttributeMappingsByProductIdAsync(productId={ProductId})", productId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var allCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeMappingsByProductCacheKey, productId);

		_logger.LogDebug("GetProductAttributeMappingsByProductIdAsync(productId={ProductId}): allCacheKey → {AllCacheKey}", productId, allCacheKey);
        var query = from pam in _productAttributeMappingRepository.Table
            orderby pam.DisplayOrder, pam.Id
            where pam.ProductId == productId
            select pam;

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("GetProductAttributeMappingsByProductIdAsync(productId)={ProductId}: external service call took {Arg1} ms", productId, __logStart.ElapsedMilliseconds);
		_logger.LogInformation("GetProductAttributeMappingsByProductIdAsync(productId)={ProductId}: find query executed in {Arg1} ms", productId, __logStartDb.ElapsedMilliseconds);
        var attributes = await _staticCacheManager.GetAsync(allCacheKey, async () => await query.ToListAsync()) ?? new List<ProductAttributeMapping>();
		_logger.LogDebug("GetProductAttributeMappingsByProductIdAsync(productId={ProductId}): attributes → {Attributes}", productId, attributes);

		_logger.LogDebug("<<< Exiting GetProductAttributeMappingsByProductIdAsync(productId={ProductId})", productId);
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
		_logger.LogDebug(">>> Entering GetProductAttributeMappingByIdAsync(productAttributeMappingId=<redacted>)");
		_logger.LogInformation("GetProductAttributeMappingByIdAsync(productAttributeMappingId)=<redacted>: find query executing");
        return await _productAttributeMappingRepository.GetByIdAsync(productAttributeMappingId, cache => default);
    }

    /// <summary>
    /// Inserts a product attribute mapping
    /// </summary>
    /// <param name="productAttributeMapping">The product attribute mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
    {
		_logger.LogDebug(">>> Entering InsertProductAttributeMappingAsync(productAttributeMapping=<redacted>)");
        await _productAttributeMappingRepository.InsertAsync(productAttributeMapping);
    }

    /// <summary>
    /// Updates the product attribute mapping
    /// </summary>
    /// <param name="productAttributeMapping">The product attribute mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
    {
		_logger.LogDebug(">>> Entering UpdateProductAttributeMappingAsync(productAttributeMapping=<redacted>)");
        await _productAttributeMappingRepository.UpdateAsync(productAttributeMapping);
    }

    #endregion

    #region Product attribute values

    /// <summary>
    /// Deletes a product attribute value
    /// </summary>
    /// <param name="productAttributeValue">Product attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
    {
		_logger.LogDebug(">>> Entering DeleteProductAttributeValueAsync(productAttributeValue={ProductAttributeValue})", productAttributeValue);
        await _productAttributeValueRepository.DeleteAsync(productAttributeValue);
    }

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
		_logger.LogDebug(">>> Entering GetProductAttributeValuesAsync(productAttributeMappingId=<redacted>)");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeValuesByAttributeCacheKey, productAttributeMappingId);

		_logger.LogWarning("GetProductAttributeValuesAsync(productAttributeMappingId=<redacted>): key → {Key}", key);
        var query = from pav in _productAttributeValueRepository.Table
            orderby pav.DisplayOrder, pav.Id
            where pav.ProductAttributeMappingId == productAttributeMappingId
            select pav;
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogInformation("GetProductAttributeValuesAsync(productAttributeMappingId)=<redacted>: external service call took {Arg1} ms", __logStart.ElapsedMilliseconds);
			_logger.LogInformation("GetProductAttributeValuesAsync(productAttributeMappingId)=<redacted>: find query executed in {Arg1} ms", __logStartDb.ElapsedMilliseconds);
        var productAttributeValues = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
		_logger.LogDebug("GetProductAttributeValuesAsync(productAttributeMappingId=<redacted>): productAttributeValues → {ProductAttributeValues}", productAttributeValues);

		_logger.LogDebug("<<< Exiting GetProductAttributeValuesAsync(productAttributeMappingId=<redacted>)");
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
		_logger.LogDebug(">>> Entering GetProductAttributeValueByIdAsync(productAttributeValueId={ProductAttributeValueId})", productAttributeValueId);
		_logger.LogInformation("GetProductAttributeValueByIdAsync(productAttributeValueId)={ProductAttributeValueId}: find query executing", productAttributeValueId);
        return await _productAttributeValueRepository.GetByIdAsync(productAttributeValueId, cache => default);
    }

    /// <summary>
    /// Inserts a product attribute value
    /// </summary>
    /// <param name="productAttributeValue">The product attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
    {
		_logger.LogDebug(">>> Entering InsertProductAttributeValueAsync(productAttributeValue={ProductAttributeValue})", productAttributeValue);
        await _productAttributeValueRepository.InsertAsync(productAttributeValue);
    }

    /// <summary>
    /// Updates the product attribute value
    /// </summary>
    /// <param name="productAttributeValue">The product attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
    {
		_logger.LogDebug(">>> Entering UpdateProductAttributeValueAsync(productAttributeValue={ProductAttributeValue})", productAttributeValue);
        await _productAttributeValueRepository.UpdateAsync(productAttributeValue);
    }

    #endregion

    #region Product attribute value pictures

    /// <summary>
    /// Deletes a list of product attribute value picture
    /// </summary>
    /// <param name="value">Product attribute value pictures</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributeValuePicturesAsync(IList<ProductAttributeValuePicture> valuePictures)
    {
		_logger.LogDebug(">>> Entering DeleteProductAttributeValuePicturesAsync(valuePictures={ValuePictures})", valuePictures);
        await _productAttributeValuePictureRepository.DeleteAsync(valuePictures);
    }

    /// <summary>
    /// Inserts a product attribute value picture
    /// </summary>
    /// <param name="value">Product attribute value picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeValuePictureAsync(ProductAttributeValuePicture valuePicture)
    {
		_logger.LogDebug(">>> Entering InsertProductAttributeValuePictureAsync(valuePicture={ValuePicture})", valuePicture);
        await _productAttributeValuePictureRepository.InsertAsync(valuePicture);
    }

    /// <summary>
    /// Updates a product attribute value picture
    /// </summary>
    /// <param name="value">Product attribute value picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeValuePictureAsync(ProductAttributeValuePicture valuePicture)
    {
		_logger.LogDebug(">>> Entering UpdateProductAttributeValuePictureAsync(valuePicture={ValuePicture})", valuePicture);
        await _productAttributeValuePictureRepository.UpdateAsync(valuePicture);
    }

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
		_logger.LogDebug(">>> Entering GetProductAttributeValuePicturesAsync(valueId={ValueId})", valueId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var allCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeValuePicturesByValueCacheKey, valueId);

		_logger.LogDebug("GetProductAttributeValuePicturesAsync(valueId={ValueId}): allCacheKey → {AllCacheKey}", valueId, allCacheKey);
        var query = from pacp in _productAttributeValuePictureRepository.Table
            join p in _pictureRepository.Table on pacp.PictureId equals p.Id
            join pp in _productPictureRepository.Table on p.Id equals pp.PictureId
            where pacp.ProductAttributeValueId == valueId
            orderby pp.DisplayOrder, pacp.PictureId
            select pacp;

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
        var valuePictures = await _staticCacheManager.GetAsync(allCacheKey, async () => await query.ToListAsync())
                            ?? new List<ProductAttributeValuePicture>();
							_logger.LogInformation("GetProductAttributeValuePicturesAsync(valueId)={ValueId}: find query executed in {Arg1} ms", valueId, __logStartDb.ElapsedMilliseconds);

		_logger.LogInformation("GetProductAttributeValuePicturesAsync(valueId)={ValueId}: external service call took {Arg1} ms", valueId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetProductAttributeValuePicturesAsync(valueId={ValueId})", valueId);
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
		_logger.LogDebug(">>> Entering FindProductAttributeValuePicture(source={Source},valueId={ValueId},pictureId={PictureId})", source, valueId, pictureId);
		_logger.LogDebug("<<< Exiting FindProductAttributeValuePicture(source={Source},valueId={ValueId},pictureId={PictureId})", source, valueId, pictureId);
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
		_logger.LogDebug(">>> Entering DeletePredefinedProductAttributeValueAsync(ppav={Ppav})", ppav);
        await _predefinedProductAttributeValueRepository.DeleteAsync(ppav);
    }

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
		_logger.LogDebug(">>> Entering GetPredefinedProductAttributeValuesAsync(productAttributeId={ProductAttributeId})", productAttributeId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.PredefinedProductAttributeValuesByAttributeCacheKey, productAttributeId);

		_logger.LogWarning("GetPredefinedProductAttributeValuesAsync(productAttributeId={ProductAttributeId}): key → {Key}", productAttributeId, key);
        var query = from ppav in _predefinedProductAttributeValueRepository.Table
            orderby ppav.DisplayOrder, ppav.Id
            where ppav.ProductAttributeId == productAttributeId
            select ppav;

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("GetPredefinedProductAttributeValuesAsync(productAttributeId)={ProductAttributeId}: external service call took {Arg1} ms", productAttributeId, __logStart.ElapsedMilliseconds);
		_logger.LogInformation("GetPredefinedProductAttributeValuesAsync(productAttributeId)={ProductAttributeId}: find query executed in {Arg1} ms", productAttributeId, __logStartDb.ElapsedMilliseconds);
        var values = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
		_logger.LogDebug("GetPredefinedProductAttributeValuesAsync(productAttributeId={ProductAttributeId}): values → {Values}", productAttributeId, values);

		_logger.LogDebug("<<< Exiting GetPredefinedProductAttributeValuesAsync(productAttributeId={ProductAttributeId})", productAttributeId);
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
		_logger.LogDebug(">>> Entering GetPredefinedProductAttributeValueByIdAsync(id={Id})", id);
		_logger.LogInformation("GetPredefinedProductAttributeValueByIdAsync(id)={Id}: find query executing", id);
        return await _predefinedProductAttributeValueRepository.GetByIdAsync(id, cache => default);
    }

    /// <summary>
    /// Inserts a predefined product attribute value
    /// </summary>
    /// <param name="ppav">The predefined product attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPredefinedProductAttributeValueAsync(PredefinedProductAttributeValue ppav)
    {
		_logger.LogDebug(">>> Entering InsertPredefinedProductAttributeValueAsync(ppav={Ppav})", ppav);
        await _predefinedProductAttributeValueRepository.InsertAsync(ppav);
    }

    /// <summary>
    /// Updates the predefined product attribute value
    /// </summary>
    /// <param name="ppav">The predefined product attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdatePredefinedProductAttributeValueAsync(PredefinedProductAttributeValue ppav)
    {
		_logger.LogDebug(">>> Entering UpdatePredefinedProductAttributeValueAsync(ppav={Ppav})", ppav);
        await _predefinedProductAttributeValueRepository.UpdateAsync(ppav);
    }

    #endregion

    #region Product attribute combinations

    /// <summary>
    /// Deletes a product attribute combination
    /// </summary>
    /// <param name="combination">Product attribute combination</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributeCombinationAsync(ProductAttributeCombination combination)
    {
		_logger.LogDebug(">>> Entering DeleteProductAttributeCombinationAsync(combination={Combination})", combination);
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
		_logger.LogDebug(">>> Entering GetAllProductAttributeCombinationsAsync(productId={ProductId})", productId);
        if (productId == 0)
            return new List<ProductAttributeCombination>();

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var combinations = await _productAttributeCombinationRepository.GetAllAsync(query =>
        {
            return from c in query
                orderby c.Id
                where c.ProductId == productId
                select c;
				_logger.LogInformation("GetAllProductAttributeCombinationsAsync(productId)={ProductId}: find query executed in {Arg1} ms", productId, __logStart.ElapsedMilliseconds);
        }, cache => cache.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeCombinationsByProductCacheKey, productId));

		_logger.LogDebug("<<< Exiting GetAllProductAttributeCombinationsAsync(productId={ProductId})", productId);
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
		_logger.LogDebug(">>> Entering GetProductAttributeCombinationByIdAsync(productAttributeCombinationId={ProductAttributeCombinationId})", productAttributeCombinationId);
		_logger.LogInformation("GetProductAttributeCombinationByIdAsync(productAttributeCombinationId)={ProductAttributeCombinationId}: find query executing", productAttributeCombinationId);
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
		_logger.LogDebug(">>> Entering GetProductAttributeCombinationBySkuAsync(sku={Sku})", sku);
        if (string.IsNullOrEmpty(sku))
            return null;

        sku = sku.Trim();
		_logger.LogDebug("GetProductAttributeCombinationBySkuAsync(sku={Sku}): sku → {Sku1}", sku, sku);

        var query = from pac in _productAttributeCombinationRepository.Table
            join p in _productRepository.Table on pac.ProductId equals p.Id
            orderby pac.Id
            where !p.Deleted && pac.Sku == sku
            select pac;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var combination = await query.FirstOrDefaultAsync();

		_logger.LogDebug("GetProductAttributeCombinationBySkuAsync(sku={Sku}): combination → {Combination}", sku, combination);
		_logger.LogInformation("GetProductAttributeCombinationBySkuAsync(sku)={Sku}: database query executed in {Arg1} ms", sku, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetProductAttributeCombinationBySkuAsync(sku={Sku})", sku);
        return combination;
    }

    /// <summary>
    /// Inserts a product attribute combination
    /// </summary>
    /// <param name="combination">Product attribute combination</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeCombinationAsync(ProductAttributeCombination combination)
    {
		_logger.LogDebug(">>> Entering InsertProductAttributeCombinationAsync(combination={Combination})", combination);
        await _productAttributeCombinationRepository.InsertAsync(combination);
    }

    /// <summary>
    /// Updates a product attribute combination
    /// </summary>
    /// <param name="combination">Product attribute combination</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeCombinationAsync(ProductAttributeCombination combination)
    {
		_logger.LogDebug(">>> Entering UpdateProductAttributeCombinationAsync(combination={Combination})", combination);
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
		_logger.LogDebug(">>> Entering DeleteProductAttributeCombinationPictureAsync(combinationPicture={CombinationPicture})", combinationPicture);
        await _productAttributeCombinationPictureRepository.DeleteAsync(combinationPicture);
    }

    /// <summary>
    /// Inserts a product attribute combination picture
    /// </summary>
    /// <param name="combination">Product attribute combination picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeCombinationPictureAsync(ProductAttributeCombinationPicture combinationPicture)
    {
		_logger.LogDebug(">>> Entering InsertProductAttributeCombinationPictureAsync(combinationPicture={CombinationPicture})", combinationPicture);
        await _productAttributeCombinationPictureRepository.InsertAsync(combinationPicture);
    }

    /// <summary>
    /// Updates a product attribute combination picture
    /// </summary>
    /// <param name="combination">Product attribute combination picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeCombinationPictureAsync(ProductAttributeCombinationPicture combinationPicture)
    {
		_logger.LogDebug(">>> Entering UpdateProductAttributeCombinationPictureAsync(combinationPicture={CombinationPicture})", combinationPicture);
        await _productAttributeCombinationPictureRepository.UpdateAsync(combinationPicture);
    }

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
		_logger.LogDebug(">>> Entering GetProductAttributeCombinationPicturesAsync(combinationId={CombinationId})", combinationId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var allCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeCombinationPicturesByCombinationCacheKey, combinationId);

		_logger.LogDebug("GetProductAttributeCombinationPicturesAsync(combinationId={CombinationId}): allCacheKey → {AllCacheKey}", combinationId, allCacheKey);
        var query = from pacp in _productAttributeCombinationPictureRepository.Table
            join p in _pictureRepository.Table on pacp.PictureId equals p.Id
            join pp in _productPictureRepository.Table on p.Id equals pp.PictureId
            where pacp.ProductAttributeCombinationId == combinationId
            orderby pp.DisplayOrder, pacp.PictureId
            select pacp;

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
        var combinationPictures = await _staticCacheManager.GetAsync(allCacheKey, async () => await query.ToListAsync()) 
                                  ?? new List<ProductAttributeCombinationPicture>();
								_logger.LogInformation("GetProductAttributeCombinationPicturesAsync(combinationId)={CombinationId}: find query executed in {Arg1} ms", combinationId, __logStartDb.ElapsedMilliseconds);

		_logger.LogInformation("GetProductAttributeCombinationPicturesAsync(combinationId)={CombinationId}: external service call took {Arg1} ms", combinationId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetProductAttributeCombinationPicturesAsync(combinationId={CombinationId})", combinationId);
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
		_logger.LogDebug(">>> Entering FindProductAttributeCombinationPicture(source={Source},combinationId={CombinationId},pictureId={PictureId})", source, combinationId, pictureId);
		_logger.LogDebug("<<< Exiting FindProductAttributeCombinationPicture(source={Source},combinationId={CombinationId},pictureId={PictureId})", source, combinationId, pictureId);
        return source.FirstOrDefault(pacp => pacp.ProductAttributeCombinationId == combinationId && pacp.PictureId == pictureId);
    }

    #endregion

    #endregion
}