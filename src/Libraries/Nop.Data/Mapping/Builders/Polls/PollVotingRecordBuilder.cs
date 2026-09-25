using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Polls;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Polls;

/// <summary>
/// Represents a poll voting record entity builder
/// </summary>
public partial class PollVotingRecordBuilder : NopEntityBuilder<PollVotingRecord>
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
            .WithColumn(nameof(PollVotingRecord.PollAnswerId)).AsInt32().ForeignKey<PollAnswer>()
            .WithColumn(nameof(PollVotingRecord.CustomerId)).AsInt32().ForeignKey<Customer>();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}