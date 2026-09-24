using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Polls;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Polls;

/// <summary>
/// Represents a poll answer entity builder
/// </summary>
public partial class PollAnswerBuilder : NopEntityBuilder<PollAnswer>
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
            .WithColumn(nameof(PollAnswer.Name)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(PollAnswer.PollId)).AsInt32().ForeignKey<Poll>();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}