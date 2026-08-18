using Nop.Core.Domain.Security;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Security.Caching;

/// <summary>
/// Represents a permission record cache event consumer
/// </summary>
public partial class PermissionRecordCacheEventConsumer : CacheEventConsumer<PermissionRecord>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(PermissionRecord entity)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveByPrefixAsync(NopSecurityDefaults.PermissionAllowedPrefix, entity.SystemName);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}