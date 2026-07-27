using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Builders.Catalog;

/// <summary>
/// Represents a review type entity builder
/// </summary>
public partial class ReviewTypeBuilder : NopEntityBuilder<ReviewType>
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
            .WithColumn(nameof(ReviewType.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(ReviewType.Description)).AsString(400).NotNullable();
			logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}