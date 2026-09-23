using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Topics;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Topics;

/// <summary>
/// Represents a topic template entity builder
/// </summary>
public partial class TopicTemplateBuilder : NopEntityBuilder<TopicTemplate>
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
            .WithColumn(nameof(TopicTemplate.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(TopicTemplate.ViewPath)).AsString(400).NotNullable();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}