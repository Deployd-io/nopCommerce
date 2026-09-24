using Nop.Core.Domain.Seo;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Seo.Caching;

/// <summary>
/// Represents an URL record cache event consumer
/// </summary>
public partial class UrlRecordCacheEventConsumer : CacheEventConsumer<UrlRecord>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(UrlRecord entity)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopSeoDefaults.UrlRecordCacheKey, entity.EntityId, entity.EntityName, entity.LanguageId);
        await RemoveAsync(NopSeoDefaults.UrlRecordBySlugCacheKey, entity.Slug);
        await RemoveAsync(NopSeoDefaults.UrlRecordEntityIdLookupCacheKey, entity.LanguageId);
        await RemoveAsync(NopSeoDefaults.UrlRecordSlugLookupCacheKey);
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}