using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Domain;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip.Data;

[NopMigration("2020/02/03 09:27:23:6455432", "Tax.FixedOrByCountryStateZip base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public override void Up()
    {
		_logger.LogError(">>> Entering Up()");
        Create.TableFor<TaxRate>();
		_logger.LogWarning("<<< Exiting Up()");
    }
}