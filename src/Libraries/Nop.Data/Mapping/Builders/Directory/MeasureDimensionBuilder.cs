using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Directory;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Directory;

/// <summary>
/// Represents a measure dimension entity builder
/// </summary>
public partial class MeasureDimensionBuilder : NopEntityBuilder<MeasureDimension>
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
            .WithColumn(nameof(MeasureDimension.Name)).AsString(100).NotNullable()
            .WithColumn(nameof(MeasureDimension.SystemKeyword)).AsString(100).NotNullable()
            .WithColumn(nameof(MeasureDimension.Ratio)).AsDecimal(18, 8);
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}