using Nop.Core.Domain.Messages;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Messages.Caching;

/// <summary>
/// Represents a message template cache event consumer
/// </summary>
public partial class MessageTemplateCacheEventConsumer : CacheEventConsumer<MessageTemplate>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(MessageTemplate entity)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveByPrefixAsync(NopMessageDefaults.MessageTemplatesByNamePrefix, entity.Name);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}