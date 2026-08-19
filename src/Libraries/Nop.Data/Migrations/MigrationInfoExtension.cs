using System.Reflection;
using FluentMigrator.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Migrations;

public static partial class MigrationInfoExtension
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Gets the flag which indicate whether the migration should be applied into DB on the debug mode
    /// </summary>
    public static bool IsNeedToApplyInDbOnDebugMode(this IMigrationInfo info)
    {
		_logger.LogDebug(">>> Entering IsNeedToApplyInDbOnDebugMode(info={Info})", info);
        var applyInDbOnDebugMode = info.Migration.GetType().GetCustomAttribute<NopMigrationAttribute>()?.ApplyInDbOnDebugMode;

		_logger.LogDebug("IsNeedToApplyInDbOnDebugMode(info={Info}): applyInDbOnDebugMode → {ApplyInDbOnDebugMode}", info, applyInDbOnDebugMode);
		_logger.LogDebug("<<< Exiting IsNeedToApplyInDbOnDebugMode(info={Info})", info);
        return applyInDbOnDebugMode ?? true;
    }
}
