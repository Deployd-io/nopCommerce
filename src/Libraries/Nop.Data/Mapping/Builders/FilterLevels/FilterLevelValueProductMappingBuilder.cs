using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.FilterLevels;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.FilterLevels;

/// <summary>
/// Represents a filter level value and product mapping entity builder
/// </summary>
public partial class FilterLevelValueProductMappingBuilder : NopEntityBuilder<FilterLevelValueProductMapping>
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
            .WithColumn(nameof(FilterLevelValueProductMapping.FilterLevelValueId)).AsInt32().ForeignKey<FilterLevelValue>()
            .WithColumn(nameof(FilterLevelValueProductMapping.ProductId)).AsInt32().ForeignKey<Product>();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}
