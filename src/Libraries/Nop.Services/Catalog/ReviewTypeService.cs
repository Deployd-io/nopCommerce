using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog;

/// <summary>
/// Review type service implementation
/// </summary>
public partial class ReviewTypeService : IReviewTypeService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<ProductReviewReviewTypeMapping> _productReviewReviewTypeMappingRepository;
    protected readonly IRepository<ReviewType> _reviewTypeRepository;
    protected readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public ReviewTypeService(IRepository<ProductReviewReviewTypeMapping> productReviewReviewTypeMappingRepository,
        IRepository<ReviewType> reviewTypeRepository,
        IStaticCacheManager staticCacheManager)
    {
		_logger.LogDebug(">>> Entering ReviewTypeService(productReviewReviewTypeMappingRepository=<redacted>,reviewTypeRepository={ReviewTypeRepository},staticCacheManager={StaticCacheManager})", reviewTypeRepository, staticCacheManager);
        _productReviewReviewTypeMappingRepository = productReviewReviewTypeMappingRepository;
		_logger.LogDebug("ReviewTypeService(productReviewReviewTypeMappingRepository=<redacted>,reviewTypeRepository={ReviewTypeRepository},staticCacheManager={StaticCacheManager}): _productReviewReviewTypeMappingRepository → {_productReviewReviewTypeMappingRepository}", reviewTypeRepository, staticCacheManager, _productReviewReviewTypeMappingRepository);
        _reviewTypeRepository = reviewTypeRepository;
		_logger.LogDebug("ReviewTypeService(productReviewReviewTypeMappingRepository=<redacted>,reviewTypeRepository={ReviewTypeRepository},staticCacheManager={StaticCacheManager}): _reviewTypeRepository → {_reviewTypeRepository}", reviewTypeRepository, staticCacheManager, _reviewTypeRepository);
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods

    #region Review type

    /// <summary>
    /// Gets all review types
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the review types
    /// </returns>
    public virtual async Task<IList<ReviewType>> GetAllReviewTypesAsync()
    {
		_logger.LogDebug(">>> Entering GetAllReviewTypesAsync()");
        return await _reviewTypeRepository.GetAllAsync(
            query => query.OrderBy(reviewType => reviewType.DisplayOrder).ThenBy(reviewType => reviewType.Id),
            cache => default);
    }

    /// <summary>
    /// Gets a review type 
    /// </summary>
    /// <param name="reviewTypeId">Review type identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the review type
    /// </returns>
    public virtual async Task<ReviewType> GetReviewTypeByIdAsync(int reviewTypeId)
    {
		_logger.LogDebug(">>> Entering GetReviewTypeByIdAsync(reviewTypeId={ReviewTypeId})", reviewTypeId);
		_logger.LogInformation("GetReviewTypeByIdAsync(reviewTypeId)={ReviewTypeId}: find query executing", reviewTypeId);
        return await _reviewTypeRepository.GetByIdAsync(reviewTypeId, cache => default);
    }

    /// <summary>
    /// Inserts a review type
    /// </summary>
    /// <param name="reviewType">Review type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertReviewTypeAsync(ReviewType reviewType)
    {
		_logger.LogDebug(">>> Entering InsertReviewTypeAsync(reviewType={ReviewType})", reviewType);
        await _reviewTypeRepository.InsertAsync(reviewType);
    }

    /// <summary>
    /// Updates a review type
    /// </summary>
    /// <param name="reviewType">Review type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateReviewTypeAsync(ReviewType reviewType)
    {
		_logger.LogDebug(">>> Entering UpdateReviewTypeAsync(reviewType={ReviewType})", reviewType);
        await _reviewTypeRepository.UpdateAsync(reviewType);
    }

    /// <summary>
    /// Delete review type
    /// </summary>
    /// <param name="reviewType">Review type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteReviewTypeAsync(ReviewType reviewType)
    {
		_logger.LogDebug(">>> Entering DeleteReviewTypeAsync(reviewType={ReviewType})", reviewType);
        await _reviewTypeRepository.DeleteAsync(reviewType);
    }

    #endregion

    #region Product review type mapping

    /// <summary>
    /// Gets product review and review type mappings by product review identifier
    /// </summary>
    /// <param name="productReviewId">The product review identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product review and review type mapping collection
    /// </returns>
    public virtual async Task<IList<ProductReviewReviewTypeMapping>> GetProductReviewReviewTypeMappingsByProductReviewIdAsync(
        int productReviewId)
    {
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductReviewTypeMappingByReviewIdCacheKey, productReviewId);
		_logger.LogDebug(">>> Entering GetProductReviewReviewTypeMappingsByProductReviewIdAsync(productReviewId={ProductReviewId})", productReviewId);

        var query = from pam in _productReviewReviewTypeMappingRepository.Table
            orderby pam.Id
            where pam.ProductReviewId == productReviewId
            select pam;

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("GetProductReviewReviewTypeMappingsByProductReviewIdAsync(productReviewId)={ProductReviewId}: external service call took {Arg1} ms", productReviewId, __logStart.ElapsedMilliseconds);
		_logger.LogInformation("GetProductReviewReviewTypeMappingsByProductReviewIdAsync(productReviewId)={ProductReviewId}: find query executed in {Arg1} ms", productReviewId, __logStartDb.ElapsedMilliseconds);
        var productReviewReviewTypeMappings = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
		_logger.LogDebug("GetProductReviewReviewTypeMappingsByProductReviewIdAsync(productReviewId={ProductReviewId}): productReviewReviewTypeMappings → {ProductReviewReviewTypeMappings}", productReviewId, productReviewReviewTypeMappings);

		_logger.LogDebug("<<< Exiting GetProductReviewReviewTypeMappingsByProductReviewIdAsync(productReviewId={ProductReviewId})", productReviewId);
        return productReviewReviewTypeMappings;
    }

    /// <summary>
    /// Inserts a product review and review type mapping
    /// </summary>
    /// <param name="productReviewReviewType">Product review and review type mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductReviewReviewTypeMappingsAsync(ProductReviewReviewTypeMapping productReviewReviewType)
    {
		_logger.LogDebug(">>> Entering InsertProductReviewReviewTypeMappingsAsync(productReviewReviewType={ProductReviewReviewType})", productReviewReviewType);
        await _productReviewReviewTypeMappingRepository.InsertAsync(productReviewReviewType);
    }

    #endregion

    #endregion
}