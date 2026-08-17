using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog;

/// <summary>
/// Product tag service
/// </summary>
public partial class ProductTagService : IProductTagService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly ICustomerService _customerService;
    protected readonly IRepository<Product> _productRepository;
    protected readonly IRepository<ProductProductTagMapping> _productProductTagMappingRepository;
    protected readonly IRepository<ProductTag> _productTagRepository;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public ProductTagService(
        IAclService aclService,
        ICustomerService customerService,
        IRepository<Product> productRepository,
        IRepository<ProductProductTagMapping> productProductTagMappingRepository,
        IRepository<ProductTag> productTagRepository,
        IStaticCacheManager staticCacheManager,
        IStoreMappingService storeMappingService,
        IUrlRecordService urlRecordService,
        IWorkContext workContext)
    {
        _aclService = aclService;
        _customerService = customerService;
        _productRepository = productRepository;
        _productProductTagMappingRepository = productProductTagMappingRepository;
        _productTagRepository = productTagRepository;
        _staticCacheManager = staticCacheManager;
        _storeMappingService = storeMappingService;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Delete a product-product tag mapping
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="productTagIds">Product tag identifiers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task DeleteProductProductTagMappingAsync(int productId, IList<int> productTagIds)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogDebug(">>> Entering DeleteProductProductTagMappingAsync(productId={ProductId},productTagIds={ProductTagIds})", productId, productTagIds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var mappingRecords = await _productProductTagMappingRepository.Table
            .Where(pptm => pptm.ProductId == productId && productTagIds.Contains(pptm.ProductTagId))
                                .ToListAsync();
								var __logStart = System.Diagnostics.Stopwatch.StartNew();
								_logger.LogInformation("DeleteProductProductTagMappingAsync(productId,productTagIds)={ProductId},{ProductTagIds}: database query executed in {Arg2} ms", productId, productTagIds, __logStart.ElapsedMilliseconds);

        await _productProductTagMappingRepository.DeleteAsync(mappingRecords);
		_logger.LogDebug("<<< Exiting DeleteProductProductTagMappingAsync(productId={ProductId},productTagIds={ProductTagIds})", productId, productTagIds);
		_logger.LogInformation("DeleteProductProductTagMappingAsync(productId,productTagIds)={ProductId},{ProductTagIds}: external service call took {Arg2} ms", productId, productTagIds, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Indicates whether a product tag exists
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="productTagId">Product tag identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    protected virtual async Task<bool> ProductTagExistsAsync(Product product, int productTagId)
    {
		_logger.LogDebug(">>> Entering ProductTagExistsAsync(product={Product},productTagId={ProductTagId})", product, productTagId);
        ArgumentNullException.ThrowIfNull(product);

		_logger.LogDebug("<<< Exiting ProductTagExistsAsync(product={Product},productTagId={ProductTagId})", product, productTagId);
        return await _productProductTagMappingRepository.Table
            .AnyAsync(pptm => pptm.ProductId == product.Id && pptm.ProductTagId == productTagId);
    }

    /// <summary>
    /// Gets product tag by name
    /// </summary>
    /// <param name="name">Product tag name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product tag
    /// </returns>
    protected virtual async Task<ProductTag> GetProductTagByNameAsync(string name)
    {
		_logger.LogDebug(">>> Entering GetProductTagByNameAsync(name={Name})", name);
        var query = from pt in _productTagRepository.Table
            where pt.Name == name
            select pt;

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var productTag = await query.FirstOrDefaultAsync();
		_logger.LogDebug("GetProductTagByNameAsync(name={Name}): productTag → {ProductTag}", name, productTag);
		_logger.LogDebug("<<< Exiting GetProductTagByNameAsync(name={Name})", name);
        return productTag;
    }

    /// <summary>
    /// Inserts a product tag
    /// </summary>
    /// <param name="productTag">Product tag</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InsertProductTagAsync(ProductTag productTag)
    {
        await _productTagRepository.InsertAsync(productTag);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Delete a product tag
    /// </summary>
    /// <param name="productTag">Product tag</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductTagAsync(ProductTag productTag)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productTagRepository.DeleteAsync(productTag);
    }

    /// <summary>
    /// Delete product tags
    /// </summary>
    /// <param name="productTags">Product tags</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductTagsAsync(IList<ProductTag> productTags)
    {
		_logger.LogDebug(">>> Entering DeleteProductTagsAsync(productTags={ProductTags})", productTags);
        ArgumentNullException.ThrowIfNull(productTags);

        foreach (var productTag in productTags)
            await DeleteProductTagAsync(productTag);
			_logger.LogDebug("<<< Exiting DeleteProductTagsAsync(productTags={ProductTags})", productTags);
    }

    /// <summary>
    /// Gets all product tags
    /// </summary>
    /// <param name="tagName">Tag name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product tags
    /// </returns>
    public virtual async Task<IList<ProductTag>> GetAllProductTagsAsync(string tagName = null)
    {
		_logger.LogDebug(">>> Entering GetAllProductTagsAsync(tagName={TagName})", tagName);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var allProductTags = await _productTagRepository.GetAllAsync(query => query, getCacheKey: cache => default);

        if (!string.IsNullOrEmpty(tagName))
			_logger.LogDebug("GetAllProductTagsAsync(tagName={TagName}): allProductTags → {AllProductTags}", tagName, allProductTags);
			_logger.LogInformation("GetAllProductTagsAsync(tagName)={TagName}: find query executed in {Arg1} ms", tagName, __logStart.ElapsedMilliseconds);
            allProductTags = allProductTags.Where(tag => tag.Name.Contains(tagName)).ToList();

		_logger.LogDebug("<<< Exiting GetAllProductTagsAsync(tagName={TagName})", tagName);
        return allProductTags;
    }

    /// <summary>
    /// Gets all product tags by product identifier
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product tags
    /// </returns>
    public virtual async Task<IList<ProductTag>> GetAllProductTagsByProductIdAsync(int productId)
    {
		_logger.LogDebug(">>> Entering GetAllProductTagsByProductIdAsync(productId={ProductId})", productId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductTagsByProductCacheKey, productId);

		_logger.LogWarning("GetAllProductTagsByProductIdAsync(productId={ProductId}): key → {Key}", productId, key);
        return await _staticCacheManager.GetAsync(key, async () =>
        {
            var tagMapping = from ptm in _productProductTagMappingRepository.Table
                join pt in _productTagRepository.Table on ptm.ProductTagId equals pt.Id
                where ptm.ProductId == productId
                orderby pt.Id
                select pt;

			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogInformation("GetAllProductTagsByProductIdAsync(productId)={ProductId}: database query executed in {Arg1} ms", productId, __logStart.ElapsedMilliseconds);
            return await tagMapping.ToListAsync();
        });
		_logger.LogInformation("GetAllProductTagsByProductIdAsync(productId)={ProductId}: external service call took {Arg1} ms", productId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetAllProductTagsByProductIdAsync(productId={ProductId})", productId);
    }

    /// <summary>
    /// Gets product tag
    /// </summary>
    /// <param name="productTagId">Product tag identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product tag
    /// </returns>
    public virtual async Task<ProductTag> GetProductTagByIdAsync(int productTagId)
    {
		_logger.LogDebug(">>> Entering GetProductTagByIdAsync(productTagId={ProductTagId})", productTagId);
		_logger.LogDebug("<<< Exiting GetProductTagByIdAsync(productTagId={ProductTagId})", productTagId);
        return await _productTagRepository.GetByIdAsync(productTagId, cache => default);
    }

    /// <summary>
    /// Gets product tags
    /// </summary>
    /// <param name="productTagIds">Product tags identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product tags
    /// </returns>
    public virtual async Task<IList<ProductTag>> GetProductTagsByIdsAsync(int[] productTagIds)
    {
		_logger.LogDebug(">>> Entering GetProductTagsByIdsAsync(productTagIds={ProductTagIds})", productTagIds);
		_logger.LogDebug("<<< Exiting GetProductTagsByIdsAsync(productTagIds={ProductTagIds})", productTagIds);
        return await _productTagRepository.GetByIdsAsync(productTagIds);
    }

    /// <summary>
    /// Inserts a product-product tag mapping
    /// </summary>
    /// <param name="tagMapping">Product-product tag mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductProductTagMappingAsync(ProductProductTagMapping tagMapping)
    {
        await _productProductTagMappingRepository.InsertAsync(tagMapping);
    }

    /// <summary>
    /// Updates the product tag
    /// </summary>
    /// <param name="productTag">Product tag</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductTagAsync(ProductTag productTag)
    {
        ArgumentNullException.ThrowIfNull(productTag);

        await _productTagRepository.UpdateAsync(productTag);

        var seName = await _urlRecordService.ValidateSeNameAsync(productTag, string.Empty, productTag.Name, true);
        await _urlRecordService.SaveSlugAsync(productTag, seName, 0);
    }

    /// <summary>
    /// Get products quantity linked to a passed tag identifier
    /// </summary>
    /// <param name="productTagId">Product tag identifier</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of products
    /// </returns>
    public virtual async Task<int> GetProductCountByProductTagIdAsync(int productTagId, int storeId, bool showHidden = false)
    {
		_logger.LogDebug(">>> Entering GetProductCountByProductTagIdAsync(productTagId={ProductTagId},storeId={StoreId},showHidden={ShowHidden})", productTagId, storeId, showHidden);
        var dictionary = await GetProductCountAsync(storeId, showHidden);
        if (dictionary.TryGetValue(productTagId, out var value))
			_logger.LogDebug("GetProductCountByProductTagIdAsync(productTagId={ProductTagId},storeId={StoreId},showHidden={ShowHidden}): dictionary → {Dictionary}", productTagId, storeId, showHidden, dictionary);
            return value;

        return 0;
    }

    /// <summary>
    /// Get product count for every linked tag
    /// </summary>
    /// <param name="storeId">Store identifier</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dictionary of "product tag ID : product count"
    /// </returns>
    public virtual async Task<Dictionary<int, int>> GetProductCountAsync(int storeId, bool showHidden = false)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductTagCountCacheKey, storeId, customerRoleIds, showHidden);

        return await _staticCacheManager.GetAsync(key, async () =>
        {
            var query = _productProductTagMappingRepository.Table;
            var productsQuery = _productRepository.Table.Where(p => !p.Deleted);

            if (!showHidden || storeId > 0)
            {
                //apply store mapping constraints
                productsQuery = await _storeMappingService.ApplyStoreMapping(productsQuery, storeId);
            }

            if (!showHidden)
            {
                productsQuery = productsQuery.Where(p => p.Published);

                //apply ACL constraints
                productsQuery = await _aclService.ApplyAcl(productsQuery, customerRoleIds);
            }

            query = query.Where(pc => productsQuery.Any(p => pc.ProductId == p.Id));

            var pTagCount = from pt in _productTagRepository.Table
                join ptm in query on pt.Id equals ptm.ProductTagId
                group ptm by ptm.ProductTagId into ptmGrouped
                select new
                {
                    ProductTagId = ptmGrouped.Key,
                    ProductCount = ptmGrouped.Count()
                };

            return await pTagCount.ToDictionaryAsync(item => item.ProductTagId, item => item.ProductCount);
        });
    }

    /// <summary>
    /// Update product tags
    /// </summary>
    /// <param name="product">Product for update</param>
    /// <param name="productTags">Product tags</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductTagsAsync(Product product, string[] productTags)
    {
		_logger.LogDebug(">>> Entering UpdateProductTagsAsync(product={Product},productTags={ProductTags})", product, productTags);
        ArgumentNullException.ThrowIfNull(product);

        //product tags
        var existingProductTags = await GetAllProductTagsByProductIdAsync(product.Id);
		_logger.LogDebug("UpdateProductTagsAsync(product={Product},productTags={ProductTags}): existingProductTags → {ExistingProductTags}", product, productTags, existingProductTags);
        var productTagIdsToRemove = new List<int>();
		_logger.LogDebug("UpdateProductTagsAsync(product={Product},productTags={ProductTags}): productTagIdsToRemove → {ProductTagIdsToRemove}", product, productTags, productTagIdsToRemove);
        foreach (var existingProductTag in existingProductTags)
        {
            var found = false;
			_logger.LogDebug("UpdateProductTagsAsync(product={Product},productTags={ProductTags}): found → {Found}", product, productTags, found);
            foreach (var newProductTag in productTags)
            {
                if (!existingProductTag.Name.Equals(newProductTag, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                found = true;
                break;
            }

            if (!found)
                productTagIdsToRemove.Add(existingProductTag.Id);
        }
        
        await DeleteProductProductTagMappingAsync(product.Id, productTagIdsToRemove);

        foreach (var productTagName in productTags)
        {
            ProductTag productTag;
            var productTag2 = await GetProductTagByNameAsync(productTagName);
			_logger.LogDebug("UpdateProductTagsAsync(product={Product},productTags={ProductTags}): productTag2 → {ProductTag2}", product, productTags, productTag2);
            if (productTag2 == null)
            {
                //add new product tag
                productTag = new ProductTag
				_logger.LogDebug("UpdateProductTagsAsync(product={Product},productTags={ProductTags}): productTag → {ProductTag}", product, productTags, productTag);
                {
                    Name = productTagName
                };
                await InsertProductTagAsync(productTag);
            }
            else
                productTag = productTag2;

            if (!await ProductTagExistsAsync(product, productTag.Id))
                await InsertProductProductTagMappingAsync(new ProductProductTagMapping { ProductTagId = productTag.Id, ProductId = product.Id });

            var seName = await _urlRecordService.ValidateSeNameAsync(productTag, string.Empty, productTag.Name, true);
			_logger.LogDebug("UpdateProductTagsAsync(product={Product},productTags={ProductTags}): seName → {SeName}", product, productTags, seName);
            await _urlRecordService.SaveSlugAsync(productTag, seName, 0);
        }

        //cache
        await _staticCacheManager.RemoveByPrefixAsync(NopEntityCacheDefaults<ProductTag>.Prefix);
		_logger.LogDebug("<<< Exiting UpdateProductTagsAsync(product={Product},productTags={ProductTags})", product, productTags);
    }

    #endregion
}