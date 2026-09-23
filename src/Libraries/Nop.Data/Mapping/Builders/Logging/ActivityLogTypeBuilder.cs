using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Logging;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Logging;

/// <summary>
/// Represents an activity log type entity builder
/// </summary>
public partial class ActivityLogTypeBuilder : NopEntityBuilder<ActivityLogType>
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
            .WithColumn(nameof(ActivityLogType.SystemKeyword)).AsString(100).NotNullable()
            .WithColumn(nameof(ActivityLogType.Name)).AsString(200).NotNullable();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}