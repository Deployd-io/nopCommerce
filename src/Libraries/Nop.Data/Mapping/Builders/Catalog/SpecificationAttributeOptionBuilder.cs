using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Catalog;

/// <summary>
/// Represents a specification attribute option entity builder
/// </summary>
public partial class SpecificationAttributeOptionBuilder : NopEntityBuilder<SpecificationAttributeOption>
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
            .WithColumn(nameof(SpecificationAttributeOption.Name)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(SpecificationAttributeOption.ColorSquaresRgb)).AsString(100).Nullable()
            .WithColumn(nameof(SpecificationAttributeOption.SpecificationAttributeId)).AsInt32().ForeignKey<SpecificationAttribute>();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}