using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Catalog;

/// <summary>
/// Represents a product attribute combination picture entity builder
/// </summary>
public partial class ProductAttributeCombinationPictureBuilder : NopEntityBuilder<ProductAttributeCombinationPicture>
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
            .WithColumn(nameof(ProductAttributeCombinationPicture.ProductAttributeCombinationId)).AsInt32().ForeignKey<ProductAttributeCombination>()
            .WithColumn(nameof(ProductAttributeCombinationPicture.PictureId)).AsInt32();
			logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}
