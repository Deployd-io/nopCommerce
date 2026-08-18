using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Catalog;

/// <summary>
/// Represents a manufacturer entity builder
/// </summary>
public partial class ManufacturerBuilder : NopEntityBuilder<Manufacturer>
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
            .WithColumn(nameof(Manufacturer.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(Manufacturer.MetaKeywords)).AsString(400).Nullable()
            .WithColumn(nameof(Manufacturer.MetaTitle)).AsString(400).Nullable()
            .WithColumn(nameof(Manufacturer.PageSizeOptions)).AsString(200).Nullable();
			_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}