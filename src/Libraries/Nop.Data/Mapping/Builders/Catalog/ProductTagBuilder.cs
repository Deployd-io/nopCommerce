using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Catalog;

/// <summary>
/// Represents a product tag entity builder
/// </summary>
public partial class ProductTagBuilder : NopEntityBuilder<ProductTag>
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
            .WithColumn(nameof(ProductTag.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(ProductTag.MetaKeywords)).AsString(400).Nullable()
            .WithColumn(nameof(ProductTag.MetaTitle)).AsString(400).Nullable();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}