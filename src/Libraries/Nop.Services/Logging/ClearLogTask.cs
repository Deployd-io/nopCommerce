using Nop.Core.Domain.Common;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Logging;

/// <summary>
/// Represents a task to clear [Log] table
/// </summary>
public partial class ClearLogTask : IScheduleTask
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CommonSettings _commonSettings;
    protected readonly ILogger _logger;

    #endregion

    #region Ctor

    public ClearLogTask(CommonSettings commonSettings,
        ILogger logger)
    {
        _commonSettings = commonSettings;
		_logger.LogDebug("ClearLogTask(commonSettings={CommonSettings},logger={Logger}): _commonSettings → {_commonSettings}", commonSettings, logger, _commonSettings);
        _logger = logger;
		_logger.LogError(">>> Entering ClearLogTask(commonSettings={CommonSettings},logger={Logger})", commonSettings, logger);
    _logger.LogWarning("<<< Exiting ClearLogTask(commonSettings={CommonSettings},logger={Logger})", commonSettings, logger);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async System.Threading.Tasks.Task ExecuteAsync()
    {
		_logger.LogError(">>> Entering ExecuteAsync()");
        var utcNow = DateTime.UtcNow;

        await _logger.ClearLogAsync(_commonSettings.ClearLogOlderThanDays == 0 ? null : utcNow.AddDays(-_commonSettings.ClearLogOlderThanDays));
		_logger.LogWarning("<<< Exiting ExecuteAsync()");
    }

    #endregion
}