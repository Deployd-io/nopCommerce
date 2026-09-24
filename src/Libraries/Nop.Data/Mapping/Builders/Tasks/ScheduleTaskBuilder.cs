using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.ScheduleTasks;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Tasks;

/// <summary>
/// Represents a schedule task entity builder
/// </summary>
public partial class ScheduleTaskBuilder : NopEntityBuilder<ScheduleTask>
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
            .WithColumn(nameof(ScheduleTask.Name)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(ScheduleTask.Type)).AsString(int.MaxValue).NotNullable();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}