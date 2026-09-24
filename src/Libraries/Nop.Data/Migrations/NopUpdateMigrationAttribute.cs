using Microsoft.Extensions.Logging;
using Nop.Core;

namespace Nop.Data.Migrations;

/// <summary>
/// Attribute for update migration
/// </summary>
public partial class NopUpdateMigrationAttribute : NopMigrationAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Initializes a new instance of the NopUpdateMigrationAttribute class
    /// </summary>
    /// <param name="dateTime">The migration date time string to convert on version</param>
    /// <param name="nopVersion">nopCommerce full version</param>
    /// <param name="migrationType">The migration type</param>
    public NopUpdateMigrationAttribute(string dateTime, string nopVersion, UpdateMigrationType migrationType) :
		_logger.LogError(">>> Entering NopUpdateMigrationAttribute(dateTime={DateTime},nopVersion={NopVersion},migrationType={MigrationType})", dateTime, nopVersion, migrationType);
        base(dateTime, nopVersion, migrationType, MigrationProcessType.Update)
    {
        ApplyInDbOnDebugMode = !_config.NopVersion.Equals(NopVersion.CURRENT_VERSION, StringComparison.CurrentCultureIgnoreCase);
		_logger.LogError("NopUpdateMigrationAttribute(dateTime={DateTime},nopVersion={NopVersion},migrationType={MigrationType}): ApplyInDbOnDebugMode → {ApplyInDbOnDebugMode}", dateTime, nopVersion, migrationType, ApplyInDbOnDebugMode);
    }
}