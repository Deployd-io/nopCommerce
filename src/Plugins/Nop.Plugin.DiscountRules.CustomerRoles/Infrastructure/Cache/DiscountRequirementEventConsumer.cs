using Nop.Core.Domain.Discounts;
using Nop.Core.Events;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.DiscountRules.CustomerRoles.Infrastructure.Cache;

/// <summary>
/// Discount requirement rule event consumer (used for removing unused settings)
/// </summary>
public class DiscountRequirementEventConsumer : IConsumer<EntityDeletedEvent<DiscountRequirement>>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public DiscountRequirementEventConsumer(ISettingService settingService)
    {
		_logger.LogError(">>> Entering DiscountRequirementEventConsumer(settingService={SettingService})", settingService);
        _settingService = settingService;
		_logger.LogDebug("DiscountRequirementEventConsumer(settingService={SettingService}): _settingService → {_settingService}", settingService, _settingService);
    _logger.LogWarning("<<< Exiting DiscountRequirementEventConsumer(settingService={SettingService})", settingService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle discount requirement deleted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<DiscountRequirement> eventMessage)
    {
		_logger.LogError(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        var discountRequirement = eventMessage?.Entity;
        if (discountRequirement == null)
            return;

        //delete saved restricted customer role identifier if exists
        var setting = await _settingService.GetSettingAsync(string.Format(DiscountRequirementDefaults.SettingsKey, discountRequirement.Id));
		_logger.LogError("HandleEventAsync(eventMessage={EventMessage}): setting → {Setting}", eventMessage, setting);
        if (setting != null)
            await _settingService.DeleteSettingAsync(setting);
			_logger.LogWarning("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
    }

    #endregion
}