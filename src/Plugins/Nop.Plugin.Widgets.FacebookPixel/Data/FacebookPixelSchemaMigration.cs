using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.FacebookPixel.Domain;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Widgets.FacebookPixel.Data;

[NopMigration("2020/03/25 12:00:00", "Widgets.FacebookPixel base schema", MigrationProcessType.Installation)]
public class FacebookPixelSchemaMigration : AutoReversingMigration
{
private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogError(">>> Entering Up()");
        Create.TableFor<FacebookPixelConfiguration>();
		_logger.LogWarning("<<< Exiting Up()");
    }

    #endregion
}