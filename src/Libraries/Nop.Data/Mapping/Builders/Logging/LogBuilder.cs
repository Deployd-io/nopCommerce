using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Logging;

/// <summary>
/// Represents a log entity builder
/// </summary>
public partial class LogBuilder : NopEntityBuilder<Log>
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
            .WithColumn(nameof(Log.ShortMessage)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(Log.IpAddress)).AsString(100).Nullable()
            .WithColumn(nameof(Log.CustomerId)).AsInt32().Nullable().ForeignKey<Customer>();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}