using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.RFQ.Domains;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.RFQ.Data.Mapping.Builders;

public class QuoteItemBuilder : NopEntityBuilder<QuoteItem>
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
        table.WithColumn(nameof(QuoteItem.QuoteId)).AsInt32().ForeignKey<Quote>();
        table.WithColumn(nameof(QuoteItem.ProductId)).AsInt32().ForeignKey<Product>(onDelete: Rule.None);
		_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}