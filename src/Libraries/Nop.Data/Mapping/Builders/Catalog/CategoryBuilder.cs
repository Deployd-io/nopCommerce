using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Catalog;

/// <summary>
/// Represents a category entity builder
/// </summary>
public partial class CategoryBuilder : NopEntityBuilder<Category>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
		_logger.LogDebug(">>> Entering MapEntity(table={Table})", table);
        table
            .WithColumn(nameof(Category.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(Category.MetaKeywords)).AsString(400).Nullable()
            .WithColumn(nameof(Category.MetaTitle)).AsString(400).Nullable()
            .WithColumn(nameof(Category.PageSizeOptions)).AsString(200).Nullable();
			_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}