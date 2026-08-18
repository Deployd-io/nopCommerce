using Nop.Core.Domain.Media;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.AzureBlob.Services.Caching;

/// <summary>
/// Represents a picture cache event consumer
/// </summary>
public class PictureCacheEventConsumer : CacheEventConsumer<Picture>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(Picture entity)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveByPrefixAsync(AzureBlobDefaults.ThumbsExistsPrefix);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}