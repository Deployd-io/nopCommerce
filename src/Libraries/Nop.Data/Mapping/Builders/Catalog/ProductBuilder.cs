using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Catalog;

/// <summary>
/// Represents a product entity builder
/// </summary>
public partial class ProductBuilder : NopEntityBuilder<Product>
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
            .WithColumn(nameof(Product.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(Product.MetaKeywords)).AsString(400).Nullable()
            .WithColumn(nameof(Product.MetaTitle)).AsString(400).Nullable()
            .WithColumn(nameof(Product.Sku)).AsString(400).Nullable()
            .WithColumn(nameof(Product.ManufacturerPartNumber)).AsString(400).Nullable()
            .WithColumn(nameof(Product.Gtin)).AsString(400).Nullable()
            .WithColumn(nameof(Product.RequiredProductIds)).AsString(1000).Nullable()
            .WithColumn(nameof(Product.AllowedQuantities)).AsString(1000).Nullable();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}