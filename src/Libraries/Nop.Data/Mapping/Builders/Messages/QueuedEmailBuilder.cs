using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Messages;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Messages;

/// <summary>
/// Represents a queued email entity builder
/// </summary>
public partial class QueuedEmailBuilder : NopEntityBuilder<QueuedEmail>
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
            .WithColumn(nameof(QueuedEmail.From)).AsString(500).NotNullable()
            .WithColumn(nameof(QueuedEmail.FromName)).AsString(500).Nullable()
            .WithColumn(nameof(QueuedEmail.To)).AsString(500).NotNullable()
            .WithColumn(nameof(QueuedEmail.ToName)).AsString(500).Nullable()
            .WithColumn(nameof(QueuedEmail.ReplyTo)).AsString(500).Nullable()
            .WithColumn(nameof(QueuedEmail.ReplyToName)).AsString(500).Nullable()
            .WithColumn(nameof(QueuedEmail.CC)).AsString(500).Nullable()
            .WithColumn(nameof(QueuedEmail.Bcc)).AsString(500).Nullable()
            .WithColumn(nameof(QueuedEmail.Subject)).AsString(1000).Nullable()
            .WithColumn(nameof(QueuedEmail.EmailAccountId)).AsInt32().ForeignKey<EmailAccount>();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}