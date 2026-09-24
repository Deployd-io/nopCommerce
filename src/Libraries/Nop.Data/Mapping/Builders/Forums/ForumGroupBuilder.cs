using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Forums;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Forums;

/// <summary>
/// Represents a forum group entity builder
/// </summary>
public partial class ForumGroupBuilder : NopEntityBuilder<ForumGroup>
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
        table.WithColumn(nameof(ForumGroup.Name)).AsString(200).NotNullable();
		_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}