using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Pickup.PickupInStore.Domain;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Pickup.PickupInStore.Data.Migrations;

[NopMigration("2020/02/03 09:30:17:6455422", "Pickup.PickupInStore base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public override void Up()
    {
		_logger.LogError(">>> Entering Up()");
        Create.TableFor<StorePickupPoint>();
		_logger.LogWarning("<<< Exiting Up()");
    }
}