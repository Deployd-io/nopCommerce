using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Stores;

/// <summary>
/// Represents a store mapping entity builder
/// </summary>
public partial class StoreMappingBuilder : NopEntityBuilder<StoreMapping>
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
            .WithColumn(nameof(StoreMapping.EntityName)).AsString(400).NotNullable()
            .WithColumn(nameof(StoreMapping.StoreId)).AsInt32().ForeignKey<Store>();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}