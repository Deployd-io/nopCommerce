using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Pickup.PickupInStore.Domain;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Pickup.PickupInStore.Data.Migrations;

[NopMigration("2024-05-29 00:00:00", "Pickup.PickupInStore 4.70.7. Increase precision of Longitude/Latitude fields", MigrationProcessType.Update)]
public class LonLatUpdateMigration : MigrationBase
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        Alter.Table(nameof(StorePickupPoint))
            .AlterColumn(nameof(StorePickupPoint.Latitude))
                .AsDecimal(18, 8)
                .Nullable();

        Alter.Table(nameof(StorePickupPoint))
            .AlterColumn(nameof(StorePickupPoint.Longitude))
                .AsDecimal(18, 8)
                .Nullable();
				_logger.LogDebug("<<< Exiting Up()");
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
		_logger.LogDebug(">>> Entering Down()");
        //nothing
		_logger.LogDebug("<<< Exiting Down()");
    }

    #endregion
}
