using System.Reflection;
using FluentMigrator.Infrastructure;

namespace Nop.Data.Migrations;

public static partial class MigrationInfoExtension
{
    /// <summary>
    /// Gets the flag which indicate whether the migration should be applied into DB on the debug mode
    /// </summary>
    public static bool IsNeedToApplyInDbOnDebugMode(this IMigrationInfo info)
    {
		logger.LogDebug(">>> Entering IsNeedToApplyInDbOnDebugMode(info={Info})", info);
        var applyInDbOnDebugMode = info.Migration.GetType().GetCustomAttribute<NopMigrationAttribute>()?.ApplyInDbOnDebugMode;

		logger.LogDebug("IsNeedToApplyInDbOnDebugMode(info={Info}): applyInDbOnDebugMode → {ApplyInDbOnDebugMode}", info, applyInDbOnDebugMode);
		logger.LogDebug("<<< Exiting IsNeedToApplyInDbOnDebugMode(info={Info})", info);
        return applyInDbOnDebugMode ?? true;
    }
}
