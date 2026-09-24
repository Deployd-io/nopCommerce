using Nop.Core.Domain.Localization;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Localization.Caching;

/// <summary>
/// Represents a localized property cache event consumer
/// </summary>
public partial class LocalizedPropertyCacheEventConsumer : CacheEventConsumer<LocalizedProperty>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(LocalizedProperty entity)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopLocalizationDefaults.LocalizedPropertyCacheKey, entity.LanguageId, entity.EntityId, entity.LocaleKeyGroup, entity.LocaleKey);
        await RemoveAsync(NopLocalizationDefaults.LocalizedPropertiesCacheKey, entity.EntityId, entity.LocaleKeyGroup, entity.LocaleKey);
        await RemoveAsync(NopLocalizationDefaults.LocalizedPropertyLookupCacheKey, entity.LanguageId);
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}