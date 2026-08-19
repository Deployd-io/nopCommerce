using Nop.Core.Domain.Common;
using Nop.Services.Configuration;
using Nop.Services.ScheduleTasks;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Common;

/// <summary>
/// Represents a task to reset license check
/// </summary>
public partial class ResetLicenseCheckTask : IScheduleTask
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public ResetLicenseCheckTask(ISettingService settingService)
    {
		_logger.LogDebug(">>> Entering ResetLicenseCheckTask(settingService={SettingService})", settingService);
        _settingService = settingService;
		_logger.LogDebug("ResetLicenseCheckTask(settingService={SettingService}): _settingService → {_settingService}", settingService, _settingService);
		_logger.LogDebug("<<< Exiting ResetLicenseCheckTask(settingService={SettingService})", settingService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ExecuteAsync()
    {
		_logger.LogDebug(">>> Entering ExecuteAsync()");
        await _settingService.SetSettingAsync($"{nameof(AdminAreaSettings)}.{nameof(AdminAreaSettings.CheckLicense)}", true);
		_logger.LogDebug("<<< Exiting ExecuteAsync()");
    }

    #endregion
}