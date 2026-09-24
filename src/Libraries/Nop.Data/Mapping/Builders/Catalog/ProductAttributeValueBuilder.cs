using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Catalog;

/// <summary>
/// Represents a product attribute value entity builder
/// </summary>
public partial class ProductAttributeValueBuilder : NopEntityBuilder<ProductAttributeValue>
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
            .WithColumn(nameof(ProductAttributeValue.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(ProductAttributeValue.ColorSquaresRgb)).AsString(100).Nullable()
            .WithColumn(nameof(ProductAttributeValue.ProductAttributeMappingId)).AsInt32().ForeignKey<ProductAttributeMapping>();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}