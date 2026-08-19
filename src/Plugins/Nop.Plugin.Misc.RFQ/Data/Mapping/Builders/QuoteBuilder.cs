using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.RFQ.Domains;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.RFQ.Data.Mapping.Builders;

public class QuoteBuilder : NopEntityBuilder<Quote>
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
        table.WithColumn(nameof(Quote.CustomerId)).AsInt32().ForeignKey<Customer>();
		_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}