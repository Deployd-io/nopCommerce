using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Forums;

/// <summary>
/// Represents a forum post vote entity builder
/// </summary>
public partial class ForumPostVoteBuilder : NopEntityBuilder<ForumPostVote>
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
        table.WithColumn(nameof(ForumPostVote.ForumPostId)).AsInt32().ForeignKey<ForumPost>();
		_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}