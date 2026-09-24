using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Shipping;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Shipping;

/// <summary>
/// Represents a product availability range entity builder
/// </summary>
public partial class ProductAvailabilityRangeBuilder : NopEntityBuilder<ProductAvailabilityRange>
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
        table.WithColumn(nameof(ProductAvailabilityRange.Name)).AsString(400).NotNullable();
		_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}