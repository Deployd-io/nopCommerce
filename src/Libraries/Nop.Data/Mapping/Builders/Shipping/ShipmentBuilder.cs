using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Shipping;

/// <summary>
/// Represents a shipment entity builder
/// </summary>
public partial class ShipmentBuilder : NopEntityBuilder<Shipment>
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
        table.WithColumn(nameof(Shipment.OrderId)).AsInt32().ForeignKey<Order>();
		_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}