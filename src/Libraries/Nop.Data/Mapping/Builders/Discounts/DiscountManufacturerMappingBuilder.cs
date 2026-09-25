using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Discounts;

/// <summary>
/// Represents a discount manufacturer mapping entity builder
/// </summary>
public partial class DiscountManufacturerMappingBuilder : NopEntityBuilder<DiscountManufacturerMapping>
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
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(DiscountManufacturerMapping), nameof(DiscountManufacturerMapping.DiscountId)))
            .AsInt32().PrimaryKey().ForeignKey<Discount>()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(DiscountManufacturerMapping), nameof(DiscountManufacturerMapping.EntityId)))
            .AsInt32().PrimaryKey().ForeignKey<Manufacturer>();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}