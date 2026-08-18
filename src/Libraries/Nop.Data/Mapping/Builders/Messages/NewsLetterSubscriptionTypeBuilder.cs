using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Messages;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Messages;

/// <summary>
/// Represents NewsLetterSubscriptionType entity builder
/// </summary>
public partial class NewsLetterSubscriptionTypeBuilder : NopEntityBuilder<NewsLetterSubscriptionType>
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
            .WithColumn(nameof(NewsLetterSubscriptionType.Name)).AsString(400).NotNullable();
			_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}
