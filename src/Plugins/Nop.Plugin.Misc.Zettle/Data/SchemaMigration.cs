using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.Zettle.Domain;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Zettle.Data;

[NopMigration("2022/09/15 12:00:00", "Misc.Zettle base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogError(">>> Entering Up()");
        Create.TableFor<ZettleRecord>();
		_logger.LogWarning("<<< Exiting Up()");
    }

    #endregion
}