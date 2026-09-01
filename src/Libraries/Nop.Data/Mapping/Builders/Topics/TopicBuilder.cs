using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Topics;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Topics;

/// <summary>
/// Represents a topic entity builder
/// </summary>
public partial class TopicBuilder : NopEntityBuilder<Topic>
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
        table.WithColumn(nameof(Topic.SystemName)).AsString(400).Nullable();
		_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}
