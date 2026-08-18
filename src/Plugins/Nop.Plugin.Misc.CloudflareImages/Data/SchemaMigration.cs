using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.CloudflareImages.Data;

[NopMigration("2025-04-25 08:00:00", "Misc.CloudflareImages base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        Create.TableFor<Domain.CloudflareImages>();
		_logger.LogDebug("<<< Exiting Up()");
    }

    #endregion
}