using Nop.Core.Domain.Discounts;
using Nop.Core.Events;
using Nop.Services.Configuration;
using Nop.Services.Events;

namespace Nop.Plugin.DiscountRules.CustomerRoles.Infrastructure.Cache;

/// <summary>
/// Discount requirement rule event consumer (used for removing unused settings)
/// </summary>
public class DiscountRequirementEventConsumer : IConsumer<EntityDeletedEvent<DiscountRequirement>>
{
    #region Fields

    protected readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public DiscountRequirementEventConsumer(ISettingService settingService)
    {
		logger.LogDebug(">>> Entering DiscountRequirementEventConsumer(settingService={SettingService})", settingService);
        _settingService = settingService;
		logger.LogDebug("<<< Exiting DiscountRequirementEventConsumer(settingService={SettingService})", settingService);
		logger.LogDebug("DiscountRequirementEventConsumer(settingService={SettingService}): _settingService → {_settingService}", settingService, _settingService);
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
		logger.LogDebug(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        var discountRequirement = eventMessage?.Entity;
        if (discountRequirement == null)
            return;

        //delete saved restricted customer role identifier if exists
        var setting = await _settingService.GetSettingAsync(string.Format(DiscountRequirementDefaults.SettingsKey, discountRequirement.Id));
        if (setting != null)
			logger.LogDebug("HandleEventAsync(eventMessage={EventMessage}): setting → {Setting}", eventMessage, setting);
            await _settingService.DeleteSettingAsync(setting);
			logger.LogDebug("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
    }

    #endregion
}
