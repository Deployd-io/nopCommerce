using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Services.Attributes;
using Nop.Services.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Orders;

/// <summary>
/// Checkout attribute service extensions
/// </summary>
public static partial class CheckoutAttributeServiceExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Gets all checkout attributes
    /// </summary>
    /// <param name="service">Checkout attribute service</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="excludeShippableAttributes">A value indicating whether we should exclude shippable attributes</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the checkout attributes
    /// </returns>
    public static async Task<IList<CheckoutAttribute>> GetAllAttributesAsync(this IAttributeService<CheckoutAttribute, CheckoutAttributeValue> service, IStaticCacheManager staticCacheManager, IStoreMappingService storeMappingService, int storeId = 0, bool excludeShippableAttributes = false)
    {
		_logger.LogDebug(">>> Entering GetAllAttributesAsync(service={Service},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,storeId={StoreId},excludeShippableAttributes={ExcludeShippableAttributes})", service, staticCacheManager, storeId, excludeShippableAttributes);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = staticCacheManager.PrepareKeyForDefaultCache(NopOrderDefaults.CheckoutAttributesAllCacheKey, storeId, excludeShippableAttributes);

		_logger.LogWarning("GetAllAttributesAsync(service={Service},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,storeId={StoreId},excludeShippableAttributes={ExcludeShippableAttributes}): key → {Key}", service, staticCacheManager, storeId, excludeShippableAttributes, key);
        return await staticCacheManager.GetAsync(key, async () =>
        {
            var checkoutAttributes = (await service.GetAllAttributesAsync()).ToAsyncEnumerable();

            if (storeId > 0)
                //store mapping
                checkoutAttributes = checkoutAttributes.WhereAwait(async ca => await storeMappingService.AuthorizeAsync(ca, storeId));

            if (excludeShippableAttributes)
                //remove attributes which require shippable products
                checkoutAttributes = checkoutAttributes.Where(x => !x.ShippableProductRequired);

			_logger.LogInformation("GetAllAttributesAsync(service,staticCacheManager,storeMappingService,storeId,excludeShippableAttributes)={Service},{StaticCacheManager},{StoreMappingService},{StoreId},{ExcludeShippableAttributes}: database query executed in {Arg5} ms", service, staticCacheManager, storeMappingService, storeId, excludeShippableAttributes, __logStart.ElapsedMilliseconds);
			_logger.LogInformation("GetAllAttributesAsync(service,staticCacheManager,storeMappingService,storeId,excludeShippableAttributes)={Service},{StaticCacheManager},{StoreMappingService},{StoreId},{ExcludeShippableAttributes}: external service call took {Arg5} ms", service, staticCacheManager, storeMappingService, storeId, excludeShippableAttributes, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetAllAttributesAsync(service={Service},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>,storeId={StoreId},excludeShippableAttributes={ExcludeShippableAttributes})", service, staticCacheManager, storeId, excludeShippableAttributes);
            return await checkoutAttributes.ToListAsync();
        });
    }
}