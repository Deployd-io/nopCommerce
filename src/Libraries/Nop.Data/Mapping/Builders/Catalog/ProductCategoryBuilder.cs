using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Catalog;

/// <summary>
/// Represents a product category entity builder
/// </summary>
public partial class ProductCategoryBuilder : NopEntityBuilder<ProductCategory>
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
            .WithColumn(nameof(ProductCategory.CategoryId)).AsInt32().ForeignKey<Category>()
            .WithColumn(nameof(ProductCategory.ProductId)).AsInt32().ForeignKey<Product>();
			logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}