using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Catalog;

/// <summary>
/// Represents a product manufacturer entity builder
/// </summary>
public partial class ProductManufacturerBuilder : NopEntityBuilder<ProductManufacturer>
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
            .WithColumn(nameof(ProductManufacturer.ManufacturerId)).AsInt32().ForeignKey<Manufacturer>()
            .WithColumn(nameof(ProductManufacturer.ProductId)).AsInt32().ForeignKey<Product>();
			_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}