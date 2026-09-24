using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Stores;

/// <summary>
/// Represents a store entity builder
/// </summary>
public partial class StoreBuilder : NopEntityBuilder<Store>
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
            .WithColumn(nameof(Store.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(Store.Url)).AsString(400).NotNullable()
            .WithColumn(nameof(Store.Hosts)).AsString(1000).Nullable()
            .WithColumn(nameof(Store.CompanyName)).AsString(1000).Nullable()
            .WithColumn(nameof(Store.CompanyAddress)).AsString(1000).Nullable()
            .WithColumn(nameof(Store.CompanyPhoneNumber)).AsString(1000).Nullable()
            .WithColumn(nameof(Store.CompanyVat)).AsString(1000).Nullable();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}