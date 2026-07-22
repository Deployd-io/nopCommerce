using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.FilterLevels;

namespace Nop.Data.Mapping.Builders.FilterLevels;

/// <summary>
/// Represents a filter level value entity builder
/// </summary>
public partial class FilterLevelValueBuilder : NopEntityBuilder<FilterLevelValue>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
		logger.LogDebug(">>> Entering MapEntity(table={Table})", table);
        table
            .WithColumn(nameof(FilterLevelValue.FilterLevel1Value)).AsString(100).Nullable()
            .WithColumn(nameof(FilterLevelValue.FilterLevel2Value)).AsString(100).Nullable()
            .WithColumn(nameof(FilterLevelValue.FilterLevel3Value)).AsString(100).Nullable();
			logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}
