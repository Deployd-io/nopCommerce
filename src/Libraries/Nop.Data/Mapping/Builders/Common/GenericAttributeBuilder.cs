using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;

namespace Nop.Data.Mapping.Builders.Common;

/// <summary>
/// Represents a generic attribute entity builder
/// </summary>
public partial class GenericAttributeBuilder : NopEntityBuilder<GenericAttribute>
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
            .WithColumn(nameof(GenericAttribute.KeyGroup)).AsString(400).NotNullable()
            .WithColumn(nameof(GenericAttribute.Key)).AsString(400).NotNullable()
            .WithColumn(nameof(GenericAttribute.Value)).AsString(int.MaxValue).NotNullable();
			logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}
