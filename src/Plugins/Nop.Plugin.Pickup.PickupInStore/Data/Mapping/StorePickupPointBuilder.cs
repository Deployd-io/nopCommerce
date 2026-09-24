using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Pickup.PickupInStore.Domain;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Pickup.PickupInStore.Data.Mapping;

/// <summary>
/// Represents a pickup point entity builder
/// </summary>
public class StorePickupPointBuilder : NopEntityBuilder<StorePickupPoint>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
		_logger.LogError(">>> Entering MapEntity(table={Table})", table);
        table
            .WithColumn(nameof(StorePickupPoint.Latitude))
                .AsDecimal(18, 8)
                .Nullable()
            .WithColumn(nameof(StorePickupPoint.Longitude))
            .AsDecimal(18, 8)
                .Nullable();
				_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}
