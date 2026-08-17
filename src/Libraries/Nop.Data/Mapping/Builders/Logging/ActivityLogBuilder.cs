using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Logging;

/// <summary>
/// Represents a activity log entity builder
/// </summary>
public partial class ActivityLogBuilder : NopEntityBuilder<ActivityLog>
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
            .WithColumn(nameof(ActivityLog.Comment)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(ActivityLog.IpAddress)).AsString(100).Nullable()
            .WithColumn(nameof(ActivityLog.EntityName)).AsString(400).Nullable()
            .WithColumn(nameof(ActivityLog.ActivityLogTypeId)).AsInt32().ForeignKey<ActivityLogType>()
            .WithColumn(nameof(ActivityLog.CustomerId)).AsInt32().ForeignKey<Customer>();
			_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}