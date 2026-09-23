using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;
using Nop.Services.Discounts;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a category cache event consumer
/// </summary>
public partial class CategoryCacheEventConsumer : CacheEventConsumer<Category>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="entityEventType">Entity event type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(Category entity, EntityEventType entityEventType)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
        await RemoveByPrefixAsync(NopCatalogDefaults.CategoriesByParentCategoryPrefix, entity);
        await RemoveByPrefixAsync(NopCatalogDefaults.CategoriesByParentCategoryPrefix, entity.ParentCategoryId);
        await RemoveByPrefixAsync(NopCatalogDefaults.CategoriesChildIdsPrefix, entity);
        await RemoveByPrefixAsync(NopCatalogDefaults.CategoriesChildIdsPrefix, entity.ParentCategoryId);
        await RemoveByPrefixAsync(NopCatalogDefaults.CategoriesHomepagePrefix);
        await RemoveByPrefixAsync(NopCatalogDefaults.CategoryBreadcrumbPrefix);
        await RemoveByPrefixAsync(NopCatalogDefaults.CategoryProductsNumberPrefix);
        await RemoveByPrefixAsync(NopDiscountDefaults.CategoryIdsPrefix);

        if (entityEventType == EntityEventType.Delete)
        {
            await RemoveAsync(NopCatalogDefaults.SpecificationAttributeOptionsByCategoryCacheKey, entity);
            await RemoveByPrefixAsync(NopCatalogDefaults.ManufacturersByCategoryWithIdPrefix, entity);
        }

        if (entityEventType == EntityEventType.Delete || !entity.Published)
                await RemoveByPrefixAsync(NopCatalogDefaults.ProductCategoriesByProductWithoutIdPrefix);
        
        await RemoveAsync(NopDiscountDefaults.AppliedDiscountsCacheKey, nameof(Category), entity);

        await base.ClearCacheAsync(entity, entityEventType);
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
    }
}