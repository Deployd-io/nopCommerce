using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Directory;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Directory;

/// <summary>
/// Represents a country entity builder
/// </summary>
public partial class CountryBuilder : NopEntityBuilder<Country>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table) => table
		_logger.LogDebug(">>> Entering MapEntity(table={Table})", table);
        .WithColumn(nameof(Country.Name)).AsString(100).NotNullable()
        .WithColumn(nameof(Country.TwoLetterIsoCode)).AsString(2).Nullable()
        .WithColumn(nameof(Country.ThreeLetterIsoCode)).AsString(3).Nullable();

    #endregion
	_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
}