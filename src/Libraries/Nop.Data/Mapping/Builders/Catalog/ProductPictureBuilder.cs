using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Catalog;

/// <summary>
/// Represents a product picture entity builder
/// </summary>
public partial class ProductPictureBuilder : NopEntityBuilder<ProductPicture>
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
            .WithColumn(nameof(ProductPicture.PictureId)).AsInt32().ForeignKey<Picture>()
            .WithColumn(nameof(ProductPicture.ProductId)).AsInt32().ForeignKey<Product>();
			logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}