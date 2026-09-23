using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Infrastructure.Cache;

/// <summary>
/// Event consumer of the "Fixed or by weight" shipping plugin (used for removing unused settings)
/// </summary>
public class FixedByWeightByTotalEventConsumer : IConsumer<EntityDeletedEvent<ShippingMethod>>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public FixedByWeightByTotalEventConsumer(ISettingService settingService)
    {
		_logger.LogError(">>> Entering FixedByWeightByTotalEventConsumer(settingService={SettingService})", settingService);
        _settingService = settingService;
		_logger.LogDebug("FixedByWeightByTotalEventConsumer(settingService={SettingService}): _settingService → {_settingService}", settingService, _settingService);
    _logger.LogWarning("<<< Exiting FixedByWeightByTotalEventConsumer(settingService={SettingService})", settingService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle shipping method deleted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<ShippingMethod> eventMessage)
    {
		_logger.LogError(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        var shippingMethod = eventMessage?.Entity;
        if (shippingMethod == null)
            return;

        //delete saved fixed rate if exists
        var setting = await _settingService.GetSettingAsync(string.Format(FixedByWeightByTotalDefaults.FIXED_RATE_SETTINGS_KEY, shippingMethod.Id));
		_logger.LogError("HandleEventAsync(eventMessage={EventMessage}): setting → {Setting}", eventMessage, setting);
        if (setting != null)
            await _settingService.DeleteSettingAsync(setting);
			_logger.LogWarning("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
    }

    #endregion
}