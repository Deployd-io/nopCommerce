using Nop.Core.Domain.Localization;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Localization.Caching;

/// <summary>
/// Represents a locale string resource cache event consumer
/// </summary>
public partial class LocaleStringResourceCacheEventConsumer : CacheEventConsumer<LocaleStringResource>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache by entity event type
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(LocaleStringResource entity)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopLocalizationDefaults.LocaleStringResourcesAllPublicCacheKey, entity.LanguageId);
        await RemoveAsync(NopLocalizationDefaults.LocaleStringResourcesAllAdminCacheKey, entity.LanguageId);
        await RemoveAsync(NopLocalizationDefaults.LocaleStringResourcesAllCacheKey, entity.LanguageId);
        await RemoveByPrefixAsync(NopLocalizationDefaults.LocaleStringResourcesByNamePrefix, entity.LanguageId);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}