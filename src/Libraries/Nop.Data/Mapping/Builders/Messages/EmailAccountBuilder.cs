using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Messages;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Messages;

/// <summary>
/// Represents an email account entity builder
/// </summary>
public partial class EmailAccountBuilder : NopEntityBuilder<EmailAccount>
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
            .WithColumn(nameof(EmailAccount.DisplayName)).AsString(255).Nullable()
            .WithColumn(nameof(EmailAccount.Email)).AsString(255).NotNullable()
            .WithColumn(nameof(EmailAccount.Host)).AsString(255).NotNullable()
            .WithColumn(nameof(EmailAccount.Username)).AsString(255).NotNullable()
            .WithColumn(nameof(EmailAccount.Password)).AsString(255).NotNullable();
			_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}